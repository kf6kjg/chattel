// IChattelLocalStorage.cs
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
using InWorldz.Data.Assets.Stratus;

namespace Chattel {
	/// <summary>
	/// Interface to describe all operations expected for local storage to handle.
	/// </summary>
	public interface IChattelLocalStorage {
		/// <summary>
		/// Requests that an asset be fetched from local storage.
		/// </summary>
		/// <returns><c>true</c>, if get asset was found, <c>false</c> otherwise.</returns>
		/// <param name="assetId">Asset identifier.</param>
		/// <param name="asset">The resulting asset.</param>
		bool TryGetAsset(Guid assetId, out StratusAsset asset);

		/// <summary>
		/// Stores the asset in local storage.
		/// </summary>
		/// <param name="asset">Asset to store.</param>
		void StoreAsset(StratusAsset asset);

		/// <summary>
		/// Purges all items that match the passed filter.
		/// Fields in each filter element are handled in as an AND condition, while sibling filters are handled in an OR condition.
		/// Thus if you wanted to purge all assets that have the temp flag set true OR all assets with the local flag set true, you'd have an array of two filter objects, the first would set the temp flag to true, the second would set the local flag to true.
		/// If instead you wanted to purge all assets that have the temp flag set true AND local flag set true, you'd have an array of a single filter object with both the temp flag and the local flag set to true.
		/// </summary>
		void PurgeAll(); // TODO: a filter object with internal nullable fields and setters that set the fields to a not null condition.  Each field is in an AND condition.  This would take an array of such fields, each of which would be cosidered to be OR'd.

		/// <summary>
		/// Purge the specified asset from local storage.
		/// </summary>
		/// <param name="assetId">Asset identifier.</param>
		void Purge(Guid assetId);
	}
}
