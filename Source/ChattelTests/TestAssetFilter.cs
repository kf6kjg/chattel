// TestAssetFilter.cs
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
using System.Text.RegularExpressions;
using Chattel;
using InWorldz.Data.Assets.Stratus;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public class TestAssetFilter {
		#region IdFilter MatchAsset

		[Test]
		public void TestAssetFilter_IdFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^000"),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_IdFilter_Match_True() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^d5c"),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region TypeFilter MatchAsset

		[Test]
		public void TestAssetFilter_TypeFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^000"),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_TypeFilter_Match_True() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^d5c"),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region LocalFilter MatchAsset

		[Test]
		public void TestAssetFilter_LocalFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^000"),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_LocalFilter_Match_True() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^d5c"),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region TemporaryFilter MatchAsset

		[Test]
		public void TestAssetFilter_TemporaryFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^000"),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_TemporaryFilter_Match_True() {
			var asset = new StratusAsset {
				Id = Guid.Parse("d5c70f1f606340c58b9382c5c5a31d69"),
			};

			var filter = new AssetFilter {
				IdFilter = new Regex("^d5c"),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region CreateTimeRangeFilter MatchAsset

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Nothing_True() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Null_Null_True() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					null,
					null
				),
			};

			Assert.True(filter.MatchAsset(asset));
		}


		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_WrongKind_Null_False() {
			
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(012345678L, DateTimeKind.Local),
					null
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Future_Null_False() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(234567890L, DateTimeKind.Utc),
					null
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Past_Null_True() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(012345678L, DateTimeKind.Utc),
					null
				),
			};

			Assert.True(filter.MatchAsset(asset));
		}


		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Null_WrongKind_False() {
			
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					null,
					new DateTime(234567890L, DateTimeKind.Local)
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Null_Future_True() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					null,
					new DateTime(234567890L, DateTimeKind.Utc)
				),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Null_Past_False() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					null,
					new DateTime(012345678L, DateTimeKind.Utc)
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}


		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_WrongKind_WrongKind_False() {
			
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(012345678L, DateTimeKind.Local),
					new DateTime(234567890L, DateTimeKind.Local)
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Future_Future_False() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(234567890L, DateTimeKind.Utc),
					new DateTime(345678901L, DateTimeKind.Utc)
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Past_Past_False() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(001234567L, DateTimeKind.Utc),
					new DateTime(012345678L, DateTimeKind.Utc)
				),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_CreateTimeRangeFilter_Past_Future_True() {
			var asset = new StratusAsset {
				CreateTime = new DateTime(123456789L, DateTimeKind.Utc),
			};

			var filter = new AssetFilter {
				CreateTimeRangeFilter = new DateRange(
					new DateTime(012345678L, DateTimeKind.Utc),
					new DateTime(234567890L, DateTimeKind.Utc)
				),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region NameFilter MatchAsset

		[Test]
		public void TestAssetFilter_NameFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Name = "Qwerty",
			};

			var filter = new AssetFilter {
				NameFilter = new Regex("^Asdf", RegexOptions.IgnoreCase),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_NameFilter_Match_True() {
			var asset = new StratusAsset {
				Name = "Qwerty",
			};

			var filter = new AssetFilter {
				NameFilter = new Regex("^qwe", RegexOptions.IgnoreCase),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region DescriptionFilter MatchAsset

		[Test]
		public void TestAssetFilter_DescriptionFilter_Mismatch_False() {
			var asset = new StratusAsset {
				Description = "Qwerty",
			};

			var filter = new AssetFilter {
				DescriptionFilter = new Regex("^Asdf", RegexOptions.IgnoreCase),
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_DescriptionFilter_Match_True() {
			var asset = new StratusAsset {
				Description = "Qwerty",
			};

			var filter = new AssetFilter {
				DescriptionFilter = new Regex("^qwe", RegexOptions.IgnoreCase),
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion

		#region StorageFlagsFilter MatchAsset

		[Test]
		public void TestAssetFilter_StorageFlagsFilter_Mismatch_False() {
			var asset = new StratusAsset {
				StorageFlags = 0b0010,
			};

			var filter = new AssetFilter {
				StorageFlagsFilter = 0b1101,
			};

			Assert.False(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_StorageFlagsFilter_Match_True() {
			var asset = new StratusAsset {
				StorageFlags = 0b0001,
			};

			var filter = new AssetFilter {
				StorageFlagsFilter = 0b0001,
			};

			Assert.True(filter.MatchAsset(asset));
		}

		[Test]
		public void TestAssetFilter_StorageFlagsFilter_PartialMatch_True() {
			var asset = new StratusAsset {
				StorageFlags = 0b0011,
			};

			var filter = new AssetFilter {
				StorageFlagsFilter = 0b0001,
			};

			Assert.True(filter.MatchAsset(asset));
		}

		#endregion
	}
}
