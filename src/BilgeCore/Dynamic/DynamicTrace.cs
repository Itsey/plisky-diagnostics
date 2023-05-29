using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Plisky.Diagnostics {
    /// <summary>
    /// The dynamic trace class supports storing references to instances of Bilge so that trace can be changed dynamically.
    /// </summary>
    public class DynamicTrace {
        private List<WeakReference<Bilge>> bilgeInstances = new List<WeakReference<Bilge>>();

        /// <summary>
        /// Creates an instance of bilge, while storing a copy of the instance for dynamic configuration later.
        /// </summary>
        /// <param name="initialisationContext">the context to pass to the instance</param>
        /// <returns>The new Bilge instance</returns>
        public Bilge CreateBilge(string initialisationContext) {
            var newInstance = new Bilge(initialisationContext);
            bilgeInstances.Add(new WeakReference<Bilge>(newInstance));
            return newInstance;
        }

        /// <summary>
        /// Sets the resolver string to establish trace for new bilge instances
        /// </summary>
        /// <param name="newConfiguration">The configuration string to use</param>
        public void SetConfigurationResolver(string newConfiguration) {
            var cr = Bilge.SetConfigurationResolver(newConfiguration);
            foreach (var l in bilgeInstances) {
                if (l.TryGetTarget(out var nextInstance)) {
                    string ctxt = nextInstance.GetContexts().First(x => x.Item1 == Bilge.BILGE_INSTANCE_CONTEXT_STR).Item2;
                    nextInstance.ActiveTraceLevel = cr(ctxt, nextInstance.ActiveTraceLevel);
                }
            }
        }

        /// <summary>
        /// Attempts to set the trace level on all instances that were created wtih the DynamicTrace CreateBilge method.
        /// </summary>
        /// <param name="newTraceLevel">The desired trace level</param>
        /// <returns>Number of instances updated</returns>
        public int SetTraceLevel(SourceLevels newTraceLevel) {
            int instancesHit = 0;
            foreach (var l in bilgeInstances) {
                if (l.TryGetTarget(out var nextInstance)) {
                    nextInstance.ActiveTraceLevel = newTraceLevel;
                    instancesHit++;
                }
            }
            return instancesHit;
        }
    }
}