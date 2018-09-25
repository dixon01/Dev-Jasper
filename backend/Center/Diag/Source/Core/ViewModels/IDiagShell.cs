// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IDiagShell.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Diag.Core.Models;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// Defines the properties of the icenter.diag shell.
    /// </summary>
    public interface IDiagShell : IShellViewModel, IWindowViewModel
    {
        /// <summary>
        /// Gets the state of the Diag application.
        /// </summary>
        /// <value>
        /// The state of the Diag application.
        /// </value>
        IDiagApplicationState DiagApplicationState { get; }

        /// <summary>
        /// Gets or sets a value indicating whether icenter.diag is
        /// automatically refreshing the list of units using UDCP.
        /// </summary>
        bool IsAutoRefresh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show tile view.
        /// </summary>
        bool ShowTileView { get; set; }

        /// <summary>
        /// Gets the list of all units.
        /// </summary>
        ObservableItemCollection<UnitViewModelBase> AllUnits { get; }

        /// <summary>
        /// Gets the connected units.
        /// </summary>
        FilteredObservableCollection<UnitViewModelBase> ConnectedUnits { get; }

        /// <summary>
        /// Gets the All Units Tab.
        /// </summary>
        AllUnitsTab AllUnitsTab { get; }

        /// <summary>
        /// Gets the selected unit.
        /// </summary>
        UnitViewModelBase SelectedUnit { get; }

        /// <summary>
        /// Gets or sets the selected unit tab.
        /// </summary>
        UnitTabBase SelectedUnitTab { get; set; }

        /// <summary>
        /// Gets the list of all tabs visible in the main window.
        /// </summary>
        ObservableCollection<UnitTabBase> Tabs { get; }

        /// <summary>
        /// Gets the Toggle unit connection command
        /// </summary>
        ICommand ToggleUnitConnectionCommand { get; }

        /// <summary>
        /// Gets the Toggle unit favorite state command
        /// </summary>
        ICommand ToggleUnitFavoriteCommand { get; }

        /// <summary>
        /// Gets the announce unit command
        /// </summary>
        ICommand AnnounceUnitCommand { get; }

        /// <summary>
        /// Gets the request add unit command
        /// </summary>
        ICommand RequestAddUnitCommand { get; }

        /// <summary>
        /// Gets the reboot unit command
        /// </summary>
        ICommand RebootUnitCommand { get; }

        /// <summary>
        /// Gets the connect unit command
        /// </summary>
        ICommand ConnectUnitCommand { get; }

        /// <summary>
        /// Gets the disconnect unit command
        /// </summary>
        ICommand DisconnectUnitCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is busy
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets the busy message
        /// </summary>
        string BusyMessage { get; set; }

        /// <summary>
        /// Gets a value indicating whether show grid view.
        /// </summary>
        bool ShowGridView { get; }

        /// <summary>
        /// Creates a tab for the given unit
        /// </summary>
        /// <param name="unit">the unit</param>
        void CreateUnitTab(UnitViewModelBase unit);

        /// <summary>
        /// Removes the tab for the given unit
        /// </summary>
        /// <param name="unit">the unit</param>
        void RemoveUnitTab(UnitViewModelBase unit);
    }
}