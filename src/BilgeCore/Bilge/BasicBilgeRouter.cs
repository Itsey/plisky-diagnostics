namespace Plisky.Diagnostics {

    using System;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Basic bilge router is there for applications such as blazor that can not use multithreaded routers.
    /// </summary>
    internal class BasicBilgeRouter : BilgeRouter {
        private volatile bool shutdownEnabled;

        private volatile bool shutdownRequested = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBilgeRouter"/> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        internal BasicBilgeRouter(string processId) : base(processId) {
        }

        /// <summary>
        /// Clear everything down.
        /// </summary>
        public override void ActualClearEverything() {
        }

        /// <summary>
        /// Reset everything.
        /// </summary>
        public override void ActualReInitialise() {
            shutdownRequested = false;
            shutdownEnabled = false;
        }

        /// <summary>
        /// Shut everything down.
        /// </summary>
        public override void ActualShutdown() {
            shutdownRequested = true;
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="msgs">The messages to queue.</param>
        protected override async void ActualAddMessage(MessageMetadata[] msgs) {
            if (shutdownEnabled || System.Environment.HasShutdownStarted) {
                return;
            }

            var hndlr = handlers;
            if (hndlr == null || hndlr.Length == 0 || msgs.Length == 0) { return; }

            PrepareMessage(msgs);

            var tasks = new Task[hndlr.Length];
            try {
                for (int i = 0; i < hndlr.Length; i++) {
                    tasks[i] = hndlr[i].HandleMessageAsync(msgs);
                }
                await Task.WhenAll(tasks);
            } catch (Exception) {
                ErrorCount++;
                if (!SuppressHandlerErrors) {
                    throw;
                }
            }
        }

        /// <summary>
        /// Forces a flush of all messages.
        /// </summary>
        protected override void ActualFlushMessages() {
            Emergency.Diags.Log($"Flush, done ");
        }

        /// <summary>
        /// Gets the status from all the handlers
        /// </summary>
        /// <param name="sb">A string builder to add the statuses into.</param>
        /// <returns>A message from the handlers indiciating their current status.</returns>
        protected override StringBuilder ActualGetHandlerStatuses(StringBuilder sb) {
            sb.Append($"Simple Router Active\n");
            sb.Append("___________________________\n");

            return sb;
        }

        /// <summary>
        /// Determines wheter the current configuration is clean
        /// </summary>
        /// <returns>True if currently clean</returns>
        protected override bool ActualIsClean() {
            return true;
        }
    }
}