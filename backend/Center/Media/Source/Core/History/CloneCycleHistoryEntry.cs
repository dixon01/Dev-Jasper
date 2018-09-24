// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloneCycleHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
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
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout element.
    /// </summary>
    public class CloneCycleHistoryEntry : HistoryEntryBase
    {
        private readonly CycleRefConfigDataViewModelBase element;
        private readonly CyclePackageConfigDataViewModel cyclePackage;
        private readonly MediaProjectDataViewModel project;
        private readonly IMediaShell shell;
        private readonly CycleNavigationViewModel cycleNavigator;

        private CycleRefConfigDataViewModelBase clonedReference;

        private Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel> newResolutions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloneCycleHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        ///     The list of selected element to be cloned.
        /// </param>
        /// <param name="cyclePackage">
        ///     The cycle Package.
        /// </param>
        /// <param name="project">the project</param>
        /// <param name="shell">the shell</param>
        /// <param name="cycleNavigator">the cycle navigator view model</param>
        /// <param name="displayText">
        ///     The text to be displayed for this Entry on the UI.
        /// </param>
        public CloneCycleHistoryEntry(
            CycleRefConfigDataViewModelBase element,
            CyclePackageConfigDataViewModel cyclePackage,
            MediaProjectDataViewModel project,
            IMediaShell shell,
            CycleNavigationViewModel cycleNavigator,
            string displayText)
            : base(displayText)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (cyclePackage == null)
            {
                throw new ArgumentNullException("cyclePackage");
            }

            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (shell == null)
            {
                throw new ArgumentNullException("shell");
            }

            if (cycleNavigator == null)
            {
                throw new ArgumentNullException("cycleNavigator");
            }

            this.element = element;
            this.cyclePackage = cyclePackage;
            this.project = project;
            this.shell = shell;
            this.cycleNavigator = cycleNavigator;
            this.newResolutions = new Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel>();
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.element.MakeDirty();
            this.newResolutions = new Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel>();

            var clone = (CycleConfigDataViewModelBase)this.element.Reference.Clone();
            clone.Name.Value = string.Format(
                "{0} {1}",
                MediaStrings.CycleController_CloneNamePrefix,
                clone.Name.Value);
            clone.DisplayText = clone.Name.Value;

            if (clone is StandardCycleConfigDataViewModel)
            {
                var newReference = new StandardCycleRefConfigDataViewModel(this.shell)
                {
                    Reference = clone,
                    IsInEditMode = true,
                };

                this.clonedReference = newReference;
                this.cyclePackage.StandardCycles.Add(newReference);
                this.project.InfomediaConfig.Cycles.StandardCycles.Add(
                    (StandardCycleConfigDataViewModel)newReference.Reference);
            }
            else
            {
                var newReference = new EventCycleRefConfigDataViewModel(this.shell)
                {
                    Reference = clone,
                    IsInEditMode = true,
                };

                this.clonedReference = newReference;

                this.cyclePackage.EventCycles.Add(newReference);
                this.project.InfomediaConfig.Cycles.EventCycles.Add(
                    (EventCycleConfigDataViewModel)newReference.Reference);
            }

            this.clonedReference.Reference.CyclePackageReferences.Add(this.cyclePackage);
            foreach (var section in this.clonedReference.Reference.Sections)
            {
                if (section.Layout != null)
                {
                    var layout = (LayoutConfigDataViewModel)section.Layout;
                    var resolution = layout.AddCurrentResolution();
                    if (resolution != null)
                    {
                        this.newResolutions[layout] = resolution;
                    }

                    var referenceToAdd =
                        new LayoutCycleSectionRefDataViewModel(this.clonedReference.Reference, section);
                    layout.CycleSectionReferences.Add(referenceToAdd);
                }
            }

            this.cycleNavigator.HighlightedCycle = this.clonedReference;
            this.cycleNavigator.ChooseCycle.Execute(this.clonedReference);
            this.cycleNavigator.SelectedNavigation = CycleNavigationSelection.Cycle;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();

            foreach (var section in this.clonedReference.Reference.Sections)
            {
                var referenceToRemove =
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.FirstOrDefault(
                        r => r.CycleReference == this.clonedReference.Reference && section == r.SectionReference);
                if (referenceToRemove != null)
                {
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Remove(referenceToRemove);
                }
            }

            if (this.clonedReference is StandardCycleRefConfigDataViewModel)
            {
                this.cyclePackage.StandardCycles.Remove((StandardCycleRefConfigDataViewModel)this.clonedReference);
                this.project.InfomediaConfig.Cycles.StandardCycles.Remove(
                    (StandardCycleConfigDataViewModel)this.clonedReference.Reference);
            }
            else
            {
                this.cyclePackage.EventCycles.Remove((EventCycleRefConfigDataViewModel)this.clonedReference);
                this.project.InfomediaConfig.Cycles.EventCycles.Remove(
                    (EventCycleConfigDataViewModel)this.clonedReference.Reference);
            }

            if (this.cycleNavigator.CurrentCycle == this.clonedReference.Reference)
            {
                var standardCycleRef = this.cycleNavigator.CurrentCyclePackage.StandardCycles.FirstOrDefault();
                this.cycleNavigator.CurrentCycle = standardCycleRef != null ? standardCycleRef.Reference : null;
            }

            foreach (var resolution in this.newResolutions)
            {
                var layout = resolution.Key;
                layout.Resolutions.Remove(resolution.Value);
            }
        }
    }
}
