// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for the UnitConfiguration specific Pre and Post implementations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Configuration.HardwareDescription;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the UnitConfiguration specific Pre and Post implementations
    /// </summary>
    [TestClass]
    public class UnitConfigurationDataControllerTest
    {
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
        /// Tests the creation and preparation of an editable unit configuration entity.
        /// </summary>
        [TestMethod]
        public void EditUnitConfigurationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var tenant2 = new Tenant { Id = 2, Name = "SecondTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.UnitConfigurationManagerMock.Setup(manager => manager.Create())
            .Returns(new UnitConfigurationWritableModel());
            var documentReadable =
                new Helpers.DocumentReadableModelMock(new Document { Id = 10, Name = "Document", Tenant = tenant });
            var documentReadable2 =
                new Helpers.DocumentReadableModelMock(new Document { Id = 20, Name = "Document2", Tenant = tenant2 });
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<DocumentQuery>()))
                .Returns(
                    Task.FromResult(
                        new List<DocumentReadableModel> { documentReadable2, documentReadable }.AsEnumerable()));
            var productTypeReadable =
                new Helpers.ProductTypeReadableModelMock(new ProductType { Id = 100, Name = "ProductType" });
            var productTypeReadable2 =
                new Helpers.ProductTypeReadableModelMock(new ProductType { Id = 110, Name = "ProductType" });
            connectionControllerMockSetup.ProductTypeManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<ProductTypeQuery>()))
                .Returns(
                    Task.FromResult(
                        new List<ProductTypeReadableModel>
                            {
                                productTypeReadable2, productTypeReadable
                            }.AsEnumerable()));

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            UnitConfigurationReadableModel unitConfiguration =
                new Helpers.UnitConfigurationReadableModelMock(
                    new UnitConfiguration
                        {
                            Id = 10,
                            Document =
                                new Document
                                    {
                                        Id = 10,
                                        Tenant = tenant,
                                        Name = "Document",
                                        Description = "Description"
                                    },
                            ProductType = new ProductType { Id = 100, Name = "ProductType" },
                        });
            var unitConfigurationEntity = dataController.UnitConfiguration.Factory.CreateReadOnly(unitConfiguration);
            var entity = dataController.UnitConfiguration.EditEntityAsync(unitConfigurationEntity).Result;
            Assert.IsNotNull(entity);
            var unitConfigurationDataViewModel = entity as UnitConfigurationDataViewModel;
            Assert.IsNotNull(unitConfigurationDataViewModel);
            Assert.IsFalse(unitConfigurationDataViewModel.IsLoading);
            Assert.IsNotNull(unitConfigurationDataViewModel.Document);
            Assert.IsNotNull(unitConfigurationDataViewModel.ProductType);
            Assert.IsTrue(unitConfigurationDataViewModel.IsReadOnlyProductType);
            Assert.AreEqual(unitConfigurationDataViewModel.Name, unitConfiguration.Document.Name);
            Assert.AreEqual(unitConfigurationDataViewModel.Description, unitConfiguration.Document.Description);
            Assert.AreEqual(2, unitConfigurationDataViewModel.ProductType.Entities.Count);
        }

        /// <summary>
        /// Tests the saving of a unit configuration.
        /// </summary>
        [TestMethod]
        public void SaveUnitConfigurationWithUpdatedDocumentTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.UnitConfigurationManagerMock.Setup(
               manager => manager.CommitAndVerifyAsync(It.IsAny<UnitConfigurationWritableModel>()))
               .Returns((UnitConfigurationWritableModel u) => Task.FromResult(UpdateReadableModel(u)));
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentWritableModel>()))
                .Returns(
                    (DocumentWritableModel d) =>
                        {
                            var doc = d.ToDto();
                            doc.Id++;
                            return Task.FromResult((DocumentReadableModel)new Helpers.DocumentReadableModelMock(doc));
                        });
            var unitConfiguration = CreateUnitConfigurationMock(tenant);
            var unitConfigurationWritable = unitConfiguration.ToChangeTrackingModel();
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);

            var writable = dataController.Factory.Create(unitConfigurationWritable);
            writable.Name = "UpdatedDocument";
            writable.Description = "NewDescription";
            var savedModel = dataController.UnitConfiguration.SaveEntityAsync(writable).Result;
            Assert.AreEqual("UpdatedDocument", savedModel.DisplayText);
            var savedUnitConfiguration = savedModel as UnitConfigurationReadOnlyDataViewModel;
            Assert.IsNotNull(savedUnitConfiguration);
            Assert.AreEqual(writable.Description, savedUnitConfiguration.Document.Description);
            connectionControllerMockSetup.UnitConfigurationManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(unitConfigurationWritable), Times.Once());
            connectionControllerMockSetup.DocumentManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentWritableModel>()), Times.Once());
        }

        /// <summary>
        /// Tests saving a new unit configuration which doesn't have a document yet.
        /// </summary>
        [TestMethod]
        public void SaveNewUnitConfigurationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var state = Helpers.CreateApplicationStateMock(container, tenant);
            state.Setup(s => s.CurrentUser).Returns(new User { Id = 1000, FirstName = "User" });
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(manager => manager.Create())
                .Returns(new DocumentWritableModel());
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentWritableModel>()))
                .Returns(
                    (DocumentWritableModel d) =>
                        {
                            var doc = d.ToDto();
                            doc.Id = 300;
                            return Task.FromResult((DocumentReadableModel)new Helpers.DocumentReadableModelMock(doc));
                        });
            connectionControllerMockSetup.DocumentVersionManagerMock.Setup(manager => manager.Create())
                .Returns(new DocumentVersionWritableModel());
            connectionControllerMockSetup.DocumentVersionManagerMock.Setup(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentVersionWritableModel>()))
                .Returns(
                    (DocumentVersionWritableModel version) =>
                    {
                        var dto = version.ToDto();
                        dto.Id = 350;
                        return
                            Task.FromResult(
                                (DocumentVersionReadableModel)new Helpers.DocumentVersionReadableModelMock(dto));
                    });
            connectionControllerMockSetup.UserManagerMock.Setup(manager => manager.Wrap(It.IsAny<User>()))
                .Returns(new Helpers.UserReadableModelMock(new User { Id = 1000, FirstName = "User" }));
            connectionControllerMockSetup.UnitConfigurationManagerMock.Setup(
              manager => manager.CommitAndVerifyAsync(It.IsAny<UnitConfigurationWritableModel>()))
              .Returns((UnitConfigurationWritableModel u) => Task.FromResult(UpdateReadableModel(u)));
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var productType =
                new Helpers.ProductTypeReadableModelMock(new ProductType { Id = 100, Name = "ProductType" });
            var unitConfigurationWritable = new UnitConfigurationWritableModel { ProductType = productType };

            var writable = dataController.Factory.Create(unitConfigurationWritable);
            writable.Name = "NewDocument";
            writable.Description = "NewDescription";
            var savedModel = dataController.UnitConfiguration.SaveEntityAsync(writable).Result;

            Assert.IsNotNull(savedModel);
            Assert.AreEqual("NewDocument", savedModel.DisplayText);
            var savedUnitConfiguration = savedModel as UnitConfigurationReadOnlyDataViewModel;
            Assert.IsNotNull(savedUnitConfiguration);
            Assert.AreEqual(writable.Description, savedUnitConfiguration.Document.Description);

            connectionControllerMockSetup.UnitConfigurationManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(unitConfigurationWritable),
                Times.Once());
            connectionControllerMockSetup.DocumentManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentWritableModel>()),
                Times.Once());
            connectionControllerMockSetup.DocumentVersionManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentVersionWritableModel>()),
                Times.Once());
            connectionControllerMockSetup.UserManagerMock.Verify(
                manager => manager.Wrap(It.IsAny<User>()),
                Times.Once());
        }

        /// <summary>
        /// Tests deleting a new unit configuration with all related entities (and removing references).
        /// </summary>
        [TestMethod]
        public void DeleteUnitConfigurationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant" });
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var connectionController = new ConnectionControllerMock();
            connectionController.Configure();
            dataController.Initialize(connectionController);

            var user = connectionController.UserChangeTrackingManager.Create();
            var userReadable = connectionController.UserChangeTrackingManager.CommitAndVerifyAsync(user).Result;

            var tenant = connectionController.TenantChangeTrackingManager.Create();
            tenant.Name = "TestTenant";
            var tenantReadable = connectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(tenant).Result;

            var productType = connectionController.ProductTypeChangeTrackingManager.Create();
            productType.HardwareDescriptor = new XmlData(HardwareDescriptors.InfoVision.TopboxMini);
            var productTypeReadable =
                connectionController.ProductTypeChangeTrackingManager.CommitAndVerifyAsync(productType).Result;

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
            version1.Commit();

            var version2 = connectionController.DocumentVersionChangeTrackingManager.Create();
            version2.Major = 1;
            version2.Minor = 0;
            version2.Content = new XmlData(new UnitConfigData());
            version2.CreatingUser = userReadable;
            version2.Document = documentReadable;
            version2.Commit();

            var unitConfig = connectionController.UnitConfigurationChangeTrackingManager.Create();
            unitConfig.Document = documentReadable;
            unitConfig.ProductType = productTypeReadable;
            var unitConfigReadable =
                connectionController.UnitConfigurationChangeTrackingManager.CommitAndVerifyAsync(unitConfig).Result;

            var unitConfig2 = connectionController.UnitConfigurationChangeTrackingManager.Create();
            unitConfig2.ProductType = productTypeReadable;
            var unitConfig2Readable =
                connectionController.UnitConfigurationChangeTrackingManager.CommitAndVerifyAsync(unitConfig2).Result;

            var updateGroup = connectionController.UpdateGroupChangeTrackingManager.Create();
            updateGroup.Name = "MyGroup";
            updateGroup.UnitConfiguration = unitConfigReadable;
            var updateGroupReadable =
                connectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(updateGroup).Result;

            var updateGroup2 = connectionController.UpdateGroupChangeTrackingManager.Create();
            updateGroup2.Name = "OtherGroup";
            updateGroup2.UnitConfiguration = unitConfig2Readable;
            var updateGroup2Readable =
                connectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(updateGroup2).Result;

            // ACT
            var dvm = dataController.Factory.CreateReadOnly(unitConfigReadable);
            dataController.UnitConfiguration.DeleteEntityAsync(dvm).Wait();

            // ASSERT
            updateGroupReadable.LoadNavigationPropertiesAsync().Wait();
            Assert.IsNull(updateGroupReadable.UnitConfiguration);

            updateGroup2Readable.LoadNavigationPropertiesAsync().Wait();
            Assert.IsNotNull(updateGroup2Readable.UnitConfiguration);

            documentReadable.LoadNavigationPropertiesAsync().Wait();
            Assert.AreEqual(0, documentReadable.Versions.Count);
        }

        private static UnitConfigurationReadableModel CreateUnitConfigurationMock(Tenant tenant)
        {
            UnitConfigurationReadableModel unitConfiguration =
                new Helpers.UnitConfigurationReadableModelMock(
                    new UnitConfiguration
                        {
                            Id = 10,
                            Document =
                                new Document
                                    {
                                        Id = 10,
                                        Tenant = tenant,
                                        Name = "Document",
                                        Description = "Description"
                                    },
                            ProductType = new ProductType { Id = 100, Name = "ProductType" },
                        });
            return unitConfiguration;
        }

        private static UnitConfigurationReadableModel UpdateReadableModel(
            UnitConfigurationWritableModel unitConfigurationWritableModel)
        {
            var configuration = new UnitConfiguration
                                    {
                                        Id = unitConfigurationWritableModel.Id,
                                        Document = unitConfigurationWritableModel.Document.ToDto(),
                                        ProductType = unitConfigurationWritableModel.ProductType.ToDto(),
                                    };
            return new Helpers.UnitConfigurationReadableModelMock(configuration);
        }
    }
}
