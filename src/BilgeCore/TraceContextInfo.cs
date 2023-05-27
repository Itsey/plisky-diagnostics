namespace Plisky.Plumbing {

    internal class TraceContextInfo {

        internal TraceContextInfo(string meth, int? line, string fn) {
            MethodName = meth;
            LineNumber = line;
            Filename = fn;
        }

        public string Filename { get; set; }

        public int? LineNumber { get; set; }

        public string MethodName { get; set; }
    }
}