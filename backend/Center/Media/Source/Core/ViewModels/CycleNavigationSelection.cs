// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleNavigationSelection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CycleNavigationSelection.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;

    /// <summary>
    /// the CycleNavigationSelection
    /// </summary>
    public enum CycleNavigationSelection
    {
        /// <summary>
        /// Nothing selected
        /// </summary>
        None,

        /// <summary>
        /// cycle package selected
        /// </summary>
        CyclePackage,

        /// <summary>
        /// cycle selected
        /// </summary>
        Cycle,

        /// <summary>
        /// section selected
        /// </summary>
        Section,
    }
}