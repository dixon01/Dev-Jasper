// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCycleHistoryEntry.cs" company="Gorba AG">
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

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a layout element.
    /// </summary>
    public class CreateCycleHistoryEntry : HistoryEntryBase
    {
        private readonly CycleRefConfigDataViewModelBase element;
        private readonly CyclePackageConfigDataViewModel parent;
        private readonly List<LayoutCycleSectionRefDataViewModel> cycleSectionReferences;
        private readonly MediaProjectDataViewModel project;
        private readonly CycleNavigationViewModel cycleNavigator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCycleHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The element data view model.
        /// </param>
        /// <param name="parent">
        /// The parent that contains the element.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="cycleNavigator">
        /// The cycle navigator
        /// </param>
        /// <param name="displayText">
        /// The display text shown on the UI.
        /// </param>
        public CreateCycleHistoryEntry(
            CycleRefConfigDataViewModelBase element,
            CyclePackageConfigDataViewModel parent,
            MediaProjectDataViewModel project,
            CycleNavigationViewModel cycleNavigator,
            string displayText)
            : base(displayText)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (cycleNavigator == null)
            {
                throw new ArgumentNullException("cycleNavigator");
            }

            this.element = element;
            this.parent = parent;
            this.project = project;
            this.cycleNavigator = cycleNavigator;
            this.cycleSectionReferences = new List<LayoutCycleSectionRefDataViewModel>();
            foreach (var section in this.element.Reference.Sections)
            {
                var cycleSectionReference = new LayoutCycleSectionRefDataViewModel(this.element.Reference, section);
                this.cycleSectionReferences.Add(cycleSectionReference);
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();

            foreach (var section in this.element.Reference.Sections)
            {
                var referenceToRemove =
                    this.cycleSectionReferences.FirstOrDefault(
                        r => r.CycleReference == this.element.Reference && section == r.SectionReference);
                if (referenceToRemove != null)
                {
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Remove(referenceToRemove);
                }
            }

            var standardCycleRef = this.element as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                this.parent.StandardCycles.Remove(standardCycleRef);
                this.project.InfomediaConfig.Cycles.StandardCycles.Remove(
                    (StandardCycleConfigDataViewModel)standardCycleRef.Reference);
            }
            else
            {
                var eventCycleRef = this.element as EventCycleRefConfigDataViewModel;
                if (eventCycleRef != null)
                {
                    this.parent.EventCycles.Remove(eventCycleRef);
                    this.project.InfomediaConfig.Cycles.EventCycles.Remove(
                        (EventCycleConfigDataViewModel)eventCycleRef.Reference);
                }
            }

            this.element.Reference.CyclePackageReferences.Remove(this.parent);

            // select remaining cycle
            if (this.cycleNavigator.CurrentCycle == this.element.Reference)
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
                || this.cycleNavigator.HighlightedCycle == this.element)
            {
                this.cycleNavigator.HighlightCurrentCycle();
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.element.MakeDirty();

            CycleRefConfigDataViewModelBase cycleRef = null;

            var standardCycleRef = this.element as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                this.parent.StandardCycles.Add(standardCycleRef);
                this.project.InfomediaConfig.Cycles.StandardCycles.Add(
                    (StandardCycleConfigDataViewModel)standardCycleRef.Reference);
                foreach (var section in this.element.Reference.Sections)
                {
                    var referenceToAdd = new LayoutCycleSectionRefDataViewModel(this.element.Reference, section);
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Add(referenceToAdd);
                }

                cycleRef = standardCycleRef;
            }
            else
            {
                var eventCycleRef = this.element as EventCycleRefConfigDataViewModel;
                if (eventCycleRef != null)
                {
                    this.parent.EventCycles.Add(eventCycleRef);
                    this.project.InfomediaConfig.Cycles.EventCycles.Add(
                        (EventCycleConfigDataViewModel)eventCycleRef.Reference);
                    foreach (var section in this.element.Reference.Sections)
                    {
                        var referenceToAdd = new LayoutCycleSectionRefDataViewModel(this.element.Reference, section);
                        ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Add(referenceToAdd);
                    }

                    cycleRef = eventCycleRef;
                }
            }

            this.element.Reference.CyclePackageReferences.Add(this.parent);

            if (cycleRef != null)
            {
                this.cycleNavigator.ChooseCycle.Execute(cycleRef);
                this.cycleNavigator.CurrentCycle.IsItemSelected = true;
                this.cycleNavigator.HighlightedCycle = cycleRef;
            }
        }
    }
}