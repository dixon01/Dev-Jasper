// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CHandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CHandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="DS021CHandler"/>
    /// </summary>
    [TestClass]
    public class DS021CHandlerTest
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
        /// Test to see that the right number of stations is flushed
        /// </summary>
        [TestMethod]
        public void FlushNumberOfStationsTest()
        {
            var handler = new DS021CHandler();
            var config = new DS021CConfig
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
                FlushTimeout = TimeSpan.Zero,
                TakeDestinationFromLastStop = false,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(new DS021C { StopData = new[] { "0", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "05", "5th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");

            ximple = null;
            handler.HandleInput(new DS021C { StopData = new[] { "1", "06", "6th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "07", "7th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "08", "8th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "09", "9th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "1", "10", "10th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(10, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");

            ximple = null;
            handler.HandleInput(new DS021C { StopData = new[] { "2", "11", "11th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(11, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
        }

        /// <summary>
        /// Test to see that the right number of stations is flushed
        /// </summary>
        [TestMethod]
        public void HideNextStopTest()
        {
            var handler = new DS021CHandler();
            var config = new DS021CConfig
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
                FlushTimeout = TimeSpan.Zero,
                TakeDestinationFromLastStop = false,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(new DS021C { StopData = new[] { "0", "01", "1st Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "02", "2nd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "03", "3rd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "2", "04", "4th Hst" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010J { Data = 2002 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010J { Data = 3002 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            ximple = null;
            handler.HandleInput(new DS010J { Data = 2003 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Hst", cell.Value);

            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);
        }

        /// <summary>
        /// Test to see that the destination is taken from DS021c index 101
        /// </summary>
        [TestMethod]
        public void DestinationFrom101Test()
        {
            var handler = new DS021CHandler();
            var config = new DS021CConfig
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
                FlushTimeout = TimeSpan.Zero,
                TakeDestinationFromLastStop = false,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                UsedForDestination = new GenericUsageDS021Base { Column = "1", Table = "0", Row = "0" }
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
                                            new Column { Name = "A", Index = 0 },
                                            new Column { Name = "B", Index = 1 }
                                        }
                                }
                        }
                };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            var cache = new XimpleCache();
            handler.XimpleCreated += (sender, args) => cache.Add(args.Ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "0", "001", "1st Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "101", "Final Destination" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "002", "2nd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "003", "3rd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "2", "004", "4th Hst" } });

            var ximple = cache.Dump();
            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.FindAll(c => c.ColumnNumber == 0).Count); // 4 stops
            Assert.AreEqual(1, ximple.Cells.FindAll(c => c.ColumnNumber == 1).Count); // 1 destination

            // first stop is 1st Hst
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            // destination is visible as well
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Final Destination", cell.Value);
        }

        /// <summary>
        /// Test to see that the destination is taken from DS021c last stop
        /// </summary>
        [TestMethod]
        public void DestinationFromLastStopTest()
        {
            var handler = new DS021CHandler();
            var config = new DS021CConfig
            {
                Enabled = true,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
                FlushTimeout = TimeSpan.Zero,
                TakeDestinationFromLastStop = true,
                UsedFor = new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                UsedForDestination = new GenericUsageDS021Base { Column = "1", Table = "0", Row = "0" }
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
                                            new Column { Name = "A", Index = 0 },
                                            new Column { Name = "B", Index = 1 }
                                        }
                                }
                        }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            var cache = new ProtranXimpleCache();
            handler.XimpleCreated += (sender, args) => cache.Add(args.Ximple);

            handler.HandleInput(new DS021C { StopData = new[] { "0", "001", "1st Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "101", "Final Destination" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "002", "2nd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "1", "003", "3rd Hst" } });
            handler.HandleInput(new DS021C { StopData = new[] { "2", "004", "4th Hst" } });

            var ximple = cache.Dump();
            Assert.AreEqual(4, ximple.Cells.FindAll(c => c.ColumnNumber == 0).Count); // 4 stops
            Assert.AreEqual(1, ximple.Cells.FindAll(c => c.ColumnNumber == 1).Count); // 1 destination

            // first stop is 1st Hst
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1st Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            // destination is visible as well
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);
        }
    }
}
