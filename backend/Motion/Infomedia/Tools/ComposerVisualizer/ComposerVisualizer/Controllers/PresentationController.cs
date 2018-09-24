// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The presentation controller.
    /// </summary>
    public class PresentationController : ControllerBase, IController
    {
        private readonly PresentationManager presentationManager;

        private ICommandRegistry commandRegistry;

        private bool running;

        private PresentationsDataViewModel presentations;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationController"/> class.
        /// </summary>
        /// <param name="presentationManager">
        /// The presentation manager.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public PresentationController(PresentationManager presentationManager, ICommandRegistry commandRegistry)
        {
            this.PresentationTree = new PresentationTreeViewModel();
            this.presentationManager = presentationManager;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the presentation tree.
        /// </summary>
        public PresentationTreeViewModel PresentationTree { get; private set; }

        /// <summary>
        /// Starts the controller
        /// </summary>
        public void Run()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.CreateTreeView();
            this.PresentationTree.TreeViewRoot = this.presentations;
            this.presentationManager.ScreensUpdated += this.PresentationManagerOnScreensUpdated;
        }

        /// <summary>
        /// Stops the controller
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
        }

        /// <summary>
        /// Freezes or unfreezes the visualization of layout view
        /// </summary>
        /// <param name="isFrozen">
        /// Flag indicating when the visualization must be frozen or not.
        /// </param>
        public void FreezeVisualization(bool isFrozen)
        {
            if (isFrozen)
            {
                this.presentationManager.ScreensUpdated -= this.PresentationManagerOnScreensUpdated;
            }
            else
            {
                this.UpdatePresentationTree();
                this.presentationManager.ScreensUpdated += this.PresentationManagerOnScreensUpdated;
            }
        }

        private void CreateTreeView()
        {
            this.presentations = new PresentationsDataViewModel();
            foreach (var masterPresentationEngine in this.presentationManager.MasterEngines)
            {
                var physicalScreen = this.CreatePhysicalScreenProperties(masterPresentationEngine);
                physicalScreen.NodeVisible = true;

                foreach (var masterCycle in masterPresentationEngine.MasterCyleManager.StandardCycles)
                {
                    var cycle = this.PopulateMasterCycles(masterPresentationEngine, masterCycle);

                    physicalScreen.MasterCycles.Add(cycle);
                }

                foreach (var eventCycle in masterPresentationEngine.MasterCyleManager.EventCycleInfos)
                {
                    var cycle = this.PopulateEventMasterCycles(eventCycle);
                    physicalScreen.MasterEventCycles.Add(cycle);
                }

                this.presentations.PhysicalScreens.Add(physicalScreen);
            }
        }

        private MasterEventCycleDataViewModel PopulateEventMasterCycles(
            CycleManagerBase<MasterLayout>.EventCycleInfo eventCycle)
        {
            var eventCycleProperties = new MasterEventCycleDataViewModel();
            foreach (var evaluatorBase in eventCycle.Cycle.TriggerHandler.Evaluators)
            {
                eventCycleProperties.Trigger.Add(this.RetrieveEvaluators(evaluatorBase));
            }

            eventCycleProperties.Enabled = this.RetrieveEvaluators(eventCycle.Cycle.EnabledHandler.Evaluator);
            eventCycleProperties.Name = eventCycle.Cycle.Config.Name;
            eventCycleProperties.NodeVisible = eventCycle.WasTriggered;

            foreach (var section in eventCycle.Cycle.Sections)
            {
                var sectionProperties = new MasterSectionDataViewModel();
                sectionProperties.Name = "Master Section";
                sectionProperties.Duration = section.Config.Duration;
                sectionProperties.Enabled = this.RetrieveEvaluators(section.EnabledHandler.Evaluator);
                sectionProperties.Layout = section.Config.Layout;
                sectionProperties.NodeVisible = section.IsEnabled();
                var masterLayout = new MasterLayoutDataViewModel();
                masterLayout.Name = section.Config.Layout;
                masterLayout.NodeVisible = section.IsEnabled();
                sectionProperties.MasterLayouts.Add(masterLayout);
                eventCycleProperties.MasterSections.Add(sectionProperties);
            }

            return eventCycleProperties;
        }

        private MasterCycleDataViewModel PopulateMasterCycles(
            MasterPresentationEngine masterPresentationEngine,
            CycleBase<MasterLayout> masterCycle)
        {
            var cycle = new MasterCycleDataViewModel();
            cycle.Name = masterCycle.Config.Name;
            cycle.NodeVisible = masterCycle.IsActive;
            cycle.Enabled = this.RetrieveEvaluators(masterCycle.EnabledHandler.Evaluator);
            foreach (var masterSection in masterCycle.Sections)
            {
                var masterSec = new MasterSectionDataViewModel();
                masterSec.Name = "Master Section";
                masterSec.NodeVisible = masterSection.IsEnabled();
                masterSec.Enabled = this.RetrieveEvaluators(masterSection.EnabledHandler.Evaluator);
                masterSec.Layout = masterSection.Config.Layout;

                var masterLayout = new MasterLayoutDataViewModel();
                masterLayout.Name = masterSection.Config.Layout;
                masterLayout.NodeVisible = masterSection.IsEnabled();

                var virtualDisplayComposer = masterPresentationEngine.VirtualDisplayComposer;
                var virtualDisp = this.CreateVirtualDisplayProperties(virtualDisplayComposer);
                virtualDisp.NodeVisible = true;

                var packageCycleManager = virtualDisplayComposer.PresentationEngine.PackageCycleManager;
                foreach (var standardCycle in packageCycleManager.StandardCycles)
                {
                    var stdCycle = new CycleDataViewModel();
                    stdCycle.Name = standardCycle.Config.Name;
                    stdCycle.NodeVisible = standardCycle.IsActive;
                    stdCycle.Enabled = this.RetrieveEvaluators(standardCycle.EnabledHandler.Evaluator);
                    stdCycle.ImagePath = "/Images/cyclereference_16x16.png";
                    foreach (var section in standardCycle.Sections)
                    {
                        bool secEnabled = standardCycle.CurrentSection != null &&
                            standardCycle.CurrentSection.Equals(section) && standardCycle.IsActive;
                        var sec = this.CreateSectionProperties(section, secEnabled);
                        sec.Name = this.CheckSectionType(section, sec);
                        stdCycle.Sections.Add(sec);
                    }

                    virtualDisp.StandardCycles.Add(stdCycle);
                }

                foreach (var eventCycle in packageCycleManager.EventCycleInfos)
                {
                    var evntCycle = this.CreateEventCycleProperties(eventCycle);
                    foreach (var section in eventCycle.Cycle.Sections)
                    {
                        bool secEnabled = eventCycle.Cycle.CurrentSection != null &&
                            eventCycle.Cycle.CurrentSection.Equals(section) && eventCycle.Cycle.IsActive;
                        var sec = this.CreateSectionProperties(section, secEnabled);
                        sec.Name = this.CheckSectionType(section, sec);

                        evntCycle.Sections.Add(sec);
                    }

                    virtualDisp.EventCycles.Add(evntCycle);
                }

                masterLayout.VirtualDisplays.Add(virtualDisp);
                masterSec.MasterLayouts.Add(masterLayout);
                cycle.MasterSections.Add(masterSec);
            }

            return cycle;
        }

        private PhysicalScreenDataViewModel CreatePhysicalScreenProperties(
            MasterPresentationEngine masterPresentationEngine)
        {
            var physicalScreen = new PhysicalScreenDataViewModel();
            physicalScreen.Height = masterPresentationEngine.ScreenConfig.Height;
            physicalScreen.Identifier = masterPresentationEngine.ScreenConfig.Identifier;
            physicalScreen.Name = masterPresentationEngine.ScreenConfig.Name;
            physicalScreen.Type = masterPresentationEngine.ScreenConfig.Type;
            physicalScreen.Visible =
                this.RetrieveEvaluators(masterPresentationEngine.VisibleHandler.Evaluator);
            physicalScreen.Width = masterPresentationEngine.ScreenConfig.Width;
            physicalScreen.NodeVisible = true;
            return physicalScreen;
        }

        private VirtualDisplayDataViewModel CreateVirtualDisplayProperties(
            VirtualDisplayComposer virtualDisplayComposer)
        {
            var virtualDispProperties = new VirtualDisplayDataViewModel();
            virtualDispProperties.CyclePackage = virtualDisplayComposer.VirtualDisplay.CyclePackage;
            virtualDispProperties.Height = virtualDisplayComposer.VirtualDisplay.Height;
            virtualDispProperties.Width = virtualDisplayComposer.VirtualDisplay.Width;
            virtualDispProperties.Name = virtualDisplayComposer.VirtualDisplay.Name;
            return virtualDispProperties;
        }

        private SectionDataViewModel CreateSectionProperties(SectionBase<Page> section, bool secEnabled)
        {
            var sectionProperties = new SectionDataViewModel();
            sectionProperties.Duration = section.Config.Duration;
            sectionProperties.Enabled = this.RetrieveEvaluators(section.EnabledHandler.Evaluator);
            sectionProperties.Layout = section.Config.Layout;
            sectionProperties.NodeVisible = secEnabled;
            var sectionLayout = new LayoutDataViewModel();
            sectionLayout.Name = section.Config.Layout;
            sectionLayout.NodeVisible = secEnabled;
            sectionProperties.LayoutNode.Add(sectionLayout);
            return sectionProperties;
        }

        private EventCycleDataViewModel CreateEventCycleProperties(CycleManagerBase<Page>.EventCycleInfo cycleInfo)
        {
            var eventCycleProperties = new EventCycleDataViewModel();
            foreach (var evaluatorBase in cycleInfo.Cycle.TriggerHandler.Evaluators)
            {
                eventCycleProperties.Trigger.Add(this.RetrieveEvaluators(evaluatorBase));
            }

            eventCycleProperties.Enabled = this.RetrieveEvaluators(cycleInfo.Cycle.EnabledHandler.Evaluator);
            eventCycleProperties.Name = cycleInfo.Cycle.Config.Name;
            eventCycleProperties.NodeVisible = cycleInfo.WasTriggered;
            eventCycleProperties.ImagePath = "/Images/event_dark_16x16.png";
            return eventCycleProperties;
        }

        private string CheckSectionType(SectionBase<Page> sec, SectionDataViewModel sectionDataViewModel)
        {
            var section = sec as StandardSection;
            if (section != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionstandard_dark_16x16.png";
                return "Standard Section";
            }

            var multi = sec as MultiSection;
            if (multi != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionmulti_dark_16x16.png";
                return "Multi Section";
            }

            var webmedia = sec as WebmediaSection;
            if (webmedia != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionwebmedia_dark_16x16.png";
                return "Web Section";
            }

            var image = sec as ImageSection;
            if (image != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionimage_dark_16x16.png";
                return "Image Section";
            }

            var video = sec as VideoSection;
            if (video != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionvideo_dark_16x16.png";
                return "Video Section";
            }

            var pool = sec as PoolSection;
            if (pool != null)
            {
                sectionDataViewModel.ImagePath = "/Images/sectionpool_dark_16x16.png";
                return "Pool Section";
            }

            return "Unknown Section";
        }

        private void PresentationManagerOnScreensUpdated(object sender, ScreenChangesEventArgs screenChangesEventArgs)
        {
            this.UpdatePresentationTree();
        }

        private void UpdatePresentationTree()
        {
            foreach (var physicalScreenDataViewModel in this.PresentationTree.TreeViewRoot.PhysicalScreens)
            {
                foreach (var masterPresentationEngine in this.presentationManager.MasterEngines)
                {
                    if (!masterPresentationEngine.ScreenConfig.Name.Equals(physicalScreenDataViewModel.Name))
                    {
                        continue;
                    }

                    foreach (var masterCycle in masterPresentationEngine.MasterCyleManager.StandardCycles)
                    {
                        foreach (var masterCycleDataViewModel in physicalScreenDataViewModel.MasterCycles)
                        {
                            if (!masterCycle.Config.Name.Equals(masterCycleDataViewModel.Name))
                            {
                                continue;
                            }

                            masterCycleDataViewModel.NodeVisible = masterCycle.IsActive;
                            masterCycleDataViewModel.Enabled =
                                this.RetrieveEvaluators(masterCycle.EnabledHandler.Evaluator);
                            foreach (var section in masterCycle.Sections)
                            {
                                foreach (var masterSectionDataViewModel in masterCycleDataViewModel.MasterSections)
                                {
                                    var nodeVisible = masterCycle.CurrentSection != null
                                                      && masterCycle.CurrentSection.Equals(section)
                                                      && masterCycle.IsActive;
                                    masterSectionDataViewModel.NodeVisible = nodeVisible;
                                    var masterLayoutDataViewModel = masterSectionDataViewModel.MasterLayouts[0];
                                    masterLayoutDataViewModel.NodeVisible = nodeVisible;

                                    var virtualDisplayComposer = masterPresentationEngine.VirtualDisplayComposer;
                                    var packageCycleManager =
                                        virtualDisplayComposer.PresentationEngine.PackageCycleManager;

                                    foreach (var virtualDisplayDataViewModel
                                        in masterLayoutDataViewModel.VirtualDisplays)
                                    {
                                        foreach (var standardCycle in packageCycleManager.StandardCycles)
                                        {
                                            foreach (var cycleDataViewModel
                                                in virtualDisplayDataViewModel.StandardCycles)
                                            {
                                                if (!standardCycle.Config.Name.Equals(cycleDataViewModel.Name))
                                                {
                                                    continue;
                                                }

                                                this.RePopulateCyclesAndSections(cycleDataViewModel, standardCycle);
                                            }
                                        }

                                        foreach (var eventCycleInfo in packageCycleManager.EventCycleInfos)
                                        {
                                            foreach (var cycleDataViewModel in virtualDisplayDataViewModel.EventCycles)
                                            {
                                                if (!eventCycleInfo.Cycle.Config.Name.Equals(cycleDataViewModel.Name))
                                                {
                                                    continue;
                                                }

                                                this.RePopulateEventCyclesAndSections(
                                                    cycleDataViewModel,
                                                    eventCycleInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    this.RePopulateMasterEventCycles(masterPresentationEngine, physicalScreenDataViewModel);
                }
            }
        }

        private void RePopulateMasterEventCycles(
            MasterPresentationEngine masterPresentationEngine,
            PhysicalScreenDataViewModel physicalScreenDataViewModel)
        {
            foreach (var eventCycleInfo in masterPresentationEngine.MasterCyleManager.EventCycleInfos)
            {
                foreach (var masterEventCycleDataViewModel in physicalScreenDataViewModel.MasterEventCycles)
                {
                    if (!eventCycleInfo.Cycle.Config.Name.Equals(masterEventCycleDataViewModel.Name))
                    {
                        continue;
                    }

                    masterEventCycleDataViewModel.Trigger.Clear();
                    foreach (var evaluatorBase in eventCycleInfo.Cycle.TriggerHandler.Evaluators)
                    {
                        masterEventCycleDataViewModel.Trigger.Add(this.RetrieveEvaluators(evaluatorBase));
                    }

                    masterEventCycleDataViewModel.Enabled =
                        this.RetrieveEvaluators(eventCycleInfo.Cycle.EnabledHandler.Evaluator);
                    masterEventCycleDataViewModel.NodeVisible = eventCycleInfo.WasTriggered;

                    foreach (var section in eventCycleInfo.Cycle.Sections)
                    {
                        foreach (var masterSectionDataViewModel in masterEventCycleDataViewModel.MasterSections)
                        {
                            masterSectionDataViewModel.Enabled =
                                this.RetrieveEvaluators(section.EnabledHandler.Evaluator);
                            masterSectionDataViewModel.NodeVisible = section.IsEnabled();
                            var masterLayoutDataViewModel = masterSectionDataViewModel.MasterLayouts[0];
                            masterLayoutDataViewModel.NodeVisible = section.IsEnabled();
                        }
                    }
                }
            }
        }

        private void RePopulateEventCyclesAndSections(
            EventCycleDataViewModel cycleDataViewModel,
            CycleManagerBase<Page>.EventCycleInfo eventCycleInfo)
        {
            cycleDataViewModel.NodeVisible = eventCycleInfo.Cycle.IsActive;
            cycleDataViewModel.Enabled = this.RetrieveEvaluators(eventCycleInfo.Cycle.EnabledHandler.Evaluator);

            foreach (var sectionBase in eventCycleInfo.Cycle.Sections)
            {
                foreach (var sectionDataViewModel in cycleDataViewModel.Sections)
                {
                    if (!sectionBase.Config.Layout.Equals(sectionDataViewModel.Layout))
                    {
                        continue;
                    }

                    var visible = eventCycleInfo.Cycle.CurrentSection != null
                                  && eventCycleInfo.Cycle.CurrentSection.Equals(sectionBase)
                                  && eventCycleInfo.Cycle.IsActive;
                    sectionDataViewModel.NodeVisible = visible;
                    sectionDataViewModel.Enabled = this.RetrieveEvaluators(sectionBase.EnabledHandler.Evaluator);
                    var layoutDataViewModel = sectionDataViewModel.LayoutNode[0];
                    layoutDataViewModel.NodeVisible = visible;
                }
            }
        }

        private void RePopulateCyclesAndSections(CycleDataViewModel cycleDataViewModel, CycleBase<Page> standardCycle)
        {
            cycleDataViewModel.NodeVisible = standardCycle.IsActive;
            cycleDataViewModel.Enabled = this.RetrieveEvaluators(standardCycle.EnabledHandler.Evaluator);

            foreach (var sectionBase in standardCycle.Sections)
            {
                foreach (var sectionDataViewModel in cycleDataViewModel.Sections)
                {
                    if (!sectionBase.Config.Layout.Equals(sectionDataViewModel.Layout))
                    {
                        continue;
                    }

                    var visible = standardCycle.CurrentSection != null
                                  && standardCycle.CurrentSection.Equals(sectionBase)
                                  && standardCycle.IsActive;
                    sectionDataViewModel.NodeVisible = visible;
                    sectionDataViewModel.Enabled = this.RetrieveEvaluators(sectionBase.EnabledHandler.Evaluator);
                    var layoutDataViewModel = sectionDataViewModel.LayoutNode[0];
                    layoutDataViewModel.NodeVisible = visible;
                }
            }
        }
    }
}