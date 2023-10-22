namespace Plisky.Diagnostics {
    /// <summary>
    /// Holds the context of an error number including a description of what that error means.
    /// </summary>
    public class ErrorDescription {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDescription"/> class which holds the context of an error number including a description of what that error means.
        /// </summary>
        public ErrorDescription() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDescription"/> class which holds the context of an error number including a description of what that error means.
        /// </summary>
        /// <param name="hResult">An error code to include.</param>
        public ErrorDescription(int hResult) {
            HResult = hResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDescription"/> class which holds the context of an error number including a description of what that error means.
        /// </summary>
        /// <param name="upperDigits">The upper 16 digits to use as an error code.</param>
        /// <param name="lowerDigits">The lower 16 digits to use as an error code.</param>
        public ErrorDescription(short upperDigits, short lowerDigits) {
            HResult = GetHResult(upperDigits, lowerDigits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDescription"/> class which holds the context of an error number including a description of what that error means.
        /// </summary>
        /// <param name="hResult">An error code to include.</param>
        /// <param name="errorContext">Additional supporting error context, describing the error itself.</param>
        public ErrorDescription(int hResult, string errorContext) : this(hResult) {
            Context = errorContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDescription"/> class which holds the context of an error number including a description of what that error means.
        /// </summary>
        /// <param name="upperDigits">The upper 16 digits to return.</param>
        /// <param name="lowerDigits">The lower 16 digits to return.</param>
        /// <param name="errorContext">Additional supporting error context, describing the error itself.</param>
        public ErrorDescription(short upperDigits, short lowerDigits, string errorContext) : this(upperDigits, lowerDigits) {
            Context = errorContext;
        }

        /// <summary>
        /// Gets or sets the description of what the error number is used for.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets a Numeric code for representing the error number.
        /// </summary>
        public int HResult { get; set; }

        /// <summary>
        /// Returns a  combined hResult value by shifting upperDigits left by 16bits and the adding the lower digits.
        /// </summary>
        /// <param name="upperDigits">The upper 16 digits to return.</param>
        /// <param name="lowerDigits">The lower 16 digits to return.</param>
        /// <returns>A combined value of upperDigits and LowerDigits.</returns>
        public static int GetHResult(short upperDigits, short lowerDigits) {
            return (upperDigits << 16) + lowerDigits;
        }
    }
}