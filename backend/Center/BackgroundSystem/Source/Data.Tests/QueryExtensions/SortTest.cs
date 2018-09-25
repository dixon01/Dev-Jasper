// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SortTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests sorting.
    /// </summary>
    [TestClass]
    public class SortTest
    {
        /// <summary>
        /// Tests the ascending sort.
        /// </summary>
        [TestMethod]
        public void TestSortAscending()
        {
            var set = new[]
                          {
                              new UpdateCommand { UpdateIndex = 3 }, new UpdateCommand { UpdateIndex = 7 },
                              new UpdateCommand { UpdateIndex = 2 }, new UpdateCommand { UpdateIndex = 5 },
                              new UpdateCommand { UpdateIndex = 8 }
                          };

            var filtered = set.AsQueryable().Apply(UpdateCommandQuery.Create().OrderByUpdateIndex()).ToList();
            Assert.AreEqual(5, filtered.Count);
            Assert.AreEqual(2, filtered[0].UpdateIndex);
            Assert.AreEqual(3, filtered[1].UpdateIndex);
            Assert.AreEqual(5, filtered[2].UpdateIndex);
            Assert.AreEqual(7, filtered[3].UpdateIndex);
            Assert.AreEqual(8, filtered[4].UpdateIndex);
        }

        /// <summary>
        /// Tests the descending sort.
        /// </summary>
        [TestMethod]
        public void TestSortDescending()
        {
            var set = new[]
                          {
                              new UpdateCommand { UpdateIndex = 3 }, new UpdateCommand { UpdateIndex = 7 },
                              new UpdateCommand { UpdateIndex = 2 }, new UpdateCommand { UpdateIndex = 5 },
                              new UpdateCommand { UpdateIndex = 8 }
                          };

            var filtered = set.AsQueryable().Apply(UpdateCommandQuery.Create().OrderByUpdateIndexDescending()).ToList();
            Assert.AreEqual(5, filtered.Count);
            Assert.AreEqual(8, filtered[0].UpdateIndex);
            Assert.AreEqual(7, filtered[1].UpdateIndex);
            Assert.AreEqual(5, filtered[2].UpdateIndex);
            Assert.AreEqual(3, filtered[3].UpdateIndex);
            Assert.AreEqual(2, filtered[4].UpdateIndex);
        }

        /// <summary>
        /// Tests the ascending sort with skipping.
        /// </summary>
        [TestMethod]
        public void TestSortSkip()
        {
            var set = new[]
                          {
                              new UpdateCommand { UpdateIndex = 3 }, new UpdateCommand { UpdateIndex = 7 },
                              new UpdateCommand { UpdateIndex = 2 }, new UpdateCommand { UpdateIndex = 5 },
                              new UpdateCommand { UpdateIndex = 8 }
                          };

            var filtered = set.AsQueryable().Apply(UpdateCommandQuery.Create().OrderByUpdateIndex().Skip(3)).ToList();
            Assert.AreEqual(2, filtered.Count);
            Assert.AreEqual(7, filtered[0].UpdateIndex);
            Assert.AreEqual(8, filtered[1].UpdateIndex);
        }

        /// <summary>
        /// Tests the ascending sort with a take.
        /// </summary>
        [TestMethod]
        public void TestSortTake()
        {
            var set = new[]
                          {
                              new UpdateCommand { UpdateIndex = 3 }, new UpdateCommand { UpdateIndex = 7 },
                              new UpdateCommand { UpdateIndex = 2 }, new UpdateCommand { UpdateIndex = 5 },
                              new UpdateCommand { UpdateIndex = 8 }
                          };

            var filtered = set.AsQueryable().Apply(UpdateCommandQuery.Create().OrderByUpdateIndex().Take(3)).ToList();
            Assert.AreEqual(3, filtered.Count);
            Assert.AreEqual(2, filtered[0].UpdateIndex);
            Assert.AreEqual(3, filtered[1].UpdateIndex);
            Assert.AreEqual(5, filtered[2].UpdateIndex);
        }

        /// <summary>
        /// Tests the ascending sort with a skip and a take.
        /// </summary>
        [TestMethod]
        public void TestSortSkipTake()
        {
            var set = new[]
                          {
                              new UpdateCommand { UpdateIndex = 3 }, new UpdateCommand { UpdateIndex = 7 },
                              new UpdateCommand { UpdateIndex = 2 }, new UpdateCommand { UpdateIndex = 5 },
                              new UpdateCommand { UpdateIndex = 8 }
                          };

            var filtered =
                set.AsQueryable().Apply(UpdateCommandQuery.Create().OrderByUpdateIndex().Skip(1).Take(2)).ToList();
            Assert.AreEqual(2, filtered.Count);
            Assert.AreEqual(3, filtered[0].UpdateIndex);
            Assert.AreEqual(5, filtered[1].UpdateIndex);
        }
    }
}
