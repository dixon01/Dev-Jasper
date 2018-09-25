// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteLayoutElementHistoryEntryTest.cs" company="Gorba AG">
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
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="DeleteLayoutElementsHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class DeleteLayoutElementHistoryEntryTest
    {
        private UnityContainer unityContainer;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.unityContainer = Helpers.InitializeServiceLocator();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
            this.unityContainer = null;
        }

        /// <summary>
        /// Tests that the layout element must not be null.
        /// </summary>
        [TestMethod]
        public void ConstructorElementNullTest()
        {
            try
            {
                var editor = InitializeEditor();
                var entry = new DeleteLayoutElementsHistoryEntry(null, editor, null, null, "Test");
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("elements", e.ParamName);
            }
        }

        /// <summary>
        /// Tests that the layout editor must not be null.
        /// </summary>
        [TestMethod]
        public void ConstructorEditorNullTest()
        {
            try
            {
                var entry = new DeleteLayoutElementsHistoryEntry(
                    new List<DrawableElementDataViewModelBase>(), null, null, null, "Test");
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("editor", e.ParamName);
            }
        }

        /// <summary>
        /// Tests the aggregation of two <see cref="DeleteLayoutElementsHistoryEntry"/>s.
        /// </summary>
        [TestMethod]
        public void AggregateTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new CommandRegistry();

            var existingElement = new StaticTextElementDataViewModel(editor.Parent);
            var list = new List<DrawableElementDataViewModelBase>
                           {
                               existingElement
                           };
            var existingEntry = new DeleteLayoutElementsHistoryEntry(list, editor, commandRegistry, state, "Test");
            editor.Elements.Add(existingElement);
            var newEntry = new DeleteLayoutElementsHistoryEntry(
                new List<DrawableElementDataViewModelBase>(), editor, commandRegistry, state, "Test2");
            Assert.IsFalse(existingEntry.Aggregate(newEntry));
            Assert.AreEqual("Test", existingEntry.DisplayText);
        }

        /// <summary>
        /// Tests the Undo of a <see cref="DeleteLayoutElementsHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void UndoTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new CommandRegistry();

            var existingElement = new StaticTextElementDataViewModel(editor.Parent);
            var list = new List<DrawableElementDataViewModelBase> { existingElement };
            var existingEntry = new DeleteLayoutElementsHistoryEntry(list, editor, commandRegistry, state, "Undo test");
            existingEntry.Undo();
            Assert.AreEqual(2, editor.Elements.Count);
            Assert.AreEqual(1, editor.SelectedElements.Count);
        }

        /// <summary>
        /// Tests the Undo of a <see cref="DeleteLayoutElementsHistoryEntry"/> where one element has a customer font.
        /// </summary>
        [TestMethod]
        public void UndoWithFontReferencesTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var fontResource = new ResourceInfoDataViewModel
            {
                Facename = "TestFont",
                ReferencesCount = 2,
                Hash = "Hash"
            };
            var resourceManagerMock = Helpers.CreateResourceManagerMock(this.unityContainer);
            resourceManagerMock.Setup(mock => mock.TextElementManager).Returns(new TextElementReferenceManager());
            state.CurrentProject.Resources.Add(fontResource);
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new Mock<CommandRegistry>();
            var commandMock = new Mock<ICommand>();
            commandRegistry.Setup(c => c.GetCommand(CommandCompositionKeys.Project.SelectResource))
                .Returns(commandMock.Object);

            var existingElement1 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry1"),
                FontFace = { Value = "TestFont" },
                ZIndex = { Value = 1 }
            };
            var existingElement2 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry2"),
                FontFace = { Value = "Arial" },
                ZIndex = { Value = 0 }
            };

            var list = new List<DrawableElementDataViewModelBase> { existingElement1, existingElement2 };
            Assert.AreEqual(0, editor.SelectedElements.Count);
            Assert.AreEqual(1, editor.Elements.Count);
            var existingEntry = new DeleteLayoutElementsHistoryEntry(
                list,
                editor,
                commandRegistry.Object,
                state,
                "Undo test");
            existingEntry.Undo();
            Assert.AreEqual(3, editor.Elements.Count);
            Assert.AreEqual(2, editor.SelectedElements.Count);
            commandRegistry.Verify(c => c.GetCommand(CommandCompositionKeys.Project.SelectResource), Times.Once());
            commandMock.Verify(
                c =>
                    c.Execute(
                        It.Is<SelectResourceParameters>(
                            parameters =>
                                parameters.CurrentSelectedResourceHash == "Hash"
                                && parameters.PreviousSelectedResourceHash == null)));
        }

        /// <summary>
        /// Tests the Redo of a <see cref="DeleteLayoutElementsHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void RedoTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new CommandRegistry();

            var existingElement1 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry1")
            };
            var existingElement2 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry2")
            };
            var list = new List<DrawableElementDataViewModelBase> { existingElement1, existingElement2 };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            ((LayoutConfigDataViewModel)state.CurrentLayout).Resolutions.First().Elements.AddRange(list);
            Assert.AreEqual(2, editor.SelectedElements.Count);
            Assert.AreEqual(3, editor.Elements.Count);
            var existingEntry = new DeleteLayoutElementsHistoryEntry(list, editor, commandRegistry, state, "Redo test");
            existingEntry.Redo();
            Assert.AreEqual(1, editor.Elements.Count);
            Assert.AreEqual(0, editor.SelectedElements.Count);
        }

        /// <summary>
        /// Tests the Redo of a <see cref="DeleteLayoutElementsHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void RedoWithFontReferencesTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var fontResource = new ResourceInfoDataViewModel
                               {
                                   Facename = "TestFont",
                                   ReferencesCount = 2,
                                   Hash = "Hash"
                               };
            state.CurrentProject.Resources.Add(fontResource);
            var resourceManagerMock = Helpers.CreateResourceManagerMock(this.unityContainer);
            resourceManagerMock.Setup(mock => mock.TextElementManager).Returns(new TextElementReferenceManager());
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new Mock<CommandRegistry>();
            var commandMock = new Mock<ICommand>();
            commandRegistry.Setup(c => c.GetCommand(CommandCompositionKeys.Project.SelectResource))
                .Returns(commandMock.Object);

            var existingElement1 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry1"),
                FontFace = { Value = "TestFont" }
            };
            var existingElement2 = new StaticTextElementDataViewModel(editor.Parent)
            {
                ElementName = new DataValue<string>("TestEntry2")
            };
            var list = new List<DrawableElementDataViewModelBase> { existingElement1, existingElement2 };
            editor.SelectedElements.AddRange(list);
            editor.Elements.AddRange(list);
            ((LayoutConfigDataViewModel)state.CurrentLayout).Resolutions.First().Elements.AddRange(list);
            Assert.AreEqual(2, editor.SelectedElements.Count);
            Assert.AreEqual(3, editor.Elements.Count);
            var existingEntry = new DeleteLayoutElementsHistoryEntry(
                list,
                editor,
                commandRegistry.Object,
                state,
                "Redo test");
            existingEntry.Redo();
            Assert.AreEqual(1, editor.Elements.Count);
            Assert.AreEqual(0, editor.SelectedElements.Count);
            commandRegistry.Verify(c => c.GetCommand(CommandCompositionKeys.Project.SelectResource), Times.Once());
            commandMock.Verify(
                c =>
                    c.Execute(
                        It.Is<SelectResourceParameters>(
                            parameters =>
                                parameters.CurrentSelectedResourceHash == null
                                && parameters.PreviousSelectedResourceHash == "Hash")));
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
            var controller = new MediaShellController(mediaShell, commandRegistry);
            return (TftEditorViewModel)mediaShell.Editor;
        }
    }
}
