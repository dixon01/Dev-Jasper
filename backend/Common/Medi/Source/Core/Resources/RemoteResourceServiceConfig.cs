// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteResourceServiceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteResourceServiceConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The remote resource service configuration.
    /// Use this configuration if you want to connect to a resource service running
    /// in a different node but on the same unit.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Resources.Services.RemoteResourceService, Gorba.Common.Medi.Resources")]
    public class RemoteResourceServiceConfig : ResourceServiceConfigBase
    {
    }
}