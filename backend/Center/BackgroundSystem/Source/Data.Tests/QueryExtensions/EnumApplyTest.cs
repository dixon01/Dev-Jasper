// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumApplyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Log;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Log;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the integer filtering.
    /// </summary>
    [TestClass]
    public class EnumApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable().Apply(LogEntryFilter.Create().WithLevel(Common.ServiceModel.Log.Level.Info)).ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the Different filtering.
        /// </summary>
        [TestMethod]
        public void TestDifferent()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        LogEntryFilter.Create().WithLevel(Common.ServiceModel.Log.Level.Info, EnumComparison.Different))
                    .ToList();
            Assert.AreEqual(5, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThan filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestGreaterThan()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        LogEntryFilter.Create()
                            .WithLevel(Common.ServiceModel.Log.Level.Debug, EnumComparison.GreaterThan))
                    .ToList();
            Assert.AreEqual(4, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestGreaterThanOrEqualTo()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        LogEntryFilter.Create()
                            .WithLevel(Common.ServiceModel.Log.Level.Debug, EnumComparison.GreaterThan))
                    .ToList();
            Assert.AreEqual(5, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThan filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestLessThan()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        LogEntryFilter.Create()
                            .WithLevel(Common.ServiceModel.Log.Level.Info, EnumComparison.GreaterThan))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestLessThanOrEqualTo()
        {
            var set = new[]
                          {
                              new LogEntry { Level = Level.Trace }, new LogEntry { Level = Level.Debug },
                              new LogEntry { Level = Level.Info }, new LogEntry { Level = Level.Warn },
                              new LogEntry { Level = Level.Error }, new LogEntry { Level = Level.Fatal }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        LogEntryFilter.Create()
                            .WithLevel(Common.ServiceModel.Log.Level.Info, EnumComparison.LessThanOrEqualTo))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }
    }
}