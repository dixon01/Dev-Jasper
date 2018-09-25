

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;

	using Gorba.Center.Common.ServiceModel.Collections;

    namespace AccessControl
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        public partial interface IAuthorizationChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Removed;

            Task AddAsync(AuthorizationWritableModel writableModel);

            Task DeleteAsync(AuthorizationReadableModel model);

            Task<AuthorizationReadableModel> GetAsync(int id);

            Task<IEnumerable<AuthorizationReadableModel>> QueryAsync(AuthorizationQuery query = null);
            
            Task<AuthorizationReadableModel> CommitAndVerifyAsync(AuthorizationWritableModel writableModel);

            AuthorizationReadableModel Wrap(Authorization entity);

            AuthorizationWritableModel Create();

            AuthorizationWritableModel CreateCopy(AuthorizationReadableModel model);
        }

        public partial class AuthorizationReadableModel : ReadableModelBase<AuthorizationDelta>
        {

            protected readonly Authorization Authorization;

            protected readonly object locker = new object();

            private volatile bool populated;

            public AuthorizationReadableModel(Authorization entity)
            {
                this.Authorization = entity;
            }

            public int Id { get; private set; }

            public virtual UserRoleReadableModel UserRole { get; protected set; }

            public DataScope DataScope { get; private set; }

            public Permission Permission { get; private set; }

            public override async Task ApplyAsync(AuthorizationDelta delta)
            {
                if (delta.DataScope != null)
                {
                    this.DataScope = delta.DataScope.Value;
                    this.OnPropertyChanged("DataScope");
                }

                if (delta.Permission != null)
                {
                    this.Permission = delta.Permission.Value;
                    this.OnPropertyChanged("Permission");
                }

                if (delta.UserRole != null)
                {
                    var userRoleChangeTrackingManager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                    var id = delta.UserRole.ReferenceId;
                    this.UserRole = id.HasValue ? await userRoleChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UserRole");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((AuthorizationReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(AuthorizationReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Authorization.Id;
                this.DataScope = this.Authorization.DataScope;
                this.Permission = this.Authorization.Permission;
                this.CreatedOn = this.Authorization.CreatedOn;
                this.LastModifiedOn = this.Authorization.LastModifiedOn;
                this.Version = new Version(this.Authorization.Version);
            }

            public AuthorizationWritableModel ToChangeTrackingModel()
            {
                var model = new AuthorizationWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class AuthorizationWritableModel : WritableModelBase<AuthorizationDelta>
        {
            private UserRoleReadableModel userRole;
            private DataScope dataScope;
            private Permission permission;

            internal protected AuthorizationWritableModel(AuthorizationReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.dataScope = readableModel.DataScope;
                this.permission = readableModel.Permission;
                this.userRole = readableModel.UserRole;
                this.Delta = new AuthorizationDelta(readableModel);
            }

            public AuthorizationWritableModel()
                : base(new Version(0))
            {
                this.Delta = new AuthorizationDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public DataScope DataScope
            {
                get
                {
                    return this.dataScope;
                }

                set
                {
                    this.dataScope = value;
                    this.Delta.ChangeDataScope(value);
                }
            }

            public Permission Permission
            {
                get
                {
                    return this.permission;
                }

                set
                {
                    this.permission = value;
                    this.Delta.ChangePermission(value);
                }
            }

            public UserRoleReadableModel UserRole
            {
                get
                {
                    return this.userRole;
                }

                set
                {
                    this.userRole = value;
                    this.Delta.ChangeUserRole(value);
                }
            }

            public AuthorizationReadableModel ReadableModel { get; private set; }

            public static AuthorizationWritableModel CreateCopyFrom(AuthorizationReadableModel readableModel)
            {
                var model = new AuthorizationWritableModel();
                model.DataScope = readableModel.DataScope;
                model.Permission = readableModel.Permission;
                model.UserRole = readableModel.UserRole;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUserRoleChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Removed;

            Task AddAsync(UserRoleWritableModel writableModel);

            Task DeleteAsync(UserRoleReadableModel model);

            Task<UserRoleReadableModel> GetAsync(int id);

            Task<IEnumerable<UserRoleReadableModel>> QueryAsync(UserRoleQuery query = null);
            
            Task<UserRoleReadableModel> CommitAndVerifyAsync(UserRoleWritableModel writableModel);

            UserRoleReadableModel Wrap(UserRole entity);

            UserRoleWritableModel Create();

            UserRoleWritableModel CreateCopy(UserRoleReadableModel model);
        }

        public partial class UserRoleReadableModel : ReadableModelBase<UserRoleDelta>
        {
            protected readonly ObservableReadOnlyCollection<AuthorizationReadableModel> authorizations =
                new ObservableReadOnlyCollection<AuthorizationReadableModel>();

            protected readonly ObservableReadOnlyDictionary<string, string> userDefinedProperties;

            protected readonly UserRole UserRole;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UserRoleReadableModel(UserRole entity)
            {
                this.UserRole = entity;
                if (entity.UserDefinedProperties == null)
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>();
                }
                else
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>(entity.UserDefinedProperties);
                }
            }

            public IObservableReadOnlyDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.userDefinedProperties;
                }
            }

            public int Id { get; private set; }

            public virtual IObservableReadOnlyCollection<AuthorizationReadableModel> Authorizations
            {
                get
                {
                    return this.authorizations;
                }
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public override async Task ApplyAsync(UserRoleDelta delta)
            {
                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.UserDefinedProperties != null)
                {
                    foreach (var key in delta.UserDefinedProperties.Keys)
                    {
                        var newValue = delta.UserDefinedProperties[key];
                        string oldValue;
                        if (!this.userDefinedProperties.TryGetValue(key, out oldValue) || oldValue != newValue)
                        {
                            this.userDefinedProperties[key] = newValue;
                            this.OnPropertyChanged(key);
                        }
                    }
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UserRoleReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UserRoleReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UserRole.Id;
                this.Name = this.UserRole.Name;
                this.Description = this.UserRole.Description;
                this.CreatedOn = this.UserRole.CreatedOn;
                this.LastModifiedOn = this.UserRole.LastModifiedOn;
                this.Version = new Version(this.UserRole.Version);
            }

            public UserRoleWritableModel ToChangeTrackingModel()
            {
                var model = new UserRoleWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UserRoleWritableModel : WritableModelBase<UserRoleDelta>
        {            private string name;
            private string description;

            internal protected UserRoleWritableModel(UserRoleReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.name = readableModel.Name;
                this.description = readableModel.Description;
                this.Delta = new UserRoleDelta(readableModel);

                foreach (var udp in readableModel.UserDefinedProperties)
                {
                    this.Delta.UserDefinedPropertiesDelta[udp.Key] = udp.Value;
                }
            }

            public UserRoleWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UserRoleDelta();
            }

            public IDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.Delta.UserDefinedPropertiesDelta;
                }
            } 

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public UserRoleReadableModel ReadableModel { get; private set; }

            public static UserRoleWritableModel CreateCopyFrom(UserRoleReadableModel readableModel)
            {
                var model = new UserRoleWritableModel();
                model.Name = readableModel.Name;
                model.Description = readableModel.Description;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Membership
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        public partial interface IAssociationTenantUserUserRoleChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Removed;

            Task AddAsync(AssociationTenantUserUserRoleWritableModel writableModel);

            Task DeleteAsync(AssociationTenantUserUserRoleReadableModel model);

            Task<AssociationTenantUserUserRoleReadableModel> GetAsync(int id);

            Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>> QueryAsync(AssociationTenantUserUserRoleQuery query = null);
            
            Task<AssociationTenantUserUserRoleReadableModel> CommitAndVerifyAsync(AssociationTenantUserUserRoleWritableModel writableModel);

            AssociationTenantUserUserRoleReadableModel Wrap(AssociationTenantUserUserRole entity);

            AssociationTenantUserUserRoleWritableModel Create();

            AssociationTenantUserUserRoleWritableModel CreateCopy(AssociationTenantUserUserRoleReadableModel model);
        }

        public partial class AssociationTenantUserUserRoleReadableModel : ReadableModelBase<AssociationTenantUserUserRoleDelta>
        {

            protected readonly AssociationTenantUserUserRole AssociationTenantUserUserRole;

            protected readonly object locker = new object();

            private volatile bool populated;

            public AssociationTenantUserUserRoleReadableModel(AssociationTenantUserUserRole entity)
            {
                this.AssociationTenantUserUserRole = entity;
            }

            public int Id { get; private set; }

            public virtual TenantReadableModel Tenant { get; protected set; }

            public virtual UserReadableModel User { get; protected set; }

            public virtual AccessControl.UserRoleReadableModel UserRole { get; protected set; }

            public override async Task ApplyAsync(AssociationTenantUserUserRoleDelta delta)
            {
                if (delta.Tenant != null)
                {
                    var tenantChangeTrackingManager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var id = delta.Tenant.ReferenceId;
                    this.Tenant = id.HasValue ? await tenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Tenant");
                }

                if (delta.User != null)
                {
                    var userChangeTrackingManager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    var id = delta.User.ReferenceId;
                    this.User = id.HasValue ? await userChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("User");
                }

                if (delta.UserRole != null)
                {
                    var userRoleChangeTrackingManager = DependencyResolver.Current.Get<AccessControl.IUserRoleChangeTrackingManager>();
                    var id = delta.UserRole.ReferenceId;
                    this.UserRole = id.HasValue ? await userRoleChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UserRole");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((AssociationTenantUserUserRoleReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(AssociationTenantUserUserRoleReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.AssociationTenantUserUserRole.Id;
                this.CreatedOn = this.AssociationTenantUserUserRole.CreatedOn;
                this.LastModifiedOn = this.AssociationTenantUserUserRole.LastModifiedOn;
                this.Version = new Version(this.AssociationTenantUserUserRole.Version);
            }

            public AssociationTenantUserUserRoleWritableModel ToChangeTrackingModel()
            {
                var model = new AssociationTenantUserUserRoleWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class AssociationTenantUserUserRoleWritableModel : WritableModelBase<AssociationTenantUserUserRoleDelta>
        {
            private TenantReadableModel tenant;

            private UserReadableModel user;

            private AccessControl.UserRoleReadableModel userRole;

            internal protected AssociationTenantUserUserRoleWritableModel(AssociationTenantUserUserRoleReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.tenant = readableModel.Tenant;
                this.user = readableModel.User;
                this.userRole = readableModel.UserRole;
                this.Delta = new AssociationTenantUserUserRoleDelta(readableModel);
            }

            public AssociationTenantUserUserRoleWritableModel()
                : base(new Version(0))
            {
                this.Delta = new AssociationTenantUserUserRoleDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public TenantReadableModel Tenant
            {
                get
                {
                    return this.tenant;
                }

                set
                {
                    this.tenant = value;
                    this.Delta.ChangeTenant(value);
                }
            }

            public UserReadableModel User
            {
                get
                {
                    return this.user;
                }

                set
                {
                    this.user = value;
                    this.Delta.ChangeUser(value);
                }
            }

            public AccessControl.UserRoleReadableModel UserRole
            {
                get
                {
                    return this.userRole;
                }

                set
                {
                    this.userRole = value;
                    this.Delta.ChangeUserRole(value);
                }
            }

            public AssociationTenantUserUserRoleReadableModel ReadableModel { get; private set; }

            public static AssociationTenantUserUserRoleWritableModel CreateCopyFrom(AssociationTenantUserUserRoleReadableModel readableModel)
            {
                var model = new AssociationTenantUserUserRoleWritableModel();
                model.Tenant = readableModel.Tenant;
                model.User = readableModel.User;
                model.UserRole = readableModel.UserRole;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface ITenantChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Removed;

            Task AddAsync(TenantWritableModel writableModel);

            Task DeleteAsync(TenantReadableModel model);

            Task<TenantReadableModel> GetAsync(int id);

            Task<IEnumerable<TenantReadableModel>> QueryAsync(TenantQuery query = null);
            
            Task<TenantReadableModel> CommitAndVerifyAsync(TenantWritableModel writableModel);

            TenantReadableModel Wrap(Tenant entity);

            TenantWritableModel Create();

            TenantWritableModel CreateCopy(TenantReadableModel model);
        }

        public partial class TenantReadableModel : ReadableModelBase<TenantDelta>
        {
            protected readonly ObservableReadOnlyCollection<UserReadableModel> users =
                new ObservableReadOnlyCollection<UserReadableModel>();
            protected readonly ObservableReadOnlyCollection<Update.UpdateGroupReadableModel> updateGroups =
                new ObservableReadOnlyCollection<Update.UpdateGroupReadableModel>();

            protected readonly ObservableReadOnlyDictionary<string, string> userDefinedProperties;

            protected readonly Tenant Tenant;

            protected readonly object locker = new object();

            private volatile bool populated;

            public TenantReadableModel(Tenant entity)
            {
                this.Tenant = entity;
                if (entity.UserDefinedProperties == null)
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>();
                }
                else
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>(entity.UserDefinedProperties);
                }
            }

            public IObservableReadOnlyDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.userDefinedProperties;
                }
            }

            public int Id { get; private set; }

            public virtual IObservableReadOnlyCollection<UserReadableModel> Users
            {
                get
                {
                    return this.users;
                }
            }

            public virtual IObservableReadOnlyCollection<Update.UpdateGroupReadableModel> UpdateGroups
            {
                get
                {
                    return this.updateGroups;
                }
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public override async Task ApplyAsync(TenantDelta delta)
            {
                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.UserDefinedProperties != null)
                {
                    foreach (var key in delta.UserDefinedProperties.Keys)
                    {
                        var newValue = delta.UserDefinedProperties[key];
                        string oldValue;
                        if (!this.userDefinedProperties.TryGetValue(key, out oldValue) || oldValue != newValue)
                        {
                            this.userDefinedProperties[key] = newValue;
                            this.OnPropertyChanged(key);
                        }
                    }
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((TenantReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(TenantReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Tenant.Id;
                this.Name = this.Tenant.Name;
                this.Description = this.Tenant.Description;
                this.CreatedOn = this.Tenant.CreatedOn;
                this.LastModifiedOn = this.Tenant.LastModifiedOn;
                this.Version = new Version(this.Tenant.Version);
            }

            public TenantWritableModel ToChangeTrackingModel()
            {
                var model = new TenantWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class TenantWritableModel : WritableModelBase<TenantDelta>
        {            private string name;
            private string description;

            internal protected TenantWritableModel(TenantReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.name = readableModel.Name;
                this.description = readableModel.Description;
                this.Delta = new TenantDelta(readableModel);

                foreach (var udp in readableModel.UserDefinedProperties)
                {
                    this.Delta.UserDefinedPropertiesDelta[udp.Key] = udp.Value;
                }
            }

            public TenantWritableModel()
                : base(new Version(0))
            {
                this.Delta = new TenantDelta();
            }

            public IDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.Delta.UserDefinedPropertiesDelta;
                }
            } 

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public TenantReadableModel ReadableModel { get; private set; }

            public static TenantWritableModel CreateCopyFrom(TenantReadableModel readableModel)
            {
                var model = new TenantWritableModel();
                model.Name = readableModel.Name;
                model.Description = readableModel.Description;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUserChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UserReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UserReadableModel>> Removed;

            Task AddAsync(UserWritableModel writableModel);

            Task DeleteAsync(UserReadableModel model);

            Task<UserReadableModel> GetAsync(int id);

            Task<IEnumerable<UserReadableModel>> QueryAsync(UserQuery query = null);
            
            Task<UserReadableModel> CommitAndVerifyAsync(UserWritableModel writableModel);

            UserReadableModel Wrap(User entity);

            UserWritableModel Create();

            UserWritableModel CreateCopy(UserReadableModel model);
        }

        public partial class UserReadableModel : ReadableModelBase<UserDelta>
        {
            protected readonly ObservableReadOnlyCollection<AssociationTenantUserUserRoleReadableModel> associationTenantUserUserRoles =
                new ObservableReadOnlyCollection<AssociationTenantUserUserRoleReadableModel>();

            protected readonly ObservableReadOnlyDictionary<string, string> userDefinedProperties;

            protected readonly User User;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UserReadableModel(User entity)
            {
                this.User = entity;
                if (entity.UserDefinedProperties == null)
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>();
                }
                else
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>(entity.UserDefinedProperties);
                }
            }

            public IObservableReadOnlyDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.userDefinedProperties;
                }
            }

            public int Id { get; private set; }

            public virtual TenantReadableModel OwnerTenant { get; protected set; }

            public virtual IObservableReadOnlyCollection<AssociationTenantUserUserRoleReadableModel> AssociationTenantUserUserRoles
            {
                get
                {
                    return this.associationTenantUserUserRoles;
                }
            }

            public string Username { get; private set; }

            public string Domain { get; private set; }

            public string HashedPassword { get; private set; }

            public string FirstName { get; private set; }

            public string LastName { get; private set; }

            public string Email { get; private set; }

            public string Culture { get; private set; }

            public string TimeZone { get; private set; }

            public string Description { get; private set; }

            public DateTime? LastLoginAttempt { get; private set; }

            public DateTime? LastSuccessfulLogin { get; private set; }

            public int ConsecutiveLoginFailures { get; private set; }

            public bool IsEnabled { get; private set; }

            public override async Task ApplyAsync(UserDelta delta)
            {
                if (delta.Username != null)
                {
                    this.Username = delta.Username.Value;
                    this.OnPropertyChanged("Username");
                }

                if (delta.Domain != null)
                {
                    this.Domain = delta.Domain.Value;
                    this.OnPropertyChanged("Domain");
                }

                if (delta.HashedPassword != null)
                {
                    this.HashedPassword = delta.HashedPassword.Value;
                    this.OnPropertyChanged("HashedPassword");
                }

                if (delta.FirstName != null)
                {
                    this.FirstName = delta.FirstName.Value;
                    this.OnPropertyChanged("FirstName");
                }

                if (delta.LastName != null)
                {
                    this.LastName = delta.LastName.Value;
                    this.OnPropertyChanged("LastName");
                }

                if (delta.Email != null)
                {
                    this.Email = delta.Email.Value;
                    this.OnPropertyChanged("Email");
                }

                if (delta.Culture != null)
                {
                    this.Culture = delta.Culture.Value;
                    this.OnPropertyChanged("Culture");
                }

                if (delta.TimeZone != null)
                {
                    this.TimeZone = delta.TimeZone.Value;
                    this.OnPropertyChanged("TimeZone");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.LastLoginAttempt != null)
                {
                    this.LastLoginAttempt = delta.LastLoginAttempt.Value;
                    this.OnPropertyChanged("LastLoginAttempt");
                }

                if (delta.LastSuccessfulLogin != null)
                {
                    this.LastSuccessfulLogin = delta.LastSuccessfulLogin.Value;
                    this.OnPropertyChanged("LastSuccessfulLogin");
                }

                if (delta.ConsecutiveLoginFailures != null)
                {
                    this.ConsecutiveLoginFailures = delta.ConsecutiveLoginFailures.Value;
                    this.OnPropertyChanged("ConsecutiveLoginFailures");
                }

                if (delta.IsEnabled != null)
                {
                    this.IsEnabled = delta.IsEnabled.Value;
                    this.OnPropertyChanged("IsEnabled");
                }

                if (delta.OwnerTenant != null)
                {
                    var ownerTenantChangeTrackingManager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                    var id = delta.OwnerTenant.ReferenceId;
                    this.OwnerTenant = id.HasValue ? await ownerTenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("OwnerTenant");
                }

                if (delta.UserDefinedProperties != null)
                {
                    foreach (var key in delta.UserDefinedProperties.Keys)
                    {
                        var newValue = delta.UserDefinedProperties[key];
                        string oldValue;
                        if (!this.userDefinedProperties.TryGetValue(key, out oldValue) || oldValue != newValue)
                        {
                            this.userDefinedProperties[key] = newValue;
                            this.OnPropertyChanged(key);
                        }
                    }
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UserReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UserReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.User.Id;
                this.Username = this.User.Username;
                this.Domain = this.User.Domain;
                this.HashedPassword = this.User.HashedPassword;
                this.FirstName = this.User.FirstName;
                this.LastName = this.User.LastName;
                this.Email = this.User.Email;
                this.Culture = this.User.Culture;
                this.TimeZone = this.User.TimeZone;
                this.Description = this.User.Description;
                this.LastLoginAttempt = this.User.LastLoginAttempt;
                this.LastSuccessfulLogin = this.User.LastSuccessfulLogin;
                this.ConsecutiveLoginFailures = this.User.ConsecutiveLoginFailures;
                this.IsEnabled = this.User.IsEnabled;
                this.CreatedOn = this.User.CreatedOn;
                this.LastModifiedOn = this.User.LastModifiedOn;
                this.Version = new Version(this.User.Version);
            }

            public UserWritableModel ToChangeTrackingModel()
            {
                var model = new UserWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UserWritableModel : WritableModelBase<UserDelta>
        {
            private TenantReadableModel ownerTenant;
            private string username;
            private string domain;
            private string hashedPassword;
            private string firstName;
            private string lastName;
            private string email;
            private string culture;
            private string timeZone;
            private string description;
            private DateTime? lastLoginAttempt;
            private DateTime? lastSuccessfulLogin;
            private int consecutiveLoginFailures;
            private bool isEnabled;

            internal protected UserWritableModel(UserReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.username = readableModel.Username;
                this.domain = readableModel.Domain;
                this.hashedPassword = readableModel.HashedPassword;
                this.firstName = readableModel.FirstName;
                this.lastName = readableModel.LastName;
                this.email = readableModel.Email;
                this.culture = readableModel.Culture;
                this.timeZone = readableModel.TimeZone;
                this.description = readableModel.Description;
                this.lastLoginAttempt = readableModel.LastLoginAttempt;
                this.lastSuccessfulLogin = readableModel.LastSuccessfulLogin;
                this.consecutiveLoginFailures = readableModel.ConsecutiveLoginFailures;
                this.isEnabled = readableModel.IsEnabled;
                this.ownerTenant = readableModel.OwnerTenant;
                this.Delta = new UserDelta(readableModel);

                foreach (var udp in readableModel.UserDefinedProperties)
                {
                    this.Delta.UserDefinedPropertiesDelta[udp.Key] = udp.Value;
                }
            }

            public UserWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UserDelta();
            }

            public IDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.Delta.UserDefinedPropertiesDelta;
                }
            } 

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Username
            {
                get
                {
                    return this.username;
                }

                set
                {
                    this.username = value;
                    this.Delta.ChangeUsername(value);
                }
            }

            public string Domain
            {
                get
                {
                    return this.domain;
                }

                set
                {
                    this.domain = value;
                    this.Delta.ChangeDomain(value);
                }
            }

            public string HashedPassword
            {
                get
                {
                    return this.hashedPassword;
                }

                set
                {
                    this.hashedPassword = value;
                    this.Delta.ChangeHashedPassword(value);
                }
            }

            public string FirstName
            {
                get
                {
                    return this.firstName;
                }

                set
                {
                    this.firstName = value;
                    this.Delta.ChangeFirstName(value);
                }
            }

            public string LastName
            {
                get
                {
                    return this.lastName;
                }

                set
                {
                    this.lastName = value;
                    this.Delta.ChangeLastName(value);
                }
            }

            public string Email
            {
                get
                {
                    return this.email;
                }

                set
                {
                    this.email = value;
                    this.Delta.ChangeEmail(value);
                }
            }

            public string Culture
            {
                get
                {
                    return this.culture;
                }

                set
                {
                    this.culture = value;
                    this.Delta.ChangeCulture(value);
                }
            }

            public string TimeZone
            {
                get
                {
                    return this.timeZone;
                }

                set
                {
                    this.timeZone = value;
                    this.Delta.ChangeTimeZone(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public DateTime? LastLoginAttempt
            {
                get
                {
                    return this.lastLoginAttempt;
                }

                set
                {
                    this.lastLoginAttempt = value;
                    this.Delta.ChangeLastLoginAttempt(value);
                }
            }

            public DateTime? LastSuccessfulLogin
            {
                get
                {
                    return this.lastSuccessfulLogin;
                }

                set
                {
                    this.lastSuccessfulLogin = value;
                    this.Delta.ChangeLastSuccessfulLogin(value);
                }
            }

            public int ConsecutiveLoginFailures
            {
                get
                {
                    return this.consecutiveLoginFailures;
                }

                set
                {
                    this.consecutiveLoginFailures = value;
                    this.Delta.ChangeConsecutiveLoginFailures(value);
                }
            }

            public bool IsEnabled
            {
                get
                {
                    return this.isEnabled;
                }

                set
                {
                    this.isEnabled = value;
                    this.Delta.ChangeIsEnabled(value);
                }
            }

            public TenantReadableModel OwnerTenant
            {
                get
                {
                    return this.ownerTenant;
                }

                set
                {
                    this.ownerTenant = value;
                    this.Delta.ChangeOwnerTenant(value);
                }
            }

            public UserReadableModel ReadableModel { get; private set; }

            public static UserWritableModel CreateCopyFrom(UserReadableModel readableModel)
            {
                var model = new UserWritableModel();
                model.Username = readableModel.Username;
                model.Domain = readableModel.Domain;
                model.HashedPassword = readableModel.HashedPassword;
                model.FirstName = readableModel.FirstName;
                model.LastName = readableModel.LastName;
                model.Email = readableModel.Email;
                model.Culture = readableModel.Culture;
                model.TimeZone = readableModel.TimeZone;
                model.Description = readableModel.Description;
                model.LastLoginAttempt = readableModel.LastLoginAttempt;
                model.LastSuccessfulLogin = readableModel.LastSuccessfulLogin;
                model.ConsecutiveLoginFailures = readableModel.ConsecutiveLoginFailures;
                model.IsEnabled = readableModel.IsEnabled;
                model.OwnerTenant = readableModel.OwnerTenant;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Units
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        public partial interface IProductTypeChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Removed;

            Task AddAsync(ProductTypeWritableModel writableModel);

            Task DeleteAsync(ProductTypeReadableModel model);

            Task<ProductTypeReadableModel> GetAsync(int id);

            Task<IEnumerable<ProductTypeReadableModel>> QueryAsync(ProductTypeQuery query = null);
            
            Task<ProductTypeReadableModel> CommitAndVerifyAsync(ProductTypeWritableModel writableModel);

            ProductTypeReadableModel Wrap(ProductType entity);

            ProductTypeWritableModel Create();

            ProductTypeWritableModel CreateCopy(ProductTypeReadableModel model);
        }

        public partial class ProductTypeReadableModel : ReadableModelBase<ProductTypeDelta>
        {
            protected readonly ObservableReadOnlyCollection<UnitReadableModel> units =
                new ObservableReadOnlyCollection<UnitReadableModel>();

            protected readonly ProductType ProductType;

            protected readonly object locker = new object();

            private volatile bool populated;

            public ProductTypeReadableModel(ProductType entity)
            {
                this.ProductType = entity;
            }

            public int Id { get; private set; }

            public virtual IObservableReadOnlyCollection<UnitReadableModel> Units
            {
                get
                {
                    return this.units;
                }
            }

            public UnitTypes UnitType { get; private set; }

            public string Name { get; private set; }

            public string Description { get; private set; }
    
            public virtual XmlData HardwareDescriptor { get; protected set; }

            public override async Task ApplyAsync(ProductTypeDelta delta)
            {
                if (delta.UnitType != null)
                {
                    this.UnitType = delta.UnitType.Value;
                    this.OnPropertyChanged("UnitType");
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.HardwareDescriptor != null)
                {
                    this.HardwareDescriptor = delta.HardwareDescriptor.Value;
                    this.OnPropertyChanged("HardwareDescriptor");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((ProductTypeReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(ProductTypeReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.ProductType.Id;
                this.UnitType = this.ProductType.UnitType;
                this.Name = this.ProductType.Name;
                this.Description = this.ProductType.Description;
                this.HardwareDescriptor = this.ProductType.HardwareDescriptor;
                this.CreatedOn = this.ProductType.CreatedOn;
                this.LastModifiedOn = this.ProductType.LastModifiedOn;
                this.Version = new Version(this.ProductType.Version);
            }

            public ProductTypeWritableModel ToChangeTrackingModel()
            {
                var model = new ProductTypeWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class ProductTypeWritableModel : WritableModelBase<ProductTypeDelta>
        {            private UnitTypes unitType;
            private string name;
            private string description;
            private XmlData hardwareDescriptor;

            internal protected ProductTypeWritableModel(ProductTypeReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.unitType = readableModel.UnitType;
                this.name = readableModel.Name;
                this.description = readableModel.Description;
                this.hardwareDescriptor = readableModel.HardwareDescriptor;
                this.Delta = new ProductTypeDelta(readableModel);
            }

            public ProductTypeWritableModel()
                : base(new Version(0))
            {
                this.Delta = new ProductTypeDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public UnitTypes UnitType
            {
                get
                {
                    return this.unitType;
                }

                set
                {
                    this.unitType = value;
                    this.Delta.ChangeUnitType(value);
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public XmlData HardwareDescriptor
            {
                get
                {
                    return this.hardwareDescriptor;
                }

                set
                {
                    this.hardwareDescriptor = value;
                    this.Delta.ChangeHardwareDescriptor(value);
                }
            }

            public ProductTypeReadableModel ReadableModel { get; private set; }

            public static ProductTypeWritableModel CreateCopyFrom(ProductTypeReadableModel readableModel)
            {
                var model = new ProductTypeWritableModel();
                model.UnitType = readableModel.UnitType;
                model.Name = readableModel.Name;
                model.Description = readableModel.Description;
                model.HardwareDescriptor = readableModel.HardwareDescriptor;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUnitChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Removed;

            Task AddAsync(UnitWritableModel writableModel);

            Task DeleteAsync(UnitReadableModel model);

            Task<UnitReadableModel> GetAsync(int id);

            Task<IEnumerable<UnitReadableModel>> QueryAsync(UnitQuery query = null);
            
            Task<UnitReadableModel> CommitAndVerifyAsync(UnitWritableModel writableModel);

            UnitReadableModel Wrap(Unit entity);

            UnitWritableModel Create();

            UnitWritableModel CreateCopy(UnitReadableModel model);
        }

        public partial class UnitReadableModel : ReadableModelBase<UnitDelta>
        {
            protected readonly ObservableReadOnlyCollection<Update.UpdateCommandReadableModel> updateCommands =
                new ObservableReadOnlyCollection<Update.UpdateCommandReadableModel>();

            protected readonly ObservableReadOnlyDictionary<string, string> userDefinedProperties;

            protected readonly Unit Unit;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UnitReadableModel(Unit entity)
            {
                this.Unit = entity;
                if (entity.UserDefinedProperties == null)
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>();
                }
                else
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>(entity.UserDefinedProperties);
                }
            }

            public IObservableReadOnlyDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.userDefinedProperties;
                }
            }

            public int Id { get; private set; }

            public virtual Membership.TenantReadableModel Tenant { get; protected set; }

            public virtual ProductTypeReadableModel ProductType { get; protected set; }

            public virtual Update.UpdateGroupReadableModel UpdateGroup { get; protected set; }

            public virtual IObservableReadOnlyCollection<Update.UpdateCommandReadableModel> UpdateCommands
            {
                get
                {
                    return this.updateCommands;
                }
            }

            public string Name { get; private set; }

            public string NetworkAddress { get; private set; }

            public string Description { get; private set; }

            public bool IsConnected { get; private set; }

            public override async Task ApplyAsync(UnitDelta delta)
            {
                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.NetworkAddress != null)
                {
                    this.NetworkAddress = delta.NetworkAddress.Value;
                    this.OnPropertyChanged("NetworkAddress");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.IsConnected != null)
                {
                    this.IsConnected = delta.IsConnected.Value;
                    this.OnPropertyChanged("IsConnected");
                }

                if (delta.Tenant != null)
                {
                    var tenantChangeTrackingManager = DependencyResolver.Current.Get<Membership.ITenantChangeTrackingManager>();
                    var id = delta.Tenant.ReferenceId;
                    this.Tenant = id.HasValue ? await tenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Tenant");
                }

                if (delta.ProductType != null)
                {
                    var productTypeChangeTrackingManager = DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                    var id = delta.ProductType.ReferenceId;
                    this.ProductType = id.HasValue ? await productTypeChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("ProductType");
                }

                if (delta.UpdateGroup != null)
                {
                    var updateGroupChangeTrackingManager = DependencyResolver.Current.Get<Update.IUpdateGroupChangeTrackingManager>();
                    var id = delta.UpdateGroup.ReferenceId;
                    this.UpdateGroup = id.HasValue ? await updateGroupChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UpdateGroup");
                }

                if (delta.UserDefinedProperties != null)
                {
                    foreach (var key in delta.UserDefinedProperties.Keys)
                    {
                        var newValue = delta.UserDefinedProperties[key];
                        string oldValue;
                        if (!this.userDefinedProperties.TryGetValue(key, out oldValue) || oldValue != newValue)
                        {
                            this.userDefinedProperties[key] = newValue;
                            this.OnPropertyChanged(key);
                        }
                    }
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UnitReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UnitReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Unit.Id;
                this.Name = this.Unit.Name;
                this.NetworkAddress = this.Unit.NetworkAddress;
                this.Description = this.Unit.Description;
                this.IsConnected = this.Unit.IsConnected;
                this.CreatedOn = this.Unit.CreatedOn;
                this.LastModifiedOn = this.Unit.LastModifiedOn;
                this.Version = new Version(this.Unit.Version);
            }

            public UnitWritableModel ToChangeTrackingModel()
            {
                var model = new UnitWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UnitWritableModel : WritableModelBase<UnitDelta>
        {
            private Membership.TenantReadableModel tenant;

            private ProductTypeReadableModel productType;

            private Update.UpdateGroupReadableModel updateGroup;
            private string name;
            private string networkAddress;
            private string description;
            private bool isConnected;

            internal protected UnitWritableModel(UnitReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.name = readableModel.Name;
                this.networkAddress = readableModel.NetworkAddress;
                this.description = readableModel.Description;
                this.isConnected = readableModel.IsConnected;
                this.tenant = readableModel.Tenant;
                this.productType = readableModel.ProductType;
                this.updateGroup = readableModel.UpdateGroup;
                this.Delta = new UnitDelta(readableModel);

                foreach (var udp in readableModel.UserDefinedProperties)
                {
                    this.Delta.UserDefinedPropertiesDelta[udp.Key] = udp.Value;
                }
            }

            public UnitWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UnitDelta();
            }

            public IDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.Delta.UserDefinedPropertiesDelta;
                }
            } 

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string NetworkAddress
            {
                get
                {
                    return this.networkAddress;
                }

                set
                {
                    this.networkAddress = value;
                    this.Delta.ChangeNetworkAddress(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public bool IsConnected
            {
                get
                {
                    return this.isConnected;
                }

                set
                {
                    this.isConnected = value;
                    this.Delta.ChangeIsConnected(value);
                }
            }

            public Membership.TenantReadableModel Tenant
            {
                get
                {
                    return this.tenant;
                }

                set
                {
                    this.tenant = value;
                    this.Delta.ChangeTenant(value);
                }
            }

            public ProductTypeReadableModel ProductType
            {
                get
                {
                    return this.productType;
                }

                set
                {
                    this.productType = value;
                    this.Delta.ChangeProductType(value);
                }
            }

            public Update.UpdateGroupReadableModel UpdateGroup
            {
                get
                {
                    return this.updateGroup;
                }

                set
                {
                    this.updateGroup = value;
                    this.Delta.ChangeUpdateGroup(value);
                }
            }

            public UnitReadableModel ReadableModel { get; private set; }

            public static UnitWritableModel CreateCopyFrom(UnitReadableModel readableModel)
            {
                var model = new UnitWritableModel();
                model.Name = readableModel.Name;
                model.NetworkAddress = readableModel.NetworkAddress;
                model.Description = readableModel.Description;
                model.IsConnected = readableModel.IsConnected;
                model.Tenant = readableModel.Tenant;
                model.ProductType = readableModel.ProductType;
                model.UpdateGroup = readableModel.UpdateGroup;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Resources
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        public partial interface IContentResourceChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Removed;

            Task AddAsync(ContentResourceWritableModel writableModel);

            Task DeleteAsync(ContentResourceReadableModel model);

            Task<ContentResourceReadableModel> GetAsync(int id);

            Task<IEnumerable<ContentResourceReadableModel>> QueryAsync(ContentResourceQuery query = null);
            
            Task<ContentResourceReadableModel> CommitAndVerifyAsync(ContentResourceWritableModel writableModel);

            ContentResourceReadableModel Wrap(ContentResource entity);

            ContentResourceWritableModel Create();

            ContentResourceWritableModel CreateCopy(ContentResourceReadableModel model);
        }

        public partial class ContentResourceReadableModel : ReadableModelBase<ContentResourceDelta>
        {

            protected readonly ContentResource ContentResource;

            protected readonly object locker = new object();

            private volatile bool populated;

            public ContentResourceReadableModel(ContentResource entity)
            {
                this.ContentResource = entity;
            }

            public int Id { get; private set; }

            public virtual Membership.UserReadableModel UploadingUser { get; protected set; }

            public string OriginalFilename { get; private set; }

            public string Description { get; private set; }

            public string ThumbnailHash { get; private set; }

            public string Hash { get; private set; }

            public HashAlgorithmTypes HashAlgorithmType { get; private set; }

            public string MimeType { get; private set; }

            public long Length { get; private set; }

            public override async Task ApplyAsync(ContentResourceDelta delta)
            {
                if (delta.OriginalFilename != null)
                {
                    this.OriginalFilename = delta.OriginalFilename.Value;
                    this.OnPropertyChanged("OriginalFilename");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.ThumbnailHash != null)
                {
                    this.ThumbnailHash = delta.ThumbnailHash.Value;
                    this.OnPropertyChanged("ThumbnailHash");
                }

                if (delta.Hash != null)
                {
                    this.Hash = delta.Hash.Value;
                    this.OnPropertyChanged("Hash");
                }

                if (delta.HashAlgorithmType != null)
                {
                    this.HashAlgorithmType = delta.HashAlgorithmType.Value;
                    this.OnPropertyChanged("HashAlgorithmType");
                }

                if (delta.MimeType != null)
                {
                    this.MimeType = delta.MimeType.Value;
                    this.OnPropertyChanged("MimeType");
                }

                if (delta.Length != null)
                {
                    this.Length = delta.Length.Value;
                    this.OnPropertyChanged("Length");
                }

                if (delta.UploadingUser != null)
                {
                    var uploadingUserChangeTrackingManager = DependencyResolver.Current.Get<Membership.IUserChangeTrackingManager>();
                    var id = delta.UploadingUser.ReferenceId;
                    this.UploadingUser = id.HasValue ? await uploadingUserChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UploadingUser");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((ContentResourceReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(ContentResourceReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.ContentResource.Id;
                this.OriginalFilename = this.ContentResource.OriginalFilename;
                this.Description = this.ContentResource.Description;
                this.ThumbnailHash = this.ContentResource.ThumbnailHash;
                this.Hash = this.ContentResource.Hash;
                this.HashAlgorithmType = this.ContentResource.HashAlgorithmType;
                this.MimeType = this.ContentResource.MimeType;
                this.Length = this.ContentResource.Length;
                this.CreatedOn = this.ContentResource.CreatedOn;
                this.LastModifiedOn = this.ContentResource.LastModifiedOn;
                this.Version = new Version(this.ContentResource.Version);
            }

            public ContentResourceWritableModel ToChangeTrackingModel()
            {
                var model = new ContentResourceWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class ContentResourceWritableModel : WritableModelBase<ContentResourceDelta>
        {
            private Membership.UserReadableModel uploadingUser;
            private string originalFilename;
            private string description;
            private string thumbnailHash;
            private string hash;
            private HashAlgorithmTypes hashAlgorithmType;
            private string mimeType;
            private long length;

            internal protected ContentResourceWritableModel(ContentResourceReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.originalFilename = readableModel.OriginalFilename;
                this.description = readableModel.Description;
                this.thumbnailHash = readableModel.ThumbnailHash;
                this.hash = readableModel.Hash;
                this.hashAlgorithmType = readableModel.HashAlgorithmType;
                this.mimeType = readableModel.MimeType;
                this.length = readableModel.Length;
                this.uploadingUser = readableModel.UploadingUser;
                this.Delta = new ContentResourceDelta(readableModel);
            }

            public ContentResourceWritableModel()
                : base(new Version(0))
            {
                this.Delta = new ContentResourceDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string OriginalFilename
            {
                get
                {
                    return this.originalFilename;
                }

                set
                {
                    this.originalFilename = value;
                    this.Delta.ChangeOriginalFilename(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public string ThumbnailHash
            {
                get
                {
                    return this.thumbnailHash;
                }

                set
                {
                    this.thumbnailHash = value;
                    this.Delta.ChangeThumbnailHash(value);
                }
            }

            public string Hash
            {
                get
                {
                    return this.hash;
                }

                set
                {
                    this.hash = value;
                    this.Delta.ChangeHash(value);
                }
            }

            public HashAlgorithmTypes HashAlgorithmType
            {
                get
                {
                    return this.hashAlgorithmType;
                }

                set
                {
                    this.hashAlgorithmType = value;
                    this.Delta.ChangeHashAlgorithmType(value);
                }
            }

            public string MimeType
            {
                get
                {
                    return this.mimeType;
                }

                set
                {
                    this.mimeType = value;
                    this.Delta.ChangeMimeType(value);
                }
            }

            public long Length
            {
                get
                {
                    return this.length;
                }

                set
                {
                    this.length = value;
                    this.Delta.ChangeLength(value);
                }
            }

            public Membership.UserReadableModel UploadingUser
            {
                get
                {
                    return this.uploadingUser;
                }

                set
                {
                    this.uploadingUser = value;
                    this.Delta.ChangeUploadingUser(value);
                }
            }

            public ContentResourceReadableModel ReadableModel { get; private set; }

            public static ContentResourceWritableModel CreateCopyFrom(ContentResourceReadableModel readableModel)
            {
                var model = new ContentResourceWritableModel();
                model.OriginalFilename = readableModel.OriginalFilename;
                model.Description = readableModel.Description;
                model.ThumbnailHash = readableModel.ThumbnailHash;
                model.Hash = readableModel.Hash;
                model.HashAlgorithmType = readableModel.HashAlgorithmType;
                model.MimeType = readableModel.MimeType;
                model.Length = readableModel.Length;
                model.UploadingUser = readableModel.UploadingUser;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IResourceChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Removed;

            Task AddAsync(ResourceWritableModel writableModel);

            Task DeleteAsync(ResourceReadableModel model);

            Task<ResourceReadableModel> GetAsync(int id);

            Task<IEnumerable<ResourceReadableModel>> QueryAsync(ResourceQuery query = null);
            
            Task<ResourceReadableModel> CommitAndVerifyAsync(ResourceWritableModel writableModel);

            ResourceReadableModel Wrap(Resource entity);

            ResourceWritableModel Create();

            ResourceWritableModel CreateCopy(ResourceReadableModel model);
        }

        public partial class ResourceReadableModel : ReadableModelBase<ResourceDelta>
        {

            protected readonly Resource Resource;

            protected readonly object locker = new object();

            private volatile bool populated;

            public ResourceReadableModel(Resource entity)
            {
                this.Resource = entity;
            }

            public int Id { get; private set; }

            public virtual Membership.UserReadableModel UploadingUser { get; protected set; }

            public string OriginalFilename { get; private set; }

            public string Description { get; private set; }

            public string Hash { get; private set; }

            public string ThumbnailHash { get; private set; }

            public string MimeType { get; private set; }

            public long Length { get; private set; }

            public override async Task ApplyAsync(ResourceDelta delta)
            {
                if (delta.OriginalFilename != null)
                {
                    this.OriginalFilename = delta.OriginalFilename.Value;
                    this.OnPropertyChanged("OriginalFilename");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Hash != null)
                {
                    this.Hash = delta.Hash.Value;
                    this.OnPropertyChanged("Hash");
                }

                if (delta.ThumbnailHash != null)
                {
                    this.ThumbnailHash = delta.ThumbnailHash.Value;
                    this.OnPropertyChanged("ThumbnailHash");
                }

                if (delta.MimeType != null)
                {
                    this.MimeType = delta.MimeType.Value;
                    this.OnPropertyChanged("MimeType");
                }

                if (delta.Length != null)
                {
                    this.Length = delta.Length.Value;
                    this.OnPropertyChanged("Length");
                }

                if (delta.UploadingUser != null)
                {
                    var uploadingUserChangeTrackingManager = DependencyResolver.Current.Get<Membership.IUserChangeTrackingManager>();
                    var id = delta.UploadingUser.ReferenceId;
                    this.UploadingUser = id.HasValue ? await uploadingUserChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UploadingUser");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((ResourceReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(ResourceReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Resource.Id;
                this.OriginalFilename = this.Resource.OriginalFilename;
                this.Description = this.Resource.Description;
                this.Hash = this.Resource.Hash;
                this.ThumbnailHash = this.Resource.ThumbnailHash;
                this.MimeType = this.Resource.MimeType;
                this.Length = this.Resource.Length;
                this.CreatedOn = this.Resource.CreatedOn;
                this.LastModifiedOn = this.Resource.LastModifiedOn;
                this.Version = new Version(this.Resource.Version);
            }

            public ResourceWritableModel ToChangeTrackingModel()
            {
                var model = new ResourceWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class ResourceWritableModel : WritableModelBase<ResourceDelta>
        {
            private Membership.UserReadableModel uploadingUser;
            private string originalFilename;
            private string description;
            private string hash;
            private string thumbnailHash;
            private string mimeType;
            private long length;

            internal protected ResourceWritableModel(ResourceReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.originalFilename = readableModel.OriginalFilename;
                this.description = readableModel.Description;
                this.hash = readableModel.Hash;
                this.thumbnailHash = readableModel.ThumbnailHash;
                this.mimeType = readableModel.MimeType;
                this.length = readableModel.Length;
                this.uploadingUser = readableModel.UploadingUser;
                this.Delta = new ResourceDelta(readableModel);
            }

            public ResourceWritableModel()
                : base(new Version(0))
            {
                this.Delta = new ResourceDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string OriginalFilename
            {
                get
                {
                    return this.originalFilename;
                }

                set
                {
                    this.originalFilename = value;
                    this.Delta.ChangeOriginalFilename(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public string Hash
            {
                get
                {
                    return this.hash;
                }

                set
                {
                    this.hash = value;
                    this.Delta.ChangeHash(value);
                }
            }

            public string ThumbnailHash
            {
                get
                {
                    return this.thumbnailHash;
                }

                set
                {
                    this.thumbnailHash = value;
                    this.Delta.ChangeThumbnailHash(value);
                }
            }

            public string MimeType
            {
                get
                {
                    return this.mimeType;
                }

                set
                {
                    this.mimeType = value;
                    this.Delta.ChangeMimeType(value);
                }
            }

            public long Length
            {
                get
                {
                    return this.length;
                }

                set
                {
                    this.length = value;
                    this.Delta.ChangeLength(value);
                }
            }

            public Membership.UserReadableModel UploadingUser
            {
                get
                {
                    return this.uploadingUser;
                }

                set
                {
                    this.uploadingUser = value;
                    this.Delta.ChangeUploadingUser(value);
                }
            }

            public ResourceReadableModel ReadableModel { get; private set; }

            public static ResourceWritableModel CreateCopyFrom(ResourceReadableModel readableModel)
            {
                var model = new ResourceWritableModel();
                model.OriginalFilename = readableModel.OriginalFilename;
                model.Description = readableModel.Description;
                model.Hash = readableModel.Hash;
                model.ThumbnailHash = readableModel.ThumbnailHash;
                model.MimeType = readableModel.MimeType;
                model.Length = readableModel.Length;
                model.UploadingUser = readableModel.UploadingUser;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Update
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        public partial interface IUpdateCommandChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Removed;

            Task AddAsync(UpdateCommandWritableModel writableModel);

            Task DeleteAsync(UpdateCommandReadableModel model);

            Task<UpdateCommandReadableModel> GetAsync(int id);

            Task<IEnumerable<UpdateCommandReadableModel>> QueryAsync(UpdateCommandQuery query = null);
            
            Task<UpdateCommandReadableModel> CommitAndVerifyAsync(UpdateCommandWritableModel writableModel);

            UpdateCommandReadableModel Wrap(UpdateCommand entity);

            UpdateCommandWritableModel Create();

            UpdateCommandWritableModel CreateCopy(UpdateCommandReadableModel model);
        }

        public partial class UpdateCommandReadableModel : ReadableModelBase<UpdateCommandDelta>
        {
            protected readonly ObservableReadOnlyCollection<UpdatePartReadableModel> includedParts =
                new ObservableReadOnlyCollection<UpdatePartReadableModel>();
            protected readonly ObservableReadOnlyCollection<UpdateFeedbackReadableModel> feedbacks =
                new ObservableReadOnlyCollection<UpdateFeedbackReadableModel>();

            protected readonly UpdateCommand UpdateCommand;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UpdateCommandReadableModel(UpdateCommand entity)
            {
                this.UpdateCommand = entity;
            }

            public int Id { get; private set; }

            public virtual Units.UnitReadableModel Unit { get; protected set; }

            public virtual IObservableReadOnlyCollection<UpdatePartReadableModel> IncludedParts
            {
                get
                {
                    return this.includedParts;
                }
            }

            public virtual IObservableReadOnlyCollection<UpdateFeedbackReadableModel> Feedbacks
            {
                get
                {
                    return this.feedbacks;
                }
            }

            public int UpdateIndex { get; private set; }

            public bool WasTransferred { get; private set; }

            public bool WasInstalled { get; private set; }
    
            public virtual XmlData Command { get; protected set; }

            public override async Task ApplyAsync(UpdateCommandDelta delta)
            {
                if (delta.UpdateIndex != null)
                {
                    this.UpdateIndex = delta.UpdateIndex.Value;
                    this.OnPropertyChanged("UpdateIndex");
                }

                if (delta.WasTransferred != null)
                {
                    this.WasTransferred = delta.WasTransferred.Value;
                    this.OnPropertyChanged("WasTransferred");
                }

                if (delta.WasInstalled != null)
                {
                    this.WasInstalled = delta.WasInstalled.Value;
                    this.OnPropertyChanged("WasInstalled");
                }

                if (delta.Command != null)
                {
                    this.Command = delta.Command.Value;
                    this.OnPropertyChanged("Command");
                }

                if (delta.Unit != null)
                {
                    var unitChangeTrackingManager = DependencyResolver.Current.Get<Units.IUnitChangeTrackingManager>();
                    var id = delta.Unit.ReferenceId;
                    this.Unit = id.HasValue ? await unitChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Unit");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UpdateCommandReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UpdateCommandReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UpdateCommand.Id;
                this.UpdateIndex = this.UpdateCommand.UpdateIndex;
                this.WasTransferred = this.UpdateCommand.WasTransferred;
                this.WasInstalled = this.UpdateCommand.WasInstalled;
                this.Command = this.UpdateCommand.Command;
                this.CreatedOn = this.UpdateCommand.CreatedOn;
                this.LastModifiedOn = this.UpdateCommand.LastModifiedOn;
                this.Version = new Version(this.UpdateCommand.Version);
            }

            public UpdateCommandWritableModel ToChangeTrackingModel()
            {
                var model = new UpdateCommandWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UpdateCommandWritableModel : WritableModelBase<UpdateCommandDelta>
        {
            private Units.UnitReadableModel unit;
            private int updateIndex;
            private bool wasTransferred;
            private bool wasInstalled;
            private XmlData command;

            internal protected UpdateCommandWritableModel(UpdateCommandReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.updateIndex = readableModel.UpdateIndex;
                this.wasTransferred = readableModel.WasTransferred;
                this.wasInstalled = readableModel.WasInstalled;
                this.unit = readableModel.Unit;
                this.command = readableModel.Command;
                this.Delta = new UpdateCommandDelta(readableModel);
            }

            public UpdateCommandWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UpdateCommandDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public int UpdateIndex
            {
                get
                {
                    return this.updateIndex;
                }

                set
                {
                    this.updateIndex = value;
                    this.Delta.ChangeUpdateIndex(value);
                }
            }

            public bool WasTransferred
            {
                get
                {
                    return this.wasTransferred;
                }

                set
                {
                    this.wasTransferred = value;
                    this.Delta.ChangeWasTransferred(value);
                }
            }

            public bool WasInstalled
            {
                get
                {
                    return this.wasInstalled;
                }

                set
                {
                    this.wasInstalled = value;
                    this.Delta.ChangeWasInstalled(value);
                }
            }

            public XmlData Command
            {
                get
                {
                    return this.command;
                }

                set
                {
                    this.command = value;
                    this.Delta.ChangeCommand(value);
                }
            }

            public Units.UnitReadableModel Unit
            {
                get
                {
                    return this.unit;
                }

                set
                {
                    this.unit = value;
                    this.Delta.ChangeUnit(value);
                }
            }

            public UpdateCommandReadableModel ReadableModel { get; private set; }

            public static UpdateCommandWritableModel CreateCopyFrom(UpdateCommandReadableModel readableModel)
            {
                var model = new UpdateCommandWritableModel();
                model.UpdateIndex = readableModel.UpdateIndex;
                model.WasTransferred = readableModel.WasTransferred;
                model.WasInstalled = readableModel.WasInstalled;
                model.Unit = readableModel.Unit;
                model.Command = readableModel.Command;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUpdateFeedbackChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Removed;

            Task AddAsync(UpdateFeedbackWritableModel writableModel);

            Task DeleteAsync(UpdateFeedbackReadableModel model);

            Task<UpdateFeedbackReadableModel> GetAsync(int id);

            Task<IEnumerable<UpdateFeedbackReadableModel>> QueryAsync(UpdateFeedbackQuery query = null);
            
            Task<UpdateFeedbackReadableModel> CommitAndVerifyAsync(UpdateFeedbackWritableModel writableModel);

            UpdateFeedbackReadableModel Wrap(UpdateFeedback entity);

            UpdateFeedbackWritableModel Create();

            UpdateFeedbackWritableModel CreateCopy(UpdateFeedbackReadableModel model);
        }

        public partial class UpdateFeedbackReadableModel : ReadableModelBase<UpdateFeedbackDelta>
        {

            protected readonly UpdateFeedback UpdateFeedback;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UpdateFeedbackReadableModel(UpdateFeedback entity)
            {
                this.UpdateFeedback = entity;
            }

            public int Id { get; private set; }

            public virtual UpdateCommandReadableModel UpdateCommand { get; protected set; }

            public DateTime Timestamp { get; private set; }

            public UpdateState State { get; private set; }
    
            public virtual XmlData Feedback { get; protected set; }

            public override async Task ApplyAsync(UpdateFeedbackDelta delta)
            {
                if (delta.Timestamp != null)
                {
                    this.Timestamp = delta.Timestamp.Value;
                    this.OnPropertyChanged("Timestamp");
                }

                if (delta.State != null)
                {
                    this.State = delta.State.Value;
                    this.OnPropertyChanged("State");
                }

                if (delta.Feedback != null)
                {
                    this.Feedback = delta.Feedback.Value;
                    this.OnPropertyChanged("Feedback");
                }

                if (delta.UpdateCommand != null)
                {
                    var updateCommandChangeTrackingManager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                    var id = delta.UpdateCommand.ReferenceId;
                    this.UpdateCommand = id.HasValue ? await updateCommandChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UpdateCommand");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UpdateFeedbackReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UpdateFeedbackReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UpdateFeedback.Id;
                this.Timestamp = this.UpdateFeedback.Timestamp;
                this.State = this.UpdateFeedback.State;
                this.Feedback = this.UpdateFeedback.Feedback;
                this.CreatedOn = this.UpdateFeedback.CreatedOn;
                this.LastModifiedOn = this.UpdateFeedback.LastModifiedOn;
                this.Version = new Version(this.UpdateFeedback.Version);
            }

            public UpdateFeedbackWritableModel ToChangeTrackingModel()
            {
                var model = new UpdateFeedbackWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UpdateFeedbackWritableModel : WritableModelBase<UpdateFeedbackDelta>
        {
            private UpdateCommandReadableModel updateCommand;
            private DateTime timestamp;
            private UpdateState state;
            private XmlData feedback;

            internal protected UpdateFeedbackWritableModel(UpdateFeedbackReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.timestamp = readableModel.Timestamp;
                this.state = readableModel.State;
                this.updateCommand = readableModel.UpdateCommand;
                this.feedback = readableModel.Feedback;
                this.Delta = new UpdateFeedbackDelta(readableModel);
            }

            public UpdateFeedbackWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UpdateFeedbackDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public DateTime Timestamp
            {
                get
                {
                    return this.timestamp;
                }

                set
                {
                    this.timestamp = value;
                    this.Delta.ChangeTimestamp(value);
                }
            }

            public UpdateState State
            {
                get
                {
                    return this.state;
                }

                set
                {
                    this.state = value;
                    this.Delta.ChangeState(value);
                }
            }

            public XmlData Feedback
            {
                get
                {
                    return this.feedback;
                }

                set
                {
                    this.feedback = value;
                    this.Delta.ChangeFeedback(value);
                }
            }

            public UpdateCommandReadableModel UpdateCommand
            {
                get
                {
                    return this.updateCommand;
                }

                set
                {
                    this.updateCommand = value;
                    this.Delta.ChangeUpdateCommand(value);
                }
            }

            public UpdateFeedbackReadableModel ReadableModel { get; private set; }

            public static UpdateFeedbackWritableModel CreateCopyFrom(UpdateFeedbackReadableModel readableModel)
            {
                var model = new UpdateFeedbackWritableModel();
                model.Timestamp = readableModel.Timestamp;
                model.State = readableModel.State;
                model.UpdateCommand = readableModel.UpdateCommand;
                model.Feedback = readableModel.Feedback;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUpdateGroupChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Removed;

            Task AddAsync(UpdateGroupWritableModel writableModel);

            Task DeleteAsync(UpdateGroupReadableModel model);

            Task<UpdateGroupReadableModel> GetAsync(int id);

            Task<IEnumerable<UpdateGroupReadableModel>> QueryAsync(UpdateGroupQuery query = null);
            
            Task<UpdateGroupReadableModel> CommitAndVerifyAsync(UpdateGroupWritableModel writableModel);

            UpdateGroupReadableModel Wrap(UpdateGroup entity);

            UpdateGroupWritableModel Create();

            UpdateGroupWritableModel CreateCopy(UpdateGroupReadableModel model);
        }

        public partial class UpdateGroupReadableModel : ReadableModelBase<UpdateGroupDelta>
        {
            protected readonly ObservableReadOnlyCollection<Units.UnitReadableModel> units =
                new ObservableReadOnlyCollection<Units.UnitReadableModel>();
            protected readonly ObservableReadOnlyCollection<UpdatePartReadableModel> updateParts =
                new ObservableReadOnlyCollection<UpdatePartReadableModel>();

            protected readonly ObservableReadOnlyDictionary<string, string> userDefinedProperties;

            protected readonly UpdateGroup UpdateGroup;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UpdateGroupReadableModel(UpdateGroup entity)
            {
                this.UpdateGroup = entity;
                if (entity.UserDefinedProperties == null)
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>();
                }
                else
                {
                    this.userDefinedProperties = new ObservableReadOnlyDictionary<string, string>(entity.UserDefinedProperties);
                }
            }

            public IObservableReadOnlyDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.userDefinedProperties;
                }
            }

            public int Id { get; private set; }

            public virtual Membership.TenantReadableModel Tenant { get; protected set; }

            public virtual Configurations.UnitConfigurationReadableModel UnitConfiguration { get; protected set; }

            public virtual Configurations.MediaConfigurationReadableModel MediaConfiguration { get; protected set; }

            public virtual IObservableReadOnlyCollection<Units.UnitReadableModel> Units
            {
                get
                {
                    return this.units;
                }
            }

            public virtual IObservableReadOnlyCollection<UpdatePartReadableModel> UpdateParts
            {
                get
                {
                    return this.updateParts;
                }
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public override async Task ApplyAsync(UpdateGroupDelta delta)
            {
                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Tenant != null)
                {
                    var tenantChangeTrackingManager = DependencyResolver.Current.Get<Membership.ITenantChangeTrackingManager>();
                    var id = delta.Tenant.ReferenceId;
                    this.Tenant = id.HasValue ? await tenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Tenant");
                }

                if (delta.UnitConfiguration != null)
                {
                    var unitConfigurationChangeTrackingManager = DependencyResolver.Current.Get<Configurations.IUnitConfigurationChangeTrackingManager>();
                    var id = delta.UnitConfiguration.ReferenceId;
                    this.UnitConfiguration = id.HasValue ? await unitConfigurationChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UnitConfiguration");
                }

                if (delta.MediaConfiguration != null)
                {
                    var mediaConfigurationChangeTrackingManager = DependencyResolver.Current.Get<Configurations.IMediaConfigurationChangeTrackingManager>();
                    var id = delta.MediaConfiguration.ReferenceId;
                    this.MediaConfiguration = id.HasValue ? await mediaConfigurationChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("MediaConfiguration");
                }

                if (delta.UserDefinedProperties != null)
                {
                    foreach (var key in delta.UserDefinedProperties.Keys)
                    {
                        var newValue = delta.UserDefinedProperties[key];
                        string oldValue;
                        if (!this.userDefinedProperties.TryGetValue(key, out oldValue) || oldValue != newValue)
                        {
                            this.userDefinedProperties[key] = newValue;
                            this.OnPropertyChanged(key);
                        }
                    }
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UpdateGroupReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UpdateGroupReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UpdateGroup.Id;
                this.Name = this.UpdateGroup.Name;
                this.Description = this.UpdateGroup.Description;
                this.CreatedOn = this.UpdateGroup.CreatedOn;
                this.LastModifiedOn = this.UpdateGroup.LastModifiedOn;
                this.Version = new Version(this.UpdateGroup.Version);
            }

            public UpdateGroupWritableModel ToChangeTrackingModel()
            {
                var model = new UpdateGroupWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UpdateGroupWritableModel : WritableModelBase<UpdateGroupDelta>
        {
            private Membership.TenantReadableModel tenant;

            private Configurations.UnitConfigurationReadableModel unitConfiguration;

            private Configurations.MediaConfigurationReadableModel mediaConfiguration;
            private string name;
            private string description;

            internal protected UpdateGroupWritableModel(UpdateGroupReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.name = readableModel.Name;
                this.description = readableModel.Description;
                this.tenant = readableModel.Tenant;
                this.unitConfiguration = readableModel.UnitConfiguration;
                this.mediaConfiguration = readableModel.MediaConfiguration;
                this.Delta = new UpdateGroupDelta(readableModel);

                foreach (var udp in readableModel.UserDefinedProperties)
                {
                    this.Delta.UserDefinedPropertiesDelta[udp.Key] = udp.Value;
                }
            }

            public UpdateGroupWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UpdateGroupDelta();
            }

            public IDictionary<string, string> UserDefinedProperties
            {
                get
                {
                    return this.Delta.UserDefinedPropertiesDelta;
                }
            } 

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public Membership.TenantReadableModel Tenant
            {
                get
                {
                    return this.tenant;
                }

                set
                {
                    this.tenant = value;
                    this.Delta.ChangeTenant(value);
                }
            }

            public Configurations.UnitConfigurationReadableModel UnitConfiguration
            {
                get
                {
                    return this.unitConfiguration;
                }

                set
                {
                    this.unitConfiguration = value;
                    this.Delta.ChangeUnitConfiguration(value);
                }
            }

            public Configurations.MediaConfigurationReadableModel MediaConfiguration
            {
                get
                {
                    return this.mediaConfiguration;
                }

                set
                {
                    this.mediaConfiguration = value;
                    this.Delta.ChangeMediaConfiguration(value);
                }
            }

            public UpdateGroupReadableModel ReadableModel { get; private set; }

            public static UpdateGroupWritableModel CreateCopyFrom(UpdateGroupReadableModel readableModel)
            {
                var model = new UpdateGroupWritableModel();
                model.Name = readableModel.Name;
                model.Description = readableModel.Description;
                model.Tenant = readableModel.Tenant;
                model.UnitConfiguration = readableModel.UnitConfiguration;
                model.MediaConfiguration = readableModel.MediaConfiguration;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUpdatePartChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Removed;

            Task AddAsync(UpdatePartWritableModel writableModel);

            Task DeleteAsync(UpdatePartReadableModel model);

            Task<UpdatePartReadableModel> GetAsync(int id);

            Task<IEnumerable<UpdatePartReadableModel>> QueryAsync(UpdatePartQuery query = null);
            
            Task<UpdatePartReadableModel> CommitAndVerifyAsync(UpdatePartWritableModel writableModel);

            UpdatePartReadableModel Wrap(UpdatePart entity);

            UpdatePartWritableModel Create();

            UpdatePartWritableModel CreateCopy(UpdatePartReadableModel model);
        }

        public partial class UpdatePartReadableModel : ReadableModelBase<UpdatePartDelta>
        {
            protected readonly ObservableReadOnlyCollection<UpdateCommandReadableModel> relatedCommands =
                new ObservableReadOnlyCollection<UpdateCommandReadableModel>();

            protected readonly UpdatePart UpdatePart;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UpdatePartReadableModel(UpdatePart entity)
            {
                this.UpdatePart = entity;
            }

            public int Id { get; private set; }

            public virtual UpdateGroupReadableModel UpdateGroup { get; protected set; }

            public virtual IObservableReadOnlyCollection<UpdateCommandReadableModel> RelatedCommands
            {
                get
                {
                    return this.relatedCommands;
                }
            }

            public UpdatePartType Type { get; private set; }

            public DateTime Start { get; private set; }

            public DateTime End { get; private set; }

            public string Description { get; private set; }
    
            public virtual XmlData Structure { get; protected set; }
    
            public virtual XmlData InstallInstructions { get; protected set; }
    
            public virtual XmlData DynamicContent { get; protected set; }

            public override async Task ApplyAsync(UpdatePartDelta delta)
            {
                if (delta.Type != null)
                {
                    this.Type = delta.Type.Value;
                    this.OnPropertyChanged("Type");
                }

                if (delta.Start != null)
                {
                    this.Start = delta.Start.Value;
                    this.OnPropertyChanged("Start");
                }

                if (delta.End != null)
                {
                    this.End = delta.End.Value;
                    this.OnPropertyChanged("End");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Structure != null)
                {
                    this.Structure = delta.Structure.Value;
                    this.OnPropertyChanged("Structure");
                }

                if (delta.InstallInstructions != null)
                {
                    this.InstallInstructions = delta.InstallInstructions.Value;
                    this.OnPropertyChanged("InstallInstructions");
                }

                if (delta.DynamicContent != null)
                {
                    this.DynamicContent = delta.DynamicContent.Value;
                    this.OnPropertyChanged("DynamicContent");
                }

                if (delta.UpdateGroup != null)
                {
                    var updateGroupChangeTrackingManager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    var id = delta.UpdateGroup.ReferenceId;
                    this.UpdateGroup = id.HasValue ? await updateGroupChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("UpdateGroup");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UpdatePartReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UpdatePartReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UpdatePart.Id;
                this.Type = this.UpdatePart.Type;
                this.Start = this.UpdatePart.Start;
                this.End = this.UpdatePart.End;
                this.Description = this.UpdatePart.Description;
                this.Structure = this.UpdatePart.Structure;
                this.InstallInstructions = this.UpdatePart.InstallInstructions;
                this.DynamicContent = this.UpdatePart.DynamicContent;
                this.CreatedOn = this.UpdatePart.CreatedOn;
                this.LastModifiedOn = this.UpdatePart.LastModifiedOn;
                this.Version = new Version(this.UpdatePart.Version);
            }

            public UpdatePartWritableModel ToChangeTrackingModel()
            {
                var model = new UpdatePartWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UpdatePartWritableModel : WritableModelBase<UpdatePartDelta>
        {
            private UpdateGroupReadableModel updateGroup;
            private UpdatePartType type;
            private DateTime start;
            private DateTime end;
            private string description;
            private XmlData structure;
            private XmlData installInstructions;
            private XmlData dynamicContent;

            internal protected UpdatePartWritableModel(UpdatePartReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.type = readableModel.Type;
                this.start = readableModel.Start;
                this.end = readableModel.End;
                this.description = readableModel.Description;
                this.updateGroup = readableModel.UpdateGroup;
                this.structure = readableModel.Structure;
                this.installInstructions = readableModel.InstallInstructions;
                this.dynamicContent = readableModel.DynamicContent;
                this.Delta = new UpdatePartDelta(readableModel);
            }

            public UpdatePartWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UpdatePartDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public UpdatePartType Type
            {
                get
                {
                    return this.type;
                }

                set
                {
                    this.type = value;
                    this.Delta.ChangeType(value);
                }
            }

            public DateTime Start
            {
                get
                {
                    return this.start;
                }

                set
                {
                    this.start = value;
                    this.Delta.ChangeStart(value);
                }
            }

            public DateTime End
            {
                get
                {
                    return this.end;
                }

                set
                {
                    this.end = value;
                    this.Delta.ChangeEnd(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public XmlData Structure
            {
                get
                {
                    return this.structure;
                }

                set
                {
                    this.structure = value;
                    this.Delta.ChangeStructure(value);
                }
            }

            public XmlData InstallInstructions
            {
                get
                {
                    return this.installInstructions;
                }

                set
                {
                    this.installInstructions = value;
                    this.Delta.ChangeInstallInstructions(value);
                }
            }

            public XmlData DynamicContent
            {
                get
                {
                    return this.dynamicContent;
                }

                set
                {
                    this.dynamicContent = value;
                    this.Delta.ChangeDynamicContent(value);
                }
            }

            public UpdateGroupReadableModel UpdateGroup
            {
                get
                {
                    return this.updateGroup;
                }

                set
                {
                    this.updateGroup = value;
                    this.Delta.ChangeUpdateGroup(value);
                }
            }

            public UpdatePartReadableModel ReadableModel { get; private set; }

            public static UpdatePartWritableModel CreateCopyFrom(UpdatePartReadableModel readableModel)
            {
                var model = new UpdatePartWritableModel();
                model.Type = readableModel.Type;
                model.Start = readableModel.Start;
                model.End = readableModel.End;
                model.Description = readableModel.Description;
                model.UpdateGroup = readableModel.UpdateGroup;
                model.Structure = readableModel.Structure;
                model.InstallInstructions = readableModel.InstallInstructions;
                model.DynamicContent = readableModel.DynamicContent;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Documents
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        public partial interface IDocumentChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Removed;

            Task AddAsync(DocumentWritableModel writableModel);

            Task DeleteAsync(DocumentReadableModel model);

            Task<DocumentReadableModel> GetAsync(int id);

            Task<IEnumerable<DocumentReadableModel>> QueryAsync(DocumentQuery query = null);
            
            Task<DocumentReadableModel> CommitAndVerifyAsync(DocumentWritableModel writableModel);

            DocumentReadableModel Wrap(Document entity);

            DocumentWritableModel Create();

            DocumentWritableModel CreateCopy(DocumentReadableModel model);
        }

        public partial class DocumentReadableModel : ReadableModelBase<DocumentDelta>
        {
            protected readonly ObservableReadOnlyCollection<DocumentVersionReadableModel> versions =
                new ObservableReadOnlyCollection<DocumentVersionReadableModel>();

            protected readonly Document Document;

            protected readonly object locker = new object();

            private volatile bool populated;

            public DocumentReadableModel(Document entity)
            {
                this.Document = entity;
            }

            public int Id { get; private set; }

            public virtual Membership.TenantReadableModel Tenant { get; protected set; }

            public virtual IObservableReadOnlyCollection<DocumentVersionReadableModel> Versions
            {
                get
                {
                    return this.versions;
                }
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public override async Task ApplyAsync(DocumentDelta delta)
            {
                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Tenant != null)
                {
                    var tenantChangeTrackingManager = DependencyResolver.Current.Get<Membership.ITenantChangeTrackingManager>();
                    var id = delta.Tenant.ReferenceId;
                    this.Tenant = id.HasValue ? await tenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Tenant");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((DocumentReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(DocumentReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Document.Id;
                this.Name = this.Document.Name;
                this.Description = this.Document.Description;
                this.CreatedOn = this.Document.CreatedOn;
                this.LastModifiedOn = this.Document.LastModifiedOn;
                this.Version = new Version(this.Document.Version);
            }

            public DocumentWritableModel ToChangeTrackingModel()
            {
                var model = new DocumentWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class DocumentWritableModel : WritableModelBase<DocumentDelta>
        {
            private Membership.TenantReadableModel tenant;
            private string name;
            private string description;

            internal protected DocumentWritableModel(DocumentReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.name = readableModel.Name;
                this.description = readableModel.Description;
                this.tenant = readableModel.Tenant;
                this.Delta = new DocumentDelta(readableModel);
            }

            public DocumentWritableModel()
                : base(new Version(0))
            {
                this.Delta = new DocumentDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public Membership.TenantReadableModel Tenant
            {
                get
                {
                    return this.tenant;
                }

                set
                {
                    this.tenant = value;
                    this.Delta.ChangeTenant(value);
                }
            }

            public DocumentReadableModel ReadableModel { get; private set; }

            public static DocumentWritableModel CreateCopyFrom(DocumentReadableModel readableModel)
            {
                var model = new DocumentWritableModel();
                model.Name = readableModel.Name;
                model.Description = readableModel.Description;
                model.Tenant = readableModel.Tenant;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IDocumentVersionChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Removed;

            Task AddAsync(DocumentVersionWritableModel writableModel);

            Task DeleteAsync(DocumentVersionReadableModel model);

            Task<DocumentVersionReadableModel> GetAsync(int id);

            Task<IEnumerable<DocumentVersionReadableModel>> QueryAsync(DocumentVersionQuery query = null);
            
            Task<DocumentVersionReadableModel> CommitAndVerifyAsync(DocumentVersionWritableModel writableModel);

            DocumentVersionReadableModel Wrap(DocumentVersion entity);

            DocumentVersionWritableModel Create();

            DocumentVersionWritableModel CreateCopy(DocumentVersionReadableModel model);
        }

        public partial class DocumentVersionReadableModel : ReadableModelBase<DocumentVersionDelta>
        {

            protected readonly DocumentVersion DocumentVersion;

            protected readonly object locker = new object();

            private volatile bool populated;

            public DocumentVersionReadableModel(DocumentVersion entity)
            {
                this.DocumentVersion = entity;
            }

            public int Id { get; private set; }

            public virtual DocumentReadableModel Document { get; protected set; }

            public virtual Membership.UserReadableModel CreatingUser { get; protected set; }

            public int Major { get; private set; }

            public int Minor { get; private set; }

            public string Description { get; private set; }
    
            public virtual XmlData Content { get; protected set; }

            public override async Task ApplyAsync(DocumentVersionDelta delta)
            {
                if (delta.Major != null)
                {
                    this.Major = delta.Major.Value;
                    this.OnPropertyChanged("Major");
                }

                if (delta.Minor != null)
                {
                    this.Minor = delta.Minor.Value;
                    this.OnPropertyChanged("Minor");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Content != null)
                {
                    this.Content = delta.Content.Value;
                    this.OnPropertyChanged("Content");
                }

                if (delta.Document != null)
                {
                    var documentChangeTrackingManager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                    var id = delta.Document.ReferenceId;
                    this.Document = id.HasValue ? await documentChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Document");
                }

                if (delta.CreatingUser != null)
                {
                    var creatingUserChangeTrackingManager = DependencyResolver.Current.Get<Membership.IUserChangeTrackingManager>();
                    var id = delta.CreatingUser.ReferenceId;
                    this.CreatingUser = id.HasValue ? await creatingUserChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("CreatingUser");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((DocumentVersionReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(DocumentVersionReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.DocumentVersion.Id;
                this.Major = this.DocumentVersion.Major;
                this.Minor = this.DocumentVersion.Minor;
                this.Description = this.DocumentVersion.Description;
                this.Content = this.DocumentVersion.Content;
                this.CreatedOn = this.DocumentVersion.CreatedOn;
                this.LastModifiedOn = this.DocumentVersion.LastModifiedOn;
                this.Version = new Version(this.DocumentVersion.Version);
            }

            public DocumentVersionWritableModel ToChangeTrackingModel()
            {
                var model = new DocumentVersionWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class DocumentVersionWritableModel : WritableModelBase<DocumentVersionDelta>
        {
            private DocumentReadableModel document;

            private Membership.UserReadableModel creatingUser;
            private int major;
            private int minor;
            private string description;
            private XmlData content;

            internal protected DocumentVersionWritableModel(DocumentVersionReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.major = readableModel.Major;
                this.minor = readableModel.Minor;
                this.description = readableModel.Description;
                this.document = readableModel.Document;
                this.creatingUser = readableModel.CreatingUser;
                this.content = readableModel.Content;
                this.Delta = new DocumentVersionDelta(readableModel);
            }

            public DocumentVersionWritableModel()
                : base(new Version(0))
            {
                this.Delta = new DocumentVersionDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public int Major
            {
                get
                {
                    return this.major;
                }

                set
                {
                    this.major = value;
                    this.Delta.ChangeMajor(value);
                }
            }

            public int Minor
            {
                get
                {
                    return this.minor;
                }

                set
                {
                    this.minor = value;
                    this.Delta.ChangeMinor(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public XmlData Content
            {
                get
                {
                    return this.content;
                }

                set
                {
                    this.content = value;
                    this.Delta.ChangeContent(value);
                }
            }

            public DocumentReadableModel Document
            {
                get
                {
                    return this.document;
                }

                set
                {
                    this.document = value;
                    this.Delta.ChangeDocument(value);
                }
            }

            public Membership.UserReadableModel CreatingUser
            {
                get
                {
                    return this.creatingUser;
                }

                set
                {
                    this.creatingUser = value;
                    this.Delta.ChangeCreatingUser(value);
                }
            }

            public DocumentVersionReadableModel ReadableModel { get; private set; }

            public static DocumentVersionWritableModel CreateCopyFrom(DocumentVersionReadableModel readableModel)
            {
                var model = new DocumentVersionWritableModel();
                model.Major = readableModel.Major;
                model.Minor = readableModel.Minor;
                model.Description = readableModel.Description;
                model.Document = readableModel.Document;
                model.CreatingUser = readableModel.CreatingUser;
                model.Content = readableModel.Content;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Software
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        public partial interface IPackageChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Removed;

            Task AddAsync(PackageWritableModel writableModel);

            Task DeleteAsync(PackageReadableModel model);

            Task<PackageReadableModel> GetAsync(int id);

            Task<IEnumerable<PackageReadableModel>> QueryAsync(PackageQuery query = null);
            
            Task<PackageReadableModel> CommitAndVerifyAsync(PackageWritableModel writableModel);

            PackageReadableModel Wrap(Package entity);

            PackageWritableModel Create();

            PackageWritableModel CreateCopy(PackageReadableModel model);
        }

        public partial class PackageReadableModel : ReadableModelBase<PackageDelta>
        {
            protected readonly ObservableReadOnlyCollection<PackageVersionReadableModel> versions =
                new ObservableReadOnlyCollection<PackageVersionReadableModel>();

            protected readonly Package Package;

            protected readonly object locker = new object();

            private volatile bool populated;

            public PackageReadableModel(Package entity)
            {
                this.Package = entity;
            }

            public int Id { get; private set; }

            public virtual IObservableReadOnlyCollection<PackageVersionReadableModel> Versions
            {
                get
                {
                    return this.versions;
                }
            }

            public string PackageId { get; private set; }

            public string ProductName { get; private set; }

            public string Description { get; private set; }

            public override async Task ApplyAsync(PackageDelta delta)
            {
                if (delta.PackageId != null)
                {
                    this.PackageId = delta.PackageId.Value;
                    this.OnPropertyChanged("PackageId");
                }

                if (delta.ProductName != null)
                {
                    this.ProductName = delta.ProductName.Value;
                    this.OnPropertyChanged("ProductName");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((PackageReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(PackageReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.Package.Id;
                this.PackageId = this.Package.PackageId;
                this.ProductName = this.Package.ProductName;
                this.Description = this.Package.Description;
                this.CreatedOn = this.Package.CreatedOn;
                this.LastModifiedOn = this.Package.LastModifiedOn;
                this.Version = new Version(this.Package.Version);
            }

            public PackageWritableModel ToChangeTrackingModel()
            {
                var model = new PackageWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class PackageWritableModel : WritableModelBase<PackageDelta>
        {            private string packageId;
            private string productName;
            private string description;

            internal protected PackageWritableModel(PackageReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.packageId = readableModel.PackageId;
                this.productName = readableModel.ProductName;
                this.description = readableModel.Description;
                this.Delta = new PackageDelta(readableModel);
            }

            public PackageWritableModel()
                : base(new Version(0))
            {
                this.Delta = new PackageDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string PackageId
            {
                get
                {
                    return this.packageId;
                }

                set
                {
                    this.packageId = value;
                    this.Delta.ChangePackageId(value);
                }
            }

            public string ProductName
            {
                get
                {
                    return this.productName;
                }

                set
                {
                    this.productName = value;
                    this.Delta.ChangeProductName(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public PackageReadableModel ReadableModel { get; private set; }

            public static PackageWritableModel CreateCopyFrom(PackageReadableModel readableModel)
            {
                var model = new PackageWritableModel();
                model.PackageId = readableModel.PackageId;
                model.ProductName = readableModel.ProductName;
                model.Description = readableModel.Description;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IPackageVersionChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Removed;

            Task AddAsync(PackageVersionWritableModel writableModel);

            Task DeleteAsync(PackageVersionReadableModel model);

            Task<PackageVersionReadableModel> GetAsync(int id);

            Task<IEnumerable<PackageVersionReadableModel>> QueryAsync(PackageVersionQuery query = null);
            
            Task<PackageVersionReadableModel> CommitAndVerifyAsync(PackageVersionWritableModel writableModel);

            PackageVersionReadableModel Wrap(PackageVersion entity);

            PackageVersionWritableModel Create();

            PackageVersionWritableModel CreateCopy(PackageVersionReadableModel model);
        }

        public partial class PackageVersionReadableModel : ReadableModelBase<PackageVersionDelta>
        {

            protected readonly PackageVersion PackageVersion;

            protected readonly object locker = new object();

            private volatile bool populated;

            public PackageVersionReadableModel(PackageVersion entity)
            {
                this.PackageVersion = entity;
            }

            public int Id { get; private set; }

            public virtual PackageReadableModel Package { get; protected set; }

            public string SoftwareVersion { get; private set; }

            public string Description { get; private set; }
    
            public virtual XmlData Structure { get; protected set; }

            public override async Task ApplyAsync(PackageVersionDelta delta)
            {
                if (delta.SoftwareVersion != null)
                {
                    this.SoftwareVersion = delta.SoftwareVersion.Value;
                    this.OnPropertyChanged("SoftwareVersion");
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.OnPropertyChanged("Description");
                }

                if (delta.Structure != null)
                {
                    this.Structure = delta.Structure.Value;
                    this.OnPropertyChanged("Structure");
                }

                if (delta.Package != null)
                {
                    var packageChangeTrackingManager = DependencyResolver.Current.Get<IPackageChangeTrackingManager>();
                    var id = delta.Package.ReferenceId;
                    this.Package = id.HasValue ? await packageChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Package");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((PackageVersionReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(PackageVersionReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.PackageVersion.Id;
                this.SoftwareVersion = this.PackageVersion.SoftwareVersion;
                this.Description = this.PackageVersion.Description;
                this.Structure = this.PackageVersion.Structure;
                this.CreatedOn = this.PackageVersion.CreatedOn;
                this.LastModifiedOn = this.PackageVersion.LastModifiedOn;
                this.Version = new Version(this.PackageVersion.Version);
            }

            public PackageVersionWritableModel ToChangeTrackingModel()
            {
                var model = new PackageVersionWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class PackageVersionWritableModel : WritableModelBase<PackageVersionDelta>
        {
            private PackageReadableModel package;
            private string softwareVersion;
            private string description;
            private XmlData structure;

            internal protected PackageVersionWritableModel(PackageVersionReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.softwareVersion = readableModel.SoftwareVersion;
                this.description = readableModel.Description;
                this.package = readableModel.Package;
                this.structure = readableModel.Structure;
                this.Delta = new PackageVersionDelta(readableModel);
            }

            public PackageVersionWritableModel()
                : base(new Version(0))
            {
                this.Delta = new PackageVersionDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public string SoftwareVersion
            {
                get
                {
                    return this.softwareVersion;
                }

                set
                {
                    this.softwareVersion = value;
                    this.Delta.ChangeSoftwareVersion(value);
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }

                set
                {
                    this.description = value;
                    this.Delta.ChangeDescription(value);
                }
            }

            public XmlData Structure
            {
                get
                {
                    return this.structure;
                }

                set
                {
                    this.structure = value;
                    this.Delta.ChangeStructure(value);
                }
            }

            public PackageReadableModel Package
            {
                get
                {
                    return this.package;
                }

                set
                {
                    this.package = value;
                    this.Delta.ChangePackage(value);
                }
            }

            public PackageVersionReadableModel ReadableModel { get; private set; }

            public static PackageVersionWritableModel CreateCopyFrom(PackageVersionReadableModel readableModel)
            {
                var model = new PackageVersionWritableModel();
                model.SoftwareVersion = readableModel.SoftwareVersion;
                model.Description = readableModel.Description;
                model.Package = readableModel.Package;
                model.Structure = readableModel.Structure;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Configurations
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        public partial interface IMediaConfigurationChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Removed;

            Task AddAsync(MediaConfigurationWritableModel writableModel);

            Task DeleteAsync(MediaConfigurationReadableModel model);

            Task<MediaConfigurationReadableModel> GetAsync(int id);

            Task<IEnumerable<MediaConfigurationReadableModel>> QueryAsync(MediaConfigurationQuery query = null);
            
            Task<MediaConfigurationReadableModel> CommitAndVerifyAsync(MediaConfigurationWritableModel writableModel);

            MediaConfigurationReadableModel Wrap(MediaConfiguration entity);

            MediaConfigurationWritableModel Create();

            MediaConfigurationWritableModel CreateCopy(MediaConfigurationReadableModel model);
        }

        public partial class MediaConfigurationReadableModel : ReadableModelBase<MediaConfigurationDelta>
        {
            protected readonly ObservableReadOnlyCollection<Update.UpdateGroupReadableModel> updateGroups =
                new ObservableReadOnlyCollection<Update.UpdateGroupReadableModel>();

            protected readonly MediaConfiguration MediaConfiguration;

            protected readonly object locker = new object();

            private volatile bool populated;

            public MediaConfigurationReadableModel(MediaConfiguration entity)
            {
                this.MediaConfiguration = entity;
            }

            public int Id { get; private set; }

            public virtual Documents.DocumentReadableModel Document { get; protected set; }

            public virtual IObservableReadOnlyCollection<Update.UpdateGroupReadableModel> UpdateGroups
            {
                get
                {
                    return this.updateGroups;
                }
            }

            public override async Task ApplyAsync(MediaConfigurationDelta delta)
            {
                if (delta.Document != null)
                {
                    var documentChangeTrackingManager = DependencyResolver.Current.Get<Documents.IDocumentChangeTrackingManager>();
                    var id = delta.Document.ReferenceId;
                    this.Document = id.HasValue ? await documentChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Document");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((MediaConfigurationReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(MediaConfigurationReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.MediaConfiguration.Id;
                this.CreatedOn = this.MediaConfiguration.CreatedOn;
                this.LastModifiedOn = this.MediaConfiguration.LastModifiedOn;
                this.Version = new Version(this.MediaConfiguration.Version);
            }

            public MediaConfigurationWritableModel ToChangeTrackingModel()
            {
                var model = new MediaConfigurationWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class MediaConfigurationWritableModel : WritableModelBase<MediaConfigurationDelta>
        {
            private Documents.DocumentReadableModel document;

            internal protected MediaConfigurationWritableModel(MediaConfigurationReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.document = readableModel.Document;
                this.Delta = new MediaConfigurationDelta(readableModel);
            }

            public MediaConfigurationWritableModel()
                : base(new Version(0))
            {
                this.Delta = new MediaConfigurationDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public Documents.DocumentReadableModel Document
            {
                get
                {
                    return this.document;
                }

                set
                {
                    this.document = value;
                    this.Delta.ChangeDocument(value);
                }
            }

            public MediaConfigurationReadableModel ReadableModel { get; private set; }

            public static MediaConfigurationWritableModel CreateCopyFrom(MediaConfigurationReadableModel readableModel)
            {
                var model = new MediaConfigurationWritableModel();
                model.Document = readableModel.Document;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUnitConfigurationChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Removed;

            Task AddAsync(UnitConfigurationWritableModel writableModel);

            Task DeleteAsync(UnitConfigurationReadableModel model);

            Task<UnitConfigurationReadableModel> GetAsync(int id);

            Task<IEnumerable<UnitConfigurationReadableModel>> QueryAsync(UnitConfigurationQuery query = null);
            
            Task<UnitConfigurationReadableModel> CommitAndVerifyAsync(UnitConfigurationWritableModel writableModel);

            UnitConfigurationReadableModel Wrap(UnitConfiguration entity);

            UnitConfigurationWritableModel Create();

            UnitConfigurationWritableModel CreateCopy(UnitConfigurationReadableModel model);
        }

        public partial class UnitConfigurationReadableModel : ReadableModelBase<UnitConfigurationDelta>
        {
            protected readonly ObservableReadOnlyCollection<Update.UpdateGroupReadableModel> updateGroups =
                new ObservableReadOnlyCollection<Update.UpdateGroupReadableModel>();

            protected readonly UnitConfiguration UnitConfiguration;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UnitConfigurationReadableModel(UnitConfiguration entity)
            {
                this.UnitConfiguration = entity;
            }

            public int Id { get; private set; }

            public virtual Documents.DocumentReadableModel Document { get; protected set; }

            public virtual Units.ProductTypeReadableModel ProductType { get; protected set; }

            public virtual IObservableReadOnlyCollection<Update.UpdateGroupReadableModel> UpdateGroups
            {
                get
                {
                    return this.updateGroups;
                }
            }

            public override async Task ApplyAsync(UnitConfigurationDelta delta)
            {
                if (delta.Document != null)
                {
                    var documentChangeTrackingManager = DependencyResolver.Current.Get<Documents.IDocumentChangeTrackingManager>();
                    var id = delta.Document.ReferenceId;
                    this.Document = id.HasValue ? await documentChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Document");
                }

                if (delta.ProductType != null)
                {
                    var productTypeChangeTrackingManager = DependencyResolver.Current.Get<Units.IProductTypeChangeTrackingManager>();
                    var id = delta.ProductType.ReferenceId;
                    this.ProductType = id.HasValue ? await productTypeChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("ProductType");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UnitConfigurationReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UnitConfigurationReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UnitConfiguration.Id;
                this.CreatedOn = this.UnitConfiguration.CreatedOn;
                this.LastModifiedOn = this.UnitConfiguration.LastModifiedOn;
                this.Version = new Version(this.UnitConfiguration.Version);
            }

            public UnitConfigurationWritableModel ToChangeTrackingModel()
            {
                var model = new UnitConfigurationWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UnitConfigurationWritableModel : WritableModelBase<UnitConfigurationDelta>
        {
            private Documents.DocumentReadableModel document;

            private Units.ProductTypeReadableModel productType;

            internal protected UnitConfigurationWritableModel(UnitConfigurationReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.document = readableModel.Document;
                this.productType = readableModel.ProductType;
                this.Delta = new UnitConfigurationDelta(readableModel);
            }

            public UnitConfigurationWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UnitConfigurationDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public Documents.DocumentReadableModel Document
            {
                get
                {
                    return this.document;
                }

                set
                {
                    this.document = value;
                    this.Delta.ChangeDocument(value);
                }
            }

            public Units.ProductTypeReadableModel ProductType
            {
                get
                {
                    return this.productType;
                }

                set
                {
                    this.productType = value;
                    this.Delta.ChangeProductType(value);
                }
            }

            public UnitConfigurationReadableModel ReadableModel { get; private set; }

            public static UnitConfigurationWritableModel CreateCopyFrom(UnitConfigurationReadableModel readableModel)
            {
                var model = new UnitConfigurationWritableModel();
                model.Document = readableModel.Document;
                model.ProductType = readableModel.ProductType;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }

    namespace Log
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;
    }

    namespace Meta
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        public partial interface ISystemConfigChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Removed;

            Task AddAsync(SystemConfigWritableModel writableModel);

            Task DeleteAsync(SystemConfigReadableModel model);

            Task<SystemConfigReadableModel> GetAsync(int id);

            Task<IEnumerable<SystemConfigReadableModel>> QueryAsync(SystemConfigQuery query = null);
            
            Task<SystemConfigReadableModel> CommitAndVerifyAsync(SystemConfigWritableModel writableModel);

            SystemConfigReadableModel Wrap(SystemConfig entity);

            SystemConfigWritableModel Create();

            SystemConfigWritableModel CreateCopy(SystemConfigReadableModel model);
        }

        public partial class SystemConfigReadableModel : ReadableModelBase<SystemConfigDelta>
        {

            protected readonly SystemConfig SystemConfig;

            protected readonly object locker = new object();

            private volatile bool populated;

            public SystemConfigReadableModel(SystemConfig entity)
            {
                this.SystemConfig = entity;
            }

            public int Id { get; private set; }

            public Guid SystemId { get; private set; }
    
            public virtual XmlData Settings { get; protected set; }

            public override async Task ApplyAsync(SystemConfigDelta delta)
            {
                if (delta.SystemId != null)
                {
                    this.SystemId = delta.SystemId.Value;
                    this.OnPropertyChanged("SystemId");
                }

                if (delta.Settings != null)
                {
                    this.Settings = delta.Settings.Value;
                    this.OnPropertyChanged("Settings");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((SystemConfigReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(SystemConfigReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.SystemConfig.Id;
                this.SystemId = this.SystemConfig.SystemId;
                this.Settings = this.SystemConfig.Settings;
                this.CreatedOn = this.SystemConfig.CreatedOn;
                this.LastModifiedOn = this.SystemConfig.LastModifiedOn;
                this.Version = new Version(this.SystemConfig.Version);
            }

            public SystemConfigWritableModel ToChangeTrackingModel()
            {
                var model = new SystemConfigWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class SystemConfigWritableModel : WritableModelBase<SystemConfigDelta>
        {            private Guid systemId;
            private XmlData settings;

            internal protected SystemConfigWritableModel(SystemConfigReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.systemId = readableModel.SystemId;
                this.settings = readableModel.Settings;
                this.Delta = new SystemConfigDelta(readableModel);
            }

            public SystemConfigWritableModel()
                : base(new Version(0))
            {
                this.Delta = new SystemConfigDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public Guid SystemId
            {
                get
                {
                    return this.systemId;
                }

                set
                {
                    this.systemId = value;
                    this.Delta.ChangeSystemId(value);
                }
            }

            public XmlData Settings
            {
                get
                {
                    return this.settings;
                }

                set
                {
                    this.settings = value;
                    this.Delta.ChangeSettings(value);
                }
            }

            public SystemConfigReadableModel ReadableModel { get; private set; }

            public static SystemConfigWritableModel CreateCopyFrom(SystemConfigReadableModel readableModel)
            {
                var model = new SystemConfigWritableModel();
                model.SystemId = readableModel.SystemId;
                model.Settings = readableModel.Settings;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }

        public partial interface IUserDefinedPropertyChangeTrackingManager : IChangeTrackingManager
        {
            event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Added;

            event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Removed;

            Task AddAsync(UserDefinedPropertyWritableModel writableModel);

            Task DeleteAsync(UserDefinedPropertyReadableModel model);

            Task<UserDefinedPropertyReadableModel> GetAsync(int id);

            Task<IEnumerable<UserDefinedPropertyReadableModel>> QueryAsync(UserDefinedPropertyQuery query = null);
            
            Task<UserDefinedPropertyReadableModel> CommitAndVerifyAsync(UserDefinedPropertyWritableModel writableModel);

            UserDefinedPropertyReadableModel Wrap(UserDefinedProperty entity);

            UserDefinedPropertyWritableModel Create();

            UserDefinedPropertyWritableModel CreateCopy(UserDefinedPropertyReadableModel model);
        }

        public partial class UserDefinedPropertyReadableModel : ReadableModelBase<UserDefinedPropertyDelta>
        {

            protected readonly UserDefinedProperty UserDefinedProperty;

            protected readonly object locker = new object();

            private volatile bool populated;

            public UserDefinedPropertyReadableModel(UserDefinedProperty entity)
            {
                this.UserDefinedProperty = entity;
            }

            public int Id { get; private set; }

            public virtual Membership.TenantReadableModel Tenant { get; protected set; }

            public UserDefinedPropertyEnabledEntity OwnerEntity { get; private set; }

            public string Name { get; private set; }

            public override async Task ApplyAsync(UserDefinedPropertyDelta delta)
            {
                if (delta.OwnerEntity != null)
                {
                    this.OwnerEntity = delta.OwnerEntity.Value;
                    this.OnPropertyChanged("OwnerEntity");
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.OnPropertyChanged("Name");
                }

                if (delta.Tenant != null)
                {
                    var tenantChangeTrackingManager = DependencyResolver.Current.Get<Membership.ITenantChangeTrackingManager>();
                    var id = delta.Tenant.ReferenceId;
                    this.Tenant = id.HasValue ? await tenantChangeTrackingManager.GetAsync(id.Value) : null;
                    this.OnPropertyChanged("Tenant");
                }

                await base.ApplyAsync(delta);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((UserDefinedPropertyReadableModel)obj);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected bool Equals(UserDefinedPropertyReadableModel other)
            {
                return this.Id == other.Id;
            }

            protected void Populate()
            {
                if (this.populated)
                {
                    return;
                }

                lock (this.locker)
                {
                    if (this.populated)
                    {
                        return;
                    }

                    this.populated = true;
                }

                this.Id = this.UserDefinedProperty.Id;
                this.OwnerEntity = this.UserDefinedProperty.OwnerEntity;
                this.Name = this.UserDefinedProperty.Name;
                this.CreatedOn = this.UserDefinedProperty.CreatedOn;
                this.LastModifiedOn = this.UserDefinedProperty.LastModifiedOn;
                this.Version = new Version(this.UserDefinedProperty.Version);
            }

            public UserDefinedPropertyWritableModel ToChangeTrackingModel()
            {
                var model = new UserDefinedPropertyWritableModel(this);
                this.OnChangeTrackingModelCreated(model);
                return model;
            }
        }

        public partial class UserDefinedPropertyWritableModel : WritableModelBase<UserDefinedPropertyDelta>
        {
            private Membership.TenantReadableModel tenant;
            private UserDefinedPropertyEnabledEntity ownerEntity;
            private string name;

            internal protected UserDefinedPropertyWritableModel(UserDefinedPropertyReadableModel readableModel)
                : base(readableModel.Version)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ownerEntity = readableModel.OwnerEntity;
                this.name = readableModel.Name;
                this.tenant = readableModel.Tenant;
                this.Delta = new UserDefinedPropertyDelta(readableModel);
            }

            public UserDefinedPropertyWritableModel()
                : base(new Version(0))
            {
                this.Delta = new UserDefinedPropertyDelta();
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel == null ? default(int) : this.ReadableModel.Id;
                }
            }

            public UserDefinedPropertyEnabledEntity OwnerEntity
            {
                get
                {
                    return this.ownerEntity;
                }

                set
                {
                    this.ownerEntity = value;
                    this.Delta.ChangeOwnerEntity(value);
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.Delta.ChangeName(value);
                }
            }

            public Membership.TenantReadableModel Tenant
            {
                get
                {
                    return this.tenant;
                }

                set
                {
                    this.tenant = value;
                    this.Delta.ChangeTenant(value);
                }
            }

            public UserDefinedPropertyReadableModel ReadableModel { get; private set; }

            public static UserDefinedPropertyWritableModel CreateCopyFrom(UserDefinedPropertyReadableModel readableModel)
            {
                var model = new UserDefinedPropertyWritableModel();
                model.OwnerEntity = readableModel.OwnerEntity;
                model.Name = readableModel.Name;
                model.Tenant = readableModel.Tenant;
                return model;
            }
            
            public override bool HasChanges()
            {
                var delta = this.Delta.GetChangedDelta();
                return delta != null;
            }
        }
    }
}
