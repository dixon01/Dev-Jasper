// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryLayerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RepositoryLayerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Tests;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Units;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using ModelAuthorization = Model.AccessControl.Authorization;
    using ModelProductType = Model.Units.ProductType;
    using ModelUserRole = Model.AccessControl.UserRole;
    using ModelXmlData = Model.XmlData;

    /// <summary>
    /// The repository layer test.
    /// </summary>
    [TestClass]
    public class RepositoryLayerTest
    {
        private TestCenterDataContext mockCenterDataContext;

        /// <summary>
        /// Initializes a test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.mockCenterDataContext = SetupMockCenterDataContext();
        }

        /// <summary>
        /// Cleans up a test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.mockCenterDataContext = null;
        }

        /// <summary>
        /// The test create simple entity.
        /// </summary>
        [TestMethod]
        public void TestCreateSimpleEntity()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.TestCreateSimpleEntityAsync();
            task.Wait();
        }

        /// <summary>
        /// The test read simple entity.
        /// </summary>
        [TestMethod]
        public void TestReadSimpleEntity()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.TestReadSimpleEntityAsync();
            task.Wait();
        }

        /// <summary>
        /// The test delete simple entity.
        /// </summary>
        [TestMethod]
        public void TestDeleteSimpleEntity()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.TestDeleteSimpleEntityAsync();
            task.Wait();

            task = this.TestDeleteSimpleEntityNotPresentAsync();
            task.Wait();
        }

        /// <summary>
        /// The test delete entity with Xml Data.
        /// </summary>
        [TestMethod]
        public void TestDeleteEntityWithXml()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.TestDeleteEntityWithXmlAsync();
            task.Wait();
        }

        /// <summary>
        /// The test update simple entity.
        /// </summary>
        [TestMethod]
        public void TestUpdateSimpleEntity()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.TestUpdateSimpleEntityAsync();
            task.Wait();
        }

        private static TestCenterDataContext SetupMockCenterDataContext()
        {
            var testCenterDataContext = new TestCenterDataContext();
            testCenterDataContext.Clear();
            var mockDbContextFactory = new Mock<DataContextFactory>();
            mockDbContextFactory.Setup(c => c.Create()).Returns(testCenterDataContext);
            DataContextFactory.SetCurrent(mockDbContextFactory.Object);
            return testCenterDataContext;
        }

        private async Task TestDeleteSimpleEntityAsync()
        {
            this.mockCenterDataContext.Authorizations = new FakeDbSetIdFind<ModelAuthorization>();

            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 4 });
            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 5 });
            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 3 });

            var databaseEntity = new Authorization { Id = 4 }.ToDatabase();

            var repository = AuthorizationRepositoryFactory.Current.Create();

            Assert.AreEqual(this.mockCenterDataContext.Authorizations.Count(), 3);
            await repository.RemoveAsync(databaseEntity);

            Assert.AreEqual(this.mockCenterDataContext.Authorizations.Count(), 2);
            var deletedIdExists = this.mockCenterDataContext.Authorizations.Any(a => a.Id == 4);
            Assert.IsFalse(deletedIdExists);
        }

        private async Task TestDeleteSimpleEntityNotPresentAsync()
        {
            this.mockCenterDataContext.Authorizations = new FakeDbSetIdFind<ModelAuthorization>();
            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 3 });
            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 5 });
            var databaseEntity = new Authorization { Id = 4 }.ToDatabase();

            var repository = AuthorizationRepositoryFactory.Current.Create();

            Assert.AreEqual(this.mockCenterDataContext.Authorizations.Count(), 2);
            await repository.RemoveAsync(databaseEntity);

            Assert.AreEqual(this.mockCenterDataContext.Authorizations.Count(), 2);
        }

        private async Task TestDeleteEntityWithXmlAsync()
        {
            this.mockCenterDataContext.ProductTypes = new FakeDbSetIdFind<ModelProductType>();
            this.mockCenterDataContext.XmlData = new FakeDbSetIdFind<ModelXmlData>();

            var xmlData1 = new ModelXmlData { Id = 1, Xml = "<xml>1</xml>", Type = "My.Type, My" };
            var xmlData2 = new ModelXmlData { Id = 2, Xml = "<xml>2</xml>", Type = "My.Type, My" };
            var xmlData3 = new ModelXmlData { Id = 3, Xml = "<xml>3</xml>", Type = "My.Type, My" };
            var xmlData4 = new ModelXmlData { Id = 4, Xml = "<xml>4</xml>", Type = "My.Type, My" };

            this.mockCenterDataContext.XmlData.Add(xmlData1);
            this.mockCenterDataContext.XmlData.Add(xmlData2);
            this.mockCenterDataContext.XmlData.Add(xmlData3);
            this.mockCenterDataContext.XmlData.Add(xmlData4);

            this.mockCenterDataContext.ProductTypes.Add(new ModelProductType { Id = 4, HardwareDescriptor = xmlData1 });
            this.mockCenterDataContext.ProductTypes.Add(new ModelProductType { Id = 5, HardwareDescriptor = xmlData2 });
            this.mockCenterDataContext.ProductTypes.Add(new ModelProductType { Id = 3, HardwareDescriptor = xmlData3 });

            var databaseEntity = new ProductType { Id = 4 }.ToDatabase();

            var repository = ProductTypeRepositoryFactory.Current.Create();

            Assert.AreEqual(this.mockCenterDataContext.ProductTypes.Count(), 3);
            Assert.AreEqual(this.mockCenterDataContext.XmlData.Count(), 4);

            Assert.IsTrue(this.mockCenterDataContext.ProductTypes.Any(p => p.Id == 4));
            Assert.IsTrue(this.mockCenterDataContext.XmlData.Any(x => x.Id == 1));

            await repository.RemoveAsync(databaseEntity);

            Assert.AreEqual(this.mockCenterDataContext.ProductTypes.Count(), 2);
            Assert.AreEqual(this.mockCenterDataContext.XmlData.Count(), 3);

            Assert.IsFalse(this.mockCenterDataContext.ProductTypes.Any(p => p.Id == 4));
            Assert.IsFalse(this.mockCenterDataContext.XmlData.Any(x => x.Id == 1));
        }

        private async Task TestReadSimpleEntityAsync()
        {
            this.mockCenterDataContext.Authorizations = new FakeDbSetIdFind<ModelAuthorization>();
            this.mockCenterDataContext.Authorizations.Add(new ModelAuthorization { Id = 4 });

            var repository = AuthorizationRepositoryFactory.Current.Create();
            var result = await repository.FindAsync(4);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 4);

            result = await repository.FindAsync(3);
            Assert.IsNull(result);
        }

        private async Task TestCreateSimpleEntityAsync()
        {
            this.mockCenterDataContext.Authorizations = new FakeDbSetIdFind<ModelAuthorization>();
            this.mockCenterDataContext.UserRoles = new FakeDbSetIdFind<ModelUserRole> { new ModelUserRole { Id = 12 } };

            var databaseEntity = new Authorization
                         {
                             Id = 4,
                             UserRole = new UserRole { Id = 12 },
                             CreatedOn = new DateTime(2015, 1, 2, 3, 4, 5, DateTimeKind.Utc)
                         }.ToDatabase();

            var repository = AuthorizationRepositoryFactory.Current.Create();
            var result = await repository.AddAsync(databaseEntity);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 4);
            Assert.AreEqual(result.UserRole.Id, 12);

            // insert the second time updates
            var newTime = new DateTime(2015, 1, 2, 3, 4, 6, DateTimeKind.Utc);
            databaseEntity.CreatedOn = newTime;
            result = await repository.AddAsync(databaseEntity);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 4);
            Assert.AreEqual(result.CreatedOn, newTime);
            Assert.AreEqual(result.UserRole.Id, 12);
        }

        private async Task TestUpdateSimpleEntityAsync()
        {
            this.mockCenterDataContext.UserRoles = new FakeDbSetIdFind<ModelUserRole> { new ModelUserRole { Id = 12 } };
            this.mockCenterDataContext.Authorizations = new FakeDbSetIdFind<ModelAuthorization>();

            var databaseEntity = new Authorization
            {
                Id = 4,
                Version = 1,
                UserRole = new UserRole { Id = 12 }
            }.ToDatabase();

            var repository = AuthorizationRepositoryFactory.Current.Create();
            var result = await repository.AddAsync(databaseEntity);
            Assert.IsFalse(this.mockCenterDataContext.SetReferenceWasCalled);
            Assert.IsNotNull(result);
            databaseEntity.Version = 2;
            result = await repository.UpdateAsync(databaseEntity);

            Assert.IsNotNull(result);
            Assert.IsTrue(this.mockCenterDataContext.SetValueWasCalled);
            Assert.IsTrue(this.mockCenterDataContext.SetReferenceWasCalled);
            Assert.AreEqual(result.Id, 4);
            Assert.AreEqual(result.Version, 2);
        }

        /// <summary>
        /// The mock center data context.
        /// </summary>
        private class TestCenterDataContext : CenterDataContext
        {
            static TestCenterDataContext()
            {
                // This is needed to ensure that EntityFramework.SqlServer.dll is copied to the output folder.
                // This fixes "Provider not loaded" exception on TFS build server.
                // See also http://robsneuron.blogspot.nl/2013/11/entity-framework-upgrade-to-6.html
                // ReSharper disable once UnusedVariable
                var ensureDllIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            }

            public bool SetValueWasCalled { get; set; }

            public bool SetReferenceWasCalled { get; set; }

            public override void SetValues<TEntity>(TEntity original, TEntity entity)
            {
                this.SetValueWasCalled = true;
            }

            public override void SetReference<TEntity>(TEntity entity, string referenceName, object newValue)
            {
                this.SetReferenceWasCalled = true;
            }

            public override void LoadReference<TEntity>(TEntity original, string referenceName)
            {
                // don't do anything as it is confusing EF
            }

            public void Clear()
            {
                this.Authorizations = null;
                this.UserRoles = null;
                this.UnitConfigurations = null;
                this.MediaConfigurations = null;
                this.Documents = null;
                this.DocumentVersions = null;
                this.LogEntries = null;
                this.Tenants = null;
                this.Users = null;
                this.AssociationTenantUserUserRoles = null;
                this.UserDefinedProperties = null;
                this.SystemConfigs = null;
                this.Resources = null;
                this.Packages = null;
                this.PackageVersions = null;
                this.ProductTypes = null;
                this.Units = null;
                this.UpdateGroups = null;
                this.UpdateParts = null;
                this.UpdateCommands = null;
                this.UpdateFeedbacks = null;
                this.XmlData = null;
            }

            /// <summary>
            /// The save changes async which does nothing.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task<int> SaveChangesAsync()
            {
                return Task.FromResult(5);
            }
        }

        private class FakeDbSetIdFind<T> : FakeDbSet<T>
            where T : class
        {
            public override T Find(params object[] id)
            {
                return this.Local.FirstOrDefault(
                    item =>
                    {
                        var property = item.GetType().GetProperty("Id");
                        if (property == null)
                        {
                            throw new Exception(item.GetType().ToString() + " has no property 'Id' used by Find()");
                        }

                        var value = property.GetValue(item);

                        if (!(value is int))
                        {
                            throw new Exception(
                                item.GetType().ToString() + " property 'Id' is not an int. Used by Find()");
                        }

                        var intValue = (int)value;

                        return intValue == (int)id[0];
                    });
            }
        }
    }
}