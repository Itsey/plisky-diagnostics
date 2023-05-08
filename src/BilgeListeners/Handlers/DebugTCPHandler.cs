namespace Plisky.Diagnostics.Listeners {
#if false
    /// <summary>
    /// A debugging tcp handler that also logs out messages
    /// </summary>
    public class DebugTCPHandler : TCPHandler {
        /// <summary>
        /// trace messages
        /// </summary>
        public Queue<string> TraceMessages = new Queue<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugTCPHandler"/> class.
        /// Adds internal logging
        /// </summary>
        /// <param name="targetIp">The target IP that FlimFlam is listening on</param>
        /// <param name="targetPort">The target port that FlimFlam is using</param>
        /// <param name="harshFails">Whether an assert failure should throw an exception</param>
        public DebugTCPHandler(string targetIp, int targetPort, bool harshFails = false) : base(targetIp, targetPort, harshFails) {
            InternalLog($"Online {targetIp}:{targetPort}");
        }

        /// <summary>
        /// Send internal log
        /// </summary>
        /// <param name="what"></param>
        public void InternalLog(string what) {
            TraceMessages.Enqueue(what);
        }
    }
#endif
}