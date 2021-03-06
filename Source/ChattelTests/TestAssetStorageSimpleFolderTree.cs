﻿// TestChattelConfiguration.cs
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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chattel;
using InWorldz.Data.Assets.Stratus;
using NSubstitute;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestAssetStorageSimpleFolderTree {
		private static readonly DirectoryInfo LOCAL_STORAGE_DIR_INFO = new DirectoryInfo(Constants.LOCAL_STORAGE_PATH);

		public static void CleanLocalStorageFolder(DirectoryInfo localStorage) {
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			try {
				localStorage.Delete(true);
			}
			catch {
				// Failure to delete is fine.
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			localStorage.Refresh();
		}

		public static void CreateLocalStorageFolder(DirectoryInfo localStorage) {
			localStorage.Create();
			localStorage.Refresh();
		}

		public static void CreateLocalStorageEntry(DirectoryInfo localStorage, StratusAsset asset) {
			var path = UuidToLocalPath(localStorage, asset.Id);

			Directory.GetParent(path).Create();
			using (var file = File.Create(path)) {
				ProtoBuf.Serializer.Serialize(file, asset);
			}
		}

		public static StratusAsset GetLocalStorageEntry(DirectoryInfo localStorage, Guid assetId) {
			var path = UuidToLocalPath(localStorage, assetId);
			StratusAsset asset;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				asset = ProtoBuf.Serializer.Deserialize<StratusAsset>(stream);
			}
			return asset;
		}

		public static bool LocalStorageEntryExists(DirectoryInfo localStorage, Guid assetId) {
			return File.Exists(UuidToLocalPath(localStorage, assetId));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTreeUtil_CreateLocalStorageEntry_and_LocalStorageEntryExists() {
			// I know, bad form to test two things at once.

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId), "Failed to prep local storage file!");
		}

		/// <summary>
		/// Converts a GUID to a path based on the local storage location.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="id">Asset identifier.</param>
		private static string UuidToLocalPath(DirectoryInfo localStorage, Guid id) {
			var noPunctuationAssetId = id.ToString("N");
			var path = localStorage.FullName;
			for (var index = 0; index < noPunctuationAssetId.Length; index += 2) {
				path = Path.Combine(path, noPunctuationAssetId.Substring(index, 2));
			}
			return path + ".pbasset";
		}


		[OneTimeSetUp]
		public static void Setup() {
			CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[SetUp]
		public static void PrepareBeforeEveryTest() {
			CreateLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		#region Ctor

		[Test]
		public static void TestAssetStorageSimpleFolderTree_Ctor_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			Assert.DoesNotThrow(() => new AssetStorageSimpleFolderTree(config));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_Ctor_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new AssetStorageSimpleFolderTree(null));
		}

		#endregion

		#region TryGetAsset

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_EmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			Assert.DoesNotThrow(() => localStorage.TryGetAsset(Guid.NewGuid(), out var asset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_Unknown_ReturnsFalse() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			Assert.False(localStorage.TryGetAsset(Guid.NewGuid(), out var asset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_Unknown_OutNull() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			localStorage.TryGetAsset(Guid.NewGuid(), out var asset);
			Assert.Null(asset);
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_Known_ReturnsTrue() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.True(localStorage.TryGetAsset(assetId, out var asset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_Known_OutNotNull() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			localStorage.TryGetAsset(assetId, out var asset);
			Assert.NotNull(asset);
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_TryGetAsset_Known_OutEqualAssets() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, testAsset);

			localStorage.TryGetAsset(testAsset.Id, out var asset);
			Assert.AreEqual(testAsset, asset);
		}

		#endregion

		#region StoreAsset

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_NoLocalStorage_DoesntThrow() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			Assert.DoesNotThrow(() => localStorage.StoreAsset(testAsset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_NoLocalStorage_DoesntExist() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			localStorage.StoreAsset(testAsset);

			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, testAsset.Id));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_Null_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			Assert.DoesNotThrow(() => localStorage.StoreAsset(null));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_Single_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			Assert.DoesNotThrow(() => localStorage.StoreAsset(testAsset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_Single_CreatesAsset() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			localStorage.StoreAsset(testAsset);

			Assert.IsTrue(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, testAsset.Id));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_Single_AssetStoredCorrectly() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
				Description = "7nwtzcv84w78",
			};

			localStorage.StoreAsset(testAsset);

			Assert.AreEqual(testAsset, GetLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, testAsset.Id));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_ParallelDuplicate_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
				Description = "7nwtzcv84w78",
			};

			Assert.DoesNotThrow(
				() =>
					Parallel.Invoke(
						() => {
							localStorage.StoreAsset(testAsset);
						},
						() => {
							localStorage.StoreAsset(testAsset);
						}
					)
			);
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_SerialDuplicate_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
				Description = "7nwtzcv84w78",
			};

			localStorage.StoreAsset(testAsset);

			Assert.DoesNotThrow(() => localStorage.StoreAsset(testAsset));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_StoreAsset_SerialDuplicate_DoesntOverwrite() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();

			var testAsset1 = new StratusAsset {
				Id = assetId,
				Description = "7nwtzcv84w78",
			};

			var testAsset2 = new StratusAsset {
				Id = assetId,
				Description = "f4fn983984",
			};

			localStorage.StoreAsset(testAsset1);
			localStorage.StoreAsset(testAsset2);

			Assert.AreEqual(testAsset1, GetLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, testAsset2.Id));
		}

		#endregion

		#region PurgeAll

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_Null_EmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			Assert.DoesNotThrow(() => localStorage.PurgeAll(null));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_Null_NonEmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.DoesNotThrow(() => localStorage.PurgeAll(null));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_Null_NonEmptyLocalStorage_RemovesEntry() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			localStorage.PurgeAll(null);

			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}


		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_EmptyList_EmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			Assert.DoesNotThrow(() => localStorage.PurgeAll(new List<AssetFilter>{ }));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_EmptyList_NonEmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.DoesNotThrow(() => localStorage.PurgeAll(new List<AssetFilter> { }));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_EmptyList_NonEmptyLocalStorage_RemovesEntry() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			localStorage.PurgeAll(new List<AssetFilter> { });

			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}


		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_EmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			Assert.DoesNotThrow(() => localStorage.PurgeAll(new List<AssetFilter>{
				new AssetFilter {
					LocalFilter = true,
				}
			}));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_Match_NonEmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
				Local = true,
			});

			Assert.DoesNotThrow(() => localStorage.PurgeAll(new List<AssetFilter> {
				new AssetFilter {
					LocalFilter = true,
				}
			}));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_Match_NonEmptyLocalStorage_RemovesEntry() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
				Local = true,
			});

			localStorage.PurgeAll(new List<AssetFilter> {
				new AssetFilter {
					LocalFilter = true,
				}
			});

			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_Nonmatch_NonEmptyLocalStorage_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
				Local = false,
			});

			Assert.DoesNotThrow(() => localStorage.PurgeAll(new List<AssetFilter> {
				new AssetFilter {
					LocalFilter = true,
				}
			}));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_Nonmatch_NonEmptyLocalStorage_DoesntRemoveEntry() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
				Local = false,
			});

			localStorage.PurgeAll(new List<AssetFilter> {
				new AssetFilter {
					LocalFilter = true,
				}
			});

			Assert.IsTrue(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_PurgeAll_SingleFilter_OneMatch_OneMismatch_Correct() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId1 = Guid.NewGuid();
			var assetId2 = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId1,
				Local = false,
			});
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId2,
				Local = true,
			});

			localStorage.PurgeAll(new List<AssetFilter> {
				new AssetFilter {
					LocalFilter = true,
				}
			});

			Assert.IsTrue(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId1));
			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId2));
		}

		#endregion

		#region Purge

		[Test]
		public static void TestAssetStorageSimpleFolderTree_Purge_Unknown_AssetNotFoundException() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();

			Assert.Throws<AssetNotFoundException>(() => localStorage.Purge(assetId));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_Purge_Known_DoesntThrow() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId), "Failed to prep local storage file!");

			Assert.DoesNotThrow(() => localStorage.Purge(assetId));
		}

		[Test]
		public static void TestAssetStorageSimpleFolderTree_Purge_NonEmptyLocalStorage_RemovesEntry() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);

			var assetId = Guid.NewGuid();
			CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

			Assert.That(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId), "Failed to prep local storage file!");

			localStorage.Purge(assetId);

			Assert.IsFalse(LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		// No test for checking to see if the folder tree got cleaned up.
		// Mainly because I've not implemented such cleanup. Using up too many inodes? Use a better local storage implmentation.

		#endregion
	}
}
