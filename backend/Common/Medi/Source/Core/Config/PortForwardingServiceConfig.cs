// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortForwardingServiceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortForwardingServiceConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the port forwarding service.
    /// Currently this config is empty because configuring it
    /// just means that the service should be created.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Ports.PortForwardingService, Gorba.Common.Medi.Ports")]
    public class PortForwardingServiceConfig : ServiceConfigBase
    {
    }
}
