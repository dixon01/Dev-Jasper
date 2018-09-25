// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DiagApplicationState.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// Implements the <see cref="IDiagApplicationState"/> interface to provide the state of the Media application.
    /// </summary>
    [Export]
    [Export(typeof(IApplicationState))]
    [Export(typeof(IDiagApplicationState))]
    [DataContract]
    public class DiagApplicationState : ConnectedApplicationState, IDiagApplicationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagApplicationState"/> class.
        /// </summary>
        public DiagApplicationState()
        {
            this.FavoriteUnits = new List<UnitFavorite>();
            this.UnitDescriptions = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IDiagShell Shell { get; private set; }

        /// <summary>
        /// Gets or sets the favorite units
        /// </summary>
        [DataMember]
        public IEnumerable<UnitFavorite> FavoriteUnits { get; set; }

        /// <summary>
        /// Gets or sets the descriptions of the different units
        /// </summary>
        [DataMember]
        public IDictionary<string, string> UnitDescriptions { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="shell">The shell.</param>
        public void Initialize(IDiagShell shell)
        {
            this.Shell = shell;

            if (this.UnitDescriptions == null)
            {
                this.UnitDescriptions = new Dictionary<string, string>();
            }

            if (this.FavoriteUnits == null)
            {
                this.FavoriteUnits = Enumerable.Empty<UnitFavorite>();
            }
        }
    }
}