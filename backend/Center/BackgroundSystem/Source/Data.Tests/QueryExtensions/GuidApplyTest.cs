// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidApplyTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GuidApplyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests.QueryExtensions
{
    using System;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Guid filtering.
    /// </summary>
    [TestClass]
    public class GuidApplyTest
    {
        /// <summary>
        /// Tests the ExactMatch filtering.
        /// </summary>
        [TestMethod]
        public void TestExactMatch()
        {
            var guidA = new Guid("63dd224e-bc91-4616-99c4-6ab52276f5f2");
            var guidB = new Guid("8103c234-9548-4545-91b1-6de2edcd795f");
            var guidC = new Guid("86eb06fe-88f7-49da-960f-371a2061f064");
            var set = new[]
                          {
                              new SystemConfig { SystemId = guidA },
                              new SystemConfig { SystemId = guidB },
                              new SystemConfig { SystemId = guidC }
                          };

            var filtered = set.AsQueryable().Apply(SystemConfigFilter.Create().WithSystemId(guidA)).ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        /// <summary>
        /// Tests the Different filtering.
        /// </summary>
        [TestMethod]
        public void TestDifferent()
        {
            var guidA = new Guid("63dd224e-bc91-4616-99c4-6ab52276f5f2");
            var guidB = new Guid("8103c234-9548-4545-91b1-6de2edcd795f");
            var guidC = new Guid("86eb06fe-88f7-49da-960f-371a2061f064");
            var set = new[]
                          {
                              new SystemConfig { SystemId = guidA },
                              new SystemConfig { SystemId = guidB },
                              new SystemConfig { SystemId = guidC }
                          };

            var filtered =
                set.AsQueryable()
                    .Apply(SystemConfigFilter.Create().WithSystemId(guidA, GuidComparison.Different))
                    .ToList();
            Assert.AreEqual(2, filtered.Count);
        }
    }
}