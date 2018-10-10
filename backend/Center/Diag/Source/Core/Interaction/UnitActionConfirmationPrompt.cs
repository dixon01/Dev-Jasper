// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitActionConfirmationPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitActionConfirmationPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The unit action prompt notification.
    /// </summary>
    public class UnitActionConfirmationPrompt : PromptNotification
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }
}
