// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoginViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LoginViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The LoginViewModel interface.
    /// </summary>
    public interface ILoginViewModel : IDialogViewModel
    {
        /// <summary>
        /// Gets or sets the activity message.
        /// </summary>
        /// <value>
        /// The activity message.
        /// </value>
        ActivityMessage ActivityMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        bool IsBusy { get; set; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        IConnectedApplicationState ApplicationState { get; }

        /// <summary>
        /// Gets or sets the input server.
        /// </summary>
        /// <value>
        /// The input server.
        /// </value>
        string InputServer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is server URI valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is server URI valid; otherwise, <c>false</c>.
        /// </value>
        bool IsServerUriValid { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether had no tenants.
        /// </summary>
        bool HadNoTenants { get; set; }

        /// <summary>
        /// Gets the server URI.
        /// </summary>
        Uri ServerUri { get; }

        /// <summary>
        /// Gets the recent servers for smart completion.
        /// </summary>
        IEnumerable<string> RecentServers { get; }
    }
}
