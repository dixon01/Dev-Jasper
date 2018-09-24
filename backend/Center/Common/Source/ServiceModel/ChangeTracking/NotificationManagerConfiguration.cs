// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationManagerConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManagerConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines the configuration of a notification manager.
    /// </summary>
    public class NotificationManagerConfiguration : ICloneable
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the path of the notifications.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A new instance with same property values.</returns>
        public NotificationManagerConfiguration Clone()
        {
            return (NotificationManagerConfiguration)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}