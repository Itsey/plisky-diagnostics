namespace Plisky.Diagnostics.Listeners {

    using System;

    /// <summary>
    /// Console formatter
    /// </summary>
    public class ConsoleFormatter : BaseMessageFormatter {

        /// <summary>
        /// Perform the actual conversion.
        /// </summary>
        /// <param name="msg">The message metadata to convert.</param>
        /// <returns>The string of the converted message meta data.</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            string result = $"{DateTime.Now.ToString("HH:mm:ss")} ({msg.NetThreadId})>> {msg.Body}";
            return result;
        }
    }
}