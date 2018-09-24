// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActivityMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the model of feedback given to user about the current or the most recent activity.
    /// </summary>
    public class ActivityMessage : ViewModelBase
    {
        private const ActivityMessageType DefaultActivityMessageType = ActivityMessageType.Info;

        private bool isHidden;

        private string message;

        private ActivityMessageType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMessage"/> class.
        /// </summary>
        public ActivityMessage()
        {
            this.Type = DefaultActivityMessageType;
        }

        /// <summary>
        /// The activity message type.
        /// </summary>
        public enum ActivityMessageType
        {
            /// <summary>
            /// None type.
            /// </summary>
            None = 0,

            /// <summary>
            /// Info type, used for normal messages.
            /// </summary>
            Info = 1,

            /// <summary>
            /// Warning type, used to warn the user.
            /// </summary>
            Warning = 2,

            /// <summary>
            /// Error type, used to alert about errors.
            /// </summary>
            Error = 3
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message is hidden; otherwise, <c>false</c>.
        /// </value>
        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }

            set
            {
                this.SetProperty(ref this.isHidden, value, () => this.IsHidden);
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.SetProperty(ref this.message, value, () => this.Message);
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ActivityMessageType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.SetProperty(ref this.type, value, () => this.Type);
            }
        }
    }
}