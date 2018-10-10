// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletePhysicalScreenHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the deletion of physical screens.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of physical screens.
    /// </summary>
    public class DeletePhysicalScreenHistoryEntry : HistoryEntryBase
    {
        private readonly IEnumerable<PhysicalScreenRefConfigDataViewModel> elements;

        private readonly IMediaApplicationState state;

        private readonly Dictionary<PhysicalScreenRefConfigDataViewModel, List<DeleteCyclesHistoryEntry>>
            cyclesHistoryEntries;

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePhysicalScreenHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="state">
        /// The application state.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public DeletePhysicalScreenHistoryEntry(
            IEnumerable<PhysicalScreenRefConfigDataViewModel> elements,
            IMediaApplicationState state,
            ICommandRegistry commandRegistry,
            string displayText)
            : base(displayText)
        {
            this.elements = elements;
            this.state = state;
            this.commandRegistry = commandRegistry;
            this.cyclesHistoryEntries =
                new Dictionary<PhysicalScreenRefConfigDataViewModel, List<DeleteCyclesHistoryEntry>>();
            foreach (var screenRef in this.elements)
            {
                this.cyclesHistoryEntries[screenRef] = new List<DeleteCyclesHistoryEntry>();
                foreach (var display in screenRef.VirtualDisplays)
                {
                    var cycles =
                        display.Reference.CyclePackage.StandardCycles.Cast<CycleRefConfigDataViewModelBase>().ToList();
                    cycles.AddRange(display.Reference.CyclePackage.EventCycles);

                    var deleteCycleHistoryEntry = new DeleteCyclesHistoryEntry(
                        cycles,
                        display.Reference.CyclePackage,
                        this.state.CurrentProject,
                        display.Shell.CycleNavigator,
                        this.commandRegistry,
                        string.Empty,
                        true);
                    this.cyclesHistoryEntries[screenRef].Add(deleteCycleHistoryEntry);
                }
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            foreach (var screenRef in this.elements)
            {
                var deleteCycles = this.cyclesHistoryEntries[screenRef];
                deleteCycles.ForEach(d => d.Do());

                foreach (var display in screenRef.VirtualDisplays)
                {
                    this.state.CurrentProject.InfomediaConfig.CyclePackages.Remove(display.Reference.CyclePackage);
                    this.state.CurrentProject.InfomediaConfig.VirtualDisplays.Remove(display.Reference);
                }

                this.state.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                    .PhysicalScreens.Remove(screenRef);
                if (this.state.CurrentPhysicalScreen == screenRef.Reference)
                {
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Choose)
                        .Execute(
                            this.state.CurrentProject.InfomediaConfig.PhysicalScreens.FirstOrDefault(
                                screen => !object.ReferenceEquals(screen, screenRef.Reference)));
                }

                this.state.CurrentProject.InfomediaConfig.PhysicalScreens.Remove(screenRef.Reference);
            }

            this.UnsetPredefinedFormulaReferences();
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            foreach (var screenRef in this.elements)
            {
                var deleteCycles = this.cyclesHistoryEntries[screenRef];
                deleteCycles.ForEach(d => d.Undo());

                foreach (var display in screenRef.VirtualDisplays)
                {
                    this.state.CurrentProject.InfomediaConfig.CyclePackages.Add(display.Reference.CyclePackage);
                    this.state.CurrentProject.InfomediaConfig.VirtualDisplays.Add(display.Reference);
                }

                this.state.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                    .PhysicalScreens.Add(screenRef);
                this.state.CurrentProject.InfomediaConfig.PhysicalScreens.Add(screenRef.Reference);
            }

            this.SetPredefinedFormulaReferences();
        }

        private void UnsetPredefinedFormulaReferences()
        {
            foreach (var element in this.elements)
            {
                var predefinedFormulas = element.Reference.GetContainedPredefinedFormulas();
                predefinedFormulas.DecreaseReferencesCount(this.state.CurrentProject);
            }
        }

        private void SetPredefinedFormulaReferences()
        {
            foreach (var element in this.elements)
            {
                var predefinedFormulas = element.Reference.GetContainedPredefinedFormulas();
                predefinedFormulas.IncreaseReferencesCount(this.state.CurrentProject);
            }
        }
    }
}
