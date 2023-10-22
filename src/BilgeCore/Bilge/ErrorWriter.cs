namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Performs basic logging at the error level but also adds features to support the reporting and recording of errors using hResult error codes.
    /// </summary>
    public class ErrorWriter : BilgeWriter {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWriter"/> class.
        /// </summary>
        /// <param name="router">The router to use for sending messages</param>
        /// <param name="config">The configuration for the library</param>
        /// <param name="yourTraceLevel">The trace level to use for this writer</param>
        public ErrorWriter(BilgeRouter router, ConfigSettings config, SourceLevels yourTraceLevel) : base(router, config, yourTraceLevel) {
            baseCommandLevel = TraceCommandTypes.ErrorMsg;
        }

        /// <summary>
        /// Exposes the captured error codes so that checks can be made against all of the error codes that have been reported.
        /// </summary>
        protected List<int> ReportedErrorCodes { get; set; } = new List<int>();

        /// <summary>
        /// Returns true if the given hResult has already been recorded by one of the error recording calls. Note this will only return true
        /// if the record has been hit in this session, it can not persist error codes between runs.  This is used internally by the Assert.Report functions
        /// </summary>
        /// <param name="hResult">The error code to check for existance.</param>
        /// <returns>True if it is alrady recorded, falise if it has not been recorded</returns>
        public bool ErrorCodeReported(int hResult) {
            lock (ReportedErrorCodes) {
                return ReportedErrorCodes.Contains(hResult);
            }
        }

        /// <summary>
        /// Records an error hResult using two 16bit values to create a single 32 bit hResult, this will send the recorded error message and the context string to the trace stream to
        /// capture that there is a potential error message.  This is not reporting an error, but recording that one can happen.  To report an error use Report.
        /// </summary>
        /// <param name="errorDescriber">An ErrorDescription describing the error.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>an hResult representing the combined upper and lower digits.  The upper digits are shifted left to create this.</returns>
        public int Record(ErrorDescription errorDescriber, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            var mmd = CreateErrorRecordingMessage(errorDescriber.HResult, errorDescriber.Context, meth, pth, ln);
            ActiveRouteMessage(mmd);
            return errorDescriber.HResult;
        }


        /// <summary>
        /// Records an error hResult using two 16bit values to create a single 32 bit hResult, this will send the recorded error message and the context string to the trace stream to
        /// capture that there is a potential error message.  This is not reporting an error, but recording that one can happen.  To report an error use Report.
        /// </summary>
        /// <param name="hResultUpper">The upper 16 bit value to use.</param>
        /// <param name="hResultLower">The lower 16 bit value to use..</param>
        /// <param name="errorContext">Contextual information supporting the error reporting.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>an hResult representing the combined upper and lower digits.  The upper digits are shifted left to create this.</returns>
        public int Report(short hResultUpper, short hResultLower, string errorContext, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            return Report(ErrorDescription.GetHResult(hResultUpper, hResultLower), errorContext, meth, pth, ln);
        }

        /// <summary>
        /// Records an error hResult using a single 32 bit hResult, this will send the recorded error message and the context string to the trace stream to
        /// capture that there is a potential error message.  This is not reporting an error, but recording that one can happen.  To report an error use Report.
        /// </summary>
        /// <param name="hResult">An error code representing the error number.</param>
        /// <param name="errorContext">Contextual information supporting the error reporting.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>an hResult representing the combined upper and lower digits.  The upper digits are shifted left to create this.</returns>
        public int Report(int hResult, string errorContext, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            var mmd = CreateErrorReportingMessage(hResult, errorContext, meth, pth, ln);
            ActiveRouteMessage(mmd);
            return hResult;
        }




        /// <summary>
        /// ReportRecordException reports and records an exception that can then be thrown.  Uses an hResult style error number to identify the error.
        /// </summary>
        /// <typeparam name="TException">The type of exception that should be created.</typeparam>
        /// <param name="hResult">An error code to use to represent the error.</param>
        /// <param name="message">The message that will be passed into the first parameter of the contructor for the exception type.  This calls the constructor with a single string parameter.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>        
        /// <returns>An exception that can be thrown.</returns>
        public TException ReportRecordException<TException>(int hResult, string message, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) where TException : Exception, new() {
            return ReportRecordException<TException>(hResult, message, meth, pth, ln);
        }

        /// <summary>
        /// ReportRecordException reports and records an exception that can then be thrown.  Uses an hResult style error number to identify the error.
        /// </summary>
        /// <typeparam name="TException">The type of exception that should be created.</typeparam>
        /// <param name="hResultUpper">The upper 16 bit value to use.</param>
        /// <param name="hResultLower">The lower 16 bit value to use.</param>
        /// <param name="message">The message that will be passed into the first parameter of the contructor for the exception type.  This calls the constructor with a single string parameter.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>        
        /// <returns>An exception that can be thrown.</returns>
        public TException ReportRecordException<TException>(short hResultUpper, short hResultLower, string message, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) where TException : Exception, new() {
            return ReportRecordException<TException>(ErrorDescription.GetHResult(hResultUpper, hResultLower), message, meth, pth, ln);
        }

        /// <summary>
        /// Reports an exception and at the same time records it so that it can be checked against the correct error numbers.  This is a shortcut for throwing a new exception and reporting and recording it, it only works if
        /// the first parameter to the exception is a string.
        /// </summary>
        /// <typeparam name="TException">The exception to be thrown.</typeparam>
        /// <param name="hResult">The error code to use.</param>        
        /// <param name="message">The message that will be passed into the first parameter of the contructor for the exception type.  This calls the constructor with a single string parameter.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>        
        /// <returns>An exception that can be thrown.</returns>
        public TException ReportException<TException>(int hResult, string message, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) where TException : Exception, new() {
            return ReportException<TException>(hResult, message, null, meth, pth, ln);
        }

        /// <summary>
        /// Reports an exception and at the same time records it so that it can be checked against the correct error numbers.  This is a shortcut for throwing a new exception and reporting and recording it, it only works if
        /// the first parameter to the exception is a string.
        /// </summary>
        /// <typeparam name="TException">The exception to be thrown.</typeparam>
        /// <param name="hResultUpper">The upper 16 bit value to use.</param>
        /// <param name="hResultLower">The lower 16 bit value to use.</param>
        /// <param name="message">The message that will be passed into the first parameter of the contructor for the exception type.  This calls the constructor with a single string parameter.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>        
        /// <returns>An exception that can be thrown.</returns>
        public TException ReportException<TException>(short hResultUpper, short hResultLower, string message, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) where TException : Exception, new() {
            return ReportException<TException>(ErrorDescription.GetHResult(hResultUpper, hResultLower), message,null, meth, pth, ln);
        }


        /// <summary>
        /// Reports an exception and at the same time records it so that it can be checked against the correct error numbers.  This is a shortcut for throwing a new exception and reporting and recording it, it only works if
        /// the first parameter to the exception is a string.
        /// </summary>
        /// <typeparam name="TException">The exception to be thrown.</typeparam>
        /// <param name="hResult">The error code to use.</param>        
        /// <param name="message">The message that will be passed into the first parameter of the contructor for the exception type.  This calls the constructor with a single string parameter.</param>
        /// <param name="supportingData">Dictionary of addtional supporting values to add to the exception</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>        
        /// <returns>An exception that can be thrown.</returns>
        public TException ReportException<TException>(int hResult, string message, Dictionary<string, string> supportingData = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) where TException : Exception, new() {

            var mmd = CreateErrorReportingMessage(hResult, message, meth, pth, ln);

            ActiveRouteMessage(mmd);

            var ex = (Exception)Activator.CreateInstance(typeof(TException), message);
            if (ex != null) {
#if NETCOREAPP
                ex.HResult = hResult;
#else
                var hresultField = typeof(Exception).GetField("_HResult", BindingFlags.NonPublic | BindingFlags.Instance);
                hresultField.SetValue(ex, hResult);
#endif
                if (supportingData != null) {
                    foreach (string field in supportingData.Keys) {
                        ex.Data.Add(field, supportingData[field]);
                    }
                }

            }

            return (TException)ex;

        }



        /// <summary>
        /// Rcords a new error code and reports an instance of that error.  The error describer records the values for the error code and the context is the context for reporting the error.
        /// </summary>
        /// <param name="errorDescriber">An ErrorDescription describing the error.</param>
        /// <param name="errorContext">Contextual information supporting the error reporting.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>an hResult representing the combined upper and lower digits.  The upper digits are shifted left to create this.</returns>
        public int ReportRecord(ErrorDescription errorDescriber, string errorContext, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            var mmd = CreateErrorRecordingMessage(errorDescriber.HResult, errorDescriber.Context, meth, pth, ln);
            ActiveRouteMessage(mmd);
            mmd = CreateErrorReportingMessage(errorDescriber.HResult, errorContext, meth, pth, ln);
            ActiveRouteMessage(mmd);
            return errorDescriber.HResult;
        }



        private void AddErrorCode(int hResult) {
            lock (ReportedErrorCodes) {
                if (!this.ReportedErrorCodes.Contains(hResult)) {
                    this.ReportedErrorCodes.Add(hResult);
                }
            }
        }

        private MessageMetadata CreateErrorRecordingMessage(int hResult, string context, string meth, string pth, int ln) {
            AddErrorCode(hResult);

            var mmd = CreateMessageMetaData(TraceCommandTypes.CommandData, context, $"0x{hResult:X8}", meth, pth, ln);
            mmd.MessageTags.Add("plisky.hresult", $"0x{hResult:X8}");
            mmd.MessageTags.Add("plisky.commandtype", $"ERROR_RECORD");
            return mmd;
        }

        private MessageMetadata CreateErrorReportingMessage(int hResult, string context, string meth, string pth, int ln) {
            var mmd = CreateMessageMetaData(TraceCommandTypes.CommandData, context, $"0x{hResult:X8}", meth, pth, ln);
            mmd.MessageTags.Add("plisky.hresult", $"0x{hResult:X8}");
            mmd.MessageTags.Add("plisky.commandtype", $"ERROR_REPORT");
            return mmd;
        }
    }
}