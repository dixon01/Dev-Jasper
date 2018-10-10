// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadableWritableMappingTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for DTO cloning
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.Client.ChangeTracking.AccessControl;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for DTO cloning
    /// </summary>
    [TestClass]
    public class ReadableWritableMappingTest
    {
        /// <summary>
        /// Initializes the test by resetting the mocks.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ResetMocks();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ResetMocks();
        }

        /// <summary>
        /// The readable to writeable simple mapping test.
        /// </summary>
        [TestMethod]
        public void ReadableToWriteableSimpleMappingTest()
        {
            var now = DateTime.UtcNow;
            var tomorrow = DateTime.UtcNow.AddDays(1);
            var userRole = new UserRole { Id = 34, Name = "TestUser" };

            var entity = new Authorization
                             {
                                 Id = 2,
                                 DataScope = DataScope.MediaConfiguration,
                                 Permission = Permission.Create,
                                 Version = 12,
                                 UserRole = userRole,
                                 CreatedOn = now,
                                 LastModifiedOn = tomorrow,
                             };

            // ReSharper disable once UnusedVariable
            var userRoleChangeTrackingManager =
                SetupChangeTrackingManager<UserRoleChangeTrackingManager, IUserRoleChangeTrackingManager>();

            var authorizationChangeTrackingManager =
                SetupChangeTrackingManager<AuthorizationChangeTrackingManager, IAuthorizationChangeTrackingManager>();

            var readable = authorizationChangeTrackingManager.Wrap(entity);
            var writeable = readable.ToChangeTrackingModel();

            Assert.IsNotNull(writeable);
            Assert.AreEqual(writeable.Id, readable.Id);
            Assert.AreEqual(writeable.DataScope, readable.DataScope);
            Assert.AreEqual(writeable.Permission, readable.Permission);
            Assert.AreEqual(writeable.Version.Value, readable.Version.Value + 1);
            Assert.AreEqual(writeable.UserRole, readable.UserRole);
            Assert.AreEqual(writeable.ReadableModel, readable);
        }

        /// <summary>
        /// The readable to writeable complex mapping test.
        /// </summary>
        [TestMethod]
        public void ReadableToWriteableComplexMappingTest()
        {
            var sourceTenant = new Tenant
                                   {
                                       Id = 1,
                                       Name = "Tenant",
                                       UpdateGroups = new Collection<UpdateGroup>()
                                   };

            var sourceProductType = new ProductType
                                        {
                                            Id = 20,
                                            Name = "ProductType",
                                            HardwareDescriptorXml = "HardwareDescriptor",
                                            Units = new Collection<Unit>()
                                        };
            var sourceUpdateGroup = new UpdateGroup
                                        {
                                            Id = 30,
                                            Tenant = sourceTenant,
                                            Units = new Collection<Unit>()
                                        };

            var sourceUnit = new Unit
                         {
                             Id = 10,
                             Tenant = sourceTenant,
                             ProductType = sourceProductType,
                             UpdateGroup = sourceUpdateGroup,
                             UserDefinedProperties = new Dictionary<string, string> { { "UDPKey", "UDPValue" } }
                         };

            sourceTenant.UpdateGroups.Add(sourceUpdateGroup);
            sourceUpdateGroup.Units.Add(sourceUnit);
            sourceProductType.Units.Add(sourceUnit);

            var unitChangeTrackingManager = SetupChangeTrackingManagers();

            var readable = unitChangeTrackingManager.Wrap(sourceUnit);
            var writeable = readable.ToChangeTrackingModel();

            Assert.IsNotNull(writeable);
            Assert.AreEqual(writeable.Id, readable.Id);
            Assert.AreEqual(writeable.Tenant, readable.Tenant);
            Assert.AreEqual(writeable.ProductType, readable.ProductType);
            Assert.AreEqual(writeable.UpdateGroup, readable.UpdateGroup);

            var writeableValues = writeable.UserDefinedProperties.Values.ToList();
            var readableValues = readable.UserDefinedProperties.Values.ToList();
            var isValuesEqual = writeableValues.Count() == readableValues.Count()
                                && writeableValues.Intersect(readableValues).Count() == readableValues.Count();
            Assert.IsTrue(isValuesEqual);
        }

        /// <summary>
        /// Tests the loading of the reference properties of a readable model.
        /// </summary>
        [TestMethod]
        public void LoadReferencePropertiesTest()
        {
            SetupUnitDataService();
            var unitChangeTrackingManager = SetupChangeTrackingManagers();
            var readable = unitChangeTrackingManager.Wrap(new Unit { Id = 10 });

            // Verify that neither reference properties nor navigation properties are loaded.
            try
            {
                // ReSharper disable once UnusedVariable
                var tenant = readable.Tenant;
                Assert.Fail("The reference property Tenant shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var productType = readable.ProductType;
                Assert.Fail("The reference property ProductType shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var updateGroup = readable.UpdateGroup;
                Assert.Fail("The reference property UpdateGroup shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var commands = readable.UpdateCommands;
                Assert.Fail("The navigation property UpdateCommands shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            readable.LoadReferencePropertiesAsync().Wait();

            // Verify that only reference properties are loaded.
            Assert.IsNotNull(readable.Tenant);
            Assert.IsNotNull(readable.ProductType);
            Assert.IsNotNull(readable.UpdateGroup);
            try
            {
                // ReSharper disable once UnusedVariable
                var commands = readable.UpdateCommands;
                Assert.Fail("The navigation property UpdateCommands shouldn't be loaded.");
            }
            catch (ChangeTrackingException)
            {
            }
        }

        /// <summary>
        /// Tests the complex mapping including the loading navigation properties.
        /// </summary>
        [TestMethod]
        public void ReadableToWriteableComplexMappingWithNavigationPropertiesTest()
        {
            SetupUnitDataService();
            var unitChangeTrackingManager = SetupChangeTrackingManagers();
            var readable = unitChangeTrackingManager.Wrap(new Unit { Id = 10 });

            // Verify that neither reference properties nor navigation properties are loaded.
            try
            {
                // ReSharper disable once UnusedVariable
                var tenant = readable.Tenant;
                Assert.Fail("The reference property Tenant shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var productType = readable.ProductType;
                Assert.Fail("The reference property ProductType shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var updateGroup = readable.UpdateGroup;
                Assert.Fail("The reference property UpdateGroup shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var commands = readable.UpdateCommands;
                Assert.Fail("The navigation property UpdateCommands shouldn't be loaded yet.");
            }
            catch (ChangeTrackingException)
            {
            }

            readable.LoadNavigationPropertiesAsync().Wait();

            // Verify that all properties are loaded.
            Assert.IsNotNull(readable.UpdateCommands);
            Assert.AreEqual(1, readable.UpdateCommands.Count);
            Assert.IsNotNull(readable.Tenant);
            Assert.IsNotNull(readable.ProductType);
            Assert.IsNotNull(readable.UpdateGroup);

            var writeable = readable.ToChangeTrackingModel();

            Assert.IsNotNull(writeable);
            Assert.AreEqual(writeable.Id, readable.Id);
            Assert.AreEqual(writeable.Tenant, readable.Tenant);
            Assert.AreEqual(writeable.ProductType, readable.ProductType);
            Assert.AreEqual(writeable.UpdateGroup, readable.UpdateGroup);

            var writeableValues = writeable.UserDefinedProperties.Values.ToList();
            var readableValues = readable.UserDefinedProperties.Values.ToList();
            var isValuesEqual = writeableValues.Count() == readableValues.Count()
                                && writeableValues.Intersect(readableValues).Count() == readableValues.Count();
            Assert.IsTrue(isValuesEqual);
        }

        private static IUnitChangeTrackingManager SetupChangeTrackingManagers()
        {
            // ReSharper disable once UnusedVariable
            var tenantChangeTrackingManager =
                SetupChangeTrackingManager<TenantChangeTrackingManager, ITenantChangeTrackingManager>();

            // ReSharper disable once UnusedVariable
            var productTypeChangeTrackingManager =
                SetupChangeTrackingManager<ProductTypeChangeTrackingManager, IProductTypeChangeTrackingManager>();

            var unitChangeTrackingManager =
                SetupChangeTrackingManager<UnitChangeTrackingManager, IUnitChangeTrackingManager>();

            // ReSharper disable once UnusedVariable
            var updateGroupChangeTrackingManager =
                SetupChangeTrackingManager<UpdateGroupChangeTrackingManager, IUpdateGroupChangeTrackingManager>();

            // ReSharper disable once UnusedVariable
            var updateCommandChangeTrackingManager =
                SetupChangeTrackingManager<UpdateCommandChangeTrackingManager, IUpdateCommandChangeTrackingManager>();

            return unitChangeTrackingManager;
        }

        private static T SetupChangeTrackingManager<T, TInterface>() where T : ChangeTrackingManagerBase, TInterface
        {
            var changeTrackingManager =
                (T)Activator.CreateInstance(typeof(T), CreateNotificationSubscriptionConfiguration(), null);
            DependencyResolver.Current.Register<TInterface>(changeTrackingManager);
            var wait = new ManualResetEventSlim();
            changeTrackingManager.Running += (sender, args) => wait.Set();
            Task.Run(() => changeTrackingManager.RunAsync());
            return changeTrackingManager;
        }

        private static void SetupUnitDataService()
        {
            var unitDataServiceMock = new Mock<IUnitDataService>();
            var unitDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUnitDataService>>();
            var unitChannel = ChannelScope<IUnitDataService>.Create(unitDataServiceMock.Object);
            unitDataServiceFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(unitChannel);
            ChannelScopeFactory<IUnitDataService>.SetCurrent(unitDataServiceFactoryMock.Object);

            var sourceTenant = new Tenant
            {
                Id = 1,
                Name = "Tenant",
                UpdateGroups = new Collection<UpdateGroup>()
            };

            var sourceProductType = new ProductType
            {
                Id = 20,
                Name = "ProductType",
                HardwareDescriptorXml = "HardwareDescriptor",
                Units = new Collection<Unit>()
            };
            var sourceUpdateGroup = new UpdateGroup
            {
                Id = 30,
                Tenant = sourceTenant,
                UpdateParts = new Collection<UpdatePart>(),
                Units = new Collection<Unit>()
            };

            var sourceUpdateCommand = new UpdateCommand
            {
                CommandXml = "CommandXml",
                Id = 40,
                IncludedParts = new Collection<UpdatePart>(),
            };
            var sourceUnit = new Unit
            {
                Id = 10,
                Tenant = sourceTenant,
                ProductType = sourceProductType,
                UpdateGroup = sourceUpdateGroup,
                UserDefinedProperties = new Dictionary<string, string> { { "UDPKey", "UDPValue" } },
                UpdateCommands = new Collection<UpdateCommand> { sourceUpdateCommand }
            };

            sourceUpdateCommand.Unit = sourceUnit;
            sourceTenant.UpdateGroups.Add(sourceUpdateGroup);
            sourceUpdateGroup.Units.Add(sourceUnit);
            sourceProductType.Units.Add(sourceUnit);

            var units = new[] { sourceUnit };
            var unitsDictionary = units.ToDictionary(unit => unit.Id, user => user);
            var unitTaskCompletionSource = new TaskCompletionSource<Unit>();
            unitTaskCompletionSource.SetResult(unitsDictionary[10]);
            unitDataServiceMock.Setup(service => service.GetAsync(10)).Returns(unitTaskCompletionSource.Task);
            unitDataServiceMock.Setup(service => service.QueryAsync(It.IsAny<UnitQuery>()))
                .Returns<UnitQuery>(
                    q =>
                    Task.FromResult(
                        (IEnumerable<Unit>)
                        units.Select(u => u.ToDatabase()).AsQueryable().Apply(q).Select(u => u.ToDto(q))));
        }

        private static void ResetMocks()
        {
            var unitDataServiceFactoryMock =
                new Mock<ChannelScopeFactory<IUnitDataService>>(MockBehavior.Strict);
            ChannelScopeFactory<IUnitDataService>.SetCurrent(unitDataServiceFactoryMock.Object);
            var readyGateFactoryMock = new Mock<ReadyGateFactory>();
            var readyGateMock = new Mock<IReadyGate>();
            var completedTask = new TaskCompletionSource<bool>();
            completedTask.SetResult(true);
            readyGateMock.Setup(gate => gate.PingPongAsync()).Returns(completedTask.Task);
            readyGateMock.Setup(gate => gate.PongAsync(It.IsAny<PongNotification>())).Returns(completedTask.Task);
            readyGateMock.Setup(gate => gate.WaitReadyAsync()).Returns(completedTask.Task);
            readyGateFactoryMock.Setup(
                factory => factory.Create(It.IsAny<string>(), It.IsAny<Func<Notification, Task<Guid>>>()))
                .Returns(readyGateMock.Object);
            ReadyGateFactory.SetCurrent(readyGateFactoryMock.Object);
        }

        private static NotificationSubscriptionConfiguration CreateNotificationSubscriptionConfiguration()
        {
            return new NotificationSubscriptionConfiguration(
                new NotificationManagerConfiguration(),
                string.Empty,
                string.Empty,
                string.Empty);
        }
    }
}
