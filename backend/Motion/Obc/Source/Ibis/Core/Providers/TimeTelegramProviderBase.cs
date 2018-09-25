// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeTelegramProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeTelegramProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Common.Entities.Gps;

    /// <summary>
    /// Base class for date and time telegrams.
    /// It doesn't allow to create periodic telegrams until the system GPS time is known (if configured so)
    /// </summary>
    /// <typeparam name="TConfig">The type of config used by this provider.</typeparam>
    /// <typeparam name="TTelegram">The type of telegram created by this provider.</typeparam>
    public abstract class TimeTelegramProviderBase<TConfig, TTelegram> : TelegramProviderBase<TConfig, TTelegram>
        where TConfig : TelegramConfigBase
        where TTelegram : Telegram
    {
        private GpsData gpsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTelegramProviderBase{TConfig,TTelegram}"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected TimeTelegramProviderBase(TConfig config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();
            MessageDispatcher.Instance.Subscribe<GpsData>(this.HandleGpsData);
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<GpsData>(this.HandleGpsData);
            base.Stop();
        }

        /// <summary>
        /// Creates the periodic telegrams if needed.
        /// This method is called frequently and should return quickly if no telegrams are required.
        /// </summary>
        /// <returns>
        /// The list of <see cref="Telegram"/>s, this can be empty if currently no periodic telegram is to be sent.
        /// </returns>
        public override IEnumerable<Telegram> CreatePeriodicTelegrams()
        {
            if (this.Context.Config.Functionality.PreventSendingDateTimeBeforeSynch
                && (this.gpsData == null || !this.gpsData.SatelliteTimeUtc.HasValue))
            {
                return TelegramProviderBase.EmptyTelegrams;
            }

            return base.CreatePeriodicTelegrams();
        }

        private void HandleGpsData(object sender, MessageEventArgs<GpsData> e)
        {
            this.gpsData = e.Message;
        }
    }
}