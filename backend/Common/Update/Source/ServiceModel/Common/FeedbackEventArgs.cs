// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;

    /// <summary>
    /// Event arguments for the <see cref="IUpdateProvider.FeedbackReceived"/> event.
    /// </summary>
    public class FeedbackEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackEventArgs"/> class.
        /// </summary>
        /// <param name="logFiles">
        /// The received log files.
        /// </param>
        /// <param name="updateStates">
        /// The received update states.
        /// </param>
        /// <param name="uploadedFiles">
        /// The uploaded files.
        /// </param>
        public FeedbackEventArgs(IReceivedLogFile[] logFiles, UpdateStateInfo[] updateStates, IReceivedLogFile[] uploadedFiles)
        {
            this.ReceivedLogFiles = logFiles;
            this.ReceivedUpdateStates = updateStates;
            this.UploadedFiles = uploadedFiles;
        }

        /// <summary>
        /// Gets the received log files.
        /// </summary>
        public IReceivedLogFile[] ReceivedLogFiles { get; }

        /// <summary>
        /// Gets the received update states.
        /// </summary>
        public UpdateStateInfo[] ReceivedUpdateStates { get; }

        /// <summary>
        /// Gets the uploaded files.
        /// </summary>
        public IReceivedLogFile[] UploadedFiles { get; }
    }
}