// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextChangedBehavior.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TextChangedBehavior.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// the text changed behavior
    /// </summary>
    public static class TextChangedBehavior
    {
        private static readonly DependencyProperty UserInputProperty = DependencyProperty.RegisterAttached("UserInput", typeof(bool), typeof(TextChangedBehavior));

        private static DependencyProperty textChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "TextChangedCommand",
                typeof(ICommand),
                typeof(TextChangedBehavior),
                new UIPropertyMetadata(TextChangedCommandChanged));

        /// <summary>
        /// Gets or sets the text changed command property
        /// </summary>
        public static DependencyProperty TextChangedCommandProperty
        {
            get
            {
                return textChangedCommandProperty;
            }

            set
            {
                textChangedCommandProperty = value;
            }
        }

        /// <summary>
        /// set text changed command
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetTextChangedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(textChangedCommandProperty, value);
        }

        // Subscribe to the events if we have a valid command
        private static void TextChangedCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var textBox = target as TextBox;
            if (textBox != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    textBox.PreviewKeyDown += TextBoxPreviewKeyDown;
                    textBox.PreviewTextInput += TextBoxPreviewTextInput;
                    DataObject.AddPastingHandler(textBox, TextBoxTextPasted);
                    textBox.TextChanged += TextBoxTextChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    textBox.PreviewKeyDown -= TextBoxPreviewKeyDown;
                    textBox.PreviewTextInput -= TextBoxPreviewTextInput;
                    DataObject.RemovePastingHandler(textBox, TextBoxTextPasted);
                    textBox.TextChanged -= TextBoxTextChanged;
                }
            }
        }

        // Catches User input
        private static void TextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            SetUserInput(textBox, true);
        }

        // Catches Backspace, Delete, Enter
        private static void TextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                if (e.Key == Key.Return)
                {
                    if (textBox.AcceptsReturn)
                    {
                        SetUserInput(textBox, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {
                    if (textBox.SelectionLength > 0 || textBox.SelectionStart < textBox.Text.Length)
                    {
                        SetUserInput(textBox, true);
                    }
                }
                else if (e.Key == Key.Back)
                {
                    if (textBox.SelectionLength > 0 || textBox.SelectionStart > 0)
                    {
                        SetUserInput(textBox, true);
                    }
                }
            }
        }
        
        // Catches pasting
        private static void TextBoxTextPasted(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text, true) == false)
            {
                return;
            }
            
            SetUserInput(textBox, true);
        }
        
        private static void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextChangedFired(textBox, e);
            SetUserInput(textBox, false);
        }

        private static void TextChangedFired(TextBox sender, TextChangedEventArgs e)
        {
            var command = (ICommand)sender.GetValue(textChangedCommandProperty);
            var arguments = new object[] { sender, e, GetUserInput(sender) };
            command.Execute(arguments);
        }

        #region UserInput

        private static void SetUserInput(DependencyObject target, bool value)
        {
            target.SetValue(UserInputProperty, value);
        }
        
        private static bool GetUserInput(DependencyObject target)
        {
            return (bool)target.GetValue(UserInputProperty);
        }

        #endregion // UserInput
    }
}
