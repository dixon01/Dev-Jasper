// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceDataStoreConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResourceDataStoreConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The file resource data store configuration.
    /// The connected data store will store all information locally in files.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Resources.Data.FileResourceDataStore, Gorba.Common.Medi.Resources")]
    public class FileResourceDataStoreConfig : ResourceDataStoreConfigBase
    {
    }
}