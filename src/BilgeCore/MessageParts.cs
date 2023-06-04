namespace Plisky.Diagnostics {

    /// <summary>
    /// Holds all of the parts that are used to create messages.
    /// </summary>
    public class MessageParts {

        /// <summary>
        /// loc data
        /// </summary>
        public string AdditionalLocationData;

        /// <summary>
        /// Classname
        /// </summary>
        public string ClassName;

        /// <summary>
        /// actual message
        /// </summary>
        public string DebugMessage;

        /// <summary>
        /// line no
        /// </summary>
        public string LineNumber;

        /// <summary>
        /// machine name
        /// </summary>
        public string MachineName;

        /// <summary>
        /// Requires replacements
        /// </summary>
        public bool MessagePartsRequiresReplacements;

        /// <summary>
        /// message type
        /// </summary>
        public string MessageType;

        /// <summary>
        /// method name
        /// </summary>
        public string MethodName;

        /// <summary>
        /// module name
        /// </summary>
        public string ModuleName;

        /// <summary>
        /// net thread id
        /// </summary>
        public string NetThreadId;

        /// <summary>
        /// os thread id
        /// </summary>
        public string OSThreadId;

        /// <summary>
        /// Parameter
        /// </summary>
        public string ParameterInfo;

        /// <summary>
        /// prepend
        /// </summary>
        public bool Prepend;

        /// <summary>
        /// procuess id
        /// </summary>
        public string ProcessId;

        /// <summary>
        /// further info
        /// </summary>
        public string SecondaryMessage;

        /// <summary>
        /// Should tirgger a refresh
        /// </summary>
        public bool TriggerRefresh;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParts"/> class.
        /// </summary>
        public MessageParts() {
            MachineName = ModuleName = ClassName = MethodName = LineNumber = OSThreadId = NetThreadId = MessageType = DebugMessage = SecondaryMessage = AdditionalLocationData = ProcessId = string.Empty;
        }
    }
}