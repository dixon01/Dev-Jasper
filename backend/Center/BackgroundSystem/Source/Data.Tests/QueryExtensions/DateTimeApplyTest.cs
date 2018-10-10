// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeApplyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests the DateTime filtering.
    /// </summary>
    [TestClass]
    public class DateTimeApplyTest
    {
        private static readonly DateTime UtcNow = new DateTime(2015, 03, 23, 7, 15, 18, DateTimeKind.Utc);

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var timeProviderMock = new Mock<TimeProvider>(MockBehavior.Strict);
            timeProviderMock.SetupGet(provider => provider.UtcNow).Returns(UtcNow);
            TimeProvider.Current = timeProviderMock.Object;
        }

        /// <summary>
        /// Cleans the test up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var timeProviderMock = new Mock<TimeProvider>(MockBehavior.Strict);
            TimeProvider.Current = timeProviderMock.Object;
        }

        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) }
                          };

            var filtered = set.AsQueryable().Apply(DocumentFilter.Create().WithCreatedOn(utcNow)).ToList();
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(utcNow, filtered.Select(document => document.CreatedOn).Single());
        }

        /// <summary>
        /// Tests the Different filtering.
        /// </summary>
        [TestMethod]
        public void TestDifferent()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithCreatedOn(utcNow, DateTimeComparison.Different))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThan filtering.
        /// </summary>
        [TestMethod]
        public void TestGreaterThan()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) },
                              new Document { CreatedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithCreatedOn(utcNow.AddMinutes(1), DateTimeComparison.GreaterThan))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the GreaterThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        public void TestGreaterThanOrEqualTo()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) },
                              new Document { CreatedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithCreatedOn(utcNow.AddMinutes(1), DateTimeComparison.GreaterThanOrEqualTo))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThan filtering.
        /// </summary>
        [TestMethod]
        public void TestLessThan()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) },
                              new Document { CreatedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithCreatedOn(utcNow.AddMinutes(1), DateTimeComparison.LessThan))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the LessThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        public void TestLessThanOrEqualTo()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { CreatedOn = utcNow },
                              new Document { CreatedOn = utcNow.AddMinutes(1) },
                              new Document { CreatedOn = utcNow.AddMinutes(2) },
                              new Document { CreatedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithCreatedOn(utcNow.AddMinutes(1), DateTimeComparison.LessThanOrEqualTo))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the nested filtering.
        /// </summary>
        [TestMethod]
        public void TestNestedApply()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { CreatedOn = utcNow } },
                              new DocumentVersion { Document = new Document { CreatedOn = utcNow.AddMinutes(1) } },
                              new DocumentVersion { Document = new Document { CreatedOn = utcNow.AddMinutes(2) } },
                              new DocumentVersion { Document = new Document { CreatedOn = utcNow.AddMinutes(3) } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create()
                                    .WithCreatedOn(utcNow.AddMinutes(1), DateTimeComparison.LessThanOrEqualTo)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }
    }
}