// ChattelWriter.cs
//
// Author:
//       Ricky Curtice <ricky@rwcproductions.com>
//
// Copyright (c) 2016 Richard Curtice
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public class ChattelWriter {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelWriter>();

		private readonly ChattelConfiguration _config;
		private readonly IChattelLocalStorage _localStorage;

		private readonly ConcurrentDictionary<Guid, ReaderWriterLockSlim> _activeWriteLocks = new ConcurrentDictionary<Guid, ReaderWriterLockSlim>();

		private readonly WriteCache _writeCache;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="localStorage">Instance of the IChattelLocalStorage interface. If left null, then the default AssetStorageSimpleFolderTree will be instantiated.</param>
		/// <param name="purgeLocalStorage">Whether or not to attempt to purge local storage.</param>
		/// <exception cref="!:ChattelConfigurationException">Thrown if the are pending assets to be sent upstream and there are no upstream servers configured.</exception>
		public ChattelWriter(ChattelConfiguration config, IChattelLocalStorage localStorage, bool purgeLocalStorage) {
			_config = config ?? throw new ArgumentNullException(nameof(config));

			if (config.LocalStorageEnabled) {
				_localStorage = localStorage ?? new AssetStorageSimpleFolderTree(config);
			}

			if (purgeLocalStorage) {
				_localStorage?.PurgeAll();
			}

			if (config.LocalStorageEnabled && config.WriteCacheFile != null) {
				_writeCache = new WriteCache(config.WriteCacheFile, config.WriteCacheRecordCount, this, localStorage);
				config.WriteCacheFile.Refresh();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="localStorage">Instance of the IChattelLocalStorage interface. If left null, then the default AssetStorageSimpleFolderTree will be instantiated.</param>
		/// <exception cref="!:ChattelConfigurationException">Thrown if the are pending assets to be sent upstream and there are no upstream servers configured.</exception>
		public ChattelWriter(ChattelConfiguration config, IChattelLocalStorage localStorage) : this(config, localStorage, false) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="purgeLocalStorage">Whether or not to attempt to purge local storage.</param>
		/// <exception cref="!:ChattelConfigurationException">Thrown if the are pending assets to be sent upstream and there are no upstream servers configured.</exception>
		public ChattelWriter(ChattelConfiguration config, bool purgeLocalStorage) : this(config, null, purgeLocalStorage) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <exception cref="!:ChattelConfigurationException">Thrown if the are pending assets to be sent upstream and there are no upstream servers configured.</exception>
		public ChattelWriter(ChattelConfiguration config) : this(config, null, false) {
		}

		public bool HasUpstream => _config.SerialParallelAssetServers.Any();

		/// <summary>
		/// Sends the asset to the asset servers.
		/// Throws AssetExistsException or AggregateException.
		/// </summary>
		/// <param name="asset">The asset to store.</param>
		public void PutAssetSync(StratusAsset asset) {
			asset = asset ?? throw new ArgumentNullException(nameof(asset));

			if (asset.Id == Guid.Empty) {
				throw new ArgumentException("Asset cannot have zero ID.", nameof(asset));
			}

			// Handle parallel calls with the same asset ID.
			var firstLock = new ReaderWriterLockSlim();
			try {
				firstLock.EnterWriteLock();
				var activeLock = _activeWriteLocks.GetOrAdd(asset.Id, firstLock);
				if (firstLock != activeLock) {
					LOG.Log(Logging.LogLevel.Warn, () => $"Another thread already storing asset with ID {asset.Id}, halting this call until the first completes, then just returning.");
					// There's another thread currently adding this exact ID, so we need to wait on it so that we return when it's actually ready for a GET.
					activeLock.EnterReadLock();
					activeLock.ExitReadLock();
					return;
				}

				// Hit up local storage first.
				if (_localStorage?.TryGetAsset(asset.Id, out StratusAsset result) ?? false) {
					_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
					firstLock.ExitWriteLock();
					throw new AssetExistsException(asset.Id);
				}

				var exceptions = new List<Exception>();
				var success = false;
				WriteCacheNode activeNode = null;

				// First step: get it in local storage.
				try {
					_localStorage?.StoreAsset(asset);

					if (HasUpstream) {
						// Write to writecache file. In this way if we crash after this point we can recover and send the asset to the servers.
						activeNode = _writeCache.WriteNode(asset);
						// If that fails, it'll throw.
					}
					else {
						// Set success if there're no upstream servers. This supports applications that act as asset servers.
						success = true;
					}
				}
				catch (WriteCacheFullException) {
					// Let this exception out: the user requested a writecache and now it's full, so storing an asset should immediately fail as there's no way to be certain that the asset will be safely retained as requested.
					throw;
				}
				catch (Exception e) {
					exceptions.Add(e);
				}

				// Got to go try the servers now.
				foreach (var parallelServers in _config.SerialParallelAssetServers) {
					// Remember each iteration of this loop is going through serially accessed blocks of parallel-access servers.
					// Therefore any failure or problem in one of the blocks means to just continue to the next block.
					try {
						if (parallelServers.Count() == 1) {
							parallelServers.First().StoreAssetSync(asset);
						}
						else {
							parallelServers.AsParallel().ForAll(server => server.StoreAssetSync(asset));
						}

						if (activeNode != null) {
							_writeCache.ClearNode(activeNode);
						}

						success = true;
						break; // It was successfully stored in the first bank of parallel servers, don't do the next bank.
					}
					catch (AssetException e) {
						exceptions.Add(e);
					}
					catch (AggregateException e) {
						// Unwind the aggregate one layer.
						foreach (var ex in e.InnerExceptions) {
							exceptions.Add(ex);
						}
					}
					catch (Exception e) {
						exceptions.Add(e);
					}
				}

				if (!success) {
					throw new AggregateException("Unable to store asset in any asset server. See inner exceptions for details.", exceptions);
				}
			}
			finally {
				_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
				firstLock.ExitWriteLock();
			}
		}
	}
}
