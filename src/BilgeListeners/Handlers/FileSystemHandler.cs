namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handler to write the messages to the file system
    /// </summary>
    public class FileSystemHandler : BaseHandler, IBilgeMessageListener {

        /// <summary>
        /// the filestream
        /// </summary>
        protected FileStream fs;

        private FSHandlerOptions opt;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemHandler"/> class.
        /// </summary>
        /// <param name="sLFHandlerOptions">The options.</param>
        public FileSystemHandler(FSHandlerOptions sLFHandlerOptions) {
            opt = sLFHandlerOptions;
            Formatter = DefaultFormatter(false);
            Name = nameof(FileSystemHandler);
        }

        /// <summary>
        /// returns the name of this handler
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the status of this handler
        /// </summary>
        /// <returns>The status string.</returns>
        public string GetStatus() {
            return "OK";
        }

        /// <summary>
        /// async message hanlder
        /// </summary>
        /// <param name="msg">The message</param>
        /// <returns>Task for async.</returns>
        public async Task HandleMessageAsync(MessageMetadata[] msg) {
            fs = new FileStream(opt.FileName, FileMode.Append, FileAccess.Write);

            try {
                var sb = new StringBuilder();
                for (int i = 0; i < msg.Length; i++) {
                    sb.Append(Formatter.Convert(msg[i]));
                    if (!msg[i].Body.EndsWith(Environment.NewLine)) {
                        sb.Append(Environment.NewLine);
                    }
                }

                byte[] data = Encoding.ASCII.GetBytes(sb.ToString().ToCharArray());
                await fs.WriteAsync(data, 0, data.Length);
            } finally {
                fs.Close();
            }
        }

        /// <summary>
        /// Prepares the file system handler to be initialised, using a defined initialisation string.
        /// </summary>
        /// <param name="initialisationString">The string that can be used to initialise the handler</param>
        /// <returns>A prepared and configured filesystemhandler</returns>
        [HandlerInitialisation("FIL")]
        public FileSystemHandler InitiliaseFrom(string initialisationString) {
            return null;
        }
    }
}