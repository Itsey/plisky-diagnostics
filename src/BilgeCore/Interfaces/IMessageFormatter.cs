namespace Plisky.Diagnostics {

    /// <summary>
    /// IMessageFormatter is an interface used by the listeners to convert a structured MesageMetaData into a string representation for transport or display,
    /// therea are several built in message formatters but others can be used too.  The most common ones are for console, readable text file and import into
    /// FlimFlam either using the legacy formatter or the V2 Formatter.
    /// </summary>
    public interface IMessageFormatter {

        /// <summary>
        /// Converts the Message structure to a string using the formatter information
        /// </summary>
        /// <param name="msg">The message structure to convert</param>
        /// <returns>String representation of the message structure</returns>
        string Convert(MessageMetadata msg);

        /// <summary>
        /// uses a reference during the conversion
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="uniquenessReference">a unique value</param>
        /// <returns>converted string</returns>
        string ConvertWithReference(MessageMetadata msg, string uniquenessReference);
    }

#if FALSE
    Code was added but not sure why now.

    public interface IMessageFormatter2 {

        /// <summary>
        /// Updated version of Convert that takes a MessageMetadata and converts it into a message string according to the rules of the formatter, Format message can
        /// return more than one string where multiple messages are required. Wherever possible this should replace
        /// </summary>
        /// <param name="msg">The message metadata that is to be used to format the message.</param>
        /// <returns>One or more message formatted strings representing the metadata.</returns>
        string[] FormatMessage(MessageMetadata msg);

        /// <summary>
        /// Updated version of Convert that takes a MessageMetadata and converts it into a message string according to the rules of the formatter, Format message can
        /// return more than one string where multiple messages are required. Wherever possible this should replace
        /// </summary>
        /// <param name="msg">The message metadata that is to be used to format the message.</param>
        /// <param name="uniquenessReference">a unique value</param>
        /// <returns>One or more message formatted strings representing the metadata.</returns>
        string[] FormatMessage(MessageMetadata msg, string uniquenessReference);
    }
#endif
}