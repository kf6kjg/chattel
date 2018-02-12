// TestChattelWriter.cs
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
	public static class TestChattelWriter {
		private static readonly DirectoryInfo LOCAL_STORAGE_DIR_INFO = new DirectoryInfo(Constants.LOCAL_STORAGE_PATH);
		private static readonly FileInfo WRITE_CACHE_FILE_INFO = new FileInfo(Constants.WRITE_CACHE_PATH);
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;

		[OneTimeSetUp]
		public static void Setup() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
			TestWriteCache.CleanWriteCache(WRITE_CACHE_FILE_INFO);
		}

		[SetUp]
		public static void PrepareBeforeEveryTest() {
			TestAssetStorageSimpleFolderTree.CreateLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
			TestWriteCache.CleanWriteCache(WRITE_CACHE_FILE_INFO);
		}

		#region Ctor

		[Test]
		public static void TestChattelWriter_Ctor_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelWriter(null));
		}

		[Test]
		public static void TestChattelWriter_Ctor_Null_False_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelWriter(null, false));
		}

		[Test]
		public static void TestChattelWriter_Ctor_Null_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelWriter(null, null));
		}

		[Test]
		public static void TestChattelWriter_Ctor_Null_Null_False_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelWriter(null, null, false));
		}


		[Test]
		public static void TestChattelWriter_Ctor_BareCfg_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelWriter(config));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BareCfg_False_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelWriter(config, false));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BareCfg_Null_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelWriter(config, null));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BareCfg_Null_False_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelWriter(config, null, false));
		}


		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_Null_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, null);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_Null_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, null, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}


		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_LSTree_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, localStorage);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_LSTree_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, localStorage, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_Null_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, null, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_LSTree_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config, localStorage, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelWriter_Ctor_BasicCfg_CreatesWriteCache() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT, server);

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelWriter(config);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			WRITE_CACHE_FILE_INFO.Refresh();

			Assert.True(WRITE_CACHE_FILE_INFO.Exists);
		}

		#endregion

		#region HasUpstream

		[Test]
		public static void TestChattelWriter_HasUpstream_None_False() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);

			var writer = new ChattelWriter(config);

			Assert.False(writer.HasUpstream);
		}

		[Test]
		public static void TestChattelWriter_HasUpstream_Mocked_True() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);

			var writer = new ChattelWriter(config);

			Assert.True(writer.HasUpstream);
		}

		#endregion

		#region PutAssetSync

		[Test]
		public static void TestChattelWriter_PutAssetSync_Null_ArgumentNullException() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);

			var writer = new ChattelWriter(config);

			Assert.Throws<ArgumentNullException>(() => writer.PutAssetSync(null));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_EmptyId_ArgumentException() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);

			var writer = new ChattelWriter(config);

			Assert.Throws<ArgumentException>(() => writer.PutAssetSync(new StratusAsset {
				Id = Guid.Empty,
			}));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_Duplicates_AssetExistsException() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			localStorage.TryGetAsset(testAsset.Id, out var junk).Returns(x => {
				x[1] = testAsset;
				return true;
			});

			Assert.Throws<AssetExistsException>(() => writer.PutAssetSync(testAsset));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesLocalStorage_NoRemote() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			localStorage.Received(1).StoreAsset(testAsset);
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesLocalStorage_OneRemote() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			localStorage.Received(1).StoreAsset(testAsset);
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesRemote_NoLocalStorage() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			server.Received(1).StoreAssetSync(testAsset);
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesRemote_WithLocalStorage() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			server.Received(1).StoreAssetSync(testAsset);
		}


		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesRemoteSeries_CorrectOrder() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(new List<List<IAssetServer>> {
				new List<IAssetServer> {
					server1,
				},
				new List<IAssetServer> {
					server2,
				},
			});
			var writer = new ChattelWriter(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server1.WhenForAnyArgs(x => x.StoreAssetSync(testAsset)).Do(x => throw new AssetWriteException(testAsset.Id));

			writer.PutAssetSync(testAsset);

			Received.InOrder(() => {
				server1.StoreAssetSync(testAsset);
				server2.StoreAssetSync(testAsset);
			});
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesBackupRemoteParallel_NoLocalStorage() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(new List<List<IAssetServer>> {
				new List<IAssetServer> {
					server1,
					server2,
				},
			});
			var writer = new ChattelWriter(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server1.WhenForAnyArgs(x => x.StoreAssetSync(testAsset)).Do(x => throw new AssetWriteException(testAsset.Id));

			writer.PutAssetSync(testAsset);

			server2.Received(1).StoreAssetSync(testAsset);
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesBackupRemoteSerial_NoLocalStorage() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(new List<List<IAssetServer>> {
				new List<IAssetServer> {
					server1,
				},
				new List<IAssetServer> {
					server2,
				},
			});
			var writer = new ChattelWriter(config);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server1.WhenForAnyArgs(x => x.StoreAssetSync(testAsset)).Do(x => throw new AssetWriteException(testAsset.Id));

			writer.PutAssetSync(testAsset);

			server2.Received(1).StoreAssetSync(testAsset);
		}


		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesBackupRemoteSerial_WithLocalStorage() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, new List<List<IAssetServer>> {
				new List<IAssetServer> {
					server1,
					server2,
				},
			});
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server1
				.When(x => x.StoreAssetSync(Arg.Any<StratusAsset>()))
				.Do(x => {
					throw new AssetWriteException(testAsset.Id);
				})
			;

			writer.PutAssetSync(testAsset);

			server2.Received(1).StoreAssetSync(testAsset);
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesBackupRemoteParallel_WithLocalStorage() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, new List<List<IAssetServer>> {
				new List<IAssetServer> {
					server1,
				},
				new List<IAssetServer> {
					server2,
				},
			});
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server1.WhenForAnyArgs(x => x.StoreAssetSync(testAsset)).Do(x => throw new AssetWriteException(testAsset.Id));

			writer.PutAssetSync(testAsset);

			server2.Received(1).StoreAssetSync(testAsset);
		}


		[Test]
		public static void TestChattelWriter_PutAssetSync_MultipleParallel_DoesntThrow() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset1 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var testAsset2 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var testAsset3 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			Assert.DoesNotThrow(() => Parallel.Invoke(
				() => writer.PutAssetSync(testAsset1),
				() => writer.PutAssetSync(testAsset2),
				() => writer.PutAssetSync(testAsset3)
			));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_MultipleParallel_AllReceived() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset1 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var testAsset2 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var testAsset3 = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			Parallel.Invoke(
				() => writer.PutAssetSync(testAsset1),
				() => writer.PutAssetSync(testAsset2),
				() => writer.PutAssetSync(testAsset3)
			);

			server.Received(1).StoreAssetSync(testAsset1);
			server.Received(1).StoreAssetSync(testAsset2);
			server.Received(1).StoreAssetSync(testAsset3);
		}


		[Test]
		public static void TestChattelWriter_PutAssetSync_ServerError_AggregateException() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			server
				.WhenForAnyArgs(x => x.StoreAssetSync(Arg.Any<StratusAsset>()))
				.Do(x => {
					throw new Exception(); // Just needs an error to cause remote storage failure.
				})
			;

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			Assert.Throws<AggregateException>(() => writer.PutAssetSync(testAsset));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_ServerError_AggregateException_ContainsCorrectException() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			server
				.WhenForAnyArgs(x => x.StoreAssetSync(Arg.Any<StratusAsset>()))
				.Do(x => {
					throw new DriveNotFoundException(); // Just needs an error to cause remote storage failure.
				})
			;

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			try {
				writer.PutAssetSync(testAsset);
			}
			catch (AggregateException e) {
				Assert.IsInstanceOf(typeof(DriveNotFoundException), e.InnerException);
				return;
			}

			Assert.Fail();
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_FullWriteCache_WriteCacheFullException() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			server
				.WhenForAnyArgs(x => x.StoreAssetSync(Arg.Any<StratusAsset>()))
				.Do(x => {
					throw new Exception(); // Just needs an error to cause remote storage failure.
				})
			;

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			try {
				writer.PutAssetSync(testAsset);
			}
			catch (AggregateException) {
				// moving right along.
			}
			try {
				writer.PutAssetSync(testAsset);
			}
			catch (AggregateException) {
				// moving right along.
			}
			// Write cache currenlty requires one left empty.

			Assert.Throws<WriteCacheFullException>(() => writer.PutAssetSync(testAsset));
		}

		[Test]
		public static void TestChattelWriter_PutAssetSync_FullWriteCache_DoesntHitRemote() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			server
				.WhenForAnyArgs(x => x.StoreAssetSync(Arg.Any<StratusAsset>()))
				.Do(x => {
					throw new Exception(); // Just needs an error to cause remote storage failure.
				})
			;

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			try {
				writer.PutAssetSync(testAsset);
			}
			catch (AggregateException) {
				// moving right along.
			}
			try {
				writer.PutAssetSync(testAsset);
			}
			catch (AggregateException) {
				// moving right along.
			}
			try {
				writer.PutAssetSync(testAsset);
			}
			catch (WriteCacheFullException) {
				// moving right along.
			}
			// Write cache currenlty requires one left empty.

			server.Received(2).StoreAssetSync(testAsset);
		}

		// Sequence correctness tests

		[Test]
		public static void TestChattelWriter_PutAssetSync_ChecksLocalBeforeWritingLocal() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			Received.InOrder(() => {
				localStorage.TryGetAsset(testAsset.Id, out var junk);
				localStorage.StoreAsset(testAsset);
			});
		}

		// Untestable: public static void TestChattelWriter_PutAssetSync_WritesLocalBeforeWriteCache()
		// Untestable: public static void TestChattelWriter_PutAssetSync_WritesWriteCacheBeforeRemote() {

		[Test]
		public static void TestChattelWriter_PutAssetSync_WritesLocalBeforeRemote() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 4, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var writer = new ChattelWriter(config, localStorage);

			var testAsset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			writer.PutAssetSync(testAsset);

			Received.InOrder(() => {
				localStorage.StoreAsset(testAsset);
				server.StoreAssetSync(testAsset);
			});
		}

		#endregion
	}
}
