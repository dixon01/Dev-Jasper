// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompatibilityCheckerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Compatibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="CompatibilityChecker"/>.
    /// </summary>
    [TestClass]
    public class CompatibilityCheckerTest
    {
        /// <summary>
        /// Test if all versions are meet.
        /// </summary>
        [TestMethod]
        public void VersionsFulfilled()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);

            var availableSoftware = new List<FeatureComponentRequirements.SoftwareConfig>();
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("99.99")));
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.DxRenderer,
                    new SoftwareComponentVersion("99.99")));
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Composer,
                    new SoftwareComponentVersion("99.99")));

            var parameters = new CompatibilityCheckParameters
                                 {
                                     MediaApplicationState = state,
                                     SoftwareConfigs = availableSoftware
                                 };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsFalse(hasIssues);
            Assert.IsTrue(messages.Count == 0);
        }

        /// <summary>
        /// Test if feature is not used but version is too low
        /// </summary>
        [TestMethod]
        public void VersionTooLowFeatureNotUsed()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);

            var availableSoftware = AddTooLowAvailableVersions();

            var parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsFalse(hasIssues);
            Assert.IsTrue(messages.Count == 0);
        }

        /// <summary>
        /// Test if feature is used and version is too low
        /// </summary>
        [TestMethod]
        public void UsedLedTextElement()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var shellMock = Helpers.CreateMediaShell();
            shellMock.Setup(shell => shell.MediaApplicationState).Returns(state);

            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig(shellMock.Object);
            state.CurrentProject.InfomediaConfig = infomediaConfig;

            var standardSectionConfigDataViewModel = AddLedLayoutToProject(shellMock, state);

            var layout = new LayoutConfigDataViewModel(shellMock.Object);
            state.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.First()
                .VirtualDisplays.First()
                .Reference.CyclePackage.StandardCycles.First()
                .Reference.Sections.First()
                .Layout = layout;

            var dataModel = new ResolutionConfigDataModel { DisplayText = "DisplayText", Height = 100, Width = 50 };
            var font = new FontDataModel { Color = "Red", Face = "TimesRoman;Arial" }; // feature: multi select
            var textElement = new TextElementDataModel
                                  {
                                      Value = "TestText",
                                      Font = font,
                                      Overflow = TextOverflow.ScrollRing // feature: ringscroll
                                  };
            dataModel.Elements.Add(textElement);
            layout.Resolutions.Add(new ResolutionConfigDataViewModel(shellMock.Object, dataModel));

            standardSectionConfigDataViewModel.Layout = layout;
            state.CurrentProject.InfomediaConfig.Layouts.Add(layout);

            var availableSoftware = AddTooLowAvailableVersions();

            var parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsTrue(hasIssues);
            Assert.IsTrue(messages.Count == 5); // two feature errors
        }

        /// <summary>
        /// Test if feature is used and version is too low
        /// </summary>
        [TestMethod]
        public void UsedLiveStreamElement()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var shellMock = Helpers.CreateMediaShell();
            shellMock.Setup(shell => shell.MediaApplicationState).Returns(state);

            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig(shellMock.Object);
            state.CurrentProject.InfomediaConfig = infomediaConfig;

            var liveStreamElement = new LiveStreamElementDataViewModel(shellMock.Object);
            infomediaConfig.Layouts.First().Resolutions.ToList().First().Elements.Add(liveStreamElement);

            var availableSoftware = AddTooLowAvailableVersions();
            var parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsTrue(hasIssues);
            Assert.IsTrue(messages.Count == 3);

            // recheck with one version fulfilled should add only one software error
            availableSoftware.First(e => e.Component == FeatureComponentRequirements.SoftwareComponent.DxRenderer)
                .Version = new SoftwareComponentVersion("99.99");
            parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };

            hasIssues = checker.Check(parameters, messages);
            Assert.IsTrue(hasIssues);

            // 5 messages because the clearing of the CompatibilityMessages shouldn't be done within the checker.
            Assert.IsTrue(messages.Count == 5);
        }

        /// <summary>
        /// Test if feature is used and version is too low
        /// </summary>
        [TestMethod]
        public void UsedRssElement()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var shellMock = Helpers.CreateMediaShell();
            shellMock.Setup(shell => shell.MediaApplicationState).Returns(state);

            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig(shellMock.Object);
            state.CurrentProject.InfomediaConfig = infomediaConfig;

            var rssElement = new RssTickerElementDataViewModel(shellMock.Object);

            infomediaConfig.Layouts.First().Resolutions.ToList().First().Elements.Add(rssElement);

            var availableSoftware = AddTooLowAvailableVersions();
            var parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsTrue(hasIssues);
            Assert.AreEqual(4, messages.Count);
        }

        /// <summary>
        /// Test if feature is used and version is too low
        /// </summary>
        [TestMethod]
        public void UsedSpecialFonts()
        {
            var checker = new CompatibilityChecker();

            var container = Helpers.InitializeServiceLocator();
            var state = Helpers.MockApplicationState(container);
            var shellMock = Helpers.CreateMediaShell();
            shellMock.Setup(shell => shell.MediaApplicationState).Returns(state);

            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig(shellMock.Object);
            state.CurrentProject.InfomediaConfig = infomediaConfig;

            var standardSectionConfigDataViewModel = AddLedLayoutToProject(shellMock, state);

            var layout = new LayoutConfigDataViewModel(shellMock.Object);
            state.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.First()
                .VirtualDisplays.First()
                .Reference.CyclePackage.StandardCycles.First()
                .Reference.Sections.First()
                .Layout = layout;

            var resource = new ResourceInfoDataViewModel
                               {
                                   Facename = "CUxFont",
                                   Type = ResourceType.Font,
                                   Filename = @"c:\CUxFont.CUx",
                                   LedFontType = LedFontType.CUxFont
                               };
            state.CurrentProject.Resources.Add(resource);

            var dataModel = new ResolutionConfigDataModel { DisplayText = "DisplayText", Height = 100, Width = 50 };
            var font = new FontDataModel { Color = "Red", Face = "CUxFont" };
            var textElement = new TextElementDataModel
            {
                Value = "TestText",
                Font = font,
            };

            dataModel.Elements.Add(textElement);
            layout.Resolutions.Add(new ResolutionConfigDataViewModel(shellMock.Object, dataModel));

            standardSectionConfigDataViewModel.Layout = layout;
            state.CurrentProject.InfomediaConfig.Layouts.Add(layout);

            var availableSoftware = AddTooLowAvailableVersions();

            var parameters = new CompatibilityCheckParameters
            {
                MediaApplicationState = state,
                SoftwareConfigs = availableSoftware
            };
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
            var hasIssues = checker.Check(parameters, messages);

            Assert.IsTrue(hasIssues);
            Assert.AreEqual(3, messages.Count);
        }

        private static StandardSectionConfigDataViewModel AddLedLayoutToProject(
            Mock<IMediaShell> shellMock,
            MediaApplicationState state)
        {
            var screen = new PhysicalScreenConfigDataViewModel(shellMock.Object);
            screen.Type.Value = PhysicalScreenType.LED;
            state.CurrentProject.InfomediaConfig.PhysicalScreens.Add(screen);

            var virtualDisplayRefDataModel = new VirtualDisplayRefConfigDataModel();
            var virtualDisplayRef = new VirtualDisplayRefConfigDataViewModel(
                shellMock.Object,
                virtualDisplayRefDataModel);

            var standardSectionConfigDataViewModel = new StandardSectionConfigDataViewModel(shellMock.Object);
            var standardCycle = new StandardCycleRefConfigDataViewModel(shellMock.Object);

            standardCycle.Reference = new StandardCycleConfigDataViewModel(shellMock.Object);
            standardCycle.Reference.Sections.Add(standardSectionConfigDataViewModel);
            virtualDisplayRef.Reference = new VirtualDisplayConfigDataViewModel(shellMock.Object);
            virtualDisplayRef.Reference.CyclePackage = new CyclePackageConfigDataViewModel(shellMock.Object);
            virtualDisplayRef.Reference.CyclePackage.StandardCycles.Add(standardCycle);

            var screenRef = new PhysicalScreenRefConfigDataViewModel(shellMock.Object);
            screenRef.Reference = screen;
            screenRef.VirtualDisplays.Add(virtualDisplayRef);

            state.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.Add(screenRef);
            return standardSectionConfigDataViewModel;
        }

        private static List<FeatureComponentRequirements.SoftwareConfig> AddTooLowAvailableVersions()
        {
            var availableSoftware = new List<FeatureComponentRequirements.SoftwareConfig>();

            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("0.1")));
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.DxRenderer,
                    new SoftwareComponentVersion("0.1")));
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Composer,
                    new SoftwareComponentVersion("0.1")));
            availableSoftware.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("0.1")));

            return availableSoftware;
        }
    }
}
