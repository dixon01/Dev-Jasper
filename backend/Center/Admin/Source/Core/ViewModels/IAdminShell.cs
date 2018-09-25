// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdminShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdminShell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels.Editor;
    using Gorba.Center.Admin.Core.ViewModels.Navigator;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The icenter.admin shell interface.
    /// </summary>
    public interface IAdminShell : IShellViewModel, IWindowViewModel
    {
        /// <summary>
        /// Gets the state of the Admin application.
        /// </summary>
        /// <value>
        /// The state of the Admin application.
        /// </value>
        IAdminApplicationState AdminApplicationState { get; }

        /// <summary>
        /// Gets the navigator view model.
        /// </summary>
        NavigatorViewModel Navigator { get; }

        /// <summary>
        /// Gets the home stage view model.
        /// </summary>
        HomeStageViewModel HomeStage { get; }

        /// <summary>
        /// Gets the list of all entity stage view models.
        /// </summary>
        ObservableCollection<EntityStageViewModelBase> EntityStages { get; }

        /// <summary>
        /// Gets the list of all stages for removable media (USB sticks).
        /// </summary>
        ObservableCollection<RemovableMediaStageViewModel> RemovableMediaStages { get; }

        /// <summary>
        /// Gets or sets the currently displayed stage view model.
        /// </summary>
        StageViewModelBase CurrentStage { get; set; }

        /// <summary>
        /// Gets the entity editor.
        /// </summary>
        EntityEditorViewModel Editor { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the complete window is busy. This is used to disable the window
        /// while a modal dialog is open.
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        string BusyMessage { get; set; }
    }
}
