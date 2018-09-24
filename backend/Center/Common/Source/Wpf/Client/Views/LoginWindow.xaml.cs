// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginWindowBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.ViewModels.Windows;

    /// <summary>
    /// Interaction logic for LoginWindowBase.xaml
    /// </summary>
    public partial class LoginWindow : ILoginWindowView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();

            this.Loaded += this.OnLoaded;
            this.Closing += this.OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.PasswordBox.Focus();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            this.PasswordBox.Clear();
        }

        private void Server_OnKeyUp(object sender, KeyEventArgs e)
        {
            var context = (LoginWindowBase)this.DataContext;
            context.InputServer = this.Server.Text;
        }

        private void Server_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var context = (LoginWindowBase)this.DataContext;
            context.InputServer = this.Server.Text;
        }

        private void TextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (control != null)
            {
                control.SelectAll();
                return;
            }

            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.SelectAll();
            }
        }
    }
}
