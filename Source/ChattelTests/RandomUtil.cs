// RandomUtil.cs
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
using System.Text;

namespace ChattelTests {
	public static class RandomUtil {
		public static readonly Random Rnd = new Random();

		public static bool NextBool() {
			return Rnd.Next() > (int.MaxValue / 2);
		}

		public static byte NextByte() {
			var data = new byte[1];
			Rnd.NextBytes(data);
			return data[0];
		}

		public static sbyte NextSByte() {
			var data = new byte[1];
			Rnd.NextBytes(data);
			return (sbyte)data[0];
		}

		public static uint NextUInt() {
			var data = new byte[sizeof(uint)];
			Rnd.NextBytes(data);
			return BitConverter.ToUInt32(data, 0);
		}

		public static ulong NextULong() {
			var data = new byte[sizeof(ulong)];
			Rnd.NextBytes(data);
			return BitConverter.ToUInt64(data, 0);
		}

		public static string StringUTF8(uint length) {
			var data = new byte[sizeof(uint) * length];
			Rnd.NextBytes(data);
			return Encoding.UTF8.GetString(data).Substring(0, (int)length);
		}
	}
}
