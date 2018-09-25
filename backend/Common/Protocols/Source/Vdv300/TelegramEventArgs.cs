// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300
{
    using System;

    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// EventArgs containing a telegram.
    /// </summary>
    public class TelegramEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramEventArgs"/> class.
        /// </summary>
        /// <param name="telegram">The telegram associated to this event.</param>
        public TelegramEventArgs(Telegram telegram)
        {
            this.Telegram = telegram;
        }

        /// <summary>
        /// Gets or sets the telegram associated to this event.
        /// </summary>
        public Telegram Telegram { get; set; }
    }
}
