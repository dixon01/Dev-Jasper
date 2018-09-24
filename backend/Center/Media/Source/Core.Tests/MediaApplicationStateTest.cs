// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaApplicationStateTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaApplicationStateSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the serialization of the <see cref="MediaApplicationState"/> object required by the
    /// <see cref="ApplicationStateManager"/>.
    /// </summary>
    [TestClass]
    public class MediaApplicationStateTest
    {
        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var writableFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem);
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var writableFileSystem = new Mock<IWritableFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem.Object);
        }

        /// <summary>
        /// Tests the serialization of the <see cref="MediaApplicationState"/>.
        /// </summary>
        [TestMethod]
        public void SerializeAndDeserializeMediaApplicationStateTest()
        {
            var utcNow = DateTime.UtcNow;
            var dataContractSerializer = new DataContractSerializer(typeof(MediaApplicationState));
            using (var stream = new MemoryStream())
            {
                var applicationState = new MediaApplicationState();
                const string Path = @"C:\Test\Test.icm";
                const string ProjectName = "Test.icm";
                const string ProjectDirectory = @"C:\Test";
                const string ImageDirectory = @"C:\Images";
                const string UserName = "TestUser";
                const string ConnectionUrl = "icenter.gorba.com";
                var project = new RecentProjectDataViewModel
                                  {
                                      FilePath = Path,
                                      LastUsed = utcNow,
                                      ProjectName = ProjectName
                                  };
                applicationState.RecentProjects.Add(project);
                applicationState.UserName = UserName;
                applicationState.RecentServers.Add(ConnectionUrl);
                var column = new ColumnDataViewModel { Name = "Column", Index = 1 };
                var table = new TableDataViewModel
                            {
                                Name = "Table",
                                Columns =
                                    new ExtendedObservableCollection<ColumnDataViewModel>
                                    {
                                        column
                                    },
                                Index = 2
                            };
                var language = new LanguageDataViewModel { Name = "English", Index = 3 };
                applicationState.RecentDictionaryValues.Add(
                    new DictionaryValueDataViewModel
                        {
                            DisplayText = "DictionaryValue",
                            Column = column,
                            Table = table,
                            Language = language,
                            Row = 10
                        });
                applicationState.LastUsedDirectories[DialogDirectoryTypes.Project] = ProjectDirectory;
                applicationState.LastUsedDirectories[DialogDirectoryTypes.Image] = ImageDirectory;
                dataContractSerializer.WriteObject(stream, applicationState);

                stream.Seek(0, SeekOrigin.Begin);

                var result = (MediaApplicationState)dataContractSerializer.ReadObject(stream);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.RecentProjects.Count);
                Assert.AreEqual(ProjectName, result.RecentProjects[0].ProjectName);
                Assert.AreEqual(Path, result.RecentProjects[0].FilePath);
                Assert.AreEqual(utcNow, result.RecentProjects[0].LastUsed);

                Assert.AreEqual(1, result.RecentDictionaryValues.Count);
                Assert.AreEqual("Column", result.RecentDictionaryValues[0].Column.Name);
                Assert.AreEqual("Table", result.RecentDictionaryValues[0].Table.Name);
                Assert.AreEqual(1, result.RecentDictionaryValues[0].Table.Columns.Count);
                Assert.AreEqual(10, result.RecentDictionaryValues[0].Row);
                Assert.AreEqual("English", result.RecentDictionaryValues[0].Language.Name);
                string lastUsedDirectory;
                Assert.IsTrue(
                    result.LastUsedDirectories.TryGetValue(DialogDirectoryTypes.Project, out lastUsedDirectory));
                Assert.AreEqual(ProjectDirectory, lastUsedDirectory);
                Assert.IsTrue(
                    result.LastUsedDirectories.TryGetValue(DialogDirectoryTypes.Image, out lastUsedDirectory));
                Assert.AreEqual(ImageDirectory, lastUsedDirectory);
                Assert.IsFalse(
                    result.LastUsedDirectories.TryGetValue(DialogDirectoryTypes.Video, out lastUsedDirectory));
                Assert.IsNull(lastUsedDirectory);
                Assert.AreEqual(UserName, result.UserName);
                Assert.AreEqual(1, result.RecentServers.Count());
                Assert.AreEqual(ConnectionUrl, result.RecentServers.First());
            }
        }

        /// <summary>
        /// Tests changing the tenant.
        /// </summary>
        [TestMethod]
        public void ChangeTenantTest()
        {
            var container = InitializeServiceLocator();
            var state = new MediaApplicationState();
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                    .Execute(It.IsAny<MenuNavigationParameters>()));

            shellMock.Setup(shell => shell.CommandRegistry).Returns(commandRegistryMock.Object);
            state.Initialize(shellMock.Object);
            var documentReadableModel = new Helpers.DocumentReadableModelMock(new Document { Name = "TestProject" });
            var mediaConfigurationReadableModel =
                new Helpers.MediaConfigurationReadableModelMock(
                    new MediaConfiguration { Id = 100, },
                    documentReadableModel);
            var mediaConfigurationDataViewModel = new MediaConfigurationDataViewModel(
                mediaConfigurationReadableModel,
                shellMock.Object,
                commandRegistryMock.Object);
            state.AllExistingProjects.Add(
                1,
                new ObservableCollection<MediaConfigurationDataViewModel> { mediaConfigurationDataViewModel });
            state.IsExistingProjectsLoaded = true;
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            var projectControllerMock = new Mock<IProjectController>();
            applicationControllerMock.Setup(controller => controller.ShellController.ProjectController)
                .Returns(projectControllerMock.Object);
            applicationControllerMock.Setup(controller => controller.GetExistingUpdateGroupsAsync())
                .Returns(Task.FromResult(true));
            container.RegisterInstance(typeof(IMediaApplicationController), applicationControllerMock.Object);
            var newTenant = new TenantReadableModelMock(new Tenant { Name = "TestTenant", Id = 1 });
            state.CurrentTenant = newTenant;
            Assert.AreEqual(1, state.ExistingProjects.Count);
            Assert.AreEqual(mediaConfigurationDataViewModel, state.ExistingProjects.First());
            Assert.AreEqual(newTenant, state.CurrentTenant);
            commandRegistryMock.Verify(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                    .Execute(It.IsAny<MenuNavigationParameters>()),
                Times.Once());
            ResetServiceLocator();
        }

        private static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        private static UnityContainer InitializeServiceLocator()
        {
            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);

            return unityContainer;
        }

        private class TenantReadableModelMock : TenantReadableModel
        {
            public TenantReadableModelMock(Tenant entity)
                : base(entity)
            {
                this.Populate();
            }
        }
    }
}