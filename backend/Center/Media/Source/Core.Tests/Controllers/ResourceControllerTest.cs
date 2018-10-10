// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="ResourceController"/>.
    /// </summary>
    [TestClass]
    public class ResourceControllerTest
    {
        private IUnityContainer unityContainer;

        private Mock<IDispatcher> dispatcherMock;

        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>
        /// The test context.
        /// </value>
        public TestContext TestContext { get; set; }

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
            this.dispatcherMock = new Mock<IDispatcher>();
            this.unityContainer.RegisterInstance(this.dispatcherMock.Object);
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
            this.dispatcherMock = null;
        }

        /// <summary>
        /// Test the addition of a non existing resource.
        /// This test has been ignored because it contains an async void method call which can't be tested with
        /// TFS2010
        /// </summary>
        [TestMethod]
        [Ignore]
        public void AddNonExistingResourceTest()
        {
            var projectTest = this.SetupProjectTest();
            this.dispatcherMock.Setup(d => d.Dispatch(It.IsAny<Action>()));
            const string ResourcePath = @"C:\test.jpg";
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            projectTest.MediaProjectDataViewModelMock.SetupGet(model => model.Resources).Returns(resources);
            projectTest.ProjectManagerMock.Setup(
                manager => manager.AddResource("D41D8CD98F00B204E9800998ECF8427E", ResourcePath, false));
            projectTest.ProjectManagerMock.Setup(manager => manager.GetResource(It.IsAny<string>()))
                .Returns(default(IResource));
            var file = projectTest.TestingFileSystem.CreateFile(ResourcePath);
            var parameters = new AddResourceParameters { Resources = new[] { file }, Type = ResourceType.Image };
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.AddResource)
                        .Execute(parameters);
            projectTest.ProjectManagerMock.Verify(
                manager => manager.AddResource("D41D8CD98F00B204E9800998ECF8427E", ResourcePath, false),
                Times.Once(),
                "Resource not added exactly once");
            this.dispatcherMock.Verify(d => d.Dispatch(It.IsAny<Action>()), Times.Once());
        }

        /// <summary>
        /// Test the addition of a resource which is already added.
        /// This test is ignored because of a desired popup which appears.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void AddExistingResourceTest()
        {
            var projectTest = this.SetupProjectTest();
            const string ResourcePath = @"C:\test.jpg";
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            projectTest.MediaProjectDataViewModelMock.SetupGet(model => model.Resources).Returns(resources);
            projectTest.ProjectManagerMock.Setup(
                manager => manager.AddResource("D41D8CD98F00B204E9800998ECF8427E", ResourcePath, false));
            projectTest.ProjectManagerMock.Setup(manager => manager.GetResource(It.IsAny<string>()))
                .Returns(default(IResource));

            var file = projectTest.TestingFileSystem.CreateFile(ResourcePath);
            var parameters = new AddResourceParameters { Resources = new[] { file }, Type = ResourceType.Image };
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters);
            projectTest.ProjectManagerMock.Verify(
                manager => manager.AddResource("D41D8CD98F00B204E9800998ECF8427E", ResourcePath, false),
                Times.Once(),
                "Resource not added exactly once");
            Assert.AreEqual(1, resources.Count);

            // add resource a second time
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters);
            projectTest.ProjectManagerMock.Verify(
                manager => manager.AddResource("D41D8CD98F00B204E9800998ECF8427E", ResourcePath, false),
                Times.Exactly(2),
                "Second add did not trigger correct amount of AddResource()");
            Assert.AreEqual(1, resources.Count);
        }

        /// <summary>
        /// Test the addition of a resource which has only a different extension.
        /// This test has been ignored because it contains an async void method call which can't be tested with
        /// TFS2010
        /// </summary>
        [TestMethod]
        [Ignore]
        public void AddResourceWithDifferentExtensionTest()
        {
            // test is written for font files but ResourceType.Image is used.
            // GetFontFaceName() reads the file to determine the facename which will break this test.
            // Also tests that folder are correctly created if one is at root level.
            this.AddResourceWithDifferentExtension(
                @"C:\test.FON",
                @"C:\test.FNT",
                @"C:\test (1).FNT");
            this.AddResourceWithDifferentExtension(
                @"C:\folder\test.FON",
                @"C:\folder\test.FNT",
                @"C:\folder\test (1).FNT");
            this.AddResourceWithDifferentExtension(
                @"C:\folder\test.FON",
                @"C:\test.FNT",
                @"C:\test (1).FNT");
        }

        /// <summary>
        /// Test the deletion of a used resource.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UpdateException))]
        public void DeleteUsedResourceExceptionTest()
        {
            var projectTest = this.SetupProjectTest();
            const string Hash = "D41D8CD98F00B204E9800998ECF8427E";
            var usedResource = new ResourceInfoDataViewModel { Hash = Hash, ReferencesCount = 1 };
            var viewModelMock = new Mock<DataViewModelBase>();
            usedResource.SetReference(viewModelMock.Object);
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>
                {
                    usedResource
                };
            projectTest.MediaProjectDataViewModelMock.SetupGet(model => model.Resources).Returns(resources);
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.DeleteResource).Execute(Hash);
        }

        /// <summary>
        /// The detection of the led font type.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Controllers\TestFonts\CUxFont.rx")]
        [DeploymentItem(@"Controllers\TestFonts\FonUnicodeChines.rx")]
        [DeploymentItem(@"Controllers\TestFonts\FonUnicodeHebrew.rx")]
        [DeploymentItem(@"Controllers\TestFonts\FonUnicodeArab.rx")]
        [DeploymentItem(@"Controllers\TestFonts\FonFont.rx")]
        [DeploymentItem(@"Controllers\TestFonts\FntFont.rx")]
        public void DetectLedFontTypes()
        {
            Assert.IsTrue(this.DetectLedFontType("CUxFont", LedFontType.CUxFont));
            Assert.IsTrue(this.DetectLedFontType("FonUnicodeChines", LedFontType.FonUnicodeChines));
            Assert.IsTrue(this.DetectLedFontType("FonUnicodeHebrew", LedFontType.FonUnicodeHebrew));
            Assert.IsTrue(this.DetectLedFontType("FonUnicodeArab", LedFontType.FonUnicodeArab));
            Assert.IsTrue(this.DetectLedFontType("FonFont", LedFontType.FonFont));
            Assert.IsTrue(this.DetectLedFontType("FntFont", LedFontType.FntFont));
        }

        /// <summary>
        /// The detection of the led font type with a brocken font.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Controllers\TestFonts\BrokenFont.rx")]
        [ExpectedException(typeof(FileFormatException))]
        public void DetectLedFontTypesBrokenFont()
        {
            this.DetectLedFontType("BrokenFont", LedFontType.Unknown);
        }

        private static IWritableFileInfo CopyToTestFileSystem(
            IWritableFileSystem fileSystem,
            string destinationFolder,
            string actualPath)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var path = Path.Combine(@"C:\", destinationFolder);
            fileSystem.CreateDirectory(path);
            path = Path.Combine(path, Path.GetFileName(actualPath));
            // ReSharper restore AssignNullToNotNullAttribute
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

        private static void WriteDummyContent(IWritableFileInfo file, string content)
        {
            using (var fileStream = file.OpenWrite())
            {
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(content);
                streamWriter.Close();
                fileStream.Close();
            }
        }

        private bool DetectLedFontType(string font, LedFontType expectedType)
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var shellMock = Helpers.CreateMediaShell();
            shellMock.Setup(shell => shell.MediaApplicationState).Returns(state);

            Helpers.CreateMediaConfiguration(container, "C:\\");

            // fake font file
            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            var localResources = new AppDataResourceProvider(@"Gorba\Center\Media\Resources");

            // normaly a file would have a hash as name, this way it is more readable
            var inputPath = Path.Combine(this.TestContext.DeploymentDirectory, font + ".rx");
            var testableInputPath1 = CopyToTestFileSystem(
                localFileSystem,
                Settings.Default.AppDataResourcesRelativePath,
                inputPath);

            var outputPath = @"C:\ProjectFile.icm";
            var projectFile = ProjectFile.CreateNew(
                outputPath);
            var hash = font;
            projectFile.MediaProject = new MediaProjectDataModel
                                           {
                                               Resources =
                                                   {
                                                       new ResourceInfo
                                                           {
                                                               Filename = testableInputPath1.FullName,
                                                               Hash = hash
                                                           }
                                                   }
                                           };
            localResources.AddResource(hash, testableInputPath1.FullName, false);
            projectFile.SaveAsync(localResources).Wait();

            var commandRegistry = new CommandRegistry();
            var shellController = new MediaShellController(shellMock.Object, commandRegistry);
            var controller = new ResourceController(shellController, shellMock.Object, commandRegistry);

            var resource = new ResourceInfoDataViewModel
                               {
                                   Facename = font,
                                   Type = ResourceType.Font,
                                   Filename = font,
                                   Hash = hash,
                                   IsLedFont = true,
                                   LedFontType = LedFontType.Unknown
                               };
            state.CurrentProject.Resources.Add(resource);

            controller.UpdateResourcesLedFontType();

            return resource.LedFontType == expectedType;
        }

        private void AddResourceWithDifferentExtension(
            string resourcePath1,
            string resourcePath2,
            string uniqueResourcePath2)
        {
            var projectTest = this.SetupProjectTest();
            this.dispatcherMock.Setup(d => d.Dispatch(It.IsAny<Action>()));
            projectTest.TestingFileSystem.CreateDirectory("C:\\folder");
            var file1 = projectTest.TestingFileSystem.CreateFile(resourcePath1);
            WriteDummyContent(file1, "AAA");
            var file2 = projectTest.TestingFileSystem.CreateFile(resourcePath2);
            WriteDummyContent(file2, "BBB");

            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            projectTest.MediaProjectDataViewModelMock.SetupGet(model => model.Resources).Returns(resources);

            projectTest.ProjectManagerMock.Setup(
                manager => manager.AddResource("E1FAFFB3E614E6C2FBA74296962386B7", resourcePath1, false));
            projectTest.ProjectManagerMock.Setup(
                manager => manager.AddResource("2BB225F0BA9A58930757A868ED57D9A3", resourcePath2, false));
            projectTest.ProjectManagerMock.Setup(manager => manager.GetResource(It.IsAny<string>()))
                .Returns(default(IResource));

            var parameters1 = new AddResourceParameters { Resources = new[] { file1 }, Type = ResourceType.Image };
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters1);

            var parameters2 = new AddResourceParameters { Resources = new[] { file2 }, Type = ResourceType.Image };
            projectTest.CommandRegistryMock.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters2);

            projectTest.ProjectManagerMock.Verify(
                manager => manager.AddResource("E1FAFFB3E614E6C2FBA74296962386B7", resourcePath1, false),
                Times.Once(),
                "Resource not added exactly once");
            projectTest.ProjectManagerMock.Verify(
                manager => manager.AddResource("2BB225F0BA9A58930757A868ED57D9A3", resourcePath2, false),
                Times.Once(),
                "Resource not added exactly once");
        }

        private SetupProjectTestResult SetupProjectTest()
        {
            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Default);
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            mediaShellMock.Setup(
                shell => shell.Dictionary).Returns(() => new DictionaryDataViewModel(new Dictionary()));
            var commandRegistry = new CommandRegistry();
            var mediaProjectDataViewModelMock = new Mock<MediaProjectDataViewModel>();
            mediaProjectDataViewModelMock.SetupGet(model => model.IsDirty).Returns(false);
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
                    new List<PhysicalScreenTypeConfig>
                                                         {
                                                             screenType
                                                         }
            };

            var mediaConfiguration = new MediaConfiguration
            {
                PhysicalScreenSettings = physicalScreenSettings
            };
            this.unityContainer.RegisterInstance(mediaConfiguration);
            var projectManagerMock = new Mock<IProjectManager>(MockBehavior.Strict);
            var applicationStateMock = new Mock<IMediaApplicationState>();
            applicationStateMock.SetupGet(state => state.ProjectManager).Returns(projectManagerMock.Object);
            applicationStateMock.SetupGet(state => state.CurrentProject).Returns(mediaProjectDataViewModelMock.Object);
            applicationStateMock.SetupGet(state => state.ConsistencyMessages)
                                .Returns(new ExtendedObservableCollection<ConsistencyMessageDataViewModel>());

            mediaShellMock.Setup(shell => shell.MediaApplicationState).Returns(() => applicationStateMock.Object);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            var permissionControllerMock = new Mock<IPermissionController>();
            permissionControllerMock.Setup(controller => controller.PermissionTrap(
                Permission.Write,
                DataScope.MediaConfiguration)).Returns(true);
            applicationControllerMock.Setup(controller => controller.PermissionController)
                .Returns(permissionControllerMock.Object);
            this.unityContainer.RegisterInstance(applicationControllerMock.Object);
            var testingFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(testingFileSystem);

            // ReSharper disable once UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            var resourceManagerMock = new Mock<IResourceManager>();
            resourceManagerMock.Setup(m => m.CheckAvailableDiskSpace(It.IsAny<IFileInfo>())).Returns(true);
            resourceManagerMock.Setup(m => m.CheckUsedDiskSpace(It.IsAny<long>())).Returns(true);
            this.unityContainer.RegisterInstance(typeof(IResourceManager), resourceManagerMock.Object);
            return new SetupProjectTestResult(
                commandRegistry,
                projectManagerMock,
                mediaProjectDataViewModelMock,
                testingFileSystem);
        }

        private struct SetupProjectTestResult
        {
            public SetupProjectTestResult(
                CommandRegistry commandRegistryMock,
                Mock<IProjectManager> projectManagerMock,
                Mock<MediaProjectDataViewModel> mediaProjectDataViewModelMock,
                TestingFileSystem testingFileSystem)
                : this()
            {
                this.CommandRegistryMock = commandRegistryMock;
                this.ProjectManagerMock = projectManagerMock;
                this.MediaProjectDataViewModelMock = mediaProjectDataViewModelMock;
                this.TestingFileSystem = testingFileSystem;
            }

            public CommandRegistry CommandRegistryMock { get; private set; }

            public Mock<IProjectManager> ProjectManagerMock { get; private set; }

            public Mock<MediaProjectDataViewModel> MediaProjectDataViewModelMock { get; private set; }

            public TestingFileSystem TestingFileSystem { get; private set; }
        }
    }
}
