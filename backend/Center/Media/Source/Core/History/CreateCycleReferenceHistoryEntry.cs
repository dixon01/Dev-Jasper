// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCycleReferenceHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the creation of a cycle reference.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a cycle reference.
    /// </summary>
    public class CreateCycleReferenceHistoryEntry : HistoryEntryBase
    {
        private readonly IMediaShell shell;
        private readonly CycleRefConfigDataViewModelBase reference;

        private readonly CyclePackageConfigDataViewModel cyclePackage;

        private Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel> newResolutions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCycleReferenceHistoryEntry"/> class.
        /// </summary>
        /// <param name="shell">
        /// The <see cref="IMediaShell"/>.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <param name="cyclePackage">
        /// The cycle package.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public CreateCycleReferenceHistoryEntry(
            IMediaShell shell,
            CycleRefConfigDataViewModelBase reference,
            CyclePackageConfigDataViewModel cyclePackage,
            string displayText)
            : base(displayText)
        {
            this.shell = shell;
            this.reference = reference;
            this.cyclePackage = cyclePackage;
            this.newResolutions = new Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel>();
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.newResolutions = new Dictionary<LayoutConfigDataViewModel, ResolutionConfigDataViewModel>();
            var standardCycleRef = this.reference as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                standardCycleRef.Reference.CyclePackageReferences.Add(this.cyclePackage);
                this.cyclePackage.StandardCycles.Add(standardCycleRef);
            }
            else
            {
                var eventCycleRef = this.reference as EventCycleRefConfigDataViewModel;
                if (eventCycleRef != null)
                {
                    eventCycleRef.Reference.CyclePackageReferences.Add(this.cyclePackage);
                    this.cyclePackage.EventCycles.Add(eventCycleRef);
                }
            }

            foreach (var section in this.reference.Reference.Sections)
            {
                if (section.Layout != null)
                {
                    var layout = (LayoutConfigDataViewModel)section.Layout;
                    var resolution = layout.AddCurrentResolution();
                    if (resolution != null)
                    {
                        this.newResolutions[layout] = resolution;
                    }
                }
            }

            this.shell.CycleNavigator.HighlightedCycle = this.reference;
            this.shell.CycleNavigator.ChooseCycle.Execute(this.reference);
            this.shell.CycleNavigator.SelectedNavigation = CycleNavigationSelection.Cycle;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            foreach (var resolution in this.newResolutions)
            {
                var layout = resolution.Key;
                layout.Resolutions.Remove(resolution.Value);
            }

            var standardCycleRef = this.reference as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                standardCycleRef.Reference.CyclePackageReferences.Remove(this.cyclePackage);
                this.cyclePackage.StandardCycles.Remove(standardCycleRef);
                return;
            }

            var eventCycleRef = this.reference as EventCycleRefConfigDataViewModel;
            if (eventCycleRef != null)
            {
                eventCycleRef.Reference.CyclePackageReferences.Remove(this.cyclePackage);
                this.cyclePackage.EventCycles.Remove(eventCycleRef);
            }
        }
    }
}
