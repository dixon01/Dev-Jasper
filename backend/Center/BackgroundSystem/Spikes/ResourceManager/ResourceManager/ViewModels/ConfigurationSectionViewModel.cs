// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigurationSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Controllers;
    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Defines a section for configuration.
    /// </summary>
    public class ConfigurationSectionViewModel : ViewModelBase
    {
        private string userName;
        private string password;
        private bool credentialsCreated;

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.userName = value;
                this.OnPropertyChanged("UserName");
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
            }
        }

        /// <summary>
        /// Gets the create credentials command.
        /// </summary>
        public ICommand CreateCredentialsCommand
        {
            get
            {
                return new AsyncCommand(this.GetCredentials);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether credentials created.
        /// </summary>
        public bool CredentialsCreated
        {
            get
            {
                return this.credentialsCreated;
            }

            set
            {
                this.credentialsCreated = value;
                this.OnPropertyChanged("CredentialsCreated");
            }
        }

        private void GetCredentials()
        {
            var controller = DependencyResolver.Current.Get<ApplicationController>();
            var userCredentials = new UserCredentials(this.userName, SecurityUtility.Md5(this.password));
            controller.CreateCredentials(userCredentials);
            this.CredentialsCreated = true;
        }
    }
}