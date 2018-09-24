// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsistencyChecker.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Consistency
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    using Font = System.Drawing.Font;

    /// <summary>
    /// Checks the project for consistency
    /// </summary>
    public class ConsistencyChecker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private InfomediaConfigDataViewModel infomediaConfig;

        /// <summary>
        /// Gets or sets the application state.
        /// </summary>
        public IMediaApplicationState MediaApplicationState { get; set; }

        /// <summary>
        /// The check.
        /// </summary>
        public virtual void Check()
        {
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();

            this.infomediaConfig = this.MediaApplicationState.CurrentProject.InfomediaConfig;

            if (this.infomediaConfig == null)
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = this.MediaApplicationState.CurrentProject,
                    Text = MediaStrings.ConsistencyChecker_NoProjectTitle,
                    Description = MediaStrings.ConsistencyChecker_NoProjectTitle,
                    Severity = Severity.Error
                };
                messages.Add(message);
            }
            else
            {
                // errors
                this.CheckPhysicalScreensPresent(messages);
                this.CheckVirtualDisplaysPresent(messages);
                this.CheckLayoutPresent(messages);
                this.CheckStandardCyclePresent(messages);
                this.CheckCyclePackagePresent(messages);
                this.CheckVirtualDisplaysLinked(messages);
                this.CheckInvalidFormulas(messages);
                this.CheckUnsetImagesInTextualReplacements(messages);
                this.CheckFonts(messages);
                this.CheckEventCycleTriggers(messages);

                this.CheckLayoutElementsUnavailableMedia(messages);
                this.CheckSectionsUnavailableMedia(messages);
                this.CheckLedAudioPhysicalScreenIdentifiers(messages);
                this.CheckAudioLayouts(messages);

                // warnings
                this.CheckLayoutsContainItemsWarning(messages);
                this.CheckUnusedLayout(messages);
                this.CheckUnusedCycle(messages);
                this.CheckPredefinedFormulaEmpty(messages);
                this.CheckTextElementsNotEmpty(messages);
                this.CheckTextElements(messages);
            }

            this.MediaApplicationState.ConsistencyMessages = messages;
        }

        private static void AddElementError(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string sourceDescription,
            string text,
            LayoutElementDataViewModelBase element)
        {
            var message = new ConsistencyMessageDataViewModel
                              {
                                  Source = element,
                                  Description = sourceDescription,
                                  Severity = Severity.Error,
                                  Text = text
                              };
            messages.Add(message);
        }

        private static void CheckLedScreenIdentifier(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            PhysicalScreenConfigDataViewModel screen,
            string sourceDescription)
        {
            if (screen.Identifier != null && !string.IsNullOrWhiteSpace(screen.Identifier.Value))
            {
                int id;
                if (int.TryParse(screen.Identifier.Value, out id))
                {
                    if (id > 0 && id <= 15)
                    {
                        return;
                    }

                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text = MediaStrings.ConsistencyChecker_LedScreenIdentifierOutOfRangeTitle,
                                Description =
                                    string.Format(
                                        MediaStrings.ConsistencyChecker_LedScreenIdentifierOutOfRange,
                                        sourceDescription),
                                Severity = Severity.Warning,
                                Source = screen
                            });
                    return;
                }
            }

            messages.Add(
                new ConsistencyMessageDataViewModel
                    {
                        Text = MediaStrings.ConsistencyChecker_InvalidLedScreenIdentifierTitle,
                        Description =
                            string.Format(
                                MediaStrings.ConsistencyChecker_InvalidLedScreenIdentifier,
                                sourceDescription),
                        Severity = Severity.Error,
                        Source = screen
                    });
        }

        private static void CheckAudioScreenIdentifier(
        ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
        PhysicalScreenConfigDataViewModel screen,
        IEnumerable<PhysicalScreenConfigDataViewModel> audioScreens,
        string sourceDescription)
        {
            if (screen.Identifier != null && !string.IsNullOrWhiteSpace(screen.Identifier.Value))
            {
                int id;
                if (int.TryParse(screen.Identifier.Value, out id))
                {
                    var sameIds =
                        audioScreens.Where(
                            s =>
                            s.Identifier != null && s.Identifier.Value == screen.Identifier.Value
                            && screen.Name.Value != s.Name.Value);
                    if (sameIds.Count() != 0)
                    {
                        messages.Add(
                            new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    MediaStrings
                                    .ConsistencyChecker_InvalidAudioScreenIdentifierTitle,
                                Description =
                                    string.Format(
                                        MediaStrings.ConsistencyChecker_InvalidAudioScreenIdentifier,
                                        sourceDescription),
                                Severity = Severity.Error,
                                Source = screen
                            });
                    }

                    if (id > 0 && id <= 3)
                    {
                        return;
                    }

                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = MediaStrings.ConsistencyChecker_AudioScreenIdentifierOutOfRangeTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_AudioScreenIdentifierOutOfRange,
                                    sourceDescription),
                            Severity = Severity.Warning,
                            Source = screen
                        });
                    return;
                }
            }

            messages.Add(
                new ConsistencyMessageDataViewModel
                {
                    Text = MediaStrings.ConsistencyChecker_InvalidAudioScreenIdentifierTitle,
                    Description =
                        string.Format(
                            MediaStrings.ConsistencyChecker_InvalidAudioScreenIdentifier,
                            sourceDescription),
                    Severity = Severity.Error,
                    Source = screen
                });
        }

        private static List<FontDefinition> GetUsedFonts(InfomediaConfigDataViewModel infomediaConfig)
        {
            var usedFonts = new List<FontDefinition>();

            foreach (var layout in infomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        var textElement = element as TextElementDataViewModel;
                        if (textElement != null)
                        {
                            var sourceDescription = layout.Name.Value
                                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                                    + resolution.Width.Value + "x" + resolution.Height.Value
                                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                                    + element.ElementName.Value;
                            var elementFontFaces = textElement.Font.Face.Value.Split(';');
                            usedFonts.AddRange(
                                elementFontFaces.Select(
                                    elementFontFace =>
                                    new FontDefinition
                                        {
                                            Name = elementFontFace,
                                            Height = textElement.Font.Height.Value,
                                            Italic = textElement.Font.Italic.Value,
                                            Weight = textElement.Font.Weight.Value,
                                            SourceDescription = sourceDescription,
                                            SourceElement = textElement
                                        }));
                        }
                    }
                }
            }

            return usedFonts;
        }

        private static AudioOutputElementDataViewModel GetAudioOutputElement(ResolutionConfigDataViewModel resolution)
        {
            AudioOutputElementDataViewModel audioOutputElement = null;
            if (resolution.Elements.Count == 1)
            {
                audioOutputElement = resolution.Elements.First() as AudioOutputElementDataViewModel;
            }

            return audioOutputElement;
        }

        private static void CheckAudioOutputFrameNotEmpty(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            AudioOutputElementDataViewModel audioOutputFrame,
            LayoutConfigDataViewModel layout,
            ResolutionConfigDataViewModel resolution)
        {
            if (audioOutputFrame.Elements.Count == 0)
            {
                var sourceDescription = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                        + resolution.Width.Value + "x" + resolution.Height.Value
                                        + MediaStrings.ConsistencyChecker_NameSeperator
                                        + audioOutputFrame.ElementName.Value;

                messages.Add(
                    new ConsistencyMessageDataViewModel
                    {
                        Text = MediaStrings.ConsistencyChecker_AudioFrameEmptyTitle,
                        Description =
                            string.Format(
                                MediaStrings.ConsistencyChecker_LocationPlaceholder,
                                sourceDescription),
                        Severity = Severity.Warning,
                        Source = layout
                    });
            }
        }

        private static Font CreateFont(FontDefinition font, FontStyle style)
        {
            try
            {
                return new Font(
                    font.Name,
                    font.Height,
                    style);
            }
            catch (Exception exception)
            {
                Logger.DebugException("Error while creating font. Possible wrong values by the user", exception);
            }

            return null;
        }

        private static void CheckNoAudioElementsContained(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            ResolutionConfigDataViewModel resolution,
            LayoutConfigDataViewModel layout)
        {
            if (resolution.Elements.Count != 0)
            {
                var sourceDescription = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                        + resolution.Width.Value + "x" + resolution.Height.Value;

                messages.Add(
                    new ConsistencyMessageDataViewModel
                    {
                        Text =
                            MediaStrings
                            .ConsistencyChecker_AudioStandardCycleLayoutNotEmptyTitle,
                        Description =
                            string.Format(
                                MediaStrings
                            .ConsistencyChecker_AudioStandardCycleLayoutNotEmpty,
                                sourceDescription),
                        Severity = Severity.Warning,
                        Source = layout
                    });
            }
        }

        private void CheckLedAudioPhysicalScreenIdentifiers(
          ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            var audioScreens =
                this.infomediaConfig.PhysicalScreens.Where(s => s.Type.Value == PhysicalScreenType.Audio).ToList();
            foreach (var screen in this.infomediaConfig.PhysicalScreens)
            {
                var sourceDescription = screen.Name.Value;

                if (screen.Type.Value == PhysicalScreenType.TFT)
                {
                    continue;
                }

                if (screen.Type.Value == PhysicalScreenType.LED)
                {
                    CheckLedScreenIdentifier(messages, screen, sourceDescription);
                    continue;
                }

                if (screen.Type.Value == PhysicalScreenType.Audio)
                {
                    CheckAudioScreenIdentifier(messages, screen, audioScreens, sourceDescription);
                }
            }
        }

        private void CheckEventCycleTriggers(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var eventCycle in this.infomediaConfig.Cycles.EventCycles)
            {
                var hasSystemDateUsed = eventCycle.Trigger.Coordinates.Any(
                    coordinate => coordinate.Table.Value == 0
                                  && coordinate.Column.Value == 0
                                  && coordinate.Row.Value == 0
                                  && coordinate.Language.Value == 0);

                if (hasSystemDateUsed)
                {
                    var sourceDescription = eventCycle.Name.Value;
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = MediaStrings.ConsistencyChecker_EventCycleContainsWrongTriggerTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_LocationPlaceholder,
                                    sourceDescription),
                            Severity = Severity.Error,
                            Source = eventCycle.Trigger,
                            SourceParent = eventCycle
                        });
                }
            }
        }

        private void CheckTextElementsNotEmpty(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (
                var message in this.infomediaConfig.Layouts.SelectMany(
                    layout => (from resolution1 in layout.Resolutions
                               let layout1 = layout
                               from message in
                                   (from TextElementDataViewModel textElement in
                                        resolution1.Elements.Where(e => e is TextElementDataViewModel)
                                    where
                                        string.IsNullOrEmpty(textElement.Value.Value)
                                        && textElement.Value.Formula == null
                                    let sourceDescription =
                                        layout1.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                        + textElement.ElementName.Value
                                    select
                                        new ConsistencyMessageDataViewModel
                                            {
                                                SourceParent = resolution1,
                                                Source = textElement,
                                                Text =
                                                    MediaStrings
                                                    .ConsistencyChecker_LayoutElementTextNotSetTitle,
                                                Description =
                                                    string.Format(
                                                        MediaStrings
                                                    .ConsistencyChecker_LocationPlaceholder,
                                                        sourceDescription),
                                                Severity = Severity.Warning
                                            })
                               select message)))
            {
                messages.Add(message);
            }
        }

        private void CheckPredefinedFormulaEmpty(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var formula in this.infomediaConfig.Evaluations)
            {
                if (formula.Evaluation == null)
                {
                    var message = new ConsistencyMessageDataViewModel
                                      {
                                          SourceParent = this.infomediaConfig.Evaluations,
                                          Source = formula,
                                          Text =
                                              string.Format(
                                                  MediaStrings.ConsistencyChecker_PredefinedFormulaNotSetTitle,
                                                  formula.Name.Value),
                                          Description = MediaStrings.ConsistencyChecker_PredefinedFormulaNotSet,
                                          Severity = Severity.Error
                                      };
                    messages.Add(message);
                    continue;
                }

                if (!formula.Evaluation.IsValid())
                {
                    var message = new ConsistencyMessageDataViewModel
                    {
                        SourceParent = this.infomediaConfig.Evaluations,
                        Source = formula,
                        Text =
                            string.Format(
                                MediaStrings.ConsistencyChecker_InvalidPredefinedFormulaTitle,
                                formula.Name.Value),
                        Description = MediaStrings.ConsistencyChecker_InvalidPredefinedFormula,
                        Severity = Severity.Error
                    };
                    messages.Add(message);
                }
            }
        }

        private List<ResourceInfoDataViewModel> GetVideoResorcesList()
        {
            var videoResources = Enumerable.Empty<ResourceInfoDataViewModel>();
            if (this.MediaApplicationState.CurrentProject.Resources != null)
            {
                videoResources = from res in this.MediaApplicationState.CurrentProject.Resources
                                 where res.Type == ResourceType.Video
                                 select res;
            }

            var videoResourcesList = videoResources.ToList();
            return videoResourcesList;
        }

        private List<ResourceInfoDataViewModel> GetImageResorcesList()
        {
            var imageResources = Enumerable.Empty<ResourceInfoDataViewModel>();
            if (this.MediaApplicationState.CurrentProject.Resources != null)
            {
                imageResources = from res in this.MediaApplicationState.CurrentProject.Resources
                                 where res.Type == ResourceType.Image
                                 select res;
            }

            var imageResourcesList = imageResources.ToList();
            return imageResourcesList;
        }

        private void CheckSectionsUnavailableMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var standardCycle in this.infomediaConfig.Cycles.StandardCycles)
            {
                foreach (var section in standardCycle.Sections)
                {
                    var sourceDescription = standardCycle.Name.Value
                                            + MediaStrings.ConsistencyChecker_NameSeperator
                                            + section.Name;
                    this.CheckSectionForMedia(messages, section, sourceDescription);
                }
            }

            foreach (var eventCycle in this.infomediaConfig.Cycles.EventCycles)
            {
                foreach (var section in eventCycle.Sections)
                {
                    var sourceDescription = eventCycle.Name.Value
                                            + MediaStrings.ConsistencyChecker_NameSeperator
                                            + section.Name;
                    this.CheckSectionForMedia(messages, section, sourceDescription);
                }
            }
        }

        private void CheckAudioLayouts(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (
                var screen in this.infomediaConfig.PhysicalScreens.Where(s => s.Type.Value == PhysicalScreenType.Audio))
            {
                var screenRef = this.infomediaConfig.MasterPresentation.MasterLayouts.First()
                        .PhysicalScreens.FirstOrDefault(s => s.Reference == screen);
                if (screenRef == null)
                {
                    continue;
                }

                foreach (var virtualDisplayRefConfigDataViewModel in screenRef.VirtualDisplays)
                {
                    var cyclePackage = virtualDisplayRefConfigDataViewModel.Reference.CyclePackage;
                    var standardCycles =
                        cyclePackage.StandardCycles.Concat<CycleRefConfigDataViewModelBase>(cyclePackage.EventCycles);
                    foreach (var cycle in standardCycles)
                    {
                        foreach (var sectionConfigDataViewModelBase in cycle.Reference.Sections)
                        {
                            var layout = sectionConfigDataViewModelBase.Layout as LayoutConfigDataViewModel;
                            if (layout == null)
                            {
                                continue;
                            }

                            var width = virtualDisplayRefConfigDataViewModel.Reference.Width.Value;
                            var height = virtualDisplayRefConfigDataViewModel.Reference.Height.Value;
                            var resolution = layout.IndexedResolutions[width, height];
                            if (resolution == null)
                            {
                                continue;
                            }

                            if (cycle is StandardCycleRefConfigDataViewModel)
                            {
                                CheckNoAudioElementsContained(messages, resolution, layout);
                            }
                            else if (cycle is EventCycleRefConfigDataViewModel)
                            {
                                this.CheckEventCycleAudioLayout(
                                    messages,
                                    resolution,
                                    cycle,
                                    sectionConfigDataViewModelBase,
                                    layout);
                            }
                            else
                            {
                                throw new Exception("Unknown cycle type.");
                            }
                        }
                    }
                }
            }
        }

        private void CheckEventCycleAudioLayout(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            ResolutionConfigDataViewModel resolution,
            CycleRefConfigDataViewModelBase cycle,
            SectionConfigDataViewModelBase sectionConfigDataViewModelBase,
            LayoutConfigDataViewModel layout)
        {
            var audioOutputElement = GetAudioOutputElement(resolution);
            if (audioOutputElement == null)
            {
                var sourceDescription = cycle.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                        + sectionConfigDataViewModelBase.Name;

                messages.Add(
                    new ConsistencyMessageDataViewModel
                    {
                        Text =
                            MediaStrings
                            .ConsistencyChecker_EventcycleSectionSetToSilentLayoutTitle,
                        Description =
                            string.Format(
                                MediaStrings
                            .ConsistencyChecker_LocationPlaceholder,
                                sourceDescription),
                        Severity = Severity.Warning,
                        Source = sectionConfigDataViewModelBase
                    });
            }
            else
            {
                CheckAudioOutputFrameNotEmpty(messages, audioOutputElement, layout, resolution);

                foreach (var element in audioOutputElement.Elements)
                {
                    this.CheckAudioPlaybackElement(messages, element, layout, resolution);
                }
            }
        }

        private void CheckAudioPlaybackElement(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            PlaybackElementDataViewModelBase element,
            LayoutConfigDataViewModel layout,
            ResolutionConfigDataViewModel resolution)
        {
            var audioFile = element as AudioFileElementDataViewModel;
            if (audioFile != null)
            {
                if (audioFile.Filename.Value == string.Empty && audioFile.Filename.Formula == null)
                {
                    var sourceDescription = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                            + resolution.Width.Value + "x" + resolution.Height.Value
                                            + MediaStrings.ConsistencyChecker_NameSeperator + element.ElementName.Value;

                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = MediaStrings.ConsistencyChecker_AudioFileNotSetTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_AudioFileNotSet,
                                    sourceDescription),
                            Severity = Severity.Error,
                            Source = element
                        });
                }
            }

            var staticTts = element as TextToSpeechElementDataViewModel;
            var dynamicTts = element as DynamicTtsElementDataViewModel;
            if (staticTts != null && dynamicTts == null)
            {
                if (staticTts.Value.Value == string.Empty)
                {
                    var sourceDescription = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                            + resolution.Width.Value + "x" + resolution.Height.Value
                                            + MediaStrings.ConsistencyChecker_NameSeperator + element.ElementName.Value;

                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = MediaStrings.ConsistencyChecker_StaticTtsEmptyTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_LocationPlaceholder,
                                    sourceDescription),
                            Severity = Severity.Error,
                            Source = element
                        });
                }
            }

            if (dynamicTts != null)
            {
                if (staticTts.Value.Value == string.Empty && staticTts.Value.Formula == null)
                {
                    var sourceDescription = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                            + resolution.Width.Value + "x" + resolution.Height.Value
                                            + MediaStrings.ConsistencyChecker_NameSeperator + element.ElementName.Value;

                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = MediaStrings.ConsistencyChecker_DynamicTtsEmptyTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_DynamicTtsEmptyTitle,
                                    sourceDescription),
                            Severity = Severity.Error,
                            Source = element
                        });
                }
            }
        }

        private void CheckSectionForMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            SectionConfigDataViewModelBase section,
            string sourceDescription)
        {
            var imageSection = section as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                this.CheckImageSectionMedia(messages, imageSection, sourceDescription);
                return;
            }

            var videoSection = section as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                this.CheckVideoSectionMedia(messages, videoSection, sourceDescription);
                return;
            }

            var poolSection = section as PoolSectionConfigDataViewModel;
            if (poolSection != null)
            {
                this.CheckPoolSectionMedia(messages, poolSection, sourceDescription);
            }
        }

        private void CheckPoolSectionMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            PoolSectionConfigDataViewModel poolSection,
            string sourceDescription)
        {
            if (poolSection.Pool == null)
            {
                messages.Add(
                    new ConsistencyMessageDataViewModel
                    {
                        Text = MediaStrings.ConsistencyChecker_PoolSectionPoolNotSetTitle,
                        Description =
                        string.Format(
                                MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                        Severity = Severity.Error,
                        Source = poolSection
                    });
            }
        }

        private void CheckVideoSectionMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            VideoSectionConfigDataViewModel videoSection,
            string sourceDescription)
        {
            if (videoSection.VideoUri.Value == string.Empty)
            {
                messages.Add(
                    new ConsistencyMessageDataViewModel
                    {
                        Source = videoSection,
                        Text = MediaStrings.ConsistencyChecker_VideoSectionVideoNotSetTitle,
                        Description =
                        string.Format(
                                MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                        Severity = Severity.Error,
                    });
            }
            else
            {
                if (videoSection.Video == null
                    && !Uri.IsWellFormedUriString(videoSection.VideoUri.Value, UriKind.Absolute))
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Source = videoSection,
                                Text =
                                    string.Format(
                                        MediaStrings
                                    .ConsistencyChecker_VideoSectionVideoNotInMediaTitle,
                                        videoSection.VideoUri.Value),
                                Description =
                                    string.Format(
                                        MediaStrings
                                    .ConsistencyChecker_LocationPlaceholder,
                                        sourceDescription),
                                Severity = Severity.Error,
                            });
                }
            }
        }

        private void CheckImageSectionMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            ImageSectionConfigDataViewModel imageSection,
            string sourceDescription)
        {
            if (imageSection.Image == null)
            {
                if (imageSection.Filename.Value == string.Empty)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Source = imageSection,
                            Text =
                                MediaStrings.ConsistencyChecker_ImageSectionImageNotSetTitle,
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                            Severity = Severity.Error,
                        });
                }
                else
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Source = imageSection,
                            Text =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_ImageSectionImageNotInMediaTitle,
                                    imageSection.Filename.Value),
                            Description =
                                string.Format(
                                    MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                            Severity = Severity.Error,
                        });
                }
            }
        }

        private void CheckLayoutElementsUnavailableMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            var videoResourcesList = this.GetVideoResorcesList();
            var imageResourceList = this.GetImageResorcesList();

            foreach (var layout in this.infomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        var imageElement = element as ImageElementDataViewModel;
                        if (imageElement != null)
                        {
                            var sourceDescription = layout.Name.Value
                                                   + MediaStrings.ConsistencyChecker_NameSeperator
                                                   + resolution.Width.Value + "x" + resolution.Height.Value
                                                   + MediaStrings.ConsistencyChecker_NameSeperator
                                                   + element.ElementName.Value;
                            this.CheckImageElementMedia(messages, imageElement, sourceDescription);
                        }
                        else
                        {
                            var sourceDescription = layout.Name.Value
                                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                                    + resolution.Width.Value + "x" + resolution.Height.Value
                                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                                    + element.ElementName.Value;

                            var videoElement = element as VideoElementDataViewModel;
                            if (videoElement != null)
                            {
                                this.CheckVideoElementMedia(
                                    messages, videoElement, videoResourcesList, imageResourceList, sourceDescription);
                            }
                        }
                    }
                }
            }
        }

        private void CheckVideoElementMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            VideoElementDataViewModel videoElement,
            IEnumerable<ResourceInfoDataViewModel> videoResourcesList,
            List<ResourceInfoDataViewModel> imageResourceList,
            string sourceDescription)
        {
            if (string.IsNullOrEmpty(videoElement.VideoUri.Value))
            {
                if (videoElement.VideoUri.Formula == null)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    MediaStrings
                                    .ConsistencyChecker_LayoutElementVideoNotSetTitle,
                                Description =
                                    string.Format(
                                        MediaStrings
                                    .ConsistencyChecker_LocationPlaceholder,
                                        sourceDescription),
                                Severity = Severity.Error,
                                Source = videoElement,
                            });
                }

                return;
            }

            if (videoElement.VideoUri.Formula != null)
            {
                return;
            }

            if (videoElement.PreviewImageHash == "UriVideoPlaceholder")
            {
                this.CheckVideoElementUri(messages, videoElement, sourceDescription);
                return;
            }

            var liveStream = videoElement as LiveStreamElementDataViewModel;
            if (liveStream != null)
            {
                this.CheckVideoElementUri(messages, liveStream, sourceDescription);
                this.CheckLiveStreamElement(messages, liveStream, imageResourceList, sourceDescription);
                return;
            }

            var found = videoResourcesList.Any(resource => resource.Filename.EndsWith(videoElement.VideoUri.Value));
            if (found)
            {
                return;
            }

            messages.Add(
                new ConsistencyMessageDataViewModel
                    {
                        Text =
                            string.Format(
                                MediaStrings
                            .ConsistencyChecker_LayoutElementVideoNotInMediaTitle,
                                videoElement.VideoUri.Value),
                        Description =
                            string.Format(
                                MediaStrings.ConsistencyChecker_LocationPlaceholder,
                                sourceDescription),
                        Severity = Severity.Error,
                        Source = videoElement,
                    });
        }

        private void CheckLiveStreamElement(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            LiveStreamElementDataViewModel liveStream,
            IEnumerable<ResourceInfoDataViewModel> imageResourcesList,
            string sourceDescription)
        {
            if (string.IsNullOrEmpty(liveStream.FallbackImage.Value))
            {
                messages.Add(
                   new ConsistencyMessageDataViewModel
                   {
                       Text =
                           MediaStrings
                           .ConsistencyChecker_LiveStreamHasNoFallbackTitle,
                       Description =
                           string.Format(
                               MediaStrings.ConsistencyChecker_LocationPlaceholder,
                               sourceDescription),
                       Severity = Severity.Warning,
                       Source = liveStream,
                   });

                return;
            }

            var found = imageResourcesList.Any(resource => resource.Filename.EndsWith(liveStream.FallbackImage.Value));
            if (!found)
            {
                messages.Add(
                   new ConsistencyMessageDataViewModel
                   {
                       Text =
                           MediaStrings
                           .ConsistencyChecker_LiveStreamFallbackNotInRessourcesTitle,
                       Description =
                           string.Format(
                               MediaStrings.ConsistencyChecker_LocationPlaceholder,
                               sourceDescription),
                       Severity = Severity.Warning,
                       Source = liveStream,
                   });
            }
        }

        private void CheckVideoElementUri(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            VideoElementDataViewModel videoElement,
            string sourceDescription)
        {
            if (!Uri.IsWellFormedUriString(videoElement.VideoUri.Value, UriKind.Absolute))
            {
               messages.Add(
                   new ConsistencyMessageDataViewModel
                   {
                       Text =
                           MediaStrings
                           .ConsistencyChecker_LayoutElementStreamNotSetTitle,
                       Description =
                           string.Format(
                               MediaStrings.ConsistencyChecker_LocationPlaceholder,
                               sourceDescription),
                       Severity = Severity.Error,
                       Source = videoElement,
                   });
            }
        }

        private void CheckImageElementMedia(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            ImageElementDataViewModel imageElement,
            string sourceDescription)
        {
            if (imageElement.Image == null)
            {
                if (imageElement.Filename.Formula != null)
                {
                    return;
                }

                if (imageElement.Filename.Value == string.Empty)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    MediaStrings.ConsistencyChecker_LayoutElementImageNotSetTitle,
                                Description =
                                    string.Format(
                                        MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                                Severity = Severity.Error,
                                Source = imageElement,
                            });
                }
                else
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    string.Format(
                                        MediaStrings.ConsistencyChecker_LayoutElementImageNotInMediaTitle,
                                        imageElement.Filename.Value),
                                Description =
                                    string.Format(
                                       MediaStrings.ConsistencyChecker_LocationPlaceholder, sourceDescription),
                                Severity = Severity.Error,
                                Source = imageElement,
                            });
                }
            }
        }

        private void CheckUnsetImagesInTextualReplacements(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.MediaApplicationState.CurrentProject.Replacements == null)
            {
                return;
            }

            foreach (var textualReplacement in this.MediaApplicationState.CurrentProject.Replacements)
            {
                if (textualReplacement.IsImageReplacement && textualReplacement.Image == null)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    MediaStrings
                                    .ConsistencyChecker_ReplacementImageNotSetTitle,
                                Description =
                                    string.Format(
                                        MediaStrings.ConsistencyChecker_ReplacementImageNotSet,
                                        textualReplacement.Number.Value),
                                Severity = Severity.Error,
                                Source = textualReplacement,
                            });
                }
            }
        }

        private void CheckFonts(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            var usedFonts = GetUsedFonts(this.infomediaConfig);

            this.CheckForUnavailableFonts(messages, usedFonts);
        }

        private void CheckForUnavailableFonts(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            IEnumerable<FontDefinition> usedFonts)
        {
            foreach (var font in usedFonts)
            {
                var fontFound = false;
                var style = font.Italic ? FontStyle.Italic : FontStyle.Regular;
                if (font.Height == 0)
                {
                    continue;
                }

                using (var fontTester = CreateFont(font, style))
                {
                    // is a installed font
                    if (fontTester != null && fontTester.Name == font.Name)
                    {
                        fontFound = true;
                    }

                    // is a user font
                    if (!fontFound)
                    {
                        var userFont =
                            this.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                                r => r.Type == ResourceType.Font && r.Facename == font.Name);
                        if (userFont != null)
                        {
                            fontFound = true;
                        }
                    }

                    if (!fontFound)
                    {
                        var styleName = font.Italic ? MediaStrings.ConsistencyChecker_FontStyleItalic : string.Empty;
                        var message = new ConsistencyMessageDataViewModel
                                          {
                                              Source = font.SourceElement,
                                              Text =
                                                  MediaStrings
                                                  .ConsistencyChecker_FontNotFoundTitle,
                                              Description =
                                                  string.Format(
                                                      MediaStrings
                                                  .ConsistencyChecker_FontNotFound,
                                                      font.Name,
                                                      styleName,
                                                      font.SourceDescription),
                                              Severity = Severity.Error
                                          };
                        messages.Add(message);
                    }
                }
            }
        }

        private void CheckInvalidFormulas(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            this.CheckInvalidFormulasInLayoutElements(messages);
            this.CheckInvalidFormulasInCycles(messages);
            this.CheckInvalidFormulasInPhysicalScreens(messages);
        }

        private void CheckInvalidFormulasInPhysicalScreens(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var screen in this.infomediaConfig.PhysicalScreens)
            {
                this.CheckInvalidFormulaInObject(messages, screen, screen.Name.Value);
            }
        }

        private void CheckInvalidFormulasInCycles(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var standardCycle in this.infomediaConfig.Cycles.StandardCycles)
            {
                foreach (var section in standardCycle.Sections)
                {
                    this.CheckInvalidFormulaInObject(
                        messages,
                        section,
                        standardCycle.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator + section.Name);
                }

                this.CheckInvalidFormulaInObject(messages, standardCycle, standardCycle.Name.Value);
            }

            foreach (var eventCycle in this.infomediaConfig.Cycles.EventCycles)
            {
                foreach (var section in eventCycle.Sections)
                {
                    this.CheckInvalidFormulaInObject(
                        messages,
                        section,
                        eventCycle.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator + section.Name);
                }

                this.CheckInvalidFormulaInObject(messages, eventCycle, eventCycle.Name.Value);
            }
        }

        private void CheckLayoutsContainItemsWarning(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var layout in this.infomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    // skip standard audio layout, which can not be modified by the user
                    if (resolution.Width.Value == 0
                        && resolution.Height.Value == 0
                        && resolution.Elements.Count == 0)
                    {
                        continue;
                    }

                    if (!resolution.Elements.Any())
                    {
                        var text = string.Format(
                            MediaStrings.ConsistencyChecker_LayoutWithoutElementsTitle,
                            layout.Name.Value,
                            resolution.Width.Value,
                            resolution.Height.Value);
                        messages.Add(
                            new ConsistencyMessageDataViewModel
                                {
                                    Text = text,
                                    Description = MediaStrings.ConsistencyChecker_LayoutWithoutElements,
                                    Severity = Severity.Warning,
                                    Source = resolution,
                                });
                    }
                }
            }
        }

        private void CheckUnusedLayout(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var layout in this.infomediaConfig.Layouts)
            {
                if (!layout.CycleSectionReferences.Any())
                {
                    messages.Add(new ConsistencyMessageDataViewModel
                    {
                        Text = string.Format(MediaStrings.ConsistencyChecker_UnusedLayoutTitle, layout.Name.Value),
                        Description = MediaStrings.ConsistencyChecker_UnusedLayout,
                        Severity = Severity.Warning,
                        Source = layout,
                    });
                }
            }
        }

        private void CheckUnusedCycle(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var cycle in this.infomediaConfig.Cycles.StandardCycles)
            {
                if (!this.infomediaConfig.CyclePackages.Any(cp => cp.StandardCycles.Any(sc => sc.Reference == cycle)))
                {
                    messages.Add(new ConsistencyMessageDataViewModel
                    {
                        Text = string.Format(MediaStrings.ConsistencyChecker_UnusedCycleTitle, cycle.Name.Value),
                        Description = MediaStrings.ConsistencyChecker_UnusedCycle,
                        Severity = Severity.Warning,
                        Source = cycle,
                    });
                }
            }

            foreach (var cycle in this.infomediaConfig.Cycles.EventCycles)
            {
                if (!this.infomediaConfig.CyclePackages.Any(cp => cp.EventCycles.Any(sc => sc.Reference == cycle)))
                {
                    messages.Add(new ConsistencyMessageDataViewModel
                    {
                        Text = string.Format(MediaStrings.ConsistencyChecker_UnusedCycleTitle, cycle.Name.Value),
                        Description = MediaStrings.ConsistencyChecker_UnusedCycle,
                        Severity = Severity.Warning,
                        Source = cycle,
                    });
                }
            }
        }

        private void CheckInvalidFormulasInLayoutElements(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var layout in this.infomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        var audioOutputElement = element as AudioOutputElementDataViewModel;
                        if (audioOutputElement != null)
                        {
                            var name = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                       + element.ElementName.Value;
                            this.CheckInvalidFormulaInObject(messages, element, name);
                            foreach (var playbackElement in audioOutputElement.Elements)
                            {
                                name = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                       + playbackElement.ElementName.Value;
                                this.CheckInvalidFormulaInObject(messages, playbackElement, name);
                            }
                        }
                        else
                        {
                            var name = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                       + element.ElementName.Value;
                            this.CheckInvalidFormulaInObject(messages, element, name);
                        }
                    }
                }
            }
        }

        private void CheckInvalidFormulaInObject(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            object obj,
            string parentName)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var dynamicProperty = propertyInfo.GetValue(obj, new object[0]) as IDynamicDataValue;
                if (dynamicProperty != null)
                {
                    if (dynamicProperty.Formula != null)
                    {
                        var csvMappingFormula = dynamicProperty.Formula as CsvMappingEvalDataViewModel;
                        if (!dynamicProperty.Formula.IsValid())
                        {
                            var translatedPropertyname =
                                MediaStrings.ResourceManager.GetString("PropertyGridField_" + propertyInfo.Name)
                                ?? propertyInfo.Name;

                            var formulaUsage = parentName + MediaStrings.ConsistencyChecker_NameSeperator
                                               + translatedPropertyname;
                            var formulaName = dynamicProperty.Formula.GetType()
                                .Name.Replace("EvalDataViewModel", string.Empty);
                            var predefinedFormula = dynamicProperty.Formula as EvaluationConfigDataViewModel;
                            if (predefinedFormula != null)
                            {
                                formulaName = predefinedFormula.Name.Value;
                            }

                            var message = new ConsistencyMessageDataViewModel
                                              {
                                                  SourceParent = obj,
                                                  Source = dynamicProperty,
                                                  Text =
                                                      MediaStrings
                                                      .ConsistencyChecker_InvalidFormulaTitle,
                                                  Description =
                                                      string.Format(
                                                          MediaStrings
                                                      .ConsistencyChecker_InvalidFormula,
                                                          formulaName,
                                                          formulaUsage),
                                                  Severity = Severity.Error
                                              };
                            messages.Add(message);
                            continue;
                        }

                        if (csvMappingFormula != null
                            && (string.IsNullOrEmpty(csvMappingFormula.FileName.Value)
                                || (csvMappingFormula.FileName.Value != "codeconversion"
                                && this.MediaApplicationState.CurrentProject.CsvMappings.All(
                                    mapping => mapping.Filename.Value != csvMappingFormula.FileName.Value))))
                        {
                            var translatedPropertyname =
                               MediaStrings.ResourceManager.GetString("PropertyGridField_" + propertyInfo.Name)
                               ?? propertyInfo.Name;

                            var formulaUsage = parentName + MediaStrings.ConsistencyChecker_NameSeperator
                                               + translatedPropertyname;
                            var formulaName = dynamicProperty.Formula.GetType()
                                .Name.Replace("EvalDataViewModel", string.Empty);
                            var message = new ConsistencyMessageDataViewModel
                            {
                                SourceParent = obj,
                                Source = dynamicProperty,
                                Text =
                                    MediaStrings
                                    .ConsistencyChecker_InvalidFormulaTitle,
                                Description =
                                    string.Format(
                                        MediaStrings
                                    .ConsistencyChecker_CsvFileNotFound,
                                        csvMappingFormula.FileName.Value,
                                        formulaName,
                                        formulaUsage),
                                Severity = Severity.Error
                            };
                            messages.Add(message);
                        }
                    }
                }
            }
        }

        private void CheckVirtualDisplaysLinked(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var virtualDisplays in this.infomediaConfig.VirtualDisplays)
            {
                if (virtualDisplays.CyclePackage == null)
                {
                    var message = new ConsistencyMessageDataViewModel
                    {
                        Source =
                            this.infomediaConfig.CyclePackages,
                        Text = MediaStrings.ConsistencyChecker_VirtualDisplayNotLinkedTitle,
                        Description =
                            string.Format(
                            MediaStrings.ConsistencyChecker_VirtualDisplayNotLinked, virtualDisplays.Name),
                        Severity = Severity.Error
                    };
                    messages.Add(message);
                }
            }
        }

        // ReSharper disable UnusedMember.Local
        private void MakeWarning(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        // ReSharper restore UnusedMember.Local
        {
            var message = new ConsistencyMessageDataViewModel
            {
                Source = this,
                Text =
                    "Dummy warning.",
                Description =
                    "This is an artifact.",
                Severity = Severity.Warning
            };
            messages.Add(message);
        }

        // ReSharper disable UnusedMember.Local
        private void MakeError(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        // ReSharper restore UnusedMember.Local
        {
            var message = new ConsistencyMessageDataViewModel
            {
                Source = this,
                Text =
                    "Dummy error.",
                Description =
                    "This is an artifact.",
                Severity = Severity.Error
            };
            messages.Add(message);
        }

        private void CheckCyclePackagePresent(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.infomediaConfig.CyclePackages == null
                || this.infomediaConfig.CyclePackages.Count < 1)
            {
                var message = new ConsistencyMessageDataViewModel
                                  {
                                      Source = this.infomediaConfig.CyclePackages,
                                      Text = MediaStrings.ConsistencyChecker_NoCyclePackageTitle,
                                      Description = MediaStrings.ConsistencyChecker_NoCyclePackage,
                                      Severity = Severity.Error
                                  };
                messages.Add(message);
            }
        }

        private void CheckStandardCyclePresent(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.infomediaConfig.Cycles.StandardCycles == null
                || this.infomediaConfig.Cycles.StandardCycles.Count < 1)
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = this.infomediaConfig.Cycles.StandardCycles,
                    Text = MediaStrings.ConsistencyChecker_NoStandardCycleTitle,
                    Description = MediaStrings.ConsistencyChecker_NoStandardCycle,
                    Severity = Severity.Error
                };
                messages.Add(message);
            }
        }

        private void CheckLayoutPresent(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.infomediaConfig.Layouts == null
                || this.infomediaConfig.Layouts.Count < 1)
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = this.infomediaConfig.Layouts,
                    Text = MediaStrings.ConsistencyChecker_NoLayoutTitle,
                    Description = MediaStrings.ConsistencyChecker_NoLayout,
                    Severity = Severity.Error
                };
                messages.Add(message);
            }
        }

        private void CheckVirtualDisplaysPresent(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.infomediaConfig.VirtualDisplays == null
                || this.infomediaConfig.VirtualDisplays.Count < 1)
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = this.infomediaConfig.VirtualDisplays,
                    Text = MediaStrings.ConsistencyChecker_NoVirtualDisplayTitle,
                    Description = MediaStrings.ConsistencyChecker_NoVirtualDisplay,
                    Severity = Severity.Error
                };
                messages.Add(message);
            }
        }

        private void CheckPhysicalScreensPresent(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            if (this.infomediaConfig.PhysicalScreens == null
                || this.infomediaConfig.PhysicalScreens.Count < 1)
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = this.infomediaConfig.PhysicalScreens,
                    Text = MediaStrings.ConsistencyChecker_NoPhysicalScreenTitle,
                    Description = MediaStrings.ConsistencyChecker_NoPhysicalScreen,
                    Severity = Severity.Error
                };
                messages.Add(message);
            }
        }

        private void CheckTextElements(ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var layout in this.infomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements.OfType<RssTickerElementDataViewModel>())
                    {
                        var description = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                          + element.ElementName.Value;
                        var sourceDescription = string.Format(
                            MediaStrings.ConsistencyChecker_LocationPlaceholder,
                            description);
                        this.CheckRssTickerElement(messages, element, sourceDescription);
                    }

                    foreach (var element in resolution.Elements.OfType<TextElementDataViewModel>())
                    {
                        var description = layout.Name.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                          + element.ElementName.Value;
                        var sourceDescription = string.Format(
                            MediaStrings.ConsistencyChecker_LocationPlaceholder,
                            description);
                        this.CheckTextElement(messages, element, sourceDescription);
                    }
                }
            }
        }

        private void CheckTextElement(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            TextElementDataViewModel element,
            string sourceDescription)
        {
            if (element.ScrollSpeed.Value == 0
                && (element.Overflow.Value == TextOverflow.Scroll
                    || element.Overflow.Value == TextOverflow.ScrollAlways))
            {
                var message = new ConsistencyMessageDataViewModel
                {
                    Source = element,
                    Description = sourceDescription,
                    Severity = Severity.Error,
                    Text = MediaStrings.ConsistencyChecker_ScrollSpeedZero
                };
                messages.Add(message);
            }

            if (!this.IsLedFont(element.Font.Face.Value))
            {
                if (element.FontHeight.Value <= 0)
                {
                    AddElementError(
                        messages, sourceDescription, MediaStrings.ConsistencyChecker_InvalidFontHeight, element);
                }

                if (element.FontWeight.Value <= 0)
                {
                    AddElementError(
                        messages, sourceDescription, MediaStrings.ConsistencyChecker_InvalidFontWeight, element);
                }
            }
        }

        private bool IsLedFont(string fontName)
        {
            return this.MediaApplicationState.CurrentProject.Resources.Any(
                r => r.IsLedFont && r.Facename.Equals(fontName));
        }

        private void CheckRssTickerElement(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            RssTickerElementDataViewModel element,
            string sourceDescription)
        {
            if (string.IsNullOrEmpty(element.RssUrl.Value))
            {
                var message = new ConsistencyMessageDataViewModel
                                  {
                                      Source = element,
                                      Description = sourceDescription,
                                      Severity = Severity.Warning,
                                      Text = MediaStrings.ConsistencyChecker_RssTickerAddressEmpty
                                  };
                messages.Add(message);
            }

            if (element.ScrollSpeed.Value == 0)
            {
                var message = new ConsistencyMessageDataViewModel
                                  {
                                      Source = element,
                                      Description = sourceDescription,
                                      Severity = Severity.Error,
                                      Text = MediaStrings.ConsistencyChecker_ScrollSpeedZero
                                  };
                messages.Add(message);
            }

            if (element.Validity.Value.TotalMinutes < 0)
            {
                var message = new ConsistencyMessageDataViewModel
                                  {
                                      Source = element,
                                      Description = sourceDescription,
                                      Severity = Severity.Error,
                                      Text = MediaStrings.ConsistencyChecker_RssTickerInvalidValidity
                                  };
                var lastError = messages.LastOrDefault(m => m.Severity == Severity.Error);
                if (lastError == null)
                {
                    messages.Insert(0, message);
                }
                else
                {
                    var index = messages.IndexOf(lastError);
                    messages.Insert(index, message);
                }
            }

            if (element.FontHeight.Value <= 0)
            {
                AddElementError(
                    messages, sourceDescription, MediaStrings.ConsistencyChecker_InvalidFontHeight, element);
            }

            if (element.FontWeight.Value <= 0)
            {
                AddElementError(
                    messages, sourceDescription, MediaStrings.ConsistencyChecker_InvalidFontWeight, element);
            }
        }

        private class FontDefinition
        {
            public string Name { get; set; }

            public int Height { get; set; }

            public bool Italic { get; set; }

            public int Weight { get; set; }

            public string SourceDescription { get; set; }

            public object SourceElement { get; set; }
        }
    }
}
