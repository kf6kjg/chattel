// TestLocalRead.cs
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

namespace SpeedTests {
	public class TestLocalRead : ReadTests, ITestSetUp, ITestTearDown, IDisposable {
		private static readonly DirectoryInfo LOCAL_STORAGE_DIR_INFO = new DirectoryInfo(Constants.LOCAL_STORAGE_PATH);

		private readonly uint _dataSize;

		public TestLocalRead(uint assetDataSize) {
			_dataSize = assetDataSize;

			ChattelCleanup.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);

			ChattelCleanup.CreateLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
			var config = new ChattelConfiguration(LOCAL_STORAGE_DIR_INFO.FullName);
			_reader = new ChattelReader(config);
		}

		void ITestSetUp.SetUp() {
			ChattelCleanup.CreateLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
			ChattelCleanup.CreateLocalStorageEntry(LOCAL_STORAGE_DIR_INFO, new InWorldz.Data.Assets.Stratus.StratusAsset {
				Id = _knownAssetId = Guid.NewGuid(),
				Name = "Adama",
				Data = new byte[_dataSize],
			});
		}

		void ITestTearDown.TearDown() {
			ChattelCleanup.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// dispose managed state (managed objects).
					_reader = null;
					ChattelCleanup.CleanLocalStorageFolder(LOCAL_STORAGE_DIR_INFO);
				}

				// free unmanaged resources (unmanaged objects) and override a finalizer below.
				// set large fields to null.

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion
	}
}
