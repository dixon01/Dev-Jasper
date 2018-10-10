// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigurationDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Configurations
{
    using Gorba.Center.Admin.Core.DataViewModels.Documents;

    /// <summary>
    /// The extension of the data view model for unit configuration.
    /// </summary>
    public partial class UnitConfigurationDataViewModel
    {
        private string name;

        private string description;

        /// <summary>
        /// Gets a value indicating whether is the product type is read-only.
        /// </summary>
        public bool IsReadOnlyProductType
        {
            get
            {
                return this.Id != 0;
            }
        }

        /// <summary>
        /// Gets or sets the name of the configuration (this is actually mapped by the controller to the
        /// <see cref="DocumentReadOnlyDataViewModel.Name"/> of the referenced <see cref="Document"/>).
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the name of the configuration (this is actually mapped by the controller to the
        /// <see cref="DocumentReadOnlyDataViewModel.Description"/> of the referenced <see cref="Document"/>).
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }
    }
}
