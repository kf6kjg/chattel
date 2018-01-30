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
using System.IO;
using Chattel;
using NUnit.Framework;

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'

namespace ChattelTests {
	[TestFixture]
	public class TestWriteCache {
		private const uint WRITE_CACHE_MAX_RECORD_COUNT = 16;
		private readonly byte[] WRITE_CACHE_MAGIC_NUMBER = System.Text.Encoding.ASCII.GetBytes("WHIPLRU1");

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
		public void TestCtorMinimal_DoesntThrow() {
			Assert.DoesNotThrow(() => new WriteCache(
				WriteCacheFileInfo,
				2,
				null,
				null
			));
		}

		[Test]
		public void TestCtorNullFile_ArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new WriteCache(
				null,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));
		}

		[Test]
		public void TestCtorZeroRecords_ArgumentOutOfRangeException() {
			Assert.Throws<ArgumentOutOfRangeException>(() => new WriteCache(
				WriteCacheFileInfo,
				0,
				null,
				null
			));
		}


		[Test]
		public void TestCtor_CreatesWriteCacheFile() {
			Assert.DoesNotThrow(() => new WriteCache(
				WriteCacheFileInfo,
				WRITE_CACHE_MAX_RECORD_COUNT,
				null,
				null
			));

			FileAssert.Exists(WriteCacheFileInfo.FullName);
		}

		[Test]
		public void TestCtor_CreatesWriteCacheFileWithCorrectMagicNumber() {
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
		public void TestCtor_CreatesWriteCacheFileWithCorrectRecordCount() {
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
		public void TestCtor_CreatesWriteCacheFileWithRecordsAllAvailable() {
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
		public void TestCtor_UpdatesWriteCacheFileWithCorrectRecordCount() {
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
		public void TestCtor_UpdatesWriteCacheFileWithRecordsAllAvailable() {
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

		// TODO: test for cases where the write cache has pending entries.  Should use a mocked ChattelWriter and IChattelCache.

		#endregion
	}
}
