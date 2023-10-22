using System;
using System.Diagnostics;
using Plisky.Diagnostics;

namespace DevConsoleTest {

    /// <summary>
    /// Houses a quick internal performance test.
    /// </summary>
    internal class QuickPerfTest {

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickPerfTest"/> class.
        /// </summary>
        /// <param name="v">The string to use to identify the test.</param>
        /// <param name="desiredTraceLevel">The trace level to use.</param>
        internal QuickPerfTest(string v, SourceLevels desiredTraceLevel) {
            TestName = v;
            PerfTestInstance = new Bilge(TestName, tl: desiredTraceLevel);
            Sw = new Stopwatch();
        }

        /// <summary>
        /// The instance of the performacne test
        /// </summary>
        internal Bilge PerfTestInstance { get; private set; }

        /// <summary>
        /// A stopwatch to time the test
        /// </summary>
        internal Stopwatch Sw { get; private set; }

        /// <summary>
        /// The test itself
        /// </summary>
        internal Action Test { get;  set; }

        /// <summary>
        /// A reference identifier for the test
        /// </summary>
        internal string TestName { get; }
    }
}