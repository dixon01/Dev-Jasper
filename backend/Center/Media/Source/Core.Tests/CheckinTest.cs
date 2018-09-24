// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckinTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckinTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for check-in.
    /// </summary>
    [TestClass]
    public class CheckinTest
    {
        private IUnityContainer unityContainer;

        private Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>> tenantAuthorizations;

        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.unityContainer = Helpers.InitializeServiceLocator();
            this.unityContainer.RegisterInstance(Mock.Of<IDispatcher>());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocatorMock.Object);
            ChangeHistoryFactory.ResetCurrent();
        }

        /// <summary>
        /// Tests a successful check-in.
        /// </summary>
        [TestMethod]
        public void CheckInSuccessTest()
        {
            var state = Helpers.MockApplicationState((UnityContainer)this.unityContainer);
            var connectionControllerMock = Helpers.CreateConnectionControllerMock();
            var resourceTaskCompSource = new TaskCompletionSource<IEnumerable<ResourceReadableModel>>();
            connectionControllerMock.Setup(c => c.ResourceChangeTrackingManager.QueryAsync(It.IsAny<ResourceQuery>()))
                .Returns(() =>
                    {
                        resourceTaskCompSource.TrySetResult(new List<ResourceReadableModel>());
                                   return resourceTaskCompSource.Task;
                });
            var commandRegistry = new CommandRegistry();
            var changeHistoryControllerMock = new Mock<IChangeHistoryController>();
            changeHistoryControllerMock.Setup(history => history.ResetChangeHistory());
            var mediaShellControllerMock = new Mock<IMediaShellController>();
            mediaShellControllerMock.Setup(controller => controller.ChangeHistoryController)
                .Returns(changeHistoryControllerMock.Object);
            Helpers.CreateMediaApplicationMock(
                (UnityContainer)this.unityContainer,
                state,
                mediaShellControllerMock,
                commandRegistry,
                connectionControllerMock);
            var currentProject = state.CurrentProject;
            state.CurrentProject = null;
            state.CurrentTenant = new TenantReadableModel(new Tenant { Id = 1, Name = "Tenant" });
            this.SetupTenantAuthorizations(state);
            state.CurrentProject = currentProject;
            Assert.IsFalse(state.CurrentProject.IsCheckedIn);
            SetupMediaConfigurationMock(state, connectionControllerMock);

            var projectController = new ProjectController(
                state.Shell,
                mediaShellControllerMock.Object,
                new MainMenuPrompt(state.Shell, commandRegistry),
                commandRegistry);

            var parameters = new CreateDocumentVersionParameters(0, 1, string.Empty);
            projectController.CheckInProjectAsync(parameters).Wait();

            Assert.IsTrue(state.CurrentProject.IsCheckedIn);
            Assert.IsFalse(state.CurrentProject.IsDirty);
            connectionControllerMock.Verify(
                c => c.DocumentVersionChangeTrackingManager.AddAsync(It.IsAny<DocumentVersionWritableModel>()),
                Times.Once());
        }

        private static void SetupMediaConfigurationMock(
            MediaApplicationState state,
            Mock<IConnectionController> connectionControllerMock)
        {
            var xmlData = new XmlData(state.CurrentProject.ToDataModel());
            var documentVersion = new DocumentVersion { Content = xmlData, };
            var document = new Document { Name = "TestProject" };
            var documentReadable = new Helpers.DocumentReadableModelMock(document);
            var documentVersionReadable = new Helpers.DocumentVersionReadableModelMock(documentVersion);
            documentReadable.MockedDocumentVersions.Add(documentVersionReadable);
            var documentVersionQueryResult = new TaskCompletionSource<IEnumerable<DocumentVersionReadableModel>>();
            var documentVersions = new List<DocumentVersionReadableModel> { documentVersionReadable };
            documentVersionQueryResult.SetResult(documentVersions);
            connectionControllerMock.Setup(
                c => c.DocumentVersionChangeTrackingManager.QueryAsync(It.IsAny<DocumentVersionQuery>()))
                .Returns(documentVersionQueryResult.Task);

            var configuration = new MediaConfiguration { Document = document };
            var mediaConfiguration =
                new MediaConfigurationDataViewModel(
                    new Helpers.MediaConfigurationReadableModelMock(configuration, documentReadable),
                    state.Shell,
                    new CommandRegistry());
            state.ExistingProjects.Add(mediaConfiguration);
        }

        private void SetupTenantAuthorizations(MediaApplicationState state)
        {
            this.tenantAuthorizations = new Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>>();
            this.tenantAuthorizations.Add(
                state.CurrentTenant,
                new List<AuthorizationReadableModel>
                    {
                        new Helpers.AuthorizationReadableModelMock(
                            new Authorization
                                {
                                    Id = 1,
                                    Permission = Permission.Read,
                                    DataScope = DataScope.MediaConfiguration
                                }),
                        new Helpers.AuthorizationReadableModelMock(
                            new Authorization
                                {
                                    Id = 2,
                                    Permission = Permission.Write,
                                    DataScope = DataScope.MediaConfiguration
                                })
                    });
        }
    }
}
