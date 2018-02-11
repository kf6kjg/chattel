// TestDateRange.cs
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
using Chattel;
using NUnit.Framework;

namespace ChattelTests {
	[TestFixture]
	public class TestDateRange {

		#region DateTimeExtensions.IsInRange

		[Test]
		public void TestDateTimeExtensions_IsInRange_Nothing_DoesntThrow() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.DoesNotThrow(() => date.IsInRange(new DateRange()));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Nothing_True() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.True(date.IsInRange(new DateRange()));
		}


		[Test]
		public void TestDateTimeExtensions_IsInRange_Null_Null_DoesntThrow() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.DoesNotThrow(() => date.IsInRange(new DateRange(null, null)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Null_Null_True() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.True(date.IsInRange(new DateRange(null, null)));
		}


		[Test]
		public void TestDateTimeExtensions_IsInRange_Null_WrongKind_ArgumentOutOfRangeException() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.Throws<ArgumentOutOfRangeException>(() => date.IsInRange(new DateRange(
				null,
				new DateTime(234567890L, DateTimeKind.Utc)
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Null_Future_True() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.True(date.IsInRange(new DateRange(
				null,
				new DateTime(234567890L, DateTimeKind.Unspecified)
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Null_Past_False() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.False(date.IsInRange(new DateRange(
				null,
				new DateTime(012345678L, DateTimeKind.Unspecified)
			)));
		}


		[Test]
		public void TestDateTimeExtensions_IsInRange_WrongKind_Null_ArgumentOutOfRangeException() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.Throws<ArgumentOutOfRangeException>(() => date.IsInRange(new DateRange(
				new DateTime(012345678L, DateTimeKind.Utc),
				null
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Future_Null_False() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.False(date.IsInRange(new DateRange(
				new DateTime(234567890L, DateTimeKind.Unspecified),
				null
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Past_Null_True() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.True(date.IsInRange(new DateRange(
				new DateTime(012345678L, DateTimeKind.Unspecified),
				null
			)));
		}


		[Test]
		public void TestDateTimeExtensions_IsInRange_WrongKind_WrongKind_ArgumentOutOfRangeException() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.Throws<ArgumentOutOfRangeException>(() => date.IsInRange(new DateRange(
				new DateTime(012345678L, DateTimeKind.Utc),
				new DateTime(234567890L, DateTimeKind.Utc)
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Past_Past_False() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.False(date.IsInRange(new DateRange(
				new DateTime(012345678L, DateTimeKind.Unspecified),
				new DateTime(023456789L, DateTimeKind.Unspecified)
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Future_Future_False() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.False(date.IsInRange(new DateRange(
				new DateTime(234567890L, DateTimeKind.Unspecified),
				new DateTime(345678901L, DateTimeKind.Unspecified)
			)));
		}

		[Test]
		public void TestDateTimeExtensions_IsInRange_Past_Future_True() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			Assert.True(date.IsInRange(new DateRange(
				new DateTime(012345678L, DateTimeKind.Unspecified),
				new DateTime(234567890L, DateTimeKind.Unspecified)
			)));
		}

		#endregion

		#region Ctor

		[Test]
		public void TestDateRange_Ctor_Nothing_DoesntThrow() {
			Assert.DoesNotThrow(() => new DateRange());
		}

		[Test]
		public void TestDateRange_Ctor_MismatchedKinds_ArgumentException() {
			Assert.Throws<ArgumentException>(() => new DateRange(
				new DateTime(123456789L, DateTimeKind.Local),
				new DateTime(123456789L, DateTimeKind.Unspecified)
			));
		}

		[Test]
		public void TestDateRange_Ctor_EndBeforeStart_ArgumentException() {
			Assert.Throws<ArgumentException>(() => new DateRange(
				new DateTime(234567890L, DateTimeKind.Unspecified),
				new DateTime(123456789L, DateTimeKind.Unspecified)
			));
		}

		[Test]
		public void TestDateRange_Ctor_StartBeforeEnd_DoesntThrow() {
			Assert.DoesNotThrow(() => new DateRange(
				new DateTime(123456789L, DateTimeKind.Unspecified),
				new DateTime(234567890L, DateTimeKind.Unspecified)
			));
		}

		[Test]
		public void TestDateRange_Ctor_NullStart_DoesntThrow() {
			Assert.DoesNotThrow(() => new DateRange(
				null,
				new DateTime(234567890L, DateTimeKind.Unspecified)
			));
		}

		[Test]
		public void TestDateRange_Ctor_NullEnd_DoesntThrow() {
			Assert.DoesNotThrow(() => new DateRange(
				new DateTime(234567890L, DateTimeKind.Unspecified),
				null
			));
		}

		[Test]
		public void TestDateRange_Ctor_NullBoth_DoesntThrow() {
			Assert.DoesNotThrow(() => new DateRange(
				null,
				null
			));
		}

		#endregion

		#region Ctor set Start

		[Test]
		public void TestDateRange_Ctor_Nothing_StartIsNull() {
			var range = new DateRange();
			Assert.Null(range.Start);
		}

		[Test]
		public void TestDateRange_Ctor_Start_IsCorrect() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			var range = new DateRange(date, null);
			Assert.AreEqual(date, range.Start);
		}


		#endregion

		#region Ctor set End

		[Test]
		public void TestDateRange_Ctor_Nothing_EndIsNull() {
			var range = new DateRange();
			Assert.Null(range.End);
		}

		[Test]
		public void TestDateRange_Ctor_End_IsCorrect() {
			var date = new DateTime(123456789L, DateTimeKind.Unspecified);

			var range = new DateRange(null, date);
			Assert.AreEqual(date, range.End);
		}

		#endregion
	}
}
