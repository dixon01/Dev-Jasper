// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectComboBox.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   MultiSelect ComboBox
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls.MultiselectCombobox
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Telerik.Windows.Controls;

    using TextSearch = Telerik.Windows.Controls.TextSearch;

    /// <summary>
    /// MultiSelect ComboBox
    /// </summary>
    public class MultiSelectComboBox : RadComboBox, INotifyPropertyChanged
    {
        /// <summary>
        /// DisplayBindingPath Property
        /// </summary>
        public static readonly DependencyProperty DisplayBindingPathProperty =
            DependencyProperty.Register(
                "DisplayBindingPath",
                typeof(string),
                typeof(MultiSelectComboBox),
                new PropertyMetadata(null, (obj, e) => TextSearch.SetTextPath(obj, e.NewValue as string)));

        /// <summary>
        /// SelectedItems Property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(ObservableCollection<object>),
                typeof(MultiSelectComboBox),
                new PropertyMetadata(new ObservableCollection<object>(), SelectedItemsChanged));

        private ObservableCollection<object> selectedValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectComboBox"/> class.
        /// Initializes a new instance of MultiSelectComboBox
        /// </summary>
        public MultiSelectComboBox()
        {
            this.ClearSelectionButtonVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the display member path (we can't reuse DisplayMemberPath property)
        /// </summary>
        public string DisplayBindingPath
        {
            get { return this.GetValue(DisplayBindingPathProperty) as string; }
            set { this.SetValue(DisplayBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected items
        /// </summary>
        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Gets the selected values
        /// </summary>
        public ObservableCollection<object> SelectedValues
        {
            get
            {
                if (this.selectedValues == null)
                {
                    this.selectedValues = new ObservableCollection<object>();
                    this.selectedValues.CollectionChanged += this.SelectedValuesCollectionChanged;
                }

                return this.selectedValues;
            }
        }

        /// <summary>
        /// Called when the Items property changed
        /// </summary>
        /// <param name="e">change information</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            var selectedItemsList = this.SelectedItems;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    var items = e.NewItems;
                    foreach (var value in this.SelectedValues)
                    {
                        foreach (var item in items)
                        {
                            if (this.GetSelectedValue(item).Equals(value) && !selectedItemsList.Contains(item))
                            {
                                selectedItemsList.Add(item);
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var idx = selectedItemsList.IndexOf(item);
                        if (idx >= 0)
                        {
                            selectedItemsList.RemoveAt(idx);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Create a new ComboBox item
        /// </summary>
        /// <returns>a new ComboBox item</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectComboBoxItem(this);
        }

        /// <summary>
        /// The on key down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.IsDropDownOpen = true;
        }

        private static void SelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var me = (MultiSelectComboBox)sender;
            if (e.OldValue != null)
            {
                ((ObservableCollection<object>)e.OldValue).CollectionChanged -= me.SelectedItemsCollectionChanged;
            }

            if (e.NewValue != null)
            {
                ((ObservableCollection<object>)e.NewValue).CollectionChanged += me.SelectedItemsCollectionChanged;
            }
        }

        private void RemoveCollectionChangedEvents()
        {
            this.SelectedItems.CollectionChanged -= this.SelectedItemsCollectionChanged;
            this.SelectedValues.CollectionChanged -= this.SelectedValuesCollectionChanged;
        }

        private void AddCollectionChangedEvents()
        {
            this.SelectedItems.CollectionChanged += this.SelectedItemsCollectionChanged;
            this.SelectedValues.CollectionChanged += this.SelectedValuesCollectionChanged;
        }

        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.SelectedValuePath != null)
            {
                this.RemoveCollectionChangedEvents();
                try
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            this.AddSelectedValues(e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            this.RemoveSelectedValues(e.OldItems);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            this.RemoveSelectedValues(e.OldItems);
                            this.AddSelectedValues(e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            this.SelectedValues.Clear();
                            foreach (var item in this.Items)
                            {
                                this.UpdateSelectedItem(item, false);
                            }

                            this.AddSelectedValues(e.NewItems);
                            break;
                    }
                }
                finally
                {
                    this.AddCollectionChangedEvents();
                }
            }

            this.RaiseSelectedItemsPropertyChanged();
        }

        private void RemoveSelectedValues(IList items)
        {
            foreach (var item in items)
            {
                this.SelectedValues.Remove(this.GetSelectedValue(item));
                this.UpdateSelectedItem(item, false);
            }
        }

        private void AddSelectedValues(IList items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    var selectedValue = this.GetSelectedValue(item);
                    if (!this.SelectedValues.Contains(selectedValue))
                    {
                        this.SelectedValues.Add(selectedValue);
                    }

                    this.UpdateSelectedItem(item, true);
                }
            }
        }

        private object GetSelectedValue(object item)
        {
            return DataControlHelper.GetPropertyInfo(item.GetType(), this.SelectedValuePath).GetValue(item, null);
        }

        private void SelectedValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RemoveCollectionChangedEvents();
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.AddSelectedItems(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveSelectedItems(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.RemoveSelectedItems(e.OldItems);
                        this.AddSelectedItems(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        var selectedItemsList = this.SelectedItems.ToList();
                        this.SelectedItems.Clear();
                        foreach (var item in selectedItemsList)
                        {
                            this.UpdateSelectedItem(item, false);
                        }

                        this.AddSelectedItems(e.NewItems);
                        break;
                }
            }
            finally
            {
                this.AddCollectionChangedEvents();
            }

            this.RaiseSelectedItemsPropertyChanged();
        }

        private void RaiseSelectedItemsPropertyChanged()
        {
            if (this.PropertyChanged != null)
            {
                // To update the selection box
                this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedItems"));
            }
        }

        private void RemoveSelectedItems(IList values)
        {
            foreach (var value in values)
            {
                var item = this.SelectedItems.FirstOrDefault(e => this.GetSelectedValue(e).Equals(value));
                if (item != null)
                {
                    this.SelectedItems.Remove(item);
                    this.UpdateSelectedItem(item, false);
                }
            }
        }

        private void AddSelectedItems(IList values)
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    var item = this.Items.Cast<object>().FirstOrDefault(e => this.GetSelectedValue(e).Equals(value));
                    if (item != null)
                    {
                        this.SelectedItems.Add(item);
                        this.UpdateSelectedItem(item, true);
                    }
                }
            }
        }

        private void UpdateSelectedItem(object item, bool select)
        {
            var obj = this.ItemContainerGenerator.ContainerFromItem(item);
            if (obj != null)
            {
                var cb = obj.FindChildByType<CheckBox>();
                if (cb != null && cb.IsChecked != select)
                {
                    cb.IsChecked = select;
                }
            }
        }
    }
}