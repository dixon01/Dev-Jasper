// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetBackgroundSystemConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetBackgroundSystemConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Gets the BackgroundSystem configuration for the specified portal address.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "BackgroundSystemConfiguration")]
    public sealed class GetBackgroundSystemConfiguration : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the portal address.
        /// </summary>
        [Alias("Address", "PortalAddress")]
        [Parameter]
        public string CenterPortalAddress { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var configuration =
                BackgroundSystemConfigurationProvider.Current.GetConfiguration(this.CenterPortalAddress);
            this.WriteObject(configuration);
        }
    }
}