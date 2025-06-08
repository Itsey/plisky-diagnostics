namespace Plisky.Diagnostics.Listeners {

    /// <summary>
    /// Options for the in memory handler
    /// </summary>
    public class InMemoryHandlerOptions : HandlerOptions {

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryHandlerOptions"/> class.
        /// </summary>
        /// <param name="queueDepth">How many messages should be stored in the queue before old messages are lost</param>
        public InMemoryHandlerOptions(int queueDepth = 5000) : base(string.Empty) {
            MaxQueueDepth = queueDepth;
            ClearOnGet = true;
        }

        /// <summary>
        /// When true then a get of the InMemoryHandler will also clear the message queue.
        /// </summary>
        public bool ClearOnGet { get; set; }

        /// <summary>
        /// maximum queue depth for the handler
        /// </summary>
        public int MaxQueueDepth { get; set; }
    }
}