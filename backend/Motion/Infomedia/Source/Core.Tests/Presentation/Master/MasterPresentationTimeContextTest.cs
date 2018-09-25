// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterPresentationTimeContextTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterPresentationTimeContextTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Master
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="MasterPresentationTimeContext"/>.
    /// </summary>
    [TestClass]
    public class MasterPresentationTimeContextTest
    {
        /// <summary>
        /// Tests <see cref="MasterPresentationTimeContext.AddTimeElapsedHandler"/>
        /// and <see cref="MasterPresentationTimeContext.AddTimeReachedHandler"/>.
        /// </summary>
        [TestMethod]
        public void TestTimers()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var target = new MasterPresentationTimeContext();
            target.NextTimeReached += (s, e) => target.NotifyTimeReached(e.Time);

            var timer = timerFactory[target.GetType().Name].First();
            var timer1Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(20), time => timer1Elapsed = true);
            Assert.IsFalse(timer1Elapsed);

            var timer2Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(30), time => timer2Elapsed = true);
            Assert.IsFalse(timer2Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(20));
            timer.RaiseElapsed();
            Assert.IsTrue(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            timer1Elapsed = false;

            timeProvider.AddTime(TimeSpan.FromSeconds(5));

            var timer3Elapsed = false;
            var timer3Time = new DateTime(2000, 1, 1, 12, 0, 40, DateTimeKind.Utc);
            target.AddTimeReachedHandler(timer3Time, time => timer3Elapsed = time >= timer3Time);
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(5));
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsTrue(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            timer2Elapsed = false;

            timeProvider.AddTime(TimeSpan.FromSeconds(10));
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsTrue(timer3Elapsed);
        }

        /// <summary>
        /// Tests the timers with additional system time changes (backwards and forwards).
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void TestTimersWithSystemTimeChange()
        {
            var timeProvider = new ManualTimeProvider(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var timerFactory = new TestableTimerFactory();
            TimeProvider.Current = timeProvider;
            TimerFactory.Current = timerFactory;

            var target = new MasterPresentationTimeContext();
            target.NextTimeReached += (s, e) => target.NotifyTimeReached(e.Time);

            var timer = timerFactory[target.GetType().Name].First();
            var timer1Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(20), time => timer1Elapsed = true); // expires at t + 20
            Assert.IsFalse(timer1Elapsed);

            var timer2Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(35), time => timer2Elapsed = true); // expires at t + 35
            Assert.IsFalse(timer2Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(10)); // t + 10, 12:00:10
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);

            var timer3Elapsed = false;
            target.AddTimeReachedHandler(
                new DateTime(2000, 1, 1, 12, 30, 00, DateTimeKind.Utc), time => timer3Elapsed = true);
            Assert.IsFalse(timer3Elapsed);

            var timer4Elapsed = false;
            target.AddTimeReachedHandler(
                new DateTime(2000, 1, 1, 14, 30, 30, DateTimeKind.Utc), time => timer4Elapsed = true);
            Assert.IsFalse(timer4Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(10)); // t + 20, 12:00:20
            timer.RaiseElapsed();
            Assert.IsTrue(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            timer1Elapsed = false;

            var timer5Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(25), time => timer5Elapsed = true); // expires at t + 45
            Assert.IsFalse(timer5Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 25, 12:00:25
            timeProvider.SetUtcNow(new DateTime(2000, 1, 1, 14, 30, 0, DateTimeKind.Utc));
            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 30, 14:30:05
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsFalse(timer5Elapsed);

            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsTrue(timer3Elapsed); // timer 3 has passed
            Assert.IsFalse(timer4Elapsed); // timer 4 hasn't passed yet, but needs to be recalculated
            Assert.IsFalse(timer5Elapsed);
            timer3Elapsed = false;

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 35, 14:30:10
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsTrue(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsFalse(timer5Elapsed);
            timer2Elapsed = false;

            var timer6Elapsed = false;
            target.AddTimeReachedHandler(
                new DateTime(2000, 1, 1, 14, 30, 40, DateTimeKind.Utc), time => timer6Elapsed = true);
            Assert.IsFalse(timer6Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 40, 14:30:15
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer5Elapsed);
            Assert.IsFalse(timer6Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 45, 14:30:20
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsTrue(timer5Elapsed);
            Assert.IsFalse(timer6Elapsed);
            timer5Elapsed = false;

            var timer7Elapsed = false;
            target.AddTimeReachedHandler(
                new DateTime(2000, 1, 1, 14, 31, 20, DateTimeKind.Utc), time => timer7Elapsed = true);
            Assert.IsFalse(timer7Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(10)); // t + 55, 14:30:30
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsTrue(timer4Elapsed); // the recalculated timer 4 has to expire now
            Assert.IsFalse(timer5Elapsed);
            Assert.IsFalse(timer6Elapsed);
            Assert.IsFalse(timer7Elapsed);
            timer4Elapsed = false;

            var timer8Elapsed = false;
            target.AddTimeElapsedHandler(TimeSpan.FromSeconds(25), time => timer8Elapsed = true); // expires at t + 80
            Assert.IsFalse(timer8Elapsed);

            timeProvider.AddTime(TimeSpan.FromSeconds(10)); // t + 65, 14:30:40
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsFalse(timer5Elapsed);
            Assert.IsTrue(timer6Elapsed);
            Assert.IsFalse(timer7Elapsed);
            Assert.IsFalse(timer8Elapsed);
            timer6Elapsed = false;

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 70, 14:30:45
            timeProvider.SetUtcNow(new DateTime(2000, 1, 1, 10, 15, 0, DateTimeKind.Utc));
            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 75, 10:15:05
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsFalse(timer5Elapsed);
            Assert.IsFalse(timer6Elapsed);
            Assert.IsTrue(timer7Elapsed); // timer 7 must be triggered because the system time went backwards
            Assert.IsFalse(timer8Elapsed); // timer 8 is not affected by the backwards move (it's a relative timer)
            timer7Elapsed = false;

            timeProvider.AddTime(TimeSpan.FromSeconds(5)); // t + 80, 10:15:10
            timer.RaiseElapsed();
            Assert.IsFalse(timer1Elapsed);
            Assert.IsFalse(timer2Elapsed);
            Assert.IsFalse(timer3Elapsed);
            Assert.IsFalse(timer4Elapsed);
            Assert.IsFalse(timer5Elapsed);
            Assert.IsFalse(timer6Elapsed);
            Assert.IsFalse(timer7Elapsed);
            Assert.IsTrue(timer8Elapsed);
            timer8Elapsed = false;

            // verify that all timers have been removed and won't trigger again
            for (int i = 0; i < 2000; i++)
            {
                timeProvider.AddTime(TimeSpan.FromSeconds(10));
                timer.RaiseElapsed();
                Assert.IsFalse(timer1Elapsed);
                Assert.IsFalse(timer2Elapsed);
                Assert.IsFalse(timer3Elapsed);
                Assert.IsFalse(timer4Elapsed);
                Assert.IsFalse(timer5Elapsed);
                Assert.IsFalse(timer6Elapsed);
                Assert.IsFalse(timer7Elapsed);
                Assert.IsFalse(timer8Elapsed);
            }
        }
    }
}
