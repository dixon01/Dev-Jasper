// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080HandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Test for
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
    /// Test for <see cref="DS080Handler"/>
    /// </summary>
    [TestClass]
    public class DS080HandlerTest
    {
        /// <summary>
        /// Simple test for DS080
        /// </summary>
        [TestMethod]
        public void SimpleDS080Test()
        {
            var handler = new DS080Handler();
            var config = new DS080Config
            {
                Enabled = true,
                UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "0" },
                OpenValue = "1",
                CloseValue = "0"
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(new DS080());
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("1", ximple.Cells[0].Value);
        }

        /// <summary>
        /// Test to see if DS010b resets the value set by DS080
        /// </summary>
        [TestMethod]
        public void DS080ResetWithDS010BTest()
        {
            var handler = new DS080Handler();
            var config = new DS080Config
            {
                Enabled = true,
                UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "0" },
                OpenValue = "1",
                CloseValue = "0",
                ResetWithDS010B = true
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("0", ximple.Cells[0].Value);

            handler.HandleInput(new DS080());
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("1", ximple.Cells[0].Value);

            // no Ximple is emitted when same index is sent
            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("0", ximple.Cells[0].Value);
        }

        /// <summary>
        /// Test to see if DS010b does not reset the value set by DS080 if not configured
        /// </summary>
        [TestMethod]
        public void DS080ResetWithoutDS010BTest()
        {
            var handler = new DS080Handler();
            var config = new DS080Config
            {
                Enabled = true,
                UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "0" },
                OpenValue = "1",
                CloseValue = "0",
                ResetWithDS010B = false
            };
            var dictionary = new Dictionary
            {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
            };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            handler.HandleInput(new DS080());
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("1", ximple.Cells[0].Value);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 1 });
            Assert.IsNull(ximple);

            ximple = null;
            handler.HandleInput(new DS010B { StopIndex = 2 });
            Assert.IsNull(ximple);
        }
    }
}
