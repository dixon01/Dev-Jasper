// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleCacheTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for XimpleCacheTest and is intended
//   to contain all XimpleCacheTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Tests.Utils
{
    using System.Linq;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for XimpleCacheTest and is intended
    /// to contain all XimpleCacheTest Unit Tests
    /// </summary>
    [TestClass]
    public class XimpleCacheTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddTest()
        {
            var target = new ProtranXimpleCache();
            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                    {
                        ColumnNumber = 0,
                        RowNumber = 0,
                        TableNumber = 0,
                        Value = "Hello World"
                    });

            target.Add(ximple);

            var dump = target.Dump();
            Assert.AreEqual(1, dump.Cells.Count);
            var cell = dump.Cells[0];
            Assert.AreEqual(0, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual("Hello World", cell.Value);
        }

        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddMultipleTest()
        {
            var target = new ProtranXimpleCache();
            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    TableNumber = 0,
                    Value = "Hello World"
                });
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 1,
                    RowNumber = 1,
                    TableNumber = 1,
                    Value = "Test it!"
                });

            target.Add(ximple);

            var dump = target.Dump();
            Assert.AreEqual(2, dump.Cells.Count);
            var cell = dump.Cells.First(c => c.TableNumber == 0);
            Assert.AreEqual(0, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual("Hello World", cell.Value);
            cell = dump.Cells.First(c => c.TableNumber == 1);
            Assert.AreEqual(1, cell.ColumnNumber);
            Assert.AreEqual(1, cell.RowNumber);
            Assert.AreEqual(1, cell.TableNumber);
            Assert.AreEqual("Test it!", cell.Value);
        }

        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddOverwriteTest()
        {
            var target = new ProtranXimpleCache();
            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    TableNumber = 0,
                    Value = "Hello World"
                });

            target.Add(ximple);

            var dump = target.Dump();
            Assert.AreEqual(1, dump.Cells.Count);
            var cell = dump.Cells[0];
            Assert.AreEqual(0, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual("Hello World", cell.Value);

            ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    TableNumber = 0,
                    Value = "Foo Bar"
                });

            target.Add(ximple);

            dump = target.Dump();
            Assert.AreEqual(1, dump.Cells.Count);
            cell = dump.Cells[0];
            Assert.AreEqual(0, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual("Foo Bar", cell.Value);
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [TestMethod]
        public void ClearTest()
        {
            var target = new ProtranXimpleCache();
            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    TableNumber = 0,
                    Value = "Hello World"
                });

            target.Add(ximple);

            var dump = target.Dump();
            Assert.AreEqual(1, dump.Cells.Count);
            var cell = dump.Cells[0];
            Assert.AreEqual(0, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual("Hello World", cell.Value);

            target.Clear();

            dump = target.Dump();
            Assert.AreEqual(0, dump.Cells.Count);
        }
    }
}
