namespace Plisky.Diagnostics.Listeners {
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Default handler for sending messages over the network.
    /// </summary>
    public class TCPHandler : BaseHandler, IBilgeMessageListener {
        /// <summary>
        /// If this is true then a failed assert will terminate the app
        /// </summary>
        internal bool FailsAreHarsh;

        /// <summary>
        /// Current handler status
        /// </summary>
        internal string Status = "Untouched";

        /// <summary>
        /// The client for comms.
        /// </summary>
        internal AsyncTCPClient TcpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPHandler"/> class.
        /// constructor
        /// </summary>
        /// <param name="targetIp">The ip address to target</param>
        /// <param name="targetPort">The port to target</param>
        /// <param name="harshFails">Whether the app should terminate on assertiona failure</param>
        public TCPHandler(string targetIp, int targetPort, bool harshFails = false)
            : this(new TCPHandlerOptions(targetIp, targetPort, harshFails)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPHandler"/> class.
        /// </summary>
        /// <param name="tho">Options to initialise with</param>
        public TCPHandler(TCPHandlerOptions tho) {
            Priority = 1;
            try {
                Target = $"{tho.Address}:{tho.Port}";
                FailsAreHarsh = tho.HarshFails;
                TcpClient = new AsyncTCPClient(tho.Address, tho.Port);
                DestinationAddress = tho.Address;
                DestinationPort = tho.Port;
                Formatter = DefaultFormatter(false);
            } catch (Exception ex) {
                LastFault = ex;
                throw;
            }
        }

        /// <summary>
        /// The target address to use.
        /// </summary>
        public string DestinationAddress { get; private set; }

        /// <summary>
        /// The target port to use.
        /// </summary>
        public int DestinationPort { get; private set; }

        /// <summary>
        /// last error that occured.
        /// </summary>
        public Exception LastFault { get; internal set; }

        /// <summary>
        /// name of the handler
        /// </summary>
        public string Name => nameof(TCPHandler);

        /// <summary>
        /// The target to send messages to.
        /// </summary>
        internal string Target { get; set; }

        /// <summary>
        /// Performs the initialisation of the handler from a string
        /// </summary>
        /// <param name="initialisationString">a string containing initialisation information</param>
        /// <returns>An initialised handler</returns>
        [HandlerInitialisation("TCP")]
        public static TCPHandler InitiliaseFrom(string initialisationString) {
            var tho = new TCPHandlerOptions(initialisationString);
            if (tho.Parse()) {
                return new TCPHandler(tho);
            }

            return null;
        }

        /// <summary>
        /// clean up on shutdown.
        /// </summary>
        public override void CleanUpResources() {
            if (TcpClient != null) {
                TcpClient.Dispose();
            }
        }

        /// <summary>
        /// flush all data
        /// </summary>
        public override void Flush() {
        }

        /// <summary>
        /// get the status.
        /// </summary>
        /// <returns>the status string.</returns>
        public string GetStatus() {
            return $"To: {Target} status:{Status} + {TcpClient.GetStatus()}.";
        }

        /// <summary>
        /// default modern handler.
        /// </summary>
        /// <param name="msgMeta">the message.</param>
        public async void HandleMessageAsync(MessageMetadata msgMeta) {
            try {
                string msg = Formatter.Convert(msgMeta);
                await TcpClient.WriteToExternalSocket(msg);
                Status = "ok";
            } catch (Exception ex) {
                LastFault = ex;
                Status = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// modern handle.
        /// </summary>
        /// <param name="msg">the message.</param>
        /// <returns>Task for async</returns>
        public async Task HandleMessageAsync(MessageMetadata[] msg) {
            try {
                // TODO : Be a bit smart here, dont do them ALL at once.
                string whatToWrite = null;
                bool assertFailFoud = false;

                var sb = new StringBuilder();
                for (int i = 0; i < msg.Length; i++) {
                    sb.Append(Formatter.Convert(msg[i]));
                    sb.Append(Constants.TCPEND_MARKERTAG);
                    if (msg[i].CommandType == TraceCommandTypes.AssertionFailed) {
                        assertFailFoud = true;
                    }
                }

                whatToWrite = sb.ToString();
                await TcpClient.WriteToExternalSocket(whatToWrite).ConfigureAwait(false);
                Status = "ok";
                if (assertFailFoud && FailsAreHarsh) {
                    try {
                        Process.GetCurrentProcess().Kill();
                    } catch (NotSupportedException) {
                    } catch (InvalidOperationException) {
                    }
                }
            } catch (Exception ex) {
                LastFault = ex;
                throw;
            }
        }
    }
}