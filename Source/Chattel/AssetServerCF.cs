// AssetServerCF.cs
//
// Author:
//       Ricky Curtice <ricky@rwcproductions.com>
//
// Copyright (c) 2017 Richard Curtice
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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using InWorldz.Data.Assets.Stratus;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;

namespace Chattel {
	/// <summary>
	/// Provides a connection to a Rackspace CloudFiles asset server.
	/// </summary>
	public sealed class AssetServerCF : IAssetServer {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<AssetServerCF>();

		private const int DEFAULT_READ_TIMEOUT = 45 * 1000;
		private const int DEFAULT_WRITE_TIMEOUT = 10 * 1000;
		/// <summary>
		/// How many hex characters to use for the CF container prefix
		/// </summary>
		private const int CONTAINER_UUID_PREFIX_LEN = 4;


		public string Username { get; private set; }
		public string APIKey { get; private set; }
		public string DefaultRegion { get; private set; }
		public bool UseInternalURL { get; private set; }
		public string ContainerPrefix { get; private set; }

		private readonly string _serverHandle;

		private InWorldz.Data.Assets.Stratus.CoreExt.ExtendedCloudFilesProvider _provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Chattel.AssetServerCF"/> class by opening up and warming the connection to CF.
		/// </summary>
		/// <param name="serverTitle">Server title. Used to provide a visuall handle for this server in the logs.</param>
		/// <param name="username">Username.</param>
		/// <param name="apiKey">API key.</param>
		/// <param name="defaultRegion">Default region.</param>
		/// <param name="useInternalUrl">If set to <c>true</c> use internal URL.</param>
		/// <param name="containerPrefix">Container prefix.</param>
		public AssetServerCF(string serverTitle, string username, string apiKey, string defaultRegion, bool useInternalUrl, string containerPrefix) {
			_serverHandle = serverTitle;

			Username = username;
			APIKey = apiKey;
			DefaultRegion = defaultRegion;
			UseInternalURL = useInternalUrl;
			ContainerPrefix = containerPrefix;

			var identity = new CloudIdentity { Username = Username, APIKey = APIKey };
			var restService = new InWorldz.Data.Assets.Stratus.CoreExt.ExtendedJsonRestServices(DEFAULT_READ_TIMEOUT, DEFAULT_WRITE_TIMEOUT);
			_provider = new InWorldz.Data.Assets.Stratus.CoreExt.ExtendedCloudFilesProvider(identity, DefaultRegion, null, restService);

			//warm up
			_provider.GetAccountHeaders(useInternalUrl: UseInternalURL, region: DefaultRegion);

			LOG.Log(Logging.LogLevel.Info, () => $"[{_serverHandle}] CF connection prepared for region '{DefaultRegion}' and prefix '{ContainerPrefix}' under user '{Username}'.");
		}

		/// <summary>
		/// Handles an incoming request for an asset from the remote server.
		/// </summary>
		/// <returns>The asset or null if not found.</returns>
		/// <param name="assetID">Asset identifier.</param>
		public StratusAsset RequestAssetSync(Guid assetID) {
			string assetIdStr = assetID.ToString();

			using (var memStream = new MemoryStream()) {
				try {
					WarnIfLongOperation($"GetObject for {assetID}", () => _provider.GetObject(GenerateContainerName(assetIdStr), GenerateAssetObjectName(assetIdStr), memStream, useInternalUrl: UseInternalURL, region: DefaultRegion));
				}
				catch {
					return null; // Just skip out for this round if there was an error.
				}

				memStream.Position = 0;

				var stratusAsset = ProtoBuf.Serializer.Deserialize<StratusAsset>(memStream);

				if (stratusAsset?.Data == null) {
					throw new InvalidOperationException($"[{_serverHandle}] Asset deserialization failed. Asset ID: {assetID}, Stream Len: {memStream.Length}");
				}

				return stratusAsset;
			}
		}

		/// <summary>
		/// Handles a request to store an asset to the remote server.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <exception cref="T:Chattel.AssetExistsException">Thrown if the remote server determined that the asset already exists.</exception>
		/// <exception cref="T:Chattel.AssetWriteException">Thrown when there was an error response from the remote server.</exception>
		public void StoreAssetSync(StratusAsset asset) {
			asset = asset ?? throw new ArgumentNullException(nameof(asset));
			if (asset.Id == Guid.Empty) {
				throw new ArgumentException("Assets must not have a zero ID");
			}

			using (var memStream = new MemoryStream()) {
				try {
					ProtoBuf.Serializer.Serialize(memStream, asset);
					memStream.Position = 0;

					var assetIdStr = asset.Id.ToString();

					var mheaders = GenerateStorageHeaders(asset, memStream);

					WarnIfLongOperation("CreateObject",
						() => _provider.CreateObject(
							GenerateContainerName(assetIdStr),
							memStream,
							GenerateAssetObjectName(assetIdStr),
							"application/octet-stream",
							headers: mheaders,
							useInternalUrl: UseInternalURL,
							region: DefaultRegion
						)
					);
				}
				catch (ResponseException e) {
					if (e.Response.StatusCode == System.Net.HttpStatusCode.PreconditionFailed) {
						throw new AssetExistsException(asset.Id, e);
					}

					throw new AssetWriteException(asset.Id, e);
				}
			}
		}

		/// <summary>
		/// CF containers are PREFIX_#### where we use the first N chars of the hex representation
		/// of the asset ID to partition the space. The hex alpha chars in the container name are uppercase.
		/// </summary>
		/// <param name="assetId"></param>
		/// <returns></returns>
		private string GenerateContainerName(string assetId) {
			return ContainerPrefix + assetId.Substring(0, CONTAINER_UUID_PREFIX_LEN).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// The object name is defined by the assetId, dashes stripped, with the .asset prefix.
		/// </summary>
		/// <param name="assetId"></param>
		/// <returns></returns>
		private static string GenerateAssetObjectName(string assetId) {
			return assetId.Replace("-", string.Empty).ToLower(System.Globalization.CultureInfo.InvariantCulture) + ".asset";
		}

		private static Dictionary<string, string> GenerateStorageHeaders(StratusAsset asset, MemoryStream stream) {
			//the HTTP headers only accept letters and digits
			var fixedName = new StringBuilder();
			var appended = false;
			foreach (var letter in asset.Name) {
				var c = (char)(0x000000ff & (uint)letter);
				if (c == 127 || (c < ' ' && c != '\t')) {
					continue;
				}

				fixedName.Append(letter);
				appended = true;
			}

			if (!appended) {
				fixedName.Append("empty");
			}

			var headers = new Dictionary<string, string> {
				{"ETag", Md5Hash(stream)},
				{"X-Object-Meta-Temp", asset.Temporary ? "1" : "0"},
				{"X-Object-Meta-Local", asset.Local ? "1" : "0"},
				{"X-Object-Meta-Type", asset.Type.ToString()},
				{"X-Object-Meta-Name", fixedName.ToString()},
				{"If-None-Match", "*"},
			};

			stream.Position = 0;

			return headers;
		}

		private static string Md5Hash(Stream data) {
			byte[] hash;
			using (var md5 = MD5.Create()) {
				hash = md5.ComputeHash(data);
			}
			return ByteArrayToHexString(hash);
		}

		private static string ByteArrayToHexString(byte[] dataMd5) {
			var sb = new StringBuilder();
			for (var i = 0; i < dataMd5.Length; i++) {
				sb.AppendFormat("{0:x2}", dataMd5[i]);
			}
			return sb.ToString();
		}

		private void WarnIfLongOperation(string opName, Action operation) {
			const long WARNING_TIME = 5000; // ms

			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			operation();
			stopwatch.Stop();

			if (stopwatch.ElapsedMilliseconds >= WARNING_TIME) {
				LOG.Log(Logging.LogLevel.Warn, () => $"[{_serverHandle}] Slow CF operation {opName} took {stopwatch.ElapsedMilliseconds} ms.");
			}
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		private void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// dispose managed state (managed objects).
				}

				_provider = null;

				disposedValue = true;
			}
		}

		/// <summary>
		/// Releases all resource used by this object.
		/// </summary>
		/// <remarks>Call <see cref="IDisposable.Dispose()"/> when you are finished using the objec. The
		/// <see cref="IDisposable.Dispose()"/> method leaves the object in an unusable state. After
		/// calling <see cref="IDisposable.Dispose()"/>, you must release all references to the object
		/// so the garbage collector can reclaim the memory that the object was occupying.</remarks>
		void IDisposable.Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion
	}
}
