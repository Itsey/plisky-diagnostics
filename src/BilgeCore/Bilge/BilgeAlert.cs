using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Plisky.Diagnostics {
    /// <summary>
    /// Manages Alert type records for Bilge, that do not go through the traditional trace level selection but are deisgned to push notifications to the monitoring
    /// infrastructure irrespective of trace level.
    /// </summary>
    public class BilgeAlert {
        private readonly string AlertContextId = "Alerting";
        private DateTime onlineAt;
        private Dictionary<string, string> AlertingContext { get; set; }

        private BilgeRouter Router {
            get {
                return BilgeRouter.Router;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeAlert"/> class.
        /// </summary>
        internal BilgeAlert() {
            AlertingContext = new Dictionary<string, string> {
                { Bilge.BILGE_INSTANCE_CONTEXT_STR, AlertContextId }
            };
        }

        /// <summary>
        /// Alerting level call to indicate that an application is online and ready to begin processing.  Starts the uptime counter and sends basic telemetry to the
        /// trace stream.
        /// </summary>
        /// <param name="appName">An identifier for the application</param>        
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>A string that can be written out indicating the app is online ( e.g. can be passed to console.writeline )</returns>
        public string Online(string appName, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            return Online(appName, null, meth, pth, ln);
        }

        /// <summary>
        /// Alerting level call to indicate that an application is online and ready to begin processing.  Starts the uptime counter and sends basic telemetry to the
        /// trace stream.
        /// </summary>
        /// <param name="appName">An identifier for the application</param>        
        /// <param name="properties">A dictionary of properties to be added to the alert</param>
        /// <param name="meth">The method name of the calling method.</param>
        /// <param name="pth">The path to the file of source for the calling method.</param>
        /// <param name="ln">The line number where the call was made.</param>
        /// <returns>A string that can be written out indicating the app is online ( e.g. can be passed to console.writeline )</returns>
        public string Online(string appName, Dictionary<string, string> properties, [CallerMemberName] string meth = null, [CallerFilePath] string pth = null, [CallerLineNumber] int ln = 0) {
            if (string.IsNullOrWhiteSpace(appName)) {
                appName = "Unknown";
            }

            string ver = GetVersionFromAssembly();
            AlertingContext[Bilge.BILGE_INSTANCE_CONTEXT_STR] = appName;
            onlineAt = DateTime.Now;
            string toWrite = $"{appName} Online. v-{ver}. @{onlineAt}";

            var alertValues = new Dictionary<string, string> {
                { "alert-name", "online" },
                { "onlineAt", onlineAt.ToString() },
                { "machine-name", Router.MachineNameCache },
                { "app-name", appName },
                { "app-ver", ver },
                { "alert-id", Guid.NewGuid().ToString() }
            };

            if (properties != null) {
                foreach (string k in properties.Keys) {
                    alertValues.Add(k.ToString(), properties[k].ToString());
                }
            }

            AlertQueue(toWrite, alertValues, meth, pth, ln);
            return toWrite;

        }

        private void AlertQueue(string message, Dictionary<string, string> values, string meth, string pth, int line) {
#if NETCOREAPP
            meth ??= "-ba-alert-";
            pth ??= "alerting";
#else
            meth = meth ?? "-ba-alert-";
            pth = pth ?? "alerting";
#endif
            var mmd = new MessageMetadata(meth, pth, line) {
                CommandType = TraceCommandTypes.Alert,
                Body = message
            };
            foreach (string l in values.Keys) {
                mmd.MessageTags.Add(l, values[l]);
            }
            Router.PrepareMetaData(mmd, AlertingContext);
            Router.QueueMessage(mmd);
        }

        private string GetVersionFromAssembly() {
            var asm = Assembly.GetEntryAssembly();
            return asm != null ? asm.GetName().Version.ToString() : "unknown";
        }


    }
}