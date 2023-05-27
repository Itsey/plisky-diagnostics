namespace Plisky.Diagnostics {

    using System;
#if NETCOREAPP
    using System.Text.Json;
#endif

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

            string result;
#if NETCOREAPP
            return JsonSerializer.Serialize(msg, typeof(MessageMetadata));
#else
            try {
                string ald = msg.ClassName + "::" + msg.MethodName;
                string dt = DateTime.Now.ToString("hh:mm:ss dd-MM-yyyy");
                string metaPart = $"\"v\":\"2\",\"uq\":\"{defaultUqr}\",\"dt\":\"{dt}\"";
                string cnamePart = $"\"c\":\"{EscapeString(msg.ClassName)}\",\"l\":\"{msg.LineNumber}\",\"mn\":\"{EscapeString(msg.MethodName)}\",\"md\":\"{EscapeString(msg.FileName)}\",\"al\":\"{EscapeString(ald)}\"";
                string procPart = $"\"nt\":\"{msg.NetThreadId}\",\"p\":\"{msg.ProcessId}\",\"t\":\"{msg.OsThreadId}\",\"man\":\"{EscapeString(msg.MachineName)}\"";
                string msgPart = $"\"m\":\"{EscapeString(msg.Body)}\",\"s\":\"{EscapeString(msg.FurtherDetails)}\",\"mt\":\"{TraceCommands.TraceCommandToString(msg.CommandType)}\"";
                result = $"{{ {metaPart},{cnamePart},{procPart},{msgPart} }}";
            } catch (Exception ex) {
                Emergency.Diags.Log("EXX >> " + ex.Message);
                throw;
            }
#endif
            return result;
        }
    }
}