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
	public static class TestStratusAssetExtensions {
		#region HasAssetData

		[Test]
		public static void TestStratusAsset_HasAssetData_Empty() {
			var asset = new StratusAsset {
				Data = new byte[] { }
			};

			Assert.False(asset.HasAssetData());
		}

		[Test]
		public static void TestStratusAsset_HasAssetData_Null() {
			var asset = new StratusAsset {
				Data = null
			};

			Assert.False(asset.HasAssetData());
		}

		[Test]
		public static void TestStratusAsset_HasAssetData_One() {
			var asset = new StratusAsset {
				Data = new byte[] { 1 }
			};

			Assert.True(asset.HasAssetData());
		}

		#endregion

		#region IsAudioAsset

		[Test]
		public static void TestStratusAsset_IsAudioAsset__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.True(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.True(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsAudioAsset());
		}

		[Test]
		public static void TestStratusAsset_IsAudioAsset_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsAudioAsset());
		}

		#endregion

		#region IsBinaryAsset

		[Test]
		public static void TestStratusAsset_IsBinaryAsset__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.True(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsBinaryAsset());
		}

		[Test]
		public static void TestStratusAsset_IsBinaryAsset_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.True(asset.IsBinaryAsset());
		}

		#endregion

		#region IsFolder

		[Test]
		public static void TestStratusAsset_IsFolder__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.True(asset.IsFolder());
		}

		[Test]
		public static void TestStratusAsset_IsFolder_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsFolder());
		}

		#endregion

		#region IsImageAsset

		[Test]
		public static void TestStratusAsset_IsImageAsset__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.True(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsImageAsset());
		}

		[Test]
		public static void TestStratusAsset_IsImageAsset_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsImageAsset());
		}

		#endregion

		#region IsLink

		[Test]
		public static void TestStratusAsset_IsLink__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.True(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.True(asset.IsLink());
		}

		[Test]
		public static void TestStratusAsset_IsLink_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsLink());
		}

		#endregion

		#region IsTextualAsset

		[Test]
		public static void TestStratusAsset_IsTextualAsset__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.True(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsTextualAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextualAsset_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsTextualAsset());
		}

		#endregion

		#region IsTextureAsset

		[Test]
		public static void TestStratusAsset_IsTextureAsset__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.True(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.True(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.IsTextureAsset());
		}

		[Test]
		public static void TestStratusAsset_IsTextureAsset_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.IsTextureAsset());
		}

		#endregion

		#region MightContainReferences

		[Test]
		public static void TestStratusAsset_MightContainReferences__1Unknown() {
			var asset = new StratusAsset {
				Type = -1 // Unknown
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_00Texture() {
			var asset = new StratusAsset {
				Type = 0 // Texture
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_01Sound() {
			var asset = new StratusAsset {
				Type = 1 // Sound
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_02CallingCard() {
			var asset = new StratusAsset {
				Type = 2
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_03Landmark() {
			var asset = new StratusAsset {
				Type = 3
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_05Clothing() {
			var asset = new StratusAsset {
				Type = 5
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_06Object() {
			var asset = new StratusAsset {
				Type = 6 // Object
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_07Notecard() {
			var asset = new StratusAsset {
				Type = 7 // Notecard
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_10LSLText() {
			var asset = new StratusAsset {
				Type = 10 // LSLText
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_11LSLBytecode() {
			var asset = new StratusAsset {
				Type = 11 // LSLBytecode
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_12TextureTGA() {
			var asset = new StratusAsset {
				Type = 12 // TextureTGA
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_13Bodypart() {
			var asset = new StratusAsset {
				Type = 13 // Bodypart
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_17SoundWAV() {
			var asset = new StratusAsset {
				Type = 17 // SoundWAV
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_18ImageTGA() {
			var asset = new StratusAsset {
				Type = 18 // ImageTGA
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_19ImageJPEG() {
			var asset = new StratusAsset {
				Type = 19 // ImageJPEG
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_20Animation() {
			var asset = new StratusAsset {
				Type = 20 // Animation
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_21Gesture() {
			var asset = new StratusAsset {
				Type = 21 // Gesture
			};

			Assert.True(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_22Simstate() {
			var asset = new StratusAsset {
				Type = 22 // Simstate
			};

			Assert.True(asset.MightContainReferences()); // Maybe?
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_24Link() {
			var asset = new StratusAsset {
				Type = 24
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_25LinkFolder() {
			var asset = new StratusAsset {
				Type = 25
			};

			Assert.False(asset.MightContainReferences());
		}

		[Test]
		public static void TestStratusAsset_MightContainReferences_49Mesh() {
			var asset = new StratusAsset {
				Type = 49
			};

			Assert.False(asset.MightContainReferences());
		}

		#endregion

	}
}
