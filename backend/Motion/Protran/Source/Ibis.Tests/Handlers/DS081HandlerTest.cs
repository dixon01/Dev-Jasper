// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS081HandlerTest.cs" company="Gorba AG">
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
    /// Test for <see cref="DS081Handler"/>
    /// </summary>
    [TestClass]
    public class DS081HandlerTest
    {
        /// <summary>
        /// Simple test for DS081
        /// </summary>
        [TestMethod]
        public void SimpleDS081Test()
        {
            var handler = new DS081Handler();
            var config = new DS081Config
                {
                    Enabled = true,
                    UsedFor = new GenericUsage { Column = "0", Table = "0", Row = "0" },
                    Value = "0"
                };
            var dictionary = new Dictionary
                {
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 0 } } } }
                };
            handler.Configure(config, new IbisConfigContextMock(dictionary));

            Ximple ximple = null;
            handler.XimpleCreated += (sender, args) => ximple = args.Ximple;

            handler.HandleInput(new DS081());
            Assert.IsNotNull(ximple);
            Assert.AreEqual(1, ximple.Cells.Count);
            Assert.AreEqual("0", ximple.Cells[0].Value);
        }
    }
}