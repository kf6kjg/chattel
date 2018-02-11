// AssetServerWHIP.cs
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
using InWorldz.Data.Assets.Stratus;
using InWorldz.Whip.Client;

namespace Chattel {
	/// <summary>
	/// Provides a WHIP-protocol connection to a WHIP or WHIP-compatible asset server.
	/// </summary>
	public sealed class AssetServerWHIP : IAssetServer {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<AssetServerWHIP>();

		public string Host { get; private set; }
		public int Port { get; private set; }
		public string Password { get; private set; }

		private readonly string _serverHandle;

		private RemoteServer _provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Chattel.AssetServerWHIP"/> class by opening up a connection to the remote WHIP server.
		/// </summary>
		/// <param name="serverTitle">Server title. Used to provide a visuall handle for this server in the logs.</param>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		/// <param name="password">Password.</param>
		public AssetServerWHIP(string serverTitle, string host, int port, string password) {
			_serverHandle = serverTitle;

			Host = host;
			Port = port;
			Password = password;

			_provider = new RemoteServer(Host, (ushort)Port, Password);
			_provider.Start(); // TODO: this needs to be started when needed, and shutdown after no usage for a period of time.  Noted that the library doesn't like repeated start stops, it seems to not keep up the auth info, so the whole _whipServer instance would need to be scrapped and reinitialized.
			var status = _provider.GetServerStatus();

			LOG.Log(Logging.LogLevel.Info, () => $"[{_serverHandle}] WHIP connection prepared for host {Host}:{Port}\n'{status}'.");
		}

		/// <summary>
		/// Handles an incoming request for an asset from the remote server.
		/// </summary>
		/// <returns>The asset or null if not found.</returns>
		/// <param name="assetID">Asset identifier.</param>
		public StratusAsset RequestAssetSync(Guid assetID) {
			Asset whipAsset = null;

			try {
				whipAsset = _provider.GetAsset(assetID.ToString());
			}
			catch (AssetServerError e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[{_serverHandle}] Error getting asset from server.", e);
				return null;
			}
			catch (AuthException e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[{_serverHandle}] Authentication error getting asset from server.", e);
				return null;
			}

			return StratusAsset.FromWHIPAsset(whipAsset);
		}

		/// <summary>
		/// Handles a request to store an asset to the remote server.
		/// </summary>
		/// <param name="asset">Asset.</param>
		/// <exception cref="T:Chattel.AssetWriteException">Thrown if there was an error storing the asset.</exception>
		public void StoreAssetSync(StratusAsset asset) {
			asset = asset ?? throw new ArgumentNullException(nameof(asset));
			if (asset.Id == Guid.Empty) {
				throw new ArgumentException("Assets must not have a zero ID");
			}

			try {
				_provider.PutAsset(StratusAsset.ToWHIPAsset(asset));
			}
			catch (AssetServerError e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[{_serverHandle}] Error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
			catch (AuthException e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[{_serverHandle}] Authentication error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		private void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					try {
						_provider?.Stop();
					}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
					catch {
						// Nothing to do here.
					}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
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
