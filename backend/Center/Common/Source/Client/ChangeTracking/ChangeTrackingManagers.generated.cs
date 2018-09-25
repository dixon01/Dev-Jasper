namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;	

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

    public partial class AuthorizationChangeTrackingManager : ChangeTrackingManagerBase, IAuthorizationChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalAuthorizationReadableModel> models =
            new ConcurrentDictionary<int, InternalAuthorizationReadableModel>();

        public event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<AuthorizationReadableModel>> Removed;

        public AuthorizationChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public AuthorizationWritableModel Create()
        {
            var model = new AuthorizationWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public AuthorizationWritableModel CreateCopy(AuthorizationReadableModel readableModel)
        {
            var model = AuthorizationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(AuthorizationWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(AuthorizationReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<AuthorizationReadableModel>> QueryAsync(AuthorizationQuery query = null)
        {
            if (query == null)
            {
                query = AuthorizationQuery.Create();
            }

            var list = new List<AuthorizationReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IAuthorizationDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IAuthorizationDataService>();
        }

        private void Release(int id)
        {
            InternalAuthorizationReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalAuthorizationReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<AuthorizationDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<AuthorizationDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (AuthorizationWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new AuthorizationDeltaNotification
            {
                Delta = new AuthorizationDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<AuthorizationReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<AuthorizationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UserRoleChangeTrackingManager : ChangeTrackingManagerBase, IUserRoleChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUserRoleReadableModel> models =
            new ConcurrentDictionary<int, InternalUserRoleReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserRoleReadableModel>> Removed;

        public UserRoleChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UserRoleWritableModel Create()
        {
            var model = new UserRoleWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UserRoleWritableModel CreateCopy(UserRoleReadableModel readableModel)
        {
            var model = UserRoleWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UserRoleWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UserRoleReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UserRoleReadableModel>> QueryAsync(UserRoleQuery query = null)
        {
            if (query == null)
            {
                query = UserRoleQuery.Create();
            }

            var list = new List<UserRoleReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUserRoleDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUserRoleDataService>();
        }

        private void Release(int id)
        {
            InternalUserRoleReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUserRoleReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UserRoleDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UserRoleDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UserRoleWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UserRoleDeltaNotification
            {
                Delta = new UserRoleDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UserRoleReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UserRoleReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

    public partial class AssociationTenantUserUserRoleChangeTrackingManager : ChangeTrackingManagerBase, IAssociationTenantUserUserRoleChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalAssociationTenantUserUserRoleReadableModel> models =
            new ConcurrentDictionary<int, InternalAssociationTenantUserUserRoleReadableModel>();

        public event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>> Removed;

        public AssociationTenantUserUserRoleChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public AssociationTenantUserUserRoleWritableModel Create()
        {
            var model = new AssociationTenantUserUserRoleWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public AssociationTenantUserUserRoleWritableModel CreateCopy(AssociationTenantUserUserRoleReadableModel readableModel)
        {
            var model = AssociationTenantUserUserRoleWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(AssociationTenantUserUserRoleWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(AssociationTenantUserUserRoleReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            if (query == null)
            {
                query = AssociationTenantUserUserRoleQuery.Create();
            }

            var list = new List<AssociationTenantUserUserRoleReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IAssociationTenantUserUserRoleDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IAssociationTenantUserUserRoleDataService>();
        }

        private void Release(int id)
        {
            InternalAssociationTenantUserUserRoleReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalAssociationTenantUserUserRoleReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<AssociationTenantUserUserRoleDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<AssociationTenantUserUserRoleDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (AssociationTenantUserUserRoleWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new AssociationTenantUserUserRoleDeltaNotification
            {
                Delta = new AssociationTenantUserUserRoleDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class TenantChangeTrackingManager : ChangeTrackingManagerBase, ITenantChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalTenantReadableModel> models =
            new ConcurrentDictionary<int, InternalTenantReadableModel>();

        public event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<TenantReadableModel>> Removed;

        public TenantChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public TenantWritableModel Create()
        {
            var model = new TenantWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public TenantWritableModel CreateCopy(TenantReadableModel readableModel)
        {
            var model = TenantWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(TenantWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(TenantReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<TenantReadableModel>> QueryAsync(TenantQuery query = null)
        {
            if (query == null)
            {
                query = TenantQuery.Create();
            }

            var list = new List<TenantReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<ITenantDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<ITenantDataService>();
        }

        private void Release(int id)
        {
            InternalTenantReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalTenantReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<TenantDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<TenantDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (TenantWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new TenantDeltaNotification
            {
                Delta = new TenantDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<TenantReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<TenantReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UserChangeTrackingManager : ChangeTrackingManagerBase, IUserChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUserReadableModel> models =
            new ConcurrentDictionary<int, InternalUserReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UserReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserReadableModel>> Removed;

        public UserChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UserWritableModel Create()
        {
            var model = new UserWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UserWritableModel CreateCopy(UserReadableModel readableModel)
        {
            var model = UserWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UserWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UserReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UserReadableModel>> QueryAsync(UserQuery query = null)
        {
            if (query == null)
            {
                query = UserQuery.Create();
            }

            var list = new List<UserReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUserDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUserDataService>();
        }

        private void Release(int id)
        {
            InternalUserReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUserReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UserDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UserDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UserWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UserDeltaNotification
            {
                Delta = new UserDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UserReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UserReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

    public partial class ProductTypeChangeTrackingManager : ChangeTrackingManagerBase, IProductTypeChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalProductTypeReadableModel> models =
            new ConcurrentDictionary<int, InternalProductTypeReadableModel>();

        public event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ProductTypeReadableModel>> Removed;

        public ProductTypeChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public ProductTypeWritableModel Create()
        {
            var model = new ProductTypeWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public ProductTypeWritableModel CreateCopy(ProductTypeReadableModel readableModel)
        {
            var model = ProductTypeWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(ProductTypeWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(ProductTypeReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<ProductTypeReadableModel>> QueryAsync(ProductTypeQuery query = null)
        {
            if (query == null)
            {
                query = ProductTypeQuery.Create();
            }

            var list = new List<ProductTypeReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IProductTypeDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IProductTypeDataService>();
        }

        private void Release(int id)
        {
            InternalProductTypeReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalProductTypeReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<ProductTypeDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<ProductTypeDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (ProductTypeWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new ProductTypeDeltaNotification
            {
                Delta = new ProductTypeDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<ProductTypeReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<ProductTypeReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UnitChangeTrackingManager : ChangeTrackingManagerBase, IUnitChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUnitReadableModel> models =
            new ConcurrentDictionary<int, InternalUnitReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UnitReadableModel>> Removed;

        public UnitChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UnitWritableModel Create()
        {
            var model = new UnitWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UnitWritableModel CreateCopy(UnitReadableModel readableModel)
        {
            var model = UnitWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UnitWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UnitReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UnitReadableModel>> QueryAsync(UnitQuery query = null)
        {
            if (query == null)
            {
                query = UnitQuery.Create();
            }

            var list = new List<UnitReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUnitDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUnitDataService>();
        }

        private void Release(int id)
        {
            InternalUnitReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUnitReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UnitDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UnitDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UnitWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UnitDeltaNotification
            {
                Delta = new UnitDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UnitReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UnitReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

    public partial class ContentResourceChangeTrackingManager : ChangeTrackingManagerBase, IContentResourceChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalContentResourceReadableModel> models =
            new ConcurrentDictionary<int, InternalContentResourceReadableModel>();

        public event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ContentResourceReadableModel>> Removed;

        public ContentResourceChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public ContentResourceWritableModel Create()
        {
            var model = new ContentResourceWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public ContentResourceWritableModel CreateCopy(ContentResourceReadableModel readableModel)
        {
            var model = ContentResourceWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(ContentResourceWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(ContentResourceReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<ContentResourceReadableModel>> QueryAsync(ContentResourceQuery query = null)
        {
            if (query == null)
            {
                query = ContentResourceQuery.Create();
            }

            var list = new List<ContentResourceReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IContentResourceDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IContentResourceDataService>();
        }

        private void Release(int id)
        {
            InternalContentResourceReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalContentResourceReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<ContentResourceDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<ContentResourceDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (ContentResourceWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new ContentResourceDeltaNotification
            {
                Delta = new ContentResourceDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<ContentResourceReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<ContentResourceReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class ResourceChangeTrackingManager : ChangeTrackingManagerBase, IResourceChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalResourceReadableModel> models =
            new ConcurrentDictionary<int, InternalResourceReadableModel>();

        public event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<ResourceReadableModel>> Removed;

        public ResourceChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public ResourceWritableModel Create()
        {
            var model = new ResourceWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public ResourceWritableModel CreateCopy(ResourceReadableModel readableModel)
        {
            var model = ResourceWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(ResourceWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(ResourceReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<ResourceReadableModel>> QueryAsync(ResourceQuery query = null)
        {
            if (query == null)
            {
                query = ResourceQuery.Create();
            }

            var list = new List<ResourceReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IResourceDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IResourceDataService>();
        }

        private void Release(int id)
        {
            InternalResourceReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalResourceReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<ResourceDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<ResourceDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (ResourceWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new ResourceDeltaNotification
            {
                Delta = new ResourceDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<ResourceReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<ResourceReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

    public partial class UpdateCommandChangeTrackingManager : ChangeTrackingManagerBase, IUpdateCommandChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUpdateCommandReadableModel> models =
            new ConcurrentDictionary<int, InternalUpdateCommandReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateCommandReadableModel>> Removed;

        public UpdateCommandChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UpdateCommandWritableModel Create()
        {
            var model = new UpdateCommandWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UpdateCommandWritableModel CreateCopy(UpdateCommandReadableModel readableModel)
        {
            var model = UpdateCommandWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UpdateCommandWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UpdateCommandReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UpdateCommandReadableModel>> QueryAsync(UpdateCommandQuery query = null)
        {
            if (query == null)
            {
                query = UpdateCommandQuery.Create();
            }

            var list = new List<UpdateCommandReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUpdateCommandDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUpdateCommandDataService>();
        }

        private void Release(int id)
        {
            InternalUpdateCommandReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUpdateCommandReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UpdateCommandDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UpdateCommandDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UpdateCommandWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UpdateCommandDeltaNotification
            {
                Delta = new UpdateCommandDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UpdateCommandReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UpdateCommandReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UpdateFeedbackChangeTrackingManager : ChangeTrackingManagerBase, IUpdateFeedbackChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUpdateFeedbackReadableModel> models =
            new ConcurrentDictionary<int, InternalUpdateFeedbackReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateFeedbackReadableModel>> Removed;

        public UpdateFeedbackChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UpdateFeedbackWritableModel Create()
        {
            var model = new UpdateFeedbackWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UpdateFeedbackWritableModel CreateCopy(UpdateFeedbackReadableModel readableModel)
        {
            var model = UpdateFeedbackWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UpdateFeedbackWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UpdateFeedbackReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UpdateFeedbackReadableModel>> QueryAsync(UpdateFeedbackQuery query = null)
        {
            if (query == null)
            {
                query = UpdateFeedbackQuery.Create();
            }

            var list = new List<UpdateFeedbackReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUpdateFeedbackDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUpdateFeedbackDataService>();
        }

        private void Release(int id)
        {
            InternalUpdateFeedbackReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUpdateFeedbackReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UpdateFeedbackDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UpdateFeedbackDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UpdateFeedbackWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UpdateFeedbackDeltaNotification
            {
                Delta = new UpdateFeedbackDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UpdateGroupChangeTrackingManager : ChangeTrackingManagerBase, IUpdateGroupChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUpdateGroupReadableModel> models =
            new ConcurrentDictionary<int, InternalUpdateGroupReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdateGroupReadableModel>> Removed;

        public UpdateGroupChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UpdateGroupWritableModel Create()
        {
            var model = new UpdateGroupWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UpdateGroupWritableModel CreateCopy(UpdateGroupReadableModel readableModel)
        {
            var model = UpdateGroupWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UpdateGroupWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UpdateGroupReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UpdateGroupReadableModel>> QueryAsync(UpdateGroupQuery query = null)
        {
            if (query == null)
            {
                query = UpdateGroupQuery.Create();
            }

            var list = new List<UpdateGroupReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUpdateGroupDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUpdateGroupDataService>();
        }

        private void Release(int id)
        {
            InternalUpdateGroupReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUpdateGroupReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UpdateGroupDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UpdateGroupDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UpdateGroupWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UpdateGroupDeltaNotification
            {
                Delta = new UpdateGroupDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UpdatePartChangeTrackingManager : ChangeTrackingManagerBase, IUpdatePartChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUpdatePartReadableModel> models =
            new ConcurrentDictionary<int, InternalUpdatePartReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UpdatePartReadableModel>> Removed;

        public UpdatePartChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UpdatePartWritableModel Create()
        {
            var model = new UpdatePartWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UpdatePartWritableModel CreateCopy(UpdatePartReadableModel readableModel)
        {
            var model = UpdatePartWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UpdatePartWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UpdatePartReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UpdatePartReadableModel>> QueryAsync(UpdatePartQuery query = null)
        {
            if (query == null)
            {
                query = UpdatePartQuery.Create();
            }

            var list = new List<UpdatePartReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUpdatePartDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUpdatePartDataService>();
        }

        private void Release(int id)
        {
            InternalUpdatePartReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUpdatePartReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UpdatePartDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UpdatePartDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UpdatePartWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UpdatePartDeltaNotification
            {
                Delta = new UpdatePartDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UpdatePartReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UpdatePartReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

    public partial class DocumentChangeTrackingManager : ChangeTrackingManagerBase, IDocumentChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalDocumentReadableModel> models =
            new ConcurrentDictionary<int, InternalDocumentReadableModel>();

        public event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<DocumentReadableModel>> Removed;

        public DocumentChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public DocumentWritableModel Create()
        {
            var model = new DocumentWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public DocumentWritableModel CreateCopy(DocumentReadableModel readableModel)
        {
            var model = DocumentWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(DocumentWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(DocumentReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<DocumentReadableModel>> QueryAsync(DocumentQuery query = null)
        {
            if (query == null)
            {
                query = DocumentQuery.Create();
            }

            var list = new List<DocumentReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IDocumentDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IDocumentDataService>();
        }

        private void Release(int id)
        {
            InternalDocumentReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalDocumentReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<DocumentDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<DocumentDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (DocumentWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new DocumentDeltaNotification
            {
                Delta = new DocumentDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<DocumentReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<DocumentReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class DocumentVersionChangeTrackingManager : ChangeTrackingManagerBase, IDocumentVersionChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalDocumentVersionReadableModel> models =
            new ConcurrentDictionary<int, InternalDocumentVersionReadableModel>();

        public event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<DocumentVersionReadableModel>> Removed;

        public DocumentVersionChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public DocumentVersionWritableModel Create()
        {
            var model = new DocumentVersionWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public DocumentVersionWritableModel CreateCopy(DocumentVersionReadableModel readableModel)
        {
            var model = DocumentVersionWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(DocumentVersionWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(DocumentVersionReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<DocumentVersionReadableModel>> QueryAsync(DocumentVersionQuery query = null)
        {
            if (query == null)
            {
                query = DocumentVersionQuery.Create();
            }

            var list = new List<DocumentVersionReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IDocumentVersionDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IDocumentVersionDataService>();
        }

        private void Release(int id)
        {
            InternalDocumentVersionReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalDocumentVersionReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<DocumentVersionDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<DocumentVersionDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (DocumentVersionWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new DocumentVersionDeltaNotification
            {
                Delta = new DocumentVersionDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<DocumentVersionReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<DocumentVersionReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

    public partial class PackageChangeTrackingManager : ChangeTrackingManagerBase, IPackageChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalPackageReadableModel> models =
            new ConcurrentDictionary<int, InternalPackageReadableModel>();

        public event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<PackageReadableModel>> Removed;

        public PackageChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public PackageWritableModel Create()
        {
            var model = new PackageWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public PackageWritableModel CreateCopy(PackageReadableModel readableModel)
        {
            var model = PackageWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(PackageWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(PackageReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<PackageReadableModel>> QueryAsync(PackageQuery query = null)
        {
            if (query == null)
            {
                query = PackageQuery.Create();
            }

            var list = new List<PackageReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IPackageDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IPackageDataService>();
        }

        private void Release(int id)
        {
            InternalPackageReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalPackageReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<PackageDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<PackageDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (PackageWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new PackageDeltaNotification
            {
                Delta = new PackageDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<PackageReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<PackageReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class PackageVersionChangeTrackingManager : ChangeTrackingManagerBase, IPackageVersionChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalPackageVersionReadableModel> models =
            new ConcurrentDictionary<int, InternalPackageVersionReadableModel>();

        public event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<PackageVersionReadableModel>> Removed;

        public PackageVersionChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public PackageVersionWritableModel Create()
        {
            var model = new PackageVersionWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public PackageVersionWritableModel CreateCopy(PackageVersionReadableModel readableModel)
        {
            var model = PackageVersionWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(PackageVersionWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(PackageVersionReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<PackageVersionReadableModel>> QueryAsync(PackageVersionQuery query = null)
        {
            if (query == null)
            {
                query = PackageVersionQuery.Create();
            }

            var list = new List<PackageVersionReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IPackageVersionDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IPackageVersionDataService>();
        }

        private void Release(int id)
        {
            InternalPackageVersionReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalPackageVersionReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<PackageVersionDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<PackageVersionDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (PackageVersionWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new PackageVersionDeltaNotification
            {
                Delta = new PackageVersionDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<PackageVersionReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<PackageVersionReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

    public partial class MediaConfigurationChangeTrackingManager : ChangeTrackingManagerBase, IMediaConfigurationChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalMediaConfigurationReadableModel> models =
            new ConcurrentDictionary<int, InternalMediaConfigurationReadableModel>();

        public event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<MediaConfigurationReadableModel>> Removed;

        public MediaConfigurationChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public MediaConfigurationWritableModel Create()
        {
            var model = new MediaConfigurationWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public MediaConfigurationWritableModel CreateCopy(MediaConfigurationReadableModel readableModel)
        {
            var model = MediaConfigurationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(MediaConfigurationWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(MediaConfigurationReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<MediaConfigurationReadableModel>> QueryAsync(MediaConfigurationQuery query = null)
        {
            if (query == null)
            {
                query = MediaConfigurationQuery.Create();
            }

            var list = new List<MediaConfigurationReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IMediaConfigurationDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IMediaConfigurationDataService>();
        }

        private void Release(int id)
        {
            InternalMediaConfigurationReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalMediaConfigurationReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<MediaConfigurationDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<MediaConfigurationDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (MediaConfigurationWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new MediaConfigurationDeltaNotification
            {
                Delta = new MediaConfigurationDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UnitConfigurationChangeTrackingManager : ChangeTrackingManagerBase, IUnitConfigurationChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUnitConfigurationReadableModel> models =
            new ConcurrentDictionary<int, InternalUnitConfigurationReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UnitConfigurationReadableModel>> Removed;

        public UnitConfigurationChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UnitConfigurationWritableModel Create()
        {
            var model = new UnitConfigurationWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UnitConfigurationWritableModel CreateCopy(UnitConfigurationReadableModel readableModel)
        {
            var model = UnitConfigurationWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UnitConfigurationWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UnitConfigurationReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UnitConfigurationReadableModel>> QueryAsync(UnitConfigurationQuery query = null)
        {
            if (query == null)
            {
                query = UnitConfigurationQuery.Create();
            }

            var list = new List<UnitConfigurationReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUnitConfigurationDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUnitConfigurationDataService>();
        }

        private void Release(int id)
        {
            InternalUnitConfigurationReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUnitConfigurationReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UnitConfigurationDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UnitConfigurationDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UnitConfigurationWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UnitConfigurationDeltaNotification
            {
                Delta = new UnitConfigurationDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UnitConfigurationReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UnitConfigurationReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

    public partial class SystemConfigChangeTrackingManager : ChangeTrackingManagerBase, ISystemConfigChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalSystemConfigReadableModel> models =
            new ConcurrentDictionary<int, InternalSystemConfigReadableModel>();

        public event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<SystemConfigReadableModel>> Removed;

        public SystemConfigChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public SystemConfigWritableModel Create()
        {
            var model = new SystemConfigWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public SystemConfigWritableModel CreateCopy(SystemConfigReadableModel readableModel)
        {
            var model = SystemConfigWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(SystemConfigWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(SystemConfigReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<SystemConfigReadableModel>> QueryAsync(SystemConfigQuery query = null)
        {
            if (query == null)
            {
                query = SystemConfigQuery.Create();
            }

            var list = new List<SystemConfigReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<ISystemConfigDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<ISystemConfigDataService>();
        }

        private void Release(int id)
        {
            InternalSystemConfigReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalSystemConfigReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<SystemConfigDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<SystemConfigDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (SystemConfigWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new SystemConfigDeltaNotification
            {
                Delta = new SystemConfigDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<SystemConfigReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<SystemConfigReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }

    public partial class UserDefinedPropertyChangeTrackingManager : ChangeTrackingManagerBase, IUserDefinedPropertyChangeTrackingManager
    {
        private readonly ConcurrentDictionary<int, InternalUserDefinedPropertyReadableModel> models =
            new ConcurrentDictionary<int, InternalUserDefinedPropertyReadableModel>();

        public event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Added;

        public event EventHandler<ReadableModelEventArgs<UserDefinedPropertyReadableModel>> Removed;

        public UserDefinedPropertyChangeTrackingManager(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
            : base(configuration, userCredentials)
        {
        }

        public UserDefinedPropertyWritableModel Create()
        {
            var model = new UserDefinedPropertyWritableModel();
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public UserDefinedPropertyWritableModel CreateCopy(UserDefinedPropertyReadableModel readableModel)
        {
            var model = UserDefinedPropertyWritableModel.CreateCopyFrom(readableModel);
            model.Committed += this.TrackingModelOnCommitted;
            return model;
        }

        public async Task AddAsync(UserDefinedPropertyWritableModel model)
        {
            if (model.Id != default(int))
            {
                throw new Exception();
            }
            
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.AddAsync(entity);
            }
        }

        public async Task DeleteAsync(UserDefinedPropertyReadableModel model)
        {
            await this.WaitReadyAsync();
            var entity = model.ToDto();
            using (var channelScope = this.CreateChannelScope())
            {
                await channelScope.Channel.DeleteAsync(entity);
            }
        }

        public virtual async Task<IEnumerable<UserDefinedPropertyReadableModel>> QueryAsync(UserDefinedPropertyQuery query = null)
        {
            if (query == null)
            {
                query = UserDefinedPropertyQuery.Create();
            }

            var list = new List<UserDefinedPropertyReadableModel>();
            await this.WaitReadyAsync();
            var entities = await this.QueryEntitiesAsync(query);
            return entities.Select(entity => this.Wrap(entity, true)).ToList();
        }
        
        private ChannelScope<IUserDefinedPropertyDataService> CreateChannelScope()
        {
            return this.CreateChannelScope<IUserDefinedPropertyDataService>();
        }

        private void Release(int id)
        {
            InternalUserDefinedPropertyReadableModel value;
            if (this.models.TryRemove(id, out value))
            {
                value.ChangeTrackingModelUpdated -= this.TrackingModelOnCommitted;
            }
        }

        private void Track(InternalUserDefinedPropertyReadableModel readableModel)
        {
            this.models[readableModel.Id] = readableModel;
            readableModel.ChangeTrackingModelCreated += this.ReadableModelOnChangeTrackingModelCreated;
        }

        private void ReadableModelOnChangeTrackingModelCreated(
            object sender,
            ChangeTrackingModelCreatedEventArgs<UserDefinedPropertyDelta> writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Committed += this.TrackingModelOnCommitted;
        }

        private async void TrackingModelOnCommitted(
            object sender,
            ModelUpdatedEventArgs<UserDefinedPropertyDelta> writableModelUpdatedEventArgs)
        {
            Logger.Trace("Changes committed");
            var model = (UserDefinedPropertyWritableModel)sender;
            model.Committed -= this.TrackingModelOnCommitted;
            if (writableModelUpdatedEventArgs.Delta.Id == default(int))
            {
                try
                {
                    var dto = model.ToDto();
                    using (var channelScope = this.CreateChannelScope())
                    {
                        await channelScope.Channel.AddAsync(dto);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, "Error during commit");
                    this.RaiseOperationCompleted(model.Id, false);
                }

                return;
            }

            var delta = writableModelUpdatedEventArgs.Delta.GetChangedDelta();
            if (delta == null)
            {
                return;
            }

            var notification = new UserDefinedPropertyDeltaNotification
            {
                Delta = new UserDefinedPropertyDeltaMessage(delta),
                EntityId = model.Id,
                WasAccepted = true
            };
            await this.PostNotificationAsync(notification).ConfigureAwait(false);
        }

        private void RaiseAdded(ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
        {
			var newProperty = string.Empty;
            var handler = this.Added;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Added handler", exception);
            }
        }

        private void RaiseRemoved(ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
        {
            var handler = this.Removed;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, e);
            }
            catch(Exception exception)
            {
                this.Logger.WarnException("Error while calling Removed handler", exception);
            }
        }
    }
    }
}
