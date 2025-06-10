namespace Plisky.Diagnostics {
    using System;
    using System.Threading;

    /// <summary>
    /// BaseMessageFormatter is a base class for formatting trace messages
    /// </summary>
    public abstract class BaseMessageFormatter : IMessageFormatter {
        private static readonly string truncationCache = Constants.MESSAGETRUNCATE + "[" + Environment.MachineName + "][{0}" + Constants.TRUNCATE_DATAENDMARKER;

        /// <summary>
        /// Default value for uniqueness reference
        /// </summary>
        public const string DEFAULT_UQR = "--uqr--";

        /// <summary>
        /// Access to the options for formatting.
        /// </summary>
        protected MessageFormatterOptions mfo;



        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessageFormatter"/> class.
        /// Default contstructor specifies default options
        /// </summary>
        public BaseMessageFormatter() {
            mfo = new MessageFormatterOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessageFormatter"/> class.
        /// Specify custom options.
        /// </summary>
        /// <param name="nfo">The options to use</param>
        public BaseMessageFormatter(MessageFormatterOptions nfo) {
            mfo = nfo;
        }

        /// <summary>
        /// This will take a single long string and return it as a series of truncated strings with the length that is
        /// specified in theLength parameter used to do the chopping up.  There is nothing clever or special about this
        /// routine it does not break on words or aynthing like that.
        /// </summary>
        /// <param name="theLongString">The string that is to be chopped up into smaller strings</param>
        /// <param name="theLength">The length at which the smaller strings are to be created</param>
        /// <returns>The input string split into an array.</returns>
        public static string[] MakeManyStrings(string theLongString, int theLength) {
            if (theLongString == null) { return null; }
            if (theLength <= 0) { throw new ArgumentException("theLength parameter cannot be <=0 for MakeManyStrings method"); }

            string[] result;

            if (theLongString.Length <= theLength) {
                // Special case where no splitting is necessary;
                result = new string[1];
                result[0] = theLongString;
                return result;
            }

            double exactNoChops = (double)((double)theLongString.Length / (double)theLength);
            int noChops = (int)Math.Ceiling(exactNoChops);

            result = new string[noChops];

            // All other cases where theLongString actually needs to be chopped up into smaller chunks
            int remainingChars = theLongString.Length;
            int currentOffset = 0;
            int currentChopCount = 0;
            while (remainingChars > theLength) {
                result[currentChopCount++] = theLongString.Substring(currentOffset, theLength);
                remainingChars -= theLength;
                currentOffset += theLength;
            }
            result[currentChopCount] = theLongString.Substring(currentOffset, remainingChars);

#if DEBUG
            if (currentChopCount != (noChops - 1)) {
                throw new NotSupportedException("This really should not happen");
            }
#endif

            string truncJoinIdentifier = Thread.CurrentThread.GetHashCode().ToString();
            string truncateStartIdentifier = string.Format(truncationCache, truncJoinIdentifier);
            result[0] = result[0] + Constants.MESSAGETRUNCATE + truncJoinIdentifier + Constants.MESSAGETRUNCATE;
            for (int i = 1; i < result.Length - 1; i++) {
                result[i] = truncateStartIdentifier + result[i] + Constants.MESSAGETRUNCATE;
            }
            result[result.Length - 1] = truncateStartIdentifier + result[result.Length - 1];
            return result;
        }

        /// <summary>
        /// replaces line feeds with double quoted text.
        /// </summary>
        /// <param name="input">A string to escape.</param>
        /// <returns>The escaped string</returns>
        protected string EscapeString(string input) {
            return input.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("\\", "\\\\");
        }

        /// <summary>
        /// Performs a conversion using the default uniqueness reference
        /// </summary>
        /// <param name="msg">The raw trace message</param>
        /// <returns>The string to write to the trace stream</returns>
        public string Convert(MessageMetadata msg) {
            return ConvertWithReference(msg, DEFAULT_UQR);
        }

        /// <summary>
        /// Converts the messsage using the supplied uniqueness reference
        /// </summary>
        /// <param name="msg">The message that is to be converted.</param>
        /// <param name="uniquenessReference">A uniqueness reference for context.</param>
        /// <returns>The string forming the converted message.</returns>
        public string ConvertWithReference(MessageMetadata msg, string uniquenessReference) {
            if (msg == null) {
                throw new ArgumentNullException(nameof(msg), "Can not convert a null message data");
            }
            if (string.IsNullOrEmpty(uniquenessReference)) {
                uniquenessReference = DEFAULT_UQR;
            }
            msg.NullsToEmptyStrings();
            string result = DefaultConvertWithReference(msg, uniquenessReference);
            if (mfo.AppendNewline && (!result.EndsWith(Environment.NewLine))) {
                result = result + Environment.NewLine;
            }
            return result;
        }

        /// <summary>
        /// Performs the conversion of the raw message into the string for outputting to the trace stream.
        /// </summary>
        /// <param name="msg">The raw trace message</param>
        /// <returns>The content to be logged</returns>
        protected abstract string ActualConvert(MessageMetadata msg);

        /// <summary>
        /// Performs a default conversion using the supplied uniqueness reference.
        /// </summary>
        /// <param name="msg">The trace message to write</param>
        /// <param name="uniquenessReference">The uniqueness reference to use</param>
        /// <returns>The string to log to the output</returns>
        protected virtual string DefaultConvertWithReference(MessageMetadata msg, string uniquenessReference) {
            string result = ActualConvert(msg);
            if (uniquenessReference != DEFAULT_UQR) {
                result = result.Replace(DEFAULT_UQR, uniquenessReference);
            }
            return result;
        }
    }
}