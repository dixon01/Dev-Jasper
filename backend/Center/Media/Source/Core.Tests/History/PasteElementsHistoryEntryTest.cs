// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasteElementsHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="PasteElementsHistoryEntry" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="PasteElementsHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class PasteElementsHistoryEntryTest
    {
        /// <summary>
        /// Tests do/undo of pasting a single element without references.
        /// </summary>
        [TestMethod]
        public void PasteSingleElementWithoutReferencesTest()
        {
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            Helpers.CreateResourceManagerMock(container);
            var shellMock = new Mock<IMediaShell>();
            var layoutEditor = new TftEditorViewModel();
            var newElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Static" } };
            var existingElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Text1" } };
            layoutEditor.Elements.Add(existingElement);
            var newElements = new List<LayoutElementDataViewModelBase> { newElement };
            var historyEntry = new PasteElementsHistoryEntry(
                newElements,
                layoutEditor,
                commandRegistryMock.Object,
                state,
                string.Empty);
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            historyEntry.InitialDo();
            Assert.AreEqual(2, layoutEditor.Elements.Count);
            var pastedElement = layoutEditor.Elements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(pastedElement);
            Assert.AreEqual("Static", pastedElement.Value.Value);
            historyEntry.Undo();
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            var previousElement = layoutEditor.Elements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(previousElement);
            Assert.AreEqual("Text1", previousElement.Value.Value);
            commandRegistryMock.Verify(c => c.GetCommand(CommandCompositionKeys.Project.SelectResource), Times.Never());
        }

        /// <summary>
        /// Tests do/undo of pasting a single audio element with references.
        /// </summary>
        [TestMethod]
        public void PasteSingleAudioElementWithReferencesTest()
        {
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var verifyIncreaseResourceCount =
                CreateSelectResourceExpression("AudioFileHash", null);
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(
                new ResourceInfoDataViewModel { Filename = "0001.mp3", Hash = "AudioFileHash" });
            var resourceManagerMock = Helpers.CreateResourceManagerMock(container);
            var audioreferenceManager = new AudioElementReferenceManager();
            resourceManagerMock.Setup(r => r.AudioElementManager).Returns(audioreferenceManager);
            var shellMock = new Mock<IMediaShell>();
            var layoutEditor = new AudioEditorViewModel();
            layoutEditor.CurrentAudioOutputElement = new AudioOutputElementDataViewModel(shellMock.Object);
            var newElement = new AudioFileElementDataViewModel(shellMock.Object) { Filename = { Value = "0001.mp3" } };
            var existingElement = new AudioPauseElementDataViewModel(shellMock.Object)
                                      {
                                          Duration = { Value = TimeSpan.FromMinutes(1) }
                                      };
            layoutEditor.CurrentAudioOutputElement.Elements.Add(existingElement);
            var newElements = new List<LayoutElementDataViewModelBase> { newElement };
            var historyEntry = new PasteElementsHistoryEntry(
                newElements,
                layoutEditor,
                commandRegistryMock.Object,
                state,
                string.Empty);
            Assert.AreEqual(1, layoutEditor.CurrentAudioOutputElement.Elements.Count);
            historyEntry.InitialDo();
            Assert.AreEqual(2, layoutEditor.CurrentAudioOutputElement.Elements.Count);
            commandRegistryMock.Verify(verifyIncreaseResourceCount, Times.Once());
            var pastedElement =
                layoutEditor.CurrentAudioOutputElement.Elements.Last() as AudioFileElementDataViewModel;
            Assert.IsNotNull(pastedElement);
            Assert.AreEqual("0001.mp3", pastedElement.Filename.Value);
            historyEntry.Undo();
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "AudioFileHash"), Times.Once());
            Assert.AreEqual(1, layoutEditor.CurrentAudioOutputElement.Elements.Count);
            var previousElement =
                layoutEditor.CurrentAudioOutputElement.Elements.First() as AudioPauseElementDataViewModel;
            Assert.IsNotNull(previousElement);
            Assert.AreEqual(TimeSpan.FromMinutes(1), previousElement.Duration.Value);
            historyEntry.Do();
            commandRegistryMock.Verify(verifyIncreaseResourceCount, Times.Exactly(2));
        }

        /// <summary>
        /// Tests do/undo of pasting a single image element and verifies that the image reference counts are updated.
        /// </summary>
        [TestMethod]
        public void PasteSingleElementWithMediaReferencesTest()
        {
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var verifyIncreaseResourceCount =
                CreateSelectResourceExpression("ImageHash", null);
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var resourceInfo = new ResourceInfoDataViewModel { Filename = "Image.png", Hash = "ImageHash" };
            state.CurrentProject.Resources.Add(resourceInfo);
            var resourceManagerMock = Helpers.CreateResourceManagerMock(container);
            var imageReferenceManager = new ImageElementReferenceManager();
            resourceManagerMock.Setup(r => r.ImageElementManager).Returns(imageReferenceManager);
            var shellMock = new Mock<IMediaShell>();
            var layoutEditor = new TftEditorViewModel();
            var newElement = new ImageElementDataViewModel(shellMock.Object) { Filename = { Value = "Image.png" } };
            var existingElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Text1" } };
            layoutEditor.Elements.Add(existingElement);
            var newElements = new List<LayoutElementDataViewModelBase> { newElement };
            var historyEntry = new PasteElementsHistoryEntry(
                newElements,
                layoutEditor,
                commandRegistryMock.Object,
                state,
                string.Empty);
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            historyEntry.InitialDo();
            Assert.AreEqual(2, layoutEditor.Elements.Count);
            commandRegistryMock.Verify(verifyIncreaseResourceCount, Times.Once());
            var pastedElement = layoutEditor.Elements.First() as ImageElementDataViewModel;
            Assert.IsNotNull(pastedElement);
            Assert.AreEqual("Image.png", pastedElement.Filename.Value);
            Assert.IsTrue(resourceInfo.IsUsedVisible);
            historyEntry.Undo();
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            var previousElement = layoutEditor.Elements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(previousElement);
            Assert.AreEqual("Text1", previousElement.Value.Value);
            Assert.IsFalse(resourceInfo.IsUsedVisible);
            historyEntry.Do();
            commandRegistryMock.Verify(verifyIncreaseResourceCount, Times.Exactly(2));
        }

        /// <summary>
        /// Tests do/undo of pasting multiple elements with references
        /// and verifies that all reference counts are updated.
        /// </summary>
        [TestMethod]
        public void PasteMultipleElementsWithReferencesTest()
        {
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var verifyIncreaseImageResourceCount = CreateSelectResourceExpression("ImageHash", null);
            var verifyIncreaseVideoResourceCount = CreateSelectResourceExpression("VideoHash", null);
            var verifyDecreaseImageResourceCount = CreateSelectResourceExpression(null, "ImageHash");
            var verifyDecreaseVideoResourceCount = CreateSelectResourceExpression(null, "VideoHash");
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var imageResource = new ResourceInfoDataViewModel
                                    {
                                        Filename = "Image.png",
                                        Hash = "ImageHash",
                                        Type = ResourceType.Image
                                    };
            var videoResource = new ResourceInfoDataViewModel
                                    {
                                        Filename = "Video.mpg",
                                        Hash = "VideoHash",
                                        Type = ResourceType.Video
                                    };
            state.CurrentProject.Resources.Add(imageResource);
            state.CurrentProject.Resources.Add(videoResource);
            var resourceManagerMock = Helpers.CreateResourceManagerMock(container);
            var imageReferenceManager = new ImageElementReferenceManager();
            var videoReferenceManager = new VideoElementReferenceManager();
            resourceManagerMock.Setup(r => r.ImageElementManager).Returns(imageReferenceManager);
            resourceManagerMock.Setup(r => r.VideoElementManager).Returns(videoReferenceManager);
            var shellMock = new Mock<IMediaShell>();
            shellMock.Setup(mock => mock.MediaApplicationState).Returns(state);
            var layoutEditor = new TftEditorViewModel();
            var newElement1 = new ImageElementDataViewModel(shellMock.Object) { Filename = { Value = "Image.png" } };
            var newElement2 = new VideoElementDataViewModel(shellMock.Object)
                                  {
                                      VideoUri = { Value = "Video.mpg" },
                                      Hash = "VideoHash"
                                  };
            var existingElement = new StaticTextElementDataViewModel(shellMock.Object) { Value = { Value = "Text1" } };
            layoutEditor.Elements.Add(existingElement);
            var newElements = new List<LayoutElementDataViewModelBase> { newElement1, newElement2 };
            var historyEntry = new PasteElementsHistoryEntry(
                newElements,
                layoutEditor,
                commandRegistryMock.Object,
                state,
                string.Empty);
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            historyEntry.InitialDo();
            Assert.AreEqual(3, layoutEditor.Elements.Count);
            commandRegistryMock.Verify(verifyIncreaseImageResourceCount, Times.Once());
            commandRegistryMock.Verify(verifyIncreaseVideoResourceCount, Times.Once());
            var firstPastedElement = layoutEditor.Elements.First() as VideoElementDataViewModel;
            var secondPastedElement = layoutEditor.Elements[1] as ImageElementDataViewModel;
            Assert.IsNotNull(firstPastedElement);
            Assert.IsNotNull(secondPastedElement);
            Assert.AreEqual("Video.mpg", firstPastedElement.VideoUri.Value);
            Assert.AreEqual("Image.png", secondPastedElement.Filename.Value);
            Assert.IsTrue(imageResource.IsUsedVisible);
            Assert.IsTrue(videoResource.IsUsedVisible);
            historyEntry.Undo();
            commandRegistryMock.Verify(verifyDecreaseImageResourceCount, Times.Once());
            commandRegistryMock.Verify(verifyDecreaseVideoResourceCount, Times.Once());
            Assert.AreEqual(1, layoutEditor.Elements.Count);
            var previousElement = layoutEditor.Elements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(previousElement);
            Assert.AreEqual("Text1", previousElement.Value.Value);
            Assert.IsFalse(imageResource.IsUsedVisible);
            Assert.IsFalse(videoResource.IsUsedVisible);
            historyEntry.Do();
            commandRegistryMock.Verify(verifyIncreaseImageResourceCount, Times.Exactly(2));
            commandRegistryMock.Verify(verifyIncreaseVideoResourceCount, Times.Exactly(2));
        }

        private static Expression<Action<ICommandRegistry>> CreateSelectResourceExpression(
            string currentHash,
            string previousHash)
        {
            Expression<Action<ICommandRegistry>> expression =
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(
                        It.Is<SelectResourceParameters>(
                            p =>
                            p.CurrentSelectedResourceHash == currentHash
                            && p.PreviousSelectedResourceHash == previousHash));
            return expression;
        }
    }
}
