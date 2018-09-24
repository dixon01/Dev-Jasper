// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MappingTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Serialization;

    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using DtoProductType = Gorba.Center.Common.ServiceModel.Units.ProductType;
    using DtoTenant = Gorba.Center.Common.ServiceModel.Membership.Tenant;
    using DtoUnit = Gorba.Center.Common.ServiceModel.Units.Unit;
    using DtoUpdateCommand = Gorba.Center.Common.ServiceModel.Update.UpdateCommand;
    using DtoUpdateGroup = Gorba.Center.Common.ServiceModel.Update.UpdateGroup;
    using DtoUpdatePart = Gorba.Center.Common.ServiceModel.Update.UpdatePart;
    using DtoUser = Gorba.Center.Common.ServiceModel.Membership.User;
    using DtoXmlData = Gorba.Center.Common.ServiceModel.XmlData;

    /// <summary>
    /// Tests for mapping.
    /// </summary>
    [TestClass]
    public class MappingTest
    {
        /// <summary>
        /// Tests the mapping from DTO to database entities.
        /// </summary>
        [TestMethod]
        public void MapDtoToDatabaseTest()
        {
            var sourceTenant = new DtoTenant { Id = 1, Name = "Tenant" };
            var sourceUser = new DtoUser
                              {
                                  Id = 11,
                                  OwnerTenant = sourceTenant
                              };
            sourceTenant.Users = new List<DtoUser> { sourceUser };
            var destinationTenant = sourceTenant.ToDatabase();
            Assert.AreEqual(1, destinationTenant.Id);
            Assert.AreEqual("Tenant", destinationTenant.Name);
            Assert.AreEqual(1, destinationTenant.Users.Count);
            var destinationUser = destinationTenant.Users.Single();
            Assert.AreEqual(11, destinationUser.Id);
            Assert.AreSame(destinationTenant, destinationUser.OwnerTenant);
        }

        /// <summary>
        /// Tests the mapping from database to DTO entities.
        /// </summary>
        [TestMethod]
        public void MapDatabaseToDtoTest()
        {
            var destinationTenant = new Tenant { Id = 1, Name = "Tenant" };
            var destinationUser = new User { Id = 11, OwnerTenant = destinationTenant };
            destinationTenant.Users = new List<User> { destinationUser };
            var sourceTenant = destinationTenant.ToDto();
            Assert.AreEqual(1, sourceTenant.Id);
            Assert.AreEqual("Tenant", sourceTenant.Name);
            Assert.AreEqual(1, sourceTenant.Users.Count);
            var sourceUser = sourceTenant.Users.Single();
            Assert.AreEqual(11, sourceUser.Id);
            Assert.AreSame(sourceTenant, sourceUser.OwnerTenant);
        }

        /// <summary>
        /// Tests the mapping from database to DTO without reference property.
        /// </summary>
        [TestMethod]
        public void MapDatabaseToDtoWithoutReferencePropertyTest()
        {
            var sourceUser = new User { Id = 1 };
            var destinationUser = sourceUser.ToDto();
            Assert.IsNull(destinationUser.OwnerTenant);
            Assert.IsNull(destinationUser.AssociationTenantUserUserRoles);
            Assert.AreEqual(1, destinationUser.Id);
        }

        /// <summary>
        /// Tests the mapping from DTO to database without reference property.
        /// </summary>
        [TestMethod]
        public void MapDtoToDatabaseWithoutReferencePropertyTest()
        {
            var dtoUser = new DtoUser
                              {
                                  Id = 1,
                                  FirstName = "User"
                              };
            var destinationUser = dtoUser.ToDatabase();
            Assert.AreEqual(1, destinationUser.Id);
            Assert.AreEqual("User", destinationUser.FirstName);
            Assert.IsNull(destinationUser.AssociationTenantUserUserRoles);
            Assert.IsNull(destinationUser.OwnerTenant);
        }

        /// <summary>
        /// Tests the mapping from database to DTO for a complex entity.
        /// </summary>
        [TestMethod]
        public void MapDatabaseToDtoComplexEntityTest()
        {
            var sourceTenant = new Tenant { Id = 1, Name = "Tenant", UpdateGroups = new Collection<UpdateGroup>() };
            var sourceProductType = new ProductType
                                        {
                                            Id = 10,
                                            HardwareDescriptor = new XmlData { Xml = "HardwareDescriptor" }
                                        };
            var userDefinedProperty = new UserDefinedProperty { Id = 20, Tenant = sourceTenant, Name = "UDP" };
            var sourceUnitUserDefinedProperty = new UnitUserDefinedProperty
                                                    {
                                                        PropertyDefinition = userDefinedProperty,
                                                        Id = 30,
                                                        Value = "UnitUDP"
                                                    };

            var sourceUserDefinedProperties = new Collection<UnitUserDefinedProperty> { sourceUnitUserDefinedProperty };
            var structureXmlData = new DtoXmlData(new TestXmlDataObject());
            var updateParts = new Collection<UpdatePart>
                                  {
                                      new UpdatePart
                                          {
                                              Structure = structureXmlData.ToDatabase(),
                                              Id = 50,
                                              Type = UpdatePartType.Presentation
                                          }
                                  };
            var sourceUpdateCommand = new UpdateCommand
                                    {
                                        Command = new XmlData { Xml = "UpdateCommand" },
                                        Id = 40,
                                        IncludedParts = updateParts,
                                        UpdateIndex = 100
                                    };
            updateParts.First().RelatedCommands = new Collection<UpdateCommand> { sourceUpdateCommand };
            var sourceUpdateGroup = new UpdateGroup
                                  {
                                      Id = 60,
                                      Tenant = sourceTenant,
                                      Units = new Collection<Unit>(),
                                      UpdateParts = updateParts
                                  };
            sourceTenant.UpdateGroups.Add(sourceUpdateGroup);
            var sourceUnit = new Unit
                                 {
                                     Id = 11,
                                     Tenant = sourceTenant,
                                     ProductType = sourceProductType,
                                     UserDefinedProperties = sourceUserDefinedProperties,
                                     UpdateCommands = new Collection<UpdateCommand> { sourceUpdateCommand },
                                     UpdateGroup = sourceUpdateGroup,
                                 };
            sourceUpdateCommand.Unit = sourceUnit;
            sourceUpdateGroup.Units.Add(sourceUnit);

            var destinationUnit = sourceUnit.ToDto();
            Assert.AreEqual(sourceTenant.Name, destinationUnit.Tenant.Name);
            Assert.IsNotNull(destinationUnit.UpdateCommands);
            Assert.AreEqual(1, destinationUnit.UpdateCommands.Count);
            var destinationUpdateCommand = destinationUnit.UpdateCommands.First();
            Assert.AreEqual(sourceUpdateCommand.Command.Xml, destinationUpdateCommand.CommandXml);
            Assert.AreEqual(1, destinationUpdateCommand.IncludedParts.Count);
            Assert.AreEqual(100, destinationUpdateCommand.UpdateIndex);
            Assert.AreSame(destinationUnit, destinationUpdateCommand.Unit);
            var destinationUpdatePart = destinationUpdateCommand.IncludedParts.First();
            Assert.AreEqual(Common.ServiceModel.Update.UpdatePartType.Presentation, destinationUpdatePart.Type);
            Assert.AreSame(destinationUpdateCommand, destinationUpdatePart.RelatedCommands.First());
            Assert.IsNotNull(destinationUpdatePart.Structure);
            Assert.AreEqual(structureXmlData.Xml, destinationUpdatePart.StructureXml);
            Assert.AreEqual(structureXmlData.Type, destinationUpdatePart.StructureXmlType);
            var destinationProductType = destinationUnit.ProductType;
            Assert.AreEqual(sourceProductType.HardwareDescriptor.Xml, destinationProductType.HardwareDescriptorXml);
            Assert.IsNotNull(destinationProductType.HardwareDescriptor);
            Assert.IsNotNull(destinationUnit.UserDefinedProperties);
            Assert.IsTrue(destinationUnit.UserDefinedProperties.ContainsKey("UDP"));
            Assert.AreEqual(sourceUnitUserDefinedProperty.Value, destinationUnit.UserDefinedProperties["UDP"]);
            var destinationUpdateGroup = destinationUnit.UpdateGroup;
            Assert.AreSame(destinationUpdateGroup.Units.First(), destinationUnit);
            Assert.AreSame(destinationUnit.Tenant, destinationUpdateGroup.Tenant);
            Assert.AreSame(destinationUpdateGroup, destinationUnit.Tenant.UpdateGroups.First());
        }

        /// <summary>
        /// Tests the mapping from DTO to database with a complex DTO.
        /// </summary>
        [TestMethod]
        public void MapDtoToDatabaseComplexTest()
        {
            var sourceTenant = new DtoTenant
                                   {
                                       Id = 1,
                                       Name = "Tenant",
                                       UpdateGroups = new Collection<DtoUpdateGroup>()
                                   };
            var sourceProductType = new DtoProductType
                                        {
                                            Id = 20,
                                            Name = "ProductType",
                                            HardwareDescriptorXml = "HardwareDescriptor",
                                            Units = new Collection<DtoUnit>()
                                        };
            var sourceUpdateGroup = new DtoUpdateGroup
                                        {
                                            Id = 30,
                                            Tenant = sourceTenant,
                                            UpdateParts = new Collection<DtoUpdatePart>(),
                                            Units = new Collection<DtoUnit>()
                                        };
            var sourceUpdatePart = new DtoUpdatePart
                                       {
                                           Id = 40,
                                           UpdateGroup = sourceUpdateGroup,
                                           Structure = new DtoXmlData(new TestXmlDataObject { TestData = "XmlData" }),
                                           Type = Common.ServiceModel.Update.UpdatePartType.Presentation,
                                           RelatedCommands = new Collection<DtoUpdateCommand>()
                                       };
            var sourceUpdateCommand = new DtoUpdateCommand
                                          {
                                              Id = 50,
                                              UpdateIndex = 100,
                                              IncludedParts = new Collection<DtoUpdatePart> { sourceUpdatePart }
                                          };
            var sourceUnit = new DtoUnit
                                 {
                                     Id = 10,
                                     Tenant = sourceTenant,
                                     ProductType = sourceProductType,
                                     UpdateGroup = sourceUpdateGroup,
                                     UpdateCommands = new Collection<DtoUpdateCommand> { sourceUpdateCommand },
                                     UserDefinedProperties = new Dictionary<string, string>
                                                                {
                                                                    { "UDPKey", "UDPValue" }
                                                                }
                                 };
            sourceTenant.UpdateGroups.Add(sourceUpdateGroup);
            sourceUpdateCommand.Unit = sourceUnit;
            sourceUpdatePart.RelatedCommands.Add(sourceUpdateCommand);
            sourceUpdateGroup.Units.Add(sourceUnit);
            sourceProductType.Units.Add(sourceUnit);

            var destinationUnit = sourceUnit.ToDatabase();
            Assert.IsNotNull(destinationUnit);
            Assert.AreEqual(sourceTenant.Name, destinationUnit.Tenant.Name);
            Assert.IsNotNull(destinationUnit.UpdateCommands);
            Assert.AreEqual(1, destinationUnit.UpdateCommands.Count);
            var destinationUpdateCommand = destinationUnit.UpdateCommands.First();
            Assert.AreEqual(1, destinationUpdateCommand.IncludedParts.Count);
            Assert.AreEqual(100, destinationUpdateCommand.UpdateIndex);
            Assert.AreSame(destinationUnit, destinationUpdateCommand.Unit);
            var destinationUpdatePart = destinationUpdateCommand.IncludedParts.First();
            Assert.AreEqual(UpdatePartType.Presentation, destinationUpdatePart.Type);
            Assert.AreSame(destinationUpdateCommand, destinationUpdatePart.RelatedCommands.First());
            Assert.AreEqual(sourceUpdatePart.Structure.Xml, destinationUpdatePart.Structure.Xml);
            Assert.AreEqual(sourceUpdatePart.Structure.Type, destinationUpdatePart.Structure.Type);
            var destinationProductType = destinationUnit.ProductType;
            Assert.AreEqual(sourceProductType.HardwareDescriptor.Xml, destinationProductType.HardwareDescriptor.Xml);
            Assert.IsNotNull(destinationUnit.UserDefinedProperties);
            Assert.IsTrue(destinationUnit.RawUserDefinedProperties.ContainsKey("UDPKey"));
            Assert.AreEqual("UDPValue", destinationUnit.RawUserDefinedProperties["UDPKey"]);
            var destinationUpdateGroup = destinationUnit.UpdateGroup;
            Assert.AreSame(destinationUpdateGroup.Units.First(), destinationUnit);
            Assert.AreSame(destinationUnit.Tenant, destinationUpdateGroup.Tenant);
            Assert.AreSame(destinationUpdateGroup, destinationUnit.Tenant.UpdateGroups.First());
        }

        /*
        /// <summary>
        /// Tests the mapping from DTO to database entities with a complex hierarchy.
        /// </summary>
        [TestMethod]
        public void MapDtoToDatabaseComplexHierarchyTest()
        {
            var sourceOperation = new Operation
                                   {
                                       Id = 1,
                                       Name = "Operation"
                                   };
            var sourceActivity = new Activity
                              {
                                  Id = 11,
                                  Operation = sourceOperation
                              };
            sourceOperation.Activities.Add(sourceActivity);
            var activityInstanceId1 = new Guid("73d8df7a-4982-4e8f-af5e-830d5ef2d247");
            var activityInstanceId2 = new Guid("bbcaf0f9-eb38-4969-90a7-acfaa5d79758");
            var sourceActivityInstance1 = new ActivityInstance
                              {
                                  Id = activityInstanceId1,
                                  Activity = sourceActivity
                              };
            sourceActivity.Instances.Add(sourceActivityInstance1);
            var sourceActivityInstance2 = new ActivityInstance
                              {
                                  Id = activityInstanceId2,
                                  Activity = sourceActivity,
                                  State = ActivityInstanceState.Created
                              };
            sourceActivity.Instances.Add(sourceActivityInstance2);
            var destinationOperation = sourceOperation.ToDatabase();
            Assert.AreEqual(1, destinationOperation.Id);
            Assert.AreEqual("Operation", destinationOperation.Name);
            Assert.AreEqual(1, destinationOperation.Activities.Count);
            var destinationActivity = destinationOperation.Activities.Single();
            Assert.AreEqual(11, destinationActivity.Id);
            Assert.AreSame(destinationOperation, destinationActivity.Operation);
            Assert.AreEqual(2, destinationActivity.Instances.Count);
            var destinationActivityInstance1 =
                destinationActivity.Instances.Single(instance => instance.Id == activityInstanceId1);
            Assert.AreEqual(Model.Operations.ActivityInstanceState.None, destinationActivityInstance1.State);
            Assert.AreSame(destinationActivity, destinationActivityInstance1.Activity);
            var destinationActivityInstance2 =
                destinationActivity.Instances.Single(instance => instance.Id == activityInstanceId2);
            Assert.AreSame(destinationActivity, destinationActivityInstance2.Activity);
            Assert.AreEqual(Model.Operations.ActivityInstanceState.Created, destinationActivityInstance2.State);
        }

        /// <summary>
        /// Tests the mapping from database to DTO entities with complex hierarchy.
        /// </summary>
        [TestMethod]
        public void MapDatabaseToDtoComplexHierarchyTest()
        {
            var sourceOperation = new Model.Operations.Operation
                                   {
                                       Id = 1,
                                       Name = "Operation"
                                   };
            var sourceActivity = new Model.Operations.Activity
                              {
                                  Id = 11,
                                  Operation = sourceOperation
                              };
            sourceOperation.Activities.Add(sourceActivity);
            var activityInstanceId1 = new Guid("50514da8-b433-49c7-9297-682067f35429");
            var activityInstanceId2 = new Guid("308ae3cc-892b-4233-98a2-1c530b900033");
            var sourceActivityInstance1 = new Model.Operations.ActivityInstance
                              {
                                  Id = activityInstanceId1,
                                  Activity = sourceActivity
                              };
            sourceActivity.Instances.Add(sourceActivityInstance1);
            var sourceActivityInstance2 = new Model.Operations.ActivityInstance
                              {
                                  Id = activityInstanceId2,
                                  Activity = sourceActivity,
                                  State = Model.Operations.ActivityInstanceState.Created
                              };
            sourceActivity.Instances.Add(sourceActivityInstance2);
            var destinationOperation = sourceOperation.ToDto();
            Assert.AreEqual(1, destinationOperation.Id);
            Assert.AreEqual("Operation", destinationOperation.Name);
            Assert.AreEqual(1, destinationOperation.Activities.Count);
            var destinationActivity = destinationOperation.Activities.Single();
            Assert.AreEqual(11, destinationActivity.Id);
            Assert.AreSame(destinationOperation, destinationActivity.Operation);
            Assert.AreEqual(2, destinationActivity.Instances.Count);
            var destinationActivityInstance1 =
                destinationActivity.Instances.Single(instance => instance.Id == activityInstanceId1);
            Assert.AreEqual(ActivityInstanceState.None, destinationActivityInstance1.State);
            Assert.AreSame(destinationActivity, destinationActivityInstance1.Activity);
            var destinationActivityInstance2 =
                destinationActivity.Instances.Single(instance => instance.Id == activityInstanceId2);
            Assert.AreSame(destinationActivity, destinationActivityInstance2.Activity);
            Assert.AreEqual(ActivityInstanceState.Created, destinationActivityInstance2.State);
        }
         */

        /// <summary>
        /// Serializable class used for testing.
        /// </summary>
        public class TestXmlDataObject
        {
            /// <summary>
            /// Gets or sets the test data.
            /// </summary>
            [XmlAttribute]
            public string TestData { get; set; }
        }
    }
}