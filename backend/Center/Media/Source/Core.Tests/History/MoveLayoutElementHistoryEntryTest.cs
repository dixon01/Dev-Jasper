// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveLayoutElementHistoryEntryTest.cs" company="Gorba AG">
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
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="MoveLayoutElementsHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class MoveLayoutElementHistoryEntryTest
    {
        private UnityContainer unityContainer;

        private MediaApplicationState state;

        private TftEditorViewModel editor;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.unityContainer = Helpers.InitializeServiceLocator();
            this.state = Helpers.MockApplicationState(this.unityContainer);
            this.editor = Helpers.InitializeEditorContent(this.state);
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
            this.state = null;
            this.editor = null;
        }

        /// <summary>
        /// Tests that the layout element must not be null.
        /// </summary>
        [TestMethod]
        public void ConstructorElementsNullTest()
        {
            try
            {
                // ReSharper disable ObjectCreationAsStatement
                new MoveLayoutElementsHistoryEntry(null, this.editor, 0, 0, "Test");
                // ReSharper restore ObjectCreationAsStatement
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
                // ReSharper disable ObjectCreationAsStatement
                new MoveLayoutElementsHistoryEntry(
                    // ReSharper restore ObjectCreationAsStatement
                    new List<DrawableElementDataViewModelBase>(), null, 0, 0, "Test");
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("editor", e.ParamName);
            }
        }

        /// <summary>
        /// Tests the successful aggregation of two <see cref="MoveLayoutElementsHistoryEntry"/>s.
        /// </summary>
        [TestMethod]
        public void AggregateSuccessTest()
        {
            var moveX = 10;
            var moveY = 20;
            var existingElement = new StaticTextElementDataViewModel(this.editor.Parent)
                                      {
                                          X = new DataValue<int>(110),
                                          Y = new DataValue<int>(70)
                                      };
            var list = new List<DrawableElementDataViewModelBase>
                           {
                               existingElement
                           };
            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, moveX, moveY, "Test");
            Assert.AreEqual(10, existingEntry.MoveX);
            Assert.AreEqual(20, existingEntry.MoveY);
            this.editor.Elements.Add(existingElement);
            this.editor.SelectedElements.Add(existingElement);
            moveX += 20;
            existingElement.X.Value = 130;

            var newEntry = new MoveLayoutElementsHistoryEntry(
                list, this.editor, 20, 0, "Test2");
            Assert.IsTrue(existingEntry.Aggregate(newEntry));
            Assert.AreEqual(moveX, existingEntry.MoveX);
            Assert.AreEqual(moveY, existingEntry.MoveY);
        }

        /// <summary>
        /// Tests the aggregation of two different history entry types.
        /// The aggregation should return false.
        /// </summary>
        [TestMethod]
        public void AggregateDifferentTypesTest()
        {
            var commandRegistry = new CommandRegistry();
            const int MoveX = 10;
            const int MoveY = 20;
            var existingElement = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(110),
                Y = new DataValue<int>(70)
            };

            var list = new List<DrawableElementDataViewModelBase>
            {
                existingElement
            };

            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, MoveX, MoveY, "Test");
            Assert.AreEqual(10, existingEntry.MoveX);
            Assert.AreEqual(20, existingEntry.MoveY);
            this.editor.Elements.Add(existingElement);
            this.editor.SelectedElements.Add(existingElement);

            var secondEntry = new StaticTextElementDataViewModel(this.editor.Parent)
                                  {
                                      ElementName = new DataValue<string>("SecondEntry")
                                  };
            var newEntry = new CreateLayoutElementHistoryEntry(
                secondEntry, this.editor, commandRegistry, this.state, "Add element");

            Assert.IsFalse(existingEntry.Aggregate(newEntry));
        }

        /// <summary>
        /// Tests the aggregation of 2 <see cref="MoveLayoutElementsHistoryEntry"/>s with different number of elements.
        /// The aggregation should return false.
        /// </summary>
        [TestMethod]
        public void AggregateDifferentNumberOfElementsTest()
        {
            const int MoveX = 10;
            const int MoveY = 20;
            var existingElement1 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(110),
                Y = new DataValue<int>(70)
            };
            var existingElement2 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(50),
                Y = new DataValue<int>(20)
            };
            var list = new List<DrawableElementDataViewModelBase>
                           {
                               existingElement1,
                               existingElement2
                           };
            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, MoveX, MoveY, "Move 2 elements");
            Assert.AreEqual(10, existingEntry.MoveX);
            Assert.AreEqual(20, existingEntry.MoveY);
            this.editor.Elements.AddRange(list);
            this.editor.SelectedElements.Add(existingElement1);

            var newEntry =
                new MoveLayoutElementsHistoryEntry(
                    new List<DrawableElementDataViewModelBase> { existingElement1 },
                    this.editor,
                    20,
                    0,
                    "Move one element");
            Assert.IsFalse(existingEntry.Aggregate(newEntry));
            Assert.AreEqual(MoveX, existingEntry.MoveX);
            Assert.AreEqual(MoveY, existingEntry.MoveY);
        }

        /// <summary>
        /// Tests the aggregation of 2 <see cref="MoveLayoutElementsHistoryEntry"/>s with different elements
        /// in the list.
        /// The aggregation should return false.
        /// </summary>
        [TestMethod]
        public void AggregateDifferentElementsTest()
        {
            const int MoveX = 10;
            const int MoveY = 20;
            var element1 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(110),
                Y = new DataValue<int>(70),
                ElementName = new DataValue<string>("Element1")
            };
            var element2 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(50),
                Y = new DataValue<int>(20),
                ElementName = new DataValue<string>("Element2")
            };
            var list = new List<DrawableElementDataViewModelBase>
                           {
                               element1,
                               element2
                           };
            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, MoveX, MoveY, "Move 2 elements");
            Assert.AreEqual(10, existingEntry.MoveX);
            Assert.AreEqual(20, existingEntry.MoveY);
            this.editor.Elements.AddRange(list);
            this.editor.SelectedElements.AddRange(list);

            var element3 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(10),
                Y = new DataValue<int>(10),
                ElementName = new DataValue<string>("Element3")
            };

            var newEntry = new MoveLayoutElementsHistoryEntry(
                new List<DrawableElementDataViewModelBase> { element1, element3 }, this.editor, 20, 0, "Move elements");
            Assert.IsFalse(existingEntry.Aggregate(newEntry));
            Assert.AreEqual(MoveX, existingEntry.MoveX);
            Assert.AreEqual(MoveY, existingEntry.MoveY);
        }

        /// <summary>
        /// Tests the Undo of a <see cref="MoveLayoutElementsHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void UndoTest()
        {
            var existingElement = new StaticTextElementDataViewModel(this.editor.Parent)
                                      {
                                          X = new DataValue<int>(100),
                                          Y = new DataValue<int>(200)
                                      };
            this.editor.Elements.Add(existingElement);
            this.editor.SelectedElements.Add(existingElement);
            var list = new List<DrawableElementDataViewModelBase> { existingElement };
            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, 10, 10, "Undo test");
            existingEntry.Undo();
            Assert.AreEqual(2, this.editor.Elements.Count);
            Assert.AreEqual(1, this.editor.SelectedElements.Count);
            Assert.AreEqual(90, existingElement.X.Value);
            Assert.AreEqual(190, existingElement.Y.Value);
        }

        /// <summary>
        /// Tests the Redo of a <see cref="MoveLayoutElementsHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void RedoTest()
        {
            var existingElement1 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1")
            };
            var existingElement2 = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(10),
                Y = new DataValue<int>(20),
                ElementName = new DataValue<string>("TestEntry2")
            };
            var list = new List<DrawableElementDataViewModelBase> { existingElement1, existingElement2 };
            this.editor.SelectedElements.AddRange(list);
            this.editor.Elements.AddRange(list);
            Assert.AreEqual(2, this.editor.SelectedElements.Count);
            Assert.AreEqual(3, this.editor.Elements.Count);
            var existingEntry = new MoveLayoutElementsHistoryEntry(list, this.editor, 10, 10, "Redo test");
            existingEntry.Redo();
            Assert.AreEqual(3, this.editor.Elements.Count);
            Assert.AreEqual(2, this.editor.SelectedElements.Count);
            Assert.AreEqual(110, existingElement1.X.Value);
            Assert.AreEqual(210, existingElement1.Y.Value);
            Assert.AreEqual(20, existingElement2.X.Value);
            Assert.AreEqual(30, existingElement2.Y.Value);
        }

        /// <summary>
        /// Tests the handling of the IsDirty flag when used within the <see cref="ChangeHistory"/>.
        /// </summary>
        [TestMethod]
        public void IsDirtyHandlingTest()
        {
            var element = new StaticTextElementDataViewModel(this.editor.Parent)
            {
                X = new DataValue<int>(100),
                Y = new DataValue<int>(200),
                ElementName = new DataValue<string>("TestEntry1")
            };
            var list = new List<DrawableElementDataViewModelBase> { element };
           this.editor.SelectedElements.AddRange(list);
            this.editor.Elements.AddRange(list);
            var entry = new MoveLayoutElementsHistoryEntry(list, this.editor, 10, 10, "Move by 10");
            entry.Do();
            Assert.AreEqual(110, element.X.Value);
            Assert.AreEqual(210, element.Y.Value);
        }
    }
}
