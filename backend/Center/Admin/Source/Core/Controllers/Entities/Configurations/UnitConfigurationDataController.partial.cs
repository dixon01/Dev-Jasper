// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigurationDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UnitConfigurationDataController"/>.
    /// </summary>
    public partial class UnitConfigurationDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UnitConfigurationQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query = UnitConfigurationQuery.Create().IncludeDocument(DocumentFilter.Create().WithTenant(currentTenant));
        }

        partial void PostStartEditEntity(UnitConfigurationDataViewModel dataViewModel)
        {
            if (dataViewModel.Document.SelectedEntity == null)
            {
                return;
            }

            dataViewModel.Name = dataViewModel.Document.SelectedEntity.Name;
            dataViewModel.Description = dataViewModel.Document.SelectedEntity.Description;
        }

        partial void PostCopyEntity(UnitConfigurationDataViewModel dataViewModel)
        {
            this.PostStartEditEntity(dataViewModel);
        }

        partial void Filter(ref Func<UnitConfigurationReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void FilterResults(
            ref Func<IEnumerable<UnitConfigurationReadableModel>, Task<IEnumerable<UnitConfigurationReadableModel>>>
                asyncMethod)
        {
            asyncMethod = this.FilterResultsAsync;
        }

        partial void PreSaveEntity(ref Func<UnitConfigurationDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreSaveEntityAsync;
        }

        partial void PreDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreDeleteEntityAsync;
        }

        partial void PostDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostDeleteEntityAsync;
        }

        private async Task<bool> FilterAsync(UnitConfigurationReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.Document.LoadReferencePropertiesAsync();
            if (!readableModel.Document.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id))
            {
                return false;
            }

            // TODO: WES: why do we need to load these?
            // loading versions as well here
            await readableModel.Document.LoadNavigationPropertiesAsync();
            return true;
        }

        private async Task<IEnumerable<UnitConfigurationReadableModel>> FilterResultsAsync(
            IEnumerable<UnitConfigurationReadableModel> results)
        {
            var models = new List<UnitConfigurationReadableModel>(results);
            foreach (var model in models)
            {
                await model.LoadReferencePropertiesAsync();

                // TODO: WES: why do we need to load these? If not needed, remove this method
                // loading versions as well here
                await model.Document.LoadNavigationPropertiesAsync();
            }

            return results;
        }

        private async Task PreSaveEntityAsync(UnitConfigurationDataViewModel dvm)
        {
            DocumentReadableModel document;
            if (dvm.Document.SelectedEntity == null)
            {
                document = await this.CreateDefaultDocumentAsync(dvm);
            }
            else if (dvm.Document.SelectedEntity.Name != dvm.Name
                     || dvm.Document.SelectedEntity.Description != dvm.Description)
            {
                var edit = dvm.Document.SelectedEntity.ReadableModel.ToChangeTrackingModel();
                edit.Name = dvm.Name;
                edit.Description = dvm.Description;
                document = await this.ConnectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(edit);
            }
            else
            {
                return;
            }

            dvm.Document.SelectedEntity = this.Factory.CreateReadOnly(document);
        }

        private async Task PreDeleteEntityAsync(UnitConfigurationReadOnlyDataViewModel dataViewModel)
        {
            var model = dataViewModel.ReadableModel;

            var updateGroupManager = this.DataController.ConnectionController.UpdateGroupChangeTrackingManager;
            var updateGroups =
                await updateGroupManager.QueryAsync(UpdateGroupQuery.Create().WithUnitConfiguration(model.ToDto()));

            // remove this unit configuration from all update groups that still reference it
            foreach (var updateGroup in updateGroups.ToList())
            {
                var editableGroup = updateGroup.ToChangeTrackingModel();
                editableGroup.UnitConfiguration = null;
                await updateGroupManager.CommitAndVerifyAsync(editableGroup);
            }

            await model.LoadReferencePropertiesAsync();
            await model.Document.LoadNavigationPropertiesAsync();

            foreach (var version in model.Document.Versions.ToList())
            {
                await
                    this.DataController.ConnectionController.DocumentVersionChangeTrackingManager.DeleteAsync(version);
            }
        }

        private async Task PostDeleteEntityAsync(UnitConfigurationReadOnlyDataViewModel dataViewModel)
        {
            // model.Document was loaded above in PreDeleteEntityAsync()
            var model = dataViewModel.ReadableModel;
            await this.DataController.ConnectionController.DocumentChangeTrackingManager.DeleteAsync(model.Document);
        }

        private async Task<DocumentReadableModel> CreateDefaultDocumentAsync(
            UnitConfigurationDataViewModel unitConfig)
        {
            var document = this.ConnectionController.DocumentChangeTrackingManager.Create();
            document.Tenant = this.ApplicationState.CurrentTenant;
            document.Name = unitConfig.Name;
            document.Description = unitConfig.Description;
            var doc = await this.ConnectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(document);

            var version = this.ConnectionController.DocumentVersionChangeTrackingManager.Create();
            version.Document = doc;
            version.CreatingUser =
                this.ConnectionController.UserChangeTrackingManager.Wrap(this.ApplicationState.CurrentUser);
            version.Major = 0;
            version.Minor = 0;
            version.Description = unitConfig.Description;
            version.Content = new XmlData(new UnitConfigData());
            await this.ConnectionController.DocumentVersionChangeTrackingManager.CommitAndVerifyAsync(version);

            return doc;
        }
    }
}