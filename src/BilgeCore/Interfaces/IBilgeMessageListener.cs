namespace Plisky.Diagnostics {
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handler support
    /// </summary>
    public interface IBilgeMessageListener {
        /// <summary>
        /// Priority of this handler.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Name of this handler.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the current status of the listener, typically "OK" for most operations or an error message where writing to the trace stream has failed recently.
        /// </summary>
        /// <returns>A string representing the current status of the listener.</returns>
        string GetStatus();

        /// <summary>
        /// Add the correct formatter to the listener.
        /// </summary>
        /// <param name="fmt">The formatter to use to set for formatting future messages.</param>
        void SetFormatter(IMessageFormatter fmt);

        /// <summary>
        /// Perform the default message handling operations.
        /// </summary>
        /// <param name="msg">One or more messages to add to the trace stream.</param>
        /// <returns>A task to use for async processing.</returns>
        Task HandleMessageAsync(MessageMetadata[] msg);

       

        /// <summary>
        /// force buffer flush
        /// </summary>
        void Flush();

        /// <summary>
        /// This is a custom dispose implementation because the internals will call dispose but
        /// that causes FxCop style rules therefore this method is used to clear resources instead.
        /// </summary>
        void CleanUpResources();
    }
}