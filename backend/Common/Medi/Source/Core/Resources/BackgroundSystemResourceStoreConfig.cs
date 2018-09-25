// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemResourceStoreConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemResourceStoreConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The resource store config for the background system.
    /// Be aware that the connected resource store is only available in the BackgroundSystem application.
    /// </summary>
    [Implementation("Gorba.Center.BackgroundSystem.Core.Resources.BackgroundSystemResourceStore, "
                    + "Gorba.Center.BackgroundSystem.Core")]
    public class BackgroundSystemResourceStoreConfig : ResourceStoreConfigBase
    {
    }
}