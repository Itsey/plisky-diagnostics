namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.ComTypes;

    public partial class BilgeWriter {
        /// <summary>
        /// Dumps an object into the trace stream, using a series of different approaches for displaying the object depending
        /// on the type of the object that is dumped.
        /// </summary>
        /// <param name="target">The object to be displayed in the trace stream</param>
        /// <param name="context">A context string for the trace entry</param>
        /// <param name="meth">The method name</param>
        /// <param name="pth">The path to the file of source</param>
        /// <param name="ln">The line number</param>
        public void Dump(object target, string context, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            InternalDump(target, context, null, meth, pth, ln);
        }

        /// <summary>
        /// <para> TimeStart is used for rudementary timing of sections of code.  Time start will write a time start identifier to the
        /// trace stream and start an internal timer.  When TimeStop is called for the same timer title then the value of the elapsed
        /// time is written to the trace stream.</para><para>
        /// The TimeStart method relies on a unique timerTitle to be passed to it.  There can only be one active timerTitle of the same
        /// name at any one time.  Each TimeStart(timerTitle) method call must be matched with a TimeStop(timerTitle) method call to ensure
        /// that the timing information is writtten to the trace stream.  timerTitles are case sensitive and must be specified exactly.
        /// </para><para>
        /// This is not a highly effective or accurate profilling mechanism but will suffice for quick timings.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>This method is dependant on the DEBUG preprosessing identifier.</para>
        /// <para>This method has a Trace level of Verbose.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when timerTitle is null or a zero length string.</exception>
        /// <param name="timerTitle">The unique title for the timer that is being started.</param>
        /// <param name="meth">The method name</param>
        /// <param name="pth">The path to the file of source</param>
        /// <param name="ln">The line number</param>

        [Conditional("DEBUG")]
        public void TimeStart(string timerTitle, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (timerTitle == null || (timerTitle.Length == 0)) {
                throw new ArgumentNullException(nameof(timerTitle), "timerTitle parameter cannot be null or empty for a call to TimeStart");
            }

            MessageMetadata mmd = new MessageMetadata(meth, pth, ln);
            InternalTimeCheckpoint(mmd, timerTitle, Constants.TIMERNAME, true);
        }

        /// <summary>
        /// <para> TimeStart is used for rudementary timing of sections of code.  Time start will write a time start identifier to the
        /// trace stream and start an internal timer.  When TimeStop is called for the same timer title then the value of the elapsed
        /// time is written to the trace stream.</para><para>
        /// The TimeStart method relies on a unique timerTitle to be passed to it.  There can only be one active timerTitle of the same
        /// name at any one time.  Each TimeStart(timerTitle) method call must be matched with a TimeStop(timerTitle) method call to ensure
        /// that the timing information is writtten to the trace stream.  timerTitles are case sensitive and must be specified exactly.
        /// </para><para>
        /// This is not a highly effective or accurate profilling mechanism but will suffice for quick timings.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>This method is dependant on the DEBUG preprosessing identifier.</para>
        /// <para>This method has a Trace level of Verbose.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when timerTitle is null or a zero length string.</exception>
        /// <param name="timerTitle">The unique title for the timer that is being started.</param>
        /// <param name="timerCategoryName">A category describing a collection of related timings.</param>
        /// <param name="meth">The method name</param>
        /// <param name="pth">The path to the file of source</param>
        /// <param name="ln">The line number</param>
        [Conditional("DEBUG")]
        public void TimeStart(string timerTitle, string timerCategoryName, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {

            if (timerTitle == null || (timerTitle.Length == 0)) {
                throw new ArgumentNullException(nameof(timerTitle), "timerTitle parameter cannot be null or empty for a call to TimeStart");
            }

            MessageMetadata mmd = new MessageMetadata(meth, pth, ln);
            InternalTimeCheckpoint(mmd, timerTitle, timerCategoryName, true);
        }

        /// <summary>
        /// <para> TimeStop will take a corresponding TimeStart entry and record the difference in milliseconds between the TimeStart and
        /// TimeStop method calls.  The results of this along with the start and stop times will then be written to the debugging stream.</para>
        /// <para> The TimeStop method requires that it is called with a timerTitle parameter that matches exactly a timerTitle that has
        /// already been passed to a TimeStart method call. timerTitles are case sensitive and must be specified exactly.
        /// </para><para>
        /// This is not a highly effective or accurate profilling mechanism but will suffice for quick timings.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>This method is dependant on the TRACE preprosessing identifier.</para>
        /// <para>This method has a Trace level of Verbose.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when timerTitle is null or a zero length string.</exception>
        /// <param name="timerTitle">The unique title for the timer that is being started.</param>
        /// <param name="meth">The Method Name</param>
        /// <param name="pth">The caller path</param>
        /// <param name="ln">The Line Number</param>
        [Conditional("TRACE")]
        public void TimeStop(string timerTitle, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (timerTitle == null || (timerTitle.Length == 0)) {
                throw new ArgumentNullException(nameof(timerTitle), "The timerTitle cannot be null or empty when calling TimeStop");
            }

            MessageMetadata mmd = new MessageMetadata(meth, pth, ln);
            InternalTimeCheckpoint(mmd, timerTitle, Constants.TIMERNAME, false);
        }

        /// <summary>
        /// <para> TimeStop will take a corresponding TimeStart entry and record the difference in milliseconds between the TimeStart and
        /// TimeStop method calls.  The results of this along with the start and stop times will then be written to the debugging stream.</para>
        /// <para> The TimeStop method requires that it is called with a timerTitle parameter that matches exactly a timerTitle that has
        /// already been passed to a TimeStart method call. timerTitles are case sensitive and must be specified exactly.
        /// </para><para>
        /// This is not a highly effective or accurate profilling mechanism but will suffice for quick timings.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>This method is dependant on the TRACE preprosessing identifier.</para>
        /// <para>This method has a Trace level of Verbose.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when timerTitle is null or a zero length string.</exception>
        /// <param name="timerTitle">The unique title for the timer that is being started.</param>
        /// <param name="timerCategoryName">A category describing a collection of related timings.</param>
        /// <param name="meth">The Method Name</param>
        /// <param name="pth">The caller path</param>
        /// <param name="ln">The Line Number</param>
        public void TimeStop(string timerTitle, string timerCategoryName, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {

            if (timerTitle == null || (timerTitle.Length == 0)) {
                throw new ArgumentNullException("timerTitle", "The timerTitle cannot be null or empty when calling TimeStop");
            }

            MessageMetadata mmd = new MessageMetadata(meth, pth, ln);
            InternalTimeCheckpoint(mmd, timerTitle, timerCategoryName, false);
        }

        /// <summary>
        /// The enter section method marks a section of debugging code into a descreet block.  Sections are marked on a per
        /// thread basis and can be used by viewers or by Tex to alter the trace output.
        /// </summary>
        /// <remarks><para>While it should be possible to disable output by section this is not implemented yet
        /// either in Tex or in any of the shipped viewers , including mex.</para>
        /// <para>This method is dependant on the TRACE preprosessing identifier.</para>
        /// </remarks>
        /// <param name="sectionName">The friendly name of the secion</param>
        /// <param name="meth">The Method Name</param>
        /// <param name="pth">The caller path</param>
        /// <param name="ln">The Line Number</param>
        [Conditional("TRACE")]
        public void EnterSection(string sectionName, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            ActiveRouteMessage(TraceCommandTypes.SectionStart, sectionName, null, meth, pth, ln);
        }

        /// <summary>
        /// The exit section method marks the termination of a section of code.  Section enter and exit blocks are used by viewers
        /// to determine which parts of the code to view at once.
        /// </summary>
        /// <remarks>Disabling output per section not implemented yet
        /// <para>This method is dependant on the TRACE preprosessing identifier.</para>
        /// </remarks>
        [Conditional("TRACE")]
        public void LeaveSection([CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            ActiveRouteMessage(TraceCommandTypes.SectionEnd, string.Empty, null, meth, pth, ln);
        }

        /// <summary>
        /// The E override to provide a string will replace the automatically generated method name with the string that you
        /// provide in the first parameter.
        /// </summary>
        /// <remarks><para>This method is dependant on the TRACE preprosessing identifier.</para></remarks>
        /// <param name="entryContext">The name of the block being entered</param>
        /// <param name="meth">The method name</param>
        /// <param name="pth">The path to the file of source</param>
        /// <param name="ln">The line number</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "E", Justification = "Maintained name for backward compatibility")]
        [Conditional("TRACE")]
        public void E(string entryContext = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (entryContext == null) { entryContext = string.Empty; }
            var mmd = new MessageMetadata(meth, pth, ln);
            InternalE(mmd, entryContext);
        }

        /// <summary>
        /// The X override is the indicator for leaving a block that has been entered with E.
        /// </summary>
        /// <remarks><para>This method is dependant on the TRACE preprosessing identifier.</para></remarks>
        /// <param name="meth">The method name</param>
        /// <param name="pth">The path to the file of source</param>
        /// <param name="ln">The line number.</param>
        /// <param name="exitContext">The context for the exit calling method.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X", Justification = "Maintained name for backward compatibility")]
        public void X(string exitContext = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (exitContext == null) { exitContext = string.Empty; }
            var mmd = new MessageMetadata(meth, pth, ln);
            InternalX(mmd, exitContext);
        }

        /// <summary>
        /// log it
        /// </summary>
        /// <param name="message">The log message to capture.</param>
        /// <param name="moreInfo">Further supporting information additional to the basic log.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        public void Log(string message, string moreInfo = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            DefaultRouteMessage(message, moreInfo, meth, pth, ln);
        }

        /// <summary>
        /// Log a message with supporting context added to the message tags.  Additional context is passed as a dictionary of name value pairs that are added to the
        /// trace message to be sent to the stream.
        /// </summary>
        /// <param name="message">The content of the log message.</param>
        /// <param name="context">Additional name value pairs of context.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        public void Log(string message, Dictionary<string, string> context, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (!IsWriting) { return; }
            var mmd = CreateMessageMetaData(baseCommandLevel, message, null, meth, pth, ln);
            if (context != null) {
                foreach (var l in context.Keys) {
                    mmd.MessageTags.Add(l, context[l]);
                }
            }
            ActiveRouteMessage(mmd);
        }

        /// <summary>
        /// Log a message with supporting structured data, passed through the xo dynamic paramenter.  Typically this is an ExpandoObject which is passed in with supporting structured
        /// contextual information.
        /// </summary>
        /// <param name="message">The message body to send.</param>
        /// <param name="context">A dynamic structure representing additonal context.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        public void Log(string message, dynamic context, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (!IsWriting) { return; }
            var mmd = CreateMessageMetaData(baseCommandLevel, message, null, meth, pth, ln);
            mmd.StructuredData = context;
            ActiveRouteMessage(mmd);
        }

        /// <summary>
        /// Log a message using a callback to fully populate the messagemetadata structure, this is used to ensure that the calls to calculate the trace content are only done
        /// when the tracer is writing information, this can create a significant performance boost in some circumstances.  The function should return the messagemetadata that it
        /// is passed after setting the body and further info and any other elements that are important.
        /// </summary>
        /// <param name="content">A function callback that takes a messagemetadata and returns a modified messagemetadata</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        public void Log(Func<MessageMetadata, MessageMetadata> content, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (!IsWriting) { return; }

            if (content != null) {
                var mmd = content(CreateMessageMetaData(TraceCommandTypes.LogMessage, null, null, meth, pth, ln));
                mmd = content(mmd);
                ActiveRouteMessage(mmd);
            }
        }

        /// <summary>
        /// Log a message using a callback to fully populate the messagemetadata structure, this is used to ensure that the calls to calculate the trace content are only done
        /// when the tracer is writing information, this can create a significant performance boost in some circumstances.  The function should return the a string which is then
        /// placed into the body of the trace message.
        /// </summary>
        /// <param name="content">A function callback that returns text to log to the trace stream.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        public void Log(Func<string> content, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (!IsWriting) { return; }

            if (content != null) {
                var mmd = CreateMessageMetaData(TraceCommandTypes.LogMessage, content(), null, meth, pth, ln);
                ActiveRouteMessage(mmd);
            }
        }

        /// <summary>
        /// Records the flow of program execution, by default tracing out the message name.
        /// </summary>
        /// <param name="moreInfo">Additional supporting trace information for the flow.</param>
        /// <param name="meth">The method name calling the flow.</param>
        /// <param name="pth">The path to the file of source.</param>
        /// <param name="ln">The line number.</param>
        public void Flow(string moreInfo = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            string msg = $"Flow [{meth}]";
            DefaultRouteMessage(msg, moreInfo, meth, pth, ln);
        }
    }
}