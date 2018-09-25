// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterAcknowledge.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegisterAcknowledge type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// The acknowledge to a <see cref="RegisterMessage"/>.
    /// </summary>
    public class RegisterAcknowledge
    {
        /// <summary>
        /// Gets or sets the request that triggered this acknowledge.
        /// </summary>
        public RegisterMessage Request { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the information about the application.
        /// </summary>
        public ApplicationInfo Info { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("RegisterAcknowledge[{0}]", this.ApplicationId);
        }
    }
}