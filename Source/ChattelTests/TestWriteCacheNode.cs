// TestWriteCacheNode.cs
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
using Chattel;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public class TestWriteCacheNode {
		[Test]
		public void TestThrowsOnNewWithNullArray() {
			Assert.Throws<ArgumentNullException>(() => new WriteCacheNode(null, 0));
		}

		[Test]
		public void TestThrowsOnNewWithTooShortArray() {
			var buffer = new byte[0];
			Assert.Throws<ArgumentOutOfRangeException>(() => new WriteCacheNode(buffer, 0));
		}

		[Test]
		public void TestAcceptsNewWithExtraLongArray() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE * 2];
			Assert.DoesNotThrow(() => new WriteCacheNode(buffer, 0));
		}

		[Test]
		public void TestFileOffsetCorrect() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			var offset = ulong.MaxValue;
			var node = new WriteCacheNode(buffer, offset);
			Assert.AreEqual(offset, node.FileOffset);
		}

		[Test]
		public void TestIsAvailableCorrectTrue() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			buffer[0] = 0;
			var node = new WriteCacheNode(buffer, 0);
			Assert.AreEqual(true, node.IsAvailable);
		}

		[Test]
		public void TestIsAvailableCorrectFalse1() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			buffer[0] = 1;
			var node = new WriteCacheNode(buffer, 0);
			Assert.AreEqual(false, node.IsAvailable);
		}

		[Test]
		public void TestIsAvailableCorrectFalse255() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			buffer[0] = 255;
			var node = new WriteCacheNode(buffer, 0);
			Assert.AreEqual(false, node.IsAvailable);
		}

		[Test]
		public void TestAssetIdCorrectZeros() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			var node = new WriteCacheNode(buffer, 0);
			Assert.AreEqual(Guid.Empty, node.AssetId);
		}

		[Test]
		public void TestAssetIdCorrectRandom() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];
			var guid = Guid.NewGuid();
			Buffer.BlockCopy(guid.ToByteArray(), 0, buffer, 1, 16);
			var node = new WriteCacheNode(buffer, 0);
			Assert.AreEqual(guid, node.AssetId);
		}

		[Test]
		public void TestToByteArrayCorrectRandom() {
			var buffer = new byte[WriteCacheNode.BYTE_SIZE];

			var guid = Guid.NewGuid();
			var offset = RandomUtil.NextULong();

			buffer[0] = RandomUtil.NextBool() ? (byte)0 : (byte)1;
			Buffer.BlockCopy(guid.ToByteArray(), 0, buffer, 1, 16);

			var node = new WriteCacheNode(buffer, offset);
			Assert.AreEqual(buffer, node.ToByteArray());
		}
	}
}
