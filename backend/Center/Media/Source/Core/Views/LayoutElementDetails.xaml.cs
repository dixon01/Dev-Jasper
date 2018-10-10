// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementDetails.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LayoutElementDetails.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LayoutElementDetails
    {
        /// <summary>
        /// the element property
        /// </summary>
        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(
            "Elements",
            typeof(ExtendedObservableCollection<LayoutElementDataViewModelBase>),
            typeof(LayoutElementDetails),
            new PropertyMetadata(
                default(ExtendedObservableCollection<LayoutElementDataViewModelBase>), OnElementsChanged));

        /// <summary>
        /// the Editor Callbacks property
        /// </summary>
        public static readonly DependencyProperty EditorCallbacksProperty =
            DependencyProperty.Register(
                "EditorCallbacks",
                typeof(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>),
                typeof(LayoutElementDetails),
                new PropertyMetadata(default(Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>)));

        /// <summary>
        /// The navigation callbacks property.
        /// </summary>
        public static readonly DependencyProperty NavigationCallbacksProperty =
            DependencyProperty.Register(
                "NavigationCallbacks",
                typeof(Dictionary<string, KeyValuePair<string, Action>>),
                typeof(LayoutElementDetails),
                new PropertyMetadata(default(Dictionary<string, KeyValuePair<string, Action>>)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan DeferTimerInterval = TimeSpan.FromMilliseconds(1);

        private readonly DispatcherTimer deferPropertyGridUpdateTimer;

        private bool isChanginginGrid;

        private bool isChangingInEditor;

        private List<LayoutElementDataViewModelBase> oldElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutElementDetails"/> class.
        /// </summary>
        public LayoutElementDetails()
        {
            this.InitializeComponent();

            this.deferPropertyGridUpdateTimer = new DispatcherTimer
                                                {
                                                    Interval = DeferTimerInterval
                                                };
            this.deferPropertyGridUpdateTimer.Tick += this.PropertyGridUpdateTimerElapsed;
            this.Loaded += this.OnLoaded;

            this.EditorCallbacks = new Dictionary<Type, Action<PropertyGridItem, PropertyGridItemDataSource>>();
            this.NavigationCallbacks = new Dictionary<string, KeyValuePair<string, Action>>();
            this.NavigationCallbacks.Add(
                "FontFace",
                new KeyValuePair<string, Action>(MediaStrings.PropertyGrid_NavigateMediaText, this.ShowMainMenuPrompt));
                this.PropertyGrid.FieldsCreated += this.PropertyGridOnFieldsCreated;
        }

        /// <summary>
        /// Gets or sets the navigation callbacks.
        /// </summary>
        public Dictionary<string, KeyValuePair<string, Action>> NavigationCallbacks
        {
            get
            {
                return (Dictionary<string, KeyValuePair<string, Action>>)this.GetValue(NavigationCallbacksProperty);
            }

            set
            {
                this.SetValue(NavigationCallbacksProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the currently displayed element
        /// </summary>
        public ObservableCollection<LayoutElementDataViewModelBase> Elements
        {
            get
            {
                return (ObservableCollection<LayoutElementDataViewModelBase>)this.GetValue(ElementsProperty);
            }

            set
            {
                this.SetValue(ElementsProperty, value);
            }
        }

        /// <summary>
        /// Gets the refresh property grid interaction request
        /// </summary>
        public IInteractionRequest RefreshPropertyGridRequest
        {
            get
            {
                return InteractionManager<UpdateLayoutDetailsPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the update properties command
        /// </summary>
        public ICommand UpdatePropertiesCommand
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

        private static void OnElementsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var instance = (LayoutElementDetails)dependencyObject;
            var previousElements = (ExtendedObservableCollection<LayoutElementDataViewModelBase>)e.OldValue;
            if (previousElements != null)
            {
                previousElements.CollectionChanged -= instance.OnElementListChanged;

                foreach (var element in previousElements)
                {
                    element.PropertyChanged -= instance.LayoutElementPropertyChanged;
                }
            }

            var elements = (ExtendedObservableCollection<LayoutElementDataViewModelBase>)e.NewValue;
            if (elements != null)
            {
                elements.CollectionChanged += instance.OnElementListChanged;

                foreach (var element in elements)
                {
                    element.PropertyChanged += instance.LayoutElementPropertyChanged;
                }
            }
        }

        private void ShowMainMenuPrompt()
        {
            var context = (IMediaShell)this.DataContext;
            var parameters = new MenuNavigationParameters
                             {
                                 Root = MenuNavigationParameters.MainMenuEntries.FileResourceManager,
                                 SubMenu = "Fonts"
                             };
            context.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu).Execute(parameters);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            var shell = (MediaShell)this.DataContext;

            PropertyGridContextMenuFactory.SetupDefaultContextMenu(
                this.PropertyGrid,
                new RelayCommand(this.OnOpenFormulaEditor),
                new RelayCommand(this.OnOpenAnimationEditor),
                shell.RemoveLayoutFormulaCommand,
                shell.RemoveLayoutAnimationCommand);
        }

        private void OnOpenFormulaEditor()
        {
            var dataContext = (MediaShell)this.DataContext;

            this.oldElements = this.Elements.Select(e => (LayoutElementDataViewModelBase)e.Clone()).ToList();

            dataContext.ShowFormulaEditorCommand.Execute(this.PropertyGrid.ItemContextMenu);
        }

        private void OnOpenAnimationEditor()
        {
            var dataContext = (MediaShell)this.DataContext;

            this.oldElements = this.Elements.Select(e => (LayoutElementDataViewModelBase)e.Clone()).ToList();

            dataContext.ShowAnimationEditorCommand.Execute(this.PropertyGrid.ItemContextMenu);
        }

        private void LayoutElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isChanginginGrid)
            {
                return;
            }

            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            var element = (LayoutElementDataViewModelBase)sender;
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
                        if (this.PropertyGrid.DataIsSameForAll(data, propertyInfo, this.Elements))
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

        private void OnElementListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    ((LayoutElementDataViewModelBase)oldItem).PropertyChanged -= this.LayoutElementPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    ((LayoutElementDataViewModelBase)newItem).PropertyChanged += this.LayoutElementPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var oldItem in this.Elements)
                {
                    oldItem.PropertyChanged -= this.LayoutElementPropertyChanged;
                }
            }

            this.oldElements =
                this.Elements.Select(element => (LayoutElementDataViewModelBase)element.Clone()).ToList();
            this.deferPropertyGridUpdateTimer.Stop();
            this.deferPropertyGridUpdateTimer.Start();
        }

        private void PropertyGridUpdateTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.deferPropertyGridUpdateTimer.Stop();
            this.UpdatePropertyGrid();
        }

        private void PropertyGridOnFieldsCreated(object sender, FieldsCreatedEventArgs fieldsCreatedEventArgs)
        {
            var elementsList = fieldsCreatedEventArgs.Elements.OfType<VideoElementDataViewModel>().ToList();

            // handling multiple selected items too. If at least one is a Video element (and not a LiveStream element)
            // the FallbackImage property should be removed
            if (!elementsList.Any() || elementsList.All(e => e.GetType() != typeof(VideoElementDataViewModel)))
            {
                return;
            }

            var fallbackImage = fieldsCreatedEventArgs.Fields.FirstOrDefault(item => item.FieldName == "FallbackImage");
            if (fallbackImage == null)
            {
                return;
            }

            fieldsCreatedEventArgs.Fields.Remove(fallbackImage);
        }

        private void UpdatePropertyGrid()
        {
            var shell = this.DataContext as IMediaShell;
            if (shell == null)
            {
                // Control not yet loaded
                return;
            }

            if (this.HasChanged())
            {
                var editor = ((MediaShell)this.DataContext).Editor as EditorViewModelBase;
                if (editor != null)
                {
                    this.EnsurePredefinedFormulaReferences();

                    var newElements = this.Elements.Select(e => (LayoutElementDataViewModelBase)e.Clone()).ToList();
                    UpdateEntityParameters parameters;
                    if (this.Elements.First() is PlaybackElementDataViewModelBase)
                    {
                        parameters = new UpdateEntityParameters(
                            this.oldElements,
                            newElements,
                            editor.CurrentAudioOutputElement.Elements);
                    }
                    else if (this.Elements.First() is AudioOutputElementDataViewModel)
                    {
                        var elementContainer =
                            ((LayoutConfigDataViewModel)editor.Parent.MediaApplicationState.CurrentLayout).Resolutions
                                .First().Elements;
                        parameters = new UpdateEntityParameters(
                            this.oldElements,
                            newElements,
                            elementContainer);
                    }
                    else
                    {
                        parameters = new UpdateEntityParameters(this.oldElements, newElements, editor.Elements);
                    }

                    editor.UpdateElementCommand.Execute(parameters);
                    this.oldElements = this.Elements.Select(e => (LayoutElementDataViewModelBase)e.Clone()).ToList();
                }
            }

            this.oldElements = null;
            this.UpdateDomainValueCollection(shell);
            var physicalScreenConfigDataViewModel =
                ((IMediaShell)this.DataContext).MediaApplicationState.CurrentPhysicalScreen;
            if (physicalScreenConfigDataViewModel != null)
            {
                IEnumerable<string> multiSelectFields = null;
                if (physicalScreenConfigDataViewModel.Type.Value == PhysicalScreenType.LED)
                {
                    multiSelectFields = new List<string> { "FontFace" };
                }

                this.PropertyGrid.UpdateContent(
                   this.Elements,
                   this.PropertyGridItemPropertyChanged,
                   text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text),
                   text => MediaStrings.ResourceManager.GetString("PropertyGridField_" + text + "Tooltip"),
                   multiSelectFields,
                   physicalScreenConfigDataViewModel.Type.Value.ToString());
            }

            this.ExpandPropertyGridGroups();
        }

        private void EnsurePredefinedFormulaReferences()
        {
            foreach (var element in this.Elements)
            {
                var properties = element.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (typeof(IDynamicDataValue).IsAssignableFrom(property.PropertyType))
                    {
                        var dynamicDataValue = (IDynamicDataValue)property.GetValue(element, new object[0]);
                        if (dynamicDataValue != null && dynamicDataValue.Formula != null)
                        {
                            dynamicDataValue.Formula = dynamicDataValue.Formula;
                        }
                    }
                }
            }
        }

        private bool HasChanged()
        {
            var result = false;

            if (this.oldElements != null)
            {
                var hasChange = false;
                foreach (var oldElement in this.oldElements)
                {
                    var original = this.Elements.FirstOrDefault(elem => elem.GetHashCode() == oldElement.ClonedFrom);

                    if (original != null && !original.EqualsViewModel(oldElement))
                    {
                        hasChange = true;
                        break;
                    }
                }

                result = hasChange;
            }

            return result;
        }

        private void ExpandPropertyGridGroups()
        {
            foreach (var gridGroup in this.PropertyGrid.Groups)
            {
                gridGroup.IsExpanded = true;
            }
        }

        private void PropertyGridItemPropertyChanged(PropertyGridItem item, object datasource)
        {
            if (this.isChangingInEditor)
            {
                return;
            }

            this.isChanginginGrid = true;

            var oldLayoutElements = new List<LayoutElementDataViewModelBase>();
            var newElements = new List<LayoutElementDataViewModelBase>();
            foreach (var element in this.Elements)
            {
                try
                {
                    oldLayoutElements.Add((LayoutElementDataViewModelBase)element.Clone());
                    var property = element.GetType().GetProperty(item.Name);
                    var concreteValue = (IDataValue)property.GetValue(element, null);
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
                        if (property.CanWrite)
                        {
                            PropertyGrid.SetConvertedValue(concreteValue, datasource, property, element);
                        }
                    }

                    newElements.Add((LayoutElementDataViewModelBase)element.Clone());
                }
                catch (Exception exception)
                {
                    Logger.WarnException("Exception while Converting PropertyGrid Value.", exception);
                    this.UpdatePropertyGrid();
                }
            }

            var editor = ((MediaShell)this.DataContext).Editor as EditorViewModelBase;
            if (editor != null && newElements.Count > 0 && oldLayoutElements.Count > 0)
            {
                UpdateEntityParameters parameters;
                if (newElements.First() is AudioOutputElementDataViewModel)
                {
                    parameters = new UpdateEntityParameters(
                        oldLayoutElements,
                        newElements,
                        new List<AudioOutputElementDataViewModel> { editor.CurrentAudioOutputElement });
                }
                else if (newElements.First() is PlaybackElementDataViewModelBase)
                {
                    parameters = new UpdateEntityParameters(
                        oldLayoutElements,
                        newElements,
                        editor.CurrentAudioOutputElement.Elements);
                }
                else
                {
                    parameters = new UpdateEntityParameters(oldLayoutElements, newElements, editor.Elements);
                }

                editor.UpdateElementCommand.Execute(parameters);
            }

            this.isChanginginGrid = false;
        }

        private void UpdateDomainValueCollection(IMediaShell shell)
        {
            var currentProject = shell.MediaApplicationState.CurrentProject;
            if (currentProject == null || currentProject.InfomediaConfig == null)
            {
                return;
            }

            var physicalScreen = shell.MediaApplicationState.CurrentPhysicalScreen;
            if (physicalScreen != null && physicalScreen.Type.Value == PhysicalScreenType.TFT)
            {
                this.PropertyGrid.AddDomainValue("FontFace", currentProject.AvailableFonts);
                return;
            }

            this.PropertyGrid.AddDomainValue("FontFace", currentProject.AvailableLedFonts);
        }
    }
}