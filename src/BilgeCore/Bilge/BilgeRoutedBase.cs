namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Plisky.Plumbing;

    /// <summary>
    /// Base class for router
    /// </summary>
    public abstract class BilgeRoutedBase {

        /// <summary>
        /// Access to underlying router
        /// </summary>
        protected BilgeRouter router;

        /// <summary>
        /// Access to configuration settings
        /// </summary>
        protected ConfigSettings sets;

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeRoutedBase"/> class.
        /// default constructor to set up routing
        /// </summary>
        /// <param name="rt">The router to configure</param>
        /// <param name="cs">The configuration settings to get access to</param>
        public BilgeRoutedBase(BilgeRouter rt, ConfigSettings cs) {
            router = rt ?? throw new ArgumentNullException(nameof(rt));
            sets = cs ?? throw new ArgumentNullException(nameof(cs));
        }

        /// <summary>
        /// Route messages to the trace
        /// </summary>
        /// <param name="mmd">The message to route</param>
        protected virtual void ActiveRouteMessage(MessageMetadata mmd) {
#if DEBUG
            if (mmd == null) { throw new ArgumentNullException(nameof(mmd)); }
            if (sets == null) { throw new InvalidOperationException("Settings must be set before this call. DevFault"); }
            if (sets.MetaContexts == null) { throw new InvalidOperationException("Settings.MetaContexts must be set before this call. DevFault"); }
#endif

            // Some methods pass all this context around so the can call this directly.  All of the shared routing info should be done her
            // with the other overload only used to call this one.

            if (sets.SessionFilter != null) {
                if (!sets.SessionFilter(sets.MetaContexts)) {
                    return;
                }
            }

            if (string.IsNullOrEmpty(mmd.ClassName) && sets.TraceConfig.AddClassDetailToTrace) {
                var csf = InternalUtil.GetCallingStackFrame();
                mmd.ClassName = csf.Item1;
            }


            if (sets.TraceConfig.AddTimestamps) {
                mmd.TimeStamp = DateTime.Now;
            }

            router.PrepareMetaData(mmd, sets.MetaContexts);
            router.QueueMessage(mmd);
        }

        /// <summary>
        /// Route messages to trace
        /// </summary>
        /// <param name="tct">The command type to use</param>
        /// <param name="messageBody">The trace message body</param>
        /// <param name="furtherInfo">A secondary trace message</param>
        /// <param name="methodName">The owning method name</param>
        /// <param name="fileName">The source file name</param>
        /// <param name="lineNumber">The source line number</param>
        protected void ActiveRouteMessage(TraceCommandTypes tct, string messageBody, string furtherInfo = null, string methodName = null, string fileName = null, int lineNumber = 0) {
            var mmd = CreateMessageMetaData(tct, messageBody, furtherInfo, methodName, fileName, lineNumber);
            ActiveRouteMessage(mmd);
        }

        /// <summary>
        /// Method to create a MessageMetadata structure including all of the content passed as parameters.
        /// </summary>
        /// <param name="tct">The trace command type to use</param>
        /// <param name="messageBody">The message body</param>
        /// <param name="furtherInfo">Additional supporting information</param>
        /// <param name="methodName">The method name</param>
        /// <param name="fileName">The filename</param>
        /// <param name="lineNumber">The line number</param>
        /// <returns>A MessageMetadata type with the information populated.</returns>
        protected MessageMetadata CreateMessageMetaData(TraceCommandTypes tct, string messageBody, string furtherInfo = null, string methodName = null, string fileName = null, int lineNumber = 0) {
            var mmd = new MessageMetadata() {
                CommandType = tct,
                MethodName = methodName,
                FileName = fileName,
                LineNumber = lineNumber.ToString(),
                Body = messageBody,
                FurtherDetails = furtherInfo
            };
            return mmd;
        }
    }
}