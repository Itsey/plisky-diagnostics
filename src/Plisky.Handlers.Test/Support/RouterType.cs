namespace Plisky.Diagnostics.Test {

    public enum RouterType {

        /// <summary>
        /// Default Entry for the enum.
        /// </summary>
        Unknown = 0x0000,

        /// <summary>
        /// Mock router used only for testing.
        /// </summary>
        Mock,

        /// <summary>
        /// Simple router uses single thread and works in limited environments such as blazor.
        /// </summary>
        Simple,

        /// <summary>
        /// Default router uses a thread to queue the sending of messages to the stream.
        /// </summary>
        Queued
    }
}