namespace Plisky.Diagnostics {

    /// <summary>
    /// Interface for action support
    /// </summary>
    public interface IBilgeActionEvent {

        /// <summary>
        /// Number of times action occured
        /// </summary>
        int CallCount { get; }

        /// <summary>
        /// Data message
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Meta data
        /// </summary>
        string Meta { get; }

        /// <summary>
        /// Name of the action
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Action was succesfull or not
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Directly set the call count
        /// </summary>
        /// <param name="i">the call count</param>
        void SetCallCount(int i);
    }
}