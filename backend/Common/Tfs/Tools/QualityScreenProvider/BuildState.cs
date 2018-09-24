// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BuildState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.QualityScreenProvider
{
    /// <summary>
    /// The build state.
    /// </summary>
    public enum BuildState
    {
        /// <summary>
        /// The build was fully successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The build was partially successful.
        /// </summary>
        Partial = 1,

        /// <summary>
        /// The build failed.
        /// </summary>
        Failed = 2,
    }
}
