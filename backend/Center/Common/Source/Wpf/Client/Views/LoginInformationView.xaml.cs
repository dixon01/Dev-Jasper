// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginInformationView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginInformationView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;

    /// <summary>
    /// The login information view.
    /// </summary>
    public partial class LoginInformationView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginInformationView"/> class.
        /// </summary>
        public LoginInformationView()
        {
            this.InitializeComponent();
        }

        private void OnOptionChange(object sender, MouseButtonEventArgs e)
        {
            this.TenantDropDown.IsOpen = false;
            this.UserDropDown.IsOpen = false;
            this.UserDropDownList.SelectedItem = null;
        }

        private void OnLogOutClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as LoginInformationViewModel;
            if (viewModel != null)
            {
                viewModel.LogoutCommand.Execute(null);
            }
        }

        private void OnChangePasswordClicked(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as LoginInformationViewModel;
            if (viewModel == null)
            {
                return;
            }

            var prompt = new ChangePasswordPrompt(
                Strings.ChangePassword_Title,
                viewModel.ApplicationState.CurrentUser.HashedPassword);
            InteractionManager<ChangePasswordPrompt>.Current.Raise(prompt, this.OnPasswordChanged);
        }

        private void OnPasswordChanged(ChangePasswordPrompt prompt)
        {
            if (prompt == null || !prompt.Confirmed)
            {
                return;
            }

            var viewModel = this.DataContext as LoginInformationViewModel;
            if (viewModel != null)
            {
                viewModel.ChangePasswordCommand.Execute(prompt.NewPassword);
                prompt.NewPassword = null;
            }
        }
    }
}
