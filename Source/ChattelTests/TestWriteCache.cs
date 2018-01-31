// TestWriteCache.cs
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
using System.Linq;
using Chattel;
using InWorldz.Data.Assets.Stratus;
using NSubstitute;
using NUnit.Framework;

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'

namespace ChattelTests {
	[TestFixture]
	public class TestWriteCache {
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;
		private static readonly byte[] WRITE_CACHE_MAGIC_NUMBER = System.Text.Encoding.ASCII.GetBytes("WHIPLRU1");

		public static readonly FileInfo WriteCacheFileInfo = new FileInfo(Constants.WRITE_CACHE_PATH);

		public static void CleanWriteCache() {
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			try {
				WriteCacheFileInfo.Delete();
			}
			catch {
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			WriteCacheFileInfo.Refresh();
		}

		internal static void CreateWriteCache(FileInfo wcache, IEnumerable<Tuple<Guid, bool>> nodes) {
			using (var fs = new FileStream(wcache.FullName, FileMode.Create, FileAccess.Write)) {
				try {
					fs.SetLength(WRITE_CACHE_MAGIC_NUMBER.Length + 2 * WriteCacheNode.BYTE_SIZE);
					fs.Seek(0, SeekOrigin.Begin);

					// Write the header
					fs.Write(WRITE_CACHE_MAGIC_NUMBER, 0, WRITE_CACHE_MAGIC_NUMBER.Length);

					// Write rows.
					foreach (var node in nodes) {
						fs.WriteByte(node.Item2 ? (byte)0 : (byte)1);
						fs.Write(node.Item1.ToByteArray(), 0, 16);
					}
				}
				finally {
					fs.Close();
				}
			}

			wcache.Refresh();
		}

		public class MockServerConfig : IAssetServerConfig {
			public Type Type => typeof(MockServer);

			public string Name => "MockServer";
		}

		public class MockServer : IAssetServer {
			public MockServerConfig Config { get; }

			public MockServer(MockServerConfig config) {
				Config = config;
			}

			public StratusAsset RequestAssetSync(Guid assetID) {
				return null;
			}

			public void StoreAssetSync(StratusAsset asset) {
				// It's a mock, the data can go visit Mars for all we care.
			}

			void IDisposable.Dispose() {
				// Likewise, nothing to see here, move along.
			}
		}

		[SetUp]
		public void BeforeEveryTest() {
			CleanWriteCache();
		}

		[TearDown]
		public void CleanupAfterEveryTest() {
			CleanWriteCache();
		}

		#region Ctor

		[Test]
		public void TestWriteCache_Ctor_Minimal_DoesntThrow() {
			Assert.DoesNotThrow(() => new WriteCache(
				WriteCacheFileInfo,
				2,
				null,
				null
			));
		}

		[Test]
		public void TestWriteCache_Ctor_NullFile_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new WriteCache(
				null,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));
		}

		[Test]
		public void TestWriteCache_Ctor_ZeroRecords_ArgumentOutOfRangeException() {
			Assert.Throws<ArgumentOutOfRangeException>(() => new WriteCache(
				WriteCacheFileInfo,
				0,
				null,
				null
			));
		}


		[Test]
		public void TestWriteCache_Ctor_CreatesWriteCacheFile() {
			Assert.DoesNotThrow(() => new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));

			FileAssert.Exists(WriteCacheFileInfo.FullName);
		}

		[Test]
		public void TestWriteCache_Ctor_CreatesWriteCacheFileWithCorrectMagicNumber() {
			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var buffer = new byte[WRITE_CACHE_MAGIC_NUMBER.Length];
			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				fs.Read(buffer, 0, buffer.Length);
				fs.Close();
			}

			Assert.AreEqual(WRITE_CACHE_MAGIC_NUMBER, buffer);
		}

		[Test]
		public void TestWriteCache_Ctor_CreatesWriteCacheFileWithCorrectRecordCount() {
			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var dataLength = new FileInfo(WriteCacheFileInfo.FullName).Length - WRITE_CACHE_MAGIC_NUMBER.Length;
			var recordCount = dataLength / WriteCacheNode.BYTE_SIZE;

			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, recordCount);
		}

		[Test]
		public void TestWriteCache_Ctor_CreatesWriteCacheFileWithRecordsAllAvailable() {
			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					// Skip the header
					fs.Seek(WRITE_CACHE_MAGIC_NUMBER.Length, SeekOrigin.Begin);

					// Check each row.
					for (var recordIndex = 0; recordIndex < WRITE_CACHE_MAX_RECORD_COUNT; ++recordIndex) {
						var buffer = new byte[WriteCacheNode.BYTE_SIZE];
						fs.Read(buffer, 0, buffer.Length);
						Assert.AreEqual(0, buffer[0], $"Record #{recordIndex + 1} is not marked as available!");
					}
				}
				finally {
					fs.Close();
				}
			}
		}

		[Test]
		public void TestWriteCache_Ctor_UpdatesWriteCacheFileWithCorrectRecordCount() {
			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT / 2,
				null,
				null
			);

			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var dataLength = new FileInfo(WriteCacheFileInfo.FullName).Length - WRITE_CACHE_MAGIC_NUMBER.Length;
			var recordCount = dataLength / WriteCacheNode.BYTE_SIZE;

			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, recordCount);
		}

		[Test]
		public void TestWriteCache_Ctor_UpdatesWriteCacheFileWithRecordsAllAvailable() {
			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT / 2,
				null,
				null
			);

			new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					// Skip the header
					fs.Seek(WRITE_CACHE_MAGIC_NUMBER.Length, SeekOrigin.Begin);

					// Check each row.
					for (var recordIndex = 0; recordIndex < WRITE_CACHE_MAX_RECORD_COUNT; ++recordIndex) {
						var buffer = new byte[WriteCacheNode.BYTE_SIZE];
						fs.Read(buffer, 0, buffer.Length);
						Assert.AreEqual(0, buffer[0], $"Record #{recordIndex + 1} is not marked as available!");
					}
				}
				finally {
					fs.Close();
				}
			}
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_NullWriter_NullCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WriteCacheFileInfo, records);

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				null,
				null
			));
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_NullWriter_MockCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WriteCacheFileInfo, records);

			var cache = Substitute.For<IChattelCache>();

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				null,
				cache
			));
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_MockWriter_NullCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WriteCacheFileInfo, records);

			var cache = Substitute.For<IChattelCache>();
			var writer = Substitute.For<ChattelWriter>(new ChattelConfiguration(), cache, false);

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				writer,
				null
			));
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_MockWriter_MockCache_CallsCacheGet() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WriteCacheFileInfo, records);

			var cache = Substitute.For<IChattelCache>();
			var writer = Substitute.For<ChattelWriter>(new ChattelConfiguration(serialParallelServerConfigs: new List<List<IAssetServerConfig>> { new List<IAssetServerConfig> { new MockServerConfig() } }), cache, false);

			cache.TryGetCachedAsset(firstId, out var asset1).Returns(false);

			new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				writer,
				cache
			);

			cache.Received().TryGetCachedAsset(firstId, out var assetJunk1);
			cache.DidNotReceive().TryGetCachedAsset(Guid.Empty, out var assetJunk2);
			cache.DidNotReceive().TryGetCachedAsset(lastId, out var assetJunk3);
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_MockWriter_MockCache_CallsWriterPut() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WriteCacheFileInfo, records);

			var cache = Substitute.For<IChattelCache>();
			var writer = Substitute.For<ChattelWriter>(new ChattelConfiguration(serialParallelServerConfigs: new List<List<IAssetServerConfig>> { new List<IAssetServerConfig> { new MockServerConfig() } }), cache, false);

			var firstAsset = new StratusAsset {
				Id = firstId,
			};

			var lastAsset = new StratusAsset {
				Id = lastId,
			};

			cache.TryGetCachedAsset(firstId, out var asset1).Returns(parms => { parms[1] = firstAsset; return true; });
			cache.TryGetCachedAsset(lastId, out var asset2).Returns(parms => { parms[1] = lastAsset; return true; });

			cache.CacheAsset(firstAsset);
			cache.CacheAsset(lastAsset);

			new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				writer,
				cache
			);

			writer.Received().PutAssetSync(firstAsset);
			writer.DidNotReceive().PutAssetSync(lastAsset);
		}

		[Test]
		public void TestWriteCache_Ctor_ExistingFile_MockWriter_MockCache_ClearsWriteCache() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WriteCacheFileInfo, records);

			var cache = Substitute.For<IChattelCache>();
			var writer = Substitute.For<ChattelWriter>(new ChattelConfiguration(serialParallelServerConfigs: new List<List<IAssetServerConfig>> { new List<IAssetServerConfig> { new MockServerConfig() } }), cache, false);

			var firstAsset = new StratusAsset {
				Id = firstId,
			};

			var lastAsset = new StratusAsset {
				Id = lastId,
			};

			cache.TryGetCachedAsset(firstId, out var asset1).Returns(parms => { parms[1] = firstAsset; return true; });
			cache.TryGetCachedAsset(lastId, out var asset2).Returns(parms => { parms[1] = lastAsset; return true; });

			cache.CacheAsset(firstAsset);
			cache.CacheAsset(lastAsset);

			new WriteCache(
				WriteCacheFileInfo,
				(uint)records.Length,
				writer,
				cache
			);

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					// Skip the header
					fs.Seek(WRITE_CACHE_MAGIC_NUMBER.Length, SeekOrigin.Begin);

					// Check each row.
					for (var recordIndex = 0; recordIndex < WRITE_CACHE_MAX_RECORD_COUNT; ++recordIndex) {
						var buffer = new byte[WriteCacheNode.BYTE_SIZE];
						fs.Read(buffer, 0, buffer.Length);
						Assert.AreEqual(0, buffer[0], $"Record #{recordIndex + 1} is not marked as available!");
					}
				}
				finally {
					fs.Close();
				}
			}
		}

		#endregion

		#region ClearNode

		[Test]
		public void TestWriteCache_ClearNode_Null_ArgumentNullException() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.Throws<ArgumentNullException>(() => wc.ClearNode(null));
		}

		[Test]
		public void TestWriteCache_ClearNode_DoesntThrow() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var node = wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			});

			Assert.DoesNotThrow(() => wc.ClearNode(node));
		}

		[Test]
		public void TestWriteCache_ClearNode_SetFileByteCorrectly() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var node = wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			});

			wc.ClearNode(node);

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					fs.Seek((long)node.FileOffset, SeekOrigin.Begin);

					// Check each row.
					var buffer = new byte[1];
					fs.Read(buffer, 0, buffer.Length);
					Assert.AreEqual(0, buffer[0]);
				}
				finally {
					fs.Close();
				}
			}
		}

		[Test]
		public void TestWriteCache_ClearNode_LeftGuidIntact() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var id = Guid.NewGuid();

			var node = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			wc.ClearNode(node);

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					fs.Seek((long)node.FileOffset, SeekOrigin.Begin);

					// Check each row.
					var buffer = new byte[WriteCacheNode.BYTE_SIZE];
					fs.Read(buffer, 0, buffer.Length);
					Assert.AreEqual(id.ToByteArray(), buffer.Skip(1));
				}
				finally {
					fs.Close();
				}
			}
		}

		#endregion

		#region WriteNode

		[Test]
		public void TestWriteCache_WriteNode_Null_ArgumentNullException() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.Throws<ArgumentNullException>(() => wc.WriteNode(null));
		}

		[Test]
		public void TestWriteCache_WriteNode_DoesntThrow() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.DoesNotThrow(() => wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			}));
		}


		[Test]
		public void TestWriteCache_WriteNode_SetFileByteCorrectly() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var node = wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			});

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					fs.Seek((long)node.FileOffset, SeekOrigin.Begin);

					// Check each row.
					var buffer = new byte[1];
					fs.Read(buffer, 0, buffer.Length);
					Assert.AreEqual(1, buffer[0]);
				}
				finally {
					fs.Close();
				}
			}
		}

		[Test]
		public void TestWriteCache_WriteNode_WroteCorrectGuid() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var id = Guid.NewGuid();

			var node = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			using (var fs = new FileStream(WriteCacheFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
				try {
					fs.Seek((long)node.FileOffset, SeekOrigin.Begin);

					// Check each row.
					var buffer = new byte[WriteCacheNode.BYTE_SIZE];
					fs.Read(buffer, 0, buffer.Length);
					Assert.AreEqual(id.ToByteArray(), buffer.Skip(1));
				}
				finally {
					fs.Close();
				}
			}
		}

		[Test]
		public void TestWriteCache_WriteNode_TwiceDoesntReturnSameNode() {
			var wc = new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var id = Guid.NewGuid();

			var node1 = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			var node2 = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			Assert.AreNotSame(node1, node2);
		}

		#endregion
	}
}
