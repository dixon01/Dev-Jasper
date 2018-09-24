// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    /// <summary>
    /// The state of an update that can be reported by a unit to the application that created the update.
    /// </summary>
    public enum UpdateState
    {
        /// <summary>
        /// The state is currently unknown (e.g. if the update has not yet been created).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The update has been created but not yet sent to the unit.
        /// </summary>
        Created = 1,

        /// <summary>
        /// The update is being sent to the unit, but no feed back has yet arrived.
        /// </summary>
        Transferring = 2,

        /// <summary>
        /// The update has been received by the unit.
        /// </summary>
        Transferred = 3,

        /// <summary>
        /// The unit is installing the update.
        /// </summary>
        Installing = 4,

        /// <summary>
        /// The update was successfully installed.
        /// </summary>
        Installed = 10,

        /// <summary>
        /// The update was ignored because a newer update is already available.
        /// </summary>
        Ignored = 11,

        /// <summary>
        /// The update was partially installed.
        /// </summary>
        PartiallyInstalled = 12,

        /// <summary>
        /// The transfer of the update failed.
        /// </summary>
        TransferFailed = 20,

        /// <summary>
        /// The installation of the update failed, nothing was installed.
        /// </summary>
        InstallationFailed = 21
    }
}