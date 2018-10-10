// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationRequestBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationRequestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from clients to System Manager.
    /// Do not use outside this DLL.
    /// Base class for requests to re-launch or exit an application.
    /// </summary>
    public abstract class ApplicationRequestBase
    {
        /// <summary>
        /// Gets or sets the application id of the application to re-launch or exit.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the reason for doing this request.
        /// </summary>
        public string Reason { get; set; }
    }
}