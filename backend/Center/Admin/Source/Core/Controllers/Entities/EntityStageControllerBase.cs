// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityStageControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityStageControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The base class for all generated entity controllers.
    /// </summary>
    public abstract class EntityStageControllerBase : SynchronizableControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityStageControllerBase"/> class.
        /// </summary>
        /// <param name="dataController">
        /// The data controller.
        /// </param>
        protected EntityStageControllerBase(DataControllerBase dataController)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.DataController = dataController;
        }

        /// <summary>
        /// Gets or sets the name of the partition to which this entity belongs.
        /// </summary>
        public string PartitionName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of this entity.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the view model of the stage for this entity.
        /// </summary>
        public EntityStageViewModelBase StageViewModel { get; protected set; }

        /// <summary>
        /// Gets or sets the model for this stage.
        /// </summary>
        public StageModel Model { get; set; }

        /// <summary>
        /// Gets the data controller used for accessing data models from the database.
        /// </summary>
        public DataControllerBase DataController { get; private set; }

        /// <summary>
        /// Initializes this controller with the given data controller.
        /// </summary>
        public virtual void Initialize()
        {
            this.UpdatePermissions();

            var applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
            applicationState.PropertyChanged += this.ApplicationStateOnPropertyChanged;
        }

        /// <summary>
        /// Filters the given column.
        /// </summary>
        /// <param name="columnName">
        /// The name of the column (equal to the name of the property).
        /// </param>
        /// <returns>
        /// The <see cref="Visibility"/>. By default all columns are visible.
        /// The following return values have the given meaning:
        /// <see cref="Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="Visibility.Hidden"/>: the column is completely removed and can't be selected by the user.
        /// </returns>
        public Visibility FilterColumn(string columnName)
        {
            if (this.Model == null)
            {
                return Visibility.Visible;
            }

            if (this.Model.ColumnVisibilities == null)
            {
                this.Model.ColumnVisibilities = new Dictionary<string, Visibility>();
            }

            Visibility visibility;
            if (this.Model.ColumnVisibilities.TryGetValue(columnName, out visibility))
            {
                return visibility;
            }

            return this.GetInitialColumnVisibility(columnName);
        }

        /// <summary>
        /// Sets the visibility of a column.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <param name="visibility">
        /// The visibility to set to the column.
        /// The following values have the given meaning:
        /// <see cref="Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="Visibility.Hidden"/>: the column is completely removed and can't be selected by the user.
        /// </param>
        public void SetColumnVisibility(string columnName, Visibility visibility)
        {
            if (this.Model == null)
            {
                return;
            }

            if (this.Model.ColumnVisibilities == null)
            {
                this.Model.ColumnVisibilities = new Dictionary<string, Visibility>();
            }

            var initial = this.GetInitialColumnVisibility(columnName);
            if (initial != visibility)
            {
                this.Model.ColumnVisibilities[columnName] = visibility;
                return;
            }

            // it's set back to the default, let's remove it
            this.Model.ColumnVisibilities.Remove(columnName);
        }

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public abstract bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel);

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public abstract bool SupportsEntity(DataViewModelBase dataViewModel);

        /// <summary>
        /// Asynchronously navigates to the entity with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The string id of the entity to navigate to.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public async Task NavigateToAsync(string id)
        {
            this.LoadData();
            await this.DataController.AwaitAllDataAsync();

            this.StageViewModel.SelectedInstance =
                this.StageViewModel.Instances.OfType<ReadOnlyDataViewModelBase>()
                    .FirstOrDefault(e => e.GetIdString().Equals(id));
        }

        /// <summary>
        /// Loads the entity instances from the given connection controller.
        /// </summary>
        public abstract void LoadData();

        /// <summary>
        /// Updates the property display parameters for the given property.
        /// Subclasses can use this to filter or alter properties shown in the property grid.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to be modified.
        /// </param>
        public virtual void UpdatePropertyDisplay(PropertyDisplayParameters parameters)
        {
        }

        /// <summary>
        /// Checks if the given data view model can be copied.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to copy.
        /// </param>
        /// <returns>
        /// True if it can be copied, false otherwise.
        /// </returns>
        public virtual bool CanCopyEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return this.SupportsEntity(dataViewModel);
        }

        /// <summary>
        /// Checks if the given data view model can be edited.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to edit.
        /// </param>
        /// <returns>
        /// True if it can be edited, false otherwise.
        /// </returns>
        public virtual bool CanEditEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return this.SupportsEntity(dataViewModel);
        }

        /// <summary>
        /// Checks if the given data view model can be deleted.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to delete.
        /// </param>
        /// <returns>
        /// True if it can be deleted, false otherwise.
        /// </returns>
        public virtual bool CanDeleteEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return this.SupportsEntity(dataViewModel);
        }

        /// <summary>
        /// Gets the initial column visibility if the user hasn't chosen the visibility yet.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="Visibility"/>.
        /// The following return values have the given meaning:
        /// <see cref="Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="Visibility.Hidden"/>: the column is completely removed and can't be selected by the user.
        /// </returns>
        protected virtual Visibility GetInitialColumnVisibility(string columnName)
        {
            switch (columnName)
            {
                case "CreatedOn":
                case "LastModifiedOn":
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        /// <summary>
        /// Updates the permissions of the <see cref="StageViewModel"/> with the permission controller.
        /// Subclasses should check the data scope to set the stage's:
        /// - <see cref="EntityStageViewModelBase.CanCreate"/>
        /// - <see cref="EntityStageViewModelBase.CanRead"/>
        /// - <see cref="EntityStageViewModelBase.CanWrite"/>
        /// - <see cref="EntityStageViewModelBase.CanDelete"/>
        /// </summary>
        protected virtual void UpdatePermissions()
        {
            this.StageViewModel.CanCreate = true;
            this.StageViewModel.CanRead = true;
            this.StageViewModel.CanWrite = true;
            this.StageViewModel.CanDelete = true;
        }

        /// <summary>
        /// Updates the permissions of the <see cref="StageViewModel"/> by checking the
        /// permissions against the given <paramref name="dataScope"/> through the <see cref="PermissionController"/>.
        /// </summary>
        /// <param name="dataScope">
        /// The data scope.
        /// </param>
        protected void UpdatePermissions(DataScope dataScope)
        {
            var applicationController = ServiceLocator.Current.GetInstance<IAdminApplicationController>();
            this.StageViewModel.CanCreate = applicationController.PermissionController.HasPermission(
                Permission.Create,
                dataScope);
            this.StageViewModel.CanRead = applicationController.PermissionController.HasPermission(
                Permission.Read,
                dataScope);
            this.StageViewModel.CanWrite = applicationController.PermissionController.HasPermission(
                Permission.Write,
                dataScope);
            this.StageViewModel.CanDelete = applicationController.PermissionController.HasPermission(
                Permission.Delete,
                dataScope);
        }

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var applicationState = sender as IAdminApplicationState;
            if (e.PropertyName != "CurrentTenant" || applicationState == null)
            {
                return;
            }

            if (applicationState.CurrentTenant == null)
            {
                return;
            }

            this.UpdatePermissions();
        }
    }
}
