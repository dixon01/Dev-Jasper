// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfiguratorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfiguratorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.AhdlcRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Composer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Export;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Init;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.IO;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Update;
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The controller for the unit configurator window.
    /// </summary>
    public sealed class UnitConfiguratorController : WindowControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<CategoryControllerBase> categoryControllers = new List<CategoryControllerBase>();
        private readonly List<ExportControllerBase> exportControllers = new List<ExportControllerBase>();

        private DocumentVersionWritableModel documentVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfiguratorController"/> class.
        /// </summary>
        /// <param name="unitConfiguration">
        /// The unit configuration view model that will be edited.
        /// </param>
        /// <param name="dataController">
        /// The data controller used to access data from the background system.
        /// </param>
        public UnitConfiguratorController(
            UnitConfigurationReadOnlyDataViewModel unitConfiguration,
            DataController dataController)
            : this(unitConfiguration, new CommandRegistry(), dataController)
        {
        }

        private UnitConfiguratorController(
            UnitConfigurationReadOnlyDataViewModel unitConfiguration,
            ICommandRegistry commandRegistry,
            DataController dataController)
            : base(new UnitConfiguratorViewModel(commandRegistry))
        {
            this.UnitConfiguration = unitConfiguration;
            this.CommandRegistry = commandRegistry;
            this.DataController = dataController;

            this.CreateControllers();
            this.InitializeControllers();

            this.SetupCommands();

            this.Window.Created += this.WindowOnCreated;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public UnitConfiguratorViewModel ViewModel
        {
            get
            {
                return (UnitConfiguratorViewModel)this.Window;
            }
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the data controller to access data view models.
        /// </summary>
        public DataController DataController { get; private set; }

        /// <summary>
        /// Gets the unit configuration that is being edited.
        /// </summary>
        public UnitConfigurationReadOnlyDataViewModel UnitConfiguration { get; private set; }

        /// <summary>
        /// Gets the hardware descriptor of the hardware that is being configured.
        /// </summary>
        public HardwareDescriptor HardwareDescriptor { get; private set; }

        /// <summary>
        /// Gets the version number of the document currently being created.
        /// </summary>
        public Version VersionNumber
        {
            get
            {
                return new Version(this.documentVersion.Major, this.documentVersion.Minor);
            }
        }

        /// <summary>
        /// Get a part controller with the given unique key.
        /// </summary>
        /// <param name="key">
        /// The the unique key to identify the searched part.
        /// If this is null, the first part of the given type is returned.
        /// </param>
        /// <typeparam name="T">
        /// The type of part expected.
        /// </typeparam>
        /// <returns>
        /// The part.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// If the given key and/or type don't exist.
        /// </exception>
        public T GetPart<T>(string key = null) where T : PartControllerBase
        {
            foreach (var controller in this.categoryControllers)
            {
                T part;
                if (controller.TryGetPart(key, out part))
                {
                    return part;
                }
            }

            throw new KeyNotFoundException("Couldn't find part controller: " + key);
        }

        /// <summary>
        /// Gets all export controllers.
        /// </summary>
        /// <returns>
        /// The list of all export controllers.
        /// </returns>
        public IEnumerable<ExportControllerBase> GetExportControllers()
        {
            return this.exportControllers;
        }

        /// <summary>
        /// Gets the export controller of the given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of export controller.
        /// </typeparam>
        /// <returns>
        /// The export controller, never null.
        /// </returns>
        public T GetExportController<T>() where T : ExportControllerBase
        {
            return this.exportControllers.OfType<T>().Single();
        }

        /// <summary>
        /// Asynchronously creates the entire export structure for this unit configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> returning all root folders.
        /// </returns>
        public async Task<List<ExportFolder>> CreateExportStructureAsync()
        {
            var rootFolders = new List<ExportFolder>();
            await Task.WhenAll(this.exportControllers.Select(c => c.CreateExportStructureAsync(rootFolders)));
            return rootFolders;
        }

        /// <summary>
        /// Creates a <see cref="UnitConfigData"/> model object representing all configurations to be saved in the
        /// XML document.
        /// </summary>
        /// <returns>
        /// The <see cref="UnitConfigData"/> for all controllers.
        /// </returns>
        public UnitConfigData CreateUnitConfigData()
        {
            var data = new UnitConfigData();
            this.DoWithControllers(data, (controller, categoryData) => controller.Save(categoryData));
            return data;
        }

        /// <summary>
        /// Loads the given unit configuration data into this configurator.
        /// </summary>
        /// <param name="data">
        /// The data to be loaded.
        /// </param>
        public void LoadData(UnitConfigData data)
        {
            var isInitializationVisible = data.Categories.Count > 1;

            foreach (var category in this.categoryControllers)
            {
                category.ViewModel.CanBeVisible = isInitializationVisible;
            }

            this.DoWithControllers(data, (controller, categoryData) => controller.Load(categoryData));
        }

        /// <summary>
        /// Brings this window to the front.
        /// </summary>
        public void BringToFront()
        {
            this.ViewModel.BringToFront();
        }

        /// <summary>
        /// Navigates to the given part.
        /// </summary>
        /// <param name="part">
        /// The part to navigate to.
        /// </param>
        public void NavigateToPart(PartViewModelBase part)
        {
            this.ViewModel.SelectedItem = part;
        }

        /// <summary>
        /// Asynchronously saves the configuration to database with the version
        /// and comment of the <paramref name="checkinTrapResult"/>.
        /// </summary>
        /// <param name="checkinTrapResult">
        /// The object containing the version and comment.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        public async Task CheckInAsync(CheckinTrapResult checkinTrapResult)
        {
            try
            {
                if (checkinTrapResult.Decision == CheckinUserDecision.Checkin)
                {
                    this.documentVersion.Major = checkinTrapResult.Major;
                    this.documentVersion.Minor = checkinTrapResult.Minor;
                    this.documentVersion.Description = checkinTrapResult.CheckinComment;
                    await this.SaveDataAsync();
                    this.CreateNextVersion();

                    // load commited version
                    this.LoadVersion(await this.GetLatestVersionAsync());
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't save document version");
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.ViewModel.IsSaving = false;
        }

        /// <summary>
        /// Creates a new version for the document being edited.
        /// This method must be called after <see cref="SaveDataAsync"/> if the user can continue to edit the document.
        /// </summary>
        public void CreateNextVersion()
        {
            var newVersion = this.DataController.ConnectionController.DocumentVersionChangeTrackingManager.Create();
            newVersion.Document = this.UnitConfiguration.Document.ReadableModel;
            newVersion.CreatingUser = this.documentVersion.CreatingUser;

            newVersion.Major = -1;
            newVersion.Minor = -1;
            newVersion.Content = this.documentVersion.Content;
            this.documentVersion = newVersion;
        }

        /// <summary>
        /// Raises the <see cref="WindowControllerBase.WindowClosing"/> event.
        /// </summary>
        /// <param name="cancelEventArgs">
        /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        /// </param>
        protected override async void RaiseWindowClosing(CancelEventArgs cancelEventArgs)
        {
            base.RaiseWindowClosing(cancelEventArgs);
            if (this.ViewModel.IsSaving)
            {
                // we can't close while saving
                cancelEventArgs.Cancel = true;
                return;
            }

            if (!this.ViewModel.IsDirty)
            {
                // just close the window
                return;
            }

            var messageBoxResult = MessageBox.Show(
                AdminStrings.UnitConfiguration_UnsavedBox_Content,
                AdminStrings.UnitConfiguration_UnsavedBox_Title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            switch (messageBoxResult)
            {
                case MessageBoxResult.No:
                    return;
                case MessageBoxResult.Cancel:
                    cancelEventArgs.Cancel = true;
                    return;
            }

            // IMPORTANT: this needs to be done before the first async call, otherwise it won't cancel!
            cancelEventArgs.Cancel = true;

            var taskCompletionSource = new TaskCompletionSource<CheckinTrapResult>();
            var checkinArguments = new CheckinDialogArguments
                                       {
                                           OnCheckinCompleted = async checkinResult =>
                                               {
                                                   await this.CheckInAsync(checkinResult);
                                                   taskCompletionSource.TrySetResult(checkinResult);
                                               }
                                       };
            this.Commit(checkinArguments);
            var result = await taskCompletionSource.Task;
            if (result.Decision == CheckinUserDecision.Cancel)
            {
                cancelEventArgs.Cancel = true;
                return;
            }

            this.Close();
        }

        private async Task SaveDataAsync()
        {
            var data = this.CreateUnitConfigData();
            this.documentVersion.Content = new XmlData(data);
            this.ViewModel.ClearDirty();

            this.ViewModel.IsSaving = true;
            try
            {
                await
                    this.DataController.ConnectionController.DocumentVersionChangeTrackingManager.CommitAndVerifyAsync(
                        this.documentVersion);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't save document version");
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.ViewModel.IsSaving = false;
        }

        private void SetupCommands()
        {
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.NavigateToPart,
                new RelayCommand<PartViewModelBase>(this.NavigateToPart, this.CanNavigateToPart));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.NavigateToPrevious,
                new RelayCommand(this.NavigateToPrevious, this.CanNavigateToPrevious));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.NavigateToNext,
                new RelayCommand(this.NavigateToNext, this.CanNavigateToNext));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.Commit,
                new RelayCommand<CheckinDialogArguments>(this.Commit, this.CanCommit));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.CommitAndLoad,
                new RelayCommand<DocumentVersionReadableModel>(this.CommitAndLoad));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.LoadVersion,
                new RelayCommand<DocumentVersionReadableModel>(this.LoadVersion));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.Cancel,
                new RelayCommand(this.Cancel, this.CanCancel));

            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitConfig.Export,
                new RelayCommand(this.Export, this.CanExport));
        }

        private async void CommitAndLoad(DocumentVersionReadableModel readable)
        {
            this.ViewModel.IsSaving = true;

            var taskCompletionSource = new TaskCompletionSource<CheckinTrapResult>();
            var checkinArguments = new CheckinDialogArguments
            {
                Skippable = false,
                OnCheckinCompleted = async checkinResult =>
                {
                    await this.CheckInAsync(checkinResult);
                    taskCompletionSource.TrySetResult(checkinResult);
                }
            };
            this.Commit(checkinArguments);
            var result = await taskCompletionSource.Task;

            if (result.Decision == CheckinUserDecision.Cancel)
            {
                this.ViewModel.RevertCurrentVersion();
                this.ViewModel.IsSaving = false;
                return;
            }

            this.LoadVersion(readable);
        }

        private async void LoadVersion(DocumentVersionReadableModel readable)
        {
            this.ViewModel.IsLoading = true;

            await readable.LoadReferencePropertiesAsync();
            await readable.LoadXmlPropertiesAsync();

            // copy everyting in the current working version writable model
            this.documentVersion.Document = readable.Document;
            this.documentVersion.Major = readable.Major;
            this.documentVersion.Minor = readable.Minor;
            this.documentVersion.Content = readable.Content;
            this.documentVersion.Description = readable.Description;
            this.documentVersion.CreatingUser = readable.CreatingUser;

            this.LoadData();

            this.ViewModel.IsLoading = false;

            // after IsLoading filtered categorys are updated
            this.RestoreLastVisibleTab();
        }

        private void RestoreLastVisibleTab()
        {
            var visibleCategorys = this.ViewModel.FilteredCategories.OfType<CategoryViewModel>();

            foreach (var category in visibleCategorys)
            {
                if (category.Equals(this.ViewModel.LastVisibleTab))
                {
                    this.ViewModel.SelectedItem = category;
                    category.IsExpanded = true;
                    return;
                }

                foreach (var part in category.FilteredParts.OfType<PartViewModelBase>()
                                                           .Where(part => part.Equals(this.ViewModel.LastVisibleTab)))
                {
                    this.ViewModel.SelectedItem = part;
                    return;
                }
            }

            var selectedCategory = this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().FirstOrDefault();
            if (selectedCategory == null)
            {
                return;
            }

            this.ViewModel.SelectedItem = selectedCategory;
            selectedCategory.IsExpanded = true;
        }

        private void CreateControllers()
        {
            this.categoryControllers.Add(new InitializationCategoryController());

            switch (this.UnitConfiguration.ProductType.UnitType)
            {
                case UnitTypes.Tft:
                case UnitTypes.Obu:
                    this.categoryControllers.Add(new HardwareCategoryController());
                    this.categoryControllers.Add(new SoftwareCategoryController());
                    this.categoryControllers.Add(new TimeSyncCategoryController());
                    this.categoryControllers.Add(new SystemConfigCategoryController());
                    this.categoryControllers.Add(new SplashScreensCategoryController());
                    this.categoryControllers.Add(new UpdateCategoryController());
                    this.categoryControllers.Add(new ProtranCategoryController());
                    this.categoryControllers.Add(new TransformationsCategoryController());
                    this.categoryControllers.Add(new IoProtocolCategoryController());
                    this.categoryControllers.Add(new IbisProtocolCategoryController());
                    this.categoryControllers.Add(new Vdv301ProtocolCategoryController());
                    this.categoryControllers.Add(new ComposerCategoryController());
                    this.categoryControllers.Add(new DirectXRendererCategoryController());
                    this.categoryControllers.Add(new AhdlcRendererCategoryController());
                    this.categoryControllers.Add(new AudioRendererCategoryController());
                    this.categoryControllers.Add(new ThorebC90CategoryController());

                    this.exportControllers.Add(new SystemManagerExportController());
                    this.exportControllers.Add(new HardwareManagerExportController());
                    this.exportControllers.Add(new UpdateExportController());
                    this.exportControllers.Add(new ProtranExportController());
                    this.exportControllers.Add(new ComposerExportController());
                    this.exportControllers.Add(new DirectXRendererExportController());
                    this.exportControllers.Add(new AhdlcRendererExportController());
                    this.exportControllers.Add(new AudioRendererExportController());
                    this.exportControllers.Add(new AcapelaExportController());
                    this.exportControllers.Add(new BusModuleExportController());
                    this.exportControllers.Add(new TerminalExportController());
                    this.exportControllers.Add(new IbisControlExportController());
                    this.exportControllers.Add(new McuExportController());
                    this.exportControllers.Add(new LamExportController());
                    break;

                case UnitTypes.EPaper:
                    this.categoryControllers.Add(new MainUnitCategoryController());
                    this.exportControllers.Add(new MainUnitExportController());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.categoryControllers.Add(new ConclusionController());
        }

        private void InitializeControllers()
        {
            foreach (var controller in this.categoryControllers)
            {
                controller.Initialize(this);
                this.ViewModel.Categories.Add(controller.ViewModel);
            }

            foreach (var controller in this.exportControllers)
            {
                controller.Initialize(this);
            }
        }

        private async Task InitializeAsync()
        {
            this.ViewModel.IsLoading = true;

            var connectionController = this.DataController.ConnectionController;
            var applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
            var document = this.UnitConfiguration.Document.ReadableModel;
            var version = connectionController.DocumentVersionChangeTrackingManager.Create();
            version.Document = document;
            version.CreatingUser = connectionController.UserChangeTrackingManager.Wrap(applicationState.CurrentUser);

            var lastVersion = await this.GetLatestVersionAsync();
            if (lastVersion == null)
            {
                // should not happen
            }
            else
            {
                version.Major = lastVersion.Major;
                version.Minor = lastVersion.Minor;
                version.Content = lastVersion.Content;
            }

            this.documentVersion = version;
            this.ViewModel.DocumentName = this.UnitConfiguration.Document.Name;
            this.ViewModel.DocumentVersions = document.Versions;
            this.SetTitle();

            await this.UnitConfiguration.ProductType.ReadableModel.LoadXmlPropertiesAsync();
            this.HardwareDescriptor =
                (HardwareDescriptor)this.UnitConfiguration.ProductType.ReadableModel.HardwareDescriptor.Deserialize();
            await this.PrepareControllersAsync(this.HardwareDescriptor);

            this.LoadData();

            this.ViewModel.IsLoading = false;
            this.ViewModel.SelectedItem =
                this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().FirstOrDefault();

            var category = this.ViewModel.SelectedItem as CategoryViewModel;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (category != null)
            {
                category.IsExpanded = true;
            }
        }

        private async Task PrepareControllersAsync(HardwareDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            await Task.WhenAll(this.categoryControllers.Select(c => c.PrepareAsync(descriptor)));
        }

        private void LoadData()
        {
            var data = (UnitConfigData)this.documentVersion.Content.Deserialize();
            if (data.Categories.Count == 0)
            {
                // add the no data flag (otherwise we don't know if there is old data or no data)
                data.Categories.Add(
                    new UnitConfigCategory
                        {
                            Key = UnitConfigKeys.Initialization.Category,
                            Parts =
                                {
                                    new UnitConfigPart
                                        {
                                            Key = UnitConfigKeys.Initialization.LoadData,
                                            Values =
                                                {
                                                    new UnitConfigPartValue
                                                        {
                                                            Key = LoadDataPartController.HasNoDataKey,
                                                            Value = true.ToString()
                                                        }
                                                }
                                        }
                                }
                        });
            }

            this.LoadData(data);

            this.ViewModel.ClearDirty();
            this.ViewModel.CurrentVersion = this.GetReadableModelByVersion(this.documentVersion);

            if (this.UnitConfiguration.Document.ReadableModel.Versions.Any(v =>
                        v.Major > this.ViewModel.CurrentVersion.Major
                        || (v.Major == this.ViewModel.CurrentVersion.Major
                            && v.Minor > this.ViewModel.CurrentVersion.Minor)))
            {
                this.ViewModel.IsLatestVersion = false;
            }
            else
            {
                this.ViewModel.IsLatestVersion = true;
            }
        }

        private async Task<DocumentVersionReadableModel> GetLatestVersionAsync()
        {
            var document = this.UnitConfiguration.Document.ReadableModel;
            await document.LoadNavigationPropertiesAsync();

            var latest = document.Versions.OrderByDescending(v => v.Id).FirstOrDefault();

            if (latest != null)
            {
                await latest.LoadXmlPropertiesAsync();
            }

            return latest;
        }

        private DocumentVersionReadableModel GetReadableModelByVersion(
            DocumentVersionWritableModel documentVersionWritableModel)
        {
            var document = this.UnitConfiguration.Document.ReadableModel;
            var readable = document.Versions.FirstOrDefault(v =>
                v.Major == documentVersionWritableModel.Major
                && v.Minor == documentVersionWritableModel.Minor);

            if (readable == null)
            {
                throw new Exception("No readable UnitConfiguration document found.");
            }

            return readable;
        }

        private void DoWithControllers(UnitConfigData data, Action<CategoryControllerBase, UnitConfigCategory> action)
        {
            foreach (var controller in this.categoryControllers)
            {
                var categoryData = data.Categories.FirstOrDefault(p => p.Key == controller.Key);
                if (categoryData == null)
                {
                    categoryData = new UnitConfigCategory { Key = controller.Key };
                    data.Categories.Add(categoryData);
                }

                action(controller, categoryData);
            }
        }

        private bool CanNavigateToPart(PartViewModelBase part)
        {
            return part != null;
        }

        private bool CanNavigateToPrevious(object obj)
        {
            return this.ViewModel.SelectedItem != null && this.ViewModel.SelectedItem
                   != this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().FirstOrDefault();
        }

        private void NavigateToPrevious()
        {
            var category = this.ViewModel.SelectedItem as CategoryViewModel;
            if (category != null)
            {
                var previousCategory =
                    this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().PreviousBefore(c => c == category);
                if (previousCategory != null)
                {
                    this.ViewModel.SelectedItem =
                        previousCategory.FilteredParts.OfType<PartViewModelBase>().LastOrDefault();
                }

                return;
            }

            var part = this.ViewModel.SelectedItem as PartViewModelBase;
            if (part == null)
            {
                return;
            }

            var previousPart = part.Category.FilteredParts.OfType<PartViewModelBase>().PreviousBefore(p => p == part);
            if (previousPart != null)
            {
                this.ViewModel.SelectedItem = previousPart;
                return;
            }

            this.ViewModel.SelectedItem = part.Category;
            part.Category.IsExpanded = true;
        }

        private bool CanNavigateToNext(object obj)
        {
            var lastCategory = this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().LastOrDefault();
            if (lastCategory == null)
            {
                return false;
            }

            return this.ViewModel.SelectedItem
                   != lastCategory.FilteredParts.OfType<PartViewModelBase>().LastOrDefault();
        }

        private void NavigateToNext()
        {
            var category = this.ViewModel.SelectedItem as CategoryViewModel;
            if (category != null)
            {
                this.ViewModel.SelectedItem = category.FilteredParts.OfType<PartViewModelBase>().FirstOrDefault();
                return;
            }

            var part = this.ViewModel.SelectedItem as PartViewModelBase;
            if (part == null)
            {
                return;
            }

            var nextPart = part.Category.FilteredParts.OfType<PartViewModelBase>().NextAfter(p => p == part);
            if (nextPart != null)
            {
                this.ViewModel.SelectedItem = nextPart;
                return;
            }

            var nextCategory =
                this.ViewModel.FilteredCategories.OfType<CategoryViewModel>().NextAfter(c => c == part.Category);
            this.ViewModel.SelectedItem = nextCategory;
        }

        private bool CanCommit(object obj)
        {
            return this.ViewModel.IsDirty &&
                this.categoryControllers.All(controller => controller.ViewModel.ErrorState < ErrorState.Missing);
        }

        private async void Commit(CheckinDialogArguments arguments)
        {
            this.ViewModel.IsSaving = true;
            if (arguments == null)
            {
                arguments = new CheckinDialogArguments
                                {
                                    Skippable = false,
                                    OnCheckinCompleted = async result => { await this.CheckInAsync(result); }
                                };
            }

            var major = 0;
            var minor = 0;

            var document = this.UnitConfiguration.Document.ReadableModel;
            await document.LoadNavigationPropertiesAsync();
            var lastVersion = document.Versions.OrderByDescending(v => v.Id).FirstOrDefault();

            if (lastVersion != null)
            {
                major = lastVersion.Major;
                minor = lastVersion.Minor;
            }

            var checkInPrompt = new CheckInPrompt
            {
                Major = (major + 1) + ".0",
                Minor = string.Format("{0}.{1}", major, minor + 1),
                OnCheckinCompleted = arguments.OnCheckinCompleted,
                IsSkippable = arguments.Skippable,
                ConfigurationLabel = AdminStrings.UnitConfig_Title + ": ",
                ConfigurationTitle = this.ViewModel.DocumentName,
                RequiredDataScope = DataScope.UnitConfiguration
            };

            InteractionManager<CheckInPrompt>.Current.Raise(checkInPrompt);
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private void Cancel()
        {
            this.Close();
        }

        private bool CanExport(object obj)
        {
            return !this.GetPart<LoadDataPartController>().ViewModel.IsVisible;
        }

        private void Export()
        {
            var interaction = new SaveFileDialogInteraction
                {
                    DefaultExtension = ".ucg",
                    AddExtension = true,
                    OverwritePrompt = true,
                    Filter = AdminStrings.UnitConfiguration_Export_FileFilters,
                    Title = AdminStrings.UnitConfiguration_Export_Title
                };

            InteractionManager<SaveFileDialogInteraction>.Current.Raise(interaction, this.ExportSelected);
        }

        private void ExportSelected(SaveFileDialogInteraction interaction)
        {
            if (!interaction.Confirmed)
            {
                return;
            }

            try
            {
                var container = new UnitConfigContainer
                    {
                        CreationTime = TimeProvider.Current.Now,
                        Name = this.ViewModel.DocumentName,
                        ProductType = this.UnitConfiguration.ProductType.Name,
                        UnitConfig = this.CreateUnitConfigData()
                    };
                var configurator = new Configurator(interaction.FileName);
                configurator.Serialize(container);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Error(ex, "Couldn't export unit configuration to " + interaction.FileName);
            }
        }

        private void SetTitle()
        {
            this.ViewModel.Title = string.Format(
                "{0} - {1}",
                AdminStrings.UnitConfig_Title,
                this.ViewModel.DocumentName);
        }

        private async void WindowOnCreated(object sender, EventArgs eventArgs)
        {
            try
            {
                await this.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Error(ex,"Couldn't load unit configuration " + this.UnitConfiguration.DocumentDisplayText);
            }
        }
    }
}
