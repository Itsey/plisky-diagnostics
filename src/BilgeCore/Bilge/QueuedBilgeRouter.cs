namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// BilgeRouter acts as a central point taking all of the inputs from the Bilge/BilgeWriter combinations and routing them through
    /// a static queue of messages that is handled by a dedicated thread.  Bilge2 no longer allows for non queued messages and therefore
    /// all messages will be queued here priort to being passed to the handlers.
    /// </summary>
    internal class QueuedBilgeRouter : BilgeRouter {
#if DEBUG
        private static int threadsActive = 0;
        private const int THREADWAITWHENNOEVENTS = 5000;  // 5 seconds temporarily to allow for longer debugging time
#else
        private const int THREADWAITWHENNOEVENTS = 500;    // Normally half a second ok
#endif

        protected override void Purge() {
            base.Purge();

            if (!shutdownRequested) { shutdownRequested = true; }
            s_dispatcherThread = null;

#if DEBUG
            if (threadsActive == 1) { threadsActive = 0; }
#endif
        }


        protected override bool ActualIsClean() {
            if ((messageQueue != null) && (messageQueue.Count > 0)) {
#if ACTIVEDEBUG
                Emergency.Diags.Log($"CleanCheck {messageQueue.Count} messages.");
#endif
                return false;
            }
            if ((handlers != null) && (handlers.Length > 0)) {
#if ACTIVEDEBUG
                Emergency.Diags.Log($"CleanCheck {handlers.Length} handlers.");
#endif

                return false;
            }
            return true;
        }

        
        private Thread s_dispatcherThread;
        private ConcurrentQueue<MessageMetadata> messageQueue;

        private AutoResetEvent queuedMessageResetEvent;
        private volatile int internalMessageMaxQueueLength = -1;  // Specific to this router because it has queues too
        private volatile bool shutdownEnabled;

        private void EnableQueuedMessages() {
            if ((s_dispatcherThread != null) && (queuedMessageResetEvent != null)) {
                // Thread is running.
                if (shutdownEnabled) {
                    while (s_dispatcherThread != null) {
                        queuedMessageResetEvent.Set();
                        //not sure why this was here Thread.Sleep(0);
                    }
                } else {
                    // Its running and not shutting down just leave it.
                    return;
                }
            }

            shutdownEnabled = false;

            queuedMessageResetEvent = new AutoResetEvent(false);
            messageQueue = new ConcurrentQueue<MessageMetadata>();
            elapsedTimer = new Stopwatch();
            elapsedTimer.Start();

#if DEBUG
            if (threadsActive == 1) {
                throw new InvalidOperationException("Attempting to bring online a second background thread");
            }
#endif
            s_dispatcherThread = new Thread(new ThreadStart(DispatcherThreadMethod));
            s_dispatcherThread.Start();
        }

        public override void ActualClearEverything() {
            WriteToHandlerOnlyOnFail = false;
            FailureOccuredForWrite = false;
            messageQueue = new ConcurrentQueue<MessageMetadata>();
        }

        protected override void ActualAddMessage(MessageMetadata[] mm) {
            if (!shutdownRequested) {
                ActualQueueMessage(mm);
            }
        }


        internal void ActualQueueMessage(MessageMetadata[] mp) {
#if ACTIVEDEBUG
            Emergency.Diags?.Log($"Message queued " + mp.Body);
#endif

            if (shutdownRequested) {
                // Not really designed to allow you to "shutdown" and keep writing, but on multithreaded apps
                // thats probably impossible to avoid therefore just return in release builds.
#if DEBUG
                throw new NotSupportedException("ShutdownEnabled, this method should not be called when shutdown has begun");
#else
                return;
#endif
            }

            if (!HaveHandlers) {
                // No handlers, just throw the content away.
                return;
            }

            if (internalMessageMaxQueueLength > 0) {
                // Remove oldest messages until we come down below the limit again.
                while (messageQueue.Count >= internalMessageMaxQueueLength) {
                    while (!messageQueue.TryDequeue(out MessageMetadata mpx)) { };
                }
            }
            foreach (var x in mp) {
                messageQueue.Enqueue(x);
            }
            elapsedTimer?.Restart();
            queuedMessageResetEvent.Set();
        }

        private volatile bool shutdownRequested = false;

        public override void ActualShutdown() {
#if ACTIVEDEBUG
            Emergency.Diags?.Log($"Shutdown requested");
#endif

            if (queuedMessageResetEvent != null) {
                queuedMessageResetEvent.Set();
            }
            shutdownRequested = true;

            if (s_dispatcherThread != null) {
#if ACTIVEDEBUG
                Emergency.Diags?.Log($"Waiting on dispatcher thread");
#endif

                s_dispatcherThread.Join();
            }


            lock (handlerLock) {
                if (handlers != null) {
                    for (int i = 0; i < handlers.Length; i++) {
                        var next = handlers[i] as IDisposable;
#if ACTIVEDEBUG
                    Emergency.Diags?.Log($"Disposing handler {i} {handlers[i].Name}");
#endif

                        if (next != null) {
                            next.Dispose();
                        }
                    }
                }
            }

        }

        protected override StringBuilder ActualGetHandlerStatuses(StringBuilder sb) {


            sb.Append($"QueueDepth: {messageQueue.Count} Max: {internalMessageMaxQueueLength}\n");
            sb.Append("___________________________\n");


            return sb;
        }

        /// <summary>
        /// Called to write out the messages that are queued.
        /// </summary>
        private void TriggerQueueWrite() {
            queuedMessageResetEvent.Set();
        }

        public override void ActualReInitialise() {
            shutdownRequested = false;
            shutdownEnabled = false;
            EnableQueuedMessages();
        }

        private List<Task> activeTasks = new List<Task>();

        private void DispatcherThreadMethod() {
#if DEBUG
            // As this is static and uses a lot of static data it should never be shared with another one of these threads. We put this check
            // into the development builds to verify that this does not occur.
            Interlocked.Increment(ref threadsActive);
            if (threadsActive > 1) {
                throw new InvalidOperationException("It should not be possible to have more than one thread active using the queues");
            }
            try {
#endif
                Thread.CurrentThread.Name = "Bilge>>RouterQueueDispatcher";
                Thread.CurrentThread.IsBackground = true;

                while ((!shutdownRequested) && (!System.Environment.HasShutdownStarted)) {
                    ClearCompletedActiveTasks();

                    if (messageQueue.Count == 0) {
                        // If the thread has nothing to do it waits for the next message or 5 seconds in case theres a message that
                        // has come in since the zero was set or in case a shutdown request was made.
                        if (!queuedMessageResetEvent.WaitOne(THREADWAITWHENNOEVENTS, false)) {
                            // If we come out of our wait and there are still no events, or shutodwn is requested bomb out
                            if ((messageQueue.Count == 0) || (System.Environment.HasShutdownStarted)) {
                                continue;
                            }
                        }
                    }

                    // WriteOnFail support.
                    if ((WriteToHandlerOnlyOnFail) && (!FailureOccuredForWrite)) {
                        // Not quite the same this, there are events but we dont want to deal with them
                        Thread.Sleep(THREADWAITWHENNOEVENTS);
                        continue;
                    }

                    // WriteOnFail means only write when failure occured-  at this point we reset.
                    if (FailureOccuredForWrite) { FailureOccuredForWrite = false; }
                    lock (activeTasks) {
                        // Message Batching support - group messages up together to send either by number or milliseconds or both
                        if ((MessageBatchCapacity > 0) || (MessageBatchDelay > 0)) {
                            if ((elapsedTimer?.ElapsedMilliseconds <= MessageBatchDelay) && (messageQueue.Count < MessageBatchCapacity)) {
                                continue;
                            }
                        }

                        while ((messageQueue.Count > 0) && (!System.Environment.HasShutdownStarted)) {
                            RouteAllQueuedMessages();
                        }
                    }
                }

                // Destroy resources associated with the high perf implementation
                messageQueue = null;
                queuedMessageResetEvent = null;

#if DEBUG
            } finally {
                Interlocked.Decrement(ref threadsActive);
            }
#endif
            // Remove the reference as the last thing we do
            s_dispatcherThread = null;
        }

        private void RouteAllQueuedMessages() {
            // Take a copy of the existing messages to process, but put a new queue in to catch any new messages
            var tmq = new ConcurrentQueue<MessageMetadata>();
            var tmq2 = messageQueue;
            messageQueue = tmq;
            var copyOfCurrentMessages = tmq2.ToArray();
            if (tmq2.Count > 0) {
                activeTasks.Add(RouteMessage(copyOfCurrentMessages));
            }
        }

        private void ClearCompletedActiveTasks() {
            int i = 0; // Clear down any tasks that have completed.
            while (i < activeTasks.Count) {
                if (activeTasks[i].IsCompleted) {
                    activeTasks.RemoveAt(i);
                } else {
                    i++;
                }
            }
        }

        protected override void ActualFlushMessages() {
            int msgs = messageQueue.Count;
            if (msgs != 0) {
                RouteAllQueuedMessages();
                Emergency.Diags.Log($"Flush, messages {msgs}");

                Task.WaitAll(activeTasks.ToArray());

                // This takes the current count of messages into msgs, and will run through processing
                // messages - until either there are none left

                int looopProtect = 0;
                while (messageQueue.Count > 0) {
                    queuedMessageResetEvent.Set();
                    Thread.Sleep(1);
                    looopProtect++;
                    if (looopProtect > 100) {
                        Emergency.Diags.Log($"Level Two Flush Occurs, Waiting Longer");
                        Thread.Sleep(10);
                        if (looopProtect > 110) {
                            break;
                        }
                    }
                }
            }
            if (handlers != null) {
                foreach (var h in handlers) {
                    h.Flush();
                }
            }

            Task.WaitAll(activeTasks.ToArray());

            Emergency.Diags.Log($"Flush, done ");
        }

        private async Task RouteMessage(MessageMetadata[] messagesToRoute) {
            if ((handlers == null) || (handlers.Length == 0)) { return; }
            PrepareMessage(messagesToRoute);

            Emergency.Diags.Log($"InternalRouteMessage >> " + messagesToRoute.Length.ToString());

            var hndlr = handlers;
            Task[] tasks = new Task[hndlr.Length];
            try {
                for (int i = 0; i < hndlr.Length; i++) {
                    Emergency.Diags.Log($"Handler {i} routing message");
                    tasks[i] = hndlr[i].HandleMessageAsync(messagesToRoute);
                }
                await Task.WhenAll(tasks);
            } catch (Exception) {
                ErrorCount++;
                if (!SuppressHandlerErrors) {
                    throw;
                }
            }
        }


        

        internal QueuedBilgeRouter(string processId) : base(processId) {
            EnableQueuedMessages();
        }
    }
}