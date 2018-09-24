// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The state machine's context.
    /// </summary>
    public class Context
    {
        private State lastState;
        private State currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context()
        {
            this.lastState = null;
            this.SetNewState(new WaitingForCtuState());
        }

        /// <summary>
        /// Event that is fired whenever a state has to send a CTU to the TopBox.
        /// </summary>
        public event EventHandler<TripletsProducedEventArgs> TripletsProduced;

        /// <summary>
        /// Event that is fired whenever a state has some log to be written in the GUI.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessageProduced;

        /// <summary>
        /// Event that is fired whenever there's something new
        /// about the download process of a file.
        /// </summary>
        public event EventHandler<DownloadFileNotificationEventArgs> DownloadFileNotificationProduced;

        /// <summary>
        /// Gets or sets FtpServerIP.
        /// </summary>
        public string FtpServerIP { get; set; }

        /// <summary>
        /// Triggers the state machine's current state.
        /// </summary>
        public void Trigger()
        {
            this.Trigger(null);
        }

        /// <summary>
        /// Triggers the state machine's current state with a specific CTU's triplet.
        /// </summary>
        /// <param name="triplet">The CTU triplet to be managed.</param>
        public void Trigger(Triplet triplet)
        {
            this.currentState.Handle(this, triplet);
        }

        /// <summary>
        /// Sets the new state machine's state.
        /// </summary>
        /// <param name="state">The new state to be applyed.</param>
        public void SetNewState(State state)
        {
            this.lastState = this.currentState;
            this.currentState = state;

            var logEvent = new LogMessageEventArgs(
                this.currentState.GetType().Name, this.currentState.NextStates, string.Empty)
                { PreviousStateName = this.lastState != null ? this.lastState.GetType().Name : string.Empty };
            this.RaiseLogMessageProduced(logEvent);
        }

        /// <summary>
        /// Sets the new state machine's as the last set before.
        /// </summary>
        public void ReturnToLastState()
        {
            this.SetNewState(this.lastState);
        }

        /// <summary>
        /// Notifies to all the registered handlers about some triplets
        /// to be sent to the TopBox.
        /// </summary>
        /// <param name="e">The event containing the triplets.</param>
        public void RaiseTripletsProduced(TripletsProducedEventArgs e)
        {
            var handler = this.TripletsProduced;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Notifies to all the registered handlers about a log message
        /// produce by a state.
        /// </summary>
        /// <param name="e">The event containing the log message.</param>
        public void RaiseLogMessageProduced(LogMessageEventArgs e)
        {
            var handler = this.LogMessageProduced;
            if (handler != null)
            {
                e.PreviousStateName = this.lastState != null ? this.lastState.GetType().Name : string.Empty;
                handler(this, e);
            }
        }

        /// <summary>
        /// Notifies to all the registered handlers about the download progress
        /// of a file.
        /// </summary>
        /// <param name="e">The event containing the download progress of a file.</param>
        public void RaiseDownloadFileNotificationProduced(DownloadFileNotificationEventArgs e)
        {
            var handler = this.DownloadFileNotificationProduced;
            if (handler != null)
            {
                handler(this, e);
            }
        }        
    }
}
