// TestChattelReader.cs
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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chattel;
using InWorldz.Data.Assets.Stratus;
using NSubstitute;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestChattelReader {
		private static readonly DirectoryInfo LOCAL_STORAGE_DIR_INFO = new DirectoryInfo(Constants.LOCAL_STORAGE_PATH);

		[OneTimeSetUp]
		public static void Setup() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[SetUp]
		public static void PrepareBeforeEveryTest() {
			TestAssetStorageSimpleFolderTree.CreateLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		#region Ctor

		[Test]
		public static void TestChattelReader_Ctor_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelReader(null));
		}

		[Test]
		public static void TestChattelReader_Ctor_Null_False_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelReader(null, false));
		}

		[Test]
		public static void TestChattelReader_Ctor_Null_Null_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelReader(null, null));
		}

		[Test]
		public static void TestChattelReader_Ctor_Null_Null_False_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ChattelReader(null, null, false));
		}


		[Test]
		public static void TestChattelReader_Ctor_BareCfg_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelReader(config));
		}

		[Test]
		public static void TestChattelReader_Ctor_BareCfg_False_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelReader(config, false));
		}

		[Test]
		public static void TestChattelReader_Ctor_BareCfg_Null_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelReader(config, null));
		}

		[Test]
		public static void TestChattelReader_Ctor_BareCfg_Null_False_DoesntThrow() {
			var config = new ChattelConfiguration(Substitute.For<IAssetServer>());
			Assert.DoesNotThrow(() => new ChattelReader(config, null, false));
		}


		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_Null_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, null);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_Null_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, null, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}


		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_LSTree_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, localStorage);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_LSTree_False_DoesntPurgeLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, localStorage, false);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.True(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_Null_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, null, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		[Test]
		public static void TestChattelReader_Ctor_BasicCfg_LSTree_True_PurgesLocalStorage() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = new AssetStorageSimpleFolderTree(config);
			var assetId = Guid.NewGuid();
			TestAssetStorageSimpleFolderTree.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new StratusAsset {
				Id = assetId,
			});

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
			new ChattelReader(config, localStorage, true);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'

			Assert.False(TestAssetStorageSimpleFolderTree.LocalStorageEntryExists(LOCAL_STORAGE_DIR_INFO, assetId));
		}

		#endregion

		#region HasUpstream

		[Test]
		public static void TestChattelReader_HasUpstream_None_False() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);

			var reader = new ChattelReader(config);

			Assert.False(reader.HasUpstream);
		}

		[Test]
		public static void TestChattelReader_HasUpstream_Mocked_True() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);

			var reader = new ChattelReader(config);

			Assert.True(reader.HasUpstream);
		}

		#endregion

		#region GetAssetAsync(Guid, AssetHandler)

		[Test]
		public static void TestChattelReader_GetAssetAsync2_LocalCacheIsRead() {
			// Simply need to verify that CacheRule.Normal is in effect.

			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {});

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync2_LocalCacheIsWritten() {
			// Simply need to verify that CacheRule.Normal is in effect.

			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {});

			localStorage.Received(1).StoreAsset(asset);
		}


		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_EmptyGuid_CallbackCalled() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var reader = new ChattelReader(config);

			var wait = new AutoResetEvent(false);
			var callbackWasCalled = false;

			reader.GetAssetAsync(Guid.Empty, resultAsset => {
				callbackWasCalled = true;
				wait.Set();
			});

			wait.WaitOne();

			Assert.True(callbackWasCalled);
		}

		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_EmptyGuid_ReturnsNull() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var reader = new ChattelReader(config);

			var wait = new AutoResetEvent(false);

			reader.GetAssetAsync(Guid.Empty, resultAsset => {
				Assert.Null(resultAsset);
				wait.Set();
			});

			wait.WaitOne();
		}

		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_CachedAsset_ReturnsEqualAsset() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			localStorage
				.TryGetAsset(asset.Id, out var junk)
				.Returns(x => {
					x[1] = asset;
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			var wait = new AutoResetEvent(false);

			reader.GetAssetAsync(asset.Id, resultAsset => {
				Assert.AreEqual(asset, resultAsset);
				wait.Set();
			});

			wait.WaitOne();
		}

		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_SingleServer_ReturnsEqualAsset() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config);

			var wait = new AutoResetEvent(false);

			reader.GetAssetAsync(asset.Id, resultAsset => {
				Assert.AreEqual(asset, resultAsset);
				wait.Set();
			});

			wait.WaitOne();
		}

		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_ParallelServer_ReturnsEqualAsset() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(new List<List<IAssetServer>> {
				new List<IAssetServer> { server1, server2 }
			});
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server1.WhenForAnyArgs(x => x.StoreAssetSync(asset)).Do(x => throw new AssetWriteException(asset.Id));

			server2.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config);

			var wait = new AutoResetEvent(false);

			reader.GetAssetAsync(asset.Id, resultAsset => {
				Assert.AreEqual(asset, resultAsset);
				wait.Set();
			});

			wait.WaitOne();
		}

		[Test]
		[Timeout(500)]
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_SerialServer_ReturnsEqualAsset() {
			var server1 = Substitute.For<IAssetServer>();
			var server2 = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(new List<List<IAssetServer>> {
				new List<IAssetServer> { server1 },
				new List<IAssetServer> { server2 }
			});
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server1.WhenForAnyArgs(x => x.StoreAssetSync(asset)).Do(x => throw new AssetWriteException(asset.Id));

			server2.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config);

			var wait = new AutoResetEvent(false);

			reader.GetAssetAsync(asset.Id, resultAsset => {
				Assert.AreEqual(asset, resultAsset);
				wait.Set();
			});

			wait.WaitOne();
		}

		[Test]
		[Timeout(900)] // Must be less than 2x the delay of the server.
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_SingleSlowServer_ParallelReads_ReturnsEqualAssetToBoth() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server
				.RequestAssetSync(asset.Id)
				.Returns(x => {
					Thread.Sleep(500); // Slow server call.
					return asset;
				})
			;

			var reader = new ChattelReader(config);

			StratusAsset asset1 = null;
			StratusAsset asset2 = null;

			Parallel.Invoke(
				() => {
					reader.GetAssetAsync(asset.Id, resultAsset => {
						asset1 = resultAsset;
					});
				},
				() => {
					reader.GetAssetAsync(asset.Id, resultAsset => {
						asset2 = resultAsset;
					});
				}
			);

			Assert.AreEqual(asset, asset1);
			Assert.AreEqual(asset, asset2);
		}

		[Test]
		[Timeout(900)] // Must be less than 2x the delay of the server.
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_SingleSlowServer_ParallelReads_TakeExpectedTime() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server
				.RequestAssetSync(asset.Id)
				.Returns(x => {
					Thread.Sleep(500); // Slow server call.
					return asset;
				})
			;

			var reader = new ChattelReader(config);

			var time1ms = 0L;
			var time2ms = 0L;

			Parallel.Invoke(
				() => {
					var timer = new Stopwatch();
					timer.Restart();
					reader.GetAssetAsync(asset.Id, resultAsset => {
						timer.Stop();
						time1ms = timer.ElapsedMilliseconds;
					});
				},
				() => {
					var timer = new Stopwatch();
					timer.Restart();
					reader.GetAssetAsync(asset.Id, resultAsset => {
						timer.Stop();
						time2ms = timer.ElapsedMilliseconds;
					});
				}
			);

			// Both calls are to take about the same time, as the server is going to take its jolly time getting back to us.
			Assert.Less(time1ms, 600);
			Assert.Greater(time1ms, 490);
			Assert.Less(time2ms, 600);
			Assert.Greater(time2ms, 490);
		}

		[Test]
		[Timeout(900)] // Must be less than 2x the delay of the server.
		public static void TestChattelReader_GetAssetAsync2_UncachedAsset_SingleSlowServer_ParallelReads_SingleServerCall() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(server);
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
				Name = "Avengers",
			};
			server
				.RequestAssetSync(asset.Id)
				.Returns(x => {
					Thread.Sleep(500); // Slow server call.
					return asset;
				})
			;

			var reader = new ChattelReader(config);

			Parallel.Invoke(
				() => {
					reader.GetAssetAsync(asset.Id, resultAsset => { });
				},
				() => {
					reader.GetAssetAsync(asset.Id, resultAsset => { });
				},
				() => {
					reader.GetAssetAsync(asset.Id, resultAsset => { });
				}
			);

			// The server should only be hit once for multiple parallel calls for the same asset ID.
			server.Received(1).RequestAssetSync(asset.Id);
		}

		#endregion

		#region GetAssetAsync(Guid, AssetHandler, CacheRule) Cache Rules with upstream

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleNormal_LocalCacheIsRead() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.Normal);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleNormal_LocalCacheIsWritten() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.Normal);

			localStorage.Received(1).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipRead_LocalCacheIsNotRead() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipRead);

			localStorage.Received(0).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipRead_LocalCacheIsWritten() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipRead);

			localStorage.Received(1).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipWrite_LocalCacheIsRead() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipWrite_LocalCacheIsNotWritten() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(0).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipReadWrite_LocalCacheIsNotRead() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipRead | ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(0).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_WithUpstream_CacheRuleSkipReadWrite_LocalCacheIsNotWritten() {
			var server = Substitute.For<IAssetServer>();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, server);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			server.RequestAssetSync(asset.Id).Returns(asset);

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipRead | ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(0).StoreAsset(asset);
		}

		#endregion

		#region GetAssetAsync(Guid, AssetHandler, CacheRule) Cache Rules without upstream

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleNormal_LocalCacheIsRead() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.Normal);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleNormal_LocalCacheIsNotWritten() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.Normal);

			localStorage.Received(0).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipRead_LocalCacheIsReadAnyway() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipRead);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipRead_LocalCacheIsNotWritten() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipRead);

			localStorage.Received(0).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipWrite_LocalCacheIsRead() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipWrite_LocalCacheIsNotWritten() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(0).StoreAsset(asset);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipReadWrite_LocalCacheIsReadAnyway() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var assetId = Guid.NewGuid();
			localStorage
				.TryGetAsset(assetId, out var junk)
				.Returns(x => {
					x[1] = new StratusAsset();
					return true;
				})
			;

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(assetId, resultAsset => {}, ChattelReader.CacheRule.SkipRead | ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(1).TryGetAsset(assetId, out junk);
		}

		[Test]
		public static void TestChattelReader_GetAssetAsync3_NoUpstream_CacheRuleSkipReadWrite_LocalCacheIsNotWritten() {
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			var localStorage = Substitute.For<IChattelLocalStorage>();
			var asset = new StratusAsset {
				Id = Guid.NewGuid(),
			};

			var reader = new ChattelReader(config, localStorage);

			reader.GetAssetAsync(asset.Id, resultAsset => {}, ChattelReader.CacheRule.SkipRead | ChattelReader.CacheRule.SkipWrite);

			localStorage.Received(0).StoreAsset(asset);
		}

		#endregion
	}
}
