namespace Plisky.Diagnostics {

    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Trace commands represents further information about the trace commands themselvers
    /// </summary>
    public static class TraceCommands {

        /// <summary>
        /// Convert a string identifier to the correspondng trace command
        /// </summary>
        /// <param name="theCmdText">The string representing the command type to identify.</param>
        /// <returns>The TraceCommandType that was identified.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Long switch statement but very simple to read")]
        public static TraceCommandTypes StringToTraceCommand(string theCmdText) {
            switch (theCmdText) {
                case Constants.MSGFMT_LOG: return TraceCommandTypes.LogMessage;
                case Constants.MSGFMT_LOGVERBOSE: return TraceCommandTypes.LogMessageVerb;
                case Constants.INTERNALMSG: return TraceCommandTypes.InternalMsg;
                case Constants.TRACEMESSAGEIN: return TraceCommandTypes.TraceMessageIn;
                case Constants.TRACEMESSAGEOUT: return TraceCommandTypes.TraceMessageOut;
                case Constants.ASSERTIONFAILED: return TraceCommandTypes.AssertionFailed;
                case Constants.MOREINFO: return TraceCommandTypes.MoreInfo;
                case Constants.COMMANDONLY: return TraceCommandTypes.CommandOnly;
                case Constants.MSGFMT_XMLCOMMAND: return TraceCommandTypes.CommandData;
                case Constants.ERRORMSG: return TraceCommandTypes.ErrorMsg;
                case Constants.WARNINGMSG: return TraceCommandTypes.WarningMsg;
                case Constants.EXCEPTIONBLOCK: return TraceCommandTypes.ExceptionBlock;
                case Constants.EXCEPTIONDATA: return TraceCommandTypes.ExceptionData;
                case Constants.EXCSTART: return TraceCommandTypes.ExcStart;
                case Constants.EXCEND: return TraceCommandTypes.ExcEnd;
                case Constants.SECTIONSTART: return TraceCommandTypes.SectionStart;
                case Constants.SECTIONEND: return TraceCommandTypes.SectionEnd;
                case Constants.RESOURCEEAT: return TraceCommandTypes.ResourceEat;
                case Constants.RESOURCEPUKE: return TraceCommandTypes.ResourcePuke;
                case Constants.RESOURCECOUNT: return TraceCommandTypes.ResourceCount;
                case Constants.MSGFMT_CUSTOM: return TraceCommandTypes.Custom;
                case Constants.MSGFMT_ALERT: return TraceCommandTypes.Alert;
            }

            throw new ArgumentException($"Unreachable Code, the value of the parameter is invalid {theCmdText}.", nameof(theCmdText));
        }

        /// <summary>
        /// Convert a trace command to the corresponding string
        /// </summary>
        /// <param name="theCommand">The trace command type to turn into a string.</param>
        /// <returns>The string representing the trace command type.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Long switch statement but very simple to read")]
        public static string TraceCommandToString(TraceCommandTypes theCommand) {
            switch (theCommand) {
                case TraceCommandTypes.LogMessage: return Constants.MSGFMT_LOG;
                case TraceCommandTypes.LogMessageVerb: return Constants.MSGFMT_LOGVERBOSE;
                case TraceCommandTypes.InternalMsg: return Constants.INTERNALMSG;
                case TraceCommandTypes.TraceMessageIn: return Constants.TRACEMESSAGEIN;
                case TraceCommandTypes.TraceMessageOut: return Constants.TRACEMESSAGEOUT;
                case TraceCommandTypes.AssertionFailed: return Constants.ASSERTIONFAILED;
                case TraceCommandTypes.MoreInfo: return Constants.MOREINFO;
                case TraceCommandTypes.CommandOnly: return Constants.COMMANDONLY;
                case TraceCommandTypes.ErrorMsg: return Constants.ERRORMSG;
                case TraceCommandTypes.WarningMsg: return Constants.WARNINGMSG;
                case TraceCommandTypes.ExceptionData: return Constants.EXCEPTIONDATA;
                case TraceCommandTypes.ExceptionBlock: return Constants.EXCEPTIONBLOCK;
                case TraceCommandTypes.ExcStart: return Constants.EXCSTART;
                case TraceCommandTypes.ExcEnd: return Constants.EXCEND;
                case TraceCommandTypes.SectionStart: return Constants.SECTIONSTART;
                case TraceCommandTypes.SectionEnd: return Constants.SECTIONEND;
                case TraceCommandTypes.ResourceEat: return Constants.RESOURCEEAT;
                case TraceCommandTypes.ResourcePuke: return Constants.RESOURCEPUKE;
                case TraceCommandTypes.ResourceCount: return Constants.RESOURCECOUNT;
                case TraceCommandTypes.CommandData: return Constants.MSGFMT_XMLCOMMAND;
                case TraceCommandTypes.Alert: return Constants.MSGFMT_ALERT;
                case TraceCommandTypes.Custom: return Constants.MSGFMT_CUSTOM;
            }

            throw new ArgumentException($"TraceCommandToString - Unknown Trace Command Request Mapping - {theCommand.ToString()}", "theCommand");
        }

        /// <summary>
        /// This will take the trace command enum and turn each of the valid entries into a readable string that is suitable
        /// to be printed on the screen or displayed to the user.
        /// </summary>
        /// <param name="tcstring">The trace command types enum selected and valid value</param>
        /// <returns>string representing that value</returns>
        internal static TraceCommandTypes ReadableStringToTraceCommand(string tcstring) {
            switch (tcstring) {
                case "Assertion": return TraceCommandTypes.AssertionFailed;
                case "Command": return TraceCommandTypes.CommandOnly;
                case "Error": return TraceCommandTypes.ErrorMsg;
                case "Exception Block": return TraceCommandTypes.ExceptionBlock;
                case "Exception": return TraceCommandTypes.ExceptionData;
                case "Exception Inner Start": return TraceCommandTypes.ExcEnd;
                case "Exception Inner End": return TraceCommandTypes.ExcStart;
                case "Internal": return TraceCommandTypes.InternalMsg;
                case "Log Message": return TraceCommandTypes.LogMessage;
                case "Log Minimal": return TraceCommandTypes.LogMessageMini;
                case "Log Verbose": return TraceCommandTypes.LogMessageVerb;
                case "Chain Message": return TraceCommandTypes.MoreInfo;
                case "Trace Message": return TraceCommandTypes.TraceMessage;
                case "Trace Enter": return TraceCommandTypes.TraceMessageIn;
                case "Trace Exit": return TraceCommandTypes.TraceMessageOut;
                case "Warning Message": return TraceCommandTypes.WarningMsg;
                case "Section Start": return TraceCommandTypes.SectionStart;
                case "Section End": return TraceCommandTypes.SectionEnd;
                case "Resource Allocation": return TraceCommandTypes.ResourceEat;
                case "Resource DeAllocation": return TraceCommandTypes.ResourcePuke;
                case "Resource Value Setting": return TraceCommandTypes.ResourceCount;
                case "XML Command": return TraceCommandTypes.CommandData;
                case "Custom Command": return TraceCommandTypes.Custom;
                default: throw new ArgumentException("Invalid String passed to ReadableStringToTraceCommand", "tctstring invalid: " + tcstring);
            }
        }

        /// <summary>
        /// This will take the trace command enum and turn each of the valid entries into a readable string that is suitable
        /// to be printed on the screen or displayed to the user.
        /// </summary>
        /// <param name="tct">The trace command types enum selected and valid value</param>
        /// <returns>string representing that value</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Long switch statement but very simple to read")]
        internal static string TraceCommandToReadableString(TraceCommandTypes tct) {
            switch (tct) {
                case TraceCommandTypes.AssertionFailed: return "Assertion";
                case TraceCommandTypes.CommandOnly: return "Command";
                case TraceCommandTypes.ErrorMsg: return "Error";
                case TraceCommandTypes.ExceptionBlock: return "Exception Block";
                case TraceCommandTypes.ExceptionData: return "Exception";
                case TraceCommandTypes.ExcEnd: return "Exception Inner Start";
                case TraceCommandTypes.ExcStart: return "Exception Inner End";
                case TraceCommandTypes.InternalMsg: return "Internal";
                case TraceCommandTypes.LogMessage: return "Log Message";
                case TraceCommandTypes.LogMessageMini: return "Log Minimal";
                case TraceCommandTypes.LogMessageVerb: return "Log Verbose";
                case TraceCommandTypes.MoreInfo: return "Chain Message";
                case TraceCommandTypes.TraceMessage: return "Trace Message";
                case TraceCommandTypes.TraceMessageIn: return "Trace Enter";
                case TraceCommandTypes.TraceMessageOut: return "Trace Exit";
                case TraceCommandTypes.WarningMsg: return "Warning Message";
                case TraceCommandTypes.SectionStart: return "Section Start";
                case TraceCommandTypes.SectionEnd: return "Section End";
                case TraceCommandTypes.ResourceEat: return "Resource Allocation";
                case TraceCommandTypes.ResourcePuke: return "Resource DeAllocation";
                case TraceCommandTypes.ResourceCount: return "Resource Value Setting";
                case TraceCommandTypes.CommandData: return "XML Command";
                case TraceCommandTypes.Custom: return "Custom Command";
                default: throw new ArgumentException("Invalid Trace Command Type selected in TraceCommandToReadableString", "tct");
            }
        }
    }
}