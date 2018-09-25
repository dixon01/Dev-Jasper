// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSourceBase.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSourceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Clients
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Base class for all classes implementing <see cref="IUpdateSource"/>.
    /// </summary>
    public abstract class UpdateSourceBase : UpdateComponentBase, IUpdateSource
    {
        /// <summary>
        /// Event that is fired when new commands are received for one or more units.
        /// </summary>
        public event EventHandler<UpdateCommandsEventArgs> CommandsReceived;

        /// <summary>
        /// Gets or sets the uploads directory.
        /// </summary>
        public string UploadsDirectory { get; set; }

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
        public abstract void SendFeedback(
            IEnumerable<IReceivedLogFile> logFiles,
            IEnumerable<UpdateStateInfo> stateInfos,
            IProgressMonitor progressMonitor);

        /// <summary>
        /// Upload files to the remote server.
        /// </summary>
        /// <param name="uploadFiles">
        /// The log files to upload.
        /// </param>
        /// <exception cref="UpdateException">
        /// Couldn't upload files
        /// </exception>
        public abstract void UploadFiles(IList<IReceivedLogFile> uploadFiles);

        /// <summary>
        /// Upload files from the given directory to the remote server
        /// </summary>
        public abstract void UploadFiles();

        /// <summary>
        /// Raises the <see cref="CommandsReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCommandReceived(UpdateCommandsEventArgs e)
        {
            var handler = this.CommandsReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}