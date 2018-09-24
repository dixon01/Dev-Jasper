// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Data
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Composer;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Motion.Infomedia.Core.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for TableControllerTest and is intended
    /// to contain all TableControllerTest Unit Tests
    /// </summary>
    [TestClass]
    public class TableControllerTest
    {
        #region Public Methods

        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// A test for GetCellValue
        /// </summary>
        [TestMethod]
        public void GetCellValueTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            target.UpdateData(ximple);
            var expected = "TestValue";
            var actual = target.GetCellValue(0, 0, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetCellValue        
        /// </summary>
        [TestMethod]
        public void GetCellValueTest2()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            target.UpdateData(ximple);
            var expected = "Stop11";
            // ColumnNumber = 1,
            // RowNumber = 10,
            // TableNumber = 10,
            // LanguageNumber = 0,
            var actual = target.GetCellValue(0, 10, 1, 10);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetCellValue
        /// </summary>
        [TestMethod]
        public void GetCellValueWithUnknownColumnTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            target.UpdateData(ximple);
            var table = 0;
            var column = 895;
            var row = 0;
            var lang = 0;

            var actual = target.GetCellValue(lang, table, column, row);
            Assert.IsNull(actual);

            table = 20;
            column = 0;

            actual = target.GetCellValue(lang, table, column, row);
            Assert.IsNull(actual);

            table = 0;
            row = 230;
            actual = target.GetCellValue(lang, table, column, row);
            Assert.IsNull(actual);

            row = 0;
            lang = 111;
            actual = target.GetCellValue(lang, table, column, row);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests if the generic tables are created and/or updated correctly according a given Ximple.
        /// </summary>
        [TestMethod]
        public void UpdateDataTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            target.UpdateData(ximple);
            var column = 0;
            var row = 0;
            int tableNumber;
            var expectedValue = "TestValue";

            var table = target.GetTable(0, 0);
            Assert.IsNotNull(table);
            Assert.AreEqual(1, table.ColumnCount);
            Assert.AreEqual(expectedValue, table.GetCellValue(row, column));

            table = target.GetTable(0, 10);
            Assert.IsNotNull(table);
            Assert.AreEqual(11, table.RowCount);
            Assert.AreEqual(2, table.ColumnCount);

            column = 1;
            row = 8;
            tableNumber = 10;
            expectedValue = "Stop9";
            ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell
                                 {
                                     ColumnNumber = column,
                                     RowNumber = row,
                                     TableNumber = tableNumber,
                                     LanguageNumber = 0,
                                     Value = expectedValue
                                 });
            target.UpdateData(ximple);

            column = 1;
            row = 8;
            tableNumber = 10;
            var language = 2;
            expectedValue = "Haltestelle9";

            ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell
                                 {
                                     ColumnNumber = column,
                                     RowNumber = row,
                                     TableNumber = tableNumber,
                                     LanguageNumber = language,
                                     Value = expectedValue
                                 });
            target.UpdateData(ximple);

            table = target.GetTable(language, tableNumber);
            Assert.IsNotNull(table);
            Assert.AreEqual(9, table.RowCount);
            Assert.AreEqual(expectedValue, table.GetCellValue(row, column));
        }

        /// <summary>
        /// Tests if the generic tables are created and/or updated correctly according a given Ximple
        /// with negative row numbers.
        /// </summary>
        [TestMethod]
        public void UpdateNegativeDataTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            target.UpdateData(ximple);
            var column = 0;
            var row = 0;
            int tableNumber;
            var expectedValue = "TestValue";

            var table = target.GetTable(0, 0);
            Assert.IsNotNull(table);
            Assert.AreEqual(1, table.ColumnCount);
            Assert.AreEqual(expectedValue, table.GetCellValue(row, column));

            table = target.GetTable(0, 10);
            Assert.IsNotNull(table);
            Assert.AreEqual(11, table.RowCount);
            Assert.AreEqual(2, table.ColumnCount);

            column = 1;
            row = 8;
            tableNumber = 10;
            expectedValue = "Stop9";
            ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell
            {
                ColumnNumber = column,
                RowNumber = row,
                TableNumber = tableNumber,
                LanguageNumber = 0,
                Value = expectedValue
            });
            target.UpdateData(ximple);

            column = 1;
            row = -2;
            tableNumber = 10;
            expectedValue = "Stop-2";

            ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell
            {
                ColumnNumber = column,
                RowNumber = row,
                TableNumber = tableNumber,
                LanguageNumber = 0,
                Value = expectedValue
            });
            target.UpdateData(ximple);

            table = target.GetTable(0, tableNumber);
            Assert.IsNotNull(table);
            Assert.AreEqual(13, table.RowCount);
            Assert.AreEqual(expectedValue, table.GetCellValue(row, column));
        }

        /// <summary>
        /// Tests the event DataReceived
        /// </summary>
        [TestMethod]
        public void DataReceivedTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            List<XimpleCell> actual = null;
            target.DataReceived += delegate(object sender, TableEventArgs e) { actual = e.NewValues; };
            target.UpdateData(ximple);
            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(ximple.Cells, actual);

            var ximpleCell = new XimpleCell
                                 {
                                     ColumnNumber = 3,
                                     RowNumber = 1,
                                     TableNumber = 2,
                                     LanguageNumber = 0,
                                     Value = "AddedCell"
                                 };
            ximple.Cells.Add(ximpleCell);
            target.UpdateData(ximple);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(ximpleCell, actual[0]);
        }

        [TestMethod]
        public void DataReceivedCannedMessageTableTest()
        {
            var target = new TableController(new XimpleInactivityConfig());
            var ximple = CreateXimpleStub();
            List<XimpleCell> actual = null;
            target.DataReceived += delegate (object sender, TableEventArgs e) { actual = e.NewValues; };
            target.UpdateData(ximple);
            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(ximple.Cells, actual);

            var ximpleCell = new XimpleCell
            {
                ColumnNumber = 0,
                RowNumber = 1,
                TableNumber = 101,
                LanguageNumber = 2,
                Value = "1"
            };
            ximple.Cells.Add(ximpleCell);
            target.UpdateData(ximple);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(ximpleCell, actual[0]);
        }


        #endregion

        #region Methods

        private static Ximple CreateXimpleStub()
        {
            var ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell
                                 {
                                     ColumnNumber = 0,
                                     RowNumber = 0,
                                     TableNumber = 0,
                                     LanguageNumber = 0,
                                     Value = "TestValue"
                                 });
            ximple.Cells.Add(new XimpleCell
                                 {
                                     ColumnNumber = 1,
                                     RowNumber = 10,
                                     TableNumber = 10,
                                     LanguageNumber = 0,
                                     Value = "Stop11"
                                 });
            return ximple;
        }

        #endregion
    }
}