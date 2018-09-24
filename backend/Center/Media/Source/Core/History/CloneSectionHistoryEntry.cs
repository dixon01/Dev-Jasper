// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloneSectionHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a layout element.
    /// </summary>
    public class CloneSectionHistoryEntry : HistoryEntryBase
    {
        private readonly SectionConfigDataViewModelBase element;
        private readonly CycleConfigDataViewModelBase cycle;
        private readonly MediaProjectDataViewModel project;
        private readonly IMediaShell shell;
        private readonly CycleNavigationViewModel cycleNavigator;
        private LayoutCycleSectionRefDataViewModel cycleSectionReference;

        private SectionConfigDataViewModelBase clonedElement;

        private ResolutionConfigDataViewModel newResolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloneSectionHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        ///     The list of selected element to be cloned.
        /// </param>
        /// <param name="cycle">
        ///     The cycle.
        /// </param>
        /// <param name="project">the project</param>
        /// <param name="shell">the shell</param>
        /// <param name="cycleNavigator">the cycle navigator view model</param>
        /// <param name="displayText">
        ///     The text to be displayed for this Entry on the UI.
        /// </param>
        public CloneSectionHistoryEntry(
            SectionConfigDataViewModelBase element,
            CycleConfigDataViewModelBase cycle,
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

            if (cycle == null)
            {
                throw new ArgumentNullException("cycle");
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
            this.cycle = cycle;
            this.project = project;
            this.shell = shell;
            this.cycleNavigator = cycleNavigator;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.element.MakeDirty();

            var clone = (SectionConfigDataViewModelBase)this.element.Clone();
            clone.Name = string.Format(
                "{0} {1}",
                MediaStrings.CycleController_CloneNamePrefix,
                clone.Name);
            clone.DisplayText = clone.Name;
            clone.IsInEditMode = true;
            this.clonedElement = clone;
            var layout = (LayoutConfigDataViewModel)clone.Layout;
            this.newResolution = layout.AddCurrentResolution();
            if (this.newResolution != null)
            {
                layout.Resolutions.Add(this.newResolution);
            }

            this.cycleSectionReference = new LayoutCycleSectionRefDataViewModel(this.cycle, clone);
            ((LayoutConfigDataViewModel)this.clonedElement.Layout).CycleSectionReferences.Add(
                this.cycleSectionReference);
            this.cycle.Sections.Add(clone);
            this.cycleNavigator.HighlightedSection = clone;
            this.cycleNavigator.ChooseSection.Execute(clone);
            this.cycleNavigator.SelectedNavigation = CycleNavigationSelection.Section;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();

            ((LayoutConfigDataViewModel)this.clonedElement.Layout).CycleSectionReferences.Remove(
                this.cycleSectionReference);
            this.cycle.Sections.Remove(this.clonedElement);

            if (this.cycleNavigator.CurrentSection == this.clonedElement)
            {
                var firstSection = this.cycleNavigator.CurrentCycle.Sections.FirstOrDefault();
                this.cycleNavigator.CurrentSection = firstSection;
            }

            if (this.newResolution != null)
            {
                ((LayoutConfigDataViewModel)this.clonedElement.Layout).Resolutions.Remove(this.newResolution);
            }
        }
    }
}
