// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationReadOnlyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigurationReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Configurations
{
    using System.Windows.Input;

    /// <summary>
    /// The extension of the read-only data view model for unit configuration.
    /// </summary>
    public partial class UnitConfigurationReadOnlyDataViewModel
    {
        /// <summary>
        /// Gets the edit unit configuration command.
        /// </summary>
        public ICommand EditUnitConfigurationCommand
        {
            get
            {
                return this.Factory.CommandRegistry.GetCommand(CommandCompositionKeys.Entities.UnitConfiguration.Edit);
            }
        }
    }
}