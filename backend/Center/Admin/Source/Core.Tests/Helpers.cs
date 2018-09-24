// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Helper class for Admin unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Moq;

    /// <summary>
    /// Helper class for Admin unit tests.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Resets the service locator.
        /// </summary>
        public static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        /// <summary>
        /// Initializes the service locator.
        /// </summary>
        /// <returns>
        /// The <see cref="UnityContainer"/>.
        /// </returns>
        public static UnityContainer InitializeServiceLocator()
        {
            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);

            return unityContainer;
        }

        /// <summary>
        /// Creates a mock of the <see cref="IAdminApplicationState"/>.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="currentTenant">
        /// The current tenant.
        /// </param>
        /// <returns>
        /// The <see cref="Mock"/>.
        /// </returns>
        public static Mock<IAdminApplicationState> CreateApplicationStateMock(
            UnityContainer container,
            Tenant currentTenant)
        {
            var applicationState = new Mock<IAdminApplicationState>();
            container.RegisterInstance(applicationState.Object);
            container.RegisterInstance<IConnectedApplicationState>(applicationState.Object);
            if (currentTenant != null)
            {
                var tenantReadable = new TenantReadableModelMock(currentTenant);
                applicationState.Setup(state => state.CurrentTenant).Returns(tenantReadable);
            }

            return applicationState;
        }

        /// <summary>
        /// Creates a connection controller mock and assigns mocks for all change tracking managers.
        /// </summary>
        /// <returns>
        /// The <see cref="ConnectionControllerResult"/>.
        /// </returns>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here. Many managers to setup.")]
        public static ConnectionControllerResult CreateConnectionControllerMock()
        {
            const MockBehavior Behavior = MockBehavior.Strict;

            var connectionControllerMock = new Mock<IConnectionController>(Behavior);
            var documentChangeTrackingManagerMock = new Mock<IDocumentChangeTrackingManager>(Behavior);
            var userRoleChangeTrackingManagerMock = new Mock<IUserRoleChangeTrackingManager>(Behavior);
            var authorizationChangeTrackingManagerMock = new Mock<IAuthorizationChangeTrackingManager>(Behavior);
            var tenantChangeTrackingManagerMock = new Mock<ITenantChangeTrackingManager>(Behavior);
            var userChangeTrackingManagerMock = new Mock<IUserChangeTrackingManager>(Behavior);
            var associationTenantUserUserRole = new Mock<IAssociationTenantUserUserRoleChangeTrackingManager>(Behavior);
            var unitChangeTrackingManagerMock = new Mock<IUnitChangeTrackingManager>(Behavior);
            var resourceChangeTrackingManagerMock = new Mock<IResourceChangeTrackingManager>(Behavior);
            var updateGroupChangeTrackingManagerMock = new Mock<IUpdateGroupChangeTrackingManager>(Behavior);
            var updatePartChangeTrackingManagerMock = new Mock<IUpdatePartChangeTrackingManager>(Behavior);
            var updateCommandChangeTrackingManagerMock = new Mock<IUpdateCommandChangeTrackingManager>(Behavior);
            var updateFeedbackChangeTrackingManagerMock = new Mock<IUpdateFeedbackChangeTrackingManager>(Behavior);
            var productTypeChangeTrackingManagerMock = new Mock<IProductTypeChangeTrackingManager>(Behavior);
            var documentVersionChangeTrackingManagerMock = new Mock<IDocumentVersionChangeTrackingManager>(Behavior);
            var packageChangeTrackingManagerMock = new Mock<IPackageChangeTrackingManager>(Behavior);
            var packageVersionChangeTrackingManagerMock = new Mock<IPackageVersionChangeTrackingManager>(Behavior);
            var unitConfigurationChangeTrackingManagerMock =
                new Mock<IUnitConfigurationChangeTrackingManager>(Behavior);
            var mediaConfigurationChangeTrackingManagerMock =
                new Mock<IMediaConfigurationChangeTrackingManager>(Behavior);
            var userDefinedPropertyChangeTrackingManagerMock =
                new Mock<IUserDefinedPropertyChangeTrackingManager>(Behavior);
            var systemConfigChangeTrackingManagerMock = new Mock<ISystemConfigChangeTrackingManager>(Behavior);
            connectionControllerMock.Setup(controller => controller.DocumentChangeTrackingManager)
                .Returns(documentChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UserRoleChangeTrackingManager)
                .Returns(userRoleChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.AuthorizationChangeTrackingManager)
                .Returns(authorizationChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.TenantChangeTrackingManager)
                .Returns(tenantChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UserChangeTrackingManager)
                .Returns(userChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.AssociationTenantUserUserRoleChangeTrackingManager)
                .Returns(associationTenantUserUserRole.Object);
            connectionControllerMock.Setup(controller => controller.UnitChangeTrackingManager)
                .Returns(unitChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.ResourceChangeTrackingManager)
                .Returns(resourceChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UpdateGroupChangeTrackingManager)
                .Returns(updateGroupChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UpdatePartChangeTrackingManager)
                .Returns(updatePartChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UpdateCommandChangeTrackingManager)
                .Returns(updateCommandChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UpdateFeedbackChangeTrackingManager)
                .Returns(updateFeedbackChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.ProductTypeChangeTrackingManager)
                .Returns(productTypeChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.DocumentVersionChangeTrackingManager)
                .Returns(documentVersionChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.PackageChangeTrackingManager)
                .Returns(packageChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.PackageVersionChangeTrackingManager)
                .Returns(packageVersionChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UnitConfigurationChangeTrackingManager)
                .Returns(unitConfigurationChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.MediaConfigurationChangeTrackingManager)
                .Returns(mediaConfigurationChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.UserDefinedPropertyChangeTrackingManager)
                .Returns(userDefinedPropertyChangeTrackingManagerMock.Object);
            connectionControllerMock.Setup(controller => controller.SystemConfigChangeTrackingManager)
                .Returns(systemConfigChangeTrackingManagerMock.Object);
            return new ConnectionControllerResult
            {
                ConnectionControllerMock = connectionControllerMock,
                UserRoleManagerMock = userRoleChangeTrackingManagerMock,
                AuthorizationManagerMock = authorizationChangeTrackingManagerMock,
                TenantManagerMock = tenantChangeTrackingManagerMock,
                UserManagerMock = userChangeTrackingManagerMock,
                AssociationManagerMock = associationTenantUserUserRole,
                DocumentManagerMock = documentChangeTrackingManagerMock,
                DocumentVersionManagerMock = documentVersionChangeTrackingManagerMock,
                MediaConfigurationManagerMock = mediaConfigurationChangeTrackingManagerMock,
                PackageManagerMock = packageChangeTrackingManagerMock,
                PackageVersionManagerMock = packageVersionChangeTrackingManagerMock,
                ProductTypeManagerMock = productTypeChangeTrackingManagerMock,
                ResourceManagerMock = resourceChangeTrackingManagerMock,
                UnitConfigurationManagerMock = unitConfigurationChangeTrackingManagerMock,
                UnitManagerMock = unitChangeTrackingManagerMock,
                UpdateCommandManagerMock = updateCommandChangeTrackingManagerMock,
                UpdateFeedbackManagerMock = updateFeedbackChangeTrackingManagerMock,
                UpdateGroupManagerMock = updateGroupChangeTrackingManagerMock,
                UpdatePartManagerMock = updatePartChangeTrackingManagerMock,
                UserDefinedPropertyManagerMock = userDefinedPropertyChangeTrackingManagerMock,
                SystemConfigManagerMock = systemConfigChangeTrackingManagerMock
            };
        }

        /// <summary>
        /// Creates a <see cref="DataController"/> and initializes it.
        /// </summary>
        /// <param name="connectionControllerMock">
        /// The connection controller mock.
        /// </param>
        /// <returns>
        /// The <see cref="DataController"/>.
        /// </returns>
        public static DataController SetupDataController(Mock<IConnectionController> connectionControllerMock)
        {
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            dataController.Initialize(connectionControllerMock.Object);
            return dataController;
        }

        /// <summary>
        /// Initializes the channel scope factory and returns the mocked channel.
        /// </summary>
        /// <typeparam name="T">The type of the channel</typeparam>
        /// <returns>
        /// The mocked channel.
        /// </returns>
        public static Mock<T> SetupChannelScopeFactory<T>() where T : class
        {
            var mock = new Mock<T>();
            ChannelScopeFactory<T>.SetCurrent(new InstanceChannelScopeFactory<T>(mock.Object));
            return mock;
        }

        /// <summary>
        /// The connection controller result.
        /// </summary>
        public class ConnectionControllerResult
        {
            /// <summary>
            /// Gets or sets the connection controller mock.
            /// </summary>
            public Mock<IConnectionController> ConnectionControllerMock { get; set; }

            /// <summary>
            /// Gets or sets the document manager mock.
            /// </summary>
            public Mock<IDocumentChangeTrackingManager> DocumentManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the user role manager mock.
            /// </summary>
            public Mock<IUserRoleChangeTrackingManager> UserRoleManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the authorization manager mock.
            /// </summary>
            public Mock<IAuthorizationChangeTrackingManager> AuthorizationManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the tenant manager mock.
            /// </summary>
            public Mock<ITenantChangeTrackingManager> TenantManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the user manager mock.
            /// </summary>
            public Mock<IUserChangeTrackingManager> UserManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the association manager mock.
            /// </summary>
            public Mock<IAssociationTenantUserUserRoleChangeTrackingManager> AssociationManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the unit manager mock.
            /// </summary>
            public Mock<IUnitChangeTrackingManager> UnitManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the resource manager mock.
            /// </summary>
            public Mock<IResourceChangeTrackingManager> ResourceManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the update group manager mock.
            /// </summary>
            public Mock<IUpdateGroupChangeTrackingManager> UpdateGroupManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the update part manager mock.
            /// </summary>
            public Mock<IUpdatePartChangeTrackingManager> UpdatePartManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the update command manager mock.
            /// </summary>
            public Mock<IUpdateCommandChangeTrackingManager> UpdateCommandManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the update feedback manager mock.
            /// </summary>
            public Mock<IUpdateFeedbackChangeTrackingManager> UpdateFeedbackManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the product type manager mock.
            /// </summary>
            public Mock<IProductTypeChangeTrackingManager> ProductTypeManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the document version manager mock.
            /// </summary>
            public Mock<IDocumentVersionChangeTrackingManager> DocumentVersionManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the package manager mock.
            /// </summary>
            public Mock<IPackageChangeTrackingManager> PackageManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the package version manager mock.
            /// </summary>
            public Mock<IPackageVersionChangeTrackingManager> PackageVersionManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the unit configuration manager mock.
            /// </summary>
            public Mock<IUnitConfigurationChangeTrackingManager> UnitConfigurationManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the media configuration manager mock.
            /// </summary>
            public Mock<IMediaConfigurationChangeTrackingManager> MediaConfigurationManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the user defined property manager mock.
            /// </summary>
            public Mock<IUserDefinedPropertyChangeTrackingManager> UserDefinedPropertyManagerMock { get; set; }

            /// <summary>
            /// Gets or sets the system config manager mock.
            /// </summary>
            public Mock<ISystemConfigChangeTrackingManager> SystemConfigManagerMock { get; set; }
        }

        /// <summary>
        /// The tenant readable model mock.
        /// </summary>
        public class TenantReadableModelMock : TenantReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TenantReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public TenantReadableModelMock(Tenant entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        /// <summary>
        /// The document readable model mock.
        /// </summary>
        public sealed class DocumentReadableModelMock : DocumentReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public DocumentReadableModelMock(Document entity)
                : base(entity)
            {
                this.Populate();
                this.Tenant = new TenantReadableModelMock(entity.Tenant);
            }

            /// <summary>
            /// The load reference properties async.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task LoadReferencePropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads navigation properties (references and collections).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadNavigationPropertiesAsync()
            {
                if (this.Document.Versions != null)
                {
                    foreach (var version in this.Document.Versions)
                    {
                        this.versions.Add(new DocumentVersionReadableModelMock(version));
                    }
                }

                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads XML properties (usually with large contents).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// The unit configuration readable model mock.
        /// </summary>
        public sealed class UnitConfigurationReadableModelMock : UnitConfigurationReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UnitConfigurationReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public UnitConfigurationReadableModelMock(UnitConfiguration entity)
                : base(entity)
            {
                this.Populate();
                this.Document = new DocumentReadableModelMock(entity.Document);
                this.ProductType = new ProductTypeReadableModelMock(entity.ProductType);
            }

            /// <summary>
            /// The load reference properties async.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task LoadReferencePropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads navigation properties (references and collections).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadNavigationPropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads XML properties (usually with large contents).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// The product type readable model mock.
        /// </summary>
        public class ProductTypeReadableModelMock : ProductTypeReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProductTypeReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public ProductTypeReadableModelMock(ProductType entity)
                : base(entity)
            {
                this.Populate();
            }

            /// <summary>
            /// The load reference properties async.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public override Task LoadReferencePropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads navigation properties (references and collections).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadNavigationPropertiesAsync()
            {
                return Task.FromResult(0);
            }

            /// <summary>
            /// Loads XML properties (usually with large contents).
            /// </summary>
            /// <returns>A <see cref="Task"/> that can be awaited.</returns>
            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// The user readable model mock.
        /// </summary>
        public class UserReadableModelMock : UserReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UserReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public UserReadableModelMock(User entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        /// <summary>
        /// The document version readable model mock.
        /// </summary>
        public sealed class DocumentVersionReadableModelMock : DocumentVersionReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentVersionReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public DocumentVersionReadableModelMock(DocumentVersion entity)
                : base(entity)
            {
                this.Populate();
                this.CreatingUser = new UserReadableModelMock(entity.CreatingUser);
                this.Document = new DocumentReadableModelMock(entity.Document);
            }
        }

        /// <summary>
        /// The update group readable model mock.
        /// </summary>
        public sealed class UpdateGroupReadableModelMock : UpdateGroupReadableModel
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateGroupReadableModelMock"/> class.
            /// </summary>
            /// <param name="entity">
            /// The entity.
            /// </param>
            public UpdateGroupReadableModelMock(UpdateGroup entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private class InstanceChannelScopeFactory<T> : ChannelScopeFactory<T>
            where T : class
        {
            private readonly T instance;

            public InstanceChannelScopeFactory(T instance)
            {
                this.instance = instance;
            }

            public override ChannelScope<T> Create(UserCredentials userCredentials)
            {
                return new InstanceChannelScope(this.instance);
            }

            private class InstanceChannelScope : ChannelScope<T>
            {
                public InstanceChannelScope(T instance)
                    : base(instance)
                {
                }

                protected override void Dispose(bool isDisposing)
                {
                    // Nothing to dispose
                }
            }
        }
    }
}
