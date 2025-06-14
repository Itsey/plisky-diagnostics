﻿namespace Plisky.Diagnostics {
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Determines the behaviour of the assertion failure in Bilge.
    /// </summary>
    public enum AssertionStyle {
        /// <summary>
        /// No action is taken, the assertion is ignored.
        /// </summary>
        Nothing,

        /// <summary>
        /// The default behaviour is taken, this will cause a hard applicaton termination.
        /// </summary>
        Default,

        /// <summary>
        /// The application is terminated on assertion failure.
        /// </summary>
        Fail,

        /// <summary>
        /// A Bilge Assert Exception is thrown at the point the assertion fails.
        /// </summary>
        Throw
    }

    /// <summary>
    /// Provides Assertion support into Bilge.
    /// </summary>
    public class BilgeAssert : BilgeRoutedBase {
        private Action<MessageMetadata> assertAction;
        private AssertionStyle style = AssertionStyle.Default;

        /// <summary>
        /// Holds the associated error instance of bilge which is where the error numbers are captured.
        /// </summary>
        protected ErrorWriter errorInstance;


        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeAssert"/> class.
        /// </summary>
        /// <param name="rt">The Bilge Router.</param>
        /// <param name="cs">Bilge active configuration settings.</param>
        /// <param name="ewi">The error writer instance.</param>
        internal BilgeAssert(BilgeRouter rt, ConfigSettings cs, ErrorWriter ewi) : base(rt, cs) {
            errorInstance = ewi;

            SetAssertAction();
        }

        /// <summary>
        /// Configures the assertion behaviour using the style passed in as the newStyle parameter.
        /// </summary>
        /// <param name="newStyle">The desired assertion behaviour</param>
        public void ConfigureAsserts(AssertionStyle newStyle) {
            style = newStyle;
            SetAssertAction();
        }

        /// <summary>
        /// Causes an assertion failure, using the assertion behaviour to determine whether an exception is thrown or the process terminates or nothing happens.
        /// </summary>
        /// <param name="msg">Additional Info</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
#if NETCOREAPP
        [DoesNotReturn]
#endif
        [Conditional("DEBUG")]
        public void Fail(string msg, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            AssertionFailureIfFalse(false, msg, meth, pth, ln);
        }

        /// <summary>
        /// Causes an assertion failure if 'what' is true, using the assertion behaviour to determine whether an exception is thrown or the process terminates or nothing happens.
        /// </summary>
        /// <param name="what">Must be false to not fail.</param>
        /// <param name="msg">Additional context information.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        [Conditional("DEBUG")]
        public void False(bool what, string msg = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            AssertionFailureIfFalse(!what, msg, meth, pth, ln);
        }

        /// <summary>
        /// Causes an assertion failure if 'what' is null, using the assertion behaviour to determine whether an exception is thrown or the process terminates or nothing happens.
        /// </summary>
        /// <param name="what">Causes the assertion to fail if null is passed in.</param>
        /// <param name="msg">Supporting message information relating to the failure.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        [Conditional("DEBUG")]
        public void NotNull(object what, string msg = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            AssertionFailureIfFalse(what != null, msg, meth, pth, ln);
        }

        /// <summary>
        /// Asserts that the error number has previously been recorded with a call to error.record, if the error has not previously been recorded an assertion failure
        /// occurs using the assertion behaviour configured.
        /// </summary>
        /// <param name="hResult">The error number associated with the error.</param>
        /// <param name="context">Supporting contextual information for the error report.</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        [Conditional("DEBUG")]
        public void Recorded(int hResult, string context = "Application", [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            FailIfMissingErrorCode(context, meth, pth, ln, hResult);
        }

        /// <summary>
        /// Causes an assertion failure if 'what' is false, using the assertion behaviour to determine whether an exception is thrown or the process terminates or nothing happens.
        /// </summary>
        /// <param name="what">Must be true to not fail</param>
        /// <param name="msg">Additional Info</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        [Conditional("DEBUG")]
        public void True(bool what, string msg = null, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            AssertionFailureIfFalse(what, msg, meth, pth, ln);
        }

        /// <summary>
        /// Actually triggers the assertion failure if the what parameter is false, this is used so that the conditional compilation does not strip the internal calls to the routing method.
        /// </summary>
        /// <param name="what">A parameter that will cause an assertion failure if its false.</param>
        /// <param name="msg">The message to display on assertion failure.</param>
        /// <param name="meth">The method where the asserion failure occured.</param>
        /// <param name="pth">The path to the file where the failure occured.</param>
        /// <param name="ln">The line number within the file where the failure occured.</param>
        protected void AssertionFailureIfFalse(bool what, string msg = null, string meth = null, string pth = null, int ln = 0) {
            if (!what) {
                ActiveRouteMessage(TraceCommandTypes.AssertionFailed, msg, null, meth, pth, ln);
            }
        }

        /// <summary>
        /// Performs message routing for the trace stream.
        /// </summary>
        /// <param name="mmd">The message metatadata to write.</param>
        protected override void ActiveRouteMessage(MessageMetadata mmd) {
            base.ActiveRouteMessage(mmd);
            if (mmd.CommandType == TraceCommandTypes.AssertionFailed) {
                assertAction(mmd);
            }
        }

        private void FailIfMissingErrorCode(string context, string meth, string pth, int ln, int rcode) {
            if (!errorInstance.ErrorCodeReported(rcode)) {
                Fail($"[{context}] used an unrecoreded error code {rcode}", meth, pth, ln);
            }
        }

        private void SetAssertAction() {
            switch (style) {
                case AssertionStyle.Nothing:
                    assertAction = (x) => {
                        return;
                    };
                    break;

                case AssertionStyle.Default:
                    assertAction = (x) => {
                        if (Debugger.IsAttached) {
                            Debugger.Break();
                        } else {
                            var ex = new BilgeAssertException(x.Context, x.FurtherDetails, new StackTrace(0, true).ToString());
                            string pn = Process.GetCurrentProcess().ProcessName.ToLower();
                            if (pn.StartsWith("vstesthost") || pn.StartsWith("vstest")) {
                                throw ex;
                            }
                            Environment.FailFast(ex.Message, ex);
                        }
                    };
                    break;
                case AssertionStyle.Fail:

                    assertAction = (x) => {
                        if (Debugger.IsAttached) {
                            Debugger.Break();
                        } else {
                            var ex = new BilgeAssertException(x.Context, x.FurtherDetails, new StackTrace(0, true).ToString());
                            Environment.FailFast(ex.Message, ex);
                        }
                    };
                    break;

                case AssertionStyle.Throw:
                    assertAction = (x) => {
                        var ex = new BilgeAssertException(x.Context, x.FurtherDetails, new StackTrace(0, true).ToString());
                        throw ex;
                    };
                    break;
            }
        }
    }
}