// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateSectionHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="CreateSectionHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class CreateSectionHistoryEntryTest
    {
        /// <summary>
        /// Tests do/undo creating an image section and verifies that the media resource count is updated.
        /// </summary>
        [TestMethod]
        public void CreateImageSectionTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var container = Helpers.InitializeServiceLocator();

            Helpers.CreateMediaConfiguration(
                container,
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(
                new ResourceInfoDataViewModel { Hash = "ImageHash", Filename = "Image.png" });
            container.RegisterInstance(typeof(IMediaApplicationState), state);

            var mediaProjectDataViewModel = new MediaProjectDataViewModel { IsCheckedIn = true };
            shellMock.Setup(shell => shell.MediaApplicationState.CurrentProject).Returns(mediaProjectDataViewModel);

            var cycleConfigDataViewModel = new StandardCycleConfigDataViewModel(shellMock.Object);
            var imageSection = new ImageSectionConfigDataViewModel(shellMock.Object)
                                   {
                                       Filename = { Value = "Image.png" },
                                       Layout = state.CurrentLayout
                                   };
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));

            var historyEntry = new CreateSectionHistoryEntry(
                imageSection,
                cycleConfigDataViewModel,
                commandRegistryMock.Object,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(1, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(imageSection, cycleConfigDataViewModel.Sections.First());
            Assert.AreEqual(2, ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Count);
            var cycleSectionReference = ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Never());
            historyEntry.Undo();
            Assert.AreEqual(0, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(1, ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Count);
            cycleSectionReference = ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Last();
            Assert.AreNotEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreNotEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
            historyEntry.Do();
            Assert.AreEqual(1, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(2, ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Count);
            cycleSectionReference = ((LayoutConfigDataViewModel)imageSection.Layout).CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Exactly(2));
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
        }

        /// <summary>
        /// Tests the do/undo of creating an image section where the assigned layout doesn't contain the resolution
        /// of the current virtual display.
        /// </summary>
        [TestMethod]
        public void CreateImageSectionWithLayoutNotHavingResolutionTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(
                new ResourceInfoDataViewModel { Hash = "ImageHash", Filename = "Image.png" });
            var cycleConfigDataViewModel = new StandardCycleConfigDataViewModel(state.Shell);
            var layout = new LayoutConfigDataViewModel(state.Shell) { Name = { Value = "ImageSectionLayout" } };
            var imageSection = new ImageSectionConfigDataViewModel(state.Shell)
            {
                Filename = { Value = "Image.png" },
                Layout = layout
            };
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));

            Assert.AreEqual(0, layout.Resolutions.Count);
            var historyEntry = new CreateSectionHistoryEntry(
                imageSection,
                cycleConfigDataViewModel,
                commandRegistryMock.Object,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(1, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(imageSection, cycleConfigDataViewModel.Sections.First());
            Assert.AreEqual(1, layout.Resolutions.Count);
            var resolution = layout.Resolutions.First();
            Assert.AreEqual(state.CurrentVirtualDisplay.Height.Value, resolution.Height.Value);
            Assert.AreEqual(state.CurrentVirtualDisplay.Width.Value, resolution.Width.Value);
            Assert.AreEqual(1, layout.CycleSectionReferences.Count);
            var cycleSectionReference = layout.CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Never());
            historyEntry.Undo();
            Assert.AreEqual(0, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(0, layout.CycleSectionReferences.Count);
            Assert.AreEqual(0, layout.Resolutions.Count);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
            historyEntry.Do();
            Assert.AreEqual(1, cycleConfigDataViewModel.Sections.Count);
            Assert.AreEqual(1, layout.Resolutions.Count);
            resolution = layout.Resolutions.First();
            Assert.AreEqual(state.CurrentVirtualDisplay.Height.Value, resolution.Height.Value);
            Assert.AreEqual(state.CurrentVirtualDisplay.Width.Value, resolution.Width.Value);
            Assert.AreEqual(1, layout.CycleSectionReferences.Count);
            cycleSectionReference = layout.CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Exactly(2));
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
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
