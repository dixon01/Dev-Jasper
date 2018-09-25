// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendDownloadResponseEmptySuccessState.cs" company="Gorba AG">
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
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The SendDownloadResponseState state
    /// </summary>
    public class SendDownloadResponseEmptySuccessState : State
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendDownloadResponseEmptySuccessState"/> class.
        /// </summary>
        public SendDownloadResponseEmptySuccessState()
        {
            this.NextStates = new List<string> { "WaitingForCtu" };
        }

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state Machine Context.
        /// </param>
        /// <param name="triplet">
        /// The triplet eventually to be used.
        /// </param>
        public override void Handle(Context stateMachine, Triplet triplet)
        {
            // we can arrive in this state only from the WaitingForCtuState
            // I've only to create a CTU with the "empty" success and send it.
            var responseTriplet = new DownloadProgressResponse { StatusCode = DownloadStatusCode.Success, FileAbsName = string.Empty };

            var e = new TripletsProducedEventArgs();
            e.Triplets.Add(responseTriplet);
            stateMachine.RaiseTripletsProduced(e);

            // and now I've to return spontaneously to the WaitForCtuState
            var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Sending Download Progress Response (empty).");
            stateMachine.RaiseLogMessageProduced(logEvent);
            stateMachine.SetNewState(new WaitingForCtuState());
            stateMachine.Trigger();
        }
    }
}
