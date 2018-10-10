// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStoreConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStoreConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for resource store configurations.
    /// The resource store is where the actual resources (files) are stored.
    /// </summary>
    [XmlInclude(typeof(FileResourceStoreConfig))]
    [XmlInclude(typeof(BackgroundSystemResourceStoreConfig))]
    public abstract class ResourceStoreConfigBase
    {
    }
}