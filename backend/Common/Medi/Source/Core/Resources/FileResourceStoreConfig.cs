// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceStoreConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResourceStoreConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The file resource store configuration.
    /// The connected resource store will simply store resources locally in files.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Resources.Data.FileResourceStore, Gorba.Common.Medi.Resources")]
    public class FileResourceStoreConfig : ResourceStoreConfigBase
    {
    }
}