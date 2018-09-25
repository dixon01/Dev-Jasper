// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NavigationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// The navigation controller.
    /// </summary>
    public class NavigationController : INavigationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly IMediaShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        /// <param name="shell">
        ///     The shell.
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry.
        /// </param>
        public NavigationController(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.shell = shell;

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Layout.NavigateTo,
                new RelayCommand<LayoutConfigDataViewModel>(this.NavigateTo));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Navigation.GoToConsistencyProblem,
                new RelayCommand<ConsistencyMessageDataViewModel>(this.GoToConsistencyProblem));
        }

        private void NavigateToDynamicDataValue(ConsistencyMessageDataViewModel msg)
        {
            var layoutElement = msg.SourceParent as LayoutElementDataViewModelBase;
            if (layoutElement != null)
            {
                var element = layoutElement;
                this.NavigateTo(element, (IDynamicDataValue)msg.Source);
            }

            var cycleConfig = msg.SourceParent as CycleConfigDataViewModelBase;
            if (cycleConfig != null)
            {
                var layout =
                    this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.FirstOrDefault(
                        l =>
                        l.CycleSectionReferences.Any(
                            c => c.CycleReference == cycleConfig));
                if (layout != null)
                {
                    this.NavigateTo(layout);
                }

                this.ShowCycleNavigation();
            }

            var sectionConfig = msg.SourceParent as SectionConfigDataViewModelBase;
            if (sectionConfig != null)
            {
                this.NavigateTo((LayoutConfigDataViewModel)sectionConfig.Layout);
                this.NavigateTo(sectionConfig);
                this.ShowSectionNavigation();
            }
        }

        private void NavigateTo(CycleConfigDataViewModelBase cycle)
        {
            this.shell.MediaApplicationState.CurrentCycle = cycle;

            this.SelectCycleInCycleNavigationCurrentPackage(cycle);
        }

        private void NavigateTo(LayoutConfigDataViewModel layout)
        {
            if (layout == null)
            {
                Logger.Debug("No layout to navigate to.");
                return;
            }

            this.shell.MediaApplicationState.CurrentLayout = layout;
            var currentVirtualDisplay = this.shell.MediaApplicationState.CurrentVirtualDisplay;
            var resolution =
                layout.IndexedResolutions[currentVirtualDisplay.Width.Value, currentVirtualDisplay.Height.Value];
            if (resolution != null)
            {
                var physicalScreenRef = this.GetPhysicalScreenRef(
                    this.shell.MediaApplicationState.CurrentPhysicalScreen);
                ((MediaShell)this.shell).SetCurrentEditor(physicalScreenRef.Reference.Type.Value);
                this.LoadResolutionElements(resolution);

                var currentCycle = this.shell.MediaApplicationState.CurrentCycle;
                var currentSection = this.shell.MediaApplicationState.CurrentSection;
                var sectionReference =
                    layout.CycleSectionReferences.FirstOrDefault(
                        r => r.CycleReference == currentCycle && r.SectionReference == currentSection);
                if (sectionReference == null)
                {
                    sectionReference = layout.CycleSectionReferences.FirstOrDefault();
                    if (sectionReference != null)
                    {
                        this.NavigateTo(sectionReference);
                    }

                    return;
                }

                if (currentVirtualDisplay.CyclePackage != this.shell.MediaApplicationState.CurrentCyclePackage)
                {
                    var virtualDisplayRef = physicalScreenRef.VirtualDisplays.FirstOrDefault(
                        d => d.Reference.CyclePackage == this.shell.MediaApplicationState.CurrentCyclePackage)
                                            ?? physicalScreenRef.VirtualDisplays.FirstOrDefault();

                    this.NavigateTo(virtualDisplayRef);
                }
            }
            else
            {
                this.NavigateToFirstResolution(layout);
            }
        }

        private void NavigateToFirstResolution(LayoutConfigDataViewModel layout)
        {
            var resolution = layout.Resolutions.FirstOrDefault();
            if (resolution == null)
            {
                Logger.Error("Layout {0} has no resolutions defined!", layout.Name.Value);
                return;
            }

            var physicalScreenRef = this.GetPhysicalScreenRef(resolution, layout.Name.Value);
            if (physicalScreenRef == null)
            {
                physicalScreenRef = this.GetFirstPhysicalScreenRef(resolution);
                if (physicalScreenRef == null)
                {
                    return;
                }
            }

            ((MediaShell)this.shell).SetCurrentEditor(physicalScreenRef.Reference.Type.Value);
            this.shell.MediaApplicationState.CurrentPhysicalScreen = physicalScreenRef.Reference;
            this.LoadResolutionElements(resolution);

            var sectionReference = layout.CycleSectionReferences.FirstOrDefault();
            if (sectionReference != null)
            {
                this.NavigateTo(sectionReference);
                if (physicalScreenRef.Reference.Type.Value == PhysicalScreenType.LED)
                {
                    InteractionManager<UpdateLedDisplayPrompt>.Current.Raise(new UpdateLedDisplayPrompt());
                }

                return;
            }

            var virtualDisplayRef = physicalScreenRef.VirtualDisplays.FirstOrDefault();
            this.NavigateTo(virtualDisplayRef);
            if (physicalScreenRef.Reference.Type.Value == PhysicalScreenType.LED)
            {
                InteractionManager<UpdateLedDisplayPrompt>.Current.Raise(new UpdateLedDisplayPrompt());
            }
        }

        private void NavigateTo(SectionConfigDataViewModelBase section)
        {
            this.shell.MediaApplicationState.CurrentSection = section;
            var cycles = this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Cycles;
            CycleConfigDataViewModelBase cycle =
                cycles.StandardCycles.FirstOrDefault(c => c.Sections.Contains(section))
                ?? (CycleConfigDataViewModelBase)cycles.EventCycles.FirstOrDefault(c => c.Sections.Contains(section));

            if (cycle != null)
            {
                this.shell.MediaApplicationState.CurrentCycle = cycle;
                if (
                    cycle.CyclePackageReferences.All(
                        package => package != this.shell.MediaApplicationState.CurrentCyclePackage))
                {
                    this.shell.MediaApplicationState.CurrentCyclePackage =
                        cycle.CyclePackageReferences.FirstOrDefault();
                }
            }
        }

        private void NavigateTo(TextualReplacementDataViewModel replacement)
        {
            this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu).Execute(replacement);
        }

        private void NavigateTo(EvaluationConfigDataViewModel predefinedFormula)
        {
            this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu).Execute(predefinedFormula);
        }

        private void NavigateTo(LayoutCycleSectionRefDataViewModel sectionReference)
        {
            var currentProject = this.shell.MediaApplicationState.CurrentProject;

            var virtualDisplay =
                   this.shell.MediaApplicationState.CurrentVirtualDisplayReferences.FirstOrDefault(
                       display =>
                       display.Reference.CyclePackage.StandardCycles.Any(
                           c =>
                           c.Reference == sectionReference.CycleReference
                           && c.Reference.Sections.Any(s => s == sectionReference.SectionReference))
                       || display.Reference.CyclePackage.EventCycles.Any(
                           c =>
                           c.Reference == sectionReference.CycleReference
                           && c.Reference.Sections.Any(s => s == sectionReference.SectionReference)));
            if (virtualDisplay != null)
            {
                this.shell.MediaApplicationState.CurrentCyclePackage = virtualDisplay.Reference.CyclePackage;
                this.shell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplay.Reference;
                this.NavigateTo(sectionReference.CycleReference);
                this.NavigateTo(sectionReference.SectionReference);
                return;
            }

            var cyclePackage =
                currentProject.InfomediaConfig.CyclePackages.FirstOrDefault(
                    package =>
                    package.StandardCycles.Any(
                        c =>
                        c.Reference == sectionReference.CycleReference
                        && c.Reference.Sections.Any(s => s == sectionReference.SectionReference))
                        || package.EventCycles.Any(c => c.Reference == sectionReference.CycleReference
                        && c.Reference.Sections.Any(s => s == sectionReference.SectionReference)));

            this.shell.CycleNavigator.CurrentCyclePackage = cyclePackage;

            var physicalScreenRef =
                this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts[0]
                    .PhysicalScreens.FirstOrDefault(
                        p =>
                        p.VirtualDisplays.Any(
                            d => cyclePackage != null && d.Reference.CyclePackageName == cyclePackage.Name.Value));
            if (physicalScreenRef != null)
            {
                this.shell.MediaApplicationState.CurrentPhysicalScreen = physicalScreenRef.Reference;
                var virtualDisplayRef =
                    physicalScreenRef.VirtualDisplays.FirstOrDefault(
                        reference => cyclePackage != null
                            && reference.Reference.CyclePackageName == cyclePackage.Name.Value);
                if (virtualDisplayRef != null)
                {
                    this.shell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplayRef.Reference;
                }
            }

            this.NavigateTo(sectionReference.CycleReference);
            this.NavigateTo(sectionReference.SectionReference);
        }

        private void NavigateTo(LayoutElementDataViewModelBase element, IDynamicDataValue dynamicDataValue = null)
        {
            var done = false;

            foreach (var layout in this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts)
            {
                foreach (var resolution in layout.Resolutions)
                {
                    if (element is AudioElementDataViewModelBase)
                    {
                        var elementsContainer =
                            resolution.Elements.FirstOrDefault(e => e is AudioOutputElementDataViewModel);

                        if (elementsContainer != null)
                        {
                            var audioOutputElement = (AudioOutputElementDataViewModel)elementsContainer;
                            if (audioOutputElement == element
                                || audioOutputElement.Elements.Any(audioElement => audioElement == element))
                            {
                                this.NavigateTo(layout);
                            }

                            this.shell.Editor.SelectedElements.Clear();
                            this.shell.Editor.SelectedElements.Add(element);

                            if (dynamicDataValue != null)
                            {
                                this.ShowFormulaEditor(element, dynamicDataValue);
                            }

                            done = true;
                        }
                    }
                    else
                    {
                        if (resolution.Elements.Any(layoutElement => layoutElement == element))
                        {
                            this.NavigateTo(layout);

                            // TODO: navigate to resolution
                            this.shell.Editor.SelectedElements.Clear();
                            this.shell.Editor.SelectedElements.Add(element);

                            if (dynamicDataValue != null)
                            {
                                this.ShowFormulaEditor(element, dynamicDataValue);
                            }

                            done = true;
                        }
                    }

                    if (done)
                    {
                        break;
                    }
                }

                if (done)
                {
                    break;
                }
            }
        }

        private void NavigateTo(VirtualDisplayRefConfigDataViewModel virtualDisplayRef)
        {
            if (virtualDisplayRef != null)
            {
                this.shell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplayRef.Reference;
                this.shell.CycleNavigator.CurrentCyclePackage = virtualDisplayRef.Reference.CyclePackage;
                var cycle = virtualDisplayRef.Reference.CyclePackage.StandardCycles.FirstOrDefault();
                if (cycle != null)
                {
                    this.shell.CycleNavigator.CurrentCycle = cycle.Reference;
                    this.shell.CycleNavigator.CurrentSection = cycle.Reference.Sections.FirstOrDefault();
                }
            }
        }

        private void NavigateTo(PhysicalScreenConfigDataViewModel physicalScreen)
        {
            if (physicalScreen != null)
            {
                if (physicalScreen == this.shell.MediaApplicationState.CurrentPhysicalScreen)
                {
                    this.ShowPhysicalScreenDialog();
                    return;
                }

                this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.Choose)
                    .Execute(physicalScreen);
                this.ShowPhysicalScreenDialog();
            }
        }

        private void GoToConsistencyProblem(ConsistencyMessageDataViewModel msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }

            var currentProject = this.shell.MediaApplicationState.CurrentProject;

            if (msg.Source is ResolutionConfigDataViewModel)
            {
                var resolution = (ResolutionConfigDataViewModel)msg.Source;
                var layouts = currentProject.InfomediaConfig.Layouts;
                var layout = layouts.FirstOrDefault(l => l.Resolutions.Contains(resolution));
                if (layout != null)
                {
                    this.NavigateTo(layout);
                }
            }
            else if (msg.Source is LayoutConfigDataViewModel)
            {
                var layoutSource = (LayoutConfigDataViewModel)msg.Source;
                this.NavigateTo(layoutSource);
            }
            else if (msg.Source is CycleConfigDataViewModelBase)
            {
                this.NavigateTo((CycleConfigDataViewModelBase)msg.Source);
                this.ShowCycleNavigation();
            }
            else if (msg.Source is IDynamicDataValue)
            {
                this.NavigateToDynamicDataValue(msg);
            }
            else if (msg.Source is TextualReplacementDataViewModel)
            {
                this.NavigateTo((TextualReplacementDataViewModel)msg.Source);
            }
            else if (msg.Source is LayoutElementDataViewModelBase)
            {
                this.NavigateTo((LayoutElementDataViewModelBase)msg.Source);
            }
            else if (msg.Source is EvaluationConfigDataViewModel)
            {
                this.NavigateTo((EvaluationConfigDataViewModel)msg.Source);
            }
            else if (msg.Source is SectionConfigDataViewModelBase)
            {
                this.ShowSectionNavigation();
                this.NavigateTo((SectionConfigDataViewModelBase)msg.Source);
            }
            else if (msg.Source == currentProject.InfomediaConfig.CyclePackages)
            {
                // TODO: this.ShowCyclePackageNavigation();
            }
            else if (msg.Source == currentProject.InfomediaConfig.Cycles)
            {
                this.ShowCycleNavigation();
            }
            else if (msg.Source == currentProject.InfomediaConfig.Cycles.StandardCycles)
            {
                this.ShowCycleNavigation();
            }
            else if (msg.Source == currentProject.InfomediaConfig.Layouts)
            {
                this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowLayoutNavigation).Execute(null);
            }
            else if (msg.Source == currentProject.InfomediaConfig.VirtualDisplays)
            {
                // TODO: this.ShowVirtualDisplayNavigation();
            }
            else if (msg.Source is PhysicalScreenConfigDataViewModel)
            {
                this.NavigateTo((PhysicalScreenConfigDataViewModel)msg.Source);
            }
            else if (msg.Source is GenericTriggerConfigDataViewModel)
            {
                var cycle = (EventCycleConfigDataViewModel)msg.SourceParent;
                this.NavigateTo((LayoutConfigDataViewModel)cycle.Sections.First().Layout);
                this.ShowCycleNavigation();
            }
        }

        private void SelectCycleInCycleNavigationCurrentPackage(CycleConfigDataViewModelBase cycle)
        {
            var newHighlightedStandardCycle =
                this.shell.CycleNavigator.CurrentCyclePackage.StandardCycles.FirstOrDefault(
                    c => object.ReferenceEquals(c.Reference, cycle));
            if (newHighlightedStandardCycle != null)
            {
                this.shell.CycleNavigator.HighlightedCycle = newHighlightedStandardCycle;
                return;
            }

            var newHighlightedEventCycle =
                this.shell.CycleNavigator.CurrentCyclePackage.EventCycles.FirstOrDefault(
                    c => object.ReferenceEquals(c.Reference, cycle));
            if (newHighlightedEventCycle != null)
            {
                this.shell.CycleNavigator.HighlightedCycle = newHighlightedEventCycle;
            }
        }

        private PhysicalScreenRefConfigDataViewModel GetFirstPhysicalScreenRef(ResolutionConfigDataViewModel resolution)
        {
            var masterLayout =
               this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                   .FirstOrDefault();
            if (masterLayout == null)
            {
                return null;
            }

            return masterLayout.PhysicalScreens.FirstOrDefault(
                  screen =>
                  screen.VirtualDisplays.Any(
                      display =>
                      display.Reference.Width.Value == resolution.Width.Value
                      && display.Reference.Height.Value == resolution.Height.Value));
        }

        private PhysicalScreenRefConfigDataViewModel GetPhysicalScreenRef(
           PhysicalScreenConfigDataViewModel physicalScreen)
        {
            var masterLayout =
                this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                    .FirstOrDefault();
            if (masterLayout == null)
            {
                return null;
            }

            return masterLayout.PhysicalScreens.FirstOrDefault(
                    screen =>
                    screen.Reference == physicalScreen);
        }

        private void ShowCycleNavigation()
        {
            Logger.Debug("Request to show the cycle navigation.");
            InteractionManager<ShowCycleNavigationPrompt>.Current.Raise(new ShowCycleNavigationPrompt());
        }

        private void ShowPhysicalScreenDialog()
        {
            Logger.Debug("Request to show the physical screens dialog");
            InteractionManager<ResolutionNavigationPrompt>.Current.Raise(
                new ResolutionNavigationPrompt(this.shell, this.commandRegistry));
        }

        private void ShowSectionNavigation()
        {
            Logger.Debug("Request to show the cycle navigation.");
            InteractionManager<ShowSectionNavigationPrompt>.Current.Raise(new ShowSectionNavigationPrompt());
        }

        private void ShowFormulaEditor(LayoutElementDataViewModelBase element, IDynamicDataValue dataValue)
        {
            Logger.Debug("Request to show the Formula Editor.");
            var oldElement = (LayoutElementDataViewModelBase)element.Clone();
            Action<FormulaEditorPrompt> callback = prompt =>
                {
                    if (!element.EqualsViewModel(oldElement))
                    {
                        var newElements = new List<LayoutElementDataViewModelBase>
                                              {
                                                  (LayoutElementDataViewModelBase)
                                                  element.Clone()
                                              };
                        var oldElements = new List<LayoutElementDataViewModelBase> { oldElement };
                        UpdateEntityParameters parameters;
                        if (newElements.First() is PlaybackElementDataViewModelBase)
                        {
                            parameters = new UpdateEntityParameters(
                                oldElements,
                                newElements,
                                this.shell.Editor.CurrentAudioOutputElement.Elements);
                        }
                        else if (newElements.First() is AudioOutputElementDataViewModel)
                        {
                            var elementContainer =
                                ((LayoutConfigDataViewModel)this.shell.MediaApplicationState.CurrentLayout).Resolutions
                                    .First().Elements;
                            parameters = new UpdateEntityParameters(oldElements, newElements, elementContainer);
                        }
                        else
                        {
                            parameters = new UpdateEntityParameters(
                                oldElements,
                                newElements,
                                this.shell.Editor.Elements);
                        }

                        ((EditorViewModelBase)this.shell.Editor).UpdateElementCommand.Execute(parameters);
                    }

                    InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
                };
            InteractionManager<FormulaEditorPrompt>.Current.Raise(
                new FormulaEditorPrompt(this.shell, dataValue, this.commandRegistry) { IsOpen = true }, callback);
        }

        private PhysicalScreenRefConfigDataViewModel GetPhysicalScreenRef(
            ResolutionConfigDataViewModel resolution, string layoutName)
        {
            var masterLayout =
                this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts
                    .FirstOrDefault();
            if (masterLayout == null)
            {
                return null;
            }

            var screens = masterLayout.PhysicalScreens.Where(
                    screen =>
                    screen.VirtualDisplays.Any(
                        display =>
                        display.Reference.Width.Value == resolution.Width.Value
                        && display.Reference.Height.Value == resolution.Height.Value));
            foreach (var screen in screens)
            {
                foreach (var display in screen.VirtualDisplays)
                {
                    if (
                        display.Reference.CyclePackage.EventCycles.Any(
                            eventCycle =>
                            eventCycle.Reference.Sections.Any(section => section.Layout.Name.Value == layoutName)))
                    {
                        return screen;
                    }

                    if (
                        display.Reference.CyclePackage.StandardCycles.Any(
                            standardCycle =>
                            standardCycle.Reference.Sections.Any(section => section.Layout.Name.Value == layoutName)))
                    {
                        return screen;
                    }
                }
            }

            return null;
        }

        private void LoadResolutionElements(ResolutionConfigDataViewModel resolution)
        {
            if (this.shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.Audio)
            {
                this.shell.Editor.Elements.Clear();
                this.shell.Editor.SelectedElements.Clear();
                var element = resolution.Elements.FirstOrDefault();
                if (element != null)
                {
                    this.shell.Editor.CurrentAudioOutputElement = (AudioOutputElementDataViewModel)element;
                    this.shell.Editor.CanCreateAudioElements = true;
                }
                else
                {
                    this.shell.Editor.CurrentAudioOutputElement = null;
                    this.shell.Editor.CanCreateAudioElements = false;
                }
            }
            else
            {
                this.shell.Editor.CurrentAudioOutputElement = null;
                this.shell.Editor.Elements.Clear();
                this.shell.Editor.SelectedElements.Clear();
                this.shell.Editor.CanCreateAudioElements = false;
                foreach (var element in resolution.Elements.Where(e => e is GraphicalElementDataViewModelBase))
                {
                    this.shell.Editor.Elements.Add((GraphicalElementDataViewModelBase)element);
                }

                this.shell.Editor.SortByZOrder();
            }

            InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
        }
    }
}
