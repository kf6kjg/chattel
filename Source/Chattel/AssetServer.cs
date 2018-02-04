// IAssetServer.cs
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
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public abstract class AssetServer : IDisposable {
		private static readonly Dictionary<string, ServerTypeSetup> KNOWN_SERVER_TYPES = new Dictionary<string, ServerTypeSetup>();
		protected static void Register<T>(string name, IEnumerable<Param> paramDefaults) where T : AssetServer {
			KNOWN_SERVER_TYPES.Add(name, new ServerTypeSetup {
				Type = typeof(T),
				Params = paramDefaults,
			});
		}
		//public static TypeInfo 
		public abstract StratusAsset RequestAssetSync(Guid assetID);
		public abstract void StoreAssetSync(StratusAsset asset);

		public abstract void Dispose();


		public struct ServerTypeSetup {
			public Type Type;
			IEnumerable<Param> Params;
		}

		public abstract class Param {
			public string Key { get; set; }
		}

		public class ParamInt : Param {
			public int DefaultValue { get; set; }
		}

		public class ParamKeys : Param {
			public string[] DefaultValue { get; set; }
		}

		public class ParamLong : Param {
			public long DefaultValue { get; set; }
		}

		public class ParamFloat : Param {
			public float DefaultValue { get; set; }
		}

		public class ParamDouble : Param {
			public double DefaultValue { get; set; }
		}

		public class ParamString : Param {
			public string DefaultValue { get; set; }
		}

		public class ParamValues : Param {
			public string[] DefaultValue { get; set; }
		}

		public class ParamBoolean : Param {
			public bool DefaultValue { get; set; }
		}
	}
}
