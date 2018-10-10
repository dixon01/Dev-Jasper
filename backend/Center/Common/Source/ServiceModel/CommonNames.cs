// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonNames.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommonNames type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    /// <summary>
    /// Collection of common entity names that are used throughout the system.
    /// </summary>
    public static class CommonNames
    {
        /// <summary>
        /// The username of the overall administrator.
        /// </summary>
        public const string AdminUsername = "admin";

        /// <summary>
        /// The tenant name of the overall administration; <see cref="AdminUsername"/> belongs to this tenant.
        /// </summary>
        public const string AdminTenantName = "Administration";

        /// <summary>
        /// The name of the super user role which always has all permissions.
        /// </summary>
        public const string SuperuserRoleName = "Superuser";

#if __UseLuminatorTftDisplay

        /// <summary>
        /// The username of the tenant administrator.
        /// </summary>
        public const string TenantAdminUsername = "tenantadmin";
        
        /// <summary>
        /// The tenant administration; <see cref="TenantAdminUsername"/> belongs to this role.
        /// </summary>
        public const string TenantAdminRoleName = "Tenant Administration";

        /// <summary>
        /// The username of the unit administrator.
        /// </summary>
        public const string UnitAdminUsername = "unitadmin";
        
        /// <summary>
        /// The unit administration; <see cref="UnitAdminUsername"/> belongs to this role.
        /// </summary>
        public const string UnitAdminRoleName = "Unit Administration";

#endif
    }
}
