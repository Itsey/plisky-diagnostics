namespace Plisky.Diagnostics {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Primary message router
    /// </summary>
    public abstract class BilgeRouter {

        /// <summary>
        /// Provides access to all the call counts of action calls
        /// </summary>
        public Dictionary<string, int> ActionCallCountRecord = new Dictionary<string, int>();

        /// <summary>
        /// all action handlers
        /// </summary>
        protected List<Tuple<string, Action<IBilgeActionEvent>>> ActionHandlers = new List<Tuple<string, Action<IBilgeActionEvent>>>();

        /// <summary>
        /// Timer for delayed writes
        /// </summary>
        protected Stopwatch elapsedTimer;

        /// <summary>
        /// Lock to be used around the handlers array.
        /// </summary>
        protected object handlerLock = new object();

        /// <summary>
        /// curreently loaded handlers
        /// </summary>
        protected IBilgeMessageListener[] handlers;

        /// <summary>
        /// Holds the incomming messages
        /// </summary>
        protected ConcurrentQueue<MessageMetadata> incommingMessageQueue = new ConcurrentQueue<MessageMetadata>();

        /// <summary>
        /// If true then this router is shutting down
        /// </summary>
        protected bool ShutdownRequestActive = false;

        private const int MAX_LOOP_BEFORE_WRITE = 250;
        private static string processIdCache = null;
        private static BilgeRouter rtr;
        private Exception flushHiddenException;

        private volatile int messageQueueMaximum = -1;

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeRouter"/> class.
        /// Constructor for the router
        /// </summary>
        /// <param name="processId">The process ID associated with this router</param>
        protected BilgeRouter(string processId) {
            ProcessIdCache = processId;
            string mn;
            try {
                mn = Environment.MachineName;
            } catch (InvalidOperationException) {
                // Swallow this - on containers cant get machine name.
                mn = "Unknown";
            }
            MachineNameCache = mn;
            elapsedTimer = new Stopwatch();
            elapsedTimer.Start();
        }

        /// <summary>
        /// Lazy initialisation of the router itself
        /// </summary>
        public static BilgeRouter Router {
            get {
                if (rtr == null) {
                    try {
                        rtr = new QueuedBilgeRouter(GetProcessIdentity());
                    } catch (PlatformNotSupportedException) {
                        SetBasicRouting();
                    }
                }
                return rtr;
            }
        }

        /// <summary>
        /// Tells you how many errors the handlers have thrown.
        /// </summary>
        public int ErrorCount { get; protected set; } = 0;

        /// <summary>
        /// If true then only writes when a failure occurs
        /// </summary>
        public bool FailureOccuredForWrite { get; set; }

        /// <summary>
        /// determines if stack information should be injected
        /// </summary>
        public bool InjectStackInformation { get; set; } = false;

        /// <summary>
        /// Caches the  name of the machine that is being used in trace
        /// </summary>
        public string MachineNameCache { get; private set; }

        /// <summary>
        /// Limit the number of messages that are grouped together before a write
        /// </summary>
        public int MessageBatchCapacity { get; set; }

        /// <summary>
        /// Limit the time spent being grouped before a write
        /// </summary>
        public int MessageBatchDelay { get; set; }

        /// <summary>
        /// True if message queuing is enabled
        /// </summary>
        public bool QueueMessages { get; set; }

        /// <summary>
        /// When set to true handler errors are caught and swallowed by the router, when set to false they are thrown.
        /// </summary>
        public bool SuppressHandlerErrors { get; set; } = true;

        /// <summary>
        /// IF this is true messages are only written when a failure occurs.
        /// </summary>
        public bool WriteToHandlerOnlyOnFail { get; set; }

        /// <summary>
        /// True if there are handlers
        /// </summary>
        protected bool HaveHandlers { get; set; } = false;

        /// <summary>
        /// Caches the identity of the process that is running trace
        /// </summary>
        protected string ProcessIdCache { get; private set; }

        /// <summary>
        /// Determines whether the code sould try and retrieve the current thread id, if an error occurs this is set to false to prevent retries.
        /// </summary>
        protected bool ShouldGetCurrentThreadId { get; set; } = true;

        /// <summary>
        /// Flips the router to being a single threaded router that is suited for environments that can not
        /// support threads being created.
        /// </summary>
        /// <param name="br">The bilge router to use.</param>
        public static void SetBasicRouting(BilgeRouter br = null) {
            if (br != null) {
                rtr = br;
            } else {
                rtr = new BasicBilgeRouter(GetProcessIdentity());
            }
        }

        /// <summary>
        /// Triggers an action
        /// </summary>
        /// <param name="evt">The action event interface to call</param>
        /// <param name="configSettings">Applicaiton configuration</param>
        public void ActionOccurs(IBilgeActionEvent evt, ConfigSettings configSettings) {
            if (!ActionCallCountRecord.ContainsKey(evt.Name)) {
                ActionCallCountRecord.Add(evt.Name, 1);
            } else {
                ActionCallCountRecord[evt.Name] += 1;
            }
            evt.SetCallCount(ActionCallCountRecord[evt.Name]);

            var mmd = new MessageMetadata();

            mmd.CommandType = TraceCommandTypes.Custom;
            mmd.ClassName = "Bilge-Action";
            mmd.FileName = "Bilge-Action";
            mmd.Body = $"[BilgeAction]-]{evt.Name}[--]{evt.Data ?? "null"}[--]{evt.CallCount}";
            mmd.FurtherDetails = evt.Meta;
            mmd.Context = "Bilge-Action";
            mmd.LineNumber = "0";

            PrepareMetaData(mmd, configSettings.MetaContexts);
            QueueMessage(mmd);

            foreach (var f in ActionHandlers) {
                try {
                    f.Item2(evt);
                } catch (NullReferenceException) {
                    // Swallow at least this exception.
                }
            }
        }

        /// <summary>
        /// Clear everything down.
        /// </summary>
        public abstract void ActualClearEverything();

        /// <summary>
        /// Reset everything.
        /// </summary>
        public abstract void ActualReInitialise();

        /// <summary>
        /// Shut everything down.
        /// </summary>
        public abstract void ActualShutdown();

        /// <summary>
        /// Adds an action handler ( V3.x onwards)
        /// </summary>
        /// <param name="action">The action to call when the event is triggered</param>
        /// <param name="name">The name of the action to call</param>
        /// <returns>true if the action was executed</returns>
        public bool AddActionHandler(Action<IBilgeActionEvent> action, string name) {
            name = name.ToLower();

            foreach (var f in ActionHandlers) {
                if (f.Item1 == name) {
                    return false;
                }
            }
            ActionHandlers.Add(new Tuple<string, Action<IBilgeActionEvent>>(name, action));
            return true;
        }

        /// <summary>
        /// Removes an action handler ( V3.x onwards)
        /// </summary>
        /// <param name="action">The action to remove</param>
        /// <param name="name">The name of the action</param>
        public void RemoveActionHandler(Action<IBilgeActionEvent> action, string name) {
            name = name.ToLower();

            for (int i = ActionHandlers.Count - 1; i >= 0; i--) {
                var f = ActionHandlers[i];
                if (f.Item2.Equals(action)) {
                    ActionHandlers.Remove(new Tuple<string, Action<IBilgeActionEvent>>(name, action));
                }
            }
        }

        /// <summary>
        /// Clears down eveything for the router
        /// </summary>
        public void ClearEverything() {
            Shutdown();
            ActualClearEverything();
            lock (handlerLock) {
                handlers = null;
            }
            ReInitialise();
        }

        /// <summary>
        /// REbuilds the router internally
        /// </summary>
        public void ReInitialise() {
            ActionCallCountRecord = new Dictionary<string, int>();
            ActionHandlers = new List<Tuple<string, Action<IBilgeActionEvent>>>();
            ShutdownRequestActive = false;
            ActualReInitialise();
        }

        /// <summary>
        /// Shuts down the router
        /// </summary>
        public void Shutdown() {
            ShutdownRequestActive = true;
            ActualShutdown();
        }

        /// <summary>
        /// This is a single place to get the process identity as on some platforms its not supported, therefore its cached by
        /// this method.
        /// </summary>
        /// <returns>THe process ID or unknown if that is not available.</returns>
        internal static string GetProcessIdentity() {
            try {
                if (processIdCache == null) {
                    processIdCache = Process.GetCurrentProcess().Id.ToString();
                }
            } catch (PlatformNotSupportedException) {
                processIdCache = "unknown";
            }
            return processIdCache;
        }

        /// <summary>
        /// clears down the entire router
        /// </summary>
        internal static void PurgeRouter() {
            rtr.Purge();
            rtr = null;
        }

        /// <summary>
        /// Performs a shutdown on the router
        /// </summary>
        internal static void RouterShutdown() {
            // If you dont do this then the act of shutting down when its new can create the router
            // in the first place as its used for lazy int.
            if (rtr != null) {
                rtr.Shutdown();
            }
        }

        /// <summary>
        /// Adds a new message listener into the router.
        /// </summary>
        /// <param name="ibmh">The message listener to add.</param>
        internal void AddHandler(IBilgeMessageListener ibmh) {
            if (ibmh == null) { return; }
#if ACTIVEDEBUG
            Emergency.Diags?.Log($"AddingHandler {ibmh.Name}");
#endif
            ActualAddHandler(ibmh);
        }

        /// <summary>
        /// Flushes all of the messages
        /// </summary>
        internal void FlushMessages() {
            try {
                WriteAllMessages();
                ActualFlushMessages();
            } catch (Exception ex) {
                flushHiddenException = ex;
            }
        }

        /// <summary>
        /// Returns a enumerable of all of the handlers
        /// </summary>
        /// <returns>All of the handlers</returns>
        /// <exception cref="InvalidOperationException">Thrown when a null handler has been added in debug mode</exception>
        internal IEnumerable<IBilgeMessageListener> GetHandlers() {
            if (handlers == null) { yield break; }

            IBilgeMessageListener[] h = null;

            lock (handlerLock) {
                if (handlers != null) {
                    h = handlers;
                }
            }
            if (h != null) {
                foreach (var f in h) {
#if DEBUG
                    if (f == null) { throw new InvalidOperationException("This is incorrect code behaviour, the array should not contain nulls"); }
#endif
                    yield return f;
                }
            }
        }

        /// <summary>
        /// Returns all the handlers statuses.
        /// </summary>
        /// <returns>A string of handler statuses</returns>
        internal string GetHandlerStatuses() {
            var sb = new StringBuilder();

            sb = ActualGetHandlerStatuses(sb);

            var hndTemp = handlers;

            if (hndTemp != null) {
                foreach (var h in hndTemp) {
                    sb.AppendLine($"Handler {h.Name}");
                    sb.AppendLine(h.GetStatus());
                    sb.AppendLine(string.Empty);
                }
            }

            if (flushHiddenException != null) {
                sb.AppendLine($"__ router __");
                sb.AppendLine($"  FlushExceptionMasked: " + flushHiddenException.Message);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines wheter the current configuration is clean
        /// </summary>
        /// <returns>True if currently clean</returns>
        internal bool IsClean() {
            return ActualIsClean();
        }

        /// <summary>
        /// Prepares the message metadata for processing.
        /// </summary>
        /// <param name="mmd">Metadata to prepare.</param>
        /// <param name="contextKeys">A dicutionary of additonal context key value pairs.</param>
        internal void PrepareMetaData(MessageMetadata mmd, Dictionary<string, string> contextKeys) {
            mmd.NetThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            mmd.OsThreadId = GetCurrentOperatingSystemThreadId() ?? mmd.NetThreadId;
            mmd.Context = contextKeys[Bilge.BILGE_INSTANCE_CONTEXT_STR];
        }

        /// <summary>
        /// Queues a message for processing
        /// </summary>
        /// <param name="mm">the message to process</param>
        internal void QueueMessage(MessageMetadata mm) {
            if (messageQueueMaximum > 0) {
                // Remove oldest messages until we come down below the limit again.
                while (incommingMessageQueue.Count >= messageQueueMaximum) {
                    while (!incommingMessageQueue.TryDequeue(out var _)) { }
                }
            }

            incommingMessageQueue.Enqueue(mm);

            if ((MessageBatchCapacity > 0) || (MessageBatchDelay > 0)) {
                if ((elapsedTimer?.ElapsedMilliseconds <= MessageBatchDelay) && (incommingMessageQueue.Count < MessageBatchCapacity)) {
                    return;
                }
            }

            WriteAllMessages();
        }

        /// <summary>
        /// Adds a handler to the router.
        /// </summary>
        /// <param name="ibmh">The new handler to add</param>
        protected virtual void ActualAddHandler(IBilgeMessageListener ibmh) {
            IBilgeMessageListener[] replacement;
            HaveHandlers = true;

            if (handlers != null) {
                replacement = new IBilgeMessageListener[handlers.Length + 1]; // lastUsedHandler + 2];

                lock (handlerLock) {
                    for (int i = 0; i < handlers.Length; i++) {
                        replacement[i] = handlers[i];
                    }
                    replacement[handlers.Length] = ibmh;
                }
            } else {
                replacement = new IBilgeMessageListener[1] { ibmh };
            }

#if DEBUG
            foreach (var h in replacement) {
                if (h == null) {
                    throw new InvalidOperationException("This is a fault, there should be no null entries in the handler array");
                }
            }
#endif
            handlers = replacement;
        }

        /// <summary>
        /// Returns the underlying operating system thread id as a string, or null if that functon is not avaialable on the current platform, if an exception occurs trying to
        /// retrieve the id then the function will return null from then on.
        /// </summary>
        /// <returns>A string value of the OS thread ID or null if it can not be returned.</returns>
        protected string GetCurrentOperatingSystemThreadId() {
            if (ShouldGetCurrentThreadId) {
                try {
                    uint threadId = 0;
#if NETCOREAPP
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                        ShouldGetCurrentThreadId = false;
#else
                    threadId = GetCurrentThreadId();
#endif
#if NETCOREAPP
                }
#endif

                    return threadId.ToString();
                } catch (DllNotFoundException) {
                    ShouldGetCurrentThreadId = false;
                }
            }
            return null;
        }

        /// <summary>
        /// Calls the actual message handler with the message
        /// </summary>
        /// <param name="mmd">An array of messages to add.</param>
        protected abstract void ActualAddMessage(MessageMetadata[] mmd);

        /// <summary>
        /// Forces a flush of all messages.
        /// </summary>
        protected abstract void ActualFlushMessages();

        /// <summary>
        /// Gets the status from all the handlers
        /// </summary>
        /// <param name="sb">A string builder to add the statuses into.</param>
        /// <returns>A message from the handlers indiciating their current status.</returns>
        protected abstract StringBuilder ActualGetHandlerStatuses(StringBuilder sb);

        /// <summary>
        /// Determines if currently clean
        /// </summary>
        /// <returns>True if clean</returns>
        protected abstract bool ActualIsClean();

        /// <summary>
        /// Replace all of the contents
        /// </summary>
        /// <param name="mmd">Message Meta Data</param>
        /// <param name="msg">THe Message</param>
        /// <returns>String with replacements made</returns>
        protected string PerformSupportedReplacements(MessageMetadata mmd, string msg) {
            if (msg.IndexOf('%') >= 0) {
                msg = msg.Replace("%TS%", string.Format("{0,0:D2}:{1,0:D2}:{2,0:D2}.{3,0:D3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond));
                msg = msg.Replace("%MN%", mmd.MethodName);
            }
            return msg;
        }

        /// <summary>
        /// Prepares the message adding in process and machine information
        /// </summary>
        /// <param name="msg">The message</param>
        /// <exception cref="InvalidOperationException">thrown when the messages queue is empty</exception>
        protected void PrepareMessage(MessageMetadata[] msg) {
#if DEBUG
            if (msg.Length == 0) {
                throw new InvalidOperationException("Should not be calling this with no data, that makes no sense");
            }
#endif
            for (int i = 0; i < msg.Length; i++) {
                if (!string.IsNullOrEmpty(msg[i].Body)) {
                    msg[i].Body = PerformSupportedReplacements(msg[i], msg[i].Body);
                }
                if (!string.IsNullOrEmpty(msg[i].FurtherDetails)) {
                    msg[i].FurtherDetails = PerformSupportedReplacements(msg[i], msg[i].FurtherDetails);
                }
                msg[i].MachineName = MachineNameCache;
                msg[i].ProcessId = ProcessIdCache;
            }
        }

        /// <summary>
        /// Destroys the router
        /// </summary>
        protected virtual void Purge() {
        }

        private void WriteAllMessages() {
            if (incommingMessageQueue.Count == 0) {
                return;
            }

            var messages = new List<MessageMetadata>();
            int loopProtect = 0;
            while (incommingMessageQueue.Count > 0) {
                if (ShutdownRequestActive) { return; }

                loopProtect++;
                if (incommingMessageQueue.TryDequeue(out var mpx)) {
                    messages.Add(mpx);
                }
                if (loopProtect > MAX_LOOP_BEFORE_WRITE) {
                    // Theoretical edge case where someone keeps writing faster than we can keep reading and we never escape
                    // the loop trying to close down incomming messages.
                    break;
                }
            }
            ActualAddMessage(messages.ToArray());
        }
    }
}