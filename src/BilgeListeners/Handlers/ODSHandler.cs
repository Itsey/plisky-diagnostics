namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Plisky.Plumbing;

    /// <summary>
    /// output debug string handler
    /// </summary>
    public class ODSHandler : BaseHandler, IBilgeMessageListener {
        private string status;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODSHandler"/> class.
        /// </summary>
        public ODSHandler() {
            Formatter = DefaultFormatter(true);
        }

        /// <summary>
        /// name of the handler
        /// </summary>
        public string Name => nameof(ODSHandler);

        /// <summary>
        /// write to output debug string.
        /// </summary>
        /// <param name="thisMsg">message to write</param>
        public static void InternalOutputDebugString(string thisMsg) {
            try {
                // The output debug string API seems to be a little strange when it comes to handeling large amounts of
                // data and does not seem to be able to handle long strings properly.  It is likely that it is my code
                // that is at fault but untill I can get to the bottom of it this listener will chop long strings up
                // into chunks and send them as separate chunks of 1024 bytes in each.  It is then the viewers job
                // to reassemble them, however in order to help the viewer specialist string truncated identifiers will
                // be sent as the markers to the extended strings.
                if (thisMsg.Length > Constants.LIMIT_OUTPUT_DATA_TO) {
                    string[] messageParts = LegacyFlimFlamFormatter.MakeManyStrings(thisMsg, Constants.LIMIT_OUTPUT_DATA_TO);
                    // Truncation identifier is #TNK#[MACHINENAME][TRUNCJOINID]XEX

                    for (int partct = 0; partct < messageParts.Length; partct++) {
                        OutputDebugString(messageParts[partct]);
                    }

                    return;
                }

                OutputDebugString(thisMsg);
            } catch (Exception ex) {
                InternalUtil.LogInternalError(InternalUtil.InternalErrorCodes.ODSListenerError, "There was an error trying to send data to outputdebugstring. " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// write to outputdebugstring.
        /// </summary>
        /// <param name="s">Text To Write</param>
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Want to send to ODS not to a debugger")]
        [DllImport("kernel32.dll", EntryPoint = "OutputDebugStringA", SetLastError = false)]
        public static extern void OutputDebugString(string s);

        /// <summary>
        /// free resources
        /// </summary>
        public override void CleanUpResources() {
            // No unmanaged resources.
        }

        /// <summary>
        /// flush messages
        /// </summary>
        public override void Flush() {
            // No caching, therefore flush not required.
        }

        /// <summary>
        /// return handler status.
        /// </summary>
        /// <returns>The status string.</returns>
        public string GetStatus() {
            return $"write status {status}";
        }

        /// <summary>
        /// legacy handle message
        /// </summary>
        /// <param name="msgMeta">The message</param>
        public void HandleMessage(MessageMetadata msgMeta) {
            try {
                string msg = Formatter.Convert(msgMeta);
                InternalOutputDebugString(msg);
                status = "ok";
            } catch (Exception ex) {
                status = "write failed " + ex.Message;
                throw;
            }
        }

        /// <summary>
        /// modern message hanlder
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <returns>Task for async execution.</returns>
        public Task HandleMessageAsync(MessageMetadata[] msg) {
            throw new NotImplementedException();
        }
    }
}