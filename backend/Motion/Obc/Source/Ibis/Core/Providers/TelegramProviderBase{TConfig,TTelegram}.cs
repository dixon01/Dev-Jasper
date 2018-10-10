// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramProviderBase{TConfig,TTelegram}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Base class for all telegram providers giving some convenience methods and properties.
    /// </summary>
    /// <typeparam name="TConfig">The type of config used by this provider.</typeparam>
    /// <typeparam name="TTelegram">The type of telegram created by this provider.</typeparam>
    public abstract class TelegramProviderBase<TConfig, TTelegram> : TelegramProviderBase
        where TConfig : TelegramConfigBase
        where TTelegram : Telegram
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly long periodicWait;
        private long lastPeriodicTicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramProviderBase{TConfig,TTelegram}"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        protected TelegramProviderBase(TConfig config, IIbisContext context)
            : base(config, context)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());

            this.Config = config;
            this.IbisConfig = context.Config;
            this.periodicWait = Math.Max(
                config.RepeatInterval * 1000,
                this.IbisConfig.IbisTelegrams.MinRepeatIntervalConfig * 1000);

            this.ExpectsAnswer = typeof(IAddressedTelegram).IsAssignableFrom(typeof(TTelegram));
        }

        /// <summary>
        /// Gets the telegram config.
        /// </summary>
        protected new TConfig Config { get; private set; }

        /// <summary>
        /// Gets the IBIS config.
        /// </summary>
        protected IbisConfig IbisConfig { get; private set; }

        /// <summary>
        /// Creates the periodic telegrams if needed.
        /// This method is called frequently and should return quickly if no telegrams are required.
        /// </summary>
        /// <returns>
        /// The list of <see cref="Telegram"/>s, this can be empty if currently no periodic telegram is to be sent.
        /// </returns>
        public override IEnumerable<Telegram> CreatePeriodicTelegrams()
        {
            if (this.periodicWait <= 0)
            {
                yield break;
            }

            var ticks = TimeProvider.Current.TickCount;
            if (ticks - this.lastPeriodicTicks < this.periodicWait)
            {
                yield break;
            }

            var telegrams = this.DoCreatePeriodicTelegrams();
            this.lastPeriodicTicks = ticks;
            foreach (var telegram in telegrams)
            {
                yield return telegram;
            }
        }

        /// <summary>
        /// Handles the answer to a telegram.
        /// </summary>
        /// <param name="answer">
        /// The answer or null if no answer was received.
        /// </param>
        /// <param name="telegram">
        /// The original telegram that was sent to get the answer.
        /// </param>
        /// <returns>
        /// True if the answer was recognized and expected and handled by this provider.
        /// </returns>
        public override bool HandleAnswer(Telegram answer, Telegram telegram)
        {
            return this.ExpectsAnswer && this.HandleAnswer(answer, (TTelegram)telegram);
        }

        /// <summary>
        /// Handles the answer to a telegram.
        /// </summary>
        /// <param name="answer">
        /// The answer or null if no answer was received.
        /// </param>
        /// <param name="telegram">
        /// The original telegram that was sent to get the answer.
        /// </param>
        /// <returns>
        /// True if the answer was recognized and expected and handled by this provider.
        /// </returns>
        protected virtual bool HandleAnswer(Telegram answer, TTelegram telegram)
        {
            return false;
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected virtual IEnumerable<TTelegram> DoCreatePeriodicTelegrams()
        {
            yield break;
        }

        /// <summary>
        /// Raises the <see cref="TelegramProviderBase.TelegramCreated"/> event with the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected void RaiseTelegramCreated(TTelegram telegram)
        {
            this.RaiseTelegramCreated(new TelegramEventArgs(telegram));
        }
    }
}
