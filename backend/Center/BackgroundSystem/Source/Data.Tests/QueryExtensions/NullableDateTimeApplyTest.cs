﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableDateTimeApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NullableDateTimeApplyTest type.
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
    public class NullableDateTimeApplyTest
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) }
                          };

            var filtered = set.AsQueryable().Apply(DocumentFilter.Create().WithLastModifiedOn(utcNow)).ToList();
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(utcNow, filtered.Select(document => document.LastModifiedOn).Single());
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(DocumentFilter.Create().WithLastModifiedOn(utcNow, NullableDateTimeComparison.Different))
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filter = DocumentFilter.Create()
                .WithLastModifiedOn(utcNow.AddMinutes(1), NullableDateTimeComparison.GreaterThan);
            var filtered =
                set.AsQueryable()
                    .Apply(filter)
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithLastModifiedOn(utcNow.AddMinutes(1), NullableDateTimeComparison.GreaterThanOrEqualTo))
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filter = DocumentFilter.Create()
                .WithLastModifiedOn(utcNow.AddMinutes(1), NullableDateTimeComparison.LessThan);
            var filtered =
                set.AsQueryable()
                    .Apply(filter)
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
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = utcNow.AddMinutes(1) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithLastModifiedOn(utcNow.AddMinutes(1), NullableDateTimeComparison.LessThanOrEqualTo))
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
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(1) } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(2) } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(3) } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create()
                                    .WithLastModifiedOn(
                                        utcNow.AddMinutes(1),
                                        NullableDateTimeComparison.LessThanOrEqualTo)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the null value on ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestNullValueOnExactMatch()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithLastModifiedOn(null))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the null value on Different filtering.
        /// </summary>
        [TestMethod]
        public void TestNullValueOnDifferent()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentFilter.Create()
                            .WithLastModifiedOn(null, NullableDateTimeComparison.Different))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        /// <summary>
        /// Tests the null value on GreaterThan filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullValueOnGreaterThan()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            set.AsQueryable()
                .Apply(
                    DocumentFilter.Create()
                        .WithLastModifiedOn(null, NullableDateTimeComparison.GreaterThan))
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                .ToList();
        }

        /// <summary>
        /// Tests the null value on GreaterThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullValueOnGreaterThanOrEqualTo()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            set.AsQueryable()
                .Apply(
                    DocumentFilter.Create()
                        .WithLastModifiedOn(null, NullableDateTimeComparison.GreaterThanOrEqualTo))
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                .ToList();
        }

        /// <summary>
        /// Tests the null value on LessThan filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullValueOnLessThan()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            set.AsQueryable()
                .Apply(
                    DocumentFilter.Create()
                        .WithLastModifiedOn(null, NullableDateTimeComparison.LessThan))
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                .ToList();
        }

        /// <summary>
        /// Tests the null value on LessThanOrEqualTo filtering.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullValueOnLessThanOrEqualTo()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new Document { LastModifiedOn = utcNow },
                              new Document { LastModifiedOn = null },
                              new Document { LastModifiedOn = utcNow.AddMinutes(2) },
                              new Document { LastModifiedOn = utcNow.AddMinutes(3) }
                          };

            set.AsQueryable()
                .Apply(
                    DocumentFilter.Create()
                        .WithLastModifiedOn(null, NullableDateTimeComparison.LessThanOrEqualTo))
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                .ToList();
        }

        /// <summary>
        /// Tests the null value on ExactMatch filtering on nested property.
        /// </summary>
        [TestMethod]
        public void TestNestedNullValueOnExactMatch()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = null } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(2) } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(3) } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create()
                                    .WithLastModifiedOn(null)))
                    .ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the null value on Different filtering on nested property.
        /// </summary>
        [TestMethod]
        public void TestNestedNullValueOnDifferent()
        {
            var utcNow = TimeProvider.Current.UtcNow;
            var set = new[]
                          {
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = null } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(2) } },
                              new DocumentVersion { Document = new Document { LastModifiedOn = utcNow.AddMinutes(3) } }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(
                        DocumentVersionFilter.Create()
                            .IncludeDocument(
                                DocumentFilter.Create()
                                    .WithLastModifiedOn(null, NullableDateTimeComparison.Different)))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }
    }
}