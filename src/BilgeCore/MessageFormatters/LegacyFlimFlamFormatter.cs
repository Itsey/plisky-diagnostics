﻿namespace Plisky.Diagnostics {
    using System;

    /// <summary>
    /// Formats for the old flimflam
    /// </summary>
    public class LegacyFlimFlamFormatter : BaseMessageFormatter {
        /// <summary>
        /// Perform the actual conversion
        /// </summary>
        /// <param name="msg">The message to convert</param>
        /// <returns>A string representing the message</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            string result;
            var nextMsg = new MessageParts();
            msg.NullsToEmptyStrings();

            nextMsg.DebugMessage = msg.Body;
            nextMsg.SecondaryMessage = msg.FurtherDetails;
            nextMsg.ClassName = msg.ClassName;
            nextMsg.LineNumber = msg.LineNumber;
            nextMsg.MethodName = msg.MethodName;
            nextMsg.MachineName = msg.MachineName;
            nextMsg.NetThreadId = msg.NetThreadId;
            nextMsg.OSThreadId = msg.OsThreadId;
            nextMsg.ProcessId = msg.ProcessId;
            nextMsg.ModuleName = msg.FileName;
            nextMsg.AdditionalLocationData = msg.ClassName + "::" + msg.MethodName;

            // Populate Message Type
            nextMsg.MessageType = TraceCommands.TraceCommandToString(msg.CommandType);

            result = TraceMessageFormat.AssembleFormattedStringFromMessageStructure(nextMsg) + Environment.NewLine;


            return result;
        }
    }
}