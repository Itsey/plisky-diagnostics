#pragma warning disable CS1591 // XML Comments
#pragma warning disable SA1600 // XML Comments

namespace DevConsoleTest {

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Listeners;

    /// <summary>
    /// A custom handler
    /// </summary>
    public class CustomHandler : BaseHandler, IBilgeMessageListener {
        private string statusString = "OK";

        /// <summary>
        /// Returns the name of the custom handler
        /// </summary>
        public string Name => nameof(CustomHandler);

        /// <summary>
        /// Returns the status of the custom handler.
        /// </summary>
        /// <returns>A string indicating the status</returns>
        public string GetStatus() {
            return "OK";
        }

        public Task HandleMessageAsync(MessageMetadata[] msg) {
            try {
                foreach (var nextMsg in msg) {
                    string f = JsonSerializer.Serialize(nextMsg, typeof(MessageMetadata));
                    Console.WriteLine(f);
                }
            } catch (NotSupportedException nx) {
                statusString = "HandlerFailed > " + DateTime.Now.ToString() + " > " + nx.Message;
            }
            return Task.CompletedTask;
        }
    }
}

#pragma warning restore CS1591 // XML Comments
#pragma warning restore SA1600 // XML Comments