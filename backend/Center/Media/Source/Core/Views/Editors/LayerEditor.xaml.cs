// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayerEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Views.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for LayerEditor.xaml
    /// </summary>
    public partial class LayerEditor
    {
        /// <summary>
        /// the selected element
        /// </summary>
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            "Selected",
            typeof(ExtendedObservableCollection<LayoutElementDataViewModelBase>),
            typeof(LayerEditor),
            new PropertyMetadata(
                default(ExtendedObservableCollection<LayoutElementDataViewModelBase>), SelectedChanged));

        /// <summary>
        /// The elements
        /// </summary>
        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(
            "Elements",
            typeof(ExtendedObservableCollection<GraphicalElementDataViewModelBase>),
            typeof(LayerEditor),
            new PropertyMetadata(default(ExtendedObservableCollection<GraphicalElementDataViewModelBase>)));

        private bool suspendChangeEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerEditor"/> class.
        /// </summary>
        public LayerEditor()
        {
            this.InitializeComponent();

            this.Selected = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
        }

        /// <summary>
        /// Gets or sets the selected elements
        /// </summary>
        public ExtendedObservableCollection<LayoutElementDataViewModelBase> Selected
        {
            get
            {
                return (ExtendedObservableCollection<LayoutElementDataViewModelBase>)this.GetValue(SelectedProperty);
            }

            set
            {
                this.SetValue(SelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the elements
        /// </summary>
        public ExtendedObservableCollection<GraphicalElementDataViewModelBase> Elements
        {
            get
            {
                return (ExtendedObservableCollection<GraphicalElementDataViewModelBase>)this.GetValue(ElementsProperty);
            }

            set
            {
                this.SetValue(ElementsProperty, value);
            }
        }

        private static void SelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var instance = (LayerEditor)dependencyObject;
            if (e.OldValue != null)
            {
                ((ExtendedObservableCollection<LayoutElementDataViewModelBase>)e.OldValue).CollectionChanged -=
                    instance.OnSelectedElementsChanged;
            }

            if (e.NewValue != null)
            {
                ((ExtendedObservableCollection<LayoutElementDataViewModelBase>)e.NewValue).CollectionChanged +=
                    instance.OnSelectedElementsChanged;
            }
        }

        private void OnSelectedElementsChanged(
            object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.suspendChangeEvents = true;
            this.LayerList.UnselectAll();
            if (this.Selected != null)
            {
                foreach (var selectedElement in this.Selected)
                {
                    this.LayerList.SelectedItems.Add(selectedElement);
                }
            }

            this.suspendChangeEvents = false;
        }

        private void OnElementNameChanged(string oldElementName, string newElementName, object sourceObject)
        {
            var graphicalElement = (LayoutElementDataViewModelBase)sourceObject;
            var context = (MediaShell)this.DataContext;

            var oldGraphicalElements = new List<LayoutElementDataViewModelBase>();
            var newGraphicalElements = new List<LayoutElementDataViewModelBase>();

            oldGraphicalElements.Add((LayoutElementDataViewModelBase)graphicalElement.Clone());

            var newGraphicalElement = (LayoutElementDataViewModelBase)graphicalElement.Clone();
            newGraphicalElement.ElementName.Value = newElementName;
            newGraphicalElements.Add(newGraphicalElement);

            var elements = new List<LayoutElementDataViewModelBase> { graphicalElement };

            var parameters = new UpdateEntityParameters(
                oldGraphicalElements, newGraphicalElements, elements);

            context.RenameLayoutElement.Execute(parameters);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.suspendChangeEvents)
            {
                foreach (var removedItem in e.RemovedItems)
                {
                    this.Selected.Remove((LayoutElementDataViewModelBase)removedItem);
                }

                foreach (var addedItem in e.AddedItems)
                {
                    this.Selected.Add((LayoutElementDataViewModelBase)addedItem);
                }
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var index = this.GetItemIndexAtPosition(e.GetPosition);
            if (index != -1)
            {
                var item = this.Elements[index];
                var graphicalItem = item;
                var oldElements = new List<LayoutElementDataViewModelBase>
                                      {
                                          (LayoutElementDataViewModelBase)graphicalItem.Clone()
                                      };
                var context = (MediaShell)this.DataContext;
                    graphicalItem.Visible.Value = !graphicalItem.Visible.Value;
                    var newElements = new List<LayoutElementDataViewModelBase>
                                          {
                                              (LayoutElementDataViewModelBase)graphicalItem.Clone()
                                          };
                    var parameters = new UpdateEntityParameters(oldElements, newElements, context.Editor.Elements);
                    ((EditorViewModelBase)context.Editor).UpdateElementCommand.Execute(parameters);
                    InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            }
        }

        private int GetItemIndexAtPosition(Func<IInputElement, Point> getPosition)
        {
            int result = -1;

            for (int i = 0; i < this.LayerList.Items.Count; ++i)
            {
                var item = this.GetListViewItem(i);
                if (this.IsMouseOverTarget(item, getPosition))
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        private bool IsMouseOverTarget(Visual target, Func<IInputElement, Point> getPosition)
        {
            if (target != null)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(target);
                var mousePos = getPosition((IInputElement)target);
                return bounds.Contains(mousePos);
            }

            return false;
        }

        private ListViewItem GetListViewItem(int index)
        {
            if (this.LayerList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return this.LayerList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }
    }
}
