// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaneGroupExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PaneGroupExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The PaneGroupExtensions.
    /// </summary>
    public class PaneGroupExtensions
    {
        private RadPaneGroup Group { get; set; }

        private Dictionary<object, RadPane> Panes { get; set; }

        public static readonly DependencyProperty ItemHeaderTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemHeaderTemplate",
                typeof(DataTemplate),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(OnItemHeaderTemplateChanged));

        public static readonly DependencyProperty ItemCanFloatProperty =
            DependencyProperty.RegisterAttached(
                "ItemCanFloat",
                typeof(bool),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(OnItemCanFloatChanged));

        public static readonly DependencyProperty ItemTitleTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemTitleTemplate",
                typeof(DataTemplate),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(OnItemTitleTemplateChanged));

        public static readonly DependencyProperty ItemTitleDisplayMemberPathProperty =
            DependencyProperty.RegisterAttached(
                "ItemTitleDisplayMemberPath",
                typeof(string),
                typeof(PaneGroupExtensions),
                new PropertyMetadata("", OnItemTitleDisplayMemberPathChanged));

        public static readonly DependencyProperty ItemStyleProperty =
            DependencyProperty.RegisterAttached(
                "ItemStyle",
                typeof(Style),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(OnItemStyleChanged));

        public static readonly DependencyProperty ItemContentTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemContentTemplate",
                typeof(DataTemplate),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(OnItemContentTemplateChanged));

        public static readonly DependencyProperty ItemResourcesProperty =
            DependencyProperty.RegisterAttached(
                "ItemResources",
                typeof(ResourceDictionary),
                typeof(PaneGroupExtensions));

        public static readonly DependencyProperty ItemContentTemplateSelectorProperty =
            DependencyProperty.RegisterAttached(
                "ItemContentTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(null, OnItemContentTemplateSelectorChanged));

        private static readonly DependencyProperty PaneGroupExtensionProperty =
            DependencyProperty.RegisterAttached(
                "PaneGroupExtension",
                typeof(PaneGroupExtensions),
                typeof(PaneGroupExtensions),
                null);

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached(
                "Template",
                typeof(ControlTemplate),
                typeof(PaneGroupExtensions),
                new PropertyMetadata(null, OnTemplateChanged));

        private PaneGroupExtensions()
        {
            this.Panes = new Dictionary<object, RadPane>();
        }

        public static bool GetItemCanFloat(DependencyObject obj)
        {
            return (bool)obj.GetValue(ItemCanFloatProperty);
        }

        public static void SetItemCanFloat(DependencyObject obj, bool value)
        {
            obj.SetValue(ItemCanFloatProperty, value);
        }

        public static DataTemplate GetItemHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(ItemHeaderTemplateProperty);
        }

        public static void SetItemHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(ItemHeaderTemplateProperty, value);
        }

        public static void SetItemTitleTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(ItemTitleTemplateProperty, value);
        }

        public static string GetItemTitleDisplayMemberPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ItemTitleDisplayMemberPathProperty);
        }

        public static void SetItemTitleDisplayMemberPath(DependencyObject obj, string value)
        {
            obj.SetValue(ItemTitleDisplayMemberPathProperty, value);
        }

        public static DataTemplate GetItemTitleTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(ItemTitleTemplateProperty);
        }

        public static ControlTemplate GetTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        public static ResourceDictionary GetItemResources(DependencyObject obj)
        {
            return (ResourceDictionary)obj.GetValue(ItemResourcesProperty);
        }

        public static void SetItemResources(DependencyObject obj, ResourceDictionary value)
        {
            obj.SetValue(ItemResourcesProperty, value);
        }

        public static DataTemplate GetItemContentTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(ItemContentTemplateProperty);
        }

        public static void SetItemContentTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(ItemContentTemplateProperty, value);
        }

        public static DataTemplateSelector GetItemContentTemplateSelector(DependencyObject obj)
        {
            return (DataTemplateSelector)obj.GetValue(ItemContentTemplateSelectorProperty);
        }

        public static void SetItemContentTemplateSelector(DependencyObject obj, DataTemplateSelector value)
        {
            obj.SetValue(ItemContentTemplateSelectorProperty, value);
        }

        public static Style GetItemStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(ItemStyleProperty);
        }

        public static void SetItemStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ItemStyleProperty, value);
        }

        public static IEnumerable GetItemsSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(DependencyObject obj, IEnumerable<RadPane> value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.CanFloatProperty, newValue);
                }
            }
        }

        private static void OnItemHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as DataTemplate;
            var newValue = e.NewValue as DataTemplate;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.HeaderTemplateProperty, newValue);
                }
            }
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as ControlTemplate;
            var newValue = e.NewValue as ControlTemplate;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.TemplateProperty, newValue);
                }
            }
        }

        private static void OnItemTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as DataTemplate;
            var newValue = e.NewValue as DataTemplate;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                    return;

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.TitleTemplateProperty, newValue);
                }
            }
        }

        private static void OnItemTitleDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as string;
            var newValue = e.NewValue as string;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                    return;

                if (string.IsNullOrEmpty(newValue))
                {
                    foreach (var pane in extension.Panes.Values)
                        pane.ClearValue(RadPane.TitleProperty);
                }

                foreach (var paneModel in extension.Panes)
                {
                    // TODO: Update the binding of the Title.
                    extension.Panes[paneModel].SetBinding(RadPane.TitleProperty, new Binding(newValue) { Source = paneModel });
                }
            }
        }

        private static void OnItemContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as DataTemplate;
            var newValue = e.NewValue as DataTemplate;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.ContentTemplateProperty, newValue);
                }
            }
        }

        private static void OnItemContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as DataTemplateSelector;
            var newValue = e.NewValue as DataTemplateSelector;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.ContentTemplateSelectorProperty, newValue);
                }
            }
        }

        private static void OnItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as DataTemplate;
            var newValue = e.NewValue as DataTemplate;
            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);

                if (extension == null)
                {
                    return;
                }

                foreach (var pane in extension.Panes.Values)
                {
                    pane.SetValue(RadPane.StyleProperty, newValue);
                }
            }
        }

        private static PaneGroupExtensions GetPaneGroupExtension(DependencyObject obj)
        {
            return (PaneGroupExtensions)obj.GetValue(PaneGroupExtensionProperty);
        }

        private static void SetPaneGroupExtension(DependencyObject obj, PaneGroupExtensions value)
        {
            obj.SetValue(PaneGroupExtensionProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = d as RadPaneGroup;
            var oldValue = e.OldValue as IEnumerable;
            var newValue = e.NewValue as IEnumerable;
            var oldValueObservableCollection = e.OldValue as INotifyCollectionChanged;
            var newValueObservableCollection = e.NewValue as INotifyCollectionChanged;

            if (@group != null)
            {
                var extension = GetPaneGroupExtension(@group);
                if (extension == null)
                {
                    extension = new PaneGroupExtensions { Group = @group };
                    SetPaneGroupExtension(@group, extension);
                }

                if (oldValue != null)
                {
                    foreach (var paneModel in oldValue)
                    {
                        extension.RemoveItem(paneModel);
                    }

                    if (oldValueObservableCollection != null)
                    {
                        oldValueObservableCollection.CollectionChanged -= extension.OnItemsSourceCollectionChanged;
                    }
                }

                if (newValue != null)
                {
                    foreach (var paneModel in newValue)
                    {
                        extension.AddItem(paneModel);
                    }

                    if (newValueObservableCollection != null)
                    {
                        newValueObservableCollection.CollectionChanged += extension.OnItemsSourceCollectionChanged;
                    }
                }
            }
        }

        private void RemoveItem(object paneModel)
        {
            if (this.Panes.ContainsKey(paneModel))
            {
                var pane = this.Panes[paneModel];

                // NOTE: This code depends on the custom logic! This implementation will leave the panes that are removed from the group by the user.

                ////if (this.Group.Items.Contains(pane))
                ////{
                ////    this.Group.Items.Remove(pane);
                ////}

                // NOTE: This code depends on the custom logic! This implementation will remove all the panes no matter
                pane.RemoveFromParent();

                this.Panes.Remove(paneModel);
            }
        }

        private void AddItem(object paneModel)
        {
            this.InsertItem(this.Panes.Count, paneModel);
        }

        private void InsertItem(int index, object paneModel)
        {
            if (this.Panes.ContainsKey(paneModel))
                return;

            var pane = new RadPane
            {
                DataContext = paneModel,
                Header = paneModel,
                Content = paneModel,
                CanFloat = GetItemCanFloat(this.Group),
                HeaderTemplate = GetItemHeaderTemplate(this.Group),
                TitleTemplate = GetItemTitleTemplate(this.Group),
                ContentTemplate = GetItemContentTemplate(this.Group),
                Resources = GetItemResources(this.Group),
                ContentTemplateSelector = GetItemContentTemplateSelector(this.Group),
                Template = GetTemplate(this.Group),
                Style = GetItemStyle(this.Group),
            };

            var titleDisplayMemberPath = GetItemTitleDisplayMemberPath(this.Group);
            if (!string.IsNullOrEmpty(titleDisplayMemberPath))
            {
                pane.SetBinding(RadPane.TitleProperty, new Binding(titleDisplayMemberPath) { Source = paneModel });
            }

            this.Panes.Add(paneModel, pane);

            if (index > this.Group.Items.Count)
            {
                index = this.Group.Items.Count;
            }

            this.Group.Items.Insert(index, pane);
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.ClearItems();
                foreach (var paneModel in GetItemsSource(this.Group))
                {
                    this.AddItem(paneModel);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var paneModel in e.OldItems)
                    {
                        this.RemoveItem(paneModel);
                    }
                }

                if (e.NewItems != null)
                {
                    int i = e.NewStartingIndex;
                    foreach (var paneModel in e.NewItems)
                    {
                        this.InsertItem(i++, paneModel);
                    }
                }
            }
        }

        private void ClearItems()
        {
            foreach (var pane in this.Panes.Values)
            {
                pane.RemoveFromParent();
            }

            this.Panes.Clear();
        } 
    }
}