// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageNotificationType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageNotificationType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    /// <summary>
    /// Defines the possible types of message notifications.
    /// </summary>
    public enum MessageNotificationType
    {
        /// <summary>
        /// Undefined type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Info message.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning message.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error message.
        /// </summary>
        Error = 3
    }
}