namespace Plisky.Diagnostics {

    /// <summary>
    /// Holds all of the parts that are used to create messages.
    /// </summary>
    public class MessageParts {

        /// <summary>
        /// loc data
        /// </summary>
        public string AdditionalLocationData { get; set; }

        /// <summary>
        /// Classname
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// actual message
        /// </summary>
        public string DebugMessage { get; set; }

        /// <summary>
        /// line no
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// machine name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Requires replacements
        /// </summary>
        public bool MessagePartsRequiresReplacements { get; set; }

        /// <summary>
        /// message type
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// module name
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// net thread id
        /// </summary>
        public string NetThreadId { get; set; }

        /// <summary>
        /// os thread id
        /// </summary>
        public string OSThreadId { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        public string ParameterInfo { get; set; }

        /// <summary>
        /// prepend
        /// </summary>
        public bool Prepend { get; set; }

        /// <summary>
        /// procuess id
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// further info
        /// </summary>
        public string SecondaryMessage { get; set; }

        /// <summary>
        /// Should tirgger a refresh
        /// </summary>
        public bool TriggerRefresh { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParts"/> class.
        /// </summary>
        public MessageParts() {
            MachineName = ModuleName = ClassName = MethodName = LineNumber = OSThreadId = NetThreadId = MessageType = DebugMessage = SecondaryMessage = AdditionalLocationData = ProcessId = string.Empty;
        }
    }
}