namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    using Version = Gorba.Center.Common.ServiceModel.ChangeTracking.Version;

    namespace AccessControl
    {
        public partial class AuthorizationChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UserRoles"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UserRoles", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new AuthorizationDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as AuthorizationDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Authorization entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new AuthorizationDelta(entity.Id, DeltaOperation.Created);
                if (entity.UserRole != null)
                {
                    delta.ChangeUserRole(entity.UserRole.Id);
                }

                var notification = new AuthorizationDeltaNotification
                {
                    Delta = new AuthorizationDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Authorization entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new AuthorizationDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new AuthorizationDeltaNotification
                                        {
                                            Delta = new AuthorizationDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Authorization original, Authorization entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new AuthorizationDelta(original);

                if (original.Permission != entity.Permission)
                {
                    delta.ChangePermission(entity.Permission);
                }

                var originalUserRoleId = original.UserRole == null ?  (int?)null : original.UserRole.Id;
                var entityUserRoleId = entity.UserRole == null ?  (int?)null : entity.UserRole.Id;
                if (originalUserRoleId != entityUserRoleId)
                {
                    delta.ChangeUserRole(entityUserRoleId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new AuthorizationDeltaNotification 
                {
                    Delta = new AuthorizationDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(AuthorizationDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(AuthorizationQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Permission != null)
                {
                    model.Permission = delta.Permission.Value;
                }

                if (delta.UserRole != null)
                {
                    if (!delta.UserRole.ReferenceId.HasValue)
                    {
                        model.UserRole = UserRole.Null;
                    }
                    else
                    {
                        model.UserRole = new UserRole { Id = delta.UserRole.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new AuthorizationDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new AuthorizationDeltaNotification
                {
                    Delta = new AuthorizationDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UserRoleChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                return Enumerable.Empty<Tuple<string, INotificationManager>>();
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UserRoleDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UserRoleDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UserRole entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UserRoleDelta(entity.Id, DeltaOperation.Created);
                foreach (var userDefinedProperty in entity.UserDefinedProperties)
                {
                    delta.UserDefinedPropertiesDelta.Add(userDefinedProperty.Key, userDefinedProperty.Value);
                }

                var notification = new UserRoleDeltaNotification
                {
                    Delta = new UserRoleDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UserRole entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UserRoleDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UserRoleDeltaNotification
                                        {
                                            Delta = new UserRoleDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UserRole original, UserRole entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UserRoleDelta(original);

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UserRoleDeltaNotification 
                {
                    Delta = new UserRoleDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UserRoleDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UserRoleQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UserRoleDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                foreach (var userDefinedProperty in delta.UserDefinedProperties)
                {
                    model.UserDefinedProperties[userDefinedProperty.Key] = userDefinedProperty.Value;
                }

                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UserRoleDeltaNotification
                {
                    Delta = new UserRoleDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Configurations
    {
        public partial class UnitConfigurationChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Documents"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Documents", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "ProductTypes"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("ProductTypes", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UnitConfigurationDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UnitConfigurationDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UnitConfiguration entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UnitConfigurationDelta(entity.Id, DeltaOperation.Created);
                if (entity.Document != null)
                {
                    delta.ChangeDocument(entity.Document.Id);
                }

                if (entity.ProductType != null)
                {
                    delta.ChangeProductType(entity.ProductType.Id);
                }

                var notification = new UnitConfigurationDeltaNotification
                {
                    Delta = new UnitConfigurationDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UnitConfiguration entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UnitConfigurationDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UnitConfigurationDeltaNotification
                                        {
                                            Delta = new UnitConfigurationDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UnitConfiguration original, UnitConfiguration entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UnitConfigurationDelta(original);

                var originalDocumentId = original.Document == null ?  (int?)null : original.Document.Id;
                var entityDocumentId = entity.Document == null ?  (int?)null : entity.Document.Id;
                if (originalDocumentId != entityDocumentId)
                {
                    delta.ChangeDocument(entityDocumentId);
                }

                var originalProductTypeId = original.ProductType == null ?  (int?)null : original.ProductType.Id;
                var entityProductTypeId = entity.ProductType == null ?  (int?)null : entity.ProductType.Id;
                if (originalProductTypeId != entityProductTypeId)
                {
                    delta.ChangeProductType(entityProductTypeId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UnitConfigurationDeltaNotification 
                {
                    Delta = new UnitConfigurationDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UnitConfigurationDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UnitConfigurationQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Document != null)
                {
                    if (!delta.Document.ReferenceId.HasValue)
                    {
                        model.Document = Document.Null;
                    }
                    else
                    {
                        model.Document = new Document { Id = delta.Document.ReferenceId.Value };
                    }
                }

                if (delta.ProductType != null)
                {
                    if (!delta.ProductType.ReferenceId.HasValue)
                    {
                        model.ProductType = ProductType.Null;
                    }
                    else
                    {
                        model.ProductType = new ProductType { Id = delta.ProductType.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UnitConfigurationDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UnitConfigurationDeltaNotification
                {
                    Delta = new UnitConfigurationDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class MediaConfigurationChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Documents"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Documents", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new MediaConfigurationDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as MediaConfigurationDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(MediaConfiguration entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new MediaConfigurationDelta(entity.Id, DeltaOperation.Created);
                if (entity.Document != null)
                {
                    delta.ChangeDocument(entity.Document.Id);
                }

                var notification = new MediaConfigurationDeltaNotification
                {
                    Delta = new MediaConfigurationDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(MediaConfiguration entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new MediaConfigurationDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new MediaConfigurationDeltaNotification
                                        {
                                            Delta = new MediaConfigurationDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(MediaConfiguration original, MediaConfiguration entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new MediaConfigurationDelta(original);

                var originalDocumentId = original.Document == null ?  (int?)null : original.Document.Id;
                var entityDocumentId = entity.Document == null ?  (int?)null : entity.Document.Id;
                if (originalDocumentId != entityDocumentId)
                {
                    delta.ChangeDocument(entityDocumentId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new MediaConfigurationDeltaNotification 
                {
                    Delta = new MediaConfigurationDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(MediaConfigurationDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(MediaConfigurationQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Document != null)
                {
                    if (!delta.Document.ReferenceId.HasValue)
                    {
                        model.Document = Document.Null;
                    }
                    else
                    {
                        model.Document = new Document { Id = delta.Document.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new MediaConfigurationDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new MediaConfigurationDeltaNotification
                {
                    Delta = new MediaConfigurationDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Documents
    {
        public partial class DocumentChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new DocumentDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as DocumentDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Document entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new DocumentDelta(entity.Id, DeltaOperation.Created);
                if (entity.Tenant != null)
                {
                    delta.ChangeTenant(entity.Tenant.Id);
                }

                var notification = new DocumentDeltaNotification
                {
                    Delta = new DocumentDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Document entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new DocumentDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new DocumentDeltaNotification
                                        {
                                            Delta = new DocumentDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Document original, Document entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new DocumentDelta(original);

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                var originalTenantId = original.Tenant == null ?  (int?)null : original.Tenant.Id;
                var entityTenantId = entity.Tenant == null ?  (int?)null : entity.Tenant.Id;
                if (originalTenantId != entityTenantId)
                {
                    delta.ChangeTenant(entityTenantId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new DocumentDeltaNotification 
                {
                    Delta = new DocumentDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(DocumentDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(DocumentQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Tenant != null)
                {
                    if (!delta.Tenant.ReferenceId.HasValue)
                    {
                        model.Tenant = Tenant.Null;
                    }
                    else
                    {
                        model.Tenant = new Tenant { Id = delta.Tenant.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new DocumentDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new DocumentDeltaNotification
                {
                    Delta = new DocumentDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class DocumentVersionChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Documents"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Documents", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Users"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Users", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new DocumentVersionDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as DocumentVersionDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(DocumentVersion entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new DocumentVersionDelta(entity.Id, DeltaOperation.Created);
                if (entity.Document != null)
                {
                    delta.ChangeDocument(entity.Document.Id);
                }

                if (entity.CreatingUser != null)
                {
                    delta.ChangeCreatingUser(entity.CreatingUser.Id);
                }

                var notification = new DocumentVersionDeltaNotification
                {
                    Delta = new DocumentVersionDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(DocumentVersion entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new DocumentVersionDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new DocumentVersionDeltaNotification
                                        {
                                            Delta = new DocumentVersionDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(DocumentVersion original, DocumentVersion entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new DocumentVersionDelta(original);

                if (original.Major != entity.Major)
                {
                    delta.ChangeMajor(entity.Major);
                }

                if (original.Minor != entity.Minor)
                {
                    delta.ChangeMinor(entity.Minor);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.Content != entity.Content)
                {
                    delta.ChangeContent(entity.Content);
                }

                var originalDocumentId = original.Document == null ?  (int?)null : original.Document.Id;
                var entityDocumentId = entity.Document == null ?  (int?)null : entity.Document.Id;
                if (originalDocumentId != entityDocumentId)
                {
                    delta.ChangeDocument(entityDocumentId);
                }

                var originalCreatingUserId = original.CreatingUser == null ?  (int?)null : original.CreatingUser.Id;
                var entityCreatingUserId = entity.CreatingUser == null ?  (int?)null : entity.CreatingUser.Id;
                if (originalCreatingUserId != entityCreatingUserId)
                {
                    delta.ChangeCreatingUser(entityCreatingUserId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new DocumentVersionDeltaNotification 
                {
                    Delta = new DocumentVersionDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(DocumentVersionDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(DocumentVersionQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Major != null)
                {
                    model.Major = delta.Major.Value;
                }

                if (delta.Minor != null)
                {
                    model.Minor = delta.Minor.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Content != null)
                {
                    model.Content = delta.Content.Value;
                }

                if (delta.Document != null)
                {
                    if (!delta.Document.ReferenceId.HasValue)
                    {
                        model.Document = Document.Null;
                    }
                    else
                    {
                        model.Document = new Document { Id = delta.Document.ReferenceId.Value };
                    }
                }

                if (delta.CreatingUser != null)
                {
                    if (!delta.CreatingUser.ReferenceId.HasValue)
                    {
                        model.CreatingUser = User.Null;
                    }
                    else
                    {
                        model.CreatingUser = new User { Id = delta.CreatingUser.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new DocumentVersionDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new DocumentVersionDeltaNotification
                {
                    Delta = new DocumentVersionDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Log
    {    }

    namespace Membership
    {
        public partial class TenantChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                return Enumerable.Empty<Tuple<string, INotificationManager>>();
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new TenantDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as TenantDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Tenant entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new TenantDelta(entity.Id, DeltaOperation.Created);
                foreach (var userDefinedProperty in entity.UserDefinedProperties)
                {
                    delta.UserDefinedPropertiesDelta.Add(userDefinedProperty.Key, userDefinedProperty.Value);
                }

                var notification = new TenantDeltaNotification
                {
                    Delta = new TenantDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Tenant entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new TenantDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new TenantDeltaNotification
                                        {
                                            Delta = new TenantDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Tenant original, Tenant entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new TenantDelta(original);

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new TenantDeltaNotification 
                {
                    Delta = new TenantDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(TenantDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(TenantQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new TenantDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                foreach (var userDefinedProperty in delta.UserDefinedProperties)
                {
                    model.UserDefinedProperties[userDefinedProperty.Key] = userDefinedProperty.Value;
                }

                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new TenantDeltaNotification
                {
                    Delta = new TenantDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UserChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UserDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UserDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(User entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UserDelta(entity.Id, DeltaOperation.Created);
                if (entity.OwnerTenant != null)
                {
                    delta.ChangeOwnerTenant(entity.OwnerTenant.Id);
                }

                foreach (var userDefinedProperty in entity.UserDefinedProperties)
                {
                    delta.UserDefinedPropertiesDelta.Add(userDefinedProperty.Key, userDefinedProperty.Value);
                }

                var notification = new UserDeltaNotification
                {
                    Delta = new UserDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(User entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UserDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UserDeltaNotification
                                        {
                                            Delta = new UserDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(User original, User entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UserDelta(original);

                if (original.Username != entity.Username)
                {
                    delta.ChangeUsername(entity.Username);
                }

                if (original.Domain != entity.Domain)
                {
                    delta.ChangeDomain(entity.Domain);
                }

                if (original.HashedPassword != entity.HashedPassword)
                {
                    delta.ChangeHashedPassword(entity.HashedPassword);
                }

                if (original.FirstName != entity.FirstName)
                {
                    delta.ChangeFirstName(entity.FirstName);
                }

                if (original.LastName != entity.LastName)
                {
                    delta.ChangeLastName(entity.LastName);
                }

                if (original.Email != entity.Email)
                {
                    delta.ChangeEmail(entity.Email);
                }

                if (original.Culture != entity.Culture)
                {
                    delta.ChangeCulture(entity.Culture);
                }

                if (original.TimeZone != entity.TimeZone)
                {
                    delta.ChangeTimeZone(entity.TimeZone);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.LastLoginAttempt != entity.LastLoginAttempt)
                {
                    delta.ChangeLastLoginAttempt(entity.LastLoginAttempt);
                }

                if (original.LastSuccessfulLogin != entity.LastSuccessfulLogin)
                {
                    delta.ChangeLastSuccessfulLogin(entity.LastSuccessfulLogin);
                }

                if (original.ConsecutiveLoginFailures != entity.ConsecutiveLoginFailures)
                {
                    delta.ChangeConsecutiveLoginFailures(entity.ConsecutiveLoginFailures);
                }

                if (original.IsEnabled != entity.IsEnabled)
                {
                    delta.ChangeIsEnabled(entity.IsEnabled);
                }

                var originalOwnerTenantId = original.OwnerTenant == null ?  (int?)null : original.OwnerTenant.Id;
                var entityOwnerTenantId = entity.OwnerTenant == null ?  (int?)null : entity.OwnerTenant.Id;
                if (originalOwnerTenantId != entityOwnerTenantId)
                {
                    delta.ChangeOwnerTenant(entityOwnerTenantId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UserDeltaNotification 
                {
                    Delta = new UserDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UserDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UserQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Username != null)
                {
                    model.Username = delta.Username.Value;
                }

                if (delta.Domain != null)
                {
                    model.Domain = delta.Domain.Value;
                }

                if (delta.HashedPassword != null)
                {
                    model.HashedPassword = delta.HashedPassword.Value;
                }

                if (delta.FirstName != null)
                {
                    model.FirstName = delta.FirstName.Value;
                }

                if (delta.LastName != null)
                {
                    model.LastName = delta.LastName.Value;
                }

                if (delta.Email != null)
                {
                    model.Email = delta.Email.Value;
                }

                if (delta.Culture != null)
                {
                    model.Culture = delta.Culture.Value;
                }

                if (delta.TimeZone != null)
                {
                    model.TimeZone = delta.TimeZone.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.LastLoginAttempt != null)
                {
                    model.LastLoginAttempt = delta.LastLoginAttempt.Value;
                }

                if (delta.LastSuccessfulLogin != null)
                {
                    model.LastSuccessfulLogin = delta.LastSuccessfulLogin.Value;
                }

                if (delta.ConsecutiveLoginFailures != null)
                {
                    model.ConsecutiveLoginFailures = delta.ConsecutiveLoginFailures.Value;
                }

                if (delta.IsEnabled != null)
                {
                    model.IsEnabled = delta.IsEnabled.Value;
                }

                if (delta.OwnerTenant != null)
                {
                    if (!delta.OwnerTenant.ReferenceId.HasValue)
                    {
                        model.OwnerTenant = Tenant.Null;
                    }
                    else
                    {
                        model.OwnerTenant = new Tenant { Id = delta.OwnerTenant.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UserDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                foreach (var userDefinedProperty in delta.UserDefinedProperties)
                {
                    model.UserDefinedProperties[userDefinedProperty.Key] = userDefinedProperty.Value;
                }

                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UserDeltaNotification
                {
                    Delta = new UserDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class AssociationTenantUserUserRoleChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Users"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Users", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UserRoles"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UserRoles", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new AssociationTenantUserUserRoleDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as AssociationTenantUserUserRoleDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(AssociationTenantUserUserRole entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new AssociationTenantUserUserRoleDelta(entity.Id, DeltaOperation.Created);
                if (entity.Tenant != null)
                {
                    delta.ChangeTenant(entity.Tenant.Id);
                }

                if (entity.User != null)
                {
                    delta.ChangeUser(entity.User.Id);
                }

                if (entity.UserRole != null)
                {
                    delta.ChangeUserRole(entity.UserRole.Id);
                }

                var notification = new AssociationTenantUserUserRoleDeltaNotification
                {
                    Delta = new AssociationTenantUserUserRoleDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(AssociationTenantUserUserRole entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new AssociationTenantUserUserRoleDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new AssociationTenantUserUserRoleDeltaNotification
                                        {
                                            Delta = new AssociationTenantUserUserRoleDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(AssociationTenantUserUserRole original, AssociationTenantUserUserRole entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new AssociationTenantUserUserRoleDelta(original);

                var originalTenantId = original.Tenant == null ?  (int?)null : original.Tenant.Id;
                var entityTenantId = entity.Tenant == null ?  (int?)null : entity.Tenant.Id;
                if (originalTenantId != entityTenantId)
                {
                    delta.ChangeTenant(entityTenantId);
                }

                var originalUserId = original.User == null ?  (int?)null : original.User.Id;
                var entityUserId = entity.User == null ?  (int?)null : entity.User.Id;
                if (originalUserId != entityUserId)
                {
                    delta.ChangeUser(entityUserId);
                }

                var originalUserRoleId = original.UserRole == null ?  (int?)null : original.UserRole.Id;
                var entityUserRoleId = entity.UserRole == null ?  (int?)null : entity.UserRole.Id;
                if (originalUserRoleId != entityUserRoleId)
                {
                    delta.ChangeUserRole(entityUserRoleId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new AssociationTenantUserUserRoleDeltaNotification 
                {
                    Delta = new AssociationTenantUserUserRoleDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(AssociationTenantUserUserRoleDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(AssociationTenantUserUserRoleQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Tenant != null)
                {
                    if (!delta.Tenant.ReferenceId.HasValue)
                    {
                        model.Tenant = Tenant.Null;
                    }
                    else
                    {
                        model.Tenant = new Tenant { Id = delta.Tenant.ReferenceId.Value };
                    }
                }

                if (delta.User != null)
                {
                    if (!delta.User.ReferenceId.HasValue)
                    {
                        model.User = User.Null;
                    }
                    else
                    {
                        model.User = new User { Id = delta.User.ReferenceId.Value };
                    }
                }

                if (delta.UserRole != null)
                {
                    if (!delta.UserRole.ReferenceId.HasValue)
                    {
                        model.UserRole = UserRole.Null;
                    }
                    else
                    {
                        model.UserRole = new UserRole { Id = delta.UserRole.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new AssociationTenantUserUserRoleDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new AssociationTenantUserUserRoleDeltaNotification
                {
                    Delta = new AssociationTenantUserUserRoleDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Meta
    {
        public partial class UserDefinedPropertyChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UserDefinedPropertyDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UserDefinedPropertyDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UserDefinedProperty entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UserDefinedPropertyDelta(entity.Id, DeltaOperation.Created);
                if (entity.Tenant != null)
                {
                    delta.ChangeTenant(entity.Tenant.Id);
                }

                var notification = new UserDefinedPropertyDeltaNotification
                {
                    Delta = new UserDefinedPropertyDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UserDefinedProperty entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UserDefinedPropertyDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UserDefinedPropertyDeltaNotification
                                        {
                                            Delta = new UserDefinedPropertyDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UserDefinedProperty original, UserDefinedProperty entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UserDefinedPropertyDelta(original);

                if (original.OwnerEntity != entity.OwnerEntity)
                {
                    delta.ChangeOwnerEntity(entity.OwnerEntity);
                }

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                var originalTenantId = original.Tenant == null ?  (int?)null : original.Tenant.Id;
                var entityTenantId = entity.Tenant == null ?  (int?)null : entity.Tenant.Id;
                if (originalTenantId != entityTenantId)
                {
                    delta.ChangeTenant(entityTenantId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UserDefinedPropertyDeltaNotification 
                {
                    Delta = new UserDefinedPropertyDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UserDefinedPropertyDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UserDefinedPropertyQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.OwnerEntity != null)
                {
                    model.OwnerEntity = delta.OwnerEntity.Value;
                }

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Tenant != null)
                {
                    if (!delta.Tenant.ReferenceId.HasValue)
                    {
                        model.Tenant = Tenant.Null;
                    }
                    else
                    {
                        model.Tenant = new Tenant { Id = delta.Tenant.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UserDefinedPropertyDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UserDefinedPropertyDeltaNotification
                {
                    Delta = new UserDefinedPropertyDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class SystemConfigChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                return Enumerable.Empty<Tuple<string, INotificationManager>>();
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new SystemConfigDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as SystemConfigDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(SystemConfig entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new SystemConfigDelta(entity.Id, DeltaOperation.Created);
                var notification = new SystemConfigDeltaNotification
                {
                    Delta = new SystemConfigDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(SystemConfig entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new SystemConfigDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new SystemConfigDeltaNotification
                                        {
                                            Delta = new SystemConfigDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(SystemConfig original, SystemConfig entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new SystemConfigDelta(original);

                if (original.SystemId != entity.SystemId)
                {
                    delta.ChangeSystemId(entity.SystemId);
                }

                if (original.Settings != entity.Settings)
                {
                    delta.ChangeSettings(entity.Settings);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new SystemConfigDeltaNotification 
                {
                    Delta = new SystemConfigDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(SystemConfigDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(SystemConfigQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.SystemId != null)
                {
                    model.SystemId = delta.SystemId.Value;
                }

                if (delta.Settings != null)
                {
                    model.Settings = delta.Settings.Value;
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new SystemConfigDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new SystemConfigDeltaNotification
                {
                    Delta = new SystemConfigDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Resources
    {
        public partial class ResourceChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Users"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Users", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new ResourceDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as ResourceDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Resource entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new ResourceDelta(entity.Id, DeltaOperation.Created);
                if (entity.UploadingUser != null)
                {
                    delta.ChangeUploadingUser(entity.UploadingUser.Id);
                }

                var notification = new ResourceDeltaNotification
                {
                    Delta = new ResourceDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Resource entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new ResourceDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new ResourceDeltaNotification
                                        {
                                            Delta = new ResourceDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Resource original, Resource entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new ResourceDelta(original);

                if (original.OriginalFilename != entity.OriginalFilename)
                {
                    delta.ChangeOriginalFilename(entity.OriginalFilename);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.Hash != entity.Hash)
                {
                    delta.ChangeHash(entity.Hash);
                }

                if (original.ThumbnailHash != entity.ThumbnailHash)
                {
                    delta.ChangeThumbnailHash(entity.ThumbnailHash);
                }

                if (original.MimeType != entity.MimeType)
                {
                    delta.ChangeMimeType(entity.MimeType);
                }

                if (original.Length != entity.Length)
                {
                    delta.ChangeLength(entity.Length);
                }

                var originalUploadingUserId = original.UploadingUser == null ?  (int?)null : original.UploadingUser.Id;
                var entityUploadingUserId = entity.UploadingUser == null ?  (int?)null : entity.UploadingUser.Id;
                if (originalUploadingUserId != entityUploadingUserId)
                {
                    delta.ChangeUploadingUser(entityUploadingUserId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new ResourceDeltaNotification 
                {
                    Delta = new ResourceDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(ResourceDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(ResourceQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.OriginalFilename != null)
                {
                    model.OriginalFilename = delta.OriginalFilename.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Hash != null)
                {
                    model.Hash = delta.Hash.Value;
                }

                if (delta.ThumbnailHash != null)
                {
                    model.ThumbnailHash = delta.ThumbnailHash.Value;
                }

                if (delta.MimeType != null)
                {
                    model.MimeType = delta.MimeType.Value;
                }

                if (delta.Length != null)
                {
                    model.Length = delta.Length.Value;
                }

                if (delta.UploadingUser != null)
                {
                    if (!delta.UploadingUser.ReferenceId.HasValue)
                    {
                        model.UploadingUser = User.Null;
                    }
                    else
                    {
                        model.UploadingUser = new User { Id = delta.UploadingUser.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new ResourceDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new ResourceDeltaNotification
                {
                    Delta = new ResourceDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class ContentResourceChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Users"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Users", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new ContentResourceDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as ContentResourceDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(ContentResource entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new ContentResourceDelta(entity.Id, DeltaOperation.Created);
                if (entity.UploadingUser != null)
                {
                    delta.ChangeUploadingUser(entity.UploadingUser.Id);
                }

                var notification = new ContentResourceDeltaNotification
                {
                    Delta = new ContentResourceDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(ContentResource entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new ContentResourceDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new ContentResourceDeltaNotification
                                        {
                                            Delta = new ContentResourceDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(ContentResource original, ContentResource entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new ContentResourceDelta(original);

                if (original.OriginalFilename != entity.OriginalFilename)
                {
                    delta.ChangeOriginalFilename(entity.OriginalFilename);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.ThumbnailHash != entity.ThumbnailHash)
                {
                    delta.ChangeThumbnailHash(entity.ThumbnailHash);
                }

                if (original.Hash != entity.Hash)
                {
                    delta.ChangeHash(entity.Hash);
                }

                if (original.HashAlgorithmType != entity.HashAlgorithmType)
                {
                    delta.ChangeHashAlgorithmType(entity.HashAlgorithmType);
                }

                if (original.MimeType != entity.MimeType)
                {
                    delta.ChangeMimeType(entity.MimeType);
                }

                if (original.Length != entity.Length)
                {
                    delta.ChangeLength(entity.Length);
                }

                var originalUploadingUserId = original.UploadingUser == null ?  (int?)null : original.UploadingUser.Id;
                var entityUploadingUserId = entity.UploadingUser == null ?  (int?)null : entity.UploadingUser.Id;
                if (originalUploadingUserId != entityUploadingUserId)
                {
                    delta.ChangeUploadingUser(entityUploadingUserId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new ContentResourceDeltaNotification 
                {
                    Delta = new ContentResourceDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(ContentResourceDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(ContentResourceQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.OriginalFilename != null)
                {
                    model.OriginalFilename = delta.OriginalFilename.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.ThumbnailHash != null)
                {
                    model.ThumbnailHash = delta.ThumbnailHash.Value;
                }

                if (delta.Hash != null)
                {
                    model.Hash = delta.Hash.Value;
                }

                if (delta.HashAlgorithmType != null)
                {
                    model.HashAlgorithmType = delta.HashAlgorithmType.Value;
                }

                if (delta.MimeType != null)
                {
                    model.MimeType = delta.MimeType.Value;
                }

                if (delta.Length != null)
                {
                    model.Length = delta.Length.Value;
                }

                if (delta.UploadingUser != null)
                {
                    if (!delta.UploadingUser.ReferenceId.HasValue)
                    {
                        model.UploadingUser = User.Null;
                    }
                    else
                    {
                        model.UploadingUser = new User { Id = delta.UploadingUser.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new ContentResourceDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new ContentResourceDeltaNotification
                {
                    Delta = new ContentResourceDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Software
    {
        public partial class PackageChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                return Enumerable.Empty<Tuple<string, INotificationManager>>();
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new PackageDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as PackageDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Package entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new PackageDelta(entity.Id, DeltaOperation.Created);
                var notification = new PackageDeltaNotification
                {
                    Delta = new PackageDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Package entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new PackageDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new PackageDeltaNotification
                                        {
                                            Delta = new PackageDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Package original, Package entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new PackageDelta(original);

                if (original.PackageId != entity.PackageId)
                {
                    delta.ChangePackageId(entity.PackageId);
                }

                if (original.ProductName != entity.ProductName)
                {
                    delta.ChangeProductName(entity.ProductName);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new PackageDeltaNotification 
                {
                    Delta = new PackageDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(PackageDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(PackageQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.PackageId != null)
                {
                    model.PackageId = delta.PackageId.Value;
                }

                if (delta.ProductName != null)
                {
                    model.ProductName = delta.ProductName.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new PackageDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new PackageDeltaNotification
                {
                    Delta = new PackageDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class PackageVersionChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Packages"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Packages", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new PackageVersionDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as PackageVersionDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(PackageVersion entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new PackageVersionDelta(entity.Id, DeltaOperation.Created);
                if (entity.Package != null)
                {
                    delta.ChangePackage(entity.Package.Id);
                }

                var notification = new PackageVersionDeltaNotification
                {
                    Delta = new PackageVersionDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(PackageVersion entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new PackageVersionDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new PackageVersionDeltaNotification
                                        {
                                            Delta = new PackageVersionDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(PackageVersion original, PackageVersion entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new PackageVersionDelta(original);

                if (original.SoftwareVersion != entity.SoftwareVersion)
                {
                    delta.ChangeSoftwareVersion(entity.SoftwareVersion);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.Structure != entity.Structure)
                {
                    delta.ChangeStructure(entity.Structure);
                }

                var originalPackageId = original.Package == null ?  (int?)null : original.Package.Id;
                var entityPackageId = entity.Package == null ?  (int?)null : entity.Package.Id;
                if (originalPackageId != entityPackageId)
                {
                    delta.ChangePackage(entityPackageId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new PackageVersionDeltaNotification 
                {
                    Delta = new PackageVersionDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(PackageVersionDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(PackageVersionQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.SoftwareVersion != null)
                {
                    model.SoftwareVersion = delta.SoftwareVersion.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Structure != null)
                {
                    model.Structure = delta.Structure.Value;
                }

                if (delta.Package != null)
                {
                    if (!delta.Package.ReferenceId.HasValue)
                    {
                        model.Package = Package.Null;
                    }
                    else
                    {
                        model.Package = new Package { Id = delta.Package.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new PackageVersionDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new PackageVersionDeltaNotification
                {
                    Delta = new PackageVersionDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Units
    {
        public partial class ProductTypeChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                return Enumerable.Empty<Tuple<string, INotificationManager>>();
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new ProductTypeDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as ProductTypeDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(ProductType entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new ProductTypeDelta(entity.Id, DeltaOperation.Created);
                var notification = new ProductTypeDeltaNotification
                {
                    Delta = new ProductTypeDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(ProductType entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new ProductTypeDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new ProductTypeDeltaNotification
                                        {
                                            Delta = new ProductTypeDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(ProductType original, ProductType entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new ProductTypeDelta(original);

                if (original.UnitType != entity.UnitType)
                {
                    delta.ChangeUnitType(entity.UnitType);
                }

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.HardwareDescriptor != entity.HardwareDescriptor)
                {
                    delta.ChangeHardwareDescriptor(entity.HardwareDescriptor);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new ProductTypeDeltaNotification 
                {
                    Delta = new ProductTypeDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(ProductTypeDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(ProductTypeQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.UnitType != null)
                {
                    model.UnitType = delta.UnitType.Value;
                }

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.HardwareDescriptor != null)
                {
                    model.HardwareDescriptor = delta.HardwareDescriptor.Value;
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new ProductTypeDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new ProductTypeDeltaNotification
                {
                    Delta = new ProductTypeDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UnitChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "ProductTypes"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("ProductTypes", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UpdateGroups"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UpdateGroups", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UnitDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UnitDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(Unit entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UnitDelta(entity.Id, DeltaOperation.Created);
                if (entity.Tenant != null)
                {
                    delta.ChangeTenant(entity.Tenant.Id);
                }

                if (entity.ProductType != null)
                {
                    delta.ChangeProductType(entity.ProductType.Id);
                }

                if (entity.UpdateGroup != null)
                {
                    delta.ChangeUpdateGroup(entity.UpdateGroup.Id);
                }

                foreach (var userDefinedProperty in entity.UserDefinedProperties)
                {
                    delta.UserDefinedPropertiesDelta.Add(userDefinedProperty.Key, userDefinedProperty.Value);
                }

                var notification = new UnitDeltaNotification
                {
                    Delta = new UnitDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(Unit entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UnitDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UnitDeltaNotification
                                        {
                                            Delta = new UnitDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(Unit original, Unit entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UnitDelta(original);

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.NetworkAddress != entity.NetworkAddress)
                {
                    delta.ChangeNetworkAddress(entity.NetworkAddress);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.IsConnected != entity.IsConnected)
                {
                    delta.ChangeIsConnected(entity.IsConnected);
                }

                var originalTenantId = original.Tenant == null ?  (int?)null : original.Tenant.Id;
                var entityTenantId = entity.Tenant == null ?  (int?)null : entity.Tenant.Id;
                if (originalTenantId != entityTenantId)
                {
                    delta.ChangeTenant(entityTenantId);
                }

                var originalProductTypeId = original.ProductType == null ?  (int?)null : original.ProductType.Id;
                var entityProductTypeId = entity.ProductType == null ?  (int?)null : entity.ProductType.Id;
                if (originalProductTypeId != entityProductTypeId)
                {
                    delta.ChangeProductType(entityProductTypeId);
                }

                var originalUpdateGroupId = original.UpdateGroup == null ?  (int?)null : original.UpdateGroup.Id;
                var entityUpdateGroupId = entity.UpdateGroup == null ?  (int?)null : entity.UpdateGroup.Id;
                if (originalUpdateGroupId != entityUpdateGroupId)
                {
                    delta.ChangeUpdateGroup(entityUpdateGroupId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UnitDeltaNotification 
                {
                    Delta = new UnitDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UnitDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UnitQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.NetworkAddress != null)
                {
                    model.NetworkAddress = delta.NetworkAddress.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.IsConnected != null)
                {
                    model.IsConnected = delta.IsConnected.Value;
                }

                if (delta.Tenant != null)
                {
                    if (!delta.Tenant.ReferenceId.HasValue)
                    {
                        model.Tenant = Tenant.Null;
                    }
                    else
                    {
                        model.Tenant = new Tenant { Id = delta.Tenant.ReferenceId.Value };
                    }
                }

                if (delta.ProductType != null)
                {
                    if (!delta.ProductType.ReferenceId.HasValue)
                    {
                        model.ProductType = ProductType.Null;
                    }
                    else
                    {
                        model.ProductType = new ProductType { Id = delta.ProductType.ReferenceId.Value };
                    }
                }

                if (delta.UpdateGroup != null)
                {
                    if (!delta.UpdateGroup.ReferenceId.HasValue)
                    {
                        model.UpdateGroup = UpdateGroup.Null;
                    }
                    else
                    {
                        model.UpdateGroup = new UpdateGroup { Id = delta.UpdateGroup.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UnitDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                foreach (var userDefinedProperty in delta.UserDefinedProperties)
                {
                    model.UserDefinedProperties[userDefinedProperty.Key] = userDefinedProperty.Value;
                }

                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UnitDeltaNotification
                {
                    Delta = new UnitDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }

    namespace Update
    {
        public partial class UpdateGroupChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Tenants"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Tenants", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UnitConfigurations"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UnitConfigurations", manager);

                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "MediaConfigurations"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("MediaConfigurations", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UpdateGroupDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UpdateGroupDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UpdateGroup entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UpdateGroupDelta(entity.Id, DeltaOperation.Created);
                if (entity.Tenant != null)
                {
                    delta.ChangeTenant(entity.Tenant.Id);
                }

                if (entity.UnitConfiguration != null)
                {
                    delta.ChangeUnitConfiguration(entity.UnitConfiguration.Id);
                }

                if (entity.MediaConfiguration != null)
                {
                    delta.ChangeMediaConfiguration(entity.MediaConfiguration.Id);
                }

                foreach (var userDefinedProperty in entity.UserDefinedProperties)
                {
                    delta.UserDefinedPropertiesDelta.Add(userDefinedProperty.Key, userDefinedProperty.Value);
                }

                var notification = new UpdateGroupDeltaNotification
                {
                    Delta = new UpdateGroupDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UpdateGroup entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UpdateGroupDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UpdateGroupDeltaNotification
                                        {
                                            Delta = new UpdateGroupDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UpdateGroup original, UpdateGroup entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UpdateGroupDelta(original);

                if (original.Name != entity.Name)
                {
                    delta.ChangeName(entity.Name);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                var originalTenantId = original.Tenant == null ?  (int?)null : original.Tenant.Id;
                var entityTenantId = entity.Tenant == null ?  (int?)null : entity.Tenant.Id;
                if (originalTenantId != entityTenantId)
                {
                    delta.ChangeTenant(entityTenantId);
                }

                var originalUnitConfigurationId = original.UnitConfiguration == null ?  (int?)null : original.UnitConfiguration.Id;
                var entityUnitConfigurationId = entity.UnitConfiguration == null ?  (int?)null : entity.UnitConfiguration.Id;
                if (originalUnitConfigurationId != entityUnitConfigurationId)
                {
                    delta.ChangeUnitConfiguration(entityUnitConfigurationId);
                }

                var originalMediaConfigurationId = original.MediaConfiguration == null ?  (int?)null : original.MediaConfiguration.Id;
                var entityMediaConfigurationId = entity.MediaConfiguration == null ?  (int?)null : entity.MediaConfiguration.Id;
                if (originalMediaConfigurationId != entityMediaConfigurationId)
                {
                    delta.ChangeMediaConfiguration(entityMediaConfigurationId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UpdateGroupDeltaNotification 
                {
                    Delta = new UpdateGroupDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UpdateGroupDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UpdateGroupQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Name != null)
                {
                    model.Name = delta.Name.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Tenant != null)
                {
                    if (!delta.Tenant.ReferenceId.HasValue)
                    {
                        model.Tenant = Tenant.Null;
                    }
                    else
                    {
                        model.Tenant = new Tenant { Id = delta.Tenant.ReferenceId.Value };
                    }
                }

                if (delta.UnitConfiguration != null)
                {
                    if (!delta.UnitConfiguration.ReferenceId.HasValue)
                    {
                        model.UnitConfiguration = UnitConfiguration.Null;
                    }
                    else
                    {
                        model.UnitConfiguration = new UnitConfiguration { Id = delta.UnitConfiguration.ReferenceId.Value };
                    }
                }

                if (delta.MediaConfiguration != null)
                {
                    if (!delta.MediaConfiguration.ReferenceId.HasValue)
                    {
                        model.MediaConfiguration = MediaConfiguration.Null;
                    }
                    else
                    {
                        model.MediaConfiguration = new MediaConfiguration { Id = delta.MediaConfiguration.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UpdateGroupDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                foreach (var userDefinedProperty in delta.UserDefinedProperties)
                {
                    model.UserDefinedProperties[userDefinedProperty.Key] = userDefinedProperty.Value;
                }

                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UpdateGroupDeltaNotification
                {
                    Delta = new UpdateGroupDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UpdatePartChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UpdateGroups"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UpdateGroups", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UpdatePartDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UpdatePartDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UpdatePart entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UpdatePartDelta(entity.Id, DeltaOperation.Created);
                if (entity.UpdateGroup != null)
                {
                    delta.ChangeUpdateGroup(entity.UpdateGroup.Id);
                }

                var notification = new UpdatePartDeltaNotification
                {
                    Delta = new UpdatePartDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UpdatePart entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UpdatePartDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UpdatePartDeltaNotification
                                        {
                                            Delta = new UpdatePartDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UpdatePart original, UpdatePart entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UpdatePartDelta(original);

                if (original.Type != entity.Type)
                {
                    delta.ChangeType(entity.Type);
                }

                if (original.Start != entity.Start)
                {
                    delta.ChangeStart(entity.Start);
                }

                if (original.End != entity.End)
                {
                    delta.ChangeEnd(entity.End);
                }

                if (original.Description != entity.Description)
                {
                    delta.ChangeDescription(entity.Description);
                }

                if (original.Structure != entity.Structure)
                {
                    delta.ChangeStructure(entity.Structure);
                }

                if (original.InstallInstructions != entity.InstallInstructions)
                {
                    delta.ChangeInstallInstructions(entity.InstallInstructions);
                }

                if (original.DynamicContent != entity.DynamicContent)
                {
                    delta.ChangeDynamicContent(entity.DynamicContent);
                }

                var originalUpdateGroupId = original.UpdateGroup == null ?  (int?)null : original.UpdateGroup.Id;
                var entityUpdateGroupId = entity.UpdateGroup == null ?  (int?)null : entity.UpdateGroup.Id;
                if (originalUpdateGroupId != entityUpdateGroupId)
                {
                    delta.ChangeUpdateGroup(entityUpdateGroupId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UpdatePartDeltaNotification 
                {
                    Delta = new UpdatePartDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UpdatePartDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UpdatePartQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Type != null)
                {
                    model.Type = delta.Type.Value;
                }

                if (delta.Start != null)
                {
                    model.Start = delta.Start.Value;
                }

                if (delta.End != null)
                {
                    model.End = delta.End.Value;
                }

                if (delta.Description != null)
                {
                    model.Description = delta.Description.Value;
                }

                if (delta.Structure != null)
                {
                    model.Structure = delta.Structure.Value;
                }

                if (delta.InstallInstructions != null)
                {
                    model.InstallInstructions = delta.InstallInstructions.Value;
                }

                if (delta.DynamicContent != null)
                {
                    model.DynamicContent = delta.DynamicContent.Value;
                }

                if (delta.UpdateGroup != null)
                {
                    if (!delta.UpdateGroup.ReferenceId.HasValue)
                    {
                        model.UpdateGroup = UpdateGroup.Null;
                    }
                    else
                    {
                        model.UpdateGroup = new UpdateGroup { Id = delta.UpdateGroup.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UpdatePartDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UpdatePartDeltaNotification
                {
                    Delta = new UpdatePartDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UpdateCommandChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "Units"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("Units", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UpdateCommandDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UpdateCommandDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UpdateCommand entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UpdateCommandDelta(entity.Id, DeltaOperation.Created);
                if (entity.Unit != null)
                {
                    delta.ChangeUnit(entity.Unit.Id);
                }

                var notification = new UpdateCommandDeltaNotification
                {
                    Delta = new UpdateCommandDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UpdateCommand entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UpdateCommandDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UpdateCommandDeltaNotification
                                        {
                                            Delta = new UpdateCommandDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UpdateCommand original, UpdateCommand entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UpdateCommandDelta(original);

                if (original.UpdateIndex != entity.UpdateIndex)
                {
                    delta.ChangeUpdateIndex(entity.UpdateIndex);
                }

                if (original.WasTransferred != entity.WasTransferred)
                {
                    delta.ChangeWasTransferred(entity.WasTransferred);
                }

                if (original.WasInstalled != entity.WasInstalled)
                {
                    delta.ChangeWasInstalled(entity.WasInstalled);
                }

                if (original.Command != entity.Command)
                {
                    delta.ChangeCommand(entity.Command);
                }

                var originalUnitId = original.Unit == null ?  (int?)null : original.Unit.Id;
                var entityUnitId = entity.Unit == null ?  (int?)null : entity.Unit.Id;
                if (originalUnitId != entityUnitId)
                {
                    delta.ChangeUnit(entityUnitId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UpdateCommandDeltaNotification 
                {
                    Delta = new UpdateCommandDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UpdateCommandDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UpdateCommandQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.UpdateIndex != null)
                {
                    model.UpdateIndex = delta.UpdateIndex.Value;
                }

                if (delta.WasTransferred != null)
                {
                    model.WasTransferred = delta.WasTransferred.Value;
                }

                if (delta.WasInstalled != null)
                {
                    model.WasInstalled = delta.WasInstalled.Value;
                }

                if (delta.Command != null)
                {
                    model.Command = delta.Command.Value;
                }

                if (delta.Unit != null)
                {
                    if (!delta.Unit.ReferenceId.HasValue)
                    {
                        model.Unit = Unit.Null;
                    }
                    else
                    {
                        model.Unit = new Unit { Id = delta.Unit.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UpdateCommandDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UpdateCommandDeltaNotification
                {
                    Delta = new UpdateCommandDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }

        public partial class UpdateFeedbackChangeTrackingDataService
        {
            protected override IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers()
            {
                INotificationManager manager;
                NotificationManagerConfiguration notificationManagerConfiguration;
                notificationManagerConfiguration = new NotificationManagerConfiguration
                                                           {
                                                               ConnectionString =
                                                                   this.BackgroundSystemConfiguration
                                                                   .NotificationsConnectionString,
                                                               Path = "UpdateCommands"
                                                           };
                manager = NotificationManagerFactory.Current.Create(notificationManagerConfiguration);
                yield return new Tuple<string, INotificationManager>("UpdateCommands", manager);
            }

            private async Task OnErrorAsync(
                int id,
                DeltaNotificationType notificationType = DeltaNotificationType.PropertiesChanged)
            {
                var notification = new UpdateFeedbackDeltaNotification
                {
                    EntityId = id,
                    NotificationType = notificationType,
                    WasAccepted = false
                };
                Logger.Trace("Sending error for notification type '{0}'", notificationType);
                await this.PostNotificationAsync(notification);
            }

            protected override async Task OnNotification(Notification notification)
            {
                var deltaNotification = notification as UpdateFeedbackDeltaNotification;
                if (deltaNotification == null)
                {
                    throw new NotSupportedException();
                }

                Logger.Trace("Handling notification '{0}'", notification.Id);
                await this.OnNotificationAsync(deltaNotification);
            }

            private async Task OnEntityAddedAsync(UpdateFeedback entity)
            {
                Logger.Trace("Entity {0} was added", entity.Id);
                var delta = new UpdateFeedbackDelta(entity.Id, DeltaOperation.Created);
                if (entity.UpdateCommand != null)
                {
                    delta.ChangeUpdateCommand(entity.UpdateCommand.Id);
                }

                var notification = new UpdateFeedbackDeltaNotification
                {
                    Delta = new UpdateFeedbackDeltaMessage(delta),
                    EntityId = entity.Id,
                    NotificationType = DeltaNotificationType.EntityAdded,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityDeletedAsync(UpdateFeedback entity)
            {
                Logger.Trace("Entity {0} was deleted", entity.Id);
                var delta = new UpdateFeedbackDelta(entity.Id, DeltaOperation.Deleted);
                var notification = new UpdateFeedbackDeltaNotification
                                        {
                                            Delta = new UpdateFeedbackDeltaMessage(delta),
                                            EntityId = entity.Id,
                                            NotificationType = DeltaNotificationType.EntityRemoved,
                                            WasAccepted = true
                                        };
                await this.PostNotificationAsync(notification);
            }

            private async Task OnEntityUpdatedAsync(UpdateFeedback original, UpdateFeedback entity)
            {
                Logger.Trace("Entity {0} was updated", entity.Id);
                var delta = new UpdateFeedbackDelta(original);

                if (original.Timestamp != entity.Timestamp)
                {
                    delta.ChangeTimestamp(entity.Timestamp);
                }

                if (original.State != entity.State)
                {
                    delta.ChangeState(entity.State);
                }

                if (original.Feedback != entity.Feedback)
                {
                    delta.ChangeFeedback(entity.Feedback);
                }

                var originalUpdateCommandId = original.UpdateCommand == null ?  (int?)null : original.UpdateCommand.Id;
                var entityUpdateCommandId = entity.UpdateCommand == null ?  (int?)null : entity.UpdateCommand.Id;
                if (originalUpdateCommandId != entityUpdateCommandId)
                {
                    delta.ChangeUpdateCommand(entityUpdateCommandId);
                }
                delta.SetLastModifiedOn(entity.LastModifiedOn);

                var changeNotification = new UpdateFeedbackDeltaNotification 
                {
                    Delta = new UpdateFeedbackDeltaMessage(delta),
                    EntityId = entity.Id,
                    WasAccepted = true
                };
                await this.PostNotificationAsync(changeNotification);
            }

            private async Task OnNotificationAsync(UpdateFeedbackDeltaNotification notification)
            {
                var delta = notification.Delta.CreateDelta();
                if (delta.Version == null)
                {
                    throw new InvalidDataException("Version can't be null on delta");
                }

                var model = (await this.dataService.QueryAsync(UpdateFeedbackQuery.Create().WithId(delta.Id)).ConfigureAwait(false)).Single();

                if (delta.Timestamp != null)
                {
                    model.Timestamp = delta.Timestamp.Value;
                }

                if (delta.State != null)
                {
                    model.State = delta.State.Value;
                }

                if (delta.Feedback != null)
                {
                    model.Feedback = delta.Feedback.Value;
                }

                if (delta.UpdateCommand != null)
                {
                    if (!delta.UpdateCommand.ReferenceId.HasValue)
                    {
                        model.UpdateCommand = UpdateCommand.Null;
                    }
                    else
                    {
                        model.UpdateCommand = new UpdateCommand { Id = delta.UpdateCommand.ReferenceId.Value };
                    }
                }

                if (model.Version + 1 != delta.Version.Value)
                {
                    // TODO: not accepted!
                    this.Logger.Debug(
                        "Model version ({0} + 1) != delta version ({1})",
                        model.Version,
                        delta.Version.Value);
                    var reply = new UpdateFeedbackDeltaNotification
                    {
                        Delta = notification.Delta,
                        EntityId = notification.EntityId,
                        ReplyTo = notification.Id.ToString(),
                        WasAccepted = false
                    };
                    await this.PostNotificationAsync(reply);
                    return;
                }

                model.Version = delta.Version.Value;
                var saved = await this.dataService.UpdateAsync(model);
                if (!saved.LastModifiedOn.HasValue)
                {
                    throw new InvalidOperationException();
                }
                
                delta.SetLastModifiedOn(saved.LastModifiedOn);
                var acceptedReply = new UpdateFeedbackDeltaNotification
                {
                    Delta = new UpdateFeedbackDeltaMessage(delta),
                    EntityId = notification.EntityId,
                    ReplyTo = notification.Id.ToString(),
                    WasAccepted = true
                };
                await this.PostNotificationAsync(acceptedReply);
            }
        }
    }
}
