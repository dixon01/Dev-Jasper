// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectManagerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.ProjectManagement
{
    using System;
    using System.IO;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines unit tests for the <see cref="ProjectManager"/> class.
    /// </summary>
    [TestClass]
    public class ProjectManagerTest
    {
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
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(fileSystemMock.Object);
        }

        /// <summary>
        /// Verifies that an <see cref="InvalidOperationException"/> is thrown if the project was never either loaded
        /// or created.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The expected InvalidOperationException was not thrown",
            AllowDerivedTypes = false)]
        public void SaveInvalidOperationTest()
        {
            var localResourceProviderMock = new Mock<IResourceProvider>(MockBehavior.Strict);
            var projectManager = new ProjectManager(localResourceProviderMock.Object);
            var project = new MediaProjectDataModel();
            try
            {
                projectManager.SaveAsync(project).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
            }
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentNullException"/> is thrown if the project was never either loaded
        /// or created.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "The expected ArgumentNullException was not thrown",
            AllowDerivedTypes = false)]
        public void SaveNullMediaProjectTest()
        {
            var localResourceProviderMock = new Mock<IResourceProvider>(MockBehavior.Strict);
            var projectManager = new ProjectManager(localResourceProviderMock.Object);
            try
            {
                projectManager.SaveAsync(null).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
            }
        }

        /// <summary>
        /// Verifies that a resource is added to the local resource provider.
        /// </summary>
        [TestMethod]
        public void AddResourceTest()
        {
            var localResourceProviderMock = new Mock<IResourceProvider>(MockBehavior.Strict);
            localResourceProviderMock.Setup(provider => provider.AddResource("hash", "path", false))
                                     .Verifiable("The resource was not added to the local provider");
            var projectManager = new ProjectManager(localResourceProviderMock.Object);
            projectManager.AddResource("hash", "path", false);
            localResourceProviderMock.Verify(
                provider => provider.AddResource("hash", "path", false),
                Times.Once(),
                "The resource was not added exactly once to the local provider");
        }

        /// <summary>
        /// Verifies that the IsFileSelected property is set to true after loading a project.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ProjectManagement\ProjectWithResource.icm")]
        public void FileSelectedAfterProjectLoad()
        {
             var localResourceProviderMock = new Mock<IResourceProvider>();
             var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            localFileSystem.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var projectFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "ProjectWithResource.icm");
            var projectFileInfo = CopyToTestFileSystem(localFileSystem, projectFilePath);

            var projectManager = new ProjectManager(localResourceProviderMock.Object);
            // ReSharper disable UnusedVariable
            var project = projectManager.LoadProject(projectFileInfo.FullName);
            // ReSharper restore UnusedVariable
            Assert.IsTrue(projectManager.IsFileSelected);
        }

        /// <summary>
        /// Verifies that a resource is retrieved from the local resource provider.
        /// If it doesn't exist, it is searched in the inner file.
        /// It also implicitly tests the <see cref="ProjectManager.LoadProject"/> method.
        /// </summary>
        [DeploymentItem(@"ProjectManagement\ProjectWithResource.icm")]
        [TestMethod]
        public void GetResourceTest()
        {
            // hash for the Resource.txt with the text 'Resource test'
            const string Content1 = "Resource test";
            const string Hash1 = "2B5A90F1D592319548932AB8FCC8010D";

            var localFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(localFileSystem);

            localFileSystem.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var projectFilePath = Path.Combine(this.TestContext.DeploymentDirectory, "ProjectWithResource.icm");
            var projectFileInfo = CopyToTestFileSystem(localFileSystem, projectFilePath);

            var localResourceProviderMock = new Mock<IResourceProvider>(MockBehavior.Strict);
            localResourceProviderMock.Setup(provider => provider.GetResource(Hash1))
                                     .Throws(new UpdateException())
                                     .Verifiable("The resource was not requested to the local provider");
            // ReSharper disable ImplicitlyCapturedClosure
            Func<string, bool> isTempPath = p => p.EndsWith(Hash1 + ".tmp");
            // ReSharper restore ImplicitlyCapturedClosure
            localResourceProviderMock.Setup(
                provider => provider.AddResource(Hash1, It.Is<string>(p => isTempPath(p)), true))
                                     .Verifiable("The resource was not added to the local provider");
            var projectManager = new ProjectManager(localResourceProviderMock.Object);
            projectManager.LoadProject(projectFileInfo.FullName);
            var resource = projectManager.GetResource(Hash1);
            using (var stream = resource.OpenRead())
            {
                VerifyContent(stream, Content1);
            }

            localResourceProviderMock.Verify(
                provider => provider.GetResource(Hash1),
                Times.Once(),
                "The resource was not requested exactly once to the local provider");
            localResourceProviderMock.Verify(
                provider => provider.AddResource(Hash1, It.Is<string>(p => isTempPath(p)), true),
                Times.Once(),
                "The resource was not added to the local provider");
        }

        private static IWritableFileInfo CopyToTestFileSystem(IWritableFileSystem fileSystem, string actualPath)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            var path = Path.Combine(@"C:\", Path.GetFileName(actualPath));
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

        private static void VerifyContent(Stream stream, string content)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var storedContent = streamReader.ReadToEnd();
                Assert.AreEqual(content, storedContent);
            }
        }
    }
}