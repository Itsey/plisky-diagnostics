namespace Plisky.Diagnostics {

    using System;

    /// <summary>
    /// Attribute used to initialise handlers
    /// </summary>
    public class HandlerInitialisationAttribute : Attribute {

        /// <summary>
        /// Initialises a handler with a specific prefix
        /// </summary>
        /// <param name="listenerPfx">the prefix</param>
        public HandlerInitialisationAttribute(string listenerPfx) {
            ListenerPrefix = listenerPfx;
        }

        /// <summary>
        /// The prefix to use for handler initialisation
        /// </summary>
        public string ListenerPrefix { get; set; }
    }
}