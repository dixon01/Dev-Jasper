// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for stage controllers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers;
    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.Entities.AccessControl;
    using Gorba.Center.Admin.Core.Controllers.Entities.Configurations;
    using Gorba.Center.Admin.Core.Controllers.Entities.Documents;
    using Gorba.Center.Admin.Core.Controllers.Entities.Membership;
    using Gorba.Center.Admin.Core.Controllers.Entities.Resources;
    using Gorba.Center.Admin.Core.Controllers.Entities.Software;
    using Gorba.Center.Admin.Core.Controllers.Entities.Units;
    using Gorba.Center.Admin.Core.Controllers.Entities.Update;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for stage controllers.
    /// </summary>
    [TestClass]
    public class StageControllerTest
    {
        private ObservableCollection<TenantReadableModel> authorizedTenants;
        private Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>> authorizations;

        private TenantReadableModel tenantA;
        private TenantReadableModel tenantB;

        private AdminApplicationState appStateMock;

        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The test if users are loaded by LoadData()
        /// </summary>
        [TestMethod]
        public void LoadUsersTest()
        {
            this.SetupAdminApplicationStateWithUserMock();
            var container = Helpers.InitializeServiceLocator();
            container.RegisterInstance<IAdminApplicationState>(this.appStateMock);
            container.RegisterInstance<IConnectedApplicationState>(this.appStateMock);
            var applicatinControllerMock = new Mock<IAdminApplicationController>();
            applicatinControllerMock.Setup(controller => controller.PermissionController)
                .Returns(new GrantingPermissionController());
            container.RegisterInstance(typeof(IAdminApplicationController), applicatinControllerMock.Object);
            var userList = this.CreateUserReadableModelList();

            var queryResultMock = Task.FromResult(userList);

            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.UserManagerMock.Setup(u => u.QueryAsync(It.IsAny<UserQuery>()))
                .Returns(queryResultMock);
            connectionControllerMockSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));

            var commandRegistry = new CommandRegistry();
            var dataViewModelFactory = new DataViewModelFactory(commandRegistry);
            var dataController = new DataController(dataViewModelFactory);
            dataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var userDataController = new UserDataController(dataController);
            userDataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var stageController = new UserStageController(userDataController);
            stageController.Initialize();
            stageController.LoadData();

            Assert.IsNotNull(stageController.StageViewModel.Instances);
            Assert.AreEqual(stageController.StageViewModel.Instances.Count, 2);

            var resultUser = stageController.StageViewModel.Instances[0] as UserReadOnlyDataViewModel;
            Assert.IsNotNull(resultUser);
            Assert.AreEqual(resultUser.Id, 4);
            Assert.AreEqual(resultUser.FirstName, "John");
            Assert.AreEqual(resultUser.LastName, "Doe");

            resultUser = stageController.StageViewModel.Instances[1] as UserReadOnlyDataViewModel;
            Assert.IsNotNull(resultUser);
            Assert.AreEqual(resultUser.Id, 7);
            Assert.AreEqual(resultUser.FirstName, "Lisa");
            Assert.AreEqual(resultUser.LastName, "Mueller");
        }

        /// <summary>
        /// The update property display test.
        /// </summary>
        [TestMethod]
        public void UpdatePropertyDisplayTest()
        {
            this.SetupAdminApplicationStateWithUserMock();
            var container = Helpers.InitializeServiceLocator();
            container.RegisterInstance<IAdminApplicationState>(this.appStateMock);
            container.RegisterInstance<IConnectedApplicationState>(this.appStateMock);
            var applicatinControllerMock = new Mock<IAdminApplicationController>();
            applicatinControllerMock.Setup(controller => controller.PermissionController)
                .Returns(new GrantingPermissionController());
            container.RegisterInstance(typeof(IAdminApplicationController), applicatinControllerMock.Object);
            var userList = this.CreateUserReadableModelList();
            var queryResultMock = Task.FromResult(userList);

            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.UserManagerMock.Setup(u => u.QueryAsync(null)).Returns(queryResultMock);
            connectionControllerMockSetup.UserManagerMock.Setup(u => u.QueryAsync(It.IsAny<UserQuery>()))
                .Returns(queryResultMock);
            connectionControllerMockSetup.TenantManagerMock.Setup(
               manager => manager.QueryAsync(It.IsAny<TenantQuery>()))
               .Returns(Task.FromResult((new List<TenantReadableModel> { this.tenantA }).AsEnumerable()));
            connectionControllerMockSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));
            var commandRegistry = new CommandRegistry();
            var dataViewModelFactory = new DataViewModelFactory(commandRegistry);
            var dataController = new DataController(dataViewModelFactory);
            dataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var userDataController = new UserDataController(dataController);
            userDataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var stageController = new UserStageController(userDataController);
            stageController.Initialize();
            stageController.LoadData();

            userDataController.AwaitAllDataAsync().Wait();

            var selectedUser = stageController.StageViewModel.Instances[0] as UserReadOnlyDataViewModel;
            var editedUser = (UserDataViewModel)stageController.DataController.EditEntityAsync(selectedUser).Result;
            var parameters = new PropertyDisplayParameters(editedUser, "LastName");
            stageController.UpdatePropertyDisplay(parameters);
            Assert.AreEqual(parameters.OrderIndex, 1);
        }

        /// <summary>
        /// Tests the part of the can execute logic which is not UI dependant
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. This is a long test method.")]
        [TestMethod]
        public void TestAllPermissions()
        {
            var allPermissionList = new List<Permission>
                                    {
                                        Permission.Create,
                                        Permission.Read,
                                        Permission.Write,
                                        Permission.Delete
                                    };

            this.TestCombinationsScopeAndPermissions(
                a => new UserRoleDataController(a),
                a => new UserRoleStageController(a),
                new List<DataScope> { DataScope.AccessControl },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new AuthorizationDataController(a),
                a => new AuthorizationStageController(a),
                new List<DataScope> { DataScope.AccessControl },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new TenantDataController(a),
                a => new TenantStageController(a),
                new List<DataScope> { DataScope.Tenant },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UserDataController(a),
                a => new UserStageController(a),
                new List<DataScope> { DataScope.User },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new AssociationTenantUserUserRoleDataController(a),
                a => new AssociationTenantUserUserRoleStageController(a),
                new List<DataScope> { DataScope.User },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new ProductTypeDataController(a),
                a => new ProductTypeStageController(a),
                new List<DataScope> { DataScope.ProductType },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UnitDataController(a),
                a => new UnitStageController(a),
                new List<DataScope> { DataScope.Unit },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new ResourceDataController(a),
                a => new ResourceStageController(a),
                new List<DataScope> { DataScope.Resource },
                new List<Permission> { Permission.Delete, Permission.Read });

            this.TestCombinationsScopeAndPermissions(
                a => new UpdateGroupDataController(a),
                a => new UpdateGroupStageController(a),
                new List<DataScope> { DataScope.Update },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UpdatePartDataController(a),
                a => new UpdatePartStageController(a),
                new List<DataScope> { DataScope.Update },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UpdateCommandDataController(a),
                a => new UpdateCommandStageController(a),
                new List<DataScope> { DataScope.Update },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UpdateFeedbackDataController(a),
                a => new UpdateFeedbackStageController(a),
                new List<DataScope> { DataScope.Update },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new DocumentDataController(a),
                a => new DocumentStageController(a),
                new List<DataScope> { DataScope.UnitConfiguration, DataScope.MediaConfiguration },
                new List<Permission> { Permission.Write, Permission.Delete, Permission.Read });

            this.TestCombinationsScopeAndPermissions(
                a => new DocumentVersionDataController(a),
                a => new DocumentVersionStageController(a),
                new List<DataScope> { DataScope.UnitConfiguration, DataScope.MediaConfiguration },
                new List<Permission> { Permission.Write, Permission.Delete, Permission.Read });

            this.TestCombinationsScopeAndPermissions(
                a => new PackageDataController(a),
                a => new PackageStageController(a),
                new List<DataScope> { DataScope.Software },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new PackageVersionDataController(a),
                a => new PackageVersionStageController(a),
                new List<DataScope> { DataScope.Software },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new UnitConfigurationDataController(a),
                a => new UnitConfigurationStageController(a),
                new List<DataScope> { DataScope.UnitConfiguration },
                allPermissionList);

            this.TestCombinationsScopeAndPermissions(
                a => new MediaConfigurationDataController(a),
                a => new MediaConfigurationStageController(a),
                new List<DataScope> { DataScope.MediaConfiguration },
                new List<Permission> { Permission.Write, Permission.Read });
        }

        private void TestCombinationsScopeAndPermissions<TDataController>(
            Func<DataController, TDataController> ctorTDataController,
            Func<TDataController, EntityStageControllerBase> ctorTStageController,
            List<DataScope> scopeList,
            List<Permission> permissionList)
            where TDataController : DataControllerBase
        {
            var emptyPermissionList = new List<Permission>();
            var emptyScopeList = new List<DataScope>();

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                scopeList,
                permissionList,
                true);

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                scopeList,
                emptyPermissionList,
                false);

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                emptyScopeList,
                permissionList,
                false);

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                emptyScopeList,
                emptyPermissionList,
                false);

            // test wrong scope
            var wrongScope = DataScope.AccessControl;
            var found = false;
            foreach (DataScope scope in Enum.GetValues(typeof(DataScope)))
            {
                if (!scopeList.Contains(scope))
                {
                    wrongScope = scope;
                    found = true;
                }
            }

            Assert.IsTrue(found);

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                new List<DataScope> { wrongScope },
                emptyPermissionList,
                false);

            // test wrong permission
            var wrongPermission = Permission.Abort;
            found = false;
            foreach (Permission permission in Enum.GetValues(typeof(Permission)))
            {
                if (!permissionList.Contains(permission))
                {
                    wrongPermission = permission;
                    found = true;
                }
            }

            Assert.IsTrue(found);

            this.CanExecuteTest(
                ctorTDataController,
                ctorTStageController,
                scopeList,
                new List<Permission> { wrongPermission },
                false);
        }

        private void SetupAdminApplicationStateWithUserMock()
        {
            var allPermissionList = new List<Permission>
                                    {
                                        Permission.Create,
                                        Permission.Read,
                                        Permission.Write,
                                        Permission.Delete
                                    };
            this.SetupAdminApplicationStateMock(new List<DataScope> { DataScope.User }, allPermissionList);
        }

        // shouldBe: if true checks if all given permissions are true
        private void CanExecuteTest<TDataController>(
            Func<DataController, TDataController> ctorTDataController,
            Func<TDataController, EntityStageControllerBase> ctorTStageController,
            IEnumerable<DataScope> scopeList,
            List<Permission> permissionList,
            bool shouldBe)
            where TDataController : DataControllerBase
        {
            this.SetupAdminApplicationStateMock(scopeList, permissionList);
            var container = Helpers.InitializeServiceLocator();
            container.RegisterInstance<IAdminApplicationState>(this.appStateMock);
            container.RegisterInstance<IConnectedApplicationState>(this.appStateMock);

            var permissionControllerMock = new Mock<IPermissionController>(MockBehavior.Strict);
            permissionControllerMock.Setup(c => c.HasPermission(It.IsAny<Permission>(), It.IsAny<DataScope>()))
                .Returns<Permission, DataScope>(
                    (p, d) =>
                    this.authorizations[this.appStateMock.CurrentTenant].Any(
                        a => a.Permission == p && a.DataScope == d));

            var applicationControllerMock = new Mock<IAdminApplicationController>();
            applicationControllerMock.Setup(controller => controller.PermissionController)
                .Returns(permissionControllerMock.Object);
            container.RegisterInstance(typeof(IAdminApplicationController), applicationControllerMock.Object);

            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();

            var commandRegistry = new CommandRegistry();
            var dataViewModelFactory = new DataViewModelFactory(commandRegistry);
            var dataController = new DataController(dataViewModelFactory);
            dataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var userDataController = ctorTDataController(dataController);
            userDataController.Initialize(connectionControllerMockSetup.ConnectionControllerMock.Object);

            var stageController = ctorTStageController(userDataController);
            stageController.Initialize();

            Assert.AreEqual(
                stageController.StageViewModel.CanCreate,
                shouldBe && permissionList.Contains(Permission.Create));
            Assert.AreEqual(
                stageController.StageViewModel.CanRead,
                shouldBe && permissionList.Contains(Permission.Read));
            Assert.AreEqual(
                stageController.StageViewModel.CanWrite,
                shouldBe && permissionList.Contains(Permission.Write));
            Assert.AreEqual(
                stageController.StageViewModel.CanDelete,
                shouldBe && permissionList.Contains(Permission.Delete));
        }

        private IEnumerable<UserReadableModel> CreateUserReadableModelList()
        {
            var userA = new User { Id = 4, FirstName = "John", LastName = "Doe", Username = "John" };
            var userB = new User { Id = 7, FirstName = "Lisa", LastName = "Mueller", Username = "Lisa" };
            var userReadableModelA = new UserReadableModelTest(userA);
            var userReadableModelB = new UserReadableModelTest(userB);
            var userList = new List<UserReadableModel> { userReadableModelA, userReadableModelB };
            return userList;
        }

        private void SetupAdminApplicationStateMock(IEnumerable<DataScope> scopeList, List<Permission> permissionList)
        {
            this.authorizedTenants = this.CreateTenants();
            this.tenantA = this.authorizedTenants[0]; // has permissions
            this.tenantB = this.authorizedTenants[1];

            this.CreateAuthorizations(scopeList, permissionList);

            this.appStateMock = new AdminApplicationState
                {
                    CurrentTenant = this.tenantA
                };
            foreach (var tenant in this.authorizedTenants)
            {
                this.appStateMock.AuthorizedTenants.Add(tenant);
            }
        }

        private ObservableCollection<TenantReadableModel> CreateTenants()
        {
            var tenants = new ObservableCollection<TenantReadableModel>
                          {
                              new Helpers.TenantReadableModelMock(new Tenant { Id = 1, Name = "Tenant A" }),
                              new Helpers.TenantReadableModelMock(new Tenant { Id = 2, Name = "Tenant B" }),
                          };
            return tenants;
        }

        private void CreateAuthorizations(IEnumerable<DataScope> scopeList, List<Permission> permissionList)
        {
            var authorizationListA = new List<AuthorizationReadableModel>();

            foreach (var scope in scopeList)
            {
                authorizationListA.AddRange(
                    permissionList.Select(
                        permission =>
                        new AuthorizationReadableModelMock(
                            new Authorization
                                {
                                    DataScope = scope, Permission = permission
                                })));
            }

            this.authorizations = new Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>>
                                  {
                                      { this.authorizedTenants[0], authorizationListA }
                                  };
        }

        private class AuthorizationReadableModelMock : AuthorizationReadableModel
        {
            public AuthorizationReadableModelMock(Authorization entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private class UserReadableModelTest : UserReadableModel
        {
            public UserReadableModelTest(User entity)
                : base(entity)
            {
                this.Populate();
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }

        private class GrantingPermissionController : IPermissionController
        {
            public bool HasPermission(Permission desiredPermission, DataScope scope)
            {
                return true;
            }

            public bool HasPermission(TenantReadableModel tenant, Permission desiredPermission, DataScope scope)
            {
                return true;
            }

            public bool PermissionTrap(Permission desiredPermission, DataScope scope)
            {
                return true;
            }

            public Task LoadPermissionsAsync(
                User user, DataScope applicationDataScope, IList<DataScope> allowedDataScopes)
            {
                return Task.FromResult(0);
            }
        }
    }
}
