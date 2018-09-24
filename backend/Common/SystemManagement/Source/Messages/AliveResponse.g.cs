// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliveResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AliveResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from clients to the System Manager.
    /// Do not use outside this DLL.
    /// Response from a watchdog client to the watchdog after
    /// getting a <see cref="AliveRequest"/>.
    /// </summary>
    public class AliveResponse
    {
        /// <summary>
        /// Gets or sets the unique application id from which this response comes.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the id from the corresponding request.
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
            return string.Format("AliveResponse[{0}]", this.RequestId);
        }
    }
}
