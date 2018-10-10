// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoCloneTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for DTO cloning
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for DTO cloning
    /// </summary>
    [TestClass]
    public class DtoCloneTest
    {
        /// <summary>
        /// Tests the cloning of a simple DTO.
        /// </summary>
        [TestMethod]
        public void CloneSimpleDtoTest()
        {
            var originalUserRole = new UserRole
                                       {
                                           Id = 10,
                                           Authorizations = new Collection<Authorization>(),
                                           Name = "UserRole"
                                       };
            var originalAuthorization = new Authorization
                               {
                                   Id = 1,
                                   DataScope = DataScope.Tenant,
                                   Permission = Permission.Interact,
                                   UserRole = originalUserRole
                               };
            originalUserRole.Authorizations.Add(originalAuthorization);
            var clone = (Authorization)originalAuthorization.Clone();
            Assert.AreEqual(originalAuthorization.Id, clone.Id);
            Assert.IsNotNull(clone.UserRole);
            Assert.AreSame(originalUserRole, clone.UserRole);
            Assert.AreSame(originalAuthorization, clone.UserRole.Authorizations.First());
            Assert.AreEqual(originalAuthorization.Permission, clone.Permission);
            Assert.AreEqual(originalAuthorization.DataScope, clone.DataScope);
        }

        /// <summary>
        /// Test the cloning of a complex DTO (Unit) with reference properties and collections.
        /// </summary>
        [TestMethod]
        public void CloneComplexDtoTest()
        {
            var originalUpdateGroup = new UpdateGroup
                                           {
                                               Id = 100,
                                               Name = "UpdateGroup1",
                                               UpdateParts = new Collection<UpdatePart>(),
                                               Units = new Collection<Unit>()
                                           };
            var originalUser = new User { Id = 1, FirstName = "User", };
            var originalTenant = new Tenant
                                     {
                                         Id = 1,
                                         Name = "Tenant",
                                         UpdateGroups = new[] { originalUpdateGroup },
                                         Users = new[] { originalUser }
                                     };
            var originalUserDefinedProperties = new Dictionary<string, string> { { "UDPKey", "UDPValue" } };
            var originalUpdateCommand1 = new UpdateCommand { Id = 50, CommandXml = "Command1" };
            var originalUpdateCommand2 = new UpdateCommand { Id = 60, CommandXml = "Command2" };
            var originalUpdateCommands = new[] { originalUpdateCommand1, originalUpdateCommand2 };
            var originalUnit = new Unit
                                   {
                                       Id = 10,
                                       Tenant = originalTenant,
                                       UpdateGroup = originalUpdateGroup,
                                       UserDefinedProperties = originalUserDefinedProperties,
                                       UpdateCommands = originalUpdateCommands
                                   };
            originalUpdateCommand1.Unit = originalUnit;
            originalUpdateCommand2.Unit = originalUnit;
            originalUpdateGroup.Units.Add(originalUnit);

            var clonedUnit = (Unit)originalUnit.Clone();
            Assert.IsNotNull(clonedUnit);
            Assert.AreEqual(originalUnit.Id, clonedUnit.Id);
            Assert.AreSame(originalUnit.Tenant, clonedUnit.Tenant);
            Assert.AreSame(originalUpdateGroup, clonedUnit.UpdateGroup);
            Assert.AreNotSame(originalUnit.UserDefinedProperties, clonedUnit.UserDefinedProperties);
            Assert.IsTrue(clonedUnit.UserDefinedProperties.ContainsKey("UDPKey"));
            Assert.AreEqual(originalUserDefinedProperties["UDPKey"], clonedUnit.UserDefinedProperties["UDPKey"]);
            Assert.IsNotNull(clonedUnit.UpdateCommands);
            var clonedUpdateCommands = clonedUnit.UpdateCommands;
            Assert.AreEqual(2, clonedUpdateCommands.Count);
            Assert.AreNotSame(originalUpdateCommands, clonedUpdateCommands);
            foreach (var clonedUpdateCommand in clonedUpdateCommands)
            {
                var originalCommand = originalUpdateCommands.FirstOrDefault(c => c.Id == clonedUpdateCommand.Id);
                Assert.AreSame(originalCommand, clonedUpdateCommand);
            }
        }
    }
}
