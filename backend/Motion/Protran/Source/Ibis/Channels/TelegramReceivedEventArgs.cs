// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramReceivedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Event that is fired by a channel when a new telegram is received.
    /// </summary>
    public class TelegramReceivedEventArgs : TelegramEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        public TelegramReceivedEventArgs(Telegram telegram, TelegramConfig config)
            : base(telegram)
        {
            this.Config = config;
        }

        /// <summary>
        /// Gets the telegram config.
        /// </summary>
        public TelegramConfig Config { get; private set; }
    }
}
