namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// InMemoryHandler is, as suggested by the name a handler that stores the log data in memory to be retireved later.  Therefore it does not actually
    /// write the data anyhwere.  It is up to the caller to retrieve the messages.  As such you should configure maximum queue depths to ensure that not
    /// too many resources are consumed by old logs.
    /// </summary>
    public class InMemoryHandler : BaseHandler, IBilgeMessageListener {

        /// <summary>
        /// Get all the messages.
        /// </summary>
        protected Queue<MessageMetadata> messages = new Queue<MessageMetadata>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryHandler"/> class.
        /// default constructor for the imh.
        /// </summary>
        /// <param name="imho">Memory handler options.</param>
        public InMemoryHandler(InMemoryHandlerOptions imho) : this() {
            if (imho != null) {
                MaxQueueDepth = imho.MaxQueueDepth;
                ClearOnGet = imho.ClearOnGet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryHandler"/> class.
        /// constructor allowing depth override for the inmemoryhandler.
        /// </summary>
        /// <param name="maxDepth">the maximum queue depth.</param>
        public InMemoryHandler(int maxDepth = 5000) {
            Formatter = DefaultFormatter(false);
            MaxQueueDepth = maxDepth;
            ClearOnGet = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether clear down the queue on get.
        /// </summary>
        public bool ClearOnGet { get; set; }

        /// <summary>
        /// Gets or sets return the max queue depth.
        /// </summary>
        public int MaxQueueDepth { get; set; }

        /// <summary>
        /// Gets or sets a message callback, this is called each time that a message is written to the in memory handler. The return value from this func is
        /// placed into the internal message queue.  If null is returned no message is added to the internal message queue.
        /// </summary>
        public Func<MessageMetadata, MessageMetadata> MessageCallback { get; set; }

        /// <summary>
        /// Gets name of the handler.
        /// </summary>
        public string Name => nameof(InMemoryHandler);

        /// <summary>
        /// Return all of the messages.
        /// </summary>
        /// <returns>All of the messages.</returns>
        public string[] GetAllMessages() {
            var exm = messages;
            if (ClearOnGet) {
                messages = new Queue<MessageMetadata>();
            }

            return exm.Select(p => Formatter.ConvertWithReference(p, p.Index.ToString())).ToArray();
        }

        /// <summary>
        /// Returns the oldest message, removing it from the queue if clear on get is set.
        /// </summary>
        /// <returns>The message.</returns>
        public string GetMessage() {
            if (messages.Count == 0) {
                return null;
            }

            var msg = ClearOnGet ? messages.Dequeue() : messages.Peek();
            return Formatter.ConvertWithReference(msg, msg.Index.ToString());
        }

        /// <summary>
        /// Get the count of messages.
        /// </summary>
        /// <returns>The message count.</returns>
        public int GetMessageCount() {
            return messages.Count;
        }

        /// <summary>
        /// returns the status.
        /// </summary>
        /// <returns>The status.</returns>
        public string GetStatus() {
            return $"writing ok, current depth {GetMessageCount()} maxDepth {MaxQueueDepth}";
        }

        /// <summary>
        /// modern message handling.
        /// </summary>
        /// <param name="msg">the messages.</param>
        /// <returns>async task.</returns>
        public Task HandleMessageAsync(MessageMetadata[] msg) {
            var sb = new StringBuilder();
            foreach (var v in msg) {
                var nextMessage = v;
                if (MessageCallback != null) {
                    nextMessage = MessageCallback(nextMessage);
                }

                if (nextMessage != null) {
                    messages.Enqueue(nextMessage);
                }

                if (messages.Count > MaxQueueDepth) {
                    _ = messages.Dequeue();
                }
            }

#if NET452
            return new Task(() => { });
#else
            return Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Initialises from an initialisation string.
        /// </summary>
        /// <param name="initialisationString">the string to use.</param>
        /// <returns>a new initialised listener.</returns>
        public IBilgeMessageListener InitiliaseFrom(string initialisationString) {
            return null;
        }
    }
}