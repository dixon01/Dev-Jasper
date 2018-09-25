// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005HandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO005HandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for GO005HandlerTest and is intended
    /// to contain all GO005Handler Unit Tests
    /// </summary>
    [TestClass]
    public class GO005HandlerTest
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
        /// Test to see that the right number of stations is flushed at the right time.
        /// </summary>
        [TestMethod]
        public void GO005FlushTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0001", "1st Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0002", "2nd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0003", "3rd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0004", "4th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0005", "5th Hst\n  " } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");

            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0006", "6th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0007", "7th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0008", "8th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0009", "9th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0010", "10th Hst\n  " } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(10, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            status = null;
            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0011", "11th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L05 0012   " } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(11, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNull(status);
        }

        /// <summary>
        /// Test to see that the right number of stations is flushed
        /// </summary>
        [TestMethod]
        public void HideLastStopTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                HideLastStop = true,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0001", "1st Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0002", "2nd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0003", "3rd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0004", "4th Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0005   " } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            // last stop has to be an empty one
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);
        }

        /// <summary>
        /// Test to see that the next stop is empty when a 999 stop index is sent
        /// and check that the ASCII line number is transmitted correctly for the current stop.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void HideNextStopTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                HideNextStopForIndex = 999,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                AsciiLineNumberUsedFor = new GenericUsageDS021Base { Column = "1", Table = "0", Row = "0" }
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
                                            new Column { Name = "C", Index = 0 },
                                            new Column { Name = "D", Index = 1 }
                                        }
                                }
                        }
                };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0001", "1st Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0002", "2nd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L8  0003", "3rd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L8  0004", "4th Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L8  0005   " } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L7", cell.Value);

            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L7", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0002" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L7", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L7", cell.Value);

            // special case: we go back from 0999 to the last stop index, this
            // should also trigger an update with all stops visible again
            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0002" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L7", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0003" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("L8", cell.Value);
        }

        /// <summary>
        /// Test to see that the next stop is empty when a 999 stop index is sent
        /// and check that the ASCII line number is transmitted correctly for the current stop.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void BufferNextRouteTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 6,
                FlushTimeout = TimeSpan.Zero,
                BufferNextRoute = true,
                HideNextStopForIndex = 999,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            /////////////////////////////////////////// 1 ///////////////////////////////////////////

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0001", "1st Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0002", "2nd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0003", "3rd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0004", "4th Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0005   " } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0002" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0003" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0004" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // send the next route (has to be buffered)
            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L9  0001", "1st Stop\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L9  0002", "2nd Stop\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L9  0003", "3rd Stop\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L9  0004   " } });

            Assert.IsNull(ximple);

            // still on first route
            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            /////////////////////////////////////////// 2 ///////////////////////////////////////////

            // now load the new stop list
            ximple = null;
            var cache = new XimpleCache(); // we merge all ximples we get since we will get more than one
            handler.XimpleCreated += (s, e) => cache.Add(e.Ximple);
            handler.HandleInput(new DS010 { StopIndex = "0001" });

            // use the merged Ximple from the cache
            ximple = cache.Dump();
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Stop", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Stop", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Stop", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0002" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Stop", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // start sending the next route (has to be buffered again)
            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0001", "1st Halt\n  " } });

            // intermediate (same) index update has no effect
            handler.HandleInput(new DS010 { StopIndex = "0002" });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0002", "2nd Halt\n  " } });

            Assert.IsNull(ximple);

            // still on first route
            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0999" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Stop", cell.Value);

            // continue sending the next route (still buffered)
            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0003", "3rd Halt\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0004", "4th Halt\n  " } });
            Assert.IsNull(ximple);

            /////////////////////////////////////////// 3 ///////////////////////////////////////////

            // now load the third stop list (currently empty since we didn't flush yet)
            ximple = null;
            cache.Clear();
            handler.HandleInput(new DS010 { StopIndex = "0001" });

            // use the merged Ximple from the cache
            ximple = cache.Dump();
            Assert.AreEqual(3, ximple.Cells.Count);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));

            // finish sending the next route (not buffered)
            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0005", "5th Halt\n  " } });
            Assert.IsNull(ximple); // no flush

            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0006", "6th Halt\n  " } });
            Assert.IsNotNull(ximple); // intermediate flush

            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0007", "7th Halt\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0008", "8th Halt\n  " } });
            Assert.IsNull(ximple); // no flush

            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0009   " } });
            Assert.IsNotNull(ximple); // final flush
            Assert.AreEqual(8, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 4 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("5th Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 5 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("6th Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 6 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7th Halt", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 7 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("8th Halt", cell.Value);
        }

        /// <summary>
        /// Test to see that the right number of stations is flushed at the right time
        /// without having care about the run's value (as the customer STOAG does).
        /// </summary>
        [TestMethod]
        public void DeleteRouteTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                DeleteRoute = false
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0001", "1st Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0002", "2nd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0003", "3rd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0004", "4th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0005", "5th Hst\n  " } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0000" });
            Assert.IsNull(ximple);
        }

        /// <summary>
        /// Test to see that the right number of stations is flushed at the right time
        /// having care also about the run's value (specific for the STOAG customer).
        /// </summary>
        [TestMethod]
        public void DeleteRouteStoagTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                DeleteRoute = true // for the STOAG customer, we need to take care also about the run's value.
            };

            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            // now I send to the GO005 an information having the run's value.
            handler.HandleInput(new DS002 { RunNumber = 0x00 });

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0001", "1st Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0002", "2nd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0003", "3rd Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0004", "4th Hst\n  " } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new GO005 { StopData = new[] { "xx", "L05 0005", "5th Hst\n  " } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0000" });

            // having sent the run's value 0x00 and the stop index 0x00
            // I expect that the stops are now cleared, so, I expect
            // and XIMPLE with empty cells.
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));
        }

        /// <summary>
        /// Test to see that the the current route is not deleted (when routes are buffered)
        /// if the lower value index of x0000 is received (specific for the STOAG customer).
        /// </summary>
        [TestMethod]
        public void LoadNewRouteStoagTest()
        {
            var handler = new GO005Handler();
            var config = new GO005Config
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FlushTimeout = TimeSpan.Zero,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                DeleteRoute = true // for the STOAG customer, we need to take care also about the run's value.
            };

            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0001", "1st Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0002", "2nd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0003", "3rd Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0004", "4th Hst\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L7  0005   " } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0002" });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            ximple = null;
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0001", "1st Halt\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0002", "2nd Halt\n  " } });
            handler.HandleInput(new GO005 { StopData = new[] { "xx", "L23 0003", "3rd Halt\n  " } });

            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0000" });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010 { StopIndex = "0001" });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));
        }
    }
}
