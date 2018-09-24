// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionBootstrapperTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultApplicationStateManagerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Tests
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Extensions;
    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Common.Utility.Files;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the default implementation of the <see cref="CompositionBootstrapper"/>.
    /// </summary>
    [TestClass]
    public class CompositionBootstrapperTest
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
            var serializeState = new CompositionState { Text = this.TestContext.TestName };
            this.Serialize(serializeState);
            var defaultApplicationStateManager = new ApplicationStateManager.DefaultApplicationStateManager();
            ApplicationStateManager.SetCurrent(defaultApplicationStateManager);
            var state = defaultApplicationStateManager.Load<CompositionState>("Test", "CompositionLoad");
            Assert.AreEqual(this.TestContext.TestName, state.Text);
            var currentAssembly = this.GetType().Assembly;
            var compositionBatch = new CompositionBatch();
            compositionBatch.TryExportApplicationStateManagerValue<CompositionState, IApplicationState>(
                "Test", "CompositionLoad");
            var bootstrapper = new CompositionBootstrapper(compositionBatch, currentAssembly);
            var bootstrapperResult = bootstrapper.Bootstrap<Main, IApplicationState>();
            var s = bootstrapperResult.State as CompositionState;
            Assert.AreEqual(this.TestContext.TestName, s.Text);
        }

        /// <summary>
        /// Test for path permission check.
        /// </summary>
        [TestMethod]
        public void GetFailingPathPermissionsTest()
        {
            var currentAssembly = this.GetType().Assembly;
            var filePath = this.TestContext.TestDir;
            var bootstrapper = new CompositionBootstrapper(null, currentAssembly);
            var permissions = bootstrapper.GetFailingPathPermissions(filePath, FileIOPermissionAccess.AllAccess);
            Assert.IsFalse(permissions.Any());
        }

        private void Serialize(CompositionState state)
        {
            var filePath = Path.Combine(this.TestContext.DeploymentDirectory, "CompositionLoad.state");
            var dataContractSerializer = new DataContractSerializer(typeof(CompositionState));
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                dataContractSerializer.WriteObject(fileStream, state);
            }
        }

        /// <summary>
        /// Test state class.
        /// </summary>
        [DataContract]
        [Export]
        public class CompositionState : IApplicationState
        {
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the test text.
            /// </summary>
            /// <value>
            /// The test text.
            /// </value>
            [DataMember]
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the current username.
            /// </summary>
            public string CurrentUsername { get; set; }

            /// <summary>
            /// Gets a value indicating whether this instance has changes, making it <c>dirty</c>.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
            /// </value>
            public bool IsDirty { get; private set; }

            /// <summary>
            /// Gets or sets the application options.
            /// </summary>
            public ApplicationOptions Options { get; set; }

            /// <summary>
            /// Gets the display context.
            /// </summary>
            public DisplayContext DisplayContext { get; private set; }

            /// <summary>
            /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
            /// </summary>
            public void MakeDirty()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Clears the <see cref="IsDirty"/> flag. The default behavior clears the flag on the current object and
            /// all its children.
            /// </summary>
            public void ClearDirty()
            {
                throw new NotImplementedException();
            }

            private void RaisePropertyChanged(string propertyName)
            {
                var handler = this.PropertyChanged;
                if (handler == null)
                {
                    return;
                }

                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Test main class.
        /// </summary>
        [Export]
        public class Main : IApplicationController
        {
            /// <summary>
            /// Occurs when the application shutdown is requested by the controller.
            /// </summary>
            public event EventHandler ShutdownCompleted;

            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            /// <value>
            /// The state.
            /// </value>
            [Import]
            public IApplicationState State { get; set; }

            /// <summary>
            /// Initializes this controller.
            /// </summary>
            public void Initialize()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Runs the controller logic until completed or until the <see cref="Shutdown"/>.
            /// </summary>
            public void Run()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Requests the shutdown of this controller.
            /// </summary>
            public void Shutdown()
            {
                throw new NotImplementedException();
            }

            private void RaiseShutdownCompleted()
            {
                var handler = this.ShutdownCompleted;
                if (handler == null)
                {
                    return;
                }

                handler(this, EventArgs.Empty);
            }
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