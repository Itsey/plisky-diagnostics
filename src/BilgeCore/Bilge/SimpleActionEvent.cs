﻿namespace Plisky.Diagnostics {

    /// <summary>
    /// Simple action has occured
    /// </summary>
    public class SimpleActionEvent : IBilgeActionEvent {

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleActionEvent"/> class.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="data">A Data field associated with the action.</param>
        /// <param name="value">The value of the action.</param>
        /// <param name="success">Indicates whether it was successfull, defaults to true.</param>
        public SimpleActionEvent(string name, string data, string value, bool success = true) {
            Name = name;
            CallCount = 0;
            Data = data;
            Meta = value;
            Success = success;
        }

        /// <summary>
        /// repeated action count.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// associated data.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// meta info
        /// </summary>
        public string Meta { get; private set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// action worked
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// directly set the call count
        /// </summary>
        /// <param name="i">the call count to set</param>
        public void SetCallCount(int i) {
            CallCount = i;
        }
    }
}