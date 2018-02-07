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
	public static class TestWriteCache {
		private static readonly FileInfo WRITE_CACHE_FILE_INFO = new FileInfo(Constants.WRITE_CACHE_PATH);
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;
		private static readonly byte[] WRITE_CACHE_MAGIC_NUMBER = System.Text.Encoding.ASCII.GetBytes("WHIPLRU1");


		public static void CleanWriteCache(FileInfo wcache) {
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			try {
				wcache.Delete();
			}
			catch {
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			wcache.Refresh();
		}

		public static void CreateWriteCache(FileInfo wcache, IEnumerable<Tuple<Guid, bool>> nodes) {
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


		[OneTimeSetUp]
		public static void Setup() {
			CleanWriteCache(WRITE_CACHE_FILE_INFO);
		}

		[TearDown]
		public static void CleanupAfterEveryTest() {
			CleanWriteCache(WRITE_CACHE_FILE_INFO);
		}

		#region Ctor

		[Test]
		public static void TestWriteCache_Ctor_Minimal_DoesntThrow() {
			Assert.DoesNotThrow(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				2,
				null,
				null
			));
		}

		[Test]
		public static void TestWriteCache_Ctor_NullFile_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new WriteCache(
				null,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));
		}

		[Test]
		public static void TestWriteCache_Ctor_ZeroRecords_ArgumentOutOfRangeException() {
			Assert.Throws<ArgumentOutOfRangeException>(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				0,
				null,
				null
			));
		}


		[Test]
		public static void TestWriteCache_Ctor_CreatesWriteCacheFile() {
			Assert.DoesNotThrow(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));

			FileAssert.Exists(WRITE_CACHE_FILE_INFO.FullName);
		}

		[Test]
		public static void TestWriteCache_Ctor_CreatesWriteCacheFileWithCorrectMagicNumber() {
			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var buffer = new byte[WRITE_CACHE_MAGIC_NUMBER.Length];
			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
				fs.Read(buffer, 0, buffer.Length);
				fs.Close();
			}

			Assert.AreEqual(WRITE_CACHE_MAGIC_NUMBER, buffer);
		}

		[Test]
		public static void TestWriteCache_Ctor_CreatesWriteCacheFileWithCorrectRecordCount() {
			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var dataLength = new FileInfo(WRITE_CACHE_FILE_INFO.FullName).Length - WRITE_CACHE_MAGIC_NUMBER.Length;
			var recordCount = dataLength / WriteCacheNode.BYTE_SIZE;

			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, recordCount);
		}

		[Test]
		public static void TestWriteCache_Ctor_CreatesWriteCacheFileWithRecordsAllAvailable() {
			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_Ctor_UpdatesWriteCacheFileWithCorrectRecordCount() {
			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT / 2,
				null,
				null
			);

			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var dataLength = new FileInfo(WRITE_CACHE_FILE_INFO.FullName).Length - WRITE_CACHE_MAGIC_NUMBER.Length;
			var recordCount = dataLength / WriteCacheNode.BYTE_SIZE;

			Assert.AreEqual(WRITE_CACHE_MAX_RECORD_COUNT, recordCount);
		}

		[Test]
		public static void TestWriteCache_Ctor_UpdatesWriteCacheFileWithRecordsAllAvailable() {
			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT / 2,
				null,
				null
			);

			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_Ctor_ExistingFile_NullWriter_NullCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				null,
				null
			));
		}

		[Test]
		public static void TestWriteCache_Ctor_ExistingFile_NullWriter_MockCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			var localStorage = Substitute.For<IChattelLocalStorage>();

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				null,
				localStorage
			));
		}

		[Test]
		public static void TestWriteCache_Ctor_ExistingFile_MockWriter_NullCache_ChattelConfigurationException() {
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
				new Tuple<Guid, bool>(Guid.NewGuid(), false),
			};
			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			var localStorage = Substitute.For<IChattelLocalStorage>();
			var server = Substitute.For<IAssetServer>();
			var writer = new ChattelWriter(new ChattelConfiguration(server), localStorage, false);

			Assert.Throws<ChattelConfigurationException>(() => new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				writer,
				null
			));
		}

		[Test]
		public static void TestWriteCache_Ctor_ExistingFile_MockWriter_MockLocalStorage_CallsLocalStorageGet() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			var localStorage = Substitute.For<IChattelLocalStorage>();
			var server = Substitute.For<IAssetServer>();
			var writer = new ChattelWriter(new ChattelConfiguration(server), localStorage, false);

			localStorage.TryGetAsset(firstId, out var asset1).Returns(false);

			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				writer,
				localStorage
			);

			localStorage.Received().TryGetAsset(firstId, out var assetJunk1);
			localStorage.DidNotReceive().TryGetAsset(Guid.Empty, out var assetJunk2);
			localStorage.DidNotReceive().TryGetAsset(lastId, out var assetJunk3);
		}

		[Test]
		public static void TestWriteCache_Ctor_ExistingFile_MockWriter_MockLocalStorage_CallsServerStore() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			var localStorage = Substitute.For<IChattelLocalStorage>();
			var server = Substitute.For<IAssetServer>();
			var writer = new ChattelWriter(new ChattelConfiguration(server), localStorage, false);

			var firstAsset = new StratusAsset {
				Id = firstId,
			};

			var lastAsset = new StratusAsset {
				Id = lastId,
			};

			localStorage.TryGetAsset(firstId, out var asset1).Returns(parms => { parms[1] = firstAsset; return true; });
			localStorage.TryGetAsset(lastId, out var asset2).Returns(parms => { parms[1] = lastAsset; return true; });

			localStorage.StoreAsset(firstAsset);
			localStorage.StoreAsset(lastAsset);

			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				writer,
				localStorage
			);

			server.Received().StoreAssetSync(firstAsset);
			server.DidNotReceive().StoreAssetSync(lastAsset);
		}

		[Test]
		public static void TestWriteCache_Ctor_ExistingFile_MockWriter_MockLocalStorage_ClearsWriteCache() {
			var firstId = Guid.NewGuid();
			var lastId = Guid.NewGuid();
			var records = new Tuple<Guid, bool>[] {
				new Tuple<Guid, bool>(firstId, false),
				new Tuple<Guid, bool>(Guid.Empty, true),
				new Tuple<Guid, bool>(lastId, true),
			};

			CreateWriteCache(WRITE_CACHE_FILE_INFO, records);

			var localStorage = Substitute.For<IChattelLocalStorage>();
			var server = Substitute.For<IAssetServer>();
			var writer = new ChattelWriter(new ChattelConfiguration(new List<List<IAssetServer>> { new List<IAssetServer> { server } }), localStorage, false);

			var firstAsset = new StratusAsset {
				Id = firstId,
			};

			var lastAsset = new StratusAsset {
				Id = lastId,
			};

			localStorage.TryGetAsset(firstId, out var asset1).Returns(parms => { parms[1] = firstAsset; return true; });
			localStorage.TryGetAsset(lastId, out var asset2).Returns(parms => { parms[1] = lastAsset; return true; });

			localStorage.StoreAsset(firstAsset);
			localStorage.StoreAsset(lastAsset);

			new WriteCache(
				WRITE_CACHE_FILE_INFO,
				(uint)records.Length,
				writer,
				localStorage
			);

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_ClearNode_Null_ArgumentNullException() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.Throws<ArgumentNullException>(() => wc.ClearNode(null));
		}

		[Test]
		public static void TestWriteCache_ClearNode_DoesntThrow() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
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
		public static void TestWriteCache_ClearNode_SetFileByteCorrectly() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var node = wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			});

			wc.ClearNode(node);

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_ClearNode_LeftGuidIntact() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var id = Guid.NewGuid();

			var node = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			wc.ClearNode(node);

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_WriteNode_Null_ArgumentNullException() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.Throws<ArgumentNullException>(() => wc.WriteNode(null));
		}

		[Test]
		public static void TestWriteCache_WriteNode_DoesntThrow() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			Assert.DoesNotThrow(() => wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			}));
		}


		[Test]
		public static void TestWriteCache_WriteNode_SetFileByteCorrectly() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var node = wc.WriteNode(new StratusAsset {
				Id = Guid.NewGuid(),
			});

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_WriteNode_WroteCorrectGuid() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			);

			var id = Guid.NewGuid();

			var node = wc.WriteNode(new StratusAsset {
				Id = id,
			});

			using (var fs = new FileStream(WRITE_CACHE_FILE_INFO.FullName, FileMode.Open, FileAccess.Read)) {
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
		public static void TestWriteCache_WriteNode_TwiceDoesntReturnSameNode() {
			var wc = new WriteCache(
				WRITE_CACHE_FILE_INFO,
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
