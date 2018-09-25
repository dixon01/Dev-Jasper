// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangeMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateChangeMessageBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message base class for communication between System Manager and clients.
    /// Do not use outside this DLL.
    /// </summary>
    public abstract class StateChangeMessageBase
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public ApplicationState State { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}[{1},{2}]", this.GetType().Name, this.ApplicationId, this.State);
        }
    }
}