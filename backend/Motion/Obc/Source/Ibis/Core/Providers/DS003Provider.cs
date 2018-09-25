// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS003.
    /// </summary>
    public class DS003Provider : TelegramProviderBase<DS003Config, DS003>
    {
        private int currentStopId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS003Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        public DS003Provider(DS003Config config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();

            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS003> DoCreatePeriodicTelegrams()
        {
            yield return new DS003 { DestinationNumber = this.Context.Destination };
        }

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            this.currentStopId = e.Message.StopId;
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.currentStopId = 0;
        }

        private void HandleTripEnded(object sender, MessageEventArgs<evTripEnded> e)
        {
            this.currentStopId = 0;
        }
    }
}