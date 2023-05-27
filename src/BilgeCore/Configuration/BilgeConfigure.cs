namespace Plisky.Diagnostics.Test {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// BilgeConfigure is used to apply Bilge configuration from initialisation strings so that the level of debugging and listeners
    /// can be quickly changed from a single init string.
    /// </summary>
    public class BilgeConfigure {
        private static FileSystemWatcher fsw;

        private static string fswFilename;

        private static string lastFileInitString;

        private Dictionary<string, MethodInfo> pendingListenerImplementors = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeConfigure"/> class.
        /// Creates a new instance of the bilge configuration class which is used to configure bilge from strings.
        /// </summary>
        public BilgeConfigure() {
            Configuration = new BilgeConfiguration();
        }

        /// <summary>
        /// The active configuration that will be applied to bilge once Apply is called.
        /// </summary>
        public BilgeConfiguration Configuration { get; set; }

        /// <summary>
        /// Fluent interface syntactic sugar, completely optional.
        /// </summary>
        public BilgeConfigure From {
            get { return this; }
        }

        /// <summary>
        /// Splits an input string into chunks allowing you to specify the splitters
        /// </summary>
        /// <param name="initString">STring to chunkify</param>
        /// <param name="openDelimiter">Chunk start delimiter</param>
        /// <param name="closeDelimiter">chunk close delimiter</param>
        /// <returns>String split into chunks</returns>
        public static string[] Chunkify(string initString, char openDelimiter = '[', char closeDelimiter = ']') {
            var result = new List<string>();
            int idx = initString.IndexOf(openDelimiter);
            while (idx >= 0) {
                initString = initString.Substring(idx + 1);
                int idxEnd = initString.IndexOf(closeDelimiter);
                if (idxEnd >= 0) {
                    string nextChunk = initString.Substring(0, idxEnd);
                    result.Add(nextChunk);
                    initString = initString.Substring(nextChunk.Length + 1);
                    idx = initString.IndexOf(openDelimiter);
                } else {
                    idx = -1;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Takes the current configuration and applies it to bilge, setting a configuration resolver and a series of
        /// handlers.  The configuration resolver will change the future instances of bilge that are created.
        /// </summary>
        public void Apply() {
            if (!string.IsNullOrEmpty(Configuration.ResolverInitString)) {
                Bilge.SetConfigurationResolver(Configuration.ResolverInitString);
            }

            foreach (string l in Configuration.HandlerStrings) {
                TryAddHandler(l);
            }
        }

        /// <summary>
        /// Apply configuration from an initialisation string that is stored in the system environment variables.
        /// </summary>
        /// <param name="environmentVariableName">The environment variable name to use to get the value from</param>
        /// <returns>The instance for fluent configuration</returns>
        public BilgeConfigure EnvironmentVariable(string environmentVariableName = "BILGEINIT") {
            string next = GetFromEnvironmentVariable(environmentVariableName);
            ApplyConfiguration(next);
            return this;
        }

        /// <summary>
        /// Configures a file system watcher for the given filename.  Each time that this file is changed an attempt will be made to
        /// reconfigure bilge using the infomration from the file.  This allows for dynamic enabling and disabling of Bilge.
        /// </summary>
        /// <param name="filenameToUse">The filename to read the configuration from</param>
        /// <returns>An instance of this class for fluent initialisation</returns>
        public BilgeConfigure FileSystemWatcher(string filenameToUse) {
            fswFilename = filenameToUse;
            fsw = new FileSystemWatcher();
            fsw.Changed += Fsw_Changed;
            return this;
        }

        /// <summary>
        /// Register a handler from its initialisation attribute
        /// </summary>
        /// <param name="type">The type of the handler to register</param>
        public void RegisterHandler(Type type) {
            if (type == null) { return; }

            foreach (var l in type.GetMethods()) {
                var ca = l.GetCustomAttribute<HandlerInitialisationAttribute>();
                if (ca != null) {
                    string prefix = ca.ListenerPrefix;
                    if (!ca.ListenerPrefix.EndsWith(":")) {
                        ca.ListenerPrefix += ":";
                    }

                    ca.ListenerPrefix = ca.ListenerPrefix.ToUpper();

                    pendingListenerImplementors.Add(ca.ListenerPrefix, l);
                }
            }
        }

        /// <summary>
        /// Apply configuration from an initialisation string directly.
        /// </summary>
        /// <param name="initStringToUse">The bilge initialisation string</param>
        /// <returns>The instance for fluent configuration</returns>
        public BilgeConfigure StringValue(string initStringToUse) {
            ApplyConfiguration(initStringToUse);
            return this;
        }

        /// <summary>
        /// Applys configuration by reading the first line of a text file and parsing that as an intialisation string.
        /// </summary>
        /// <param name="filenameToUse">The filename to read the settings from.  If it does not exist defaults wil be used.</param>
        /// <returns>Instance of this class for fluent initialisation</returns>
        public BilgeConfigure TextFile(string filenameToUse) {
            ReadAndApplyFromFile(filenameToUse);
            return this;
        }

        /// <summary>
        /// Applies a configuration item
        /// </summary>
        /// <param name="next">the configuration to apply</param>
        protected void ApplyConfiguration(string next) {
            if (string.IsNullOrWhiteSpace(next)) {
                return;
            }

            string[] chnks = GetChunks(next);
            foreach (string l in chnks) {
                string nextChunk = l;
                string nextUpper = l.Substring(0, 4).ToUpper();
                string nextVal = l.Substring(4);

                switch (nextUpper) {
                    case "TLV:":
                        nextChunk = nextChunk.Substring(4);
                        Configuration.OverallSourceLevel = (SourceLevels)Enum.Parse(typeof(SourceLevels), nextVal, true);
                        break;

                    case "LST:":
                        foreach (string nexthandlerString in Chunkify(nextVal, '(', ')')) {
                            Configuration.AddHandler(nexthandlerString);
                        }
                        break;

                    case "RSL:":
                        Configuration.ResolverInitString = nextVal;
                        break;
                }
            }
        }

        /// <summary>
        /// Splits an input string into chunks
        /// </summary>
        /// <param name="v">The string to chunk up</param>
        /// <returns>The chunked string</returns>
        protected string[] GetChunks(string v) {
            if (string.IsNullOrWhiteSpace(v)) {
                return null;
            }
            return Chunkify(v);
        }

        /// <summary>
        /// Gets an environment variable by name ( use to allow overrides to replace this )
        /// </summary>
        /// <param name="variableName">Environment variable name to get</param>
        /// <returns>the value from the environment variable</returns>
        protected virtual string GetFromEnvironmentVariable(string variableName) {
            return Environment.GetEnvironmentVariable(variableName);
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e) {
            ReadAndApplyFromFile(fswFilename);
        }

        private void ReadAndApplyFromFile(string fn) {
            var l = File.ReadAllText(fn);
            if (l != lastFileInitString) {
                lastFileInitString = l;
                ApplyConfiguration(lastFileInitString);
            }
        }

        private void TryAddHandler(string l) {
            if (string.IsNullOrEmpty(l)) {
                throw new InvalidOperationException();
            }

            string tagIdentifier = l.Substring(0, 4);

            if (pendingListenerImplementors.ContainsKey(tagIdentifier)) {
                var listenerInstance = (IBilgeMessageListener)pendingListenerImplementors[tagIdentifier].Invoke(null, new object[] { l });
                Bilge.AddHandler(listenerInstance);
            }
        }
    }
}