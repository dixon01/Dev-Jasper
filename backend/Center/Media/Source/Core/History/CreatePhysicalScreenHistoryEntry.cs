// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePhysicalScreenHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   History entry that contains all information to undo / redo the creation of a physical screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// History entry that contains all information to undo / redo the creation of a physical screen.
    /// </summary>
    public class CreatePhysicalScreenHistoryEntry : HistoryEntryBase
    {
        private readonly IMediaShell mediaShell;

        private readonly PhysicalScreenConfigDataViewModel physicalScreen;

        private readonly IEnumerable<CyclePackageConfigDataViewModel> cyclePackages;

        private readonly IEnumerable<VirtualDisplayConfigDataViewModel> virtualDisplays;

        private readonly IEnumerable<VirtualDisplayRefConfigDataViewModel> virtualRefDisplays;

        private readonly PhysicalScreenRefConfigDataViewModel physicalScreenRef;

        private readonly IPhysicalScreenController physicalScreenController;

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePhysicalScreenHistoryEntry"/> class.
        /// </summary>
        /// <param name="physicalScreenController">
        /// The physical Screen Controller.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="physicalScreen">
        /// The physical screen.
        /// </param>
        /// <param name="cyclePackages">
        /// The cycle packages.
        /// </param>
        /// <param name="virtualDisplays">
        /// The virtual displays.
        /// </param>
        /// <param name="virtualRefDisplays">
        /// The virtual display references.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public CreatePhysicalScreenHistoryEntry(
            IPhysicalScreenController physicalScreenController,
            ICommandRegistry commandRegistry,
            PhysicalScreenConfigDataViewModel physicalScreen,
            IEnumerable<CyclePackageConfigDataViewModel> cyclePackages,
            IEnumerable<VirtualDisplayConfigDataViewModel> virtualDisplays,
            IEnumerable<VirtualDisplayRefConfigDataViewModel> virtualRefDisplays,
            string displayText)
            : base(displayText)
        {
            this.mediaShell = physicalScreenController.MediaShell;
            this.physicalScreenController = physicalScreenController;
            this.physicalScreen = physicalScreen;
            this.cyclePackages = cyclePackages;
            this.virtualDisplays = virtualDisplays;
            this.virtualRefDisplays = virtualRefDisplays;
            this.commandRegistry = commandRegistry;
            this.physicalScreenRef = new PhysicalScreenRefConfigDataViewModel(this.mediaShell)
            {
                Reference = this.physicalScreen,
                VirtualDisplays =
                    new ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>()
            };

            foreach (var reference in this.virtualRefDisplays)
            {
                this.physicalScreenRef.VirtualDisplays.Add(reference);
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var index = 0;
            var existingCycleNames =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Select(
                        c => c.Name.Value).ToList();
            existingCycleNames.AddRange(
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.EventCycles.Select(
                    eventCycleConfigDataViewModel => eventCycleConfigDataViewModel.Name.Value));
            foreach (var cyclePackage in this.cyclePackages)
            {
                var virtualDisplay = this.virtualDisplays.FirstOrDefault(v => v.CyclePackage == cyclePackage);
                if (virtualDisplay == null)
                {
                    return;
                }

                var cyclePackageNamePrefix = string.Format(
                "{0} {1} {2}x{3}",
                MediaStrings.PhysicalScreenController_DefaultCyclePackageName,
                this.physicalScreen.Type.Value,
                virtualDisplay.Width.Value,
                virtualDisplay.Height.Value);
                cyclePackage.Name.Value =
                    this.physicalScreenController.GenerateCyclePackageName(cyclePackageNamePrefix);
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.CyclePackages.Add(cyclePackage);
                var cycle = cyclePackage.StandardCycles.First().Reference;
                var cycleIndex = 1;
                var name = cycle.Name.Value + cycleIndex;
                var isNameUnique = existingCycleNames.All(c => c != name);
                while (!isNameUnique)
                {
                    cycleIndex++;
                    name = cycle.Name.Value + cycleIndex;
                    isNameUnique = existingCycleNames.All(c => c != name);
                }

                cycle.Name.Value = name;
                existingCycleNames.Add(name);
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Add(
                    (StandardCycleConfigDataViewModel)cycle);
                var layout = (LayoutConfigDataViewModel)cycle.Sections.First().Layout;
                layout.Name.Value =
                    this.physicalScreenController.GenerateLayoutName(
                        layout.Resolutions.First().Width.Value,
                        layout.Resolutions.First().Height.Value,
                        index,
                        this.physicalScreen.Type.Value);
                if (this.physicalScreen.Type.Value == PhysicalScreenType.Audio)
                {
                    this.CreateAudioEventCycle(layout, cyclePackage, existingCycleNames, index);
                }

                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.Add(layout);
                index++;
            }

            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.Add(
                this.physicalScreen);
            if (this.physicalScreen.Type.Value == PhysicalScreenType.LED)
            {
                this.CreateLedFontResource(Settings.Default.DefaultAhdlcFontSmall);
                this.CreateLedFontResource(Settings.Default.DefaultAhdlcFontBig);
            }

            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                .First().PhysicalScreens.Add(this.physicalScreenRef);
            foreach (var virtualDisplay in this.virtualDisplays)
            {
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.VirtualDisplays
                        .Add(virtualDisplay);
            }

            this.physicalScreenController.Parent.ProjectController.RefreshLayoutUsageReferences(
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig);
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            foreach (var cyclePackage in this.cyclePackages)
            {
                var cycle = cyclePackage.StandardCycles.First().Reference;
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.StandardCycles.Remove(
                    (StandardCycleConfigDataViewModel)cycle);
                var layout = (LayoutConfigDataViewModel)cycle.Sections.First().Layout;
                if (this.mediaShell.MediaApplicationState.CurrentLayout == layout)
                {
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                        .Execute(
                            this.mediaShell.MediaApplicationState.CurrentLayout =
                            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                                l => !object.ReferenceEquals(layout, l)));
                }

                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.Remove(layout);
                if (this.physicalScreen.Type.Value == PhysicalScreenType.Audio)
                {
                    var eventCycle = cyclePackage.EventCycles.First().Reference;
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.EventCycles.Remove(
                        (EventCycleConfigDataViewModel)eventCycle);
                    var eventLayout = (LayoutConfigDataViewModel)eventCycle.Sections.First().Layout;
                    if (this.mediaShell.MediaApplicationState.CurrentLayout == eventLayout)
                    {
                        this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                            .Execute(
                                this.mediaShell.MediaApplicationState.CurrentLayout =
                                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts
                                    .FirstOrDefault(l => !object.ReferenceEquals(eventLayout, l)));
                    }

                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig
                           .Layouts.Remove(eventLayout);
                }

                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.CyclePackages.Remove(cyclePackage);
            }

            foreach (var virtualDisplay in this.virtualDisplays)
            {
                this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.VirtualDisplays
                        .Remove(virtualDisplay);
            }

            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.Remove(
                this.physicalScreen);
            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                .First().PhysicalScreens.Remove(this.physicalScreenRef);

            this.physicalScreenController.Parent.ProjectController.RefreshLayoutUsageReferences(
              this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig);
        }

        private void CreateAudioEventCycle(
          LayoutConfigDataViewModel layout,
          CyclePackageConfigDataViewModel cyclePackage,
          List<string> existingCycleNames,
          int index)
        {
            var firstResolution = layout.Resolutions.First();
            var eventCycle = cyclePackage.EventCycles.First().Reference;
            var eventCycleIndex = 1;
            var eventCycleName = eventCycle.Name.Value + eventCycleIndex;
            var isNameUnique = existingCycleNames.All(c => c != eventCycleName);
            while (!isNameUnique)
            {
                eventCycleIndex++;
                eventCycleName = eventCycle.Name.Value + eventCycleIndex;
                isNameUnique = existingCycleNames.All(c => c != eventCycleName);
            }

            eventCycle.Name.Value = eventCycleName;
            existingCycleNames.Add(eventCycleName);
            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles.EventCycles.Add(
                (EventCycleConfigDataViewModel)eventCycle);
            var eventCycleLayout = (LayoutConfigDataViewModel)eventCycle.Sections.First().Layout;
            eventCycleLayout.Name.Value =
                this.physicalScreenController.GenerateLayoutName(
                    firstResolution.Width.Value,
                    firstResolution.Height.Value,
                    index,
                    this.physicalScreen.Type.Value,
                    true);

            if (!this.ResolutionHasAudioOutputElement(eventCycleLayout.Resolutions.First()))
            {
                eventCycleLayout.Resolutions.First()
                    .Elements.Add(
                        new AudioOutputElementDataViewModel(this.mediaShell)
                            {
                                Volume = { Value = 80 },
                                ElementName =
                                    {
                                        Value = MediaStrings.ElementName_AudioOutput
                                    }
                            });
            }

            this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.Add(eventCycleLayout);
        }

        private bool ResolutionHasAudioOutputElement(ResolutionConfigDataViewModel resolution)
        {
            if (resolution.Elements.Count == 0)
            {
                return false;
            }

            if (resolution.Elements.Count == 1
                && resolution.Elements.First() is AudioOutputElementDataViewModel)
            {
                return true;
            }

            throw new Exception("Audio layout with more than one AudioOutputElementDataViewModel.");
        }

        private void CreateLedFontResource(string filename)
        {
            using (
            var stream =
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Gorba.Center.Media.Core.Resources" + @"." + filename))
            {
                if (stream == null)
                {
                    return;
                }

                var hash = ResourceHash.Create(stream);
                if (this.mediaShell.MediaApplicationState.CurrentProject.Resources.Any(r => r.Hash == hash))
                {
                    return;
                }

                var tempPath = Path.GetTempPath();
                var tempFileName = Path.Combine(tempPath, filename);
                if (!File.Exists(tempFileName))
                {
                    stream.Position = 0;
                    using (var tempFile = new FileStream(tempFileName, FileMode.Create))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            tempFile.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                var file = FileSystemManager.Local.GetFile(tempFileName);
                var parameters = new AddResourceParameters { Resources = new[] { file }, Type = ResourceType.Font };
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource).Execute(parameters);
            }
        }
    }
}
