// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckInPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The commit prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Interaction
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The check in prompt.
    /// </summary>
    public class CheckInPrompt : PromptNotification, IDataErrorInfo
    {
        private string minor;

        private string major;

        private string checkInComment;

        private string error;

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IPermissionController PermissionController
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IClientApplicationController>().PermissionController;
            }
        }

        /// <summary>
        /// Gets the connected application state.
        /// </summary>
        public IConnectedApplicationState ConnectedApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        public string Minor
        {
            get
            {
                return this.minor;
            }

            set
            {
                this.SetProperty(ref this.minor, value, () => this.Minor);
            }
        }

        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        public string Major
        {
            get
            {
                return this.major;
            }

            set
            {
                this.SetProperty(ref this.major, value, () => this.Major);
            }
        }

        /// <summary>
        /// Gets or sets the comment for the check in.
        /// </summary>
        public string CheckInComment
        {
            get
            {
                return this.checkInComment;
            }

            set
            {
                this.SetProperty(ref this.checkInComment, value, () => this.CheckInComment);
            }
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.error;
            }

            set
            {
                this.SetProperty(ref this.error, value, () => this.Error);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is skippable.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public bool IsSkippable { get; set; }

        /// <summary>
        /// Gets or sets the check in continuation.
        /// </summary>
        public Action<CheckinTrapResult> OnCheckinCompleted { get; set; }

        /// <summary>
        /// Gets or sets the configuration label.
        /// </summary>
        public string ConfigurationLabel { get; set; }

        /// <summary>
        /// Gets or sets the configuration title.
        /// </summary>
        public string ConfigurationTitle { get; set; }

        /// <summary>
        /// Gets or sets the busy indicator text.
        /// </summary>
        public string BusyIndicatorText { get; set; }

        /// <summary>
        /// Gets or sets the data scope where the user needs write permission.
        /// </summary>
        public DataScope RequiredDataScope { get; set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                if (columnName.Equals("CheckInComment"))
                {
                    if (string.IsNullOrEmpty(this.CheckInComment))
                    {
                        this.Error = Strings.CheckInDialog_ErrorDescription_Empty;
                        return this.Error;
                    }
                }

                this.Error = string.Empty;
                return string.Empty;
            }
        }
    }
}
