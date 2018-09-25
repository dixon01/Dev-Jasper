// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// The shell.
    /// </summary>
    public class Shell : ViewModelBase
    {
        private readonly Dispatcher dispatcher;

        private string title;

        private string status;

        private NotificationInfo selectedNotification;

        private string centerPortalAddress;

        private string time;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        public Shell(Dispatcher dispatcher)
        {
            // ReSharper disable once UnusedVariable
            var timer = new DispatcherTimer(
                new TimeSpan(0, 0, 1),
                DispatcherPriority.Normal,
                delegate
                    {
                        this.Time = DateTime.Now.ToLongTimeString();
                    },
                dispatcher);
            this.dispatcher = dispatcher;
            this.Notifications = new ObservableCollection<NotificationInfo>();
            this.NotificationsView = (CollectionView)CollectionViewSource.GetDefaultView(this.Notifications);
            this.NotificationsView.SortDescriptions.Add(
                new SortDescription("EnqueuedAtLocalTime", ListSortDirection.Descending));
            this.ClearCommand = new AsyncCommand(this.Clear, this.CanClear);
            this.Tools = new Tools(this);
        }

        /// <summary>
        /// Gets or sets the center portal address.
        /// </summary>
        public string CenterPortalAddress
        {
            get
            {
                return this.centerPortalAddress;
            }

            set
            {
                if (this.centerPortalAddress == value)
                {
                    return;
                }

                this.centerPortalAddress = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the clear command.
        /// </summary>
        public ICommand ClearCommand { get; private set; }

        /// <summary>
        /// Gets the tools.
        /// </summary>
        public Tools Tools { get; private set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (value == this.title)
                {
                    return;
                }

                this.title = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        public ObservableCollection<NotificationInfo> Notifications { get; private set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected notification.
        /// </summary>
        public NotificationInfo SelectedNotification
        {
            get
            {
                return this.selectedNotification;
            }

            set
            {
                if (this.selectedNotification == value)
                {
                    return;
                }

                this.selectedNotification = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the filtered and sorted view of notifications.
        /// </summary>
        public CollectionView NotificationsView { get; private set; }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public string Time
        {
            get
            {
                return this.time;
            }

            private set
            {
                if (this.time == value)
                {
                    return;
                }

                this.time = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Adds a notification.
        /// </summary>
        /// <param name="notificationInfo">The notification to add.</param>
        public void AddNotification(NotificationInfo notificationInfo)
        {
            notificationInfo.PropertyChanged += this.NotificationInfoOnPropertyChanged;
            Action addToShell = () =>
                {
                    this.Notifications.Add(notificationInfo);
                    var replies =
                        this.Notifications.Where(
                            info =>
                            info.NotificationType == NotificationType.Pong && !string.IsNullOrEmpty(info.ReplyTo))
                            .Select(info => new Guid(info.ReplyTo))
                            .ToList();
                    var waitingForReplies =
                        this.Notifications.Where(
                            info => info.NotificationType == NotificationType.Ping && !info.HasReply).ToList();
                    if (waitingForReplies.Any())
                    {
                        foreach (var notification in replies)
                        {
                            var waitingForReply = waitingForReplies.SingleOrDefault(info => info.Id == notification);
                            if (waitingForReply == null)
                            {
                                continue;
                            }

                            waitingForReply.HasReply = true;
                        }
                    }

                    CommandManager.InvalidateRequerySuggested();
                };
            this.dispatcher.InvokeAsync(addToShell, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Removes all notifications.
        /// </summary>
        internal void Clear()
        {
            this.Notifications.ToList().ForEach(info => info.PropertyChanged -= this.NotificationInfoOnPropertyChanged);
            this.dispatcher.InvokeAsync(this.Notifications.Clear, DispatcherPriority.DataBind);
        }

        private bool CanClear()
        {
            var canClear = false;
            this.dispatcher.Invoke(() => canClear = this.Notifications.Any());
            return canClear;
        }

        private void NotificationInfoOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "IsSelected")
            {
                return;
            }

            var notification = (NotificationInfo)sender;
            if (!string.IsNullOrEmpty(notification.ReplyTo))
            {
                var id = new Guid(notification.ReplyTo);
                var reply = this.Notifications.SingleOrDefault(info => info.Id == id);
                if (reply != null)
                {
                    reply.IsHighlighted = notification.IsSelected;
                    return;
                }
            }

            var request =
                this.Notifications.Where(
                    info =>
                    notification.Id != info.Id && !string.IsNullOrEmpty(info.ReplyTo)
                    && new Guid(info.ReplyTo) == notification.Id).ToList();
            if (!request.Any())
            {
                return;
            }

            request.ForEach(info => info.IsHighlighted = notification.IsSelected);
        }
    }
}