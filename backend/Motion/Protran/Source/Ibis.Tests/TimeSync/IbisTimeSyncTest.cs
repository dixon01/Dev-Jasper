// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTimeSyncTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTimeSyncTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.TimeSync
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;
    using Gorba.Motion.Protran.Ibis.TimeSync;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="IbisTimeSync"/>.
    /// </summary>
    [TestClass]
    public class IbisTimeSyncTest
    {
        /// <summary>
        /// Tests the simple reception of a DS005.
        /// </summary>
        [TestMethod]
        public void TestDS005()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2013, 9, 22, 17, 23, 44));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var config = new TimeSyncConfig
            {
                Enabled = true,
                InitialDelay = TimeSpan.Zero,
                Tolerance = TimeSpan.Zero,
                WaitTelegrams = 0
            };
            var context = new IbisConfigContextMock(CreateDictionary());
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
            var simplePort = new SimplePort("SystemTime", false, true, new IntegerValues(0, int.MaxValue), 0);
            GioomClient.Instance.RegisterPort(simplePort);
            var target = new IbisTimeSync();
            target.Configure(config, null, context);

            target.Start();
            try
            {
                var telegram = new DS005 { Time = "15:24" };

                Assert.IsTrue(target.Accept(telegram));

                target.HandleInput(telegram);
                Assert.AreEqual(simplePort.Value.Value, 0);
            }
            finally
            {
                target.Stop();
                GioomClient.Instance.DeregisterPort(simplePort);
            }
        }

        /// <summary>
        /// Tests the simple reception of a DS006a.
        /// </summary>
        [TestMethod]
        public void TestDS006A()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2013, 9, 22, 17, 23, 44));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var config = new TimeSyncConfig
            {
                Enabled = true,
                InitialDelay = TimeSpan.Zero,
                Tolerance = TimeSpan.Zero,
                WaitTelegrams = 0
            };
            var context = new IbisConfigContextMock(CreateDictionary());
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
            var simplePort = new SimplePort("SystemTime", false, true, new IntegerValues(0, int.MaxValue), 0);
            GioomClient.Instance.RegisterPort(simplePort);
            var target = new IbisTimeSync();
            target.Configure(config, null, context);

            target.Start();
            try
            {
                var telegram = new DS006A { DateTime = "22092013152422" };

                Assert.IsTrue(target.Accept(telegram));

                target.HandleInput(telegram);
                Assert.AreEqual(simplePort.Value.Value, 1379856262);
            }
            finally
            {
                target.Stop();
                GioomClient.Instance.DeregisterPort(simplePort);
            }
        }

        /// <summary>
        /// Tests the simple reception of a DS006.
        /// </summary>
        [TestMethod]
        public void TestDS006()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2013, 9, 22, 17, 23, 44));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var config = new TimeSyncConfig
            {
                Enabled = true,
                InitialDelay = TimeSpan.Zero,
                Tolerance = TimeSpan.Zero,
                WaitTelegrams = 0
            };
            var context = new IbisConfigContextMock(CreateDictionary());
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
            var simplePort = new SimplePort("SystemTime", false, true, new IntegerValues(0, int.MaxValue), 0);
            GioomClient.Instance.RegisterPort(simplePort);
            var target = new IbisTimeSync();
            target.Configure(config, null, context);

            target.Start();
            try
            {
                var telegram = new DS006 { Date = "24.09.2013" };

                Assert.IsTrue(target.Accept(telegram));

                target.HandleInput(telegram);
                Assert.AreEqual(simplePort.Value.Value, 0);
            }
            finally
            {
                target.Stop();
                GioomClient.Instance.DeregisterPort(simplePort);
            }
        }

        /// <summary>
        /// Tests the simple reception of a DS006 and DS005.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestDS006DS005()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2013, 9, 22, 17, 23, 44));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var config = new TimeSyncConfig
            {
                Enabled = true,
                InitialDelay = TimeSpan.Zero,
                Tolerance = TimeSpan.Zero,
                WaitTelegrams = 0
            };
            var context = new IbisConfigContextMock(CreateDictionary());
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
            var simplePort = new SimplePort("SystemTime", false, true, new IntegerValues(0, int.MaxValue), 0);
            GioomClient.Instance.RegisterPort(simplePort);
            var target = new IbisTimeSync();
            target.Configure(config, null, context);

            target.Start();
            try
            {
                var telegram = new DS006 { Date = "24.09.2013" };

                Assert.IsTrue(target.Accept(telegram));

                target.HandleInput(telegram);
                Assert.AreEqual(simplePort.Value.Value, 0);

                var timeTelegram = new DS005 { Time = "15:24" };

                Assert.IsTrue(target.Accept(timeTelegram));

                target.HandleInput(timeTelegram);
                Assert.AreEqual(simplePort.Value.Value, 1380029040);
            }
            finally
            {
                target.Stop();
                GioomClient.Instance.DeregisterPort(simplePort);
            }
        }

        private static Dictionary CreateDictionary()
        {
            return new Dictionary
            {
                Languages = { new Language { Name = "L", Index = 0 } },
                Tables = { new Table { Name = "T", Index = 0, Columns = { new Column { Name = "C", Index = 2 } } } }
            };
        }
    }
}
