// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationFlagDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AuthorizationFlagDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.AccessControl
{
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// A single selection of an authorization (<see cref="Permission"/> and <see cref="DataScope"/>).
    /// </summary>
    public class AuthorizationFlagDataViewModel : ViewModelBase
    {
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationFlagDataViewModel"/> class.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <param name="dataScope">
        /// The data scope.
        /// </param>
        public AuthorizationFlagDataViewModel(Permission permission, DataScope dataScope)
        {
            this.DataScope = dataScope;
            this.Permission = permission;
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// Gets the data scope.
        /// </summary>
        public DataScope DataScope { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this authorization is checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                this.SetProperty(ref this.isChecked, value, () => this.IsChecked);
            }
        }
    }
}