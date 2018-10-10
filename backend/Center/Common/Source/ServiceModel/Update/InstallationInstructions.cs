// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationInstructions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationInstructions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Update
{
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Installation instructions structure for <see cref="UpdatePart.InstallInstructions"/>.
    /// </summary>
    public class InstallationInstructions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to install the command after reboot of the system.
        /// If this flag is not set (i.e. null), the corresponding update part might or might not be installed after
        /// boot. If it is set, it might still be overridden by another update part that requires it to be different.
        /// Default value is null.
        /// </summary>
        public bool? InstallAfterBoot { get; set; }

        /// <summary>
        /// Gets or sets the pre-installation commands.
        /// These commands are executed before the installation starts.
        /// </summary>
        public RunCommands PreInstallation { get; set; }

        /// <summary>
        /// Gets or sets the post-installation commands.
        /// These commands are executed after the installation completed successfully.
        /// </summary>
        public RunCommands PostInstallation { get; set; }
    }
}
