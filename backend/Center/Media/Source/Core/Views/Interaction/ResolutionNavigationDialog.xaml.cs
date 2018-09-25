// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionNavigationDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ResolutionNavigationDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// Interaction logic for ResolutionNavigationDialog.xaml
    /// </summary>
    public partial class ResolutionNavigationDialog
    {
        /// <summary>
        /// the Editor Callbacks property
        /// </summary>
        public static readonly DependencyProperty EditorCallbacksProperty =
            DependencyProperty.Register(
                "EditorCallbacks",
                typeof(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>),
                typeof(ResolutionNavigationDialog),
                new PropertyMetadata(default(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan DeferTimerInterval = TimeSpan.FromMilliseconds(1);

        private readonly DispatcherTimer deferPropertyGridUpdateTimer;

        private bool isChangingInGrid;

        private bool isChangingInEditor;

        private List<PhysicalScreenConfigDataViewModel> oldElements;

        private PhysicalScreenConfigDataViewModel previousHighlightedPhysicalScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionNavigationDialog"/> class.
        /// </summary>
        public ResolutionNavigationDialog()
        {
            this.InitializeComponent();
            this.deferPropertyGridUpdateTimer = new DispatcherTimer
            {
                Interval = DeferTimerInterval
            };
            this.deferPropertyGridUpdateTimer.Tick += this.PropertyGridUpdateTimerElapsed;
            this.Loaded += this.OnLoaded;
            this.EditorCallbacks = new Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>();
        }

        /// <summary>
        /// Gets the update properties command
        /// </summary>
        public ICommand UpdateResolutionPropertiesCommand
        {
            get
            {
                return new RelayCommand(this.UpdatePropertyGrid);
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

        /// <summary>
        /// Gets the refresh property grid interaction request
        /// </summary>
        public IInteractionRequest RefreshResolutionPropertyGridRequest
        {
            get
            {
                return InteractionManager<UpdateResolutionDetailsPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        private void OnPhysicalScreenHighlightChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is ResolutionNavigationPrompt context))
            {
                return;
            }

            this.RenewScreenPropertyChangedEventRegistration(context);
            this.deferPropertyGridUpdateTimer.Stop();
            this.deferPropertyGridUpdateTimer.Start();
        }

        private void RenewScreenPropertyChangedEventRegistration(ResolutionNavigationPrompt context)
        {
            if (this.previousHighlightedPhysicalScreen != null)
            {
                this.previousHighlightedPhysicalScreen.PropertyChanged -= this.PhysicalScreenPropertyChanged;
            }

            this.previousHighlightedPhysicalScreen = context.HighlightedPhysicalScreen;

            if (context.HighlightedPhysicalScreen != null)
            {
                context.HighlightedPhysicalScreen.PropertyChanged += this.PhysicalScreenPropertyChanged;
            }
        }

        private void PhysicalScreenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChangingInGrid)
            {
                return;
            }

            var element = (PhysicalScreenConfigDataViewModel)sender;
            var elements = new ObservableCollection<PhysicalScreenConfigDataViewModel> { element };

            this.OnPropertyChangeSetDataSource(e, element, elements);

            this.deferPropertyGridUpdateTimer.Stop();
            this.deferPropertyGridUpdateTimer.Start();
        }

        private void OnPropertyChangeSetDataSource<T>(
           PropertyChangedEventArgs e,
           T element,
           IEnumerable<T> elements)
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

        private void PropertyGridUpdateTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.deferPropertyGridUpdateTimer.Stop();
            this.UpdatePropertyGrid();
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // Only initialize the ContextMenu once, otherwise the PlacementTarget will be null
            if (this.PropertyGrid != null && this.PropertyGrid.ItemContextMenu == null)
            {
                PropertyGridContextMenuFactory.SetupDefaultContextMenu(
                    this.PropertyGrid,
                    new RelayCommand(this.OnOpenFormulaEditor),
                    new RelayCommand(
                        this.OnRemoveFormulaCommandTriggered,
                        this.OnCanExecuteRemoveFormulaCommandTriggered));
            }
        }

        private void OnRemoveFormulaCommandTriggered(object o)
        {
            if (!(this.DataContext is ResolutionNavigationPrompt context))
            {
                return;
            }

            context.RemoveFormulaCommand.Execute(o);
        }

        private bool OnCanExecuteRemoveFormulaCommandTriggered(object o)
        {
            if (!(this.DataContext is ResolutionNavigationPrompt context))
            {
                return false;
            }

            var result = context.RemoveFormulaCommand.CanExecute(o);
            return result;
        }

        private void UpdatePropertyGrid()
        {
            if (!(this.DataContext is ResolutionNavigationPrompt context))
            {
                return;
            }

            if (context.HighlightedPhysicalScreen != null)
            {
                var elements = new ObservableCollection<PhysicalScreenConfigDataViewModel>
                                   {
                                       context
                                           .HighlightedPhysicalScreen
                                   };

                if (this.HasChanged())
                {
                    var dataContext = (MediaShell)context.Parent;
                    var editor = dataContext.Editor as EditorViewModelBase;
                    if (editor != null)
                    {
                        var newElements = elements.Select(e => (PhysicalScreenConfigDataViewModel)e.Clone()).ToList();
                        var parameters = new UpdateEntityParameters(this.oldElements, newElements, elements);
                        editor.UpdateElementCommand.Execute(parameters);
                    }
                }

                this.PropertyGrid.AddDomainValue("Resolution", context.AvailableResolutions);
                this.PropertyGrid.UpdateContent(
                    elements,
                    this.OnGridItemPropertyChanged,
                    text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text),
                    this.GetToolTip,
                    filter: context.HighlightedPhysicalScreen.Type.Value.ToString());
            }
            else
            {
                this.PropertyGrid.Clear(this.OnGridItemPropertyChanged);
            }

            this.ExpandPropertyGridGroups();
        }

        private string GetToolTip(string text)
        {
            if (text.Equals("identifier", StringComparison.InvariantCultureIgnoreCase))
            {
                var context = (ResolutionNavigationPrompt)this.DataContext;
                var physicalScreen = context.HighlightedPhysicalScreen;
                return MediaStrings.ResourceManager.GetString(
                    "PropertyGridField_" + physicalScreen.Type.Value + "_" + text + "Tooltip");
            }

            return MediaStrings.ResourceManager.GetString("PropertyGridField_" + text + "Tooltip");
        }

        private void ExpandPropertyGridGroups()
        {
            foreach (var gridGroup in this.PropertyGrid.Groups)
            {
                gridGroup.IsExpanded = true;
            }
        }

        private void OnGridItemPropertyChanged(PropertyGridItem propertyGridItem, object datasource)
        {
            if (this.isChangingInEditor)
            {
                return;
            }

            this.isChangingInGrid = true;

            this.UpdateElements(propertyGridItem, datasource);

            this.isChangingInGrid = false;
        }

        private void UpdateElements(PropertyGridItem propertyGridItem, object datasource)
        {
            var oldScreenElements = new List<PhysicalScreenConfigDataViewModel>();
            var newElements = new List<PhysicalScreenConfigDataViewModel>();

            var context = (ResolutionNavigationPrompt)this.DataContext;

            var elements = new ObservableCollection<PhysicalScreenConfigDataViewModel>
                               {
                                   context
                                       .HighlightedPhysicalScreen
                               };

            foreach (var element in elements)
            {
                try
                {
                    oldScreenElements.Add((PhysicalScreenConfigDataViewModel)element.Clone());
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

                    newElements.Add((PhysicalScreenConfigDataViewModel)element.Clone());
                }
                catch (Exception exception)
                {
                    Logger.WarnException("Exception while Converting PropertyGrid Value.", exception);
                    this.UpdatePropertyGrid();
                }
            }

            var parameters = new UpdateEntityParameters(oldScreenElements, newElements, elements);
            var dataContext = (MediaShell)context.Parent;
            var editor = dataContext.Editor as EditorViewModelBase;
            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private bool HasChanged()
        {
            var result = false;

            var context = (ResolutionNavigationPrompt)this.DataContext;

            var elements = new ObservableCollection<PhysicalScreenConfigDataViewModel>
                                   {
                                       context.HighlightedPhysicalScreen
                                   };

            if (this.oldElements != null)
            {
                var hasEqual = false;
                foreach (var oldElement in this.oldElements)
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

        private void OnOpenFormulaEditor()
        {
            var context = (ResolutionNavigationPrompt)this.DataContext;

            this.oldElements = new List<PhysicalScreenConfigDataViewModel>
                                      {
                                          (PhysicalScreenConfigDataViewModel)context.HighlightedPhysicalScreen.Clone()
                                      };

            context.ShowFormulaEditorCommand.Execute(this.PropertyGrid.ItemContextMenu);
        }
    }
}
