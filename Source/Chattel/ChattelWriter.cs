// ChattelWriter.cs
//
// Author:
//       Ricky Curtice <ricky@rwcproductions.com>
//
// Copyright (c) 2016 Richard Curtice
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public class ChattelWriter {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<ChattelWriter>();
		// Storage for assets that are WIP for remote storage.
		private static readonly byte[] WRITE_CACHE_MAGIC_NUMBER = System.Text.Encoding.ASCII.GetBytes("WHIPLRU1");

		private readonly ChattelConfiguration _config;
		private readonly IChattelCache _cache;

		private readonly ConcurrentDictionary<Guid, ReaderWriterLockSlim> _activeWriteLocks = new ConcurrentDictionary<Guid, ReaderWriterLockSlim>();

		// TODO: consider moving the writecache operations into their own class. Testing and encapsulation...
		private readonly WriteCacheNode[] _writeCacheNodes;
		private readonly object _writeCacheNodeLock = new object();
		private WriteCacheNode _nextAvailableWriteCacheNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelWriter"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		/// <param name="cache">Instance of the IChattelCache interface. If left null, then the default ChattelCache will be instantiated.</param>
		/// <param name="purgeCache">Whether or not to attempt to purge the cache.</param>
		/// <exception cref="!:ChattelConfigurationException">Thrown if the are pending assets to be sent upstream and there are no upstream servers configured.</exception>
		public ChattelWriter(ChattelConfiguration config, IChattelCache cache = null, bool purgeCache = false) {
			_config = config ?? throw new ArgumentNullException(nameof(config));

			if (config.CacheEnabled) {
				_cache = cache ?? new ChattelCache(config);
			}

			if (purgeCache) {
				_cache?.Purge();
			}

			if (config.CacheEnabled) {
				// If the file doesn't exist, create it and zero the needed records.
				if (!config.WriteCacheFile?.Exists ?? false) {
					LOG.Log(Logging.LogLevel.Info, () => $"Write cache file doesn't exist, creating and formatting file '{config.WriteCacheFile.FullName}'");
					WriteCacheInitialize(config.WriteCacheFile, config.WriteCacheRecordCount);
					LOG.Log(Logging.LogLevel.Debug, () => $"Write cache formatting complete.");
				}

				var writeCacheFileRecordCount = (uint)(((config.WriteCacheFile?.Length ?? 0) - WRITE_CACHE_MAGIC_NUMBER.Length) / WriteCacheNode.BYTE_SIZE);

				if (writeCacheFileRecordCount < config.WriteCacheRecordCount) {
					// Expand the file.
					WriteCacheExpand(config.WriteCacheFile, config.WriteCacheRecordCount - writeCacheFileRecordCount);
				}
				else if (writeCacheFileRecordCount > config.WriteCacheRecordCount) {
					// For now, use the file size.
					LOG.Log(Logging.LogLevel.Warn, () => $"Write cache not able to be shrunk in this version of Chattel, continuing with old value of {writeCacheFileRecordCount} records instead of requested {config.WriteCacheRecordCount} records.");
					config.WriteCacheRecordCount = writeCacheFileRecordCount;
					// TODO: find a way to shrink the file without losing ANY of the records that have not yet been submitted to an upstream server.
					// Could get difficult in the case of a full file...
				}

				LOG.Log(Logging.LogLevel.Info, () => $"Reading write cache from file '{config.WriteCacheFile?.FullName}'. Expecting {config.WriteCacheRecordCount} records, found {writeCacheFileRecordCount} records, choosing the larger.");
				_writeCacheNodes = WriteCacheRead(config.WriteCacheFile, out IEnumerable<WriteCacheNode> assetsToBeSentUpstream).ToArray();
				LOG.Log(Logging.LogLevel.Debug, () => $"Reading write cache complete.");

				if (!HasUpstream && assetsToBeSentUpstream.Any()) {
					throw new ChattelConfigurationException("Write cache indicates assets needing to be sent to remote servers, but there are no remote servers configured!");
				}

				// Send the assets to the remote server. Yes do this in the startup thread: if you can't access the servers, then why continue?
				foreach (var assetCacheNode in assetsToBeSentUpstream) {
					LOG.Log(Logging.LogLevel.Debug, () => $"Attempting to remotely store {assetCacheNode.AssetId}.");

					if (cache.TryGetCachedAsset(assetCacheNode.AssetId, out StratusAsset asset)) {
						try {
							PutAssetSync(asset);
						}
						catch (AssetExistsException) {
							// Ignore these.
							LOG.Log(Logging.LogLevel.Info, () => $"Remote server reports that the asset with ID {assetCacheNode.AssetId} already exists.");
						}

						WriteCacheClearNode(config.WriteCacheFile, assetCacheNode);
					}
					else {
						LOG.Log(Logging.LogLevel.Warn, () => $"Write cache indicates asset {assetCacheNode.AssetId} has not been sent upstream, but the cache reports that there's no such asset!.");
					}
				}

				// Bootstrap the system.
				WriteCacheNodeGetNextAvailable();
			}
		}

		public bool HasUpstream => _config?.SerialParallelAssetServers.Any() ?? false;

		/// <summary>
		/// Alias for PutAssetSync
		/// </summary>
		/// <param name="asset">The asset to store.</param>
		[Obsolete("Please convert to PutAssetSync")]
		public void WriteAssetSync(StratusAsset asset) {
			PutAssetSync(asset);
		}

		/// <summary>
		/// Sends the asset to the asset servers.
		/// Throws AssetExistsException or AggregateException.
		/// </summary>
		/// <param name="asset">The asset to store.</param>
		public void PutAssetSync(StratusAsset asset) {
			asset = asset ?? throw new ArgumentNullException(nameof(asset));

			if (asset.Id == Guid.Empty) {
				throw new ArgumentException("Asset cannot have zero ID.", nameof(asset));
			}

			// Handle parallel calls with the same asset ID.
			var firstLock = new ReaderWriterLockSlim();
			try {
				firstLock.EnterWriteLock();
				var activeLock = _activeWriteLocks.GetOrAdd(asset.Id, firstLock);
				if (firstLock != activeLock) {
					// There's another thread currently adding this exact ID, so we need to wait on it so that we return when it's actually ready for a GET.
					activeLock.EnterReadLock();
					activeLock.ExitReadLock();
					return;
				}

				// Hit up the cache first.
				if (_cache?.TryGetCachedAsset(asset.Id, out StratusAsset result) ?? false) {
					_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
					firstLock.ExitWriteLock();
					throw new AssetExistsException(asset.Id);
				}

				var exceptions = new List<Exception>();
				var success = false;
				WriteCacheNode activeNode = null;

				// First step: get it in the local disk cache.
				try {
					_cache?.CacheAsset(asset);

					if (HasUpstream) {
						// Write to writecache file. In this way if we crash after this point we can recover and send the asset to the servers.
						activeNode = WriteCacheWriteNode(_config.WriteCacheFile, asset);
						// If that fails, it'll throw.
					}
					else {
						// Set success if there're no upstream servers. This supports applications that act as asset servers.
						success = true;
					}
				}
				catch (WriteCacheNodesFull) {
					throw;
				}
				catch (Exception e) {
					exceptions.Add(e);
				}

				// Got to go try the servers now.
				foreach (var parallelServers in _config.SerialParallelAssetServers) {
					// Remember each iteration of this loop is going through serially accessed blocks of parallel-access servers.
					// Therefore any failure or problem in one of the blocks means to just continue to the next block.
					try {
						if (parallelServers.Count() == 1) {
							parallelServers.First().StoreAssetSync(asset);
						}
						else {
							parallelServers.AsParallel().ForAll(server => server.StoreAssetSync(asset));
						}

						if (activeNode != null) {
							WriteCacheClearNode(_config.WriteCacheFile, activeNode);
						}

						success = true;
						break; // It was successfully stored in the first bank of parallel servers, don't do the next bank.
					}
					catch (AssetException e) {
						exceptions.Add(e);
					}
					catch (AggregateException e) {
						// Unwind the aggregate one layer.
						foreach (var ex in e.InnerExceptions) {
							exceptions.Add(ex);
						}
					}
					catch (Exception e) {
						exceptions.Add(e);
					}
				}

				if (!success) {
					throw new AggregateException("Unable to store asset in any asset server. See inner exceptions for details.", exceptions);
				}
			}
			finally {
				_activeWriteLocks.TryRemove(asset.Id, out ReaderWriterLockSlim lockObj);
				firstLock.ExitWriteLock();
			}
		}

		#region Write cache utilities

		private void WriteCacheInitialize(FileInfo fileInfo, uint recordCount) {
			if (fileInfo == null) {
				return;
			}

			using (var fileStream = fileInfo.Create()) {
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

		private void WriteCacheExpand(FileInfo fileInfo, uint recordCount) {
			if (fileInfo == null) {
				return;
			}

			var bytesNeeded = (int)(recordCount * WriteCacheNode.BYTE_SIZE);
			LOG.Log(Logging.LogLevel.Info, () => $"Write cache file expanding by {bytesNeeded} bytes to accomodate requested change in record count.");
			using (var stream = new FileStream(fileInfo.FullName, FileMode.Append)) {
				stream.Write(new byte[bytesNeeded], 0, bytesNeeded); // In C# a new array is already init'd to 0s.
			}
		}

		private IEnumerable<WriteCacheNode> WriteCacheRead(FileInfo fileInfo, out IEnumerable<WriteCacheNode> assetsToSend) {
			if (fileInfo == null) {
				assetsToSend = null;
				return null;
			}

			var fileRecordCount = (int)(fileInfo.Length / WriteCacheNode.BYTE_SIZE);

			var assetsToSendOut = new List<WriteCacheNode>();

			using (var mmf = MemoryMappedFile.CreateFromFile(fileInfo.FullName, FileMode.Open)) {
				using (var stream = mmf.CreateViewStream()) {
					var offset = 0UL;
					{
						var magic_number = new byte[WRITE_CACHE_MAGIC_NUMBER.Length];
						stream.Read(magic_number, 0, WRITE_CACHE_MAGIC_NUMBER.Length);
						if (!magic_number.SequenceEqual(WRITE_CACHE_MAGIC_NUMBER)) {
							throw new InvalidDataException($"Magic number mismatch when given path: {fileInfo.FullName}");
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

		private void WriteCacheClearNode(FileInfo fileInfo, WriteCacheNode node) {
			fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
			node = node ?? throw new ArgumentNullException(nameof(node));

			// Clear the byte on disk before clearing in memory.
			using (var mmf = MemoryMappedFile.CreateFromFile(fileInfo.FullName, FileMode.Open))
			using (var accessor = mmf.CreateViewAccessor((long)node.FileOffset, WriteCacheNode.BYTE_SIZE)) {
				accessor.Write(0, (byte)0);
			}
			node.IsAvailable = true;
		}

		private WriteCacheNode WriteCacheWriteNode(FileInfo fileInfo, StratusAsset asset) {
			fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
			asset = asset ?? throw new ArgumentNullException(nameof(asset));

			var writeCachNode = WriteCacheNodeGetNextAvailable();
			writeCachNode.AssetId = asset.Id;

			try {
				using (var mmf = MemoryMappedFile.CreateFromFile(fileInfo.FullName, FileMode.Open, "whiplruwritecache"))
				using (var accessor = mmf.CreateViewAccessor((long)writeCachNode.FileOffset, WriteCacheNode.BYTE_SIZE)) {
					var nodeBytes = writeCachNode.ToByteArray();

					accessor.WriteArray(0, nodeBytes, 0, (int)WriteCacheNode.BYTE_SIZE);
				}
			}
			catch (Exception e) {
				LOG.Log(Logging.LogLevel.Warn, () => $"{asset.Id} failed to write to disk-based write cache!", e);
			}

			return writeCachNode;
		}

		private WriteCacheNode WriteCacheNodeGetNextAvailable() {
			WriteCacheNode writeCacheNode;

			void updateToNextNode() {
				if (_nextAvailableWriteCacheNode == null) {
					try {
						_nextAvailableWriteCacheNode = _writeCacheNodes.First(node => node.IsAvailable);
					}
					catch (InvalidOperationException) {
						// No available nodes found, which means we are out of ability to safely continue until one becomes available...
						_nextAvailableWriteCacheNode = null;
						throw new WriteCacheNodesFull("All write cache nodes are full!");
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

		#endregion
	}
}
