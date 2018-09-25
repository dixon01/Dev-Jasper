// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridActionEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridActionEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// The property grid text editor.
    /// </summary>
    public partial class PropertyGridActionEditor
    {
        /// <summary>
        /// The data converter property.
        /// </summary>
        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register(
            "DataConverter",
            typeof(IValueConverter),
            typeof(PropertyGridActionEditor),
            new PropertyMetadata(default(IValueConverter)));

        /// <summary>
        /// The validate on data errors property.
        /// </summary>
        public static readonly DependencyProperty ValidateOnDataErrorsProperty =
            DependencyProperty.Register(
                "ValidateOnDataErrors",
                typeof(bool),
                typeof(PropertyGridActionEditor),
                new PropertyMetadata(true));

        /// <summary>
        /// The action callback
        /// </summary>
        public static readonly DependencyProperty ActionCallbackProperty = DependencyProperty.Register(
            "ActionCallback",
            typeof(Action<PropertyGridItem, PropertyGridItemDataSource>),
            typeof(PropertyGridActionEditor),
            new PropertyMetadata(default(Action<PropertyGridItem, PropertyGridItemDataSource>)));

        /// <summary>
        /// The button style property
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            "ButtonStyle",
            typeof(Style),
            typeof(PropertyGridActionEditor),
            new PropertyMetadata(default(Style)));

        /// <summary>
        /// The update callback
        /// </summary>
        public static readonly DependencyProperty UpdateCallbackProperty = DependencyProperty.Register(
            "UpdateCallback",
            typeof(Action<PropertyGridItem>),
            typeof(PropertyGridActionEditor),
            new PropertyMetadata(default(Action<PropertyGridItem>)));

        /// <summary>
        /// the property grid item property
        /// </summary>
        public static readonly DependencyProperty PropertyGridItemProperty =
            DependencyProperty.Register(
                "PropertyGridItem",
                typeof(PropertyGridItem),
                typeof(PropertyGridActionEditor),
                new PropertyMetadata(default(PropertyGridItem)));

        private bool hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridActionEditor"/> class.
        /// </summary>
        public PropertyGridActionEditor()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the data converter.
        /// </summary>
        public IValueConverter DataConverter
        {
            get
            {
                return (IValueConverter)GetValue(DataConverterProperty);
            }

            set
            {
                SetValue(DataConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate on data errors.
        /// </summary>
        public bool ValidateOnDataErrors
        {
            get
            {
                return (bool)GetValue(ValidateOnDataErrorsProperty);
            }

            set
            {
                SetValue(ValidateOnDataErrorsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the action callback
        /// </summary>
        public Action<PropertyGridItem, PropertyGridItemDataSource> ActionCallback
        {
            get
            {
                return (Action<PropertyGridItem, PropertyGridItemDataSource>)GetValue(ActionCallbackProperty);
            }

            set
            {
                SetValue(ActionCallbackProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the button style
        /// </summary>
        public Style ButtonStyle
        {
            get
            {
                return (Style)GetValue(ButtonStyleProperty);
            }

            set
            {
                SetValue(ButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the update callback
        /// </summary>
        public Action<PropertyGridItem> UpdateCallback
        {
            get
            {
                return (Action<PropertyGridItem>)GetValue(UpdateCallbackProperty);
            }

            set
            {
                SetValue(UpdateCallbackProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property grid item
        /// </summary>
        public PropertyGridItem PropertyGridItem
        {
            get
            {
                return (PropertyGridItem)GetValue(PropertyGridItemProperty);
            }

            set
            {
                SetValue(PropertyGridItemProperty, value);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataConverter != null)
            {
                var binding = new Binding("Value")
                                  {
                                      Mode = BindingMode.TwoWay,
                                      Converter = this.DataConverter,
                                      ValidatesOnDataErrors = this.ValidateOnDataErrors,
                                      NotifyOnValidationError = this.ValidateOnDataErrors
                                  };
                this.PropertyGridTextBox.SetBinding(TextBox.TextProperty, binding);
            }

            this.hasChanged = false;
        }

        private void PropertyGridTextBox_OnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.hasChanged)
            {
                var be = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
        }

        private void OnEditListClick(object sender, RoutedEventArgs e)
        {
            if (this.ActionCallback != null)
            {
                var dataSource = (PropertyGridItemDataSource)DataContext;
                this.ActionCallback(PropertyGridItem, dataSource);
            }
        }
    }
}