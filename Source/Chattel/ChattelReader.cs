// ChattelReader.cs
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
using System.Threading.Tasks;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public sealed class ChattelReader {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelReader>();

		public delegate void AssetHandler(StratusAsset asset);

		private readonly ChattelConfiguration _config;
		private readonly IChattelLocalStorage _localStorage;

		private readonly ConcurrentDictionary<Guid, Queue<AssetHandler>> _idsBeingFetched = new ConcurrentDictionary<Guid, Queue<AssetHandler>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// If local storage is enabled, but no local storage instance was passed in, automatically sets up and uses the AssetStorageSimpleFolderTree for local storage.
		/// If purgeLocalStorage is set, purges all assets in the storage.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="localStorage">Instance of the IChattelLocalStorage interface. If left null, then the default AssetStorageSimpleFolderTree will be instantiated.</param>
		/// <param name="purgeLocalStorage">Whether or not to attempt to purge local storage.</param>
		public ChattelReader(ChattelConfiguration config, IChattelLocalStorage localStorage, bool purgeLocalStorage) {
			_config = config ?? throw new ArgumentNullException(nameof(config));

			if (_config.LocalStorageEnabled) {
				_localStorage = localStorage ?? new AssetStorageSimpleFolderTree(config);
			}

			if (purgeLocalStorage) {
				_localStorage?.PurgeAll(null);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// If local storage is enabled, but no local storage instance was passed in, automatically sets up and uses the AssetStorageSimpleFolderTree for local storage.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="localStorage">Instance of the IChattelLocalStorage interface. If left null, then the default AssetStorageSimpleFolderTree will be instantiated.</param>
		public ChattelReader(ChattelConfiguration config, IChattelLocalStorage localStorage)
			: this(config, localStorage, false) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// Automatically sets up and uses the AssetStorageSimpleFolderTree.
		/// If purgeLocalStorage is set, purges all assets in the storage for local storage.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="purgeLocalStorage">Whether or not to attempt to purge local storage.</param>
		public ChattelReader(ChattelConfiguration config, bool purgeLocalStorage)
			: this(config, config?.LocalStorageEnabled ?? false ? new AssetStorageSimpleFolderTree(config) : null, purgeLocalStorage)  {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// Automatically sets up and uses the AssetStorageSimpleFolderTree.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		public ChattelReader(ChattelConfiguration config)
			: this(config, new AssetStorageSimpleFolderTree(config), false) {
		}

		/// <summary>
		/// Quick check for whether or not any upstream servers were configured.
		/// </summary>
		public bool HasUpstream => _config.SerialParallelAssetServers.Any();

		/// <summary>
		/// Ruleset for local storage operations when used as a cache for remote servers.
		/// </summary>
		[Flags]
		public enum CacheRule : uint {
			Normal = 0,
			SkipRead = 1,
			SkipWrite = 2,
		}

		/// <summary>
		/// Gets the asset from the server.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		/// <param name="handler">Callback delegate to hand the asset to.</param>
		/// <param name="cacheRule">Bitfield controlling how local storage is to be handled when used as a cache for remote servers.</param>
		public void GetAssetAsync(Guid assetId, AssetHandler handler, CacheRule cacheRule) {
			// Ask for null, get null.
			if (assetId == Guid.Empty) {
				handler(null);
			}

			// TODO: see if https://github.com/Reactive-Extensions/Rx.NET would do a better job, but they have to finish releasing 4.0 first.

			// It might be beneficial to move the listener processsing to another thread, but then you potentially lose parallism across multiple asset IDs.

			StratusAsset result = null;

			while (true) {
				// Hit up the local storage first. If there's no upstream then ignore skipread.
				if (!(cacheRule.HasFlag(CacheRule.SkipRead) && _config.SerialParallelAssetServers.Any()) && (_localStorage?.TryGetAsset(assetId, out result) ?? false)) {
					handler(result);
					return;
				}

				var listeners = new Queue<AssetHandler>();
				listeners.Enqueue(handler); // Add myself to the new listeners list first thing, assuming, probably wrongly, that the following test is true.  If wrong, meh: this queue gets dropped like an old potato.
				if (_idsBeingFetched.TryAdd(assetId, listeners)) {
					// Got to go try the servers now.
					foreach (var parallelServers in _config.SerialParallelAssetServers) {
						if (parallelServers.Count() == 1) { // Optimization: no need to hit up the parallel stuff if there's only 1.
							result = parallelServers.First().RequestAssetSync(assetId);
						}
						else {
							result = parallelServers.AsParallel().Select(server => server.RequestAssetSync(assetId)).FirstOrDefault(a => a != null);
						}

						if (result != null) {
							if (!cacheRule.HasFlag(CacheRule.SkipWrite)) {
								_localStorage?.StoreAsset(result);
							}
							break;
						}
					}

					// Now to process the listeners.
					var exceptions = new ConcurrentQueue<Exception>();

					lock (listeners) { // Prevent new listeners from being added.
						Parallel.ForEach(listeners, waiting_handler => {
							if (waiting_handler == null) {
								LOG.Log(Logging.LogLevel.Warn, () => $"Attempted to process a handler for assetId {assetId} that was null!");
								return;
							}

							try {
								waiting_handler(result);
							}
							catch (Exception e) {
								exceptions.Enqueue(e);
							}
						});

						_idsBeingFetched.TryRemove(assetId, out Queue<AssetHandler> trash);
					}

					if (exceptions.Count > 0) {
						LOG.Log(Logging.LogLevel.Error, () => $"Exceptions ({exceptions.Count}) were thrown by handler(s) listening for asset {assetId}", new AggregateException(exceptions));
					}

					return; // We're done here.
				}

				// See if we can add ourselves to the listener list.
				if (_idsBeingFetched.TryGetValue(assetId, out listeners)) {
					// Skiplock: if the lock cannot be taken, move on to the retry because the list is already being emptied.
					var lockTaken = false;
					try {
						Monitor.TryEnter(listeners, ref lockTaken);
						if (lockTaken) {
							listeners.Enqueue(handler);
							return;
						}
					}
					finally {
						if (lockTaken) {
							Monitor.Exit(listeners);
						}
					}

					// lock was skipped, therefore that list is already being cleaned out.
				}

				// It's gone already, so let's try again as the asset should be in local storage or we should query the servers again.
				Thread.Sleep(50);
			}
		}

		/// <summary>
		/// Gets the asset from the server using CacheRule.Normal
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		/// <param name="handler">Callback delegate to hand the asset to.</param>
		public void GetAssetAsync(Guid assetId, AssetHandler handler) {
			GetAssetAsync(assetId, handler, CacheRule.Normal);
		}
	}
}
