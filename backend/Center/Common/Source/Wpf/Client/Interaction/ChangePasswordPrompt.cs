// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePasswordPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangePasswordPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Interaction
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Prompt that asks the user to change the password.
    /// </summary>
    public class ChangePasswordPrompt : PromptNotification, INotifyDataErrorInfo
    {
        private readonly ErrorViewModelBase errors = new ErrorViewModelBase();

        private readonly string currentHashedPassword;

        private string currentPassword;

        private string newPassword;

        private string repeatPassword;

        private bool showCurrentPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordPrompt"/> class.
        /// </summary>
        /// <param name="title">
        /// The title of the dialog.
        /// </param>
        /// <param name="currentHashedPassword">
        /// The MD5 hash of the current password.
        /// </param>
        public ChangePasswordPrompt(string title, string currentHashedPassword)
        {
            this.currentHashedPassword = currentHashedPassword;
            this.Title = title;
            this.ShowCurrentPassword = true;
            this.CurrentPassword = string.Empty;
            this.NewPassword = string.Empty;
            this.RepeatPassword = string.Empty;

            this.errors.ErrorsChanged += (s, e) => this.RaiseErrorsChanged(e);
            this.errors.PropertyChanged += (s, e) => this.RaisePropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets or sets the current password.
        /// </summary>
        public string CurrentPassword
        {
            get
            {
                return this.currentPassword;
            }

            set
            {
                if (this.SetProperty(ref this.currentPassword, value, () => this.CurrentPassword))
                {
                    this.UpdateCurrentPasswordError();
                }
            }
        }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword
        {
            get
            {
                return this.newPassword;
            }

            set
            {
                if (!this.SetProperty(ref this.newPassword, value, () => this.NewPassword))
                {
                    return;
                }

                this.errors.ChangeError(
                    "NewPassword",
                    Strings.ChangePassword_Error_InvalidPassword,
                    string.IsNullOrWhiteSpace(value) || value.Length < 8);
                this.VerifyRepeatPassword();
            }
        }

        /// <summary>
        /// Gets or sets the repeated password (for verification of the new password).
        /// </summary>
        public string RepeatPassword
        {
            get
            {
                return this.repeatPassword;
            }

            set
            {
                if (this.SetProperty(ref this.repeatPassword, value, () => this.RepeatPassword))
                {
                    this.VerifyRepeatPassword();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current password box should be shown.
        /// </summary>
        public bool ShowCurrentPassword
        {
            get
            {
                return this.showCurrentPassword;
            }

            set
            {
                if (this.SetProperty(ref this.showCurrentPassword, value, () => this.ShowCurrentPassword))
                {
                    this.UpdateCurrentPasswordError();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <returns>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </returns>
        public bool HasErrors
        {
            get
            {
                return this.errors.HasErrors;
            }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or
        /// <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
        public IEnumerable GetErrors(string propertyName)
        {
            return this.errors.GetErrors(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseErrorsChanged(DataErrorsChangedEventArgs e)
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void VerifyRepeatPassword()
        {
            this.errors.ChangeError(
                "RepeatPassword",
                Strings.ChangePassword_Error_RepeatNoMatch,
                !string.Equals(this.NewPassword, this.RepeatPassword));
        }

        private void UpdateCurrentPasswordError()
        {
            var hasError = this.ShowCurrentPassword && this.currentPassword != null
                           && !SecurityUtility.Md5(this.currentPassword).Equals(this.currentHashedPassword);
            this.errors.ChangeError("CurrentPassword", Strings.ChangePassword_Error_CurrentNoMatch, hasError);
        }
    }
}
