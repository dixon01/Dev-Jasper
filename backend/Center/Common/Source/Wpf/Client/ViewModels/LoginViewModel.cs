// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The login view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Windows;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel : LoginWindowBase, ILoginViewModel, IStartupDialogViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="exitCommand">
        /// The exit Command.
        /// </param>
        /// <param name="loginCommand">
        /// The login Command.
        /// </param>
        /// <param name="recentServers">
        /// The recent servers.
        /// </param>
        /// <param name="offlineModeCommand">
        /// The command to go into offline mode.
        /// If this parameter is left null, the offline mode won't be available.
        /// </param>
        public LoginViewModel(
            IDialogFactory factory,
            ICommand exitCommand,
            ICommand loginCommand,
            IEnumerable<string> recentServers,
            ICommand offlineModeCommand = null)
            : base(factory, exitCommand, loginCommand, recentServers, string.Empty, offlineModeCommand)
        {
        }

        /// <summary>
        /// Gets the application state.
        /// </summary>
        public IConnectedApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether had no tenants.
        /// </summary>
        public bool HadNoTenants { get; set; }

        /// <summary>
        /// Gets or sets the version of this application.
        /// </summary>
        public string ApplicationVersion { get; set; }
    }
}
