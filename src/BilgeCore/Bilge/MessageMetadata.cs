using System;

namespace Plisky.Diagnostics {
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Describes meta data for a trace message, this is how trace content is sent to the handlers for processing.
    /// </summary>
    public class MessageMetadata {
        private static long baseIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageMetadata"/> class.
        /// </summary>
        public MessageMetadata() {
            Index = Interlocked.Increment(ref baseIndex);
            MessageTags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageMetadata"/> class.
        /// </summary>
        /// <param name="method">The method where the trace originated</param>
        /// <param name="sourcePath">The filename and path where the trace originated</param>
        /// <param name="lineNo">The line number of the source file where the trace entry occured</param>
        public MessageMetadata(string method, string sourcePath, int lineNo) : this() {
            MethodName = method;
            FileName = sourcePath;
            LineNumber = lineNo.ToString();
        }

        /// <summary>
        /// The contents of the trace message itself.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The classname where the trace took place.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// What command type is this message, including the level and type of message.
        /// </summary>
        public TraceCommandTypes CommandType { get; set; }

        /// <summary>
        /// The context of the trace message.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// FilmFlamVersion an identifier holding the version of the message.
        /// </summary>
        public string Ffv { get; set; }

        /// <summary>
        /// The source code filename where the method calling the trace function was at the time of the call.  Requires PDB files to be present.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Supporting and additional information relating to the trace method.
        /// </summary>
        public string FurtherDetails { get; set; }

        /// <summary>
        /// Raw index for the message.
        /// </summary>
        public long Index { get; private set; }

        /// <summary>
        /// Line number where trace originated.
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// The machine name where the trace originated from.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Adds an expansable set of tags and context to the message, to provide additional information or context to the trace message.
        /// </summary>
        public IDictionary<string, string> MessageTags { get; set; }

        /// <summary>
        /// The method name where trace originated.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Thread identity, separate to underlying OS Thread Identity.
        /// </summary>
        public string NetThreadId { get; set; }

        /// <summary>
        /// The identity of the operating system level thread.
        /// </summary>
        public string OsThreadId { get; set; }

        /// <summary>
        /// PID of the process that is generating the trace content.
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// Dynamic additonal data to accompany the trace message, used for structured logging of data.
        /// </summary>
        public dynamic StructuredData { get; set; }

        /// <summary>
        /// The time at which the trace entry was written, note that this is optional.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Create a new MMD copying all of the content to the new one, note that structured data will be a reference to the existing structured
        /// data but all of the other content is deep copied across.
        /// </summary>
        /// <returns>A new MMD with all of the fields copied across.</returns>
        public MessageMetadata Clone() {
#pragma warning disable IDE0003
            var mmd = new MessageMetadata() {
                CommandType = this.CommandType,
                Context = this.Context,
                MethodName = this.MethodName,
                FileName = this.FileName,
                LineNumber = this.LineNumber,
                Body = this.Body,
                FurtherDetails = this.FurtherDetails,
                MachineName = this.MachineName,
                ProcessId = this.ProcessId,
                NetThreadId = this.NetThreadId,
                OsThreadId = this.OsThreadId,
                ClassName = this.ClassName,
                TimeStamp = this.TimeStamp
            };

            // Unclear on how to deep copy a dynamic structure therefore have just copied it.
            mmd.StructuredData = this.StructuredData;
            if (this.MessageTags != null) {
                mmd.MessageTags = new Dictionary<string, string>();
                foreach (string l in this.MessageTags.Keys) {
                    mmd.MessageTags.Add(l, this.MessageTags[l]);
                }
            }
            return mmd;
#pragma warning restore IDE0003
        }

        /// <summary>
        /// Ensures that all of the content is set to empty strings rather than null.
        /// </summary>
        public void NullsToEmptyStrings() {
            Context = Context ?? string.Empty;
            MethodName = MethodName ?? string.Empty;
            FileName = FileName ?? string.Empty;
            LineNumber = LineNumber ?? string.Empty;
            Body = Body ?? string.Empty;
            FurtherDetails = FurtherDetails ?? string.Empty;
            MachineName = MachineName ?? string.Empty;
            ProcessId = ProcessId ?? string.Empty;
            NetThreadId = NetThreadId ?? string.Empty;
            OsThreadId = OsThreadId ?? string.Empty;
            ClassName = ClassName ?? string.Empty;
        }

        /// <summary>
        /// Returns a string holding the index and the body properties.
        /// </summary>
        /// <returns>String of Index and Body.</returns>
        public override string ToString() {
            return $"{Index}:{Body}";
        }
    }
}