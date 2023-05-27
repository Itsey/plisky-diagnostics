namespace Plisky.Diagnostics {
#if NETCOREAPP
    using System.Text.Json;

    /// <summary>
    /// Formatter to support FlimFlam V2, this is a JSON format coded within this class (not using an external json library) as it
    /// does not need to be flexible just use the data that Blige sends.
    /// </summary>
    public class FlimFlamV4Formatter : BaseMessageFormatter {
        /// <summary>
        /// Performs the conversion
        /// </summary>
        /// <param name="msg">The message that is to be sent</param>
        /// <returns>A string representation ready to send</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            return ConvertMsg(msg, DEFAULT_UQR);
        }

        /// <summary>
        /// Performs a conversion using the specfied uniqueness reference
        /// </summary>
        /// <param name="msg">The message that is to be sent</param>
        /// <param name="uniquenessReference">A Uniqueness reference to include in the message</param>
        /// <returns>The string ready to be sent</returns>
        protected override string DefaultConvertWithReference(MessageMetadata msg, string uniquenessReference) {
            return ConvertMsg(msg, uniquenessReference);
        }

        private string ConvertMsg(MessageMetadata msg, string defaultUqr) {
            msg.NullsToEmptyStrings();
            msg.Ffv = "ffv0004";
            return JsonSerializer.Serialize(msg, typeof(MessageMetadata));
        }
    }

#endif
}