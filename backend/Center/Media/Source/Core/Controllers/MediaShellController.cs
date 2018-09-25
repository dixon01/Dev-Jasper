// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Controllers.EditorControllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.Simulation;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Center.Media.Core.ViewModels.Options;

    using NLog;

    /// <summary>
    /// Defines the controller used for the shell of the Media application.
    /// </summary>
    [Export(typeof(IMediaShellController)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MediaShellController : WindowControllerBase, IMediaShellController, IWeakEventListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private AboutScreenPrompt aboutScreenPrompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaShellController"/> class.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public MediaShellController(IMediaShell shell, ICommandRegistry commandRegistry)
            : base(shell)
        {
            PropertyChangedEventManager.AddListener(shell, this, "ActiveStage");
            this.commandRegistry = commandRegistry;

            this.RegisterCommands();

            this.MainMenuPrompt = new MainMenuPrompt(shell, commandRegistry);
            this.Shell.Closing += this.ShellOnClosing;
            this.Shell.Closed += this.ShellOnClosed;
            this.ChangeHistoryController = new ChangeHistoryController(this, this.Shell, this.commandRegistry);

            this.ProjectController = new ProjectController(
                this.Shell,
                this,
                this.MainMenuPrompt,
                commandRegistry);

            this.FormulaController = new FormulaController(this, this.Shell, commandRegistry);
            this.FormulaManagerController = new FormulaManagerController(this, this.Shell, commandRegistry);
            this.PostUndoController = new PostUndoController(this, shell, commandRegistry);
            this.GeneralEditorController = new GeneralEditorController(this, shell, commandRegistry);
            this.TftEditorController = new TftEditorController(this, shell, commandRegistry);
            this.LedEditorController = new LedEditorController(this, shell, commandRegistry);
            this.AudioEditorController = new AudioEditorController(this, shell, commandRegistry);
            this.LayerEditorController = new LayerEditorController(this, shell, commandRegistry);
            this.CycleController = new CycleController(this, this.Shell, commandRegistry);
            this.ExportController = new ExportController(this, this.MainMenuPrompt, commandRegistry);
            this.PhysicalScreenController = new PhysicalScreenController(this, shell, commandRegistry);
            this.ImportController = new ImportController(this, commandRegistry);
            this.SimulationManager = new SimulationManager(this.Shell);
            this.TextReplacementController = new TextReplacementController(
                this.Shell,
                this,
                this.commandRegistry);
            this.CsvMappingController = new CsvMappingController(
                this.Shell,
                this,
                this.commandRegistry);
            this.ResourceController = new ResourceController(this, this.Shell, commandRegistry);
            this.LayoutController = new LayoutController(this, this.Shell, commandRegistry);
            this.NavigationController = new NavigationController(this.Shell, this.commandRegistry);
            this.OptionsController = new OptionsController(commandRegistry, "Media", "MediaApplication");
        }

        /// <summary>
        /// Gets or sets the main menu prompt.
        /// </summary>
        /// <value>
        /// The main menu prompt.
        /// </value>
        public MainMenuPrompt MainMenuPrompt { get; set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        [Import]
        public IMediaApplicationController ParentController { get; set; }

        IShellViewModel IShellController.Shell
        {
            get
            {
                return this.Shell;
            }
        }

        /// <summary>
        /// Gets the options controller.
        /// </summary>
        public OptionsController OptionsController { get; private set; }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public IMediaShell Shell
        {
            get
            {
                return this.Window as IMediaShell;
            }
        }

        /// <summary>
        /// Gets the layout editor controller.
        /// </summary>
        public EditorControllerBase PostUndoController { get; private set; }

        /// <summary>
        /// Gets the TFT editor controller.
        /// </summary>
        public EditorControllerBase TftEditorController { get; private set; }

        /// <summary>
        /// Gets the general editor controller.
        /// </summary>
        public EditorControllerBase GeneralEditorController { get; private set; }

        /// <summary>
        /// Gets the led editor controller.
        /// </summary>
        public EditorControllerBase LedEditorController { get; private set; }

        /// <summary>
        /// Gets the audio editor controller.
        /// </summary>
        public EditorControllerBase AudioEditorController { get; private set; }

        /// <summary>
        /// Gets or sets the export controller.
        /// </summary>
        /// <value>
        /// The export controller.
        /// </value>
        public IExportController ExportController { get; set; }

        /// <summary>
        /// Gets or sets the import controller.
        /// </summary>
        public IImportController ImportController { get; set; }

        /// <summary>
        /// Gets the project controller.
        /// </summary>
        /// <value>
        /// The project controller.
        /// </value>
        public IProjectController ProjectController { get; private set; }

        /// <summary>
        /// Gets the formula controller.
        /// </summary>
        /// <value>
        /// The formula controller.
        /// </value>
        public IFormulaController FormulaController { get; private set; }

        /// <summary>
        /// Gets the physical screen controller.
        /// </summary>
        /// <value>
        /// The physical screen controller.
        /// </value>
        public IPhysicalScreenController PhysicalScreenController { get; private set; }

        /// <summary>
        /// Gets the text replacement controller.
        /// </summary>
        public ITextReplacementController TextReplacementController { get; private set; }

        /// <summary>
        /// Gets the csv mapping controller.
        /// </summary>
        public ICsvMappingController CsvMappingController { get; private set; }

        /// <summary>
        /// Gets the resource controller.
        /// </summary>
        public IResourceController ResourceController { get; private set; }

        /// <summary>
        /// Gets or sets the change history controller.
        /// </summary>
        public IChangeHistoryController ChangeHistoryController { get; set; }

        /// <summary>
        /// Gets or sets the Cycle controller.
        /// </summary>
        /// <value>
        /// The Cycle controller.
        /// </value>
        private ICycleController CycleController { get; set; }

        /// <summary>
        /// Gets or sets the formula manager controller.
        /// </summary>
        /// <value>
        /// The formula manager controller.
        /// </value>
        private IFormulaManagerController FormulaManagerController { get; set; }

        /// <summary>
        /// Gets or sets the simulation manager.
        /// </summary>
        /// <value>
        /// The simulation manager.
        /// </value>
        private SimulationManager SimulationManager { get; set; }

        /// <summary>
        /// Gets or sets the layout controller.
        /// </summary>
        private ILayoutController LayoutController { get; set; }

        /// <summary>
        /// Gets or sets the layer editor controller.
        /// </summary>
        private ILayerEditorController LayerEditorController { get; set; }

        /// <summary>
        /// Gets or sets the navigation controller.
        /// </summary>
        private INavigationController NavigationController { get; set; }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">
        /// The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event contextMenu.</param>
        /// <returns>
        /// true if the listener handled the event.
        /// It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF
        /// to register a listener for an event that the listener does not handle.
        /// Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = (PropertyChangedEventArgs)e;
            return this.OnPropertyChanged(args);
        }

        private static IDynamicDataValue GetFormulaDataValueFromContextMenu(ContextMenu contextMenu)
        {
            IDynamicDataValue dataValue = null;

            if (contextMenu != null)
            {
                var item = contextMenu.PlacementTarget as PropertyGridItem;
                if (item != null)
                {
                    var dataSource = item.Tag as IDataValue;

                    var value = dataSource as IDynamicDataValue;
                    if (value != null)
                    {
                        dataValue = value;
                    }
                }
            }

            return dataValue;
        }

        private static IAnimatedDataValue GetAnimationDataValueFromContextMenu(ContextMenu contextMenu)
        {
            IAnimatedDataValue dataValue = null;

            if (contextMenu != null)
            {
                var item = contextMenu.PlacementTarget as PropertyGridItem;
                if (item != null)
                {
                    var dataSource = item.Tag as IDataValue;

                    var value = dataSource as IAnimatedDataValue;
                    if (value != null)
                    {
                        dataValue = value;
                    }
                }
            }

            return dataValue;
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here it is a long init")]
        private void RegisterCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowMainMenu, new RelayCommand(this.ShowMainMenu));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowEditMenu, new RelayCommand(this.ShowEditMenu));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowViewMenu, new RelayCommand(this.ShowViewMenu));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.Menu.ShowMenuEntry,
                new RelayCommand<MenuNavigationParameters.MainMenuEntries>(
                    this.ShowMainMenuEntry,
                    this.CanExecuteMenuEntry));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.Menu.CloseMainMenu,
                new RelayCommand(this.CloseMainMenu));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowFormulaEditor,
                new RelayCommand<ContextMenu>(this.ShowFormulaEditor));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowAnimationEditor,
                new RelayCommand<ContextMenu>(this.ShowAnimationEditor));

            this.commandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.UI.ShowNavigationFormulaEditor,
               new RelayCommand<ContextMenu>(this.ShowNavigationFormulaEditor));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowNavigationAnimationEditor,
                new RelayCommand<ContextMenu>(
                    this.ShowNavigationAnimationEditor,
                    this.CanExecuteShowNavigationAnimationEditor));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowNavigationTriggerEditor,
                new RelayCommand<TriggerEditorParameters>(this.ShowNavigationTriggerEditor));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowLayoutNavigation, new RelayCommand(this.ShowLayoutNavigation));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowResolutionNavigation,
                new RelayCommand(this.ShowResolutionNavigation));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Preview.Play, new RelayCommand(this.PlayPreview));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Preview.Pause, new RelayCommand(this.PausePreview));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.Menu.ToggleSimulation,
                new RelayCommand(this.ShowSimulationToggle, this.CanShowSimulationToggle));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.Menu.ToggleEdgeSnap, new RelayCommand(this.UseEdgeSnapToggle));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowConsistencyDialog,
                new RelayCommand(this.ShowConsistencyCheckDialog));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowCheckinDialog,
                new RelayCommand<CheckinDialogArguments>(this.ShowCheckInDialog, this.CanCheckin));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowAboutScreen,
                new RelayCommand(this.ShowAboutScreen));

            this.commandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.UI.ShowOptionsDialog,
               new RelayCommand(this.ShowOptionsDialog));
        }

        private void ShowOptionsDialog()
        {
            Logger.Debug("Showing options dialog");

            var optionsPrompt = new OptionsPrompt(this.commandRegistry, this.Shell.MediaApplicationState.Options);
            var category = optionsPrompt.Categories.First();
            category.Title = FrameworkStrings.OptionsDialog_GeneralTitle;
            category.TitleTooltip = FrameworkStrings.OptionsDialog_GeneralTooltip;
            var generalIcon = new BitmapImage();
            generalIcon.BeginInit();
            generalIcon.UriSource =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Common.Wpf.Views;component/Icons/application-gear_32x32.png");
            generalIcon.EndInit();
            category.CategoryIconSource = generalIcon;
            var group = category.Groups.First();
            group.Label = FrameworkStrings.OptionsDialog_LanguageLabel;
            group.GroupLabelTooltip = FrameworkStrings.OptionsDialog_LanguageLabelTooltip;
            ((LanguageOptionGroupViewModel)group).RestartInformation =
               FrameworkStrings.OptionsDialog_LanguageRestartInformation;

            var resourceCategory = optionsPrompt.Categories.First(c => c is LocalResourceOptionCategoryViewModel);
            resourceCategory.Title = MediaStrings.OptionsDialog_LocalResourceCategoryTitle;
            resourceCategory.TitleTooltip = MediaStrings.OptionsDialog_LocalResourceCategoryTooltip;
            var resourceIcon = new BitmapImage();
            resourceIcon.BeginInit();
            var resourceIconUri =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Media.Core;"
                    + "component/Resources/Images/Icons/archive_32x32.png");
            resourceIcon.UriSource = resourceIconUri;
            resourceIcon.EndInit();
            resourceCategory.CategoryIconSource = resourceIcon;
            var resourceGroup = resourceCategory.Groups.First(g => g is LocalResourceOptionGroupViewModel);
            resourceGroup.Label = MediaStrings.OptionsDialog_LocalResourceGroupLabel;
            resourceGroup.GroupLabelTooltip = MediaStrings.OptionsDialog_LocalResourceGroupTooltip;

            var rendererCategory = optionsPrompt.Categories.First(c => c is RendererOptionCategoryViewModel);
            rendererCategory.Title = MediaStrings.OptionsDialog_RendererCategoryTitle;
            rendererCategory.TitleTooltip = MediaStrings.OptionsDialog_RendererCategoryTooltip;
            var rendererIcon = new BitmapImage();
            rendererIcon.BeginInit();
            rendererIcon.UriSource =
                new Uri(
                    @"pack://application:,,,/Gorba.Center.Media.Core;"
                    + "component/Resources/Images/Icons/directxrenderer_32x32.png");
            rendererIcon.EndInit();
            rendererCategory.CategoryIconSource = rendererIcon;
            var rendererGroup = rendererCategory.Groups.First();
            rendererGroup.Label = MediaStrings.OptionsDialog_RendererGroupLabel;
            rendererGroup.GroupLabelTooltip = MediaStrings.OptionsDialog_RendererGroupTooltip;
            optionsPrompt.SelectedCategory = optionsPrompt.Categories.First();
            InteractionManager<OptionsPrompt>.Current.Raise(optionsPrompt);
        }

        private bool CanShowSimulationToggle(object obj)
        {
            return this.Shell.Editor is TftEditorViewModel;
        }

        private bool CanExecuteCreatePermission()
        {
            return this.Shell.PermissionController.HasPermission(Permission.Create, DataScope.MediaConfiguration);
        }

        private bool CanExecuteReadPermission()
        {
            return this.Shell.PermissionController.HasPermission(Permission.Read, DataScope.MediaConfiguration);
        }

        private bool CanExecuteWritePermission()
        {
            return this.Shell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration);
        }

        private bool CanExecuteProjectOpen()
        {
            return this.Shell.MediaApplicationState.CurrentProject != null;
        }

        private bool CanExecuteMenuEntry(MenuNavigationParameters.MainMenuEntries desiredEntry)
        {
            switch (desiredEntry)
            {
                case MenuNavigationParameters.MainMenuEntries.FileNew:
                case MenuNavigationParameters.MainMenuEntries.FileImport:
                    return this.CanExecuteCreatePermission();
                case MenuNavigationParameters.MainMenuEntries.FileOpen:
                    return this.CanExecuteReadPermission();
                case MenuNavigationParameters.MainMenuEntries.FileExport:
                    return this.CanExecuteProjectOpen();
                case MenuNavigationParameters.MainMenuEntries.FileResourceManager:
                    return this.CanExecuteReadPermission() && this.CanExecuteProjectOpen();
                case MenuNavigationParameters.MainMenuEntries.FileReplacement:
                    return this.CanExecuteReadPermission() && this.CanExecuteProjectOpen();
                case MenuNavigationParameters.MainMenuEntries.FileFormulaManager:
                    return this.CanExecuteReadPermission() && this.CanExecuteProjectOpen();
                case MenuNavigationParameters.MainMenuEntries.FileSaveAs:
                    return this.CanExecuteCreatePermission() && this.CanExecuteProjectOpen();
                default:
                    Logger.Error("Unknown menu object '{0}'", desiredEntry);
                    break;
            }

            return false;
        }

        private void ShowMainMenuEntry(MenuNavigationParameters.MainMenuEntries desiredEntry)
        {
            var newSelection = new MenuNavigationParameters { Root = desiredEntry };

            this.ShowMainMenu(newSelection);
        }

        private void CloseMainMenu()
        {
            // TODO
        }

        private bool CanCheckin(object obj)
        {
            if (!this.Shell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration)
                || this.Shell.MediaApplicationState.CurrentProject == null
                || this.Shell.MediaApplicationState.CurrentProject.IsCheckedIn)
            {
                return false;
            }

            return true;
        }

        private void PlayPreview()
        {
            Logger.Debug("Request to play the preview.");
        }

        private void PausePreview()
        {
            Logger.Debug("Request to pause the preview.");
        }

        private void ShowSimulationToggle()
        {
            if (this.Shell.SimulationIsVisible)
            {
                this.HideSimulation();
            }
            else
            {
                this.ShowSimulation();
            }
        }

        private void UseEdgeSnapToggle()
        {
            this.Shell.MediaApplicationState.UseEdgeSnap = !this.Shell.MediaApplicationState.UseEdgeSnap;
        }

        private void ShowSimulation()
        {
            Logger.Debug("Request to show the simulation.");

            this.Shell.SimulationIsVisible = true;
        }

        private void HideSimulation()
        {
            Logger.Debug("Request to hide the simulation.");

            this.Shell.SimulationIsVisible = false;
        }

        private void ShowEditMenu()
        {
            Logger.Debug("Request to show the edit menu.");
            InteractionManager<EditMenuPrompt>.Current.Raise(new EditMenuPrompt(this.commandRegistry));
        }

        private void ShowViewMenu()
        {
            Logger.Debug("Request to show the view menu.");
            InteractionManager<ViewMenuPrompt>.Current.Raise(new ViewMenuPrompt(this.commandRegistry, this.Shell));
        }

        private void ShowLayoutNavigation()
        {
            Logger.Debug("Request to show the layout navigation.");
            var currentProject = this.Shell.MediaApplicationState.CurrentProject.InfomediaConfig;

            var prompt = new LayoutNavigationPrompt(this.commandRegistry)
            {
                Shell = this.Shell,
                Layouts = currentProject.Layouts,
                Projects = new List<InfomediaConfigDataViewModel> { currentProject },
            };
            InteractionManager<LayoutNavigationPrompt>.Current.Raise(prompt);
        }

        private void ShowResolutionNavigation()
        {
            Logger.Debug("Request to show the layout navigation.");
            this.Shell.ResolutionNavigation = new ResolutionNavigationPrompt(this.Shell, this.commandRegistry)
            {
                IsOpen = true
            };
            InteractionManager<ResolutionNavigationPrompt>.Current.Raise(this.Shell.ResolutionNavigation);
        }

        private void ShowFormulaEditor(ContextMenu data)
        {
            var dataValue = GetFormulaDataValueFromContextMenu(data);
            Action<FormulaEditorPrompt> callback = prompt =>
                {
                    dataValue.RaiseFormulaChanged();
                    InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                };
            Logger.Debug("Request to show the Formula Editor.");
            InteractionManager<FormulaEditorPrompt>.Current.Raise(
                new FormulaEditorPrompt(this.Shell, dataValue, this.commandRegistry) { IsOpen = true }, callback);
        }

        private void ShowAnimationEditor(ContextMenu data)
        {
            var dataValue = GetAnimationDataValueFromContextMenu(data);

            Logger.Debug("Request to show the Animation Editor.");
            InteractionManager<AnimationEditorPrompt>.Current.Raise(
                new AnimationEditorPrompt(this.Shell, dataValue, this.commandRegistry) { IsOpen = true },
                prompt => InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt()));
        }

        private void ShowNavigationFormulaEditor(ContextMenu data)
        {
            var dataValue = GetFormulaDataValueFromContextMenu(data);

            Logger.Debug("Request to show the Formula Editor.");
            InteractionManager<FormulaNavigationEditorPrompt>.Current.Raise(
                new FormulaNavigationEditorPrompt(this.Shell, dataValue, this.commandRegistry) { IsOpen = true },
                prompt => InteractionManager<UpdateCycleDetailsPrompt>.Current.Raise(new UpdateCycleDetailsPrompt()));
        }

        private void ShowNavigationTriggerEditor(TriggerEditorParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Logger.Debug("Request to show the Trigger Editor.");
            var genericTrigger = (GenericTriggerConfigDataViewModel)parameters.DataSource.Value;

            InteractionManager<TriggerNavigationEditorPrompt>.Current.Raise(
                new TriggerNavigationEditorPrompt(this.Shell, genericTrigger, this.commandRegistry) { IsOpen = true },
                prompt => InteractionManager<UpdateCycleDetailsPrompt>.Current.Raise(new UpdateCycleDetailsPrompt()));
        }

        private void ShowNavigationAnimationEditor(ContextMenu data)
        {
            var dataValue = GetAnimationDataValueFromContextMenu(data);

            Logger.Debug("Request to show the Animation Editor.");
            InteractionManager<AnimationNavigationEditorPrompt>.Current.Raise(
                new AnimationNavigationEditorPrompt(this.Shell, dataValue, this.commandRegistry) { IsOpen = true },
                prompt => InteractionManager<UpdateCycleDetailsPrompt>.Current.Raise(new UpdateCycleDetailsPrompt()));
        }

        private bool CanExecuteShowNavigationAnimationEditor(ContextMenu data)
        {
            var dataValue = GetAnimationDataValueFromContextMenu(data);

            return dataValue != null;
        }

        private void ShowCheckInDialog(CheckinDialogArguments checkinDialogArguments)
        {
            Logger.Debug("Request to show the check-in dialog");

            if (this.Shell.MediaApplicationState.CurrentProject == null)
            {
                return;
            }

            if (checkinDialogArguments == null)
            {
                checkinDialogArguments =
                    new CheckinDialogArguments { Skippable = false, OnCheckinCompleted = s => { } };
            }

            var currentConfiguration =
                this.Shell.MediaApplicationState.ExistingProjects.FirstOrDefault(
                    p => p.Name == this.Shell.MediaApplicationState.CurrentProject.Name);
            var major = 0;
            var minor = 0;
            if (currentConfiguration != null)
            {
                var currentVersion = currentConfiguration.Document.ReadableModel.Versions.LastOrDefault();
                if (currentVersion != null)
                {
                    major = currentVersion.Major;
                    minor = currentVersion.Minor;
                }
            }

            var checkInPrompt = new CheckInPrompt
                                    {
                                        Major = (major + 1) + ".0",
                                        Minor = string.Format("{0}.{1}", major, minor + 1),
                                        OnCheckinCompleted = checkinDialogArguments.OnCheckinCompleted,
                                        IsSkippable = checkinDialogArguments.Skippable,
                                        ConfigurationLabel = MediaStrings.CheckInDialog_ProjectLabel,
                                        ConfigurationTitle = this.Shell.MediaApplicationState.CurrentProject.Name,
                                        RequiredDataScope = DataScope.MediaConfiguration
                                    };

            InteractionManager<CheckInPrompt>.Current.Raise(checkInPrompt);
        }

        private void ShowAboutScreen()
        {
            Logger.Debug("Request to show the about screen");
            if (this.aboutScreenPrompt == null)
            {
                var applicationIcon = new BitmapImage();
                applicationIcon.BeginInit();
                applicationIcon.UriSource =
                    new Uri("pack://application:,,,/Gorba.Center.Media.Core;component/Resources/media_196x196.png");
                applicationIcon.EndInit();

                this.aboutScreenPrompt = new AboutScreenPrompt
                                             {
                                                 ApplicationIconSource = applicationIcon,
                                                 IsOpen = true
                                             };
            }
            else
            {
                InteractionAction.SkipNextMouseUp = true;
            }

            InteractionManager<AboutScreenPrompt>.Current.Raise(this.aboutScreenPrompt);
        }

        private void ShowConsistencyCheckDialog()
        {
            Logger.Debug("Request to show the consistency check dialog.");
            var prompt = new ConsistencyCheckPrompt(
                this.Shell.MediaApplicationState.ConsistencyMessages,
                this.Shell.MediaApplicationState.CompatibilityMessages,
                this.commandRegistry);
            InteractionManager<ConsistencyCheckPrompt>.Current.Raise(prompt);
        }

        private void ShowMainMenu(object newSelection)
        {
            Logger.Debug("Request to show the main menu.");

            if (!this.MainMenuPrompt.IsOpen)
            {
                InteractionManager<MainMenuPrompt>.Current.Raise(this.MainMenuPrompt);
                this.MainMenuPrompt.IsOpen = true;
            }
            else if (newSelection is ExecutedRoutedEventArgs)
            {
                InteractionManager<MainMenuPrompt>.Current.Raise(this.MainMenuPrompt);
                this.MainMenuPrompt.IsOpen = false;
            }

            this.MainMenuPrompt.RaiseNavigationRequest(newSelection);
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            return true;
        }

        private void ShellOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.RaiseWindowClosing(cancelEventArgs);
        }

        private void ShellOnClosed(object sender, EventArgs eventArgs)
        {
            this.RaiseWindowClosed();
        }
    }
}