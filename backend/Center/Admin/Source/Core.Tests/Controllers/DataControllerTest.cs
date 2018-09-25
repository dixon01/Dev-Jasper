// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for the generated data controllers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Membership;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the generated data controllers.
    /// </summary>
    [TestClass]
    public class DataControllerTest
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
        /// The await all data test.
        /// </summary>
        [TestMethod]
        public void AwaitAllDataTest()
        {
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            var document1 = new Helpers.DocumentReadableModelMock(
                new Document { Id = 10, Name = "Document1", Tenant = tenant });
            var document2 = new Helpers.DocumentReadableModelMock(
                new Document { Id = 11, Name = "Document2", Tenant = tenant });
            var testDocuments = new List<DocumentReadableModel>
                                    {
                                       document1, document2
                                    };
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<DocumentQuery>()))
                .Returns(Task.FromResult(testDocuments.AsEnumerable()));
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var all = dataController.Document.All.ToList();
            connectionControllerMockSetup.DocumentManagerMock.Verify(
                manager =>
                manager.QueryAsync(
                    It.Is<DocumentQuery>(
                        query =>
                        query.Tenant != null && query.Tenant.Id != null && query.Tenant.Id.Value == tenant.Id
                        && query.Tenant.Id.Comparison == Int32Comparison.ExactMatch)),
                Times.Once(),
                "The expected document query filtering for the current Tenant was not executed");
            Assert.AreEqual(2, all.Count);
            var documentDataViewModel = all.First(doc => doc.Id == document1.Id);
            Assert.IsNotNull(documentDataViewModel);
            Assert.AreEqual(document1.Name, documentDataViewModel.Name);
            documentDataViewModel = all.First(doc => doc.Id == document2.Id);
            Assert.IsNotNull(documentDataViewModel);
            Assert.AreEqual(document2.Name, documentDataViewModel.Name);
        }

        /// <summary>
        /// The supports entity test.
        /// </summary>
        [TestMethod]
        public void SupportsEntityTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var tenantDataViewModel = dataController.Factory.CreateReadOnly(tenantReadable);
            Assert.IsTrue(dataController.Tenant.SupportsEntity(tenantDataViewModel));
            Assert.IsFalse(dataController.Document.SupportsEntity(tenantDataViewModel));
        }

        /// <summary>
        /// Tests getting an entity.
        /// </summary>
        [TestMethod]
        public void GetEntityTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var secondTenant = new Tenant { Id = 2, Name = "SecondTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);

            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            DocumentReadableModel document =
                new Helpers.DocumentReadableModelMock(new Document { Id = 10, Tenant = tenant, Name = "Document" });
            DocumentReadableModel documentWrongTenant =
                new Helpers.DocumentReadableModelMock(
                    new Document { Id = 20, Tenant = secondTenant, Name = "Document" });
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.GetAsync(10)).Returns(Task.FromResult(document));
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.GetAsync(20)).Returns(Task.FromResult(documentWrongTenant));
            var readOnlyDataViewModel = dataController.Document.GetEntityAsync("10").Result;
            Assert.IsNotNull(readOnlyDataViewModel);
            var documentDataViewModel = readOnlyDataViewModel as DocumentReadOnlyDataViewModel;
            Assert.IsNotNull(documentDataViewModel);
            readOnlyDataViewModel = dataController.Document.GetEntityAsync("20").Result;
            Assert.IsNull(readOnlyDataViewModel);
            connectionControllerMockSetup.DocumentManagerMock.Verify(manager => manager.GetAsync(10), Times.Once());
            connectionControllerMockSetup.DocumentManagerMock.Verify(manager => manager.GetAsync(20), Times.Once());
        }

        /// <summary>
        /// Tests a successful creation of an entity data view model
        /// </summary>
        [TestMethod]
        public void CreateEntitySuccessTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var tenant2 = new Tenant { Id = 2, Name = "SecondTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(manager => manager.Create())
                .Returns(new DocumentWritableModel());
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            var tenantReadable2 = new Helpers.TenantReadableModelMock(tenant2);
            connectionControllerMockSetup.TenantManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<TenantQuery>()))
                .Returns(
                    Task.FromResult(new List<TenantReadableModel> { tenantReadable2, tenantReadable }.AsEnumerable()));
            connectionControllerMockSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var entity = dataController.Document.CreateEntityAsync().Result;
            Assert.IsNotNull(entity);
            var documentDataViewModel = entity as DocumentDataViewModel;
            Assert.IsNotNull(documentDataViewModel);
            Assert.IsFalse(documentDataViewModel.IsLoading);
            Assert.IsNotNull(documentDataViewModel.Tenant);
            Assert.IsNotNull(documentDataViewModel.Tenant.SelectedEntity);
            Assert.AreEqual(tenant.Name, documentDataViewModel.Tenant.SelectedEntity.Name);
        }

        /// <summary>
        /// Tests that no exception is thrown if there was a problem connecting to the BGS.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void CreateEntityNoConnectionTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(manager => manager.Create())
                .Returns(new DocumentWritableModel());
            connectionControllerMockSetup.TenantManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<TenantQuery>())).Throws(new Exception("No tenants"));
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var entity = dataController.Document.CreateEntityAsync().Result;
        }

        /// <summary>
        /// Tests the creation and preparation of an editable entity without an implementation of PostStartEditEntity().
        /// </summary>
        [TestMethod]
        public void EditEntityWithoutPostImplementationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var tenant2 = new Tenant { Id = 2, Name = "SecondTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(manager => manager.Create())
            .Returns(new DocumentWritableModel());
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            var tenantReadable2 = new Helpers.TenantReadableModelMock(tenant2);
            connectionControllerMockSetup.TenantManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<TenantQuery>()))
                .Returns(
                    Task.FromResult(new List<TenantReadableModel> { tenantReadable2, tenantReadable }.AsEnumerable()));
            connectionControllerMockSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            DocumentReadableModel document =
               new Helpers.DocumentReadableModelMock(new Document { Id = 10, Tenant = tenant, Name = "Document" });
            var documentEntity = dataController.Document.Factory.CreateReadOnly(document);
            var entity = dataController.Document.EditEntityAsync(documentEntity).Result;
            Assert.IsNotNull(entity);
            var documentDataViewModel = entity as DocumentDataViewModel;
            Assert.IsNotNull(documentDataViewModel);
            Assert.IsFalse(documentDataViewModel.IsLoading);
            Assert.IsNotNull(documentDataViewModel.Tenant);
            Assert.AreEqual(document.Name, entity.DisplayText);
            Assert.AreEqual(tenant.Name, documentDataViewModel.Tenant.SelectedEntity.Name);
        }

        /// <summary>
        /// Tests the call for edit on the wrong data controller.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void EditEntityWrongTypeTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };

            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            var tenantDataViewModel = dataController.Factory.CreateReadOnly(tenantReadable);
            dataController.MediaConfiguration.EditEntityAsync(tenantDataViewModel).Wait();
        }

        /// <summary>
        /// Tests the copy of an entity without its own post implementation.
        /// </summary>
        [TestMethod]
        public void CopyEntityWithoutPostImplementationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            var documentWritable = new DocumentWritableModel
                                       {
                                           Name = "Copy",
                                           Tenant = tenantReadable
                                       };
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.CreateCopy(It.IsAny<DocumentReadableModel>())).Returns(documentWritable);
            connectionControllerMockSetup.TenantManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<TenantQuery>()))
                .Returns(Task.FromResult((new List<TenantReadableModel> { tenantReadable }).AsEnumerable()));
            connectionControllerMockSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            DocumentReadableModel document =
               new Helpers.DocumentReadableModelMock(new Document { Id = 10, Tenant = tenant, Name = "Document" });
            var readable = dataController.Factory.CreateReadOnly(document);
            var copy = dataController.Document.CopyEntityAsync(readable).Result;
            Assert.IsNotNull(copy);
            var documentCopy = copy as DocumentDataViewModel;
            Assert.IsNotNull(documentCopy);
            Assert.AreEqual(documentWritable.Name, documentCopy.Name);
            Assert.AreEqual(tenant.Id, documentCopy.Tenant.SelectedEntity.Id);
        }

        /// <summary>
        /// Tests the call for copy on the wrong data controller.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void CopyEntityWrongTypeTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };

            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
            var tenantDataViewModel = dataController.Factory.CreateReadOnly(tenantReadable);
            dataController.MediaConfiguration.CopyEntityAsync(tenantDataViewModel).Wait();
        }

        /// <summary>
        /// Tests the saving of an entity that doesn't have its own pre and post saving implementation.
        /// </summary>
        [TestMethod]
        public void SaveEntityWithoutPreAndPostImplementationTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
             var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
             Helpers.CreateApplicationStateMock(container, tenant);
            DocumentReadableModel document =
               new Helpers.DocumentReadableModelMock(new Document { Id = 10, Tenant = tenant, Name = "Document" });
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.CommitAndVerifyAsync(It.IsAny<DocumentWritableModel>()))
                .Returns(Task.FromResult(document));
            var documentWritableModel = new DocumentWritableModel
                                            {
                                                Name = "Writable",
                                                Tenant = tenantReadable,
                                            };
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var writable = dataController.Factory.Create(documentWritableModel);
            var savedModel = dataController.Document.SaveEntityAsync(writable).Result;
            Assert.AreEqual("Document", savedModel.DisplayText);
            connectionControllerMockSetup.DocumentManagerMock.Verify(
                manager => manager.CommitAndVerifyAsync(documentWritableModel), Times.Once());
        }

        /// <summary>
        /// Tests the deletion of an entity.
        /// </summary>
        [TestMethod]
        public void DeleteEntityTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            Helpers.CreateApplicationStateMock(container, tenant);
            DocumentReadableModel document =
               new Helpers.DocumentReadableModelMock(new Document { Id = 10, Tenant = tenant, Name = "Document" });

            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.DocumentManagerMock.Setup(
                manager => manager.DeleteAsync(It.IsAny<DocumentReadableModel>()))
                .Returns(Task.FromResult(0));

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var documentReadable = dataController.Factory.CreateReadOnly(document);
            dataController.Document.DeleteEntityAsync(documentReadable).Wait();
            connectionControllerMockSetup.DocumentManagerMock.Verify(
                manager => manager.DeleteAsync(document),
                Times.Once());
        }

        /// <summary>
        /// Tests the call for delete on the wrong data controller.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteEntityWrongTypeTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };

            Helpers.CreateApplicationStateMock(container, tenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();

            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var tenantReadable = new Helpers.TenantReadableModelMock(tenant);
             var tenantDataViewModel = dataController.Factory.CreateReadOnly(tenantReadable);
            try
            {
                dataController.Document.DeleteEntityAsync(tenantDataViewModel).Wait();
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(1, exception.InnerExceptions.Count);
                throw exception.InnerException;
            }
        }
    }
}
