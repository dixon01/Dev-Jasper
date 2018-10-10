// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteCyclesHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="DeleteCyclesHistoryEntry" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="DeleteCyclesHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class DeleteCyclesHistoryEntryTest
    {
        /// <summary>
        /// Tests do/undo of deletion of a cycle that has multiple references, so only the reference should be deleted.
        /// </summary>
        [TestMethod]
        public void DeleteCycleWithMultipleReferencesTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                    .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()));
            state.Shell.CycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object);

            // ReSharper disable once UnusedVariable
            var resourceManagerMock = Helpers.CreateResourceManagerMock(container);
            Helpers.AddPhysicalScreenToProject(state.Shell, 1024, 748);
            state.CurrentCycle.CyclePackageReferences.Add(state.CurrentProject.InfomediaConfig.CyclePackages.Last());
            var cycleReference = new StandardCycleRefConfigDataViewModel(state.Shell)
                                     {
                                         Reference =
                                             state.CurrentCycle
                                     };
            state.CurrentProject.InfomediaConfig.CyclePackages.Last().StandardCycles.Add(cycleReference);
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Count);
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.CyclePackages.Last().StandardCycles.Count);
            Assert.AreEqual(2, state.CurrentCycle.CyclePackageReferences.Count);

            var cyclesToDelete = new List<CycleRefConfigDataViewModelBase>
                                     {
                                         cycleReference
                                     };
            var historyEntry = new DeleteCyclesHistoryEntry(
                cyclesToDelete,
                state.CurrentProject.InfomediaConfig.CyclePackages.Last(),
                state.CurrentProject,
                state.Shell.CycleNavigator,
                commandRegistryMock.Object,
                string.Empty);

            historyEntry.Do();
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(1, state.CurrentProject.InfomediaConfig.CyclePackages.Last().StandardCycles.Count);
            Assert.AreEqual(1, state.CurrentCycle.CyclePackageReferences.Count);

            historyEntry.Undo();
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.StandardCycles.Count);
            Assert.AreEqual(2, state.CurrentProject.InfomediaConfig.CyclePackages.Last().StandardCycles.Count);
            Assert.AreEqual(2, state.CurrentCycle.CyclePackageReferences.Count);
        }

        /// <summary>
        /// Test do/undo of a cycle that has only one reference and so the reference and cycle should be deleted.
        /// </summary>
        [TestMethod]
        public void DeleteCycleWithOneReferenceTest()
        {
            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var commandRegistryMock = new Mock<ICommandRegistry>();
            commandRegistryMock.Setup(
                c =>
                c.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose)
                    .Execute(It.IsAny<CycleRefConfigDataViewModelBase>()));
            state.Shell.CycleNavigator = new CycleNavigationViewModel(state.Shell, commandRegistryMock.Object);

            // ReSharper disable once UnusedVariable
            var resourceManagerMock = Helpers.CreateResourceManagerMock(container);
            var layout = new LayoutConfigDataViewModel(state.Shell) { Name = { Value = "EventLayout" } };
            var section = new StandardSectionConfigDataViewModel(state.Shell)
                              {
                                  Layout = layout
                              };
            var eventCycle = new EventCycleConfigDataViewModel(state.Shell)
                                 {
                                     Sections =
                                         new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                                             {
                                                 section
                                             }
                                 };
            layout.CycleSectionReferences.Add(new LayoutCycleSectionRefDataViewModel(eventCycle, section));
            var eventCycleRef = new EventCycleRefConfigDataViewModel(state.Shell) { Reference = eventCycle };
            state.CurrentCyclePackage.EventCycles.Add(eventCycleRef);
            eventCycle.CyclePackageReferences.Add(state.CurrentCyclePackage);
            state.CurrentProject.InfomediaConfig.Cycles.EventCycles.Add(eventCycle);
            Assert.AreEqual(1, state.CurrentProject.InfomediaConfig.Cycles.EventCycles.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.EventCycles.Count);
            Assert.AreEqual(1, state.CurrentCycle.CyclePackageReferences.Count);
            var cyclesToDelete = new List<CycleRefConfigDataViewModelBase>
                                     {
                                         eventCycleRef
                                     };
            var historyEntry = new DeleteCyclesHistoryEntry(
                cyclesToDelete,
                state.CurrentCyclePackage,
                state.CurrentProject,
                state.Shell.CycleNavigator,
                commandRegistryMock.Object,
                string.Empty);

            historyEntry.Do();
            Assert.AreEqual(0, state.CurrentProject.InfomediaConfig.Cycles.EventCycles.Count);
            Assert.AreEqual(0, state.CurrentCyclePackage.EventCycles.Count);
            Assert.AreEqual(1, state.CurrentCycle.CyclePackageReferences.Count);
            Assert.AreEqual(0, layout.CycleSectionReferences.Count);

            historyEntry.Undo();
            Assert.AreEqual(1, state.CurrentProject.InfomediaConfig.Cycles.EventCycles.Count);
            Assert.AreEqual(1, state.CurrentCyclePackage.EventCycles.Count);
            Assert.AreEqual(1, layout.CycleSectionReferences.Count);
        }
    }
}
