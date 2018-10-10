// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleControllerCommandsTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the commands of the <see cref="CycleController"/>.
    /// </summary>
    [TestClass]
    public class CycleControllerCommandsTest
    {
        private UnityContainer unityContainer;

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var changeHistoryMock = new Mock<IChangeHistory>();
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>();
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            this.unityContainer = Helpers.InitializeServiceLocator();
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            applicationControllerMock.Setup(
                controller => controller.ShellController.ProjectController.OnProjectGotDirty())
                .Returns(Task.FromResult(0));
            this.unityContainer.RegisterInstance(typeof(IMediaApplicationController), applicationControllerMock.Object);
        }

        /// <summary>
        /// Cleans up this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ChangeHistoryFactory.ResetCurrent();
            ServiceLocator.SetLocatorProvider(() => null);
        }

        /// <summary>
        /// Tests the creation of a standard cycle.
        /// </summary>
        [TestMethod]
        public void CreateCycleCommandStandardTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var cycleNavigator = InitializeCycleNavigation();
            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(0, state.CurrentCyclePackage.EventCycles.Count);

            cycleNavigator.CreateNewCycle.Execute(CycleType.StandardCycle);

            Assert.AreEqual(2, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(0, state.CurrentCyclePackage.EventCycles.Count);
            Assert.AreEqual(
                "C1024x748_0",
                state.CurrentCyclePackage.StandardCycles[0].Name.Value);
            Assert.AreEqual(
                MediaStrings.CycleController_NewCycleName + "1",
                state.CurrentCyclePackage.StandardCycles[1].Name.Value);
        }

        /// <summary>
        /// Tests the creation of an event cycle.
        /// </summary>
        [TestMethod]
        public void CreateCycleCommandEventTest()
        {
            var state = Helpers.MockApplicationState(this.unityContainer);
            var cycleNavigator = InitializeCycleNavigation();
            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(0, state.CurrentCyclePackage.EventCycles.Count);

            cycleNavigator.CreateNewCycle.Execute(CycleType.EventCycle);

            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.EventCycles.Count);
            Assert.AreEqual(
                MediaStrings.CycleController_NewEventCycleName + "1",
                state.CurrentCyclePackage.EventCycles[0].Name.Value);
        }

        private static CycleNavigationViewModel InitializeCycleNavigation()
        {
            var mediaShellParams = new MediaShell.MediaShellParams
            {
                Factory = new MediaShellFactory(),
                Stages = new List<Lazy<IStage, IStageMetadata>>(),
                MenuItems = new List<Lazy<MenuItemBase, IMenuItemMetadata>>(),
                StatusBarItems = new List<Lazy<StatusBarItemBase>>()
            };
            var commandRegistry = new CommandRegistry();
            var mediaShell = new MediaShell(mediaShellParams, commandRegistry);
            // ReSharper disable UnusedVariable
            var controller = new MediaShellController(mediaShell, commandRegistry);
            // ReSharper restore UnusedVariable
            return mediaShell.CycleNavigator;
        }
    }
}
