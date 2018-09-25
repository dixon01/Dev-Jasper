namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;

    namespace AccessControl
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        public partial class AuthorizationReadableModel
        {
        }

        public partial class UserRoleReadableModel
        {
            public virtual async Task ApplyAsync(AuthorizationDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.UserRole == null)
                        {
                            this.RemoveAuthorizations(delta.Id);
                            return;
                        }

                        if (delta.UserRole.ReferenceId == this.Id)
                        {
                            await this.AddAuthorizationsAsync(delta.Id);
                            return;
                        }

                        this.RemoveAuthorizations(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddAuthorizationsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveAuthorizations(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddAuthorizationsAsync(int id)
            {
                if (this.Authorizations.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IAuthorizationChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.authorizations.Add(model);
            }

            private void RemoveAuthorizations(int id)
            {
                if (this.Authorizations.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Authorizations.Single(u => u.Id == id);
                this.authorizations.Remove(item);
            }
        }
    }

    namespace Membership
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        public partial class AssociationTenantUserUserRoleReadableModel
        {
        }

        public partial class TenantReadableModel
        {
            public virtual async Task ApplyAsync(UserDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.OwnerTenant == null)
                        {
                            this.RemoveUsers(delta.Id);
                            return;
                        }

                        if (delta.OwnerTenant.ReferenceId == this.Id)
                        {
                            await this.AddUsersAsync(delta.Id);
                            return;
                        }

                        this.RemoveUsers(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUsersAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUsers(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUsersAsync(int id)
            {
                if (this.Users.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.users.Add(model);
            }

            private void RemoveUsers(int id)
            {
                if (this.Users.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Users.Single(u => u.Id == id);
                this.users.Remove(item);
            }

            public virtual async Task ApplyAsync(Update.UpdateGroupDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.Tenant == null)
                        {
                            this.RemoveUpdateGroups(delta.Id);
                            return;
                        }

                        if (delta.Tenant.ReferenceId == this.Id)
                        {
                            await this.AddUpdateGroupsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUpdateGroupsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUpdateGroupsAsync(int id)
            {
                if (this.UpdateGroups.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<Update.IUpdateGroupChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.updateGroups.Add(model);
            }

            private void RemoveUpdateGroups(int id)
            {
                if (this.UpdateGroups.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.UpdateGroups.Single(u => u.Id == id);
                this.updateGroups.Remove(item);
            }
        }

        public partial class UserReadableModel
        {
            public virtual async Task ApplyAsync(AssociationTenantUserUserRoleDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.User == null)
                        {
                            this.RemoveAssociationTenantUserUserRoles(delta.Id);
                            return;
                        }

                        if (delta.User.ReferenceId == this.Id)
                        {
                            await this.AddAssociationTenantUserUserRolesAsync(delta.Id);
                            return;
                        }

                        this.RemoveAssociationTenantUserUserRoles(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddAssociationTenantUserUserRolesAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveAssociationTenantUserUserRoles(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddAssociationTenantUserUserRolesAsync(int id)
            {
                if (this.AssociationTenantUserUserRoles.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.associationTenantUserUserRoles.Add(model);
            }

            private void RemoveAssociationTenantUserUserRoles(int id)
            {
                if (this.AssociationTenantUserUserRoles.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.AssociationTenantUserUserRoles.Single(u => u.Id == id);
                this.associationTenantUserUserRoles.Remove(item);
            }
        }
    }

    namespace Units
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        public partial class ProductTypeReadableModel
        {
            public virtual async Task ApplyAsync(UnitDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.ProductType == null)
                        {
                            this.RemoveUnits(delta.Id);
                            return;
                        }

                        if (delta.ProductType.ReferenceId == this.Id)
                        {
                            await this.AddUnitsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUnits(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUnitsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUnits(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUnitsAsync(int id)
            {
                if (this.Units.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.units.Add(model);
            }

            private void RemoveUnits(int id)
            {
                if (this.Units.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Units.Single(u => u.Id == id);
                this.units.Remove(item);
            }
        }

        public partial class UnitReadableModel
        {
            public virtual async Task ApplyAsync(Update.UpdateCommandDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.Unit == null)
                        {
                            this.RemoveUpdateCommands(delta.Id);
                            return;
                        }

                        if (delta.Unit.ReferenceId == this.Id)
                        {
                            await this.AddUpdateCommandsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUpdateCommands(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUpdateCommandsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUpdateCommands(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUpdateCommandsAsync(int id)
            {
                if (this.UpdateCommands.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<Update.IUpdateCommandChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.updateCommands.Add(model);
            }

            private void RemoveUpdateCommands(int id)
            {
                if (this.UpdateCommands.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.UpdateCommands.Single(u => u.Id == id);
                this.updateCommands.Remove(item);
            }
        }
    }

    namespace Resources
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        public partial class ContentResourceReadableModel
        {
        }

        public partial class ResourceReadableModel
        {
        }
    }

    namespace Update
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        public partial class UpdateCommandReadableModel
        {
            public virtual async Task ApplyAsync(UpdateFeedbackDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.UpdateCommand == null)
                        {
                            this.RemoveFeedbacks(delta.Id);
                            return;
                        }

                        if (delta.UpdateCommand.ReferenceId == this.Id)
                        {
                            await this.AddFeedbacksAsync(delta.Id);
                            return;
                        }

                        this.RemoveFeedbacks(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddFeedbacksAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveFeedbacks(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddFeedbacksAsync(int id)
            {
                if (this.Feedbacks.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUpdateFeedbackChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.feedbacks.Add(model);
            }

            private void RemoveFeedbacks(int id)
            {
                if (this.Feedbacks.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Feedbacks.Single(u => u.Id == id);
                this.feedbacks.Remove(item);
            }
        }

        public partial class UpdateFeedbackReadableModel
        {
        }

        public partial class UpdateGroupReadableModel
        {
            public virtual async Task ApplyAsync(Units.UnitDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.UpdateGroup == null)
                        {
                            this.RemoveUnits(delta.Id);
                            return;
                        }

                        if (delta.UpdateGroup.ReferenceId == this.Id)
                        {
                            await this.AddUnitsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUnits(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUnitsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUnits(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUnitsAsync(int id)
            {
                if (this.Units.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<Units.IUnitChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.units.Add(model);
            }

            private void RemoveUnits(int id)
            {
                if (this.Units.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Units.Single(u => u.Id == id);
                this.units.Remove(item);
            }

            public virtual async Task ApplyAsync(UpdatePartDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.UpdateGroup == null)
                        {
                            this.RemoveUpdateParts(delta.Id);
                            return;
                        }

                        if (delta.UpdateGroup.ReferenceId == this.Id)
                        {
                            await this.AddUpdatePartsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUpdateParts(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUpdatePartsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUpdateParts(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUpdatePartsAsync(int id)
            {
                if (this.UpdateParts.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IUpdatePartChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.updateParts.Add(model);
            }

            private void RemoveUpdateParts(int id)
            {
                if (this.UpdateParts.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.UpdateParts.Single(u => u.Id == id);
                this.updateParts.Remove(item);
            }
        }

        public partial class UpdatePartReadableModel
        {        }
    }

    namespace Documents
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        public partial class DocumentReadableModel
        {
            public virtual async Task ApplyAsync(DocumentVersionDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.Document == null)
                        {
                            this.RemoveVersions(delta.Id);
                            return;
                        }

                        if (delta.Document.ReferenceId == this.Id)
                        {
                            await this.AddVersionsAsync(delta.Id);
                            return;
                        }

                        this.RemoveVersions(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddVersionsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveVersions(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddVersionsAsync(int id)
            {
                if (this.Versions.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IDocumentVersionChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.versions.Add(model);
            }

            private void RemoveVersions(int id)
            {
                if (this.Versions.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Versions.Single(u => u.Id == id);
                this.versions.Remove(item);
            }
        }

        public partial class DocumentVersionReadableModel
        {
        }
    }

    namespace Software
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        public partial class PackageReadableModel
        {
            public virtual async Task ApplyAsync(PackageVersionDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.Package == null)
                        {
                            this.RemoveVersions(delta.Id);
                            return;
                        }

                        if (delta.Package.ReferenceId == this.Id)
                        {
                            await this.AddVersionsAsync(delta.Id);
                            return;
                        }

                        this.RemoveVersions(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddVersionsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveVersions(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddVersionsAsync(int id)
            {
                if (this.Versions.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<IPackageVersionChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.versions.Add(model);
            }

            private void RemoveVersions(int id)
            {
                if (this.Versions.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.Versions.Single(u => u.Id == id);
                this.versions.Remove(item);
            }
        }

        public partial class PackageVersionReadableModel
        {
        }
    }

    namespace Configurations
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        public partial class MediaConfigurationReadableModel
        {
            public virtual async Task ApplyAsync(Update.UpdateGroupDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.MediaConfiguration == null)
                        {
                            this.RemoveUpdateGroups(delta.Id);
                            return;
                        }

                        if (delta.MediaConfiguration.ReferenceId == this.Id)
                        {
                            await this.AddUpdateGroupsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUpdateGroupsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUpdateGroupsAsync(int id)
            {
                if (this.UpdateGroups.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<Update.IUpdateGroupChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.updateGroups.Add(model);
            }

            private void RemoveUpdateGroups(int id)
            {
                if (this.UpdateGroups.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.UpdateGroups.Single(u => u.Id == id);
                this.updateGroups.Remove(item);
            }
        }

        public partial class UnitConfigurationReadableModel
        {
            public virtual async Task ApplyAsync(Update.UpdateGroupDelta delta)
            {
                switch (delta.DeltaOperation)
                {
                    case DeltaOperation.Updated:
                        if (delta.UnitConfiguration == null)
                        {
                            this.RemoveUpdateGroups(delta.Id);
                            return;
                        }

                        if (delta.UnitConfiguration.ReferenceId == this.Id)
                        {
                            await this.AddUpdateGroupsAsync(delta.Id);
                            return;
                        }

                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    case DeltaOperation.Created:
                        await this.AddUpdateGroupsAsync(delta.Id);
                        break;
                    case DeltaOperation.Deleted:
                        this.RemoveUpdateGroups(delta.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private async Task AddUpdateGroupsAsync(int id)
            {
                if (this.UpdateGroups.Any(m => m.Id == id))
                {
                    return;
                }

                var manager = DependencyResolver.Current.Get<Update.IUpdateGroupChangeTrackingManager>();
                var model = await manager.GetAsync(id);
                this.updateGroups.Add(model);
            }

            private void RemoveUpdateGroups(int id)
            {
                if (this.UpdateGroups.All(model => model.Id != id))
                {
                    return;
                }

                var item = this.UpdateGroups.Single(u => u.Id == id);
                this.updateGroups.Remove(item);
            }
        }
    }

    namespace Log
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;
    }

    namespace Meta
    {
        using System.Threading.Tasks;

        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        public partial class SystemConfigReadableModel
        {
        }

        public partial class UserDefinedPropertyReadableModel
        {
        }
    }
}
