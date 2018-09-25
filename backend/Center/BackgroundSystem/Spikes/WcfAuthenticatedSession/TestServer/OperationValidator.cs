// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationValidator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System;
    using System.Linq;
    using System.Security;

    using WcfAuthenticatedSession.ServiceModel;

    /// <summary>
    /// Component used to validate operations according to user, permission, data scope
    /// and optionally tenant identifier.
    /// </summary>
    public abstract class OperationValidator
    {
        static OperationValidator()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current validator.
        /// </summary>
        public static OperationValidator Current { get; private set; }

        /// <summary>
        /// Resets the current validator.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultOperationValidator.Instance);
        }

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="instance">The validator.</param>
        public static void SetCurrent(OperationValidator instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Validates an operation.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="tenantId">
        /// The tenant id.
        /// </param>
        public abstract void ValidateOperation(Permission permission, Users user, int? tenantId = null);

        /// <summary>
        /// Restricts a query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <typeparam name="T">The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public abstract IQueryable<T> RestrictQuery<T>(IQueryable<T> query, Permission permission, Tenants tenant);

        private class DefaultOperationValidator : OperationValidator
        {
            static DefaultOperationValidator()
            {
                Instance = new DefaultOperationValidator();
            }

            public static DefaultOperationValidator Instance { get; private set; }

            public override void ValidateOperation(Permission permission, Users user, int? tenantId = null)
            {
                if (user == Users.God)
                {
                    return;
                }

                if (tenantId.HasValue)
                {
                    if ((permission == Permission.Read && user == Users.Reader)
                        || (permission == Permission.Write && user == Users.Writer))
                    {
                        return;
                    }
                }

                throw new SecurityException();
            }

            public override IQueryable<T> RestrictQuery<T>(IQueryable<T> query, Permission permission, Tenants tenants)
            {
                throw new NotImplementedException();
            }

            /*
            public override void ValidateOperation(
                OperationPermission permission, string dataScope, int? tenantId = null)
            {
                if (OperationContext.Current == null || OperationContext.Current.ServiceSecurityContext == null)
                {
                    Logger.Debug(
                        "Operation with permission '{0}', data scope '{1}' and tenant id {2}"
                        + " accepted because the current OperationContext is null",
                        permission,
                        dataScope,
                        tenantId);
                    return;
                }

                if (string.Equals(
                    OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.AuthenticationType,
                    typeof(LoginValidator).Name))
                {
                    var authorizationTokens =
                        this.ListAuthorizationTokens(
                            OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name, permission, dataScope)
                                         .ToList();
                    if (
                        !authorizationTokens.Any(
                            token =>
                            (token.Tenant == null || token.Tenant.Id == tenantId)
                            && token.Permissions.ContainsKey(dataScope)
                            && token.Permissions[dataScope].Contains(permission)))
                    {
                        throw new SecurityException("Operation not allowed");
                    }
                }
            }

            public override void ValidateUserOperation(OperationPermission permission, int userId)
            {
                if (OperationContext.Current == null || OperationContext.Current.ServiceSecurityContext == null)
                {
                    Logger.Debug(
                        "Operation with permission '{0}' on user {1}"
                        + " allowed because the current OperationContext is null",
                        permission,
                        userId);
                    return;
                }

                if (!string.Equals(
                    OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.AuthenticationType,
                    typeof(LoginValidator).Name))
                {
                    return;
                }

                using (var dataContext = DataContextProvider.Current.Provide(true))
                {
                    var isValid = ExtendQuery(dataContext.Users, permission).Any(user => user.Id == userId);
                    if (!isValid)
                    {
                        throw new SecurityException("Operation not allowed for the specified user");
                    }
                }
            }

            public override FilterBase RestrictFilter(
                OperationPermission permission, string dataScope, FilterBase filter)
            {
                if (filter == null)
                {
                    throw new ArgumentNullException("filter");
                }

                if (string.IsNullOrWhiteSpace(dataScope))
                {
                    throw new ArgumentOutOfRangeException("dataScope");
                }

                if (OperationContext.Current != null && OperationContext.Current.ServiceSecurityContext != null
                    && string.Equals(
                        OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.AuthenticationType,
                        typeof(LoginValidator).Name))
                {
                    Logger.Debug(
                        "Operation with permission '{0}', data scope '{1}' and tenant id {2}"
                        + " accepted because the current OperationContext is null",
                        permission,
                        dataScope,
                        filter.TenantId);

                    var membershipService = ProxyProvider<IMembershipService>.Current.Provide();
                    var authorizationTokens =
                        membershipService.ListAuthorizationTokens(
                            OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name, permission, dataScope)
                                         .ToList();
                    if (authorizationTokens.Count == 0)
                    {
                        Logger.Error(
                            "Operation with permission '{0}', data scope '{1}' and tenant id {2}" + " not accepted",
                            permission,
                            dataScope,
                            filter.TenantId);
                        throw new SecurityException("The operation is not authorized");
                    }

                    var wildCard =
                        authorizationTokens.SingleOrDefault(
                            token =>
                            token.Tenant == null && token.Permissions.ContainsKey(dataScope)
                            && token.Permissions[dataScope].Contains(permission));
                    if (wildCard != null)
                    {
                        return filter;
                    }

                    if (filter.TenantId.HasValue
                        && !authorizationTokens.Where(token => token.Tenant != null)
                                                   .Select(tenant => tenant.Tenant.Id)
                                                   .Contains(filter.TenantId.Value))
                    {
                        throw new SecurityException("The operation is not authorized");
                    }

                    var authorizedFilterData = new List<FilterData>();
                    foreach (var authorizedTenant in authorizationTokens.Where(token => token.Tenant != null))
                    {
                        authorizedFilterData.Add(
                            new FilterData(authorizedTenant.Tenant.Id.ToString(CultureInfo.InvariantCulture)));
                    }

                    if (filter.FilterData.ContainsKey("TenantId"))
                    {
                        var filterData = filter.FilterData["TenantId"];
                        filter.FilterData.Remove("TenantId");
                        authorizedFilterData = authorizedFilterData.Intersect(filterData).ToList();
                    }

                    filter.FilterData.Add("TenantId", authorizedFilterData);
                    return filter;
                }

                return filter;
            }

            public override IQueryable<User> RestrictUserQuery(IQueryable<User> query, OperationPermission permission)
            {
                if (OperationContext.Current == null || OperationContext.Current.ServiceSecurityContext == null)
                {
                    Logger.Debug(
                        "User query with permission '{0}' unfiltered because the current OperationContext is null",
                        permission);
                    return query;
                }

                if (string.Equals(
                    OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.AuthenticationType,
                    typeof(LoginValidator).Name))
                {
                    query = ExtendQuery(query, permission);
                    return query;
                }

                return query;
            }

            public override IEnumerable<AuthorizationToken> ListAuthorizationTokens(
                string username, OperationPermission? permission, string dataScope)
            {
                var lowerUsername = username.ToLower();
                var lowerDataScope = dataScope == null ? null : dataScope.ToLower();
                var lowerPermission = permission.HasValue ? permission.ToString().ToLower() : null;

                using (var associationRepository = CreateAssociationPermissionDataScopeUserRoleRepository())
                {
                    var query =
                        from a in
                            associationRepository.Repository.Query()
                                                 .Where(
                                                     a =>
                                                     (lowerPermission == null
                                                      || a.Permission.Name.ToLower() == lowerPermission)
                                                     && (lowerDataScope == null
                                                         || a.DataScope.Name.ToLower() == lowerDataScope))
                        let roles = a.UserRole.AssociationTenantUserUserRoles
                        from role in roles
                        where role.User.Username.ToLower() == lowerUsername
                        let item =
                            new { DataScope = a.DataScope.Name, Permission = a.Permission.Name.ToLower(), role.Tenant }
                        group item by item.Tenant into grouped
                        select new { Tenant = grouped.Key, Associations = grouped };
                    var list = query.ToList();

                    // ToList() are needed by WCF serialization
                    var result = query.Where(i => i.Tenant != null)
                                      .ToDictionary(
                                          arg => arg.Tenant.ToDto(),
                                          arg =>
                                          arg.Associations.Select(
                                              a =>
                                              new Authorization
                                              {
                                                  DataScope = a.DataScope,
                                                  Permission =
                                                      (OperationPermission)
                                                      Enum.Parse(typeof(OperationPermission), a.Permission, true)
                                              })
                                             .Distinct()
                                             .ToList());
                    var tokens = (from t in result.Keys select this.GetAuthorizationToken(t, result[t])).ToList();
                    var nullTenant = list.SingleOrDefault(t => t.Tenant == null);
                    if (nullTenant == null)
                    {
                        Logger.Trace("Association with null tenant not found for ");
                        return tokens;
                    }

                    // ToList() is required by WCF serialization
                    var authorizations =
                        nullTenant.Associations.Select(
                            a =>
                            new Authorization
                            {
                                DataScope = a.DataScope,
                                Permission =
                                    (OperationPermission)Enum.Parse(typeof(OperationPermission), a.Permission, true)
                            })
                                  .ToList();
                    tokens.Add(this.GetAuthorizationToken(null, authorizations));

                    return tokens;
                }
            }

            public override IQueryable<Common.Data.Entities.Tenant> RestrictTenantQuery(
                IQueryable<Common.Data.Entities.Tenant> query)
            {
                if (OperationContext.Current == null || OperationContext.Current.ServiceSecurityContext == null)
                {
                    Logger.Debug("Tenant query unfiltered because the current OperationContext is null");
                    return query;
                }

                if (string.Equals(
                    OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.AuthenticationType,
                    typeof(LoginValidator).Name))
                {
                    query = ExtendQuery(query);
                    return query;
                }

                return query;
            }

            private static IQueryable<Common.Data.Entities.Tenant> ExtendQuery(
                IQueryable<Common.Data.Entities.Tenant> query,
                IDataContext dataContext = null)
            {
                var createNewDataContext = dataContext == null;
                var lowerUsername = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name.ToLower();
                if (createNewDataContext)
                {
                    dataContext = DataContextProvider.Current.Provide(true);
                }

                var nonWildIdentifiers =
                    dataContext.AssociationsTenantUserUserRole
                               .Include(a => a.Tenant)
                               .ToList()
                               .Select(a => a.Id);
                var wildCardTenants =
                    dataContext.AssociationsTenantUserUserRole.Where(
                    // ReSharper disable ImplicitlyCapturedClosure
                        a => a.User.Username.ToLower() == lowerUsername && !nonWildIdentifiers.Contains(a.Id))
                    // ReSharper restore ImplicitlyCapturedClosure
                               .ToList()
                               .Select(a => a.Id)
                               .ToList();
                var hasWildCard =
                    dataContext.AssociationsPermissionDataScopeUserRole.Any(
                        ar => ar.UserRole.AssociationTenantUserUserRoles.Any(a => wildCardTenants.Contains(a.Id)));
                if (hasWildCard)
                {
                    if (createNewDataContext)
                    {
                        dataContext.Dispose();
                    }

                    return query;
                }

                var tenants =
                    dataContext.Tenants.Where(
                    // ReSharper disable ImplicitlyCapturedClosure
                        tenant =>
                            // ReSharper restore ImplicitlyCapturedClosure
                        tenant.AssociationTenantUserUserRoles.Any(
                            a =>
                            a.User.Username.ToLower() == lowerUsername
                            && a.UserRole.AssociationPermissionDataScopeUserRoles.Any()))
                               .Select(tenant => tenant.Id)
                               .ToList();
                var tenantIdentifiers = tenants.Distinct().ToList();

                query =
                    query.Where(
                        tenant =>
                        tenantIdentifiers.Contains(tenant.Id));
                if (createNewDataContext)
                {
                    dataContext.Dispose();
                }

                return query;
            }

            private static IQueryable<User> ExtendQuery(
                IQueryable<User> query,
                OperationPermission permission,
                IDataContext dataContext = null)
            {
                var createNewDataContext = dataContext == null;
                var lowerPermission = permission.ToString().ToLower();
                var lowerDataScope = DataScopes.Membership.User.ToLower();
                var lowerUsername = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name.ToLower();
                if (createNewDataContext)
                {
                    dataContext = DataContextProvider.Current.Provide(true);
                }

                var nonWildIdentifiers =
                    dataContext.AssociationsTenantUserUserRole
                               .Include(a => a.Tenant)
                               .ToList()
                               .Select(a => a.Id);
                var wildCardTenants =
                    dataContext.AssociationsTenantUserUserRole.Where(
                    // ReSharper disable ImplicitlyCapturedClosure
                        a => a.User.Username.ToLower() == lowerUsername && !nonWildIdentifiers.Contains(a.Id))
                    // ReSharper restore ImplicitlyCapturedClosure
                               .ToList()
                               .Select(a => a.Id)
                               .ToList();
                var hasWildCard =
                    dataContext.AssociationsPermissionDataScopeUserRole.Any(
                        ar =>
                        ar.Permission.Name.ToLower() == lowerPermission && ar.DataScope.Name == lowerDataScope
                        && ar.UserRole.AssociationTenantUserUserRoles.Any(a => wildCardTenants.Contains(a.Id)));
                if (hasWildCard)
                {
                    if (createNewDataContext)
                    {
                        dataContext.Dispose();
                    }

                    return query;
                }

                var tenants =
                    dataContext.Tenants.Where(
                    // ReSharper disable ImplicitlyCapturedClosure
                        tenant =>
                            // ReSharper restore ImplicitlyCapturedClosure
                        tenant.AssociationTenantUserUserRoles.Any(
                            a =>
                            a.User.Username.ToLower() == lowerUsername
                            && a.UserRole.AssociationPermissionDataScopeUserRoles.Any(
                                ar =>
                                ar.Permission.Name.ToLower() == lowerPermission
                                    && ar.DataScope.Name == lowerDataScope)))
                               .Select(tenant => tenant.Id)
                               .ToList();
                var tenantIdentifiers = tenants.Distinct().ToList();

                query =
                    query.Where(
                        user =>
                        tenantIdentifiers.Contains(user.OwnerTenantId)
                        || user.AssociationTenantUserUserRoles.Any(
                            a => a.TenantId.HasValue && tenantIdentifiers.Contains(a.TenantId.Value)));
                if (createNewDataContext)
                {
                    dataContext.Dispose();
                }

                return query;
            }

            private static RepositoryContext<IAssociationPermissionDataScopeUserRoleRepository>
                CreateAssociationPermissionDataScopeUserRoleRepository()
            {
                var dataContext = DataContextProvider.Current.Provide(true);
                var repository =
                    new AssociationPermissionDataScopeUserRoleRepository(
                        dataContext.AssociationsPermissionDataScopeUserRole);
                return
                    new RepositoryContext<IAssociationPermissionDataScopeUserRoleRepository>(dataContext, repository);
            }

            private AuthorizationToken GetAuthorizationToken(Tenant tenant, IEnumerable<Authorization> associations)
            {
                var list = associations.ToList();
                var tenantAuthorization = new AuthorizationToken { Tenant = tenant };

                var query = from a in list
                            group a by a.DataScope into g
                            select new { DataScope = g.Key, Items = g.ToList() };
                foreach (var item in query)
                {
                    tenantAuthorization.Permissions.Add(item.DataScope, item.Items.Select(a => a.Permission).ToList());
                }

                return tenantAuthorization;
            }*/
        }
    }
}