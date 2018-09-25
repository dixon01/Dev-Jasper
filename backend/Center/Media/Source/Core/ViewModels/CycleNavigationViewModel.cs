// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleNavigationViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The ViewModel for all contents and commands needed by the cycle navigation.
    /// </summary>
    public class CycleNavigationViewModel : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private CycleNavigationSelection selectedNavigation;

        private CycleRefConfigDataViewModelBase highlightedCycle;

        private SectionConfigDataViewModelBase highlightedSection;

        private CyclePackageConfigDataViewModel highlightedCyclePackage;

        private ExtendedObservableCollection<CycleNavigationTreeViewDataViewModel> treeViewFirstLevelElements;

        private CycleNavigationTreeViewDataViewModel selectedCycleNavigationTreeViewDataViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleNavigationViewModel"/> class.
        /// </summary>
        public CycleNavigationViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleNavigationViewModel"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public CycleNavigationViewModel(IMediaShell parent, ICommandRegistry commandRegistry)
        {
            this.Parent = parent;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the current cycle package of the project.
        /// </summary>
        public CyclePackageConfigDataViewModel CurrentCyclePackage
        {
            get
            {
                return this.Parent.MediaApplicationState.CurrentCyclePackage;
            }

            set
            {
                this.Parent.MediaApplicationState.CurrentCyclePackage = value;
                this.RaisePropertyChanged(() => this.CurrentCyclePackage);
            }
        }

        /// <summary>
        /// Gets or sets the current cycle of the project.
        /// </summary>
        public CycleConfigDataViewModelBase CurrentCycle
        {
            get
            {
                return this.Parent.MediaApplicationState.CurrentCycle;
            }

            set
            {
                this.Parent.MediaApplicationState.CurrentCycle = value;
                this.RaisePropertyChanged(() => this.CurrentCycle);
            }
        }

        /// <summary>
        /// Gets or sets the current section of the selected cycle.
        /// </summary>
        public SectionConfigDataViewModelBase CurrentSection
        {
            get
            {
                return this.Parent.MediaApplicationState.CurrentSection;
            }

            set
            {
                this.Parent.MediaApplicationState.CurrentSection = value;
                this.RaisePropertyChanged(() => this.CurrentSection);
            }
        }

        /// <summary>
        /// Gets or sets the highlighted cycle of the project.
        /// </summary>
        public CycleRefConfigDataViewModelBase HighlightedCycle
        {
            get
            {
                return this.highlightedCycle;
            }

            set
            {
                this.SetProperty(ref this.highlightedCycle, value, () => this.HighlightedCycle);
            }
        }

        /// <summary>
        /// Gets or sets the highlighted section of the selected cycle.
        /// </summary>
        public SectionConfigDataViewModelBase HighlightedSection
        {
            get
            {
                return this.highlightedSection;
            }

            set
            {
                this.SetProperty(ref this.highlightedSection, value, () => this.HighlightedSection);
            }
        }

        /// <summary>
        /// Gets or sets the highlighted cycle package of the project.
        /// </summary>
        public CyclePackageConfigDataViewModel HighlightedCyclePackage
        {
            get
            {
                return this.highlightedCyclePackage;
            }

            set
            {
                this.SetProperty(ref this.highlightedCyclePackage, value, () => this.HighlightedCyclePackage);
            }
        }

        /// <summary>
        /// Gets or sets the cycle packages.
        /// </summary>
        public ExtendedObservableCollection<CyclePackageConfigDataViewModel> CyclePackages
        {
            get
            {
                ExtendedObservableCollection<CyclePackageConfigDataViewModel> result = null;
                var currentProject = this.Parent.MediaApplicationState.CurrentProject;
                if (currentProject != null && currentProject.InfomediaConfig != null)
                {
                    result = currentProject.InfomediaConfig.CyclePackages;
                }

                return result;
            }

            set
            {
                var currentProject = this.Parent.MediaApplicationState.CurrentProject;
                if (currentProject != null && currentProject.InfomediaConfig != null)
                {
                    currentProject.InfomediaConfig.CyclePackages = value;
                    this.RaisePropertyChanged(() => this.CyclePackages);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected navigation
        /// </summary>
        public CycleNavigationSelection SelectedNavigation
        {
            get
            {
                return this.selectedNavigation;
            }

            set
            {
                this.SetProperty(ref this.selectedNavigation, value, () => this.SelectedNavigation);
                InteractionManager<UpdateCycleDetailsPrompt>.Current.Raise(new UpdateCycleDetailsPrompt());
            }
        }

        /// <summary>
        /// Gets or sets the selected cycle navigation tree view data view model.
        /// </summary>
        public CycleNavigationTreeViewDataViewModel SelectedCycleNavigationTreeViewDataViewModel
        {
            get
            {
                return this.selectedCycleNavigationTreeViewDataViewModel;
            }

            set
            {
                this.SetProperty(
                    ref this.selectedCycleNavigationTreeViewDataViewModel,
                    value,
                    () => this.SelectedCycleNavigationTreeViewDataViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the parent ViewModel.
        /// </summary>
        public IMediaShell Parent { get; set; }

        /// <summary>
        /// Gets the create new cycle command
        /// </summary>
        public ICommand CreateNewCycle
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.CreateNew);
            }
        }

        /// <summary>
        /// Gets the delete cycle command
        /// </summary>
        public ICommand DeleteCycle
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.Delete);
            }
        }

        /// <summary>
        /// Gets the choose Cycle command
        /// </summary>
        public ICommand ChooseCycle
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.Choose);
            }
        }

        /// <summary>
        /// Gets the clone cycle command
        /// </summary>
        public ICommand CloneCycle
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.Clone);
            }
        }

        /// <summary>
        /// Gets the create cycle reference command.
        /// </summary>
        public ICommand CreateCycleReference
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.CreateReference);
            }
        }

        /// <summary>
        /// Gets the create new Section command
        /// </summary>
        public ICommand CreateNewSection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.CreateNew);
            }
        }

        /// <summary>
        /// Gets the delete Section command
        /// </summary>
        public ICommand DeleteSection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.Delete);
            }
        }

        /// <summary>
        /// Gets the choose Section command
        /// </summary>
        public ICommand ChooseSection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.Choose);
            }
        }

        /// <summary>
        /// Gets the choose Layout command
        /// </summary>
        public ICommand ChooseLayout
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
            }
        }

        /// <summary>
        /// Gets the clone Section command
        /// </summary>
        public ICommand CloneSection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.Clone);
            }
        }

        /// <summary>
        /// Gets the rename cycle name command.
        /// </summary>
        public ICommand RenameCycle
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.Rename);
            }
        }

        /// <summary>
        /// Gets the rename Section command.
        /// </summary>
        public ICommand RenameSection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.Rename);
            }
        }

        /// <summary>
        /// Gets the command to rename a cycle package.
        /// </summary>
        public ICommand RenameCyclePackage
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.CyclePackage.Rename);
            }
        }

        /// <summary>
        /// Gets the command to choose a cycle package.
        /// </summary>
        public ICommand ChooseCyclePackage
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.CyclePackage.Choose);
            }
        }

        /// <summary>
        /// Gets the ShowCycleTypeSelection command.
        /// </summary>
        public ICommand ShowCycleTypeSelection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.ShowCycleTypeSelection);
            }
        }

        /// <summary>
        /// Gets the ShowSectionTypeSelection command.
        /// </summary>
        public ICommand ShowSectionTypeSelection
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.ShowSectionTypeSelection);
            }
        }

        /// <summary>
        /// Gets the RemoveFormula command.
        /// </summary>
        public ICommand RemoveCycleFormulaCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.RemoveFormula);
            }
        }

        /// <summary>
        /// Gets the Remove Animation command.
        /// </summary>
        public ICommand RemoveCycleAnimationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.RemoveAnimation);
            }
        }

        /// <summary>
        /// Gets the RemoveFormula command.
        /// </summary>
        public ICommand RemoveSectionFormulaCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.RemoveFormula);
            }
        }

        /// <summary>
        /// Gets the Remove Animation command.
        /// </summary>
        public ICommand RemoveSectionAnimationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.RemoveAnimation);
            }
        }

        /// <summary>
        /// Gets the CyclePackages grouped by physical screen type.
        /// </summary>
        public ExtendedObservableCollection<CycleNavigationTreeViewDataViewModel> TreeViewFirstLevelElements
        {
            get
            {
                return this.treeViewFirstLevelElements;
            }

            private set
            {
                this.SetProperty(ref this.treeViewFirstLevelElements, value, () => this.TreeViewFirstLevelElements);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the CreateCycle Interaction Request.
        /// </summary>
        public IInteractionRequest CreateCycleInteractionRequest
        {
            get
            {
                return InteractionManager<SelectCycleTypePrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the SelectSectionType Interaction Request.
        /// </summary>
        public IInteractionRequest SelectSectionTypeInteractionRequest
        {
            get
            {
                return InteractionManager<SelectSectionTypePrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the show formula editor command.
        /// </summary>
        public ICommand ShowFormulaEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowNavigationFormulaEditor);
            }
        }

        /// <summary>
        /// Gets the show trigger editor command.
        /// </summary>
        public ICommand ShowTriggerEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowNavigationTriggerEditor);
            }
        }

        /// <summary>
        /// Gets the show animation editor command.
        /// </summary>
        public ICommand ShowAnimationEditorCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowNavigationAnimationEditor);
            }
        }

        /// <summary>
        /// Initializes the CycleNavigation ViewModel.
        /// </summary>
        public void Initialize()
        {
            this.Parent.MediaApplicationState.PropertyChanged += this.MediaApplicationStateOnPropertyChanged;
        }

        /// <summary>
        /// Unsubscribes the CollectionChanged event of <see cref="MasterLayoutConfigDataViewModel.PhysicalScreens"/>.
        /// </summary>
        public void UnsubscribePhysicalScreenCollectionChangedEvent()
        {
            this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts.First()
                .PhysicalScreens.CollectionChanged -= this.OnCyclePackagesChanged;
        }

        /// <summary>
        /// The move to existing cycle after a delete operation.
        /// </summary>
        public void SelectDefaultStandardCycle()
        {
            var cycleRef = this.CurrentCyclePackage.StandardCycles.FirstOrDefault();
            if (cycleRef == null)
            {
                return;
            }

            this.ChooseCycle.Execute(cycleRef);
            this.CurrentCycle.IsItemSelected = true;
            this.HighlightedCycle = cycleRef;
        }

        /// <summary>
        /// The move to existing cycle after a delete operation. If no EventCycle is found use default StandardCycle
        /// </summary>
        public void SelectDefaultEventCycle()
        {
            var cycleRef = this.CurrentCyclePackage.EventCycles.FirstOrDefault();

            if (cycleRef == null)
            {
                this.SelectDefaultStandardCycle();
            }
            else
            {
                this.ChooseCycle.Execute(cycleRef);
                this.CurrentCycle.IsItemSelected = true;
                this.HighlightedCycle = cycleRef;
            }
        }

        /// <summary>
        /// The highlight current cycle.
        /// </summary>
        public void HighlightCurrentCycle()
        {
            CycleRefConfigDataViewModelBase cycleRef =
                this.CurrentCyclePackage.EventCycles.FirstOrDefault(ec => ec.Reference == this.CurrentCycle);
            if (cycleRef == null)
            {
                cycleRef =
                    this.CurrentCyclePackage.StandardCycles.FirstOrDefault(ec => ec.Reference == this.CurrentCycle);
            }

            if (cycleRef != null)
            {
                this.HighlightedCycle = cycleRef;
            }
        }

        /// <summary>
        /// The highlight current cycle.
        /// </summary>
        public void HighlightCurrentSection()
        {
            if (this.CurrentCycle == null)
            {
                return;
            }

            SectionConfigDataViewModelBase section =
                this.CurrentCycle.Sections.FirstOrDefault(s => s == this.CurrentSection);

            if (section != null)
            {
                this.HighlightedSection = section;
            }
        }

        private void MediaApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentCyclePackage":
                    this.RaisePropertyChanged(() => this.CurrentCyclePackage);
                    break;
                case "CurrentCycle":
                    this.RaisePropertyChanged(() => this.CurrentCycle);
                    break;
                case "CurrentSection":
                    this.RaisePropertyChanged(() => this.CurrentSection);
                    break;
                case "CurrentProject":
                    this.RaisePropertyChanged(() => this.CyclePackages);
                    if (this.Parent.MediaApplicationState.CurrentProject != null
                        && this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig != null)
                    {
                        this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation
                            .MasterLayouts.First().PhysicalScreens.CollectionChanged += this.OnCyclePackagesChanged;
                        this.UpdateTreeViewCollection();
                    }

                    break;
            }
        }

        private void OnCyclePackagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateTreeViewCollection();
            this.RaisePropertyChanged(() => this.SelectedCycleNavigationTreeViewDataViewModel);
        }

        private void UpdateTreeViewCollection()
        {
            if (this.treeViewFirstLevelElements == null)
            {
                this.treeViewFirstLevelElements =
                    new ExtendedObservableCollection<CycleNavigationTreeViewDataViewModel>();
            }

            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() => this.TreeViewFirstLevelElements.Clear());

            if (this.Parent.MediaApplicationState.CurrentProject == null
                || this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig == null)
            {
                return;
            }

            var tftTypes =
                this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts[0]
                    .PhysicalScreens.Where(p => p.Reference.Type.Value == PhysicalScreenType.TFT).ToList();
            var ledTypes =
                this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts[0]
                    .PhysicalScreens.Where(p => p.Reference.Type.Value == PhysicalScreenType.LED).ToList();
            var audioTypes =
                this.Parent.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation.MasterLayouts[0]
                    .PhysicalScreens.Where(p => p.Reference.Type.Value == PhysicalScreenType.Audio).ToList();
            if (tftTypes.Count != 0)
            {
                var cyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
                foreach (
                    var virtualDisplayRefConfigDataViewModel in
                        tftTypes.SelectMany(
                            physicalScreenConfigDataViewModel => physicalScreenConfigDataViewModel.VirtualDisplays))
                {
                    cyclePackages.Add(virtualDisplayRefConfigDataViewModel.Reference.CyclePackage);
                }

                dispatcher.Dispatch(() =>
                    this.TreeViewFirstLevelElements.Add(
                        new CycleNavigationTreeViewDataViewModel(PhysicalScreenType.TFT, cyclePackages)));
            }

            if (ledTypes.Count != 0)
            {
                var cyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
                foreach (
                    var virtualDisplayRefConfigDataViewModel in
                        ledTypes.SelectMany(
                            physicalScreenConfigDataViewModel => physicalScreenConfigDataViewModel.VirtualDisplays))
                {
                    cyclePackages.Add(virtualDisplayRefConfigDataViewModel.Reference.CyclePackage);
                }

                dispatcher.Dispatch(() => this.TreeViewFirstLevelElements.Add(
                    new CycleNavigationTreeViewDataViewModel(PhysicalScreenType.LED, cyclePackages)));
            }

            if (audioTypes.Count != 0)
            {
                var cyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
                foreach (
                    var virtualDisplayRefConfigDataViewModel in
                        audioTypes.SelectMany(
                            physicalScreenConfigDataViewModel => physicalScreenConfigDataViewModel.VirtualDisplays))
                {
                    cyclePackages.Add(virtualDisplayRefConfigDataViewModel.Reference.CyclePackage);
                }

                dispatcher.Dispatch(() => this.TreeViewFirstLevelElements.Add(
                    new CycleNavigationTreeViewDataViewModel(PhysicalScreenType.Audio, cyclePackages)));
            }

            if (this.SelectedCycleNavigationTreeViewDataViewModel != null)
            {
                this.SelectedCycleNavigationTreeViewDataViewModel =
                    this.TreeViewFirstLevelElements.FirstOrDefault(e =>
                        e.PhysicalScreenType == this.selectedCycleNavigationTreeViewDataViewModel.PhysicalScreenType);
            }

            this.RaisePropertyChanged(() => this.TreeViewFirstLevelElements);
        }
    }
}
