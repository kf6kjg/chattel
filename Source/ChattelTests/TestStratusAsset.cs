// TestStratusAsset.cs
//
// Author:
//       Ricky Curtice <ricky@rwcproductions.com>
//
// Copyright (c) 2018 Richard Curtice
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
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestStratusAsset {
		private static readonly Guid ASSET_ID = Guid.NewGuid();
		private static readonly StratusAsset STRATUS_ASSET = new StratusAsset {
			CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
			Data = new byte[] { 0, 1, 2, 3 },
			Description = "asdf",
			Id = ASSET_ID,
			Local = false,
			Name = "fdsa",
			StorageFlags = 0,
			Temporary = false,
			Type = 12,
		};
		private static readonly InWorldz.Whip.Client.Asset WHIP_ASSET = new InWorldz.Whip.Client.Asset(
			ASSET_ID.ToString("N"),
			(byte)STRATUS_ASSET.Type,
			STRATUS_ASSET.Local,
			STRATUS_ASSET.Temporary,
			1517468421,
			STRATUS_ASSET.Name,
			STRATUS_ASSET.Description,
			STRATUS_ASSET.Data
		);

		#region Conversions

		[Test]
		public static void TestStratusAsset_FromWHIPAsset_Correct() {
			Assert.AreEqual(STRATUS_ASSET, StratusAsset.FromWHIPAsset(WHIP_ASSET));
		}

		[Test]
		public static void TestStratusAsset_ToWHIPAsset_Correct() {
			Assert.AreEqual(WHIP_ASSET.Serialize().data, StratusAsset.ToWHIPAsset(STRATUS_ASSET).Serialize().data);
		}

		[Test]
		public static void TestStratusAsset_FromWHIPSerialized_Correct() {
			Assert.AreEqual(STRATUS_ASSET, StratusAsset.FromWHIPSerialized(WHIP_ASSET.Serialize().data));
		}

		[Test]
		public static void TestStratusAsset_ToWHIPSerialized_Correct() {
			Assert.AreEqual(WHIP_ASSET.Serialize().data, StratusAsset.ToWHIPSerialized(STRATUS_ASSET));
		}

		#endregion

		#region Comparison

		[Test]
		public static void TestStratusAsset_Equals_Full_Equal() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_NoData_Equal() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_Bare_Equal() {
			var asset1 = new StratusAsset {
			};
			var asset2 = new StratusAsset {
			};

			Assert.AreEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_CreateTimeDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468422).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_DataDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 4, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_DataDiffNull_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = null,
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_DescriptionDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "adf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_IdDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = Guid.NewGuid(),
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_LocalDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = true,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_NameDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fds",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_StorageFlagsDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 1,
				Temporary = false,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_TempDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = true,
				Type = 12,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		[Test]
		public static void TestStratusAsset_Equals_TypeDiff_NotEqual() {
			var asset1 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 12,
			};
			var asset2 = new StratusAsset {
				CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
				Data = new byte[] { 0, 1, 2, 3 },
				Description = "asdf",
				Id = ASSET_ID,
				Local = false,
				Name = "fdsa",
				StorageFlags = 0,
				Temporary = false,
				Type = 1,
			};

			Assert.AreNotEqual(asset1, asset2);
		}

		#endregion
	}
}
