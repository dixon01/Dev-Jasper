// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginWindowBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginWindowBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Helpers;

    /// <summary>
    /// Base Login window view model.
    /// </summary>
    public abstract class LoginWindowBase : DialogViewModelBase, IDataErrorInfo
    {
        private readonly ICommand exitCommand;

        private readonly CenterUriBuilder builder = new CenterUriBuilder();

        private readonly ICommand doLoginCommand;

        private Uri baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");

        private bool isServerUriValid;

        private string inputServer;

        private string password;

        private string username;

        private Uri serverUri;

        private ActivityMessage activityMessage = new ActivityMessage();

        private bool isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindowBase"/> class.
        /// </summary>
        /// <param name="factory">
        /// The view.
        /// </param>
        /// <param name="exitCommand">
        /// The exit Command.
        /// </param>
        /// <param name="doLoginCommand">
        /// The do Login Command.
        /// </param>
        /// <param name="previousServers">
        /// The previous servers for smart completion.
        /// </param>
        /// <param name="previousUsername">
        /// The user name of the last login.
        /// </param>
        /// <param name="offlineModeCommand">
        /// The command to go into offline mode.
        /// If this parameter is left null, the offline mode won't be available.
        /// </param>
        protected LoginWindowBase(
            IDialogFactory factory,
            ICommand exitCommand,
            ICommand doLoginCommand,
            IEnumerable<string> previousServers,
            string previousUsername,
            ICommand offlineModeCommand = null)
            : base(factory)
        {
            this.exitCommand = exitCommand;
            this.doLoginCommand = doLoginCommand;
            this.OfflineModeCommand = offlineModeCommand ?? new RelayCommand(() => { }, s => false);
            this.HasOfflineMode = offlineModeCommand != null;
            this.Error = null;
            this.RecentServers = previousServers;
            this.Username = previousUsername;
            this.WindowTitle = string.Empty;
        }

        /// <summary>
        /// Gets or sets the base URI.
        /// </summary>
        public Uri BaseUri
        {
            get
            {
                return this.baseUri;
            }

            set
            {
                this.SetProperty(ref this.baseUri, value, () => this.BaseUri);
            }
        }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                return this.exitCommand;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the DoLogin command.
        /// </summary>
        public ICommand DoLoginCommand
        {
            get
            {
                return this.doLoginCommand;
            }
        }

        /// <summary>
        /// Gets the offline mode command.
        /// </summary>
        public ICommand OfflineModeCommand { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this application supports offline mode.
        /// </summary>
        public bool HasOfflineMode { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                if (this.SetProperty(ref this.isBusy, value, () => this.IsBusy))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the server URI is valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server URI valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerUriValid
        {
            get
            {
                return this.isServerUriValid;
            }

            set
            {
                if (this.SetProperty(ref this.isServerUriValid, value, () => this.IsServerUriValid))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.SetProperty(ref this.password, value, () => this.Password))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets the recent servers for smart completion.
        /// </summary>
        public IEnumerable<string> RecentServers { get; private set; }

        /// <summary>
        /// Gets or sets the input server.
        /// </summary>
        /// <value>
        /// The input server.
        /// </value>
        public string InputServer
        {
            get
            {
                return this.inputServer;
            }

            set
            {
                if (this.SetProperty(ref this.inputServer, value, () => this.InputServer))
                {
                    this.IsServerUriValid = this.builder.TryBuild(this.baseUri, value, out this.serverUri);

                    // No need to invalidate command, already done in the IsServerUriValid setter.
                }
            }
        }

        /// <summary>
        /// Gets the generated server URI.
        /// </summary>
        public Uri ServerUri
        {
            get
            {
                return this.serverUri;
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                if (this.SetProperty(ref this.username, value, () => this.Username))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets the activity message.
        /// </summary>
        /// <value>
        /// The activity message.
        /// </value>
        public ActivityMessage ActivityMessage
        {
            get
            {
                return this.activityMessage;
            }

            set
            {
                this.SetProperty(ref this.activityMessage, value, () => this.ActivityMessage);
            }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the application title that is displayed in the header bar section.
        /// </summary>
        public string ApplicationTitle { get; set; }

        /// <summary>
        /// Gets or sets the application icon that is displayed in the header bar section.
        /// </summary>
        public ImageSource ApplicationIcon { get; set; }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column for the validation.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public virtual string this[string columnName]
        {
            get
            {
                if (columnName == "InputServer" && !this.isServerUriValid)
                {
                    return "Server invalid";
                }

                return null;
            }
        }
    }
}