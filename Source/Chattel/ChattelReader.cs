﻿// Chattel.cs
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
using System.Linq;
using InWorldz.Data.Assets.Stratus;
using OpenMetaverse;

namespace Chattel {
	public class ChattelReader {
		//private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly ChattelConfiguration _config;
		private readonly ChattelCache _cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		public ChattelReader(ChattelConfiguration config) {
			if (config == null) {
				throw new ArgumentNullException(nameof(config));
			}

			_config = config;

			if (_config.CacheEnabled) {
				_cache = new ChattelCache(config);
			}
		}

		/// <summary>
		/// Alias for GetAssetSync
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		public StratusAsset ReadAssetSync(UUID assetId) {
			return GetAssetSync(assetId);
		}

		/// <summary>
		/// Gets the asset from the server.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetId">Asset identifier.</param>
		public StratusAsset GetAssetSync(UUID assetId) {
			StratusAsset result = null;

			// Ask for null, get null.
			if (assetId.Guid == Guid.Empty) {
				return null;
			}

			// Hit up the cache first.
			if (_cache?.TryGetCachedAsset(assetId, out result) ?? false) {
				return result;
			}

			// Got to go try the servers now.
			foreach (var parallelServers in _config.SerialParallelAssetServers) {
				if (parallelServers.Count == 1) {
					result = parallelServers[0].RequestAssetSync(assetId);
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
	}
}
