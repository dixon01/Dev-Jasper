// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int32ApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Int32ApplyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the integer filtering.
    /// </summary>
    [TestClass]
    public class Int32ApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var set = new[]
                          {
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered = set.AsQueryable().Apply(DocumentFilter.Create().WithId(2)).ToList();
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
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(DocumentFilter.Create().WithId(2, Int32Comparison.Different)).ToList();
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
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(DocumentFilter.Create().WithId(2, Int32Comparison.GreaterThan)).ToList();
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
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithId(2, Int32Comparison.GreaterThanOrEqualTo))
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
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered =
                    set.AsQueryable().Apply(DocumentFilter.Create().WithId(2, Int32Comparison.LessThan)).ToList();
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
                              new Document { Id = 1 }, new Document { Id = 2 }, new Document { Id = 3 },
                              new Document { Id = 4 }
                          };

            var filtered =
                set.AsQueryable().Apply(DocumentFilter.Create().WithId(2, Int32Comparison.LessThanOrEqualTo)).ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the nested filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedApply()
        {
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { Id = 1 } },
                              new DocumentVersion { Document = new Document { Id = 2 } },
                              new DocumentVersion { Document = new Document { Id = 3 } },
                              new DocumentVersion { Document = new Document { Id = 4 } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(DocumentFilter.Create().WithId(2, Int32Comparison.GreaterThan)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }
    }
}