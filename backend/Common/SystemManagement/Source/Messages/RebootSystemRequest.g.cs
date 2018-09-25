// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RebootSystemRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RebootSystemRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from clients to System Manager.
    /// Do not use outside this DLL.
    /// Request to reboot the entire system.
    /// </summary>
    public class RebootSystemRequest
    {
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        public string Reason { get; set; }
    }
}
