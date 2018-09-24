// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRoleDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserRoleDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.AccessControl
{
    using Gorba.Center.Common.ServiceModel.AccessControl;

    /// <summary>
    /// Extension of the <see cref="UserRoleDataViewModel"/> to allow to select
    /// authorizations from within the user role editor.
    /// </summary>
    public partial class UserRoleDataViewModel
    {
        /// <summary>
        /// Gets or sets the matrix of authorizations (<see cref="Permission"/>s and <see cref="DataScope"/>s).
        /// </summary>
        public AuthorizationMatrixDataViewModel Authorizations { get; set; }
    }
}
