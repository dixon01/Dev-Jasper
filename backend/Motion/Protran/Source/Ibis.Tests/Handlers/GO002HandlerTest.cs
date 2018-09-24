// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002HandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for GO002HandlerTest and is intended
//   to contain all GO002HandlerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for GO002HandlerTest and is intended
    /// to contain all GO002HandlerTest Unit Tests
    /// </summary>
    [TestClass]
    public class GO002HandlerTest
    {
        private const string AheadValidExpression = @"^([\d]{1,3})' früher$";
        private const string DelayedValidExpression = @"^([\d]{1,3})' später$";
        private const string ExpressionOk = "ok";

        private static readonly Dictionary Dictionary = new Dictionary
        {
            Tables =
            {
                new Table
                {
                    Name = "Connections",
                    Index = 13,
                    Columns =
                    {
                        new Column { Name = "0", Index = 0 },
                        new Column { Name = "2", Index = 2 },
                        new Column { Name = "3", Index = 3 },
                        new Column { Name = "7", Index = 7 },
                        new Column { Name = "9", Index = 9 },
                        new Column { Name = "4", Index = 4 }
                    }
                }
            }
        };

        private readonly Regex aheadValidator = new Regex(AheadValidExpression);
        private readonly Regex delayedValidator = new Regex(DelayedValidExpression);
        private readonly Regex validatorinTime = new Regex(ExpressionOk);

        /// <summary>
        /// Initializes the necessary test resources.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            SetupHelper.SetupCoreServices();
        }

        /// <summary>
        /// A test for GO002Handler Validity
        /// </summary>
        [TestMethod]
        public void ValidityTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002 = new GO002
                               {
                                   StopIndex = 9,
                                   RowNumber = 0,
                                   Pictogram = "0",
                                   LineNumber = "00032",
                                   DepartureTime = "1641",
                                   TrackNumber = "1a",
                                   Deviation = "6' später",
                                   Data = "Biel"
                               };

            handler.HandleInput(tlgGo002);
            Assert.IsNull(ximple);
        }

        /// <summary>
        /// A test for GO002Handler
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void RowIndex9Test()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "12345",
                                        TrackNumber = "8",
                                        Deviation = "ok",
                                        Data = "Biel"
                                    };
            var tlgGo002Second = new GO002
                                     {
                                         StopIndex = 1,
                                         RowNumber = 2,
                                         Pictogram = "2",
                                         LineNumber = "321AB",
                                         TrackNumber = "4C",
                                         Deviation = "5' später",
                                         Data = "Bern"
                                     };
            var tlgGo002Third = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 3,
                                        Pictogram = "3",
                                        LineNumber = "WESAS",
                                        TrackNumber = "56",
                                        Deviation = "5' früher",
                                        Data = "Zurich"
                                    };
            var tlgGo002Fourth = new GO002
                                     {
                                         StopIndex = 1,
                                         RowNumber = 4,
                                         Pictogram = "4",
                                         LineNumber = "COSTO",
                                         TrackNumber = "1A",
                                         Deviation = "ok",
                                         Data = "Solothurn"
                                     };
            var tlgGo002Fifth = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 9,
                                        Pictogram = "5",
                                        LineNumber = "RANNO",
                                        TrackNumber = "A5",
                                        Deviation = "ok",
                                        Data = "Neuchatel"
                                    };

            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fourth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNotNull(ximple);

            var cell = ximple.Cells.Find(c => c.Value == "Biel");
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);

            cell = ximple.Cells.Find(c => c.Value == "Bern");
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(1, cell.RowNumber);

            cell = ximple.Cells.Find(c => c.Value == "Zurich");
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(2, cell.RowNumber);

            cell = ximple.Cells.Find(c => c.Value == "Solothurn");
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(3, cell.RowNumber);

            cell = ximple.Cells.Find(c => c.Value == "Neuchatel");
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(4, cell.RowNumber);

            Assert.AreEqual(30, ximple.Cells.Count);

            // Pictogram Test
            cell = ximple.Cells.Find(c => c.Value == "1");
            Assert.IsNotNull(cell);

            cell = ximple.Cells.Find(c => c.Value == "2");
            Assert.IsNotNull(cell);

            cell = ximple.Cells.Find(c => c.Value == "3");
            Assert.IsNotNull(cell);

            cell = ximple.Cells.Find(c => c.Value == "4");
            Assert.IsNotNull(cell);

            cell = ximple.Cells.Find(c => c.Value == "5");
            Assert.IsNotNull(cell);

            // Line Number test
            cell = ximple.Cells.Find(c => c.Value == "12345");
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "321AB");
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "WESAS");
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "COSTO");
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "RANNO");
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.ColumnNumber);

            // Track Number test
            cell = ximple.Cells.Find(c => c.Value == "8");
            Assert.IsNotNull(cell);
            Assert.AreEqual(7, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "4C");
            Assert.IsNotNull(cell);
            Assert.AreEqual(7, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "56");
            Assert.IsNotNull(cell);
            Assert.AreEqual(7, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "1A");
            Assert.IsNotNull(cell);
            Assert.AreEqual(7, cell.ColumnNumber);

            cell = ximple.Cells.Find(c => c.Value == "A5");
            Assert.IsNotNull(cell);
            Assert.AreEqual(7, cell.ColumnNumber);

            // Schedule deviation test
            cell = ximple.Cells.Find(c => c.TableNumber == 13 && c.ColumnNumber == 9 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            var isValid = this.validatorinTime.IsMatch(cell.Value);
            Assert.IsTrue(isValid);

            cell = ximple.Cells.Find(c => c.TableNumber == 13 && c.ColumnNumber == 9 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            isValid = this.delayedValidator.IsMatch(cell.Value);
            Assert.IsTrue(isValid);

            cell = ximple.Cells.Find(c => c.TableNumber == 13 && c.ColumnNumber == 9 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            isValid = this.aheadValidator.IsMatch(cell.Value);
            Assert.IsTrue(isValid);
        }

        /// <summary>
        /// A test for GO002Handler with DS010B
        /// </summary>
        [TestMethod]
        public void ReceivedDS010BDuringGO002CollectionTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "12345",
                                        TrackNumber = "8",
                                        Deviation = "ok",
                                        Data = "Biel"
                                    };
            var tlgGo002Second = new GO002
                                     {
                                         StopIndex = 1,
                                         RowNumber = 2,
                                         Pictogram = "2",
                                         LineNumber = "321AB",
                                         TrackNumber = "4C",
                                         Deviation = "5' später",
                                         Data = "Bern"
                                     };
            var tlgGo002Third = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 9,
                                        Pictogram = "5",
                                        LineNumber = "RANNO",
                                        TrackNumber = "A5",
                                        Deviation = "ok",
                                        Data = "Neuchatel"
                                    };

            ximple = null;
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            // now I'll send a DS010B, this should not send anything since we didn't get a a row 9 yet
            ximple = null;
            var ds10BConfig = new SimpleTelegramConfig();
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(tlgGo002Third);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(18, ximple.Cells.Count);
        }

        /// <summary>
        /// A test for GO002Handler with DS010B
        /// </summary>
        [TestMethod]
        public void ReceivedDS010BTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "12345",
                                        TrackNumber = "8",
                                        Deviation = "ok",
                                        Data = "Biel"
                                    };
            var tlgGo002Second = new GO002
                                     {
                                         StopIndex = 1,
                                         RowNumber = 2,
                                         Pictogram = "2",
                                         LineNumber = "321AB",
                                         TrackNumber = "4C",
                                         Deviation = "5' später",
                                         Data = "Bern"
                                     };
            var tlgGo002Third = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 9,
                                        Pictogram = "5",
                                        LineNumber = "RANNO",
                                        TrackNumber = "A5",
                                        Deviation = "ok",
                                        Data = "Neuchatel"
                                    };

            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            // now I'll send a DS010B in order ot force a flush for the sop index 1.
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(18, ximple.Cells.Count);
        }

        /// <summary>
        /// A test for GO002Handler with DS010j
        /// </summary>
        [TestMethod]
        public void ReceivedDS010JTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002First = new GO002
            {
                StopIndex = 1,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "12345",
                TrackNumber = "8",
                Deviation = "ok",
                Data = "Biel"
            };
            var tlgGo002Second = new GO002
            {
                StopIndex = 1,
                RowNumber = 2,
                Pictogram = "2",
                LineNumber = "321AB",
                TrackNumber = "4C",
                Deviation = "5' später",
                Data = "Bern"
            };
            var tlgGo002Third = new GO002
            {
                StopIndex = 1,
                RowNumber = 9,
                Pictogram = "5",
                LineNumber = "RANNO",
                TrackNumber = "A5",
                Deviation = "ok",
                Data = "Neuchatel"
            };

            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            // now I'll send a DS010B in order ot force a flush for the sop index 1.
            handler.HandleInput(new DS010J { Status = 2, StopIndex = 1 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(18, ximple.Cells.Count);
        }

        /// <summary>
        /// A test for GO002Handler with DS021a
        /// </summary>
        [TestMethod]
        public void ReceivedDS021ATest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "12345",
                                        TrackNumber = "8",
                                        Deviation = "ok",
                                        Data = "Biel"
                                    };
            var tlgGo002Second = new GO002
                                     {
                                         StopIndex = 1,
                                         RowNumber = 2,
                                         Pictogram = "2",
                                         LineNumber = "321AB",
                                         TrackNumber = "4C",
                                         Deviation = "5' später",
                                         Data = "Bern"
                                     };
            var tlgGo002Third = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 9,
                                        Pictogram = "5",
                                        LineNumber = "RANNO",
                                        TrackNumber = "A5",
                                        Deviation = "ok",
                                        Data = "Neuchatel"
                                    };

            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNotNull(ximple);

            // now I'll invoke a clear operation sending a valid DS021A.
            ximple = null;
            handler.HandleInput(new DS021A { StopData = new[] { "10", "1" } });

            // also clearing the protran's connections produces a XIMPLE.
            Assert.IsNotNull(ximple);
            Assert.IsTrue(ximple.Cells.Count > 0);

            // now I check that all the cells in the last XIMPLE produced are empty.
            foreach (var cell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, cell.Value);
            }

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);
        }

        /// <summary>
        /// A test for GO002Handler Flush
        /// </summary>
        [TestMethod]
        public void FlushTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            var tlgGo002 = new GO002
                               {
                                   StopIndex = 1,
                                   RowNumber = 1,
                                   Pictogram = "1",
                                   LineNumber = "000B1",
                                   DepartureTime = "0800",
                                   TrackNumber = "01",
                                   Deviation = "ok",
                                   Data = "Brügg"
                               };
            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 1,
                                        RowNumber = 9,
                                        Pictogram = "1",
                                        LineNumber = "000B1",
                                        DepartureTime = "0900",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Bern"
                                    };

            handler.HandleInput(tlgGo002);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNotNull(ximple);
        }

        /// <summary>
        /// This test will send to the GO002Handler a bunch of GO002 telegrams in order to collect
        /// several connections sets, with not ordered stop indexes. Also, sending several DSO10B telegrams
        /// will be asserted the right connections set (always the closest has to be flushed).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void ClosestStopIndexAnyStopsTest()
        {
            Ximple ximple = null;
            var handler = new GO002Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            // here starts the connection set for the STOP INDEX 5
            var tlgGo002First = new GO002
                                    {
                                        StopIndex = 5,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "00005",
                                        DepartureTime = "0800",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Brügg"
                                    };
            var tlgGo002Second = new GO002
                                     {
                                         StopIndex = 5,
                                         RowNumber = 2,
                                         Pictogram = "1",
                                         LineNumber = "00005",
                                         DepartureTime = "0900",
                                         TrackNumber = "01",
                                         Deviation = "ok",
                                         Data = "Bern"
                                     };
            var tlgGo002Third = new GO002
                                    {
                                        StopIndex = 5,
                                        RowNumber = 9,
                                        Pictogram = "1",
                                        LineNumber = "00005",
                                        DepartureTime = "0900",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Biel"
                                    };

            // here ends the connection set for the STOP INDEX 5

            // here starts the connection set for the STOP INDEX 3
            var tlgGo002Forth = new GO002
                                    {
                                        StopIndex = 3,
                                        RowNumber = 1,
                                        Pictogram = "1",
                                        LineNumber = "00003",
                                        DepartureTime = "0800",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "New York"
                                    };
            var tlgGo002Fifth = new GO002
                                    {
                                        StopIndex = 3,
                                        RowNumber = 2,
                                        Pictogram = "1",
                                        LineNumber = "00003",
                                        DepartureTime = "0900",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Miami"
                                    };
            var tlgGo002Sixth = new GO002
                                    {
                                        StopIndex = 3,
                                        RowNumber = 9,
                                        Pictogram = "1",
                                        LineNumber = "00003",
                                        DepartureTime = "0900",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Los Angeles"
                                    };

            // here ends the connection set for the STOP INDEX 3

            // here starts the connection set for the STOP INDEX 8
            var tlgGo002Seventh = new GO002
                                      {
                                          StopIndex = 8,
                                          RowNumber = 1,
                                          Pictogram = "1",
                                          LineNumber = "00008",
                                          DepartureTime = "0800",
                                          TrackNumber = "01",
                                          Deviation = "ok",
                                          Data = "Milan"
                                      };
            var tlgGo002Eigth = new GO002
                                    {
                                        StopIndex = 8,
                                        RowNumber = 2,
                                        Pictogram = "1",
                                        LineNumber = "00008",
                                        DepartureTime = "0900",
                                        TrackNumber = "01",
                                        Deviation = "ok",
                                        Data = "Rome"
                                    };
            var tlgGo002Nineth = new GO002
                                     {
                                         StopIndex = 8,
                                         RowNumber = 9,
                                         Pictogram = "1",
                                         LineNumber = "00008",
                                         DepartureTime = "0900",
                                         TrackNumber = "01",
                                         Deviation = "ok",
                                         Data = "Venice"
                                     };

            // here ends the connection set for the STOP INDEX 8

            // now I fill the handler with all the telegrams above
            // (no flushes because there's no DS010b yet)
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Forth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Sixth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Seventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eigth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Nineth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2.
            // I expect the connection set referring to the stop index 3
            var ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 3.
            ximple = null;
            ds010B = new DS010B { StopIndex = 3 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);

            // now I send an update for stop index 3
            tlgGo002Forth.Pictogram = "2";
            ximple = null;
            handler.HandleInput(tlgGo002Forth);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 4.
            // I expect the connection set referring to the stop index 5
            ximple = null;
            ds010B = new DS010B { StopIndex = 4 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00005");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 7.
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 7 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 8.
            ximple = null;
            ds010B = new DS010B { StopIndex = 8 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send the same index again, we should not get any ximple
            ximple = null;
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send an update for a different stop than the one displayed,
            // so we shouldn't get any ximple
            ximple = null;
            tlgGo002Fifth.Deviation = "7' später";
            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 9.
            // I expect a clear of the connection set because I've never sent to the handeler that connection set.
            ximple = null;
            ds010B = new DS010B { StopIndex = 9 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            foreach (var ximpleCell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, ximpleCell.Value);
            }
        }

        /// <summary>
        /// This test will send to the GO002Handler a bunch of GO002 telegrams in order to collect
        /// several connections sets, with not ordered stop indexes. Also, sending several DSO10B telegrams
        /// will be asserted the right connections set (always the closest has to be flushed).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void ClosestStopIndexNextStopOnlyTest()
        {
            Ximple ximple = null;
            var config = CreateConfig();
            config.ShowForNextStopOnly = true;
            var handler = new GO002Handler();
            handler.Configure(config, new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            ximple = null;

            // here starts the connection set for the STOP INDEX 5
            var tlgGo002First = new GO002
            {
                StopIndex = 5,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Brügg"
            };
            var tlgGo002Second = new GO002
            {
                StopIndex = 5,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Bern"
            };
            var tlgGo002Third = new GO002
            {
                StopIndex = 5,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Biel"
            };

            // here ends the connection set for the STOP INDEX 5

            // here starts the connection set for the STOP INDEX 3
            var tlgGo002Forth = new GO002
            {
                StopIndex = 3,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "New York"
            };
            var tlgGo002Fifth = new GO002
            {
                StopIndex = 3,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Miami"
            };
            var tlgGo002Sixth = new GO002
            {
                StopIndex = 3,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Los Angeles"
            };

            // here ends the connection set for the STOP INDEX 3

            // here starts the connection set for the STOP INDEX 8
            var tlgGo002Seventh = new GO002
            {
                StopIndex = 8,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Milan"
            };
            var tlgGo002Eigth = new GO002
            {
                StopIndex = 8,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Rome"
            };
            var tlgGo002Nineth = new GO002
            {
                StopIndex = 8,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Venice"
            };

            // here ends the connection set for the STOP INDEX 8

            // now I fill the handler with all the telegrams above
            // (no flushes because there's no DS010b yet)
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Forth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Sixth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Seventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eigth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Nineth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2.
            // I expect no connection set
            ximple = null;
            var ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 3.
            // I expect the connection set referring to the stop index 3
            ximple = null;
            ds010B = new DS010B { StopIndex = 3 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 4.
            // I expect an empty connection set
            int cellCount = ximple.Cells.Count;
            ximple = null;
            ds010B = new DS010B { StopIndex = 4 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(cellCount, ximple.Cells.Count);
            foreach (var ximpleCell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, ximpleCell.Value);
            }

            // now I send a DS010B with stop index equal to 8.
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 8 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 9.
            // I expect a clear of the connection set because I've never sent to the handeler that connection set.
            ximple = null;
            ds010B = new DS010B { StopIndex = 9 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            foreach (var ximpleCell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, ximpleCell.Value);
            }
        }

        /// <summary>
        /// This test will send to the GO002Handler a bunch of GO002 telegrams in order to collect
        /// several connections sets, with not ordered stop indexes. Also, sending several DSO10B telegrams
        /// will be asserted the right connections set (always the closest has to be flushed).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void ClosestStopIndexDeletePassedTest()
        {
            Ximple ximple = null;
            var config = CreateConfig();
            config.DeletePassedStops = true;
            var handler = new GO002Handler();
            handler.Configure(config, new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            ximple = null;

            // here starts the connection set for the STOP INDEX 5
            var tlgGo002First = new GO002
            {
                StopIndex = 5,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Brügg"
            };
            var tlgGo002Second = new GO002
            {
                StopIndex = 5,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Bern"
            };
            var tlgGo002Third = new GO002
            {
                StopIndex = 5,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Biel"
            };

            // here ends the connection set for the STOP INDEX 5

            // here starts the connection set for the STOP INDEX 3
            var tlgGo002Forth = new GO002
            {
                StopIndex = 3,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "New York"
            };
            var tlgGo002Fifth = new GO002
            {
                StopIndex = 3,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Miami"
            };
            var tlgGo002Sixth = new GO002
            {
                StopIndex = 3,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Los Angeles"
            };

            // here ends the connection set for the STOP INDEX 3

            // here starts the connection set for the STOP INDEX 8
            var tlgGo002Seventh = new GO002
            {
                StopIndex = 8,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Milan"
            };
            var tlgGo002Eigth = new GO002
            {
                StopIndex = 8,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Rome"
            };
            var tlgGo002Nineth = new GO002
            {
                StopIndex = 8,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Venice"
            };

            // here ends the connection set for the STOP INDEX 8

            // here starts the connection set for the STOP INDEX 11
            var tlgGo002Tenth = new GO002
            {
                StopIndex = 11,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Vienna"
            };
            var tlgGo002Eleventh = new GO002
            {
                StopIndex = 11,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Salzburg"
            };
            var tlgGo002Twelfth = new GO002
            {
                StopIndex = 11,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Klagenfurt"
            };

            // here ends the connection set for the STOP INDEX 11

            // now I fill the handler with all the telegrams above
            // (no flushes because there's no DS010b yet)
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Forth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Sixth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Seventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eigth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Nineth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Tenth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eleventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Twelfth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2.
            // I expect the connection set referring to the stop index 3
            var ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 1 (jump backwards).
            // I expect the connection set referring to the stop index 3
            ds010B = new DS010B { StopIndex = 1 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 3 (jump forward).
            ximple = null;
            ds010B = new DS010B { StopIndex = 3 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);

            // now I send an update for stop index 3
            tlgGo002Forth.Pictogram = "2";
            ximple = null;
            handler.HandleInput(tlgGo002Forth);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 4.
            // I expect the connection set referring to the stop index 5
            ximple = null;
            ds010B = new DS010B { StopIndex = 4 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00005");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 6 (jump forward).
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 6 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 4 (jump backwards).
            // I expect the connection set referring to the stop index 8 (set 5 was deleted)
            ximple = null;
            ds010B = new DS010B { StopIndex = 4 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 7.
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 7 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 8.
            ximple = null;
            ds010B = new DS010B { StopIndex = 8 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send the same index again, we should not get any ximple
            ximple = null;
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send an update for a different stop than the one displayed,
            // so we shouldn't get any ximple
            ximple = null;
            tlgGo002Fifth.Deviation = "7' später";
            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2 (jump backwards).
            ximple = null;
            ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNull(cell);

            // now I send again information for stop 5 (1/3)
            // I expect no update (no index 9 yet)
            ximple = null;
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            // now I send again information for stop 5 (2/3)
            // I expect no update (no index 9 yet)
            ximple = null;
            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            // now I send again information for stop 5 (3/3)
            // I expect to see stop 5 (index 9 is now available)
            ximple = null;
            handler.HandleInput(tlgGo002Third);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00005");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 12.
            // I expect a clear of the connection set because I've never sent to the handeler that connection set.
            ximple = null;
            ds010B = new DS010B { StopIndex = 12 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            foreach (var ximpleCell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, ximpleCell.Value);
            }

            // now I send a DS010B with stop index equal to 7 (jump backwards).
            // I expect no data since everything has been deleted previously
            ximple = null;
            ds010B = new DS010B { StopIndex = 7 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);
        }

        /// <summary>
        /// This test will send to the GO002Handler a bunch of GO002 telegrams in order to collect
        /// several connections sets, with not ordered stop indexes. Also, sending several DSO10B telegrams
        /// will be asserted the right connections set (always the closest has to be flushed).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ClosestStopIndexPreviousStopOnlyTest()
        {
            Ximple ximple = null;
            var config = CreateConfig();
            config.DeletePassedStops = true;
            var handler = new GO002Handler();
            handler.Configure(config, new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            ximple = null;

            // here starts the connection set for the STOP INDEX 5
            var tlgGo002First = new GO002
            {
                StopIndex = 5,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Brügg"
            };
            var tlgGo002Second = new GO002
            {
                StopIndex = 5,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Bern"
            };
            var tlgGo002Third = new GO002
            {
                StopIndex = 5,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Biel"
            };

            // here ends the connection set for the STOP INDEX 5

            // here starts the connection set for the STOP INDEX 3
            var tlgGo002Forth = new GO002
            {
                StopIndex = 3,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "New York"
            };
            var tlgGo002Fifth = new GO002
            {
                StopIndex = 3,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Miami"
            };
            var tlgGo002Sixth = new GO002
            {
                StopIndex = 3,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Los Angeles"
            };

            // here ends the connection set for the STOP INDEX 3

            // here starts the connection set for the STOP INDEX 8
            var tlgGo002Seventh = new GO002
            {
                StopIndex = 8,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Milan"
            };
            var tlgGo002Eigth = new GO002
            {
                StopIndex = 8,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Rome"
            };
            var tlgGo002Nineth = new GO002
            {
                StopIndex = 8,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Venice"
            };

            // here ends the connection set for the STOP INDEX 8

            // here starts the connection set for the STOP INDEX 11
            var tlgGo002Tenth = new GO002
            {
                StopIndex = 11,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Vienna"
            };
            var tlgGo002Eleventh = new GO002
            {
                StopIndex = 11,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Salzburg"
            };
            var tlgGo002Twelfth = new GO002
            {
                StopIndex = 11,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Klagenfurt"
            };

            // here ends the connection set for the STOP INDEX 11

            // now I fill the handler with all the telegrams above
            // (no flushes because there's no DS010b yet)
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Forth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Sixth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Seventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eigth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Nineth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Tenth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eleventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Twelfth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2.
            // I expect the connection set referring to the stop index 3
            var ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 1 (jump backwards).
            // I expect the connection set referring to the stop index 3
            ds010B = new DS010B { StopIndex = 1 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 3 (jump forward).
            ximple = null;
            ds010B = new DS010B { StopIndex = 3 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 2.
            // I expect the connection set referring to the stop index 5
            ximple = null;
            ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00005");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 6 (jump forward).
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 6 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 8.
            ximple = null;
            ds010B = new DS010B { StopIndex = 8 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 11.
            ximple = null;
            ds010B = new DS010B { StopIndex = 11 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00011");
            Assert.IsNotNull(cell);

            // now I send a DS010B with stop index equal to 2 (jump backwards).
            ximple = null;
            ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            foreach (var ximpleCell in ximple.Cells)
            {
                Assert.AreEqual(string.Empty, ximpleCell.Value);
            }
        }

        /// <summary>
        /// This test will send to the GO002Handler a bunch of GO002 telegrams in order to collect
        /// several connections sets, with not ordered stop indexes. Also, sending several DSO10B telegrams
        /// will be asserted the right connections set (always the closest has to be flushed).
        /// In addition this test also deletes connection information with "empty" telegrams.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ClearDataWithEmptyTelegramsTest()
        {
            Ximple ximple = null;

            var config = CreateConfig();
            config.DeletePassedStops = true;
            config.ShowForNextStopOnly = true;

            var handler = new GO002Handler();
            handler.Configure(config, new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;
            ximple = null;

            // here starts the connection set for the STOP INDEX 5
            var tlgGo002First = new GO002
            {
                StopIndex = 5,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Brügg"
            };
            var tlgGo002Second = new GO002
            {
                StopIndex = 5,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Bern"
            };
            var tlgGo002Third = new GO002
            {
                StopIndex = 5,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00005",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Biel"
            };

            // here ends the connection set for the STOP INDEX 5

            // here starts the connection set for the STOP INDEX 3
            var tlgGo002Forth = new GO002
            {
                StopIndex = 3,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "New York"
            };
            var tlgGo002Fifth = new GO002
            {
                StopIndex = 3,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Miami"
            };
            var tlgGo002Sixth = new GO002
            {
                StopIndex = 3,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00003",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Los Angeles"
            };

            // here ends the connection set for the STOP INDEX 3

            // here starts the connection set for the STOP INDEX 8
            var tlgGo002Seventh = new GO002
            {
                StopIndex = 8,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Milan"
            };
            var tlgGo002Eigth = new GO002
            {
                StopIndex = 8,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Rome"
            };
            var tlgGo002Nineth = new GO002
            {
                StopIndex = 8,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00008",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Venice"
            };

            // here ends the connection set for the STOP INDEX 8

            // here starts the connection set for the STOP INDEX 11
            var tlgGo002Tenth = new GO002
            {
                StopIndex = 11,
                RowNumber = 1,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0800",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Vienna"
            };
            var tlgGo002Eleventh = new GO002
            {
                StopIndex = 11,
                RowNumber = 2,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Salzburg"
            };
            var tlgGo002Twelfth = new GO002
            {
                StopIndex = 11,
                RowNumber = 9,
                Pictogram = "1",
                LineNumber = "00011",
                DepartureTime = "0900",
                TrackNumber = "01",
                Deviation = "ok",
                Data = "Klagenfurt"
            };

            // here ends the connection set for the STOP INDEX 11

            // now I fill the handler with all the telegrams above
            // (no flushes because there's no DS010b yet)
            handler.HandleInput(tlgGo002First);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Second);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Third);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Forth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Fifth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Sixth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Seventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eigth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Nineth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Tenth);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Eleventh);
            Assert.IsNull(ximple);

            handler.HandleInput(tlgGo002Twelfth);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 1.
            // I expect no connection set
            var ds010B = new DS010B { StopIndex = 1 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 2.
            // I expect no connection set
            ds010B = new DS010B { StopIndex = 2 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 3.
            // I expect the connection set referring to the stop index 3
            ds010B = new DS010B { StopIndex = 3 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            var cell = ximple.Cells.Find(c => c.Value == "00003");
            Assert.IsNotNull(cell);
            ximple = null;

            // now I send a DS010B with stop index equal to 4.
            // I expect an empty connection set
            ds010B = new DS010B { StopIndex = 4 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));

            // now I send a DS010B with stop index equal to 5.
            // I expect the connection set referring to the stop index 5
            ximple = null;
            ds010B = new DS010B { StopIndex = 5 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00005");
            Assert.IsNotNull(cell);
            ximple = null;

            // now I send a GO002 clearing data for stop index 5
            // I expect an empty connection set
            var go002ClearStop5 = new GO002 { DataLength = 2, StopIndex = 5 };
            handler.HandleInput(go002ClearStop5);
            Assert.IsNotNull(ximple);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));
            ximple = null;

            // now I send a DS010B with stop index equal to 6.
            // I expect no connection set
            ds010B = new DS010B { StopIndex = 6 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 7.
            // I expect no connection set
            ds010B = new DS010B { StopIndex = 7 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 8.
            // I expect the connection set referring to the stop index 8
            ximple = null;
            ds010B = new DS010B { StopIndex = 8 };
            handler.HandleInput(ds010B);
            Assert.IsNotNull(ximple);
            cell = ximple.Cells.Find(c => c.Value == "00008");
            Assert.IsNotNull(cell);
            ximple = null;

            // now I send a GO002 clearing all data
            // I expect an empty connection set
            var go002ClearAll = new GO002 { DataLength = 0 };
            handler.HandleInput(go002ClearAll);
            Assert.IsNotNull(ximple);
            ximple.Cells.ForEach(c => Assert.AreEqual(string.Empty, c.Value));
            ximple = null;

            // now I send a DS010B with stop index equal to 9.
            // I expect no connection set
            ds010B = new DS010B { StopIndex = 9 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 10.
            // I expect no connection set
            ds010B = new DS010B { StopIndex = 10 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);

            // now I send a DS010B with stop index equal to 11.
            // I expect no connection set (it was cleared before)
            ds010B = new DS010B { StopIndex = 11 };
            handler.HandleInput(ds010B);
            Assert.IsNull(ximple);
        }

        private static GO002Config CreateConfig()
        {
            return new GO002Config
                {
                    CheckLength = true,
                    StopIndexSize = 2,
                    RowNumberSize = 1,
                    PictogramSize = 1,
                    LineNumberSize = 5,
                    TrackNumberSize = 2,
                    ScheduleDeviationSize = 4,
                    FirstStopIndex = 1,
                    FirstRowIndex = 1,
                    PictogramFormat = "D:\\Infomedia\\Symbols\\Conn_{0}.png",
                    LineNumberFormat = "D:\\Infomedia\\Symbols\\L{0}.png",
                    ScheduleDeviation =
                        new ScheduleDeviation { OnTime = "ok", Ahead = "{0}' früher", Delayed = "{0}' später" },
                    DeletePassedStops = false,
                    ShowForNextStopOnly = false,
                    UsedForPictogram = new GenericUsage { Column = "0", Table = "Connections", Row = "{0}" },
                    UsedForLineNumber = new GenericUsage { Column = "2", Table = "Connections", Row = "{0}" },
                    UsedForDepartureTime = new GenericUsage { Column = "3", Table = "Connections", Row = "{0}" },
                    UsedForTrackNumber = new GenericUsage { Column = "7", Table = "Connections", Row = "{0}" },
                    UsedForScheduleDeviation = new GenericUsage { Column = "9", Table = "Connections", Row = "{0}" },
                    UsedFor = new GenericUsage { Column = "4", Table = "Connections", Row = "{0}" }
                };
        }
    }
}
