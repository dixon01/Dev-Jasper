// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DiagShell.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Diag.Core.Interaction;
    using Gorba.Center.Diag.Core.Models;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    using NLog;

    /// <summary>
    /// The DiagShell.
    /// </summary>
    [Export(typeof(IDiagShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DiagShell : ClientShellBase, IDiagShell, IWeakEventListener
    {
        private const string WindowTitle = "icenter.diag";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly Lazy<ICollectionView> statusNotifications;

        private bool isAutoRefresh;

        private bool showTileView = true;

        private UnitTabBase selectedUnitTab;

        private bool isBusy;

        private string busyMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagShell"/> class.
        /// </summary>
        /// <param name="factory">The shell factory.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public DiagShell(DiagShellFactory factory, ICommandRegistry commandRegistry)
            : base(factory, null, null, null, commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Title = WindowTitle;

            this.Notifications = new ObservableCollection<Notification>();
            this.OverallProgress = new ProgressNotification();
            this.AllUnits = new ObservableItemCollection<UnitViewModelBase>();
            this.ConnectedUnits = new FilteredObservableCollection<UnitViewModelBase>(
                this.AllUnits,
                u => u.ConnectionState == ConnectionState.Connected);
            this.Tabs = new ObservableCollection<UnitTabBase>();

            Predicate<object> filterStatusNotifications = o =>
                {
                    var notification = o as StatusNotification;
                    if (notification == null)
                    {
                        return false;
                    }

                    return !notification.IsAcknowledged;
                };
            this.statusNotifications = new Lazy<ICollectionView>(
                () =>
                {
                    var source = CollectionViewSource.GetDefaultView(this.Notifications);
                    source.Filter = filterStatusNotifications;
                    return source;
                });

            this.AllUnitsTab = new AllUnitsTab(this)
                               {
                                   Name = DiagStrings.Shell_AllUnitsTabHeader,
                               };
            this.Tabs.Add(this.AllUnitsTab);
            this.IsAutoRefresh = true;
        }

        /// <summary>
        /// Gets the notifications
        /// </summary>
        public ObservableCollection<Notification> Notifications { get; private set; }

        /// <summary>
        /// Gets the status notifications
        /// </summary>
        public ICollectionView StatusNotifications
        {
            get
            {
                return this.statusNotifications.Value;
            }
        }

        /// <summary>
        /// Gets or set the overall progress
        /// </summary>
        public ProgressNotification OverallProgress { get; private set; }

        /// <summary>
        /// Gets the application state
        /// </summary>
        public IDiagApplicationState DiagApplicationState
        {
            get
            {
                return this.ApplicationState as IDiagApplicationState;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether icenter.diag is
        /// automatically refreshing the list of units using UDCP.
        /// </summary>
        public bool IsAutoRefresh
        {
            get
            {
                return this.isAutoRefresh;
            }

            set
            {
                this.SetProperty(ref this.isAutoRefresh, value, () => this.IsAutoRefresh);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show tile view.
        /// </summary>
        public bool ShowTileView
        {
            get
            {
                return this.showTileView;
            }

            set
            {
                this.SetProperty(ref this.showTileView, value, () => this.ShowTileView);
                this.RaisePropertyChanged(() => this.ShowGridView);
            }
        }

        /// <summary>
        /// Gets a value indicating whether show grid view.
        /// </summary>
        public bool ShowGridView
        {
            get
            {
                return !this.showTileView;
            }
        }

        /// <summary>
        /// Gets the all units tab
        /// </summary>
        public AllUnitsTab AllUnitsTab { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected unit tab
        /// </summary>
        public UnitTabBase SelectedUnitTab
        {
            get
            {
                return this.selectedUnitTab;
            }

            set
            {
                this.SetProperty(ref this.selectedUnitTab, value, () => this.SelectedUnitTab);
            }
        }

        /// <summary>
        /// Gets the currently selected unit
        /// </summary>
        public UnitViewModelBase SelectedUnit
        {
            get
            {
                var tab = this.SelectedUnitTab;
                var allUnits = tab as AllUnitsTab;
                if (allUnits != null)
                {
                    return allUnits.SelectedUnit;
                }

                var unit = tab as UnitTab;
                return unit != null ? unit.Unit : null;
            }
        }

        /// <summary>
        /// Gets the list of all units.
        /// </summary>
        public ObservableItemCollection<UnitViewModelBase> AllUnits { get; private set; }

        /// <summary>
        /// Gets the connected units.
        /// </summary>
        public FilteredObservableCollection<UnitViewModelBase> ConnectedUnits { get; private set; }

        /// <summary>
        /// Gets the list of all tabs visible in the main window.
        /// </summary>
        public ObservableCollection<UnitTabBase> Tabs { get; private set; }

        /// <summary>
        /// Gets the show main menu command.
        /// </summary>
        public ICommand ShowFileMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowFileMenu);
            }
        }

        /// <summary>
        /// Gets the show view menu command.
        /// </summary>
        public ICommand ShowUnitMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowUnitMenu);
            }
        }

        /// <summary>
        /// Gets the show view menu command.
        /// </summary>
        public ICommand ShowApplicationMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowApplicationMenu);
            }
        }

        /// <summary>
        /// Gets the show view menu command.
        /// </summary>
        public ICommand ShowViewMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowViewMenu);
            }
        }

        /// <summary>
        /// Gets the Toggle Connect command
        /// </summary>
        public ICommand ToggleUnitConnectionCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.ToggleConnect);
            }
        }

        /// <summary>
        /// Gets the Toggle Favorite command
        /// </summary>
        public ICommand ToggleUnitFavoriteCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.ToggleFavorite);
            }
        }

        /// <summary>
        /// Gets the Announce Unit command
        /// </summary>
        public ICommand AnnounceUnitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.Announce);
            }
        }

        /// <summary>
        /// Gets the Reboot Unit command
        /// </summary>
        public ICommand RebootUnitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.Reboot);
            }
        }

        /// <summary>
        /// Gets the Connect Unit command
        /// </summary>
        public ICommand ConnectUnitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.Connect);
            }
        }

        /// <summary>
        /// Gets the Disconnect Unit command
        /// </summary>
        public ICommand DisconnectUnitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.Disconnect);
            }
        }

        /// <summary>
        /// Gets the request add unit command.
        /// </summary>
        public ICommand RequestAddUnitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.RequestAdd);
            }
        }

        /// <summary>
        /// Gets the main menu interaction request.
        /// </summary>
        public IInteractionRequest MainMenuInteractionRequest
        {
            get
            {
                return InteractionManager<MainMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the unit menu interaction request.
        /// </summary>
        public IInteractionRequest UnitMenuInteractionRequest
        {
            get
            {
                return InteractionManager<UnitMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the unit menu interaction request.
        /// </summary>
        public IInteractionRequest ApplicationMenuInteractionRequest
        {
            get
            {
                return InteractionManager<ApplicationMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the view menu interaction request.
        /// </summary>
        public IInteractionRequest ViewMenuInteractionRequest
        {
            get
            {
                return InteractionManager<ViewMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the add unit interaction request.
        /// </summary>
        public IInteractionRequest AddUnitRequest
        {
            get
            {
                return InteractionManager<AddUnitPromptNotification>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the add unit interaction request.
        /// </summary>
        public IInteractionRequest EditIpAddressRequest
        {
            get
            {
                return InteractionManager<EditIpSettingsPromptNotification>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the UnitActionConfirmationPrompt request.
        /// </summary>
        public IInteractionRequest UnitActionConfirmationPromptRequest
        {
            get
            {
                return InteractionManager<UnitActionConfirmationPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the add unit interaction request.
        /// </summary>
        public IInteractionRequest BusyRequest
        {
            get
            {
                return InteractionManager<BusyPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the about screen interaction request.
        /// </summary>
        public IInteractionRequest AboutScreenRequest
        {
            get
            {
                return InteractionManager<AboutScreenPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the option dialog interaction request.
        /// </summary>
        public IInteractionRequest OptionDialogRequest
        {
            get
            {
                return InteractionManager<OptionsPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, () => this.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the busy message
        /// </summary>
        public string BusyMessage
        {
            get
            {
                return this.busyMessage;
            }

            set
            {
                this.SetProperty(ref this.busyMessage, value, () => this.BusyMessage);
            }
        }

        /// <summary>
        /// Creates a tab for the given unit
        /// </summary>
        /// <param name="unit">the unit</param>
        public void CreateUnitTab(UnitViewModelBase unit)
        {
            var existingTab = this.Tabs.OfType<UnitTab>().FirstOrDefault(t => t.Unit == unit);
            if (existingTab != null)
            {
                // needed to get the tab to the foreground,
                // since due to Converter issues the Binding mode is OneWay
                this.Tabs.Remove(existingTab);
                this.Tabs.Add(existingTab);
            }
            else
            {
                this.Tabs.Add(new UnitTab(this, unit, this.commandRegistry));
            }
        }

        /// <summary>
        /// Removes the tab for the given unit
        /// </summary>
        /// <param name="unit">the unit</param>
        public void RemoveUnitTab(UnitViewModelBase unit)
        {
            var unitTab = this.Tabs.OfType<UnitTab>().FirstOrDefault(t => t.Unit == unit);
            if (unitTab != null)
            {
                this.Tabs.Remove(unitTab);
            }
        }

        /// <summary>
        /// Handling of weak events
        /// </summary>
        /// <param name="managerType">the type of the manager</param>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event arguments</param>
        /// <returns>a boolean indicating if the event was successfully handled</returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = (PropertyChangedEventArgs)e;
            return this.OnPropertyChanged(args);
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        protected override void CreateWindow()
        {
            Logger.Trace("Creating window");
            base.CreateWindow();
            PropertyChangedEventManager.AddListener(this.DiagApplicationState, this, "CurrentTenant");
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            return true;
        }
    }
}