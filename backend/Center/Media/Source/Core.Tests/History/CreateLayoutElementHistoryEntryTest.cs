// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateLayoutElementHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ProjectManagement;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="CreateLayoutElementHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class CreateLayoutElementHistoryEntryTest
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
            this.unityContainer = null;
        }

        /// <summary>
        /// Tests that the layout element must not be null.
        /// </summary>
        [TestMethod]
        public void ConstructorElementNullTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);

            try
            {
                var editor = Helpers.InitializeEditorContent(state);
                var commandRegistry = new CommandRegistry();
                // ReSharper disable UnusedVariable
                var entry = new CreateLayoutElementHistoryEntry(null, editor, commandRegistry, state, "Test");
                // ReSharper restore UnusedVariable
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("element", e.ParamName);
            }
        }

        /// <summary>
        /// Tests that the layout editor must not be null.
        /// </summary>
        [TestMethod]
        public void ConstructorEditorNullTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);

            try
            {
                var editor = Helpers.InitializeEditorContent(state);
                var commandRegistry = new CommandRegistry();
                // ReSharper disable UnusedVariable
                var entry = new CreateLayoutElementHistoryEntry(
                    new StaticTextElementDataViewModel(editor.Parent), null, commandRegistry, state, "Test");
                // ReSharper restore UnusedVariable
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("editor", e.ParamName);
            }
        }

        /// <summary>
        /// Tests the aggregation of two <see cref="CreateLayoutElementHistoryEntry"/>s.
        /// </summary>
        [TestMethod]
        public void AggregateTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);

            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new CommandRegistry();
            var existingElement = new StaticTextElementDataViewModel(editor.Parent);
            var existingEntry = new CreateLayoutElementHistoryEntry(
                existingElement, editor, commandRegistry, state, "Test");
            editor.Elements.Add(existingElement);
            var newEntry = new CreateLayoutElementHistoryEntry(
                new StaticTextElementDataViewModel(editor.Parent), editor, commandRegistry, state, "Test2");
            Assert.IsFalse(existingEntry.Aggregate(newEntry));
            Assert.AreEqual("Test", existingEntry.DisplayText);
        }

        /// <summary>
        /// Tests the Undo of a <see cref="CreateLayoutElementHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void UndoTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var resourceManagerMock = Helpers.CreateResourceManagerMock(this.unityContainer);
            resourceManagerMock.Setup(r => r.TextElementManager).Returns(new TextElementReferenceManager());
            var editor = Helpers.InitializeEditorContent(state);
            var commandRegistry = new CommandRegistry();
            var existingSelection = new[]
                                        {
                                            new StaticTextElementDataViewModel(editor.Parent),
                                            new StaticTextElementDataViewModel(editor.Parent)
                                        };
            editor.Elements.AddRange(existingSelection);
            editor.SelectedElements.AddRange(existingSelection);
            var existingElement = new StaticTextElementDataViewModel(editor.Parent);
            var existingEntry = new CreateLayoutElementHistoryEntry(
                existingElement, editor, commandRegistry, state, "Undo test");
            existingEntry.Do();
            editor.SelectedElements.Clear();
            editor.SelectedElements.Add(existingElement);
            Assert.AreEqual(4, editor.Elements.Count);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            existingEntry.Undo();
            Assert.AreEqual(3, editor.Elements.Count);
            Assert.AreEqual(2, editor.SelectedElements.Count);
        }

        /// <summary>
        /// Tests the Redo of a <see cref="CreateLayoutElementHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void RedoTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var resourceManagerMock = Helpers.CreateResourceManagerMock(this.unityContainer);
            resourceManagerMock.Setup(r => r.TextElementManager).Returns(new TextElementReferenceManager());
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
            var existingEntry2 = new CreateLayoutElementHistoryEntry(
                existingElement2, editor, commandRegistry, state, "Redo test");
            editor.Elements.Add(existingElement1);
            ((LayoutConfigDataViewModel)state.CurrentLayout).Resolutions.First().Elements.Add(existingElement1);
            editor.SelectedElements.Add(existingElement1);
            Assert.AreEqual(2, editor.Elements.Count);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            existingEntry2.Redo();
            Assert.AreEqual(3, editor.Elements.Count);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            Assert.AreEqual(existingElement2.ElementName, editor.SelectedElements[0].ElementName);
            Assert.IsTrue(editor.Elements.Contains(existingElement2));
            Assert.IsTrue(editor.Elements.Contains(existingElement1));
        }
    }
}