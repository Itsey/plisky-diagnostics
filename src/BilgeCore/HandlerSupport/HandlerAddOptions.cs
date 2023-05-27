namespace Plisky.Diagnostics {

    /// <summary>
    /// Determines the behaviour on add handler
    /// </summary>
    public enum HandlerAddOptions {

        /// <summary>
        /// Allow duplicates
        /// </summary>
        Duplicates,

        /// <summary>
        /// Only one of a given type
        /// </summary>
        SingleType,

        /// <summary>
        /// Only one of a given name
        /// </summary>
        SingleName
    }
}