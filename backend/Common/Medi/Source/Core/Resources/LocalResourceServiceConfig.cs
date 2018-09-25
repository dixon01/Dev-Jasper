// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceServiceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalResourceServiceConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The local resource service configuration.
    /// Use this configuration if you want to have the resource service running
    /// in this node.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Resources.Services.LocalResourceService, Gorba.Common.Medi.Resources")]
    public class LocalResourceServiceConfig : ResourceServiceConfigBase
    {
        /// <summary>
        /// Gets or sets the data store configuration.
        /// The data store is where the resource information is stored.
        /// If this property is not set, the default implementation will be used.
        /// </summary>
        public ResourceDataStoreConfigBase DataStore { get; set; }

        /// <summary>
        /// Gets or sets the resource store configuration.
        /// The resource store is where the actual resources (files) are stored.
        /// If this property is not set, the default implementation will be used.
        /// </summary>
        public ResourceStoreConfigBase ResourceStore { get; set; }
    }
}