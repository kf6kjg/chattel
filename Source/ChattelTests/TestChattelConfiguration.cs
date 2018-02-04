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
using System.Collections.Generic;
using System.IO;
using Chattel;
using Nini.Config;
using NSubstitute;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public static class TestChattelConfiguration {
		private static readonly DirectoryInfo CACHE_DIR_INFO = new DirectoryInfo(Constants.CACHE_PATH);
		private static readonly FileInfo WRITE_CACHE_FILE_INFO = new FileInfo(Constants.WRITE_CACHE_PATH);
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;

		[OneTimeSetUp]
		public static void Setup() {
			TestAssetCacheSimpleFolderTree.CleanCacheFolder(CACHE_DIR_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			TestAssetCacheSimpleFolderTree.CleanCacheFolder(CACHE_DIR_INFO);
		}

		#region Ctor - Direct

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullCachePath_WriteCacheFileNull() {
			var config = new ChattelConfiguration(null, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT);
			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullCachePath_CacheFolderNull() {
			var config = new ChattelConfiguration((string)null);
			Assert.IsNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullCachePath_CacheEnabledFalse() {
			var config = new ChattelConfiguration((string)null);
			Assert.False(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadCachePath_WriteCacheFileNull() {
			var config = new ChattelConfiguration("asfd", WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT);
			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadCachePath_CacheFolderNull() {
			var config = new ChattelConfiguration("asfd");
			Assert.IsNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_BadCachePath_CacheEnabledFalse() {
			var config = new ChattelConfiguration("asfd");
			Assert.False(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodCachePath_WriteCacheFileNotNull() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT);
			Assert.IsNotNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodCachePath_WriteCacheRecordCount_Correct() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, WRITE_CACHE_MAX_RECORD_COUNT);
			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, config.WriteCacheRecordCount);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodCachePath_CacheFolderNotNull() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			Assert.IsNotNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_GoodCachePath_CacheEnabledTrue() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			Assert.True(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorDirect_NullWriteCachePath_WriteCacheFileNull() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName, null, WRITE_CACHE_MAX_RECORD_COUNT);
			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ZeroWriteCacheRecordCount_WriteCacheFileNull() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName, WRITE_CACHE_FILE_INFO.FullName, 0);
			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ServerListIsIntact() {
			var serverOrig = Substitute.For<AssetServer>();

			var assetServers = new List<List<AssetServer>> {
				new List<AssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			Assert.AreEqual(assetServers, config.SerialParallelAssetServers);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ModyingSerialListDoesntChangeInternal() {
			var serverOrig = Substitute.For<AssetServer>();
			var serverNew = Substitute.For<AssetServer>();

			var assetServers = new List<List<AssetServer>> {
				new List<AssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			assetServers.Add(new List<AssetServer>{
				serverNew
			});

			Assert.AreNotEqual(assetServers, config.SerialParallelAssetServers);
		}

		[Test]
		public static void TestChattelConfiguration_CtorDirect_ModyingParallelListDoesntChangeInternal() {
			var serverOrig = Substitute.For<AssetServer>();
			var serverNew = Substitute.For<AssetServer>();

			var assetServers = new List<List<AssetServer>> {
				new List<AssetServer> {
					serverOrig
				}
			};

			var config = new ChattelConfiguration(assetServers);

			assetServers[0].Add(serverNew);

			Assert.AreNotEqual(assetServers, config.SerialParallelAssetServers);
		}

		#endregion

		#region Ctor - Nini

		[Test]
		public static void TestChattelConfiguration_CtorNini_DoesntThrow() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];

			TestAssetCacheSimpleFolderTree.CreateCacheFolder(CACHE_DIR_INFO);

			Assert.DoesNotThrow(() => new ChattelConfiguration(iniConfigSource, iniConfig));
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_NullConfigSource_ArgumentNullException() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];

			Assert.Throws<ArgumentNullException>(() => new ChattelConfiguration(null, iniConfig));
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_NullAssetConfig_DoesntThrow() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);

			Assert.DoesNotThrow(() => new ChattelConfiguration(iniConfigSource, null));
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_EmptyCachePath_WriteCacheFileNull() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", string.Empty);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_EmptyCachePath_CacheFolderNull() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", string.Empty);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_EmptyCachePath_CacheEnabledFalse() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", string.Empty);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.False(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorNini_BadCachePath_WriteCacheFileNull() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", "asfd");
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_BadCachePath_CacheFolderNull() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", "asfd");
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_BadCachePath_CacheEnabledFalse() {
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", "asfd");
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.False(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorNini_GoodCachePath_WriteCacheFileNotNull() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			iniConfig.Set("WriteCacheFilePath", WRITE_CACHE_FILE_INFO.FullName);
			iniConfig.Set("WriteCacheRecordCount", WRITE_CACHE_MAX_RECORD_COUNT);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNotNull(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_GoodCachePath_WriteCacheRecordCount_Correct() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			iniConfig.Set("WriteCacheFilePath", WRITE_CACHE_FILE_INFO.FullName);
			iniConfig.Set("WriteCacheRecordCount", WRITE_CACHE_MAX_RECORD_COUNT);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, config.WriteCacheRecordCount);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_GoodCachePath_CacheFolderNotNull() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.IsNotNull(config.CacheFolder);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_GoodCachePath_CacheEnabledTrue() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.True(config.CacheEnabled);
		}


		[Test]
		public static void TestChattelConfiguration_CtorNini_EmptyWriteCachePath_WriteCacheFileNull() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			iniConfig.Set("WriteCacheFilePath", string.Empty);
			iniConfig.Set("WriteCacheRecordCount", WRITE_CACHE_MAX_RECORD_COUNT);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_ZeroWriteCacheRecordCount_WriteCacheFileNull() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			iniConfig.Set("WriteCacheFilePath", WRITE_CACHE_FILE_INFO.FullName);
			iniConfig.Set("WriteCacheRecordCount", 0);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.Null(config.WriteCacheFile);
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_NegativeWriteCacheRecordCount_WriteCacheFileNull() {
			CACHE_DIR_INFO.Create();
			var iniConfigSource = new IniConfigSource(Constants.INI_FILE_PATH);
			var iniConfig = iniConfigSource.Configs["AssetsFake"];
			iniConfig.Set("CachePath", CACHE_DIR_INFO.FullName);
			iniConfig.Set("WriteCacheFilePath", WRITE_CACHE_FILE_INFO.FullName);
			iniConfig.Set("WriteCacheRecordCount", -1);
			var config = new ChattelConfiguration(iniConfigSource, iniConfig);

			Assert.Null(config.WriteCacheFile);
		}

		// TODO: develop a way for a faked server class to register into the INI confg system and then finish the following tests.

		[Test]
		public static void TestChattelConfiguration_CtorNini_ServerListIsIntact() {
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_ModyingSerialListDoesntChangeInternal() {
		}

		[Test]
		public static void TestChattelConfiguration_CtorNini_ModyingParallelListDoesntChangeInternal() {
		}

		#endregion

		#region CacheEnabled

		// Tested in the ctor section as it's not really easily separated.

		#endregion

		#region DisableCache

		[Test]
		public static void TestChattelConfiguration_DisableCache_CacheEnabled_False() {
			CACHE_DIR_INFO.Create();
			var config = new ChattelConfiguration(CACHE_DIR_INFO.FullName);
			config.DisableCache();
			Assert.False(config.CacheEnabled);
		}

		#endregion
	}
}
