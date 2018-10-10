// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMenuItemMetadata.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    /// <summary>
    /// Defines the metadata for composition of menu items.
    /// </summary>
    public interface IMenuItemMetadata
    {
        /// <summary>
        /// Gets the index in the displayed list of menu items.
        /// </summary>
        int Index { get; }
    }
}
