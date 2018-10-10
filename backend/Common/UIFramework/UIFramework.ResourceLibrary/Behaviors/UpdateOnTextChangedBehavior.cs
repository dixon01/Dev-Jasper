// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateOnTextChangedBehavior.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.UIFramework.ResourceLibrary.Behaviors
{
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interactivity;

    /// <summary>
    ///     The update on text changed behavior.
    /// </summary>
    public class UpdateOnTextChangedBehavior : Behavior<TextBox>
    {
        #region Methods

        /// <summary>
        ///     The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.TextChanged += this.AssociatedObject_TextChanged;
        }

        /// <summary>
        ///     The on detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.TextChanged -= this.AssociatedObject_TextChanged;
        }

        /// <summary>
        /// The associated object_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression binding = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }

        #endregion
    }
}