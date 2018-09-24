// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS004.
    /// </summary>
    public class DS004Provider : TelegramProviderBase<DS004Config, DS004>
    {
        private int lastSentZone = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS004Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS004Provider(DS004Config config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evZoneChanged>(this.HandleZoneChanged);
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evZoneChanged>(this.HandleZoneChanged);
            base.Stop();
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS004> DoCreatePeriodicTelegrams()
        {
            yield return this.CreateTelegram();
        }

        private DS004 CreateTelegram()
        {
            this.lastSentZone = this.Context.CurrentZone;
            return new DS004 { Characteristics = this.lastSentZone };
        }

        private void SendTelegramIfNeeded()
        {
            if (this.lastSentZone != this.Context.CurrentZone)
            {
                this.RaiseTelegramCreated(this.CreateTelegram());
            }
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            this.SendTelegramIfNeeded();
        }

        private void HandleZoneChanged(object sender, MessageEventArgs<evZoneChanged> e)
        {
            this.SendTelegramIfNeeded();
        }
    }
}
