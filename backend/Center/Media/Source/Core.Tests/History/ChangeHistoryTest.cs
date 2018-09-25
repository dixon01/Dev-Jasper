// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistoryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="ChangeHistory"/> with specific entries for Media application.
    /// </summary>
    [TestClass]
    public class ChangeHistoryTest
    {
        private UnityContainer unityContainer;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.unityContainer = Helpers.InitializeServiceLocator();
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
        }

        /// <summary>
        /// Tests the handling of the IsDirty flag when used within the <see cref="ChangeHistory"/>.
        /// </summary>
        [TestMethod]
        public void IsDirtyHandlingTest()
        {
            var editor = InitializeEditor();
            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);
            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");

            var history = new ChangeHistory();
            entry.Do();
            history.Add(entry);

            Assert.AreEqual(110, element.X.Value);
            Assert.AreEqual(210, element.Y.Value);
            Assert.IsTrue(element.IsDirty);

            history.Undo();

            Assert.IsFalse(element.IsDirty);

            history.Redo();
            Assert.IsTrue(element.IsDirty);
            Assert.AreEqual(110, element.X.Value);
            Assert.AreEqual(210, element.Y.Value);
        }

        /// <summary>
        /// Tests the handling of the IsDirty flag when used within the <see cref="ChangeHistory"/>.
        /// </summary>
        [TestMethod]
        public void IsDirtyHandlingWithSaveTest()
        {
            var editor = InitializeEditor();
            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);
            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");

            var history = new ChangeHistory();
            entry.Do();
            history.Add(entry);

            Assert.AreEqual(110, element.X.Value);
            Assert.AreEqual(210, element.Y.Value);
            Assert.IsTrue(element.IsDirty);
            history.AddSaveMarker();

            history.Undo();

            Assert.IsTrue(element.IsDirty);

            history.Redo();
            Assert.IsFalse(element.IsDirty);
            Assert.AreEqual(110, element.X.Value);
            Assert.AreEqual(210, element.Y.Value);
        }

        /// <summary>
        /// Tests that multiple save marks are not noticeable by the user
        /// </summary>
        [TestMethod]
        public void SaveMarkTest()
        {
            var editor = InitializeEditor();

            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);
            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");

            var history = new ChangeHistory();

            history.AddSaveMarker();
            Assert.AreEqual(0, history.UndoStack.Count);
            Assert.AreEqual(0, history.RedoStack.Count);

            entry.Do();
            history.Add(entry);
            history.AddSaveMarker();
            history.AddSaveMarker();

            Assert.IsTrue(element.IsDirty);

            history.Undo();

            Assert.AreEqual(0, history.UndoStack.Count);
            Assert.AreEqual(1, history.RedoStack.Count);

            Assert.IsTrue(element.IsDirty);
        }

        /// <summary>
        /// Tests that only one history element is undone
        /// </summary>
        [TestMethod]
        public void OneUndoTest()
        {
            var editor = InitializeEditor();

            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);

            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);

            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");
            var entry2 = new MoveLayoutElementsHistoryEntry(list, editor, 5, 5, "Move by 5");

            var history = new ChangeHistory();

            entry.Do();
            history.Add(entry);
            history.AddSaveMarker();
            entry2.Do();
            history.Add(entry2);
            history.AddSaveMarker();

            history.Undo();

            Assert.AreEqual(1, history.UndoStack.Count);
            Assert.AreEqual(1, history.RedoStack.Count);
            Assert.IsTrue(element.IsDirty);
        }

        /// <summary>
        /// Undo such that a saved state is restored and nothing is dirty
        /// </summary>
        [TestMethod]
        public void UndoIntoSaveMarkTest()
        {
            var editor = InitializeEditor();

            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);
            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");
            var entry2 = new MoveLayoutElementsHistoryEntry(list, editor, 5, 5, "Move by 5");

            var history = new ChangeHistory();

            Assert.AreEqual(0, history.UndoStack.Count);
            Assert.AreEqual(0, history.RedoStack.Count);

            entry.Do();
            history.Add(entry);
            history.AddSaveMarker();
            entry2.Do();
            history.Add(entry2);

            Assert.IsTrue(element.IsDirty);

            history.Undo();

            Assert.AreEqual(1, history.UndoStack.Count);
            Assert.AreEqual(1, history.RedoStack.Count);

            Assert.IsFalse(element.IsDirty);
        }

        /// <summary>
        /// Redo such that a saved state is restored and nothing is dirty
        /// </summary>
        [TestMethod]
        public void RedoIntoSaveMarkTest()
        {
            var editor = InitializeEditor();

            var element = new StaticTextElementDataViewModel(editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1"),
            };
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty()).Callback(element.ClearDirty);
            applicationStateMock.Setup(state => state.MakeDirty()).Callback(element.MakeDirty);
            this.unityContainer.RegisterInstance(applicationStateMock.Object);

            var list = new List<DrawableElementDataViewModelBase> { element };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, editor, 10, 10, "Move by 10");
            var entry2 = new MoveLayoutElementsHistoryEntry(list, editor, 5, 5, "Move by 5");

            var history = new ChangeHistory();

            Assert.AreEqual(0, history.UndoStack.Count);
            Assert.AreEqual(0, history.RedoStack.Count);

            entry.Do();
            history.Add(entry);
            history.AddSaveMarker();
            entry2.Do();
            history.Add(entry2);
            history.AddSaveMarker();
            history.Undo();

            Assert.IsTrue(element.IsDirty);

            history.Redo();

            Assert.AreEqual(2, history.UndoStack.Count);
            Assert.AreEqual(0, history.RedoStack.Count);

            Assert.IsFalse(element.IsDirty);
        }

        private static TftEditorViewModel InitializeEditor()
        {
            var mediaShellParams = new MediaShell.MediaShellParams
            {
                Factory = new MediaShellFactory(),
                Stages = new List<Lazy<IStage, IStageMetadata>>(),
                MenuItems = new List<Lazy<MenuItemBase, IMenuItemMetadata>>(),
                StatusBarItems = new List<Lazy<StatusBarItemBase>>()
            };
            var commandRegistry = new CommandRegistry();
            var mediaShell = new MediaShell(mediaShellParams, commandRegistry);

            // ReSharper disable UnusedVariable
            var controller = new MediaShellController(mediaShell, commandRegistry);
            // ReSharper restore UnusedVariable
            return (TftEditorViewModel)mediaShell.Editor;
        }
    }
}
