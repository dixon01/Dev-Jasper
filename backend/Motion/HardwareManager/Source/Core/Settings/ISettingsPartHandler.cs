// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsPartHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISettingsPartHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using Gorba.Common.Configuration.HardwareManager;

    /// <summary>
    /// Interface for a handler for a part of the settings.
    /// </summary>
    public interface ISettingsPartHandler
    {
        /// <summary>
        /// Apply the given settings.
        /// </summary>
        /// <param name="setting">
        /// The setting object.
        /// </param>
        /// <returns>
        /// True if the system drive should be committed and the system rebooted.
        /// </returns>
        bool ApplySetting(HardwareManagerSetting setting);
    }
}
