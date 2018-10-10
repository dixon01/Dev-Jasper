// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// The notification info.
    /// </summary>
    public class NotificationInfo : ViewModelBase
    {
        private bool isHighlighted;

        private bool isSelected;

        private bool hasReply;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public NotificationInfo(string path, Guid id, Notification notification)
            : this()
        {
            this.NotificationType = NotificationType.Unknown;
            this.Path = path;
            this.Id = id;
            this.Notification = notification;
            this.EnqueuedAtLocalTime = notification.EnqueuedTimeUtc.ToLocalTime();
            this.ExpiresAtLocalTime = notification.ExpiresAtUtc.ToLocalTime();
            this.CreateDescription(notification);
            var serializer = new DataContractSerializer(notification.GetType());
            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   OmitXmlDeclaration = true,
                                   NewLineOnAttributes = true
                               };
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.WriteObject(xmlWriter, notification);
                }

                this.Content = stringWriter.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        public NotificationInfo()
        {
            this.TimestampLocal = DateTime.Now;
        }

        /// <summary>
        /// Gets the content of the notification.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the (local) time when the notification was enqueued.
        /// </summary>
        public DateTime EnqueuedAtLocalTime { get; private set; }

        /// <summary>
        /// Gets the (local) time the notification expires.
        /// </summary>
        public DateTime ExpiresAtLocalTime { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is highlighted.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return this.isHighlighted;
            }

            set
            {
                if (this.isHighlighted == value)
                {
                    return;
                }

                this.isHighlighted = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the description of the notification.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the type of the notification.
        /// </summary>
        public NotificationType NotificationType { get; private set; }

        /// <summary>
        /// Gets or sets the ReplyTo.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime TimestampLocal { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        public Notification Notification { get; set; }

        /// <summary>
        /// Gets or sets the ReplyToSessionId.
        /// </summary>
        public string ReplyToSessionId { get; set; }

        /// <summary>
        /// Gets the time span required to receive this notification (time between enqueue and receive).
        /// </summary>
        public TimeSpan ReceivedAfter
        {
            get
            {
                return this.TimestampLocal.Subtract(this.EnqueuedAtLocalTime);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected == value)
                {
                    return;
                }

                this.isSelected = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a reply to this notification was received.
        /// </summary>
        public bool HasReply
        {
            get
            {
                return this.hasReply;
            }

            set
            {
                if (this.hasReply == value)
                {
                    return;
                }

                this.hasReply = value;
                this.RaisePropertyChanged();
            }
        }

        private void CreateDescription(Notification notification)
        {
            var sb = new StringBuilder("Id: ");
            sb.Append(notification.Id);
            sb.AppendLine();
            sb.Append("Type: ");
            var type = notification.GetType().Name;
            sb.Append(type);
            switch (type)
            {
                case "PingNotification":
                    this.NotificationType = NotificationType.Ping;
                    var readyGate = notification.Properties["ReadyGate"];
                    sb.AppendLine();
                    sb.Append(readyGate);
                    break;
                case "PongNotification":
                    this.NotificationType = NotificationType.Pong;
                    break;
            }

            this.Description = sb.ToString();
        }
    }
}