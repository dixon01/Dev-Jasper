// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="ProjectController"/>.
    /// </summary>
    [TestClass]
    public class ProjectControllerTest
    {
        private IUnityContainer unityContainer;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(this.unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
            this.unityContainer.RegisterInstance(Mock.Of<IDispatcher>());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
            ChangeHistoryFactory.ResetCurrent();
        }

        /// <summary>
        /// Tests the <see cref="ProjectController.CreateNewProjectAsync(CreateProjectParameters)"/> method.
        /// </summary>
        public void CreateNewProjectTest()
        {
            var setupProjectTest = this.SetupProjectTest();
            setupProjectTest.ProjectManagerMock.SetupSet(manager => manager.IsFileSelected = false);
            setupProjectTest.ProjectManagerMock.SetupGet(manager => manager.FullFileName).Returns(@"C:\MyTest.icm");
            var masterResolution = new ResolutionConfiguration { Height = 600, Width = 800 };
            var createParams = new CreateProjectParameters
                               {
                                   Type = PhysicalScreenType.TFT,
                                   Resolution = masterResolution
                               };
            setupProjectTest.ProjectController.CreateNewProjectAsync(createParams).Wait();

            setupProjectTest.ProjectManagerMock.VerifySet(
                manager => manager.IsFileSelected = false, Times.Once(), "'IsFlagSelected' flag not reset");
            Func<MediaProjectDataViewModel, bool> isValidReset = mp =>
                {
                    if (mp == null || mp.InfomediaConfig == null)
                    {
                        return false;
                    }

                    return mp.InfomediaConfig.Layouts.Count == 1;
                };
            setupProjectTest.ApplicationStateMock.VerifySet(
                state => state.CurrentProject = null, Times.Once(), "Current project not reset");
            setupProjectTest.ApplicationStateMock.VerifySet(
                state => state.CurrentProject = It.Is<MediaProjectDataViewModel>(model => isValidReset(model)),
                Times.Once(),
                "Current project not set to a new valid model");
            Func<MediaProjectDataViewModel, bool> isValidLayout = mp =>
                {
                    if (mp == null || mp.InfomediaConfig == null || mp.InfomediaConfig.Layouts.Count == 0)
                    {
                        return false;
                    }

                    var layout = mp.InfomediaConfig.Layouts[0];
                    if (layout.Resolutions.Count != 1)
                    {
                        return false;
                    }

                    var resolution = layout.Resolutions[0];
                    return layout.IsDirty && layout.ReferencesCount == 2 && layout.CycleSectionReferences.Count == 2
                        && resolution.Height.Value == 600 && resolution.Width.Value == 800;
                };
            setupProjectTest.ApplicationStateMock.VerifySet(
                state =>
                state.CurrentProject = It.Is<MediaProjectDataViewModel>(model => isValidLayout(model)));
            setupProjectTest.ApplicationControllerMock.Verify(
                controller => controller.InitializeLayoutEditorControllers(),
                Times.Once(),
                "InitializeLayoutEditorController not called exactly once.");
        }

        /// <summary>
        /// Tests the <see cref="ProjectController.ExecuteSaveProjectLocal"/> method.
        /// </summary>
        [TestMethod]
        public void SaveProjectLocalTest()
        {
            const string FullFileName = @"C:\MyTest.icm";
            const string Filename = "MyTest.icm";

            var setupProjectTest = this.SetupProjectTest();
            var mediaProject = new MediaProjectDataViewModel
                                   {
                                       Name = "MyTest",
                                       InfomediaConfig =
                                           new InfomediaConfigDataViewModel(
                                           setupProjectTest.MediaShellMock.Object)
                                   };

            var documentVersion = new DocumentVersion
                                      {
                                          Content =
                                              new XmlData(mediaProject.ToDataModel())
                                      };
            var document = new Document
                        {
                            Name = "MyTest",
                            Id = 1,
                            Versions = new Collection<DocumentVersion> { documentVersion }
                        };
            var mediaConfiguration =
                new MockedMediaConfigurationReadableModel(
                    new Common.ServiceModel.Configurations.MediaConfiguration { Document = document });

            var mediaConfigurationDataViewModel = new MediaConfigurationDataViewModel(
                mediaConfiguration,
                setupProjectTest.MediaShellMock.Object,
                setupProjectTest.CommandRegistry);

            var mediaProjectDataModelMock = new Mock<MediaProjectDataViewModel>();
            setupProjectTest.ProjectManagerMock.Setup(manager => manager.FullFileName).Returns(FullFileName);
            setupProjectTest.ProjectManagerMock.Setup(manager => manager.FileName).Returns(Filename);
            setupProjectTest.ApplicationStateMock.SetupGet(state => state.CurrentProject)
                            .Returns(mediaProjectDataModelMock.Object);
            setupProjectTest.ApplicationStateMock.Setup(state => state.RecentProjects)
                            .Returns(new ExtendedObservableCollection<RecentProjectDataViewModel>());
            setupProjectTest.ApplicationStateMock.Setup(state => state.ExistingProjects)
                .Returns(new ObservableCollection<MediaConfigurationDataViewModel> { mediaConfigurationDataViewModel });
            setupProjectTest.ApplicationStateMock.Setup(state => state.CurrentProject).Returns(mediaProject);
            setupProjectTest.ChangeHistoryMock.Setup(history => history.AddSaveMarker());
            setupProjectTest.TestingFileSystem.CreateDirectory(@"C:\");
            setupProjectTest.ApplicationStateMock.Setup(state => state.LastServer)
                .Returns("http://testserver.com:6091/");

            var resourceManager = new Mock<IResourceManager>();
            resourceManager.Setup(r => r.GetLocalProjectsPath()).Returns(@"C:\");
            this.unityContainer.RegisterInstance(typeof(IResourceManager), resourceManager.Object);

            Assert.IsTrue(setupProjectTest.ProjectController.ExecuteSaveProjectLocal());
            setupProjectTest.ChangeHistoryMock.Verify(
                history => history.AddSaveMarker(), Times.Once(), "Save marker not added to history");
            Assert.IsFalse(setupProjectTest.ApplicationStateMock.Object.CurrentProject.IsDirty);
            Assert.IsFalse(setupProjectTest.ApplicationStateMock.Object.IsCheckingIn);
            Assert.IsFalse(setupProjectTest.ApplicationStateMock.Object.IsDirty);
            Assert.IsFalse(setupProjectTest.ApplicationStateMock.Object.CurrentProject.IsCheckedIn);
        }

        private static MediaConfiguration CreateMediaConfiguration()
        {
            var masterLayout = new MasterLayout
            {
                Columns = "*.*",
                Name = "TestMasterLayout",
                Rows = "*",
                HorizontalGaps = "10",
                VerticalGaps = "0"
            };
            var resolution = new ResolutionConfiguration
            {
                Height = 600,
                Width = 800,
                MasterLayouts = new List<MasterLayout> { masterLayout }
            };
            var screenType = new PhysicalScreenTypeConfig
            {
                AvailableResolutions =
                    new List<ResolutionConfiguration> { resolution },
                Name = "TFT"
            };
            var physicalScreenSettings = new PhysicalScreenSettings
            {
                PhysicalScreenTypes =
                    new List<PhysicalScreenTypeConfig> { screenType }
            };

            var mediaConfiguration = new MediaConfiguration { PhysicalScreenSettings = physicalScreenSettings };
            return mediaConfiguration;
        }

        private SetupProjectTestResult SetupProjectTest()
        {
            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Default);
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);

            var permissionControllerMock = new Mock<IPermissionController>();
            permissionControllerMock.Setup(controller => controller.PermissionTrap(
                Permission.Write,
                DataScope.MediaConfiguration)).Returns(true);

            var mediaShellMock = new Mock<IMediaShell>();
            mediaShellMock.Setup(
                shell => shell.Dictionary).Returns(() => new DictionaryDataViewModel(new Dictionary()));
            mediaShellMock.Setup(
                shell => shell.PermissionController).Returns(() => permissionControllerMock.Object);

            var commandRegistry = new CommandRegistry();
            var mainMenuPrompt = new MainMenuPrompt(mediaShellMock.Object, commandRegistry);
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            this.unityContainer.RegisterInstance(applicationControllerMock.Object);
            applicationControllerMock.Setup(controller => controller.PermissionController)
                .Returns(permissionControllerMock.Object);
            var projectController = new ProjectController(
                mediaShellMock.Object,
                new MediaShellController(mediaShellMock.Object, commandRegistry)
                    {
                        ParentController = applicationControllerMock.Object
                    },
                mainMenuPrompt,
                commandRegistry);

            var mediaProjectDataViewModelMock = new Mock<MediaProjectDataViewModel>();
            mediaProjectDataViewModelMock.SetupGet(model => model.IsDirty).Returns(false);
            var mediaConfiguration = CreateMediaConfiguration();
            this.unityContainer.RegisterInstance(mediaConfiguration);
            var projectManagerMock = new Mock<IProjectManager>(MockBehavior.Strict);
            var applicationStateMock = new Mock<IMediaApplicationState>();
            applicationStateMock.SetupGet(state => state.ProjectManager).Returns(projectManagerMock.Object);
            applicationStateMock.SetupGet(state => state.CurrentProject).Returns(mediaProjectDataViewModelMock.Object);
            applicationStateMock.SetupGet(state => state.ConsistencyMessages)
                                .Returns(new ExtendedObservableCollection<ConsistencyMessageDataViewModel>());

            mediaShellMock.Setup(
                shell => shell.MediaApplicationState).Returns(() => applicationStateMock.Object);
            projectController.Initialize();

            var tenant = new MockedTenant(new Tenant { Id = 1 });

            applicationStateMock.SetupGet(state => state.CurrentTenant).Returns(tenant);
            mediaShellMock.Setup(shell => shell.MediaApplicationState).Returns(() => applicationStateMock.Object);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);
            var testingFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(testingFileSystem);
            return new SetupProjectTestResult(
                projectController,
                mediaShellMock,
                mainMenuPrompt,
                commandRegistry,
                applicationStateMock,
                projectManagerMock,
                mediaProjectDataViewModelMock,
                changeHistoryMock,
                applicationControllerMock,
                testingFileSystem);
        }

        private struct SetupProjectTestResult
        {
            public SetupProjectTestResult(
                ProjectController projectController,
                Mock<IMediaShell> mediaShellMock,
                MainMenuPrompt mainMenuPrompt,
                ICommandRegistry commandRegistry,
                Mock<IMediaApplicationState> applicationStateMock,
                Mock<IProjectManager> projectManagerMock,
                Mock<MediaProjectDataViewModel> mediaProjectDataViewModelMock,
                Mock<IChangeHistory> changeHistoryMock,
                Mock<IMediaApplicationController> applicationControllerMock,
                TestingFileSystem testingFileSystem)
                : this()
            {
                this.ProjectController = projectController;
                this.MediaShellMock = mediaShellMock;
                this.MainMenuPrompt = mainMenuPrompt;
                this.CommandRegistry = commandRegistry;
                this.ApplicationStateMock = applicationStateMock;
                this.ProjectManagerMock = projectManagerMock;
                this.MediaProjectDataViewModelMock = mediaProjectDataViewModelMock;
                this.ChangeHistoryMock = changeHistoryMock;
                this.ApplicationControllerMock = applicationControllerMock;
                TestingFileSystem = testingFileSystem;
            }

            public ProjectController ProjectController { get; private set; }

            public Mock<IMediaShell> MediaShellMock { get; private set; }

            public MainMenuPrompt MainMenuPrompt { get; private set; }

            public ICommandRegistry CommandRegistry { get; private set; }

            public Mock<IMediaApplicationState> ApplicationStateMock { get; private set; }

            public Mock<IProjectManager> ProjectManagerMock { get; private set; }

            public Mock<MediaProjectDataViewModel> MediaProjectDataViewModelMock { get; private set; }

            public Mock<IChangeHistory> ChangeHistoryMock { get; private set; }

            public Mock<IMediaApplicationController> ApplicationControllerMock { get; private set; }

            public TestingFileSystem TestingFileSystem { get; private set; }
        }

        private class MockedTenant : TenantReadableModel
        {
            public MockedTenant(Tenant tenant)
                : base(tenant)
            {
                this.Populate();
            }
        }

        private class MockedMediaConfigurationReadableModel : MediaConfigurationReadableModel
        {
            public MockedMediaConfigurationReadableModel(
                Common.ServiceModel.Configurations.MediaConfiguration mediaConfiguration)
                : base(mediaConfiguration)
            {
                this.Populate();
                this.Document = new MockedDocumentReadableModel(mediaConfiguration.Document);
            }
        }

        private class MockedDocumentReadableModel : DocumentReadableModel
        {
            public MockedDocumentReadableModel(Document document)
                : base(document)
            {
                this.Populate();
                ((ObservableReadOnlyCollection<DocumentVersionReadableModel>)this.Versions).Add(
                    new MockedDocumentVersionReadableModel(document.Versions.First()));
            }
        }

        private class MockedDocumentVersionReadableModel : DocumentVersionReadableModel
        {
            public MockedDocumentVersionReadableModel(DocumentVersion documentVersion)
                : base(documentVersion)
            {
                this.Populate();
            }
        }
    }
}