namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Socket communications using the async keyword
    /// </summary>
    internal class AsyncTCPClient : IDisposable {

        // If debugging then with stop on throw then this pops up all the time if its failing to connect, have therefore
        // dropped it right down such that it only retries once every 5 minutes.
        private const int SECONDS_NO_SOCKET_RETRY = 60;

        private bool disposedValue = false;
        private string ipAddress;
        private DateTime lastSocketException = DateTime.MinValue;
        private Action<string> logger = null;
        private uint messagesWritten;
        private int port;
        private bool socketCommunicationsDown;
        private string status;
        private NetworkStream stream;
        private TcpClient tcpClient;
        // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTCPClient"/> class.
        /// </summary>
        /// <param name="ip">The target ipaddress to send messages to</param>
        /// <param name="portToSelect">The port to use</param>
        public AsyncTCPClient(string ip, int portToSelect) {
            ipAddress = ip;
            port = portToSelect;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AsyncTCPClient"/> class.
        /// </summary>
        ~AsyncTCPClient() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        /// <summary>
        /// Set the logger callback.
        /// </summary>
        /// <param name="act">The callback.</param>
        public void SetLogger(Action<string> act) {
            logger = act;
        }

        /// <summary>
        /// Write the contents out to the socket
        /// </summary>
        /// <param name="whatToWrite">Text to write</param>
        /// <returns>Asnc</returns>
        public async Task WriteToExternalSocket(string whatToWrite) {
            unchecked {
                messagesWritten++;
            }

            if (socketCommunicationsDown && (DateTime.Now - lastSocketException).TotalSeconds < SECONDS_NO_SOCKET_RETRY) {
                status = $"Socket coms down since {lastSocketException}";
                Emergency.Diags.Log(status);
                return;
            }
            socketCommunicationsDown = false;

            byte[] data = Encoding.ASCII.GetBytes(whatToWrite);

            if ((tcpClient == null) || (!tcpClient.Connected)) {
                try {
                    tcpClient = new TcpClient(ipAddress, port);
                    stream = tcpClient.GetStream();
                } catch (SocketException sox) {
                    status = "EXX >> " + sox.Message;
                    Emergency.Diags.Log(status);
                    lastSocketException = DateTime.Now;
                    socketCommunicationsDown = true;
                    return;
                }
            }

            // Should now have reconnected or at least tried to connect.
            try {
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
            } catch (IOException iox) {
                status = "EXX >> " + iox.Message;
                Emergency.Diags.Log(status);
                lastSocketException = DateTime.Now;
                socketCommunicationsDown = true;
                return;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // As Per Pattern: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the status of the handler.
        /// </summary>
        /// <returns>A string containing handler status info</returns>
        internal string GetStatus() {
            return status + $"tried {messagesWritten}";
        }

        /// <summary>
        /// Disposes of the class
        /// </summary>
        /// <param name="disposing">true if disposing</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // As Per Pattern: dispose managed state (managed objects).
                }

                if (stream != null) {
                    stream.Dispose();
                    stream = null;
                }

                if (tcpClient != null) {
                    tcpClient.Close();
                    tcpClient = null;
                }

                // as per pattern: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // as per pattern: set large fields to null.

                disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}