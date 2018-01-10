﻿// ChattelConfiguration.cs
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
using System.Net.Sockets;
using Nini.Config;

namespace Chattel {
	public class ChattelConfiguration {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelConfiguration>();

		internal IEnumerable<IEnumerable<IAssetServer>> SerialParallelAssetServers;

		internal DirectoryInfo CacheFolder;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelConfiguration"/> class.
		/// If the cachePath is null, empty, or references a folder that doesn't exist or doesn't have write access, the cache will be disabled.
		/// The serialParallelServerConfigs parameter allows you to specify server groups that should be accessed serially with subgroups that should be accessed in parallel.
		/// Eg. if you have a new server you want to be hit for all operations, but to fallback to whichever of two older servers returns first, then set up a pattern like [ [ primary ], [ second1, second2 ] ].
		/// </summary>
		/// <param name="cachePath">Cache folder path.  Folder must exist or caching will be disabled.</param>
		/// <param name="serialParallelServerConfigs">Serially-accessed parallel server configs.</param>
		public ChattelConfiguration(string cachePath = null, IEnumerable<IEnumerable<IAssetServerConfig>> serialParallelServerConfigs = null) {
			// Set up caching
			if (string.IsNullOrWhiteSpace(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] CachePath is empty, caching assets disabled.");
			}
			else if (!Directory.Exists(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] CachePath folder does not exist, caching assets disabled.");
			}
			else {
				CacheFolder = new DirectoryInfo(cachePath);
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] Caching assets enabled at {CacheFolder.FullName}");
			}

			// Set up server handlers
			var serialParallelAssetServers = new List<List<IAssetServer>>();
			SerialParallelAssetServers = serialParallelAssetServers;

			// Set up server handlers
			if (serialParallelServerConfigs != null && serialParallelServerConfigs.Any()) {
				foreach (var parallelConfigs in serialParallelServerConfigs) {
					var parallelServerConnectors = new List<IAssetServer>();
					foreach (var config in parallelConfigs) {
						IAssetServer serverConnector = null;

						switch (config.Type) {
							case AssetServerType.WHIP:
								serverConnector = new AssetServerWHIP((AssetServerWHIPConfig)config);
								break;
							case AssetServerType.CF:
								serverConnector = new AssetServerCF((AssetServerCFConfig)config);
								break;
							default:
								LOG.Log(Logging.LogLevel.Warn, () => $"[ASSET_CONFIG] Unknown asset server type {config.Type} with name {config.Name}.");
								break;
						}

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
				LOG.Log(Logging.LogLevel.Warn, () => "[ASSET_CONFIG] Servers empty or not specified. No asset servers connectors configured.");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelConfiguration"/> class.  Can only define servers that will be accessed serially.
		/// The <paramref name="configSource"/> is the whole Nini configuration, but the <paramref name="assetConfig"/> is the block that contains the following entries:
		/// <list type="bullet">
		/// 	<item>
		/// 		<term>CachePath</term>
		/// 		<description>String. The path to the folder where the disk cache will be stored.  If empty or missing caching is disabled.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>Servers</term>
		/// 		<description>String. Comma delimited list of config section names to pull up server parameters.</description>
		/// 	</item>
		/// </list>
		/// </summary>
		/// <param name="configSource">Config source.</param>
		/// <param name="assetConfig">Config instance for the asset options.</param>
		public ChattelConfiguration(IConfigSource configSource, IConfig assetConfig) {
			// Set up caching
			var cachePath = assetConfig?.GetString("CachePath", string.Empty) ?? string.Empty;

			if (string.IsNullOrWhiteSpace(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] CachePath is empty, caching assets disabled.");
			}
			else if (!Directory.Exists(cachePath)) {
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] CachePath folder does not exist, caching assets disabled.");
			}
			else {
				CacheFolder = new DirectoryInfo(cachePath);
				LOG.Log(Logging.LogLevel.Info, () => $"[ASSET_CONFIG] Caching assets enabled at {CacheFolder.FullName}");
			}

			// Set up server handlers
			var serialParallelAssetServers = new List<List<IAssetServer>>();
			SerialParallelAssetServers = serialParallelAssetServers;

			// Read in a config list that lists the priority order of servers and their settings.
			var serialParallelServerSources = assetConfig?
				.GetString("Servers", string.Empty)
				.Split(',')
				.Where(parallelSources => !string.IsNullOrWhiteSpace(parallelSources))
				.Select(parallelSources => parallelSources
					.Split('&')
					.Where(source => !string.IsNullOrWhiteSpace(source))
					.Select(source => source.Trim())
				)
				.Where(parallelSources => parallelSources.Any())
			;

			if (serialParallelServerSources != null && serialParallelServerSources.Any()) {
				foreach (var parallelSources in serialParallelServerSources) {
					var parallelServerConnectors = new List<IAssetServer>();
					foreach (var source in parallelSources) {
						var sourceConfig = configSource.Configs[source];
						IAssetServer serverConnector = null;
						var type = sourceConfig?.GetString("Type", string.Empty).ToLower();
						try {
							switch (type) {
								case "whip":
									serverConnector = new AssetServerWHIP(
										source,
										sourceConfig.GetString("Host", string.Empty),
										sourceConfig.GetInt("Port", 32700),
										sourceConfig.GetString("Password", "changeme") // Yes, that's the default password for WHIP.
									);
									break;
								case "cf":
									serverConnector = new AssetServerCF(
										source,
										sourceConfig.GetString("Username", string.Empty),
										sourceConfig.GetString("APIKey", string.Empty),
										sourceConfig.GetString("DefaultRegion", string.Empty),
										sourceConfig.GetBoolean("UseInternalURL", true),
										sourceConfig.GetString("ContainerPrefix", string.Empty)
									);
									break;
								default:
									LOG.Log(Logging.LogLevel.Warn, () => $"[ASSET_CONFIG] Unknown asset server type in section [{source}].");
									break;
							}
						}
						catch (SocketException e) {
							LOG.Log(Logging.LogLevel.Error, () => $"[ASSET_CONFIG] Asset server of type '{type}' defined in section [{source}] failed setup. Skipping server.", e);
						}

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
				LOG.Log(Logging.LogLevel.Warn, () => "[ASSET_CONFIG] Servers empty or not specified. No asset server sections configured.");
			}
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
