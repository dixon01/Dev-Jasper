// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectStateHelpers.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Helper class for project state tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers.ProjectStates
{
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.Unity;

    using Moq;

    /// <summary>
    /// Helper class for project state tests.
    /// </summary>
    public static class ProjectStateHelpers
    {
        /// <summary>
        /// Configures media for testing project related methods like save, create.
        /// </summary>
        /// <param name="unityContainer">
        /// The unity container.
        /// </param>
        /// <returns>
        /// The <see cref="SetupProjectTestResult"/>.
        /// </returns>
        public static SetupProjectTestResult SetupProjectTest(IUnityContainer unityContainer)
        {
            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Default);
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);

            var permissionControllerMock = new Mock<IPermissionController>();
            permissionControllerMock.Setup(
                controller => controller.PermissionTrap(Permission.Write, DataScope.MediaConfiguration)).Returns(true);

            var mediaShellMock = new Mock<IMediaShell>();
            mediaShellMock.Setup(
                shell => shell.Dictionary).Returns(() => new DictionaryDataViewModel(new Dictionary()));
            mediaShellMock.Setup(shell => shell.PermissionController).Returns(() => permissionControllerMock.Object);

            var commandRegistry = new CommandRegistry();
            var mainMenuPrompt = new MainMenuPrompt(mediaShellMock.Object, commandRegistry);
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            unityContainer.RegisterInstance(applicationControllerMock.Object);
            applicationControllerMock.Setup(controller => controller.PermissionController)
                .Returns(permissionControllerMock.Object);
            var projectControllerMock = new Mock<IProjectControllerContext>();
            projectControllerMock.Setup(
                controller => controller.ParentController.ParentController.InitializeLayoutEditorControllers());
            projectControllerMock.Setup(controller => controller.ConsistencyChecker.Check());
            projectControllerMock.Setup(controller => controller.UpdateRecentProjects(false, false));
            projectControllerMock.Setup(controller => controller.NotifyWrapper(It.IsAny<StatusNotification>()));

            var mediaProjectDataViewModelMock = new Mock<MediaProjectDataViewModel>();
            mediaProjectDataViewModelMock.SetupGet(model => model.IsDirty).Returns(false);
            var mediaConfiguration = CreateMediaConfiguration();
            unityContainer.RegisterInstance(mediaConfiguration);
            var projectManagerMock = new Mock<IProjectManager>(MockBehavior.Strict);
            var applicationStateMock = new Mock<IMediaApplicationState>();
            applicationStateMock.SetupGet(state => state.ProjectManager).Returns(projectManagerMock.Object);
            applicationStateMock.SetupGet(state => state.CurrentProject).Returns(mediaProjectDataViewModelMock.Object);
            applicationStateMock.SetupGet(state => state.ConsistencyMessages)
                .Returns(new ExtendedObservableCollection<ConsistencyMessageDataViewModel>());

            mediaShellMock.Setup(shell => shell.MediaApplicationState).Returns(() => applicationStateMock.Object);
            var tenant = new MockedTenant(new Tenant { Id = 1 });

            applicationStateMock.SetupGet(state => state.CurrentTenant).Returns(tenant);
            mediaShellMock.Setup(shell => shell.MediaApplicationState).Returns(() => applicationStateMock.Object);
            unityContainer.RegisterInstance(applicationStateMock.Object);
            var testingFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(testingFileSystem);
            return new SetupProjectTestResult(
                projectControllerMock,
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

        /// <summary>
        /// The struct which contains all mocks to test project related methods like save, create.
        /// </summary>
        public struct SetupProjectTestResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SetupProjectTestResult"/> struct.
            /// </summary>
            /// <param name="projectControllerMock">
            /// The project controller mock.
            /// </param>
            /// <param name="mediaShellMock">
            /// The media shell mock.
            /// </param>
            /// <param name="mainMenuPrompt">
            /// The main menu prompt.
            /// </param>
            /// <param name="commandRegistry">
            /// The command registry.
            /// </param>
            /// <param name="applicationStateMock">
            /// The application state mock.
            /// </param>
            /// <param name="projectManagerMock">
            /// The project manager mock.
            /// </param>
            /// <param name="mediaProjectDataViewModelMock">
            /// The media project data view model mock.
            /// </param>
            /// <param name="changeHistoryMock">
            /// The change history mock.
            /// </param>
            /// <param name="applicationControllerMock">
            /// The application controller mock.
            /// </param>
            /// <param name="testingFileSystem">
            /// The testing file system.
            /// </param>
            public SetupProjectTestResult(
                Mock<IProjectControllerContext> projectControllerMock,
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
                this.ProjectControllerMock = projectControllerMock;
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

            /// <summary>
            /// Gets the project controller mock.
            /// </summary>
            public Mock<IProjectControllerContext> ProjectControllerMock { get; private set; }

            /// <summary>
            /// Gets the media shell mock.
            /// </summary>
            public Mock<IMediaShell> MediaShellMock { get; private set; }

            /// <summary>
            /// Gets the main menu prompt.
            /// </summary>
            public MainMenuPrompt MainMenuPrompt { get; private set; }

            /// <summary>
            /// Gets the command registry.
            /// </summary>
            public ICommandRegistry CommandRegistry { get; private set; }

            /// <summary>
            /// Gets the application state mock.
            /// </summary>
            public Mock<IMediaApplicationState> ApplicationStateMock { get; private set; }

            /// <summary>
            /// Gets the project manager mock.
            /// </summary>
            public Mock<IProjectManager> ProjectManagerMock { get; private set; }

            /// <summary>
            /// Gets the media project data view model mock.
            /// </summary>
            public Mock<MediaProjectDataViewModel> MediaProjectDataViewModelMock { get; private set; }

            /// <summary>
            /// Gets the change history mock.
            /// </summary>
            public Mock<IChangeHistory> ChangeHistoryMock { get; private set; }

            /// <summary>
            /// Gets the application controller mock.
            /// </summary>
            public Mock<IMediaApplicationController> ApplicationControllerMock { get; private set; }

            /// <summary>
            /// Gets the testing file system.
            /// </summary>
            public TestingFileSystem TestingFileSystem { get; private set; }
        }

        /// <summary>
        /// The mocked tenant.
        /// </summary>
        internal class MockedTenant : TenantReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockedTenant"/> class.
            /// </summary>
            /// <param name="tenant">
            /// The tenant.
            /// </param>
            public MockedTenant(Tenant tenant)
                : base(tenant)
            {
                this.Populate();
            }
        }
    }
}
