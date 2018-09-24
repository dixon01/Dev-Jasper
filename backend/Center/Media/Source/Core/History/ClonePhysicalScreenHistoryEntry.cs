// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClonePhysicalScreenHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the cloning of a physical screen.
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
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the cloning of a physical screen.
    /// </summary>
    public class ClonePhysicalScreenHistoryEntry : HistoryEntryBase
    {
        private readonly PhysicalScreenRefConfigDataViewModel element;

        private readonly IMediaShell mediaShell;

        private readonly ICommandRegistry commandRegistry;

        private readonly MediaProjectDataViewModel project;

        private ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> clonedVirtualDisplays;

        private List<CyclePackageConfigDataViewModel> clonedCyclePackages;

        private Dictionary<LayoutConfigDataViewModel, LayoutConfigDataViewModel> clonedLayouts;

        private PhysicalScreenRefConfigDataViewModel clonedPhysicalScreenReference;

        private List<CycleRefConfigDataViewModelBase> clonedCycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonePhysicalScreenHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public ClonePhysicalScreenHistoryEntry(
            PhysicalScreenRefConfigDataViewModel element,
            MediaProjectDataViewModel project,
            IMediaShell shell,
            ICommandRegistry commandRegistry,
            string displayText)
            : base(displayText)
        {
            this.element = element;
            this.project = project;
            this.mediaShell = shell;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.clonedCyclePackages = new List<CyclePackageConfigDataViewModel>();
            this.clonedLayouts = new Dictionary<LayoutConfigDataViewModel, LayoutConfigDataViewModel>();
            this.clonedCycles = new List<CycleRefConfigDataViewModelBase>();
            var clonedPhysicalScreen = (PhysicalScreenConfigDataViewModel)this.element.Reference.Clone();
            clonedPhysicalScreen.Name.Value = string.Format(
                "{0} {1}",
                MediaStrings.CycleController_CloneNamePrefix,
                clonedPhysicalScreen.Name.Value);
            clonedPhysicalScreen.DisplayText = clonedPhysicalScreen.Name.Value;
            this.CloneVirtualDisplays();
            var newPhysicalScreenRef = new PhysicalScreenRefConfigDataViewModel(this.mediaShell)
                                           {
                                               Reference =
                                                   clonedPhysicalScreen,
                                                   VirtualDisplays = this.clonedVirtualDisplays
                                           };
            this.clonedPhysicalScreenReference = newPhysicalScreenRef;
            this.project.InfomediaConfig.PhysicalScreens.Add(clonedPhysicalScreen);
            this.project.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.Add(this.clonedPhysicalScreenReference);

            this.SetPredefinedFormulaReferences();
            this.SetMediaReferences();
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            if (this.mediaShell.MediaApplicationState.CurrentPhysicalScreen
                == this.clonedPhysicalScreenReference.Reference)
            {
                this.mediaShell.MediaApplicationState.CurrentPhysicalScreen =
                    this.project.InfomediaConfig.PhysicalScreens.FirstOrDefault(
                        p => !object.ReferenceEquals(this.clonedPhysicalScreenReference.Reference, p));
            }

            this.project.InfomediaConfig.PhysicalScreens.Remove(this.clonedPhysicalScreenReference.Reference);
            this.project.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.Remove(this.clonedPhysicalScreenReference);
            foreach (var displayReference in this.clonedVirtualDisplays)
            {
                if (this.mediaShell.MediaApplicationState.CurrentVirtualDisplay == displayReference.Reference)
                {
                    this.mediaShell.MediaApplicationState.CurrentVirtualDisplay =
                        this.project.InfomediaConfig.VirtualDisplays.FirstOrDefault(
                            display => !object.ReferenceEquals(displayReference.Reference, display));
                }

                this.project.InfomediaConfig.VirtualDisplays.Remove(displayReference.Reference);

                if (this.mediaShell.MediaApplicationState.CurrentCyclePackage
                    == displayReference.Reference.CyclePackage)
                {
                    this.mediaShell.CycleNavigator.CurrentCyclePackage =
                        this.project.InfomediaConfig.CyclePackages.FirstOrDefault(
                            c => !object.ReferenceEquals(displayReference.Reference.CyclePackage, c));
                }

                this.project.InfomediaConfig.CyclePackages.Remove(displayReference.Reference.CyclePackage);
            }

            foreach (var cyclePackage in this.clonedCyclePackages)
            {
                foreach (var cycle in cyclePackage.StandardCycles)
                {
                    if (this.mediaShell.MediaApplicationState.CurrentCycle == cycle.Reference)
                    {
                        this.mediaShell.CycleNavigator.CurrentCycle =
                            this.project.InfomediaConfig.Cycles.StandardCycles.FirstOrDefault(
                                c => !object.ReferenceEquals(cycle.Reference, c));
                    }

                    this.project.InfomediaConfig.Cycles.StandardCycles.Remove(
                        (StandardCycleConfigDataViewModel)cycle.Reference);
                }

                foreach (var cycle in cyclePackage.EventCycles)
                {
                    if (this.mediaShell.MediaApplicationState.CurrentCycle == cycle.Reference)
                    {
                        this.mediaShell.CycleNavigator.CurrentCycle =
                            this.project.InfomediaConfig.Cycles.StandardCycles.FirstOrDefault(
                                c => !object.ReferenceEquals(cycle.Reference, c));
                    }

                    this.project.InfomediaConfig.Cycles.EventCycles.Remove(
                        (EventCycleConfigDataViewModel)cycle.Reference);
                }
            }

            foreach (var layout in this.clonedLayouts.Values)
            {
                if (this.mediaShell.MediaApplicationState.CurrentLayout == layout)
                {
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                        .Execute(
                            this.mediaShell.MediaApplicationState.CurrentLayout =
                            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                                l => !object.ReferenceEquals(layout, l)));
                }

                this.project.InfomediaConfig.Layouts.Remove(layout);
            }

            this.UnsetMediaReferences();
            this.UnsetPredefinedFormulaReferences();
        }

        private void CloneVirtualDisplays()
        {
            this.clonedVirtualDisplays = new ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>();
            foreach (var display in this.element.VirtualDisplays)
            {
                var clonedDisplay = (VirtualDisplayConfigDataViewModel)display.Reference.Clone();
                clonedDisplay.Name.Value = string.Format(
                    "{0} {1}", MediaStrings.CycleController_CloneNamePrefix, clonedDisplay.Name.Value);
                var clonedCyclePackage = this.CloneCyclePackage(display.Reference);
                clonedDisplay.CyclePackage = clonedCyclePackage;
                var clonedReference = (VirtualDisplayRefConfigDataViewModel)display.Clone();
                clonedReference.Reference = clonedDisplay;
                this.clonedVirtualDisplays.Add(clonedReference);
                this.project.InfomediaConfig.VirtualDisplays.Add(clonedDisplay);
            }
        }

        private CyclePackageConfigDataViewModel CloneCyclePackage(VirtualDisplayConfigDataViewModel virtualDisplay)
        {
            var clonedPackage = (CyclePackageConfigDataViewModel)virtualDisplay.CyclePackage.Clone();
            clonedPackage.Name.Value = string.Format(
                   "{0} {1}", MediaStrings.CycleController_CloneNamePrefix, clonedPackage.Name.Value);
            foreach (var cycleref in clonedPackage.StandardCycles)
            {
                var clonedCycle = (StandardCycleConfigDataViewModel)cycleref.Reference.Clone();
                clonedCycle.Name.Value = string.Format(
                    "{0} {1}", MediaStrings.CycleController_CloneNamePrefix, clonedCycle.Name.Value);
                cycleref.Reference = clonedCycle;
                cycleref.Reference.CyclePackageReferences.Add(clonedPackage);
                this.project.InfomediaConfig.Cycles.StandardCycles.Add(clonedCycle);
                foreach (var section in clonedCycle.Sections)
                {
                    section.Layout = this.CloneLayout((LayoutConfigDataViewModel)section.Layout);
                }

                this.clonedCycles.Add(cycleref);
            }

            foreach (var cycleref in clonedPackage.EventCycles)
            {
                var clonedCycle = (EventCycleConfigDataViewModel)cycleref.Reference.Clone();
                clonedCycle.Name.Value = string.Format(
                    "{0} {1}", MediaStrings.CycleController_CloneNamePrefix, clonedCycle.Name.Value);
                cycleref.Reference = clonedCycle;
                cycleref.Reference.CyclePackageReferences.Add(clonedPackage);
                this.project.InfomediaConfig.Cycles.EventCycles.Add(clonedCycle);
                foreach (var section in clonedCycle.Sections)
                {
                    section.Layout = this.CloneLayout((LayoutConfigDataViewModel)section.Layout);
                }

                this.clonedCycles.Add(cycleref);
            }

            this.project.InfomediaConfig.CyclePackages.Add(clonedPackage);
            this.clonedCyclePackages.Add(clonedPackage);
            return clonedPackage;
        }

        private LayoutConfigDataViewModel CloneLayout(LayoutConfigDataViewModel layout)
        {
            if (this.clonedLayouts.ContainsKey(layout) && this.clonedLayouts[layout] != null)
            {
                return this.clonedLayouts[layout];
            }

            var clonedLayout = (LayoutConfigDataViewModel)layout.Clone();

            var index = 0;
            var hasUniqueName = false;
            var baseName = string.Format(
                "{0} {1}", MediaStrings.LayoutNavigationDialog_CloneNamePrefix, clonedLayout.Name.Value);
            var newName = string.Empty;
            while (!hasUniqueName)
            {
                index++;
                newName = baseName;

                if (index > 1)
                {
                    newName += " (" + index + ")";
                }

                hasUniqueName = !this.project.InfomediaConfig.Layouts.Any(l => l.Name.Value.Equals(newName));
            }

            clonedLayout.Name.Value = newName;
            clonedLayout.DisplayText = newName;
            this.project.InfomediaConfig.Layouts.Add(clonedLayout);
            this.clonedLayouts[layout] = clonedLayout;
            return clonedLayout;
        }

        private void UnsetPredefinedFormulaReferences()
        {
            foreach (var predefinedFormulas in this.clonedLayouts.Values.Select(this.GetPredefinedFormulas))
            {
                predefinedFormulas.DecreaseReferencesCount(this.mediaShell.MediaApplicationState.CurrentProject);
            }

            foreach (var predefinedFormulas in this.clonedCycles.Select(this.GetPredefinedFormulas))
            {
                predefinedFormulas.DecreaseReferencesCount(this.project);
            }
        }

        private void SetPredefinedFormulaReferences()
        {
            foreach (var predefinedFormulas in this.clonedLayouts.Values.Select(this.GetPredefinedFormulas))
            {
                predefinedFormulas.IncreaseReferencesCount(this.mediaShell.MediaApplicationState.CurrentProject);
            }

            foreach (var predefinedFormulas in this.clonedCycles.Select(this.GetPredefinedFormulas))
            {
                predefinedFormulas.IncreaseReferencesCount(this.project);
            }
        }

        private IEnumerable<EvaluationConfigDataViewModel> GetPredefinedFormulas(LayoutConfigDataViewModel layout)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            foreach (var layoutElement in layout.Resolutions.SelectMany(resolution => resolution.Elements))
            {
                result.AddRange(layoutElement.GetContainedPredefinedFormulas());
            }

            return result;
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

        private void UnsetMediaReferences()
        {
            foreach (
                var layoutElement in
                    this.clonedLayouts.Values.SelectMany(
                        layout => layout.Resolutions.SelectMany(resolution => resolution.Elements)))
            {
                layoutElement.UnsetMediaReference(this.commandRegistry);
            }

            foreach (var section in this.clonedCycles.SelectMany(cycle => cycle.Reference.Sections))
            {
                section.UnsetMediaReferences(this.commandRegistry);
            }
        }

        private void SetMediaReferences()
        {
            foreach (
                var layoutElement in
                    this.clonedLayouts.Values.SelectMany(
                        layout => layout.Resolutions.SelectMany(resolution => resolution.Elements)))
            {
                layoutElement.SetMediaReference(this.commandRegistry);
            }

            foreach (var section in this.clonedCycles.SelectMany(cycle => cycle.Reference.Sections))
            {
                section.SetMediaReferences(this.commandRegistry);
            }
        }
    }
}
