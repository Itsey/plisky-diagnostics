#pragma warning disable CS1591 // XML Comments
#pragma warning disable SA1600 // XML Comments

namespace DevConsoleTest {
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Listeners;
    /*
    /// <summary>
    /// Dummy implementation of the rfsh
    /// </summary>
    public class MyRoller : RollingFileSystemHandler {
        /// <summary>
        /// The active date
        /// </summary>
        public DateTime ActiveDate = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyRoller"/> class.
        /// Temporary Rolling File System Handler
        /// </summary>
        /// <param name="sLFHandlerOptions">The options</param>
        public MyRoller(RollingFSHandlerOptions sLFHandlerOptions) : base(sLFHandlerOptions) {
        }

        /// <summary>
        /// The active date time
        /// </summary>
        /// <returns>Current Date</returns>
        public override DateTime GetDateTime() {
            return ActiveDate;
        }

        public override Task HandleMessageAsync(MessageMetadata[] msg) {

            foreach (var l in msg) {
                if (l.StructuredData != null) {
                    var eo = l.StructuredData as ExpandoObject;
                    l.Body += "SD:" + "a";
                }
            }
            return base.HandleMessageAsync(msg);
        }
    }


    */
}
#pragma warning restore CS1591 // XML Comments
#pragma warning restore SA1600 // XML Comments