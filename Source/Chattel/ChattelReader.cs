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
	public class ChattelReader {
		private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public delegate void AssetHandler(StratusAsset asset);

		private readonly ChattelConfiguration _config;
		private readonly ChattelCache _cache;

		private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, Queue<AssetHandler>> _idsBeingFetched = new System.Collections.Concurrent.ConcurrentDictionary<Guid, Queue<AssetHandler>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="purgeCache">Whether or not to attempt to purge the cache.</param>
		public ChattelReader(ChattelConfiguration config, bool purgeCache = false) {
			_config = config ?? throw new ArgumentNullException(nameof(config));

			if (_config.CacheEnabled) {
				_cache = new ChattelCache(config);

				if (purgeCache) {
					_cache.Purge();
				}
			}
		}

		public bool HasUpstream => _config?.SerialParallelAssetServers.Any() ?? false;

		/// <summary>
		/// Alias for GetAssetSync
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		[Obsolete("Please convert to GetAssetAsync")]
		public StratusAsset ReadAssetSync(Guid assetId) {
			return GetAssetSync(assetId);
		}

		/// <summary>
		/// Gets the asset from the server.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		[Obsolete("Please convert to GetAssetAsync")]
		public StratusAsset GetAssetSync(Guid assetId) {
			StratusAsset result = null;

			// Ask for null, get null.
			if (assetId == Guid.Empty) {
				return null;
			}

			// Hit up the cache first.
			if (_cache?.TryGetCachedAsset(assetId, out result) ?? false) {
				return result;
			}

			// Got to go try the servers now.
			foreach (var parallelServers in _config.SerialParallelAssetServers) {
				if (parallelServers.Count() == 1) {
					result = parallelServers.First().RequestAssetSync(assetId);
				}
				else {
					result = parallelServers.AsParallel().Select(server => server.RequestAssetSync(assetId)).FirstOrDefault(asset => asset != null);
				}

				if (result != null) {
					_cache?.CacheAsset(result);
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the asset from the server.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		/// <param name="handler">Callback delegate to hand the asset to.</param>
		public void GetAssetAsync(Guid assetId, AssetHandler handler) {
			// Ask for null, get null.
			if (assetId == Guid.Empty) {
				handler(null);
			}

			// TODO: see if https://github.com/Reactive-Extensions/Rx.NET would do a better job, but they have to finish releasing 4.0 first.
			// BUG: if first caller throws in the below code, all other listeners will never be called. Also it might be beneficial to move the listener processsing to another thread, but then you potentially lose parallism across multiple asset IDs.

			StratusAsset result = null;

			while (true) {
				// Hit up the cache first.
				if (_cache?.TryGetCachedAsset(assetId, out result) ?? false) {
					handler(result);
				}

				var listeners = new Queue<AssetHandler>();
				if (_idsBeingFetched.TryAdd(assetId, listeners)) {
					listeners.Enqueue(handler);
					StratusAsset asset = null;

					// Got to go try the servers now.
					foreach (var parallelServers in _config.SerialParallelAssetServers) {
						if (parallelServers.Count() == 1) { // Optimization: no need to hit up the parallel stuff if there's only 1.
							result = parallelServers.First().RequestAssetSync(assetId);
						}
						else {
							result = parallelServers.AsParallel().Select(server => server.RequestAssetSync(assetId)).FirstOrDefault(a => a != null);
						}

						if (result != null) {
							_cache?.CacheAsset(result);
							break;
						}
					}

					// Now to process the listeners.
					var exceptions = new ConcurrentQueue<Exception>();

					lock (listeners) { // Prevent new listeners from being added.
						Parallel.ForEach(listeners, waiting_handler => {
							try {
								waiting_handler(asset);
							}
							catch (Exception e) {
								exceptions.Enqueue(e);
							}
						});

						_idsBeingFetched.TryRemove(assetId, out Queue<AssetHandler> trash);
					}

					if (exceptions.Count > 0) {
						LOG.Error($"Exceptions ({exceptions.Count}) were thrown by handler(s) listening for asset {assetId}", new AggregateException(exceptions));
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

				// It's gone already, so let's try again as the asset should be in the cache or we should query the servers again.
				Thread.Sleep(50);
			}
		}
	}
}
