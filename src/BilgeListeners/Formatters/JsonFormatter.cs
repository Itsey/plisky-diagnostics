namespace Plisky.Diagnostics.Listeners {
#if NETCOREAPP
    using System.Text.Json;

    /// <summary>
    /// Console formatter
    /// </summary>
    public class JsonFormatter : BaseMessageFormatter {
        /// <summary>
        /// Perform the actual conversion
        /// </summary>
        /// <param name="msg">The message metadata to convert</param>
        /// <returns>The string of the converted message meta data</returns>
        protected override string ActualConvert(MessageMetadata msg) {
            return JsonSerializer.Serialize(msg, typeof(MessageMetadata));
        }
    }
#endif
}