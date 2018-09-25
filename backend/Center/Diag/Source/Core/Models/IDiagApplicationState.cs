// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IDiagApplicationState.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Models
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// Defines the state of the icenter.diag application.
    /// </summary>
    public interface IDiagApplicationState : IConnectedApplicationState
    {
        /// <summary>
        /// Gets or sets the favorite units
        /// </summary>
        IEnumerable<UnitFavorite> FavoriteUnits { get; set; }

        /// <summary>
        /// Gets or sets the descriptions of the different units
        /// </summary>
        IDictionary<string, string> UnitDescriptions { get; set; }

        /// <summary>
        /// Initializes the state.
        /// </summary>
        /// <param name="shell">The shell.</param>
        void Initialize(IDiagShell shell);
    }
}