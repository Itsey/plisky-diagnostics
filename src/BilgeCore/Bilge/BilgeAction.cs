namespace Plisky.Diagnostics {

    using System;

    /// <summary>
    /// Adds action support to bilge, items which are supposed to happen and where notifications that they have happened occurs. This is designed to extend the trace
    /// capabilities beyond literal trace into interactive test and debugging support.
    /// </summary>
    public class BilgeAction {
        private ConfigSettings cfg;

        /// <summary>
        /// Initializes a new instance of the <see cref="BilgeAction"/> class.
        /// Create an action with the current config
        /// </summary>
        /// <param name="activeConfig"></param>
        public BilgeAction(ConfigSettings activeConfig) {
            cfg = activeConfig;
        }

        /// <summary>
        /// Called to indicate an action has failed to occur or failed to execute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Failed(string name, string data) {
            Failed(name, data, null);
        }

        /// <summary>
        /// Called to indicate an action has failed to occur or failed to execute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        public void Failed(string name, string data, string value) {
            SimpleActionEvent sae = new SimpleActionEvent(name, data, value, false);
            Failed(sae);
        }

        /// <summary>
        /// Called to indicate an action has failed to occur or failed to execute
        /// </summary>
        /// <param name="evt"></param>
        public void Failed(IBilgeActionEvent evt) {
            BilgeRouter.Router.ActionOccurs(evt, cfg);
        }

        /// <summary>
        /// Called to indicate an action has occured
        /// </summary>
        /// <param name="evt"></param>
        public void Occured(IBilgeActionEvent evt) {
            BilgeRouter.Router.ActionOccurs(evt, cfg);
        }

        /// <summary>
        /// Called to indicate an action has occured
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        public void Occured(string name, string data, string value) {
            var sae = new SimpleActionEvent(name, data, value);
            Occured(sae);
        }

        /// <summary>
        /// Called to indicate an action has occured
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Occured(string name, string data) {
            Occured(name, data, null);
        }

        /// <summary>
        /// Register a callback handler that will be called when an action occurs
        /// </summary>
        /// <param name="action">The callback handler for the action</param>
        /// <param name="actionHandlerName">The reference name of the handler</param>
        /// <returns>True if the registration succeeded</returns>
        public bool RegisterHandler(Action<IBilgeActionEvent> action, string actionHandlerName) {
            return BilgeRouter.Router.AddActionHandler(action, actionHandlerName);
        }

        /// <summary>
        /// Remove a callback handler for an action 
        /// </summary>
        /// <param name="action">The action to unregister</param>
        /// <param name="actionHandlerName">The name to unregister</param>
        public void UnregisterHandler(Action<IBilgeActionEvent> action, string actionHandlerName) {
            BilgeRouter.Router.RemoveActionHandler(action, actionHandlerName);
        }
    }
}