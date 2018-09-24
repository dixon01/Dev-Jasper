// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemResourceDataStoreConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemResourceDataStoreConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The resource data store config for the background system.
    /// Be aware that the connected data store is only available in the BackgroundSystem application.
    /// </summary>
    [Implementation("Gorba.Center.BackgroundSystem.Core.Resources.BackgroundSystemResourceDataStore, "
                    + "Gorba.Center.BackgroundSystem.Core")]
    public class BackgroundSystemResourceDataStoreConfig : ResourceDataStoreConfigBase
    {
    }
}