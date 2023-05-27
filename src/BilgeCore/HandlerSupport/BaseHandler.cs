namespace Plisky.Diagnostics.Listeners {

    /// <summary>
    /// Base class to add handler support to take messages from the Bilge trace stream.
    /// </summary>
    public abstract class BaseHandler {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHandler"/> class with a default priority of 5.
        /// </summary>
        protected BaseHandler() {
            Priority = 5;
            Formatter = DefaultFormatter(false);
        }

        /// <summary>
        /// A Formatter to format the raw message stream into the outgoing message stream
        /// </summary>
        public IMessageFormatter Formatter { get; protected set; }

        /// <summary>
        /// The priority of this handler relative to other handlers
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Indicates that the trace stream should close and clean up all resources.
        /// </summary>
        public virtual void CleanUpResources() {
        }

        /// <summary>
        /// Indicates that the trace stream should be sent to the outputter as soon as possible.
        /// </summary>
        public virtual void Flush() {
        }

        /// <summary>
        /// Changes the current formatter to a custom one
        /// </summary>
        /// <param name="fmt">The new formatter to use</param>
        public void SetFormatter(IMessageFormatter fmt) {
            if (fmt != null) {
                Formatter = fmt;
            }
        }

        /// <summary>
        /// Returns the selected default formatter to the handler
        /// </summary>
        /// <param name="interactive">Determines whether the formatter should be interactive ( human readable) </param>
        /// <returns>The default formatter to use</returns>
        protected IMessageFormatter DefaultFormatter(bool interactive) {
            if (interactive) {
                return new PrettyReadableFormatter();
            }

            return new LegacyFlimFlamFormatter();
        }
    }
}