// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectComboBoxService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Service for the MultiSelect comboBox
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls.MultiselectCombobox
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Service for the MultiSelect comboBox
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter",
        Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
        Justification = "Reviewed. Suppression is OK here.")]
    public static class MultiSelectComboBoxService
    {
        /// <summary>
        /// IsChecked property
        /// </summary>
        public static DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached(
            "IsChecked",
            typeof(bool),
            typeof(MultiSelectComboBoxService),
            new PropertyMetadata(
                false,
                (obj, e) =>
                {
                    var comboBoxItem = obj.GetVisualParent<MultiSelectComboBoxItem>();
                    if (comboBoxItem != null)
                    {
                        var comboBox = comboBoxItem.ParentComboBox;
                        var selectedItems = (IList)comboBox.SelectedItems;
                        var item = comboBoxItem.DataContext;
                        if ((bool)e.NewValue)
                        {
                            if (!selectedItems.Contains(item))
                            {
                                selectedItems.Add(item);
                            }
                        }
                        else
                        {
                            selectedItems.Remove(item);
                        }
                    }
                }));

        /// <summary>
        /// SelectionBoxLoaded property called on SelectionBox load
        /// </summary>
        public static DependencyProperty SelectionBoxLoadedProperty =
            DependencyProperty.RegisterAttached(
                "SelectionBoxLoaded",
                typeof(bool),
                typeof(MultiSelectComboBoxService),
                new PropertyMetadata(
                    false,
                    (obj, e) =>
                    {
                        var targetElement = obj as TextBlock;
                        if (targetElement != null)
                        {
                            targetElement.Loaded += TargetElementLoaded;
                        }
                    }));

        /// <summary>
        /// ComboBoxItemLoaded called on ComboBoxItem load
        /// </summary>
        public static DependencyProperty ComboBoxItemLoadedProperty =
            DependencyProperty.RegisterAttached(
                "ComboBoxItemLoaded",
                typeof(bool),
                typeof(MultiSelectComboBoxService),
                new PropertyMetadata(
                    false,
                    (obj, e) =>
                    {
                        var targetElement = obj as CheckBox;
                        if (targetElement != null)
                        {
                            targetElement.Loaded += ComboBoxItemLoaded;
                            targetElement.SetBinding(MultiSelectComboBoxService.DataContextProperty, new Binding());
                        }
                    }));

        private static DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached(
            "DataContext",
            typeof(object),
            typeof(MultiSelectComboBoxService),
            new PropertyMetadata(
                null,
                (obj, e) =>
                {
                    var checkBox = (CheckBox)obj;
                    var comboBox = GetComboBox(checkBox);
                    if (comboBox != null && comboBox.SelectedItems != null)
                    {
                        checkBox.IsChecked = comboBox.SelectedItems.Contains(checkBox.DataContext);
                    }
                }));

        /// <summary>
        /// Gets a value indicating if the object is checked or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns>a value indicating if the object is checked or not</returns>
        public static bool GetIsChecked(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCheckedProperty);
        }

        /// <summary>
        /// Sets a value indicating if the object is checked or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">the value indicating if the object is checked or not</param>
        public static void SetIsChecked(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Gets the value indicating if the object is loaded or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns>the value indicating if the object is loaded or not</returns>
        public static bool GetSelectionBoxLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectionBoxLoadedProperty);
        }

        /// <summary>
        /// Sets the value indicating if the object is loaded or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">the value indicating if the object is loaded or not</param>
        public static void SetSelectionBoxLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectionBoxLoadedProperty, value);
        }

        /// <summary>
        /// Gets the value indicating if the item is loaded or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns>the value indicating if the item is loaded or not</returns>
        public static bool GetComboBoxItemLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(ComboBoxItemLoadedProperty);
        }

        /// <summary>
        /// Sets the value indicating if the item is loaded or not
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">the value indicating if the item is loaded or not</param>
        public static void SetComboBoxItemLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(ComboBoxItemLoadedProperty, value);
        }

        private static void TargetElementLoaded(object sender, RoutedEventArgs e)
        {
            var targetElement = (TextBlock)sender;
            targetElement.Loaded -= TargetElementLoaded;
            var comboBox = targetElement.GetVisualParent<MultiSelectComboBox>();
            if (comboBox != null)
            {
                targetElement.SetBinding(
                    TextBlock.TextProperty,
                    new Binding("SelectedItems")
                    {
                        Converter = new MultiSelectComboBoxConverter(),
                        Source = comboBox,
                        ConverterParameter = comboBox.DisplayBindingPath
                    });
            }
        }

        private static void ComboBoxItemLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var comboBox = GetComboBox(element);
            if (comboBox != null)
            {
                element.SetBinding(ContentControl.ContentProperty, new Binding(comboBox.DisplayBindingPath));
            }
        }

        private static MultiSelectComboBox GetComboBox(DependencyObject targetElement)
        {
            var item = targetElement.GetVisualParent<MultiSelectComboBoxItem>();
            if (item != null)
            {
                return item.ParentComboBox;
            }

            return null;
        }

        private static object GetDataContext(DependencyObject obj)
        {
            return obj.GetValue(DataContextProperty);
        }

        private static void SetDataContext(DependencyObject obj, object value)
        {
            obj.SetValue(DataContextProperty, value);
        }
    }
}
