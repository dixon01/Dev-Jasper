// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadingRemaingFilesState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System.Collections.Generic;

    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The DownloadingRemaingFilesState state.
    /// </summary>
    public class DownloadingRemaingFilesState : DownloadingFilesState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadingRemaingFilesState"/> class.
        /// </summary>
        /// <param name="stateMachine">The state Machine.</param>
        /// <param name="filesToDownload">The files to download.</param>
        public DownloadingRemaingFilesState(Context stateMachine, List<FileToDownloader> filesToDownload)
            : base(stateMachine, filesToDownload)
        {
        }
    }
}
