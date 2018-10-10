// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridMultiSelectEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;

    /// <summary>
    /// Interaction logic for PropertyGridMultiSelectEditor.xaml
    /// </summary>
    public partial class PropertyGridMultiSelectEditor
    {
        /// <summary>
        /// The action callback
        /// </summary>
        public static readonly DependencyProperty ActionCallbackProperty = DependencyProperty.Register(
            "ActionCallback",
            typeof(Action<PropertyGridItem, PropertyGridItemDataSource>),
            typeof(PropertyGridMultiSelectEditor),
            new PropertyMetadata(default(Action<PropertyGridItem, PropertyGridItemDataSource>)));

        /// <summary>
        /// the property grid item property
        /// </summary>
        public static readonly DependencyProperty PropertyGridItemProperty =
            DependencyProperty.Register(
                "PropertyGridItem",
                typeof(PropertyGridItem),
                typeof(PropertyGridMultiSelectEditor),
                new PropertyMetadata(default(PropertyGridItem)));

        /// <summary>
        /// The items property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(object),
            typeof(PropertyGridMultiSelectEditor),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// The selected items text property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsTextProperty =
            DependencyProperty.Register(
                "SelectedItemsText",
                typeof(string),
                typeof(PropertyGridMultiSelectEditor),
                new PropertyMetadata(default(string)));

        private bool isUnchecking;

        private bool hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridMultiSelectEditor"/> class.
        /// </summary>
        public PropertyGridMultiSelectEditor()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public object Items
        {
            get
            {
                return this.GetValue(ItemsProperty);
            }

            set
            {
                this.SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the action callback
        /// </summary>
        public Action<PropertyGridItem, PropertyGridItemDataSource> ActionCallback
        {
            get
            {
                return (Action<PropertyGridItem, PropertyGridItemDataSource>)this.GetValue(ActionCallbackProperty);
            }

            set
            {
                this.SetValue(ActionCallbackProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property grid item
        /// </summary>
        public PropertyGridItem PropertyGridItem
        {
            get
            {
                return (PropertyGridItem)this.GetValue(PropertyGridItemProperty);
            }

            set
            {
                this.SetValue(PropertyGridItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected items text.
        /// </summary>
        public string SelectedItemsText
        {
            get
            {
                return (string)this.GetValue(SelectedItemsTextProperty);
            }

            set
            {
                this.SetValue(SelectedItemsTextProperty, value);
            }
        }

        private void OnNavigateButtonClicked(object sender, RoutedEventArgs e)
        {
            var context = (PropertyGridItemDataSource)this.DataContext;
            if (context.NavigateButtonCommand != null)
            {
                context.NavigateButtonCommand.Execute(null);
            }

            this.ValueComboBox.IsDropDownOpen = false;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
           this.RefreshItemsList();
           this.hasChanges = false;
        }

        private void RefreshItemsList()
        {
            var context = (PropertyGridItemDataSource)this.DataContext;
            var items = new List<DataItem> { new DataItem { Value = FrameworkStrings.MultiSelectionEditor_AllItems } };
            var sourceItems = context.DomainObject as IList;
            var selectedItems = context.Value.ToString().Split(';');
            items.AddRange(
                from object sourceItem in sourceItems
                select
                    new DataItem
                    {
                        Value = sourceItem,
                        IsChecked =
                            selectedItems.Any(
                                i =>
                                i.Equals(sourceItem.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    });
            this.Items = items;
            this.SelectedItemsText = context.Value.ToString();
        }

        private void ToggleButtonOnChecked(object sender, RoutedEventArgs e)
        {
            var item = e.Source as CheckBox;
            if (item != null)
            {
                var items = this.Items as List<DataItem>;
                if (items == null)
                {
                    return;
                }

                var dataItem = item.DataContext as DataItem;
                if (dataItem == null)
                {
                    return;
                }

                if (dataItem.ValueString.Equals(
                    FrameworkStrings.MultiSelectionEditor_AllItems,
                    StringComparison.InvariantCultureIgnoreCase) && items.Count > 1)
                {
                    for (int i = 1; i < items.Count; i++)
                    {
                        items[i].IsChecked = true;
                    }
                }

                this.hasChanges = true;
            }
        }

        private void ToggleButtonOnUnchecked(object sender, RoutedEventArgs e)
        {
            var item = e.Source as CheckBox;
            if (item != null)
            {
                var items = this.Items as List<DataItem>;
                if (items == null)
                {
                    this.isUnchecking = false;
                    return;
                }

                var dataItem = item.DataContext as DataItem;
                if (dataItem == null)
                {
                    this.isUnchecking = false;
                    return;
                }

                this.hasChanges = true;
                if (!this.isUnchecking && dataItem.ValueString.Equals(
                    FrameworkStrings.MultiSelectionEditor_AllItems,
                    StringComparison.InvariantCultureIgnoreCase) && items.Count > 1)
                {
                    for (int i = 1; i < items.Count; i++)
                    {
                        items[i].IsChecked = false;
                    }

                    return;
                }

                var allItemsDataItem =
                    items.FirstOrDefault(i => i.ValueString.Equals(FrameworkStrings.MultiSelectionEditor_AllItems));
                if (allItemsDataItem != null && allItemsDataItem.IsChecked)
                {
                    this.isUnchecking = true;
                    allItemsDataItem.IsChecked = false;
                }
            }

            this.isUnchecking = false;
        }

        private void ValueComboBoxOnDropDownClosed(object sender, EventArgs e)
        {
            if (!this.hasChanges)
            {
                return;
            }

            var text = this.CreateItemsString();
            var context = (PropertyGridItemDataSource)this.DataContext;
            context.Value = text;
            this.SelectedItemsText = text;
        }

        private string CreateItemsString()
        {
            var sb = new StringBuilder();
            var items = this.Items as List<DataItem>;
            if (items == null)
            {
                return string.Empty;
            }

            foreach (var item in items)
            {
                if (item.ValueString.Equals(
                    FrameworkStrings.MultiSelectionEditor_AllItems,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (item.IsChecked)
                {
                    sb.Append(item.ValueString + ";");
                }
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        private class DataItem : ViewModelBase
        {
            private bool isChecked;
            private object value;

            /// <summary>
            /// Gets or sets a value indicating whether is checked.
            /// </summary>
            public bool IsChecked
            {
                get
                {
                    return this.isChecked;
                }

                set
                {
                    this.SetProperty(ref this.isChecked, value, () => this.IsChecked);
                }
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public object Value
            {
                get
                {
                    return this.value;
                }

                set
                {
                    this.SetProperty(ref this.value, value, () => this.Value);
                    this.RaisePropertyChanged(() => this.ValueString);
                }
            }

            /// <summary>
            /// Gets the value string.
            /// </summary>
            public string ValueString
            {
                get
                {
                    return this.Value.ToString();
                }
            }
        }
    }
}
