// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionBootstrapperTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompositionBootstrapperTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System.ComponentModel.Composition.Hosting;
    using System.Threading;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the composition bootstrapper.
    /// </summary>
    [TestClass]
    public class CompositionBootstrapperTest
    {
        private Mock<IServiceLocator> serviceLocatorMock;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.CreateServiceLocator();
            var writableFileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem);
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ResetServiceLocator();
            var writableFileSystem = new Mock<IWritableFileSystem>(MockBehavior.Strict);
            FileSystemManager.ChangeLocalFileSystem(writableFileSystem.Object);
        }

        /// <summary>
        /// Tests the bootstrap method and verifies the correct composition of the hierarchy.
        /// </summary>
        [TestMethod]
        public void BootstrapTest()
        {
            var unityContainer = new UnityContainer();
            this.serviceLocatorMock.Setup(locator => locator.GetInstance<IUnityContainer>())
                .Returns(unityContainer);
            var compositionBatch = new CompositionBatch();
            var bootstrapper = new CompositionBootstrapper(
                compositionBatch, typeof(MediaApplicationController).Assembly, typeof(CommandRegistry).Assembly);
            var applicationController =
                bootstrapper.Bootstrap<IMediaApplicationController, IMediaApplicationState>().Controller;
            Assert.IsInstanceOfType(applicationController, typeof(MediaApplicationController));
        }

        private static void ResetServiceLocator()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
        }

        private void CreateServiceLocator()
        {
            this.serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => this.serviceLocatorMock.Object);
        }
    }
}