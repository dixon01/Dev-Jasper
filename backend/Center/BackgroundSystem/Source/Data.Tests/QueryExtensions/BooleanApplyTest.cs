// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanApplyTest type.
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
    /// Tests boolean filtering.
    /// </summary>
    [TestClass]
    public class BooleanApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var set = new[]
                          {
                              new UpdateCommand { WasInstalled = false }, new UpdateCommand { WasInstalled = true },
                              new UpdateCommand { WasInstalled = true }, new UpdateCommand { WasInstalled = false },
                              new UpdateCommand { WasInstalled = false }
                          };

            var filtered = set.AsQueryable().Apply(UpdateCommandFilter.Create().WithWasInstalled(true)).ToList();
            Assert.AreEqual(2, filtered.Count);
        }

        /// <summary>
        /// Tests the Different filtering.
        /// </summary>
        [TestMethod]
        public void TestDifferent()
        {
            var set = new[]
                          {
                              new UpdateCommand { WasInstalled = false }, new UpdateCommand { WasInstalled = true },
                              new UpdateCommand { WasInstalled = true }, new UpdateCommand { WasInstalled = false },
                              new UpdateCommand { WasInstalled = false }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(UpdateCommandFilter.Create().WithWasInstalled(true, BooleanComparison.Different))
                    .ToList();
            Assert.AreEqual(3, filtered.Count);
        }

        /// <summary>
        /// Tests nested application.
        /// </summary>
        [TestMethod]
        public void TestNestedApply()
        {
            var set = new[]
                          {
                              new UpdateFeedback { UpdateCommand = new UpdateCommand { WasInstalled = false } },
                              new UpdateFeedback { UpdateCommand = new UpdateCommand { WasInstalled = true } },
                              new UpdateFeedback { UpdateCommand = new UpdateCommand { WasInstalled = true } },
                              new UpdateFeedback { UpdateCommand = new UpdateCommand { WasInstalled = false } }
                          };
            var filtered =
                set.AsQueryable()
                    .Apply(
                        UpdateFeedbackFilter.Create()
                            .IncludeUpdateCommand(UpdateCommandFilter.Create().WithWasInstalled(true)))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }
    }
}