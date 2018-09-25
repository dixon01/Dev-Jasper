// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisMaster.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Ibis.Core.Channels;
    using Gorba.Motion.Obc.Ibis.Core.Providers;

    using NLog;

    /// <summary>
    /// The IBIS (VDV 300) master control.
    /// </summary>
    public class IbisMaster
    {
        private const int AnswerTimeout = 100; // TODO: find the right value and perhaps make this configurable

        private static readonly Logger Logger = LogHelper.GetLogger<IbisMaster>();

        private readonly AutoResetEvent sleepEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent answerEvent = new AutoResetEvent(false);

        private readonly List<TelegramProviderBase> telegramProviders = new List<TelegramProviderBase>();

        private readonly Queue<KeyValuePair<TelegramProviderBase, Telegram>> eventTelegrams =
            new Queue<KeyValuePair<TelegramProviderBase, Telegram>>();

        private bool running;

        private ChannelBase channel;

        private Telegram receivedAnswer;

        private IbisContext context;

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="ibisConfig">
        /// The ibis config.
        /// </param>
        public void Configure(IbisConfig ibisConfig)
        {
            this.context = new IbisContext(ibisConfig);
            foreach (var telegramConfig in ibisConfig.IbisTelegrams.Telegrams)
            {
                if (!telegramConfig.Enabled)
                {
                    continue;
                }

                Logger.Trace("Creating provider for {0}", telegramConfig.GetType().Name);
                var provider = TelegramProviderBase.Create(telegramConfig, this.context);
                provider.TelegramCreated += this.ProviderOnTelegramCreated;
                this.telegramProviders.Add(provider);
            }

            this.channel = ChannelBase.Create(ibisConfig.InterfaceSettings);
            this.channel.TelegramReceived += this.ChannelOnTelegramReceived;
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            this.running = true;
            this.channel.Open();

            foreach (var provider in this.telegramProviders)
            {
                Logger.Trace("Starting provider: {0}", provider);
                provider.Start();
            }

            Logger.Debug("Starting Run() loop");
            var lastTicks = TimeProvider.Current.TickCount;
            while (this.running)
            {
                var hasMoreEvents = this.SendNextEventTelegram();
                this.SendPeriodicTelegrams();

                var ticks = TimeProvider.Current.TickCount;
                var sleep = 200 + lastTicks - ticks;
                if (!hasMoreEvents && sleep > 0)
                {
                    this.sleepEvent.WaitOne((int)sleep, true);
                }

                lastTicks = ticks;
            }

            Logger.Debug("Exited Run() loop");
            foreach (var provider in this.telegramProviders)
            {
                Logger.Trace("Stopping provider: {0}", provider.GetType().Name);
                provider.Stop();
            }

            this.channel.Close();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            Logger.Debug("Stopping");
            this.running = false;
            this.sleepEvent.Set();
        }

        private void SendPeriodicTelegrams()
        {
            foreach (var provider in this.telegramProviders)
            {
                IEnumerable<Telegram> telegrams;
                try
                {
                    telegrams = provider.CreatePeriodicTelegrams();
                }
                catch (Exception ex)
                {
                    Logger.WarnException("Couldn't create periodic telegrams for " + provider, ex);
                    continue;
                }

                foreach (var telegram in telegrams)
                {
                    this.SendTelegramAndAwaitAnswer(telegram, provider);
                }
            }
        }

        private bool SendNextEventTelegram()
        {
            KeyValuePair<TelegramProviderBase, Telegram> pair;
            bool hasMore;
            lock (this.eventTelegrams)
            {
                if (this.eventTelegrams.Count == 0)
                {
                    return false;
                }

                pair = this.eventTelegrams.Dequeue();
                hasMore = this.eventTelegrams.Count > 0;
            }

            this.SendTelegramAndAwaitAnswer(pair.Value, pair.Key);
            return hasMore;
        }

        private void SendTelegramAndAwaitAnswer(Telegram telegram, TelegramProviderBase provider)
        {
            try
            {
                Logger.Trace("Sending {0}", telegram);
                this.receivedAnswer = null;
                this.answerEvent.Reset();
                this.channel.SendTelegram(telegram, provider.Config);
                if (!provider.ExpectsAnswer)
                {
                    return;
                }

                var timeout = TimeProvider.Current.TickCount + AnswerTimeout;
                var ticks = TimeProvider.Current.TickCount;
                do
                {
                    if (!this.answerEvent.WaitOne((int)(timeout - ticks), true))
                    {
                        break;
                    }

                    var answer = this.receivedAnswer;
                    if (provider.HandleAnswer(answer, telegram))
                    {
                        Logger.Debug("Received expected answer to {0}: {1}", telegram, answer);
                        return;
                    }

                    Logger.Warn("Didn't get expected answer to {0}: {1}", telegram, answer);
                    ticks = TimeProvider.Current.TickCount;
                }
                while (ticks < timeout && this.running);

                Logger.Info("Didn't get answer to {0} within {1} ms", telegram, AnswerTimeout);
                provider.HandleAnswer(null, telegram);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't send telegram " + telegram, ex);
            }
        }

        private void ProviderOnTelegramCreated(object sender, TelegramEventArgs e)
        {
            Logger.Trace("Enqueuing {0}", e.Telegram);
            lock (this.eventTelegrams)
            {
                this.eventTelegrams.Enqueue(
                    new KeyValuePair<TelegramProviderBase, Telegram>(sender as TelegramProviderBase, e.Telegram));
            }

            this.sleepEvent.Set();
        }

        private void ChannelOnTelegramReceived(object sender, TelegramEventArgs e)
        {
            Logger.Trace("Received answer: {0}", e.Telegram);
            this.receivedAnswer = e.Telegram;

            this.answerEvent.Set();
        }
    }
}