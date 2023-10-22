namespace Plisky.Diagnostics {
    /// <summary>
    /// Utility class for bilge
    /// </summary>
    public class BilgeUtil : BilgeRoutedBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeUtil"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="rt">The router to use.</param>
        /// <param name="cs">The settings to use.</param>
        public BilgeUtil(BilgeRouter rt, ConfigSettings cs) : base(rt, cs) {
        }
    }
}