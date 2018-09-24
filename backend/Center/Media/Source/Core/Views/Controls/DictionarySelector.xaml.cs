// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionarySelector.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for DictionarySelector.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Interaction;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for DictionarySelector.xaml
    /// </summary>
    public partial class DictionarySelector
    {
        /// <summary>
        /// the selected table property
        /// </summary>
        public static readonly DependencyProperty SelectedTableProperty = DependencyProperty.Register(
            "SelectedTable",
            typeof(string),
            typeof(DictionarySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the selected column property
        /// </summary>
        public static readonly DependencyProperty SelectedColumnProperty = DependencyProperty.Register(
            "SelectedColumn",
            typeof(string),
            typeof(DictionarySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the selected row property
        /// </summary>
        public static readonly DependencyProperty SelectedRowProperty = DependencyProperty.Register(
            "SelectedRow",
            typeof(string),
            typeof(DictionarySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the search text
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText",
            typeof(string),
            typeof(DictionarySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySelector"/> class.
        /// </summary>
        public DictionarySelector()
        {
            this.InitializeComponent();

            this.Loaded += this.SelectorLoaded;
        }

      /// <summary>
        /// Gets or sets the selected table
        /// </summary>
        public string SelectedTable
        {
            get
            {
                return (string)this.GetValue(SelectedTableProperty);
            }

            set
            {
                this.SetValue(SelectedTableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected column
        /// </summary>
        public string SelectedColumn
        {
            get
            {
                return (string)this.GetValue(SelectedColumnProperty);
            }

            set
            {
                this.SetValue(SelectedColumnProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected row
        /// </summary>
        public string SelectedRow
        {
            get
            {
                return (string)this.GetValue(SelectedRowProperty);
            }

            set
            {
                this.SetValue(SelectedRowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get
            {
                return (string)this.GetValue(SearchTextProperty);
            }

            set
            {
                this.SetValue(SearchTextProperty, value);
            }
        }

        private void SelectorLoaded(object sender, RoutedEventArgs e)
        {
            var dictionarySelectorPrompt = (DictionarySelectorPrompt)this.DataContext;

            var dictionaryValue = dictionarySelectorPrompt.SelectedDictionaryValue;
            this.RowTextBox.Value = dictionaryValue.Row;
            this.LanguageComboBox.SelectedItem = dictionaryValue.Language;

            if (dictionaryValue.Table != null && dictionaryValue.Column != null)
            {
                dictionarySelectorPrompt.SelectedDictionaryValue =
                    (DictionaryValueDataViewModel)dictionaryValue.Clone();
                dictionarySelectorPrompt.SelectedDictionaryValue = dictionaryValue;
            }

            var parent =
                (TreeViewItem)this.DictionaryView.ItemContainerGenerator.ContainerFromItem(dictionaryValue.Table);
            if (parent == null)
            {
                return;
            }

            parent.IsExpanded = true;

            if (dictionaryValue.Column == null || dictionaryValue.Table == null)
            {
                return;
            }

            var column =
                dictionaryValue.Table.Columns.SingleOrDefault(
                    c => c.Index == dictionaryValue.Column.Index);
            if (column == null)
            {
                return;
            }

            var item = (TreeViewItem)parent.ItemContainerGenerator.ContainerFromItem(column);
            if (item == null)
            {
                return;
            }

            item.IsSelected = true;
        }

        private void ExpandAll(ItemCollection items, ItemContainerGenerator itemContainerGenerator)
        {
            foreach (var item in items)
            {
                var container = (TreeViewItem)itemContainerGenerator.ContainerFromItem(item);
                if (container == null)
                {
                    continue;
                }

                container.IsExpanded = true;

                if (container.Items != null)
                {
                    this.ExpandAll(container.Items, container.ItemContainerGenerator);
                }
            }
        }

        private void CollapseAll(ItemCollection items, ItemContainerGenerator itemContainerGenerator)
        {
            foreach (var item in items)
            {
                var container = (TreeViewItem)itemContainerGenerator.ContainerFromItem(item);
                container.IsExpanded = false;

                if (container.Items != null)
                {
                    this.CollapseAll(container.Items, container.ItemContainerGenerator);
                }
            }
        }

        private void OnSelectedColumnChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedColumn = this.DictionaryView.SelectedItem as ColumnDataViewModel;
            if (selectedColumn == null)
            {
                return;
            }

            var dictionarySelectorPrompt = (DictionarySelectorPrompt)this.DataContext;
            var mediaShell = dictionarySelectorPrompt.Shell;

            var selectedTable = mediaShell.Dictionary.Tables.First(t => t.Columns.Contains(selectedColumn));

            var dictionaryValue =
                (DictionaryValueDataViewModel)
                dictionarySelectorPrompt.SelectedDictionaryValue.Clone();

            dictionaryValue.Table = selectedTable;
            dictionaryValue.Column = selectedColumn;

            dictionarySelectorPrompt.SelectedDictionaryValue = dictionaryValue;
        }

        private void OnRowChanged(object sender, RadRangeBaseValueChangedEventArgs radRangeBaseValueChangedEventArgs)
        {
            var dictionarySelectorPrompt = (DictionarySelectorPrompt)this.DataContext;
            if (dictionarySelectorPrompt == null)
            {
                return;
            }

            var currentDictionaryValue = dictionarySelectorPrompt.SelectedDictionaryValue;
            var newRowValue = radRangeBaseValueChangedEventArgs.NewValue;
            if (newRowValue == null || ((int)newRowValue).Equals(currentDictionaryValue.Row))
            {
                return;
            }

            dictionarySelectorPrompt.SelectedDictionaryValue.Row = (int)newRowValue;
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dictionarySelectorPrompt = (DictionarySelectorPrompt)this.DataContext;
            var newLanguageValue = this.LanguageComboBox.SelectedItem as LanguageDataViewModel;

            dictionarySelectorPrompt.SelectedDictionaryValue.Language = newLanguageValue;
        }

        private void OnRecentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.RecentValuesList.SelectedItem == null)
            {
                return;
            }

            var item = (DictionaryValueDataViewModel)this.RecentValuesList.SelectedItem;

            var dictionarySelectorPrompt = (DictionarySelectorPrompt)this.DataContext;

            var dictionaryValue =
                (DictionaryValueDataViewModel)dictionarySelectorPrompt.SelectedDictionaryValue.Clone();

            dictionaryValue.Table = item.Table;
            dictionaryValue.Column = item.Column;
            dictionarySelectorPrompt.SelectedDictionaryValue = dictionaryValue;
            var table =
             dictionarySelectorPrompt.DictionaryTables.SingleOrDefault(
                 t => t.Index == dictionarySelectorPrompt.SelectedDictionaryValue.Table.Index);
            var parent = (TreeViewItem)this.DictionaryView.ItemContainerGenerator.ContainerFromItem(table);
            parent.IsExpanded = true;
            if (table == null)
            {
                return;
            }

            var column =
                table.Columns.SingleOrDefault(
                    c =>
                    c.Index == dictionarySelectorPrompt.SelectedDictionaryValue.Column.Index);
            var treeViewItem = (TreeViewItem)parent.ItemContainerGenerator.ContainerFromItem(
                column);
            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
            }

            this.RecentValuesList.SelectedItem = null;
        }

        private void OnDictionarySearched(object sender, SearchBox.PropertyGridSearchEventArgs e)
        {
            var editDynamicTextPrompt = (DictionarySelectorPrompt)this.DataContext;

            this.SearchText = e.Text;

            editDynamicTextPrompt.UpdateFilter(this.SearchText);

            this.ExpandAll(this.DictionaryView.Items, this.DictionaryView.ItemContainerGenerator);
        }

        private void OnClearDictionarySearch(object sender, EventArgs e)
        {
            this.CollapseAll(this.DictionaryView.Items, this.DictionaryView.ItemContainerGenerator);
            this.SearchText = string.Empty;

            var editDynamicTextPrompt = (DictionarySelectorPrompt)this.DataContext;
            if (editDynamicTextPrompt != null)
            {
                editDynamicTextPrompt.UpdateFilter(this.SearchText);
            }
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
