// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliveRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AliveRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// Request message asking a watchdog client if it is still alive
    /// </summary>
    public class AliveRequest
    {
        /// <summary>
        /// Gets or sets the unique application id for which this request is destined.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the id to identify this request.
        /// It is unique for a given application.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("AliveRequest[{0},{1}]", this.ApplicationId, this.RequestId);
        }
    }
}
