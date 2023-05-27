namespace Plisky.Diagnostics {
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// USed by the assertion code when the assertion behaviour is set to throw exceptions.
    /// </summary>
    public sealed class BilgeAssertException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeAssertException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="detailMessage">Further details for the excepion message.</param>
        /// <param name="stackTrace">A stack trace supporting the exception message.</param>
        public BilgeAssertException(string message, string detailMessage, string stackTrace) : base(AddNewLine(message) + AddNewLine(detailMessage) + stackTrace) {
        }

        private static string AddNewLine(string s) {
            if (s == null) {
                return s;
            }

            s = s.Trim();
            if (s.Length > 0) {
                s += Environment.NewLine;
            }
            return s;
        }
    }
}