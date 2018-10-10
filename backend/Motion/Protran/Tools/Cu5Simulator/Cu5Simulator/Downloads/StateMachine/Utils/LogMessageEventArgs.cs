// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessageEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Container of a message produced by a state
    /// (just to show it in the GUI).
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageEventArgs"/> class.
        /// </summary>
        /// <param name="stateName">The state name.</param>
        /// <param name="availableNextStates">The available next states.</param>
        /// <param name="message">The message.</param>
        public LogMessageEventArgs(string stateName, List<string> availableNextStates, string message)
        {
            this.AvailableNextStates = availableNextStates;
            this.CurrentStateName = stateName;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets StateName.
        /// </summary>
        public string CurrentStateName { get; set; }

        /// <summary>
        /// Gets or sets PreviousStateName.
        /// </summary>
        public string PreviousStateName { get; set; }

        /// <summary>
        /// Gets or sets AvailableNextStates.
        /// </summary>
        public List<string> AvailableNextStates { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        public string Message { get; set; }
    }
}
