// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMediaShell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Defines the properties of the Media shell.
    /// </summary>
    public interface IMediaShell : IShellViewModel, IWindowViewModel, IBusy
    {
        /// <summary>
        /// Gets the state of the Media application.
        /// </summary>
        /// <value>
        /// The state of the Media application.
        /// </value>
        IMediaApplicationState MediaApplicationState { get; }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        ObservableCollection<MenuItemBase> MenuItems { get; }

        /// <summary>
        /// Gets or sets the editor
        /// </summary>
        IEditorViewModel Editor { get; set; }

        /// <summary>
        /// Gets the editors.
        /// </summary>
        Dictionary<PhysicalScreenType, IEditorViewModel> Editors { get; }

        /// <summary>
        /// Gets or sets the zoom
        /// </summary>
        double Zoom { get; set; }

        /// <summary>
        /// Gets or sets the layout offset
        /// </summary>
        Point LayoutPosition { get; set; }

        /// <summary>
        /// Gets or sets the CycleNavigator ViewModel.
        /// </summary>
        CycleNavigationViewModel CycleNavigator { get; set; }

        /// <summary>
        /// Gets or sets the resolution navigation.
        /// </summary>
        ResolutionNavigationPrompt ResolutionNavigation { get; set; }

        /// <summary>
        /// Gets the dictionary
        /// </summary>
        DictionaryDataViewModel Dictionary { get; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        ICommandRegistry CommandRegistry { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the the Simulation is visible
        /// </summary>
        bool SimulationIsVisible { get; set; }

        /// <summary>
        /// Gets the permission controller.
        /// </summary>
        IPermissionController PermissionController { get; }

        /// <summary>
        /// Reloads the shell on tenant switch.
        /// </summary>
        void ReloadOnTenantSwitch();

        /// <summary>
        /// Sets the project name in front of the window title.
        /// </summary>
        /// <param name="projectName">
        /// The project name.
        /// </param>
        void SetProjectTitle(string projectName);

        /// <summary>
        /// The clear project title.
        /// </summary>
        void ClearProjectTitle();
    }
}