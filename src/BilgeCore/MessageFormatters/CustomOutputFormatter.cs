namespace Plisky.Diagnostics {
    using System;

    /// <summary>
    /// pretty formatter
    /// </summary>
    public class CustomOutputFormatter : BaseMessageFormatter {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOutputFormatter"/> class.
        /// </summary>
        public CustomOutputFormatter() {
            MessageTypeConvert = new Func<TraceCommandTypes, string>(tct => {
                return TraceCommands.TraceCommandToString(tct);
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOutputFormatter"/> class with custom options
        /// </summary>
        /// <param name="mfo">The options to use</param>
        public CustomOutputFormatter(MessageFormatterOptions mfo) : base(mfo) {
            MessageTypeConvert = new Func<TraceCommandTypes, string>(tct => {
                return TraceCommands.TraceCommandToString(tct);
            });
        }

        /// <summary>
        /// The format string to write the contents out in.
        /// 0 = DateTime.Now, 1 = Msg.Body, 2 = Msg.ClassName, 3 = Msg.LineNumber, 4 = msg.ProcessId, 5 = msg.MachineName, 6 = msg.NetThreadId, 7 = msg.OsThread, 8 = msg.Further, 9 = msgType, 10 = NewLine
        /// </summary>
        public string FormatString { get; set; } = "{9} {0} >> {1} || {2}@{3} in {4}@{5} >> T[{6}-{7}] || {8} || {10}";

        /// <summary>
        /// Custom converter for the message TraceCommandType, by default it will just use the standard convertor.
        /// </summary>
        public Func<TraceCommandTypes, string> MessageTypeConvert { get; set; }

        /// <summary>
        /// Uses Format string to convert.
        /// E.g. "{1}{2}" will just write out Body and classname.
        /// </summary>
        /// <param name="msg">The message to format</param>
        /// <returns>A formatted string</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            string nl = mfo.AppendNewline ? Environment.NewLine : string.Empty;
            string mtc = MessageTypeConvert == null ? msg.CommandType.ToString() : MessageTypeConvert(msg.CommandType);
            return string.Format(FormatString, DateTime.Now, msg.Body, msg.ClassName, msg.LineNumber, msg.ProcessId, msg.MachineName, msg.NetThreadId, msg.OsThreadId, msg.FurtherDetails, mtc, nl);
        }
    }
}