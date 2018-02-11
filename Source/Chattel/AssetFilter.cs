// AssetFilter.cs
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
using System.Text.RegularExpressions;
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	public struct AssetFilter {
		private Regex _id;

		private sbyte? _type;

		private bool? _local;

		private bool? _temporary;

		private DateRange? _createTimeRange;

		private Regex _name;

		private Regex _description;

		private uint? _storageFlags;

		/// <summary>
		/// Sets the identifier filter. Set this if you want to filter assets by ID or ID range.
		/// </summary>
		public Regex IdFilter { set => _id = value; }

		/// <summary>
		/// Sets the type filter. Set this if you want to filter assets by type.
		/// </summary>
		public sbyte TypeFilter { set => _type = value; }

		/// <summary>
		/// Sets the local flag filter. Set this if you want to filter assets by whether or not they are marked local.
		/// </summary>
		public bool LocalFilter { set => _local = value; }

		/// <summary>
		/// Sets the temporary flag filter. Set this if you want to filter assets by whether or not they are marked temporary.
		/// </summary>
		public bool TemporaryFilter { set => _temporary = value; }

		/// <summary>
		/// Sets the creation time filter. Set this if you want to filter assets by when they were created.
		/// </summary>
		public DateRange CreateTimeRangeFilter {  set => _createTimeRange = value; }

		/// <summary>
		/// Sets the name filter. Set this if you want to filter assets by name.
		/// </summary>
		public Regex NameFilter { set => _name = value; }

		/// <summary>
		/// Sets the description filter. Set this if you want to filter assets by description.
		/// </summary>
		public Regex DescriptionFilter { set => _description = value; }

		/// <summary>
		/// Sets the storage flags bitmask filter. Set this if you want to filter assets by the storage flags bitmask.
		/// Checks only to see if the given bits are set.
		/// </summary>
		public uint StorageFlagsFilter { set => _storageFlags = value; }

		/// <summary>
		/// Checks to see if the given asset matches the set flags.
		/// If no flags were set, this falls on the safe side and matches nothing.
		/// </summary>
		/// <returns><c>true</c>, if the asset was matched, <c>false</c> otherwise.</returns>
		/// <param name="asset">Asset to match filters against.</param>
		public bool MatchAsset(StratusAsset asset) {
			var match = true;
			var anyFilterHit = false;

			if (_id != null) {
				match &= _id.IsMatch(asset.Id.ToString("N"));
				anyFilterHit = true;
			}

			if (_type != null) {
				match &= _type == asset.Type;
				anyFilterHit = true;
			}

			if (_local != null) {
				match &= _local == asset.Local;
				anyFilterHit = true;
			}

			if (_temporary != null) {
				match &= _temporary == asset.Temporary;
				anyFilterHit = true;
			}

			if (_createTimeRange != null) {
				try {
					match &= asset.CreateTime.IsInRange((DateRange)_createTimeRange);
				}
				catch (ArgumentOutOfRangeException) {
					// Mismatched date kinds.
					match = false;
				}
				anyFilterHit = true;
			}

			if (_name != null) {
				match &= _name.IsMatch(asset.Name);
				anyFilterHit = true;
			}

			if (_description != null) {
				match &= _description.IsMatch(asset.Description);
				anyFilterHit = true;
			}

			if (_storageFlags != null) {
				match &= _storageFlags == (_storageFlags & asset.StorageFlags);
				anyFilterHit = true;
			}

			if (anyFilterHit) {
				return match;
			}

			return false;
		}
	}
}
