// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShouldUpdateDialogResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShouldUpdateDialogResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Deployment.Application;

    using Gorba.Center.Common.Wpf.Framework;

    /// <summary>
    /// The dialog result telling the calling controller that the application should be updated.
    /// </summary>
    public class ShouldUpdateDialogResult : DialogResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShouldUpdateDialogResult"/> class.
        /// </summary>
        /// <param name="updateCheckInfo">
        /// The update check information.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public ShouldUpdateDialogResult(UpdateCheckInfo updateCheckInfo, string password)
        {
            this.UpdateCheckInfo = updateCheckInfo;
            this.Password = password;
        }

        /// <summary>
        /// Gets the result of the update check.
        /// </summary>
        public UpdateCheckInfo UpdateCheckInfo { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get; private set; }
    }
}
