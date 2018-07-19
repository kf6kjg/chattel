// SerialRunner.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedTests {
	public class ParallelRunner : TestRunner {
		private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly uint _iterations;
		private readonly int _parallelism;

		protected string _name = "Parallel";

		public ParallelRunner(uint iterations, int parallelism) {
			_iterations = iterations;
			_parallelism = parallelism < 0 ? -1 : parallelism;
		}

		public override void RunTest(object instance, System.Reflection.MethodInfo test) {
			var className = instance.GetType().Name;

			var testParams = new object[] { };

			LOG.Debug($"{_name} test of {className}.{test.Name} starting with parallism set at " + (_parallelism < 0 ? "full" : _parallelism.ToString()) + " parallel processes...");

			var results = new ConcurrentQueue<Result>();

			Parallel.For(0, _iterations, new ParallelOptions {
				MaxDegreeOfParallelism = _parallelism
			}, index => {
				var testTime = new Stopwatch();

				Exception err = null;

				try {
					if (instance is ITestSetUp setup) {
						setup.SetUp();
					}

					testTime.Restart();
					test.Invoke(instance, testParams);
					testTime.Stop();

					if (instance is ITestTearDown teardown) {
						teardown.TearDown();
					}
				}
				catch (System.Reflection.TargetInvocationException e) {
					if (e.InnerException is TestFailedException testException) {
						err = testException;
					}
					else {
						err = e;
					}
				}

				testTime.Stop();

				results.Enqueue(new Result {
					TimeMillis = testTime.ElapsedMilliseconds,
					Error = err,
				});
			});

			var totalInnerTimeSuccess = 0L;
			var totalInnerTimeFailure = 0L;
			var failures = 0;

			foreach (var result in results) {
				if (result.Error == null) {
					totalInnerTimeSuccess += result.TimeMillis;
				}
				else {
					totalInnerTimeFailure += result.TimeMillis;
					++failures;
					if (result.Error is TestFailedException testException) {
						LOG.Info($"  Test failed with message: {testException.Message}");
					}
					else {
						LOG.Info($"  Test had an exception: {result.Error}");
					}
				}
			}

			LOG.Info($"{_name} test of {className}.{test.Name} took {totalInnerTimeSuccess + totalInnerTimeFailure}ms over {_iterations - failures} successful and {failures} failed iterations for an average of {(double)totalInnerTimeSuccess / _iterations}ms per successful call.");
		}

		private struct Result {
			public long TimeMillis;
			public Exception Error;
		}
	}
}
