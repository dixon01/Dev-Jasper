// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AHandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AHandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="DS021AHandler"/>
    /// </summary>
    [TestClass]
    public class DS021AHandlerTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

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
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");

            ximple = null;
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "06", "6th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "07", "7th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "08", "8th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "09", "9th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "10", "10th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(10, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);
        }

        /// <summary>
        /// Test to see that the right number of stations is flushed
        /// </summary>
        [TestMethod]
        public void HideLastStopTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

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
        /// Test to see that the right number of stations is flushed
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void HideDestinationTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
                FlushTimeout = TimeSpan.Zero,
                HideLastStop = false,
                HideDestinationBelow = 5, // if there are less than 5 stops left, we hide the destination
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
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "06", "6th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "07", "7th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(7, ximple.Cells.FindAll(c => c.ColumnNumber == 0).Count); // 7 stops
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
            Assert.AreEqual("7th Hst", cell.Value);

            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            // =================================== 2 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("5th Hst", cell.Value);

            // destination is visible as well
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7th Hst", cell.Value);

            // =================================== 3 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 3 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("6th Hst", cell.Value);

            // destination is visible as well
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7th Hst", cell.Value);

            // =================================== 4 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 4 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4th Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7th Hst", cell.Value);

            // destination is not visible anymore
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // =================================== 5 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 5 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("5th Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // destination is not visible anymore
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // =================================== 6 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 6 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("6th Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // destination is not visible anymore
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // =================================== 7 ===================================
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 7 });
            Assert.IsNotNull(ximple);

            // first stop is 2nd Hst
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7th Hst", cell.Value);

            // last stop in our window of 4 stops is visible with the correct name
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // destination is not visible anymore
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);
        }

        /// <summary>
        /// Test to see that the status is changed to NoData if an invalid DS021a index is sent
        /// </summary>
        [TestMethod]
        public void NoDataStatusTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.NoData, status.Value);
        }

        /// <summary>
        /// Test to check that the route is deleted when index 0 is sent
        /// </summary>
        [TestMethod]
        public void DeleteRouteTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                {
                    Enabled = true,
                    EndingStopValue = 99,
                    FlushNumberOfStations = 5,
                    FirstStopIndexValue = 1,
                    FlushTimeout = TimeSpan.Zero,
                    DeleteRouteIndexValue = 0,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 0 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(5, ximple.Cells.Count);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));
        }

        /// <summary>
        /// Test to see that the Ax indexes are handled correctly
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void ConnectionTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                {
                    Enabled = true,
                    EndingStopValue = 99,
                    FlushNumberOfStations = 5,
                    FirstStopIndexValue = 1,
                    FlushTimeout = TimeSpan.FromSeconds(1),
                    Connection =
                        new ConnectionConfig
                            {
                                Enabled = true,
                                UsedFor = new GenericUsage { Column = "Destination", Table = "0", Row = "{0}" },
                                UsedForStopName = new GenericUsage { Column = "StopName", Table = "0", Row = "0" },
                            }
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
                                            new Column { Name = "Destination", Index = 0 },
                                            new Column { Name = "StopName", Index = 1 }
                                        }
                                }
                        }
                };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            var waitEvent = new AutoResetEvent(false);
            var ximples = new List<Ximple>();
            handler.XimpleCreated += (sender, args) =>
                {
                    ximples.Add(args.Ximple);
                    waitEvent.Set();
                };

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });
            handler.HandleInput(new DS010B { StopIndex = 1 });

            // we don't care about the contents of ximple here
            ximples.Clear();
            waitEvent.Reset();

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A1", "2;440;9;Bushof" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A
                                    {
                                        StopData = new[] { "xx", "A2", "2;E10;8;Hauptbahnhof" }
                                    });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A3", "2;123;7;Dortmund" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A4", "2;456;6;Oberhausen" } });
            Assert.IsTrue(ximples.Count == 0);

            Assert.IsTrue(waitEvent.WaitOne(TimeSpan.FromSeconds(2)));
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 5);
            var cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Oberhausen", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Dortmund", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Hauptbahnhof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bushof", cell.Value);

            // update time
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A3", "2;123;10;Dortmund" } });
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 5);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Oberhausen", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Hauptbahnhof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bushof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Dortmund", cell.Value);

            // remove entry
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A4", " " } });
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 5);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Hauptbahnhof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bushof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Dortmund", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // update stop index
            ximples.Clear();
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 4);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2nd Hst", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Hauptbahnhof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bushof", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Dortmund", cell.Value);

            // update stop index past existing connections
            ximples.Clear();
            handler.HandleInput(new DS010B { StopIndex = 3 });
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 4);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // add new connection information
            ximples.Clear();
            waitEvent.Reset();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A1", "3;123;0;Oberb}ren" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A2", "3;aaa;8;Airport" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A3", "3;bbb;6;Chilbi" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A4", "3;CCC;7;Open Air" } });
            Assert.IsTrue(ximples.Count == 0);

            Assert.IsTrue(waitEvent.WaitOne(TimeSpan.FromSeconds(2)));
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 4);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3rd Hst", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Chilbi", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Open Air", cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Airport", cell.Value);

            // clear all
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "A0" } });
            Assert.IsTrue(ximples.Count == 1);
            Assert.IsTrue(ximples[0].Cells.Count == 4);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell = ximples[0].Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);
        }

        /// <summary>
        /// Test to see that the <code>Sx</code> indexes are handled correctly
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void NewstickerTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                             {
                                 Enabled = true,
                                 EndingStopValue = 99,
                                 FlushNumberOfStations = 5,
                                 FirstStopIndexValue = 1,
                                 FlushTimeout = TimeSpan.FromSeconds(1),
                                 UsedForText =
                                     new GenericUsageDS021Base
                                         {
                                             Column = "MessageText",
                                             Table = "PassengerMessages",
                                             Row = "{0}"
                                         },
                             };
            var dictionary = new Dictionary
                                 {
                                     Tables =
                                         {
                                             new Table
                                                 {
                                                     Name = "PassengerMessages",
                                                     Index = 20,
                                                     Columns =
                                                         {
                                                             new Column
                                                                 {
                                                                     Name = "MessageText",
                                                                     Index = 0
                                                                 }
                                                         }
                                                 }
                                         }
                                 };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            var waitEvent = new AutoResetEvent(false);
            var ximples = new List<Ximple>();
            handler.XimpleCreated += (sender, args) =>
                {
                    ximples.Add(args.Ximple);
                    waitEvent.Set();
                };

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S1", "Sondertext auf einen TFT-Anzei" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S2", "ger mit insgesamt maximal 256 " } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S3", "Zeichen. Dieser Text sollte au" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S4", "f dem TFT-Anzeiger des KOM1643" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S5", " darges" } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S6", "tellt werden. " } });
            Assert.IsTrue(ximples.Count == 0);

            // Flush after timeout
            waitEvent.Reset();
            Assert.IsTrue(waitEvent.WaitOne(TimeSpan.FromSeconds(2)));
            Assert.IsTrue(ximples.Count == 1);
            var cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(
                "Sondertext auf einen TFT-Anzeiger mit insgesamt maximal 256 Zeichen. " +
                "Dieser Text sollte auf dem TFT-Anzeiger des KOM1643 dargestellt werden. ",
                cell.Value);

            // Delete text
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S3", " " } });
            Assert.IsTrue(ximples.Count == 1);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(
                "Sondertext auf einen TFT-Anzeiger mit insgesamt maximal 256 f dem "
                + "TFT-Anzeiger des KOM1643 dargestellt werden. ",
                cell.Value);

            // replace a text
            ximples.Clear();
            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S4", "New text for index S4 is here " } });
            Assert.IsTrue(ximples.Count == 1);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(
                "Sondertext auf einen TFT-Anzeiger mit insgesamt maximal 256 "
                + "New text for index S4 is here  dargestellt werden. ",
                cell.Value);

            // text with index 9 is received
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S7", "Text part 7 plus " } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S8", "Text part 8 plus " } });
            Assert.IsTrue(ximples.Count == 0);
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S9", "Text part 9" } });
            Assert.IsTrue(ximples.Count == 1);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(
                "Sondertext auf einen TFT-Anzeiger mit insgesamt maximal 256 "
                + "New text for index S4 is here  dargestellt werden. Text part 7 plus Text part 8 plus Text part 9",
                cell.Value);

            // clear all
            ximples.Clear();
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "S0" } });
            Assert.IsTrue(ximples.Count == 1);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            // set a new text
            ximples.Clear();
            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "S1", "Neuer Text in einer Zelle." } });
            Assert.IsTrue(ximples.Count == 0);

            // Flush after timeout
            waitEvent.Reset();
            Assert.IsTrue(waitEvent.WaitOne(TimeSpan.FromSeconds(2)));
            Assert.IsTrue(ximples.Count == 1);
            cell = ximples[0].Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Neuer Text in einer Zelle.", cell.Value);
        }

        /// <summary>
        /// Test to see that the travel times are calculated correctly
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void TravelTimesTest()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                             {
                                 Enabled = true,
                                 EndingStopValue = 99,
                                 FlushNumberOfStations = 6,
                                 FirstStopIndexValue = 1,
                                 FlushTimeout = TimeSpan.Zero,
                                 DeleteRouteIndexValue = 0,
                                 HideLastStop = false,
                                 UsedFor =
                                     new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" },
                                 UsedForAbsoluteTime =
                                     new GenericUsageDS021Base
                                         {
                                             Column = "StopInfo",
                                             Table = "Stops",
                                             Row = "{0}"
                                         },
                                 UsedForRelativeTime =
                                     new GenericUsageDS021Base
                                         {
                                             Column = "StopTime",
                                             Table = "Stops",
                                             Row = "{0}"
                                         },
                                 UsedForDestinationAbsoluteTime =
                                     new GenericUsage
                                         {
                                             Column = "DestinationInfo",
                                             Table = "Destination",
                                             Row = "0"
                                         },
                                 UsedForDestinationRelativeTime =
                                     new GenericUsage
                                         {
                                             Column = "DestinationTime",
                                             Table = "Destination",
                                             Row = "0"
                                         },
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
                                                 },
                                             new Table
                                                 {
                                                     Name = "Destination",
                                                     Index = 11,
                                                     Columns =
                                                         {
                                                             new Column
                                                                 {
                                                                     Name =
                                                                         "DestinationInfo",
                                                                     Index = 2
                                                                 },
                                                             new Column
                                                                 {
                                                                     Name =
                                                                         "DestinationTime",
                                                                     Index = 3
                                                                 }
                                                         }
                                                 },
                                             new Table
                                                 {
                                                     Name = "Stops",
                                                     Index = 12,
                                                     Columns =
                                                         {
                                                             new Column
                                                                 {
                                                                     Name = "StopInfo",
                                                                     Index = 2
                                                                 },
                                                             new Column
                                                                 {
                                                                     Name = "StopTime",
                                                                     Index = 3
                                                                 }
                                                         }
                                                 }
                                         }
                                 };
            handler.Configure(config, new IbisConfigContextMock(dictionary));
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            Status? status = null;
            handler.StatusChanged += (sender, args) => status = handler.Status;

            TimeProvider.Current = new ManualTimeProvider(new DateTime(2013, 2, 25, 15, 00, 15).ToUniversalTime());

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "01", "1st Hst", string.Empty, "1" } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "02", "2nd Hst", string.Empty, "1", "1" } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "03", "3rd Hst", string.Empty, "1", "2" } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "04", "4th Hst", string.Empty, "1", "3" } });
            Assert.IsNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "05", "5th Hst", string.Empty, "1", "4" } });

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });
            Assert.IsNotNull(ximple);

            // Calculate travel times for stop index = 1
            // Relative time
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("0", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:00", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:03", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("6", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:06", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 4 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("10", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 4 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:10", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("10", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:10", cell.Value);

            // Update of stop index to 2, recalculate relative travel times
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNotNull(ximple);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:03", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("6", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:06", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("10", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:10", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("10", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:10", cell.Value);

            // Update of stop index to 3, recalculate travel times
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 3 });
            Assert.IsNotNull(ximple);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("2", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:02", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("5", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:05", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("9", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:09", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("9", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:09", cell.Value);

            // *********************************************************************************************************
            // Test with empty travel time in a telegram
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 0 });
            Assert.IsNotNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "01", "1st Hst", string.Empty, "1" } });
            Assert.IsNotNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "02", "2nd Hst", string.Empty, "1", "1" } });
            Assert.IsNotNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "03", "3rd Hst", string.Empty, "1", string.Empty } });
            Assert.IsNotNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "04", "4th Hst", string.Empty, "1", "3" } });
            Assert.IsNotNull(ximple);

            handler.HandleInput(
                new DS021A { StopData = new[] { "xx", "05", "5th Hst", string.Empty, "1", "4" } });

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });
            Assert.IsNotNull(ximple);

            // Calculate relative travel times for stop index = 1
            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("0", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:00", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:04", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 4 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("8", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 4 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:08", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("8", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:08", cell.Value);

            // Update of stop index to 2, recalculate relative travel times
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNotNull(ximple);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("1", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:01", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("4", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:04", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("8", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 3 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:08", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("8", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:08", cell.Value);

            // Update of stop index to 3, recalculate travel times
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 3 });
            Assert.IsNotNull(ximple);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("0", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:00", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("3", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:03", cell.Value);

            // Relative time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.RowNumber == 2 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:07", cell.Value);

            // Destination times
            // Relative time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 3);
            Assert.IsNotNull(cell);
            Assert.AreEqual("7", cell.Value);

            // Absolute time
            cell = ximple.Cells.Find(c => c.TableNumber == 11 && c.RowNumber == 0 && c.ColumnNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("15:07", cell.Value);

            // *********************************************************************************************************
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=435</code>:
        /// I use a stop list with <code>"1st Hst", "2nd Hst", "3rd Hst"</code> and so on ...
        /// loading stops 1 till 6 with DS021 and the 99 telegram
        /// send xI00 -> it shows again all stops - this in an error;
        /// xI00 must be ignored because bases on the config the index starts with 01,
        /// so 00 is an invalid index and should not lead to changes in the stop list
        /// </summary>
        [TestMethod]
        public void BugTracker435Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                {
                    Enabled = true,
                    EndingStopValue = 99,
                    FlushNumberOfStations = 10,
                    FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "2nd Hst");

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 0 });
            Assert.IsNull(ximple, "Expected no Ximple for index 0");
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=436</code>:
        /// I use a stop list with <code>"1st Hst", "2nd Hst", "3rd Hst"</code> and so on ...
        /// loading stops 1 till 6 with DS021 and the 99 telegram
        /// ---
        /// send the xI10 telegram => it shows Stop5: <code>"5th Stop"</code> and
        /// Stop6: <code>"6th Stop"</code> only => this is an error because not valid indexes must be ignored
        /// (the trip was only loaded till index 6 so index 10 is not valid)
        /// </summary>
        [TestMethod]
        public void BugTracker436Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 10,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "2nd Hst");

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 10 });
            Assert.IsNull(ximple, "Expected no Ximple for index 10");
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=437</code>:
        /// DS021a: loading stops 1 till 8 (but missing stop 7), send the 99 telegram
        /// ---
        /// send xI06 -> stop1 is the <code>"6th stop"</code> -> okay
        /// but stop2 is the <code>"2nd stop"</code> -> not okay
        /// (Stop2 must stay empty because this stop wasn't transferred before)
        /// ---
        /// send xI07 -> it displays as Stop1 the <code>"6th Stop"</code> - not correct (Stop1 must stay empty)
        /// display for Stop2 is correct (it displays the <code>"8th stop"</code>)
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void BugTracker437Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 10,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "06", "6th Hst" } });
            Assert.IsNull(ximple);
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "08", "8th Hst" } });
            Assert.IsNull(ximple);
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.MissingData, status.Value);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.MissingData, status.Value);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "2nd Hst");

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 6 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "6th Hst");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty, "Expected empty stop in row two");

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 7 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, string.Empty, "Expected empty stop in row one");
            cell = ximple.Cells.Find(c => c.RowNumber == 1 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "8th Hst");

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 8 });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "8th Hst");
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=441</code>:
        /// I send using DS021a stop1, stop2, stop3 and then again stop3 and again stop3 and again stop3.
        /// ---
        /// After sending this last stop3 the PT transfer the data to the player so he shows it.
        /// This is an error because the PT must wait till it receives n (configuration) different
        /// stops indexes before he sends it to player.
        /// In the current implementation it doesn't care so it uses the receiving of the same index
        /// to increase the counter.
        /// </summary>
        [TestMethod]
        public void BugTracker441Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            for (int i = 1; i <= 5; i++)
            {
                handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
                Assert.IsNull(ximple, "Expected no Ximple for stop 3 even if sent {0} times", i);
            }

            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=444</code>:
        /// Sending the 1st route using DS021a and finishing with 99 telegram.
        /// Now send the 3rd stop (index 03) of another (second) route using DS021a.
        /// Set the index to xI03
        /// ---
        /// The Player shows as <code>1st stop</code> the <code>3rd stop</code>
        /// from the second route. This is not correct.
        /// The ProTran has to ignore the route telegram with index 03
        /// (it doesn't start with index 01 as configured in ProTran). So the ProTran has to stay on the existing route.
        /// </summary>
        [TestMethod]
        public void BugTracker444Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "99" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNotNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 3 });
            Assert.IsNotNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 4 });
            Assert.IsNotNull(ximple);

            ximple = null;
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "A new 2nd Stop" } });
            Assert.IsNull(ximple, "Should not send Ximple for invalid first stop index");
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=445</code>:
        /// Sending the 1st three stops using DS021a.
        /// Sending the xI05 telegram.
        /// Continuing to send the remaining stops of the route.
        /// ---
        /// With the current implementation the ProTran doesn't wait till it receives
        /// the <code>5th + n</code> (configuration: FlushNumberOfStations-1) stops
        /// before it sends the stop list to Player.
        /// It sends it as soon as it receives the <code>6th</code> stop.
        /// It has to wait till it receives the <code>10th</code> stop.
        /// </summary>
        [TestMethod]
        public void BugTracker445Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
            {
                Enabled = true,
                EndingStopValue = 99,
                FlushNumberOfStations = 5,
                FirstStopIndexValue = 1,
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

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS010B { StopIndex = 4 });
            Assert.IsNull(
                ximple,
                "Expected to get no Ximple for stop index that doesn't allow to show FlushNumberOfStations = 5 stops.");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "06", "6th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "07", "7th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "08", "8th Hst" } });

            Assert.IsNotNull(ximple);
            Assert.AreEqual(8, ximple.Cells.Count);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "4th Hst");
            Assert.IsNotNull(status);
            Assert.AreEqual(Status.Ok, status.Value);

            ximple = null;
            handler.HandleInput(new DS021A { StopData = new[] { "xx", "09", "9th Hst" } });
            Assert.IsNull(ximple);
        }

        /// <summary>
        /// Test for <code>http://intranet.gorba.com/bugtracker/edit_bug.aspx?id=631</code>:
        /// Sending the 1st seven stops using DS021a.
        /// Sending the xI02 telegram.
        /// ---
        /// With the current implementation the ProTran doesn't flush after flush timeout when it receives index 2
        /// It has to flush the stops based on the index received before the flush timeout.
        /// </summary>
        [TestMethod]
        public void BugTracker631Test()
        {
            var handler = new DS021AHandler();
            var config = new DS021AConfig
                             {
                                 Enabled = true,
                                 EndingStopValue = 99,
                                 FlushNumberOfStations = 6,
                                 FirstStopIndexValue = 1,
                                 FlushTimeout = TimeSpan.FromSeconds(1),
                                 UsedFor =
                                     new GenericUsageDS021Base { Column = "0", Table = "0", Row = "{0}" }
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
            var waitEvent = new AutoResetEvent(false);
            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) =>
                {
                    ximple = args.Ximple;
                    waitEvent.Set();
                };

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "01", "1st Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "02", "2nd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "03", "3rd Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "04", "4th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "05", "5th Hst" } });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "06", "6th Hst" } });
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "1st Hst");

            handler.HandleInput(new DS021A { StopData = new[] { "xx", "07", "7th Hst" } });
            Assert.IsNotNull(ximple);

            // Flush after timeout
            ximple = null;
            waitEvent.Reset();
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsTrue(waitEvent.WaitOne(TimeSpan.FromSeconds(2)));
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.RowNumber == 0 && c.ColumnNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(cell.Value, "2nd Hst");
        }
    }
}
