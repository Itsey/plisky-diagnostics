using System;

namespace Plisky.Diagnostics.Listeners {
    /// <summary>
    /// Console formatter will summarise the message metadata into a single line of text so that it can be shown on the console.
    /// </summary>
    public class ConsoleFormatter : BaseMessageFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleFormatter"/> class.
        /// </summary>
        /// <param name="options">The options to control the approach of the formatter.</param>
        public ConsoleFormatter(MessageFormatterOptions options) : base(options) {
        }

        /// <summary>
        /// Perform the actual conversion.
        /// </summary>
        /// <param name="msg">The message metadata to convert.</param>
        /// <returns>The string of the converted message meta data suitable for display on a console app.</returns>
        protected override string ActualConvert(MessageMetadata msg) {

            string timeOutput = (msg?.TimeStamp ?? DateTime.Now).ToString("HH:mm:ss");
            string result = $"{timeOutput} ({msg.Context}:{msg.NetThreadId})>> {msg.Body}| {msg.FurtherDetails} | @ {msg.MethodName}";

            return result;
        }
    }
}