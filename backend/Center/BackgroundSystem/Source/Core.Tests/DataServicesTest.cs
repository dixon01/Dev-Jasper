// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServicesTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   CRUD tests for the data services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using NLog;
    using NLog.Targets;

    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    /// <summary>
    /// CRUD tests for the data services.
    /// </summary>
    [TestClass]
    public class DataServicesTest
    {
        private Mock<IRepository<Data.Model.Membership.Tenant>> tenantRepositoryMock;

        private Mock<IRepository<Data.Model.Membership.User>> userRepositoryMock;

        private Mock<IRepository<Data.Model.Membership.AssociationTenantUserUserRole>> atuurRepositoryMock;

        private static new readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var tenantFactory = new TenantRepositoryFactoryMock();
            this.tenantRepositoryMock = new Mock<IRepository<Data.Model.Membership.Tenant>>();
            tenantFactory.TenantRepositoryMock = this.tenantRepositoryMock;
            TenantRepositoryFactory.SetInstance(tenantFactory);

            var userFactory = new UserRepositoryFactoryMock();
            this.userRepositoryMock = new Mock<IRepository<Data.Model.Membership.User>>();
            userFactory.UserRepositoryMock = this.userRepositoryMock;
            UserRepositoryFactory.SetInstance(userFactory);

            this.atuurRepositoryMock = new Mock<IRepository<Data.Model.Membership.AssociationTenantUserUserRole>>();
            var atuurFactory = new Mock<AssociationTenantUserUserRoleRepositoryFactory>();
            atuurFactory.Setup(f => f.Create()).Returns(() => this.atuurRepositoryMock.Object);
            AssociationTenantUserUserRoleRepositoryFactory.SetInstance(atuurFactory.Object);
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            TenantRepositoryFactory.ResetInstance();
        }

        [TestMethod]
        public void NlogWriteTest()
        {
            try
            {
                var ex = new Exception("TEST");
                Logger.Error("Exception = {0}", ex); // Old to be Deprecated

                // See http://nlog-project.org/documentation/v4.3.0/html/M_NLog_Logger_Error_25.htm
                Logger.Error(ex, "Exception "); // New 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Assert.Fail();
            }
        }

        /// <summary>
        /// The add simple entity test.
        /// </summary>
        [TestMethod]
        public void AddSimpleEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.AddSimpleEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// The add null test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void AddNullTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.AddNullTestInternal();
            task.Wait();
        }

        /// <summary>
        /// The delete existing simple entity test.
        /// </summary>
        [TestMethod]
        public void DeleteExistingSimpleEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.DeleteExistingSimpleEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// The delete non existing entity test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DeleteNonExistingEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.DeleteNonExistingEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// The update simple entity test.
        /// </summary>
        [TestMethod]
        public void UpdateSimpleEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.UpdateSimpleEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests getting an entity.
        /// </summary>
        [TestMethod]
        public void GetEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.GetEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the case for getting an entity which doesn't exists.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetNonExistingEntityTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.GetNonExistingEntityTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the query without filtering.
        /// </summary>
        [TestMethod]
        public void QueryEntityWithoutFilterTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.QueryEntityWithoutFilterTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the query with simple filtering.
        /// </summary>
        [TestMethod]
        public void QueryEntityWithSimpleFilterTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.QueryEntityWithSimpleFilterTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the query with reference filtering:
        /// User -> Tenant
        /// </summary>
        [TestMethod]
        public void QueryEntityWithReferenceFilterTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.QueryEntityWithReferenceFilterTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the query with two reference filtering:
        /// AssociationTenantUserUserRole -> User -> Tenant
        /// </summary>
        [TestMethod]
        public void QueryEntityWithDoubleReferenceFilterTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.QueryEntityWithDoubleReferenceFilterTestInternal();
            task.Wait();
        }

        /// <summary>
        /// Tests the query with multiple reference filtering:
        /// AssociationTenantUserUserRole -> User -> Tenant
        /// </summary>
        [TestMethod]
        public void QueryEntityWithMultiReferenceFilterTest()
        {
            // Workaround for TFS 2010 because it can't run test methods with the signature "async Task".
            var task = this.QueryEntityWithMultiReferenceFilterTestInternal();
            task.Wait();
        }

        private async Task AddNullTestInternal()
        {
            var dtoTenant = new Tenant { Id = 1, Name = "Tenant" };
            var tenantDataService = new TenantDataService();
            await tenantDataService.AddAsync(dtoTenant);
        }

        private async Task AddSimpleEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var userEntity = new Data.Model.Membership.User { Id = 10, FirstName = "repositoryUser" };
            var tenantEntity = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant",
                Users = new[] { userEntity }
            };
            this.tenantRepositoryMock.Setup(r => r.AddAsync(Match.Create<Data.Model.Membership.Tenant>(t => t.Id == 1)))
                .Returns(Task.FromResult(tenantEntity));
            var dtoTenant = new Tenant { Id = 1, Name = "Tenant" };
            var tenantDataService = new TenantDataService();
            var result = await tenantDataService.AddAsync(dtoTenant);
            Assert.AreEqual(dtoTenant.Id, result.Id);
            Assert.AreEqual(tenantEntity.Name, result.Name);
            Assert.IsNotNull(result.Users);
            Assert.AreEqual(1, result.Users.Count);
            Assert.AreEqual(utcNow, result.CreatedOn);

            this.tenantRepositoryMock.Verify(t => t.AddAsync(It.IsAny<Data.Model.Membership.Tenant>()), Times.Once());
        }

        private async Task DeleteExistingSimpleEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow,
                Name = "RepositoryTenant2",
            };
            var tenants = new List<Data.Model.Membership.Tenant> { tenantEntity1, tenantEntity2 };
            this.tenantRepositoryMock.Setup(r => r.FindAsync(It.IsAny<object[]>()))
                .Returns(
                    (object[] i) =>
                    {
                        var id = (int)i[0];
                        return Task.FromResult(tenants.FirstOrDefault(t => t.Id == id));
                    });
            this.tenantRepositoryMock.Setup(r => r.RemoveAsync(It.IsAny<Data.Model.Membership.Tenant>()))
                .Returns(Task.FromResult<object>(null));
            var tenantDataService = new TenantDataService();
            await tenantDataService.DeleteAsync(new Tenant { Id = 1 });
            this.tenantRepositoryMock.Verify(
                r => r.RemoveAsync(It.IsAny<Data.Model.Membership.Tenant>()), Times.Once());
            await tenantDataService.DeleteAsync(new Tenant { Id = 2 });
            this.tenantRepositoryMock.Verify(
                r => r.RemoveAsync(It.IsAny<Data.Model.Membership.Tenant>()), Times.Exactly(2));
        }

        private async Task DeleteNonExistingEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenants = new List<Data.Model.Membership.Tenant> { tenantEntity1 };
            this.tenantRepositoryMock.Setup(r => r.FindAsync(It.IsAny<object[]>()))
                .Returns(
                    (object[] i) =>
                    {
                        var id = (int)i[0];
                        return Task.FromResult(tenants.FirstOrDefault(t => t.Id == id));
                    });
            this.tenantRepositoryMock.Setup(r => r.RemoveAsync(It.IsAny<Data.Model.Membership.Tenant>()))
                .Returns(Task.FromResult<object>(null));
            var tenantDataService = new TenantDataService();
            await tenantDataService.DeleteAsync(new Tenant { Id = 1 });
            this.tenantRepositoryMock.Verify(
                r => r.RemoveAsync(It.IsAny<Data.Model.Membership.Tenant>()), Times.Once());
            await tenantDataService.DeleteAsync(new Tenant { Id = 2 });
        }

        private async Task UpdateSimpleEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;

            this.tenantRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Data.Model.Membership.Tenant>()))
                .Returns(
                    (Data.Model.Membership.Tenant t) =>
                    {
                        t.LastModifiedOn = utcNow.AddDays(1);
                        return Task.FromResult(t);
                    });
            var tenantDataService = new TenantDataService();
            var dtoTenant = new Tenant { Id = 1, Name = "Tenant", CreatedOn = utcNow };
            var result = await tenantDataService.UpdateAsync(dtoTenant);
            Assert.IsNotNull(result);
            Assert.AreEqual(utcNow, result.CreatedOn);
            Assert.AreEqual(utcNow.AddDays(1), result.LastModifiedOn);
            Assert.AreEqual(dtoTenant.Id, result.Id);
            Assert.AreEqual(dtoTenant.Name, result.Name);
        }

        private async Task GetEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow.AddDays(1),
                Name = "RepositoryTenant2",
            };
            var tenants = new List<Data.Model.Membership.Tenant> { tenantEntity1, tenantEntity2 };
            this.tenantRepositoryMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).Returns(
                (object[] objectParams) =>
                {
                    var id = (int)objectParams.First();
                    return Task.FromResult(tenants.FirstOrDefault(t => t.Id == id));
                });
            var tenantDataService = new TenantDataService();
            var tenant2 = await tenantDataService.GetAsync(2);
            Assert.IsNotNull(tenant2);
            Assert.AreEqual(utcNow.AddDays(1), tenant2.CreatedOn);
        }

        private async Task GetNonExistingEntityTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenants = new List<Data.Model.Membership.Tenant> { tenantEntity1 };
            this.tenantRepositoryMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).Returns(
                (object[] objectParams) =>
                {
                    var id = (int)objectParams.First();
                    return Task.FromResult(tenants.FirstOrDefault(t => t.Id == id));
                });
            var tenantDataService = new TenantDataService();
            await tenantDataService.GetAsync(2);
        }

        private async Task QueryEntityWithoutFilterTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
                                    {
                                        Id = 1,
                                        CreatedOn = utcNow,
                                        Name = "RepositoryTenant1",
                                    };
            var tenantEntity2 = new Data.Model.Membership.Tenant
                                   {
                                       Id = 2,
                                       CreatedOn = utcNow.AddDays(1),
                                       Name = "RepositoryTenant2",
                                   };
            var fakeDbSet = new FakeDbSetIdFind<Data.Model.Membership.Tenant> { tenantEntity1, tenantEntity2 };
            this.tenantRepositoryMock.Setup(r => r.Query()).Returns(fakeDbSet.AsNoTracking);
            var tenantDataService = new TenantDataService();
            var result = await tenantDataService.QueryAsync();
            Assert.IsNotNull(result);
            var dtoTenants = result.ToList();
            Assert.AreEqual(2, dtoTenants.Count);
        }

        private async Task QueryEntityWithSimpleFilterTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow.AddDays(1),
                Name = "RepositoryTenant2",
            };
            var tenantEntity3 = new Data.Model.Membership.Tenant
            {
                Id = 3,
                CreatedOn = utcNow.AddDays(2),
                Name = "RepositoryTenant3",
            };
            var fakeDbSet = new FakeDbSetIdFind<Data.Model.Membership.Tenant>
                                {
                                    tenantEntity1,
                                    tenantEntity2,
                                    tenantEntity3
                                };
            this.tenantRepositoryMock.Setup(r => r.Query()).Returns(fakeDbSet.AsNoTracking);
            var tenantDataService = new TenantDataService();
            var result = await tenantDataService.QueryAsync(TenantQuery.Create().WithName("RepositoryTenant2"));
            Assert.IsNotNull(result);
            var dtoTenants = result.ToList();
            Assert.AreEqual(1, dtoTenants.Count);
            Assert.AreEqual(2, dtoTenants[0].Id);
            Assert.AreEqual(utcNow.AddDays(1), dtoTenants[0].CreatedOn);
            Assert.AreEqual("RepositoryTenant2", dtoTenants[0].Name);
        }

        private async Task QueryEntityWithReferenceFilterTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow.AddDays(1),
                Name = "RepositoryTenant2",
            };
            var tenantEntity3 = new Data.Model.Membership.Tenant
            {
                Id = 3,
                CreatedOn = utcNow.AddDays(2),
                Name = "RepositoryTenant3",
            };

            var userEntity1 = new Data.Model.Membership.User
            {
                Id = 1,
                Username = "User1",
                OwnerTenant = tenantEntity2
            };
            var userEntity2 = new Data.Model.Membership.User
            {
                Id = 2,
                Username = "User2",
                OwnerTenant = tenantEntity1
            };
            var userEntity3 = new Data.Model.Membership.User
            {
                Id = 3,
                Username = "User3",
                OwnerTenant = tenantEntity2
            };
            var tenantDbSet = new FakeDbSetIdFind<Data.Model.Membership.Tenant>
                                {
                                    tenantEntity1,
                                    tenantEntity2,
                                    tenantEntity3
                                };
            this.tenantRepositoryMock.Setup(r => r.Query()).Returns(tenantDbSet.AsNoTracking);
            var userDbSet = new FakeDbSetIdFind<Data.Model.Membership.User> { userEntity1, userEntity2, userEntity3 };
            this.userRepositoryMock.Setup(r => r.Query()).Returns(userDbSet.AsNoTracking);

            var userDataService = new UserDataService();
            var result =
                await
                userDataService.QueryAsync(
                    UserQuery.Create().IncludeOwnerTenant(TenantFilter.Create().WithName("RepositoryTenant2")));
            Assert.IsNotNull(result);
            var dtoUsers = result.ToList();
            Assert.AreEqual(2, dtoUsers.Count);

            var user = dtoUsers[0];
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("User1", user.Username);
            Assert.IsNotNull(user.OwnerTenant);
            Assert.AreEqual("RepositoryTenant2", user.OwnerTenant.Name);

            user = dtoUsers[1];
            Assert.AreEqual(3, user.Id);
            Assert.AreEqual("User3", user.Username);
            Assert.IsNotNull(user.OwnerTenant);
            Assert.AreEqual("RepositoryTenant2", user.OwnerTenant.Name);
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        private async Task QueryEntityWithDoubleReferenceFilterTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow.AddDays(1),
                Name = "RepositoryTenant2",
            };
            var tenantEntity3 = new Data.Model.Membership.Tenant
            {
                Id = 3,
                CreatedOn = utcNow.AddDays(2),
                Name = "RepositoryTenant3",
            };

            var userEntity1 = new Data.Model.Membership.User
            {
                Id = 1,
                Username = "User1",
                OwnerTenant = tenantEntity2
            };
            var userEntity2 = new Data.Model.Membership.User
            {
                Id = 2,
                Username = "User2",
                OwnerTenant = tenantEntity1
            };
            var userEntity3 = new Data.Model.Membership.User
            {
                Id = 3,
                Username = "User3",
                OwnerTenant = tenantEntity2
            };

            var atuur1 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 1,
                Tenant = tenantEntity1,
                User = userEntity1
            };
            var atuur2 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 2,
                Tenant = tenantEntity2,
                User = userEntity2
            };
            var atuur3 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 3,
                Tenant = tenantEntity3,
                User = userEntity3
            };
            var tenantDbSet = new FakeDbSetIdFind<Data.Model.Membership.Tenant>
                                {
                                    tenantEntity1,
                                    tenantEntity2,
                                    tenantEntity3
                                };
            this.tenantRepositoryMock.Setup(r => r.Query()).Returns(tenantDbSet.AsNoTracking);
            var userDbSet = new FakeDbSetIdFind<Data.Model.Membership.User> { userEntity1, userEntity2, userEntity3 };
            this.userRepositoryMock.Setup(r => r.Query()).Returns(userDbSet.AsNoTracking);
            var atuurDbSet = new FakeDbSetIdFind<Data.Model.Membership.AssociationTenantUserUserRole>
                                 {
                                     atuur1,
                                     atuur2,
                                     atuur3
                                 };
            this.atuurRepositoryMock.Setup(r => r.Query()).Returns(atuurDbSet.AsNoTracking);

            var dataService = new AssociationTenantUserUserRoleDataService();
            var result =
                await
                dataService.QueryAsync(
                    AssociationTenantUserUserRoleQuery.Create().IncludeUser(
                        UserFilter.Create().IncludeOwnerTenant(
                            TenantFilter.Create().WithName("RepositoryTenant2"))));
            Assert.IsNotNull(result);
            var associations = result.ToList();
            Assert.AreEqual(2, associations.Count);

            var association = associations[0];
            Assert.AreEqual(1, association.Id);
            Assert.IsNotNull(association.User);
            Assert.AreEqual("User1", association.User.Username);
            Assert.IsNotNull(association.User.OwnerTenant);
            Assert.AreEqual("RepositoryTenant2", association.User.OwnerTenant.Name);

            association = associations[1];
            Assert.AreEqual(3, association.Id);
            Assert.IsNotNull(association.User);
            Assert.AreEqual("User3", association.User.Username);
            Assert.IsNotNull(association.User.OwnerTenant);
            Assert.AreEqual("RepositoryTenant2", association.User.OwnerTenant.Name);
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        private async Task QueryEntityWithMultiReferenceFilterTestInternal()
        {
            var utcNow = DateTime.UtcNow;
            var tenantEntity1 = new Data.Model.Membership.Tenant
            {
                Id = 1,
                CreatedOn = utcNow,
                Name = "RepositoryTenant1",
            };
            var tenantEntity2 = new Data.Model.Membership.Tenant
            {
                Id = 2,
                CreatedOn = utcNow.AddDays(1),
                Name = "RepositoryTenant2",
            };
            var tenantEntity3 = new Data.Model.Membership.Tenant
            {
                Id = 3,
                CreatedOn = utcNow.AddDays(2),
                Name = "RepositoryTenant3",
            };

            var userEntity1 = new Data.Model.Membership.User
            {
                Id = 1,
                Username = "User1",
                OwnerTenant = tenantEntity2
            };
            var userEntity2 = new Data.Model.Membership.User
            {
                Id = 2,
                Username = "User2",
                OwnerTenant = tenantEntity1
            };
            var userEntity3 = new Data.Model.Membership.User
            {
                Id = 3,
                Username = "User3",
                OwnerTenant = tenantEntity2
            };

            tenantEntity1.Users = new[] { userEntity2 };
            tenantEntity2.Users = new[] { userEntity1, userEntity3 };

            var atuur1 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 1,
                Tenant = tenantEntity1,
                User = userEntity1
            };
            var atuur2 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 2,
                Tenant = tenantEntity2,
                User = userEntity2
            };
            var atuur3 = new Data.Model.Membership.AssociationTenantUserUserRole
            {
                Id = 3,
                Tenant = tenantEntity3,
                User = userEntity3
            };

            userEntity1.AssociationTenantUserUserRoles = new[] { atuur1 };
            userEntity2.AssociationTenantUserUserRoles = new[] { atuur2 };
            userEntity3.AssociationTenantUserUserRoles = new[] { atuur3 };

            var tenantDbSet = new FakeDbSetIdFind<Data.Model.Membership.Tenant>
                                {
                                    tenantEntity1,
                                    tenantEntity2,
                                    tenantEntity3
                                };
            this.tenantRepositoryMock.Setup(r => r.Query()).Returns(tenantDbSet.AsNoTracking);
            var userDbSet = new FakeDbSetIdFind<Data.Model.Membership.User> { userEntity1, userEntity2, userEntity3 };
            this.userRepositoryMock.Setup(r => r.Query()).Returns(userDbSet.AsNoTracking);
            var atuurDbSet = new FakeDbSetIdFind<Data.Model.Membership.AssociationTenantUserUserRole>
                                 {
                                     atuur1,
                                     atuur2,
                                     atuur3
                                 };
            this.atuurRepositoryMock.Setup(r => r.Query()).Returns(atuurDbSet.AsNoTracking);

            var dataService = new AssociationTenantUserUserRoleDataService();
            var result =
                await
                dataService.QueryAsync(
                    AssociationTenantUserUserRoleQuery.Create().IncludeUser(
                        UserFilter.Create().WithUsername("User3", StringComparison.Different).IncludeOwnerTenant(
                            TenantFilter.Create().WithName("RepositoryTenant2"))));
            Assert.IsNotNull(result);
            var associations = result.ToList();
            Assert.AreEqual(1, associations.Count);

            var association = associations[0];
            Assert.AreEqual(1, association.Id);
            Assert.IsNotNull(association.User);
            Assert.IsNull(association.User.AssociationTenantUserUserRoles);
            Assert.AreEqual("User1", association.User.Username);
            Assert.IsNotNull(association.User.OwnerTenant);
            Assert.IsNull(association.User.OwnerTenant.Users);
            Assert.AreEqual("RepositoryTenant2", association.User.OwnerTenant.Name);
        }

        /// <summary>
        /// The mock for the tenant repository factory.
        /// </summary>
        public class TenantRepositoryFactoryMock : TenantRepositoryFactory
        {
            /// <summary>
            /// Gets or sets the tenant repository mock.
            /// </summary>
            public Mock<IRepository<Data.Model.Membership.Tenant>> TenantRepositoryMock { get; set; }

            /// <summary>
            /// Returns the mocked tenant repository object.
            /// </summary>
            /// <returns>
            /// The mocked repository
            /// </returns>
            public override IRepository<Data.Model.Membership.Tenant> Create()
            {
                return this.TenantRepositoryMock.Object;
            }
        }

        /// <summary>
        /// The mock for the user repository factory.
        /// </summary>
        public class UserRepositoryFactoryMock : UserRepositoryFactory
        {
            /// <summary>
            /// Gets or sets the user repository mock.
            /// </summary>
            public Mock<IRepository<Data.Model.Membership.User>> UserRepositoryMock { get; set; }

            /// <summary>
            /// Returns the mocked user repository object.
            /// </summary>
            /// <returns>
            /// The mocked repository
            /// </returns>
            public override IRepository<Data.Model.Membership.User> Create()
            {
                return this.UserRepositoryMock.Object;
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
