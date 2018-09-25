// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStageMetadata.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStageMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    /// <summary>
    /// Defines the metadata for composition of stages.
    /// </summary>
    public interface IStageMetadata
    {
        /// <summary>
        /// Gets the index in the displayed list of stages.
        /// </summary>
        int Index { get; }
    }
}
