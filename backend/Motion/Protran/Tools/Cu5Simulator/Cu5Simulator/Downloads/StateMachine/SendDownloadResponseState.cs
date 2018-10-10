// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendDownloadResponseState.cs" company="Gorba AG">
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
    /// The SendDownloadResponseState state.
    /// </summary>
    public class SendDownloadResponseState : State
    {
        private readonly List<FileToDownloader> filesWhitStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendDownloadResponseState"/> class.
        /// </summary>
        /// <param name="filesWhitStatus">The files whit status.</param>
        public SendDownloadResponseState(List<FileToDownloader> filesWhitStatus)
        {
            this.filesWhitStatus = filesWhitStatus;
            this.NextStates = new List<string> { "DownloadingFileState", "DownloadingRemainingFileState" };
        }

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine's stateMachine.
        /// </param>
        /// <param name="triplet">
        /// The triplet.
        /// </param>
        public override void Handle(Context stateMachine, Triplet triplet)
        {
            // here I've only to prepare the CTU datagram
            // having triplets referring to the files.
            var tripletsEvent = new TripletsProducedEventArgs();
            foreach (var file in this.filesWhitStatus)
            {
                var tripletResponse = new DownloadProgressResponse
                    { FileAbsName = file.AbsName, StatusCode = file.CurrentStatus };
                tripletsEvent.Triplets.Add(tripletResponse);
            }

            stateMachine.RaiseTripletsProduced(tripletsEvent);

            // and now changing the state to the previous DownloadingFileState/DownloadingRemainingFileState
            var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Returning to previous state.");
            stateMachine.RaiseLogMessageProduced(logEvent);
            stateMachine.ReturnToLastState();
        }
    }
}
