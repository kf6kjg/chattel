// WriteCache.cs
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
using System.IO.MemoryMappedFiles;
using System.Linq;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	/// <summary>
	/// Storage for asset IDs that are WIP for remote storage.
	/// </summary>
	internal class WriteCache {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelWriter>();

		private static readonly byte[] WRITE_CACHE_MAGIC_NUMBER = System.Text.Encoding.ASCII.GetBytes("WHIPLRU1"); // This is a remnant from the history of this file's origins.

		private readonly FileInfo _fileInfo;

		private readonly WriteCacheNode[] _writeCacheNodes;
		private readonly object _writeCacheNodeLock = new object();
		private WriteCacheNode _nextAvailableWriteCacheNode;

		/// <summary>
		/// Opens or creates the write cache file. If there are entries in the file that are marked as not uploaded, then
		/// this ctor loads those assets from the local storage and uploads them to the remotes passed in via the ChattelWriter instance.
		/// </summary>
		/// <param name="fileInfo">FileInfo instance for the path where to load or create the write cache file.</param>
		/// <param name="recordCount">Record count to set the write cache to.</param>
		/// <param name="writer">ChattelWriter instance for uploading un-finished assets to on load.</param>
		/// <param name="localStorage">Local storage instace to load unfinished assets from.</param>
		/// <exception cref="T:Chattel.ChattelConfigurationException">Thrown if there are assets marked as needing to be uploaded but the current configuration prevents uploading.</exception>
		public WriteCache(FileInfo fileInfo, uint recordCount, ChattelWriter writer, IChattelLocalStorage localStorage) {
			_fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
			if (recordCount < 2) {
				throw new ArgumentOutOfRangeException(nameof(recordCount), "Having less than two record makes no sense and causes errors.");
			}

			// If the file doesn't exist, create it and zero the needed records.
			if (!_fileInfo.Exists) {
				LOG.Log(Logging.LogLevel.Info, () => $"Write cache file doesn't exist, creating and formatting file '{_fileInfo.FullName}'");
				Initialize(recordCount);
				_fileInfo.Refresh();
				LOG.Log(Logging.LogLevel.Debug, () => $"Write cache formatting complete.");
			}

			var writeCacheFileRecordCount = (uint)((_fileInfo.Length - WRITE_CACHE_MAGIC_NUMBER.Length) / WriteCacheNode.BYTE_SIZE);

			if (writeCacheFileRecordCount < recordCount) {
				// Expand the file.
				Expand(recordCount - writeCacheFileRecordCount);
			}
			else if (writeCacheFileRecordCount > recordCount) {
				// For now, use the file size.
				LOG.Log(Logging.LogLevel.Warn, () => $"Write cache not able to be shrunk in this version of Chattel, continuing with old value of {writeCacheFileRecordCount} records instead of requested {recordCount} records.");
				recordCount = writeCacheFileRecordCount;
				// TODO: find a way to shrink the file without losing ANY of the records that have not yet been submitted to an upstream server.
				// Could get difficult in the case of a full file...
			}

			LOG.Log(Logging.LogLevel.Info, () => $"Reading write cache from file '{_fileInfo.FullName}'. Expecting {recordCount} records, found {writeCacheFileRecordCount} records, choosing the larger.");
			_writeCacheNodes = Read(out IEnumerable<WriteCacheNode> assetsToBeSentUpstream).ToArray();
			LOG.Log(Logging.LogLevel.Debug, () => $"Reading write cache complete.");

			if (assetsToBeSentUpstream.Any()) {
				if (writer == null) {
					throw new ChattelConfigurationException("Write cache indicates assets needing to be sent to remote servers, but there is no asset writer!");
				}

				if (localStorage == null) {
					throw new ChattelConfigurationException("Write cache indicates assets needing to be sent to remote servers, but there no cache to read them from!");
				}

				if (!writer.HasUpstream) {
					throw new ChattelConfigurationException("Write cache indicates assets needing to be sent to remote servers, but there are no remote servers configured!");
				}
			}

			// Send the assets to the remote server. Yes do this in the startup thread: if you can't access the servers, then why continue?
			foreach (var assetCacheNode in assetsToBeSentUpstream) {
				LOG.Log(Logging.LogLevel.Debug, () => $"Attempting to remotely store {assetCacheNode.AssetId}.");

				if (localStorage.TryGetAsset(assetCacheNode.AssetId, out StratusAsset asset)) {
					try {
						writer.PutAssetSync(asset);
					}
					catch (AssetExistsException) {
						// Ignore these.
						LOG.Log(Logging.LogLevel.Info, () => $"Remote server reports that the asset with ID {assetCacheNode.AssetId} already exists.");
					}

					ClearNode(assetCacheNode);
				}
				else {
					LOG.Log(Logging.LogLevel.Warn, () => $"Write cache indicates asset {assetCacheNode.AssetId} has not been sent upstream, but the cache reports that there's no such asset!.");
				}
			}

			// Bootstrap the system.
			GetNextAvailableNode();
		}

		/// <summary>
		/// Opens or creates the write cache file. If there are entries in the file that are marked as not uploaded, then
		/// this ctor throws a ChattelConfigurationException.
		/// </summary>
		/// <param name="fileInfo">FileInfo instance for the path where to load or create the write cache file.</param>
		/// <param name="recordCount">Record count to set the write cache to.</param>
		/// <exception cref="T:Chattel.ChattelConfigurationException">Thrown if there are assets marked as needing to be uploaded but the current configuration prevents uploading.</exception>
		public WriteCache(FileInfo fileInfo, uint recordCount)
			: this(fileInfo, recordCount, null, null) {
		}

		private void Initialize(uint recordCount) {
			using (var fileStream = _fileInfo.Create()) {
				var maxLength = WRITE_CACHE_MAGIC_NUMBER.Length + ((long)recordCount * WriteCacheNode.BYTE_SIZE);
				fileStream.SetLength(maxLength);
				// On some FSs the file is all 0s at this point, but it behooves us to make sure of the critical points.

				// Write the header
				fileStream.Seek(0, SeekOrigin.Begin);
				fileStream.Write(WRITE_CACHE_MAGIC_NUMBER, 0, WRITE_CACHE_MAGIC_NUMBER.Length);

				// Make sure to flag all record as available, just in case the FS didn't give us a clean slate.
				for (var offset = (long)WRITE_CACHE_MAGIC_NUMBER.Length; offset < maxLength; offset += WriteCacheNode.BYTE_SIZE) {
					fileStream.Seek(offset, SeekOrigin.Begin);
					fileStream.WriteByte(0); // First byte is always the status: 0 means it's an open slot.
				}
			}
		}

		private void Expand(uint recordCount) {
			var bytesNeeded = (int)(recordCount * WriteCacheNode.BYTE_SIZE);
			LOG.Log(Logging.LogLevel.Info, () => $"Write cache file expanding by {bytesNeeded} bytes to accomodate requested change in record count.");
			using (var stream = new FileStream(_fileInfo.FullName, FileMode.Append)) {
				stream.Write(new byte[bytesNeeded], 0, bytesNeeded); // In C# a new array is already init'd to 0s.
			}
		}

		private IEnumerable<WriteCacheNode> Read(out IEnumerable<WriteCacheNode> assetsToSend) {
			var fileRecordCount = (int)(_fileInfo.Length / WriteCacheNode.BYTE_SIZE);

			var assetsToSendOut = new List<WriteCacheNode>();

			using (var mmf = MemoryMappedFile.CreateFromFile(_fileInfo.FullName, FileMode.Open)) {
				using (var stream = mmf.CreateViewStream()) {
					var offset = 0UL;
					{
						var magic_number = new byte[WRITE_CACHE_MAGIC_NUMBER.Length];
						stream.Read(magic_number, 0, WRITE_CACHE_MAGIC_NUMBER.Length);
						if (!magic_number.SequenceEqual(WRITE_CACHE_MAGIC_NUMBER)) {
							throw new InvalidDataException($"Magic number mismatch when given path: {_fileInfo.FullName}");
						}
						offset += (ulong)WRITE_CACHE_MAGIC_NUMBER.Length;
					}

					var nodes = new List<WriteCacheNode>(fileRecordCount);

					var buffer = new byte[WriteCacheNode.BYTE_SIZE];

					while (nodes.Count < fileRecordCount) {
						stream.Read(buffer, 0, (int)WriteCacheNode.BYTE_SIZE);
						var node = new WriteCacheNode(buffer, offset);
						nodes.Add(node);
						offset += WriteCacheNode.BYTE_SIZE;

						// If the node isn't available that means it's an ID that still needs to be written to long-term storage.
						if (!node.IsAvailable) {
							assetsToSendOut.Add(node);
						}
					}

					assetsToSend = assetsToSendOut;
					return nodes;
				}
			}
		}

		/// <summary>
		/// Marks the passed in node as completely uploaded to a remote server and available for reuse, both in memory and on disk.
		/// </summary>
		/// <param name="node">Node.</param>
		public void ClearNode(WriteCacheNode node) {
			node = node ?? throw new ArgumentNullException(nameof(node));

			// Clear the byte on disk before clearing in memory.
			using (var mmf = MemoryMappedFile.CreateFromFile(_fileInfo.FullName, FileMode.Open))
			using (var accessor = mmf.CreateViewAccessor((long)node.FileOffset, WriteCacheNode.BYTE_SIZE)) {
				accessor.Write(0, (byte)0);
			}
			node.IsAvailable = true;
		}

		/// <summary>
		/// Marks the next available node as in use and not yet uploaded to a remote server, both on disk and in-memory.
		/// </summary>
		/// <returns>The node that was marked as used.</returns>
		/// <param name="asset">Asset.</param>
		public WriteCacheNode WriteNode(StratusAsset asset) {
			asset = asset ?? throw new ArgumentNullException(nameof(asset));

			var writeCachNode = GetNextAvailableNode();
			writeCachNode.AssetId = asset.Id;

			try {
				using (var mmf = MemoryMappedFile.CreateFromFile(_fileInfo.FullName, FileMode.Open))
				using (var accessor = mmf.CreateViewAccessor((long)writeCachNode.FileOffset, WriteCacheNode.BYTE_SIZE)) {
					var nodeBytes = writeCachNode.ToByteArray();

					accessor.WriteArray(0, nodeBytes, 0, (int)WriteCacheNode.BYTE_SIZE);
				}
			}
			catch (Exception e) {
				LOG.Log(Logging.LogLevel.Warn, () => $"{asset.Id} failed to write to disk-based write local storage!", e);
			}

			return writeCachNode;
		}

		private WriteCacheNode GetNextAvailableNode() {
			WriteCacheNode writeCacheNode;

			void updateToNextNode() {
				if (_nextAvailableWriteCacheNode == null) {
					try {
						_nextAvailableWriteCacheNode = _writeCacheNodes.First(node => node.IsAvailable);
					}
					catch (InvalidOperationException) {
						// No available nodes found, which means we are out of ability to safely continue until one becomes available...
						_nextAvailableWriteCacheNode = null;
						throw new WriteCacheFullException("All write cache nodes are used!");
					}
				}
			}

			lock (_writeCacheNodeLock) {
				// If we've not bootstrapped, do so.
				updateToNextNode();

				writeCacheNode = _nextAvailableWriteCacheNode;
				writeCacheNode.IsAvailable = false;
				_nextAvailableWriteCacheNode = null;

				// Find the next one.
				updateToNextNode();
			}

			return writeCacheNode;
		}
	}
}
