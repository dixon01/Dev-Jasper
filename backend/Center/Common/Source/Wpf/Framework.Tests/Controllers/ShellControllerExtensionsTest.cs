// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellControllerExtensionsTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShellControllerExtensionsTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Tests.Controllers
{
    using System;
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines test methods for <see cref="ShellControllerExtensions"/>.
    /// </summary>
    [TestClass]
    public class ShellControllerExtensionsTest
    {
        private readonly Mock<IServiceLocator> serviceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var immediateDispatcher = new ImmediateDispatcher();
            this.serviceLocatorMock.Setup(locator => locator.GetInstance<IDispatcher>()).Returns(immediateDispatcher);
            ServiceLocator.SetLocatorProvider(() => this.serviceLocatorMock.Object);
        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var failingServiceLocatorMock = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => failingServiceLocatorMock.Object);
        }

        /// <summary>
        /// Tests the <see cref="ShellControllerExtensions.Notify"/> method.
        /// </summary>
        [TestMethod]
        public void NotifyTest()
        {
            var notifications = new ObservableCollection<Notification>();
            var shellMock = new Mock<IShellViewModel>();
            shellMock.SetupGet(model => model.Notifications).Returns(notifications);
            var shellController = new Mock<IShellController>();
            shellController.SetupGet(controller => controller.Shell).Returns(shellMock.Object);
            Notification messageNotification = new MessageNotification { Message = "My message" };
            shellController.Object.Notify(messageNotification);
            Assert.AreEqual(1, notifications.Count);
            var containsNotification = notifications.Contains(messageNotification);
            Assert.IsTrue(containsNotification);
        }

        /// <summary>
        /// Tests the <see cref="ShellControllerExtensions.Notify"/> method.
        /// </summary>
        [TestMethod]
        public void NotifyProgressTest()
        {
            var progress = new ProgressNotification();
            var notifications = new ObservableCollection<Notification>();
            var shellMock = new Mock<IShellViewModel>();
            shellMock.SetupGet(model => model.Notifications).Returns(notifications);
            shellMock.SetupGet(model => model.OverallProgress).Returns(progress);
            var shellController = new Mock<IShellController>();
            shellController.SetupGet(controller => controller.Shell).Returns(shellMock.Object);
            var progressNotification = new ProgressNotification { ActivityId = 1, Progress = 0.2 };
            shellController.Object.Notify(progressNotification);
            Assert.AreEqual(1, notifications.Count);
            var containsNotification = notifications.Contains(progressNotification);
            Assert.IsTrue(containsNotification);
            Assert.AreEqual(0.2, progress.Progress);
            Assert.IsFalse(progress.IsCompleted);
        }

        /// <summary>
        /// Tests the <see cref="ShellControllerExtensions.Notify"/> method when a <see cref="ProgressNotification"/>
        /// is sent and there was already a completed <see cref="ProgressNotification"/>.
        /// </summary>
        [TestMethod]
        public void NotifyAggregateProgressTest()
        {
            var progress = new ProgressNotification { IsCompleted = true };
            var notifications = new ObservableCollection<Notification>();
            var completedProgressNotification = new ProgressNotification { IsCompleted = true, Progress = 1 };
            notifications.Add(completedProgressNotification);
            var shellMock = new Mock<IShellViewModel>();
            shellMock.SetupGet(model => model.Notifications).Returns(notifications);
            shellMock.SetupGet(model => model.OverallProgress).Returns(progress);
            var shellController = new Mock<IShellController>();
            shellController.SetupGet(controller => controller.Shell).Returns(shellMock.Object);
            var progressNotification = new ProgressNotification { ActivityId = 1, Progress = 0.2 };
            shellController.Object.Notify(progressNotification);
            Assert.AreEqual(2, notifications.Count);
            var containsNotification = notifications.Contains(progressNotification);
            Assert.IsTrue(containsNotification);
            Assert.AreEqual(0.2, progress.Progress);
            Assert.IsFalse(progress.IsCompleted);
            progressNotification = new ProgressNotification { ActivityId = 1, Progress = 0.6 };
            shellController.Object.Notify(progressNotification);
            Assert.AreEqual(2, notifications.Count);
            Assert.AreEqual(0.6, progress.Progress);
            Assert.IsFalse(progress.IsCompleted);
        }

        /// <summary>
        /// Tests the <see cref="ShellControllerExtensions.Notify"/> method when a <see cref="ProgressNotification"/>
        /// is sent and there was already a completed <see cref="ProgressNotification"/>.
        /// </summary>
        [TestMethod]
        public void NotifyAggregateCompletedProgressTest()
        {
            var progress = new ProgressNotification { IsCompleted = true };
            var notifications = new ObservableCollection<Notification>();
            var completedProgressNotification = new ProgressNotification { IsCompleted = true, Progress = 1 };
            notifications.Add(completedProgressNotification);
            var shellMock = new Mock<IShellViewModel>();
            shellMock.SetupGet(model => model.Notifications).Returns(notifications);
            shellMock.SetupGet(model => model.OverallProgress).Returns(progress);
            var shellController = new Mock<IShellController>();
            shellController.SetupGet(controller => controller.Shell).Returns(shellMock.Object);
            var progressNotification = new ProgressNotification { ActivityId = 1, Progress = 0.2 };
            shellController.Object.Notify(progressNotification);
            Assert.AreEqual(2, notifications.Count);
            var containsNotification = notifications.Contains(progressNotification);
            Assert.IsTrue(containsNotification);
            Assert.AreEqual(0.2, progress.Progress);
            Assert.IsFalse(progress.IsCompleted);
            progressNotification = new ProgressNotification { ActivityId = 1, IsCompleted = true, Progress = 1 };
            shellController.Object.Notify(progressNotification);
            Assert.AreEqual(2, notifications.Count);
            Assert.AreEqual(1, progress.Progress);
            Assert.IsTrue(progress.IsCompleted);
        }

        private class ImmediateDispatcher : IDispatcher
        {
            public void Dispatch(Action actionToInvoke)
            {
                actionToInvoke();
            }
        }
    }
}