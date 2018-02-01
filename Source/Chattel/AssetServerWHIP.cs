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
	internal class AssetServerWHIP : IAssetServer {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<AssetServerWHIP>();

		public string Host { get; private set; }
		public int Port { get; private set; }
		public string Password { get; private set; }

		private readonly string _serverHandle;

		private RemoteServer _provider;

		public AssetServerWHIP(string serverTitle, string host, int port, string password) {
			_serverHandle = serverTitle;

			Host = host;
			Port = port;
			Password = password;

			_provider = new RemoteServer(Host, (ushort)Port, Password);
			_provider.Start(); // TODO: this needs to be started when needed, and shutdown after no usage for a period of time.  Noted that the library doesn't like repeated start stops, it seems to not keep up the auth info, so the whole _whipServer instance would need to be scrapped and reinitialized.
			var status = _provider.GetServerStatus();

			LOG.Log(Logging.LogLevel.Info, () => $"[WHIP_SERVER] [{_serverHandle}] WHIP connection prepared for host {Host}:{Port}\n'{status}'.");
		}

		public StratusAsset RequestAssetSync(Guid assetID) {
			Asset whipAsset = null;

			try {
				whipAsset = _provider.GetAsset(assetID.ToString());
			}
			catch (AssetServerError e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[WHIP_SERVER] [{_serverHandle}] Error getting asset from server.", e);
				return null;
			}
			catch (AuthException e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[WHIP_SERVER] [{_serverHandle}] Authentication error getting asset from server.", e);
				return null;
			}

			return StratusAsset.FromWHIPAsset(whipAsset);
		}

		public void StoreAssetSync(StratusAsset asset) {
			try {
				_provider.PutAsset(StratusAsset.ToWHIPAsset(asset));
			}
			catch (AssetServerError e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[WHIP_SERVER] [{_serverHandle}] Error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
			catch (AuthException e) {
				LOG.Log(Logging.LogLevel.Error, () => $"[WHIP_SERVER] [{_serverHandle}] Authentication error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
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

		// This code added to correctly implement the disposable pattern.
		void IDisposable.Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion
	}
}
