// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginInformationViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginInformationViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The view model representing the login information shown in the upper right corner.
    /// </summary>
    public class LoginInformationViewModel
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginInformationViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public LoginInformationViewModel(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the logout command.
        /// </summary>
        public ICommand LogoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.Logout);
            }
        }

        /// <summary>
        /// Gets the change password command.
        /// </summary>
        public ICommand ChangePasswordCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.ChangePassword);
            }
        }

        /// <summary>
        /// Gets the current application state, used for binding to the selected tenant and logged in user name.
        /// </summary>
        public IConnectedApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }
    }
}