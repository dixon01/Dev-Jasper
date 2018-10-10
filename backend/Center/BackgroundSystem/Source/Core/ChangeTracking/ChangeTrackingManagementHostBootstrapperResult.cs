// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingManagementHostBootstrapperResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The change tracking management host bootstrapper result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.ChangeTracking
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Client.ChangeTracking;

    /// <summary>
    /// The change tracking management host bootstrapper result.
    /// </summary>
    public class ChangeTrackingManagementHostBootstrapperResult : ChangeTrackingManagementBootstrapperResult
    {
        /// <summary>
        /// Gets or sets the change tracking services.
        /// </summary>
        public List<DisposableServiceHost> ChangeTrackingServiceHosts { get; set; }

        /// <summary>
        /// Gets or sets the non change tracking service hosts.
        /// </summary>
        public List<IServiceHost> NonChangeTrackingServiceHosts { get; set; }
    }
}
