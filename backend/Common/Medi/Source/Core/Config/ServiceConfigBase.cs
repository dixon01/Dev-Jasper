// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Services;

    /// <summary>
    /// Base class for all <see cref="IService"/> configurations.
    /// </summary>
    [XmlInclude(typeof(PortForwardingServiceConfig))]
    [XmlInclude(typeof(LocalResourceServiceConfig))]
    [XmlInclude(typeof(RemoteResourceServiceConfig))]
    public class ServiceConfigBase
    {
    }
}
