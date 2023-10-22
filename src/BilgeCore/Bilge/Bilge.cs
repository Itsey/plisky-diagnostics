namespace Plisky.Diagnostics {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Plisky.Diagnostics.Listeners;

    /// <summary>
    /// Provides trace support for .net.
    /// </summary>
    public partial class Bilge {

        /// <summary>
        /// Default context for Bilge.
        /// </summary>
        public const string BILGE_INSTANCE_CONTEXT_STR = "bc-ctxt";

        /// <summary>
        /// Default session context for Bilge
        /// </summary>
        public const string BILGE_SESSION_CONTEXT_STR = "bc-sess-ctxt";

        private const string WILDCARD_MATCH_INITSTRING = "*";
        private static Bilge defaultInstance = null;
        private static Func<string, SourceLevels, SourceLevels> levelResolver = DefaultLevelResolver;
        private readonly ConfigSettings activeConfig;
        private BilgeAction actualAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bilge"/> class which provides diagnostics and trace to engineers.
        /// </summary>
        /// <param name="selectedInstanceContext">The context for this particular instance of bilge, usually used to identify a subsystem</param>
        /// <param name="sessionContext">The context for a session, usually used to identify the user request</param>
        /// <param name="tl">The trace level to set this instance of bilge to</param>
        /// <param name="resetDefaults">Reset all pf the internal context of Bilge</param>
        public Bilge(string selectedInstanceContext = "-", string sessionContext = "-", SourceLevels tl = SourceLevels.Off, bool resetDefaults = false) {
            activeConfig = new ConfigSettings();
            InstanceContext = selectedInstanceContext;
            SessionContext = sessionContext;

            string procId = BilgeRouter.GetProcessIdentity();

            if (resetDefaults) {
                BilgeRouter.Router.ClearEverything();
                // BilgeRouter.PurgeRouter();
            }

            Info = new InfoWriter(BilgeRouter.Router, activeConfig, SourceLevels.Information | SourceLevels.Error | SourceLevels.Critical);
            Verbose = new VerboseWriter(BilgeRouter.Router, activeConfig, SourceLevels.All);
            Warning = new WarningWriter(BilgeRouter.Router, activeConfig, SourceLevels.Warning | SourceLevels.Error | SourceLevels.Critical);
            Error = new ErrorWriter(BilgeRouter.Router, activeConfig, SourceLevels.Error | SourceLevels.Critical);
            Critical = new ErrorWriter(BilgeRouter.Router, activeConfig, SourceLevels.Critical);
            Direct = new BilgeDirect(BilgeRouter.Router, activeConfig);
            Util = new BilgeUtil(BilgeRouter.Router, activeConfig);
            Assert = new BilgeAssert(BilgeRouter.Router, activeConfig, Error);

            var level = levelResolver(selectedInstanceContext, tl);
            SetTraceLevel(level);
        }

        /// <summary>
        /// Provides alerting, specific methods for alerting, writes to the stream irrespective of the trace level.  Most slowdown elements are disabled
        /// and specific method types are provided for alerting, such as applicaiton online, offline etc.
        /// </summary>
        public static BilgeAlert Alert { get; } = new BilgeAlert();

        /// <summary>
        /// Allows quick access to a static default instance of bilge. To change trace ensure that "default" context is enabled.
        /// </summary>
        public static Bilge Default {
            get {
                if (defaultInstance == null) {
                    defaultInstance = new Bilge("default");
                }
                return defaultInstance;
            }
        }

        /// <summary>
        /// Provides action support
        /// </summary>
        public BilgeAction Action {
            get {
                return actualAction ?? (actualAction = new BilgeAction(activeConfig));
            }
        }

        /// <summary>
        /// Establishes the active trace level, using a SourceLevel.  Passing a source leve to this sets the trace level for this instance of Bilge.
        /// </summary>
        public SourceLevels ActiveTraceLevel {
            get { return activeConfig.ActiveTraceLevel; }
            set { SetTraceLevel(value); }
        }

        /// <summary>
        /// Provides assertion capabilities, runtime checks that should only really be performed during development builds or to validate that something which
        /// should be handled elsewhere in code has really been handled.
        /// </summary>
        public BilgeAssert Assert { get; private set; }

        /// <summary>
        /// Fatal log events, the program is in a bad state and about to terminate or should be terminated.  The critical logging levels are designed to give
        /// an overview of why a program failed.
        /// </summary>
        public BilgeWriter Critical { get; private set; }

        /// <summary>
        /// Provides direct writing to the output debug stream using your own values for methods.  This always writes, irrespective of the trace level
        /// but it is sitll subject to settings such as write on fail and queueing.
        /// </summary>
        public BilgeDirect Direct { get; private set; }

        /// <summary>
        /// Errors that are recoverable from, indicates non fatal problems in the code execution paths.
        /// </summary>
        public ErrorWriter Error { get; private set; }

        /// <summary>
        /// Informational logging, designed for program flow and basic debugging.  Provides a good detailed level of logging without going into
        /// immense details.
        /// </summary>
        public BilgeWriter Info { get; private set; }

        /// <summary>
        /// Used for instance level context information, this is a name that will be applied to the configurationresolver
        /// instance to determine what trace level should be used for this instance of bilge.  This is frequently set to
        /// the subsystem of the code that is being traced.
        /// </summary>
        public string InstanceContext {
            get {
                return activeConfig.InstanceContext;
            }

            set {
                activeConfig.InstanceContext = value;
            }
        }

        /// <summary>
        /// Gets or Sets the SessionContext which is used to filter at a routing level based on the session that is specified, if this
        /// value is set then the routing filter can be used to ensure that only specific sessions are traced or not traced.  The
        /// session context is stored in lower case.
        /// </summary>
        public string SessionContext {
            get {
                return activeConfig.SessionContext;
            }

            set {
                if (value == null) {
                    value = "-";
                }
                activeConfig.SessionContext = value.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Provies access to Bilge Utility functions and debugging and diagnostic helper methods.
        /// </summary>
        public BilgeUtil Util { get; private set; }

        /// <summary>
        /// The fullest level of most detailed logging, includes additional data and secondary messages to really help get detailed information on
        /// the execution of the code. This is the most detailed and therefore slowest level of logging.
        /// </summary>
        public BilgeWriter Verbose { get; private set; }

        /// <summary>
        /// Allows warning level logging, used for concerning elements of the code that do not necesarily result in errors.
        /// </summary>
        public BilgeWriter Warning { get; private set; }

        /// <summary>
        /// IF this is true then trace will only be written to the stream when a failure event ( error or dump ) has occured
        /// </summary>
        public bool WriteOnFail {
            get {
                return BilgeRouter.Router.WriteToHandlerOnlyOnFail;
            }

            set {
                if (value != BilgeRouter.Router.WriteToHandlerOnlyOnFail) {
                    BilgeRouter.Router.WriteToHandlerOnlyOnFail = value;
                }
            }
        }

        /// <summary>
        /// Adds a Handler based on the handler options - handler options can either check for matching types, or matching names, and can
        /// refuse to add if a matching type or name is added.  Default is to allow as many duplicate handlers as you wish.
        /// </summary>
        /// <param name="ibmh">The handler to add</param>
        /// <param name="hao">The approach to take with duplicates</param>
        /// <returns>True if the handler was added</returns>
        public static bool AddHandler(IBilgeMessageListener ibmh, HandlerAddOptions hao = HandlerAddOptions.Duplicates) {
            switch (hao) {
                case HandlerAddOptions.SingleType:
                    foreach (var n in GetHandlers()) {
                        if (n.GetType() == ibmh.GetType()) {
                            return false;
                        }
                    }
                    break;

                case HandlerAddOptions.SingleName:
                    string mn = ibmh.Name.ToLower();
                    foreach (var n in GetHandlers()) {
                        if (n.Name.ToLower() == mn) {
                            return false;
                        }
                    }
                    break;

                default:
                    break;
            }

            BilgeRouter.Router.AddHandler(ibmh);
            return true;
        }

      

        /// <summary>
        /// Do Not Use
        /// </summary>
        /// <param name="ibmh">The message handler, note this is obsolete</param>
        [Obsolete("Replaced by AddHandler,takes Options.  Migrate your code to AddHandler(hnd, HandlerOptions.Duplicates) for compat.")]
        public static void AddMessageHandler(IBilgeMessageListener ibmh) {
            BilgeRouter.Router.AddHandler(ibmh);
        }

        /// <summary>
        /// Removes the static configuration resolver to ensure that resolution returns to the default.  Note any instances that have been
        /// configured by the resolver will remain configured.  This clear will only affect new instances.
        /// </summary>
        public static void ClearConfigurationResolver() {
            levelResolver = DefaultLevelResolver;
        }

        /// <summary>
        /// Flush all trace stream handlers and then clear them down so no handlers remain
        /// </summary>
        public static void ClearMessageHandlers() {
            Task.Run(async () => { await BilgeRouter.Router.FlushMessages(); }).Wait();
            BilgeRouter.Router.ClearEverything();
        }

        /// <summary>
        /// Shut everything down.
        /// </summary>
        public static void CollapseRouter() {
            BilgeRouter.RouterShutdown();
        }

        /// <summary>
        /// Converts a TraceLevel into a SourceLevels, to allow you to continue to use old code that supports trace levels and work with the change to source
        /// levels within Bilge, used by the legacy support for CurrentTraceLevel.
        /// </summary>
        /// <param name="value">The TraceLevel to use</param>
        /// <returns>Opinionated conversion to SourceLevel.</returns>
        public static SourceLevels ConvertTraceLevel(TraceLevel value) {
            var result = SourceLevels.Off;

            switch (value) {
                case TraceLevel.Off: result = SourceLevels.Off; break;
                case TraceLevel.Error: result = SourceLevels.Error | SourceLevels.Critical; break;
                case TraceLevel.Warning: result = SourceLevels.Error | SourceLevels.Critical | SourceLevels.Warning; break;
                case TraceLevel.Info: result = SourceLevels.Error | SourceLevels.Critical | SourceLevels.Warning | SourceLevels.Information; break;
                case TraceLevel.Verbose: result = SourceLevels.Error | SourceLevels.Critical | SourceLevels.Warning | SourceLevels.Information | SourceLevels.Verbose | SourceLevels.Information; break;
            }

            return result;
        }

        /// <summary>
        /// Clear all trace stream caches.
        /// </summary>
        public static async Task ForceFlush() {
            await BilgeRouter.Router.FlushMessages();
        }

        /// <summary>
        /// Gets all of the message handlers with some very basic filtering capability, this is not usually required by most implementations but can
        /// be useful when allowing for things like dynamic configuration of message handlers.  The filter string can either start or end with an  *
        /// to indicate that the name should be an exact match (no *) or end with (*text) or start with (text*) a matching text filter.
        /// </summary>
        /// <param name="filter">The filter to use to filter out handlers</param>
        /// <returns>An enumeration of the active handlers</returns>
        public static IEnumerable<IBilgeMessageListener> GetHandlers(string filter = "*") {
            string nameMatchStart = null;
            string nameMatchEnd = null;

            if (filter.StartsWith("*")) {
                nameMatchEnd = filter.Substring(1);
            } else if (filter.EndsWith("*")) {
                nameMatchStart = filter.Substring(0, filter.Length - 1);
            }

            foreach (var nextHandler in BilgeRouter.Router.GetHandlers()) {
                if (!string.IsNullOrEmpty(nextHandler.Name)) {
                    if (!string.IsNullOrEmpty(nameMatchStart)) {
                        if (nextHandler.Name.StartsWith(nameMatchStart)) {
                            yield return nextHandler;
                        }
                    } else if (!string.IsNullOrEmpty(nameMatchEnd)) {
                        if (nextHandler.Name.EndsWith(nameMatchEnd)) {
                            yield return nextHandler;
                        }
                    } else {
                        yield return nextHandler;
                    }
                } else {
                    yield return nextHandler;
                }
            }
        }

        /// <summary>
        /// Uses an internal string formatted configuration resolver. See documentation for usage.
        /// </summary>
        /// <param name="crInitialisationString">The initialisation string</param>
        /// <returns>The func used to resolve the configuration</returns>
        public static Func<string, SourceLevels, SourceLevels> SetConfigurationResolver(string crInitialisationString) {
            var newCR = GetConfigurationResolverFromString(crInitialisationString, null);
            levelResolver = newCR;
            return newCR;
        }

        /// <summary>
        /// Uses an internal string formatted configuration resolver. See documentation for usage.  Additionally configures one or more handlers, returning the handler
        /// configuration to the caller.  If autoResolve is set also uses reflection to try and initialise the handlers.
        /// </summary>
        /// <param name="crInitialisationString">The initialisation string</param>
        /// <param name="handlerRequests">The parts of the initialisation string that relate to handlers</param>
        /// <param name="autoResolve">Whether the method should try and resolve each of the handler requests</param>
        /// <returns>The func used to resolve the configuration</returns>
        public static Func<string, SourceLevels, SourceLevels> SetConfigurationResolver(string crInitialisationString, string[] handlerRequests, bool autoResolve = true) {
            var hrs = new List<string>();
            var newCR = GetConfigurationResolverFromString(crInitialisationString, hrs);
            levelResolver = newCR;

            if (autoResolve && hrs.Count > 0) {
                var allKnownTypes = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof(BaseHandler)));
            }

            return newCR;
        }

        /// <summary>
        /// Sets up a configuraiton resolver that is called for every new instance of Bilge.  This will be called with the instance name
        /// and the current trace level of the instance.  The return is your new desireds trace level.  This can be used to turn on logging
        /// based on configuration or any other external factor with minimal impact on your code base.
        /// </summary>
        /// <param name="configurationResolver">The func to use to resolve configuration</param>
        public static void SetConfigurationResolver(Func<string, SourceLevels, SourceLevels> configurationResolver) {
            levelResolver = configurationResolver;
        }

        /// <summary>
        /// Determines whether router errors should be suppressed
        /// </summary>
        /// <param name="v">True to suppress</param>
        public static void SetErrorSuppression(bool v) {
            BilgeRouter.Router.SuppressHandlerErrors = v;
        }

        /// <summary>
        /// The default is that the queued router is used which uses a secondary thread to minimise the wait time for the writing
        /// part of the code.  However not all environments nor solutions are suited to a whole thread for this therefore youc an
        /// simplify the routing and not use a secondary thread.  It is not possible to "unset" this once set in a program.
        /// </summary>
        /// <param name="br">An alternative router to use</param>
        public static void SimplifyRouter(BilgeRouter br = null) {
            BilgeRouter.SetBasicRouting(br);
        }

        /// <summary>
        /// Adds a handler that can process the trace messages
        /// </summary>
        /// <param name="ibmh">The new handler</param>
        public void AddHandler(IBilgeMessageListener ibmh) {
            BilgeRouter.Router.AddHandler(ibmh);
        }

        /// <summary>
        /// Alters the optional configuration for Bilge, to allow more detail to be added to the trace stream if that is useful, beware many of the
        /// options to add further detail also reduce performance.
        /// </summary>
        /// <param name="newConfiguration">the new configuration settings to apply.</param>
        public void ConfigureTrace(TraceConfiguration newConfiguration) {
            activeConfig.TraceConfig = newConfiguration;
        }

        /// <summary>
        /// Turns off message batching therefore all messages are sent immediately/
        /// </summary>
        public void DisableMessageBatching() {
            BilgeRouter.Router.MessageBatchDelay = 0;
            BilgeRouter.Router.MessageBatchCapacity = 0;
        }

        /// <summary>
        /// Clear the trace stream cache down and ensure that all messages are written to the underlying listeners.
        /// </summary>
        public async Task Flush() {

            await Bilge.ForceFlush();
        }

        /// <summary>
        /// This method shuts down bilge but it is the only way to be sure that all of your messages have left the
        /// internal queuing system.  Once this method has run Bilge is completely broken and no more trace can be
        /// written, therefore it should only be used when a process is exiting and you still want to keep all off
        /// the trace messages - for example during a console program.
        /// </summary>
        /// <remarks>It is possible to reinitialise with reinit=true, but this is not recommended outside of unit test scenarios.</remarks>
        /// <param name="reinit">The new handler</param>
        public void FlushAndShutdown(bool reinit = false) {
            BilgeRouter.Router.Shutdown();
            if (reinit) {
                BilgeRouter.Router.ReInitialise();
            }
        }

        /// <summary>
        /// Returns an enumeration of all of the currently loaded name value pairs added as contexts.
        /// </summary>
        /// <returns>Enumerable name value pairs as a tuple with each of the current contexts.</returns>
        public IEnumerable<Tuple<string, string>> GetContexts() {
            foreach (string l in activeConfig.MetaContexts.Keys) {
                yield return new Tuple<string, string>(l, activeConfig.MetaContexts[l]);
            }
        }

        /// <summary>
        /// Adds name value pairing context to the current instance of Bilge.
        /// </summary>
        /// <param name="contextName">A Name for the contextual information.</param>
        /// <param name="contextValue">A Value for the contextual information</param>
        /// <exception cref="ArgumentNullException">Thrown if the name is null.</exception>
        public void AddContext(string contextName, string contextValue) {
            if (string.IsNullOrWhiteSpace(contextName)) { throw new ArgumentNullException(nameof(contextName)); }            

            if (!activeConfig.MetaContexts.ContainsKey(contextName)) {
                activeConfig.MetaContexts.Add(contextName, contextValue);
            } else {
                activeConfig.MetaContexts[contextName] = contextValue;
            }

        }

        /// <summary>
        /// This method returns a string describing the current internal logging status of Bilge.  If there is no output going to your chosen
        /// listener then this method can help track down what is wrong.
        /// </summary>
        /// <returns>A status string indicating the current Bilge status</returns>
        public string GetDiagnosticStatus() {
            var result = new StringBuilder();
            result.AppendLine("__ Logging __");
            result.AppendLine($"Trace Level {ActiveTraceLevel}");
            result.AppendLine($" {nameof(Info)} Writing: {Info.IsWriting}");
            result.AppendLine($" {nameof(Verbose)} Writing: {Verbose.IsWriting}");
            result.AppendLine($" {nameof(Warning)} Writing: {Warning.IsWriting}");
            result.AppendLine($" {nameof(Error)} Writing: {Error.IsWriting}");
            result.AppendLine("__ Router __");
            result.AppendLine($" Router Total Handler Errors: {BilgeRouter.Router.ErrorCount}");
            result.AppendLine($" Router Suppress Errors: {BilgeRouter.Router.SuppressHandlerErrors}");
            result.AppendLine("__ Handlers __");
            result.Append(BilgeRouter.Router.GetHandlerStatuses());
            return result.ToString();
        }

        /// <summary>
        /// Returns the actual handlers but at the level of IBilgeMessageListener.  Usually more useful to capture the instance
        /// as you add it to bilge rather than return through this method. Method primarily used for unit testing.
        /// </summary>
        /// <returns>Enumeration of the registered Listeners</returns>
        public IEnumerable<IBilgeMessageListener> GetListeners() {
            return BilgeRouter.Router.GetHandlers();
        }

        /// <summary>
        /// Determines the level of message batching to use, message batching groups messages together before sending on to the listener stream.
        /// </summary>
        /// <param name="numberMessagesInABatch">Limit on number of messages (default 100)</param>
        /// <param name="millisecondsToWaitForBatch">Limit on delay of messages (default 100ms)</param>
        public void SetMessageBatching(int numberMessagesInABatch = 100, int millisecondsToWaitForBatch = 100) {
            BilgeRouter.Router.MessageBatchCapacity = numberMessagesInABatch;
            BilgeRouter.Router.MessageBatchDelay = millisecondsToWaitForBatch;
        }

        /// <summary>
        /// Adds a custom session filter where you can determine which messages are filtered out based on the passed meta contexts
        /// </summary>
        /// <param name="filter">A function to filter messages based on context</param>
        public void SetSessionFilter(Func<Dictionary<string, string>, bool> filter) {
            activeConfig.SessionFilter = filter;
        }

        /// <summary>
        /// Adds a session filter which will throw away messages that do not match the the text in the filter.
        /// </summary>
        /// <param name="filter">The string that must be present in the session context</param>
        public void SetSessionFilter(string filter) {
            filter = filter.ToLowerInvariant();

            SetSessionFilter((a) => {
                return a[Bilge.BILGE_SESSION_CONTEXT_STR].Contains(filter);
            });
        }

        /// <summary>
        /// Force WriteOnlyOnFailure to be True.
        /// </summary>
        public void TriggerWrite() {
            BilgeRouter.Router.FailureOccuredForWrite = true;
        }

        private static SourceLevels DefaultLevelResolver(string source, SourceLevels beforeModification) {
            return beforeModification;
        }

        private static Func<string, SourceLevels, SourceLevels> GetConfigurationResolverFromString(string initString2, List<string> handlerRequests) {
            if (handlerRequests == null) { handlerRequests = new List<string>(); }

            var matches = new List<Func<string, SourceLevels>>();
            initString2 = initString2?.ToLower();

            if (string.IsNullOrWhiteSpace(initString2) || (initString2 == "off") || (initString2 == "none")) {
                Func<string, SourceLevels, SourceLevels> result = (a, b) => {
                    return SourceLevels.Off;
                };
                return result;
            }

            switch (initString2[1]) {
                case '-': break;
                case '+': break;
                default: throw new InvalidOperationException($"The format string is in the wrong format, got {initString2[1]} needed [+,-]");
            }


            string[] stringsToParse = initString2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string initString in stringsToParse) {

                if (initString2[0] == 'h' || initString2[0] == 'H') {
                    // Handler Configuration request found, add it to the returned handler requests.
                    handlerRequests.Add(initString);
                    continue;
                }

                var sl = GetSourceLevelFromChar(initString[0]);
                string initStringMatch = initString.Substring(2);

                if (initStringMatch == $"{WILDCARD_MATCH_INITSTRING}{WILDCARD_MATCH_INITSTRING}") {
                    // Special case - ** means that everything is matched.
                    matches.Add((instanceNametoCheck) => {
                        return sl;
                    });
                    // This is a catch all therefore any entries after this one will not be parsed
                    // as this one will always match.
                    break;
                }

                if (initStringMatch.EndsWith("*")) {
                    // Starts with style matching
                    initStringMatch = initStringMatch.Substring(0, initStringMatch.Length - 1);
                    matches.Add((instanceNametoCheck) => {
                        if (instanceNametoCheck.StartsWith(initStringMatch)) {
                            return sl;
                        }

                        return SourceLevels.Off;
                    });
                } else if (initStringMatch.StartsWith(WILDCARD_MATCH_INITSTRING)) {
                    initStringMatch = initStringMatch.Substring(1);
                    matches.Add((instanceNametoCheck) => {
                        if (instanceNametoCheck.EndsWith(initStringMatch)) {
                            return sl;
                        }

                        return SourceLevels.Off;
                    });
                } else {
                    // Exact match style matching
                    matches.Add((instanceNametoCheck) => {
                        if (instanceNametoCheck == initStringMatch) {
                            return sl;
                        }

                        return SourceLevels.Off;
                    });
                }
            }

            Func<string, SourceLevels, SourceLevels> resultx = (bilgeInstanceName, originalTraceLevel) => {
                foreach (var l in matches) {
                    var res = l(bilgeInstanceName);
                    if (res != SourceLevels.Off) {
                        return res;
                    }
                }

                return SourceLevels.Off;
            };
            return resultx;
        }

        private static SourceLevels GetSourceLevelFromChar(char c) {
            switch (c) {
                case 'e': return SourceLevels.Error;
                case 'w': return SourceLevels.Warning;
                case 'o': return SourceLevels.Off;
                case 'v': return SourceLevels.Verbose;
                default:
                    throw new NotImplementedException($"The source level set in the configuration string is not a valid soure level.  Recieved {c}, should be [e,w,o,v].");
            }
        }

        private void SetTraceLevel(SourceLevels value) {
            if (activeConfig.ActiveTraceLevel == value) { return; }
            if (!Enum.IsDefined(typeof(SourceLevels), value)) {
                throw new ArgumentException($"The value passed to {nameof(SetTraceLevel)} property must be one of the TraceLevel enum values", nameof(value));
            }

            activeConfig.ActiveTraceLevel = value;
            Error.IsWriting = (activeConfig.ActiveTraceLevel & SourceLevels.Error) == SourceLevels.Error;
            Warning.IsWriting = (activeConfig.ActiveTraceLevel & SourceLevels.Warning) == SourceLevels.Warning;
            Info.IsWriting = (activeConfig.ActiveTraceLevel & SourceLevels.Information) == SourceLevels.Information;
            Verbose.IsWriting = (activeConfig.ActiveTraceLevel & SourceLevels.Verbose) == SourceLevels.Verbose;
        }

        private void SetTraceLevel(TraceLevel value) {
            SetTraceLevel(Bilge.ConvertTraceLevel(value));
        }
    }
}