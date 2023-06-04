namespace Plisky.Diagnostics {

    using System.Runtime.CompilerServices;

    /// <summary>
    /// Writes directly to the trace stream
    /// </summary>
    public class BilgeDirect {

        /// <summary>
        /// Provides access to configuration
        /// </summary>
        protected ConfigSettings config;

        private bool isWriting = true;

        private BilgeRouter router;

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeDirect"/> class.
        /// The direct router.
        /// </summary>
        /// <param name="r">The router.</param>
        /// <param name="activeConfig">Current configuration.</param>
        internal BilgeDirect(BilgeRouter r, ConfigSettings activeConfig) {
            router = r;
            config = activeConfig;
        }

        /// <summary>
        /// Current context
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Writes a message to the stream
        /// </summary>
        /// <param name="body">Body Text</param>
        /// <param name="further">Further info</param>
        /// <param name="meth">method</param>
        /// <param name="pth">filename</param>
        /// <param name="ln">line number</param>
        public void Write(string body, string further, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            var mmd = new MessageMetadata(meth, pth, ln);
            mmd.CommandType = TraceCommandTypes.Custom;
            mmd.Body = body;
            mmd.FurtherDetails = further;

            if (isWriting) {
                router.PrepareMetaData(mmd, config.MetaContexts);
                router.QueueMessage(mmd);
            }
        }

        /// <summary>
        /// Writes MessageMetaData directly out to the router and then the trace stream.
        /// </summary>
        /// <param name="mmd">The message to write</param>
        public void Write(MessageMetadata mmd) {
            if (isWriting) {
                router.PrepareMetaData(mmd, config.MetaContexts);
                router.QueueMessage(mmd);
            }
        }
    }
}