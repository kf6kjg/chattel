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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public class ChattelWriter {
		//private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelWriter>();

		private readonly ChattelConfiguration _config;
		private readonly IChattelCache _cache;

		private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, ReaderWriterLockSlim> _activeWriteLocks = new System.Collections.Concurrent.ConcurrentDictionary<Guid, ReaderWriterLockSlim>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="cache">Instance of the IChattelCache interface. If left null, then the default ChattelCache will be instantiated.</param>
		/// <param name="purgeCache">Whether or not to attempt to purge the cache.</param>
		public ChattelWriter(ChattelConfiguration config, IChattelCache cache = null, bool purgeCache = false) {
			_config = config ?? throw new ArgumentNullException(nameof(config));

			if (_config.CacheEnabled) {
				_cache = cache ?? new ChattelCache(config);
			}

			if (purgeCache) {
				_cache?.Purge();
			}
		}

		public bool HasUpstream => _config?.SerialParallelAssetServers.Any() ?? false;

		/// <summary>
		/// Alias for PutAssetSync
		/// </summary>
		/// <param name="asset">The asset to store.</param>
		[Obsolete("Please convert to PutAssetSync")]
		public void WriteAssetSync(StratusAsset asset) {
			PutAssetSync(asset);
		}

		/// <summary>
		/// Sends the asset to the asset servers.
		/// Throws AssetExistsException or AggregateException.
		/// </summary>
		/// <param name="asset">The asset to store.</param>
		public void PutAssetSync(StratusAsset asset) {
			if (asset == null || asset.Id == Guid.Empty) {
				return;
			}

			// Handle parallel calls with the same asset ID.
			var firstLock = new ReaderWriterLockSlim();
			firstLock.EnterWriteLock();
			var activeLock = _activeWriteLocks.GetOrAdd(asset.Id, firstLock);
			if (firstLock != activeLock) {
				// There's another thread currently adding this exact ID, so we need to wait on it so that we return when it's actually ready for a GET.
				activeLock.EnterReadLock();
				activeLock.ExitReadLock();
				return;
			}

			// Hit up the cache first.
			try {
				if (_cache?.TryGetCachedAsset(asset.Id, out StratusAsset result) ?? false) {
					_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
					firstLock.ExitWriteLock();
					throw new AssetExistsException(asset.Id);
				}
			}
			catch (Exception) {
				_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
				firstLock.ExitWriteLock();
				throw;
			}

			var exceptions = new List<Exception>();
			var success = false;

			try {
				_cache?.CacheAsset(asset);

				// Set success if there're no upstream servers.
				success |= !HasUpstream;
			}
			catch (Exception e) {
				exceptions.Add(e);
			}

			// Got to go try the servers now.
			foreach (var parallelServers in _config.SerialParallelAssetServers) {
				try {
					if (parallelServers.Count() == 1) {
						parallelServers.First().StoreAssetSync(asset);
					}
					else {
						parallelServers.AsParallel().ForAll(server => server.StoreAssetSync(asset));
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

			{
				_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
				firstLock.ExitWriteLock();
			}

			if (!success) {
				throw new AggregateException("Unable to store asset in any asset server. See inner exceptions for details.", exceptions);
			}
		}
	}
}
