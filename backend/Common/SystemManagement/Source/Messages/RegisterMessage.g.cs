// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegisterMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from clients to the System Manager.
    /// Do not use outside this DLL.
    /// This registers an application with System Manager.
    /// The information in this message is used to identify applications without
    /// passing any information to the application when creating it.
    /// </summary>
    public class RegisterMessage
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the process id of the process hosting the application.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("RegisterMessage[{0},{1}]", this.ProcessId, this.Name);
        }
    }
}
