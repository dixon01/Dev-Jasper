// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridTextEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridTextEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Converters;

    /// <summary>
    /// The property grid text editor.
    /// </summary>
    public partial class PropertyGridTextEditor
    {
        /// <summary>
        /// The data converter property.
        /// </summary>
        public static readonly DependencyProperty DataConverterProperty = DependencyProperty.Register(
            "DataConverter",
            typeof(IValueConverter),
            typeof(PropertyGridTextEditor),
            new PropertyMetadata(default(IValueConverter)));

        /// <summary>
        /// The validate on data errors property.
        /// </summary>
        public static readonly DependencyProperty ValidateOnDataErrorsProperty =
            DependencyProperty.Register(
                "ValidateOnDataErrors",
                typeof(bool),
                typeof(PropertyGridTextEditor),
                new PropertyMetadata(true));

        private bool hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridTextEditor"/> class.
        /// </summary>
        public PropertyGridTextEditor()
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
                return (IValueConverter)this.GetValue(DataConverterProperty);
            }

            set
            {
                this.SetValue(DataConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate on data errors.
        /// </summary>
        public bool ValidateOnDataErrors
        {
            get
            {
                return (bool)this.GetValue(ValidateOnDataErrorsProperty);
            }

            set
            {
                this.SetValue(ValidateOnDataErrorsProperty, value);
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

        private void PropertyGridTextBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.DataConverter != null)
            {
                var converter = this.DataConverter as IFormulaConverter;
                if (converter != null)
                {
                    converter.UnchangedValue = ((PropertyGridItemDataSource)this.DataContext).Value;
                }
            }

            this.PropertyGridTextBox.SelectAll();
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

        private void PropertyGridTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.hasChanged = true;
        }

        private void PropertyGridTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.hasChanged)
            {
                var be = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
        }
    }
}