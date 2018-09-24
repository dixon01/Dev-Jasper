// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ResourceManagerTest and is intended
//   to contain all ResourceManagerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.ProjectManagement
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for ResourceManagerTest and is intended
    /// to contain all ResourceManagerTest Unit Tests
    /// </summary>
    [TestClass]
    public class ResourceManagerTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(fileSystemMock.Object);
            TimeProvider.ResetToDefault();
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
            var stateMock = new Mock<IMediaApplicationState>();
            stateMock.Setup(mock => mock.Options).Returns(new ApplicationOptions());
            unityContainer.RegisterInstance(typeof(IMediaApplicationState), stateMock.Object);
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(fileSystemMock.Object);
            TimeProvider.ResetToDefault();
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        /// <summary>
        /// Tests the <see cref="ResourceManager.CheckAvailableDiskSpace"/> with enough available disk space.
        /// </summary>
        [TestMethod]
        public void CheckAvailableDiskSpaceTest()
        {
            var localFileSystem = this.SetupTestFileSystem();
            var drives = localFileSystem.GetDrives();
            foreach (var info in drives.OfType<TestingDriveInfo>())
            {
                info.SetDriveSpace(100, 100000000);
            }

            var resouceSettings = new ResourceSettings
                                      {
                                          MinRemainingDiskSpace = 10
                                      };
            var resourceManager = new ResourceManager(resouceSettings);

            Assert.IsTrue(resourceManager.CheckAvailableDiskSpace());
        }

        /// <summary>
        /// Tests the <see cref="ResourceManager.CheckAvailableDiskSpace"/> without enough available disk space.
        /// </summary>
        [TestMethod]
        public void CheckAvailableDiskSpaceInsufficientTest()
        {
            var localFileSystem = this.SetupTestFileSystem();
            var drives = localFileSystem.GetDrives();
            foreach (var info in drives.OfType<TestingDriveInfo>())
            {
                info.SetDriveSpace(100, 100000000);
            }

            var resouceSettings = new ResourceSettings
            {
                MinRemainingDiskSpace = 1000
            };
            var resourceManager = new ResourceManager(resouceSettings);

            Assert.IsFalse(resourceManager.CheckAvailableDiskSpace());
        }

        /// <summary>
        /// Tests that the <see cref="ResourceManager.CheckUsedDiskSpace"/> returns <c>true</c> if the configured
        /// max value has not been reached.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        public void CheckUsedDiskSpaceTest()
        {
            var resourceSettings = new ResourceSettings { MaxUsedDiskSpace = 10000000 };
            var target = new ResourceManager(resourceSettings);
            var localFileSystem = this.SetupTestFileSystem();
            var resourceFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            CopyToTestFileSystem(localFileSystem, resourceFilePath, resourcesPath);

            Assert.IsTrue(target.CheckUsedDiskSpace());
        }

        /// <summary>
        /// Tests that the <see cref="ResourceManager.CheckUsedDiskSpace"/> returns <c>false</c> if the configured
        /// max value is smaller than the used space.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        public void CheckUsedDiskSpaceTooMuchTest()
        {
            var resourceSettings = new ResourceSettings { MaxUsedDiskSpace = 10 };
            var target = new ResourceManager(resourceSettings);
            var localFileSystem = this.SetupTestFileSystem();
            var resourceFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            CopyToTestFileSystem(localFileSystem, resourceFilePath, resourcesPath);

            Assert.IsFalse(target.CheckUsedDiskSpace());
        }

        /// <summary>
        /// Tests that the <see cref="ResourceManager.CheckUsedDiskSpace"/> returns <c>false</c> if the configured
        /// max value is smaller than the used space including the additional file size.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        public void CheckUsedDiskSpaceAddingFileTest()
        {
            const int AdditionalFilesSize = 100000;
            var resourceSettings = new ResourceSettings { MaxUsedDiskSpace = 100 };
            var target = new ResourceManager(resourceSettings);
            var localFileSystem = this.SetupTestFileSystem();
            var resourceFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            CopyToTestFileSystem(localFileSystem, resourceFilePath, resourcesPath);
            Assert.IsTrue(target.CheckUsedDiskSpace());
            Assert.IsFalse(target.CheckUsedDiskSpace(AdditionalFilesSize));
        }

        /// <summary>
        /// Tests that only resources are deleted that were changed before the configured "keep" value.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [DeploymentItem(@"ProjectManagement\Resource2.txt")]
        public void CleanupResourcesTest()
        {
            var localFileSystem = this.SetupTestFileSystem();
            var currentDate = new DateTime(2014, 04, 10, 11, 0, 0);
            var oldDate = currentDate - TimeSpan.FromDays(30);
            var validDate = currentDate - TimeSpan.FromMinutes(30);

            var mockedApplicationState = new Mock<IMediaApplicationState>();
            mockedApplicationState.SetupGet(state => state.CurrentProject.Resources)
                                  .Returns(new ExtendedObservableCollection<ResourceInfoDataViewModel>());
            mockedApplicationState.Setup(state => state.Options).Returns(new ApplicationOptions());
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(locator => locator.GetInstance<IMediaApplicationState>())
                              .Returns(mockedApplicationState.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var resource1FilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var resource2FilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource2.txt");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            SetupTimeProvider(oldDate);
            CopyToTestFileSystem(localFileSystem, resource1FilePath, resourcesPath, true);
            SetupTimeProvider(validDate);
            CopyToTestFileSystem(localFileSystem, resource2FilePath, resourcesPath, true);
            SetupTimeProvider(currentDate);
            Assert.AreEqual(2, localFileSystem.GetDirectory(resourcesPath).GetFiles().Length);
            var resourceSettings = new ResourceSettings
                                       {
                                           RemoveLocalResourceAfter = TimeSpan.FromHours(1)
                                       };
            var resourceManager = new ResourceManager(resourceSettings);
            resourceManager.CleanupResources();
            var result = localFileSystem.GetDirectory(resourcesPath).GetFiles();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Resource2.rx", result[0].Name);
        }

        /// <summary>
        /// Tests that only resources are deleted that are not used in the current project.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\Resource.txt")]
        [DeploymentItem(@"ProjectManagement\Resource2.txt")]
        public void CleanupResourcesWithResourcesUsedInProjectTest()
        {
            var localFileSystem = this.SetupTestFileSystem();
            var currentDate = new DateTime(2014, 04, 10, 11, 0, 0);
            var oldDate = currentDate - TimeSpan.FromDays(30);
            var resource = new ResourceInfoDataViewModel { Hash = "Resource", };
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel> { resource };
            var mockedApplicationState = new Mock<IMediaApplicationState>();
            mockedApplicationState.SetupGet(state => state.CurrentProject.Resources).Returns(resources);
            mockedApplicationState.Setup(state => state.Options).Returns(new ApplicationOptions());
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(locator => locator.GetInstance<IMediaApplicationState>())
                              .Returns(mockedApplicationState.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);

            var resource1FilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource.txt");
            var resource2FilePath = Path.Combine(this.TestContext.DeploymentDirectory, "Resource2.txt");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            SetupTimeProvider(oldDate);
            CopyToTestFileSystem(localFileSystem, resource1FilePath, resourcesPath, true);
            SetupTimeProvider(oldDate);
            CopyToTestFileSystem(localFileSystem, resource2FilePath, resourcesPath, true);
            SetupTimeProvider(currentDate);
            Assert.AreEqual(2, localFileSystem.GetDirectory(resourcesPath).GetFiles().Length);
            var resourceSettings = new ResourceSettings
            {
                RemoveLocalResourceAfter = TimeSpan.FromHours(1)
            };
            var resourceManager = new ResourceManager(resourceSettings);
            resourceManager.CleanupResources();
            var result = localFileSystem.GetDirectory(resourcesPath).GetFiles();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Resource.rx", result[0].Name);
        }

        private static void CopyToTestFileSystem(
            IWritableFileSystem fileSystem, string actualPath, string destinationPath, bool createRx = false)
        {
            string path;
            if (createRx)
            {
                var newFileName = Path.GetFileNameWithoutExtension(actualPath) + ".rx";
                path = Path.Combine(destinationPath, newFileName);
            }
            else
            {
                var filename = Path.GetFileName(actualPath);
                // ReSharper disable AssignNullToNotNullAttribute
                path = Path.Combine(destinationPath, filename);
                // ReSharper restore AssignNullToNotNullAttribute
            }

            var destinationPathInfo = fileSystem.CreateFile(path);
            using (var destinationStream = destinationPathInfo.OpenWrite())
            {
                using (var sourceStream = File.OpenRead(actualPath))
                {
                    sourceStream.CopyTo(destinationStream);
                }
            }
        }

        private static void SetupTimeProvider(DateTime currentTime)
        {
            var timeProviderMock = new Mock<TimeProvider>();
            timeProviderMock.Setup(t => t.Now).Returns(currentTime);
            TimeProvider.Current = timeProviderMock.Object;
        }

        private TestingFileSystem SetupTestFileSystem()
        {
            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);
            var thumbnailsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Thumbnails");
            var resourcesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Gorba\Center\Media\Resources");
            localFileSystem.CreateDirectory(thumbnailsPath);
            localFileSystem.CreateDirectory(resourcesPath);
            return localFileSystem;
        }
    }
}
