// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddUnitDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AddUnitDialog.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Interaction logic for AddUnitDialog.xaml
    /// </summary>
    public partial class AddUnitDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddUnitDialog" /> class.
        /// </summary>
        public AddUnitDialog()
        {
            InitializeComponent();

            this.Loaded += (sender, args) => this.UnitAddressInput.Focus();
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
                ((PromptNotification)this.DataContext).Confirmed = true;
            }

            base.OnDialogKeyUp(sender, e);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            ((PromptNotification)this.DataContext).Confirmed = true;
            this.Close();
        }
    }
}
