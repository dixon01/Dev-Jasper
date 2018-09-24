// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PromptNotification.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PromptNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    /// <summary>
    /// Defines a notification used to prompt the user for actions/decisions.
    /// </summary>
    public abstract class PromptNotification : Notification
    {
        private bool isOpen;

        private bool suppressMouseEvents;

        /// <summary>
        /// Gets or sets a value indicating whether the confirmation is confirmed.
        /// </summary>
        public bool Confirmed { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the prompt is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }

            set
            {
                this.SetProperty(ref this.isOpen, value, () => this.IsOpen);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress mouse events.
        /// </summary>
        public bool SuppressMouseEvents
        {
            get
            {
                return this.suppressMouseEvents;
            }

            set
            {
                this.SetProperty(ref this.suppressMouseEvents, value, () => this.SuppressMouseEvents);
            }
        }
    }
}