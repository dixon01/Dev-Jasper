// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistoryControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ChangeHistoryController" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="ChangeHistoryController"/>.
    /// </summary>
    [TestClass]
    public class ChangeHistoryControllerTest
    {
        /// <summary>
        /// Tests the <see cref="ChangeHistoryController.AddHistoryEntry"/> and verifies that
        /// the <see cref="IHistoryEntry.Do"/> is called.
        /// </summary>
        [TestMethod]
        public void AddHistoryEntryWithDoExecutionTest()
        {
            var setupTestResult = SetupTest();

            var historyEntry = new Mock<IHistoryEntry>();
            setupTestResult.ChangeHistoryController.AddHistoryEntry(historyEntry.Object);
            setupTestResult.ChangeHistoryMock.Verify(history => history.Add(historyEntry.Object), Times.Once());
            historyEntry.Verify(entry => entry.Do(), Times.Once());
            setupTestResult.ShellMock.Verify(shell => shell.MediaApplicationState.MakeDirty(), Times.Once());
            Assert.IsFalse(setupTestResult.MediaProjectDataViewModel.IsCheckedIn);
            setupTestResult.ProjectControllerMock.Verify(
                controller => controller.ConsistencyChecker.Check(),
                Times.Once());
        }

        /// <summary>
        /// Tests the <see cref="ChangeHistoryController.AddHistoryEntry"/> with the Skip flag and verifies that
        /// the <see cref="IHistoryEntry.Do"/> is not called.
        /// </summary>
        [TestMethod]
        public void AddHistoyEntrySkipDoTest()
        {
            var setupTestResult = SetupTest();
            var historyEntry = new Mock<IHistoryEntry>();
            setupTestResult.ChangeHistoryController.AddHistoryEntry(historyEntry.Object, true);
            historyEntry.Verify(entry => entry.Do(), Times.Never());
        }

        /// <summary>
        /// Tests the <see cref="ChangeHistoryController.AddSaveMarker"/>.
        /// </summary>
        [TestMethod]
        public void AddSaveMarkerTest()
        {
            var setupTestResult = SetupTest();
            setupTestResult.ChangeHistoryController.AddSaveMarker();
            setupTestResult.ChangeHistoryMock.Verify(history => history.AddSaveMarker(), Times.Once());
        }

        /// <summary>
        /// The undo test.
        /// </summary>
        [TestMethod]
        public void UndoTest()
        {
            var setupTestResult = SetupTest();
            setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Undo).Execute(null);
            setupTestResult.ChangeHistoryMock.Verify(history => history.Undo(), Times.Once());
            setupTestResult.ProjectControllerMock.Verify(
                controller => controller.ConsistencyChecker.Check(),
                Times.Once());
        }

        /// <summary>
        /// Test the CanUndo with and without elements in the <see cref="IChangeHistory.UndoStack"/>.
        /// </summary>
        [TestMethod]
        public void CanUndoTest()
        {
            var setupTestResult = SetupTest();
            setupTestResult.ChangeHistoryMock.Setup(history => history.UndoStack)
                .Returns(new ReadOnlyCollection<IHistoryEntry>(new List<IHistoryEntry>()));
            Assert.IsFalse(
                setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Undo).CanExecute(null));
            setupTestResult.ChangeHistoryMock.Setup(history => history.UndoStack)
                .Returns(
                    new ReadOnlyCollection<IHistoryEntry>(
                        new List<IHistoryEntry> { new Mock<IHistoryEntry>().Object }));
            Assert.IsTrue(
                setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Undo).CanExecute(null));
        }

        /// <summary>
        /// The redo test.
        /// </summary>
        [TestMethod]
        public void RedoTest()
        {
            var setupTestResult = SetupTest();
            setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Redo).Execute(null);
            setupTestResult.ChangeHistoryMock.Verify(history => history.Redo(), Times.Once());
            setupTestResult.ProjectControllerMock.Verify(
                controller => controller.ConsistencyChecker.Check(),
                Times.Once());
        }

        /// <summary>
        /// Test the CanRedo with and without elements in the <see cref="IChangeHistory.RedoStack"/>.
        /// </summary>
        [TestMethod]
        public void CanRedoTest()
        {
            var setupTestResult = SetupTest();
            setupTestResult.ChangeHistoryMock.Setup(history => history.RedoStack)
                .Returns(new ReadOnlyCollection<IHistoryEntry>(new List<IHistoryEntry>()));
            Assert.IsFalse(
                setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Redo).CanExecute(null));
            setupTestResult.ChangeHistoryMock.Setup(history => history.RedoStack)
                .Returns(
                    new ReadOnlyCollection<IHistoryEntry>(
                        new List<IHistoryEntry> { new Mock<IHistoryEntry>().Object }));
            Assert.IsTrue(
                setupTestResult.CommandRegistry.GetCommand(CommandCompositionKeys.Default.Redo).CanExecute(null));
        }

        private static SetupTestResult SetupTest()
        {
            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Default);
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var mediaShellControllerMock = new Mock<IMediaShellController>();
            var projectControllerMock = new Mock<IProjectController>();
            projectControllerMock.Setup(controller => controller.ConsistencyChecker.Check());
            mediaShellControllerMock.Setup(controller => controller.ProjectController)
                .Returns(projectControllerMock.Object);
            var shellMock = new Mock<IMediaShell>();
            shellMock.Setup(shell => shell.MediaApplicationState.MakeDirty());
            var mediaProjectDataViewModel = new MediaProjectDataViewModel { IsCheckedIn = true };
            shellMock.Setup(shell => shell.MediaApplicationState.CurrentProject).Returns(mediaProjectDataViewModel);
            var commandRegistry = new CommandRegistry();
            var changeHistoryController = new ChangeHistoryController(
                mediaShellControllerMock.Object,
                shellMock.Object,
                commandRegistry);
            return new SetupTestResult(
                changeHistoryController,
                commandRegistry,
                changeHistoryMock,
                mediaShellControllerMock,
                shellMock,
                projectControllerMock,
                mediaProjectDataViewModel);
        }

        private class SetupTestResult
        {
            public SetupTestResult(
                IChangeHistoryController changeHistoryController,
                ICommandRegistry commandRegistry,
                Mock<IChangeHistory> changeHistoryMock,
                Mock<IMediaShellController> mediaShellControllerMock,
                Mock<IMediaShell> shellMock,
                Mock<IProjectController> projectControllerMock,
                MediaProjectDataViewModel project)
            {
                this.ChangeHistoryController = changeHistoryController;
                this.CommandRegistry = commandRegistry;
                this.ChangeHistoryMock = changeHistoryMock;
                this.MediaShellControllerMock = mediaShellControllerMock;
                this.ShellMock = shellMock;
                this.ProjectControllerMock = projectControllerMock;
                this.MediaProjectDataViewModel = project;
            }

            public ICommandRegistry CommandRegistry { get; set; }

            public IChangeHistoryController ChangeHistoryController { get; set; }

            public MediaProjectDataViewModel MediaProjectDataViewModel { get; set; }

            public Mock<IProjectController> ProjectControllerMock { get; set; }

            public Mock<IMediaShell> ShellMock { get; set; }

            public Mock<IMediaShellController> MediaShellControllerMock { get; set; }

            public Mock<IChangeHistory> ChangeHistoryMock { get; set; }
        }
    }
}
