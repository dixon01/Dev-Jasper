// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistoryTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines tests for the change history.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Tests
{
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the <see cref="ChangeHistory"/>.
    /// </summary>
    [TestClass]
    public class ChangeHistoryTest
    {
        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var applicationStateMock = new Mock<IApplicationState>(MockBehavior.Strict);
            applicationStateMock.Setup(state => state.ClearDirty());
            applicationStateMock.Setup(state => state.MakeDirty());
            var serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            serviceLocatorMock.Setup(locator => locator.GetInstance<IApplicationState>())
                              .Returns(applicationStateMock.Object);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
        }

        /// <summary>
        /// Tests adding entries of different types.
        /// </summary>
        [TestMethod]
        public void AddDifferentTypesTest()
        {
            var target = new ChangeHistory();
            var entry = new TestEntry("Entry 1");
            target.Add(entry);
            Assert.AreEqual(1, target.UndoStack.Count);
            var actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(1, actualEntry.Value);
            var entry2 = new SecondTestEntry("Entry 2");
            target.Add(entry2);
            Assert.AreEqual(2, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual("Entry 2", actualEntry.DisplayText);
        }

        /// <summary>
        /// Tests adding entries of the same type that can be aggregated.
        /// </summary>
        [TestMethod]
        public void AddAggregatedTest()
        {
            var target = new ChangeHistory();
            IHistoryEntry entry = new TestEntry("Entry 1");
            target.Add(entry);
            Assert.AreEqual(1, target.UndoStack.Count);
            var actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(1, actualEntry.Value);
            IHistoryEntry entry2 = new TestEntry("Entry 2");
            target.Add(entry2);
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(2, actualEntry.Value);
        }

        /// <summary>
        /// Tests undoing an entry.
        /// </summary>
        [TestMethod]
        public void SimpleUndoTest()
        {
            var target = new ChangeHistory();
            IHistoryEntry entry = new TestEntry("Entry");
            target.Add(entry);
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            target.Undo();
            Assert.AreEqual(0, target.UndoStack.Count);
            Assert.AreEqual(1, target.RedoStack.Count);
            var actualEntry = target.RedoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(0, actualEntry.Value);
        }

        /// <summary>
        /// Tests undoing an aggregated entry.
        /// </summary>
        [TestMethod]
        public void UndoAggregatedTest()
        {
            var target = new ChangeHistory();
            IHistoryEntry entry = new TestEntry("Entry 1");
            target.Add(entry);
            IHistoryEntry entry2 = new TestEntry("Entry 2");
            target.Add(entry2);
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            target.Undo();
            Assert.AreEqual(0, target.UndoStack.Count);
            Assert.AreEqual(1, target.RedoStack.Count);
            var actualEntry = target.RedoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(0, actualEntry.Value);
        }

        /// <summary>
        /// Tests redoing an entry.
        /// </summary>
        [TestMethod]
        public void SimpleRedoTest()
        {
            var target = new ChangeHistory();
            IHistoryEntry entry = new TestEntry("Entry");
            target.Add(entry);
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            target.Undo();
            Assert.AreEqual(0, target.UndoStack.Count);
            Assert.AreEqual(1, target.RedoStack.Count);
            target.Redo();
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            var actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(1, actualEntry.Value);
        }

        /// <summary>
        /// Tests redoing an aggregated entry.
        /// </summary>
        [TestMethod]
        public void RedoAggregatedTest()
        {
            var target = new ChangeHistory();
            IHistoryEntry entry = new TestEntry("Entry 1");
            target.Add(entry);
            IHistoryEntry entry2 = new TestEntry("Entry 2");
            target.Add(entry2);
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            target.Undo();
            Assert.AreEqual(0, target.UndoStack.Count);
            Assert.AreEqual(1, target.RedoStack.Count);
            target.Redo();
            Assert.AreEqual(1, target.UndoStack.Count);
            Assert.AreEqual(0, target.RedoStack.Count);
            var actualEntry = target.UndoStack.First() as TestEntry;
            Assert.IsNotNull(actualEntry);
            Assert.AreEqual(2, actualEntry.Value);
        }

        private class TestEntry : HistoryEntryBase
        {
            private int aggregateLevel = 1;

            public TestEntry(string displayText)
                : base(displayText)
            {
                this.Value = 1;
            }

            public int Value { get; private set; }

            public override bool Aggregate(IHistoryEntry otherEntry)
            {
                if (this.GetType() != otherEntry.GetType())
                {
                    return false;
                }

                this.Value++;
                this.aggregateLevel++;
                return true;
            }

            public override void Undo()
            {
                for (var i = 0; i < this.aggregateLevel; i++)
                {
                    this.Value--;
                }
            }

            public override void Do()
            {
                for (var i = 0; i < this.aggregateLevel; i++)
                {
                    this.Value++;
                }
            }
        }

        private class SecondTestEntry : TestEntry
        {
            public SecondTestEntry(string entry)
                : base(entry)
            {
            }
        }
    }
}