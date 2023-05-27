using System;
using System.Diagnostics;
using Plisky.Diagnostics;

namespace DevConsoleTest {
    internal class QuickPerfTest {
        public string TestName { get; }
        public Bilge PerfTestInstance { get; private set; }
        public Action Test { get; internal set; }
        public Stopwatch Sw { get; private set; }
        public QuickPerfTest(string v, SourceLevels desiredTraceLevel) {
           TestName = v;
            PerfTestInstance = new Bilge(TestName, tl:desiredTraceLevel);
            Sw = new Stopwatch(); 
        }

        
        
    }
}