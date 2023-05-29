namespace Plisky.Plumbing {

    /// <summary>
    /// Supplies trace context info.
    /// </summary>
    internal class TraceContextInfo {

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceContextInfo"/> class.
        /// The trace contact info.
        /// </summary>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="line">The line number where the call was made.</param>
        /// <param name="fn">The path to the file of source for the calling method.</param>
        internal TraceContextInfo(string meth, int? line, string fn) {
            MethodName = meth;
            LineNumber = line;
            Filename = fn;
        }

        /// <summary>
        /// The filename of the code.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The line number.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// The method name.
        /// </summary>
        public string MethodName { get; set; }
    }
}