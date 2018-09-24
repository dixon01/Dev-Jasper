// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitDownloadRequestState.cs" company="Gorba AG">
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
    /// The WaitDownloadRequest state.
    /// </summary>
    public class WaitDownloadRequestState : State
    {
        private readonly List<FileToDownloader> filesDownloaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitDownloadRequestState"/> class.
        /// </summary>
        /// <param name="filesDownloaded">
        /// The files to download.
        /// </param>
        public WaitDownloadRequestState(List<FileToDownloader> filesDownloaded)
        {
            this.filesDownloaded = filesDownloaded;
            this.NextStates = new List<string> { "SendDownloadResponseWithAllSuccess" };
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
            if (triplet == null)
            {
                // invalid datagram.
                return;
            }

            // in this state we can trigger only after a:
            // 1) Download Progress Request
            switch (triplet.Tag)
            {
                case TagName.DownloadProgressRequest:
                {
                    // here I've only to go to the state "SendDownloadResponseWithAllSuccess"
                    var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Received Download Process Request.");
                    stateMachine.RaiseLogMessageProduced(logEvent);
                    stateMachine.SetNewState(new SendDownloadResponseWithAllSuccessState(this.filesDownloaded));
                    stateMachine.Trigger();
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
