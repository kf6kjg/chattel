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
using System.Globalization;
using System.Reflection;
using InWorldz.Data.Assets.Stratus;
using InWorldz.Whip.Client;
using log4net;
using OpenMetaverse;

namespace Chattel {
	internal class AssetServerWHIP : IAssetServer {
		private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		// Unix-epoch starts at January 1st 1970, 00:00:00 UTC. And all our times in the server are (or at least should be) in UTC.
		private static readonly DateTime UNIX_EPOCH = DateTime.ParseExact("1970-01-01 00:00:00 +0", "yyyy-MM-dd hh:mm:ss z", DateTimeFormatInfo.InvariantInfo).ToUniversalTime();

		public string Host { get; private set; }
		public int Port { get; private set; }
		public string Password { get; private set; }

		private string _serverHandle { get; set; }

		private RemoteServer _provider = null;

		public AssetServerWHIP(AssetServerWHIPConfig config) : this(config.Name, config.Host, config.Port, config.Password) {
		}

		public AssetServerWHIP(string serverTitle, string host, int port, string password) {
			_serverHandle = serverTitle;

			Host = host;
			Port = port;
			Password = password;

			_provider = new RemoteServer(Host, (ushort)Port, Password);
			_provider.Start(); // TODO: this needs to be started when needed, and shutdown after no usage for a period of time.  Noted that the library doesn't like repeated start stops, it seems to not keep up the auth info, so the whole _whipServer instance would need to be scrapped and reinitialized.
			var status = _provider.GetServerStatus();

			LOG.Info($"[WHIP_SERVER] [{_serverHandle}] WHIP connection prepared for host {Host}:{Port}\n'{status}'.");
		}

		public void Dispose() {
			try {
				_provider?.Stop();
			}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			catch {
				// Nothing to do here.
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			_provider = null;
		}

		public StratusAsset RequestAssetSync(UUID assetID) {
			Asset whipAsset = null;

			try {
				whipAsset = _provider.GetAsset(assetID.ToString());
			}
			catch (AssetServerError e) {
				LOG.Error($"[WHIP_SERVER] [{_serverHandle}] Error getting asset from server.", e);
				return null;
			}
			catch (AuthException e) {
				LOG.Error($"[WHIP_SERVER] [{_serverHandle}] Authentication error getting asset from server.", e);
				return null;
			}

			return new StratusAsset {
				Id = assetID.Guid,
				Type = (sbyte)whipAsset.Type,
				Local = whipAsset.Local,
				Temporary = whipAsset.Temporary,
				CreateTime = UnixToUTCDateTime(whipAsset.CreateTime),
				Name = whipAsset.Name,
				Description = whipAsset.Description,
				Data = whipAsset.Data,
			};
		}

		public void StoreAssetSync(StratusAsset asset) {
			var whipAsset = new Asset(
				asset.Id.ToString(),
				(byte)asset.Type,
				asset.Local,
				asset.Temporary,
				(int)UTCDateTimeToEpoch(asset.CreateTime), // At some point this'll need to be corrected to a 64bit timestamp...
				asset.Name,
				asset.Description,
				asset.Data
			);

			try {
				_provider.PutAsset(whipAsset);
			}
			catch (AssetServerError e) {
				LOG.Error($"[WHIP_SERVER] [{_serverHandle}] Error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
			catch (AuthException e) {
				LOG.Error($"[WHIP_SERVER] [{_serverHandle}] Authentication error sending asset to server.", e);
				throw new AssetWriteException(asset.Id, e);
			}
		}

		private static DateTime UnixToUTCDateTime(long seconds) {
			return UNIX_EPOCH.AddSeconds(seconds);
		}

		private static long UTCDateTimeToEpoch(DateTime timestamp) {
			return (long)(timestamp.Subtract(UNIX_EPOCH)).TotalSeconds;
		}
	}
}
