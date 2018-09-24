// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitingForCtuState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The WaitintForCtuState state.
    /// </summary>
    public class WaitingForCtuState : State
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingForCtuState"/> class.
        /// </summary>
        public WaitingForCtuState()
        {
            this.NextStates = new List<string> { "WaitingForCtu", "SendDownloadResponseEmptySuccess", "DownloadingFiles" };
        }

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state Machine Context.
        /// </param>
        /// <param name="triplet">
        /// The triplet.
        /// </param>
        public override void Handle(Context stateMachine, Triplet triplet)
        {
            if (triplet == null)
            {
                // no trigger.
                // I stay in this state.
                return;
            }

            // in this state we can trigger after a:
            // 1) Download Abort
            // 2) Download Progress Request
            // 3) Download Start
            switch (triplet.Tag)
            {
                case TagName.DownloadAbort:
                {
                    // the current state is the "WaitintForCtuState"
                    // and I've received a Download Abort.
                    // we don't have to do nothing in this case.
                    var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Abort.");
                    stateMachine.RaiseLogMessageProduced(logEvent);
                }

                break;

                case TagName.DownloadProgressRequest:
                {
                    // the current state is the "WaitintForCtuState"
                    // and I've received a Download Progress Request.
                    // now we have to go to the SendDownloadResponseEmptySuccessState
                    var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Progress Request.");
                    stateMachine.RaiseLogMessageProduced(logEvent);
                    stateMachine.SetNewState(new SendDownloadResponseEmptySuccessState());
                    stateMachine.Trigger();
                }

                break;

                case TagName.DownloadStart:
                {
                    // the current state is the "WaitintForCtuState"
                    // and I've received a Download Progress Start.
                    // now we have to go to the DownloadingFilesState
                    var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Start.");
                    stateMachine.RaiseLogMessageProduced(logEvent);
                    stateMachine.SetNewState(new DownloadingFilesState(stateMachine, triplet));
                    stateMachine.Trigger(triplet);
                }

                break;

                default:
                {
                    var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, string.Format("Received {0}. Discarded.", triplet.Tag));
                    stateMachine.RaiseLogMessageProduced(logEvent);
                }

                break;
            }
        }
    }
}
