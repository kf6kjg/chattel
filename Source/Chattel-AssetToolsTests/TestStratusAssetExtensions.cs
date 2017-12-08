// ChattelAssetToolsTests.cs
//
// Author:
//       Ricky Curtice <ricky@rwcproductions.com>
//
// Copyright (c) 2017 
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

using NUnit.Framework;
using System;
using InWorldz.Data.Assets.Stratus;
using ChattelAssetTools;

namespace ChattelAssetToolsTests {
	[TestFixture]
	public class TestStratusAssetExtensions {

		#region IsBinaryAsset

		[Test]
		public void TestIsBinaryAsset_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public void TestIsBinaryAsset49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.True(asset.IsBinaryAsset());
		}

		#endregion
	}
}
