// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompatibilityChecker.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Checks the project for consistency
    /// </summary>
    public class CompatibilityChecker : ICompatibilityChecker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompatibilityChecker"/> class.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1110:OpeningParenthesisMustBeOnDeclarationLine",
            Justification = "Reviewed. Suppression is OK here.")]
        public CompatibilityChecker()
        {
            this.FeatureUsedCheckers =
                new Dictionary<FeatureComponentRequirements.Features,
                        Func<IMediaApplicationState, ExtendedObservableCollection<ConsistencyMessageDataViewModel>,
                        string,
                        bool>>();

            this.FeatureUsedCheckers.Add(FeatureComponentRequirements.Features.MultiFonts, this.MultiFontsUsed);

            this.FeatureUsedCheckers.Add(
                FeatureComponentRequirements.Features.RssTickerElement,
                this.RssTickerElementUsed);

            this.FeatureUsedCheckers.Add(
                FeatureComponentRequirements.Features.LiveStreamElement,
                this.LiveStreamElementUsed);

            this.FeatureUsedCheckers.Add(FeatureComponentRequirements.Features.SpecialFonts, this.SpecialFontsUsed);

            this.FeatureUsedCheckers.Add(FeatureComponentRequirements.Features.RingScroll, this.RingScrollUsed);
        }

        /// <summary>
        /// Gets or sets the feature used checkers.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public Dictionary<FeatureComponentRequirements.Features,
                          Func<IMediaApplicationState, ExtendedObservableCollection<ConsistencyMessageDataViewModel>, string, bool>> FeatureUsedCheckers
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the application state.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <summary>
        /// The check.
        /// </summary>
        /// <returns>
        /// True if an unsupported feature is used.
        /// </returns>
        public bool Check(
            CompatibilityCheckParameters parameters,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            var unsupportedFeatureUsed = false;
            var mediaApplicationState = parameters.MediaApplicationState;

            var infomediaConfig = mediaApplicationState.CurrentProject.InfomediaConfig;
            var requirements = new List<FeatureComponentRequirements.SoftwareConfig>();
            if (infomediaConfig == null)
            {
                var message = new ConsistencyMessageDataViewModel
                                  {
                                      Source = mediaApplicationState.CurrentProject,
                                      Text =
                                          MediaStrings.ConsistencyChecker_NoProjectTitle,
                                      Description =
                                          MediaStrings.ConsistencyChecker_NoProjectTitle,
                                      Severity = Severity.CompatibilityIssue
                                  };
                messages.Add(message);
            }
            else
            {
                foreach (var feature in FeatureComponentRequirements.VersionFeatureLookup)
                {
                    var featureRequirementsOk = FeatureComponentRequirements.RequirementsOk(
                        parameters.SoftwareConfigs,
                        feature.PackageConfig);

                    if (!featureRequirementsOk)
                    {
                        var isUsed = this.CheckFeatureUsage(
                            feature,
                            mediaApplicationState,
                            messages,
                            parameters.UpdateGroupName);
                        if (isUsed)
                        {
                            unsupportedFeatureUsed = true;
                            FeatureComponentRequirements.MergeRequirements(requirements, feature.PackageConfig);
                        }
                    }
                }
            }

            if (unsupportedFeatureUsed)
            {
                this.CreateVersionMismatchMessage(
                    requirements,
                    parameters.SoftwareConfigs,
                    parameters.UpdateGroupName,
                    messages);
            }

            return unsupportedFeatureUsed;
        }

        private void CreateVersionMismatchMessage(
            List<FeatureComponentRequirements.SoftwareConfig> requirements,
            List<FeatureComponentRequirements.SoftwareConfig> available,
            string updateGroupName,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            foreach (var requirement in requirements)
            {
                var provided = available.FirstOrDefault(a => a.Component == requirement.Component);
                if (provided == null)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    string.Format(
                                        MediaStrings.CompatibilityChecker_BadVersion,
                                        requirement.GetComponentName(),
                                        updateGroupName),
                                Description =
                                    string.Format(
                                        MediaStrings.CompatibilityChecker_BadVersionDescription,
                                        MediaStrings.CompatibilityChecker_NoVersionInformation,
                                        requirement.Version.GetVersionString()),
                                Severity = Severity.CompatibilityIssue,
                                Source = null
                            });
                }
                else if (requirement.Version > provided.Version)
                {
                    messages.Add(
                        new ConsistencyMessageDataViewModel
                            {
                                Text =
                                    string.Format(
                                        MediaStrings.CompatibilityChecker_BadVersion,
                                        requirement.GetComponentName(),
                                        updateGroupName),
                                Description =
                                    string.Format(
                                        MediaStrings.CompatibilityChecker_BadVersionDescription,
                                        provided.Version.GetVersionString(),
                                        requirement.Version.GetVersionString()),
                                Severity = Severity.CompatibilityIssue,
                                Source = null
                            });
                }
            }
        }

        private bool CheckFeatureUsage(
            FeatureComponentRequirements.FeatureRequirements feature,
            IMediaApplicationState mediaApplicationState,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            // check if feature is used
            Func<IMediaApplicationState, ExtendedObservableCollection<ConsistencyMessageDataViewModel>, string, bool>
                checker;
            this.FeatureUsedCheckers.TryGetValue(feature.Feature, out checker);

            if (checker == null)
            {
                throw new Exception(
                    string.Format(
                        "No checker for feature {0}",
                        FeatureComponentRequirements.GetFeatureName(feature.Feature)));
            }

            var result = checker.Invoke(mediaApplicationState, messages, updateGroupName);
            return result;
        }

        /// <summary>
        /// Checks if an RSS ticker element is used.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="updateGroupName">
        /// The update Group Name.
        /// </param>
        /// <returns>
        /// True if feature usage was found.
        /// </returns>
        private bool RssTickerElementUsed(
            IMediaApplicationState state,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            var foundUsage = false;

            this.ForAllLayoutElements(
                state.CurrentProject.InfomediaConfig.Layouts,
                (layout, resolution, element) =>
                    {
                        if (!(element is RssTickerElementDataViewModel))
                        {
                            return;
                        }

                        foundUsage = true;
                        this.AddUnsupportedFeatureUsedMessage(
                            messages,
                            layout,
                            resolution,
                            element,
                            FeatureComponentRequirements.Features.RssTickerElement,
                            updateGroupName);
                    });

            return foundUsage;
        }

        private bool LiveStreamElementUsed(
            IMediaApplicationState state,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            var foundUsage = false;

            this.ForAllLayoutElements(
                state.CurrentProject.InfomediaConfig.Layouts,
                (layout, resolution, element) =>
                {
                    if (!(element is LiveStreamElementDataViewModel))
                    {
                        return;
                    }

                    foundUsage = true;
                    this.AddUnsupportedFeatureUsedMessage(
                        messages,
                        layout,
                        resolution,
                        element,
                        FeatureComponentRequirements.Features.LiveStreamElement,
                        updateGroupName);
                });

            return foundUsage;
        }

        private bool MultiFontsUsed(
            IMediaApplicationState state,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            var foundUsage = false;
            var ledLayouts = this.GetLedLayouts(state);

            this.ForAllLayoutElements(
                ledLayouts,
                (layout, resolution, element) =>
                {
                    var textElement = element as TextElementDataViewModel;
                    if (textElement == null
                        || !textElement.FontFace.Value.Contains(";"))
                    {
                        return;
                    }

                    foundUsage = true;
                    this.AddUnsupportedFeatureUsedMessage(
                        messages,
                        layout,
                        resolution,
                        element,
                        FeatureComponentRequirements.Features.MultiFonts,
                        updateGroupName);
                });

            return foundUsage;
        }

        private IList<LayoutConfigDataViewModel> GetLedLayouts(IMediaApplicationState state)
        {
            var layouts = new List<LayoutConfigDataViewModel>();

            var infomediaConfig = state.CurrentProject.InfomediaConfig;
            foreach (var screen in infomediaConfig.PhysicalScreens.Where(s => s.Type.Value == PhysicalScreenType.LED))
            {
                var screenRef = infomediaConfig.MasterPresentation.MasterLayouts.First()
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

                            layouts.Add(layout);
                        }
                    }
                }
            }

            return layouts;
        }

        private
            bool RingScrollUsed(
            IMediaApplicationState state,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            var foundUsage = false;

            this.ForAllLayoutElements(
                state.CurrentProject.InfomediaConfig.Layouts,
                (layout, resolution, element) =>
                {
                    var textElement = element as TextElementDataViewModel;
                    if (textElement == null
                        || textElement.Overflow.Value != TextOverflow.ScrollRing)
                    {
                        return;
                    }

                    foundUsage = true;
                    this.AddUnsupportedFeatureUsedMessage(
                        messages,
                        layout,
                        resolution,
                        element,
                        FeatureComponentRequirements.Features.RingScroll,
                        updateGroupName);
                });

            return foundUsage;
        }

        private bool SpecialFontsUsed(
            IMediaApplicationState state,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            string updateGroupName)
        {
            var foundUsage = false;

            var ledLayouts = this.GetLedLayouts(state);
            this.ForAllLayoutElements(
                ledLayouts,
                (layout, resolution, element) =>
                {
                    var textElement = element as TextElementDataViewModel;
                    if (textElement == null)
                    {
                        return;
                    }

                    var fonts = textElement.Font.Face.Value.Split(';');
                    foreach (var font in fonts)
                    {
                        var resource = state.CurrentProject.Resources.FirstOrDefault(r => r.Facename == font);

                        if (resource == null
                            || (resource.LedFontType != LedFontType.Unknown // if we do not know expect usage
                                && resource.LedFontType != LedFontType.CUxFont
                                && resource.LedFontType != LedFontType.FonUnicodeArab
                                && resource.LedFontType != LedFontType.FonUnicodeChines
                                && resource.LedFontType != LedFontType.FonUnicodeHebrew))
                        {
                            continue;
                        }

                        foundUsage = true;
                        this.AddUnsupportedFeatureUsedMessage(
                            messages,
                            layout,
                            resolution,
                            element,
                            FeatureComponentRequirements.Features.SpecialFonts,
                            updateGroupName,
                            " / " + resource.Facename);
                    }
                });

            return foundUsage;
        }

        private void ForAllLayoutElements(
            IList<LayoutConfigDataViewModel> layouts,
            Action<LayoutConfigDataViewModel, ResolutionConfigDataViewModel, LayoutElementDataViewModelBase> action)
        {
            foreach (var layout in layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        action.Invoke(layout, resolution, element);
                    }
                }
            }
        }

        private string BuildSourceDescription(
            string updateGroup,
            LayoutConfigDataViewModel layout,
            ResolutionConfigDataViewModel resolution,
            LayoutElementDataViewModelBase element)
        {
            var sourceDescription = updateGroup
                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                    + layout.Name.Value
                                    + MediaStrings.ConsistencyChecker_NameSeperator
                                    + resolution.Width.Value
                                    + "x" + resolution.Height.Value + MediaStrings.ConsistencyChecker_NameSeperator
                                    + element.ElementName.Value;
            return sourceDescription;
        }

        private void AddUnsupportedFeatureUsedMessage(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages,
            LayoutConfigDataViewModel layout,
            ResolutionConfigDataViewModel resolution,
            LayoutElementDataViewModelBase element,
            FeatureComponentRequirements.Features feature,
            string updateGroup,
            string additionalLocation = null)
        {
            var sourceDescription = this.BuildSourceDescription(updateGroup, layout, resolution, element);

            if (additionalLocation != null)
            {
                sourceDescription += additionalLocation;
            }

            var featureName =
                FeatureComponentRequirements.GetFeatureName(feature);
            messages.Add(
                new ConsistencyMessageDataViewModel
                {
                    Text = string.Format(
                            MediaStrings.CompatibilityChecker_UnsupportedFeatureUsed,
                            featureName),
                    Description = string.Format(
                            MediaStrings.ConsistencyChecker_LocationPlaceholder,
                            sourceDescription),
                    Severity = Severity.CompatibilityIssue,
                    Source = element
                });
        }
    }
}