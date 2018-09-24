// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiedProjectStateTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ModifiedState" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers.ProjectStates
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="ModifiedState"/>.
    /// </summary>
    [TestClass]
    public class ModifiedProjectStateTest
    {
        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// Tests that calling <see cref="StateBase.OpenLocalAsync"/> from <see cref="ModifiedState"/> does nothing.
        /// </summary>
        [TestMethod]
        public void OpenLocalTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            var state = new ModifiedState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            Assert.AreEqual(ProjectStates.Modified, state.StateInfo);

            var resultState = state.OpenLocalAsync("test").Result;
            Assert.AreEqual(ProjectStates.Modified, resultState.StateInfo);
        }

        /// <summary>
        /// Tests saving the project locally.
        /// </summary>
        [TestMethod]
        public void SaveProjectTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var projectControllerMock = new Mock<IProjectControllerContext>();
            projectControllerMock.Setup(controller => controller.ExecuteSaveProjectLocal()).Returns(true);
            var state = new ModifiedState(shellMock.Object, commandRegistryMock.Object, projectControllerMock.Object);
            Assert.AreEqual(ProjectStates.Modified, state.StateInfo);

            var resultState = state.SaveAsync().Result;
            Assert.AreEqual(ProjectStates.Saved, resultState.StateInfo);
            projectControllerMock.Verify(controller => controller.ExecuteSaveProjectLocal(), Times.Once());
        }
    }
}