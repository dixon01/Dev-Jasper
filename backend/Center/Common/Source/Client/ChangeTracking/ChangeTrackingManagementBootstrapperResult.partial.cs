// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingManagementBootstrapperResult.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingManagementBootstrapperResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Result of the run operation on a <see cref="ChangeTrackingManagementBootstrapperResult"/>.
    /// </summary>
    public partial class ChangeTrackingManagementBootstrapperResult : IChangeTrackingManagersSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingManagementBootstrapperResult"/> class.
        /// </summary>
        public ChangeTrackingManagementBootstrapperResult()
        {
            this.Exceptions = new List<Exception>();
        }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        public List<Exception> Exceptions { get; private set; }
    }
}