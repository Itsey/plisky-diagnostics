namespace Plisky.Diagnostics {

    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Known Commands represent commands that can be sent directly to the viewer
    /// </summary>
    public enum KnownCommand {

        /// <summary>
        /// Tell the viewer to increase the indent level on the following outputs
        /// </summary>
        IncreaseIndent,

        /// <summary>
        /// Tell the viewer to decrease the indent level on the following outputs
        /// </summary>
        DecreaseIndent,

        /// <summary>
        /// Tell the viewer to remove all curent trace data from its store
        /// </summary>
        PurgeAll,

        /// <summary>
        /// Tell the viewer to remove all current trace data for thsi current process from its store
        /// </summary>
        PurgeCurrent,

        /// <summary>
        /// Tell the viewer not to leave a line between this entry and the next entry.
        /// </summary>
        DoNotLeaveLine,

        /// <summary>
        /// Tells the viewer to start ignoring all messages after this one
        /// </summary>
        StartFilteringEvents,

        /// <summary>
        /// Tells the viewer to stop ignoring all messages after this one
        /// </summary>
        StopFilteringEvents,
    }

#pragma warning disable CS3009 // Type is not CLS-compliant

    /// <summary>
    /// Trace Command Types - represent all of the possible command types that can be issued as a trace command.
    /// </summary>
    [Flags]
    public enum TraceCommandTypes : uint {

        /// <summary>
        /// Standard Log Messages.
        /// </summary>
        LogMessage = 0x00000001,

        /// <summary>
        /// Verbose Log Messages.
        /// </summary>
        LogMessageVerb = 1 << 1,

        /// <summary>
        /// Minimal Log Messages.
        /// </summary>
        LogMessageMini = 1 << 2,

        /// <summary>
        /// Internal Messages.
        /// </summary>
        InternalMsg = 1 << 3,

        /// <summary>
        /// Trace Messages, Enter.
        /// </summary>
        TraceMessageIn = 1 << 4,

        /// <summary>
        /// Trace Messages, Exit.
        /// </summary>
        TraceMessageOut = 1 << 5,

        /// <summary>
        /// Trace Messages - Other.
        /// </summary>
        TraceMessage = 1 << 6,

        /// <summary>
        /// Assertion Failure.
        /// </summary>
        AssertionFailed = 1 << 7,

        /// <summary>
        /// Further Details to an existing message.
        /// </summary>
        MoreInfo = 1 << 8,

        /// <summary>
        /// Trace Display Commands Only.
        /// </summary>
        CommandOnly = 1 << 9,

        /// <summary>
        /// Errors.
        /// </summary>
        ErrorMsg = 1 << 10,

        /// <summary>
        /// Warnings.
        /// </summary>
        WarningMsg = 1 << 11,

        /// <summary>
        /// Exception Block OF Info
        /// </summary>
        ExceptionBlock = 1 << 12,  // used for exception type flag

        /// <summary>
        /// Exception Meta Data
        /// </summary>
        ExceptionData = 1 << 13,

        /// <summary>
        /// Exception Block Start
        /// </summary>
        ExcStart = 1 << 14,

        /// <summary>
        /// Exception Block End
        /// </summary>
        ExcEnd = 1 << 15,

        /// <summary>
        /// Section Start
        /// </summary>
        SectionStart = 1 << 16,

        /// <summary>
        /// Section End
        /// </summary>
        SectionEnd = 1 << 17,

        /// <summary>
        /// Resource Consumption - Developer Trace
        /// </summary>
        ResourceEat = 1 << 18,

        /// <summary>
        /// Resource Release - Developer Trace
        /// </summary>
        ResourcePuke = 1 << 19,

        /// <summary>
        /// Resource Current Value - Developer Trace
        /// </summary>
        ResourceCount = 1 << 20,

        /// <summary>
        /// Standard Message Type
        /// </summary>
        Standard = 1 << 21,

        /// <summary>
        /// XML Formattted Command Message
        /// </summary>
        CommandData = 1 << 22,

        /// <summary>
        /// Custom and Third party messages
        /// </summary>
        Custom = 1 << 23,

        /// <summary>
        /// Alerting and Notification
        /// </summary>
        Alert = 1 << 28,

        /// <summary>
        /// Unknown, error or invalid configuration.
        /// </summary>
        Unknown = 0x00000000
    }

#pragma warning restore CS3003 // Type is not CLS-compliant

    /// <summary>
    /// Class holding all of the constants that are used by the trace program.
    /// </summary>
    public static class Constants {

        /// <summary>
        /// assertion failed indicator
        /// </summary>
        public const string ASSERTIONFAILED = "#AST#";

        /// <summary>
        /// autom timer prefix for identification
        /// </summary>
        public const string AUTOTIMER_PREFIX = "TAT_PFX";

        /// <summary>
        /// Command string for the viewer to indicate that a global purge should be performed
        /// </summary>
        public const string CMD_CAUSEGLOBALPURGE = "%PUG%";

        /// <summary>
        /// Command string for the viewer to indicate that a purge of the current process should be performed
        /// </summary>
        public const string CMD_CAUSEPURGEINCURRENT = "%PUC%";

        /// <summary>
        /// Command string to decrease the indent of the viewer that is attached
        /// </summary>
        public const string CMD_DECREASEINDENT = "%DEC%";

        /// <summary>
        /// Command string to indicate to the viewer that no line should be left between this statement and the next
        /// </summary>
        public const string CMD_DONTLEAVELINE = "%NOC%";

        /// <summary>
        /// Command string to increase the indent of the viewer that is attached
        /// </summary>
        public const string CMD_INCREASEINDENT = "%INC%";

        /// <summary>
        /// command only indicator
        /// </summary>
        public const string COMMANDONLY = "#CMD#";

        /// <summary>
        /// default length for command strings
        /// </summary>
        public const int COMMANDSTRINGLENGTH = 5;

        /// <summary>
        /// Indicates data in message
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "False positive on concatenated constants")]
        public const string DATAINDICATOR = "~~DATA::";

        /// <summary>
        /// default ip address (to listen on)
        /// </summary>
        public const string DEFAULTIP_ADDRESS = "*";

        /// <summary>
        /// default port for trace
        /// </summary>
        public const int DEFAULTPORT_NUMBER = 9060;

        /// <summary>
        /// The name for the default listener
        /// </summary>
        public const string DEFLISTNER_NAME = "TrcDefaultListener";

        /// <summary>
        /// Loops that are based on recursion have depth protection enabled so that an assign innter to outer style
        /// nastyness does not break.  The output will be very ugly but it will be better to a stackoverflow
        /// excepiton, or nasty infinite looping thing.
        /// </summary>
        public const int DEPTHPROTECTION = 255;

        /// <summary>
        /// error indicator
        /// </summary>
        public const string ERRORMSG = "#ERR#";

        /// <summary>
        /// there is a limit to display text, this is most prevalant in evidence names which are built up from type names
        /// and therefore can easily exceed the length that they are sposed to be at.  This just visciously truncates them
        /// at that length.
        /// </summary>
        public const int EVIDENCENAMELENGTH = 50;

        /// <summary>
        /// exception end indicator
        /// </summary>
        public const string EXCEND = "#EXE#";

        /// <summary>
        /// exception indicator
        /// </summary>
        public const string EXCEPTIONBLOCK = "#EXC#";

        /// <summary>
        /// exception data indicator
        /// </summary>
        public const string EXCEPTIONDATA = "#EXD#";

        /// <summary>
        /// public identifier to mark the end of an exception set of log messages
        /// </summary>
        public const string EXCEPTIONENDTAG = "X#EXCEPTIONEND#X";

        /// <summary>
        /// Exception start indicator
        /// </summary>
        public const string EXCSTART = "#EXS#";

        /// <summary>
        ///  The name of the external text writer listener
        /// </summary>
        public const string FILELISTNER_NAME = "TextWriterTraceListener";

        /// <summary>
        /// Divider in the text
        /// </summary>
        public const string GENERIC_DIVIDER_MARKER = "#^#";

        /// <summary>
        /// Class name to replace for internal methods
        /// </summary>
        public const string INTERNAL_CLASS_REPLACE = "[X_TXCLS_X]";

        /// <summary>
        /// The string used to indicate that the class and method name need to replace this string in messages.
        /// </summary>
        public const string INTERNAL_CLASSMETHOD_REPLACE = "[X_TXCLS_::_TXMTH_X]";

        /// <summary>
        /// MEthod name to replace for internal methods
        /// </summary>
        public const string INTERNAL_METHOD_REPLACE = "[X_TXCLS_::_TXMTH_X]";

        /// <summary>
        /// method name indicator
        /// </summary>
        public const string INTERNAL_METHODTAGINDICATOR = "[X_";

        /// <summary>
        /// PArams name to replace for internal methods
        /// </summary>
        public const string INTERNAL_PARAMS_REPLACE = "[X_PRMS_X]";

        /// <summary>
        /// internal indicator
        /// </summary>
        public const string INTERNALMSG = "#INT#";

        /// <summary>
        /// Restriction on the length of messages that are sent as one hit.  This limit will cause the messages to be split into
        /// multiple chunks and then sent.This appears to be a limit of the OutputDebugString win32 api function of 4096 bytes
        /// </summary>
        public const int LIMIT_OUTPUT_DATA_TO = 4050;

        /// <summary>
        /// minimal indicator
        /// </summary>
        public const string LOGMESSAGEMINI = "#LGM#";

        /// <summary>
        /// The message truncate marker is placed at the start of truncation messages and at the end of messages that expect another message
        /// to follow them containing truncate data.  This had to be more uniqued up as we search for it now that it is no longer able to be
        /// used just as a start and the end of a string.  This is mostly used for pulling out the unqiue identifier that is used to join strings back
        /// together in the viewer.
        /// </summary>
        public const string MESSAGETRUNCATE = "#X~>TNKX<~X#";

        //public const string TRACEMESSAGE    = "#TRC#";

        /// <summary>
        /// more info indicator
        /// </summary>
        public const string MOREINFO = "#MOR#";

        /// <summary>
        /// alert indicator
        /// </summary>
        public const string MSGFMT_ALERT = "#ALT#";

        /// <summary>
        /// custom indicator
        /// </summary>
        public const string MSGFMT_CUSTOM = "#CUS#";

        /// <summary>
        /// Log indicator
        /// </summary>
        public const string MSGFMT_LOG = "#LOG#";

        /// <summary>
        /// verbose indicator
        /// </summary>
        public const string MSGFMT_LOGVERBOSE = "#LGV#";

        /// <summary>
        /// xml data indicator
        /// </summary>
        public const string MSGFMT_XMLCOMMAND = "#XCM#";

        /// <summary>
        /// Error messsage - not implemented
        /// </summary>
        public const string NOT_IMP_MESSAGE = "NotImplemented:  An area of code that has not yet been implemented has been executed.";

        /// <summary>
        /// The name of the specific Output debug string listener
        /// </summary>
        public const string ODSLISTNER_NAME = "Trc_OdsListener";

        // Had to increase the uniqueness of this marker.
        // These must be used in a switch statment therefore cant be readonly statics
        /// <summary>
        /// The name for the VS.Net 2003 default listener
        /// </summary>
        public const string ORIGINALLISTENER_NAME = "Default";

        /// <summary>
        /// direct resource count indicator
        /// </summary>
        public const string RESOURCECOUNT = "#RSC#";

        /// <summary>
        /// resource consumed indicator
        /// </summary>
        public const string RESOURCEEAT = "#REA#";

        /// <summary>
        /// resource returned indicator
        /// </summary>
        public const string RESOURCEPUKE = "#RPU#";

        /// <summary>
        /// When two messages are passed into a trace string they are separated like this for sending.
        /// </summary>
        public const string SECONDARYSTRINGSEPARATOR = "~~#~~";

        /// <summary>
        /// section end indicator
        /// </summary>
        public const string SECTIONEND = "#SXX#";

        /// <summary>
        /// section start indicator
        /// </summary>
        public const string SECTIONSTART = "#SEC#";

        /// <summary>
        /// tcp stream end marker indicator
        /// </summary>
        public const string TCPEND_MARKERTAG = "#TCPLIST-END#";

        /// <summary>
        /// length of the tcp end marker tag
        /// </summary>
        public const int TCPEND_MARKERTAGLEN = 13;

        /// <summary>
        /// USed when identifying threads
        /// </summary>
        public const string THREADINITIDENTIFIER = ">TRCTNM<";

        /// <summary>
        /// Timer section identifier
        /// </summary>
        public const string TIMER_SECTIONIDENTIFIER = "|TMRCHK|";

        /// <summary>
        /// timer string delimiter end
        /// </summary>
        public const string TIMER_STRINGENDDELIMITER = "#X_X#]";

        /// <summary>
        /// Timer string constant
        /// </summary>
        public const string TIMER_STRINGSTARTCONTENTSTRING = "TMRDATA" + TIMER_STRINGSTARTDELIMITER;

        // Timers use sections for sending to Mex therefore this identifies a timer rather than a normal section
        /// <summary>
        /// timer string delimiter start
        /// </summary>
        public const string TIMER_STRINGSTARTDELIMITER = "[#X_X#";

        // Sections are used for timings now too.
        /// <summary>
        /// default timer name
        /// </summary>
        public const string TIMERNAME = "BilgeTimer";

        // Some special more info commands send data to the viewer rather than debug messages
        /// <summary>
        /// trace in indicator
        /// </summary>
        public const string TRACEMESSAGEIN = "#TRI#";

        /// <summary>
        /// trace out indator
        /// </summary>
        public const string TRACEMESSAGEOUT = "#TRO#";

        /// <summary>
        /// The truncate data end marker identifies where the end of the data that is associated with the truncation happens and therefore
        /// where the start of the actual truncated data begins.  Immediately following the last # the data begins.
        /// </summary>
        public const string TRUNCATE_DATAENDMARKER = "]#E#";

        // TCPEND_MARKERTAG.Length doesent work.  Used to work in uber.
        /// <summary>
        /// Error message - unreachable code
        /// </summary>
        public const string URC_CODE_MESSAGE = "Unreachable Code Assertion Failure.  This code should not logically have been executed.";

        /// <summary>
        /// Error message - wrong code path
        /// </summary>
        public const string URC_CODECTXT_MESSAGE = "A code path that was never expected to be possible has been reached.  The developer has not expected this eventuality and therefore the results of continuing are unpredicatable.";

        /// <summary>
        /// warning indicator
        /// </summary>
        public const string WARNINGMSG = "#WRN#";

        // When an assertion occurs the full assertion is sent to the trace stream as a single assertion message and some accompanying data
        // the accompanying data looks like assertion data but has this tag on the front of the message.
        internal const string ASSERTIONRECREATESTRING = "ASSERTDATA|";

        /// <summary>
        /// The formatted resource string.
        /// </summary>
        internal const string FORMATTEDRESOURCESTRING = RESPACKSTRINGIDENT + RESNAMEDELIMITER + "{0}" + RESNAMEDELIMITER + RESVALUEDELIMITER + "{1}" + RESVALUEDELIMITER + RESCONTEXTDELIMITER + "{2}" + RESCONTEXTDELIMITER;

        /// <summary>
        /// The internal string used when no further infromation is available.
        /// </summary>
        internal const string NFI = "No further information";

        /// <summary>
        /// Postfix to the process name.
        /// </summary>
        internal const string PROCNAMEIDENT_POSTFIX = ")";

        /// <summary>
        /// Prefix to the process name.
        /// </summary>
        internal const string PROCNAMEIDENT_PREFIX = "ProcessName(";

        /// <summary>
        /// Delimiter for resource context.
        /// </summary>
        internal const string RESCONTEXTDELIMITER = "/RCTXT/";

        /// <summary>
        /// Delimiter for reosurce names.
        /// </summary>
        internal const string RESNAMEDELIMITER = "/RNAME/";

        /// <summary>
        /// Length of the name delimiter.
        /// </summary>
        internal const int RESNAMEDELIMITERLENGTH = 7;

        // The resource commands send name value pairs through in the debug message to indicate consumption and releases of
        // resources.  They can also send constant resource values through for the two different types of resource.
        // these strings are used as markers within the debug message to hold this information therefore are used by the consumer and
        // the viewer.
        internal const string RESPACKSTRINGIDENT = "RESIDENTSTRING|";

        /// <summary>
        /// Delimiter for resource values.
        /// </summary>
        internal const string RESVALUEDELIMITER = "/RVALUE/";

        // Each TraceInternal message type has an identifier associated with it so that the viewer can
        // process it differently if required.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Looks like a false positive, tpc_constructandinit - initialise method line 157")]
        internal const string TIMF_INITIALISECALLEDAGAIN = "**001**";  // Initialise called 2x for same process. Common in asp.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Looks like a false positive, tpc_constructandinit - initialise method line 157")]
        internal const string TIMF_INITIALISECALLEDONCE = "**002**";  // Initialise called for the first time
    }

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