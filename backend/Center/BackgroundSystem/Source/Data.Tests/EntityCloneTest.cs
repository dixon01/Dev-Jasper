// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCloneTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityCloneTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for cloning.
    /// </summary>
    [TestClass]
    public class EntityCloneTest
    {
        /// <summary>
        /// Tests the cloning of a simple entity.
        /// </summary>
        [TestMethod]
        public void CloneSimpleEntityTest()
        {
            var now = DateTime.UtcNow;
            var sourceTenant = new Model.Membership.Tenant { Id = 1, Name = "Tenant" };
            var sourceUser = new Model.Membership.User
                                 {
                                     Id = 11,
                                     OwnerTenant = sourceTenant,
                                     CreatedOn = now,
                                     LastModifiedOn = now.AddDays(1)
                                 };
            var clonedUser = sourceUser.Clone();
            Assert.AreEqual(sourceUser.Id, clonedUser.Id);
            Assert.AreSame(sourceTenant, clonedUser.OwnerTenant);
            Assert.AreEqual(now, clonedUser.CreatedOn);
            Assert.AreEqual(now.AddDays(1), clonedUser.LastModifiedOn);
        }
    }
}
