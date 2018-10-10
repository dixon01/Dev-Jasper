// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserStageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Controllers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UserStageController"/>.
    /// </summary>
    public partial class UserStageController
    {
        private static readonly Tuple<string, string>[] DisplayProperties =
            {
                new Tuple<string, string>("FirstName", "First Name"),
                new Tuple<string, string>("LastName", "Last Name"),
                new Tuple<string, string>("Email", "E-Mail"),
                new Tuple<string, string>("OwnerTenant", "Owner Tenant"),
                new Tuple<string, string>("Username", "Username"),
                new Tuple<string, string>("HashedPassword", "Password"),
                new Tuple<string, string>("IsEnabled", "Is Enabled"),
                new Tuple<string, string>("Description", "Description"),
                new Tuple<string, string>("Domain", "Domain"),
                new Tuple<string, string>("CultureSelection", "Culture"),
                new Tuple<string, string>("TimeZoneSelection", "Time Zone"),
                new Tuple<string, string>("Id", "Id"),
                new Tuple<string, string>("CreatedOn", "Created On"),
                new Tuple<string, string>("LastModifiedOn", "Last Modified On")
            };

        private IPermissionController PermissionController
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;
            }
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
        public override bool CanCopyEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            // exactly the same as "edit"
            return this.CanEditEntity(dataViewModel);
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
        public override bool CanEditEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            var user = dataViewModel as UserReadOnlyDataViewModel;
            if (user == null)
            {
                return false;
            }

            return this.PermissionController.HasPermission(
                user.OwnerTenant.ReadableModel,
                Permission.Write,
                DataScope.User);
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
        public override bool CanDeleteEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            var user = dataViewModel as UserReadOnlyDataViewModel;
            if (user == null)
            {
                return false;
            }

            if (user.Username == CommonNames.AdminUsername)
            {
                return false;
            }

            if (!this.PermissionController.HasPermission(
                    user.OwnerTenant.ReadableModel,
                    Permission.Delete,
                    DataScope.User))
            {
                return false;
            }

            var state = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
            return state.CurrentUser == null || user.Id != state.CurrentUser.Id;
        }

        /// <summary>
        /// Updates the property display parameters for the given property.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to be modified.
        /// </param>
        public override void UpdatePropertyDisplay(PropertyDisplayParameters parameters)
        {
            var property = DisplayProperties.FirstOrDefault(p => parameters.PropertyName == p.Item1);
            if (property == null)
            {
                parameters.IsVisible = false;
                return;
            }

            if (parameters.PropertyName == "IsEnabled")
            {
                var user = parameters.EditingEntity as UserDataViewModel;
                if (user != null && user.Username == CommonNames.AdminUsername)
                {
                    // hide the IsEnabled property for user "admin"
                    parameters.IsVisible = false;
                    return;
                }
            }

            parameters.DisplayName = property.Item2;
            parameters.OrderIndex = Array.IndexOf(DisplayProperties, property);
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
        protected override Visibility GetInitialColumnVisibility(string columnName)
        {
            if (columnName == "HashedPassword")
            {
                // we never want to show the password
                return Visibility.Hidden;
            }

            if (columnName.Contains("Login"))
            {
                // hide login columns from the user
                return Visibility.Collapsed;
            }

            return base.GetInitialColumnVisibility(columnName);
        }

        /// <summary>
        /// Updates the permissions of the <see cref="EntityStageControllerBase.StageViewModel"/> with
        /// the permission controller.
        /// Subclasses should check the data scope to set the stage's:
        /// - <see cref="EntityStageViewModelBase.CanCreate"/>
        /// - <see cref="EntityStageViewModelBase.CanRead"/>
        /// - <see cref="EntityStageViewModelBase.CanWrite"/>
        /// - <see cref="EntityStageViewModelBase.CanDelete"/>
        /// </summary>
        protected override void UpdatePermissions()
        {
            this.UpdatePermissions(DataScope.User);
        }
    }
}
