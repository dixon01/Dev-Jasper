// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FocusExtension.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Windows;

    /// <summary>
    /// Extension class for WPF to bind the logical and keyboard focus of a UI element.
    /// </summary>
    public static class FocusExtension
    {
        /// <summary>
        /// The is focused property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool?), typeof(FocusExtension), new FrameworkPropertyMetadata(IsFocusedChanged));

        /// <summary>
        /// Gets a value indicating if an element has focus.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// <c>true</c> if the element has focus.
        /// </returns>
        public static bool? GetIsFocused(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool?)element.GetValue(IsFocusedProperty);
        }

        /// <summary>
        /// Sets the focus to an <paramref name="element"/>.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value to be set.
        /// </param>
        public static void SetIsFocused(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IsFocusedProperty, value);
        }

        private static void IsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;

            if (e.OldValue == null)
            {
                fe.GotFocus += FrameworkElementGotFocus;
                fe.LostFocus += FrameworkElementLostFocus;
            }

            if (!fe.IsVisible)
            {
                fe.IsVisibleChanged += FrameWorkElementIsVisibleChanged;
            }

            if (e.NewValue != null && (bool)e.NewValue)
            {
                fe.Focus();
            }
        }

        private static void FrameWorkElementIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var focusProperty = ((FrameworkElement)sender).GetValue(IsFocusedProperty);
            if (fe.IsVisible && focusProperty != null && (bool)focusProperty)
            {
                fe.IsVisibleChanged -= FrameWorkElementIsVisibleChanged;
                fe.Focus();
            }
        }

        private static void FrameworkElementGotFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, true);
        }

        private static void FrameworkElementLostFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, false);
        }
    }
}
