// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultApplicationStateManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultApplicationStateManagerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Tests
{
    using System.IO;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Common.Utility.Files;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the default implementation of the <see cref="ApplicationStateManager"/>.
    /// </summary>
    [TestClass]
    public class DefaultApplicationStateManagerTest
    {
        private FileSystemStub fileSystemStub;

        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>
        /// The test context.
        /// </value>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.fileSystemStub = new FileSystemStub(this.TestContext);
            FileSystemManager.ChangeLocalFileSystem(this.fileSystemStub);
            var state = new ApplicationManagerState { Text = this.TestContext.TestName };
            this.Serialize(state);
        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var failingFileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(failingFileSystemMock.Object);
        }

        /// <summary>
        /// Test for the Load method.
        /// </summary>
        [TestMethod]
        public void LoadTest()
        {
            var testContextDirectory = new Mock<IDirectoryInfo>();
            testContextDirectory.Setup(info => info.FullName).Returns(this.TestContext.DeploymentDirectory);
            var defaultApplicationStateManager = new ApplicationStateManager.DefaultApplicationStateManager();
            var state = defaultApplicationStateManager.Load<ApplicationManagerState>("Test", "ApplicationManagerLoad");
            Assert.AreEqual(this.TestContext.TestName, state.Text);
        }

        /// <summary>
        /// Test for the Save method.
        /// </summary>
        [TestMethod]
        public void SaveTest()
        {
            var testContextDirectory = new Mock<IDirectoryInfo>();
            testContextDirectory.Setup(info => info.FullName).Returns(this.TestContext.DeploymentDirectory);
            var defaultApplicationStateManager = new ApplicationStateManager.DefaultApplicationStateManager();
            var state = new ApplicationManagerState { Text = this.TestContext.TestName };
            defaultApplicationStateManager.Save("Test", "ApplicationManagerSave", state);
            state = this.Deserialize();
            Assert.AreEqual(this.TestContext.TestName, state.Text);
        }

        private ApplicationManagerState Deserialize()
        {
            var filePath = Path.Combine(this.TestContext.DeploymentDirectory, "ApplicationManagerSave.state");
            var dataContractSerializer = new DataContractSerializer(typeof(ApplicationManagerState));
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (ApplicationManagerState)dataContractSerializer.ReadObject(fileStream);
            }
        }

        private void Serialize(ApplicationManagerState state)
        {
            var filePath = Path.Combine(this.TestContext.DeploymentDirectory, "ApplicationManagerLoad.state");
            var dataContractSerializer = new DataContractSerializer(typeof(ApplicationManagerState));
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                dataContractSerializer.WriteObject(fileStream, state);
            }
        }

        /// <summary>
        /// Test state class.
        /// </summary>
        [DataContract]
        public class ApplicationManagerState
        {
            /// <summary>
            /// Gets or sets the test text.
            /// </summary>
            /// <value>
            /// The test text.
            /// </value>
            [DataMember]
            public string Text { get; set; }
        }

        private class FileSystemStub : IFileSystem
        {
            private readonly TestContext testContext;

            private readonly Mock<IDirectoryInfo> directoryInfoMock;

            public FileSystemStub(TestContext testContext)
            {
                this.testContext = testContext;
                this.directoryInfoMock = new Mock<IDirectoryInfo>();
                this.directoryInfoMock.SetupGet(info => info.FullName).Returns(testContext.DeploymentDirectory);
            }

            public IDriveInfo[] GetDrives()
            {
                throw new System.NotSupportedException();
            }

            public IFileInfo GetFile(string path)
            {
                throw new System.NotSupportedException();
            }

            public bool TryGetFile(string path, out IFileInfo file)
            {
                var fileName = Path.GetFileName(path);
                // ReSharper disable AssignNullToNotNullAttribute
                var filePath = Path.Combine(this.testContext.DeploymentDirectory, fileName);
                // ReSharper restore AssignNullToNotNullAttribute
                var fileInfoMock = new Mock<IFileInfo>();
                fileInfoMock.SetupGet(info => info.FullName).Returns(filePath);
                file = fileInfoMock.Object;
                return true;
            }

            public IDirectoryInfo GetDirectory(string path)
            {
                throw new System.NotImplementedException();
            }

            public bool TryGetDirectory(string path, out IDirectoryInfo directory)
            {
                directory = this.directoryInfoMock.Object;
                return true;
            }
        }
    }
}