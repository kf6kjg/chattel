// AssetStorageSimpleFolderTree.cs
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
using System.IO;
using System.Linq;
using InWorldz.Data.Assets.Stratus;
using ProtoBuf;

namespace Chattel {
	public class AssetStorageSimpleFolderTree : IChattelLocalStorage {
		private static readonly Logging.ILog LOG = Logging.LogProvider.For<AssetStorageSimpleFolderTree>();

		private readonly ChattelConfiguration _config;

		private readonly ConcurrentDictionary<string, StratusAsset> _assetsBeingWritten = new ConcurrentDictionary<string, StratusAsset>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ChattelReader"/> class.
		/// </summary>
		/// <param name="config">Instance of the configuration class.</param>
		public AssetStorageSimpleFolderTree(ChattelConfiguration config) {
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}

		public bool TryGetAsset(Guid assetId, out StratusAsset asset) {
			if (!_config.LocalStorageEnabled) {
				asset = null;
				return false;
			}

			// Convert the UUID into a path.
			var path = UuidToLocalPath(assetId);

			if (_assetsBeingWritten.TryGetValue(path, out asset)) {
				LOG.Log(Logging.LogLevel.Debug, () => $"Attempted to read an asset from local storage, but another thread is writing it. Shortcutting read of {path}");
				// Asset is currently being pushed to disk, so might as well return it now since I have it in memory.
				return true;
			}

			// Attempt to read and return that file.  This needs to handle happening from multiple threads in case a given asset is read from multiple threads at the same time.
			var removeFile = false;
			try {
				using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					asset = Serializer.Deserialize<StratusAsset>(stream);
				}
				return true;
			}
			catch (PathTooLongException e) {
				_config.DisableLocalStorage();
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to read a locally stored asset, but the path was too long for the filesystem. Disabling local storage.", e);
			}
			catch (DirectoryNotFoundException) {
				// Kinda expected if that's an item that's not been stored locally.
			}
			catch (UnauthorizedAccessException e) {
				_config.DisableLocalStorage();
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to read a locally stored asset, but this user is not allowed access. Disabling local storage.", e);
			}
			catch (FileNotFoundException) {
				// Kinda expected if that's an item that's not been stored locally.
			}
			catch (IOException e) {
				// This could be temporary.
				LOG.Log(Logging.LogLevel.Warn, () => "[ASSET_READER] Attempted to read a locally stored asset, but there was an IO error.", e);
			}
			catch (ProtoException e) {
				LOG.Log(Logging.LogLevel.Warn, () => $"[ASSET_READER] Attempted to read a locally stored asset, but there was a protobuf decoding error. Removing the offending local storage file as it is either corrupt or from an older installation: {path}", e);
				removeFile = true;
			}

			if (removeFile) {
				try {
					File.Delete(path);
					// TODO: at some point the folder tree should be checked for folders that should be removed.
				}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
				catch {
					// If there's a delete failure it'll just keep trying as the asset is called for again.
				}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			}

			// Nope, no ability to get the asset.
			asset = null;
			return false;
		}

		public void StoreAsset(StratusAsset asset) {
			if (!_config.LocalStorageEnabled || asset == null) { // Caching is disabled or stupidity.
				return;
			}

			var path = UuidToLocalPath(asset.Id);

			if (!_assetsBeingWritten.TryAdd(path, asset)) {
				LOG.Log(Logging.LogLevel.Debug, () => $"[ASSET_READER] Attempted to write an asset to local storage, but another thread is already doing so.  Skipping write of {path}");
				// Can't add it, which means it's already being written to disk by another thread.  No need to continue.
				return;
			}

			try {
				// Since UuidToLocalPath always returns a path underneath the local storage folder, this will only attempt to create folders there.
				Directory.CreateDirectory(Directory.GetParent(path).FullName);
				using (var file = File.Create(path)) {
					Serializer.Serialize(file, asset);
				}
				// Writing is done, remove it from the work list.
				_assetsBeingWritten.TryRemove(path, out StratusAsset temp);
				LOG.Log(Logging.LogLevel.Debug, () => $"[ASSET_READER] Wrote an asset to local storage: {path}");
			}
			catch (UnauthorizedAccessException e) {
				_config.DisableLocalStorage();
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to write an asset to local storage, but this user is not allowed access. Disabling local storage.", e);
			}
			catch (PathTooLongException e) {
				_config.DisableLocalStorage();
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to write an asset to local storage, but the path was too long for the filesystem. Disabling local storage.", e);
			}
			catch (DirectoryNotFoundException e) {
				_config.DisableLocalStorage();
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to write an asset to local storage, but local storage folder was not found. Disabling local storage.", e);
			}
			catch (IOException e) {
				// This could be temporary.
				LOG.Log(Logging.LogLevel.Error, () => "[ASSET_READER] Attempted to write an asset to local storage, but there was an IO error.", e);
			}
		}

		public void PurgeAll() {
			_config.LocalStorageFolder.EnumerateDirectories().AsParallel().ForAll(dir => dir.Delete(true));
		}

		public void Purge(Guid assetId) {
			try {
				File.Delete(UuidToLocalPath(assetId));
				// Lazily not attempting to clean up the folder tree.
				// Using up too many inodes? Use a better local storage implementation.
			}
			catch (DirectoryNotFoundException) {
				throw new AssetNotFoundException(assetId);
			}
			catch (FileNotFoundException) {
				throw new AssetNotFoundException(assetId);
			}
		}

		/// <summary>
		/// Converts a GUID to a path based on the local storage location.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="id">Asset identifier.</param>
		protected string UuidToLocalPath(Guid id) {
			var noPunctuationAssetId = id.ToString("N");
			var path = _config.LocalStorageFolder.FullName;
			for (var index = 0; index < noPunctuationAssetId.Length; index += 2) {
				path = Path.Combine(path, noPunctuationAssetId.Substring(index, 2));
			}
			return path + ".pbasset";
		}
	}
}
