// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitDataControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the Unit specific Pre and Post implementations
    /// </summary>
    [TestClass]
    public class UnitDataControllerTest
    {
        /// <summary>
        /// Tests deleting a unit with all related entities (and removing references).
        /// </summary>
        [TestMethod]
        public void DeleteUnitTest()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant" });
            var result = new TaskCompletionSource<int>();
            result.SetResult(1);
            var logEntryServiceMock = Helpers.SetupChannelScopeFactory<ILogEntryDataService>();
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var connectionController = new ConnectionControllerMock();
            connectionController.Configure();
            dataController.Initialize(connectionController);

            var tenant = connectionController.TenantChangeTrackingManager.Create();
            tenant.Name = "TestTenant";
            var tenantReadable = connectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(tenant).Result;

            var updateGroup = connectionController.UpdateGroupChangeTrackingManager.Create();
            updateGroup.Name = "TestUpdateGroup";
            updateGroup.Tenant = tenantReadable;
            var updateGroupReadable =
                connectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(updateGroup).Result;

            var unit = connectionController.UnitChangeTrackingManager.Create();
            unit.Tenant = tenantReadable;
            unit.UpdateGroup = updateGroupReadable;
            var unitReadable = connectionController.UnitChangeTrackingManager.CommitAndVerifyAsync(unit).Result;

            logEntryServiceMock.Setup(
                service => service.DeleteAsync(It.Is<LogEntryFilter>(
                    filter =>
                    filter.Unit != null && filter.Unit.Id != null
                    && filter.Unit.Id.Comparison == Int32Comparison.ExactMatch
                    && filter.Unit.Id.Value == unitReadable.Id))).Returns(result.Task);

            var updateCommand = connectionController.UpdateCommandChangeTrackingManager.Create();
            updateCommand.Unit = unitReadable;
            var updateCommandReadable =
                connectionController.UpdateCommandChangeTrackingManager.CommitAndVerifyAsync(updateCommand).Result;

            var updateCommandFeedback = connectionController.UpdateFeedbackChangeTrackingManager.Create();
            updateCommandFeedback.UpdateCommand = updateCommandReadable;
            var updateFeedbackReadable =
                connectionController.UpdateFeedbackChangeTrackingManager.CommitAndVerifyAsync(updateCommandFeedback)
                    .Result;

            updateCommandReadable.LoadNavigationPropertiesAsync().Wait();
            Assert.AreSame(
                updateFeedbackReadable,
                updateCommandReadable.Feedbacks.First(),
                "Test setup not valid. Feedback not assigned to command");

            var dvm = dataController.Factory.CreateReadOnly(unitReadable);
            dataController.Unit.DeleteEntityAsync(dvm).Wait();

            Assert.AreEqual(0, updateCommandReadable.Feedbacks.Count);
            logEntryServiceMock.Verify(
                service =>
                service.DeleteAsync(
                    It.Is<LogEntryFilter>(
                        filter =>
                        filter.Unit != null && filter.Unit.Id != null
                        && filter.Unit.Id.Comparison == Int32Comparison.ExactMatch
                        && filter.Unit.Id.Value == unitReadable.Id)),
                Times.Once());
        }
    }
}