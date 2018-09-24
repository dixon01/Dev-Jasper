// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRoleDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserRoleDataControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers;
    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The user role data controller test.
    /// </summary>
    [TestClass]
    public class UserRoleDataControllerTest
    {
        /// <summary>
        /// Tests that the user role selection is dynamically updated.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        public void TestDynamicUserRoleSelection()
        {
            var container = Helpers.InitializeServiceLocator();
            var stateMock = Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant1" });
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var connectionController = new ConnectionControllerMock();
            connectionController.Configure();
            dataController.Initialize(connectionController);

            var dispatcherMock = new Mock<IDispatcher>();
            dispatcherMock.Setup(d => d.Dispatch(It.IsAny<Action>())).Callback<Action>(a => a());
            container.RegisterInstance(dispatcherMock.Object);

            var permissionController = new PermissionController(connectionController);
            var appControllerMock = new Mock<IAdminApplicationController>();
            appControllerMock.Setup(c => c.PermissionController).Returns(permissionController);
            container.RegisterInstance(appControllerMock.Object);

            var tenant1 = connectionController.TenantChangeTrackingManager.Create();
            tenant1.Name = "TestTenant1";
            var tenant1Readable = connectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(tenant1).Result;

            var tenant2 = connectionController.TenantChangeTrackingManager.Create();
            tenant2.Name = "TestTenant2";
            var tenant2Readable = connectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(tenant2).Result;

            var user1 = connectionController.UserChangeTrackingManager.Create();
            user1.OwnerTenant = tenant1Readable;
            var user1Readable = connectionController.UserChangeTrackingManager.CommitAndVerifyAsync(user1).Result;

            var user2 = connectionController.UserChangeTrackingManager.Create();
            user2.OwnerTenant = tenant2Readable;
            var user2Readable = connectionController.UserChangeTrackingManager.CommitAndVerifyAsync(user2).Result;

            var userRole1 = connectionController.UserRoleChangeTrackingManager.Create();
            userRole1.Name = "LowestRole";
            var userRole1Readable =
                connectionController.UserRoleChangeTrackingManager.CommitAndVerifyAsync(userRole1).Result;

            var userRole2 = connectionController.UserRoleChangeTrackingManager.Create();
            userRole2.Name = "MiddleRole";
            var userRole2Readable =
                connectionController.UserRoleChangeTrackingManager.CommitAndVerifyAsync(userRole2).Result;

            var userRole3 = connectionController.UserRoleChangeTrackingManager.Create();
            userRole3.Name = "HighestRole";
            var userRole3Readable =
                connectionController.UserRoleChangeTrackingManager.CommitAndVerifyAsync(userRole3).Result;

            AddLowAuthorizations(connectionController, userRole1Readable);
            AddMiddleAuthorizations(connectionController, userRole2Readable);
            AddHighAuthorizations(connectionController, userRole3Readable);

            var association = connectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
            association.User = user1Readable;
            association.Tenant = tenant1Readable;
            association.UserRole = userRole1Readable;
            association.Commit();

            association = connectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
            association.User = user1Readable;
            association.Tenant = tenant2Readable;
            association.UserRole = userRole2Readable;
            association.Commit();

            association = connectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
            association.User = user2Readable;
            association.Tenant = tenant1Readable;
            association.UserRole = userRole2Readable;
            association.Commit();

            association = connectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
            association.User = user2Readable;
            association.Tenant = tenant2Readable;
            association.UserRole = userRole3Readable;
            association.Commit();
            var associationReadable =
                connectionController.AssociationTenantUserUserRoleChangeTrackingManager.CommitAndVerifyAsync(
                    association).Result;

            userRole1Readable.LoadNavigationPropertiesAsync().Wait();
            userRole2Readable.LoadNavigationPropertiesAsync().Wait();
            userRole3Readable.LoadNavigationPropertiesAsync().Wait();

            stateMock.Object.CurrentTenant = tenant2Readable;
            stateMock.Object.CurrentUser = user1Readable.ToDto();
            stateMock.Setup(s => s.AuthorizedTenants)
                .Returns(new ObservableCollection<TenantReadableModel>(new[] { tenant1Readable, tenant2Readable }));
            permissionController.LoadPermissionsAsync(
                user1Readable.ToDto(),
                DataScope.CenterAdmin,
                Enum.GetValues(typeof(DataScope)).Cast<DataScope>().ToArray()).Wait();

            // LOGIN WITH USER 1
            var dvm = dataController.Factory.CreateReadOnly(associationReadable);
            var writable = dataController.AssociationTenantUserUserRole.EditEntityAsync(dvm).Result;
            var target = writable as AssociationTenantUserUserRoleDataViewModel;
            Assert.IsNotNull(target);

            Assert.IsNotNull(target.Tenant.SelectedEntity);
            Assert.AreEqual("TestTenant2", target.Tenant.SelectedEntity.Name);
            Assert.AreEqual(1, target.Tenant.Entities.Count);
            Assert.IsTrue(target.Tenant.Entities.Any(t => t.Name == "TestTenant2"));

            Assert.AreEqual(2, target.UserRole.Entities.Count);
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "LowestRole"));
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "MiddleRole"));

            // LOGIN WITH USER 2
            stateMock.Object.CurrentUser = user2Readable.ToDto();
            permissionController.LoadPermissionsAsync(
                user2Readable.ToDto(),
                DataScope.CenterAdmin,
                Enum.GetValues(typeof(DataScope)).Cast<DataScope>().ToArray()).Wait();
            writable.Dispose();

            dvm = dataController.Factory.CreateReadOnly(associationReadable);
            writable = dataController.AssociationTenantUserUserRole.EditEntityAsync(dvm).Result;
            target = writable as AssociationTenantUserUserRoleDataViewModel;
            Assert.IsNotNull(target);

            Assert.IsNotNull(target.Tenant.SelectedEntity);
            Assert.AreEqual("TestTenant2", target.Tenant.SelectedEntity.Name);
            Assert.AreEqual(2, target.Tenant.Entities.Count);
            Assert.IsTrue(target.Tenant.Entities.Any(t => t.Name == "TestTenant1"));
            Assert.IsTrue(target.Tenant.Entities.Any(t => t.Name == "TestTenant2"));

            Assert.AreEqual(3, target.UserRole.Entities.Count);
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "LowestRole"));
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "MiddleRole"));
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "HighestRole"));

            target.Tenant.SelectedEntity = target.Tenant.Entities.First(t => t.Name == "TestTenant1");
            Assert.AreEqual(2, target.UserRole.Entities.Count);
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "LowestRole"));
            Assert.IsTrue(target.UserRole.Entities.Any(u => u.Name == "MiddleRole"));
        }

        private static void AddLowAuthorizations(
            ConnectionControllerMock connectionController,
            UserRoleReadableModel userRole)
        {
            AddAuthorization(connectionController, userRole, DataScope.Update, Permission.Create);
            AddAuthorization(connectionController, userRole, DataScope.Update, Permission.Read);
            AddAuthorization(connectionController, userRole, DataScope.Update, Permission.Write);
            AddAuthorization(connectionController, userRole, DataScope.Update, Permission.Delete);
            AddAuthorization(connectionController, userRole, DataScope.UnitConfiguration, Permission.Create);
            AddAuthorization(connectionController, userRole, DataScope.UnitConfiguration, Permission.Read);
            AddAuthorization(connectionController, userRole, DataScope.UnitConfiguration, Permission.Write);
            AddAuthorization(connectionController, userRole, DataScope.UnitConfiguration, Permission.Delete);
            AddAuthorization(connectionController, userRole, DataScope.CenterAdmin, Permission.Interact);
        }

        private static void AddMiddleAuthorizations(
            ConnectionControllerMock connectionController,
            UserRoleReadableModel userRole)
        {
            AddAuthorization(connectionController, userRole, DataScope.User, Permission.Create);
            AddAuthorization(connectionController, userRole, DataScope.User, Permission.Read);
            AddAuthorization(connectionController, userRole, DataScope.User, Permission.Write);
            AddAuthorization(connectionController, userRole, DataScope.User, Permission.Delete);
            AddAuthorization(connectionController, userRole, DataScope.Unit, Permission.Create);
            AddAuthorization(connectionController, userRole, DataScope.Unit, Permission.Read);
            AddAuthorization(connectionController, userRole, DataScope.Unit, Permission.Write);
            AddAuthorization(connectionController, userRole, DataScope.Unit, Permission.Delete);
            AddLowAuthorizations(connectionController, userRole);
        }

        private static void AddHighAuthorizations(
            ConnectionControllerMock connectionController,
            UserRoleReadableModel userRole)
        {
            AddAuthorization(connectionController, userRole, DataScope.MediaConfiguration, Permission.Create);
            AddAuthorization(connectionController, userRole, DataScope.MediaConfiguration, Permission.Read);
            AddAuthorization(connectionController, userRole, DataScope.MediaConfiguration, Permission.Write);
            AddAuthorization(connectionController, userRole, DataScope.MediaConfiguration, Permission.Delete);
            AddAuthorization(connectionController, userRole, DataScope.CenterMedia, Permission.Interact);
            AddMiddleAuthorizations(connectionController, userRole);
        }

        private static void AddAuthorization(
            ConnectionControllerMock connectionController,
            UserRoleReadableModel userRole,
            DataScope dataScope,
            Permission permission)
        {
            var authorization = connectionController.AuthorizationChangeTrackingManager.Create();
            authorization.UserRole = userRole;
            authorization.DataScope = dataScope;
            authorization.Permission = permission;
            authorization.Commit();
        }
    }
}
