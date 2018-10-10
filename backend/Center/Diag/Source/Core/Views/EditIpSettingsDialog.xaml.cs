// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditIpSettingsDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EditIpSettingsDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Diag.Core.Interaction;

    /// <summary>
    /// Interaction logic for EditIpSettingsDialog.xaml
    /// </summary>
    public partial class EditIpSettingsDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditIpSettingsDialog" /> class.
        /// </summary>
        public EditIpSettingsDialog()
        {
            InitializeComponent();

            this.Loaded += (s, e) => this.IpAddressBox.Focus();
        }

        private EditIpSettingsPromptNotification PromptNotification
        {
            get
            {
                return (EditIpSettingsPromptNotification)this.DataContext;
            }
        }

        /// <summary>
        /// Raises the dialog key up event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnDialogKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!this.PromptNotification.HasErrors)
                {
                    this.PromptNotification.Confirmed = true;
                    base.OnDialogKeyUp(sender, e);
                }
                else
                {
                    this.FocusError();
                }
            }
            else
            {
                base.OnDialogKeyUp(sender, e);
            }
        }

        private void FocusError()
        {
            if (Validation.GetHasError(this.IpAddressBox))
            {
                this.IpAddressBox.Focus();
            }
            else if (Validation.GetHasError(this.NetworkMaskBox))
            {
                this.NetworkMaskBox.Focus();
            }
            else if (Validation.GetHasError(this.GatewayBox))
            {
                this.GatewayBox.Focus();
            }
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (!this.PromptNotification.HasErrors)
            {
                this.PromptNotification.Confirmed = true;
                this.Close();
            }
            else
            {
                this.FocusError();
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
