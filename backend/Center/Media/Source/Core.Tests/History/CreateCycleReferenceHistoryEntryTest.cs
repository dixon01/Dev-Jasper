// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCycleReferenceHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="CreateCycleReferenceHistoryEntry" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="CreateCycleReferenceHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class CreateCycleReferenceHistoryEntryTest
    {
        /// <summary>
        /// Tests do/undo of the creation of a cycle reference on a cycle package that belongs to a virtual display
        /// with the same resolution.
        /// </summary>
        [TestMethod]
        public void CreateCycleReferenceOnDifferentCyclePackageTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                    .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()));
            state.Shell.CycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object);
            var newCyclePackage = new CyclePackageConfigDataViewModel(state.Shell)
                                      {
                                          Name =
                                              {
                                                  Value =
                                                      "NewCyclePackage"
                                              }
                                      };
            state.CurrentProject.InfomediaConfig.CyclePackages.Add(newCyclePackage);
            var newCycleReference = new StandardCycleRefConfigDataViewModel(state.Shell)
                                        {
                                            Reference = state.CurrentCycle
                                        };
            var historyEntry = new CreateCycleReferenceHistoryEntry(
                state.Shell,
                newCycleReference,
                newCyclePackage,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(2, state.CurrentCycle.CyclePackageReferences.Count);
            Assert.AreEqual(newCycleReference, newCyclePackage.StandardCycles.First());
            var layout = (LayoutConfigDataViewModel)newCycleReference.Reference.Sections.First().Layout;
            commandRegistryMock.Verify(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                    .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()),
                Times.Once());
            Assert.AreEqual(1, layout.Resolutions.Count);
            historyEntry.Undo();
            Assert.AreEqual(1, state.CurrentCycle.CyclePackageReferences.Count);
            Assert.AreEqual(0, newCyclePackage.StandardCycles.Count);
            commandRegistryMock.Verify(
               c =>
               c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                   .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()),
               Times.Once());
        }

        /// <summary>
        /// Tests do/undo of the creation of a cycle reference in a cycle package which belongs to a virtual display
        /// with a different resolution and so the layout assigned to the underlying section should add that resolution.
        /// </summary>
        [TestMethod]
        public void CreateCycleReferenceOnDifferentResolutionTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                    .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()));
            state.Shell.CycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object);
            Helpers.AddPhysicalScreenToProject(state.Shell, 200, 100);
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.CyclePackages.Count);
            var cycle = state.CurrentProject.InfomediaConfig.CyclePackages.Last().StandardCycles.First();
            var newCycleReference = new StandardCycleRefConfigDataViewModel(state.Shell)
            {
                Reference = cycle.Reference
            };
            var historyEntry = new CreateCycleReferenceHistoryEntry(
                state.Shell,
                newCycleReference,
                state.CurrentCyclePackage,
                string.Empty);
            historyEntry.Do();
            Assert.AreEqual(2, cycle.Reference.CyclePackageReferences.Count);
            Assert.AreEqual(2, state.CurrentCyclePackage.StandardCycles.Count);
            var layout = (LayoutConfigDataViewModel)cycle.Reference.Sections.First().Layout;
            Assert.AreEqual(2, layout.Resolutions.Count);
            commandRegistryMock.Verify(
               c =>
               c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                   .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()),
               Times.Once());
            historyEntry.Undo();
            Assert.AreEqual(1, cycle.Reference.CyclePackageReferences.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            layout = (LayoutConfigDataViewModel)cycle.Reference.Sections.First().Layout;
            Assert.AreEqual(1, layout.Resolutions.Count);
            commandRegistryMock.Verify(
               c =>
               c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                   .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()),
               Times.Once());
        }
    }
}
