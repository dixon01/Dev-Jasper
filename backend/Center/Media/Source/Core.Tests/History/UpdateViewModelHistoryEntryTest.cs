// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateViewModelHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="UpdateViewModelHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class UpdateViewModelHistoryEntryTest
    {
        private int hookCalls;

        /// <summary>
        /// Cleanup test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            UpdateViewModelHistoryEntry.ClearPostActionHooks();
        }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            UpdateViewModelHistoryEntry.ClearPostActionHooks();
        }

        /// <summary>
        /// Tests that a registered hook is only called for a specific element type
        /// </summary>
        [TestMethod]
        public void UpdateViewModelCallingPostHooksTest()
        {
            this.hookCalls = 0;
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(StaticTextElementDataViewModel),
                this.OnAfterUpdateStaticTextElement);
            var container = Helpers.InitializeServiceLocator();
            SetupResourceManagerMock(container);
            Helpers.MockApplicationState(container);
            var shellMock = new Mock<IMediaShell>();
            var currentElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Static" } };
            var elementsContainer = new ObservableCollection<GraphicalElementDataViewModelBase> { currentElement };
            var newElement = (StaticTextElementDataViewModel)currentElement.Clone();
            var oldElement = (StaticTextElementDataViewModel)currentElement.Clone();
            var oldElements = new List<GraphicalElementDataViewModelBase> { oldElement };
            var newElements = new List<GraphicalElementDataViewModelBase> { newElement };
            newElement.Value.Value = "UpdatedStatic";

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(1, this.hookCalls);
            var doElement = elementsContainer[0] as StaticTextElementDataViewModel;
            Assert.IsNotNull(doElement);
            Assert.AreEqual("UpdatedStatic", doElement.Value.Value);
            historyEntry.Undo();
            Assert.AreEqual(2, this.hookCalls);
            doElement = elementsContainer[0] as StaticTextElementDataViewModel;
            Assert.IsNotNull(doElement);
            Assert.AreEqual("Static", doElement.Value.Value);
            var secondElement = new ImageElementDataViewModel(shellMock.Object) { ElementName = { Value = "Image" } };
            var secondNewElement = (ImageElementDataViewModel)secondElement.Clone();
            var secondOldElement = (ImageElementDataViewModel)secondElement.Clone();
            secondNewElement.ElementName.Value = "UpdatedImage";
            oldElements.Clear();
            newElements.Clear();
            elementsContainer.Add(secondElement);
            oldElements.Add(secondOldElement);
            newElements.Add(secondNewElement);
            var secondHistoryEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                string.Empty);
            secondHistoryEntry.Do();
            Assert.AreEqual(2, this.hookCalls);
            var secondDoElement = elementsContainer[1] as ImageElementDataViewModel;
            Assert.IsNotNull(secondDoElement);
            Assert.AreEqual("UpdatedImage", secondDoElement.ElementName.Value);
        }

        /// <summary>
        /// Tests that the callbacks are called if they are passed to the <see cref="UpdateViewModelHistoryEntry"/>.
        /// </summary>
        [TestMethod]
        public void UpdateViewModelWithCallbacksTest()
        {
            var callbackCount = 0;
            Action doCallback = () => { callbackCount++; };
            Action undoCallback = () => { callbackCount--; };
            var container = Helpers.InitializeServiceLocator();
            SetupResourceManagerMock(container);
            Helpers.MockApplicationState(container);
            var shellMock = new Mock<IMediaShell>();
            var currentElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Static" } };
            var elementsContainer = new ObservableCollection<GraphicalElementDataViewModelBase> { currentElement };
            var newElement = (StaticTextElementDataViewModel)currentElement.Clone();
            var oldElement = (StaticTextElementDataViewModel)currentElement.Clone();
            var oldElements = new List<GraphicalElementDataViewModelBase> { oldElement };
            var newElements = new List<GraphicalElementDataViewModelBase> { newElement };
            newElement.Value.Value = "UpdatedStatic";
            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                doCallback,
                undoCallback,
                string.Empty);
            Assert.AreEqual(0, callbackCount);
            historyEntry.Do();
            Assert.AreEqual(1, callbackCount);
            var updatedElement = elementsContainer[0] as StaticTextElementDataViewModel;
            Assert.IsNotNull(updatedElement);
            Assert.AreEqual("UpdatedStatic", updatedElement.Value.Value);
            historyEntry.Undo();
            Assert.AreEqual(0, callbackCount);
            Assert.AreEqual("Static", updatedElement.Value.Value);
        }

        /// <summary>
        /// Tests the UpdateViewModel Do() and Undo() with changed dynamic values with and without a formula set.
        /// </summary>
        [TestMethod]
        public void UpdateViewModelWithDynamicValueTest()
        {
            var container = Helpers.InitializeServiceLocator();
            SetupResourceManagerMock(container);
            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(new ResourceInfoDataViewModel { Filename = "Image.png" });
            state.CurrentProject.Resources.Add(new ResourceInfoDataViewModel { Filename = "UpdatedImage.png" });
            var shellMock = new Mock<IMediaShell>();
            var oldVisibleFormula = new ConstantEvalDataViewModel(shellMock.Object) { Value = { Value = "Constant" } };
            var newVisibleFormula = new DayOfWeekEvalDataViewModel(shellMock.Object) { Monday = { Value = true } };
            var currentElement = new ImageElementDataViewModel(shellMock.Object)
                                     {
                                         Filename =
                                             {
                                                 Value =
                                                     "UpdatedImage.png"
                                             },
                                         Visible =
                                             {
                                                 Formula =
                                                     newVisibleFormula
                                             }
                                     };
            var elementsContainer = new ObservableCollection<GraphicalElementDataViewModelBase> { currentElement };
            var newElement = (ImageElementDataViewModel)currentElement.Clone();
            var oldElement = (ImageElementDataViewModel)currentElement.Clone();

            oldElement.Visible.Formula = oldVisibleFormula;
            oldElement.Filename.Value = "Image.png";
            var oldElements = new List<GraphicalElementDataViewModelBase> { oldElement };
            var newElements = new List<GraphicalElementDataViewModelBase> { newElement };

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                string.Empty);
            Assert.AreEqual(
                ((EvalDataViewModelBase)newElement.Visible.Formula).HumanReadable(),
                ((EvalDataViewModelBase)currentElement.Visible.Formula).HumanReadable());
            historyEntry.Undo();
            var doElement = elementsContainer[0] as ImageElementDataViewModel;
            Assert.IsNotNull(doElement);
            Assert.AreEqual(oldElement.Filename.Value, doElement.Filename.Value);
            Assert.AreEqual(
                ((EvalDataViewModelBase)oldElement.Visible.Formula).HumanReadable(),
                ((EvalDataViewModelBase)doElement.Visible.Formula).HumanReadable());
            historyEntry.Do();
            doElement = elementsContainer[0] as ImageElementDataViewModel;
            Assert.IsNotNull(doElement);
            Assert.AreEqual(newElement.Filename.Value, doElement.Filename.Value);
            Assert.AreEqual(
                ((EvalDataViewModelBase)newElement.Visible.Formula).HumanReadable(),
                ((EvalDataViewModelBase)doElement.Visible.Formula).HumanReadable());
        }

        private static void SetupResourceManagerMock(UnityContainer container)
        {
            var resourceManagerMock = new Mock<IResourceManager>();
            resourceManagerMock.SetupGet(manager => manager.AnalogClockElementManager)
                .Returns(new Mock<AnalogClockElementReferenceManager>().Object);
            resourceManagerMock.SetupGet(manager => manager.AudioElementManager)
                .Returns(new Mock<AudioElementReferenceManager>().Object);
            resourceManagerMock.SetupGet(manager => manager.TextElementManager)
                .Returns(new Mock<TextElementReferenceManager>().Object);
            resourceManagerMock.SetupGet(manager => manager.ImageElementManager)
                .Returns(new Mock<ImageElementReferenceManager>().Object);
            container.RegisterInstance(resourceManagerMock.Object);
        }

        private void OnAfterUpdateStaticTextElement(DataViewModelBase arg1, DataViewModelBase arg2)
        {
            this.hookCalls++;
        }
    }
}