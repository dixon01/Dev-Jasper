// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePasswordDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ChangePasswordDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client.Interaction;

    /// <summary>
    /// Interaction logic for ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordDialog"/> class.
        /// </summary>
        public ChangePasswordDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// handles on key up
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the parameter</param>
        protected override void OnDialogKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var viewModel = this.DataContext as ChangePasswordPrompt;
                if (viewModel != null && !viewModel.HasErrors)
                {
                    this.Close(true);
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.Close(false);
            }
        }

        private void Close(bool commit)
        {
            var viewModel = this.DataContext as ChangePasswordPrompt;
            if (viewModel != null)
            {
                viewModel.Confirmed = commit && !viewModel.HasErrors;
                viewModel.CurrentPassword = null;
                viewModel.RepeatPassword = null;

                if (!commit)
                {
                    viewModel.NewPassword = null;
                }
            }

            this.Close();
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentPasswordBox.IsVisible)
            {
                this.CurrentPasswordBox.Focus();
            }
            else
            {
                this.NewPasswordBox.Focus();
            }
        }

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close(true);
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close(false);
        }
    }
}
