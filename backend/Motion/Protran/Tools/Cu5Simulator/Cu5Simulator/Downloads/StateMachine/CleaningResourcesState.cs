// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleaningResourcesState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The CleaningResourcesState state.
    /// </summary>
    public class CleaningResourcesState : State
    {
        private readonly List<FileToDownloader> filesToDelete;
        private Context contextStateMachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleaningResourcesState"/> class.
        /// </summary>
        /// <param name="filesToDelete">
        /// The files to delete.
        /// </param>
        public CleaningResourcesState(List<FileToDownloader> filesToDelete)
        {
            this.filesToDelete = filesToDelete;
            this.NextStates = new List<string> { "WaitingForCtu" };
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
            this.contextStateMachine = stateMachine;
            this.DeleteAllResources();

            // now it's the time to go to the WaitingForCtuState
            var logEvent = new LogMessageEventArgs(this.GetType().Name, this.NextStates, "Finished to clean all the resources.");
            stateMachine.RaiseLogMessageProduced(logEvent);
            stateMachine.SetNewState(new WaitingForCtuState());
        }

        private void DeleteAllResources()
        {
            // the files don't exist in truth.
            // so, here I simply clear the list.
            foreach (var fileInfo in this.filesToDelete)
            {
                var file = new FileInfo(fileInfo.AbsName);
                var localFileName = fileInfo.DirectoryForDownloadedFiles + file.Name;
                try
                {
                    // I make sure to have a clean destination directory.
                    if (File.Exists(localFileName))
                    {
                        var logEvent = new LogMessageEventArgs(
                            this.GetType().Name, this.NextStates, string.Format("Deleting file: {0}", fileInfo.AbsName));
                        this.contextStateMachine.RaiseLogMessageProduced(logEvent);
                        File.Delete(localFileName);
                        logEvent = new LogMessageEventArgs(
                            this.GetType().Name, this.NextStates, string.Format("Deleted file: {0}", fileInfo.AbsName));
                        this.contextStateMachine.RaiseLogMessageProduced(logEvent);
                    }
                }
                catch (Exception)
                {
                    // an error was occured on deleting the file.
                    var logEvent = new LogMessageEventArgs(
                        this.GetType().Name, this.NextStates, string.Format("Error on deleting file: {0}", fileInfo.AbsName));
                    this.contextStateMachine.RaiseLogMessageProduced(logEvent);
                }
            }

            this.filesToDelete.Clear();
        }
    }
}
