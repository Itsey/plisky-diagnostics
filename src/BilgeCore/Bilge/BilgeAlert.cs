namespace Plisky.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Manages Alert type records for Bilge, that do not go through the traditional trace level selection but are deisgned to push notifications to the monitoring
    /// infrastructure irrespective of trace level.
    /// </summary>
    public class BilgeAlert {
        private string AlertContextId = "Alerting";
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
            AlertingContext = new Dictionary<string, string>();
            AlertingContext.Add(Bilge.BILGE_INSTANCE_CONTEXT_STR, AlertContextId);
        }

        /// <summary>
        /// Alerting level call to indicate that an application is online and ready to begin processing.  Starts the uptime counter and sends basic telemetry to the
        /// trace stream.
        /// </summary>
        /// <param name="appName">An identifier for the application</param>
        /// <returns>A string that can be written out indicating the app is online ( e.g. can be passed to console.writeline )</returns>
        public string Online(string appName) {
            if (string.IsNullOrWhiteSpace(appName)) {
                appName = "Unknown";
            }

            string ver = GetVersionFromAssembly();
            AlertingContext[Bilge.BILGE_INSTANCE_CONTEXT_STR] = appName;
            onlineAt = DateTime.Now;
            string toWrite = $"{appName} Online. v-{ver}. @{onlineAt}";

            var alertValues = new Dictionary<string, string>();
            alertValues.Add("alert-name", "online");
            alertValues.Add("onlineAt", onlineAt.ToString());
            alertValues.Add("machine-name", Router.MachineNameCache);
            alertValues.Add("app-name", appName);
            alertValues.Add("app-ver", ver);

            AlertQueue(toWrite, alertValues);
            return toWrite;
        }

        private void AlertQueue(string message, Dictionary<string, string> values) {
            string meth = "-ba-alert-";
            string pth = "alerting";
            int ln = 0;

            var mmd = new MessageMetadata(meth, pth, ln);
            mmd.CommandType = TraceCommandTypes.Alert;
            mmd.Body = message;
            foreach (string l in values.Keys) {
                mmd.MessageTags.Add(l, values[l]);
            }

            Router.PrepareMetaData(mmd, AlertingContext);
            Router.QueueMessage(mmd);
        }

        private string GetVersionFromAssembly() {
            var asm = Assembly.GetEntryAssembly();
            if (asm != null) {
                return asm.GetName().Version.ToString();
            }
            return "unknown";
        }

        private string JsonTheValues(Dictionary<string, string> values) {
            if (values.Count > 0) {
                var sb = new StringBuilder();
                sb.Append("{ ");
                sb.Append($" \"ALXMV\": \"ALXMV-1\"");

                string postAppend = string.Empty;

                foreach (string v in values.Keys) {
                    sb.Append(postAppend);
                    sb.Append($" \"{v}\": \"{values[v]}\"");
                    postAppend = "," + Environment.NewLine;
                }

                sb.Append("}");
                return sb.ToString();
            } else {
                return string.Empty;
            }
        }
    }
}