// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateSectionHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a layout element.
    /// </summary>
    public class CreateSectionHistoryEntry : HistoryEntryBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SectionConfigDataViewModelBase element;
        private readonly CycleConfigDataViewModelBase parent;

        private readonly LayoutCycleSectionRefDataViewModel cycleSectionReference;

        private readonly ICommandRegistry commandRegistry;

        private ResolutionConfigDataViewModel newResolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSectionHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The element data view model.
        /// </param>
        /// <param name="parent">
        /// The parent that contains the element.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="displayText">
        /// The display text shown on the UI.
        /// </param>
        public CreateSectionHistoryEntry(
            SectionConfigDataViewModelBase element,
            CycleConfigDataViewModelBase parent,
            ICommandRegistry commandRegistry,
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

            this.element = element;
            this.parent = parent;
            this.cycleSectionReference = new LayoutCycleSectionRefDataViewModel(this.parent, this.element);

            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.element.MakeDirty();
            ((LayoutConfigDataViewModel)this.element.Layout).CycleSectionReferences.Remove(this.cycleSectionReference);
            this.parent.Sections.Remove(this.element);

            if (this.newResolution != null)
            {
                var layout = (LayoutConfigDataViewModel)this.element.Layout;
                layout.Resolutions.Remove(this.newResolution);
            }

            this.UnsetMediaReferences();
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.element.MakeDirty();

            var layout = (LayoutConfigDataViewModel)this.element.Layout;
            if (layout != null)
            {
                this.newResolution = layout.AddCurrentResolution();
            }

            this.parent.Sections.Add(this.element);
            ((LayoutConfigDataViewModel)this.element.Layout).CycleSectionReferences.Add(this.cycleSectionReference);

            this.SetMediaReferences();
        }

        private void SetMediaReferences()
        {
            var imageSection = this.element as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var image = imageSection.Image;
                if (image != null)
                {
                    var sectionParameters = new SelectResourceParameters
                                                {
                                                    CurrentSelectedResourceHash =
                                                        imageSection.Image == null
                                                            ? null
                                                            : imageSection.Image.Hash
                                                };
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                        .Execute(sectionParameters);
                }
                else
                {
                    Logger.Debug(
                        "Image file '{0}' associated with image section '{1}' not found.",
                        imageSection.Filename.Value,
                        imageSection.Name);
                }
            }

            var videoSection = this.element as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                  var video = videoSection.Video;
                if (video != null)
                {
                    var sectionParameters = new SelectResourceParameters
                                                {
                                                    CurrentSelectedResourceHash =
                                                        videoSection.Video == null
                                                            ? null
                                                            : videoSection.Video.Hash
                                                };
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                        .Execute(sectionParameters);
                }
                else
                {
                    Logger.Debug(
                        "Video file '{0}' associated with video section '{1}' not found.",
                        videoSection.VideoUri.Value,
                        videoSection.Name);
                }
            }

            var poolSection = this.element as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                if (poolSection.CheckPoolExists())
                {
                    poolSection.Pool.ReferencesCount++;
                }
                else
                {
                    poolSection.Pool = null;
                }
            }
        }

        private void UnsetMediaReferences()
        {
            var imageSection = this.element as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                var image = imageSection.Image;
                if (image != null)
                {
                    var sectionParameters = new SelectResourceParameters
                    {
                        PreviousSelectedResourceHash = imageSection.Image.Hash
                    };
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                        .Execute(sectionParameters);
                }
                else
                {
                    Logger.Debug(
                        "Image file '{0}' associated with image section '{1}' not found.",
                        imageSection.Filename.Value,
                        imageSection.Name);
                }
            }

            var videoSection = this.element as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                var video = videoSection.Video;
                if (video != null)
                {
                    var sectionParameters = new SelectResourceParameters
                    {
                        PreviousSelectedResourceHash = videoSection.Video.Hash
                    };
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                        .Execute(sectionParameters);
                }
                else
                {
                    Logger.Debug(
                        "Video file '{0}' associated with video section '{1}' not found.",
                        videoSection.VideoUri.Value,
                        videoSection.Name);
                }
            }

            var poolSection = this.element as PoolSectionConfigDataViewModel;
            if (poolSection != null && poolSection.Pool != null)
            {
                poolSection.Pool.ReferencesCount--;
            }
        }
    }
}