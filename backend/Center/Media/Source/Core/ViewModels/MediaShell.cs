// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaShell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The Media shell.
    /// </summary>
    [Export(typeof(IMediaShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MediaShell : ClientShellBase, IMediaShell, IWeakEventListener
    {
        private const string WindowTitle = "icenter.media";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly Lazy<ICollectionView> statusNotifications;

        private readonly Lazy<DictionaryDataViewModel> lazyDictionary
            = new Lazy<DictionaryDataViewModel>(GetDictionary);

        private readonly Lazy<IPermissionController> lazyPermissionController
            = new Lazy<IPermissionController>(GetPermissionController);

        private IEditorViewModel editor;

        private double zoom = 100d;

        private Point layoutPosition;

        private EditorToolType selectedEditorTool;

        private CycleNavigationViewModel cycleNavigator;

        private ResolutionNavigationPrompt resolutionNavigation;

        private bool simulationIsVisible;

        private ExtendedObservableCollection<ConsistencyMessageDataViewModel> consistencyMessages;

        private bool isBusy;

        private bool isBusyIndeterminate;

        private string busyContentTextFormat;

        private double currentBusyProgress;

        private double totalBusyProgress;
        private string currentBusyProgressText;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaShell"/> class.
        /// </summary>
        /// <param name="mediaShellParams">The set of constructor parameters.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public MediaShell(MediaShellParams mediaShellParams, ICommandRegistry commandRegistry)
            : base(
                mediaShellParams.Factory,
                null,
                mediaShellParams.MenuItems,
                mediaShellParams.StatusBarItems,
                commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.TftEditorToolbarViewModel = new TftEditorToolbarViewModel(this, commandRegistry);

            this.Notifications = new ObservableCollection<Notification>();
            this.Title = WindowTitle;
            this.OverallProgress = new ProgressNotification();
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

            this.SelectedEditorTool = EditorToolType.Move;

            // TODO: make dependend on current CycleItem type
            this.Editors = new Dictionary<PhysicalScreenType, IEditorViewModel>();
            this.Editors[PhysicalScreenType.TFT] = new TftEditorViewModel(this, commandRegistry);
            this.Editors[PhysicalScreenType.LED] = new LedEditorViewModel(this, commandRegistry);
            this.Editors[PhysicalScreenType.Audio] = new AudioEditorViewModel(this, commandRegistry);

            this.editor = this.Editors[PhysicalScreenType.TFT];
            this.cycleNavigator = new CycleNavigationViewModel(this, commandRegistry);
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry
        {
            get
            {
                return this.commandRegistry;
            }
        }

        /// <summary>
        /// Gets the view model representing the TFT editor toolbar.
        /// </summary>
        public TftEditorToolbarViewModel TftEditorToolbarViewModel { get; private set; }

        /// <summary>
        /// Gets the Undo Command.
        /// </summary>
        public ICommand UndoCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Undo);
            }
        }

        /// <summary>
        /// Gets the Redo Command.
        /// </summary>
        public ICommand RedoCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Redo);
            }
        }

        /// <summary>
        /// Gets the Cut Command.
        /// </summary>
        public ICommand CutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Cut);
            }
        }

        /// <summary>
        /// Gets the Copy Command.
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Copy);
            }
        }

        /// <summary>
        /// Gets the Paste Command.
        /// </summary>
        public ICommand PasteCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Paste);
            }
        }

        /// <summary>
        /// Gets the Delete Command.
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.Delete);
            }
        }

        /// <summary>
        /// Gets the SelectAll Command.
        /// </summary>
        public ICommand SelectAllCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Default.SelectAll);
            }
        }

        /// <summary>
        /// Gets the Exit Command.
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.Application.Exit);
            }
        }

        /// <summary>
        /// Gets the ShowMainMenu Command.
        /// </summary>
        public ICommand ShowMainMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu);
            }
        }

        /// <summary>
        /// Gets the ShowEditMenu Command.
        /// </summary>
        public ICommand ShowEditMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowEditMenu);
            }
        }

        /// <summary>
        /// Gets the ShowViewMenu Command.
        /// </summary>
        public ICommand ShowViewMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowViewMenu);
            }
        }

        /// <summary>
        /// Gets the ShowFormulaEditor Command.
        /// </summary>
        public ICommand ShowFormulaEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowFormulaEditor);
            }
        }

        /// <summary>
        /// Gets the ShowAnimationEditor Command.
        /// </summary>
        public ICommand ShowAnimationEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowAnimationEditor);
            }
        }

        /// <summary>
        /// Gets the RemoveFormula command.
        /// </summary>
        public ICommand RemoveLayoutFormulaCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.RemoveFormula);
            }
        }

        /// <summary>
        /// Gets the RemoveFormula command.
        /// </summary>
        public ICommand RemoveLayoutAnimationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.RemoveAnimation);
            }
        }

        /// <summary>
        /// Gets the ShowLayoutNavigation Command.
        /// </summary>
        public ICommand ShowLayoutNavigationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowLayoutNavigation);
            }
        }

        /// <summary>
        /// Gets the ShowResolutionNavigation Command.
        /// </summary>
        public ICommand ShowResolutionNavigationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowResolutionNavigation);
            }
        }

        /// <summary>
        /// Gets the Play Preview Command.
        /// </summary>
        public ICommand PlayPreviewCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Preview.Play);
            }
        }

        /// <summary>
        /// Gets the Pause Preview Command.
        /// </summary>
        public ICommand PausePreviewCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Preview.Pause);
            }
        }

        /// <summary>
        /// Gets the Stop Preview Command.
        /// </summary>
        public ICommand StopPreviewCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Preview.Stop);
            }
        }

        /// <summary>
        /// Gets the consistency dialog command.
        /// </summary>
        public ICommand ConsistencyDialogCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowConsistencyDialog);
            }
        }

        /// <summary>
        /// Gets the choose virtual display command.
        /// </summary>
        public ICommand ChooseVirtualDisplayCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.VirtualDisplay.Choose);
            }
        }

        /// <summary>
        /// Gets the consistency interaction request.
        /// </summary>
        public IInteractionRequest ConsistencyInteractionRequest
        {
            get
            {
                return InteractionManager<ConsistencyCheckPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the MainMenu Interaction Request.
        /// </summary>
        public IInteractionRequest MainMenuInteractionRequest
        {
            get
            {
                return InteractionManager<MainMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the EditMenu Interaction Request.
        /// </summary>
        public IInteractionRequest EditMenuInteractionRequest
        {
            get
            {
                return InteractionManager<EditMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the ViewMenu Interaction Request.
        /// </summary>
        public IInteractionRequest ViewMenuInteractionRequest
        {
            get
            {
                return InteractionManager<ViewMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the FormulaEditor Interaction Request.
        /// </summary>
        public IInteractionRequest FormulaEditorInteractionRequest
        {
            get
            {
                return InteractionManager<FormulaEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the FormulaEditor Interaction Request.
        /// </summary>
        public IInteractionRequest AnimationEditorInteractionRequest
        {
            get
            {
                return InteractionManager<AnimationEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the FormulaEditor Interaction Request.
        /// </summary>
        public IInteractionRequest FormulaNavigationEditorInteractionRequest
        {
            get
            {
                return InteractionManager<FormulaNavigationEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the Trigger Editor Interaction Request.
        /// </summary>
        public IInteractionRequest TriggerNavigationEditorInteractionRequest
        {
            get
            {
                return InteractionManager<TriggerNavigationEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the FormulaEditor Interaction Request.
        /// </summary>
        public IInteractionRequest AnimationNavigationEditorInteractionRequest
        {
            get
            {
                return InteractionManager<AnimationNavigationEditorPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the LayoutNavigation Interaction Request.
        /// </summary>
        public IInteractionRequest LayoutNavigationInteractionRequest
        {
            get
            {
                return InteractionManager<LayoutNavigationPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the ResolutionNavigation Interaction Request.
        /// </summary>
        public IInteractionRequest ResolutionNavigationInteractionRequest
        {
            get
            {
                return InteractionManager<ResolutionNavigationPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the show check in dialog interaction request.
        /// </summary>
        public IInteractionRequest ShowCheckInDialogInteractionRequest
        {
            get
            {
                return InteractionManager<CheckInPrompt>.Current.GetOrCreateInteractionRequest();
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
        /// Gets the ShowCycleNavigation Command.
        /// </summary>
        public IInteractionRequest ShowCycleNavigation
        {
            get
            {
                return InteractionManager<ShowCycleNavigationPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the ShowSectionNavigation Command.
        /// </summary>
        public IInteractionRequest ShowSectionNavigation
        {
            get
            {
                return InteractionManager<ShowSectionNavigationPrompt>.Current.GetOrCreateInteractionRequest();
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
        /// Gets the notifications.
        /// </summary>
        public ObservableCollection<Notification> Notifications { get; private set; }

        /// <summary>
        /// Gets the status notifications.
        /// </summary>
        public ICollectionView StatusNotifications
        {
            get
            {
                return this.statusNotifications.Value;
            }
        }

        /// <summary>
        /// Gets or sets the Editor ViewModel.
        /// </summary>
        public IEditorViewModel Editor
        {
            get
            {
                return this.editor;
            }

            set
            {
                this.SetProperty(ref this.editor, value, () => this.Editor);
            }
        }

        /// <summary>
        /// Gets the editors.
        /// </summary>
        public Dictionary<PhysicalScreenType, IEditorViewModel> Editors { get; private set; }

        /// <summary>
        /// Gets or sets the CycleNavigator ViewModel.
        /// </summary>
        public CycleNavigationViewModel CycleNavigator
        {
            get
            {
                return this.cycleNavigator;
            }

            set
            {
                this.SetProperty(ref this.cycleNavigator, value, () => this.CycleNavigator);
            }
        }

        /// <summary>
        /// Gets or sets the ResolutionNavigation ViewModel.
        /// </summary>
        public ResolutionNavigationPrompt ResolutionNavigation
        {
            get
            {
                return this.resolutionNavigation;
            }

            set
            {
                this.SetProperty(ref this.resolutionNavigation, value, () => this.ResolutionNavigation);
            }
        }

        /// <summary>
        /// Gets or sets the Zoom
        /// </summary>
        public double Zoom
        {
            get
            {
                return this.zoom;
            }

            set
            {
                this.SetProperty(ref this.zoom, value, () => this.Zoom);
            }
        }

        /// <summary>
        /// Gets or sets the position of the layout
        /// </summary>
        public Point LayoutPosition
        {
            get
            {
                return this.layoutPosition;
            }

            set
            {
                this.SetProperty(ref this.layoutPosition, value, () => this.LayoutPosition);
            }
        }

        /// <summary>
        /// Gets the dictionary
        /// </summary>
        public DictionaryDataViewModel Dictionary
        {
            get
            {
                return this.lazyDictionary.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation is visible
        /// </summary>
        public bool SimulationIsVisible
        {
            get
            {
                return this.simulationIsVisible;
            }

            set
            {
                this.SetProperty(ref this.simulationIsVisible, value, () => this.SimulationIsVisible);
            }
        }

        /// <summary>
        /// Gets the consistency messages.
        /// </summary>
        public ExtendedObservableCollection<ConsistencyMessageDataViewModel> ConsistencyMessages
        {
            get
            {
                if (this.consistencyMessages == null)
                {
                    this.consistencyMessages = this.MediaApplicationState.ConsistencyMessages;
                    this.consistencyMessages.CollectionChanged += this.OnConsistencyChanged;
                }

                return this.consistencyMessages;
            }
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        public ProgressNotification OverallProgress { get; private set; }

        /// <summary>
        /// Gets the state of the Media application.
        /// </summary>
        /// <value>
        /// The state of the Media application.
        /// </value>
        public IMediaApplicationState MediaApplicationState
        {
            get
            {
                return this.ApplicationState as IMediaApplicationState;
            }
        }

        /// <summary>
        /// Gets or sets the selected editor tool
        /// </summary>
        public EditorToolType SelectedEditorTool
        {
            get
            {
                return this.selectedEditorTool;
            }

            set
            {
                this.SetProperty(ref this.selectedEditorTool, value, () => this.SelectedEditorTool);
                if (this.TftEditorToolbarViewModel != null)
                {
                    this.TftEditorToolbarViewModel.SelectedEditorToolChanged(value);
                }
            }
        }

        /// <summary>
        /// Gets the save project command
        /// </summary>
        public ICommand SaveProjectCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.Save);
            }
        }

        /// <summary>
        /// Gets the save as project command
        /// </summary>
        public ICommand CheckInProjectCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CheckIn);
            }
        }

        /// <summary>
        /// Gets the navigate to main menu new command.
        /// </summary>
        public ICommand ShowMenuEntryCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ShowMenuEntry);
            }
        }

        /// <summary>
        /// Gets the toggle simulation command.
        /// </summary>
        public ICommand ToggleSimulationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ToggleSimulation);
            }
        }

        /// <summary>
        /// Gets the toggle edge snap command.
        /// </summary>
        public ICommand ToggleEdgeSnapCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ToggleEdgeSnap);
            }
        }

        /// <summary>
        /// Gets the close main menu command.
        /// </summary>
        public ICommand CloseMainMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.CloseMainMenu);
            }
        }

        /// <summary>
        /// Gets the delete selected elements command
        /// </summary>
        public ICommand DeleteSelectedLayoutElements
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.DeleteSelectedElements);
            }
        }

        /// <summary>
        /// Gets the rename layout element.
        /// </summary>
        public ICommand RenameLayoutElement
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.RenameLayoutElement);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is loading pending project.
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
        /// Gets or sets a value indicating whether is busy indeterminate.
        /// </summary>
        public bool IsBusyIndeterminate
        {
            get
            {
                return this.isBusyIndeterminate;
            }

            set
            {
                this.SetProperty(ref this.isBusyIndeterminate, value, () => this.IsBusyIndeterminate);
            }
        }

        /// <summary>
        /// Gets or sets the busy content text format.
        /// </summary>
        public string BusyContentTextFormat
        {
            get
            {
                return this.busyContentTextFormat;
            }

            set
            {
                this.SetProperty(ref this.busyContentTextFormat, value, () => this.BusyContentTextFormat);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets the busy content text.
        /// </summary>
        public string BusyContentText
        {
            get
            {
                if (string.IsNullOrEmpty(this.BusyContentTextFormat))
                {
                    return string.Empty;
                }

                try
                {
                    if (!this.IsBusyIndeterminate)
                    {
                        return string.Format(
                            this.BusyContentTextFormat,
                            this.CurrentBusyProgress,
                            this.TotalBusyProgress);
                    }

                    return this.BusyContentTextFormat;
                }
                catch (FormatException ex)
                {
                    Logger.DebugException("The BusyContentTextFormat is not valid.", ex);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the current busy progress.
        /// </summary>
        public double CurrentBusyProgress
        {
            get
            {
                return this.currentBusyProgress;
            }

            set
            {
                this.SetProperty(ref this.currentBusyProgress, value, () => this.CurrentBusyProgress);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets or sets the total busy progress.
        /// </summary>
        public double TotalBusyProgress
        {
            get
            {
                return this.totalBusyProgress;
            }

            set
            {
                this.SetProperty(ref this.totalBusyProgress, value, () => this.TotalBusyProgress);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets or sets the current busy progress text.
        /// </summary>
        public string CurrentBusyProgressText
        {
            get
            {
                return this.currentBusyProgressText;
            }

            set
            {
                this.SetProperty(ref this.currentBusyProgressText, value, () => this.CurrentBusyProgressText);
            }
        }

        /// <summary>
        /// Gets the permission controller.
        /// </summary>
        public IPermissionController PermissionController
        {
            get
            {
                return this.lazyPermissionController.Value;
            }
        }

        /// <summary>
        /// Clears all properties to their default value.
        /// </summary>
        public void ClearBusy()
        {
            this.IsBusy = false;
            this.BusyContentTextFormat = string.Empty;
            this.IsBusyIndeterminate = false;
            this.CurrentBusyProgress = 0;
            this.CurrentBusyProgressText = string.Empty;
            this.TotalBusyProgress = 0;
        }

        /// <summary>
        /// Sets the current editor.
        /// </summary>
        /// <param name="screenType">
        /// The physical screen type.
        /// </param>
        public void SetCurrentEditor(PhysicalScreenType screenType)
        {
            this.Editor = this.Editors[screenType];
        }

        /// <summary>
        /// Reloads on switching the Tenant
        /// </summary>
        public void ReloadOnTenantSwitch()
        {
        }

        /// <summary>
        /// Sets the project name in front of the window title.
        /// </summary>
        /// <param name="projectName">
        /// The project name.
        /// </param>
        public void SetProjectTitle(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentNullException("projectName", @"The name can't be null or empty.");
            }

            this.Title = string.Format("{0} - {1}", projectName, WindowTitle);
        }

        /// <summary>
        /// The clear project title.
        /// </summary>
        public void ClearProjectTitle()
        {
            this.Title = WindowTitle;
        }

        /// <summary>
        /// Handling of weak events
        /// </summary>
        /// <param name="managerType">the type of the manager</param>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event arguments</param>
        /// <returns>a boolean indicating if the event was successfully handled</returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
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
            PropertyChangedEventManager.AddListener(this.MediaApplicationState, this, "CurrentTenant");
        }

        private static DictionaryDataViewModel GetDictionary()
        {
            return ServiceLocator.Current.GetInstance<DictionaryDataViewModel>();
        }

        private static IPermissionController GetPermissionController()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationController>().PermissionController;
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            return true;
        }

        private void OnConsistencyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.ConsistencyMessages);
        }

        /// <summary>
        /// Defines the parameters used by the constructor of the <see cref="MediaShell"/> class.
        /// </summary>
        [Export]
        public class MediaShellParams
        {
            /// <summary>
            /// Gets or sets the factory.
            /// </summary>
            /// <value>
            /// The factory.
            /// </value>
            [Import]
            public MediaShellFactory Factory { get; set; }

            /// <summary>
            /// Gets or sets the stages.
            /// </summary>
            /// <value>
            /// The stages.
            /// </value>
            [ImportMany]
            public IEnumerable<Lazy<IStage, IStageMetadata>> Stages { get; set; }

            /// <summary>
            /// Gets or sets the menu items.
            /// </summary>
            /// <value>
            /// The menu items.
            /// </value>
            [ImportMany]
            public IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> MenuItems { get; set; }

            /// <summary>
            /// Gets or sets the status bar items.
            /// </summary>
            /// <value>
            /// The status bar items.
            /// </value>
            [ImportMany]
            public IEnumerable<Lazy<StatusBarItemBase>> StatusBarItems { get; set; }
        }
    }
}