namespace Plisky.Diagnostics {

    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Bilge configuration (options) storage
    /// </summary>
    public class BilgeConfiguration {
        private List<string> handlers = new List<string>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public BilgeConfiguration() {
        }

        /// <summary>
        /// A series of strings that are used to add handlers to the handlers collection. Each one MUST be a properly formatted string, use AddHandler
        /// to add to this array.
        /// </summary>
        public string[] HandlerStrings {
            get {
                return handlers.ToArray();
            }
        }

        /// <summary>
        /// Indicates that a new resolver should be created, using the resolver matches to match on the name and that it should set the source
        /// level for each of the matches.  If * is used as a resolver match then ALL requests will match.
        /// </summary>
        public SourceLevels OverallSourceLevel { get; set; }

        /// <summary>
        /// The initialisation string for the resolver
        /// </summary>
        public string ResolverInitString { get; set; }

        /// <summary>
        /// Indicates whether the existing handlers should all be removed, before attempting to add any of the handlers that are listed in the
        /// HandlerStrings section of the configuration.
        /// </summary>
        public bool StripHandlers { get; set; }

        /// <summary>
        /// Add a handler using a configuration string.
        /// </summary>
        /// <param name="handlerString"></param>
        public void AddHandler(string handlerString) {
            handlers.Add(handlerString);
        }
    }
}