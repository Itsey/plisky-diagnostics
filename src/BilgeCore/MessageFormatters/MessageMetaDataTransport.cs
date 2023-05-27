namespace Plisky.Diagnostics {
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1300 // Naming Styles
#pragma warning disable SA1623 // Documentation Style
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Class is used to transport messagemetadata, using shortened names and so on to reduce the amount of data to as little as possible, this class should not
    /// be used unless its to turn a formatted message from the FlimFlamV2 Formatter class back into a message meta data.  The JSON attributes to support shrining
    /// the names are only available in net core apps therefore have shrunk the names of the properties themselves to avoid conditiona code.
    /// </summary>
    public class MessageMetaDataTransport {
        /// <summary>
        /// Date time as string format.
        /// </summary>
        public string dt { get; set; }

        /// <summary>
        /// message
        /// </summary>
        public string m { get; set; }

        /// <summary>
        /// secondary
        /// </summary>
        public string s { get; set; }

        /// <summary>
        /// commmand type
        /// </summary>
        public string mt { get; set; }

        /// <summary>
        /// classname
        /// </summary>
        public string c { get; set; }

        /// <summary>
        /// linenumber
        /// </summary>
        public string l { get; set; }

        /// <summary>
        /// process name
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// OSthreadId
        /// </summary>
        public string t { get; set; }

        /// <summary>
        /// NetthreadId
        /// </summary>
        public string nt { get; set; }

        /// <summary>
        /// MethodName
        /// </summary>
        public string mn { get; set; }

        /// <summary>
        /// MachineName
        /// </summary>
        public string man { get; set; }

        /// <summary>
        /// alt loc
        /// </summary>
        public string al { get; set; }

        /// <summary>
        /// module / filename
        /// </summary>
        public string md { get; set; }

        /// <summary>
        /// uniqueness identifier
        /// </summary>
        public string uq { get; set; }

        /// <summary>
        /// messageversion
        /// </summary>
        public string v { get; set; }

        public void FromMessageMetaData(MessageMetadata source) {
            v = "2";
            uq = "--uq--";
            md = source.FileName;
            al = string.Empty;
            man = source.MachineName;
            mn = source.MethodName;
            nt = source.NetThreadId;
            t = source.OsThreadId;
            ProcessName = source.ProcessId;
            s = source.FurtherDetails;
            m = source.Body;
            mt = TraceCommands.TraceCommandToString(source.CommandType);
        }

        public MessageMetadata ToMessageMetaData() {
            var source = new MessageMetadata();
            source.FileName = md;
            source.MachineName = man;
            source.MethodName = mn;
            source.NetThreadId = nt;
            source.OsThreadId = t;
            source.ProcessId = ProcessName;
            source.FurtherDetails = s;
            source.Body = m;
            source.CommandType = TraceCommands.StringToTraceCommand(mt);

            return source;
        }

        public TraceCommandTypes GetCommandType() {
            return TraceCommands.StringToTraceCommand(mt);
        }
    }

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}