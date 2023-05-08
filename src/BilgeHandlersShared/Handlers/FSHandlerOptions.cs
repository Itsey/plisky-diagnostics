namespace Plisky.Diagnostics.Listeners {

    /// <summary>
    /// filestream handler options
    /// </summary>
    public class FSHandlerOptions : HandlerOptions {

        /// <summary>
        /// Initializes a new instance of the <see cref="FSHandlerOptions"/> class.
        /// filestream handler options
        /// </summary>
        /// <param name="v">The options to use </param>
        public FSHandlerOptions(string v) : base(v) {
        }

        /// <summary>
        /// the filename
        /// </summary>
        public string FileName {
            get {
                return InitialisationString;
            }
        }
    }
}