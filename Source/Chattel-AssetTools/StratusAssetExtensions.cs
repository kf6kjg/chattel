// MyClass.cs
//
// Author:
//       Ricky C <>
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
using System;
using InWorldz.Data.Assets.Stratus;

namespace ChattelAssetTools {
	public static class StratusAssetExtensions {
		static StratusAssetExtensions() {
			CSJ2K.Util.BitmapImageCreator.Register();
		}

		public static bool ContainsReferences(this StratusAsset asset) {
			return
				asset.IsTextualAsset() && (
					asset.Type != (sbyte)AssetType.Notecard &&
					asset.Type != (sbyte)AssetType.CallingCard &&
					asset.Type != (sbyte)AssetType.LSLText &&
					asset.Type != (sbyte)AssetType.Landmark
				)
			;
		}

		public static bool IsTextualAsset(this StratusAsset asset) {
			return !asset.IsBinaryAsset();
		}

		public static bool IsBinaryAsset(this StratusAsset asset) {
			return
				asset.Type == (sbyte) AssetType.Animation ||
				asset.Type == (sbyte) AssetType.Gesture ||
				asset.Type == (sbyte) AssetType.Simstate ||
				asset.Type == (sbyte) AssetType.Unknown ||
				asset.Type == (sbyte) AssetType.Object ||
				asset.Type == (sbyte) AssetType.Sound ||
				asset.Type == (sbyte) AssetType.SoundWAV ||
				asset.Type == (sbyte) AssetType.Texture ||
				asset.Type == (sbyte) AssetType.TextureTGA ||
				asset.Type == (sbyte) AssetType.Folder ||
				asset.Type == (sbyte) FolderType.Root ||
				asset.Type == (sbyte) FolderType.LostAndFound ||
				asset.Type == (sbyte) FolderType.Snapshot ||
				asset.Type == (sbyte) FolderType.Trash ||
				asset.Type == (sbyte) AssetType.ImageJPEG ||
				asset.Type == (sbyte) AssetType.ImageTGA ||
				asset.Type == (sbyte) AssetType.LSLBytecode
			;
		}

		public static bool IsImageAsset(this StratusAsset asset) {
			return
				asset.Type == (sbyte)AssetType.Texture ||
				asset.Type == (sbyte)AssetType.TextureTGA ||
				asset.Type == (sbyte)AssetType.ImageJPEG ||
				asset.Type == (sbyte)AssetType.ImageTGA
			;
		}

		public static System.Drawing.Bitmap ToBitmap(this StratusAsset asset)
		{
			if (asset.Type == (sbyte)AssetType.Texture || asset.Type == (sbyte)AssetType.ImageJPEG) { // TODO: figure out Texture vs ImageJPEG
				var jp2k = CSJ2K.J2kImage.FromBytes(asset.Data);
				return jp2k.As<System.Drawing.Bitmap>();
			}

			return null;
		}

		// ----
		// And some stuff taken from LibreMetaverse, remove these if LMV ever needs to become a dependency
		// ----

		internal enum AssetType : sbyte {
			Unknown = -1,
			Texture,
			Sound,
			CallingCard,
			Landmark,
			Clothing = 5,
			Object,
			Notecard,
			Folder,
			LSLText = 10,
			LSLBytecode,
			TextureTGA,
			Bodypart,
			SoundWAV = 17,
			ImageTGA,
			ImageJPEG,
			Animation,
			Gesture,
			Simstate,
			Link = 24,
			LinkFolder,
			MarketplaceFolder,
			Mesh = 49
		}

		internal enum FolderType : sbyte {
			None = -1,
			Texture,
			Sound,
			CallingCard,
			Landmark,
			Clothing = 5,
			Object,
			Notecard,
			Root,
			[Obsolete("No longer used, please use FolderType.Root")]
			OldRoot,
			LSLText,
			BodyPart = 13,
			Trash,
			Snapshot,
			LostAndFound,
			Animation = 20,
			Gesture,
			Favorites = 23,
			EnsembleStart = 26,
			EnsembleEnd = 45,
			CurrentOutfit,
			Outfit,
			MyOutfits,
			Mesh,
			Inbox,
			Outbox,
			BasicRoot,
			MarketplaceListings,
			MarkplaceStock,
			Suitcase = 100
		}

	}
}
