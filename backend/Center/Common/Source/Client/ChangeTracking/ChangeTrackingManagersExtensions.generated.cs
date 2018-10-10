namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NLog;
    
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;

    namespace AccessControl
    {
        public partial class UserRoleChangeTrackingManager
        {
            public async Task<UserRoleReadableModel> GetAsync(int id)
            {
                InternalUserRoleReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UserRoleQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UserRoleReadableModel Wrap(UserRole entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UserRoleReadableModel Wrap(UserRole entity, bool containsAllReferences)
            {
                InternalUserRoleReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUserRoleReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillAuthorizationsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                return model;
            }

            private async Task<UserRole> QueryEntityAsync(UserRoleQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UserRole>> QueryEntitiesAsync(UserRoleQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            // The entity doesn't have reference properties. Omitting the FillReferencePropertiesAsync method.

            private async Task FillNavigationPropertiesAsync(InternalUserRoleReadableModel model)
            {
                var query = UserRoleQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");

                // The entity doesn't have reference properties. Omitting the call to FillReferenceProperties
                this.FillAuthorizationsCollectionProperty(model, entity);
            }

            private void FillAuthorizationsCollectionProperty(InternalUserRoleReadableModel model, UserRole entity)
            {
                if (entity.Authorizations == null)
                {
                    return;
                }

                model.FillAuthorizations(entity.Authorizations);
            }

            private class InternalUserRoleReadableModel : UserRoleReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UserRoleChangeTrackingManager manager;
                private volatile bool propertyAuthorizationsLoaded;

                public InternalUserRoleReadableModel(UserRoleChangeTrackingManager manager, UserRole entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyAuthorizationsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal UserRole Entity
                {
                    get
                    {
                        return this.UserRole;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override IObservableReadOnlyCollection<AuthorizationReadableModel> Authorizations
                {
                    get
                    {
                        if (!this.propertyAuthorizationsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Authorizations not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Authorizations;
                    }
                }

                public override async Task ApplyAsync(AuthorizationDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Authorizations not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    await Task.FromResult(0);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {

                    // The entity doesn't have reference properties. Omitting call to LoadReferencePropertiesAsync();	

                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void FillAuthorizations(IEnumerable<Authorization> entities)
                {
                    if (this.propertyAuthorizationsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyAuthorizationsLoaded)
                        {
                            return;
                        }

                        this.propertyAuthorizationsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Authorizations'");
                    var manager = DependencyResolver.Current.Get<IAuthorizationChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Authorizations'", items.Count);
                    foreach (var item in items)
                    {
                        this.authorizations.Add(item);
                    }
                }

            }
        } // UserRoleChangeTrackingManager

        internal static class UserRoleReadableModelManagerExtension
        {
            public static UserRoleQuery IncludeReferences(this UserRoleQuery query)
            {
                return query;
            }

            public static UserRoleQuery IncludeNavigationProperties(this UserRoleQuery query)
            {
                return query.IncludeReferences().IncludeAuthorizations(query.Authorizations);
            }

            public static UserRoleQuery IncludeXmlProperties(this UserRoleQuery query)
            {
                return query;
            }
        }

        public partial class AuthorizationChangeTrackingManager
        {
            public async Task<AuthorizationReadableModel> GetAsync(int id)
            {
                InternalAuthorizationReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = AuthorizationQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public AuthorizationReadableModel Wrap(Authorization entity)
            {
                return this.Wrap(entity, false);
            }
            
            private AuthorizationReadableModel Wrap(Authorization entity, bool containsAllReferences)
            {
                InternalAuthorizationReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalAuthorizationReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.UserRole != null)
                {
                    var changeTrackingManagerUserRole =
                        DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                    var modelUserRole = changeTrackingManagerUserRole.Wrap(entity.UserRole);
                    model.SetUserRole(modelUserRole);
                }
                else if (containsAllReferences)
                {
                    model.SetUserRole(null);
                }

                return model;
            }

            private async Task<Authorization> QueryEntityAsync(AuthorizationQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Authorization>> QueryEntitiesAsync(AuthorizationQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalAuthorizationReadableModel model)
            {
                var query = AuthorizationQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalAuthorizationReadableModel model, Authorization entity)
            {
                this.FillUserRoleReferenceProperty(model, entity);
            }

            private void FillUserRoleReferenceProperty(InternalAuthorizationReadableModel model, Authorization entity)
            {
                if (entity.UserRole == null)
                {
                    model.SetUserRole(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                var reference = manager.Wrap(entity.UserRole);
                model.SetUserRole(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalAuthorizationReadableModel model)
            {
                var query = AuthorizationQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalAuthorizationReadableModel : AuthorizationReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly AuthorizationChangeTrackingManager manager;
                private volatile bool propertyUserRoleLoaded;

                public InternalAuthorizationReadableModel(AuthorizationChangeTrackingManager manager, Authorization entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUserRoleLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Authorization Entity
                {
                    get
                    {
                        return this.Authorization;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UserRoleReadableModel UserRole
                {
                    get
                    {
                        if (!this.propertyUserRoleLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UserRole not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UserRole;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetUserRole(UserRoleReadableModel model)
                {
                    if (this.propertyUserRoleLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUserRoleLoaded)
                        {
                            return;
                        }

                        this.propertyUserRoleLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UserRole' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UserRole = model;
                }
            }
        } // AuthorizationChangeTrackingManager

        internal static class AuthorizationReadableModelManagerExtension
        {
            public static AuthorizationQuery IncludeReferences(this AuthorizationQuery query)
            {
                return query.IncludeUserRole(query.UserRole);
            }

            public static AuthorizationQuery IncludeNavigationProperties(this AuthorizationQuery query)
            {
                return query.IncludeReferences();
            }

            public static AuthorizationQuery IncludeXmlProperties(this AuthorizationQuery query)
            {
                return query;
            }
        }
	
    }	

    namespace Membership
    {
        public partial class TenantChangeTrackingManager
        {
            public async Task<TenantReadableModel> GetAsync(int id)
            {
                InternalTenantReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = TenantQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public TenantReadableModel Wrap(Tenant entity)
            {
                return this.Wrap(entity, false);
            }
            
            private TenantReadableModel Wrap(Tenant entity, bool containsAllReferences)
            {
                InternalTenantReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalTenantReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUsersCollectionProperty(model, entity);
                    this.FillUpdateGroupsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                return model;
            }

            private async Task<Tenant> QueryEntityAsync(TenantQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Tenant>> QueryEntitiesAsync(TenantQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            // The entity doesn't have reference properties. Omitting the FillReferencePropertiesAsync method.

            private async Task FillNavigationPropertiesAsync(InternalTenantReadableModel model)
            {
                var query = TenantQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");

                // The entity doesn't have reference properties. Omitting the call to FillReferenceProperties
                this.FillUsersCollectionProperty(model, entity);
                this.FillUpdateGroupsCollectionProperty(model, entity);
            }

            private void FillUsersCollectionProperty(InternalTenantReadableModel model, Tenant entity)
            {
                if (entity.Users == null)
                {
                    return;
                }

                model.FillUsers(entity.Users);
            }

            private void FillUpdateGroupsCollectionProperty(InternalTenantReadableModel model, Tenant entity)
            {
                if (entity.UpdateGroups == null)
                {
                    return;
                }

                model.FillUpdateGroups(entity.UpdateGroups);
            }

            private class InternalTenantReadableModel : TenantReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly TenantChangeTrackingManager manager;
                private volatile bool propertyUsersLoaded;
                private volatile bool propertyUpdateGroupsLoaded;

                public InternalTenantReadableModel(TenantChangeTrackingManager manager, Tenant entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUsersLoaded
                            && this.propertyUpdateGroupsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Tenant Entity
                {
                    get
                    {
                        return this.Tenant;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override IObservableReadOnlyCollection<UserReadableModel> Users
                {
                    get
                    {
                        if (!this.propertyUsersLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Users not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Users;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateGroupReadableModel> UpdateGroups
                {
                    get
                    {
                        if (!this.propertyUpdateGroupsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property UpdateGroups not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.UpdateGroups;
                    }
                }

                public override async Task ApplyAsync(UserDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Users not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task ApplyAsync(UpdateGroupDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("UpdateGroups not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    await Task.FromResult(0);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {

                    // The entity doesn't have reference properties. Omitting call to LoadReferencePropertiesAsync();	

                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void FillUsers(IEnumerable<User> entities)
                {
                    if (this.propertyUsersLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUsersLoaded)
                        {
                            return;
                        }

                        this.propertyUsersLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Users'");
                    var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Users'", items.Count);
                    foreach (var item in items)
                    {
                        this.users.Add(item);
                    }
                }


                public void FillUpdateGroups(IEnumerable<UpdateGroup> entities)
                {
                    if (this.propertyUpdateGroupsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateGroupsLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateGroupsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'UpdateGroups'");
                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'UpdateGroups'", items.Count);
                    foreach (var item in items)
                    {
                        this.updateGroups.Add(item);
                    }
                }

            }
        } // TenantChangeTrackingManager

        internal static class TenantReadableModelManagerExtension
        {
            public static TenantQuery IncludeReferences(this TenantQuery query)
            {
                return query;
            }

            public static TenantQuery IncludeNavigationProperties(this TenantQuery query)
            {
                return query.IncludeReferences().IncludeUsers(query.Users).IncludeUpdateGroups(query.UpdateGroups);
            }

            public static TenantQuery IncludeXmlProperties(this TenantQuery query)
            {
                return query;
            }
        }

        public partial class UserChangeTrackingManager
        {
            public async Task<UserReadableModel> GetAsync(int id)
            {
                InternalUserReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UserQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UserReadableModel Wrap(User entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UserReadableModel Wrap(User entity, bool containsAllReferences)
            {
                InternalUserReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUserReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillAssociationTenantUserUserRolesCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.OwnerTenant != null)
                {
                    var changeTrackingManagerOwnerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelOwnerTenant = changeTrackingManagerOwnerTenant.Wrap(entity.OwnerTenant);
                    model.SetOwnerTenant(modelOwnerTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetOwnerTenant(null);
                }

                return model;
            }

            private async Task<User> QueryEntityAsync(UserQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<User>> QueryEntitiesAsync(UserQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalUserReadableModel model)
            {
                var query = UserQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUserReadableModel model, User entity)
            {
                this.FillOwnerTenantReferenceProperty(model, entity);
            }

            private void FillOwnerTenantReferenceProperty(InternalUserReadableModel model, User entity)
            {
                if (entity.OwnerTenant == null)
                {
                    model.SetOwnerTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.OwnerTenant);
                model.SetOwnerTenant(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUserReadableModel model)
            {
                var query = UserQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillAssociationTenantUserUserRolesCollectionProperty(model, entity);
            }

            private void FillAssociationTenantUserUserRolesCollectionProperty(InternalUserReadableModel model, User entity)
            {
                if (entity.AssociationTenantUserUserRoles == null)
                {
                    return;
                }

                model.FillAssociationTenantUserUserRoles(entity.AssociationTenantUserUserRoles);
            }

            private class InternalUserReadableModel : UserReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UserChangeTrackingManager manager;
                private volatile bool propertyOwnerTenantLoaded;
                private volatile bool propertyAssociationTenantUserUserRolesLoaded;

                public InternalUserReadableModel(UserChangeTrackingManager manager, User entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyOwnerTenantLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyAssociationTenantUserUserRolesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal User Entity
                {
                    get
                    {
                        return this.User;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel OwnerTenant
                {
                    get
                    {
                        if (!this.propertyOwnerTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property OwnerTenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.OwnerTenant;
                    }
                }

                public override IObservableReadOnlyCollection<AssociationTenantUserUserRoleReadableModel> AssociationTenantUserUserRoles
                {
                    get
                    {
                        if (!this.propertyAssociationTenantUserUserRolesLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property AssociationTenantUserUserRoles not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.AssociationTenantUserUserRoles;
                    }
                }

                public override async Task ApplyAsync(AssociationTenantUserUserRoleDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("AssociationTenantUserUserRoles not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetOwnerTenant(TenantReadableModel model)
                {
                    if (this.propertyOwnerTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyOwnerTenantLoaded)
                        {
                            return;
                        }

                        this.propertyOwnerTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'OwnerTenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.OwnerTenant = model;
                }

                public void FillAssociationTenantUserUserRoles(IEnumerable<AssociationTenantUserUserRole> entities)
                {
                    if (this.propertyAssociationTenantUserUserRolesLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyAssociationTenantUserUserRolesLoaded)
                        {
                            return;
                        }

                        this.propertyAssociationTenantUserUserRolesLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'AssociationTenantUserUserRoles'");
                    var manager = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'AssociationTenantUserUserRoles'", items.Count);
                    foreach (var item in items)
                    {
                        this.associationTenantUserUserRoles.Add(item);
                    }
                }

            }
        } // UserChangeTrackingManager

        internal static class UserReadableModelManagerExtension
        {
            public static UserQuery IncludeReferences(this UserQuery query)
            {
                return query.IncludeOwnerTenant(query.OwnerTenant);
            }

            public static UserQuery IncludeNavigationProperties(this UserQuery query)
            {
                return query.IncludeReferences().IncludeAssociationTenantUserUserRoles(query.AssociationTenantUserUserRoles);
            }

            public static UserQuery IncludeXmlProperties(this UserQuery query)
            {
                return query;
            }
        }

        public partial class AssociationTenantUserUserRoleChangeTrackingManager
        {
            public async Task<AssociationTenantUserUserRoleReadableModel> GetAsync(int id)
            {
                InternalAssociationTenantUserUserRoleReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = AssociationTenantUserUserRoleQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public AssociationTenantUserUserRoleReadableModel Wrap(AssociationTenantUserUserRole entity)
            {
                return this.Wrap(entity, false);
            }
            
            private AssociationTenantUserUserRoleReadableModel Wrap(AssociationTenantUserUserRole entity, bool containsAllReferences)
            {
                InternalAssociationTenantUserUserRoleReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalAssociationTenantUserUserRoleReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Tenant != null)
                {
                    var changeTrackingManagerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelTenant = changeTrackingManagerTenant.Wrap(entity.Tenant);
                    model.SetTenant(modelTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetTenant(null);
                }

                if (entity.User != null)
                {
                    var changeTrackingManagerUser =
                        DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var modelUser = changeTrackingManagerUser.Wrap(entity.User);
                    model.SetUser(modelUser);
                }
                else if (containsAllReferences)
                {
                    model.SetUser(null);
                }

                if (entity.UserRole != null)
                {
                    var changeTrackingManagerUserRole =
                        DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                    var modelUserRole = changeTrackingManagerUserRole.Wrap(entity.UserRole);
                    model.SetUserRole(modelUserRole);
                }
                else if (containsAllReferences)
                {
                    model.SetUserRole(null);
                }

                return model;
            }

            private async Task<AssociationTenantUserUserRole> QueryEntityAsync(AssociationTenantUserUserRoleQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<AssociationTenantUserUserRole>> QueryEntitiesAsync(AssociationTenantUserUserRoleQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalAssociationTenantUserUserRoleReadableModel model)
            {
                var query = AssociationTenantUserUserRoleQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalAssociationTenantUserUserRoleReadableModel model, AssociationTenantUserUserRole entity)
            {
                this.FillTenantReferenceProperty(model, entity);
                this.FillUserReferenceProperty(model, entity);
                this.FillUserRoleReferenceProperty(model, entity);
            }

            private void FillTenantReferenceProperty(InternalAssociationTenantUserUserRoleReadableModel model, AssociationTenantUserUserRole entity)
            {
                if (entity.Tenant == null)
                {
                    model.SetTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.Tenant);
                model.SetTenant(reference);
            }

            private void FillUserReferenceProperty(InternalAssociationTenantUserUserRoleReadableModel model, AssociationTenantUserUserRole entity)
            {
                if (entity.User == null)
                {
                    model.SetUser(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                var reference = manager.Wrap(entity.User);
                model.SetUser(reference);
            }

            private void FillUserRoleReferenceProperty(InternalAssociationTenantUserUserRoleReadableModel model, AssociationTenantUserUserRole entity)
            {
                if (entity.UserRole == null)
                {
                    model.SetUserRole(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                var reference = manager.Wrap(entity.UserRole);
                model.SetUserRole(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalAssociationTenantUserUserRoleReadableModel model)
            {
                var query = AssociationTenantUserUserRoleQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalAssociationTenantUserUserRoleReadableModel : AssociationTenantUserUserRoleReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly AssociationTenantUserUserRoleChangeTrackingManager manager;
                private volatile bool propertyTenantLoaded;
                private volatile bool propertyUserLoaded;
                private volatile bool propertyUserRoleLoaded;

                public InternalAssociationTenantUserUserRoleReadableModel(AssociationTenantUserUserRoleChangeTrackingManager manager, AssociationTenantUserUserRole entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyTenantLoaded
                            && this.propertyUserLoaded
                            && this.propertyUserRoleLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal AssociationTenantUserUserRole Entity
                {
                    get
                    {
                        return this.AssociationTenantUserUserRole;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel Tenant
                {
                    get
                    {
                        if (!this.propertyTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Tenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Tenant;
                    }
                }

                public override UserReadableModel User
                {
                    get
                    {
                        if (!this.propertyUserLoaded)
                        {
                            throw new ChangeTrackingException("Reference property User not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.User;
                    }
                }

                public override UserRoleReadableModel UserRole
                {
                    get
                    {
                        if (!this.propertyUserRoleLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UserRole not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UserRole;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetTenant(TenantReadableModel model)
                {
                    if (this.propertyTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyTenantLoaded)
                        {
                            return;
                        }

                        this.propertyTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Tenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Tenant = model;
                }

                public void SetUser(UserReadableModel model)
                {
                    if (this.propertyUserLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUserLoaded)
                        {
                            return;
                        }

                        this.propertyUserLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'User' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.User = model;
                }

                public void SetUserRole(UserRoleReadableModel model)
                {
                    if (this.propertyUserRoleLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUserRoleLoaded)
                        {
                            return;
                        }

                        this.propertyUserRoleLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UserRole' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UserRole = model;
                }
            }
        } // AssociationTenantUserUserRoleChangeTrackingManager

        internal static class AssociationTenantUserUserRoleReadableModelManagerExtension
        {
            public static AssociationTenantUserUserRoleQuery IncludeReferences(this AssociationTenantUserUserRoleQuery query)
            {
                return query.IncludeTenant(query.Tenant).IncludeUser(query.User).IncludeUserRole(query.UserRole);
            }

            public static AssociationTenantUserUserRoleQuery IncludeNavigationProperties(this AssociationTenantUserUserRoleQuery query)
            {
                return query.IncludeReferences();
            }

            public static AssociationTenantUserUserRoleQuery IncludeXmlProperties(this AssociationTenantUserUserRoleQuery query)
            {
                return query;
            }
        }
	
    }	

    namespace Units
    {
        public partial class ProductTypeChangeTrackingManager
        {
            public async Task<ProductTypeReadableModel> GetAsync(int id)
            {
                InternalProductTypeReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = ProductTypeQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public ProductTypeReadableModel Wrap(ProductType entity)
            {
                return this.Wrap(entity, false);
            }
            
            private ProductTypeReadableModel Wrap(ProductType entity, bool containsAllReferences)
            {
                InternalProductTypeReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalProductTypeReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.HardwareDescriptorXml != null)
                    {
                        model.SetHardwareDescriptor(entity.HardwareDescriptor);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUnitsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                return model;
            }

            private async Task<ProductType> QueryEntityAsync(ProductTypeQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<ProductType>> QueryEntitiesAsync(ProductTypeQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalProductTypeReadableModel model)
            {
                var query = ProductTypeQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetHardwareDescriptor(entity.HardwareDescriptor);
            }

            // The entity doesn't have reference properties. Omitting the FillReferencePropertiesAsync method.

            private async Task FillNavigationPropertiesAsync(InternalProductTypeReadableModel model)
            {
                var query = ProductTypeQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");

                // The entity doesn't have reference properties. Omitting the call to FillReferenceProperties
                this.FillUnitsCollectionProperty(model, entity);
            }

            private void FillUnitsCollectionProperty(InternalProductTypeReadableModel model, ProductType entity)
            {
                if (entity.Units == null)
                {
                    return;
                }

                model.FillUnits(entity.Units);
            }

            private class InternalProductTypeReadableModel : ProductTypeReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly ProductTypeChangeTrackingManager manager;
                private volatile bool propertyUnitsLoaded;
                private volatile bool propertyHardwareDescriptorLoaded;

                public InternalProductTypeReadableModel(ProductTypeChangeTrackingManager manager, ProductType entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUnitsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyHardwareDescriptorLoaded;
                    }
                }

                internal ProductType Entity
                {
                    get
                    {
                        return this.ProductType;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override IObservableReadOnlyCollection<UnitReadableModel> Units
                {
                    get
                    {
                        if (!this.propertyUnitsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Units not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Units;
                    }
                }

                public override XmlData HardwareDescriptor
                {
                    get
                    {
                        if (!this.propertyHardwareDescriptorLoaded)
                        {
                            throw new ChangeTrackingException("XML property HardwareDescriptor not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.HardwareDescriptor;
                    }
                }

                public override async Task ApplyAsync(UnitDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Units not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    await Task.FromResult(0);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {

                    // The entity doesn't have reference properties. Omitting call to LoadReferencePropertiesAsync();	

                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void FillUnits(IEnumerable<Unit> entities)
                {
                    if (this.propertyUnitsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUnitsLoaded)
                        {
                            return;
                        }

                        this.propertyUnitsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Units'");
                    var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Units'", items.Count);
                    foreach (var item in items)
                    {
                        this.units.Add(item);
                    }
                }


                public void SetHardwareDescriptor(XmlData xmlData)
                {
                    if (this.propertyHardwareDescriptorLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyHardwareDescriptorLoaded)
                        {
                            return;
                        }

                        this.propertyHardwareDescriptorLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'HardwareDescriptor' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.HardwareDescriptor = xmlData;
                }
            }
        } // ProductTypeChangeTrackingManager

        internal static class ProductTypeReadableModelManagerExtension
        {
            public static ProductTypeQuery IncludeReferences(this ProductTypeQuery query)
            {
                return query;
            }

            public static ProductTypeQuery IncludeNavigationProperties(this ProductTypeQuery query)
            {
                return query.IncludeReferences().IncludeUnits(query.Units);
            }

            public static ProductTypeQuery IncludeXmlProperties(this ProductTypeQuery query)
            {
                return query.IncludeHardwareDescriptor();
            }
        }

        public partial class UnitChangeTrackingManager
        {
            public async Task<UnitReadableModel> GetAsync(int id)
            {
                InternalUnitReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UnitQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UnitReadableModel Wrap(Unit entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UnitReadableModel Wrap(Unit entity, bool containsAllReferences)
            {
                InternalUnitReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUnitReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUpdateCommandsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Tenant != null)
                {
                    var changeTrackingManagerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelTenant = changeTrackingManagerTenant.Wrap(entity.Tenant);
                    model.SetTenant(modelTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetTenant(null);
                }

                if (entity.ProductType != null)
                {
                    var changeTrackingManagerProductType =
                        DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                    var modelProductType = changeTrackingManagerProductType.Wrap(entity.ProductType);
                    model.SetProductType(modelProductType);
                }
                else if (containsAllReferences)
                {
                    model.SetProductType(null);
                }

                if (entity.UpdateGroup != null)
                {
                    var changeTrackingManagerUpdateGroup =
                        DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var modelUpdateGroup = changeTrackingManagerUpdateGroup.Wrap(entity.UpdateGroup);
                    model.SetUpdateGroup(modelUpdateGroup);
                }
                else if (containsAllReferences)
                {
                    model.SetUpdateGroup(null);
                }

                return model;
            }

            private async Task<Unit> QueryEntityAsync(UnitQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Unit>> QueryEntitiesAsync(UnitQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalUnitReadableModel model)
            {
                var query = UnitQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUnitReadableModel model, Unit entity)
            {
                this.FillTenantReferenceProperty(model, entity);
                this.FillProductTypeReferenceProperty(model, entity);
                this.FillUpdateGroupReferenceProperty(model, entity);
            }

            private void FillTenantReferenceProperty(InternalUnitReadableModel model, Unit entity)
            {
                if (entity.Tenant == null)
                {
                    model.SetTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.Tenant);
                model.SetTenant(reference);
            }

            private void FillProductTypeReferenceProperty(InternalUnitReadableModel model, Unit entity)
            {
                if (entity.ProductType == null)
                {
                    model.SetProductType(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                var reference = manager.Wrap(entity.ProductType);
                model.SetProductType(reference);
            }

            private void FillUpdateGroupReferenceProperty(InternalUnitReadableModel model, Unit entity)
            {
                if (entity.UpdateGroup == null)
                {
                    model.SetUpdateGroup(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                var reference = manager.Wrap(entity.UpdateGroup);
                model.SetUpdateGroup(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUnitReadableModel model)
            {
                var query = UnitQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillUpdateCommandsCollectionProperty(model, entity);
            }

            private void FillUpdateCommandsCollectionProperty(InternalUnitReadableModel model, Unit entity)
            {
                if (entity.UpdateCommands == null)
                {
                    return;
                }

                model.FillUpdateCommands(entity.UpdateCommands);
            }

            private class InternalUnitReadableModel : UnitReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UnitChangeTrackingManager manager;
                private volatile bool propertyTenantLoaded;
                private volatile bool propertyProductTypeLoaded;
                private volatile bool propertyUpdateGroupLoaded;
                private volatile bool propertyUpdateCommandsLoaded;

                public InternalUnitReadableModel(UnitChangeTrackingManager manager, Unit entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyTenantLoaded
                            && this.propertyProductTypeLoaded
                            && this.propertyUpdateGroupLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUpdateCommandsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Unit Entity
                {
                    get
                    {
                        return this.Unit;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel Tenant
                {
                    get
                    {
                        if (!this.propertyTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Tenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Tenant;
                    }
                }

                public override ProductTypeReadableModel ProductType
                {
                    get
                    {
                        if (!this.propertyProductTypeLoaded)
                        {
                            throw new ChangeTrackingException("Reference property ProductType not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.ProductType;
                    }
                }

                public override UpdateGroupReadableModel UpdateGroup
                {
                    get
                    {
                        if (!this.propertyUpdateGroupLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UpdateGroup not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UpdateGroup;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateCommandReadableModel> UpdateCommands
                {
                    get
                    {
                        if (!this.propertyUpdateCommandsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property UpdateCommands not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.UpdateCommands;
                    }
                }

                public override async Task ApplyAsync(UpdateCommandDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("UpdateCommands not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetTenant(TenantReadableModel model)
                {
                    if (this.propertyTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyTenantLoaded)
                        {
                            return;
                        }

                        this.propertyTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Tenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Tenant = model;
                }

                public void SetProductType(ProductTypeReadableModel model)
                {
                    if (this.propertyProductTypeLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyProductTypeLoaded)
                        {
                            return;
                        }

                        this.propertyProductTypeLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'ProductType' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.ProductType = model;
                }

                public void SetUpdateGroup(UpdateGroupReadableModel model)
                {
                    if (this.propertyUpdateGroupLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateGroupLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateGroupLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UpdateGroup' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UpdateGroup = model;
                }

                public void FillUpdateCommands(IEnumerable<UpdateCommand> entities)
                {
                    if (this.propertyUpdateCommandsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateCommandsLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateCommandsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'UpdateCommands'");
                    var manager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'UpdateCommands'", items.Count);
                    foreach (var item in items)
                    {
                        this.updateCommands.Add(item);
                    }
                }

            }
        } // UnitChangeTrackingManager

        internal static class UnitReadableModelManagerExtension
        {
            public static UnitQuery IncludeReferences(this UnitQuery query)
            {
                return query.IncludeTenant(query.Tenant).IncludeProductType(query.ProductType).IncludeUpdateGroup(query.UpdateGroup);
            }

            public static UnitQuery IncludeNavigationProperties(this UnitQuery query)
            {
                return query.IncludeReferences().IncludeUpdateCommands(query.UpdateCommands);
            }

            public static UnitQuery IncludeXmlProperties(this UnitQuery query)
            {
                return query;
            }
        }
	
    }	

    namespace Resources
    {
        public partial class ResourceChangeTrackingManager
        {
            public async Task<ResourceReadableModel> GetAsync(int id)
            {
                InternalResourceReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = ResourceQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public ResourceReadableModel Wrap(Resource entity)
            {
                return this.Wrap(entity, false);
            }
            
            private ResourceReadableModel Wrap(Resource entity, bool containsAllReferences)
            {
                InternalResourceReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalResourceReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.UploadingUser != null)
                {
                    var changeTrackingManagerUploadingUser =
                        DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var modelUploadingUser = changeTrackingManagerUploadingUser.Wrap(entity.UploadingUser);
                    model.SetUploadingUser(modelUploadingUser);
                }
                else if (containsAllReferences)
                {
                    model.SetUploadingUser(null);
                }

                return model;
            }

            private async Task<Resource> QueryEntityAsync(ResourceQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Resource>> QueryEntitiesAsync(ResourceQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalResourceReadableModel model)
            {
                var query = ResourceQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalResourceReadableModel model, Resource entity)
            {
                this.FillUploadingUserReferenceProperty(model, entity);
            }

            private void FillUploadingUserReferenceProperty(InternalResourceReadableModel model, Resource entity)
            {
                if (entity.UploadingUser == null)
                {
                    model.SetUploadingUser(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                var reference = manager.Wrap(entity.UploadingUser);
                model.SetUploadingUser(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalResourceReadableModel model)
            {
                var query = ResourceQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalResourceReadableModel : ResourceReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly ResourceChangeTrackingManager manager;
                private volatile bool propertyUploadingUserLoaded;

                public InternalResourceReadableModel(ResourceChangeTrackingManager manager, Resource entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUploadingUserLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Resource Entity
                {
                    get
                    {
                        return this.Resource;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UserReadableModel UploadingUser
                {
                    get
                    {
                        if (!this.propertyUploadingUserLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UploadingUser not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UploadingUser;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetUploadingUser(UserReadableModel model)
                {
                    if (this.propertyUploadingUserLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUploadingUserLoaded)
                        {
                            return;
                        }

                        this.propertyUploadingUserLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UploadingUser' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UploadingUser = model;
                }
            }
        } // ResourceChangeTrackingManager

        internal static class ResourceReadableModelManagerExtension
        {
            public static ResourceQuery IncludeReferences(this ResourceQuery query)
            {
                return query.IncludeUploadingUser(query.UploadingUser);
            }

            public static ResourceQuery IncludeNavigationProperties(this ResourceQuery query)
            {
                return query.IncludeReferences();
            }

            public static ResourceQuery IncludeXmlProperties(this ResourceQuery query)
            {
                return query;
            }
        }

        public partial class ContentResourceChangeTrackingManager
        {
            public async Task<ContentResourceReadableModel> GetAsync(int id)
            {
                InternalContentResourceReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = ContentResourceQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public ContentResourceReadableModel Wrap(ContentResource entity)
            {
                return this.Wrap(entity, false);
            }
            
            private ContentResourceReadableModel Wrap(ContentResource entity, bool containsAllReferences)
            {
                InternalContentResourceReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalContentResourceReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.UploadingUser != null)
                {
                    var changeTrackingManagerUploadingUser =
                        DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var modelUploadingUser = changeTrackingManagerUploadingUser.Wrap(entity.UploadingUser);
                    model.SetUploadingUser(modelUploadingUser);
                }
                else if (containsAllReferences)
                {
                    model.SetUploadingUser(null);
                }

                return model;
            }

            private async Task<ContentResource> QueryEntityAsync(ContentResourceQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<ContentResource>> QueryEntitiesAsync(ContentResourceQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalContentResourceReadableModel model)
            {
                var query = ContentResourceQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalContentResourceReadableModel model, ContentResource entity)
            {
                this.FillUploadingUserReferenceProperty(model, entity);
            }

            private void FillUploadingUserReferenceProperty(InternalContentResourceReadableModel model, ContentResource entity)
            {
                if (entity.UploadingUser == null)
                {
                    model.SetUploadingUser(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                var reference = manager.Wrap(entity.UploadingUser);
                model.SetUploadingUser(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalContentResourceReadableModel model)
            {
                var query = ContentResourceQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalContentResourceReadableModel : ContentResourceReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly ContentResourceChangeTrackingManager manager;
                private volatile bool propertyUploadingUserLoaded;

                public InternalContentResourceReadableModel(ContentResourceChangeTrackingManager manager, ContentResource entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUploadingUserLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal ContentResource Entity
                {
                    get
                    {
                        return this.ContentResource;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UserReadableModel UploadingUser
                {
                    get
                    {
                        if (!this.propertyUploadingUserLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UploadingUser not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UploadingUser;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetUploadingUser(UserReadableModel model)
                {
                    if (this.propertyUploadingUserLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUploadingUserLoaded)
                        {
                            return;
                        }

                        this.propertyUploadingUserLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UploadingUser' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UploadingUser = model;
                }
            }
        } // ContentResourceChangeTrackingManager

        internal static class ContentResourceReadableModelManagerExtension
        {
            public static ContentResourceQuery IncludeReferences(this ContentResourceQuery query)
            {
                return query.IncludeUploadingUser(query.UploadingUser);
            }

            public static ContentResourceQuery IncludeNavigationProperties(this ContentResourceQuery query)
            {
                return query.IncludeReferences();
            }

            public static ContentResourceQuery IncludeXmlProperties(this ContentResourceQuery query)
            {
                return query;
            }
        }
	
    }	

    namespace Update
    {
        public partial class UpdateGroupChangeTrackingManager
        {
            public async Task<UpdateGroupReadableModel> GetAsync(int id)
            {
                InternalUpdateGroupReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UpdateGroupQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UpdateGroupReadableModel Wrap(UpdateGroup entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UpdateGroupReadableModel Wrap(UpdateGroup entity, bool containsAllReferences)
            {
                InternalUpdateGroupReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUpdateGroupReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUnitsCollectionProperty(model, entity);
                    this.FillUpdatePartsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Tenant != null)
                {
                    var changeTrackingManagerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelTenant = changeTrackingManagerTenant.Wrap(entity.Tenant);
                    model.SetTenant(modelTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetTenant(null);
                }

                if (entity.UnitConfiguration != null)
                {
                    var changeTrackingManagerUnitConfiguration =
                        DependencyResolver.Current.Get<IUnitConfigurationChangeTrackingManager>();
                    var modelUnitConfiguration = changeTrackingManagerUnitConfiguration.Wrap(entity.UnitConfiguration);
                    model.SetUnitConfiguration(modelUnitConfiguration);
                }
                else if (containsAllReferences)
                {
                    model.SetUnitConfiguration(null);
                }

                if (entity.MediaConfiguration != null)
                {
                    var changeTrackingManagerMediaConfiguration =
                        DependencyResolver.Current.Get<IMediaConfigurationChangeTrackingManager>();
                    var modelMediaConfiguration = changeTrackingManagerMediaConfiguration.Wrap(entity.MediaConfiguration);
                    model.SetMediaConfiguration(modelMediaConfiguration);
                }
                else if (containsAllReferences)
                {
                    model.SetMediaConfiguration(null);
                }

                return model;
            }

            private async Task<UpdateGroup> QueryEntityAsync(UpdateGroupQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UpdateGroup>> QueryEntitiesAsync(UpdateGroupQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalUpdateGroupReadableModel model)
            {
                var query = UpdateGroupQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                this.FillTenantReferenceProperty(model, entity);
                this.FillUnitConfigurationReferenceProperty(model, entity);
                this.FillMediaConfigurationReferenceProperty(model, entity);
            }

            private void FillTenantReferenceProperty(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                if (entity.Tenant == null)
                {
                    model.SetTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.Tenant);
                model.SetTenant(reference);
            }

            private void FillUnitConfigurationReferenceProperty(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                if (entity.UnitConfiguration == null)
                {
                    model.SetUnitConfiguration(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUnitConfigurationChangeTrackingManager>();
                var reference = manager.Wrap(entity.UnitConfiguration);
                model.SetUnitConfiguration(reference);
            }

            private void FillMediaConfigurationReferenceProperty(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                if (entity.MediaConfiguration == null)
                {
                    model.SetMediaConfiguration(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IMediaConfigurationChangeTrackingManager>();
                var reference = manager.Wrap(entity.MediaConfiguration);
                model.SetMediaConfiguration(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUpdateGroupReadableModel model)
            {
                var query = UpdateGroupQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillUnitsCollectionProperty(model, entity);
                this.FillUpdatePartsCollectionProperty(model, entity);
            }

            private void FillUnitsCollectionProperty(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                if (entity.Units == null)
                {
                    return;
                }

                model.FillUnits(entity.Units);
            }

            private void FillUpdatePartsCollectionProperty(InternalUpdateGroupReadableModel model, UpdateGroup entity)
            {
                if (entity.UpdateParts == null)
                {
                    return;
                }

                model.FillUpdateParts(entity.UpdateParts);
            }

            private class InternalUpdateGroupReadableModel : UpdateGroupReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UpdateGroupChangeTrackingManager manager;
                private volatile bool propertyTenantLoaded;
                private volatile bool propertyUnitConfigurationLoaded;
                private volatile bool propertyMediaConfigurationLoaded;
                private volatile bool propertyUnitsLoaded;
                private volatile bool propertyUpdatePartsLoaded;

                public InternalUpdateGroupReadableModel(UpdateGroupChangeTrackingManager manager, UpdateGroup entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyTenantLoaded
                            && this.propertyUnitConfigurationLoaded
                            && this.propertyMediaConfigurationLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUnitsLoaded
                            && this.propertyUpdatePartsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal UpdateGroup Entity
                {
                    get
                    {
                        return this.UpdateGroup;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel Tenant
                {
                    get
                    {
                        if (!this.propertyTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Tenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Tenant;
                    }
                }

                public override UnitConfigurationReadableModel UnitConfiguration
                {
                    get
                    {
                        if (!this.propertyUnitConfigurationLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UnitConfiguration not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UnitConfiguration;
                    }
                }

                public override MediaConfigurationReadableModel MediaConfiguration
                {
                    get
                    {
                        if (!this.propertyMediaConfigurationLoaded)
                        {
                            throw new ChangeTrackingException("Reference property MediaConfiguration not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.MediaConfiguration;
                    }
                }

                public override IObservableReadOnlyCollection<UnitReadableModel> Units
                {
                    get
                    {
                        if (!this.propertyUnitsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Units not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Units;
                    }
                }

                public override IObservableReadOnlyCollection<UpdatePartReadableModel> UpdateParts
                {
                    get
                    {
                        if (!this.propertyUpdatePartsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property UpdateParts not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.UpdateParts;
                    }
                }

                public override async Task ApplyAsync(UnitDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Units not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task ApplyAsync(UpdatePartDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("UpdateParts not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetTenant(TenantReadableModel model)
                {
                    if (this.propertyTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyTenantLoaded)
                        {
                            return;
                        }

                        this.propertyTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Tenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Tenant = model;
                }

                public void SetUnitConfiguration(UnitConfigurationReadableModel model)
                {
                    if (this.propertyUnitConfigurationLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUnitConfigurationLoaded)
                        {
                            return;
                        }

                        this.propertyUnitConfigurationLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UnitConfiguration' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UnitConfiguration = model;
                }

                public void SetMediaConfiguration(MediaConfigurationReadableModel model)
                {
                    if (this.propertyMediaConfigurationLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyMediaConfigurationLoaded)
                        {
                            return;
                        }

                        this.propertyMediaConfigurationLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'MediaConfiguration' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.MediaConfiguration = model;
                }

                public void FillUnits(IEnumerable<Unit> entities)
                {
                    if (this.propertyUnitsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUnitsLoaded)
                        {
                            return;
                        }

                        this.propertyUnitsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Units'");
                    var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Units'", items.Count);
                    foreach (var item in items)
                    {
                        this.units.Add(item);
                    }
                }


                public void FillUpdateParts(IEnumerable<UpdatePart> entities)
                {
                    if (this.propertyUpdatePartsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdatePartsLoaded)
                        {
                            return;
                        }

                        this.propertyUpdatePartsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'UpdateParts'");
                    var manager = DependencyResolver.Current.Get<IUpdatePartChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'UpdateParts'", items.Count);
                    foreach (var item in items)
                    {
                        this.updateParts.Add(item);
                    }
                }

            }
        } // UpdateGroupChangeTrackingManager

        internal static class UpdateGroupReadableModelManagerExtension
        {
            public static UpdateGroupQuery IncludeReferences(this UpdateGroupQuery query)
            {
                return query.IncludeTenant(query.Tenant).IncludeUnitConfiguration(query.UnitConfiguration).IncludeMediaConfiguration(query.MediaConfiguration);
            }

            public static UpdateGroupQuery IncludeNavigationProperties(this UpdateGroupQuery query)
            {
                return query.IncludeReferences().IncludeUnits(query.Units).IncludeUpdateParts(query.UpdateParts);
            }

            public static UpdateGroupQuery IncludeXmlProperties(this UpdateGroupQuery query)
            {
                return query;
            }
        }

        public partial class UpdatePartChangeTrackingManager
        {
            public async Task<UpdatePartReadableModel> GetAsync(int id)
            {
                InternalUpdatePartReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UpdatePartQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UpdatePartReadableModel Wrap(UpdatePart entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UpdatePartReadableModel Wrap(UpdatePart entity, bool containsAllReferences)
            {
                InternalUpdatePartReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUpdatePartReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.StructureXml != null)
                    {
                        model.SetStructure(entity.Structure);
                    }

                    if (entity.InstallInstructionsXml != null)
                    {
                        model.SetInstallInstructions(entity.InstallInstructions);
                    }

                    if (entity.DynamicContentXml != null)
                    {
                        model.SetDynamicContent(entity.DynamicContent);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillRelatedCommandsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.UpdateGroup != null)
                {
                    var changeTrackingManagerUpdateGroup =
                        DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var modelUpdateGroup = changeTrackingManagerUpdateGroup.Wrap(entity.UpdateGroup);
                    model.SetUpdateGroup(modelUpdateGroup);
                }
                else if (containsAllReferences)
                {
                    model.SetUpdateGroup(null);
                }

                return model;
            }

            private async Task<UpdatePart> QueryEntityAsync(UpdatePartQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UpdatePart>> QueryEntitiesAsync(UpdatePartQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalUpdatePartReadableModel model)
            {
                var query = UpdatePartQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetStructure(entity.Structure);

                model.SetInstallInstructions(entity.InstallInstructions);

                model.SetDynamicContent(entity.DynamicContent);
            }

            private async Task FillReferencePropertiesAsync(InternalUpdatePartReadableModel model)
            {
                var query = UpdatePartQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUpdatePartReadableModel model, UpdatePart entity)
            {
                this.FillUpdateGroupReferenceProperty(model, entity);
            }

            private void FillUpdateGroupReferenceProperty(InternalUpdatePartReadableModel model, UpdatePart entity)
            {
                if (entity.UpdateGroup == null)
                {
                    model.SetUpdateGroup(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                var reference = manager.Wrap(entity.UpdateGroup);
                model.SetUpdateGroup(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUpdatePartReadableModel model)
            {
                var query = UpdatePartQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillRelatedCommandsCollectionProperty(model, entity);
            }

            private void FillRelatedCommandsCollectionProperty(InternalUpdatePartReadableModel model, UpdatePart entity)
            {
                if (entity.RelatedCommands == null)
                {
                    return;
                }

                model.FillRelatedCommands(entity.RelatedCommands);
            }

            private class InternalUpdatePartReadableModel : UpdatePartReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UpdatePartChangeTrackingManager manager;
                private volatile bool propertyUpdateGroupLoaded;
                private volatile bool propertyRelatedCommandsLoaded;
                private volatile bool propertyStructureLoaded;
                private volatile bool propertyInstallInstructionsLoaded;
                private volatile bool propertyDynamicContentLoaded;

                public InternalUpdatePartReadableModel(UpdatePartChangeTrackingManager manager, UpdatePart entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUpdateGroupLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyRelatedCommandsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyStructureLoaded
                            && this.propertyInstallInstructionsLoaded
                            && this.propertyDynamicContentLoaded;
                    }
                }

                internal UpdatePart Entity
                {
                    get
                    {
                        return this.UpdatePart;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UpdateGroupReadableModel UpdateGroup
                {
                    get
                    {
                        if (!this.propertyUpdateGroupLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UpdateGroup not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UpdateGroup;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateCommandReadableModel> RelatedCommands
                {
                    get
                    {
                        if (!this.propertyRelatedCommandsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property RelatedCommands not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.RelatedCommands;
                    }
                }

                public override XmlData Structure
                {
                    get
                    {
                        if (!this.propertyStructureLoaded)
                        {
                            throw new ChangeTrackingException("XML property Structure not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Structure;
                    }
                }

                public override XmlData InstallInstructions
                {
                    get
                    {
                        if (!this.propertyInstallInstructionsLoaded)
                        {
                            throw new ChangeTrackingException("XML property InstallInstructions not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.InstallInstructions;
                    }
                }

                public override XmlData DynamicContent
                {
                    get
                    {
                        if (!this.propertyDynamicContentLoaded)
                        {
                            throw new ChangeTrackingException("XML property DynamicContent not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.DynamicContent;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetUpdateGroup(UpdateGroupReadableModel model)
                {
                    if (this.propertyUpdateGroupLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateGroupLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateGroupLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UpdateGroup' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UpdateGroup = model;
                }

                public void FillRelatedCommands(IEnumerable<UpdateCommand> entities)
                {
                    if (this.propertyRelatedCommandsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyRelatedCommandsLoaded)
                        {
                            return;
                        }

                        this.propertyRelatedCommandsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'RelatedCommands'");
                    var manager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'RelatedCommands'", items.Count);
                    foreach (var item in items)
                    {
                        this.relatedCommands.Add(item);
                    }
                }


                public void SetStructure(XmlData xmlData)
                {
                    if (this.propertyStructureLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyStructureLoaded)
                        {
                            return;
                        }

                        this.propertyStructureLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Structure' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Structure = xmlData;
                }

                public void SetInstallInstructions(XmlData xmlData)
                {
                    if (this.propertyInstallInstructionsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyInstallInstructionsLoaded)
                        {
                            return;
                        }

                        this.propertyInstallInstructionsLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'InstallInstructions' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.InstallInstructions = xmlData;
                }

                public void SetDynamicContent(XmlData xmlData)
                {
                    if (this.propertyDynamicContentLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyDynamicContentLoaded)
                        {
                            return;
                        }

                        this.propertyDynamicContentLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'DynamicContent' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.DynamicContent = xmlData;
                }
            }
        } // UpdatePartChangeTrackingManager

        internal static class UpdatePartReadableModelManagerExtension
        {
            public static UpdatePartQuery IncludeReferences(this UpdatePartQuery query)
            {
                return query.IncludeUpdateGroup(query.UpdateGroup);
            }

            public static UpdatePartQuery IncludeNavigationProperties(this UpdatePartQuery query)
            {
                return query.IncludeReferences().IncludeRelatedCommands(query.RelatedCommands);
            }

            public static UpdatePartQuery IncludeXmlProperties(this UpdatePartQuery query)
            {
                return query.IncludeStructure().IncludeInstallInstructions().IncludeDynamicContent();
            }
        }

        public partial class UpdateCommandChangeTrackingManager
        {
            public async Task<UpdateCommandReadableModel> GetAsync(int id)
            {
                InternalUpdateCommandReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UpdateCommandQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UpdateCommandReadableModel Wrap(UpdateCommand entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UpdateCommandReadableModel Wrap(UpdateCommand entity, bool containsAllReferences)
            {
                InternalUpdateCommandReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUpdateCommandReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.CommandXml != null)
                    {
                        model.SetCommand(entity.Command);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillIncludedPartsCollectionProperty(model, entity);
                    this.FillFeedbacksCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Unit != null)
                {
                    var changeTrackingManagerUnit =
                        DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                    var modelUnit = changeTrackingManagerUnit.Wrap(entity.Unit);
                    model.SetUnit(modelUnit);
                }
                else if (containsAllReferences)
                {
                    model.SetUnit(null);
                }

                return model;
            }

            private async Task<UpdateCommand> QueryEntityAsync(UpdateCommandQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UpdateCommand>> QueryEntitiesAsync(UpdateCommandQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalUpdateCommandReadableModel model)
            {
                var query = UpdateCommandQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetCommand(entity.Command);
            }

            private async Task FillReferencePropertiesAsync(InternalUpdateCommandReadableModel model)
            {
                var query = UpdateCommandQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUpdateCommandReadableModel model, UpdateCommand entity)
            {
                this.FillUnitReferenceProperty(model, entity);
            }

            private void FillUnitReferenceProperty(InternalUpdateCommandReadableModel model, UpdateCommand entity)
            {
                if (entity.Unit == null)
                {
                    model.SetUnit(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                var reference = manager.Wrap(entity.Unit);
                model.SetUnit(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUpdateCommandReadableModel model)
            {
                var query = UpdateCommandQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillIncludedPartsCollectionProperty(model, entity);
                this.FillFeedbacksCollectionProperty(model, entity);
            }

            private void FillIncludedPartsCollectionProperty(InternalUpdateCommandReadableModel model, UpdateCommand entity)
            {
                if (entity.IncludedParts == null)
                {
                    return;
                }

                model.FillIncludedParts(entity.IncludedParts);
            }

            private void FillFeedbacksCollectionProperty(InternalUpdateCommandReadableModel model, UpdateCommand entity)
            {
                if (entity.Feedbacks == null)
                {
                    return;
                }

                model.FillFeedbacks(entity.Feedbacks);
            }

            private class InternalUpdateCommandReadableModel : UpdateCommandReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UpdateCommandChangeTrackingManager manager;
                private volatile bool propertyUnitLoaded;
                private volatile bool propertyIncludedPartsLoaded;
                private volatile bool propertyFeedbacksLoaded;
                private volatile bool propertyCommandLoaded;

                public InternalUpdateCommandReadableModel(UpdateCommandChangeTrackingManager manager, UpdateCommand entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUnitLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyIncludedPartsLoaded
                            && this.propertyFeedbacksLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyCommandLoaded;
                    }
                }

                internal UpdateCommand Entity
                {
                    get
                    {
                        return this.UpdateCommand;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UnitReadableModel Unit
                {
                    get
                    {
                        if (!this.propertyUnitLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Unit not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Unit;
                    }
                }

                public override IObservableReadOnlyCollection<UpdatePartReadableModel> IncludedParts
                {
                    get
                    {
                        if (!this.propertyIncludedPartsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property IncludedParts not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.IncludedParts;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateFeedbackReadableModel> Feedbacks
                {
                    get
                    {
                        if (!this.propertyFeedbacksLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Feedbacks not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Feedbacks;
                    }
                }

                public override XmlData Command
                {
                    get
                    {
                        if (!this.propertyCommandLoaded)
                        {
                            throw new ChangeTrackingException("XML property Command not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Command;
                    }
                }

                public override async Task ApplyAsync(UpdateFeedbackDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Feedbacks not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetUnit(UnitReadableModel model)
                {
                    if (this.propertyUnitLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUnitLoaded)
                        {
                            return;
                        }

                        this.propertyUnitLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Unit' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Unit = model;
                }

                public void FillIncludedParts(IEnumerable<UpdatePart> entities)
                {
                    if (this.propertyIncludedPartsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyIncludedPartsLoaded)
                        {
                            return;
                        }

                        this.propertyIncludedPartsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'IncludedParts'");
                    var manager = DependencyResolver.Current.Get<IUpdatePartChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'IncludedParts'", items.Count);
                    foreach (var item in items)
                    {
                        this.includedParts.Add(item);
                    }
                }


                public void FillFeedbacks(IEnumerable<UpdateFeedback> entities)
                {
                    if (this.propertyFeedbacksLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyFeedbacksLoaded)
                        {
                            return;
                        }

                        this.propertyFeedbacksLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Feedbacks'");
                    var manager = DependencyResolver.Current.Get<IUpdateFeedbackChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Feedbacks'", items.Count);
                    foreach (var item in items)
                    {
                        this.feedbacks.Add(item);
                    }
                }


                public void SetCommand(XmlData xmlData)
                {
                    if (this.propertyCommandLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyCommandLoaded)
                        {
                            return;
                        }

                        this.propertyCommandLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Command' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Command = xmlData;
                }
            }
        } // UpdateCommandChangeTrackingManager

        internal static class UpdateCommandReadableModelManagerExtension
        {
            public static UpdateCommandQuery IncludeReferences(this UpdateCommandQuery query)
            {
                return query.IncludeUnit(query.Unit);
            }

            public static UpdateCommandQuery IncludeNavigationProperties(this UpdateCommandQuery query)
            {
                return query.IncludeReferences().IncludeIncludedParts(query.IncludedParts).IncludeFeedbacks(query.Feedbacks);
            }

            public static UpdateCommandQuery IncludeXmlProperties(this UpdateCommandQuery query)
            {
                return query.IncludeCommand();
            }
        }

        public partial class UpdateFeedbackChangeTrackingManager
        {
            public async Task<UpdateFeedbackReadableModel> GetAsync(int id)
            {
                InternalUpdateFeedbackReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UpdateFeedbackQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UpdateFeedbackReadableModel Wrap(UpdateFeedback entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UpdateFeedbackReadableModel Wrap(UpdateFeedback entity, bool containsAllReferences)
            {
                InternalUpdateFeedbackReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUpdateFeedbackReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.FeedbackXml != null)
                    {
                        model.SetFeedback(entity.Feedback);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.UpdateCommand != null)
                {
                    var changeTrackingManagerUpdateCommand =
                        DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                    var modelUpdateCommand = changeTrackingManagerUpdateCommand.Wrap(entity.UpdateCommand);
                    model.SetUpdateCommand(modelUpdateCommand);
                }
                else if (containsAllReferences)
                {
                    model.SetUpdateCommand(null);
                }

                return model;
            }

            private async Task<UpdateFeedback> QueryEntityAsync(UpdateFeedbackQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UpdateFeedback>> QueryEntitiesAsync(UpdateFeedbackQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalUpdateFeedbackReadableModel model)
            {
                var query = UpdateFeedbackQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetFeedback(entity.Feedback);
            }

            private async Task FillReferencePropertiesAsync(InternalUpdateFeedbackReadableModel model)
            {
                var query = UpdateFeedbackQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUpdateFeedbackReadableModel model, UpdateFeedback entity)
            {
                this.FillUpdateCommandReferenceProperty(model, entity);
            }

            private void FillUpdateCommandReferenceProperty(InternalUpdateFeedbackReadableModel model, UpdateFeedback entity)
            {
                if (entity.UpdateCommand == null)
                {
                    model.SetUpdateCommand(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                var reference = manager.Wrap(entity.UpdateCommand);
                model.SetUpdateCommand(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUpdateFeedbackReadableModel model)
            {
                var query = UpdateFeedbackQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalUpdateFeedbackReadableModel : UpdateFeedbackReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UpdateFeedbackChangeTrackingManager manager;
                private volatile bool propertyUpdateCommandLoaded;
                private volatile bool propertyFeedbackLoaded;

                public InternalUpdateFeedbackReadableModel(UpdateFeedbackChangeTrackingManager manager, UpdateFeedback entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyUpdateCommandLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyFeedbackLoaded;
                    }
                }

                internal UpdateFeedback Entity
                {
                    get
                    {
                        return this.UpdateFeedback;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override UpdateCommandReadableModel UpdateCommand
                {
                    get
                    {
                        if (!this.propertyUpdateCommandLoaded)
                        {
                            throw new ChangeTrackingException("Reference property UpdateCommand not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.UpdateCommand;
                    }
                }

                public override XmlData Feedback
                {
                    get
                    {
                        if (!this.propertyFeedbackLoaded)
                        {
                            throw new ChangeTrackingException("XML property Feedback not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Feedback;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetUpdateCommand(UpdateCommandReadableModel model)
                {
                    if (this.propertyUpdateCommandLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateCommandLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateCommandLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'UpdateCommand' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.UpdateCommand = model;
                }

                public void SetFeedback(XmlData xmlData)
                {
                    if (this.propertyFeedbackLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyFeedbackLoaded)
                        {
                            return;
                        }

                        this.propertyFeedbackLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Feedback' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Feedback = xmlData;
                }
            }
        } // UpdateFeedbackChangeTrackingManager

        internal static class UpdateFeedbackReadableModelManagerExtension
        {
            public static UpdateFeedbackQuery IncludeReferences(this UpdateFeedbackQuery query)
            {
                return query.IncludeUpdateCommand(query.UpdateCommand);
            }

            public static UpdateFeedbackQuery IncludeNavigationProperties(this UpdateFeedbackQuery query)
            {
                return query.IncludeReferences();
            }

            public static UpdateFeedbackQuery IncludeXmlProperties(this UpdateFeedbackQuery query)
            {
                return query.IncludeFeedback();
            }
        }
	
    }	

    namespace Documents
    {
        public partial class DocumentChangeTrackingManager
        {
            public async Task<DocumentReadableModel> GetAsync(int id)
            {
                InternalDocumentReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = DocumentQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public DocumentReadableModel Wrap(Document entity)
            {
                return this.Wrap(entity, false);
            }
            
            private DocumentReadableModel Wrap(Document entity, bool containsAllReferences)
            {
                InternalDocumentReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalDocumentReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillVersionsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Tenant != null)
                {
                    var changeTrackingManagerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelTenant = changeTrackingManagerTenant.Wrap(entity.Tenant);
                    model.SetTenant(modelTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetTenant(null);
                }

                return model;
            }

            private async Task<Document> QueryEntityAsync(DocumentQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Document>> QueryEntitiesAsync(DocumentQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalDocumentReadableModel model)
            {
                var query = DocumentQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalDocumentReadableModel model, Document entity)
            {
                this.FillTenantReferenceProperty(model, entity);
            }

            private void FillTenantReferenceProperty(InternalDocumentReadableModel model, Document entity)
            {
                if (entity.Tenant == null)
                {
                    model.SetTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.Tenant);
                model.SetTenant(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalDocumentReadableModel model)
            {
                var query = DocumentQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillVersionsCollectionProperty(model, entity);
            }

            private void FillVersionsCollectionProperty(InternalDocumentReadableModel model, Document entity)
            {
                if (entity.Versions == null)
                {
                    return;
                }

                model.FillVersions(entity.Versions);
            }

            private class InternalDocumentReadableModel : DocumentReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly DocumentChangeTrackingManager manager;
                private volatile bool propertyTenantLoaded;
                private volatile bool propertyVersionsLoaded;

                public InternalDocumentReadableModel(DocumentChangeTrackingManager manager, Document entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyTenantLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyVersionsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Document Entity
                {
                    get
                    {
                        return this.Document;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel Tenant
                {
                    get
                    {
                        if (!this.propertyTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Tenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Tenant;
                    }
                }

                public override IObservableReadOnlyCollection<DocumentVersionReadableModel> Versions
                {
                    get
                    {
                        if (!this.propertyVersionsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Versions not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Versions;
                    }
                }

                public override async Task ApplyAsync(DocumentVersionDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Versions not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetTenant(TenantReadableModel model)
                {
                    if (this.propertyTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyTenantLoaded)
                        {
                            return;
                        }

                        this.propertyTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Tenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Tenant = model;
                }

                public void FillVersions(IEnumerable<DocumentVersion> entities)
                {
                    if (this.propertyVersionsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyVersionsLoaded)
                        {
                            return;
                        }

                        this.propertyVersionsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Versions'");
                    var manager = DependencyResolver.Current.Get<IDocumentVersionChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Versions'", items.Count);
                    foreach (var item in items)
                    {
                        this.versions.Add(item);
                    }
                }

            }
        } // DocumentChangeTrackingManager

        internal static class DocumentReadableModelManagerExtension
        {
            public static DocumentQuery IncludeReferences(this DocumentQuery query)
            {
                return query.IncludeTenant(query.Tenant);
            }

            public static DocumentQuery IncludeNavigationProperties(this DocumentQuery query)
            {
                return query.IncludeReferences().IncludeVersions(query.Versions);
            }

            public static DocumentQuery IncludeXmlProperties(this DocumentQuery query)
            {
                return query;
            }
        }

        public partial class DocumentVersionChangeTrackingManager
        {
            public async Task<DocumentVersionReadableModel> GetAsync(int id)
            {
                InternalDocumentVersionReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = DocumentVersionQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public DocumentVersionReadableModel Wrap(DocumentVersion entity)
            {
                return this.Wrap(entity, false);
            }
            
            private DocumentVersionReadableModel Wrap(DocumentVersion entity, bool containsAllReferences)
            {
                InternalDocumentVersionReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalDocumentVersionReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.ContentXml != null)
                    {
                        model.SetContent(entity.Content);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Document != null)
                {
                    var changeTrackingManagerDocument =
                        DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                    var modelDocument = changeTrackingManagerDocument.Wrap(entity.Document);
                    model.SetDocument(modelDocument);
                }
                else if (containsAllReferences)
                {
                    model.SetDocument(null);
                }

                if (entity.CreatingUser != null)
                {
                    var changeTrackingManagerCreatingUser =
                        DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var modelCreatingUser = changeTrackingManagerCreatingUser.Wrap(entity.CreatingUser);
                    model.SetCreatingUser(modelCreatingUser);
                }
                else if (containsAllReferences)
                {
                    model.SetCreatingUser(null);
                }

                return model;
            }

            private async Task<DocumentVersion> QueryEntityAsync(DocumentVersionQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<DocumentVersion>> QueryEntitiesAsync(DocumentVersionQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalDocumentVersionReadableModel model)
            {
                var query = DocumentVersionQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetContent(entity.Content);
            }

            private async Task FillReferencePropertiesAsync(InternalDocumentVersionReadableModel model)
            {
                var query = DocumentVersionQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalDocumentVersionReadableModel model, DocumentVersion entity)
            {
                this.FillDocumentReferenceProperty(model, entity);
                this.FillCreatingUserReferenceProperty(model, entity);
            }

            private void FillDocumentReferenceProperty(InternalDocumentVersionReadableModel model, DocumentVersion entity)
            {
                if (entity.Document == null)
                {
                    model.SetDocument(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                var reference = manager.Wrap(entity.Document);
                model.SetDocument(reference);
            }

            private void FillCreatingUserReferenceProperty(InternalDocumentVersionReadableModel model, DocumentVersion entity)
            {
                if (entity.CreatingUser == null)
                {
                    model.SetCreatingUser(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                var reference = manager.Wrap(entity.CreatingUser);
                model.SetCreatingUser(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalDocumentVersionReadableModel model)
            {
                var query = DocumentVersionQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalDocumentVersionReadableModel : DocumentVersionReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly DocumentVersionChangeTrackingManager manager;
                private volatile bool propertyDocumentLoaded;
                private volatile bool propertyCreatingUserLoaded;
                private volatile bool propertyContentLoaded;

                public InternalDocumentVersionReadableModel(DocumentVersionChangeTrackingManager manager, DocumentVersion entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyDocumentLoaded
                            && this.propertyCreatingUserLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyContentLoaded;
                    }
                }

                internal DocumentVersion Entity
                {
                    get
                    {
                        return this.DocumentVersion;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override DocumentReadableModel Document
                {
                    get
                    {
                        if (!this.propertyDocumentLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Document not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Document;
                    }
                }

                public override UserReadableModel CreatingUser
                {
                    get
                    {
                        if (!this.propertyCreatingUserLoaded)
                        {
                            throw new ChangeTrackingException("Reference property CreatingUser not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.CreatingUser;
                    }
                }

                public override XmlData Content
                {
                    get
                    {
                        if (!this.propertyContentLoaded)
                        {
                            throw new ChangeTrackingException("XML property Content not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Content;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetDocument(DocumentReadableModel model)
                {
                    if (this.propertyDocumentLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyDocumentLoaded)
                        {
                            return;
                        }

                        this.propertyDocumentLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Document' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Document = model;
                }

                public void SetCreatingUser(UserReadableModel model)
                {
                    if (this.propertyCreatingUserLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyCreatingUserLoaded)
                        {
                            return;
                        }

                        this.propertyCreatingUserLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'CreatingUser' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.CreatingUser = model;
                }

                public void SetContent(XmlData xmlData)
                {
                    if (this.propertyContentLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyContentLoaded)
                        {
                            return;
                        }

                        this.propertyContentLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Content' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Content = xmlData;
                }
            }
        } // DocumentVersionChangeTrackingManager

        internal static class DocumentVersionReadableModelManagerExtension
        {
            public static DocumentVersionQuery IncludeReferences(this DocumentVersionQuery query)
            {
                return query.IncludeDocument(query.Document).IncludeCreatingUser(query.CreatingUser);
            }

            public static DocumentVersionQuery IncludeNavigationProperties(this DocumentVersionQuery query)
            {
                return query.IncludeReferences();
            }

            public static DocumentVersionQuery IncludeXmlProperties(this DocumentVersionQuery query)
            {
                return query.IncludeContent();
            }
        }
	
    }	

    namespace Software
    {
        public partial class PackageChangeTrackingManager
        {
            public async Task<PackageReadableModel> GetAsync(int id)
            {
                InternalPackageReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = PackageQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public PackageReadableModel Wrap(Package entity)
            {
                return this.Wrap(entity, false);
            }
            
            private PackageReadableModel Wrap(Package entity, bool containsAllReferences)
            {
                InternalPackageReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalPackageReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillVersionsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                return model;
            }

            private async Task<Package> QueryEntityAsync(PackageQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<Package>> QueryEntitiesAsync(PackageQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            // The entity doesn't have reference properties. Omitting the FillReferencePropertiesAsync method.

            private async Task FillNavigationPropertiesAsync(InternalPackageReadableModel model)
            {
                var query = PackageQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");

                // The entity doesn't have reference properties. Omitting the call to FillReferenceProperties
                this.FillVersionsCollectionProperty(model, entity);
            }

            private void FillVersionsCollectionProperty(InternalPackageReadableModel model, Package entity)
            {
                if (entity.Versions == null)
                {
                    return;
                }

                model.FillVersions(entity.Versions);
            }

            private class InternalPackageReadableModel : PackageReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly PackageChangeTrackingManager manager;
                private volatile bool propertyVersionsLoaded;

                public InternalPackageReadableModel(PackageChangeTrackingManager manager, Package entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyVersionsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal Package Entity
                {
                    get
                    {
                        return this.Package;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override IObservableReadOnlyCollection<PackageVersionReadableModel> Versions
                {
                    get
                    {
                        if (!this.propertyVersionsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property Versions not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.Versions;
                    }
                }

                public override async Task ApplyAsync(PackageVersionDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("Versions not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    await Task.FromResult(0);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {

                    // The entity doesn't have reference properties. Omitting call to LoadReferencePropertiesAsync();	

                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void FillVersions(IEnumerable<PackageVersion> entities)
                {
                    if (this.propertyVersionsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyVersionsLoaded)
                        {
                            return;
                        }

                        this.propertyVersionsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'Versions'");
                    var manager = DependencyResolver.Current.Get<IPackageVersionChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'Versions'", items.Count);
                    foreach (var item in items)
                    {
                        this.versions.Add(item);
                    }
                }

            }
        } // PackageChangeTrackingManager

        internal static class PackageReadableModelManagerExtension
        {
            public static PackageQuery IncludeReferences(this PackageQuery query)
            {
                return query;
            }

            public static PackageQuery IncludeNavigationProperties(this PackageQuery query)
            {
                return query.IncludeReferences().IncludeVersions(query.Versions);
            }

            public static PackageQuery IncludeXmlProperties(this PackageQuery query)
            {
                return query;
            }
        }

        public partial class PackageVersionChangeTrackingManager
        {
            public async Task<PackageVersionReadableModel> GetAsync(int id)
            {
                InternalPackageVersionReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = PackageVersionQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public PackageVersionReadableModel Wrap(PackageVersion entity)
            {
                return this.Wrap(entity, false);
            }
            
            private PackageVersionReadableModel Wrap(PackageVersion entity, bool containsAllReferences)
            {
                InternalPackageVersionReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalPackageVersionReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.StructureXml != null)
                    {
                        model.SetStructure(entity.Structure);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Package != null)
                {
                    var changeTrackingManagerPackage =
                        DependencyResolver.Current.Get<IPackageChangeTrackingManager>();
                    var modelPackage = changeTrackingManagerPackage.Wrap(entity.Package);
                    model.SetPackage(modelPackage);
                }
                else if (containsAllReferences)
                {
                    model.SetPackage(null);
                }

                return model;
            }

            private async Task<PackageVersion> QueryEntityAsync(PackageVersionQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<PackageVersion>> QueryEntitiesAsync(PackageVersionQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalPackageVersionReadableModel model)
            {
                var query = PackageVersionQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetStructure(entity.Structure);
            }

            private async Task FillReferencePropertiesAsync(InternalPackageVersionReadableModel model)
            {
                var query = PackageVersionQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalPackageVersionReadableModel model, PackageVersion entity)
            {
                this.FillPackageReferenceProperty(model, entity);
            }

            private void FillPackageReferenceProperty(InternalPackageVersionReadableModel model, PackageVersion entity)
            {
                if (entity.Package == null)
                {
                    model.SetPackage(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IPackageChangeTrackingManager>();
                var reference = manager.Wrap(entity.Package);
                model.SetPackage(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalPackageVersionReadableModel model)
            {
                var query = PackageVersionQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalPackageVersionReadableModel : PackageVersionReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly PackageVersionChangeTrackingManager manager;
                private volatile bool propertyPackageLoaded;
                private volatile bool propertyStructureLoaded;

                public InternalPackageVersionReadableModel(PackageVersionChangeTrackingManager manager, PackageVersion entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyPackageLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyStructureLoaded;
                    }
                }

                internal PackageVersion Entity
                {
                    get
                    {
                        return this.PackageVersion;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override PackageReadableModel Package
                {
                    get
                    {
                        if (!this.propertyPackageLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Package not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Package;
                    }
                }

                public override XmlData Structure
                {
                    get
                    {
                        if (!this.propertyStructureLoaded)
                        {
                            throw new ChangeTrackingException("XML property Structure not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Structure;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetPackage(PackageReadableModel model)
                {
                    if (this.propertyPackageLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyPackageLoaded)
                        {
                            return;
                        }

                        this.propertyPackageLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Package' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Package = model;
                }

                public void SetStructure(XmlData xmlData)
                {
                    if (this.propertyStructureLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyStructureLoaded)
                        {
                            return;
                        }

                        this.propertyStructureLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Structure' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Structure = xmlData;
                }
            }
        } // PackageVersionChangeTrackingManager

        internal static class PackageVersionReadableModelManagerExtension
        {
            public static PackageVersionQuery IncludeReferences(this PackageVersionQuery query)
            {
                return query.IncludePackage(query.Package);
            }

            public static PackageVersionQuery IncludeNavigationProperties(this PackageVersionQuery query)
            {
                return query.IncludeReferences();
            }

            public static PackageVersionQuery IncludeXmlProperties(this PackageVersionQuery query)
            {
                return query.IncludeStructure();
            }
        }
	
    }	

    namespace Configurations
    {
        public partial class UnitConfigurationChangeTrackingManager
        {
            public async Task<UnitConfigurationReadableModel> GetAsync(int id)
            {
                InternalUnitConfigurationReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UnitConfigurationQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UnitConfigurationReadableModel Wrap(UnitConfiguration entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UnitConfigurationReadableModel Wrap(UnitConfiguration entity, bool containsAllReferences)
            {
                InternalUnitConfigurationReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUnitConfigurationReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUpdateGroupsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Document != null)
                {
                    var changeTrackingManagerDocument =
                        DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                    var modelDocument = changeTrackingManagerDocument.Wrap(entity.Document);
                    model.SetDocument(modelDocument);
                }
                else if (containsAllReferences)
                {
                    model.SetDocument(null);
                }

                if (entity.ProductType != null)
                {
                    var changeTrackingManagerProductType =
                        DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                    var modelProductType = changeTrackingManagerProductType.Wrap(entity.ProductType);
                    model.SetProductType(modelProductType);
                }
                else if (containsAllReferences)
                {
                    model.SetProductType(null);
                }

                return model;
            }

            private async Task<UnitConfiguration> QueryEntityAsync(UnitConfigurationQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UnitConfiguration>> QueryEntitiesAsync(UnitConfigurationQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalUnitConfigurationReadableModel model)
            {
                var query = UnitConfigurationQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUnitConfigurationReadableModel model, UnitConfiguration entity)
            {
                this.FillDocumentReferenceProperty(model, entity);
                this.FillProductTypeReferenceProperty(model, entity);
            }

            private void FillDocumentReferenceProperty(InternalUnitConfigurationReadableModel model, UnitConfiguration entity)
            {
                if (entity.Document == null)
                {
                    model.SetDocument(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                var reference = manager.Wrap(entity.Document);
                model.SetDocument(reference);
            }

            private void FillProductTypeReferenceProperty(InternalUnitConfigurationReadableModel model, UnitConfiguration entity)
            {
                if (entity.ProductType == null)
                {
                    model.SetProductType(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                var reference = manager.Wrap(entity.ProductType);
                model.SetProductType(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUnitConfigurationReadableModel model)
            {
                var query = UnitConfigurationQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillUpdateGroupsCollectionProperty(model, entity);
            }

            private void FillUpdateGroupsCollectionProperty(InternalUnitConfigurationReadableModel model, UnitConfiguration entity)
            {
                if (entity.UpdateGroups == null)
                {
                    return;
                }

                model.FillUpdateGroups(entity.UpdateGroups);
            }

            private class InternalUnitConfigurationReadableModel : UnitConfigurationReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UnitConfigurationChangeTrackingManager manager;
                private volatile bool propertyDocumentLoaded;
                private volatile bool propertyProductTypeLoaded;
                private volatile bool propertyUpdateGroupsLoaded;

                public InternalUnitConfigurationReadableModel(UnitConfigurationChangeTrackingManager manager, UnitConfiguration entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyDocumentLoaded
                            && this.propertyProductTypeLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUpdateGroupsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal UnitConfiguration Entity
                {
                    get
                    {
                        return this.UnitConfiguration;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override DocumentReadableModel Document
                {
                    get
                    {
                        if (!this.propertyDocumentLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Document not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Document;
                    }
                }

                public override ProductTypeReadableModel ProductType
                {
                    get
                    {
                        if (!this.propertyProductTypeLoaded)
                        {
                            throw new ChangeTrackingException("Reference property ProductType not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.ProductType;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateGroupReadableModel> UpdateGroups
                {
                    get
                    {
                        if (!this.propertyUpdateGroupsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property UpdateGroups not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.UpdateGroups;
                    }
                }

                public override async Task ApplyAsync(UpdateGroupDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("UpdateGroups not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetDocument(DocumentReadableModel model)
                {
                    if (this.propertyDocumentLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyDocumentLoaded)
                        {
                            return;
                        }

                        this.propertyDocumentLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Document' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Document = model;
                }

                public void SetProductType(ProductTypeReadableModel model)
                {
                    if (this.propertyProductTypeLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyProductTypeLoaded)
                        {
                            return;
                        }

                        this.propertyProductTypeLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'ProductType' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.ProductType = model;
                }

                public void FillUpdateGroups(IEnumerable<UpdateGroup> entities)
                {
                    if (this.propertyUpdateGroupsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateGroupsLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateGroupsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'UpdateGroups'");
                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'UpdateGroups'", items.Count);
                    foreach (var item in items)
                    {
                        this.updateGroups.Add(item);
                    }
                }

            }
        } // UnitConfigurationChangeTrackingManager

        internal static class UnitConfigurationReadableModelManagerExtension
        {
            public static UnitConfigurationQuery IncludeReferences(this UnitConfigurationQuery query)
            {
                return query.IncludeDocument(query.Document).IncludeProductType(query.ProductType);
            }

            public static UnitConfigurationQuery IncludeNavigationProperties(this UnitConfigurationQuery query)
            {
                return query.IncludeReferences().IncludeUpdateGroups(query.UpdateGroups);
            }

            public static UnitConfigurationQuery IncludeXmlProperties(this UnitConfigurationQuery query)
            {
                return query;
            }
        }

        public partial class MediaConfigurationChangeTrackingManager
        {
            public async Task<MediaConfigurationReadableModel> GetAsync(int id)
            {
                InternalMediaConfigurationReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = MediaConfigurationQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public MediaConfigurationReadableModel Wrap(MediaConfiguration entity)
            {
                return this.Wrap(entity, false);
            }
            
            private MediaConfigurationReadableModel Wrap(MediaConfiguration entity, bool containsAllReferences)
            {
                InternalMediaConfigurationReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalMediaConfigurationReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                    this.FillUpdateGroupsCollectionProperty(model, entity);
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Document != null)
                {
                    var changeTrackingManagerDocument =
                        DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                    var modelDocument = changeTrackingManagerDocument.Wrap(entity.Document);
                    model.SetDocument(modelDocument);
                }
                else if (containsAllReferences)
                {
                    model.SetDocument(null);
                }

                return model;
            }

            private async Task<MediaConfiguration> QueryEntityAsync(MediaConfigurationQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<MediaConfiguration>> QueryEntitiesAsync(MediaConfigurationQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalMediaConfigurationReadableModel model)
            {
                var query = MediaConfigurationQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalMediaConfigurationReadableModel model, MediaConfiguration entity)
            {
                this.FillDocumentReferenceProperty(model, entity);
            }

            private void FillDocumentReferenceProperty(InternalMediaConfigurationReadableModel model, MediaConfiguration entity)
            {
                if (entity.Document == null)
                {
                    model.SetDocument(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                var reference = manager.Wrap(entity.Document);
                model.SetDocument(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalMediaConfigurationReadableModel model)
            {
                var query = MediaConfigurationQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
                this.FillUpdateGroupsCollectionProperty(model, entity);
            }

            private void FillUpdateGroupsCollectionProperty(InternalMediaConfigurationReadableModel model, MediaConfiguration entity)
            {
                if (entity.UpdateGroups == null)
                {
                    return;
                }

                model.FillUpdateGroups(entity.UpdateGroups);
            }

            private class InternalMediaConfigurationReadableModel : MediaConfigurationReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly MediaConfigurationChangeTrackingManager manager;
                private volatile bool propertyDocumentLoaded;
                private volatile bool propertyUpdateGroupsLoaded;

                public InternalMediaConfigurationReadableModel(MediaConfigurationChangeTrackingManager manager, MediaConfiguration entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyDocumentLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded
                            && this.propertyUpdateGroupsLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal MediaConfiguration Entity
                {
                    get
                    {
                        return this.MediaConfiguration;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override DocumentReadableModel Document
                {
                    get
                    {
                        if (!this.propertyDocumentLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Document not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Document;
                    }
                }

                public override IObservableReadOnlyCollection<UpdateGroupReadableModel> UpdateGroups
                {
                    get
                    {
                        if (!this.propertyUpdateGroupsLoaded)
                        {
                            throw new ChangeTrackingException("Navigation property UpdateGroups not loaded. You must call (and await) the LoadNavigationPropertiesAsync method");
                        }

                        return base.UpdateGroups;
                    }
                }

                public override async Task ApplyAsync(UpdateGroupDelta delta)
                {
                    if (!this.NavigationPropertiesLoaded)
                    {
                        Logger.Trace("UpdateGroups not requested by the client app. Ignoring delta");
                        return;
                    }

                    await base.ApplyAsync(delta);
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetDocument(DocumentReadableModel model)
                {
                    if (this.propertyDocumentLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyDocumentLoaded)
                        {
                            return;
                        }

                        this.propertyDocumentLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Document' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Document = model;
                }

                public void FillUpdateGroups(IEnumerable<UpdateGroup> entities)
                {
                    if (this.propertyUpdateGroupsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyUpdateGroupsLoaded)
                        {
                            return;
                        }

                        this.propertyUpdateGroupsLoaded = true;
                    }

                    Logger.Trace("Setting collection property 'UpdateGroups'");
                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var items = entities.Select(entity => manager.Wrap(entity)).ToList();
                    Logger.Trace("Found {0} item(s) for collection 'UpdateGroups'", items.Count);
                    foreach (var item in items)
                    {
                        this.updateGroups.Add(item);
                    }
                }

            }
        } // MediaConfigurationChangeTrackingManager

        internal static class MediaConfigurationReadableModelManagerExtension
        {
            public static MediaConfigurationQuery IncludeReferences(this MediaConfigurationQuery query)
            {
                return query.IncludeDocument(query.Document);
            }

            public static MediaConfigurationQuery IncludeNavigationProperties(this MediaConfigurationQuery query)
            {
                return query.IncludeReferences().IncludeUpdateGroups(query.UpdateGroups);
            }

            public static MediaConfigurationQuery IncludeXmlProperties(this MediaConfigurationQuery query)
            {
                return query;
            }
        }
	
    }	

    namespace Log
    {	
    }	

    namespace Meta
    {
        public partial class UserDefinedPropertyChangeTrackingManager
        {
            public async Task<UserDefinedPropertyReadableModel> GetAsync(int id)
            {
                InternalUserDefinedPropertyReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = UserDefinedPropertyQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public UserDefinedPropertyReadableModel Wrap(UserDefinedProperty entity)
            {
                return this.Wrap(entity, false);
            }
            
            private UserDefinedPropertyReadableModel Wrap(UserDefinedProperty entity, bool containsAllReferences)
            {
                InternalUserDefinedPropertyReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalUserDefinedPropertyReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                if (entity.Tenant != null)
                {
                    var changeTrackingManagerTenant =
                        DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var modelTenant = changeTrackingManagerTenant.Wrap(entity.Tenant);
                    model.SetTenant(modelTenant);
                }
                else if (containsAllReferences)
                {
                    model.SetTenant(null);
                }

                return model;
            }

            private async Task<UserDefinedProperty> QueryEntityAsync(UserDefinedPropertyQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<UserDefinedProperty>> QueryEntitiesAsync(UserDefinedPropertyQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            // The entity doesn't have XML properties. Omitting the FillXmlPropertiesAsync method.

            private async Task FillReferencePropertiesAsync(InternalUserDefinedPropertyReadableModel model)
            {
                var query = UserDefinedPropertyQuery.Create().WithId(model.Entity.Id);
                var entity = await this.QueryEntityAsync(query);
                this.FillReferenceProperties(model, entity);
            }

            private void FillReferenceProperties(InternalUserDefinedPropertyReadableModel model, UserDefinedProperty entity)
            {
                this.FillTenantReferenceProperty(model, entity);
            }

            private void FillTenantReferenceProperty(InternalUserDefinedPropertyReadableModel model, UserDefinedProperty entity)
            {
                if (entity.Tenant == null)
                {
                    model.SetTenant(null);
                    return;
                }

                var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                var reference = manager.Wrap(entity.Tenant);
                model.SetTenant(reference);
            }

            private async Task FillNavigationPropertiesAsync(InternalUserDefinedPropertyReadableModel model)
            {
                var query = UserDefinedPropertyQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");
                this.FillReferenceProperties(model, entity);
            }

            private class InternalUserDefinedPropertyReadableModel : UserDefinedPropertyReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly UserDefinedPropertyChangeTrackingManager manager;
                private volatile bool propertyTenantLoaded;

                public InternalUserDefinedPropertyReadableModel(UserDefinedPropertyChangeTrackingManager manager, UserDefinedProperty entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertyTenantLoaded;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                internal UserDefinedProperty Entity
                {
                    get
                    {
                        return this.UserDefinedProperty;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override TenantReadableModel Tenant
                {
                    get
                    {
                        if (!this.propertyTenantLoaded)
                        {
                            throw new ChangeTrackingException("Reference property Tenant not loaded. You must call (and await) the LoadReferencePropertiesAsync method");
                        }

                        return base.Tenant;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    if (this.ReferencePropertiesLoaded)
                    {
                        return;
                    }
                        
                    await this.manager.FillReferencePropertiesAsync(this);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {
                    await this.LoadReferencePropertiesAsync();
                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override Task LoadXmlPropertiesAsync()
                {
                    // The entity doesn't have XML properties. Omitting call to FillXmlPropertiesAsync()
                    return Task.FromResult(0);
                }

                public void SetTenant(TenantReadableModel model)
                {
                    if (this.propertyTenantLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertyTenantLoaded)
                        {
                            return;
                        }

                        this.propertyTenantLoaded = true;
                    }

                    Logger.Trace(
                        "Setting reference property 'Tenant' ({0}) on entity {1}",
                        model == null ? "null" : model.Id.ToString(),
                        this.Id);
                    this.Tenant = model;
                }
            }
        } // UserDefinedPropertyChangeTrackingManager

        internal static class UserDefinedPropertyReadableModelManagerExtension
        {
            public static UserDefinedPropertyQuery IncludeReferences(this UserDefinedPropertyQuery query)
            {
                return query.IncludeTenant(query.Tenant);
            }

            public static UserDefinedPropertyQuery IncludeNavigationProperties(this UserDefinedPropertyQuery query)
            {
                return query.IncludeReferences();
            }

            public static UserDefinedPropertyQuery IncludeXmlProperties(this UserDefinedPropertyQuery query)
            {
                return query;
            }
        }

        public partial class SystemConfigChangeTrackingManager
        {
            public async Task<SystemConfigReadableModel> GetAsync(int id)
            {
                InternalSystemConfigReadableModel model;
                if (this.models.TryGetValue(id, out model))
                {
                    await model.LoadReferencePropertiesAsync();
                    return model;
                }

                var query = SystemConfigQuery.Create().WithId(id);
                var entity = await this.QueryEntityAsync(query);
                if (entity == null)
                {
                    return null;
                }

                return this.Wrap(entity, true);
            }

            public SystemConfigReadableModel Wrap(SystemConfig entity)
            {
                return this.Wrap(entity, false);
            }
            
            private SystemConfigReadableModel Wrap(SystemConfig entity, bool containsAllReferences)
            {
                InternalSystemConfigReadableModel model;
                if (!this.models.TryGetValue(entity.Id, out model))
                {
                    model = new InternalSystemConfigReadableModel(this, entity);
                    model.PopulateInternal();
                    this.Track(model);
                }

                if (!model.XmlPropertiesLoaded)
                {
                    if (entity.SettingsXml != null)
                    {
                        model.SetSettings(entity.Settings);
                    }
                }

                if (!model.NavigationPropertiesLoaded)
                {
                }

                if (model.ReferencePropertiesLoaded)
                {
                    return model;
                }

                return model;
            }

            private async Task<SystemConfig> QueryEntityAsync(SystemConfigQuery query)
            {
                return (await this.QueryEntitiesAsync(query)).SingleOrDefault();
            }

            private async Task<IEnumerable<SystemConfig>> QueryEntitiesAsync(SystemConfigQuery query)
            {
                query = query.IncludeReferences();
                using (var channelScope = this.CreateChannelScope())
                {
                    return await channelScope.Channel.QueryAsync(query);
                }
            }

            private async Task FillXmlPropertiesAsync(InternalSystemConfigReadableModel model)
            {
                var query = SystemConfigQuery.Create().WithId(model.Entity.Id).IncludeXmlProperties();
                var entity = await this.QueryEntityAsync(query);

                model.SetSettings(entity.Settings);
            }

            // The entity doesn't have reference properties. Omitting the FillReferencePropertiesAsync method.

            private async Task FillNavigationPropertiesAsync(InternalSystemConfigReadableModel model)
            {
                var query = SystemConfigQuery.Create().WithId(model.Entity.Id).IncludeNavigationProperties();
                var entity = await this.QueryEntityAsync(query);

                Logger.Trace("Filling navigation properties");

                // The entity doesn't have reference properties. Omitting the call to FillReferenceProperties
            }

            private class InternalSystemConfigReadableModel : SystemConfigReadableModel
            {
                private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

                private readonly SystemConfigChangeTrackingManager manager;
                private volatile bool propertySettingsLoaded;

                public InternalSystemConfigReadableModel(SystemConfigChangeTrackingManager manager, SystemConfig entity)
                    : base(entity)
                {
                    this.manager = manager;
                }

                public bool ReferencePropertiesLoaded
                {
                    get
                    {
                        return true;
                    }
                }

                public bool NavigationPropertiesLoaded
                {
                    get
                    {
                        return this.ReferencePropertiesLoaded;
                    }
                }

                public bool XmlPropertiesLoaded
                {
                    get
                    {
                        return true
                            && this.propertySettingsLoaded;
                    }
                }

                internal SystemConfig Entity
                {
                    get
                    {
                        return this.SystemConfig;
                    }
                }

                internal void PopulateInternal()
                {
                    this.Populate();
                }

                public override XmlData Settings
                {
                    get
                    {
                        if (!this.propertySettingsLoaded)
                        {
                            throw new ChangeTrackingException("XML property Settings not loaded. You must call (and await) the LoadXmlPropertiesAsync method");
                        }

                        return base.Settings;
                    }
                }

                public override async Task LoadReferencePropertiesAsync()
                {
                    await Task.FromResult(0);
                }

                public override async Task LoadNavigationPropertiesAsync()
                {

                    // The entity doesn't have reference properties. Omitting call to LoadReferencePropertiesAsync();	

                    if (this.NavigationPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillNavigationPropertiesAsync(this);
                }

                public override async Task LoadXmlPropertiesAsync()
                {
                    if (this.XmlPropertiesLoaded)
                    {
                        return;
                    }

                    await this.manager.FillXmlPropertiesAsync(this);
                }

                public void SetSettings(XmlData xmlData)
                {
                    if (this.propertySettingsLoaded)
                    {
                        return;
                    }

                    lock (this.locker)
                    {
                        if (this.propertySettingsLoaded)
                        {
                            return;
                        }

                        this.propertySettingsLoaded = true;
                    }

                    Logger.Trace(
                        "Setting XML property 'Settings' ({0}) on entity {1}",
                        xmlData == null ? "null" : xmlData.Type,
                        this.Id);
                    this.Settings = xmlData;
                }
            }
        } // SystemConfigChangeTrackingManager

        internal static class SystemConfigReadableModelManagerExtension
        {
            public static SystemConfigQuery IncludeReferences(this SystemConfigQuery query)
            {
                return query;
            }

            public static SystemConfigQuery IncludeNavigationProperties(this SystemConfigQuery query)
            {
                return query.IncludeReferences();
            }

            public static SystemConfigQuery IncludeXmlProperties(this SystemConfigQuery query)
            {
                return query.IncludeSettings();
            }
        }
	
    }	

}
