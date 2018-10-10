// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int64ApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Int64ApplyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Resources;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the long filtering
    /// </summary>
    [TestClass]
    public class Int64ApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var set = new[]
                          {
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered = set.AsQueryable().Apply(ResourceFilter.Create().WithLength(2)).ToList();
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
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(ResourceFilter.Create().WithLength(2, Int64Comparison.Different)).ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThan filtering.
        /// </summary>
        [TestMethod]
        public void TestGreaterThan()
        {
            var set = new[]
                          {
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(ResourceFilter.Create().WithLength(2, Int64Comparison.GreaterThan)).ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        public void TestGreaterThanOrEqualTo()
        {
            var set = new[]
                          {
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(ResourceFilter.Create().WithLength(2, Int64Comparison.GreaterThanOrEqualTo))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThan filtering.
        /// </summary>
        [TestMethod]
        public void TestLessThan()
        {
            var set = new[]
                          {
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(ResourceFilter.Create().WithLength(2, Int64Comparison.LessThan)).ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        public void TestLessThanOrEqualTo()
        {
            var set = new[]
                          {
                              new Resource { Length = 1 }, new Resource { Length = 2 }, new Resource { Length = 3 },
                              new Resource { Length = 4 }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(ResourceFilter.Create().WithLength(2, Int64Comparison.LessThanOrEqualTo))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }
    }
}