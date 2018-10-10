// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingManagerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.Tests.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.ServiceModel.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Unit tests for the change tracking managers.
    /// </summary>
    [TestClass]
    public class ChangeTrackingManagerTest
    {
        private static List<User> userList;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ResetMocks();
            userList = new List<User>();
        }

        /// <summary>
        /// Cleans the test up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ResetMocks();
        }

        /// <summary>
        /// Tests the loading of tenant and users starting from a tenant.
        /// </summary>
        [TestMethod]
        public void TestStartingFromTenant()
        {
            var userDataService = SetupUserDataService();
            SetupTenantDataService();
            var updateGroupDataService = SetupUpdateGroupDataService();
            var tenantChangeTrackingManager = SetupTenantChangeTrackingManager();
            SetupUserChangeTrackingManager();
            SetupUpdateGroupChangeTrackingManager();
            var tenant = tenantChangeTrackingManager.GetAsync(1).Result;
            userDataService.Verify(service => service.QueryAsync(It.IsAny<UserQuery>()), Times.Never());
            tenant.LoadNavigationPropertiesAsync().Wait();
            Assert.IsNotNull(tenant.Users);
            Assert.AreEqual(1, tenant.Users.Count);
            Assert.IsNotNull(tenant.Users[0].OwnerTenant);
            Assert.AreSame(tenant, tenant.Users[0].OwnerTenant);
            Assert.AreEqual(1, tenant.Users[0].OwnerTenant.Users.Count);
            updateGroupDataService.Verify(service => service.QueryAsync(It.IsAny<UpdateGroupQuery>()), Times.Never());
            Assert.AreEqual(1, tenant.UpdateGroups.Count);
            userDataService.Verify(service => service.QueryAsync(It.IsAny<UserQuery>()), Times.Never());
        }

        /// <summary>
        /// Tests the loading of a user and navigation through models.
        /// </summary>
        [TestMethod]
        public void TestStartingFromUsers()
        {
            SetupUserDataService();
            SetupTenantDataService();
            SetupUpdateGroupDataService();
            SetupTenantChangeTrackingManager();
            var userChangeTrackingManager = SetupUserChangeTrackingManager();
            SetupUpdateGroupChangeTrackingManager();
            var user = userChangeTrackingManager.GetAsync(1).Result;
            user.OwnerTenant.LoadNavigationPropertiesAsync().Wait();
            Assert.IsNotNull(user.OwnerTenant);
            Assert.AreEqual(1, user.OwnerTenant.UpdateGroups.Count);
        }

        /// <summary>
        /// Tests adding an entity through a ChangeTrackingManager.
        /// </summary>
        [TestMethod]
        public void AddTestAsync()
        {
            var userDataServiceMock = SetupUserDataService();
            SetupTenantDataService();
            SetupUpdateGroupDataService();
            var tenantChangeTrackingManager = SetupTenantChangeTrackingManager();
            var userChangeTrackingManager = SetupUserChangeTrackingManager();
            SetupUpdateGroupChangeTrackingManager();
            var user = userChangeTrackingManager.Create();
            user.FirstName = "TestUser";
            var tenant = tenantChangeTrackingManager.GetAsync(1).Result;
            user.OwnerTenant = tenant;
            userChangeTrackingManager.AddAsync(user).Wait();
            userDataServiceMock.Verify(service => service.AddAsync(It.IsAny<User>()), Times.Once());
            Assert.AreEqual(1, userList.Count);
            var addedUser = userList.FirstOrDefault();
            Assert.IsNotNull(addedUser);
            Assert.AreEqual(user.FirstName, addedUser.FirstName);
            Assert.IsNotNull(addedUser.OwnerTenant);
            Assert.AreEqual(1, addedUser.OwnerTenant.Id);
        }

        /// <summary>
        /// Tests the deletion of an entity through a ChangeTrackingManager.
        /// </summary>
        [TestMethod]
        public void DeleteTestAsync()
        {
            var userDataServiceMock = SetupUserDataService();
            var userTaskCompletionSource = new TaskCompletionSource<User>();
            userTaskCompletionSource.SetResult(new User());
            userDataServiceMock.Setup(service => service.DeleteAsync(It.IsAny<User>()))
                .Returns(userTaskCompletionSource.Task);
            SetupTenantDataService();
            SetupUpdateGroupDataService();
            SetupTenantChangeTrackingManager();
            var userChangeTrackingManager = SetupUserChangeTrackingManager();
            SetupUpdateGroupChangeTrackingManager();
            var user = userChangeTrackingManager.GetAsync(1).Result;
            userChangeTrackingManager.DeleteAsync(user).Wait();
            userDataServiceMock.Verify(service => service.DeleteAsync(It.IsAny<User>()), Times.Once());
        }

        /// <summary>
        /// Tests the query of entities through a ChangeTrackingManager.
        /// </summary>
        [TestMethod]
        public void QueryTestAsync()
        {
            SetupTenantDataService();
            SetupTenantChangeTrackingManager();

            var userDataServiceMock = SetupMultiUserDataService();
            var userChangeTrackingManager = SetupUserChangeTrackingManager();

            var userQuery = userChangeTrackingManager.QueryAsync().Result.ToList();
            userDataServiceMock.Verify(service => service.QueryAsync(It.IsAny<UserQuery>()), Times.Once());

            Assert.AreEqual(userQuery.Count(), 3);
            Assert.IsTrue(userQuery.Any(u => u.Id == 1 && u.OwnerTenant.Id == 1));
            Assert.IsTrue(userQuery.Any(u => u.Id == 3 && u.OwnerTenant.Id == 2));
            Assert.IsTrue(userQuery.Any(u => u.Id == 5 && u.OwnerTenant.Id == 1));
        }

        /// <summary>
        /// Tests the query of entities through a ChangeTrackingManager.
        /// </summary>
        [TestMethod]
        public void QueryWithFilterTestAsync()
        {
            SetupTenantDataService();
            var tenantChangeTrackingManager = SetupTenantChangeTrackingManager();

            var userDataServiceMock = SetupTenantFilteredUserDataService();
            var userChangeTrackingManager = SetupUserChangeTrackingManager();

            var filterTenant = tenantChangeTrackingManager.GetAsync(1).Result.ToDto();

            var filter = new UserQuery().WithOwnerTenant(filterTenant);
            var userQuery = userChangeTrackingManager.QueryAsync(filter).Result.ToList();
            userDataServiceMock.Verify(service => service.QueryAsync(It.IsAny<UserQuery>()), Times.Once());

            Assert.AreEqual(1, userQuery.Count());
            Assert.IsTrue(userQuery.Any(u => u.Id == 3 && u.OwnerTenant.Id == 2));
        }

        private static Mock<IUserDataService> SetupTenantFilteredUserDataService()
        {
            var userDataServiceMock = new Mock<IUserDataService>();

            var userDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUserDataService>>();
            var userChannel = ChannelScope<IUserDataService>.Create(userDataServiceMock.Object);
            userDataServiceFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(userChannel);
            ChannelScopeFactory<IUserDataService>.SetCurrent(userDataServiceFactoryMock.Object);

            var tenant2 = new Tenant { Id = 2 };
            var users = new[] { new User { Id = 3, OwnerTenant = tenant2 } };

            var queryResult = new TaskCompletionSource<IEnumerable<User>>();
            queryResult.SetResult(users);

            var emptyUserFilter = UserQuery.Create();
            emptyUserFilter.IncludeOwnerTenant(TenantFilter.Create().WithId(1));

            userDataServiceMock.Setup(
                service =>
                service.QueryAsync(
                    It.Is<UserQuery>(
                        filter =>
                        filter.AssociationTenantUserUserRoles == null && filter.OwnerTenant.Id != null
                        && filter.OwnerTenant.Id.Value == 1))).Returns(queryResult.Task);

            return userDataServiceMock;
        }

        private static Mock<IUserDataService> SetupMultiUserDataService()
        {
            var userDataServiceMock = new Mock<IUserDataService>();

            var userDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUserDataService>>();
            var userChannel = ChannelScope<IUserDataService>.Create(userDataServiceMock.Object);
            userDataServiceFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(userChannel);
            ChannelScopeFactory<IUserDataService>.SetCurrent(userDataServiceFactoryMock.Object);

            var tenant = new Tenant { Id = 1 };
            var tenant2 = new Tenant { Id = 2 };
            var users = new[]
                        {
                            new User { Id = 1, OwnerTenant = tenant },
                            new User { Id = 3, OwnerTenant = tenant2 },
                            new User { Id = 5, OwnerTenant = tenant }
                        };

            var queryResult = new TaskCompletionSource<IEnumerable<User>>();
            queryResult.SetResult(users);

            var emptyUserFilter = UserQuery.Create();
            emptyUserFilter.IncludeOwnerTenant();
            userDataServiceMock.Setup(service => service.QueryAsync(It.IsAny<UserQuery>())).Returns(queryResult.Task);

            return userDataServiceMock;
        }

        private static void ResetMocks()
        {
            var tenantDataServiceFactoryMock = new Mock<ChannelScopeFactory<ITenantDataService>>(MockBehavior.Strict);
            ChannelScopeFactory<ITenantDataService>.SetCurrent(tenantDataServiceFactoryMock.Object);
            var userDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUserDataService>>(MockBehavior.Strict);
            ChannelScopeFactory<IUserDataService>.SetCurrent(userDataServiceFactoryMock.Object);
            var updateGroupDataServiceFactoryMock =
                new Mock<ChannelScopeFactory<IUpdateGroupDataService>>(MockBehavior.Strict);
            ChannelScopeFactory<IUpdateGroupDataService>.SetCurrent(updateGroupDataServiceFactoryMock.Object);
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

        private static void SetupTenantDataService()
        {
            var tenantDataServiceMock = new Mock<ITenantDataService>();
            var tenantDataServiceFactoryMock = new Mock<ChannelScopeFactory<ITenantDataService>>();
            var tenantChannel = ChannelScope<ITenantDataService>.Create(tenantDataServiceMock.Object);
            tenantDataServiceFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(tenantChannel);
            ChannelScopeFactory<ITenantDataService>.SetCurrent(tenantDataServiceFactoryMock.Object);

            var user = new User { Id = 1 };
            var tenant1 = new Tenant
                              {
                                  Id = 1,
                                  Users = new List<User> { user },
                                  UpdateGroups = new List<UpdateGroup> { new UpdateGroup { Id = 1 } }
                              };
            user.OwnerTenant = tenant1;
            var tenants = new[]
                              {
                                  tenant1
                              };
            var tenantsDictionary = tenants.ToDictionary(tenant => tenant.Id, tenant => tenant);
            var tenantTaskCompletionSource = new TaskCompletionSource<Tenant>();
            tenantTaskCompletionSource.SetResult(tenantsDictionary[1]);
            tenantDataServiceMock.Setup(service => service.GetAsync(1)).Returns(tenantTaskCompletionSource.Task);
            var queryResult = new TaskCompletionSource<IEnumerable<Tenant>>();
            queryResult.SetResult(tenants);
            tenantDataServiceMock.Setup(service => service.QueryAsync(It.IsAny<TenantQuery>()))
                .Returns(queryResult.Task);
        }

        private static ITenantChangeTrackingManager SetupTenantChangeTrackingManager()
        {
            var tenantChangeTrackingManager =
                new TenantChangeTrackingManager(CreateNotificationSubscriptionConfiguration(), null);
            DependencyResolver.Current.Register<ITenantChangeTrackingManager>(tenantChangeTrackingManager);
            var wait = new ManualResetEventSlim();
            tenantChangeTrackingManager.Running += (sender, args) => wait.Set();
            Task.Run(() => tenantChangeTrackingManager.RunAsync());
            return tenantChangeTrackingManager;
        }

        private static Mock<IUserDataService> SetupUserDataService()
        {
            var userDataServiceMock = new Mock<IUserDataService>();
            var userDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUserDataService>>();
            var userChannel = ChannelScope<IUserDataService>.Create(userDataServiceMock.Object);
            userDataServiceFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(userChannel);
            ChannelScopeFactory<IUserDataService>.SetCurrent(userDataServiceFactoryMock.Object);

            var tenant = new Tenant { Id = 1 };
            var users = new[] { new User { Id = 1, OwnerTenant = tenant } };
            var usersDictionary = users.ToDictionary(user => user.Id, user => user);
            var userTaskCompletionSource = new TaskCompletionSource<User>();
            userTaskCompletionSource.SetResult(usersDictionary[1]);
            userDataServiceMock.Setup(service => service.GetAsync(1)).Returns(userTaskCompletionSource.Task);
            userDataServiceMock.Setup(service => service.AddAsync(It.IsAny<User>()))
                .Returns(userTaskCompletionSource.Task)
                .Callback((User user) => userList.Add(user));
            var queryResult = new TaskCompletionSource<IEnumerable<User>>();
            queryResult.SetResult(users);
            userDataServiceMock.Setup(service => service.QueryAsync(It.IsAny<UserQuery>())).Returns(queryResult.Task);
            return userDataServiceMock;
        }

        private static IUserChangeTrackingManager SetupUserChangeTrackingManager()
        {
            var userChangeTrackingManager =
                new UserChangeTrackingManager(CreateNotificationSubscriptionConfiguration(), null);
            DependencyResolver.Current.Register<IUserChangeTrackingManager>(userChangeTrackingManager);
            var wait = new ManualResetEventSlim();
            userChangeTrackingManager.Running += (sender, args) => wait.Set();
            Task.Run(() => userChangeTrackingManager.RunAsync());
            return userChangeTrackingManager;
        }

        private static NotificationSubscriptionConfiguration CreateNotificationSubscriptionConfiguration()
        {
            return new NotificationSubscriptionConfiguration(
                new NotificationManagerConfiguration(),
                string.Empty,
                string.Empty,
                string.Empty);
        }

        private static Mock<IUpdateGroupDataService> SetupUpdateGroupDataService()
        {
            var updateGroupDataServiceMock = new Mock<IUpdateGroupDataService>();
            var updateGroupChannelScopeFactoryMock = new Mock<ChannelScopeFactory<IUpdateGroupDataService>>();
            var tenantChannel = ChannelScope<IUpdateGroupDataService>.Create(updateGroupDataServiceMock.Object);
            updateGroupChannelScopeFactoryMock.Setup(factory => factory.Create(It.IsAny<UserCredentials>()))
                .Returns(tenantChannel);
            ChannelScopeFactory<IUpdateGroupDataService>.SetCurrent(updateGroupChannelScopeFactoryMock.Object);

            var updateGroups = new[] { new UpdateGroup { Id = 1 } };
            var updateGroupsDictionary = updateGroups.ToDictionary(
                updateGroup => updateGroup.Id,
                updateGroup => updateGroup);
            var updateGroupTaskCompletionSource = new TaskCompletionSource<UpdateGroup>();
            updateGroupTaskCompletionSource.SetResult(updateGroupsDictionary[1]);
            updateGroupDataServiceMock.Setup(service => service.GetAsync(1))
                .Returns(updateGroupTaskCompletionSource.Task);
            var queryResult = new TaskCompletionSource<IEnumerable<UpdateGroup>>();
            queryResult.SetResult(updateGroups);
            updateGroupDataServiceMock.Setup(service => service.QueryAsync(It.IsAny<UpdateGroupQuery>()))
                .Returns(queryResult.Task);
            return updateGroupDataServiceMock;
        }

        private static IUpdateGroupChangeTrackingManager SetupUpdateGroupChangeTrackingManager()
        {
            var updateGroupChangeTrackingManager
                =
                new UpdateGroupChangeTrackingManager(CreateNotificationSubscriptionConfiguration(), null);
            DependencyResolver.Current.Register<IUpdateGroupChangeTrackingManager>(updateGroupChangeTrackingManager);
            var wait = new ManualResetEventSlim();
            updateGroupChangeTrackingManager.Running += (sender, args) => wait.Set();
            Task.Run(() => updateGroupChangeTrackingManager.RunAsync());
            return updateGroupChangeTrackingManager;
        }
    }
}