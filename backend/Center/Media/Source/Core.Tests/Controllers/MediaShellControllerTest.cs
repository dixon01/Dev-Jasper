// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaShellControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaShellControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the <see cref="MediaShellController"/>;
    /// </summary>
    [TestClass]
    public class MediaShellControllerTest
    {
        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ResetServiceLocator();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            InteractionManager<EditMenuPrompt>.ResetCurrent();
            ChangeHistoryFactory.ResetCurrent();
            ResetServiceLocator();
        }

        /// <summary>
        /// Verifies the behavior of the constructor (registering all commands, creating sub controllers).
        /// Plain strings are used to ensure that any change in references is verified by developers.
        /// Strict mock behavior is used to force developers to add their commands to this test.
        /// </summary>
        [TestMethod]
        public void ConstructorTest()
        {
            var commandKeys = GetCommandKeys();
            commandKeys.Add("Framework.Options.Save");
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create())
                                    .Returns(new ChangeHistory())
                                    .Verifiable("ChangeHistory not created");
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var mediaShellMock = new Mock<IMediaShell>(MockBehavior.Strict);
            var commandRegistryMock = new Mock<ICommandRegistry>(MockBehavior.Strict);

            foreach (var commandKey in commandKeys)
            {
                SetupCommand(commandRegistryMock, commandKey);
            }

            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistryMock.Object);

            foreach (var commandKey in commandKeys)
            {
                VerifyCommand(commandRegistryMock, commandKey);
            }

            Assert.IsNotNull(mediaShellController.ExportController);
            Assert.IsNotNull(mediaShellController.PostUndoController);
            Assert.IsNotNull(mediaShellController.TftEditorController);
            Assert.IsNotNull(mediaShellController.LedEditorController);
            Assert.IsNotNull(mediaShellController.AudioEditorController);
            Assert.IsNotNull(mediaShellController.MainMenuPrompt);
            changeHistoryFactoryMock.Verify(
                factory => factory.Create(), Times.Once(), "ChangeHistory not created exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the Undo command.
        /// </summary>
        [TestMethod]
        public void UndoCommandTest()
        {
            InitApplicationStateMock();

            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Strict);
            changeHistoryMock.Setup(history => history.Undo()).Verifiable("Undo not executed");
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create())
                                    .Returns(changeHistoryMock.Object)
                                    .Verifiable("ChangeHistory not created");
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            mediaShellMock.Setup(shell => shell.MediaApplicationState.IsDirty).Returns(true);
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Default.Undo");
            command.Execute(It.IsAny<object>());
            changeHistoryMock.Verify(history => history.Undo(), Times.Once(), "Undo not executed exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the Redo command.
        /// </summary>
        [TestMethod]
        public void RedoCommandTest()
        {
            InitApplicationStateMock();

            var changeHistoryMock = new Mock<IChangeHistory>(MockBehavior.Strict);
            changeHistoryMock.Setup(history => history.Redo()).Verifiable("Redo not executed");
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>(MockBehavior.Strict);
            changeHistoryFactoryMock.Setup(factory => factory.Create())
                                    .Returns(changeHistoryMock.Object)
                                    .Verifiable("ChangeHistory not created");
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Default.Redo");
            command.Execute(It.IsAny<object>());
            changeHistoryMock.Verify(history => history.Redo(), Times.Once(), "Redo not executed exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the ShowEditMenu command.
        /// </summary>
        [TestMethod]
        public void ShowEditMenuCommandTest()
        {
            var interactionManagerMock = new Mock<InteractionManager<EditMenuPrompt>>(MockBehavior.Strict);
            interactionManagerMock.Setup(manager => manager.Raise(It.IsAny<EditMenuPrompt>(), null))
                                  .Verifiable("Edit Menu Prompt not sent");
            InteractionManager<EditMenuPrompt>.SetCurrent(interactionManagerMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.UI.ShowEditMenu");
            command.Execute(It.IsAny<object>());
            interactionManagerMock.Verify(
                manager => manager.Raise(It.IsAny<EditMenuPrompt>(), null),
                Times.Once(),
                "Edit Menu Prompt not sent exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the ShowMainMenu command.
        /// </summary>
        [TestMethod]
        public void ShowMainMenuCommandTest()
        {
            var interactionManagerMock = new Mock<InteractionManager<MainMenuPrompt>>(MockBehavior.Strict);
            interactionManagerMock.Setup(manager => manager.Raise(It.IsAny<MainMenuPrompt>(), null))
                                  .Verifiable("Main Menu Prompt not sent");
            InteractionManager<MainMenuPrompt>.SetCurrent(interactionManagerMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.UI.ShowMainMenu");
            command.Execute(It.IsAny<object>());
            interactionManagerMock.Verify(
                manager => manager.Raise(It.IsAny<MainMenuPrompt>(), null),
                Times.Once(),
                "Main Menu Prompt not sent exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the ShowLayoutNavigation command.
        /// </summary>
        [TestMethod]
        public void ShowLayoutNavigationCommandTest()
        {
            var interactionManagerMock = new Mock<InteractionManager<LayoutNavigationPrompt>>(MockBehavior.Strict);
            interactionManagerMock.Setup(manager => manager.Raise(It.IsAny<LayoutNavigationPrompt>(), null))
                                  .Verifiable("Layout Navigation Prompt not sent");
            InteractionManager<LayoutNavigationPrompt>.SetCurrent(interactionManagerMock.Object);
            var commandRegistry = new CommandRegistry();
            var mediaShellMock = new Mock<IMediaShell>();
            var mediaApplicationStateMock = new Mock<IMediaApplicationState>();
            mediaApplicationStateMock.Setup(state => state.CurrentProject.InfomediaConfig)
                                     .Returns(
                                         new InfomediaConfigDataViewModel(
                                             mediaShellMock.Object));
            mediaShellMock.Setup(m => m.MediaApplicationState).Returns(mediaApplicationStateMock.Object);

            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.UI.ShowLayoutNavigation");
            command.Execute(It.IsAny<object>());
            interactionManagerMock.Verify(
                manager => manager.Raise(It.IsAny<LayoutNavigationPrompt>(), null),
                Times.Once(),
                "Layout Navigation Prompt not sent exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the ShowResolutionNavigation command.
        /// </summary>
        [TestMethod]
        public void ShowResolutionNavigationCommandTest()
        {
            var interactionManagerMock = new Mock<InteractionManager<ResolutionNavigationPrompt>>(MockBehavior.Strict);
            interactionManagerMock.Setup(manager => manager.Raise(It.IsAny<ResolutionNavigationPrompt>(), null))
                                  .Verifiable("Resolution Navigation Prompt not sent");
            InteractionManager<ResolutionNavigationPrompt>.SetCurrent(interactionManagerMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var mediaApplicationStateMock = new Mock<IMediaApplicationState>();
            mediaApplicationStateMock.Setup(state => state.CurrentProject.InfomediaConfig)
                                     .Returns(
                                         new InfomediaConfigDataViewModel(
                                             mediaShellMock.Object));
            mediaApplicationStateMock.Setup(state => state.CurrentPhysicalScreen)
                                     .Returns(new PhysicalScreenConfigDataViewModel(mediaShellMock.Object));
            mediaShellMock.Setup(m => m.MediaApplicationState).Returns(mediaApplicationStateMock.Object);
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.UI.ShowResolutionNavigation");
            command.Execute(It.IsAny<object>());
            interactionManagerMock.Verify(
                manager => manager.Raise(It.IsAny<ResolutionNavigationPrompt>(), null),
                Times.Once(),
                "Resolution Navigation Prompt not sent exactly once");
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the Play command.
        /// </summary>
        [TestMethod]
        public void PlayPreviewCommandTest()
        {
            var dispatcherMock = new Mock<IDispatcher>();
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(dispatcherMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.Preview.Play");
            command.Execute(It.IsAny<object>());
        }

        /// <summary>
        /// Verifies the &quot;binding&quot; of the PausePreview command.
        /// </summary>
        [TestMethod]
        public void PausePreviewCommandTest()
        {
            var dispatcherMock = new Mock<IDispatcher>();
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(dispatcherMock.Object);
            var mediaShellMock = new Mock<IMediaShell>();
            var commandRegistry = new CommandRegistry();
            // ReSharper disable UnusedVariable
            var mediaShellController = new MediaShellController(mediaShellMock.Object, commandRegistry);
            // ReSharper restore UnusedVariable
            var command = commandRegistry.GetCommand("Media.Shell.Preview.Pause");
            command.Execute(It.IsAny<object>());
        }

        /// <summary>
        /// Tests the navigation to a layout of a different physical screen.
        /// </summary>
        [TestMethod]
        public void NavigateToLayoutOnDifferentScreenTest()
        {
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>() as UnityContainer;
            var state = Helpers.MockApplicationState(unityContainer);
            Helpers.AddPhysicalScreenToProject(state.Shell, 1000, 100);
            Assert.AreEqual(state.CurrentLayout, state.CurrentProject.InfomediaConfig.Layouts.First());
            Assert.AreEqual(state.CurrentPhysicalScreen, state.CurrentProject.InfomediaConfig.PhysicalScreens.First());
            Assert.AreEqual(state.CurrentVirtualDisplay, state.CurrentProject.InfomediaConfig.VirtualDisplays.First());
            Assert.AreEqual(state.CurrentCyclePackage, state.CurrentProject.InfomediaConfig.CyclePackages.First());
            Assert.AreEqual(state.CurrentCycle, state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.First());

            var layout = state.CurrentProject.InfomediaConfig.Layouts.Last();
            var command = state.Shell.CommandRegistry.GetCommand("Media.Shell.Layout.NavigateTo");
            command.Execute(layout);
            Assert.AreEqual(state.CurrentLayout, layout);
            Assert.AreEqual(state.CurrentPhysicalScreen, state.CurrentProject.InfomediaConfig.PhysicalScreens.Last());
            Assert.AreEqual(state.CurrentVirtualDisplay, state.CurrentProject.InfomediaConfig.VirtualDisplays.Last());
            Assert.AreEqual(state.CurrentCyclePackage, state.CurrentProject.InfomediaConfig.CyclePackages.Last());
            Assert.AreEqual(state.CurrentCycle, state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Last());
        }

        private static void VerifyCommand(Mock<ICommandRegistry> commandRegistryMock, string command)
        {
            commandRegistryMock.Verify(
                registry => registry.RegisterCommand(command, It.IsAny<ICommand>()),
                Times.AtMostOnce(),
                "The command '" + command + "' should be registered once");
        }

        private static void SetupCommand(Mock<ICommandRegistry> commandRegistryMock, string command)
        {
            commandRegistryMock.Setup(
                registry => registry.RegisterCommand(command, It.IsAny<ICommand>()))
                               .Verifiable("The command '" + command + "' should be registered once");
        }

        private static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        private static List<string> GetCommandKeys(Type keyContainer = null)
        {
            var result = new List<string>();

            if (keyContainer == null)
            {
                keyContainer = typeof(CommandCompositionKeys);
            }

            var fields = keyContainer.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var fieldInfo in fields)
            {
                var key = fieldInfo.GetValue(null) as string;
                if (key != null)
                {
                    result.Add(key);
                }
            }

            var nestedTypes = keyContainer.GetNestedTypes(BindingFlags.Public);

            foreach (var nestedKeyContainer in nestedTypes)
            {
                var nestedKeys = GetCommandKeys(nestedKeyContainer);
                result.AddRange(nestedKeys);
            }

            return result;
        }

        private static IMediaApplicationState InitApplicationStateMock()
        {
            var applicationStateMock = new Mock<IMediaApplicationState>();
            applicationStateMock.Setup(state => state.CurrentProject).Returns(() => new MediaProjectDataViewModel());
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(applicationStateMock.Object);

            return applicationStateMock.Object;
        }
    }
}