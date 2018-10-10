// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditableTextblock.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EditableTextblock.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Interaction logic for Editable Text block
    /// </summary>
    public partial class EditableTextblock : IDataErrorInfo
    {
        /// <summary>
        /// The is in edit mode property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextblock),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// the text property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableTextblock),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the source object property
        /// </summary>
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register(
            "SourceObject",
            typeof(object),
            typeof(EditableTextblock),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// the validate property
        /// </summary>
        public static readonly DependencyProperty ValidateProperty = DependencyProperty.Register(
            "Validate",
            typeof(Func<string, object, string>),
            typeof(EditableTextblock),
            new PropertyMetadata(default(Func<string, object, string>)));

        /// <summary>
        /// The TAB key is recognized by the TextBox
        /// </summary>
        public static readonly DependencyProperty AcceptsTabProperty = DependencyProperty.Register(
            "AcceptsTab",
            typeof(bool),
            typeof(EditableTextblock),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Value indicating if the TAB key should be added to the text or not.
        /// </summary>
        public static readonly DependencyProperty IsTabHandlingEnabledProperty =
        DependencyProperty.Register(
            "IsTabHandlingEnabled",
            typeof(bool),
            typeof(EditableTextblock),
            new PropertyMetadata(true));

        /// <summary>
        /// The text trimming property.
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            "TextTrimming",
            typeof(TextTrimming),
            typeof(EditableTextblock),
            new PropertyMetadata(TextTrimming.CharacterEllipsis));

        private string oldText;

        private bool isMouseDownInside;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableTextblock"/> class.
        /// </summary>
        public EditableTextblock()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// The text changed event
        /// </summary>
        public event Action<string, string, object> TextChanged;

        /// <summary>
        /// Gets or sets a value indicating whether is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)this.GetValue(IsInEditModeProperty);
            }

            set
            {
                this.SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text trimming.
        /// </summary>
        public TextTrimming TextTrimming
        {
            get
            {
                return (TextTrimming)this.GetValue(TextTrimmingProperty);
            }

            set
            {
                this.SetValue(TextTrimmingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Text to be displayed
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source object for the text
        /// </summary>
        public object SourceObject
        {
            get
            {
                return this.GetValue(SourceObjectProperty);
            }

            set
            {
                this.SetValue(SourceObjectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the validation function
        /// </summary>
        public Func<string, object, string> Validate
        {
            get
            {
                return (Func<string, object, string>)this.GetValue(ValidateProperty);
            }

            set
            {
                this.SetValue(ValidateProperty, value);
            }
        }

        /// <summary>
        /// Gets the NameDoubleClickCommand
        /// </summary>
        public ICommand TextDoubleClickCommand
        {
            get
            {
                return new RelayCommand(this.OnTextDoubleClick);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether accepts tab.
        /// </summary>
        public bool AcceptsTab
        {
            get
            {
                return (bool)this.GetValue(AcceptsTabProperty);
            }

            set
            {
                this.SetValue(AcceptsTabProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TAB key will
        /// be added to the text.
        /// </summary>
        public bool IsTabHandlingEnabled
        {
            get
            {
                return (bool)this.GetValue(IsTabHandlingEnabledProperty);
            }

            set
            {
                this.SetValue(IsTabHandlingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The indexer used for validation.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "Text"
                    && this.SourceObject != null
                    && this.Text != null
                    && this.Validate != null)
                {
                    return this.Validate(this.Text, this.SourceObject);
                }

                return string.Empty;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
            {
                return;
            }

            Mouse.AddPreviewMouseDownHandler(window, this.OnWindowMouseDown);
            Mouse.AddPreviewMouseUpHandler(window, this.OnWindowMouseUp);
        }

        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsInEditMode)
            {
                var pos = e.GetPosition(this.EditTextBox);
                if (this.IsOutsideOfTextBox(pos))
                {
                    e.Handled = true;
                }
                else
                {
                    this.isMouseDownInside = true;
                }
            }
        }

        private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsInEditMode)
            {
                var pos = e.GetPosition(this.EditTextBox);

                if (this.IsOutsideOfTextBox(pos))
                {
                    if (!this.isMouseDownInside)
                    {
                        this.StopEditing();
                    }

                    e.Handled = true;
                }
            }

            this.isMouseDownInside = false;
        }

        private bool IsOutsideOfTextBox(Point pos)
        {
            return pos.X < 0
                   || pos.X >= this.EditTextBox.ActualWidth
                   || pos.Y < 0
                   || pos.Y >= this.EditTextBox.ActualHeight;
        }

        private void OnTextDoubleClick()
        {
            this.oldText = this.Text;
            this.StartEditing();
        }

        private void OnEditNameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape)
            {
                this.StopEditing(e.Key == Key.Escape);

               e.Handled = true;
               return;
            }

            if (e.Key == Key.Tab && !this.IsTabHandlingEnabled)
            {
                var indexOfTab = this.EditTextBox.Text.IndexOf("\t", StringComparison.Ordinal);
                this.EditTextBox.Text = this.EditTextBox.Text.Remove(indexOfTab, 1);
                this.StopEditing();

               e.Handled = true;
               return;
            }

            if (e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void OnEditNameLostFocus(object sender, RoutedEventArgs e)
        {
            this.StopEditing();
        }

        private void StartEditing()
        {
            this.IsInEditMode = true;
            this.EditTextBox.Focus();
        }

        private void StopEditing(bool cancel = false)
        {
            if (cancel)
            {
                this.Text = this.oldText;
                this.IsInEditMode = false;
                return;
            }

            if (this.IsInEditMode && this.DoValidate())
            {
                this.IsInEditMode = false;

                this.RaiseTextChanged(this.EditTextBox.Text);
            }
        }

        private void RaiseTextChanged(string text)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this.oldText, text, this.SourceObject);
            }
        }

        private bool DoValidate()
        {
            return this.Validate == null || string.IsNullOrEmpty(this.Validate(this.Text, this.SourceObject));
        }
    }
}
