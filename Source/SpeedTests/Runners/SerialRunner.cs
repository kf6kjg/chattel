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
using System.Diagnostics;

namespace SpeedTests {
	public class SerialRunner : TestRunner {
		private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly uint _iterations;

		public SerialRunner(uint iterations) {
			_iterations = iterations;
		}

		public override void RunTest(object instance, System.Reflection.MethodInfo test) {
			var className = instance.GetType().Name;

			var testParams = new object[] { };

			var stopWatch = new Stopwatch();

			var failures = 0ul;

			LOG.Debug($"Serial test of {className}.{test.Name} starting...");
			stopWatch.Restart();

			for (uint count = 0; count < _iterations; ++count) {
				try {
					stopWatch.Stop();
					if (instance is ITestSetUp setup) {
						setup.SetUp();
					}
					stopWatch.Start();

					test.Invoke(instance, testParams);

					stopWatch.Stop();
					if (instance is ITestTearDown teardown) {
						teardown.TearDown();
					}
					stopWatch.Start();
				}
				catch (System.Reflection.TargetInvocationException e) {
					if (e.InnerException is TestFailedException testException) {
						++failures;
						LOG.Info($"  Test failed with message: {testException.Message}");
					}
				}
			}

			stopWatch.Stop();
			LOG.Info($"Serial test of {className}.{test.Name} took {stopWatch.ElapsedMilliseconds}ms over {_iterations - failures} successful and {failures} failed iterations for an average of {(double) stopWatch.ElapsedMilliseconds / _iterations}ms/call.");
		}
	}
}
