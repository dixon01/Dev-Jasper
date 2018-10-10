// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationStateRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationStateRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from client to the System Manager.
    /// Do not use outside this DLL.
    /// Requests a <see cref="StateChangeNotification"/> for the given application.
    /// </summary>
    public class ApplicationStateRequest
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }
    }
}
