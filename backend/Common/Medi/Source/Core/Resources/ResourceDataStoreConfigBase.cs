// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDataStoreConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceDataStoreConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for resource data store configuration.
    /// The data store is where the resource information is stored.
    /// </summary>
    [XmlInclude(typeof(FileResourceDataStoreConfig))]
    [XmlInclude(typeof(BackgroundSystemResourceDataStoreConfig))]
    public abstract class ResourceDataStoreConfigBase
    {
    }
}