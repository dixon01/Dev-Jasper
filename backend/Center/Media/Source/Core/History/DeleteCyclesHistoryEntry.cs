// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteCyclesHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout element.
    /// </summary>
    public class DeleteCyclesHistoryEntry : HistoryEntryBase
    {
        private readonly IEnumerable<CycleRefConfigDataViewModelBase> elements;
        private readonly CyclePackageConfigDataViewModel cyclePackage;
        private readonly MediaProjectDataViewModel project;
        private readonly CycleNavigationViewModel cycleNavigator;

        private ICommandRegistry commandRegistry;

        private bool internalDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCyclesHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        /// The list of selected elements to be deleted.
        /// </param>
        /// <param name="cyclePackage">
        /// The cycle Package.
        /// </param>
        /// <param name="project">
        /// the project
        /// </param>
        /// <param name="cycleNavigator">
        /// the cycle navigator view model
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry
        /// </param>
        /// <param name="displayText">
        /// The text to be displayed for this Entry on the UI.
        /// </param>
        /// <param name="internalDelete">
        /// The internal Delete.
        /// Is is used to be able to delete all cycles in a package while deleting a virtual display.
        /// </param>
        public DeleteCyclesHistoryEntry(
            IEnumerable<CycleRefConfigDataViewModelBase> elements,
            CyclePackageConfigDataViewModel cyclePackage,
            MediaProjectDataViewModel project,
            CycleNavigationViewModel cycleNavigator,
            ICommandRegistry commandRegistry,
            string displayText,
            bool internalDelete = false)
            : base(displayText)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (cyclePackage == null)
            {
                throw new ArgumentNullException("cyclePackage");
            }

            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (cycleNavigator == null)
            {
                throw new ArgumentNullException("cycleNavigator");
            }

            this.elements = elements;
            this.cyclePackage = cyclePackage;
            this.project = project;
            this.cycleNavigator = cycleNavigator;
            this.internalDelete = internalDelete;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            foreach (var cycle in this.elements)
            {
                cycle.MakeDirty();

                cycle.Reference.CyclePackageReferences.Remove(this.cyclePackage);
                if (cycle is StandardCycleRefConfigDataViewModel)
                {
                    if (this.cyclePackage.StandardCycles.Count > 1 || this.internalDelete)
                    {
                        this.cyclePackage.StandardCycles.Remove((StandardCycleRefConfigDataViewModel)cycle);
                    }
                    else
                    {
                        MessageBox.Show(MediaStrings.CycleController_CantDeleteLastCycle);
                        return;
                    }
                }
                else if (cycle is EventCycleRefConfigDataViewModel)
                {
                    this.cyclePackage.EventCycles.Remove((EventCycleRefConfigDataViewModel)cycle);
                }

                if (cycle.Reference.CyclePackageReferences.Count <= 0)
                {
                    foreach (var section in cycle.Reference.Sections)
                    {
                        if (section.Layout == null)
                        {
                            continue;
                        }

                        var referenceToRemove =
                            ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.FirstOrDefault(
                                r => r.CycleReference == cycle.Reference && r.SectionReference == section);
                        if (referenceToRemove != null)
                        {
                            ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences
                                .Remove(referenceToRemove);
                        }

                        section.UnsetMediaReferences(this.commandRegistry);
                    }

                    if (cycle.Reference is StandardCycleConfigDataViewModel)
                    {
                        var standardCycles = this.project.InfomediaConfig.Cycles.StandardCycles;
                        standardCycles.Remove((StandardCycleConfigDataViewModel)cycle.Reference);
                    }
                    else if (cycle.Reference is EventCycleConfigDataViewModel)
                    {
                        var eventCycles = this.project.InfomediaConfig.Cycles.EventCycles;
                        eventCycles.Remove((EventCycleConfigDataViewModel)cycle.Reference);
                    }
                }

                if (this.cycleNavigator.CurrentCycle == cycle.Reference)
                {
                    if (this.cycleNavigator.CurrentCycle is StandardCycleConfigDataViewModel)
                    {
                        this.cycleNavigator.SelectDefaultStandardCycle();
                    }
                    else if (this.cycleNavigator.CurrentCycle is EventCycleConfigDataViewModel)
                    {
                        this.cycleNavigator.SelectDefaultEventCycle();
                    }
                }

                if (this.cycleNavigator.HighlightedCycle == null
                    || this.cycleNavigator.HighlightedCycle == cycle)
                {
                    this.cycleNavigator.HighlightCurrentCycle();
                }

                if (cycle.Reference.CyclePackageReferences.Count <= 0)
                {
                    this.UnsetPredefinedFormulaReferences(cycle);
                }
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            foreach (var cycle in this.elements)
            {
                cycle.MakeDirty();

                cycle.Reference.CyclePackageReferences.Add(this.cyclePackage);
                if (cycle is StandardCycleRefConfigDataViewModel)
                {
                    this.cyclePackage.StandardCycles.Add((StandardCycleRefConfigDataViewModel)cycle);
                }
                else if (cycle is EventCycleRefConfigDataViewModel)
                {
                    this.cyclePackage.EventCycles.Add((EventCycleRefConfigDataViewModel)cycle);
                }

                if (cycle.Reference is StandardCycleConfigDataViewModel)
                {
                    var standardCycles = this.project.InfomediaConfig.Cycles.StandardCycles;
                    var standardCycle = (StandardCycleConfigDataViewModel)cycle.Reference;
                    if (!standardCycles.Contains(standardCycle))
                    {
                        standardCycles.Add(standardCycle);
                        this.ResetPredefinedFormulaReferences(cycle);
                    }
                }
                else if (cycle.Reference is EventCycleConfigDataViewModel)
                {
                    var eventCycles = this.project.InfomediaConfig.Cycles.EventCycles;
                    var eventCycle = (EventCycleConfigDataViewModel)cycle.Reference;
                    if (!eventCycles.Contains(eventCycle))
                    {
                        eventCycles.Add(eventCycle);
                        this.ResetPredefinedFormulaReferences(cycle);
                    }
                }

                foreach (var section in cycle.Reference.Sections)
                {
                    section.SetMediaReferences(this.commandRegistry);
                    if (section.Layout == null)
                    {
                        continue;
                    }

                    var referenceToAdd = new LayoutCycleSectionRefDataViewModel(cycle.Reference, section);
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Add(referenceToAdd);
                }

                this.cycleNavigator.ChooseCycle.Execute(cycle);
                this.cycleNavigator.CurrentCycle.IsItemSelected = true;
                this.cycleNavigator.HighlightedCycle = cycle;
            }
        }

        private void ResetPredefinedFormulaReferences(CycleRefConfigDataViewModelBase cycle)
        {
            var predefinedFormulas = this.GetPredefinedFormulas(cycle);
            predefinedFormulas.IncreaseReferencesCount(this.project);
        }

        private void UnsetPredefinedFormulaReferences(CycleRefConfigDataViewModelBase cycle)
        {
            var predefinedFormulas = this.GetPredefinedFormulas(cycle);
            predefinedFormulas.DecreaseReferencesCount(this.project);
        }

        private IEnumerable<EvaluationConfigDataViewModel> GetPredefinedFormulas(CycleRefConfigDataViewModelBase cycle)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (cycle.Reference != null)
            {
                result.AddRange(cycle.Reference.GetContainedPredefinedFormulas());

                foreach (var section in cycle.Reference.Sections)
                {
                    result.AddRange(section.GetContainedPredefinedFormulas());
                }
            }

            return result;
        }
    }
}
