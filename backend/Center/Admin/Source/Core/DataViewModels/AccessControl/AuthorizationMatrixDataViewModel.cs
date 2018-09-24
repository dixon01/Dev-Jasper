// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationMatrixDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AuthorizationMatrixDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.AccessControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.Views.Editors;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    using Telerik.Windows.Controls.Data.PropertyGrid;

    /// <summary>
    /// Data view model for the matrix of authorizations (<see cref="Permission"/>s and <see cref="DataScope"/>s).
    /// </summary>
    [Editor(typeof(AuthorizationMatrixEditor), EditorStyle.DropDown)]
    public class AuthorizationMatrixDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationMatrixDataViewModel"/> class.
        /// </summary>
        public AuthorizationMatrixDataViewModel()
        {
            this.Permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>().ToArray();

            this.Authorizations =
                Enum.GetValues(typeof(DataScope))
                    .OfType<DataScope>()
                    .Select(
                        d => this.Permissions.Select(p => new AuthorizationFlagDataViewModel(p, d)).ToList())
                    .ToList();
        }

        /// <summary>
        /// Gets the permissions (used as column headers).
        /// </summary>
        public Permission[] Permissions { get; private set; }

        /// <summary>
        /// Gets the authorizations to be displayed in a grid.
        /// </summary>
        public List<List<AuthorizationFlagDataViewModel>> Authorizations { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return AdminStrings.UserRole_SelectAuthorizations;
        }
    }
}