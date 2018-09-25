
namespace Gorba.Center.Common.Wpf.Client.Tests.Mocks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;	
    
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;

    public class UserRoleChangeTrackingManagerMock : IUserRoleChangeTrackingManager
    {
        private readonly List<UserRoleReadableModel> models = new List<UserRoleReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Removed;

        public async Task AddAsync(UserRoleWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UserRoleReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UserRoleReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UserRoleReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UserRoleReadableModel>> QueryAsync(UserRoleQuery query = null)
        {
            return Task.FromResult<IEnumerable<UserRoleReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UserRoleReadableModel> CommitAndVerifyAsync(UserRoleWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UserRoleReadableModel Wrap(UserRole entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UserRoleWritableModel Create()
        {
            var model = new UserRoleWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UserRoleWritableModel CreateCopy(UserRoleReadableModel readableModel)
        {
            var model = UserRoleWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UserRoleReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UserRoleReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UserRoleReadableModel> DoAddAsync(UserRoleWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UserRoleReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UserRoleReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UserRoleDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UserRoleDelta> e)
        {
            var writableModel = (UserRoleWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UserRoleReadableModelMock : UserRoleReadableModel
        {
            private bool loadedAuthorizations;

            public UserRoleReadableModelMock(UserRole entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedAuthorizations)
                {
                    this.loadedAuthorizations = true;

                    var manager = DependencyResolver.Current.Get<IAuthorizationChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.UserRole != null && e.Model.UserRole.Id == this.Id)
                            {
                                this.authorizations.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.authorizations.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.authorizations.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.UserRole != null && e.UserRole.Id == this.Id))
                    {
                        this.authorizations.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class AuthorizationChangeTrackingManagerMock : IAuthorizationChangeTrackingManager
    {
        private readonly List<AuthorizationReadableModel> models = new List<AuthorizationReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Removed;

        public async Task AddAsync(AuthorizationWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(AuthorizationReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<AuthorizationReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<AuthorizationReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<AuthorizationReadableModel>> QueryAsync(AuthorizationQuery query = null)
        {
            return Task.FromResult<IEnumerable<AuthorizationReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<AuthorizationReadableModel> CommitAndVerifyAsync(AuthorizationWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public AuthorizationReadableModel Wrap(Authorization entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public AuthorizationWritableModel Create()
        {
            var model = new AuthorizationWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public AuthorizationWritableModel CreateCopy(AuthorizationReadableModel readableModel)
        {
            var model = AuthorizationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<AuthorizationReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<AuthorizationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<AuthorizationReadableModel> DoAddAsync(AuthorizationWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new AuthorizationReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<AuthorizationReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<AuthorizationDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<AuthorizationDelta> e)
        {
            var writableModel = (AuthorizationWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class AuthorizationReadableModelMock : AuthorizationReadableModel
        {
            private bool loadedUserRole;

            public AuthorizationReadableModelMock(Authorization entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUserRole)
                {
                    this.loadedUserRole = true;
                    if (this.Authorization.UserRole != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                        this.UserRole = await manager.GetAsync(this.Authorization.UserRole.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class TenantChangeTrackingManagerMock : ITenantChangeTrackingManager
    {
        private readonly List<TenantReadableModel> models = new List<TenantReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Removed;

        public async Task AddAsync(TenantWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(TenantReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<TenantReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<TenantReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<TenantReadableModel>> QueryAsync(TenantQuery query = null)
        {
            return Task.FromResult<IEnumerable<TenantReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<TenantReadableModel> CommitAndVerifyAsync(TenantWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public TenantReadableModel Wrap(Tenant entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public TenantWritableModel Create()
        {
            var model = new TenantWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public TenantWritableModel CreateCopy(TenantReadableModel readableModel)
        {
            var model = TenantWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<TenantReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<TenantReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<TenantReadableModel> DoAddAsync(TenantWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new TenantReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<TenantReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<TenantDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<TenantDelta> e)
        {
            var writableModel = (TenantWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class TenantReadableModelMock : TenantReadableModel
        {
            private bool loadedUsers;
            private bool loadedUpdateGroups;

            public TenantReadableModelMock(Tenant entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUsers)
                {
                    this.loadedUsers = true;

                    var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.OwnerTenant != null && e.Model.OwnerTenant.Id == this.Id)
                            {
                                this.users.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.users.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.users.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.OwnerTenant != null && e.OwnerTenant.Id == this.Id))
                    {
                        this.users.Add(model);
                    }
                }

                if (!this.loadedUpdateGroups)
                {
                    this.loadedUpdateGroups = true;

                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.Tenant != null && e.Model.Tenant.Id == this.Id)
                            {
                                this.updateGroups.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.updateGroups.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.updateGroups.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.Tenant != null && e.Tenant.Id == this.Id))
                    {
                        this.updateGroups.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UserChangeTrackingManagerMock : IUserChangeTrackingManager
    {
        private readonly List<UserReadableModel> models = new List<UserReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UserReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserReadableModel>> Removed;

        public async Task AddAsync(UserWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UserReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UserReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UserReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UserReadableModel>> QueryAsync(UserQuery query = null)
        {
            return Task.FromResult<IEnumerable<UserReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UserReadableModel> CommitAndVerifyAsync(UserWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UserReadableModel Wrap(User entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UserWritableModel Create()
        {
            var model = new UserWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UserWritableModel CreateCopy(UserReadableModel readableModel)
        {
            var model = UserWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UserReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UserReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UserReadableModel> DoAddAsync(UserWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UserReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UserReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UserDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UserDelta> e)
        {
            var writableModel = (UserWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UserReadableModelMock : UserReadableModel
        {
            private bool loadedOwnerTenant;
            private bool loadedAssociationTenantUserUserRoles;

            public UserReadableModelMock(User entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedOwnerTenant)
                {
                    this.loadedOwnerTenant = true;
                    if (this.User.OwnerTenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.OwnerTenant = await manager.GetAsync(this.User.OwnerTenant.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedAssociationTenantUserUserRoles)
                {
                    this.loadedAssociationTenantUserUserRoles = true;

                    var manager = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.User != null && e.Model.User.Id == this.Id)
                            {
                                this.associationTenantUserUserRoles.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.associationTenantUserUserRoles.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.associationTenantUserUserRoles.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.User != null && e.User.Id == this.Id))
                    {
                        this.associationTenantUserUserRoles.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class AssociationTenantUserUserRoleChangeTrackingManagerMock : IAssociationTenantUserUserRoleChangeTrackingManager
    {
        private readonly List<AssociationTenantUserUserRoleReadableModel> models = new List<AssociationTenantUserUserRoleReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Removed;

        public async Task AddAsync(AssociationTenantUserUserRoleWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(AssociationTenantUserUserRoleReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<AssociationTenantUserUserRoleReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            return Task.FromResult<IEnumerable<AssociationTenantUserUserRoleReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<AssociationTenantUserUserRoleReadableModel> CommitAndVerifyAsync(AssociationTenantUserUserRoleWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public AssociationTenantUserUserRoleReadableModel Wrap(AssociationTenantUserUserRole entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public AssociationTenantUserUserRoleWritableModel Create()
        {
            var model = new AssociationTenantUserUserRoleWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public AssociationTenantUserUserRoleWritableModel CreateCopy(AssociationTenantUserUserRoleReadableModel readableModel)
        {
            var model = AssociationTenantUserUserRoleWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<AssociationTenantUserUserRoleReadableModel> DoAddAsync(AssociationTenantUserUserRoleWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new AssociationTenantUserUserRoleReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<AssociationTenantUserUserRoleDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<AssociationTenantUserUserRoleDelta> e)
        {
            var writableModel = (AssociationTenantUserUserRoleWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class AssociationTenantUserUserRoleReadableModelMock : AssociationTenantUserUserRoleReadableModel
        {
            private bool loadedTenant;
            private bool loadedUser;
            private bool loadedUserRole;

            public AssociationTenantUserUserRoleReadableModelMock(AssociationTenantUserUserRole entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedTenant)
                {
                    this.loadedTenant = true;
                    if (this.AssociationTenantUserUserRole.Tenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.Tenant = await manager.GetAsync(this.AssociationTenantUserUserRole.Tenant.Id);
                    }
                }

                if (!this.loadedUser)
                {
                    this.loadedUser = true;
                    if (this.AssociationTenantUserUserRole.User != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                        this.User = await manager.GetAsync(this.AssociationTenantUserUserRole.User.Id);
                    }
                }

                if (!this.loadedUserRole)
                {
                    this.loadedUserRole = true;
                    if (this.AssociationTenantUserUserRole.UserRole != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
                        this.UserRole = await manager.GetAsync(this.AssociationTenantUserUserRole.UserRole.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class ProductTypeChangeTrackingManagerMock : IProductTypeChangeTrackingManager
    {
        private readonly List<ProductTypeReadableModel> models = new List<ProductTypeReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Removed;

        public async Task AddAsync(ProductTypeWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(ProductTypeReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<ProductTypeReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<ProductTypeReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<ProductTypeReadableModel>> QueryAsync(ProductTypeQuery query = null)
        {
            return Task.FromResult<IEnumerable<ProductTypeReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<ProductTypeReadableModel> CommitAndVerifyAsync(ProductTypeWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public ProductTypeReadableModel Wrap(ProductType entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public ProductTypeWritableModel Create()
        {
            var model = new ProductTypeWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public ProductTypeWritableModel CreateCopy(ProductTypeReadableModel readableModel)
        {
            var model = ProductTypeWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<ProductTypeReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<ProductTypeReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<ProductTypeReadableModel> DoAddAsync(ProductTypeWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new ProductTypeReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<ProductTypeReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<ProductTypeDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<ProductTypeDelta> e)
        {
            var writableModel = (ProductTypeWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class ProductTypeReadableModelMock : ProductTypeReadableModel
        {
            private bool loadedUnits;

            public ProductTypeReadableModelMock(ProductType entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUnits)
                {
                    this.loadedUnits = true;

                    var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.ProductType != null && e.Model.ProductType.Id == this.Id)
                            {
                                this.units.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.units.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.units.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.ProductType != null && e.ProductType.Id == this.Id))
                    {
                        this.units.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UnitChangeTrackingManagerMock : IUnitChangeTrackingManager
    {
        private readonly List<UnitReadableModel> models = new List<UnitReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Removed;

        public async Task AddAsync(UnitWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UnitReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UnitReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UnitReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UnitReadableModel>> QueryAsync(UnitQuery query = null)
        {
            return Task.FromResult<IEnumerable<UnitReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UnitReadableModel> CommitAndVerifyAsync(UnitWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UnitReadableModel Wrap(Unit entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UnitWritableModel Create()
        {
            var model = new UnitWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UnitWritableModel CreateCopy(UnitReadableModel readableModel)
        {
            var model = UnitWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UnitReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UnitReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UnitReadableModel> DoAddAsync(UnitWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UnitReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UnitReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UnitDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UnitDelta> e)
        {
            var writableModel = (UnitWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UnitReadableModelMock : UnitReadableModel
        {
            private bool loadedTenant;
            private bool loadedProductType;
            private bool loadedUpdateGroup;
            private bool loadedUpdateCommands;

            public UnitReadableModelMock(Unit entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedTenant)
                {
                    this.loadedTenant = true;
                    if (this.Unit.Tenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.Tenant = await manager.GetAsync(this.Unit.Tenant.Id);
                    }
                }

                if (!this.loadedProductType)
                {
                    this.loadedProductType = true;
                    if (this.Unit.ProductType != null)
                    {
                        var manager = DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                        this.ProductType = await manager.GetAsync(this.Unit.ProductType.Id);
                    }
                }

                if (!this.loadedUpdateGroup)
                {
                    this.loadedUpdateGroup = true;
                    if (this.Unit.UpdateGroup != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                        this.UpdateGroup = await manager.GetAsync(this.Unit.UpdateGroup.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUpdateCommands)
                {
                    this.loadedUpdateCommands = true;

                    var manager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.Unit != null && e.Model.Unit.Id == this.Id)
                            {
                                this.updateCommands.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.updateCommands.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.updateCommands.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.Unit != null && e.Unit.Id == this.Id))
                    {
                        this.updateCommands.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class ResourceChangeTrackingManagerMock : IResourceChangeTrackingManager
    {
        private readonly List<ResourceReadableModel> models = new List<ResourceReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Removed;

        public async Task AddAsync(ResourceWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(ResourceReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<ResourceReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<ResourceReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<ResourceReadableModel>> QueryAsync(ResourceQuery query = null)
        {
            return Task.FromResult<IEnumerable<ResourceReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<ResourceReadableModel> CommitAndVerifyAsync(ResourceWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public ResourceReadableModel Wrap(Resource entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public ResourceWritableModel Create()
        {
            var model = new ResourceWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public ResourceWritableModel CreateCopy(ResourceReadableModel readableModel)
        {
            var model = ResourceWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<ResourceReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<ResourceReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<ResourceReadableModel> DoAddAsync(ResourceWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new ResourceReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<ResourceReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<ResourceDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<ResourceDelta> e)
        {
            var writableModel = (ResourceWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class ResourceReadableModelMock : ResourceReadableModel
        {
            private bool loadedUploadingUser;

            public ResourceReadableModelMock(Resource entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUploadingUser)
                {
                    this.loadedUploadingUser = true;
                    if (this.Resource.UploadingUser != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                        this.UploadingUser = await manager.GetAsync(this.Resource.UploadingUser.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class ContentResourceChangeTrackingManagerMock : IContentResourceChangeTrackingManager
    {
        private readonly List<ContentResourceReadableModel> models = new List<ContentResourceReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Removed;

        public async Task AddAsync(ContentResourceWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(ContentResourceReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<ContentResourceReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<ContentResourceReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<ContentResourceReadableModel>> QueryAsync(ContentResourceQuery query = null)
        {
            return Task.FromResult<IEnumerable<ContentResourceReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<ContentResourceReadableModel> CommitAndVerifyAsync(ContentResourceWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public ContentResourceReadableModel Wrap(ContentResource entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public ContentResourceWritableModel Create()
        {
            var model = new ContentResourceWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public ContentResourceWritableModel CreateCopy(ContentResourceReadableModel readableModel)
        {
            var model = ContentResourceWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<ContentResourceReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<ContentResourceReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<ContentResourceReadableModel> DoAddAsync(ContentResourceWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new ContentResourceReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<ContentResourceReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<ContentResourceDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<ContentResourceDelta> e)
        {
            var writableModel = (ContentResourceWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class ContentResourceReadableModelMock : ContentResourceReadableModel
        {
            private bool loadedUploadingUser;

            public ContentResourceReadableModelMock(ContentResource entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUploadingUser)
                {
                    this.loadedUploadingUser = true;
                    if (this.ContentResource.UploadingUser != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                        this.UploadingUser = await manager.GetAsync(this.ContentResource.UploadingUser.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UpdateGroupChangeTrackingManagerMock : IUpdateGroupChangeTrackingManager
    {
        private readonly List<UpdateGroupReadableModel> models = new List<UpdateGroupReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Removed;

        public async Task AddAsync(UpdateGroupWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UpdateGroupReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UpdateGroupReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UpdateGroupReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UpdateGroupReadableModel>> QueryAsync(UpdateGroupQuery query = null)
        {
            return Task.FromResult<IEnumerable<UpdateGroupReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UpdateGroupReadableModel> CommitAndVerifyAsync(UpdateGroupWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UpdateGroupReadableModel Wrap(UpdateGroup entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UpdateGroupWritableModel Create()
        {
            var model = new UpdateGroupWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UpdateGroupWritableModel CreateCopy(UpdateGroupReadableModel readableModel)
        {
            var model = UpdateGroupWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UpdateGroupReadableModel> DoAddAsync(UpdateGroupWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UpdateGroupReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UpdateGroupReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UpdateGroupDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UpdateGroupDelta> e)
        {
            var writableModel = (UpdateGroupWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UpdateGroupReadableModelMock : UpdateGroupReadableModel
        {
            private bool loadedTenant;
            private bool loadedUnitConfiguration;
            private bool loadedMediaConfiguration;
            private bool loadedUnits;
            private bool loadedUpdateParts;

            public UpdateGroupReadableModelMock(UpdateGroup entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedTenant)
                {
                    this.loadedTenant = true;
                    if (this.UpdateGroup.Tenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.Tenant = await manager.GetAsync(this.UpdateGroup.Tenant.Id);
                    }
                }

                if (!this.loadedUnitConfiguration)
                {
                    this.loadedUnitConfiguration = true;
                    if (this.UpdateGroup.UnitConfiguration != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUnitConfigurationChangeTrackingManager>();
                        this.UnitConfiguration = await manager.GetAsync(this.UpdateGroup.UnitConfiguration.Id);
                    }
                }

                if (!this.loadedMediaConfiguration)
                {
                    this.loadedMediaConfiguration = true;
                    if (this.UpdateGroup.MediaConfiguration != null)
                    {
                        var manager = DependencyResolver.Current.Get<IMediaConfigurationChangeTrackingManager>();
                        this.MediaConfiguration = await manager.GetAsync(this.UpdateGroup.MediaConfiguration.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUnits)
                {
                    this.loadedUnits = true;

                    var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.UpdateGroup != null && e.Model.UpdateGroup.Id == this.Id)
                            {
                                this.units.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.units.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.units.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.UpdateGroup != null && e.UpdateGroup.Id == this.Id))
                    {
                        this.units.Add(model);
                    }
                }

                if (!this.loadedUpdateParts)
                {
                    this.loadedUpdateParts = true;

                    var manager = DependencyResolver.Current.Get<IUpdatePartChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.UpdateGroup != null && e.Model.UpdateGroup.Id == this.Id)
                            {
                                this.updateParts.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.updateParts.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.updateParts.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.UpdateGroup != null && e.UpdateGroup.Id == this.Id))
                    {
                        this.updateParts.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UpdatePartChangeTrackingManagerMock : IUpdatePartChangeTrackingManager
    {
        private readonly List<UpdatePartReadableModel> models = new List<UpdatePartReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Removed;

        public async Task AddAsync(UpdatePartWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UpdatePartReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UpdatePartReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UpdatePartReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UpdatePartReadableModel>> QueryAsync(UpdatePartQuery query = null)
        {
            return Task.FromResult<IEnumerable<UpdatePartReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UpdatePartReadableModel> CommitAndVerifyAsync(UpdatePartWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UpdatePartReadableModel Wrap(UpdatePart entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UpdatePartWritableModel Create()
        {
            var model = new UpdatePartWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UpdatePartWritableModel CreateCopy(UpdatePartReadableModel readableModel)
        {
            var model = UpdatePartWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UpdatePartReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UpdatePartReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UpdatePartReadableModel> DoAddAsync(UpdatePartWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UpdatePartReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UpdatePartReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UpdatePartDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UpdatePartDelta> e)
        {
            var writableModel = (UpdatePartWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UpdatePartReadableModelMock : UpdatePartReadableModel
        {
            private bool loadedUpdateGroup;
            private bool loadedRelatedCommands;

            public UpdatePartReadableModelMock(UpdatePart entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUpdateGroup)
                {
                    this.loadedUpdateGroup = true;
                    if (this.UpdatePart.UpdateGroup != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                        this.UpdateGroup = await manager.GetAsync(this.UpdatePart.UpdateGroup.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UpdateCommandChangeTrackingManagerMock : IUpdateCommandChangeTrackingManager
    {
        private readonly List<UpdateCommandReadableModel> models = new List<UpdateCommandReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Removed;

        public async Task AddAsync(UpdateCommandWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UpdateCommandReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UpdateCommandReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UpdateCommandReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UpdateCommandReadableModel>> QueryAsync(UpdateCommandQuery query = null)
        {
            return Task.FromResult<IEnumerable<UpdateCommandReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UpdateCommandReadableModel> CommitAndVerifyAsync(UpdateCommandWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UpdateCommandReadableModel Wrap(UpdateCommand entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UpdateCommandWritableModel Create()
        {
            var model = new UpdateCommandWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UpdateCommandWritableModel CreateCopy(UpdateCommandReadableModel readableModel)
        {
            var model = UpdateCommandWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UpdateCommandReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UpdateCommandReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UpdateCommandReadableModel> DoAddAsync(UpdateCommandWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UpdateCommandReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UpdateCommandReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UpdateCommandDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UpdateCommandDelta> e)
        {
            var writableModel = (UpdateCommandWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UpdateCommandReadableModelMock : UpdateCommandReadableModel
        {
            private bool loadedUnit;
            private bool loadedIncludedParts;
            private bool loadedFeedbacks;

            public UpdateCommandReadableModelMock(UpdateCommand entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUnit)
                {
                    this.loadedUnit = true;
                    if (this.UpdateCommand.Unit != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                        this.Unit = await manager.GetAsync(this.UpdateCommand.Unit.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedFeedbacks)
                {
                    this.loadedFeedbacks = true;

                    var manager = DependencyResolver.Current.Get<IUpdateFeedbackChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.UpdateCommand != null && e.Model.UpdateCommand.Id == this.Id)
                            {
                                this.feedbacks.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.feedbacks.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.feedbacks.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.UpdateCommand != null && e.UpdateCommand.Id == this.Id))
                    {
                        this.feedbacks.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UpdateFeedbackChangeTrackingManagerMock : IUpdateFeedbackChangeTrackingManager
    {
        private readonly List<UpdateFeedbackReadableModel> models = new List<UpdateFeedbackReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Removed;

        public async Task AddAsync(UpdateFeedbackWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UpdateFeedbackReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UpdateFeedbackReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UpdateFeedbackReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UpdateFeedbackReadableModel>> QueryAsync(UpdateFeedbackQuery query = null)
        {
            return Task.FromResult<IEnumerable<UpdateFeedbackReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UpdateFeedbackReadableModel> CommitAndVerifyAsync(UpdateFeedbackWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UpdateFeedbackReadableModel Wrap(UpdateFeedback entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UpdateFeedbackWritableModel Create()
        {
            var model = new UpdateFeedbackWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UpdateFeedbackWritableModel CreateCopy(UpdateFeedbackReadableModel readableModel)
        {
            var model = UpdateFeedbackWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UpdateFeedbackReadableModel> DoAddAsync(UpdateFeedbackWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UpdateFeedbackReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UpdateFeedbackReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UpdateFeedbackDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UpdateFeedbackDelta> e)
        {
            var writableModel = (UpdateFeedbackWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UpdateFeedbackReadableModelMock : UpdateFeedbackReadableModel
        {
            private bool loadedUpdateCommand;

            public UpdateFeedbackReadableModelMock(UpdateFeedback entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedUpdateCommand)
                {
                    this.loadedUpdateCommand = true;
                    if (this.UpdateFeedback.UpdateCommand != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                        this.UpdateCommand = await manager.GetAsync(this.UpdateFeedback.UpdateCommand.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class DocumentChangeTrackingManagerMock : IDocumentChangeTrackingManager
    {
        private readonly List<DocumentReadableModel> models = new List<DocumentReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Removed;

        public async Task AddAsync(DocumentWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(DocumentReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<DocumentReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<DocumentReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<DocumentReadableModel>> QueryAsync(DocumentQuery query = null)
        {
            return Task.FromResult<IEnumerable<DocumentReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<DocumentReadableModel> CommitAndVerifyAsync(DocumentWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public DocumentReadableModel Wrap(Document entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public DocumentWritableModel Create()
        {
            var model = new DocumentWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public DocumentWritableModel CreateCopy(DocumentReadableModel readableModel)
        {
            var model = DocumentWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<DocumentReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<DocumentReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<DocumentReadableModel> DoAddAsync(DocumentWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new DocumentReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<DocumentReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<DocumentDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<DocumentDelta> e)
        {
            var writableModel = (DocumentWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class DocumentReadableModelMock : DocumentReadableModel
        {
            private bool loadedTenant;
            private bool loadedVersions;

            public DocumentReadableModelMock(Document entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedTenant)
                {
                    this.loadedTenant = true;
                    if (this.Document.Tenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.Tenant = await manager.GetAsync(this.Document.Tenant.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedVersions)
                {
                    this.loadedVersions = true;

                    var manager = DependencyResolver.Current.Get<IDocumentVersionChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.Document != null && e.Model.Document.Id == this.Id)
                            {
                                this.versions.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.versions.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.versions.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.Document != null && e.Document.Id == this.Id))
                    {
                        this.versions.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class DocumentVersionChangeTrackingManagerMock : IDocumentVersionChangeTrackingManager
    {
        private readonly List<DocumentVersionReadableModel> models = new List<DocumentVersionReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Removed;

        public async Task AddAsync(DocumentVersionWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(DocumentVersionReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<DocumentVersionReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<DocumentVersionReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<DocumentVersionReadableModel>> QueryAsync(DocumentVersionQuery query = null)
        {
            return Task.FromResult<IEnumerable<DocumentVersionReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<DocumentVersionReadableModel> CommitAndVerifyAsync(DocumentVersionWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public DocumentVersionReadableModel Wrap(DocumentVersion entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public DocumentVersionWritableModel Create()
        {
            var model = new DocumentVersionWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public DocumentVersionWritableModel CreateCopy(DocumentVersionReadableModel readableModel)
        {
            var model = DocumentVersionWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<DocumentVersionReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<DocumentVersionReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<DocumentVersionReadableModel> DoAddAsync(DocumentVersionWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new DocumentVersionReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<DocumentVersionReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<DocumentVersionDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<DocumentVersionDelta> e)
        {
            var writableModel = (DocumentVersionWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class DocumentVersionReadableModelMock : DocumentVersionReadableModel
        {
            private bool loadedDocument;
            private bool loadedCreatingUser;

            public DocumentVersionReadableModelMock(DocumentVersion entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedDocument)
                {
                    this.loadedDocument = true;
                    if (this.DocumentVersion.Document != null)
                    {
                        var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                        this.Document = await manager.GetAsync(this.DocumentVersion.Document.Id);
                    }
                }

                if (!this.loadedCreatingUser)
                {
                    this.loadedCreatingUser = true;
                    if (this.DocumentVersion.CreatingUser != null)
                    {
                        var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                        this.CreatingUser = await manager.GetAsync(this.DocumentVersion.CreatingUser.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class PackageChangeTrackingManagerMock : IPackageChangeTrackingManager
    {
        private readonly List<PackageReadableModel> models = new List<PackageReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Removed;

        public async Task AddAsync(PackageWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(PackageReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<PackageReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<PackageReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<PackageReadableModel>> QueryAsync(PackageQuery query = null)
        {
            return Task.FromResult<IEnumerable<PackageReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<PackageReadableModel> CommitAndVerifyAsync(PackageWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public PackageReadableModel Wrap(Package entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public PackageWritableModel Create()
        {
            var model = new PackageWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public PackageWritableModel CreateCopy(PackageReadableModel readableModel)
        {
            var model = PackageWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<PackageReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<PackageReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<PackageReadableModel> DoAddAsync(PackageWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new PackageReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<PackageReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<PackageDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<PackageDelta> e)
        {
            var writableModel = (PackageWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class PackageReadableModelMock : PackageReadableModel
        {
            private bool loadedVersions;

            public PackageReadableModelMock(Package entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedVersions)
                {
                    this.loadedVersions = true;

                    var manager = DependencyResolver.Current.Get<IPackageVersionChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.Package != null && e.Model.Package.Id == this.Id)
                            {
                                this.versions.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.versions.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.versions.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.Package != null && e.Package.Id == this.Id))
                    {
                        this.versions.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class PackageVersionChangeTrackingManagerMock : IPackageVersionChangeTrackingManager
    {
        private readonly List<PackageVersionReadableModel> models = new List<PackageVersionReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Removed;

        public async Task AddAsync(PackageVersionWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(PackageVersionReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<PackageVersionReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<PackageVersionReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<PackageVersionReadableModel>> QueryAsync(PackageVersionQuery query = null)
        {
            return Task.FromResult<IEnumerable<PackageVersionReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<PackageVersionReadableModel> CommitAndVerifyAsync(PackageVersionWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public PackageVersionReadableModel Wrap(PackageVersion entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public PackageVersionWritableModel Create()
        {
            var model = new PackageVersionWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public PackageVersionWritableModel CreateCopy(PackageVersionReadableModel readableModel)
        {
            var model = PackageVersionWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<PackageVersionReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<PackageVersionReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<PackageVersionReadableModel> DoAddAsync(PackageVersionWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new PackageVersionReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<PackageVersionReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<PackageVersionDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<PackageVersionDelta> e)
        {
            var writableModel = (PackageVersionWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class PackageVersionReadableModelMock : PackageVersionReadableModel
        {
            private bool loadedPackage;

            public PackageVersionReadableModelMock(PackageVersion entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedPackage)
                {
                    this.loadedPackage = true;
                    if (this.PackageVersion.Package != null)
                    {
                        var manager = DependencyResolver.Current.Get<IPackageChangeTrackingManager>();
                        this.Package = await manager.GetAsync(this.PackageVersion.Package.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UnitConfigurationChangeTrackingManagerMock : IUnitConfigurationChangeTrackingManager
    {
        private readonly List<UnitConfigurationReadableModel> models = new List<UnitConfigurationReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Removed;

        public async Task AddAsync(UnitConfigurationWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UnitConfigurationReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UnitConfigurationReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UnitConfigurationReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UnitConfigurationReadableModel>> QueryAsync(UnitConfigurationQuery query = null)
        {
            return Task.FromResult<IEnumerable<UnitConfigurationReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UnitConfigurationReadableModel> CommitAndVerifyAsync(UnitConfigurationWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UnitConfigurationReadableModel Wrap(UnitConfiguration entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UnitConfigurationWritableModel Create()
        {
            var model = new UnitConfigurationWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UnitConfigurationWritableModel CreateCopy(UnitConfigurationReadableModel readableModel)
        {
            var model = UnitConfigurationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UnitConfigurationReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UnitConfigurationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UnitConfigurationReadableModel> DoAddAsync(UnitConfigurationWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UnitConfigurationReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UnitConfigurationReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UnitConfigurationDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UnitConfigurationDelta> e)
        {
            var writableModel = (UnitConfigurationWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UnitConfigurationReadableModelMock : UnitConfigurationReadableModel
        {
            private bool loadedDocument;
            private bool loadedProductType;
            private bool loadedUpdateGroups;

            public UnitConfigurationReadableModelMock(UnitConfiguration entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedDocument)
                {
                    this.loadedDocument = true;
                    if (this.UnitConfiguration.Document != null)
                    {
                        var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                        this.Document = await manager.GetAsync(this.UnitConfiguration.Document.Id);
                    }
                }

                if (!this.loadedProductType)
                {
                    this.loadedProductType = true;
                    if (this.UnitConfiguration.ProductType != null)
                    {
                        var manager = DependencyResolver.Current.Get<IProductTypeChangeTrackingManager>();
                        this.ProductType = await manager.GetAsync(this.UnitConfiguration.ProductType.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUpdateGroups)
                {
                    this.loadedUpdateGroups = true;

                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.UnitConfiguration != null && e.Model.UnitConfiguration.Id == this.Id)
                            {
                                this.updateGroups.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.updateGroups.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.updateGroups.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.UnitConfiguration != null && e.UnitConfiguration.Id == this.Id))
                    {
                        this.updateGroups.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class MediaConfigurationChangeTrackingManagerMock : IMediaConfigurationChangeTrackingManager
    {
        private readonly List<MediaConfigurationReadableModel> models = new List<MediaConfigurationReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Removed;

        public async Task AddAsync(MediaConfigurationWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(MediaConfigurationReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<MediaConfigurationReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<MediaConfigurationReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<MediaConfigurationReadableModel>> QueryAsync(MediaConfigurationQuery query = null)
        {
            return Task.FromResult<IEnumerable<MediaConfigurationReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<MediaConfigurationReadableModel> CommitAndVerifyAsync(MediaConfigurationWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public MediaConfigurationReadableModel Wrap(MediaConfiguration entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public MediaConfigurationWritableModel Create()
        {
            var model = new MediaConfigurationWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public MediaConfigurationWritableModel CreateCopy(MediaConfigurationReadableModel readableModel)
        {
            var model = MediaConfigurationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<MediaConfigurationReadableModel> DoAddAsync(MediaConfigurationWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new MediaConfigurationReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<MediaConfigurationReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<MediaConfigurationDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<MediaConfigurationDelta> e)
        {
            var writableModel = (MediaConfigurationWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class MediaConfigurationReadableModelMock : MediaConfigurationReadableModel
        {
            private bool loadedDocument;
            private bool loadedUpdateGroups;

            public MediaConfigurationReadableModelMock(MediaConfiguration entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedDocument)
                {
                    this.loadedDocument = true;
                    if (this.MediaConfiguration.Document != null)
                    {
                        var manager = DependencyResolver.Current.Get<IDocumentChangeTrackingManager>();
                        this.Document = await manager.GetAsync(this.MediaConfiguration.Document.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();

                if (!this.loadedUpdateGroups)
                {
                    this.loadedUpdateGroups = true;

                    var manager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                    manager.Added += (s, e) =>
                        {
                            if (e.Model.MediaConfiguration != null && e.Model.MediaConfiguration.Id == this.Id)
                            {
                                this.updateGroups.Add(e.Model);
                            }
                        };
                    manager.Removed += (s, e) =>
                        {
                            var found = this.updateGroups.FirstOrDefault(m => m.Id == e.Model.Id);
                            if (found != null)
                            {
                                this.updateGroups.Remove(found);
                            }
                        };
                    foreach (
                        var model in
                            (await manager.QueryAsync()).Where(
                            e => e.MediaConfiguration != null && e.MediaConfiguration.Id == this.Id))
                    {
                        this.updateGroups.Add(model);
                    }
                }
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class UserDefinedPropertyChangeTrackingManagerMock : IUserDefinedPropertyChangeTrackingManager
    {
        private readonly List<UserDefinedPropertyReadableModel> models = new List<UserDefinedPropertyReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Removed;

        public async Task AddAsync(UserDefinedPropertyWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(UserDefinedPropertyReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<UserDefinedPropertyReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<UserDefinedPropertyReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<UserDefinedPropertyReadableModel>> QueryAsync(UserDefinedPropertyQuery query = null)
        {
            return Task.FromResult<IEnumerable<UserDefinedPropertyReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<UserDefinedPropertyReadableModel> CommitAndVerifyAsync(UserDefinedPropertyWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public UserDefinedPropertyReadableModel Wrap(UserDefinedProperty entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public UserDefinedPropertyWritableModel Create()
        {
            var model = new UserDefinedPropertyWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public UserDefinedPropertyWritableModel CreateCopy(UserDefinedPropertyReadableModel readableModel)
        {
            var model = UserDefinedPropertyWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<UserDefinedPropertyReadableModel> DoAddAsync(UserDefinedPropertyWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new UserDefinedPropertyReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<UserDefinedPropertyReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<UserDefinedPropertyDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<UserDefinedPropertyDelta> e)
        {
            var writableModel = (UserDefinedPropertyWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class UserDefinedPropertyReadableModelMock : UserDefinedPropertyReadableModel
        {
            private bool loadedTenant;

            public UserDefinedPropertyReadableModelMock(UserDefinedProperty entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {

                if (!this.loadedTenant)
                {
                    this.loadedTenant = true;
                    if (this.UserDefinedProperty.Tenant != null)
                    {
                        var manager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
                        this.Tenant = await manager.GetAsync(this.UserDefinedProperty.Tenant.Id);
                    }
                }
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }

    public class SystemConfigChangeTrackingManagerMock : ISystemConfigChangeTrackingManager
    {
        private readonly List<SystemConfigReadableModel> models = new List<SystemConfigReadableModel>();

        private int nextId = 1;

        public event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Removed;

        public async Task AddAsync(SystemConfigWritableModel writableModel)
        {
            await this.DoAddAsync(writableModel);
        }

        public Task DeleteAsync(SystemConfigReadableModel model)
        {
            var deleting = this.models.FirstOrDefault(e => e.Id == model.Id);
            if (deleting == null)
            {
                throw new ChangeTrackingException("Couldn't find " + model.Id);
            }

            this.models.Remove(deleting);
            this.RaiseRemoved(new ReadableModelEventArgs<SystemConfigReadableModel>(deleting));
            return Task.FromResult(0);
        }

        public Task<SystemConfigReadableModel> GetAsync(int id)
        {
            var model = this.models.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<SystemConfigReadableModel>> QueryAsync(SystemConfigQuery query = null)
        {
            return Task.FromResult<IEnumerable<SystemConfigReadableModel>>(this.models.ToList().AsQueryable().Apply(query));
        }

        public async Task<SystemConfigReadableModel> CommitAndVerifyAsync(SystemConfigWritableModel writableModel)
        {
            if (writableModel.Id == 0)
            {
                return await this.DoAddAsync(writableModel);
            }

            writableModel.Commit();
            return await this.GetAsync(writableModel.Id);
        }

        public SystemConfigReadableModel Wrap(SystemConfig entity)
        {
            return this.models.Single(e => e.Id == entity.Id);
        }

        public SystemConfigWritableModel Create()
        {
            var model = new SystemConfigWritableModel();
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public SystemConfigWritableModel CreateCopy(SystemConfigReadableModel readableModel)
        {
            var model = SystemConfigWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.EntityOnCommitted;
            return model;
        }

        public void ChangeCredentials(UserCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            this.models.Clear();
        }

        protected virtual void RaiseAdded(ReadableModelEventArgs<SystemConfigReadableModel> e)
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseRemoved(ReadableModelEventArgs<SystemConfigReadableModel> e)
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private async Task<SystemConfigReadableModel> DoAddAsync(SystemConfigWritableModel writableModel)
        {
            var dto = writableModel.ToDto();
            dto.Id = Interlocked.Increment(ref this.nextId);
            var readableModel = new SystemConfigReadableModelMock(dto);
            await readableModel.LoadReferencePropertiesAsync();
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
            this.models.Add(readableModel);
            this.RaiseAdded(new ReadableModelEventArgs<SystemConfigReadableModel>(readableModel));
            return readableModel;
        }

        private void ReadableModelOnChangeTrackingModelCreated(object sender, ChangeTrackingModelCreatedEventArgs<SystemConfigDelta> e)
        {
            e.TrackingModel.Committed += this.EntityOnCommitted;
        }

        private async void EntityOnCommitted(object sender, ModelUpdatedEventArgs<SystemConfigDelta> e)
        {
            var writableModel = (SystemConfigWritableModel)sender;
            if (e.Delta.Id == 0)
            {
                await this.DoAddAsync(writableModel);
                return;
            }

            var readableModel = this.models.Single(m => m.Id == writableModel.Id);
            await readableModel.ApplyAsync(e.Delta);
        }

        private class SystemConfigReadableModelMock : SystemConfigReadableModel
        {

            public SystemConfigReadableModelMock(SystemConfig entity)
                : base(entity)
            {
                this.Populate();
            }

            public async override Task LoadReferencePropertiesAsync()
            {
            }

            public async override Task LoadNavigationPropertiesAsync()
            {
                await this.LoadReferencePropertiesAsync();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }
}

