// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO007HandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO007HandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for GO007 Handler
    /// </summary>
    [TestClass]
    public class GO007HandlerTest
    {
        /// <summary>
        /// Initializes the necessary test resources.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            SetupHelper.SetupCoreServices();
        }

        /// <summary>
        /// The GO007 flush test.
        /// </summary>
        [TestMethod]
        public void GO007FlushTest()
        {
            var handler = new GO007Handler();
            var config = new GO007Config
                             {
                                 Enabled = true,
                                 UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "{0}" },
                             };
            var dictionary = new Dictionary
                                 {
                                     Tables =
                                         {
                                             new Table
                                                 {
                                                     Name = "T",
                                                     Index = 0,
                                                     Columns =
                                                         {
                                                             new Column
                                                                 {
                                                                     Name = "C",
                                                                     Index = 0
                                                                 }
                                                         }
                                                 }
                                         }
                                 };

            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(
                new GO007
                    {
                        StopData = new[] { string.Empty, "0413", "Stop1\u00051", "Stop2\u00052", "Stop3\u00053" }
                    });
            Assert.IsNull(ximple);
            handler.HandleInput(new DS009 { StopName = "Stop1" });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop1");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop3");

            ximple = null;
            handler.HandleInput(new DS009 { StopName = "Stop2" });
            Assert.AreEqual(3, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop3");
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);
        }

        /// <summary>
        /// Test to see the last stop is hidden
        /// </summary>
        [TestMethod]
        public void HideLastStopTest()
        {
            var handler = new GO007Handler();
            var config = new GO007Config
            {
                Enabled = true,
                UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "{0}" },
                HideLastStop = true
            };
            var dictionary = new Dictionary
            {
                Tables =
                                         {
                                             new Table
                                                 {
                                                     Name = "T",
                                                     Index = 0,
                                                     Columns =
                                                         {
                                                             new Column
                                                                 {
                                                                     Name = "C",
                                                                     Index = 0
                                                                 }
                                                         }
                                                 }
                                         }
            };

            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(
                new GO007
                    {
                        StopData = new[] { string.Empty, "0413", "Stop1\u00051", "Stop2\u00052", "Stop3\u00053" }
                    });
            Assert.IsNull(ximple);
            handler.HandleInput(new DS009 { StopName = "Stop1" });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop1");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);

            ximple = null;
            handler.HandleInput(new DS009 { StopName = "Stop2" });
            Assert.AreEqual(3, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);
        }

        /// <summary>
        /// Test hide destination below.
        /// </summary>
        [TestMethod]
        public void HideDestinationTest()
        {
            var handler = new GO007Handler();
            var config = new GO007Config
            {
                Enabled = true,
                UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "{0}" },
                UsedForDestination = new GenericUsage { Column = "1", Table = "0", Row = "0" },
                HideDestinationBelow = 3
            };
            var dictionary = new Dictionary
            {
                Tables =
                                         {
                                             new Table
                                                 {
                                                     Name = "T",
                                                     Index = 0,
                                                     Columns =
                                                         {
                                                             new Column
                                                             {
                                                                 Name = "A",
                                                                 Index = 0
                                                             },
                                                             new Column
                                                             {
                                                                 Name = "B",
                                                                 Index = 1
                                                             }
                                                         }
                                                 }
                                         }
            };

            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(
                new GO007
                    {
                        StopData = new[] { string.Empty, "0413", "Stop1\u00051", "Stop2\u00052", "Stop3\u00053" }
                    });
            Assert.IsNull(ximple);
            handler.HandleInput(new DS009 { StopName = "Stop1" });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            Assert.AreEqual(3, ximple.Cells.FindAll(c => c.ColumnNumber == 0).Count);
            Assert.AreEqual(1, ximple.Cells.FindAll(c => c.ColumnNumber == 1).Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop1");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop3");
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop3");

            ximple = null;
            handler.HandleInput(new DS009 { StopName = "Stop2" });
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop2");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "Stop3");
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty);
        }
    }
}
