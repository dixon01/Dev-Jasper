// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingDataServicesTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   CRUD tests for ChangeTrackingDataServices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Membership;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// CRUD tests for ChangeTrackingDataServices. Only Add, Delete and Update are tested, because
    /// Get and Query just call the method on the underlying data service.
    /// </summary>
    [TestClass]
    public class ChangeTrackingDataServicesTest
    {
        private static Dictionary<int, User> usersDictionary;

        /// <summary>
        /// Initializes the <see cref="NotificationManagerFactory"/>.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ResetMocks();
            NotificationManagerFactory.Set(new TestNotificationManagerFactory());
        }

        /// <summary>
        /// Cleans up the mocks and local variables.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            usersDictionary = null;
            ResetMocks();
        }

        /// <summary>
        /// Tests adding an entity through change tracking.
        /// </summary>
        [TestMethod]
        public void AddSimpleEntityTest()
        {
            UserDeltaNotification notification = null;
            var notificationMock =
                ((TestNotificationManagerFactory)NotificationManagerFactory.Current).NotificationManagerMock;
            var guid = Guid.NewGuid();
            notificationMock.Setup(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()))
                .Returns(Task.FromResult(guid))
                .Callback((UserDeltaNotification n) => notification = n);
            var userChangeTrackingDataService = this.SetupUserChangeTrackingDataService();
            var tenant = new Tenant { Id = 20, Name = "AddTestTenant" };
            var user = new User
                           {
                               Id = 30,
                               OwnerTenant = tenant
                           };
            var addedUser = userChangeTrackingDataService.AddAsync(user).Result;
            Assert.IsNotNull(addedUser);
            Assert.IsNotNull(addedUser.OwnerTenant);
            Assert.AreEqual(20, addedUser.OwnerTenant.Id);
            notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Once());
            Assert.IsNotNull(notification);
            Assert.AreEqual(DeltaNotificationType.EntityAdded, notification.NotificationType);
            var delta = notification.Delta;
            Assert.AreEqual(30, delta.Id);
            Assert.AreEqual(2, usersDictionary.Count);
        }

        /// <summary>
        /// Test the deletion of an existing and also non existing entity.
        /// </summary>
        [TestMethod]
        public void DeleteSimpleEntityTest()
        {
            UserDeltaNotification notification = null;
            var notificationMock =
                 ((TestNotificationManagerFactory)NotificationManagerFactory.Current).NotificationManagerMock;
            notificationMock.Setup(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()))
                .Returns(Task.FromResult(new Guid()))
                .Callback((UserDeltaNotification n) => notification = n);
            var userChangeTrackingDataService = this.SetupUserChangeTrackingDataService();
            Assert.AreEqual(1, usersDictionary.Count);
            userChangeTrackingDataService.DeleteAsync(new User { Id = 10 }).Wait();

            notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Once());
            Assert.IsNotNull(notification);
            Assert.AreEqual(notification.NotificationType, DeltaNotificationType.EntityRemoved);
            var delta = notification.Delta;
            Assert.AreEqual(10, delta.Id);
            Assert.AreEqual(DeltaOperation.Deleted, delta.DeltaOperation);
            Assert.AreEqual(0, usersDictionary.Count);

            // Try to delete a non existing entity
            try
            {
                userChangeTrackingDataService.DeleteAsync(new User { Id = 11 }).Wait();
            }
            catch (Exception)
            {
                notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Exactly(2));
                return;
            }

            Assert.Fail("Deletion of a non existing User should throw an exception");
        }

        /// <summary>
        /// Tests the update of an existing and a non existing entity.
        /// </summary>
        [TestMethod]
        public void UpdateEntityTest()
        {
            UserDeltaNotification notification = null;
            var notificationMock =
                 ((TestNotificationManagerFactory)NotificationManagerFactory.Current).NotificationManagerMock;
            notificationMock.Setup(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()))
                .Returns(Task.FromResult(new Guid()))
                .Callback((UserDeltaNotification n) => notification = n);
            var userChangeTrackingDataService = this.SetupUserChangeTrackingDataService();
            var user = new User { Id = 10, FirstName = "UpdatedUser", };
            var updatedUser = userChangeTrackingDataService.UpdateAsync(user).Result;
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(1, usersDictionary.Count);
            Assert.AreEqual("UpdatedUser", usersDictionary[updatedUser.Id].FirstName);
            notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Once());
            Assert.IsNotNull(notification);

            // Second update
            user.LastName = "SecondUpdate";
            updatedUser = userChangeTrackingDataService.UpdateAsync(user).Result;
            Assert.AreEqual(1, usersDictionary.Count);
            Assert.AreEqual("SecondUpdate", usersDictionary[updatedUser.Id].LastName);
            notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Exactly(2));

            // Try update a non existing user
            try
            {
                // ReSharper disable once RedundantAssignment
                updatedUser = userChangeTrackingDataService.UpdateAsync(new User { Id = 20 }).Result;
            }
            catch (Exception)
            {
                notificationMock.Verify(mock => mock.PostAsync(It.IsAny<UserDeltaNotification>()), Times.Exactly(3));
                return;
            }

            Assert.Fail("Update of a non existing entity should throw an exception");
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
            var users = new[] { new User { Id = 10, OwnerTenant = tenant, FirstName = "MockUser" } };
            usersDictionary = users.ToDictionary(user => user.Id, user => user);
            var userTaskCompletionSource = new TaskCompletionSource<User>();
            userTaskCompletionSource.SetResult(usersDictionary[10]);
            userDataServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                .Returns((int i) => Task.FromResult(usersDictionary[i]));
            userDataServiceMock.Setup(service => service.AddAsync(It.IsAny<User>()))
                .Returns((User u) => Task.FromResult(u)).Callback((User u) => usersDictionary[u.Id] = u);
            userDataServiceMock.Setup(service => service.DeleteAsync(It.IsAny<User>())).Returns(
                (User u) =>
                    {
                        if (usersDictionary.ContainsKey(u.Id))
                        {
                            usersDictionary.Remove(u.Id);
                            return Task.FromResult(0);
                        }

                        throw new Exception();
                    });
            userDataServiceMock.Setup(service => service.UpdateAsync(It.IsAny<User>())).Returns(
                (User u) =>
                    {
                        if (usersDictionary.ContainsKey(u.Id))
                        {
                            usersDictionary[u.Id] = u;
                            return Task.FromResult(u);
                        }

                        throw new Exception();
                    });
            return userDataServiceMock;
        }

        private static void ResetMocks()
        {
            var userDataServiceFactoryMock = new Mock<ChannelScopeFactory<IUserDataService>>(MockBehavior.Strict);
            ChannelScopeFactory<IUserDataService>.SetCurrent(userDataServiceFactoryMock.Object);
            NotificationManagerFactory.Reset();
        }

        private IUserDataService SetupUserChangeTrackingDataService()
        {
            var service = new UserChangeTrackingDataService(
                SetupUserDataService().Object,
                new BackgroundSystemConfiguration(),
                new NotificationSubscriptionConfiguration(new NotificationManagerConfiguration(), "Test", "Test"));
            DependencyResolver.Current.Register<IUserDataService>(service);
            var wait = new ManualResetEventSlim();
            service.ServiceReady += (sender, args) => wait.Set();
            Task.Run(() => service.StartAsync());
            Assert.IsTrue(wait.Wait(TimeSpan.FromSeconds(5)));
            return service;
        }

        private class TestNotificationSubscriber : INotificationSubscriber
        {
            public void Dispose()
            {
            }
        }

        private class TestNotificationManagerFactory : NotificationManagerFactory
        {
            public TestNotificationManagerFactory()
            {
                this.NotificationManagerMock = this.SetupNotificationManagerMock();
            }

            public Mock<INotificationManager> NotificationManagerMock { get; set; }

            public override INotificationManager Create(NotificationManagerConfiguration configuration)
            {
                return this.NotificationManagerMock.Object;
            }

            private Mock<INotificationManager> SetupNotificationManagerMock()
            {
                var notificationMock = new Mock<INotificationManager>();
                var subscriberTaskCompletionSource = new TaskCompletionSource<INotificationSubscriber>();
                subscriberTaskCompletionSource.SetResult(new TestNotificationSubscriber());
                notificationMock.Setup(
                    mock =>
                    mock.SubscribeAsync(
                        It.IsAny<INotificationObserver>(),
                        It.IsAny<NotificationSubscriptionConfiguration>()))
                    .Returns(subscriberTaskCompletionSource.Task);
                notificationMock.Setup(mock => mock.PostAsync(It.IsAny<PongNotification>()))
                    .Returns(Task.FromResult(Guid.NewGuid()));
                return notificationMock;
            }
        }
    }
}
