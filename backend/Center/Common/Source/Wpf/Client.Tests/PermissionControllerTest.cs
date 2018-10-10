// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests related to user premissions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Converters;
    using Gorba.Center.Common.Wpf.Core;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The permission controller test.
    /// </summary>
    [TestClass]
    public class PermissionControllerTest
    {
        private TenantReadableModel tenantA;

        private TenantReadableModel tenantB;

        private ObservableCollection<TenantReadableModel> authorizedTenants;

        private List<AssociationTenantUserUserRole> authorizations;

        private ConnectedApplicationState appStateMock;

        /// <summary>
        /// If current tenant is null no permission check should be true.
        /// </summary>
        [TestMethod]
        public void TenantNull()
        {
            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
            unityContainer.RegisterInstance<IConnectedApplicationState>(new ConnectedApplicationState());
            var permissionController = new PermissionController(null);

            var result = permissionController.HasPermission(Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(null, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The scopes null.
        /// </summary>
        [TestMethod]
        public void ScopesNullPermissionOk()
        {
            var permissionController = new PermissionController(this.SetupConnectionControllerMock());
            permissionController.LoadPermissionsAsync(
                new User(),
                DataScope.CenterMedia,
                new[] { DataScope.MediaConfiguration }).ConfigureAwait(false).GetAwaiter().GetResult();

            var result = permissionController.HasPermission(Permission.Create, DataScope.MediaConfiguration);
            Assert.IsTrue(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsTrue(result);

            result = permissionController.HasPermission(null, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantB, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The scopes null permission missing.
        /// </summary>
        [TestMethod]
        public void ScopesNullPermissionMissing()
        {
            var permissionController = new PermissionController(this.SetupConnectionControllerMock());
            permissionController.LoadPermissionsAsync(
                new User(),
                DataScope.CenterMedia,
                new[] { DataScope.MediaConfiguration }).ConfigureAwait(false).GetAwaiter().GetResult();

            var result = permissionController.HasPermission(Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(null, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantB, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The permission ok.
        /// </summary>
        [TestMethod]
        public void PermissionOk()
        {
            var permissionController = new PermissionController(this.SetupConnectionControllerMock());
            permissionController.LoadPermissionsAsync(
                new User(),
                DataScope.CenterMedia,
                new[] { DataScope.MediaConfiguration }).ConfigureAwait(false).GetAwaiter().GetResult();

            var result = permissionController.HasPermission(Permission.Create, DataScope.MediaConfiguration);
            Assert.IsTrue(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsTrue(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Create, DataScope.Unit);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(null, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantB, Permission.Create, DataScope.MediaConfiguration);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The permission missing.
        /// </summary>
        [TestMethod]
        public void PermissionMissing()
        {
            var permissionController = new PermissionController(this.SetupConnectionControllerMock());
            permissionController.LoadPermissionsAsync(
                new User(),
                DataScope.CenterMedia,
                new[] { DataScope.MediaConfiguration }).ConfigureAwait(false).GetAwaiter().GetResult();

            var result = permissionController.HasPermission(Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantA, Permission.Delete, DataScope.Unit);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(null, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);

            result = permissionController.HasPermission(this.tenantB, Permission.Delete, DataScope.MediaConfiguration);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The permission converter.
        /// </summary>
        [TestMethod]
        public void PermissionConverter()
        {
            var permissionController = new PermissionController(this.SetupConnectionControllerMock());
            permissionController.LoadPermissionsAsync(
                new User(),
                DataScope.CenterMedia,
                new[] { DataScope.MediaConfiguration }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (bool)new UserHasCreatePermissionConverter().Convert(
                        permissionController,
                        null,
                        DataScope.MediaConfiguration,
                        null);
            Assert.IsTrue(result);

            result = (bool)new UserHasDeletePermissionConverter().Convert(
                        permissionController,
                        null,
                        DataScope.MediaConfiguration,
                        null);
            Assert.IsFalse(result);

            result = (bool)new UserHasReadPermissionConverter().Convert(
                        permissionController,
                        null,
                        DataScope.MediaConfiguration,
                        null);
            Assert.IsFalse(result);

            result = (bool)new UserHasWritePermissionConverter().Convert(
                        permissionController,
                        null,
                        DataScope.MediaConfiguration,
                        null);
            Assert.IsFalse(result);
        }

        private IConnectionController SetupConnectionControllerMock()
        {
            this.authorizedTenants = this.CreateTenants();
            this.tenantA = this.authorizedTenants[0]; // has create permissions
            this.tenantB = this.authorizedTenants[1];

            this.CreateAuthorizations();

            this.appStateMock = new ConnectedApplicationState
                                {
                                    CurrentTenant = this.tenantA
                                };
            foreach (var tenant in this.authorizedTenants)
            {
                this.appStateMock.AuthorizedTenants.Add(tenant);
            }

            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
            unityContainer.RegisterInstance<IConnectedApplicationState>(this.appStateMock);
            unityContainer.RegisterInstance(Mock.Of<IDispatcher>());

            var connectionControllerMock = new Mock<IConnectionController>(MockBehavior.Strict);

            var associationMock = new Mock<IAssociationTenantUserUserRoleChangeTrackingManager>(MockBehavior.Strict);
            associationMock.Setup(a => a.QueryAsync(It.IsAny<AssociationTenantUserUserRoleQuery>()))
                .Returns(
                    Task.FromResult(
                        this.authorizations.Select(
                            a =>
                            (AssociationTenantUserUserRoleReadableModel)
                            new AssociationTenantUserUserRoleReadableModelMock(a))));
            connectionControllerMock.SetupGet(c => c.AssociationTenantUserUserRoleChangeTrackingManager)
                .Returns(associationMock.Object);

            var userMock = new Mock<IUserChangeTrackingManager>(MockBehavior.Strict);
            userMock.Setup(u => u.Wrap(It.IsAny<User>())).Returns<User>(u => new UserReadableModelMock(u));
            connectionControllerMock.SetupGet(c => c.UserChangeTrackingManager)
                .Returns(userMock.Object);

            var tenantMock = new Mock<ITenantChangeTrackingManager>(MockBehavior.Strict);
            connectionControllerMock.SetupGet(c => c.TenantChangeTrackingManager)
                .Returns(tenantMock.Object);

            return connectionControllerMock.Object;
        }

        private void CreateAuthorizations()
        {
            var authorizationListA = new List<AuthorizationReadableModel>
                                     {
                                         new AuthorizationReadableModelMock(
                                             new Authorization
                                             {
                                                 DataScope  = DataScope.MediaConfiguration,
                                                 Permission = Permission.Create
                                             })
                                     };

            this.authorizations = new List<AssociationTenantUserUserRole>
                {
                    new AssociationTenantUserUserRole
                        {
                            Tenant = this.authorizedTenants[0].ToDto(),
                            UserRole =
                                new UserRole
                                    {
                                        Authorizations =
                                            new List<Authorization>
                                                {
                                                    new Authorization
                                                        {
                                                            DataScope = DataScope.MediaConfiguration,
                                                            Permission = Permission.Create
                                                        }
                                                }
                                    }
                        }
                };
        }

        private ObservableCollection<TenantReadableModel> CreateTenants()
        {
            var tenants = new ObservableCollection<TenantReadableModel>
                          {
                              new TenantReadableModelMock(new Tenant { Id = 1, Name = "Tenant A" }),
                              new TenantReadableModelMock(new Tenant { Id = 2, Name = "Tenant B" }),
                          };

            return tenants;
        }

        private class AuthorizationReadableModelMock : AuthorizationReadableModel
        {
            public AuthorizationReadableModelMock(Authorization entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private class UserRoleReadableModelMock : UserRoleReadableModel
        {
            public UserRoleReadableModelMock(UserRole entity)
                : base(entity)
            {
                this.Populate();

                if (entity.Authorizations != null)
                {
                    foreach (var authorization in entity.Authorizations)
                    {
                        this.authorizations.Add(new AuthorizationReadableModelMock(authorization));
                    }
                }
            }
        }

        private sealed class AssociationTenantUserUserRoleReadableModelMock : AssociationTenantUserUserRoleReadableModel
        {
            public AssociationTenantUserUserRoleReadableModelMock(AssociationTenantUserUserRole entity)
                : base(entity)
            {
                this.Populate();

                if (entity.UserRole != null)
                {
                    this.UserRole = new UserRoleReadableModelMock(entity.UserRole);
                }

                if (entity.Tenant != null)
                {
                    this.Tenant = new TenantReadableModelMock(entity.Tenant);
                }
            }
        }

        private class TenantReadableModelMock : TenantReadableModel
        {
            public TenantReadableModelMock(Tenant entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private class UserReadableModelMock : UserReadableModel
        {
            public UserReadableModelMock(User entity)
                : base(entity)
            {
                this.Populate();
            }

            public override Task LoadNavigationPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }
}
