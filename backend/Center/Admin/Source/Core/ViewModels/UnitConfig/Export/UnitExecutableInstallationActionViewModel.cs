// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitExecutableInstallationActionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitExecutableInstallationActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    /// <summary>
    /// The installation action view model base.
    /// </summary>
    public class UnitExecutableInstallationActionViewModel : InstallationActionViewModelBase
    {
        private string displayName;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.ExecutablePathBase = value;
                this.displayName = value;
                this.RaisePropertyChanged(() => this.DisplayName);
            }
        }
    }
}