// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdminApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdminApplicationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Client;

    /// <summary>
    /// The icenter.admin application state interface.
    /// </summary>
    public interface IAdminApplicationState : IConnectedApplicationState
    {
        /// <summary>
        /// Gets or sets the recently edited entities.
        /// </summary>
        IDictionary<string, IList<RecentlyEditedEntityReference>> RecentlyEditedEntities { get; set; }

        /// <summary>
        /// Gets or sets the list of all stages with their properties.
        /// </summary>
        List<StageModel> Stages { get; set; }

        /// <summary>
        /// Initializes the state.
        /// </summary>
        /// <param name="shell">The shell.</param>
        void Initialize(IAdminShell shell);
    }
}
