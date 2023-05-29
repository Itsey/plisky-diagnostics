namespace Plisky.Diagnostics {
    using System.Diagnostics;

    /// <summary>
    /// Writes trace details out at the informational level.
    /// </summary>
    public class InfoWriter : BilgeWriter {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoWriter"/> class.
        /// </summary>
        /// <param name="router">The router to use for sending messages</param>
        /// <param name="config">The configuration for the library</param>
        /// <param name="yourTraceLevel">The trace level to use for this writer</param>
        public InfoWriter(BilgeRouter router, ConfigSettings config, SourceLevels yourTraceLevel) : base(router, config, yourTraceLevel) {
            baseCommandLevel = TraceCommandTypes.LogMessage;
        }
    }
}