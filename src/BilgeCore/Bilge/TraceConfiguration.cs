
namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Holds settings that relate to the way that trace is written from Bilge.  Settings here are optional but can add or remove content from the trace messages, by
    /// default most of the additional information is off as it adds an overhead to the trace writing processes.
    /// </summary>
    public class TraceConfiguraton {
        /// <summary>
        /// If this is true then each call to the trace will make a stack call and determine the calling class of the method, this can help in formatting the trace
        /// and provide additional information but comes at a performance cost.  Defaults to false.
        /// </summary>
        public bool AddClassDetailToTrace { get; set; }

        /// <summary>
        /// Determines if timings are automatically added to trace enter and exit
        /// </summary>
        public bool AddTimingsToEnterExit { get; internal set; }

        /// <summary>
        /// Determines whether time stamp information is written to every message in the stream.  If true then each message will have DateTime.Now written into it.
        /// </summary>
        public bool AddTimestamps { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TraceConfiguraton"/> class.
        /// </summary>
        public TraceConfiguraton() {
            AddClassDetailToTrace = false;
            AddTimingsToEnterExit = false;
            AddTimestamps = false;
        }
    }

}
