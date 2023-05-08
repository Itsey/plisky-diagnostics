namespace Plisky.Diagnostics.Listeners {

    using System;

    /// <summary>
    /// filestream handler options
    /// </summary>
    public class TCPHandlerOptions : HandlerOptions {
        private const int MINIMUM_PARTS_FOR_VALIDCONFIG = 2;
        private const string TCPHANDLER_IDENTIFIER = "TCP:";

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPHandlerOptions"/> class.
        /// Used to provide options into the TCP handler, notably the target address and port.
        /// </summary>
        /// <param name="initialisationString">Handler initialisation string TCP:Address,Port,HarshFails</param>
        public TCPHandlerOptions(string initialisationString) : base(initialisationString) {
            CanCreate = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPHandlerOptions"/> class.
        /// Used to initiatise the TCP handler from individual parameters.
        /// </summary>
        /// <param name="destinationIP">The address or host name to send trace to</param>
        /// <param name="targetPort">The port number to use</param>
        /// <param name="harshFails">Whether assertion failures trigger program exit</param>
        public TCPHandlerOptions(string destinationIP, int targetPort, bool harshFails = false) : base($"TCP:{destinationIP},{targetPort},{harshFails}") {
            Address = destinationIP;
            Port = targetPort;
            HarshFails = harshFails;
            CanCreate = true;
        }

        /// <summary>
        /// The destination address for the TCP Connection
        /// </summary>
        public string Address {
            get; private set;
        }

        /// <summary>
        /// Should Asserts cause exceptions to be thrown to terminate the process
        /// </summary>
        public bool HarshFails {
            get; private set;
        }

        /// <summary>
        /// The target port for the TCP connection
        /// </summary>
        public int Port {
            get; private set;
        }

        /// <summary>
        /// Determines whether you can create this handler from the config string.
        /// </summary>
        /// <returns>True if the config string is valid</returns>
        public override bool Parse() {
            CanCreate = true;

            if (InitialisationString.StartsWith(TCPHANDLER_IDENTIFIER)) {
                InitialisationString = InitialisationString.Substring(TCPHANDLER_IDENTIFIER.Length);
            } else {
                CanCreate = false;
                return CanCreate;
            }

            string[] parts = InitialisationString.Split(',');
            if (parts[0].Contains(":")) {
                // This is catching any other initialisation strings SFL: FIL: etc. None of those are valid for tcp
                // initialisation therefore return null.
                CanCreate = false;
                return CanCreate;
            }

            if (parts.Length < MINIMUM_PARTS_FOR_VALIDCONFIG) {
                CanCreate = false;
                return false;
            }

            if (string.IsNullOrEmpty(parts[0])) {
                CanCreate = false;
            } else {
                Address = parts[0];
            }

            try {
                Port = Convert.ToInt32(parts[1]);
                if (parts.Length == 3) {
                    HarshFails = Convert.ToBoolean(parts[2]);
                }
            } catch (FormatException) {
                CanCreate = false;
            } catch (OverflowException) {
                CanCreate = false;
            } catch (ArgumentOutOfRangeException) {
                CanCreate = false;
            }

            return CanCreate;
        }
    }
}