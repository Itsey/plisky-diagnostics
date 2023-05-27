namespace Plisky.Diagnostics.Listeners {

    /// <summary>
    /// default handler options for all handlers
    /// </summary>
    public class HandlerOptions {

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerOptions"/> class.
        /// This constructor initialises from a string, for the default instance it just stores the string but for specific
        /// listneners it will initialise accordingly.
        /// </summary>
        /// <param name="init">The initialisation string</param>
        public HandlerOptions(string init) {
            InitialisationString = init;
            CanCreate = true;
        }

        /// <summary>
        /// Indicates whether these options have everything that they need to be able to create an instance of
        /// the class that they represent options for.
        /// </summary>
        public bool CanCreate { get; set; }

        /// <summary>
        /// The initialisation string passed in on construct.
        /// </summary>
        public string InitialisationString { get; set; }

        /// <summary>
        /// Determines wheter we can create based on the current initalisation string
        /// </summary>
        /// <returns>True if there is enough info to create.</returns>
        public virtual bool Parse() {
            CanCreate = false;
            return CanCreate;
        }
    }
}