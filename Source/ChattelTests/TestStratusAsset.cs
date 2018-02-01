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
	public class TestStratusAsset {
		private static readonly Guid _assetId = Guid.NewGuid();
		private static readonly StratusAsset _stratusAsset = new StratusAsset {
			CreateTime = DateTimeOffset.FromUnixTimeSeconds(1517468421).DateTime,
			Data = new byte[] { 0, 1, 2, 3 },
			Description = "asdf",
			Id = _assetId,
			Local = false,
			Name = "fdsa",
			StorageFlags = 0,
			Temporary = false,
			Type = 12,
		};
		private static readonly InWorldz.Whip.Client.Asset _whipAsset = new InWorldz.Whip.Client.Asset(
			_assetId.ToString("N"),
			(byte)_stratusAsset.Type,
			_stratusAsset.Local,
			_stratusAsset.Temporary,
			1517468421,
			_stratusAsset.Name,
			_stratusAsset.Description,
			_stratusAsset.Data
		);

		[Test]
		public void TestStratusAsset_FromWHIPAsset_Correct() {
			Assert.AreEqual(_stratusAsset, StratusAsset.FromWHIPAsset(_whipAsset));
		}

		[Test]
		public void TestStratusAsset_ToWHIPAsset_Correct() {
			Assert.AreEqual(_whipAsset.Serialize().data, StratusAsset.ToWHIPAsset(_stratusAsset).Serialize().data);
		}

		[Test]
		public void TestStratusAsset_FromWHIPSerialized_Correct() {
			Assert.AreEqual(_stratusAsset, StratusAsset.FromWHIPSerialized(_whipAsset.Serialize().data));
		}

		[Test]
		public void TestStratusAsset_ToWHIPSerialized_Correct() {
			Assert.AreEqual(_whipAsset.Serialize().data, StratusAsset.ToWHIPSerialized(_stratusAsset));
		}

	}
}
