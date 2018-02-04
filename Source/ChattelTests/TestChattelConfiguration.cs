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

using System.Collections.Generic;
using System.IO;
using Chattel;
using NSubstitute;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestChattelConfiguration {
		private static readonly DirectoryInfo LOCAL_STORAGE_DIR_INFO = new DirectoryInfo(Constants.LOCAL_STORAGE_PATH);
		private static readonly FileInfo WRITE_CACHE_FILE_INFO = new FileInfo(Constants.WRITE_CACHE_PATH);
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;

		private static readonly List<List<IAssetServer>> ASSET_SERVERS = new List<List<IAssetServer>> {
			new List<IAssetServer> {
				Substitute.For<IAssetServer>()
			}
		};


		[OneTimeSetUp]
		public static void Setup() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			TestAssetStorageSimpleFolderTree.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		#region Ctor - Direct

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullLocalStoragePath_WriteCacheFileNull() {
			var config = new ChattelConfiguration(null, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT, ASSET_SERVERS);
			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullLocalStoragePath_LocalStorageFolderNull() {
			var config = new ChattelConfiguration((string)null);
			Assert.IsNull(config.LocalStorageFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullLocalStoragePath_LocalStorageEnabledFalse() {
			var config = new ChattelConfiguration((string)null);
			Assert.False(config.LocalStorageEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadLocalStoragePath_WriteCacheFileNull() {
			var config = new ChattelConfiguration("asfd", WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT, ASSET_SERVERS);
			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadLocalStoragePath_LocalStorageFolderNull() {
			var config = new ChattelConfiguration("asfd");
			Assert.IsNull(config.LocalStorageFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadLocalStoragePath_LocalStorageEnabledFalse() {
			var config = new ChattelConfiguration("asfd");
			Assert.False(config.LocalStorageEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodLocalStoragePath_WriteCacheFileNotNull() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT, ASSET_SERVERS);
			Assert.IsNotNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodLocalStoragePath_WriteCacheRecordCount_Correct() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT, ASSET_SERVERS);
			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, config.WriteCacheRecordCount);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodLocalStoragePath_LocalStorageFolderNotNull() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			Assert.IsNotNull(config.LocalStorageFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodLocalStoragePath_LocalStorageEnabledTrue() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			Assert.True(config.LocalStorageEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullWriteCachePath_WriteCacheFileNull() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, null, WRITE_CACHE_MAX_RECORD_COUNT, ASSET_SERVERS);
			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ZeroWriteCacheRecordCount_WriteCacheFileNull() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 0, ASSET_SERVERS);
			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ServerListIsIntact() {
			var serverOrig = Substitute.For<IAssetServer>();

			var assetServers = new List<List<IAssetServer>> {
				new List<IAssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			Assert.AreEqual(assetServers, config.SerialParallelAssetServers);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ModyingSerialListDoesntChangeInternal() {
			var serverOrig = Substitute.For<IAssetServer>();
			var serverNew = Substitute.For<IAssetServer>();

			var assetServers = new List<List<IAssetServer>> {
				new List<IAssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			assetServers.Add(new List<IAssetServer>{
				serverNew
			});

			Assert.AreNotEqual(assetServers, config.SerialParallelAssetServers);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ModyingParallelListDoesntChangeInternal() {
			var serverOrig = Substitute.For<IAssetServer>();
			var serverNew = Substitute.For<IAssetServer>();

			var assetServers = new List<List<IAssetServer>> {
				new List<IAssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			assetServers[0].Add(serverNew);

			Assert.AreNotEqual(assetServers, config.SerialParallelAssetServers);
		}

		#endregion

		#region LocalStorageEnabled

		// Tested in the ctor section as its not really easily separated.

		#endregion

		#region DisableLocalStorage

		[Test]
		public static void TestChattelConfiguration_DisableLocalStorage_LocalStorageEnabled_False() {
			LOCAL_STORAGE_DIR_INFO.Create();
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			config.DisableLocalStorage();
			Assert.False(config.LocalStorageEnabled);
		}

		#endregion
	}
}
