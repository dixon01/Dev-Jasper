// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveAsParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the set of parameters required to save a project as new one.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;

    /// <summary>
    /// Defines the set of parameters required to save a project as new one.
    /// </summary>
    public class SaveAsParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAsParameters"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        public SaveAsParameters(string name, TenantReadableModel tenant)
        {
            this.Name = name;
            this.Tenant = tenant;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the tenant.
        /// </summary>
        public TenantReadableModel Tenant { get; private set; }
    }
}
