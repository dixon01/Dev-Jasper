// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupDataControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Update.ServiceModel.Common;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the UpdateGroup specific Pre and Post implementations
    /// </summary>
    [TestClass]
    public class UpdateGroupDataControllerTest
    {
        /// <summary>
        /// Tests deleting an update group with all related entities (and removing references).
        /// </summary>
        [TestMethod]
        public void DeleteUpdateGroupTest()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant" });
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var updatePartDataServiceMock = new Mock<IUpdatePartDataService>(MockBehavior.Strict);
            var connectionController = new InternalConnectionControllerMock(updatePartDataServiceMock.Object);
            connectionController.Configure();
            dataController.Initialize(connectionController);

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

            var document2 = connectionController.DocumentChangeTrackingManager.Create();
            document.Name = "OtherConfig";
            document.Tenant = tenantReadable;
            var document2Readable =
                connectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(document2).Result;

            var unitConfig = connectionController.UnitConfigurationChangeTrackingManager.Create();
            unitConfig.Document = documentReadable;
            unitConfig.ProductType = productTypeReadable;
            var unitConfigReadable =
                connectionController.UnitConfigurationChangeTrackingManager.CommitAndVerifyAsync(unitConfig).Result;

            var unitConfig2 = connectionController.UnitConfigurationChangeTrackingManager.Create();
            unitConfig2.Document = document2Readable;
            unitConfig2.ProductType = productTypeReadable;
            unitConfig2.Commit();

            var updateGroup = connectionController.UpdateGroupChangeTrackingManager.Create();
            updateGroup.Name = "MyGroup";
            updateGroup.UnitConfiguration = unitConfigReadable;
            var updateGroupReadable =
                connectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(updateGroup).Result;

            var updatePart = connectionController.UpdatePartChangeTrackingManager.Create();
            updatePart.UpdateGroup = updateGroupReadable;
            updatePart.Type = UpdatePartType.Setup;
            updatePart.Structure = new XmlData(new UpdateFolderStructure());
            var updatePartReadable =
                connectionController.UpdatePartChangeTrackingManager.CommitAndVerifyAsync(updatePart).Result;

            IEnumerable<UpdatePart> updateParts = new List<UpdatePart> { updatePartReadable.ToDto() };
            updatePartDataServiceMock.Setup(s => s.QueryAsync(It.IsAny<UpdatePartQuery>()))
                .Returns(Task.FromResult(updateParts));
            updatePartDataServiceMock.Setup(s => s.UpdateAsync(It.IsAny<UpdatePart>()))
                .Returns<UpdatePart>(
                    p =>
                        {
                            Assert.AreEqual(0, p.RelatedCommands.Count);
                            return Task.FromResult(p);
                        });
            updatePartDataServiceMock.Setup(s => s.DeleteAsync(It.IsAny<UpdatePart>()))
                .Returns<UpdatePart>(Task.FromResult);

            // ACT
            var dvm = dataController.Factory.CreateReadOnly(updateGroupReadable);
            dataController.UpdateGroup.DeleteEntityAsync(dvm).Wait();

            // ASSERT
            Assert.AreEqual(0, connectionController.UpdateGroupChangeTrackingManager.QueryAsync().Result.Count());
            updatePartDataServiceMock.Verify(s => s.QueryAsync(It.IsAny<UpdatePartQuery>()), Times.Once());
            updatePartDataServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdatePart>()), Times.Once());
            updatePartDataServiceMock.Verify(s => s.DeleteAsync(It.IsAny<UpdatePart>()), Times.Once());
        }

        private class InternalConnectionControllerMock : ConnectionControllerMock
        {
            private readonly IUpdatePartDataService updatePartDataService;

            public InternalConnectionControllerMock(IUpdatePartDataService updatePartDataService)
            {
                this.updatePartDataService = updatePartDataService;
            }

            public override ChannelScope<T> CreateChannelScope<T>()
            {
                return new ChannelScopeMock<T>((T)this.updatePartDataService);
            }
        }

        private class ChannelScopeMock<T> : ChannelScope<T>
            where T : class
        {
            public ChannelScopeMock(T channel)
                : base(channel)
            {
            }
        }
    }
}
