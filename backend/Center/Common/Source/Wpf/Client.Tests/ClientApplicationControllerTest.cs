// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientApplicationControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="ClientApplicationControllerBase" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="ClientApplicationControllerBase"/>.
    /// </summary>
    [TestClass]
    public class ClientApplicationControllerTest
    {
        /// <summary>
        /// Initializes a test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ResetServiceLocator();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ResetServiceLocator();
        }

        /// <summary>
        /// Tests changing the password.
        /// </summary>
        [TestMethod]
        public void ChangePasswordTest()
        {
            // TODO: figure out how we can test this again
            var container = InitializeServiceLocator();
            var state = new ConnectedApplicationState();
            container.RegisterInstance(typeof(IConnectedApplicationState), state);
            var commandRegistry = new CommandRegistry();
            var applicationController = new TestClientApplicationController(commandRegistry, "TestApp");
            var connectionControllerMock = new Mock<IConnectionController>();
            var user = new User { HashedPassword = "newPasswordHashed", Id = 10 };
            connectionControllerMock.Setup(c => c.ChangePasswordAsync("newPassword"))
                .Returns(Task.FromResult(user));
            applicationController.SetConnectionControllerMock(connectionControllerMock);
            Assert.IsNull(state.CurrentUser);
            commandRegistry.GetCommand(ClientCommandCompositionKeys.ChangePassword).Execute("newPassword");
            Assert.AreEqual(user, state.CurrentUser);
            connectionControllerMock.Verify(c => c.ChangePasswordAsync("newPassword"), Times.Once());
        }

        /// <summary>
        /// Tests the logout of the <see cref="ClientApplicationControllerBase"/>.
        /// </summary>
        [TestMethod]
        public void LogoutTest()
        {
            var container = InitializeServiceLocator();
            var state = new ConnectedApplicationState();
            container.RegisterInstance(typeof(IConnectedApplicationState), state);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            state.CurrentUser = new User { Id = 10 };
            state.CurrentTenant = new TenantReadableModel(new Tenant { Name = "TestTenant" });
            var applicationController = new TestClientApplicationController(commandRegistryMock.Object, "TestApp");
            var connectionControllerMock = new Mock<IConnectionController>();
            applicationController.SetConnectionControllerMock(connectionControllerMock);
            applicationController.Logout();

            // Workaround: Resharper would say that the code after the next assert is heuristic unreachable if we
            // use Assert.IsNull.
            Assert.AreEqual(null, state.CurrentUser);
            Assert.AreEqual(null, state.CurrentTenant);
        }

        /// <summary>
        /// The reset service locator.
        /// </summary>
        private static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        /// <summary>
        /// The initialize service locator.
        /// </summary>
        /// <returns>
        /// The <see cref="UnityContainer"/>.
        /// </returns>
        private static UnityContainer InitializeServiceLocator()
        {
            var unityContainer = new UnityContainer();
            var unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);

            return unityContainer;
        }

        private class TestClientApplicationController : ClientApplicationControllerBase
        {
            public TestClientApplicationController(ICommandRegistry commandRegistry, string applicationTitle)
                : base(commandRegistry, applicationTitle, DataScope.CenterAdmin)
            {
            }

            protected override ImageSource ApplicationIcon
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override DataScope[] AllowedDataScopes
            {
                get
                {
                    return new[] { DataScope.MediaConfiguration };
                }
            }

            public void SetConnectionControllerMock(Mock<IConnectionController> controllerMock)
            {
                this.ConnectionController = controllerMock.Object;
            }

            public override void Run()
            {
                throw new NotImplementedException();
            }
        }
    }
}
