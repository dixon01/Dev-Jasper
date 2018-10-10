// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDefinedPropertyMappingTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDefinedPropertyMappingTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using DatabaseUnit = Gorba.Center.BackgroundSystem.Data.Model.Units.Unit;
    using DtoUnit = Gorba.Center.Common.ServiceModel.Units.Unit;

    /// <summary>
    /// Defines tests to verify the mapping of user defined properties.
    /// </summary>
    [TestClass]
    public class UserDefinedPropertyMappingTest
    {
        /// <summary>
        /// Tests the mapping from database to DTO.
        /// </summary>
        [TestMethod]
        public void TestUnitUserDefinedPropertiesToDto()
        {
            var userDefinedProperty = new UserDefinedProperty
                                          {
                                              Name
                                                  =
                                                  "test"
                                          };
            var databaseUnit = new DatabaseUnit
                                   {
                                       Id = 1,
                                       UserDefinedProperties =
                                           new Collection<UnitUserDefinedProperty>
                                               {
                                                   new UnitUserDefinedProperty(userDefinedProperty, "value")
                                               }
                                   };
            var dtoUnit = databaseUnit.ToDto();
            var testExists = dtoUnit.UserDefinedProperties.ContainsKey("test");
            Assert.IsTrue(testExists);
            Assert.AreEqual("value", dtoUnit.UserDefinedProperties["test"]);
        }

        /// <summary>
        /// Tests the mapping from DTO to database.
        /// </summary>
        [TestMethod]
        public void TestUnitUserDefinedPropertiesToDatabase()
        {
            var dtoUnit = new DtoUnit
                                   {
                                       Id = 1,
                                       UserDefinedProperties =
                                           new Dictionary<string, string> { { "test", "value" } }
                                   };
            var databaseUnit = dtoUnit.ToDatabase();
            var testExists = databaseUnit.RawUserDefinedProperties.ContainsKey("test");
            Assert.IsTrue(testExists);
            Assert.AreEqual("value", dtoUnit.UserDefinedProperties["test"]);
        }
    }
}