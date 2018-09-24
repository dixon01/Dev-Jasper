// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleDetailsNavigator.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CycleDetailsNavigator.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Center.Media.Core.Views.Controls;

    using NLog;

    /// <summary>
    /// Interaction logic for CycleDetailsNavigator.xaml
    /// </summary>
    public partial class CycleDetailsNavigator
    {
        /// <summary>
        /// the Editor Callbacks property
        /// </summary>
        public static readonly DependencyProperty EditorCallbacksProperty =
            DependencyProperty.Register(
                "EditorCallbacks",
                typeof(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>),
                typeof(CycleDetailsNavigator),
                new PropertyMetadata(default(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan DeferTimerInterval = TimeSpan.FromMilliseconds(1);

        private readonly DispatcherTimer deferPropertyGridCycleUpdateTimer;

        private readonly DispatcherTimer deferPropertyGridSectionUpdateTimer;

        private readonly DispatcherTimer deferPropertyGridCyclePackageUpdateTimer;

        private bool isChangingInGrid;

        private bool isChangingInEditor;

        private SectionConfigDataViewModelBase previousHighlightedSection;

        private CycleRefConfigDataViewModelBase previousHighlightedCycle;

        private CyclePackageConfigDataViewModel previousHighlightedCyclePackage;

        private List<SectionConfigDataViewModelBase> oldSectionElements;

        private List<CycleConfigDataViewModelBase> oldCycleElements;

        private CycleNavigationTreeViewDataViewModel previousParentRootNode;

        private TreeViewFolderItem previousParentCyclesFolderItem;

        private CycleRefConfigDataViewModelBase previousParentCycle;

        private CyclePackageConfigDataViewModel previousParentCyclePackage;

        private DataViewModelBase previousSelectedItem;

        private bool isCycleFolderSelected;

        private bool isRootNodeSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleDetailsNavigator"/> class.
        /// </summary>
        public CycleDetailsNavigator()
        {
            this.InitializeComponent();
            this.deferPropertyGridCycleUpdateTimer = new DispatcherTimer { Interval = DeferTimerInterval };
            this.deferPropertyGridCycleUpdateTimer.Tick += this.PropertyGridCycleUpdateTimerElapsed;

            this.deferPropertyGridSectionUpdateTimer = new DispatcherTimer { Interval = DeferTimerInterval };
            this.deferPropertyGridSectionUpdateTimer.Tick += this.PropertyGridSectionUpdateTimerElapsed;

            this.deferPropertyGridCyclePackageUpdateTimer = new DispatcherTimer { Interval = DeferTimerInterval };
            this.deferPropertyGridCyclePackageUpdateTimer.Tick += this.PropertyGridCyclePackageUpdateTimerElapsed;

            this.Loaded += this.OnLoaded;

            this.EditorCallbacks = new Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>
                                       {
                                           {
                                               typeof(GenericTriggerConfigDataViewModel),
                                               this.OnTriggerEditGenericList
                                           }
                                       };
        }

        /// <summary>
        /// Gets the update properties command
        /// </summary>
        public ICommand UpdateCyclePropertiesCommand
        {
            get
            {
                return new RelayCommand(this.UpdatePropertyGrid);
            }
        }

        /// <summary>
        /// Gets the refresh property grid interaction request
        /// </summary>
        public IInteractionRequest RefreshCyclePropertyGridRequest
        {
            get
            {
                return InteractionManager<UpdateCycleDetailsPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets or sets the editor callbacks
        /// </summary>
        public Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>> EditorCallbacks
        {
            get
            {
                return
                    (Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)
                    this.GetValue(EditorCallbacksProperty);
            }

            set
            {
                this.SetValue(EditorCallbacksProperty, value);
            }
        }

        private static TreeNodeResult TraverseCycleFolderItems(
         object item,
         TreeViewItem cycleFolderItemContainer,
         Type itemType,
         object cyclePackageItem,
         object screenTypeItem,
         object cycleFolderItem,
            bool bringToFront)
        {
            foreach (var cycle in cycleFolderItemContainer.Items)
            {
                var cycleContainer =
                       cycleFolderItemContainer.ItemContainerGenerator.ContainerFromItem(cycle) as TreeViewItem;
                if (itemType == typeof(StandardCycleRefConfigDataViewModel)
                    || itemType == typeof(EventCycleRefConfigDataViewModel))
                {
                    if (cycle == item)
                    {
                        if (cycleContainer != null && bringToFront)
                        {
                            cycleContainer.BringIntoView();
                        }

                        return new TreeNodeResult
                        {
                            Cyclepackage = cyclePackageItem as CyclePackageConfigDataViewModel,
                            RootNode = screenTypeItem as CycleNavigationTreeViewDataViewModel,
                            CycleFolder = cycleFolderItem as TreeViewFolderItem
                        };
                    }
                }
                else
                {
                    if (cycleContainer == null || cycleContainer.Items.Count <= 0
                        || cycleContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        continue;
                    }

                    foreach (var section in cycleContainer.Items)
                    {
                        var sectionContainer =
                            cycleContainer.ItemContainerGenerator.ContainerFromItem(section) as TreeViewItem;
                        if (section == item)
                        {
                            if (sectionContainer != null && bringToFront)
                            {
                                sectionContainer.BringIntoView();
                            }

                            return new TreeNodeResult
                            {
                                Cyclepackage = cyclePackageItem as CyclePackageConfigDataViewModel,
                                RootNode = screenTypeItem as CycleNavigationTreeViewDataViewModel,
                                CycleFolder = cycleFolderItem as TreeViewFolderItem,
                                Cycle = cycle as CycleRefConfigDataViewModelBase
                            };
                        }
                    }
                }
            }

            return null;
        }

        private void OnSectionHighlightChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = (CycleNavigationViewModel)this.DataContext;
            this.RenewSectionPropertyChangedEventRegistration(context);
            this.deferPropertyGridSectionUpdateTimer.Stop();
            this.deferPropertyGridSectionUpdateTimer.Start();
        }

        private void OnEventCycleHighlightChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            this.RenewCyclePropertyChangedEventRegistration(context);

            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.deferPropertyGridCycleUpdateTimer.Start();
        }

        private void OnStandardCycleHighlightChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            this.RenewCyclePropertyChangedEventRegistration(context);

            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.deferPropertyGridCycleUpdateTimer.Start();
        }

        private void OnCyclePackageHighlightChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            this.RenewCyclePackagePropertyChangedEventRegistration(context);

            this.deferPropertyGridCyclePackageUpdateTimer.Stop();
            this.deferPropertyGridCyclePackageUpdateTimer.Start();
        }

        private void RenewSectionPropertyChangedEventRegistration(CycleNavigationViewModel context)
        {
            if (this.previousHighlightedSection != null)
            {
                this.previousHighlightedSection.PropertyChanged -= this.SectionPropertyChanged;
            }

            this.previousHighlightedSection = context.HighlightedSection;

            if (context.HighlightedSection != null)
            {
                context.HighlightedSection.PropertyChanged += this.SectionPropertyChanged;
            }
        }

        private void RenewCyclePackagePropertyChangedEventRegistration(CycleNavigationViewModel context)
        {
            if (this.previousHighlightedCyclePackage != null)
            {
                this.previousHighlightedCyclePackage.PropertyChanged -= this.CyclePackagePropertyChanged;
            }

            this.previousHighlightedCyclePackage = context.HighlightedCyclePackage;

            if (context.HighlightedCyclePackage != null)
            {
                context.HighlightedCyclePackage.PropertyChanged += this.CyclePackagePropertyChanged;
            }
        }

        private void RenewCyclePropertyChangedEventRegistration(CycleNavigationViewModel context)
        {
            if (this.previousHighlightedCycle != null)
            {
                this.previousHighlightedCycle.PropertyChanged -= this.CycleReferencePropertyChanged;
                var previousCycle = this.previousHighlightedCycle.Reference;
                if (previousCycle != null)
                {
                    previousCycle.PropertyChanged -= this.CyclePropertyChanged;
                }
            }

            this.previousHighlightedCycle = context.HighlightedCycle;

            if (context.HighlightedCycle != null)
            {
                var cycle = context.HighlightedCycle.Reference;
                if (cycle != null)
                {
                    context.HighlightedCycle.Reference.PropertyChanged += this.CyclePropertyChanged;
                }

                context.HighlightedCycle.PropertyChanged += this.CycleReferencePropertyChanged;
            }
        }

        private void CycleReferencePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChangingInGrid
                || e.PropertyName == "IsChildItemSelected"
                || e.PropertyName == "IsItemSelected"
                || e.PropertyName == "IsExpanded")
            {
                return;
            }

            var element = ((CycleRefConfigDataViewModelBase)sender).Reference;
            var elements = new ObservableCollection<CycleConfigDataViewModelBase> { element };

            this.OnPropertyChangeSetDataSource(e, element, elements);

            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.deferPropertyGridCycleUpdateTimer.Start();
        }

        private void CyclePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChangingInGrid
                || e.PropertyName == "IsChildItemSelected"
                || e.PropertyName == "IsItemSelected"
                || e.PropertyName == "IsExpanded")
            {
                return;
            }

            var element = (CycleConfigDataViewModelBase)sender;
            var elements = new ObservableCollection<CycleConfigDataViewModelBase> { element };

            this.OnPropertyChangeSetDataSource(e, element, elements);

            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.deferPropertyGridCycleUpdateTimer.Start();
        }

        private void SectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChangingInGrid || e.PropertyName == "IsItemSelected")
            {
                return;
            }

            var element = (SectionConfigDataViewModelBase)sender;
            var elements = new ObservableCollection<SectionConfigDataViewModelBase> { element };

            this.OnPropertyChangeSetDataSource(e, element, elements);

            this.deferPropertyGridSectionUpdateTimer.Stop();
            this.deferPropertyGridSectionUpdateTimer.Start();
        }

        private void CyclePackagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChangingInGrid
                || e.PropertyName == "IsChildItemSelected"
                || e.PropertyName == "IsItemSelected"
                || e.PropertyName == "IsExpanded")
            {
                return;
            }

            var element = (CyclePackageConfigDataViewModel)sender;
            var elements = new ObservableCollection<CyclePackageConfigDataViewModel> { element };

            this.OnPropertyChangeSetDataSource(e, element, elements);

            this.deferPropertyGridCyclePackageUpdateTimer.Stop();
            this.deferPropertyGridCyclePackageUpdateTimer.Start();
        }

        private void OnPropertyChangeSetDataSource<T>(PropertyChangedEventArgs e, T element, IEnumerable<T> elements)
            where T : class
        {
            if (element == null)
            {
                return;
            }

            try
            {
                this.isChangingInEditor = true;
                if (this.PropertyGrid.HasDynamicProperty(e.PropertyName))
                {
                    var propertyInfo = element.GetType().GetProperty(e.PropertyName);
                    var data = propertyInfo.GetValue(element, null) as IDataValue;
                    if (data != null)
                    {
                        if (this.PropertyGrid.DataIsSameForAll(data, propertyInfo, elements))
                        {
                            this.PropertyGrid.SetWrappedDataSource(e.PropertyName, data.ValueObject);
                        }
                        else
                        {
                            this.PropertyGrid.SetWrappedDataSource(e.PropertyName, null);
                        }
                    }
                }

                this.isChangingInEditor = false;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while updating property grid", ex);
            }

            this.isChangingInEditor = false;
        }

        private void ExpandPropertyGridGroups()
        {
            foreach (var gridGroup in this.PropertyGrid.Groups)
            {
                gridGroup.IsExpanded = true;
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            PropertyGridContextMenuFactory.SetupDefaultContextMenu(
                this.PropertyGrid,
                new RelayCommand(this.OnOpenFormulaEditor),
                new RelayCommand(this.OnRemoveFormulaCommandTrigered, this.OnCanExecuteRemoveFormulaCommandTriggered));

            // new RelayCommand(this.OnOpenAnimationEditor, this.OnCanExecuteOpenAnimationEditor),
            // new RelayCommand(this.OnRemoveAnimationCommandTrigered, this.OnCanExecuteRemoveAnimationCommandTriggered)
        }

        private void OnOpenFormulaEditor()
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                this.oldCycleElements = new List<CycleConfigDataViewModelBase>
                                            {
                                                (CycleConfigDataViewModelBase)
                                                context.HighlightedCycle.Reference
                                                    .Clone()
                                            };
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                this.oldSectionElements = new List<SectionConfigDataViewModelBase>
                                              {
                                                  (SectionConfigDataViewModelBase)
                                                  context.HighlightedSection.Clone()
                                              };
            }

            context.ShowFormulaEditorCommand.Execute(this.PropertyGrid.ItemContextMenu);
        }

        private void OnOpenAnimationEditor()
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                this.oldCycleElements = new List<CycleConfigDataViewModelBase>
                                            {
                                                (CycleConfigDataViewModelBase)
                                                context.HighlightedCycle.Reference
                                                    .Clone()
                                            };
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                this.oldSectionElements = new List<SectionConfigDataViewModelBase>
                                              {
                                                  (SectionConfigDataViewModelBase)
                                                  context.HighlightedSection.Clone()
                                              };
            }

            context.ShowAnimationEditorCommand.Execute(this.PropertyGrid.ItemContextMenu);
        }

        private bool OnCanExecuteOpenAnimationEditor(object obj)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            return context.ShowAnimationEditorCommand.CanExecute(this.PropertyGrid.ItemContextMenu);
        }

        private bool OnCanExecuteRemoveFormulaCommandTriggered(object o)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            var result = false;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                result = context.RemoveCycleFormulaCommand.CanExecute(o);
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                result = context.RemoveSectionFormulaCommand.CanExecute(o);
            }

            return result;
        }

        private bool OnCanExecuteRemoveAnimationCommandTriggered(object o)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            var result = false;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                result = context.RemoveCycleAnimationCommand.CanExecute(o);
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                result = context.RemoveSectionAnimationCommand.CanExecute(o);
            }

            return result;
        }

        private void OnRemoveFormulaCommandTrigered(object o)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                context.RemoveCycleFormulaCommand.Execute(o);
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                context.RemoveSectionFormulaCommand.Execute(o);
            }
        }

        private void OnRemoveAnimationCommandTrigered(object o)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                context.RemoveCycleAnimationCommand.Execute(o);
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                context.RemoveSectionAnimationCommand.Execute(o);
            }
        }

        private void OnSectionGridItemPropertyChanged(PropertyGridItem propertyGridItem, object datasource)
        {
            if (this.isChangingInEditor)
            {
                return;
            }

            this.isChangingInGrid = true;

            this.UpdateSectionElements(propertyGridItem, datasource);

            this.isChangingInGrid = false;
        }

        private void UpdateCycleElements(PropertyGridItem propertyGridItem, object datasource)
        {
            var oldElements = new List<CycleConfigDataViewModelBase>();
            var newElements = new List<CycleConfigDataViewModelBase>();

            var context = (CycleNavigationViewModel)this.DataContext;
            var elements =
                new ObservableCollection<CycleConfigDataViewModelBase> { context.HighlightedCycle.Reference };
            foreach (var element in elements)
            {
                try
                {
                    oldElements.Add((CycleConfigDataViewModelBase)element.Clone());
                    var property = element.GetType().GetProperty(propertyGridItem.Name);
                    var concreteValue = property.GetValue(element, null);
                    var concreteDataValue = concreteValue as IDataValue;
                    var concreteDynamicValue = concreteDataValue as IDynamicDataValue;
                    if (concreteDynamicValue != null)
                    {
                        var evaluation = datasource as EvalDataViewModelBase;
                        if (evaluation == null)
                        {
                            if (concreteDynamicValue.Formula == null)
                            {
                                concreteDynamicValue.ValueObject = datasource;
                            }
                        }

                        concreteDynamicValue.Formula = evaluation;
                        property.SetValue(element, concreteDynamicValue, null);
                    }
                    else if (concreteDataValue != null)
                    {
                        concreteDataValue.ValueObject = datasource;
                        property.SetValue(element, concreteDataValue, new object[0]);
                    }
                    else
                    {
                        property.SetValue(element, concreteValue, new object[0]);
                    }

                    newElements.Add((CycleConfigDataViewModelBase)element.Clone());
                }
                catch (Exception exception)
                {
                    Logger.WarnException("Exception while Converting PropertyGrid Value.", exception);
                    this.UpdateCyclePropertyGrid();
                }
            }

            var parameters = new UpdateEntityParameters(oldElements, newElements, elements);
            var dataContext = (MediaShell)context.Parent;
            var editor = dataContext.Editor as EditorViewModelBase;
            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private void UpdateSectionElements(PropertyGridItem propertyGridItem, object datasource)
        {
            var oldElements = new List<SectionConfigDataViewModelBase>();
            var newElements = new List<SectionConfigDataViewModelBase>();

            var context = (CycleNavigationViewModel)this.DataContext;
            var elements = new ObservableCollection<SectionConfigDataViewModelBase> { context.HighlightedSection };
            ResolutionConfigDataViewModel newResolution = null;
            foreach (var element in elements)
            {
                try
                {
                    oldElements.Add((SectionConfigDataViewModelBase)element.Clone());
                    var property = element.GetType().GetProperty(propertyGridItem.Name);
                    var concreteValue = property.GetValue(element, null);
                    if (!(concreteValue is IDataValue))
                    {
                        concreteValue = datasource;
                    }

                    var concreteDynamicValue = concreteValue as IDynamicDataValue;
                    if (concreteDynamicValue != null)
                    {
                        var evaluation = datasource as EvalDataViewModelBase;
                        if (evaluation == null)
                        {
                            if (concreteDynamicValue.Formula == null)
                            {
                                concreteDynamicValue.ValueObject = datasource;
                            }
                        }

                        concreteDynamicValue.Formula = evaluation;
                        property.SetValue(element, concreteDynamicValue, null);
                    }
                    else
                    {
                        PropertyGrid.SetConvertedValue(concreteValue, datasource, property, element);

                        if (property.Name == "Layout")
                        {
                            var layout = concreteValue as LayoutConfigDataViewModel;
                            if (layout != null)
                            {
                                newResolution = layout.AddCurrentResolution();
                            }

                            if (context.CurrentSection == element)
                            {
                                context.ChooseLayout.Execute(concreteValue);
                            }
                        }
                    }

                    newElements.Add((SectionConfigDataViewModelBase)element.Clone());
                }
                catch (Exception exception)
                {
                    Logger.WarnException("Exception while Converting PropertyGrid Value.", exception);
                    this.UpdateSectionPropertyGrid();
                }
            }

            var parameters = new UpdateEntityParameters(oldElements, newElements, elements, newResolution);
            var dataContext = (MediaShell)context.Parent;
            var editor = dataContext.Editor as EditorViewModelBase;
            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private void UpdateCyclePackageElements(PropertyGridItem propertyGridItem, object datasource)
        {
            var oldElements = new List<CyclePackageConfigDataViewModel>();
            var newElements = new List<CyclePackageConfigDataViewModel>();

            var context = (CycleNavigationViewModel)this.DataContext;
            var elements =
                new ObservableCollection<CyclePackageConfigDataViewModel> { context.HighlightedCyclePackage };
            foreach (var element in elements)
            {
                try
                {
                    oldElements.Add((CyclePackageConfigDataViewModel)element.Clone());
                    var property = element.GetType().GetProperty(propertyGridItem.Name);
                    var concreteValue = property.GetValue(element, null);
                    if (!(concreteValue is IDataValue))
                    {
                        concreteValue = datasource;
                    }

                    var concreteDynamicValue = concreteValue as IDynamicDataValue;
                    if (concreteDynamicValue != null)
                    {
                        var evaluation = datasource as EvalDataViewModelBase;
                        if (evaluation == null)
                        {
                            if (concreteDynamicValue.Formula == null)
                            {
                                concreteDynamicValue.ValueObject = datasource;
                            }
                        }

                        concreteDynamicValue.Formula = evaluation;
                        property.SetValue(element, concreteDynamicValue, null);
                    }
                    else
                    {
                        PropertyGrid.SetConvertedValue(concreteValue, datasource, property, element);
                    }

                    newElements.Add((CyclePackageConfigDataViewModel)element.Clone());
                }
                catch (Exception exception)
                {
                    Logger.WarnException("Exception while Converting PropertyGrid Value.", exception);
                    this.UpdateCyclePackagePropertyGrid();
                }
            }

            var parameters = new UpdateEntityParameters(oldElements, newElements, elements);
            var dataContext = (MediaShell)context.Parent;
            var editor = dataContext.Editor as EditorViewModelBase;
            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private void PropertyGridCycleUpdateTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.UpdateCyclePropertyGrid();
        }

        private void PropertyGridCyclePackageUpdateTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.deferPropertyGridCyclePackageUpdateTimer.Stop();
            this.UpdateCyclePackagePropertyGrid();
        }

        private void PropertyGridSectionUpdateTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.deferPropertyGridSectionUpdateTimer.Stop();
            this.UpdateSectionPropertyGrid();
        }

        private void UpdateSectionPropertyGrid()
        {
            var context = (CycleNavigationViewModel)this.DataContext;
            if (context == null || context.SelectedNavigation != CycleNavigationSelection.Section)
            {
                return;
            }

            if (context.HighlightedSection != null)
            {
                var elements = new ObservableCollection<SectionConfigDataViewModelBase> { context.HighlightedSection };

                if (this.SectionHasChanged())
                {
                    var dataContext = (MediaShell)context.Parent;
                    var editor = dataContext.Editor as EditorViewModelBase;
                    if (editor != null)
                    {
                        var newElements = elements.Select(e => (SectionConfigDataViewModelBase)e.Clone()).ToList();
                        var parameters = new UpdateEntityParameters(this.oldSectionElements, newElements, elements);
                        editor.UpdateElementCommand.Execute(parameters);
                    }
                }

                this.UpdateDomainValueCollection();

                this.PropertyGrid.UpdateContent(
                    elements,
                    this.OnSectionGridItemPropertyChanged,
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text),
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text + "Tooltip"));
            }
            else
            {
                this.PropertyGrid.Clear(this.OnSectionGridItemPropertyChanged);
            }

            this.ExpandPropertyGridGroups();
        }

        private bool SectionHasChanged()
        {
            var result = false;

            var context = (CycleNavigationViewModel)this.DataContext;

            var elements = new ObservableCollection<SectionConfigDataViewModelBase> { context.HighlightedSection };

            if (this.oldSectionElements != null)
            {
                var hasEqual = false;
                foreach (var oldElement in this.oldSectionElements)
                {
                    var original = elements.FirstOrDefault(elem => elem.GetHashCode() == oldElement.ClonedFrom);

                    if (original != null && !original.EqualsViewModel(oldElement))
                    {
                        hasEqual = true;
                        break;
                    }
                }

                result = hasEqual;
            }

            return result;
        }

        private void OnCycleGridItemPropertyChanged(PropertyGridItem propertyGridItem, object datasource)
        {
            if (this.isChangingInEditor)
            {
                return;
            }

            this.isChangingInGrid = true;

            this.UpdateCycleElements(propertyGridItem, datasource);

            this.isChangingInGrid = false;
        }

        private void OnCyclePackageGridItemPropertyChanged(PropertyGridItem propertyGridItem, object datasource)
        {
            if (this.isChangingInEditor)
            {
                return;
            }

            this.isChangingInGrid = true;
            this.UpdateCyclePackageElements(propertyGridItem, datasource);
            this.isChangingInGrid = false;
        }

        private void UpdateCyclePropertyGrid()
        {
            var context = (CycleNavigationViewModel)this.DataContext;
            if (context == null || context.SelectedNavigation != CycleNavigationSelection.Cycle)
            {
                return;
            }

            if (context.HighlightedCycle != null)
            {
                var elements = new ObservableCollection<CycleConfigDataViewModelBase>
                                   {
                                       context.HighlightedCycle
                                           .Reference
                                   };

                if (this.CycleHasChanged())
                {
                    var dataContext = (MediaShell)context.Parent;
                    var editor = dataContext.Editor as EditorViewModelBase;
                    if (editor != null)
                    {
                        var newElements = elements.Select(e => (CycleConfigDataViewModelBase)e.Clone()).ToList();
                        var parameters = new UpdateEntityParameters(this.oldCycleElements, newElements, elements);
                        editor.UpdateElementCommand.Execute(parameters);
                    }
                }

                this.UpdateDomainValueCollection();

                this.PropertyGrid.UpdateContent(
                    elements,
                    this.OnCycleGridItemPropertyChanged,
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text),
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text + "Tooltip"));
            }
            else
            {
                this.PropertyGrid.Clear(this.OnCycleGridItemPropertyChanged);
            }

            this.ExpandPropertyGridGroups();
        }

        private void UpdateCyclePackagePropertyGrid()
        {
            var context = (CycleNavigationViewModel)this.DataContext;
            if (context == null || context.SelectedNavigation != CycleNavigationSelection.CyclePackage)
            {
                return;
            }

            if (context.HighlightedCyclePackage != null)
            {
                var elements = new ObservableCollection<CyclePackageConfigDataViewModel>
                                   {
                                       context
                                           .HighlightedCyclePackage
                                   };

                this.UpdateDomainValueCollection();

                this.PropertyGrid.UpdateContent(
                    elements,
                    this.OnCyclePackageGridItemPropertyChanged,
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text),
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text + "Tooltip"));
            }
            else
            {
                this.PropertyGrid.Clear(this.OnCyclePackageGridItemPropertyChanged);
            }

            this.ExpandPropertyGridGroups();
        }

        private bool CycleHasChanged()
        {
            var result = false;

            var context = (CycleNavigationViewModel)this.DataContext;

            var elements =
                new ObservableCollection<CycleConfigDataViewModelBase> { context.HighlightedCycle.Reference };

            if (this.oldCycleElements != null)
            {
                var hasEqual = false;
                foreach (var oldElement in this.oldCycleElements)
                {
                    var original = elements.FirstOrDefault(elem => elem.GetHashCode() == oldElement.ClonedFrom);

                    if (original != null && !original.EqualsViewModel(oldElement))
                    {
                        hasEqual = true;
                        break;
                    }
                }

                result = hasEqual;
            }

            return result;
        }

        private void UpdateDomainValueCollection()
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            var currentProject = context.Parent.MediaApplicationState.CurrentProject;
            if (currentProject != null && currentProject.InfomediaConfig != null)
            {
                this.PropertyGrid.AddDomainValue("Layout", currentProject.InfomediaConfig.Layouts);
                this.PropertyGrid.AddDomainValue("Pool", currentProject.InfomediaConfig.Pools);
            }
        }

        private void UpdatePropertyGrid()
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            if (context.SelectedNavigation == CycleNavigationSelection.Cycle)
            {
                CycleRefConfigDataViewModelBase cycleRef =
                    context.CurrentCyclePackage.StandardCycles.FirstOrDefault(c => c.Reference == context.CurrentCycle)
                    ?? (CycleRefConfigDataViewModelBase)context.CurrentCyclePackage.EventCycles.FirstOrDefault(
                        c => c.Reference == context.CurrentCycle);

                if (cycleRef != null && !this.isCycleFolderSelected)
                {
                    context.HighlightedCycle = cycleRef;
                    this.OnSelectedItemChanged(
                        null,
                        new ReusableList.SelectReusableEntityEventArgs { Entity = cycleRef });
                }

                this.UpdateCyclePropertyGrid();
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.Section)
            {
                context.HighlightedSection = context.CurrentSection;
                this.OnSelectedItemChanged(
                    null,
                    new ReusableList.SelectReusableEntityEventArgs { Entity = context.CurrentSection });
                this.UpdateSectionPropertyGrid();
            }
            else if (context.SelectedNavigation == CycleNavigationSelection.CyclePackage)
            {
                context.HighlightedCyclePackage = context.CurrentCyclePackage;
                this.OnSelectedItemChanged(
                    null,
                    new ReusableList.SelectReusableEntityEventArgs { Entity = context.CurrentCyclePackage });
                this.UpdateCyclePackagePropertyGrid();
            }

            this.oldCycleElements = null;

            this.oldSectionElements = null;
        }

        private void OnTriggerEditGenericList(PropertyGridItem item, PropertyGridItemDataSource dataSource)
        {
            var context = (CycleNavigationViewModel)this.DataContext;

            this.oldCycleElements = new List<CycleConfigDataViewModelBase>
                                        {
                                            (CycleConfigDataViewModelBase)
                                            context.HighlightedCycle.Reference.Clone()
                                        };

            var triggerEditorParameters = new TriggerEditorParameters { DataSource = dataSource, Item = item, };

            context.ShowTriggerEditorCommand.Execute(triggerEditorParameters);
        }

        private void OnCollapseAllClicked(object sender, RoutedEventArgs e)
        {
            var treeView = this.SectionTreeView;
            foreach (var firstLevelItem in treeView.Items.OfType<CycleNavigationTreeViewDataViewModel>())
            {
                firstLevelItem.CollapseAll();
                foreach (var cyclePackage in firstLevelItem.CyclePackages)
                {
                    var parentNodes = this.GetParentNodes(cyclePackage.StandardCycles.First(), false);
                    parentNodes.CycleFolder.IsExpanded = false;
                    if (cyclePackage.EventCycles.Any())
                    {
                        var parentEventCycleNodes = this.GetParentNodes(cyclePackage.EventCycles.First(), false);
                        parentEventCycleNodes.CycleFolder.IsExpanded = false;
                    }
                }
            }
        }

        private void OnExpandAllClicked(object sender, RoutedEventArgs e)
        {
            var treeView = this.SectionTreeView;
            foreach (var firstLevelItem in treeView.Items.OfType<CycleNavigationTreeViewDataViewModel>())
            {
                firstLevelItem.ExpandAll();
                foreach (var cyclePackage in firstLevelItem.CyclePackages)
                {
                    var parentNodes = this.GetParentNodes(cyclePackage.StandardCycles.First());
                    parentNodes.CycleFolder.IsExpanded = true;
                    if (cyclePackage.EventCycles.Any())
                    {
                        var parentEventCycleNodes = this.GetParentNodes(cyclePackage.EventCycles.First());
                        parentEventCycleNodes.CycleFolder.IsExpanded = true;
                    }
                }
            }
        }

        private CycleNavigationTreeViewDataViewModel GetRootNode(CyclePackageConfigDataViewModel item)
        {
            return (from object screenTypeItem in this.SectionTreeView.Items
                    let screentypeContainer =
                        this.SectionTreeView.ItemContainerGenerator.ContainerFromItem(screenTypeItem) as TreeViewItem
                    where
                        screentypeContainer != null && screentypeContainer.Items.Count > 0
                        && screentypeContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated
                    where screentypeContainer.Items.Cast<object>().Any(typeItem => typeItem == item)
                    select screenTypeItem as CycleNavigationTreeViewDataViewModel).FirstOrDefault();
        }

        private TreeNodeResult GetParentNodes(object item, bool bringToFront = true)
        {
            var itemType = item.GetType();
            foreach (var screenTypeItem in this.SectionTreeView.Items)
            {
                var screentypeContainer =
                    this.SectionTreeView.ItemContainerGenerator.ContainerFromItem(screenTypeItem) as TreeViewItem;
                if (screentypeContainer == null || screentypeContainer.Items.Count <= 0
                    || screentypeContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    continue;
                }

                foreach (var cyclePackageItem in screentypeContainer.Items)
                {
                    var cyclePackageContainer =
                        screentypeContainer.ItemContainerGenerator.ContainerFromItem(cyclePackageItem) as
                        TreeViewItem;
                    if (cyclePackageContainer == null || cyclePackageContainer.Items.Count <= 0
                        || cyclePackageContainer.ItemContainerGenerator.Status
                        != GeneratorStatus.ContainersGenerated)
                    {
                        continue;
                    }

                    if (cyclePackageItem == item)
                    {
                        cyclePackageContainer.BringIntoView();
                        return new TreeNodeResult
                                   {
                                       RootNode = screenTypeItem as CycleNavigationTreeViewDataViewModel
                                   };
                    }

                    foreach (var cycleFolderItem in cyclePackageContainer.Items)
                    {
                        var cycleFolderItemContainer =
                            cyclePackageContainer.ItemContainerGenerator.ContainerFromItem(cycleFolderItem) as
                            TreeViewItem;
                        if (cycleFolderItem == item)
                        {
                            if (cycleFolderItemContainer != null && bringToFront)
                            {
                                cycleFolderItemContainer.BringIntoView();
                            }

                            return new TreeNodeResult
                                       {
                                           Cyclepackage =
                                               cyclePackageItem as CyclePackageConfigDataViewModel,
                                           RootNode =
                                               screenTypeItem as CycleNavigationTreeViewDataViewModel,
                                       };
                        }

                        if (cycleFolderItemContainer == null || cycleFolderItemContainer.Items.Count <= 0
                            || cycleFolderItemContainer.ItemContainerGenerator.Status
                            != GeneratorStatus.ContainersGenerated)
                        {
                            continue;
                        }

                        var treeNodeResult = TraverseCycleFolderItems(
                            item,
                            cycleFolderItemContainer,
                            itemType,
                            cyclePackageItem,
                            screenTypeItem,
                            cycleFolderItem,
                            bringToFront);
                        if (treeNodeResult != null)
                        {
                            return treeNodeResult;
                        }
                    }
                }
            }

            return null;
        }

        private void OnTreeViewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isCycleFolderSelected = false;
            this.isRootNodeSelected = false;
            var selectedItem = this.SectionTreeView.SelectedItem;
            var context = (CycleNavigationViewModel)this.DataContext;
            if (selectedItem == null || context == null || e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            this.previousSelectedItem = selectedItem as DataViewModelBase;

            this.ClearChildItemSelections();
            var firstLevelNode = selectedItem as CycleNavigationTreeViewDataViewModel;
            if (firstLevelNode != null)
            {
                this.SelectRootNode(context, firstLevelNode);
                return;
            }

            var parentNodes = this.GetParentNodes(selectedItem);
            if (parentNodes == null)
            {
                return;
            }

            this.SetParentNodes(parentNodes, context);

            if (selectedItem is CyclePackageConfigDataViewModel)
            {
                this.NavigateToCyclePackage(context, selectedItem);
                return;
            }

            if (selectedItem is TreeViewFolderItem)
            {
                this.isCycleFolderSelected = true;
                this.NavigateToCycleFolder(context, parentNodes, selectedItem);
                this.previousParentCyclesFolderItem = (TreeViewFolderItem)selectedItem;
                return;
            }

            if (parentNodes.CycleFolder != null)
            {
                parentNodes.CycleFolder.IsChildItemSelected = true;
                this.previousParentCyclesFolderItem = parentNodes.CycleFolder;
            }

            if (selectedItem is CycleRefConfigDataViewModelBase)
            {
                this.NavigateToCycle(context, selectedItem);
                return;
            }

            if (parentNodes.Cycle != null)
            {
                parentNodes.Cycle.IsChildItemSelected = true;
                this.previousParentCycle = parentNodes.Cycle;
            }

            if (selectedItem is SectionConfigDataViewModelBase)
            {
                if (parentNodes.Cycle != null && parentNodes.Cycle.Reference != context.CurrentCycle)
                {
                    this.NavigateToCycle(context, parentNodes.Cycle);
                }

                this.NavigateToSection(context, selectedItem);
            }
        }

        private void SetParentNodes(TreeNodeResult parentNodes, CycleNavigationViewModel context)
        {
            if (parentNodes.RootNode != null)
            {
                context.SelectedCycleNavigationTreeViewDataViewModel = parentNodes.RootNode;
                context.SelectedCycleNavigationTreeViewDataViewModel.IsChildItemSelected = true;
                this.previousParentRootNode = parentNodes.RootNode;
            }

            if (parentNodes.Cyclepackage != null)
            {
                parentNodes.Cyclepackage.IsChildItemSelected = true;
                this.previousParentCyclePackage = parentNodes.Cyclepackage;
                if (parentNodes.Cyclepackage != context.Parent.MediaApplicationState.CurrentCyclePackage)
                {
                    context.ChooseCyclePackage.Execute(parentNodes.Cyclepackage);
                }
            }
        }

        private void SelectRootNode(
            CycleNavigationViewModel context,
            CycleNavigationTreeViewDataViewModel firstLevelNode)
        {
            this.isRootNodeSelected = true;
            context.ChooseCyclePackage.Execute(firstLevelNode.CyclePackages.First());
            context.SelectedCycleNavigationTreeViewDataViewModel = firstLevelNode;
        }

        private void ClearChildItemSelections()
        {
            if (this.previousParentCycle != null)
            {
                this.previousParentCycle.IsChildItemSelected = false;
            }

            if (this.previousParentCyclePackage != null)
            {
                this.previousParentCyclePackage.IsChildItemSelected = false;
            }

            if (this.previousParentCyclesFolderItem != null)
            {
                this.previousParentCyclesFolderItem.IsChildItemSelected = false;
            }

            if (this.previousParentRootNode != null)
            {
                this.previousParentRootNode.IsChildItemSelected = false;
            }
        }

        private void NavigateToCycleFolder(
            CycleNavigationViewModel context,
            TreeNodeResult parentNodes,
            object selectedItem)
        {
            var treeViewItemFolder = (TreeViewFolderItem)selectedItem;
            if (parentNodes.Cyclepackage != context.CurrentCyclePackage)
            {
                this.NavigateToCyclePackage(context, parentNodes.Cyclepackage);
            }

            context.SelectedNavigation = CycleNavigationSelection.Cycle;

            var localizedNames = MediaStrings.CycleDetailsNavigator_TreeViewCycleTypes.Split(',').Select(f => f.Trim());
            CycleRefConfigDataViewModelBase cycle;
            if (treeViewItemFolder.Name == localizedNames.Last())
            {
                cycle = parentNodes.Cyclepackage == null ? null : parentNodes.Cyclepackage.StandardCycles.First();
            }
            else
            {
                if (parentNodes.Cyclepackage != null && !parentNodes.Cyclepackage.EventCycles.Any())
                {
                    cycle = parentNodes.Cyclepackage == null ? null : parentNodes.Cyclepackage.StandardCycles.First();
                }
                else
                {
                    cycle = parentNodes.Cyclepackage == null
                                    ? null
                                    : parentNodes.Cyclepackage.EventCycles.FirstOrDefault();
                }
            }

            this.NavigateToCycle(context, cycle);
            if (cycle != null)
            {
                cycle.IsItemSelected = false;
            }

            treeViewItemFolder.IsItemSelected = true;
            treeViewItemFolder.IsChildItemSelected = false;
        }

        private void NavigateToCycle(CycleNavigationViewModel context, object selectedItem)
        {
            context.HighlightedCycle = selectedItem as CycleRefConfigDataViewModelBase;
            context.ChooseCycle.Execute(selectedItem);
            context.SelectedNavigation = CycleNavigationSelection.Cycle;
            this.RenewCyclePropertyChangedEventRegistration(context);
            this.deferPropertyGridCycleUpdateTimer.Stop();
            this.deferPropertyGridCycleUpdateTimer.Start();
        }

        private void NavigateToSection(CycleNavigationViewModel context, object selectedItem)
        {
            context.ChooseSection.Execute(selectedItem);
            context.SelectedNavigation = CycleNavigationSelection.Section;
            context.HighlightedSection = selectedItem as SectionConfigDataViewModelBase;
            this.RenewSectionPropertyChangedEventRegistration(context);
            this.deferPropertyGridSectionUpdateTimer.Stop();
            this.deferPropertyGridSectionUpdateTimer.Start();
        }

        private void NavigateToCyclePackage(CycleNavigationViewModel context, object selectedItem)
        {
            context.ChooseCyclePackage.Execute(selectedItem);
            context.SelectedNavigation = CycleNavigationSelection.CyclePackage;
            context.HighlightedCyclePackage = selectedItem as CyclePackageConfigDataViewModel;
            this.RenewCyclePackagePropertyChangedEventRegistration(context);
            this.deferPropertyGridCyclePackageUpdateTimer.Stop();
            this.deferPropertyGridCyclePackageUpdateTimer.Start();
        }

        private void OnSelectedItemChanged(object sender, ReusableList.SelectReusableEntityEventArgs e)
        {
            var context = (CycleNavigationViewModel)this.DataContext;
            if (context == null)
            {
                return;
            }

            if (this.previousSelectedItem == e.Entity)
            {
                return;
            }

            if (this.isCycleFolderSelected)
            {
                this.isCycleFolderSelected = false;
                if (this.previousSelectedItem != null)
                {
                    this.previousSelectedItem.IsItemSelected = true;
                    ((TreeViewFolderItem)this.previousSelectedItem).IsChildItemSelected = false;
                }

                return;
            }

            if (this.isRootNodeSelected)
            {
                this.isRootNodeSelected = false;
                if (this.previousSelectedItem != null)
                {
                    this.previousSelectedItem.IsItemSelected = true;
                    ((CycleNavigationTreeViewDataViewModel)this.previousSelectedItem).IsChildItemSelected = false;
                }

                return;
            }

            if (this.previousSelectedItem != null)
            {
                this.previousSelectedItem.IsItemSelected = false;
            }

            this.previousSelectedItem = e.Entity as DataViewModelBase;
            if (this.previousSelectedItem == null)
            {
                return;
            }

            this.ClearChildItemSelections();
            this.previousSelectedItem.IsItemSelected = true;
            var nodes = this.GetParentNodes(e.Entity);

            if (nodes == null)
            {
                return;
            }

            this.UpdateParentNodeSelections(nodes);
        }

        private void UpdateParentNodeSelections(TreeNodeResult nodes)
        {
            if (nodes.Cycle != null)
            {
                nodes.Cycle.IsChildItemSelected = true;
                this.previousParentCycle = nodes.Cycle;
                this.previousParentCycle.IsExpanded = true;
            }

            if (nodes.CycleFolder != null)
            {
                nodes.CycleFolder.IsChildItemSelected = true;
                this.previousParentCyclesFolderItem = nodes.CycleFolder;
                this.previousParentCyclesFolderItem.IsExpanded = true;
            }

            if (nodes.Cyclepackage != null)
            {
                nodes.Cyclepackage.IsChildItemSelected = true;
                this.previousParentCyclePackage = nodes.Cyclepackage;
                this.previousParentCyclePackage.IsExpanded = true;
            }

            if (nodes.RootNode != null)
            {
                nodes.RootNode.IsChildItemSelected = true;
                this.previousParentRootNode = nodes.RootNode;
                this.previousParentRootNode.IsExpanded = true;
            }
        }

        private class TreeNodeResult
        {
            public CyclePackageConfigDataViewModel Cyclepackage { get; set; }

            public CycleNavigationTreeViewDataViewModel RootNode { get; set; }

            public TreeViewFolderItem CycleFolder { get; set; }

            public CycleRefConfigDataViewModelBase Cycle { get; set; }
        }
    }
}
