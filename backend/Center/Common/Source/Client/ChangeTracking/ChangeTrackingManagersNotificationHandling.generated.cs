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
	using Gorba.Center.Common.ServiceModel.Exceptions;
	using Gorba.Center.Common.ServiceModel.Notifications;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
	using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;

	namespace AccessControl
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
		using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        public partial class AuthorizationChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<AuthorizationReadableModel> CommitAndVerifyAsync(AuthorizationWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as AuthorizationDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Authorization'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(AuthorizationDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddAuthorizationAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveAuthorizationAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddAuthorizationAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<AuthorizationReadableModel>(model));
            }

            private void RemoveAuthorizationAsync(int entityId)
            {
                InternalAuthorizationReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<AuthorizationReadableModel>(model));
            }

            private async Task ApplyChangesAsync(AuthorizationDelta content)
            {
                InternalAuthorizationReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly AuthorizationChangeTrackingManager manager;

                private int id;

                public CommitVerification(AuthorizationChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(AuthorizationWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Authorization with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UserRoleChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UserRoleReadableModel> CommitAndVerifyAsync(UserRoleWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "Authorizations";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationAuthorizations = notification as AuthorizationDeltaNotification;
                if (notificationAuthorizations != null)
                {
                    await this.OnNotificationAsync(notificationAuthorizations);
                    return;
                }

                var deltaNotification = notification as UserRoleDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UserRole'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UserRoleDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUserRoleAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUserRoleAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUserRoleAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UserRoleReadableModel>(model));
            }

            private void RemoveUserRoleAsync(int entityId)
            {
                InternalUserRoleReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UserRoleReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UserRoleDelta content)
            {
                InternalUserRoleReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(AuthorizationDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUserRoleChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUserRoleAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUserRoleRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUserRoleChangedAsync(AuthorizationDelta delta)
            {
                if (delta.UserRole == null)
                {
                    return;
                }
                
                InternalUserRoleReadableModel model;
                if (delta.UserRole.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.UserRole.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.UserRole.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.UserRole.ReferenceId.Value
                        && m.Authorizations.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.UserRole.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUserRoleAddedAsync(AuthorizationDelta delta)
            {
                if (delta.UserRole == null || !delta.UserRole.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUserRoleReadableModel model;
                if (this.models.TryGetValue(delta.UserRole.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUserRoleRemovedAsync(AuthorizationDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUserRoleReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Authorizations.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UserRoleChangeTrackingManager manager;

                private int id;

                public CommitVerification(UserRoleChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UserRoleWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UserRole with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Membership
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
		using Gorba.Center.Common.ServiceModel.Filters.Membership;

        public partial class AssociationTenantUserUserRoleChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<AssociationTenantUserUserRoleReadableModel> CommitAndVerifyAsync(AssociationTenantUserUserRoleWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as AssociationTenantUserUserRoleDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'AssociationTenantUserUserRole'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(AssociationTenantUserUserRoleDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddAssociationTenantUserUserRoleAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveAssociationTenantUserUserRoleAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddAssociationTenantUserUserRoleAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>(model));
            }

            private void RemoveAssociationTenantUserUserRoleAsync(int entityId)
            {
                InternalAssociationTenantUserUserRoleReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel>(model));
            }

            private async Task ApplyChangesAsync(AssociationTenantUserUserRoleDelta content)
            {
                InternalAssociationTenantUserUserRoleReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly AssociationTenantUserUserRoleChangeTrackingManager manager;

                private int id;

                public CommitVerification(AssociationTenantUserUserRoleChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(AssociationTenantUserUserRoleWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update AssociationTenantUserUserRole with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class TenantChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<TenantReadableModel> CommitAndVerifyAsync(TenantWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "Users";
                yield return "UpdateGroups";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUsers = notification as UserDeltaNotification;
                if (notificationUsers != null)
                {
                    await this.OnNotificationAsync(notificationUsers);
                    return;
                }

                var notificationUpdateGroups = notification as UpdateGroupDeltaNotification;
                if (notificationUpdateGroups != null)
                {
                    await this.OnNotificationAsync(notificationUpdateGroups);
                    return;
                }

                var deltaNotification = notification as TenantDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Tenant'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(TenantDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddTenantAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveTenantAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddTenantAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<TenantReadableModel>(model));
            }

            private void RemoveTenantAsync(int entityId)
            {
                InternalTenantReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<TenantReadableModel>(model));
            }

            private async Task ApplyChangesAsync(TenantDelta content)
            {
                InternalTenantReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UserDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnOwnerTenantChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnOwnerTenantAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnOwnerTenantRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnOwnerTenantChangedAsync(UserDelta delta)
            {
                if (delta.OwnerTenant == null)
                {
                    return;
                }
                
                InternalTenantReadableModel model;
                if (delta.OwnerTenant.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.OwnerTenant.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.OwnerTenant.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.OwnerTenant.ReferenceId.Value
                        && m.Users.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.OwnerTenant.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnOwnerTenantAddedAsync(UserDelta delta)
            {
                if (delta.OwnerTenant == null || !delta.OwnerTenant.ReferenceId.HasValue)
                {
                    return;
                }

                InternalTenantReadableModel model;
                if (this.models.TryGetValue(delta.OwnerTenant.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnOwnerTenantRemovedAsync(UserDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalTenantReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Users.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            private async Task OnNotificationAsync(UpdateGroupDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnTenantChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnTenantAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnTenantRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnTenantChangedAsync(UpdateGroupDelta delta)
            {
                if (delta.Tenant == null)
                {
                    return;
                }
                
                InternalTenantReadableModel model;
                if (delta.Tenant.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.Tenant.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.Tenant.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.Tenant.ReferenceId.Value
                        && m.UpdateGroups.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.Tenant.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnTenantAddedAsync(UpdateGroupDelta delta)
            {
                if (delta.Tenant == null || !delta.Tenant.ReferenceId.HasValue)
                {
                    return;
                }

                InternalTenantReadableModel model;
                if (this.models.TryGetValue(delta.Tenant.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnTenantRemovedAsync(UpdateGroupDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalTenantReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.UpdateGroups.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly TenantChangeTrackingManager manager;

                private int id;

                public CommitVerification(TenantChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(TenantWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Tenant with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UserChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UserReadableModel> CommitAndVerifyAsync(UserWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "AssociationTenantUserUserRoles";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationAssociationTenantUserUserRoles = notification as AssociationTenantUserUserRoleDeltaNotification;
                if (notificationAssociationTenantUserUserRoles != null)
                {
                    await this.OnNotificationAsync(notificationAssociationTenantUserUserRoles);
                    return;
                }

                var deltaNotification = notification as UserDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'User'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UserDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUserAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUserAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUserAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UserReadableModel>(model));
            }

            private void RemoveUserAsync(int entityId)
            {
                InternalUserReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UserReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UserDelta content)
            {
                InternalUserReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(AssociationTenantUserUserRoleDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUserChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUserAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUserRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUserChangedAsync(AssociationTenantUserUserRoleDelta delta)
            {
                if (delta.User == null)
                {
                    return;
                }
                
                InternalUserReadableModel model;
                if (delta.User.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.User.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.User.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.User.ReferenceId.Value
                        && m.AssociationTenantUserUserRoles.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.User.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUserAddedAsync(AssociationTenantUserUserRoleDelta delta)
            {
                if (delta.User == null || !delta.User.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUserReadableModel model;
                if (this.models.TryGetValue(delta.User.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUserRemovedAsync(AssociationTenantUserUserRoleDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUserReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.AssociationTenantUserUserRoles.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UserChangeTrackingManager manager;

                private int id;

                public CommitVerification(UserChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UserWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update User with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Units
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
		using Gorba.Center.Common.ServiceModel.Filters.Units;

        public partial class ProductTypeChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<ProductTypeReadableModel> CommitAndVerifyAsync(ProductTypeWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "Units";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUnits = notification as UnitDeltaNotification;
                if (notificationUnits != null)
                {
                    await this.OnNotificationAsync(notificationUnits);
                    return;
                }

                var deltaNotification = notification as ProductTypeDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'ProductType'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(ProductTypeDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddProductTypeAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveProductTypeAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddProductTypeAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<ProductTypeReadableModel>(model));
            }

            private void RemoveProductTypeAsync(int entityId)
            {
                InternalProductTypeReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<ProductTypeReadableModel>(model));
            }

            private async Task ApplyChangesAsync(ProductTypeDelta content)
            {
                InternalProductTypeReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UnitDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnProductTypeChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnProductTypeAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnProductTypeRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnProductTypeChangedAsync(UnitDelta delta)
            {
                if (delta.ProductType == null)
                {
                    return;
                }
                
                InternalProductTypeReadableModel model;
                if (delta.ProductType.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.ProductType.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.ProductType.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.ProductType.ReferenceId.Value
                        && m.Units.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.ProductType.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnProductTypeAddedAsync(UnitDelta delta)
            {
                if (delta.ProductType == null || !delta.ProductType.ReferenceId.HasValue)
                {
                    return;
                }

                InternalProductTypeReadableModel model;
                if (this.models.TryGetValue(delta.ProductType.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnProductTypeRemovedAsync(UnitDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalProductTypeReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Units.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly ProductTypeChangeTrackingManager manager;

                private int id;

                public CommitVerification(ProductTypeChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(ProductTypeWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update ProductType with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UnitChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UnitReadableModel> CommitAndVerifyAsync(UnitWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "UpdateCommands";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUpdateCommands = notification as UpdateCommandDeltaNotification;
                if (notificationUpdateCommands != null)
                {
                    await this.OnNotificationAsync(notificationUpdateCommands);
                    return;
                }

                var deltaNotification = notification as UnitDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Unit'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UnitDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUnitAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUnitAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUnitAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UnitReadableModel>(model));
            }

            private void RemoveUnitAsync(int entityId)
            {
                InternalUnitReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UnitReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UnitDelta content)
            {
                InternalUnitReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UpdateCommandDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUnitChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUnitAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUnitRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUnitChangedAsync(UpdateCommandDelta delta)
            {
                if (delta.Unit == null)
                {
                    return;
                }
                
                InternalUnitReadableModel model;
                if (delta.Unit.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.Unit.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.Unit.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.Unit.ReferenceId.Value
                        && m.UpdateCommands.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.Unit.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUnitAddedAsync(UpdateCommandDelta delta)
            {
                if (delta.Unit == null || !delta.Unit.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUnitReadableModel model;
                if (this.models.TryGetValue(delta.Unit.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUnitRemovedAsync(UpdateCommandDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUnitReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.UpdateCommands.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UnitChangeTrackingManager manager;

                private int id;

                public CommitVerification(UnitChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UnitWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Unit with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Resources
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
		using Gorba.Center.Common.ServiceModel.Filters.Resources;

        public partial class ContentResourceChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<ContentResourceReadableModel> CommitAndVerifyAsync(ContentResourceWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as ContentResourceDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'ContentResource'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(ContentResourceDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddContentResourceAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveContentResourceAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddContentResourceAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<ContentResourceReadableModel>(model));
            }

            private void RemoveContentResourceAsync(int entityId)
            {
                InternalContentResourceReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<ContentResourceReadableModel>(model));
            }

            private async Task ApplyChangesAsync(ContentResourceDelta content)
            {
                InternalContentResourceReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly ContentResourceChangeTrackingManager manager;

                private int id;

                public CommitVerification(ContentResourceChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(ContentResourceWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update ContentResource with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class ResourceChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<ResourceReadableModel> CommitAndVerifyAsync(ResourceWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as ResourceDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Resource'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(ResourceDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddResourceAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveResourceAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddResourceAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<ResourceReadableModel>(model));
            }

            private void RemoveResourceAsync(int entityId)
            {
                InternalResourceReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<ResourceReadableModel>(model));
            }

            private async Task ApplyChangesAsync(ResourceDelta content)
            {
                InternalResourceReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly ResourceChangeTrackingManager manager;

                private int id;

                public CommitVerification(ResourceChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(ResourceWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Resource with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Update
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
		using Gorba.Center.Common.ServiceModel.Filters.Update;

        public partial class UpdateCommandChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UpdateCommandReadableModel> CommitAndVerifyAsync(UpdateCommandWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "UpdateParts";
                yield return "UpdateFeedbacks";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationIncludedParts = notification as UpdatePartDeltaNotification;
                if (notificationIncludedParts != null)
                {
                    await this.OnNotificationAsync(notificationIncludedParts);
                    return;
                }

                var notificationFeedbacks = notification as UpdateFeedbackDeltaNotification;
                if (notificationFeedbacks != null)
                {
                    await this.OnNotificationAsync(notificationFeedbacks);
                    return;
                }

                var deltaNotification = notification as UpdateCommandDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UpdateCommand'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UpdateCommandDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUpdateCommandAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUpdateCommandAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUpdateCommandAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UpdateCommandReadableModel>(model));
            }

            private void RemoveUpdateCommandAsync(int entityId)
            {
                InternalUpdateCommandReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UpdateCommandReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UpdateCommandDelta content)
            {
                InternalUpdateCommandReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            // inverse property missing for collection IncludedParts

            private async Task OnNotificationAsync(UpdatePartDeltaNotification notification)
            {
                await Task.FromResult(0);
                Logger.Trace("Ignoring notification '{0}'. The inverse property is not defined for collection 'IncludedParts'");
            }

            private async Task OnNotificationAsync(UpdateFeedbackDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUpdateCommandChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUpdateCommandAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUpdateCommandRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUpdateCommandChangedAsync(UpdateFeedbackDelta delta)
            {
                if (delta.UpdateCommand == null)
                {
                    return;
                }
                
                InternalUpdateCommandReadableModel model;
                if (delta.UpdateCommand.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.UpdateCommand.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.UpdateCommand.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.UpdateCommand.ReferenceId.Value
                        && m.Feedbacks.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.UpdateCommand.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateCommandAddedAsync(UpdateFeedbackDelta delta)
            {
                if (delta.UpdateCommand == null || !delta.UpdateCommand.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUpdateCommandReadableModel model;
                if (this.models.TryGetValue(delta.UpdateCommand.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateCommandRemovedAsync(UpdateFeedbackDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUpdateCommandReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Feedbacks.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UpdateCommandChangeTrackingManager manager;

                private int id;

                public CommitVerification(UpdateCommandChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UpdateCommandWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UpdateCommand with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UpdateFeedbackChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UpdateFeedbackReadableModel> CommitAndVerifyAsync(UpdateFeedbackWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as UpdateFeedbackDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UpdateFeedback'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UpdateFeedbackDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUpdateFeedbackAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUpdateFeedbackAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUpdateFeedbackAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UpdateFeedbackReadableModel>(model));
            }

            private void RemoveUpdateFeedbackAsync(int entityId)
            {
                InternalUpdateFeedbackReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UpdateFeedbackReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UpdateFeedbackDelta content)
            {
                InternalUpdateFeedbackReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UpdateFeedbackChangeTrackingManager manager;

                private int id;

                public CommitVerification(UpdateFeedbackChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UpdateFeedbackWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UpdateFeedback with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UpdateGroupChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UpdateGroupReadableModel> CommitAndVerifyAsync(UpdateGroupWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "Units";
                yield return "UpdateParts";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUnits = notification as UnitDeltaNotification;
                if (notificationUnits != null)
                {
                    await this.OnNotificationAsync(notificationUnits);
                    return;
                }

                var notificationUpdateParts = notification as UpdatePartDeltaNotification;
                if (notificationUpdateParts != null)
                {
                    await this.OnNotificationAsync(notificationUpdateParts);
                    return;
                }

                var deltaNotification = notification as UpdateGroupDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UpdateGroup'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UpdateGroupDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUpdateGroupAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUpdateGroupAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUpdateGroupAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UpdateGroupReadableModel>(model));
            }

            private void RemoveUpdateGroupAsync(int entityId)
            {
                InternalUpdateGroupReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UpdateGroupReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UpdateGroupDelta content)
            {
                InternalUpdateGroupReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UnitDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUpdateGroupChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUpdateGroupAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUpdateGroupRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUpdateGroupChangedAsync(UnitDelta delta)
            {
                if (delta.UpdateGroup == null)
                {
                    return;
                }
                
                InternalUpdateGroupReadableModel model;
                if (delta.UpdateGroup.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.UpdateGroup.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.UpdateGroup.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.UpdateGroup.ReferenceId.Value
                        && m.Units.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.UpdateGroup.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateGroupAddedAsync(UnitDelta delta)
            {
                if (delta.UpdateGroup == null || !delta.UpdateGroup.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUpdateGroupReadableModel model;
                if (this.models.TryGetValue(delta.UpdateGroup.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateGroupRemovedAsync(UnitDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUpdateGroupReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Units.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            private async Task OnNotificationAsync(UpdatePartDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUpdateGroupChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUpdateGroupAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUpdateGroupRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUpdateGroupChangedAsync(UpdatePartDelta delta)
            {
                if (delta.UpdateGroup == null)
                {
                    return;
                }
                
                InternalUpdateGroupReadableModel model;
                if (delta.UpdateGroup.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.UpdateGroup.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.UpdateGroup.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.UpdateGroup.ReferenceId.Value
                        && m.UpdateParts.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.UpdateGroup.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateGroupAddedAsync(UpdatePartDelta delta)
            {
                if (delta.UpdateGroup == null || !delta.UpdateGroup.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUpdateGroupReadableModel model;
                if (this.models.TryGetValue(delta.UpdateGroup.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUpdateGroupRemovedAsync(UpdatePartDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUpdateGroupReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.UpdateParts.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UpdateGroupChangeTrackingManager manager;

                private int id;

                public CommitVerification(UpdateGroupChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UpdateGroupWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UpdateGroup with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UpdatePartChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UpdatePartReadableModel> CommitAndVerifyAsync(UpdatePartWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "UpdateCommands";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationRelatedCommands = notification as UpdateCommandDeltaNotification;
                if (notificationRelatedCommands != null)
                {
                    await this.OnNotificationAsync(notificationRelatedCommands);
                    return;
                }

                var deltaNotification = notification as UpdatePartDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UpdatePart'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UpdatePartDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUpdatePartAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUpdatePartAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUpdatePartAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UpdatePartReadableModel>(model));
            }

            private void RemoveUpdatePartAsync(int entityId)
            {
                InternalUpdatePartReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UpdatePartReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UpdatePartDelta content)
            {
                InternalUpdatePartReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            // inverse property missing for collection RelatedCommands

            private async Task OnNotificationAsync(UpdateCommandDeltaNotification notification)
            {
                await Task.FromResult(0);
                Logger.Trace("Ignoring notification '{0}'. The inverse property is not defined for collection 'RelatedCommands'");
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UpdatePartChangeTrackingManager manager;

                private int id;

                public CommitVerification(UpdatePartChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UpdatePartWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UpdatePart with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Documents
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
		using Gorba.Center.Common.ServiceModel.Filters.Documents;

        public partial class DocumentChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<DocumentReadableModel> CommitAndVerifyAsync(DocumentWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "DocumentVersions";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationVersions = notification as DocumentVersionDeltaNotification;
                if (notificationVersions != null)
                {
                    await this.OnNotificationAsync(notificationVersions);
                    return;
                }

                var deltaNotification = notification as DocumentDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Document'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(DocumentDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddDocumentAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveDocumentAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddDocumentAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<DocumentReadableModel>(model));
            }

            private void RemoveDocumentAsync(int entityId)
            {
                InternalDocumentReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<DocumentReadableModel>(model));
            }

            private async Task ApplyChangesAsync(DocumentDelta content)
            {
                InternalDocumentReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(DocumentVersionDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnDocumentChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnDocumentAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnDocumentRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnDocumentChangedAsync(DocumentVersionDelta delta)
            {
                if (delta.Document == null)
                {
                    return;
                }
                
                InternalDocumentReadableModel model;
                if (delta.Document.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.Document.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.Document.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.Document.ReferenceId.Value
                        && m.Versions.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.Document.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnDocumentAddedAsync(DocumentVersionDelta delta)
            {
                if (delta.Document == null || !delta.Document.ReferenceId.HasValue)
                {
                    return;
                }

                InternalDocumentReadableModel model;
                if (this.models.TryGetValue(delta.Document.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnDocumentRemovedAsync(DocumentVersionDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalDocumentReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Versions.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly DocumentChangeTrackingManager manager;

                private int id;

                public CommitVerification(DocumentChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(DocumentWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Document with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class DocumentVersionChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<DocumentVersionReadableModel> CommitAndVerifyAsync(DocumentVersionWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as DocumentVersionDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'DocumentVersion'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(DocumentVersionDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddDocumentVersionAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveDocumentVersionAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddDocumentVersionAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<DocumentVersionReadableModel>(model));
            }

            private void RemoveDocumentVersionAsync(int entityId)
            {
                InternalDocumentVersionReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<DocumentVersionReadableModel>(model));
            }

            private async Task ApplyChangesAsync(DocumentVersionDelta content)
            {
                InternalDocumentVersionReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly DocumentVersionChangeTrackingManager manager;

                private int id;

                public CommitVerification(DocumentVersionChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(DocumentVersionWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update DocumentVersion with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Software
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
		using Gorba.Center.Common.ServiceModel.Filters.Software;

        public partial class PackageChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<PackageReadableModel> CommitAndVerifyAsync(PackageWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "PackageVersions";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationVersions = notification as PackageVersionDeltaNotification;
                if (notificationVersions != null)
                {
                    await this.OnNotificationAsync(notificationVersions);
                    return;
                }

                var deltaNotification = notification as PackageDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'Package'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(PackageDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddPackageAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemovePackageAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddPackageAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<PackageReadableModel>(model));
            }

            private void RemovePackageAsync(int entityId)
            {
                InternalPackageReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<PackageReadableModel>(model));
            }

            private async Task ApplyChangesAsync(PackageDelta content)
            {
                InternalPackageReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(PackageVersionDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnPackageChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnPackageAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnPackageRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnPackageChangedAsync(PackageVersionDelta delta)
            {
                if (delta.Package == null)
                {
                    return;
                }
                
                InternalPackageReadableModel model;
                if (delta.Package.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.Package.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.Package.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.Package.ReferenceId.Value
                        && m.Versions.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.Package.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnPackageAddedAsync(PackageVersionDelta delta)
            {
                if (delta.Package == null || !delta.Package.ReferenceId.HasValue)
                {
                    return;
                }

                InternalPackageReadableModel model;
                if (this.models.TryGetValue(delta.Package.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnPackageRemovedAsync(PackageVersionDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalPackageReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.Versions.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly PackageChangeTrackingManager manager;

                private int id;

                public CommitVerification(PackageChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(PackageWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update Package with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class PackageVersionChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<PackageVersionReadableModel> CommitAndVerifyAsync(PackageVersionWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as PackageVersionDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'PackageVersion'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(PackageVersionDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddPackageVersionAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemovePackageVersionAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddPackageVersionAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<PackageVersionReadableModel>(model));
            }

            private void RemovePackageVersionAsync(int entityId)
            {
                InternalPackageVersionReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<PackageVersionReadableModel>(model));
            }

            private async Task ApplyChangesAsync(PackageVersionDelta content)
            {
                InternalPackageVersionReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly PackageVersionChangeTrackingManager manager;

                private int id;

                public CommitVerification(PackageVersionChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(PackageVersionWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update PackageVersion with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}

	namespace Configurations
	{
		using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
		using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        public partial class MediaConfigurationChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<MediaConfigurationReadableModel> CommitAndVerifyAsync(MediaConfigurationWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "UpdateGroups";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUpdateGroups = notification as UpdateGroupDeltaNotification;
                if (notificationUpdateGroups != null)
                {
                    await this.OnNotificationAsync(notificationUpdateGroups);
                    return;
                }

                var deltaNotification = notification as MediaConfigurationDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'MediaConfiguration'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(MediaConfigurationDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddMediaConfigurationAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveMediaConfigurationAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddMediaConfigurationAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<MediaConfigurationReadableModel>(model));
            }

            private void RemoveMediaConfigurationAsync(int entityId)
            {
                InternalMediaConfigurationReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<MediaConfigurationReadableModel>(model));
            }

            private async Task ApplyChangesAsync(MediaConfigurationDelta content)
            {
                InternalMediaConfigurationReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UpdateGroupDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnMediaConfigurationChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnMediaConfigurationAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnMediaConfigurationRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnMediaConfigurationChangedAsync(UpdateGroupDelta delta)
            {
                if (delta.MediaConfiguration == null)
                {
                    return;
                }
                
                InternalMediaConfigurationReadableModel model;
                if (delta.MediaConfiguration.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.MediaConfiguration.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.MediaConfiguration.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.MediaConfiguration.ReferenceId.Value
                        && m.UpdateGroups.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.MediaConfiguration.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnMediaConfigurationAddedAsync(UpdateGroupDelta delta)
            {
                if (delta.MediaConfiguration == null || !delta.MediaConfiguration.ReferenceId.HasValue)
                {
                    return;
                }

                InternalMediaConfigurationReadableModel model;
                if (this.models.TryGetValue(delta.MediaConfiguration.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnMediaConfigurationRemovedAsync(UpdateGroupDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalMediaConfigurationReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.UpdateGroups.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly MediaConfigurationChangeTrackingManager manager;

                private int id;

                public CommitVerification(MediaConfigurationChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(MediaConfigurationWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update MediaConfiguration with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UnitConfigurationChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UnitConfigurationReadableModel> CommitAndVerifyAsync(UnitConfigurationWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected override IEnumerable<string> GetAdditionalSubscriptions()
            {
                yield return "UpdateGroups";
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var notificationUpdateGroups = notification as UpdateGroupDeltaNotification;
                if (notificationUpdateGroups != null)
                {
                    await this.OnNotificationAsync(notificationUpdateGroups);
                    return;
                }

                var deltaNotification = notification as UnitConfigurationDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UnitConfiguration'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UnitConfigurationDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUnitConfigurationAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUnitConfigurationAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUnitConfigurationAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UnitConfigurationReadableModel>(model));
            }

            private void RemoveUnitConfigurationAsync(int entityId)
            {
                InternalUnitConfigurationReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UnitConfigurationReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UnitConfigurationDelta content)
            {
                InternalUnitConfigurationReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            private async Task OnNotificationAsync(UpdateGroupDeltaNotification notification)
            {
                if (notification.Delta == null)
                {
                    return;
                }

                var delta = notification.Delta.CreateDelta();
                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.OnUnitConfigurationChangedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.OnUnitConfigurationAddedAsync(delta);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        await this.OnUnitConfigurationRemovedAsync(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task OnUnitConfigurationChangedAsync(UpdateGroupDelta delta)
            {
                if (delta.UnitConfiguration == null)
                {
                    return;
                }
                
                InternalUnitConfigurationReadableModel model;
                if (delta.UnitConfiguration.OriginalReferenceId.HasValue)
                {
                    if (this.models.TryGetValue(delta.UnitConfiguration.OriginalReferenceId.Value, out model))
                    {
                        await model.ApplyAsync(delta);
                    }
                }

                if (!delta.UnitConfiguration.ReferenceId.HasValue)
                {
                    return;
                }

                var removed =
                    this.models.Values.Where(
                        m =>
                        m.NavigationPropertiesLoaded
                        && m.Id != delta.UnitConfiguration.ReferenceId.Value
                        && m.UpdateGroups.Any(readableModel => readableModel.Id == delta.Id)).ToList();
                removed.ForEach(async m => await m.ApplyAsync(delta));
                if (this.models.TryGetValue(delta.UnitConfiguration.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUnitConfigurationAddedAsync(UpdateGroupDelta delta)
            {
                if (delta.UnitConfiguration == null || !delta.UnitConfiguration.ReferenceId.HasValue)
                {
                    return;
                }

                InternalUnitConfigurationReadableModel model;
                if (this.models.TryGetValue(delta.UnitConfiguration.ReferenceId.Value, out model))
                {
                    await model.ApplyAsync(delta);
                }
            }

            private async Task OnUnitConfigurationRemovedAsync(UpdateGroupDelta delta)
            {
                var modelsWithNavigationPropertiesLoaded =
                    this.models.Values.OfType<InternalUnitConfigurationReadableModel>().Where(model => model.NavigationPropertiesLoaded);
                var parent =
                    modelsWithNavigationPropertiesLoaded
                        .SingleOrDefault(model => model.UpdateGroups.Any(child => child.Id == delta.Id));
                if (parent == null)
                {
                    Logger.Trace("Couldn't find a parent containing the deleted item {0}", delta.Id);
                    return;
                }

                await parent.ApplyAsync(delta);
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UnitConfigurationChangeTrackingManager manager;

                private int id;

                public CommitVerification(UnitConfigurationChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UnitConfigurationWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UnitConfiguration with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
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

        public partial class SystemConfigChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<SystemConfigReadableModel> CommitAndVerifyAsync(SystemConfigWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as SystemConfigDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'SystemConfig'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(SystemConfigDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddSystemConfigAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveSystemConfigAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddSystemConfigAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<SystemConfigReadableModel>(model));
            }

            private void RemoveSystemConfigAsync(int entityId)
            {
                InternalSystemConfigReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<SystemConfigReadableModel>(model));
            }

            private async Task ApplyChangesAsync(SystemConfigDelta content)
            {
                InternalSystemConfigReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly SystemConfigChangeTrackingManager manager;

                private int id;

                public CommitVerification(SystemConfigChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(SystemConfigWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update SystemConfig with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }

        public partial class UserDefinedPropertyChangeTrackingManager
        {
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            public event EventHandler<OperationCompletedEventArgs> OperationCompleted;
            
            public async Task<UserDefinedPropertyReadableModel> CommitAndVerifyAsync(UserDefinedPropertyWritableModel writableModel)
            {
                if (!writableModel.HasChanges())
                {
                    return writableModel.ReadableModel;
                }

                await this.WaitReadyAsync();

                var commitVerification = new CommitVerification(this);
                var id = await commitVerification.CommitAndVerifyAsync(writableModel);
                return await this.GetAsync(id);
            }
            
            protected virtual void RaiseOperationCompleted(int entityId, bool succeeded)
            {
                var args = new OperationCompletedEventArgs(entityId, succeeded);
                this.RaiseOperationCompleted(args);
            }

            private void RaiseOperationCompleted(OperationCompletedEventArgs e)
            {
                var handler = this.OperationCompleted;
                if (handler == null)
                {
                    return;
                }

                try
                {
                    handler(this, e);
                }
                catch (Exception exception)
                {
                    this.Logger.WarnException("Error while calling OperationCompleted handler", exception);
                }
            }

            protected override async Task OnNotificationInternalAsync(Notification notification)
            {
                var deltaNotification = notification as UserDefinedPropertyDeltaNotification;
                if (deltaNotification == null)
                {
                    return;
                }

                Logger.Trace("Applying delta notification '{0}' to type 'UserDefinedProperty'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnNotificationAsync(UserDefinedPropertyDeltaNotification notification)
            {
                if (!notification.WasAccepted)
                {
                    this.Logger.Debug("Changes in notification '{0}' were not accepted", notification.Id);
                    this.RaiseOperationCompleted(notification.EntityId, false);
                    return;
                }

                switch (notification.NotificationType)
                {
                    case DeltaNotificationType.PropertiesChanged:
                        await this.ApplyChangesAsync(notification.Delta.CreateDelta());
                        break;
                    case DeltaNotificationType.EntityAdded:
                        await this.AddUserDefinedPropertyAsync(notification.EntityId);
                        break;
                    case DeltaNotificationType.EntityRemoved:
                        this.RemoveUserDefinedPropertyAsync(notification.EntityId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.RaiseOperationCompleted(notification.EntityId, true);
            }

            private async Task AddUserDefinedPropertyAsync(int entityId)
            {
                var model = await this.GetAsync(entityId);
                if (model == null)
                {
                    const string Message = "Received a notification about an added entity (id {0}), but it wasn't"
                        + " possible to get it back from the service";
                    throw new ChangeTrackingException(string.Format(Message, entityId));
                }

                this.RaiseAdded(new ReadableModelEventArgs<UserDefinedPropertyReadableModel>(model));
            }

            private void RemoveUserDefinedPropertyAsync(int entityId)
            {
                InternalUserDefinedPropertyReadableModel model;
                if (!this.models.TryGetValue(entityId, out model))
                {
                    return;
                }

                this.Release(entityId);
                this.RaiseRemoved(new ReadableModelEventArgs<UserDefinedPropertyReadableModel>(model));
            }

            private async Task ApplyChangesAsync(UserDefinedPropertyDelta content)
            {
                InternalUserDefinedPropertyReadableModel model;
                if (this.models.TryGetValue(content.Id, out model))
                {
                    Logger.Trace("Applying delta for version {0}", content.Version);
                    await model.ApplyAsync(content);
                }
            }

            public sealed class OperationCompletedEventArgs
            {
                public OperationCompletedEventArgs(int entityId, bool succeeded)
                {
                    this.Succeeded = succeeded;
                    this.EntityId = entityId;
                }

                public int EntityId { get; private set; }

                public bool Succeeded { get; private set; }
            }

            private class CommitVerification : CommitVerificationBase<int>
            {
                private readonly UserDefinedPropertyChangeTrackingManager manager;

                private int id;

                public CommitVerification(UserDefinedPropertyChangeTrackingManager manager)
                {
                    this.manager = manager;
                    this.manager.OperationCompleted += this.ManagerOnOperationCompleted;
                }

                public async Task<int> CommitAndVerifyAsync(UserDefinedPropertyWritableModel writableModel)
                {
                    this.id = writableModel.Id;
                    writableModel.Commit();
                    return await this.WaitCompletion();
                }

                private void ManagerOnOperationCompleted(object sender, OperationCompletedEventArgs e)
                {
                    if (e.EntityId != this.id && this.id != default(int))
                    {
                        return;
                    }

                    if (e.Succeeded)
                    {
                        this.TrySetResult(e.EntityId);
                    }
                    else
                    {
                        this.TrySetException("Couldn't update UserDefinedProperty with ID " + this.id);
                    }

                    this.manager.OperationCompleted -= this.ManagerOnOperationCompleted;
                }
            }
        }
	}
}