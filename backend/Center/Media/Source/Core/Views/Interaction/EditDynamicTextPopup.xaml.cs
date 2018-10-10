// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditDynamicTextPopup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for EditDynamicTextPopup.xaml
    /// </summary>
    public partial class EditDynamicTextPopup
    {
        /// <summary>
        /// the selected table property
        /// </summary>
        public static readonly DependencyProperty SelectedTableProperty = DependencyProperty.Register(
            "SelectedTable",
            typeof(string),
            typeof(EditDynamicTextPopup),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the selected column property
        /// </summary>
        public static readonly DependencyProperty SelectedColumnProperty = DependencyProperty.Register(
            "SelectedColumn",
            typeof(string),
            typeof(EditDynamicTextPopup),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the selected row property
        /// </summary>
        public static readonly DependencyProperty SelectedRowProperty = DependencyProperty.Register(
            "SelectedRow",
            typeof(string),
            typeof(EditDynamicTextPopup),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the search text
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText",
            typeof(string),
            typeof(EditDynamicTextPopup),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The is led canvas property.
        /// </summary>
        public static readonly DependencyProperty IsLedCanvasProperty = DependencyProperty.Register(
            "IsLedCanvas",
            typeof(bool),
            typeof(EditDynamicTextPopup),
            new PropertyMetadata(default(bool)));

        private const int MaxRecentDictionaryItems = 5;

        private GenericEvalDataViewModel currentGenericDataViewModel;

        private bool isSelectedItemVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditDynamicTextPopup"/> class.
        /// </summary>
        public EditDynamicTextPopup()
        {
            this.InitializeComponent();

            this.Loaded += this.PopupLoaded;
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

        /// <summary>
        /// Gets or sets a value indicating whether is led canvas.
        /// </summary>
        public bool IsLedCanvas
        {
            get
            {
                return (bool)this.GetValue(IsLedCanvasProperty);
            }

            set
            {
                this.SetValue(IsLedCanvasProperty, value);
            }
        }

        /// <summary>
        /// the handler for close
        /// </summary>
        /// <param name="e">
        /// the event arguments
        /// </param>
        protected override void OnClose(EventArgs e)
        {
            base.OnClose(e);

            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            editDynamicTextPrompt.DynamicTextElement.PropertyChanged -= this.DynamicTextElementDataChanged;
            var isMultirow = editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Table.MultiRow;
            if (!isMultirow)
            {
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Row = 0;
            }

            this.AddRecentDictionaryValue(editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue);
            this.currentGenericDataViewModel = null;
        }

        private void PopupLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            editDynamicTextPrompt.DynamicTextElement.PropertyChanged += this.DynamicTextElementDataChanged;

            this.RowTextBox.Value = editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Row;
            this.LanguageComboBox.SelectedItem =
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Language;

            var dictionaryValue = editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue;
            if (editDynamicTextPrompt.DynamicTextElement.Value.Formula == null)
            {
                this.currentGenericDataViewModel = new GenericEvalDataViewModel(editDynamicTextPrompt.Shell);
                editDynamicTextPrompt.DynamicTextElement.Value.Formula = this.currentGenericDataViewModel;
            }
            else
            {
                var generic = editDynamicTextPrompt.DynamicTextElement.Value.Formula as GenericEvalDataViewModel;
                if (generic == null)
                {
                    generic = new GenericEvalDataViewModel(editDynamicTextPrompt.Shell);
                    generic.Table.Value = dictionaryValue.Table == null ? 0 : dictionaryValue.Table.Index;
                    generic.Column.Value = dictionaryValue.Column == null ? 0 : dictionaryValue.Column.Index;
                    generic.Language.Value = dictionaryValue.Language == null ? 0 : dictionaryValue.Language.Index;
                    generic.Row.Value = dictionaryValue.Row;
                    editDynamicTextPrompt.DynamicTextElement.Value.Formula = generic;
                }

                this.currentGenericDataViewModel = generic;
            }

            if (dictionaryValue.Table != null && dictionaryValue.Column != null)
            {
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue =
                    (DictionaryValueDataViewModel)dictionaryValue.Clone();
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue = dictionaryValue;
                this.currentGenericDataViewModel.Table.Value = dictionaryValue.Table.Index;
                this.currentGenericDataViewModel.Column.Value = dictionaryValue.Column.Index;
                this.currentGenericDataViewModel.Language.Value = dictionaryValue.Language.Index;
                this.currentGenericDataViewModel.Row.Value = dictionaryValue.Row;
            }

            editDynamicTextPrompt.UpdateFilter(this.SearchText);
            this.SetSelectionForSelectedDynamicTextElement();

            if (!this.isSelectedItemVisible)
            {
                this.SearchBox.OnClearSearch();
            }
        }

        private void DynamicTextElementDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedDictionaryValue")
            {
                return;
            }

            this.SetSelectionForSelectedDynamicTextElement();
        }

        private void SetSelectionForSelectedDynamicTextElement()
        {
            if (this.DataContext == null)
            {
                return;
            }

            this.UpdateLayout(); // required so ContainerFromItem() will work if searched

            this.isSelectedItemVisible = false;

            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            var selectedDictionaryValue = editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue;
            var table = editDynamicTextPrompt.DictionaryTables.SingleOrDefault(
                    t => t.Index == selectedDictionaryValue.Table.Index);

            if (table == null)
            {
                return;
            }

            var parent = (TreeViewItem)this.DictionaryView.ItemContainerGenerator.ContainerFromItem(table);
            if (parent == null)
            {
                return;
            }

            parent.IsExpanded = true;

            if (selectedDictionaryValue.Column == null)
            {
                return;
            }

            var column =
                table.Columns.SingleOrDefault(
                    c => c.Index == selectedDictionaryValue.Column.Index);

            var item = (TreeViewItem)parent.ItemContainerGenerator.ContainerFromItem(column);
            if (item != null)
            {
                item.IsSelected = true;
                this.isSelectedItemVisible = true;
            }
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
                if (container == null)
                {
                    break;
                }

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

            var editDynamicTextPrompt = (EditDynamicTextPrompt)this.DataContext;
            var mediaShell = editDynamicTextPrompt.Shell;

            var selectedTable = mediaShell.Dictionary.Tables.First(t => t.Columns.Contains(selectedColumn));

            var oldElement = (GraphicalElementDataViewModelBase)editDynamicTextPrompt.DynamicTextElement.Clone();

            var dictionaryValue =
                (DictionaryValueDataViewModel)
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Clone();

            dictionaryValue.Table = selectedTable;
            dictionaryValue.Column = selectedColumn;
            if (this.currentGenericDataViewModel != null)
            {
                this.currentGenericDataViewModel.Table.Value = selectedTable.Index;
                this.currentGenericDataViewModel.Column.Value = selectedColumn.Index;
            }

            editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue = dictionaryValue;

            this.UpdateLayoutElement(oldElement);
        }

        private void OnRowChanged(object sender, RadRangeBaseValueChangedEventArgs radRangeBaseValueChangedEventArgs)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            var currentDictionaryValue = editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue;
            var newRowValue = this.RowTextBox.Value;
            if (newRowValue == null || ((int)newRowValue).Equals(currentDictionaryValue.Row))
            {
                return;
            }

            var oldElement = (GraphicalElementDataViewModelBase)editDynamicTextPrompt.DynamicTextElement.Clone();

            var dictionaryValue =
                (DictionaryValueDataViewModel)
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Clone();

            dictionaryValue.Row = (int)newRowValue;
            if (this.currentGenericDataViewModel != null)
            {
                this.currentGenericDataViewModel.Row.Value = (int)newRowValue;
            }

            editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue = dictionaryValue;

            this.UpdateLayoutElement(oldElement);
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            var newLanguageValue = this.LanguageComboBox.SelectedItem as LanguageDataViewModel;

            var oldElement = (GraphicalElementDataViewModelBase)editDynamicTextPrompt.DynamicTextElement.Clone();

            var dictionaryValue =
                (DictionaryValueDataViewModel)
                    editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Clone();

            dictionaryValue.Language = newLanguageValue;
            if (this.currentGenericDataViewModel != null)
            {
                if (newLanguageValue != null)
                {
                    this.currentGenericDataViewModel.Language.Value = newLanguageValue.Index;
                }
            }

            editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue = dictionaryValue;

            this.UpdateLayoutElement(oldElement);
        }

        private void UpdateLayoutElement(GraphicalElementDataViewModelBase oldElement)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            var mediaShell = editDynamicTextPrompt.Shell;
            var newElement = (GraphicalElementDataViewModelBase)editDynamicTextPrompt.DynamicTextElement.Clone();
            var editor = mediaShell.Editor as EditorViewModelBase;

            if (editor != null)
            {
                var parameters = new UpdateEntityParameters(
                    new List<DataViewModelBase> { oldElement },
                    new List<DataViewModelBase> { newElement },
                    editor.Elements);

                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private void AddRecentDictionaryValue(DictionaryValueDataViewModel newElement)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            if (newElement.Table == null)
            {
                return;
            }

            Func<DictionaryValueDataViewModel, bool> predicate =
                v =>
                v.Table.Name == newElement.Table.Name
                && v.Table.Index == newElement.Table.Index
                && ((
                        v.Column != null
                        && newElement.Column != null
                        && v.Column.Name == newElement.Column.Name
                        && v.Column.Index == newElement.Column.Index)
                    || (v.Column == null
                        && newElement.Column == null));

            if (editDynamicTextPrompt.RecentDictionaryValues.Any(predicate))
            {
                editDynamicTextPrompt.RecentDictionaryValues.Remove(
                    editDynamicTextPrompt.RecentDictionaryValues.First(predicate));
            }

            editDynamicTextPrompt.RecentDictionaryValues.Insert(0, newElement);
            for (var i = editDynamicTextPrompt.RecentDictionaryValues.Count - 1; i >= MaxRecentDictionaryItems; i--)
            {
                editDynamicTextPrompt.RecentDictionaryValues.RemoveAt(i);
            }
        }

        private void OnRecentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.RecentValuesList.SelectedItem == null)
            {
                return;
            }

            var item = (DictionaryValueDataViewModel)this.RecentValuesList.SelectedItem;

            var editDynamicTextPrompt = (EditDynamicTextPrompt)this.DataContext;

            var oldElement = (GraphicalElementDataViewModelBase)editDynamicTextPrompt.DynamicTextElement.Clone();

            var dictionaryValue =
                (DictionaryValueDataViewModel)
                editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue.Clone();

            dictionaryValue.Table = item.Table;
            dictionaryValue.Column = item.Column;

            editDynamicTextPrompt.DynamicTextElement.SelectedDictionaryValue = dictionaryValue;

            this.UpdateLayoutElement(oldElement);

            this.RecentValuesList.SelectedItem = null;
        }

        private void OnDictionarySearched(object sender, SearchBox.PropertyGridSearchEventArgs e)
        {
            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            this.SearchText = e.Text;

            editDynamicTextPrompt.UpdateFilter(this.SearchText);

            this.ExpandAll(this.DictionaryView.Items, this.DictionaryView.ItemContainerGenerator);
            this.SetSelectionForSelectedDynamicTextElement();
        }

        private void OnClearDictionarySearch(object sender, EventArgs e)
        {
            this.SearchText = string.Empty;

            if (!(this.DataContext is EditDynamicTextPrompt editDynamicTextPrompt))
            {
                return;
            }

            editDynamicTextPrompt.UpdateFilter(this.SearchText);

            this.CollapseAll(this.DictionaryView.Items, this.DictionaryView.ItemContainerGenerator);

            this.SetSelectionForSelectedDynamicTextElement();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
