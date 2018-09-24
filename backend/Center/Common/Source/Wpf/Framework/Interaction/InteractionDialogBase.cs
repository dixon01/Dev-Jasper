// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionDialogBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionDialogBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines a base dialog for interaction.
    /// </summary>
    public class InteractionDialogBase : UserControl
    {
        /// <summary>
        /// the message template
        /// </summary>
        public static readonly DependencyProperty MessageTemplateProperty =
            DependencyProperty.Register("MessageTemplate", typeof(DataTemplate), typeof(InteractionDialogBase), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionDialogBase"/> class.
        /// </summary>
        public InteractionDialogBase()
        {
            this.KeyUp += this.OnDialogKeyUp;
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// the closed event
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Gets or sets the message template
        /// </summary>
        public DataTemplate MessageTemplate
        {
            get { return (DataTemplate)this.GetValue(MessageTemplateProperty); }
            set { this.SetValue(MessageTemplateProperty, value); }
        }

        /// <summary>
        /// closes the popup
        /// </summary>
        public void Close()
        {
            this.OnClose(EventArgs.Empty);
        }

        /// <summary>
        /// the handler for close
        /// </summary>
        /// <param name="e">the parameter</param>
        protected virtual void OnClose(EventArgs e)
        {
            var handler = this.Closed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// handles on key up
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the parameter</param>
        protected virtual void OnDialogKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.Close();
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}