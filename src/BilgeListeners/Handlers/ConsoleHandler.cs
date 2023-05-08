namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.Threading.Tasks;
    using Plisky.Diagnostics;

    /// <summary>
    /// handler to stream to the console
    /// </summary>
    public class ConsoleHandler : BaseHandler, IBilgeMessageListener {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleHandler"/> class.
        /// constructor.
        /// </summary>
        public ConsoleHandler() {
            Formatter = new ConsoleFormatter();
            Priority = 10;
        }

        /// <summary>
        /// name of the handler
        /// </summary>
        public string Name => nameof(ConsoleHandler);

        /// <summary>
        /// Initilaise the handler from an initialisation string
        /// </summary>
        /// <param name="initialisationString">The string to initialise from</param>
        /// <returns>An initialised console handler</returns>
        [HandlerInitialisation("CON")]
        public static ConsoleHandler InitiliaseFrom(string initialisationString) {
            return new ConsoleHandler();
        }

        /// <summary>
        /// returns the status
        /// </summary>
        /// <returns>The status of the handler</returns>
        public string GetStatus() {
            return $"writing ok";
        }

        /// <summary>
        /// handle legacy message routing for net 4
        /// </summary>
        /// <param name="msg">Messages to handle</param>
        /// <returns>Task relating to handling the message</returns>
        public Task HandleMessageAsync(MessageMetadata[] msg) {
            foreach (var v in msg) {
                if (WriteConsolePreamble(v.CommandType)) {
                    string writer = Formatter.Convert(v);
                    Console.WriteLine(writer);
                }
            }

            return new Task(() => { });
        }

        private bool WriteConsolePreamble(TraceCommandTypes commandType) {
            bool doWrite = true;
            string preamble = string.Empty;
            var cc = ConsoleColor.White;

            switch (commandType) {
                case TraceCommandTypes.LogMessageVerb:
                    cc = ConsoleColor.Blue;
                    preamble = "Verb";
                    break;

                case TraceCommandTypes.LogMessage:
                case TraceCommandTypes.LogMessageMini:
                    cc = ConsoleColor.Green;
                    preamble = "Info";
                    break;

                case TraceCommandTypes.SectionStart:
                case TraceCommandTypes.SectionEnd:
                case TraceCommandTypes.ResourceEat:
                case TraceCommandTypes.ResourcePuke:
                case TraceCommandTypes.ResourceCount:
                case TraceCommandTypes.Standard:
                case TraceCommandTypes.CommandData:
                case TraceCommandTypes.Custom:
                case TraceCommandTypes.MoreInfo:
                case TraceCommandTypes.CommandOnly:
                case TraceCommandTypes.InternalMsg:
                case TraceCommandTypes.TraceMessageIn:
                case TraceCommandTypes.TraceMessageOut:
                case TraceCommandTypes.TraceMessage:
                    doWrite = false;
                    break;

                case TraceCommandTypes.AssertionFailed:
                    cc = ConsoleColor.Blue;
                    preamble = "Assert";
                    break;

                case TraceCommandTypes.ErrorMsg:
                    cc = ConsoleColor.Red;
                    preamble = "Error";
                    break;

                case TraceCommandTypes.WarningMsg:
                    cc = ConsoleColor.DarkYellow;
                    preamble = "Warn";
                    break;

                case TraceCommandTypes.ExceptionBlock:
                case TraceCommandTypes.ExceptionData:
                case TraceCommandTypes.ExcStart:
                case TraceCommandTypes.ExcEnd:
                    cc = ConsoleColor.DarkMagenta;
                    preamble = "Fault";
                    break;

                case TraceCommandTypes.Alert:
                    cc = ConsoleColor.Blue;
                    preamble = "Alert";
                    break;

                case TraceCommandTypes.Unknown:
                    doWrite = false;
                    break;
            }

            if (doWrite) {
                Console.ForegroundColor = cc;
                Console.Write(preamble);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" : ");
            }
            return doWrite;
        }
    }
}