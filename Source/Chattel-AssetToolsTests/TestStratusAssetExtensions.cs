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
		#region HasAssetData

		[Test]
		public void TestHasAssetData_Empty() {
			var asset = new StratusAsset {
				Data = new byte[] { }
			};

			Assert.False(asset.HasAssetData());
		}

		[Test]
		public void TestHasAssetData_Null() {
			var asset = new StratusAsset {
				Data = null
			};

			Assert.False(asset.HasAssetData());
		}

		[Test]
		public void TestHasAssetData_One() {
			var asset = new StratusAsset {
				Data = new byte[] { 1 }
			};

			Assert.True(asset.HasAssetData());
		}

		#endregion

		#region IsAudioAsset

		[Test]
		public void TestIsAudioAsset_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.True(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.True(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public void TestIsAudioAsset49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsAudioAsset());
		}

		#endregion

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

		#region IsFolder

		[Test]
		public void TestIsFolder_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.True(asset.IsFolder());
		}

		[Test]
		public void TestIsFolder49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsFolder());
		}

		#endregion

		#region IsImageAsset

		[Test]
		public void TestIsImageAsset_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public void TestIsImageAsset49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsImageAsset());
		}

		#endregion

		#region IsLink

		[Test]
		public void TestIsLink_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public void TestIsLink24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.True(asset.IsLink());
		}

		[Test]
		public void TestIsLink25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.True(asset.IsLink());
		}

		[Test]
		public void TestIsLink49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsLink());
		}

		#endregion

		#region IsTextualAsset

		[Test]
		public void TestIsTextualAsset_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public void TestIsTextualAsset49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsTextualAsset());
		}

		#endregion

		#region IsTextureAsset

		[Test]
		public void TestIsTextureAsset_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public void TestIsTextureAsset49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsTextureAsset());
		}

		#endregion

		#region MightContainReferences

		[Test]
		public void TestMightContainReferences_1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.True(asset.MightContainReferences()); // Maybe?
		}

		[Test]
		public void TestMightContainReferences24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public void TestMightContainReferences49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.MightContainReferences());
		}

		#endregion

	}
}
