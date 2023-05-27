namespace Plisky.Diagnostics {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Current active configuration applied to the various writers.  Holds context data as well as the current trace level etc.
    /// </summary>
    public class ConfigSettings {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettings"/> class.
        /// </summary>
        public ConfigSettings() {
            MetaContexts = new Dictionary<string, string>();
            InstanceContext = null;
            TraceConfig = new TraceConfiguraton();
        }

        /// <summary>
        /// Holds the detailed trace configuraiton which determines how messages are written.
        /// </summary>
        public TraceConfiguraton TraceConfig { get; set; }

        /// <summary>
        /// Gets or Sets the currently Active Trace Level
        /// </summary>
        public SourceLevels ActiveTraceLevel { get; internal set; }

        /// <summary>
        /// Sets an instance context
        /// </summary>
        public string InstanceContext {
            get {
                return MetaContexts[Bilge.BILGE_INSTANCE_CONTEXT_STR];
            }

            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    value = "default";
                }

                MetaContexts[Bilge.BILGE_INSTANCE_CONTEXT_STR] = value;
            }
        }

        /// <summary>
        /// returns the current meta contexts
        /// </summary>
        public Dictionary<string, string> MetaContexts { get; set; }

        /// <summary>
        /// Sets a session context
        /// </summary>
        public string SessionContext {
            get {
                return MetaContexts[Bilge.BILGE_SESSION_CONTEXT_STR];
            }

            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    value = Guid.NewGuid().ToString();
                }
                MetaContexts[Bilge.BILGE_SESSION_CONTEXT_STR] = value;
            }
        }

        /// <summary>
        /// Sets a filter that can be used to filer session content
        /// </summary>
        public Func<Dictionary<string, string>, bool> SessionFilter { get; set; }
    }
}