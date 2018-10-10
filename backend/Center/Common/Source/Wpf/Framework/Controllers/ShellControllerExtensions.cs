// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellControllerExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShellControllerExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines extension methods for the <see cref="IShellController"/>.
    /// </summary>
    public static class ShellControllerExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Sends a <see cref="Notification"/> to the specified <paramref name="controller"/>.
        /// <see cref="ProgressNotification"/>s are handled with additional logic:
        /// - if existing notifications with the same <see cref="ProgressNotification.ActivityId"/> are found, then
        /// the handled notification is used to update the existing one
        /// - when handling <see cref="ProgressNotification"/>s, the <see cref="IShellViewModel.OverallProgress"/> is
        /// evaluated as aggregation of existing non-acknowledged progress notifications
        /// </summary>
        /// <param name="controller">The shell controller.</param>
        /// <param name="notification">The notification.</param>
        public static void Notify(this IShellController controller, Notification notification)
        {
            Logger.Debug("Received notification {0}", notification);
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() => HandleNotification(controller, notification));
        }

        /// <summary>
        /// Notifies the specified progress information.
        /// </summary>
        /// <typeparam name="T">The type of the progress notification.</typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="notification">The notification.</param>
        public static void Notify<T>(this IShellController controller, T notification)
            where T : ProgressNotification
        {
            Logger.Debug("Received progress notification {0}", notification);
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() => HandleProgressNotification(controller, notification));
        }

        private static void HandleNotification(IShellController controller, Notification notification)
        {
            var progressNotification = notification as ProgressNotification;
            if (progressNotification != null)
            {
                HandleProgressNotification(controller, progressNotification);
                return;
            }

            var promptNotification = notification as PromptNotification;
            if (promptNotification != null)
            {
                HandlePromptNotification(controller, promptNotification);
                return;
            }

            var statusNotification = notification as StatusNotification;
            if (statusNotification != null)
            {
                HandleStatusNotification(controller, statusNotification);
                return;
            }

            controller.Shell.Notifications.Add(notification);
            Logger.Info("Added notification {0}", notification);
        }

        private static void HandleStatusNotification(IShellController controller, StatusNotification statusNotification)
        {
            var existingStatusNotifications = controller.Shell.Notifications.OfType<StatusNotification>().ToList();
            existingStatusNotifications.ForEach(notification => notification.IsAcknowledged = true);
            controller.Shell.Notifications.Add(statusNotification);
            controller.Shell.StatusNotifications.Refresh();
        }

        private static void HandlePromptNotification<T>(IShellController controller, T promptNotification)
            where T : PromptNotification
        {
            var interactionManager = ServiceLocator.Current.GetInstance<InteractionManager<T>>();
            interactionManager.Raise(
                promptNotification,
                prompt =>
                    {
                    });
        }

        private static void HandleProgressNotification(
            IShellController controller, ProgressNotification progressNotification)
        {
            Logger.Debug("Handling progress notification {0}", progressNotification);
            var pendingProgressNotifications =
                controller.Shell.Notifications.Where(notification => !notification.IsAcknowledged)
                          .OfType<ProgressNotification>()
                          .ToList();
            var existingProgressNotification =
                pendingProgressNotifications.SingleOrDefault(
                    notification => notification.ActivityId == progressNotification.ActivityId);
            if (existingProgressNotification == null)
            {
                Logger.Trace("Received new progress notification {0}", progressNotification);
                controller.Shell.Notifications.Add(progressNotification);
                if (!progressNotification.IsAcknowledged)
                {
                    pendingProgressNotifications.Add(progressNotification);
                }
            }
            else
            {
                Logger.Trace("Updating existing progress notification {0}", existingProgressNotification);
                existingProgressNotification.UpdateProgress(progressNotification);
            }

            controller.Shell.OverallProgress.IsAcknowledged = false;
            controller.Shell.OverallProgress.IsCompleted =
                pendingProgressNotifications.All(notification => notification.IsCompleted);
            var incompleted = pendingProgressNotifications.Where(notification => !notification.IsCompleted).ToList();
            if (incompleted.Count == 0)
            {
                controller.Shell.OverallProgress.Progress = 1;
                controller.Shell.OverallProgress.IsCompleted = true;
                Logger.Info("All progress notifications are completed. Set overall progress to completed as well.");
                return;
            }

            controller.Shell.OverallProgress.IsCompleted = false;
            var total = 100 * incompleted.Count;
            var completed = incompleted.Sum(notification => notification.Progress);
            controller.Shell.OverallProgress.Progress = completed * 100 / total;
            Logger.Info("Updated overall progress to value {0}", controller.Shell.OverallProgress.Progress);
        }
    }
}