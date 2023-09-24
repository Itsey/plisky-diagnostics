namespace Plisky.Diagnostics {

    /// <summary>
    /// Holds settings that relate to the way that trace is written from Bilge.  Settings here are optional but can add or remove content from the trace messages, by
    /// default most of the additional information is off as it adds an overhead to the trace writing processes.
    /// </summary>
    public class TraceConfiguration {

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceConfiguration"/> class.
        /// </summary>
        public TraceConfiguration() {
            AddClassDetailToTrace = false;
            AddTimingsToEnterExit = false;
            AddTimestamps = false;
            PassContextToHandler = true;
            UseOSThreadId = true;
        }

        /// <summary>
        /// If this is true then each call to the trace will make a stack call and determine the calling class of the method, this can help in formatting the trace
        /// and provide additional information but comes at a performance cost.  Defaults to false.
        /// </summary>
        public bool AddClassDetailToTrace { get; set; }

        /// <summary>
        /// Determines whether time stamp information is written to every message in the stream.  If true then each message will have DateTime.Now written into it. Defaults to false.
        /// </summary>
        public bool AddTimestamps { get; set; }

        /// <summary>
        /// Determines if timings are automatically added to trace enter and exit.  Defaults to false.
        /// </summary>
        public bool AddTimingsToEnterExit { get; set; }

        /// <summary>
        /// Determines if the context is passed to the handler.  This is used by the BilgeRouter to determine if the context should be passed to the handler.  If this is false then no additional context messages 
        /// are added to the log which incrases performance.  If this is true full context information is passed to the handler.  Defaults to true.
        /// </summary>
        public bool PassContextToHandler { get; set; }

        /// <summary>
        /// Determines whether the OS Thread identity is attempted to be retrieved in addition to the net thread identity.  If this is true then an additional call will be made to try and identify the OS thread
        /// that the application code is running on which incurs a slight performance hit.  Defaults to true.
        /// </summary>
        public bool UseOSThreadId { get; set; }
    }
}