// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShellViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IShellViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines a shell.
    /// </summary>
    public interface IShellViewModel
    {
        /// <summary>
        /// Gets or sets the title of the application.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        ObservableCollection<Notification> Notifications { get; }

        /// <summary>
        /// Gets the status notifications.
        /// </summary>
        ICollectionView StatusNotifications { get; }

        /// <summary>
        /// Gets the overall progress intended as aggregation of <see cref="ProgressNotification"/>s received.
        /// </summary>
        ProgressNotification OverallProgress { get; }
    }
}