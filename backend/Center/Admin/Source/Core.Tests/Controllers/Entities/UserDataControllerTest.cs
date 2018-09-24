// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDataControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the User specific Pre and Post implementations
    /// </summary>
    [TestClass]
    public class UserDataControllerTest
    {
        /// <summary>
        /// Tests deleting a user with all related entities (and removing references).
        /// </summary>
        [TestMethod]
        public void DeleteUserTest()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant" });
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var connectionController = new ConnectionControllerMock();
            connectionController.Configure();
            dataController.Initialize(connectionController);

            var tenant = connectionController.TenantChangeTrackingManager.Create();
            tenant.Name = "TestTenant";
            var tenantReadable = connectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(tenant).Result;

            var user = connectionController.UserChangeTrackingManager.Create();
            user.OwnerTenant = tenantReadable;
            var userReadable = connectionController.UserChangeTrackingManager.CommitAndVerifyAsync(user).Result;

            var document = connectionController.DocumentChangeTrackingManager.Create();
            document.Name = "MyConfig";
            document.Tenant = tenantReadable;
            var documentReadable =
                connectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(document).Result;

            var version1 = connectionController.DocumentVersionChangeTrackingManager.Create();
            version1.Major = 0;
            version1.Minor = 1;
            version1.Content = new XmlData(new UnitConfigData());
            version1.CreatingUser = userReadable;
            version1.Document = documentReadable;
            var version1Readable =
                connectionController.DocumentVersionChangeTrackingManager.CommitAndVerifyAsync(version1).Result;

            var version2 = connectionController.DocumentVersionChangeTrackingManager.Create();
            version2.Major = 1;
            version2.Minor = 0;
            version2.Content = new XmlData(new UnitConfigData());
            version2.CreatingUser = userReadable;
            version2.Document = documentReadable;
            var version2Readable =
                connectionController.DocumentVersionChangeTrackingManager.CommitAndVerifyAsync(version2).Result;

            var userRole = connectionController.UserRoleChangeTrackingManager.Create();
            userRole.Name = "MyRole";
            var userRoleReadable =
                connectionController.UserRoleChangeTrackingManager.CommitAndVerifyAsync(userRole).Result;

            var association = connectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
            association.User = userReadable;
            association.Tenant = tenantReadable;
            association.UserRole = userRoleReadable;
            association.Commit();

            var resource = connectionController.ResourceChangeTrackingManager.Create();
            resource.Hash = "5d41402abc4b2a76b9719d911017c592";
            resource.UploadingUser = userReadable;
            var resourceReadable =
                connectionController.ResourceChangeTrackingManager.CommitAndVerifyAsync(resource).Result;

            // ACT
            var dvm = dataController.Factory.CreateReadOnly(userReadable);
            dataController.User.DeleteEntityAsync(dvm).Wait();

            // ASSERT
            var associations =
                connectionController.AssociationTenantUserUserRoleChangeTrackingManager.QueryAsync().Result;
            Assert.AreEqual(0, associations.Count());

            version1Readable.LoadReferencePropertiesAsync().Wait();
            Assert.IsNull(version1Readable.CreatingUser);

            version2Readable.LoadReferencePropertiesAsync().Wait();
            Assert.IsNull(version2Readable.CreatingUser);

            resourceReadable.LoadNavigationPropertiesAsync().Wait();
            Assert.IsNull(resourceReadable.UploadingUser);
        }
    }
}
