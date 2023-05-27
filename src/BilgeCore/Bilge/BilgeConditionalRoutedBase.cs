namespace Plisky.Diagnostics {

    using System.Diagnostics;

    /// <summary>
    /// A router that has conditions on whether or not messages are routed
    /// </summary>
    public abstract class BilgeConditionalRoutedBase : BilgeRoutedBase {

        /// <summary>
        /// The current trace level
        /// </summary>
        protected SourceLevels activeTraceLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeConditionalRoutedBase"/> class.
        /// </summary>
        /// <param name="rt">The Router to use</param>
        /// <param name="cs">Any configuration settings</param>
        /// <param name="yourTraceLevel">Trace Level to use</param>
        public BilgeConditionalRoutedBase(BilgeRouter rt, ConfigSettings cs, SourceLevels yourTraceLevel) : base(rt, cs) {
            activeTraceLevel = yourTraceLevel;
        }

        /// <summary>
        /// Holds the instance context
        /// </summary>
        public string ContextCache {
            get {
                return sets.InstanceContext;
            }
        }

        /// <summary>
        /// Determines whether this conditional router is actually writing to the stream.
        /// </summary>
        public bool IsWriting { get; set; }

        /// <summary>
        /// Perform message routing
        /// </summary>
        /// <param name="mmd">The message metadata to route</param>
        protected override void ActiveRouteMessage(MessageMetadata mmd) {
            if (IsWriting) {
                base.ActiveRouteMessage(mmd);
            }
        }
    }
}