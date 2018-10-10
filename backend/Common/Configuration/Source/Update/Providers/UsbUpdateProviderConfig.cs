// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbUpdateProviderConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UsbUpdateProviderConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Providers
{
    using System;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the USB update provider.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Usb.UsbUpdateProvider, Gorba.Common.Update.Usb")]
    public class UsbUpdateProviderConfig : UpdateProviderConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbUpdateProviderConfig"/> class.
        /// </summary>
        public UsbUpdateProviderConfig()
        {
            this.Name = "UsbProvider";
        }

        /// <summary>
        /// Gets or sets the repository base path in which
        /// the repository.xml file can be found.
        /// </summary>
        public string RepositoryBasePath { get; set; }
    }
}