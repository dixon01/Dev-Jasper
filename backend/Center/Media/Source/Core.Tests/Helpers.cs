// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Moq;

    using MediaConfiguration = Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration;

    /// <summary>
    /// Defines helper methods.
    /// </summary>
    public static class Helpers
    {
        private static readonly DateTime UtcNow = new DateTime(2014, 2, 25, 13, 34, 12, DateTimeKind.Utc);

        /// <summary>
        /// The create test infomedia config.
        /// </summary>
        /// <returns>
        /// The <see cref="InfomediaConfigDataModel"/>.
        /// </returns>
        public static InfomediaConfigDataModel CreateTestInfomediaConfigDataModel()
        {
            return CreateSampleInfomediaConfig();
        }

        /// <summary>
        /// The mock application state.
        /// </summary>
        /// <param name="unityContainer">
        /// The unity Container.
        /// </param>
        /// <returns>
        /// The <see cref="MediaApplicationState"/>.
        /// </returns>
        public static MediaApplicationState MockApplicationState(UnityContainer unityContainer = null)
        {
            var writableFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem);
            var mediaShellParams = new MediaShell.MediaShellParams
                                       {
                                           Factory = new MediaShellFactory(),
                                           Stages = new List<Lazy<IStage, IStageMetadata>>(),
                                           MenuItems = new List<Lazy<MenuItemBase, IMenuItemMetadata>>(),
                                           StatusBarItems = new List<Lazy<StatusBarItemBase>>()
                                       };
            var commandRegistry = new CommandRegistry();
            var shell = new MediaShell(mediaShellParams, commandRegistry);

            var applicationState = new MediaApplicationState();
            applicationState.Initialize(shell);
            if (unityContainer != null)
            {
                unityContainer.RegisterInstance(typeof(IMediaApplicationState), applicationState);
                unityContainer.RegisterInstance(typeof(IApplicationState), applicationState);
            }

            // ReSharper disable UnusedVariable
            var controller = new MediaShellController(applicationState.Shell, commandRegistry);

            // ReSharper restore UnusedVariable
            applicationState.CurrentProject = new MediaProjectDataViewModel();
            applicationState.CurrentProject.Name = "TestProject";
            applicationState.CurrentProject.InfomediaConfig = new InfomediaConfigDataViewModel(null);
            applicationState.CurrentProject.Resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            applicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.Add(
                new MasterLayoutConfigDataViewModel(shell));

            AddPhysicalScreenToProject(shell, 1024, 748);

            applicationState.CurrentLayout = applicationState.CurrentProject.InfomediaConfig.Layouts.First();
            applicationState.CurrentPhysicalScreen =
                applicationState.CurrentProject.InfomediaConfig.PhysicalScreens.First();
            applicationState.CurrentVirtualDisplay =
                applicationState.CurrentProject.InfomediaConfig.VirtualDisplays.First();
            applicationState.CurrentCyclePackage =
                applicationState.CurrentProject.InfomediaConfig.CyclePackages.First();
            applicationState.CurrentCycle =
                applicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles.First();
            applicationState.CurrentSection = applicationState.CurrentCycle.Sections.First();
            return applicationState;
        }

        /// <summary>
        /// The create media configuration.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="localResourcePath">
        /// The local Resource Path.
        /// </param>
        public static void CreateMediaConfiguration(UnityContainer container, string localResourcePath)
        {
            var resourceSettings = new ResourceSettings
            {
                MaxUsedDiskSpace = 100,
                MinRemainingDiskSpace = 1000,
                RemoveLocalResourceAfter = TimeSpan.FromMinutes(1),
                LocalResourcePath = localResourcePath
            };
            var mediaConfiguration = new Core.Configuration.MediaConfiguration { ResourceSettings = resourceSettings };
            container.RegisterInstance(typeof(Core.Configuration.MediaConfiguration), mediaConfiguration);
        }

        /// <summary>
        /// The initialize editor.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="TftEditorViewModel"/>.
        /// </returns>
        public static TftEditorViewModel InitializeEditorContent(MediaApplicationState state)
        {
            var layout = state.CurrentLayout as LayoutConfigDataViewModel;
            if (layout == null)
            {
                throw new NullReferenceException("Should not happen");
            }

            var screen = state.CurrentPhysicalScreen;
            var resolution = layout.IndexedResolutions[screen.Width.Value, screen.Height.Value];

            state.Shell.Editor.Elements.Clear();
            foreach (var element in resolution.Elements.Where(e => e is GraphicalElementDataViewModelBase))
            {
                state.Shell.Editor.Elements.Add((GraphicalElementDataViewModelBase)element);
            }

            return (TftEditorViewModel)state.Shell.Editor;
        }

        /// <summary>
        /// The add physical screen to project.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public static void AddPhysicalScreenToProject(IMediaShell shell, int width, int height)
        {
            var project = shell.MediaApplicationState.CurrentProject;
            var physicalScreensCount = project.InfomediaConfig.PhysicalScreens.Count;
            var physicalScreen = CreatePhysicalScreenConfigDataViewModel(
                shell,
                width,
                height,
                string.Format("Screen{0}x{1}_{2}", width, height, physicalScreensCount));
            var virtualDisplayName = string.Format("VD{0}x{1}_{2}", width, height, physicalScreensCount);
            var virtualDisplay = CreateVirtualDisplayConfigDataViewModel(shell, width, height, virtualDisplayName);
            var cyclePackageName = string.Format("CP{0}x{1}_{2}", width, height, physicalScreensCount);
            var cyclePackage = CreateCyclePackageConfigDataViewModel(shell, cyclePackageName);
            var cycleName = string.Format("C{0}x{1}_{2}", width, height, physicalScreensCount);
            var cycle = CreateStandardCycleConfigDataViewModel(shell, cycleName);
            var sectionName = string.Format("S{0}x{1}_{2}", width, height, physicalScreensCount);
            var section = new StandardSectionConfigDataViewModel(shell) { Name = sectionName };
            cycle.Sections.Add(section);
            cycle.CyclePackageReferences.Add(cyclePackage);
            var layoutName = string.Format("L{0}x{1}_{2}", width, height, physicalScreensCount);
            var resolution = CreateResolutionConfig(shell, width, height);
            var layout = new LayoutConfigDataViewModel(shell)
            {
                Name = { Value = layoutName },
            };
            layout.Resolutions.Add(resolution);
            layout.CycleSectionReferences.Add(new LayoutCycleSectionRefDataViewModel(cycle, section));
            cycle.Sections[0].Layout = layout;
            var cycleRef = new StandardCycleRefConfigDataViewModel(shell) { Reference = cycle };
            cyclePackage.StandardCycles.Add(cycleRef);
            virtualDisplay.CyclePackage = cyclePackage;
            var virtualDisplayRef = new VirtualDisplayRefConfigDataViewModel(shell) { Reference = virtualDisplay, };
            var physicalScreenRef = new PhysicalScreenRefConfigDataViewModel(shell) { Reference = physicalScreen };
            physicalScreenRef.VirtualDisplays.Add(virtualDisplayRef);
            project.InfomediaConfig.PhysicalScreens.Add(physicalScreen);
            project.InfomediaConfig.VirtualDisplays.Add(virtualDisplay);
            project.InfomediaConfig.CyclePackages.Add(cyclePackage);
            project.InfomediaConfig.Cycles.StandardCycles.Add(cycle);
            project.InfomediaConfig.Layouts.Add(layout);
            project.InfomediaConfig.MasterPresentation.MasterLayouts[0].PhysicalScreens.Add(physicalScreenRef);
        }

        /// <summary>
        /// Creates an IMediaShell mock which contains a dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The media shell mock.
        /// </returns>
        public static Mock<IMediaShell> CreateMediaShell(DictionaryDataViewModel dictionary = null)
        {
            if (dictionary == null)
            {
                dictionary = new DictionaryDataViewModel(new Dictionary());
            }

            var mock = new Mock<IMediaShell>();
            mock.SetupGet(s => s.Dictionary).Returns(dictionary);
            return mock;
        }

        /// <summary>
        /// The reset service locator.
        /// </summary>
        public static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        /// <summary>
        /// The initialize service locator.
        /// </summary>
        /// <returns>
        /// The <see cref="UnityContainer"/>.
        /// </returns>
        public static UnityContainer InitializeServiceLocator()
        {
            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);

            return unityContainer;
        }

        /// <summary>
        /// Creates a connection controller mock.
        /// </summary>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IConnectionController> CreateConnectionControllerMock()
        {
            var queryResult = new TaskCompletionSource<IEnumerable<ResourceReadableModel>>();
            var resources = new List<ResourceReadableModel>();
            queryResult.SetResult(resources);

            var connectionControllerMock = new Mock<IConnectionController>();
            connectionControllerMock.Setup(c => c.ResourceChangeTrackingManager.QueryAsync(null))
                .Returns(queryResult.Task);
            connectionControllerMock.Setup(c => c.DocumentVersionChangeTrackingManager.Create())
                .Returns(new DocumentVersionWritableModel());
            connectionControllerMock.Setup(c => c.UserChangeTrackingManager.Wrap(It.IsAny<User>()))
                .Returns(new UserReadableModelMock(new User { FirstName = "TestUser" }));
            connectionControllerMock.Setup(
                c => c.DocumentVersionChangeTrackingManager.AddAsync(It.IsAny<DocumentVersionWritableModel>()))
                .Returns(Task.FromResult(0));
            return connectionControllerMock;
        }

        /// <summary>
        /// Creates a complete mock of the connection controller including mocks for all change tracking managers.
        /// </summary>
        /// <returns>
        /// The <see cref="ConnectionControllerMock"/>.
        /// </returns>
        public static ConnectionControllerMock CreateCompleteConnectionControllerMock()
        {
            return new ConnectionControllerMock();
        }

        /// <summary>
        /// Creates a media application mock.
        /// </summary>
        /// <param name="unityContainer">
        /// The unity container.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="mediaShellControllerMock">
        /// The media shell controller mock.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="connectionControllerMock">
        /// The connection controller mock.
        /// </param>
        /// <param name="createProjectController">
        /// If set, creates a project controller and assigns it to the media application controller.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IMediaApplicationController> CreateMediaApplicationMock(
            UnityContainer unityContainer,
            MediaApplicationState state,
            Mock<IMediaShellController> mediaShellControllerMock,
            ICommandRegistry commandRegistry,
            Mock<IConnectionController> connectionControllerMock = null,
            bool createProjectController = true)
        {
            var mainMenuPrompt = new MainMenuPrompt(state.Shell, commandRegistry);

            var mediaApplicationControllerMock = new Mock<IMediaApplicationController>();
            if (createProjectController)
            {
                var projectController = new ProjectController(
                    state.Shell,
                    mediaShellControllerMock.Object,
                    mainMenuPrompt,
                    commandRegistry);
                mediaApplicationControllerMock.Setup(c => c.ShellController.ProjectController)
                    .Returns(projectController);
                projectController.Initialize();
                mediaShellControllerMock.Setup(c => c.ParentController).Returns(mediaApplicationControllerMock.Object);
            }
            else
            {
                mediaApplicationControllerMock.Setup(c => c.ShellController.ProjectController.OnProjectGotDirty())
                    .Returns(Task.FromResult(0));
            }

            if (connectionControllerMock != null)
            {
                mediaApplicationControllerMock.Setup(c => c.ConnectionController)
                    .Returns(connectionControllerMock.Object);
            }

            var permissionControllerMock = new Mock<IPermissionController>();
            permissionControllerMock.Setup(p => p.HasPermission(
                It.IsAny<Permission>(),
                DataScope.MediaConfiguration)).Returns(true);
            permissionControllerMock.Setup(p => p.HasPermission(It.IsAny<Permission>(), It.IsAny<DataScope>()))
                .Returns(true);
            permissionControllerMock.Setup(
                p => p.HasPermission(It.IsAny<TenantReadableModel>(), It.IsAny<Permission>(), It.IsAny<DataScope>()))
                .Returns(true);
            permissionControllerMock.Setup(p => p.PermissionTrap(
                It.IsAny<Permission>(),
                DataScope.MediaConfiguration)).Returns(true);
            mediaShellControllerMock.Setup(controller => controller.ParentController)
                .Returns(mediaApplicationControllerMock.Object);
            mediaApplicationControllerMock.Setup(controller => controller.PermissionController)
                .Returns(permissionControllerMock.Object);
            mediaApplicationControllerMock.Setup(controller => controller.GetExistingUpdateGroupsAsync())
                .Returns(Task.FromResult(true));
            unityContainer.RegisterInstance(typeof(IMediaApplicationController), mediaApplicationControllerMock.Object);
            unityContainer.RegisterInstance(typeof(IApplicationController), mediaApplicationControllerMock.Object);
            return mediaApplicationControllerMock;
        }

        /// <summary>
        /// Creates the Infomedia config.
        /// </summary>
        /// <param name="shellMockObject">
        /// The shell Mock Object.
        /// </param>
        /// <returns>
        /// A data model with the default structure for an Infomedia config.
        /// </returns>
        public static InfomediaConfigDataViewModel CreateDefaultInfomediaConfig(IMediaShell shellMockObject = null)
        {
            if (shellMockObject == null)
            {
                var shellMock = CreateMediaShell();
                shellMock.Setup(shell => shell.MediaApplicationState.CurrentProject.Resources)
                    .Returns(new ExtendedObservableCollection<ResourceInfoDataViewModel>());
                shellMockObject = shellMock.Object;
            }

            var config = new InfomediaConfigDataViewModel(shellMockObject)
            {
                CreationDate = new DataValue<DateTime>(UtcNow),
                Version = new DataValue<Version>(new Version(2, 0))
            };
            var physicalScreen = CreateDefaultPhysicalScreenConfigDataViewModel(shellMockObject);
            config.PhysicalScreens.Add(physicalScreen);
            var tftPackage = CreateDefaultCyclePackageConfigDataViewModel();
            config.CyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel> { tftPackage };
            var virtualDisplay = CreateDefaultVirtualDisplayConfigDataViewModel(shellMockObject);
            virtualDisplay.CyclePackage = tftPackage;
            config.VirtualDisplays.Add(virtualDisplay);
            config.MasterPresentation = CreateMasterPresentation(
                physicalScreen,
                virtualDisplay,
                shellMockObject);
            var defaultStandardCycleConfigDvm = CreateDefaultStandardCycleConfigDataViewModel(shellMockObject);
            defaultStandardCycleConfigDvm.CyclePackageReferences.Add(tftPackage);
            config.Cycles.StandardCycles.Add(defaultStandardCycleConfigDvm);
            tftPackage.StandardCycles.Add(
                new StandardCycleRefConfigDataViewModel(null)
                {
                    Reference = defaultStandardCycleConfigDvm
                });

            var resolutionConfig = CreateResolutionConfig();
            var testCycleLayout = new LayoutConfigDataViewModel(shellMockObject)
                {
                    Name = new DataValue<string>("Test cycle")
                };
            testCycleLayout.Resolutions = new ExtendedObservableCollection<ResolutionConfigDataViewModel>
                {
                    resolutionConfig
                };
            config.Layouts = new ExtendedObservableCollection<LayoutConfigDataViewModel> { testCycleLayout };
            return config;
        }

        /// <summary>
        /// Creates a resource manager mock and registers it on the container.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IResourceManager> CreateResourceManagerMock(UnityContainer container)
        {
            var managerMock = new Mock<IResourceManager>();
            container.RegisterInstance(typeof(IResourceManager), managerMock.Object);
            return managerMock;
        }

        private static ResolutionConfigDataViewModel CreateResolutionConfig()
        {
            var resolutionConfigDataViewModel = new ResolutionConfigDataViewModel(null)
            {
                Height = new DataValue<int>(768),
                Width = new DataValue<int>(1368)
            };
            resolutionConfigDataViewModel.Elements.Add(
                new StaticTextElementDataViewModel(null)
                {
                    Value = new AnimatedDynamicDataValue<string>("Static text")
                });
            return resolutionConfigDataViewModel;
        }

        private static StandardCycleConfigDataViewModel CreateDefaultStandardCycleConfigDataViewModel(IMediaShell shell)
        {
            return new StandardCycleConfigDataViewModel(shell)
            {
                Enabled = new DynamicDataValue<bool>(true),
                Name = new DataValue<string>("Test cycle"),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                        {
                            new StandardSectionConfigDataViewModel(shell)
                                {
                                    Duration = new DataValue<TimeSpan>(TimeSpan.FromSeconds(10))
                                }
                        }
            };
        }

        private static VirtualDisplayConfigDataViewModel CreateDefaultVirtualDisplayConfigDataViewModel(
          IMediaShell shell)
        {
            var virtualDisplay = new VirtualDisplayConfigDataViewModel(shell)
            {
                Height = new DataValue<int>(768),
                Name =
                    new DataValue<string>("TftDisplay"),
                Width = new DataValue<int>(1368)
            };
            return virtualDisplay;
        }

        private static CyclePackageConfigDataViewModel CreateDefaultCyclePackageConfigDataViewModel()
        {
            var tftPackage = new CyclePackageConfigDataViewModel(null)
            {
                Name = new DataValue<string>("TftPackage")
            };
            return tftPackage;
        }

        private static PhysicalScreenConfigDataViewModel CreateDefaultPhysicalScreenConfigDataViewModel(
            IMediaShell shell)
        {
            var physicalScreen = new PhysicalScreenConfigDataViewModel(shell)
            {
                Height = new DataValue<int>(768),
                Identifier = new DataValue<string>("0"),
                Name = new DataValue<string>("TftScreen"),
                Type = new DataValue<PhysicalScreenType>(PhysicalScreenType.TFT),
                Visible = new AnimatedDynamicDataValue<bool>(true),
                Width = new DataValue<int>(1368)
            };
            return physicalScreen;
        }

        private static MasterPresentationConfigDataViewModel CreateMasterPresentation(
            PhysicalScreenConfigDataViewModel physicalScreen,
            VirtualDisplayConfigDataViewModel virtualDisplay,
            IMediaShell mediaShellMock)
        {
            var masterSection = new MasterSectionConfigDataViewModel(mediaShellMock)
            {
                Duration =
                    new DataValue<TimeSpan>(
                    TimeSpan.FromSeconds(86400))
            };
            var sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase> { masterSection };
            var masterCycleConfigDataViewModel = new MasterCycleConfigDataViewModel(null)
            {
                Name =
                    new DataValue<string>("Master cycle"),
                    Sections = sections
            };
            var virtualDisplayRef = new VirtualDisplayRefConfigDataViewModel(null) { Reference = virtualDisplay };
            var virtualDisplays = new ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>
                                      {
                                          virtualDisplayRef
                                      };
            var physicalScreenRef = new PhysicalScreenRefConfigDataViewModel(null)
            {
                Reference = physicalScreen,
                VirtualDisplays = virtualDisplays
            };
            var physicalScreens = new ExtendedObservableCollection<PhysicalScreenRefConfigDataViewModel>
                                      {
                                          physicalScreenRef
                                      };
            var masterLayoutConfigDataViewModel = new MasterLayoutConfigDataViewModel(null)
            {
                Name = new DataValue<string>("StandardMasterLayout"),
                PhysicalScreens = physicalScreens
            };
            var masterCycles = new ExtendedObservableCollection<MasterCycleConfigDataViewModel>
                                   {
                                       masterCycleConfigDataViewModel
                                   };
            var masterLayouts = new ExtendedObservableCollection<MasterLayoutConfigDataViewModel>
                                    {
                                        masterLayoutConfigDataViewModel
                                    };
            var masterPresentation = new MasterPresentationConfigDataViewModel(null)
            {
                MasterCycles = masterCycles,
                MasterLayouts = masterLayouts
            };
            masterSection.Layout = masterLayoutConfigDataViewModel;
            return masterPresentation;
        }

        private static MasterCycleConfigDataModel CreateMasterCycleConfigDataModel(
            MasterSectionConfigDataModel masterSection)
        {
            var masterCycleConfigDataViewModel = new MasterCycleConfigDataModel
            {
                Name = "Master cycle",
                Sections = new List<SectionConfigDataModelBase> { masterSection }
            };
            return masterCycleConfigDataViewModel;
        }

        private static MasterSectionConfigDataModel CreateMasterSectionConfigDataModel()
        {
            var masterSection = new MasterSectionConfigDataModel
            {
                Duration = TimeSpan.FromSeconds(86400)
            };
            return masterSection;
        }

        private static MasterLayoutConfigDataModel CreateMasterLayoutConfigDataModel(
            PhysicalScreenRefConfigDataModel physicalScreenRef)
        {
            var masterLayoutConfigDataViewModel = new MasterLayoutConfigDataModel
            {
                Name = "StandardMasterLayout",
                PhysicalScreens = new List<PhysicalScreenRefConfigDataModel>
                        {
                            physicalScreenRef
                        }
            };
            return masterLayoutConfigDataViewModel;
        }

        private static PhysicalScreenRefConfigDataModel CreatePhysicalScreenRefConfigDataModel(
            VirtualDisplayRefConfigDataModel virtualDisplayRef, PhysicalScreenConfigDataModel physicalScreen)
        {
            var physicalScreenRef = new PhysicalScreenRefConfigDataModel
            {
                Reference = physicalScreen.Name,
                VirtualDisplays =
                    new List<VirtualDisplayRefConfigDataModel> { virtualDisplayRef }
            };
            return physicalScreenRef;
        }

        private static LayoutConfigDataModel CreateLayoutConfigDataModel()
        {
            var testCycleLayout = new LayoutConfigDataModel
            {
                Name = "Test Layout 1",
                DisplayText = "Test Layout 1 DisplayText",
            };
            return testCycleLayout;
        }

        private static ResolutionConfigDataModel CreateDefaultResolutionConfigDataModel()
        {
            var resolutionConfig = new ResolutionConfigDataModel
            {
                Height = 768,
                Width = 1368,
            };
            resolutionConfig.Elements.Add(new ImageElementDataModel
            {
                ElementName = "initially Loaded Element",
                Filename = @"Images\Background.png",
                Width = 1368,
                Height = 768,
                X = 0,
                Y = 0,
                ZIndex = -10
            });
            return resolutionConfig;
        }

        private static CyclePackageConfigDataModel CreateDefaultCyclePackageConfigDataModel()
        {
            var tftPackage = new CyclePackageConfigDataModel
            {
                Name = "TftPackage"
            };
            return tftPackage;
        }

        private static StandardCycleConfigDataModel CreateDefaultStandardCycleConfigDataModel()
        {
            return new StandardCycleConfigDataModel
            {
                Enabled = true,
                Name = "Test cycle",
                Sections =
                    new List<SectionConfigDataModelBase>
                                   {
                                       new StandardSectionConfigDataModel
                                           {
                                               Duration = TimeSpan.FromSeconds(10)
                                           }
                                   }
            };
        }

        private static EventCycleConfigDataModel CreateDefaultEventCycleConfigDataModel()
        {
            var cycle = new EventCycleConfigDataModel
            {
                Name = "Event Cycle",
                Trigger = new GenericTriggerConfigDataModel()
            };
            cycle.Trigger.Coordinates.Add(new GenericEvalDataModel());

            return cycle;
        }

        private static VirtualDisplayConfigDataModel CreateDefaultVirtualDisplayConfigDataModel()
        {
            var virtualDisplay = new VirtualDisplayConfigDataModel
            {
                Height = 768,
                Name = "TftDisplay",
                Width = 1368,
                DisplayText = "Fullscreen",
                CyclePackage = "TftPackage",
            };
            return virtualDisplay;
        }

        private static PhysicalScreenConfigDataModel CreatePhysicalScreen()
        {
            var physicalScreen = new PhysicalScreenConfigDataModel
            {
                Height = 768,
                Identifier = "0",
                Name = "TftScreen",
                DisplayText = "1368x768",
                Type = PhysicalScreenType.TFT,
                Visible = true,
                Width = 1368
            };
            return physicalScreen;
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Unit Test Code.")]
        private static InfomediaConfigDataModel CreateSampleInfomediaConfig()
        {
            var config = new InfomediaConfigDataModel
            {
                CreationDate = UtcNow,
                Version = new Version(2, 0)
            };

            var physicalScreen = CreatePhysicalScreen();
            config.PhysicalScreens.Add(physicalScreen);

            var tftPackage = CreateDefaultCyclePackageConfigDataModel();
            config.CyclePackages = new List<CyclePackageConfigDataModel> { tftPackage };
            var virtualDisplay = CreateDefaultVirtualDisplayConfigDataModel();
            config.VirtualDisplays.Add(virtualDisplay);
            virtualDisplay.CyclePackage = tftPackage.Name;

            var masterSection = CreateMasterSectionConfigDataModel();
            var masterCycleConfigDataViewModel = CreateMasterCycleConfigDataModel(masterSection);

            var virtualDisplayRef = new VirtualDisplayRefConfigDataModel
            {
                Reference = virtualDisplay.Name
            };
            var physicalScreenRef = CreatePhysicalScreenRefConfigDataModel(virtualDisplayRef, physicalScreen);
            var masterLayoutConfigDataViewModel = CreateMasterLayoutConfigDataModel(physicalScreenRef);
            masterSection.Layout = masterLayoutConfigDataViewModel.Name;
            var masterPresentation = new MasterPresentationConfigDataModel
            {
                MasterCycles =
                    new List<MasterCycleConfigDataModel>
                            {
                                masterCycleConfigDataViewModel
                            },
                MasterLayouts =
                    new List<MasterLayoutConfigDataModel>
                            {
                                masterLayoutConfigDataViewModel
                            }
            };
            config.MasterPresentation = masterPresentation;
            var cycleConfigDataModel = new CyclesConfigDataModel();
            config.Cycles = cycleConfigDataModel;

            var defaultStandardCycleConfigDataViewModel = CreateDefaultStandardCycleConfigDataModel();
            config.Cycles.StandardCycles.Add(defaultStandardCycleConfigDataViewModel);
            tftPackage.StandardCycles.Add(
                new StandardCycleRefConfigDataModel
                {
                    Reference = defaultStandardCycleConfigDataViewModel.Name
                });

            var defaultEventCycleConfigDataViewModel = CreateDefaultEventCycleConfigDataModel();
            config.Cycles.EventCycles.Add(defaultEventCycleConfigDataViewModel);
            config.CyclePackages[0].EventCycles.Add(new EventCycleRefConfigDataModel
            {
                Reference = defaultEventCycleConfigDataViewModel.Name
            });

            var testCycleLayout = CreateLayoutConfigDataModel();
            defaultStandardCycleConfigDataViewModel.Sections[0].Layout = testCycleLayout.Name;
            testCycleLayout.Resolutions = new List<ResolutionConfigDataModel>
                {
                    CreateDefaultResolutionConfigDataModel()
                };

            var testCycleLayout2 = new LayoutConfigDataModel
            {
                Name = "Test Layout 2",
                DisplayText = "Test Layout 2",
            };
            testCycleLayout2.Resolutions = new List<ResolutionConfigDataModel>
                {
                    CreateDefaultResolutionConfigDataModel()
                };

            var language = new LanguageDataViewModel { Name = "English", Index = 1 };
            var column = new ColumnDataViewModel { Name = "StopName", Index = 0 };
            var table = new TableDataViewModel
            {
                Columns = new ExtendedObservableCollection<ColumnDataViewModel> { column },
                Name = "Stops",
                MultiLanguage = true,
                MultiRow = true,
                Index = 2
            };
            column.Table = table;

            var dictionaryValue = new DictionaryValueElementDataModel
            {
                Column = column.ToDataModel(),
                Language = language.ToDataModel(),
                Row = 10,
                Table = table.ToDataModel()
            };
            var dynamicTextElement = new DynamicTextElementDataModel
            {
                Align = HorizontalAlignment.Center,
                DisplayText = "DynamicText",
                ElementName = "DynamicText1",
                SelectedDictionaryValue = dictionaryValue,
                TestData = "TestText",
                Visible = true
            };
            var staticTextElement = new StaticTextElementDataModel
                                        {
                                            Align = HorizontalAlignment.Center,
                                            DisplayText = "StaticText",
                                            ElementName = "StaticText1",
                                            Value = "Static",
                                            Visible = false
                                        };
            testCycleLayout2.Resolutions.First().Elements.Add(dynamicTextElement);
            testCycleLayout2.Resolutions.First().Elements.Add(staticTextElement);
            config.Layouts = new List<LayoutConfigDataModel>
                             {
                                 testCycleLayout,
                                 testCycleLayout2
                             };

            return config;
        }

        private static PhysicalScreenConfigDataViewModel CreatePhysicalScreenConfigDataViewModel(
            IMediaShell shell,
            int width,
            int height,
            string name)
        {
            var physicalScreen = new PhysicalScreenConfigDataViewModel(shell)
                                     {
                                         Height = new DataValue<int>(height),
                                         Identifier = new DataValue<string>("0"),
                                         Name = new DataValue<string>(name),
                                         Type = new DataValue<PhysicalScreenType>(PhysicalScreenType.TFT),
                                         Visible = new AnimatedDynamicDataValue<bool>(true),
                                         Width = new DataValue<int>(width)
                                     };
            return physicalScreen;
        }

        private static StandardCycleConfigDataViewModel CreateStandardCycleConfigDataViewModel(
            IMediaShell shell, string name)
        {
            return new StandardCycleConfigDataViewModel(shell)
            {
                Enabled = new DynamicDataValue<bool>(true),
                Name = new DataValue<string>(name),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                        {
                            new StandardSectionConfigDataViewModel(shell)
                                {
                                    Duration = new DataValue<TimeSpan>(TimeSpan.FromSeconds(10))
                                }
                        }
            };
        }

        private static VirtualDisplayConfigDataViewModel CreateVirtualDisplayConfigDataViewModel(
            IMediaShell shell,
            int width,
            int height,
            string name)
        {
            var virtualDisplay = new VirtualDisplayConfigDataViewModel(shell)
                                     {
                                         Height = new DataValue<int>(height),
                                         Name = new DataValue<string>(name),
                                         Width = new DataValue<int>(width)
                                     };
            return virtualDisplay;
        }

        private static ResolutionConfigDataViewModel CreateResolutionConfig(IMediaShell shell, int width, int height)
        {
            var resolutionConfigDataViewModel = new ResolutionConfigDataViewModel(shell)
                                                    {
                                                        Height = new DataValue<int>(height),
                                                        Width = new DataValue<int>(width)
                                                    };
            resolutionConfigDataViewModel.Elements.Add(
                new StaticTextElementDataViewModel(shell)
                    {
                        Value = new AnimatedDynamicDataValue<string>("Static text")
                    });
            return resolutionConfigDataViewModel;
        }

        private static CyclePackageConfigDataViewModel CreateCyclePackageConfigDataViewModel(
            IMediaShell shell,
            string name)
        {
            var tftPackage = new CyclePackageConfigDataViewModel(shell) { Name = new DataValue<string>(name) };
            return tftPackage;
        }

        /// <summary>
        /// The user readable model mock.
        /// </summary>
        internal class UserReadableModelMock : UserReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UserReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public UserReadableModelMock(User entity)
                : base(entity)
            {
                this.Populate();
            }

            /// <summary>
            /// The load reference properties async.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task LoadReferencePropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// The load navigation properties async.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task LoadNavigationPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// The media configuration readable model mock.
        /// </summary>
        internal sealed class MediaConfigurationReadableModelMock : MediaConfigurationReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MediaConfigurationReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            /// <param name="document">
            /// The document.
            /// </param>
            public MediaConfigurationReadableModelMock(MediaConfiguration entity, DocumentReadableModel document)
                : base(entity)
            {
                this.Populate();
                this.Document = document;
            }
        }

        /// <summary>
        /// The authorization readable model mock.
        /// </summary>
        internal class AuthorizationReadableModelMock : AuthorizationReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AuthorizationReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public AuthorizationReadableModelMock(Authorization entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        /// <summary>
        /// The document readable model mock.
        /// </summary>
        internal class DocumentReadableModelMock : DocumentReadableModel
        {
            private ObservableReadOnlyCollection<DocumentVersionReadableModelMock> mockedVersions;

            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public DocumentReadableModelMock(Document entity)
                : base(entity)
            {
                this.mockedVersions = new ObservableReadOnlyCollection<DocumentVersionReadableModelMock>();
                this.Populate();
            }

            /// <summary>
            /// Gets or sets the mocked document versions.
            /// </summary>
            public List<DocumentVersionReadableModelMock> MockedDocumentVersions
            {
                get
                {
                    return this.mockedVersions.ToList();
                }

                set
                {
                    this.mockedVersions = new ObservableReadOnlyCollection<DocumentVersionReadableModelMock>();
                    foreach (var documentVersionReadableModel in value)
                    {
                        this.mockedVersions.Add(documentVersionReadableModel);
                    }
                }
            }

            /// <summary>
            /// Gets the versions.
            /// </summary>
            public override IObservableReadOnlyCollection<DocumentVersionReadableModel> Versions
            {
                get
                {
                    return this.mockedVersions;
                }
            }
        }

        /// <summary>
        /// The document version readable model mock.
        /// </summary>
        internal class DocumentVersionReadableModelMock : DocumentVersionReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentVersionReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public DocumentVersionReadableModelMock(DocumentVersion entity)
                : base(entity)
            {
                this.Populate();
            }
        }
    }
}
