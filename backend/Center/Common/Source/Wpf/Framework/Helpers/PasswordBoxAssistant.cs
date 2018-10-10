// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordBoxAssistant.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   PasswordBoxAssistant.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Used to allow data binding to a PasswordBox.
    /// </summary>
    /// <remarks>
    /// Original implementation: http://blog.functionalfun.net/2008/06/wpf-passwordbox-and-data-binding.html
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed.")]
    public static class PasswordBoxAssistant
    {
        /// <summary>
        /// BoundPassword dependency property.
        /// </summary>
        public static readonly DependencyProperty BoundPassword = DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxAssistant),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// BindPassword dependency property.
        /// </summary>
        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword",
            typeof(bool),
            typeof(PasswordBoxAssistant),
            new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false));

        /// <summary>
        /// Sets the bind password.
        /// </summary>
        /// <param name="dp">The dependencyObject.</param>
        /// <param name="value">The value to set.</param>
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        /// <summary>
        /// Gets a value indicating whether the password binding should be done or not.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The value indicating whether the password binding should be done or not.</returns>
        public static bool GetBindPassword(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(BindPassword);
        }

        /// <summary>
        /// Gets the bound password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The bound password.</returns>
        public static string GetBoundPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BoundPassword);
        }

        /// <summary>
        /// Sets the bound password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        public static void SetBoundPassword(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(BoundPassword, value);
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;

            if (box == null)
            {
                return;
            }

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);

            // push the new password into the BoundPassword property
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (box == null || !GetBindPassword(box))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            var newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event
            var box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            var wasBound = (bool)e.OldValue;
            var needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
}