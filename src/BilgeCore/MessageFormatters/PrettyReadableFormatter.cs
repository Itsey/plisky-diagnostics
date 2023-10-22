using System;

namespace Plisky.Diagnostics {
    /// <summary>
    /// Holds the logic for a formatter designed to be read by people - e.g. for writing to files.
    /// </summary>
    public class PrettyReadableFormatter : BaseMessageFormatter {
        /// <summary>
        /// Converts a MessageMetadata into a string, making it readable for people.
        /// </summary>
        /// <param name="msg">The MessageMetadata to parse</param>
        /// <returns>A string that has been formatted to be readable by people.</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            string appendage = mfo.AppendNewline ? Environment.NewLine : string.Empty;
            var dateTime = msg?.TimeStamp ?? DateTime.Now;
            return $"{dateTime} >> {msg.Body} <<|>> {msg.ClassName}@{msg.LineNumber} in {msg.ProcessId}@{msg.MachineName} Thread[{msg.NetThreadId},{msg.OsThreadId}] ({msg.FurtherDetails}) [{msg.CommandType}]{appendage}";
        }
    }
}