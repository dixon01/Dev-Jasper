// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Interface to be implemented by classes that send out updates
    /// (currently only update clients).
    /// </summary>
    public interface IUpdateSource : IUpdateComponent
    {
        /// <summary>
        /// Event that is fired when new commands are received for one or more units.
        /// </summary>
        event EventHandler<UpdateCommandsEventArgs> CommandsReceived;

        /// <summary>
        /// Gets or sets the uploads directory.
        /// </summary>
        string UploadsDirectory { get; set; }
        
        /// <summary>
        /// Sends feedback back to the source.
        /// </summary>
        /// <param name="logFiles">
        /// The log files to upload.
        /// </param>
        /// <param name="stateInfos">
        /// The state information objects to upload.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor that observes the upload of update feedback and log files.
        /// </param>
        void SendFeedback(
            IEnumerable<IReceivedLogFile> logFiles,
            IEnumerable<UpdateStateInfo> stateInfos,
            IProgressMonitor progressMonitor);

        /// <summary>
        /// Uploads files from the given source directory
        /// </summary>
        void UploadFiles();
    }
}