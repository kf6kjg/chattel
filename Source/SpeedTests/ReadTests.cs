// ReadTests.cs
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
using System.Diagnostics;
using System.Threading;
using Chattel;
using InWorldz.Data.Assets.Stratus;

namespace SpeedTests {
	public class ReadTests {
		private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private const int TIMEOUT_MS = 100;

		protected ChattelReader _reader;
		protected Guid _knownAssetId;

		protected void TestAssetKnownExists() {
			StratusAsset asset = null;
			var stopWatch = new Stopwatch();

			stopWatch.Reset();
			using (var wait = new AutoResetEvent(false)) {
				_reader.GetAssetAsync(_knownAssetId, (a) => {
					asset = a;
					wait.Set();
				});

				wait.WaitOne(TIMEOUT_MS);
			}
			stopWatch.Stop();

			if (asset == null) {
				if (stopWatch.ElapsedMilliseconds >= 100) {
					throw new TestFailedException("Asset fetch timeout.");
				}

				throw new TestFailedException("Asset not found");
			}
		}
	}
}
