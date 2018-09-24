// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMasterDetailsStage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMasterDetailsStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails
{
    using System.Windows.Input;

    /// <summary>
    /// Defines a stage with a master/details organization.
    /// </summary>
    public interface IMasterDetailsStage
    {
        /// <summary>
        /// Gets the delete command.
        /// </summary>
        ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets the edit command.
        /// </summary>
        ICommand EditCommand { get; }

        /// <summary>
        /// Gets the new command.
        /// </summary>
        ICommand NewCommand { get; }

        /// <summary>
        /// Gets the reload command.
        /// </summary>
        ICommand ReloadCommand { get; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        object SelectedItem { get; set; }
    }
}