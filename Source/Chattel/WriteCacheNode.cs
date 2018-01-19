// WriteCacheNode.cs
//
// Author:
//       Ricky C <>
//
// Copyright (c) 2018 
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

namespace Chattel {
	internal class WriteCacheNode {
		public const uint BYTE_SIZE = 17;

		public ulong FileOffset { get; private set; }
		public bool IsAvailable { get; set; } // 1 byte
		public Guid AssetId { get; set; } // 16 bytes

		public WriteCacheNode(byte[] bytes, ulong fileOffset) {
			if (bytes == null) {
				throw new ArgumentNullException(nameof(bytes));
			}
			if (bytes.Length < BYTE_SIZE) {
				throw new ArgumentOutOfRangeException(nameof(bytes), $"Must have at least {BYTE_SIZE} bytes!");
			}

			FileOffset = fileOffset;
			IsAvailable = bytes[0] == 0;

			var guidBytes = new byte[16];
			Buffer.BlockCopy(bytes, 1, guidBytes, 0, 16);
			AssetId = new Guid(guidBytes);
		}

		public byte[] ToByteArray() {
			var outBytes = new byte[17];

			outBytes[0] = IsAvailable ? (byte)0 : (byte)1;

			Buffer.BlockCopy(AssetId.ToByteArray(), 0, outBytes, 1, 16);

			return outBytes;
		}
	}
}
