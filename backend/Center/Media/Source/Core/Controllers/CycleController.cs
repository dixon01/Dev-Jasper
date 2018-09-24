// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CycleController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// The CycleController.
    /// </summary>
    public class CycleController : ICycleController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediaShell shell;

        private readonly IMediaShellController shellController;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleController"/> class.
        /// </summary>
        /// <param name="shellController">
        ///     The application controller.
        /// </param>
        /// <param name="shell">
        ///     The shell view model.
        /// </param>
        /// <param name="commandRegistry">The command registry.</param>
        public CycleController(
            IMediaShellController shellController, IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.shell = shell;
            this.shellController = shellController;
            this.CommandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.CreateNew,
                new RelayCommand<CycleType>(this.CreateNewCycle, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.CreateNew,
                new RelayCommand<CreateSectionParameters>(this.CreateNewSection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.Delete,
                new RelayCommand(this.DeleteCycle, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.Delete,
                new RelayCommand(this.DeleteSection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.Choose,
                new RelayCommand<CycleRefConfigDataViewModelBase>(this.ChooseCycle));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.Choose,
                new RelayCommand<SectionConfigDataViewModelBase>(this.ChooseSection));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.Clone,
                new RelayCommand<CycleRefConfigDataViewModelBase>(this.CloneCycle, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.Clone,
                new RelayCommand<SectionConfigDataViewModelBase>(this.CloneSection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.Rename,
                new RelayCommand<RenameReusableEntityParameters>(this.RenameCycle, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.Rename,
                new RelayCommand<RenameReusableEntityParameters>(this.RenameSection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.CyclePackage.Rename,
                new RelayCommand<RenameReusableEntityParameters>(this.RenameCyclePackage, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.CyclePackage.Choose,
                new RelayCommand<CyclePackageConfigDataViewModel>(this.ChooseCyclePackage));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.ShowCycleTypeSelection,
                new RelayCommand(this.ShowCycleTypeSelection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Section.ShowSectionTypeSelection,
                new RelayCommand(this.ShowSectionTypeSelection, this.UserHasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Cycle.CreateReference,
                new RelayCommand<CycleRefConfigDataViewModelBase>(
                    this.CreateCycleReference,
                    this.CanCreateCycleReference));
        }

        /// <summary>
        /// Gets or sets the command registry.
        /// </summary>
        /// <value>
        /// The command registry.
        /// </value>
        public ICommandRegistry CommandRegistry { get; set; }

        private bool UserHasWritePermission(CycleType cycleType)
        {
            return this.UserHasWritePermission();
        }

        private bool UserHasWritePermission(object obj)
        {
            return this.UserHasWritePermission();
        }

        private bool UserHasWritePermission()
        {
            return this.shell != null
                   && this.shell.MediaApplicationState != null
                   && this.shell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration);
        }

        private void ShowCycleTypeSelection()
        {
            Logger.Debug("Request to show the cycle type selection.");

            var prompt = new SelectCycleTypePrompt(this.CommandRegistry);

            InteractionManager<SelectCycleTypePrompt>.Current.Raise(prompt);
        }

        private void ShowSectionTypeSelection()
        {
            Logger.Debug("Request to show the section type selection.");
            var layouts = this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts;
            var prompt = new SelectSectionTypePrompt(this.CommandRegistry, layouts);

            InteractionManager<SelectSectionTypePrompt>.Current.Raise(prompt);
        }

        private void CreateNewCycle(CycleType cycleType)
        {
            var currentCyclePackage = this.shell.CycleNavigator.CurrentCyclePackage;
            if (currentCyclePackage != null)
            {
                var currentProject = this.shell.MediaApplicationState.CurrentProject;
                var currentVirtualDisplay = this.shell.MediaApplicationState.CurrentVirtualDisplay;
                var firstLayout =
                    currentProject.InfomediaConfig.Layouts.FirstOrDefault(
                        l =>
                        l.IndexedResolutions[currentVirtualDisplay.Width.Value, currentVirtualDisplay.Height.Value]
                        != null);
                CycleRefConfigDataViewModelBase cycleReference;
                if (cycleType == CycleType.StandardCycle)
                {
                    cycleReference = this.CreateStandardCycle(currentProject, firstLayout);
                }
                else
                {
                    cycleReference = this.CreateEventCycle(currentProject, firstLayout);
                }

                var historyEntry = new CreateCycleHistoryEntry(
                        cycleReference,
                        currentCyclePackage,
                        currentProject,
                        this.shell.CycleNavigator,
                        MediaStrings.CycleController_CreateCycleHistoryEntry);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
            else
            {
                Logger.Warn("Tried to create a new cycle without selecting a cyclepackage before");

                // TODO: message box?
            }
        }

        private CycleRefConfigDataViewModelBase CreateEventCycle(
            MediaProjectDataViewModel currentProject,
            LayoutConfigDataViewModel firstLayout)
        {
            var cycleList = currentProject.InfomediaConfig.Cycles.EventCycles;

            var newNumber = 1;
            var cycleName = MediaStrings.CycleController_NewEventCycleName + newNumber;
            while (cycleList.Any(c => c.Name.Value == cycleName))
            {
                newNumber += 1;
                cycleName = MediaStrings.CycleController_NewEventCycleName + newNumber;
            }

            var cycle = new EventCycleConfigDataViewModel(this.shell) { Name = new DataValue<string>(cycleName) };

            cycle.Sections.Add(
                new StandardSectionConfigDataViewModel(this.shell)
                    {
                        Layout = firstLayout,
                        Duration = { Value = TimeSpan.FromSeconds(1) }
                    });
            var cycleReference = new EventCycleRefConfigDataViewModel(this.shell)
                                     {
                                         Reference = cycle,
                                         IsInEditMode = true
                                     };
            return cycleReference;
        }

        private CycleRefConfigDataViewModelBase CreateStandardCycle(
            MediaProjectDataViewModel currentProject,
            LayoutConfigDataViewModel firstLayout)
        {
            var cycleList = currentProject.InfomediaConfig.Cycles.StandardCycles;

            var newNumber = 1;
            var cycleName = MediaStrings.CycleController_NewCycleName + newNumber;
            while (cycleList.Any(c => c.Name.Value == cycleName))
            {
                newNumber += 1;
                cycleName = MediaStrings.CycleController_NewCycleName + newNumber;
            }

            var cycle = new StandardCycleConfigDataViewModel(this.shell) { Name = new DataValue<string>(cycleName) };

            cycle.Sections.Add(
                new StandardSectionConfigDataViewModel(this.shell)
                    {
                        Layout = firstLayout,
                        Duration = { Value = TimeSpan.FromSeconds(10) }
                    });

            var cycleReference = new StandardCycleRefConfigDataViewModel(this.shell)
                                     {
                                         Reference = cycle,
                                         IsInEditMode = true,
                                     };
            return cycleReference;
        }

        private void CreateNewSection(CreateSectionParameters sectionParameters)
        {
            var currentCycle = this.shell.CycleNavigator.CurrentCycle;
            if (currentCycle == null)
            {
                Logger.Warn("Tried to create a new section without selecting a cycle before");

                // TODO: message box?
                return;
            }

            LayoutConfigDataViewModel layout;
            if (sectionParameters.Layout != null)
            {
                layout = sectionParameters.Layout;
            }
            else
            {
                layout = this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault();
            }

            SectionConfigDataViewModelBase section;
            switch (sectionParameters.SectionType)
            {
                case SectionType.Standard:
                    section = new StandardSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                case SectionType.Image:
                    section = new ImageSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                case SectionType.Video:
                    section = new VideoSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                case SectionType.Pool:
                    section = new PoolSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                case SectionType.Multi:
                    section = new MultiSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                case SectionType.Dynamic:
                    section = new WebmediaSectionConfigDataViewModel(this.shell)
                              {
                                  Layout = layout,
                                  IsInEditMode = true,
                              };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            section.Duration.Value = currentCycle is EventCycleConfigDataViewModel
                                         ? TimeSpan.FromSeconds(1)
                                         : TimeSpan.FromSeconds(10);
            var historyEntry = new CreateSectionHistoryEntry(
                section,
                currentCycle,
                this.CommandRegistry,
                MediaStrings.CycleController_CreateSectionHistoryEntry);
            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void CreateCycleReference(CycleRefConfigDataViewModelBase cycleRef)
        {
            Logger.Debug("Request to create a new cycle reference.");
            if (cycleRef == null)
            {
                return;
            }

            var standardCycleRef = cycleRef as StandardCycleRefConfigDataViewModel;
            CycleRefConfigDataViewModelBase reference = null;
            if (standardCycleRef != null)
            {
                reference = (StandardCycleRefConfigDataViewModel)standardCycleRef.Clone();
            }
            else
            {
                var eventCycleRef = cycleRef as EventCycleRefConfigDataViewModel;
                if (eventCycleRef != null)
                {
                    reference = (EventCycleRefConfigDataViewModel)eventCycleRef.Clone();
                }
            }

            if (reference == null)
            {
                Logger.Trace("No cycle passed as parameter.");
                return;
            }

            Logger.Trace("Create a new cycle reference for cycle '{0}'.", reference.Name.Value);
            var historyEntry = new CreateCycleReferenceHistoryEntry(
                this.shell,
                reference,
                this.shell.MediaApplicationState.CurrentCyclePackage,
                MediaStrings.CycleController_CreateCycleReferenceHistoryEntry);
            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private bool CanCreateCycleReference(CycleRefConfigDataViewModelBase cycleRef)
        {
            if (cycleRef == null)
            {
                return false;
            }

            var standardCycleRef = cycleRef as StandardCycleRefConfigDataViewModel;
            if (standardCycleRef != null)
            {
                if (this.shell.MediaApplicationState.CurrentCyclePackage != null
                    && this.shell.MediaApplicationState.CurrentCyclePackage.StandardCycles != null)
                {
                    return
                        this.shell.MediaApplicationState.CurrentCyclePackage.StandardCycles.All(
                            cycle => cycle.Reference != standardCycleRef.Reference) && this.UserHasWritePermission();
                }
            }

            var eventCycleRef = cycleRef as EventCycleRefConfigDataViewModel;
            if (eventCycleRef != null)
            {
                if (this.shell.MediaApplicationState.CurrentCyclePackage != null
                    && this.shell.MediaApplicationState.CurrentCyclePackage.EventCycles != null)
                {
                    return
                        this.shell.MediaApplicationState.CurrentCyclePackage.EventCycles.All(
                            cycle => cycle.Reference != eventCycleRef.Reference) && this.UserHasWritePermission();
                }
            }

            return false;
        }

        private void DeleteCycle(object obj)
        {
            if (!(obj is IEnumerable) && obj is CycleRefConfigDataViewModelBase)
            {
                obj = new List<CycleRefConfigDataViewModelBase> { (CycleRefConfigDataViewModelBase)obj };
            }

            var cycles = obj as List<CycleRefConfigDataViewModelBase>;

            if (cycles != null)
            {
                var currentCyclePackage = this.shell.MediaApplicationState.CurrentCyclePackage;

                if (currentCyclePackage != null)
                {
                    var historyEntry = new DeleteCyclesHistoryEntry(
                        cycles,
                        currentCyclePackage,
                        this.shell.MediaApplicationState.CurrentProject,
                        this.shell.CycleNavigator,
                        this.CommandRegistry,
                        MediaStrings.CycleController_DeleteCycleHistoryEntry);
                    this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
                }
            }
        }

        private void DeleteSection(object obj)
        {
            if (!(obj is IEnumerable) && obj is SectionConfigDataViewModelBase)
            {
                obj = new List<SectionConfigDataViewModelBase> { (SectionConfigDataViewModelBase)obj };
            }

            var sections = obj as List<SectionConfigDataViewModelBase>;

            if (sections != null && sections.Count > 0)
            {
                var currentCycle = this.shell.MediaApplicationState.CurrentCycle;
                if (currentCycle != null)
                {
                    if (currentCycle.Sections.Count > 1)
                    {
                        var historyEntry = new DeleteSectionsHistoryEntry(
                            sections,
                            currentCycle,
                            this.shell.MediaApplicationState.CurrentProject,
                            this.shell.CycleNavigator,
                            this.CommandRegistry,
                            MediaStrings.CycleController_DeleteSectionHistoryEntry);
                        this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
                    }
                    else
                    {
                        MessageBox.Show(MediaStrings.CycleController_CantDeleteLastSection);
                    }
                }
            }
        }

        private void ChooseCycle(CycleRefConfigDataViewModelBase cycle)
        {
            Logger.Debug("Request to select a cycle.");
            if (cycle != null)
            {
                var currentCycle = this.shell.MediaApplicationState.CurrentCycle;
                Logger.Trace(
                   "Changing cycle from {0} to {1}.",
                   currentCycle != null ? currentCycle.Name.Value : "<null>",
                   cycle.Reference.Name.Value);
                this.shell.CycleNavigator.CurrentCycle = cycle.Reference;

                var section = cycle.Reference.Sections.FirstOrDefault();
                this.shell.CycleNavigator.CurrentSection = section;
                var cmd = this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
                if (cmd != null)
                {
                    cmd.Execute(section != null ? section.Layout : null);
                }
            }
            else
            {
                this.shell.CycleNavigator.CurrentCycle = null;
                this.shell.CycleNavigator.CurrentSection = null;
            }
        }

        private void ChooseSection(SectionConfigDataViewModelBase section)
        {
            Logger.Debug("Request to select a section.");
            if (section != null)
            {
                Logger.Trace(
                       "Changing section from {0} to {1}",
                       this.shell.MediaApplicationState.CurrentSection.Name,
                       section.Name);
            }

            this.shell.CycleNavigator.CurrentSection = section;

            var cmd = this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
            if (cmd != null)
            {
                cmd.Execute(section != null ? section.Layout : null);
            }
        }

        private void ChooseCyclePackage(CyclePackageConfigDataViewModel cyclePackage)
        {
            Logger.Debug("Request to select a cycle package.");
            if (cyclePackage != null)
            {
                Logger.Trace(
                    "Changing cycle package from {0} to {1}.",
                    this.shell.MediaApplicationState.CurrentCyclePackage.Name.Value,
                    cyclePackage.Name.Value);
                var physicalScreen = this.shell.MediaApplicationState.CurrentPhysicalScreen;
                var physicalScreenRef =
                    this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                        .First().PhysicalScreens.FirstOrDefault(s => s.Reference == physicalScreen);
                if (physicalScreenRef != null)
                {
                    if (physicalScreenRef.VirtualDisplays.All(d => d.Reference.CyclePackage != cyclePackage))
                    {
                        var screens =
                            this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation
                                .MasterLayouts.First().PhysicalScreens;
                        foreach (var screen in screens)
                        {
                            var virtualDisplay =
                                screen.VirtualDisplays.FirstOrDefault(
                                    display => display.Reference.CyclePackage == cyclePackage);
                            if (virtualDisplay != null)
                            {
                                this.shell.MediaApplicationState.CurrentPhysicalScreen = screen.Reference;
                                this.shell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplay.Reference;
                                break;
                            }
                        }
                    }
                }

                this.shell.CycleNavigator.CurrentCyclePackage = cyclePackage;
                var cycleRef = cyclePackage.StandardCycles.FirstOrDefault();
                if (cycleRef != null)
                {
                    this.shell.CycleNavigator.CurrentCycle = cycleRef.Reference;
                    if (cycleRef.Reference != null)
                    {
                        var section = cycleRef.Reference.Sections.FirstOrDefault();
                        this.shell.CycleNavigator.CurrentSection = section;
                        var cmd = this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
                        if (cmd != null)
                        {
                            cmd.Execute(section != null ? section.Layout : null);
                        }
                    }
                }
            }
            else
            {
                this.shell.CycleNavigator.CurrentCyclePackage = null;
                this.shell.CycleNavigator.CurrentCycle = null;
                this.shell.CycleNavigator.CurrentSection = null;
            }
        }

        private void CloneCycle(CycleRefConfigDataViewModelBase cycle)
        {
            if (cycle == null)
            {
                return;
            }

            var currentProject = this.shell.MediaApplicationState.CurrentProject;
            var currentCyclePackage = this.shell.CycleNavigator.CurrentCyclePackage;

            var historyEntry = new CloneCycleHistoryEntry(
                cycle,
                currentCyclePackage,
                currentProject,
                this.shell,
                this.shell.CycleNavigator,
                MediaStrings.CycleController_CloneCycleHistoryEntry);
            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void CloneSection(SectionConfigDataViewModelBase section)
        {
            if (section == null)
            {
                return;
            }

            var currentProject = this.shell.MediaApplicationState.CurrentProject;
            var currentCycle = this.shell.CycleNavigator.CurrentCycle;

            var historyEntry = new CloneSectionHistoryEntry(
                section,
                currentCycle,
                currentProject,
                this.shell,
                this.shell.CycleNavigator,
                MediaStrings.CycleController_CloneSectionHistoryEntry);
            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void RenameSection(RenameReusableEntityParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            var section = parameters.Entity as SectionConfigDataViewModelBase;
            if (section != null && parameters.NewName != section.Name)
            {
                var historyEntry = new RenameSectionHistoryEntry(
                    section,
                    parameters.NewName,
                    MediaStrings.CycleController_RenameSectionHistoryEntry);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        private void RenameCycle(RenameReusableEntityParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            var cycle = parameters.Entity as CycleRefConfigDataViewModelBase;
            if (cycle != null && parameters.NewName != cycle.Reference.Name.Value)
            {
                var historyEntry = new RenameCycleHistoryEntry(
                    cycle,
                    parameters.NewName,
                    MediaStrings.CycleController_RenameCycleHistoryEntry);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        private void RenameCyclePackage(RenameReusableEntityParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            var cyclePackage = parameters.Entity as CyclePackageConfigDataViewModel;
            if (cyclePackage != null && parameters.NewName != cyclePackage.Name.Value)
            {
                var historyEntry = new RenameCyclePackageHistoryEntry(
                    cyclePackage,
                    parameters.NewName,
                    MediaStrings.CycleController_RenameCyclePackageHistoryEntry);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }
    }
}