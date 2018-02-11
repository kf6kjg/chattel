// DateRange.cs
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

namespace Chattel {
	internal static class DateTimeExtensions {
		/// <summary>
		/// Determines if the DateTime, if the same Kind, is within the given DateRange. The range is inclusive.
		/// </summary>
		/// <returns><c>true</c>, if the DateTime is of the same kind and in the range, <c>false</c> otherwise.</returns>
		/// <param name="dateToCheck">Date to check.</param>
		/// <param name="range">Range to check against.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when DateRange's kind doesn't match the DateTime's kind.</exception>
		public static bool IsInRange(this DateTime dateToCheck, DateRange range) {
			if (
				(range.Start != null && dateToCheck.Kind != range.Start?.Kind)
				||
				(range.End != null && dateToCheck.Kind != range.End?.Kind)
			) {
				throw new ArgumentOutOfRangeException(nameof(range), "DateRange kind doesn't match the DateTime's kind!");
			}

			if (range.Start == null && range.End == null) {
				// Both null means that the range is wide open.
				return true;
			}

			if (range.Start == null) {
				// Start's null, but end is not. Means that we don't care about the starting date!
				return dateToCheck <= range.End;
			}

			if (range.End == null) {
				// End's null, but start is not. Means that we don't care about the ending date!
				return dateToCheck >= range.Start;
			}

			// Neither is null, so we care about both.
			return dateToCheck >= range.Start && dateToCheck <= range.End;
		}
	}

	/// <summary>
	/// Date range for easily determining if a given DateTime is between two others.
	/// </summary>
	public struct DateRange {
		/// <summary>
		/// Initializes a new instance of the <see cref="DateRange" /> structure to the specified start and end date.
		/// Both parameters are nullable. A null entry means skip that entry.
		/// </summary>
		/// <param name="startDate">The first date in the date range.</param>
		/// <param name="endDate">The last date in the date range.</param>
		/// <exception cref="ArgumentException">
		///	endDate is not greater than or equal to startDate, or they are not of the same kind.
		/// </exception>
		public DateRange(DateTime? startDate, DateTime? endDate) : this() {
			Start = startDate;
			End = endDate;

			if (Start != null && End != null && End?.Kind != Start?.Kind) {
				throw new ArgumentException("startDate and endDate must be the same kind");
			}

			if (End < Start) {
				throw new ArgumentException("endDate must be greater than or equal to startDate");
			}
		}

		/// <summary>
		/// Gets the start date component of the date range.
		/// </summary>
		public DateTime? Start { get; private set; }

		/// <summary>
		/// Gets the end date component of the date range.
		/// </summary>
		public DateTime? End { get; private set; }
	}
}
