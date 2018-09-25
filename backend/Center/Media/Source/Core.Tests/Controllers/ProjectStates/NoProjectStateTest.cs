// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoProjectStateTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NoProjectStateTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers.ProjectStates
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the NoProject state.
    /// </summary>
    [TestClass]
    public class NoProjectStateTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
           Helpers.ResetServiceLocator();
           FileSystemManager.ChangeLocalFileSystem(new TestingFileSystem());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
            FileSystemManager.ChangeLocalFileSystem(new TestingFileSystem());
        }

        /// <summary>
        /// Tests the resulting state after trying to open a non existing local project.
        /// </summary>
        [TestMethod]
        public void OpenNonExistingLocalProjectTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var applicationState = Helpers.MockApplicationState(container);
            var writableFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem);

            var resourceManagerMock = new Mock<IResourceManager>();
            resourceManagerMock.Setup(m => m.GetLocalProjectsPath()).Returns("C:\\TestDir");
            var projectControllerMock = new Mock<IProjectControllerContext>();
            var state = new NoProjectState(applicationState.Shell, new CommandRegistry(), projectControllerMock.Object);
            var resultState = state.OpenLocalAsync("SomeFile").Result;
            Assert.AreSame(state, resultState);
        }

        /// <summary>
        /// Tests the resulting state after opening an existing local project.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Controllers\ProjectStates\local16.rx")]
        public void OpenExistingLocalProjectTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var applicationState = Helpers.MockApplicationState(container);
            applicationState.LastServer = "local";
            var readableDocument = new Helpers.DocumentReadableModelMock(new Document { Name = "Project", Id = 16 });
            var readableConfiguration = new Helpers.MediaConfigurationReadableModelMock(
                new MediaConfiguration(),
                readableDocument);
            applicationState.ExistingProjects.Add(
                new MediaConfigurationDataViewModel(
                    readableConfiguration,
                    applicationState.Shell,
                    new CommandRegistry()));

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);
            localFileSystem.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            var projectFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "local16.rx");
            var projectFileInfo = CopyToTestFileSystem(localFileSystem, projectFilePath);

            var projectManagerMock = new Mock<IProjectManager>();
            projectManagerMock.Setup(manager => manager.GetResourceAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Mock<IResource>().Object));
            applicationState.ProjectManager = projectManagerMock.Object;
            var resourceManagerMock = new Mock<IResourceManager>();
            resourceManagerMock.Setup(m => m.GetLocalProjectsPath())
                .Returns(Path.GetDirectoryName(projectFileInfo.FullName));
            var resourceManagerMockObject = resourceManagerMock.Object;
            container.RegisterInstance(typeof(IResourceManager), resourceManagerMockObject);
            var projectControllerMock = new Mock<IProjectControllerContext>();
            projectControllerMock.Setup(
                controller => controller.ParentController.ParentController.InitializeLayoutEditorControllers());
            projectControllerMock.Setup(controller => controller.ConsistencyChecker.Check());

            // Actual test
            var state = new NoProjectState(applicationState.Shell, new CommandRegistry(), projectControllerMock.Object);
            var resultState = state.OpenLocalAsync("Project").Result;
            Assert.AreEqual(typeof(SavedState), resultState.GetType());
            Assert.AreEqual(ProjectStates.Saved, resultState.StateInfo);
            projectControllerMock.Verify(c => c.ConsistencyChecker.Check(), Times.Once());
            projectControllerMock.Verify(c => c.UpdateRecentProjects(false, false), Times.Once());
            projectControllerMock.Verify(
                c => c.ParentController.ParentController.InitializeLayoutEditorControllers(),
                Times.Once());
            projectControllerMock.Verify(c => c.NotifyWrapper(It.IsAny<StatusNotification>()), Times.Once());
        }

        /// <summary>
        /// Tests that trying to save from NoProject state does nothing.
        /// </summary>
        [TestMethod]
        public void SaveWithoutProjectTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            var state = new NoProjectState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            Assert.AreEqual(ProjectStates.NoProject, state.StateInfo);

            var resultState = state.SaveAsync().Result;
            Assert.AreEqual(ProjectStates.NoProject, resultState.StateInfo);
        }

        /// <summary>
        /// Tests that trying to check-in from NoProject state does nothing.
        /// </summary>
        [TestMethod]
        public void CheckinWithoutProjectTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            var state = new NoProjectState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            Assert.AreEqual(ProjectStates.NoProject, state.StateInfo);

            var resultState = state.CheckInAsync().Result;
            Assert.AreEqual(ProjectStates.NoProject, resultState.StateInfo);
        }

        /// <summary>
        /// Tests that trying to "check-in as" from NoProject state does nothing.
        /// </summary>
        [TestMethod]
        public void CheckinAsWithoutProjectTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            var state = new NoProjectState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            Assert.AreEqual(ProjectStates.NoProject, state.StateInfo);

            var resultState = state.CheckInAsAsync(null).Result;
            Assert.AreEqual(ProjectStates.NoProject, resultState.StateInfo);
        }

        /// <summary>
        /// Tests the resulting state when opening a project from server.
        /// </summary>
        [TestMethod]
        public void OpenFromServerTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            projectControllerMock.Setup(
                controller => controller.ExecuteOpenProject(It.IsAny<MediaConfigurationDataViewModel>()))
                .Returns(Task.FromResult(true));
            var state = new NoProjectState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            var project =
                new Mock<MediaConfigurationDataViewModel>(
                    new Mock<MediaConfigurationReadableModel>(new MediaConfiguration()).Object,
                    shellMock.Object,
                    commandRegistryMock.Object);
            var resultState = state.OpenFromServerAsync(project.Object).Result;
            Assert.AreEqual(ProjectStates.CheckedIn, resultState.StateInfo);
        }

        private static IWritableFileInfo CopyToTestFileSystem(IWritableFileSystem fileSystem, string actualPath)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var path = Path.Combine(@"C:\", Path.GetFileName(actualPath));
            var destinationPathInfo = fileSystem.CreateFile(path);
            using (var destinationStream = destinationPathInfo.OpenWrite())
            {
                using (var sourceStream = File.OpenRead(actualPath))
                {
                    sourceStream.CopyTo(destinationStream);
                }
            }

            return destinationPathInfo;
        }
    }
}
