// TestChattelConfiguration.cs
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
using System.IO;
using Chattel;
using InWorldz.Data.Assets.Stratus;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestAssetCacheSimpleFolderTree {
		private static readonly DirectoryInfo CACHE_DIR_INFO = new DirectoryInfo(Constants.CACHE_PATH);
		private static readonly FileInfo WRITE_CACHE_FILE_INFO = new FileInfo(Constants.WRITE_CACHE_PATH);
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;

		public static void CleanCacheFolder(DirectoryInfo cache) {
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			try {
				cache.Delete(true);
			}
			catch {
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			cache.Refresh();
		}

		public static void CreateCacheFolder(DirectoryInfo cache) {
			cache.Create();
			cache.Refresh();
		}

		public static void CreateCacheEntry(DirectoryInfo cache, StratusAsset asset) {
			var path = UuidToCachePath(cache, asset.Id);

			Directory.GetParent(path).Create();
			using (var file = File.Create(path)) {
				ProtoBuf.Serializer.Serialize(file, asset);
			}
		}

		public static bool CacheEntryExists(DirectoryInfo cache, Guid assetId) {
			return File.Exists(UuidToCachePath(cache, assetId));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTreeUtil_CreateCacheEntry_and_CacheEntryExists() {
			// I know, bad form to test two things at once.

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(CacheEntryExists(CACHE_DIR_INFO, assetId), "Failed to prep cache file!");
		}

		/// <summary>
		/// Converts a GUID to a path based on the cache location.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="id">Asset identifier.</param>
		private static string UuidToCachePath(DirectoryInfo cache, Guid id) {
			var noPunctuationAssetId = id.ToString("N");
			var path = cache.FullName;
			for (var index = 0; index < noPunctuationAssetId.Length; index += 2) {
				path = Path.Combine(path, noPunctuationAssetId.Substring(index, 2));
			}
			return path + ".pbasset";
		}


		[OneTimeSetUp]
		public static void Setup() {
			CleanCacheFolder(CACHE_DIR_INFO);
		}

		[SetUp]
		public static void PrepareBeforeEveryTest() {
			CreateCacheFolder(CACHE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			CleanCacheFolder(CACHE_DIR_INFO);
		}

		#region Ctor

		[Test]
		public static void TestAssetCacheSimpleFolderTree_Ctor_DoesntThrow() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			Assert.DoesNotThrow(() => new AssetCacheSimpleFolderTree(config));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_Ctor_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new AssetCacheSimpleFolderTree(null));
		}

		#endregion

		#region TryGetCachedAsset

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_EmptyCache_DoesntThrow() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);
			Assert.DoesNotThrow(() => cache.TryGetCachedAsset(Guid.NewGuid(), out var asset));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_Unknown_ReturnsFalse() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);
			Assert.False(cache.TryGetCachedAsset(Guid.NewGuid(), out var asset));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_Unknown_OutNull() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);
			cache.TryGetCachedAsset(Guid.NewGuid(), out var asset);
			Assert.Null(asset);
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_Known_ReturnsTrue() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.True(cache.TryGetCachedAsset(assetId, out var asset));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_Known_OutNotNull() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			cache.TryGetCachedAsset(assetId, out var asset);
			Assert.NotNull(asset);
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_TryGetCachedAsset_Known_OutEqualAssets() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			CreateCacheEntry(CACHE_DIR_INFO, testAsset);

			cache.TryGetCachedAsset(testAsset.Id, out var asset);
			Assert.AreEqual(testAsset, asset);
		}

		#endregion

		#region PurgeAll

		[Test]
		public static void TestAssetCacheSimpleFolderTree_PurgeAll_EmptyCache_DoesntThrow() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);
			Assert.DoesNotThrow(cache.PurgeAll);
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_PurgeAll_NonEmptyCache_DoesntThrow() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.DoesNotThrow(cache.PurgeAll);
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_PurgeAll_NonEmptyCache_RemovesEntry() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			cache.PurgeAll();

			Assert.IsFalse(CacheEntryExists(CACHE_DIR_INFO, assetId));
		}

		#endregion

		#region Purge

		[Test]
		public static void TestAssetCacheSimpleFolderTree_Purge_Unknown_AssetNotFoundException() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();

			Assert.Throws<AssetNotFoundException>(() => cache.Purge(assetId));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_Purge_Known_DoesntThrow() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(CacheEntryExists(CACHE_DIR_INFO, assetId), "Failed to prep cache file!");

			Assert.DoesNotThrow(() => cache.Purge(assetId));
		}

		[Test]
		public static void TestAssetCacheSimpleFolderTree_Purge_NonEmptyCache_RemovesEntry() {
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			var cache = new AssetCacheSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateCacheEntry(CACHE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(CacheEntryExists(CACHE_DIR_INFO, assetId), "Failed to prep cache file!");

			cache.Purge(assetId);

			Assert.IsFalse(CacheEntryExists(CACHE_DIR_INFO, assetId));
		}

		// No test for checking to see if the folder tree got cleaned up.
		// Mainly because I've not implemented such cleanup. Using up too many inodes? Use a better cache implmentation.

		#endregion
	}
}
