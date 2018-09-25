// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringApplyTest type.
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
    /// Tests the string filtering.
    /// </summary>
    [TestClass]
    public class StringApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var set = new[]
                          {
                              new Document { Name = "Test" }, new Document { Name = "test" },
                              new Document { Name = "Something else" }
                          };

            var filtered = set.AsQueryable().Apply(DocumentFilter.Create().WithName("Test")).ToList();
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
                              new Document { Name = "Test" }, new Document { Name = "test" },
                              new Document { Name = "Something else" }
                          };

            var filtered =
                set.AsQueryable().Apply(DocumentFilter.Create().WithName("Test", StringComparison.Different)).ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the CaseInsensitiveMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestCaseInsensitiveMatch()
        {
            var set = new[]
                          {
                              new Document { Name = "Test" }, new Document { Name = "test" },
                              new Document { Name = "Something else" }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithName("tEst", StringComparison.CaseInsensitiveMatch))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNullValueOnExactMatch()
        {
            var set = new[]
                          {
                              new Document { Name = "Test" }, new Document { Name = null },
                              new Document { Name = "Something else" }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithName(null))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNullValueOnDifferent()
        {
            var set = new[]
                          {
                              new Document { Name = "Test" }, new Document { Name = null },
                              new Document { Name = "Something else" }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithName(null, StringComparison.Different))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNullValueOnCaseInsensitive()
        {
            var set = new[]
                          {
                              new Document { Name = "Test" }, new Document { Name = null },
                              new Document { Name = "Something else" }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithName(null, StringComparison.CaseInsensitiveMatch))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the nested filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedApply()
        {
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { Name = "Test" } },
                              new DocumentVersion { Document = new Document { Name = "test" } },
                              new DocumentVersion { Document = new Document { Name = "Something else" } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create().WithName("tEst", StringComparison.CaseInsensitiveMatch)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedNullValueOnExactMatch()
        {
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { Name = "Test" } },
                              new DocumentVersion { Document = new Document { Name = null } },
                              new DocumentVersion { Document = new Document { Name = "Something else" } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create().WithName(null)))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedNullValueOnDifferent()
        {
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { Name = "Test" } },
                              new DocumentVersion { Document = new Document { Name = null } },
                              new DocumentVersion { Document = new Document { Name = "Something else" } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create().WithName(null, StringComparison.Different)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the null value filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedNullValueOnCaseInsensitiveMatch()
        {
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { Name = "Test" } },
                              new DocumentVersion { Document = new Document { Name = null } },
                              new DocumentVersion { Document = new Document { Name = "Something else" } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create().WithName(null, StringComparison.CaseInsensitiveMatch)))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }
    }
}