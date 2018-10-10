// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implements the <see cref="IMediaApplicationState"/> interface to provide the state of the Media application.
    /// </summary>
    [Export]
    [Export(typeof(IApplicationState))]
    [Export(typeof(IConnectedApplicationState))]
    [Export(typeof(IMediaApplicationState))]
    [DataContract]
    public class MediaApplicationState : ConnectedApplicationState, IMediaApplicationState
    {
        private static readonly AppDataResourceProvider AppDataResourceProvider =
            new AppDataResourceProvider(Settings.Default.AppDataResourcesRelativePath);

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues;

        private ExtendedObservableCollection<RecentProjectDataViewModel> recentProjects;

        private Dictionary<Guid, ExtendedObservableCollection<ResourceInfoDataViewModel>> recentMediaResources;

        private bool useEdgeSnap = true;

        private PhysicalScreenConfigDataViewModel currentPhysicalScreen;

        private VirtualDisplayConfigDataViewModel currentVirtualDisplay;

        private CyclePackageConfigDataViewModel currentCyclePackage;

        private CycleConfigDataViewModelBase currentCycle;

        private SectionConfigDataViewModelBase currentSection;

        private LayoutConfigDataViewModelBase currentLayout;

        private IProjectManager projectManager;

        private ProjectStates currentProjectState;

        private MediaProjectDataViewModel currentProject;

        private ExtendedObservableCollection<ConsistencyMessageDataViewModel> consistencyMessages;

        private ExtendedObservableCollection<ConsistencyMessageDataViewModel> compatibilityMessages;

        private ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> currentVirtualDisplayReferences;

        private MasterLayout defaultMasterLayout;

        private ObservableCollection<MediaConfigurationDataViewModel> existingProjects;

        private bool isExistingProjectsLoaded;

        private MediaConfigurationDataViewModel currentMediaConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaApplicationState"/> class.
        /// </summary>
        public MediaApplicationState()
        {
            this.recentDictionaryValues = new ExtendedObservableCollection<DictionaryValueDataViewModel>();
            this.recentMediaResources =
                new Dictionary<Guid, ExtendedObservableCollection<ResourceInfoDataViewModel>>();
            this.recentProjects = new ExtendedObservableCollection<RecentProjectDataViewModel>();
            this.LastUsedDirectories = new Dictionary<DialogDirectoryType, string>();
            this.consistencyMessages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            this.compatibilityMessages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            this.AllExistingProjects = new Dictionary<int, ObservableCollection<MediaConfigurationDataViewModel>>();
            this.existingProjects = new ObservableCollection<MediaConfigurationDataViewModel>();
            this.SystemFonts = new Dictionary<string, List<string>>();
            this.defaultMasterLayout = new MasterLayout
                                           {
                                               Name = "Normal",
                                               Columns = "*",
                                               HorizontalGaps = "0",
                                               Rows = "*",
                                               VerticalGaps = "0"
                                           };
        }

        /// <summary>
        /// Gets the default (full screen) master layout.
        /// </summary>
        public MasterLayout DefaultMasterLayout
        {
            get
            {
                return this.defaultMasterLayout;
            }
        }

        /// <summary>
        /// Gets or sets the existing projects of the current tenant.
        /// </summary>
        public ObservableCollection<MediaConfigurationDataViewModel> ExistingProjects
        {
            get
            {
                return this.existingProjects;
            }

            set
            {
                this.SetProperty(ref this.existingProjects, value, () => this.ExistingProjects);
            }
        }

        /// <summary>
        /// Gets all existing projects of all authorized tenants.
        /// </summary>
        public Dictionary<int, ObservableCollection<MediaConfigurationDataViewModel>> AllExistingProjects
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is existing projects loaded.
        /// </summary>
        public bool IsExistingProjectsLoaded
        {
            get
            {
                return this.isExistingProjectsLoaded;
            }

            set
            {
                this.SetProperty(ref this.isExistingProjectsLoaded, value, () => this.IsExistingProjectsLoaded);
            }
        }

        /// <summary>
        /// Gets or sets the current project state.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        public ProjectStates CurrentProjectState
        {
            get
            {
                return this.currentProjectState;
            }

            set
            {
                this.SetProperty(ref this.currentProjectState, value, () => this.CurrentProjectState);
            }
        }

        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        public MediaProjectDataViewModel CurrentProject
        {
            get
            {
                return this.currentProject;
            }

            set
            {
                if (this.currentProject != null)
                {
                    this.Shell.CycleNavigator.UnsubscribePhysicalScreenCollectionChangedEvent();
                }

                this.SetProperty(ref this.currentProject, value, () => this.CurrentProject);
            }
        }

        /// <summary>
        /// Gets or sets the current media configuration.
        /// </summary>
        public MediaConfigurationDataViewModel CurrentMediaConfiguration
        {
            get
            {
                return this.currentMediaConfiguration;
            }

            set
            {
                this.SetProperty(ref this.currentMediaConfiguration, value, () => this.CurrentMediaConfiguration);
            }
        }

        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        public IProjectManager ProjectManager
        {
            get
            {
                return this.projectManager;
            }

            set
            {
                this.SetProperty(ref this.projectManager, value, () => this.ProjectManager);
            }
        }

        /// <summary>
        /// Gets or sets the list of recently used projects.
        /// </summary>
        /// <value>
        /// The list of recent projects.
        /// </value>
        [DataMember(Name = "RecentProjects")]
        public ExtendedObservableCollection<RecentProjectDataViewModel> RecentProjects
        {
            get
            {
                return this.recentProjects;
            }

            set
            {
                this.SetProperty(ref this.recentProjects, value, () => this.RecentProjects);
            }
        }

        /// <summary>
        /// Gets or sets the list of dictionary values.
        /// </summary>
        /// <value>
        /// The list of recent Infomedia config.
        /// </value>
        [DataMember(Name = "RecentDictionaryValues")]
        public ExtendedObservableCollection<DictionaryValueDataViewModel> RecentDictionaryValues
        {
            get
            {
                return this.recentDictionaryValues;
            }

            set
            {
                this.SetProperty(ref this.recentDictionaryValues, value, () => this.RecentDictionaryValues);
            }
        }

        /// <summary>
        /// Gets or sets the consistency messages of the current project.
        /// </summary>
        public ExtendedObservableCollection<ConsistencyMessageDataViewModel> ConsistencyMessages
        {
            get
            {
                return this.consistencyMessages;
            }

            set
            {
                this.SetProperty(ref this.consistencyMessages, value, () => this.ConsistencyMessages);
            }
        }

        /// <summary>
        /// Gets or sets the consistency messages of the current project.
        /// </summary>
        public ExtendedObservableCollection<ConsistencyMessageDataViewModel> CompatibilityMessages
        {
            get
            {
                return this.compatibilityMessages;
            }

            set
            {
                this.SetProperty(ref this.compatibilityMessages, value, () => this.CompatibilityMessages);
            }
        }

        /// <summary>
        /// Gets or sets the last used file dialog directories for projects, images and videos.
        /// </summary>
        [DataMember(Name = "LastUsedDirectories")]
        public Dictionary<DialogDirectoryType, string> LastUsedDirectories { get; set; }

        /// <summary>
        /// Gets or sets the currently selected Physical Screen
        /// </summary>
        public PhysicalScreenConfigDataViewModel CurrentPhysicalScreen
        {
            get
            {
                return this.currentPhysicalScreen;
            }

            set
            {
                this.SetProperty(ref this.currentPhysicalScreen, value, () => this.CurrentPhysicalScreen);
                if (value != null)
                {
                    var screenRef =
                        this.currentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                            .PhysicalScreens.SingleOrDefault(s => s.ReferenceName == value.Name.Value);
                    if (screenRef != null)
                    {
                        this.CurrentVirtualDisplayReferences = screenRef.VirtualDisplays;
                    }
                    else
                    {
                        this.CurrentVirtualDisplayReferences = null;
                    }
                }
                else
                {
                    this.CurrentVirtualDisplayReferences = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected virtual display
        /// </summary>
        public VirtualDisplayConfigDataViewModel CurrentVirtualDisplay
        {
            get
            {
                return this.currentVirtualDisplay;
            }

            set
            {
                this.SetProperty(ref this.currentVirtualDisplay, value, () => this.CurrentVirtualDisplay);
            }
        }

        /// <summary>
        /// Gets or sets the virtual display references of the current physical screen.
        /// </summary>
        public ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> CurrentVirtualDisplayReferences
        {
            get
            {
                return this.currentVirtualDisplayReferences;
            }

            set
            {
                this.SetProperty(
                    ref this.currentVirtualDisplayReferences, value, () => this.CurrentVirtualDisplayReferences);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected cycle package
        /// </summary>
        public CyclePackageConfigDataViewModel CurrentCyclePackage
        {
            get
            {
                return this.currentCyclePackage;
            }

            set
            {
                this.SetProperty(ref this.currentCyclePackage, value, () => this.CurrentCyclePackage);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected cycle
        /// </summary>
        public CycleConfigDataViewModelBase CurrentCycle
        {
            get
            {
                return this.currentCycle;
            }

            set
            {
                this.SetProperty(ref this.currentCycle, value, () => this.CurrentCycle);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected cycle item
        /// </summary>
        public SectionConfigDataViewModelBase CurrentSection
        {
            get
            {
                return this.currentSection;
            }

            set
            {
                this.SetProperty(ref this.currentSection, value, () => this.CurrentSection);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected Layout
        /// </summary>
        public LayoutConfigDataViewModelBase CurrentLayout
        {
            get
            {
                return this.currentLayout;
            }

            set
            {
                this.SetProperty(ref this.currentLayout, value, () => this.CurrentLayout);
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IMediaShell Shell { get; private set; }

        /// <summary>
        /// Gets or sets the recent media resources.
        /// </summary>
        /// <value>
        /// The recent media resources.
        /// </value>
        [DataMember(Name = "RecentMediaResources")]
        public Dictionary<Guid, ExtendedObservableCollection<ResourceInfoDataViewModel>> RecentMediaResources
        {
            get
            {
                return this.recentMediaResources;
            }

            set
            {
                this.SetProperty(ref this.recentMediaResources, value, () => this.RecentMediaResources);
                this.RaisePropertyChanged(() => this.CurrentProjectRecentMediaResources);
            }
        }

        /// <summary>
        /// Gets the recent media resources for the current project.
        /// </summary>
        public ExtendedObservableCollection<ResourceInfoDataViewModel> CurrentProjectRecentMediaResources
        {
            get
            {
                if (this.CurrentProject == null)
                {
                    return null;
                }

                if (!this.RecentMediaResources.ContainsKey(this.CurrentProject.ProjectId))
                {
                    this.RecentMediaResources.Add(
                        this.CurrentProject.ProjectId, new ExtendedObservableCollection<ResourceInfoDataViewModel>());
                }

                return this.RecentMediaResources[this.CurrentProject.ProjectId];
            }
        }

        /// <summary>
        /// Gets the system fonts.
        /// </summary>
        public Dictionary<string, List<string>> SystemFonts { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edge snap mode is enabled
        /// </summary>
        [DataMember(Name = "UseEdgeSnap")]
        public bool UseEdgeSnap
        {
            get
            {
                return this.useEdgeSnap;
            }

            set
            {
                this.SetProperty(ref this.useEdgeSnap, value, () => this.UseEdgeSnap);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is currently checking in a project as new one.
        /// </summary>
        public bool IsCheckinAs { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public void Initialize(IMediaShell shell)
        {
            this.Shell = shell;
            this.existingProjects = new ObservableCollection<MediaConfigurationDataViewModel>();
            this.AllExistingProjects = new Dictionary<int, ObservableCollection<MediaConfigurationDataViewModel>>();
            SetLocalResourceDirectory();

            this.ProjectManager = new ProjectManager(AppDataResourceProvider);
            if (this.recentDictionaryValues == null)
            {
                this.recentDictionaryValues = new ExtendedObservableCollection<DictionaryValueDataViewModel>();
            }

            foreach (var item in this.recentDictionaryValues)
            {
                item.Table.InitializeOriginalColumns();
            }

            if (this.recentProjects == null)
            {
                this.recentProjects = new ExtendedObservableCollection<RecentProjectDataViewModel>();
            }

            if (this.LastUsedDirectories == null)
            {
                this.LastUsedDirectories = new Dictionary<DialogDirectoryType, string>();
            }
            else
            {
                this.CheckDirectoriesExist(this.LastUsedDirectories);
            }

            if (this.RecentMediaResources == null)
            {
                this.RecentMediaResources =
                    new Dictionary<Guid, ExtendedObservableCollection<ResourceInfoDataViewModel>>();
            }
            else
            {
                var emptyResources = this.RecentMediaResources.Where(r => r.Value.Count == 0).ToList();
                foreach (var emptyResource in emptyResources)
                {
                    this.RecentMediaResources.Remove(emptyResource.Key);
                }
            }

            if (this.ConsistencyMessages == null)
            {
                this.ConsistencyMessages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            }

            if (this.CompatibilityMessages == null)
            {
                this.CompatibilityMessages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            }

            this.defaultMasterLayout = new MasterLayout
            {
                Name = "Normal",
                Columns = "*",
                HorizontalGaps = "0",
                Rows = "*",
                VerticalGaps = "0"
            };
            this.InitializeFonts();
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Clears the <see cref="IDirty.IsDirty"/> flag on the <see cref="CurrentProject"/>.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.CurrentProject != null)
            {
                this.CurrentProject.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// Confirms that the tenant can be changed.
        /// </summary>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="continuation">
        /// The continuation.
        /// </param>
        /// <remarks>
        /// If not overridden, this method always returns <c>true</c>.
        /// </remarks>
        protected async override void ConfirmTenantChange(TenantReadableModel tenant, Action continuation)
        {
            var controller =
                ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.ProjectController;
            if (this.CurrentProject == null)
            {
                continuation();
                return;
            }

            var success = await controller.EnsureCanExitAsync();
            if (!success)
            {
                return;
            }

            if (this.CurrentProjectState == ProjectStates.Saved)
            {
                MessageBox.Show(
                    MediaStrings.Project_SavedLocallyMessage,
                    MediaStrings.Project_SavedLocallyTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }

            continuation();
            InteractionAction.SkipNextMouseUp = true;
        }

        private static void SetLocalResourceDirectory()
        {
            try
            {
                var mediaConfiguration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
                if (mediaConfiguration == null)
                {
                    return;
                }

                var localResourceRootPath = mediaConfiguration.ResourceSettings.LocalResourcePath;
                if (localResourceRootPath
                    != Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
                {
                    var resourceDirectory = Path.Combine(localResourceRootPath, "Resources");
                    AppDataResourceProvider.SetResourceDirectory(resourceDirectory);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while setting the custom resource directory root.", exception);
            }
        }

        private void InitializeFonts()
        {
            try
            {
                Logger.Trace("Initializing the system fonts dictionary.");
                var systemFontsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                if (string.IsNullOrEmpty(systemFontsDirectory))
                {
                    Logger.Warn("The environment fonts folder could not be found. Special fonts can't be copied");
                    return;
                }

                var systemFontFiles = Directory.GetFiles(systemFontsDirectory);
                this.SystemFonts = new Dictionary<string, List<string>>();

                foreach (var systemFontFileName in systemFontFiles)
                {
                    var systemFontFamilies = Fonts.GetFontFamilies(systemFontFileName);
                    var family = systemFontFamilies.FirstOrDefault();

                    if (family != null && family.FamilyNames.Count > 0)
                    {
                        var name = family.FamilyNames.First().Value;
                        if (this.SystemFonts.ContainsKey(name))
                        {
                            this.SystemFonts[name].Add(systemFontFileName);
                        }
                        else
                        {
                            this.SystemFonts[name] = new List<string> { systemFontFileName };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WarnException(
                    "Error during system font initialization."
                    + " Fonts not already installed on the TFT may not be copied when exporting.",
                    e);
            }
        }

        private void CheckDirectoriesExist(Dictionary<DialogDirectoryType, string> lastUsedDirectories)
        {
            var entriesToRemove = new List<KeyValuePair<DialogDirectoryType, string>>();
            foreach (var lastUsedDirectory in lastUsedDirectories)
            {
                if (!Directory.Exists(lastUsedDirectory.Value))
                {
                    Logger.Warn("Could not find last used directory path '{0}'. Removing entry.");
                    entriesToRemove.Add(lastUsedDirectory);
                }
            }

            foreach (var entry in entriesToRemove)
            {
                lastUsedDirectories.Remove(entry.Key);
            }
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                if (this.IsDirty)
                {
                    var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                    await controller.ShellController.ProjectController.OnProjectGotDirty();
                }
            }
            else if (e.PropertyName == "CurrentTenant")
            {
                var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                this.CurrentProject = null;
                if (this.CurrentTenant != null)
                {
                    if (this.AllExistingProjects.ContainsKey(this.CurrentTenant.Id))
                    {
                        this.ExistingProjects = this.AllExistingProjects[this.CurrentTenant.Id];
                    }
                    else
                    {
                        controller.ProjectsLoaded += (o, args) =>
                            {
                                if (this.AllExistingProjects.ContainsKey(this.CurrentTenant.Id))
                                {
                                    this.ExistingProjects = this.AllExistingProjects[this.CurrentTenant.Id];
                                }
                            };
                    }

                    if (!this.IsCheckinAs)
                    {
                        controller.ShellController.ProjectController.ClearCurrentProjectSettings();
                        var recentProject =
                             this.RecentProjects.FirstOrDefault(
                                 p =>
                                 !p.IsCheckedIn && p.TenantId == this.CurrentTenant.Id
                                 && p.ServerName.Equals(this.LastServer, StringComparison.InvariantCultureIgnoreCase));
                        if (recentProject != null)
                        {
                            this.Shell.BusyContentTextFormat = MediaStrings.Shell_LoadingPendingProject;
                            this.Shell.IsBusy = true;
                            this.Shell.IsBusyIndeterminate = true;
                            controller.ShellController.ProjectController.OpenLocalProjectAsync(
                                recentProject.ProjectName);
                            this.Shell.ClearBusy();
                        }
                        else
                        {
                            if (!this.IsCheckingIn && this.IsExistingProjectsLoaded)
                            {
                                var parameters = new MenuNavigationParameters
                                {
                                    Root = MenuNavigationParameters.MainMenuEntries.FileOpen
                                };
                                this.Shell.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                                    .Execute(parameters);
                            }
                        }
                    }

                    await controller.GetExistingUpdateGroupsAsync();
                }
            }
            else if (e.PropertyName == "CurrentProject" && this.currentProject == null)
            {
                this.Shell.ClearProjectTitle();
                this.ClearDirty();
                this.Shell.ClearBusy();
            }
        }
    }
}