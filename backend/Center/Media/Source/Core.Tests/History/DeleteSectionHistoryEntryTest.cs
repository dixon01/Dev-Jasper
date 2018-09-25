// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSectionHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="DeleteSectionsHistoryEntry" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="DeleteSectionsHistoryEntry"/>
    /// </summary>
    [TestClass]
    public class DeleteSectionHistoryEntryTest
    {
        /// <summary>
        /// Tests do/undo deleting an image section, verifies that the media resource count is updated and
        /// the cycle section reference is removed from the layout.
        /// </summary>
        [TestMethod]
        public void DeleteImageSectionTest()
        {
            var container = Helpers.InitializeServiceLocator();
            SetResourceManagerMock(container);
            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(
                new ResourceInfoDataViewModel { Hash = "ImageHash", Filename = "Image.png" });
            var cycleConfigDataViewModel = new StandardCycleConfigDataViewModel(state.Shell);
            var imageSection = new ImageSectionConfigDataViewModel(state.Shell)
            {
                Filename = { Value = "Image.png" },
                Layout = state.CurrentLayout
            };
            cycleConfigDataViewModel.Sections.Add(imageSection);
            ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Add(
                new LayoutCycleSectionRefDataViewModel(cycleConfigDataViewModel, imageSection));
            state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Add(cycleConfigDataViewModel);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));
            var cycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object);

            var historyEntry = new DeleteSectionsHistoryEntry(
                new List<SectionConfigDataViewModelBase> { imageSection },
                cycleConfigDataViewModel,
                state.CurrentProject,
                cycleNavigator,
                commandRegistryMock.Object,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(1, ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Count);
            var cycleSectionReference = ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.First();
            Assert.AreNotEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreNotEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            Assert.AreNotEqual(imageSection, cycleNavigator.HighlightedSection);
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
            historyEntry.Undo();
            Assert.AreEqual(2, ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Count);
            cycleSectionReference = ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(cycleConfigDataViewModel, cycleSectionReference.CycleReference);
            Assert.AreNotEqual(imageSection, cycleNavigator.CurrentSection);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
        }

        /// <summary>
        /// Tests do/undo deleting an image section which is set as current section, verifies that the media resource
        /// count is updated, the cycle section reference is removed from the layout
        /// and the current selected section changed.
        /// </summary>
        [TestMethod]
        public void DeleteImageSectionCurrentSelectedTest()
        {
            var container = Helpers.InitializeServiceLocator();
            SetResourceManagerMock(container);
            var state = Helpers.MockApplicationState(container);
            state.CurrentProject.Resources.Add(
                new ResourceInfoDataViewModel { Hash = "ImageHash", Filename = "Image.png" });
            var imageSection = new ImageSectionConfigDataViewModel(state.Shell)
            {
                Filename = { Value = "Image.png" },
                Layout = state.CurrentLayout
            };
            state.CurrentCycle.Sections.Add(imageSection);
            ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Add(
                new LayoutCycleSectionRefDataViewModel(state.CurrentCycle, imageSection));
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(It.IsAny<SelectResourceParameters>()));
            var cycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object)
                                     {
                                         CurrentCycle
                                             =
                                             state.CurrentCycle,
                                         CurrentSection
                                             =
                                             imageSection
                                     };

            var historyEntry = new DeleteSectionsHistoryEntry(
                new List<SectionConfigDataViewModelBase> { imageSection },
                state.CurrentCycle,
                state.CurrentProject,
                cycleNavigator,
                commandRegistryMock.Object,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(1, ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Count);
            var cycleSectionReference = ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.First();
            Assert.AreNotEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(state.CurrentCycle, cycleSectionReference.CycleReference);
            Assert.AreNotEqual(imageSection, cycleNavigator.CurrentSection);
            commandRegistryMock.Verify(CreateSelectResourceExpression(null, "ImageHash"), Times.Once());
            historyEntry.Undo();
            Assert.AreEqual(2, ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Count);
            cycleSectionReference = ((LayoutConfigDataViewModel)state.CurrentLayout).CycleSectionReferences.Last();
            Assert.AreEqual(imageSection, cycleSectionReference.SectionReference);
            Assert.AreEqual(state.CurrentCycle, cycleSectionReference.CycleReference);
            Assert.AreEqual(imageSection, cycleNavigator.CurrentSection);
            commandRegistryMock.Verify(CreateSelectResourceExpression("ImageHash", null), Times.Once());
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

        private static void SetResourceManagerMock(IUnityContainer container)
        {
            var mock = new Mock<IResourceManager>();
            var imageSectionManager = new ImageSectionReferenceManager();
            mock.SetupGet(manager => manager.ImageSectionManager).Returns(imageSectionManager);
            var videoSectionManager = new VideoSectionReferenceManager();
            mock.SetupGet(manager => manager.VideoSectionManager).Returns(videoSectionManager);
            var poolSectionManager = new PoolReferenceManager();
            mock.SetupGet(manager => manager.PoolManager).Returns(poolSectionManager);
            container.RegisterInstance(mock.Object);
        }
    }
}
