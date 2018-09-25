// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    /// <summary>
    /// The possible stages of the update application.
    /// </summary>
    public enum UpdateStage
    {
        /// <summary>
        /// Downloading one or more updates.
        /// </summary>
        ReceivingUpdate,

        /// <summary>
        /// Installing one or more updates.
        /// </summary>
        Installing,

        /// <summary>
        /// Uploading feedback.
        /// </summary>
        SendingFeedback,

        /// <summary>
        /// Forwarding one or more updates to other units.
        /// </summary>
        ForwardingUpdate,

        /// <summary>
        /// Forwarding feedback from other units.
        /// </summary>
        ForwardingFeedback,
        
        /// <summary>
        /// Uploading files from the unit to the remote server
        /// </summary>
        UploadingFiles
    }
}