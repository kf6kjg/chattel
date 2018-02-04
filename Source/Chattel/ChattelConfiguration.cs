// ChattelConfiguration.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chattel {
	public class ChattelConfiguration {
		public static readonly string DEFAULT_DB_FOLDER_PATH = "cache";
		public static readonly string DEFAULT_WRITECACHE_FILE_PATH = "chattel.wcache";
		public static readonly uint DEFAULT_WRITECACHE_RECORD_COUNT = 1024U * 1024U * 1024U/*1GB*/ / WriteCacheNode.BYTE_SIZE;

		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelConfiguration>();

		internal IEnumerable<IEnumerable<IAssetServer>> SerialParallelAssetServers;

		public DirectoryInfo CacheFolder { get; private set; }

		public FileInfo WriteCacheFile { get; private set; }

		public uint WriteCacheRecordCount { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelConfiguration"/> class.
		/// If the cachePath is null, empty, or references a folder that doesn't exist or doesn't have write access, the cache will be disabled.
		/// The serialParallelServerConfigs parameter allows you to specify server groups that should be accessed serially with subgroups that should be accessed in parallel.
		/// Eg. if you have a new server you want to be hit for all operations, but to fallback to whichever of two older servers returns first, then set up a pattern like [ [ primary ], [ second1, second2 ] ].
		/// </summary>
		/// <param name="cachePath">Cache folder path.  Folder must exist or caching will be disabled.</param>
		/// <param name="serialParallelServers">Serially-accessed parallel servers.</param>
		public ChattelConfiguration(string cachePath, string writeCachePath, uint writeCacheRecordCount, IEnumerable<IEnumerable<IAssetServer>> serialParallelServers) {
			// Set up caching
			if (string.IsNullOrWhiteSpace(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"Cache path is empty, caching assets disabled.");
			}
			else if (!Directory.Exists(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"Cache path folder does not exist, caching assets disabled.");
			}
			else {
				CacheFolder = new DirectoryInfo(cachePath);
				LOG.Log(Logging.LogLevel.Info, () => $"Caching assets enabled at {CacheFolder.FullName}");
			}

			if (string.IsNullOrWhiteSpace(writeCachePath) || !CacheEnabled || writeCacheRecordCount <= 0) {
				LOG.Log(Logging.LogLevel.Warn, () => $"Write cache file path is empty, write cache record count is zero, or caching is disabled. Crash recovery will be compromised.");
			}
			else {
				WriteCacheFile = new FileInfo(writeCachePath);
				WriteCacheRecordCount = writeCacheRecordCount;
				LOG.Log(Logging.LogLevel.Info, () => $"Write cache enabled at {WriteCacheFile.FullName} with {WriteCacheRecordCount} records.");
			}

			// Set up server handlers
			var serialParallelAssetServers = new List<List<IAssetServer>>();
			SerialParallelAssetServers = serialParallelAssetServers;

			// Copy server handle lists so that the list cannot be changed from outside.
			if (serialParallelServers != null && serialParallelServers.Any()) {
				foreach (var parallelServers in serialParallelServers) {
					var parallelServerConnectors = new List<IAssetServer>();
					foreach (var serverConnector in parallelServers) {
						if (serverConnector != null) {
							parallelServerConnectors.Add(serverConnector);
						}
					}

					if (parallelServerConnectors.Any()) {
						serialParallelAssetServers.Add(parallelServerConnectors);
					}
				}
			}
			else {
				LOG.Log(Logging.LogLevel.Warn, () => "Servers empty or not specified. No asset servers connectors configured.");
			}
		}

		public ChattelConfiguration(string cachePath, string writeCachePath, uint writeCacheRecordCount)
			: this(cachePath, writeCachePath, writeCacheRecordCount, (IEnumerable<IEnumerable<IAssetServer>>) null) {
		}
		public ChattelConfiguration(string cachePath, IEnumerable<IEnumerable<IAssetServer>> serialParallelServers)
			: this(cachePath, null, 0, serialParallelServers) {
		}
		public ChattelConfiguration(string cachePath)
			: this(cachePath, null, 0) {
		}
		public ChattelConfiguration(IEnumerable<IEnumerable<IAssetServer>> serialParallelServers)
			: this(null, null, 0, serialParallelServers) {
		}

		public ChattelConfiguration(string cachePath, string writeCachePath, uint writeCacheRecordCount, IAssetServer assetServer)
			: this(cachePath, writeCachePath, writeCacheRecordCount, assetServer != null ? new List<List<IAssetServer>> { new List<IAssetServer> { assetServer } } : null){
		}
		public ChattelConfiguration(string cachePath, IAssetServer assetServer)
			: this(cachePath, null, 0, assetServer) {
		}
		public ChattelConfiguration(IAssetServer assetServer)
			: this(null, null, 0, assetServer){
		}

		public bool CacheEnabled {
			get {
				return CacheFolder != null;
			}
		}

		public void DisableCache() {
			CacheFolder = null;
		}
	}
}
