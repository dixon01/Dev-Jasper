// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004HandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004HandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Handlers
{
    using System;

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
    /// This is a test class for GO002HandlerTest and is intended
    /// to contain all GO002HandlerTest Unit Tests
    /// </summary>
    [TestClass]
    public class GO004HandlerTest
    {
        private static readonly Dictionary Dictionary = new Dictionary
            {
                Tables =
                    {
                        new Table
                            {
                                Name = "PassengerMessages",
                                Index = 20,
                                Columns =
                                    {
                                        new Column { Name = "MessageType", Index = 0 },
                                        new Column { Name = "MessageTitle", Index = 1 },
                                        new Column { Name = "MessageText", Index = 2 }
                                    }
                            }
                    }
            };

        /// <summary>
        /// Initializes the necessary test resources.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            SetupHelper.SetupCoreServices();
            TimeProvider.ResetToDefault();
        }

        /// <summary>
        /// A simple test for GO002Handler
        /// </summary>
        [TestMethod]
        public void SimpleGO004Test()
        {
            Ximple ximple = null;
            var handler = new GO004Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (s, e) => ximple = e.Ximple;

            var telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 99999999,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);

            var type = ximple.Cells.Find(c => c.ColumnNumber == 0);
            Assert.IsNotNull(type);
            Assert.AreEqual("1", type.Value);
            var title = ximple.Cells.Find(c => c.ColumnNumber == 1);
            Assert.IsNotNull(title);
            Assert.AreEqual("Hello", title.Value);
            var text = ximple.Cells.Find(c => c.ColumnNumber == 2);
            Assert.IsNotNull(text);
            Assert.AreEqual("World[br]Second line", text.Value);
        }

        /// <summary>
        /// A test for GO002Handler using time ranges
        /// </summary>
        [TestMethod]
        public void TimeRangeGO004Test()
        {
            TimeProvider.Current = new ManualTimeProvider(new DateTime(2012, 09, 22, 14, 23, 15));

            Ximple ximple = null;
            var handler = new GO004Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (s, e) => ximple = e.Ximple;

            var telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 8001400,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNull(ximple);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 14002200,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);

            var type = ximple.Cells.Find(c => c.ColumnNumber == 0);
            Assert.IsNotNull(type);
            Assert.AreEqual("1", type.Value);
            var title = ximple.Cells.Find(c => c.ColumnNumber == 1);
            Assert.IsNotNull(title);
            Assert.AreEqual("Hello", title.Value);
            var text = ximple.Cells.Find(c => c.ColumnNumber == 2);
            Assert.IsNotNull(text);
            Assert.AreEqual("World[br]Second line", text.Value);
        }

        /// <summary>
        /// A test for GO002Handler using inverted time ranges (start time &gt; end time)
        /// </summary>
        [TestMethod]
        public void TimeRangeInvertedGO004Test()
        {
            TimeProvider.Current = new ManualTimeProvider(new DateTime(2012, 09, 22, 14, 23, 15));

            Ximple ximple = null;
            var handler = new GO004Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (s, e) => ximple = e.Ximple;

            var telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 22001400,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNull(ximple);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 14000800,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);

            var type = ximple.Cells.Find(c => c.ColumnNumber == 0);
            Assert.IsNotNull(type);
            Assert.AreEqual("1", type.Value);
            var title = ximple.Cells.Find(c => c.ColumnNumber == 1);
            Assert.IsNotNull(title);
            Assert.AreEqual("Hello", title.Value);
            var text = ximple.Cells.Find(c => c.ColumnNumber == 2);
            Assert.IsNotNull(text);
            Assert.AreEqual("World[br]Second line", text.Value);
        }

        /// <summary>
        /// A test for GO002Handler that deletes a single entry
        /// </summary>
        [TestMethod]
        public void DeleteSingleGO004Test()
        {
            Ximple ximple = null;
            var handler = new GO004Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (s, e) => ximple = e.Ximple;

            var telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 99999999,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 2,
                MessageType = 1,
                TimeRange = 99999999,
                MessageParts = new[] { "Another", "Message" }
            };

            ximple = null;
            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(6, ximple.Cells.Count);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 0,
                TimeRange = 0,
                MessageParts = new string[0]
            };

            ximple = null;
            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(6, ximple.Cells.Count);

            var type = ximple.Cells.Find(c => c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(type);
            Assert.AreEqual("1", type.Value);
            var title = ximple.Cells.Find(c => c.ColumnNumber == 1 && c.RowNumber == 0);
            Assert.IsNotNull(title);
            Assert.AreEqual("Another", title.Value);
            var text = ximple.Cells.Find(c => c.ColumnNumber == 2 && c.RowNumber == 0);
            Assert.IsNotNull(text);
            Assert.AreEqual("Message", text.Value);

            type = ximple.Cells.Find(c => c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(type);
            Assert.AreEqual(string.Empty, type.Value);
            title = ximple.Cells.Find(c => c.ColumnNumber == 1 && c.RowNumber == 1);
            Assert.IsNotNull(title);
            Assert.AreEqual(string.Empty, title.Value);
            text = ximple.Cells.Find(c => c.ColumnNumber == 2 && c.RowNumber == 1);
            Assert.IsNotNull(text);
            Assert.AreEqual(string.Empty, text.Value);
        }

        /// <summary>
        /// A test for GO002Handler that deletes a all entries
        /// </summary>
        [TestMethod]
        public void DeleteAllGO004Test()
        {
            Ximple ximple = null;
            var handler = new GO004Handler();
            handler.Configure(CreateConfig(), new IbisConfigContextMock(Dictionary));
            handler.XimpleCreated += (s, e) => ximple = e.Ximple;

            var telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 1,
                MessageType = 1,
                TimeRange = 99999999,
                MessageParts = new[] { "Hello", "World", "Second line" }
            };

            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(3, ximple.Cells.Count);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 2,
                MessageType = 1,
                TimeRange = 99999999,
                MessageParts = new[] { "Another", "Message" }
            };

            ximple = null;
            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(6, ximple.Cells.Count);

            telegram = new GO004
            {
                IbisAddress = 10,
                MessageIndex = 0,
                MessageType = 0,
                TimeRange = 0,
                MessageParts = new string[0]
            };

            ximple = null;
            handler.HandleInput(telegram);
            Assert.IsNotNull(ximple);
            Assert.AreEqual(6, ximple.Cells.Count);

            var type = ximple.Cells.Find(c => c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(type);
            Assert.AreEqual(string.Empty, type.Value);
            var title = ximple.Cells.Find(c => c.ColumnNumber == 1 && c.RowNumber == 0);
            Assert.IsNotNull(title);
            Assert.AreEqual(string.Empty, type.Value);
            var text = ximple.Cells.Find(c => c.ColumnNumber == 2 && c.RowNumber == 0);
            Assert.IsNotNull(text);
            Assert.AreEqual(string.Empty, text.Value);

            type = ximple.Cells.Find(c => c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(type);
            Assert.AreEqual(string.Empty, type.Value);
            title = ximple.Cells.Find(c => c.ColumnNumber == 1 && c.RowNumber == 1);
            Assert.IsNotNull(title);
            Assert.AreEqual(string.Empty, title.Value);
            text = ximple.Cells.Find(c => c.ColumnNumber == 2 && c.RowNumber == 1);
            Assert.IsNotNull(text);
            Assert.AreEqual(string.Empty, text.Value);
        }

        private static GO004Config CreateConfig()
        {
            return new GO004Config
                       {
                           UsedFor =
                               new GenericUsage
                                   {
                                       Column = "MessageText",
                                       Table = "PassengerMessages",
                                       Row = "{0}"
                                   },
                           UsedForTitle =
                               new GenericUsage
                                   {
                                       Column = "MessageTitle",
                                       Table = "PassengerMessages",
                                       Row = "{0}"
                                   },
                           UsedForType =
                               new GenericUsage
                                   {
                                       Column = "MessageType",
                                       Table = "PassengerMessages",
                                       Row = "{0}"
                                   }
                       };
        }
    }
}