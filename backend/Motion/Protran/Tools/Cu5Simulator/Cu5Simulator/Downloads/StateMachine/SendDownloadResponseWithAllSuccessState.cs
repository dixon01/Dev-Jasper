// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendDownloadResponseWithAllSuccessState.cs" company="Gorba AG">
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
    /// The SendDownloadResponseWithAllSuccess state.
    /// </summary>
    public class SendDownloadResponseWithAllSuccessState : State
    {
        private readonly List<FileToDownloader> filesDownloaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendDownloadResponseWithAllSuccessState"/> class.
        /// </summary>
        /// <param name="filesDownloaded">
        /// The files downloaded.
        /// </param>
        public SendDownloadResponseWithAllSuccessState(List<FileToDownloader> filesDownloaded)
        {
            this.filesDownloaded = filesDownloaded;
            this.NextStates = new List<string> { "RebootingState" };
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
            foreach (var file in this.filesDownloaded)
            {
                var tripletResponse = new DownloadProgressResponse { FileAbsName = file.AbsName, StatusCode = file.CurrentStatus };
                tripletsEvent.Triplets.Add(tripletResponse);
            }

            var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Sending Download Progress Response.");
            stateMachine.RaiseLogMessageProduced(logEvent);
            stateMachine.RaiseTripletsProduced(tripletsEvent);

            // and now let's change the state to RebootingState.
            stateMachine.SetNewState(new RebootingState());
            stateMachine.Trigger();
        }
    }
}
