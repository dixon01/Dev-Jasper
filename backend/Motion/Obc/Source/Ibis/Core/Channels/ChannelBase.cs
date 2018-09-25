// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Channels
{
    using System;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// The base class for all IBIS channels.
    /// </summary>
    public abstract class ChannelBase
    {
        /// <summary>
        /// Event that is fired whenever a telegram is received.
        /// </summary>
        public event EventHandler<TelegramEventArgs> TelegramReceived;

        /// <summary>
        /// Creates an instance of <see cref="ChannelBase"/> for the given <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="ChannelBase"/>.
        /// </returns>
        public static ChannelBase Create(InterfaceSettings settings)
        {
            if (settings.SerialPortConfig != null)
            {
                // TODO: on VM.c/VM.r this would have to use the native methods instead
                return new SerialPortChannel(settings.SerialPortConfig);
            }

            throw new NotSupportedException("Unsupported channel type");
        }

        /// <summary>
        /// Opens this channel.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Closes this channel.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Sends a telegram on this channel.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="config">
        /// The configuration of the telegram.
        /// </param>
        public abstract void SendTelegram(Telegram telegram, TelegramConfigBase config);

        /// <summary>
        /// Reopens this channel.
        /// </summary>
        protected void Reopen()
        {
            this.Close();
            this.Open();
        }

        /// <summary>
        /// Raises the <see cref="TelegramReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTelegramReceived(TelegramEventArgs e)
        {
            var handler = this.TelegramReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
