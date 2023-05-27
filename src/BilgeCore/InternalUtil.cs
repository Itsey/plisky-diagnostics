namespace Plisky.Plumbing {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Internal utilities
    /// </summary>
    public static class InternalUtil {

        /// <summary>
        /// current trace level
        /// </summary>
        public static TraceLevel internalTraceLevel = TraceLevel.Off;     // Used Internally to write to event log.

        private const string EVENTSOURCEIDENTIFIER = "-= Bilge =-";
        private const string EVENTSOURCETARGET = "Application";
        private static bool sourceExists;


        /// <summary>
        /// Util to get class name
        /// </summary>
        /// <param name="theMethod"></param>
        /// <returns></returns>
        public static string GetClassNameFromMethodbase(MethodBase theMethod) {
            // strangely the ReflectedType can return null for some Microsoft generated classes, this was first noticed
            // during a bug when tracing in some WCF classes, therefore this has been abstracted into a method that expects this.

            string result = "*Unknown*";
            if ((theMethod != null) && (theMethod.ReflectedType != null) && (theMethod.ReflectedType.Name != null)) {
                result = theMethod.ReflectedType.Name;
            }
            return result;
        }

        /// <summary>
        /// Cached assembly
        /// </summary>
        public static Assembly TraceAssemblyCache = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Util to convert stack to xml
        /// </summary>
        /// <param name="topMost"></param>
        /// <returns></returns>
        public static string GetFullXmlStacktraceWithoutTraceWrappings(out string topMost) {
            return GetXMLStackTrace(out topMost);
        }

        private static string GetXMLStackTrace(out string topMost) {
            topMost = null;
            var final = new XDocument();

            var currentStack = new StackTrace(true);

            final.Add(new XElement("stack"));

            for (int frameIdx = 0; frameIdx < currentStack.FrameCount; frameIdx++) {
                var nextFrame = currentStack.GetFrame(frameIdx);
                string methodName;
                var theMethod = nextFrame.GetMethod();

                if (theMethod.DeclaringType != null) {
                    if (theMethod.DeclaringType.Assembly == TraceAssemblyCache) {
                        // Skip methods which are in this assembly.
                        continue;
                    }
                }

                methodName = GetClassNameFromMethodbase(theMethod);

                var nextPart = new StringBuilder();

                string working = methodName + "." + theMethod.Name;
                var frame = new XElement("frm");
                final.Element("stack").Add(frame);

                if ((topMost == null) || (!string.IsNullOrEmpty(nextFrame.GetFileName()))) {
                    // First one but moves up stack so long as we have filenames.
                    topMost = working;
                }
                nextPart.Append(working);
                nextPart.Append('(');

                // Now locate and append the parameteres.
                var methodParameters = theMethod.GetParameters();
                for (int i = 0; i < methodParameters.Length; i++) {
                    var currentParameter = methodParameters[i];
                    nextPart.Append(currentParameter.ParameterType.Name);
                    nextPart.Append(' ');
                    nextPart.Append(currentParameter.Name);
                    if (i != (methodParameters.Length - 1)) {
                        nextPart.Append(", ");
                    }
                }

                string theFileName = Path.GetFileName(nextFrame.GetFileName());
                nextPart.Append(")");

                frame.Add(new XElement("call", nextPart.ToString()));
                frame.Add(new XElement("fle", theFileName));
                frame.Add(new XElement("ln", nextFrame.GetFileLineNumber().ToString()));
            }

            return final.ToString();
        }



        private static string GetStackTrace(out string topMost, bool inXml = false) {
            topMost = null;
            var finalString = new StringBuilder();
            var currentStack = new StackTrace(true);

            if (inXml) {
                finalString.Append("<stack>");
            }

            for (int frameIdx = 0; frameIdx < currentStack.FrameCount; frameIdx++) {
                var nextFrame = currentStack.GetFrame(frameIdx);
                string methodName;
                var theMethod = nextFrame.GetMethod();

                if (theMethod.DeclaringType != null) {
                    if (theMethod.DeclaringType.Assembly == TraceAssemblyCache) {
                        // Skip methods which are in this assembly.
                        continue;
                    }
                }

                methodName = GetClassNameFromMethodbase(theMethod);

                var nextPart = new StringBuilder();

                string working = methodName + "." + theMethod.Name;
                if (inXml) {
                    nextPart.Append("<frm><call>");

                    nextPart.Append(working.Replace("<", "&#x60").Replace(">", "&#x62;"));
                }
                if ((topMost == null) || (!string.IsNullOrEmpty(nextFrame.GetFileName()))) {
                    // First one but moves up stack so long as we have filenames.
                    topMost = working;
                }
                nextPart.Append(working);
                nextPart.Append("(");

                // Now locate and append the parameteres.
                var theParameters = theMethod.GetParameters();
                for (int i = 0; i < theParameters.Length; i++) {
                    var currentParameter = theParameters[i];
                    nextPart.Append(currentParameter.ParameterType.Name);
                    nextPart.Append(" ");
                    nextPart.Append(currentParameter.Name);
                    if (i != (theParameters.Length - 1)) {
                        nextPart.Append(", ");
                    }
                }

                string theFileName = Path.GetFileName(nextFrame.GetFileName());
                nextPart.Append(")");
                if (inXml) {
                    nextPart.Append("</call><fle>" + theFileName + "</fle><ln>" + nextFrame.GetFileLineNumber().ToString() + "</ln>");
                } else {
                    nextPart.Append(" @" + theFileName + ":");
                    nextPart.Append(nextFrame.GetFileLineNumber().ToString() + "\r\n");
                }

                if (inXml) {
                    nextPart.Append("</frm>");
                }
                finalString.Append(nextPart.ToString());
            }
            if (inXml) {
                finalString.Append("</stack>");
            }

            return finalString.ToString();
        }

        /// <summary>
        /// Primarily an internal function used to generate a stack trace, although available on the public interface for more
        /// general usage.  This function creates a stack from the current point in code and will remove all LPSTrace related
        /// functions from the resulting stack string.  Therefore the data captured is anything in the call stack from above the
        /// T class related functions
        /// </summary>
        /// <returns>String containing the newly created stack information</returns>
        public static string StackToString() {
            return GetStackTrace(out string top, false);
        }

      
        /// <summary>
        /// Called to report an error within Tex, this will attempt to notify
        /// the user of an error, by writing to the event log or screen
        /// </summary>
        /// <param name="tie">InternalErrorCodes enum indicating the error</param>
        /// <param name="param">An Additional parameter to describe the error more fully.</param>
        public static void LogInternalError(InternalErrorCodes tie, string param) {
            LogInternalError(tie, param, TraceLevel.Off);
        }

#if PLISKYNETFRAMEWORK
        /// <summary>
        /// Attempts to write an error into the application event log, this will try and create the source if the source can not be found
        /// but any errors will be masked.  If for some reason we are unable to write to the event log then the exception is swallowed and
        /// excecution continues.
        /// </summary>
        /// <param name="severity">The EvenntLogEntryType level of the message</param>
        /// <param name="messsage">The message to write.</param>
        /// <param name="number">The number of the error</param>
        private static void WriteToEventLog(EventLogEntryType severity, string messsage, int number) {
            try {
                if ((!sourceExists) && (!EventLog.SourceExists(EVENTSOURCEIDENTIFIER))) {
                    EventLog.CreateEventSource(EVENTSOURCEIDENTIFIER, EVENTSOURCETARGET);
                } else {
                    sourceExists = true;
                }

                EventLog.WriteEntry(EVENTSOURCEIDENTIFIER, messsage, severity, number);

                //There are several different errors, however we have no way of reporting then therefore we just ignore them.
            } catch (SecurityException) {
            } catch (ArgumentException) {
            } catch (InvalidOperationException) {
            } catch (Win32Exception) {
            }
        }

        /// <summary>
        /// Called to report an error, this will attempt to notify
        /// the user of an error, by writing to the event log or screen
        /// </summary>
        /// <param name="tie">InternalError enum indicating the error</param>
        /// <param name="param">An Additional parameter to describe the error more fully.</param>
        /// <param name="overrideLevel">Allows you to override the severity</param>
        public static void LogInternalError(InternalErrorCodes tie, string param, TraceLevel overrideLevel) {
            if (internalTraceLevel == TraceLevel.Off) { return; }

            if (param == null) { throw new InvalidOperationException("ASSERTION:  The parameter should be empty if its not specified"); }

            string errorMessage = string.Empty;
            EventLogEntryType severity = EventLogEntryType.Information;
            GetErrorCode(tie, ref errorMessage, ref severity);

            if (overrideLevel != TraceLevel.Off) {
                switch (overrideLevel) {
                    case TraceLevel.Error: severity = EventLogEntryType.Error; break;
                    case TraceLevel.Info: severity = EventLogEntryType.Information; break;
                    case TraceLevel.Verbose: severity = EventLogEntryType.SuccessAudit; break;
                    case TraceLevel.Warning: severity = EventLogEntryType.Warning; break;
                }
            }

            switch (internalTraceLevel) {
                case TraceLevel.Error:
                    if ((severity == EventLogEntryType.Information) || (severity == EventLogEntryType.SuccessAudit) ||
                        (severity == EventLogEntryType.Warning)) { return; }
                    break;

                case TraceLevel.Info:
                    if (severity == EventLogEntryType.SuccessAudit) { return; }
                    break;

                case TraceLevel.Warning:
                    if ((severity == EventLogEntryType.Information) || (severity == EventLogEntryType.SuccessAudit)) { return; }
                    break;

                default:
                    break;
            }

            errorMessage = string.Format(errorMessage, param);
            WriteToEventLog(severity, errorMessage, (int)tie);
        }

            private static void GetErrorCode(InternalErrorCodes tie, ref string errorMessage, ref EventLogEntryType severity) {

        #region long switch statement to assign strings and severitys

            switch (tie) {
                case InternalErrorCodes.CorruptConfiguration:
                    errorMessage = "The configuration is corrupt. Initialisation has not successfully completed. {0}";
                    severity = EventLogEntryType.Error;
                    break;

                case InternalErrorCodes.NoListenersWarning:
                    errorMessage = "There are no listeners currently attached, Output will be lost. {0}";
                    severity = EventLogEntryType.Warning;
                    break;

                case InternalErrorCodes.ListenerCountInformation:
                    severity = EventLogEntryType.Information;
                    break;

                case InternalErrorCodes.InitialisationError:
                    errorMessage = "There was an error during Initialisation, it is likely that the configuration is not correctly applied.  {0}";
                    severity = EventLogEntryType.Error;
                    break;

                case InternalErrorCodes.AccessDeniedToSomething:
                    errorMessage = "Access Was Denied, Unable to retrieve some infomration.  {0}";
                    severity = EventLogEntryType.Warning;
                    break;

                case InternalErrorCodes.OperatingSystemIncompatibility:
                    errorMessage = "The operating system was incompatible with the requested operation. {0}";
                    severity = EventLogEntryType.Warning;
                    break;

                case InternalErrorCodes.Diagnostics:
                    errorMessage = "Diagnostic Log\r\n {0}";
                    severity = EventLogEntryType.Information;
                    break;

                case InternalErrorCodes.TCPListenerError:
                    errorMessage = "TCPListener Error: {0}";
                    severity = EventLogEntryType.Warning;
                    break;

                case InternalErrorCodes.ODSListenerError:
                    errorMessage = "Diagnostic Log\r\n {0}";
                    severity = EventLogEntryType.Error;
                    break;

                case InternalErrorCodes.InitialisationContentCorrupt:
                    errorMessage = "Initialisation Content\r\n {0}";
                    severity = EventLogEntryType.Information;
                    break;

                default:
                    errorMessage = "Unknown Error";
                    severity = EventLogEntryType.Error;
                    break;
            }

        #endregion long switch statement to assign strings and severitys
        }

#else

        /// <summary>
        /// log an error
        /// </summary>
        /// <param name="tie"></param>
        /// <param name="param"></param>
        /// <param name="overrideLevel"></param>
        public static void LogInternalError(InternalErrorCodes tie, string param, TraceLevel overrideLevel) {
            // Not Implemented.

            if (param == null) {
                sourceExists = true;
            }
            if (sourceExists) {
                param = string.Empty;
            }
        }

#endif

        /// <summary>
        ///  Enum to describe the errors which can prevent tracing from working correctly.
        /// </summary>
        public enum InternalErrorCodes {

            /// <summary>
            /// There are no listeners
            /// </summary>
            NoListenersWarning = 0x0001,

            /// <summary>
            /// Configuration invalid
            /// </summary>
            CorruptConfiguration = 0x0002,

            /// <summary>
            /// Details around listener count, not a real error
            /// </summary>
            ListenerCountInformation = 0x0003,

            /// <summary>
            /// Error during initialisation
            /// </summary>
            InitialisationError = 0x0004,

            /// <summary>
            /// Access denied
            /// </summary>
            AccessDeniedToSomething = 0x0005,

            /// <summary>
            /// Operating system compat error
            /// </summary>
            OperatingSystemIncompatibility = 0x0006,

            /// <summary>
            /// Corrpution in intialisation data
            /// </summary>
            InitialisationContentCorrupt = 0x0007,

            /// <summary>
            /// Diagnostic data, not a real error
            /// </summary>
            Diagnostics = 0x0008,

            /// <summary>
            /// Error with TCP listener
            /// </summary>
            TCPListenerError = 0x0009,

            /// <summary>
            /// Error with output debugstring listener
            /// </summary>
            ODSListenerError = 0x000A
        }

        /// <summary>
        /// Tries to identify the stack frame of the caller of the Trace Method.
        /// </summary>
        /// <param name="matchingMethodName">The name of the method to find in the callstack</param>
        /// <returns>ClassName,MethodName,StackFrame of the target method</returns>
        public static Tuple<string,string,StackFrame> GetCallingStackFrame(string matchingMethodName = null) {
            if (matchingMethodName != null) {
                matchingMethodName = matchingMethodName.ToLowerInvariant();
            }
            var currentStack = new StackTrace(false);

            for (int frameIdx = 0; frameIdx < currentStack.FrameCount; frameIdx++) {
                var nextFrame = currentStack.GetFrame(frameIdx);
                string className;
                var theMethod = nextFrame.GetMethod();

                if (theMethod.DeclaringType != null) {
                    if (theMethod.DeclaringType.Assembly == TraceAssemblyCache) {
                        // Skip methods which are in this assembly.
                        continue;
                    }
                }

                className = GetClassNameFromMethodbase(theMethod);

                if (matchingMethodName == null ) {
                    return new Tuple<string, string, StackFrame>(className, theMethod.Name, nextFrame);
                } else if (theMethod.Name.ToLowerInvariant() == matchingMethodName) {
                    return new Tuple<string, string, StackFrame>(className, theMethod.Name, nextFrame);
                }
            }

            return null;
        }
    }
}