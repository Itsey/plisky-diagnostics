namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Default simple file handler
    /// </summary>
    public class SimpleTraceFileHandler : BaseHandler, IBilgeMessageListener {
        private WeakReference lastTask;

        /// <summary>
        /// This will create a simple text file listener designed for reading by a person, not for importing into
        /// a trace reader.
        /// </summary>
        /// <param name="pathForLog">The directory to place the file in (Defaults to %TEMP%)</param>
        /// <param name="overwriteEachTime">If set to true the same filename will be used, if false the current time will be appended</param>
        public SimpleTraceFileHandler(string pathForLog = null, bool overwriteEachTime = true) {
            string fn;
            if (string.IsNullOrEmpty(pathForLog)) {
                fn = Path.GetTempPath();
            } else {
                fn = pathForLog;
                if (!Directory.Exists(fn)) {
                    Directory.CreateDirectory(fn);
                }
            }

            if (!Directory.Exists(fn)) {
                throw new DirectoryNotFoundException(fn);
            }

            string actualFilename = "bilgedefault.log";
            if (!overwriteEachTime) {
                actualFilename = "bilgedefault" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + ".log";
            }
            pathForLog = Path.Combine(fn, actualFilename);
            TraceFilename = pathForLog;
            Formatter = new PrettyReadableFormatter();
        }

        /// <summary>
        /// name of the handler
        /// </summary>
        public string Name => nameof(SimpleTraceFileHandler);

        /// <summary>
        /// filename to write to
        /// </summary>
        public string TraceFilename { get; private set; }

        /// <summary>
        /// Initialisation string based initialiser for the simple trace file handler
        /// </summary>
        /// <param name="initialisationString">The string to initialise with</param>
        /// <returns>A handler ready to be added</returns>
        [HandlerInitialisation("SFL")]
        public static IBilgeMessageListener InitiliaseFrom(string initialisationString) {
            if (initialisationString.StartsWith("SFL:")) {
                initialisationString = initialisationString.Substring(4);
            }

            if (Directory.Exists(initialisationString)) {
                return new SimpleTraceFileHandler(initialisationString);
            }
            return new SimpleTraceFileHandler();
        }

#if !NET40ONLY
        /// <summary>
        /// modern async handle
        /// </summary>
        /// <param name="msg">The messages to handle</param>
        /// <returns>The task after messageis handled</returns>
        public async Task HandleMessageAsync(MessageMetadata[] msg) {
            var sb = new StringBuilder();
            foreach (var v in msg) {
                sb.Append(Formatter.Convert(v));
            }
            byte[] txt = Encoding.UTF8.GetBytes(sb.ToString());

            using (var fs = GetFileStream()) {
                var tsk = fs.WriteAsync(txt, 0, txt.Length);
                lastTask = new WeakReference(tsk);
                await tsk;
            }
        }
#endif

        /// <summary>
        /// clean up the resources.
        /// </summary>
        public override void CleanUpResources() {
            if ((lastTask != null) && lastTask.IsAlive) {
                var tsk = (Task)lastTask.Target;
                if (tsk != null) {
                    tsk.Wait();
                }
            }
        }

        /// <summary>
        /// flush the stream.
        /// </summary>
        public override void Flush() {
            var tsk = (Task)lastTask.Target;
            if (tsk != null) {
                tsk.Wait();
            }
        }

        /// <summary>
        /// get the status
        /// </summary>
        /// <returns></returns>
        public string GetStatus() {
            return $"Writing to {TraceFilename}";
        }

        private FileStream GetFileStream() {
            return new FileStream(TraceFilename, FileMode.Create);
        }
    }
}