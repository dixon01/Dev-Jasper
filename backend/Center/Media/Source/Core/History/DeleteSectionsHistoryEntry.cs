// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSectionsHistoryEntry.cs" company="Gorba AG">
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

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// History entry that contains all information to undo / redo the deletion of a section.
    /// </summary>
    public class DeleteSectionsHistoryEntry : HistoryEntryBase
    {
        private readonly Lazy<IResourceManager> lazyResourceManager =
            new Lazy<IResourceManager>(() => ServiceLocator.Current.GetInstance<IResourceManager>());

        private readonly ICommandRegistry commandRegistry;

        private readonly IEnumerable<SectionConfigDataViewModelBase> elements;

        private readonly CycleConfigDataViewModelBase cycle;

        private readonly MediaProjectDataViewModel project;

        private readonly CycleNavigationViewModel cycleNavigator;

        private SectionConfigDataViewModelBase selectedBefore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSectionsHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        /// The list of selected elements to be deleted.
        /// </param>
        /// <param name="cycle">
        /// The cycle.
        /// </param>
        /// <param name="project">
        /// the project
        /// </param>
        /// <param name="cycleNavigator">
        /// the cycle navigator view model
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="displayText">
        /// The text to be displayed for this Entry on the UI.
        /// </param>
        public DeleteSectionsHistoryEntry(
            IEnumerable<SectionConfigDataViewModelBase> elements,
            CycleConfigDataViewModelBase cycle,
            MediaProjectDataViewModel project,
            CycleNavigationViewModel cycleNavigator,
            ICommandRegistry commandRegistry,
            string displayText)
            : base(displayText)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (cycle == null)
            {
                throw new ArgumentNullException("cycle");
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
            this.cycle = cycle;
            this.project = project;
            this.cycleNavigator = cycleNavigator;

            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        protected IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            foreach (var section in this.elements)
            {
                section.MakeDirty();

                if (section.Layout == null)
                {
                    continue;
                }

                var cycleSectionReferenceToRemove =
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.FirstOrDefault(
                        r => r.CycleReference == this.cycle && r.SectionReference == section);
                if (cycleSectionReferenceToRemove != null)
                {
                    ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Remove(
                        cycleSectionReferenceToRemove);
                }

                this.cycle.Sections.Remove(section);

                if (this.cycleNavigator.CurrentSection == section)
                {
                    this.cycleNavigator.CurrentSection = this.cycle.Sections.FirstOrDefault();
                    this.cycleNavigator.HighlightCurrentSection();

                    this.selectedBefore = section;
                }

                this.UnsetMediaReferences(section);
                this.UnsetPredefinedFormulaReferences(section);
            }
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            foreach (var section in this.elements)
            {
                var cycleSectionReference = new LayoutCycleSectionRefDataViewModel(this.cycle, section);
                section.MakeDirty();

                if (section.Layout == null)
                {
                    continue;
                }

                ((LayoutConfigDataViewModel)section.Layout).CycleSectionReferences.Add(cycleSectionReference);
                this.cycle.Sections.Add(section);

                this.SetMediaReferences(section);
                this.ResetPredefinedFormulaReferences(section);
            }

            if (this.selectedBefore != null)
            {
                this.cycleNavigator.CurrentSection = this.selectedBefore;
                this.cycleNavigator.HighlightCurrentSection();
            }
        }

        private void ResetPredefinedFormulaReferences(SectionConfigDataViewModelBase section)
        {
            var predefinedFormulas = section.GetContainedPredefinedFormulas();
            predefinedFormulas.IncreaseReferencesCount(this.project);
        }

        private void UnsetPredefinedFormulaReferences(SectionConfigDataViewModelBase section)
        {
            var predefinedFormulas = section.GetContainedPredefinedFormulas();
            predefinedFormulas.DecreaseReferencesCount(this.project);
        }

        private void UnsetMediaReferences(SectionConfigDataViewModelBase section)
        {
            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  PreviousSelectedResourceHash =
                                                      imageSection.Image == null
                                                          ? null
                                                          : imageSection.Image.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.ImageSectionManager.UnsetReferences(imageSection);
                return;
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  PreviousSelectedResourceHash =
                                                       videoSection.Video == null ? null : videoSection.Video.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                    this.ResourceManager.VideoSectionManager.UnsetReferences(videoSection);
                return;
            }

            var poolSection = section as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                this.ResourceManager.PoolManager.UnsetReferences(poolSection);
                poolSection.Pool.ReferencesCount--;
            }
        }

        private void SetMediaReferences(SectionConfigDataViewModelBase section)
        {
            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash =
                                                      imageSection.Image == null ? null : imageSection.Image.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.ImageSectionManager.SetReferences(imageSection);
                return;
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                var selectionParameters = new SelectResourceParameters
                                              {
                                                  CurrentSelectedResourceHash =
                                                       videoSection.Video == null ? null : videoSection.Video.Hash
                                              };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                    .Execute(selectionParameters);
                this.ResourceManager.VideoSectionManager.SetReferences(videoSection);
                return;
            }

            var poolSection = section as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                if (poolSection.CheckPoolExists())
                {
                    this.ResourceManager.PoolManager.SetReferences(poolSection);
                    poolSection.Pool.ReferencesCount++;
                }
                else
                {
                    poolSection.Pool = null;
                }
            }
        }
    }
}
