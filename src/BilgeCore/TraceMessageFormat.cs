namespace Plisky.Diagnostics {

    using System;
    using System.Text.RegularExpressions;

    // This class is shared across two projects some of which use some parts and not others.
    // #pragma warning disable 0169

    /// <summary>
    /// The trace message format
    /// </summary>
    public sealed class TraceMessageFormat {

        /// <summary>
        /// exception command identifier
        /// </summary>
        public const uint EXCEPTIONCOMMANDS = 0x0000F000;

        /// <summary>
        /// A valid regex
        /// </summary>
        // TODO: This is a buggy regex, it matches too well - even non numeric PIDs are matched and they shouldnt be.
        public const string IS_VALID_TEXTSTRING_REGEX = @"\{\[[0-9A-Za-z\._]{0,}\]\[[0-9]{1,}\]\[[0-9]{1,}\]\[[0-9A-Za-z\._]{0,}\]\[[0-9A-Za-z\._\:\\-]{0,}\]\[[0-9]{0,8}\]\[[0-9A-Za-z\.\:_<>`]{0,}\]\}\#[A-Z]{3,3}\#";

        /// <summary>
        /// log command identifier
        /// </summary>
        public const uint LOGCOMMANDS = 0x00000007;

        /// <summary>
        /// Resource command identifier
        /// </summary>
        public const uint RESOURCECOMMANDS = 0x001C0000;

        /// <summary>
        /// section commmand identifier
        /// </summary>
        public const uint SECTIONCOMMANDS = 0x00030000;

        /// <summary>
        /// Older string support
        /// </summary>
        public const string SUPPORTED_TEX_LEGACYREGEX = @"\{\[[0-9A-Za-z\._]{0,}\]\[[0-9]{1,5}\]\[[0-9]{1,5}\]\[[0-9A-Za-z\.]{0,}\]\[[0-9]{0,8}\]\}\#[A-Z]{3,3}\#";

        /// <summary>
        /// Trace command identifier
        /// </summary>
        public const uint TRACECOMMANDS = 0x00000070;

        /// <summary>
        /// This regex will match if the string that is tested against it is a fully qualified and valid Tex String, it will attempt to fail any non valid tex
        /// strings.
        /// </summary>
        private static Regex cachedRegex;

        private static Regex partRegexCache;

        // Added support for older strings so can use the new viewer with older data"
        // NB Legacy does not support 64 bit apps, pids and proc ids limited to 5 digit
        private static Regex cachedTexLegacyRegex;

        // stop auto constructor
        private TraceMessageFormat() {
        }

        // Timer message strings
        // two strings - string 1 is for display string 2 is timer data
        // format TMRDATA[#X#TimerInstanceTitle#X#][#X#TimerSinkTitle#X#][#X#TimeStart#X#]

        // timeString = ">TRCTMR<|SRT|" + dt.Day + "|" + dt.Month + "|" + dt.Year + "|" + dt.Hour + "|" + dt.Minute + "|" + dt.Second + "|" + dt.Millisecond + "|";
        // internal static Regex m_timerPartsCache = new Regex(@"\[#X_X#[0-9A-Za-z,&\.:_()\s-|\[\]]{0,}#X_X#\]", RegexOptions.Compiled);

        /// <summary>
        /// Convert meta data to the string
        /// </summary>
        /// <param name="mp">A structure containing the message parts</param>
        /// <returns>A string formatted in a way that FlimFlam is able to read it</returns>
        public static string AssembleFormattedStringFromMessageStructure(MessageParts mp) {
            string result;

            if (mp.SecondaryMessage.Length > 0) {
                result = "{[" + mp.MachineName + "][" + mp.ProcessId + "][" + mp.OSThreadId + "][" + mp.NetThreadId + "][" + mp.ModuleName + "][" + mp.LineNumber + "][" + mp.AdditionalLocationData + "]}" + mp.MessageType + mp.DebugMessage + Constants.SECONDARYSTRINGSEPARATOR + mp.SecondaryMessage;
            } else {
                result = "{[" + mp.MachineName + "][" + mp.ProcessId + "][" + mp.OSThreadId + "][" + mp.NetThreadId + "][" + mp.ModuleName + "][" + mp.LineNumber + "][" + mp.AdditionalLocationData + "]}" + mp.MessageType + mp.DebugMessage;
            }
            return result;
        }

        /// <summary>
        /// builds up the string
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <param name="context">The context that the resource is used in</param>
        /// <param name="valueStr">The change in value</param>
        /// <returns>An assembled string representing the change</returns>
        public static string AssembleResourceContentString(string name, string context, string valueStr) {
            string result = string.Format(Constants.FORMATTEDRESOURCESTRING, name, valueStr, context);

#if DEBUG
            // Make sure the assemble and split represent one another.

            if (!SplitResourceContentStringByParts(result, out string tname, out string tcontext, out string tvalue)) {
                throw new InvalidOperationException("The splitting of a resource string that has just been assembled did not produce consistant results");
            }

            if (tname != name) {
                throw new InvalidOperationException("The splitting of a resource string that has just been assembled messed up the resource name");
            }
            if (tcontext != context) {
                throw new InvalidOperationException("The splitting of a resource string that has just been assembled messed up the context");
            }
            if (tvalue != valueStr) {
                throw new InvalidOperationException("The splitting of a resource string that has just been assembled messed up the value");
            }

#endif
            return result;
        }

        /// <summary>
        /// put together the timer string
        /// </summary>
        /// <param name="timerInstancetitle">The instance title for the timer string.</param>
        /// <param name="timerSinkTitle">The sink title for the timer string.</param>
        /// <param name="timeValue">The value of the time itself</param>
        /// <returns>An assembled timer string</returns>
        public static string AssembleTimerContentString(string timerInstancetitle, string timerSinkTitle, DateTime timeValue) {
            string cd = Constants.TIMER_STRINGENDDELIMITER + Constants.TIMER_STRINGSTARTDELIMITER;
            string timeRepresentation = PackageDateTimeForTexString(timeValue);
            string result = Constants.TIMER_STRINGSTARTCONTENTSTRING + timerInstancetitle + cd + timerSinkTitle + cd + timeRepresentation + Constants.TIMER_STRINGENDDELIMITER;

            return result;
        }

        /// <summary>
        /// true if exception
        /// </summary>
        /// <param name="tct">the trace command type to check</param>
        /// <returns>true if the command represents an exception command</returns>
        public static bool IsExceptionCommand(TraceCommandTypes tct) {
            return ((uint)tct & EXCEPTIONCOMMANDS) == (uint)tct;
        }

        /// <summary>
        /// Is this an older string
        /// </summary>
        /// <param name="theString">the string to test</param>
        /// <returns>true if this represents a legacy tex string</returns>
        public static bool IsLegacyTexString(string theString) {
            if ((theString == null) || (theString.Length == 0)) { return false; }
            if (!theString.StartsWith("{[")) { return false; }

            if (cachedTexLegacyRegex == null) {
                cachedTexLegacyRegex = new Regex(SUPPORTED_TEX_LEGACYREGEX, RegexOptions.Compiled);
            }

            return cachedTexLegacyRegex.IsMatch(theString);
        }

        /// <summary>
        /// true if log
        /// </summary>
        /// <param name="tct">The trace command type to compare against a log command</param>
        /// <returns>true if the trace command type represents a log command</returns>
        public static bool IsLogMessageCommand(TraceCommandTypes tct) {
            return ((uint)tct & LOGCOMMANDS) == (uint)tct;
        }

        /// <summary>
        /// true if resource
        /// </summary>
        /// <param name="tct">The trace command type to compare against a resource command</param>
        /// <returns>true if the trace command type represents a resoure command</returns>
        public static bool IsResourceCommand(TraceCommandTypes tct) {
            return ((uint)tct & RESOURCECOMMANDS) == (uint)tct;
        }

        /// <summary>
        /// true if section
        /// </summary>
        /// <param name="tct">The trace command type to compare against a section command</param>
        /// <returns>True if the TCT represents a section command</returns>
        public static bool IsSectionCommand(TraceCommandTypes tct) {
            return ((uint)tct & SECTIONCOMMANDS) == (uint)tct;
        }

        /// <summary>
        /// This method will check the passed string to see if the string passed is not a tex string, it is optimised to fail as soon as possible therefore only guarantees
        /// that the string passed does not look like a tex string
        /// </summary>
        /// <param name="theString">The string to verify whether its tex compatible or not</param>
        /// <returns>true if its a tex format string</returns>
        public static bool IsTexString(string theString) {
            if ((theString == null) || (theString.Length == 0)) { return false; }
            if (!theString.StartsWith("{[")) { return false; }

            if (cachedRegex == null) {
                cachedRegex = new Regex(IS_VALID_TEXTSTRING_REGEX, RegexOptions.Compiled);
            }

            return cachedRegex.IsMatch(theString);
        }

        /// <summary>
        /// true if trace
        /// </summary>
        /// <param name="tct">the trace command type to test</param>
        /// <returns>true if this represents a trace command</returns>
        public static bool IsTraceCommand(TraceCommandTypes tct) {
            return ((uint)tct & TRACECOMMANDS) == (uint)tct;
        }

        /// <summary>
        /// package date format string
        /// </summary>
        /// <param name="sendme">The date time to package</param>
        /// <returns>a string representing the time and date</returns>
        public static string PackageDateTimeForTexString(DateTime sendme) {
            // have had all sort of issues with date tiem formatting and settings and timezones etc. Gone to this.
            return string.Format("|DTFMT1|{0}|{1}|{2}|{3}|{4}|{5}|{6}|", sendme.Day, sendme.Month, sendme.Year, sendme.Hour, sendme.Minute, sendme.Second, sendme.Millisecond);
        }

        /// <summary>
        /// This identifys the internal structure of the trace message.  The helper functions here
        /// depend on this internal structure and are all kept here.  There should be no dependance
        /// on the structure of the string outside of thsi class.
        ///
        /// {[MACHINENAME][PROCESSID][THREADID][NETTHREADID][MODULENAME][LINENUMBER][MOREDATA]}#CMD#TEXTOFDEBUGSTRING
        ///
        /// Where MACHINENAME = Current machine name taken from Environment
        /// Where PROCESSID   = The PID assigned to the process that outputed the string
        /// where THREADID    = The numeric ID assigned to the OS Thread running the commands
        /// where NETTHREADID = The name of the .net thread running the command.
        /// where MODULENAME  = The cs filename that was executing the commands
        /// where LINENUMBER  = the numeric line number that the debug string was written from
        /// where MOREDATA    = this is additional location data, using the form Class::Method when called within Tex.
        /// NB Future enhancement 1 :
        ///
        /// </summary>
        /// <param name="debugString">The primary log message to split into its parts.</param>
        /// <param name="cmdType">The type of log message being written</param>
        /// <param name="processId">The affected process identity</param>
        /// <param name="netThreadId">The .net thread identity</param>
        /// <param name="machineName">The host machine identity</param>
        /// <param name="threadId">The underlying system thread identity</param>
        /// <param name="moduleName">The filename where the code is executed from</param>
        /// <param name="lineNumber">The line number where the code is executed from</param>
        /// <param name="moreLocInfo">supporting location data</param>
        /// <param name="debugOutput">the body of the message</param>
        public static void ReturnPartsOfString(string debugString, out string cmdType, out string processId, out string netThreadId, out string machineName, out string threadId,
          out string moduleName, out string lineNumber, out string moreLocInfo, out string debugOutput) {
            processId = null;
            machineName = null;
            threadId = null;
            debugOutput = null;
            moreLocInfo = null;
            netThreadId = null;

            if (partRegexCache == null) {
                partRegexCache = new Regex(@"\[[0-9A-Za-z\.:<>`_]{0,}\]", RegexOptions.Compiled);
            }

            var m = partRegexCache.Match(debugString);
            // This should return 5 matches for a legit debug string

            // Get each of the location identifiers from the string. - removing the surrouding
            // [] delimiters from each of the values.
            machineName = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            processId = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            threadId = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            netThreadId = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            moduleName = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            lineNumber = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            moreLocInfo = m.Captures[0].Value.Trim(new char[] { '[', ']' });

            // Now get the command type and turn it into an enum
            var cmdMatch = Regex.Match(debugString, "#[A-Z]{3,3}#");
            cmdType = cmdMatch.Captures[0].Value;

            // finally get the rest of the string as the debug message
            debugOutput = debugString.Substring(cmdMatch.Index + Constants.COMMANDSTRINGLENGTH);  // commandstrlen currently 5
        }

        /// <summary>
        /// This identifys the internal structure of the trace message.  The helper functions here
        /// depend on this internal structure and are all kept here.  There should be no dependance
        /// on the structure of the string outside of thsi class.
        ///
        /// {[MACHINENAME][PROCESSID][THREADID][MODULENAME][LINENUMBER][MOREDATA]}#CMD#TEXTOFDEBUGSTRING
        ///
        /// Where MACHINENAME = Current machine name taken from Environment
        /// Where PROCESSID   = The PID assigned to the process that outputed the string
        /// where THREADID    = The numeric ID assigned to the OS Thread running the commands
        /// where MODULENAME  = The cs filename that was executing the commands
        /// where LINENUMBER  = the numeric line number that the debug string was written from.
        /// </summary>
        /// <param name="debugString">The string to log out.</param>
        /// <param name="cmdType">The type of trace command.</param>
        /// <param name="processID">The process identifier.</param>
        /// <param name="machineName">The name of the machine doing the tracing.</param>
        /// <param name="threadID">The thread identity.</param>
        /// <param name="moduleName">The module name.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="debugOutput">the debugOutput</param>
        public static void ReturnPartsOfStringLegacy(string debugString, out string cmdType, out string processID, out string machineName, out string threadID,
          out string moduleName, out string lineNumber, out string debugOutput) {
            processID = string.Empty;
            machineName = string.Empty;
            threadID = string.Empty;
            debugOutput = string.Empty;

            if (partRegexCache == null) {
                partRegexCache = new Regex(@"\[[0-9A-Za-z\.:_]{0,}\]", RegexOptions.Compiled);
            }

            var m = partRegexCache.Match(debugString);
            // This should return 5 matches for a legit debug string

            // TODO I believe this deletes string names where [] is passed as initial or final chars test and fix.

            // Get each of the location identifiers from the string. - removing the surrouding
            // [] delimiters from each of the values.
            machineName = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            processID = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            threadID = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            moduleName = m.Captures[0].Value.Trim(new char[] { '[', ']' });
            m = m.NextMatch();
            lineNumber = m.Captures[0].Value.Trim(new char[] { '[', ']' });

            // Now get the command type and turn it into an enum
            var cmdMatch = Regex.Match(debugString, "#[A-Z]{3,3}#");
            cmdType = cmdMatch.Captures[0].Value;

            // finally get the rest of the string as the debug message
            debugOutput = debugString.Substring(cmdMatch.Index + Constants.COMMANDSTRINGLENGTH);  // commandstrlen currently 5
        }

        /// <summary>
        /// Splits the content
        /// </summary>
        /// <param name="packedString">the packed string</param>
        /// <param name="resourceName">the name of the resource</param>
        /// <param name="resourceIdent">the identifier for the resource</param>
        /// <param name="resourceValueRepresentation">the value of the resource</param>
        /// <returns>true if the parse worked</returns>
        public static bool SplitResourceContentStringByParts(string packedString, out string resourceName, out string resourceIdent, out string resourceValueRepresentation) {
            resourceName = resourceIdent = resourceValueRepresentation = null;

            if (!packedString.StartsWith(Constants.RESPACKSTRINGIDENT)) {
                return false;
            }

            // Looks valid ish.  Time to split the string into its resorces parts.

            // Chop the initial marker and the first NAME delimiter tag off the string.
            // String goes from MARKER DELIMITER1 NAME DELIMITER1 DELIMITER2 VALUE DELMITER2 DELIMITER3 CONTEXT DELIMITER 3 to
            //                                    NAME DELIMITER1 DELIMITER2 VALUE DELMITER2 DELIMITER3 CONTEXT DELIMITER 3
            int offset = Constants.RESPACKSTRINGIDENT.Length + Constants.RESNAMEDELIMITER.Length;
            string temp = packedString.Substring(offset);

            // Now copy up to the name / value delimiter pair
            offset = temp.IndexOf(Constants.RESNAMEDELIMITER + Constants.RESVALUEDELIMITER);
            resourceName = temp.Substring(0, offset);
            offset += Constants.RESNAMEDELIMITER.Length + Constants.RESVALUEDELIMITER.Length;

            // String goes from NAME DELIMITER1 DELIMITER2 VALUE DELMITER2 DELIMITER3 CONTEXT DELIMITER 3 to
            //                  VALUE DELMITER2 DELIMITER3 CONTEXT DELIMITER 3
            temp = temp.Substring(offset);

            offset = temp.IndexOf(Constants.RESVALUEDELIMITER + Constants.RESCONTEXTDELIMITER);
            resourceValueRepresentation = temp.Substring(0, offset);
            offset += Constants.RESVALUEDELIMITER.Length + Constants.RESCONTEXTDELIMITER.Length;

            // String goes from NAME DELIMITER1 DELIMITER2 VALUE DELMITER2 DELIMITER3 CONTEXT DELIMITER 3 to
            //                  CONTEXT DELIMITER 3
            temp = temp.Substring(offset);

            offset = temp.IndexOf(Constants.RESCONTEXTDELIMITER);
            resourceIdent = temp.Substring(0, offset);

            return true;
        }

        /// <summary>
        /// split a timer string
        /// </summary>
        /// <param name="timerString">The timer string to split</param>
        /// <param name="timerInstanceTitle">The instance title</param>
        /// <param name="timerSinkTitle">The timer sink title</param>
        /// <param name="timeValue">The timer value</param>
        /// <returns>True if the split worked</returns>
        public static bool SplitTimerStringByParts(string timerString, out string timerInstanceTitle, out string timerSinkTitle, out DateTime timeValue) {
            timerInstanceTitle = timerSinkTitle = string.Empty;
            timeValue = DateTime.MinValue;

            if (!timerString.StartsWith(Constants.TIMER_STRINGSTARTCONTENTSTRING)) { return false; }

            string tempForDatetime;

            var timerRegexCache = new Regex(@"\[#X_X#[0-9A-Za-z,&\.;\^£:_()<>`\s-|\[\]]{0,}#X_X#\]", RegexOptions.Compiled);
            var m = timerRegexCache.Match(timerString);
            // This should return 5 matches for a legit debug string

            int lengthOfSurrounds = Constants.TIMER_STRINGSTARTDELIMITER.Length + Constants.TIMER_STRINGENDDELIMITER.Length;
            int offsetIntoString = Constants.TIMER_STRINGSTARTDELIMITER.Length;

            timerInstanceTitle = m.Captures[0].Value;
            timerInstanceTitle = timerInstanceTitle.Substring(offsetIntoString, timerInstanceTitle.Length - lengthOfSurrounds);

            m = m.NextMatch();
            timerSinkTitle = m.Captures[0].Value;
            timerSinkTitle = timerSinkTitle.Substring(offsetIntoString, timerSinkTitle.Length - lengthOfSurrounds);

            m = m.NextMatch();
            tempForDatetime = m.Captures[0].Value;
            tempForDatetime = tempForDatetime.Substring(offsetIntoString, tempForDatetime.Length - lengthOfSurrounds);
            timeValue = UnpackageDateTimeFromTexString(tempForDatetime);

            return true;
        }

        /// <summary>
        /// unpackage string format date time
        /// </summary>
        /// <param name="packaged">The packaged string</param>
        /// <returns>returns the extracted date time</returns>
        public static DateTime UnpackageDateTimeFromTexString(string packaged) {
            // have had all sort of issues with date tiem formatting and settings and timezones etc. Gone to this.
            if (!packaged.StartsWith("|DTFMT1|")) {
                throw new InvalidOperationException("Not the right type of date format string to work with");
            }

            DateTime result;
            string[] dtParts = packaged.Substring(8).Split('|');
            // Format>>                      yy                          mm               dd                      hh                 mins               secs                          milisecs
            result = new DateTime(int.Parse(dtParts[2]), int.Parse(dtParts[1]), int.Parse(dtParts[0]), int.Parse(dtParts[3]), int.Parse(dtParts[4]), int.Parse(dtParts[5]), int.Parse(dtParts[6]));
            return result;
        }

#pragma warning disable CS3003 // Type is not CLS-compliant
#pragma warning restore CS3003 // Type is not CLS-compliant
    }

    // #pragma warning restore 0169
}