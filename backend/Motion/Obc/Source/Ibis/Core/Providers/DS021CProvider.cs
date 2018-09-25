// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS021c.
    /// </summary>
    public class DS021CProvider : TelegramProviderBase<DS021CConfig, DS021C>
    {
        private readonly List<int> addresses;

        private bool stopListLoaded;

        private int currentTripId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021CProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS021CProvider(DS021CConfig config, IIbisContext context)
            : base(config, context)
        {
            this.addresses = context.Config.Devices.GorbaTft.Addresses.FindAll(a => a != -1);
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();

            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
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
        protected override bool HandleAnswer(Telegram answer, DS021C telegram)
        {
            return answer is DS120;
        }

        private void SendStopList()
        {
            this.stopListLoaded = true;
            if (RemoteEventHandler.CurrentTrip == null || this.addresses.Count == 0)
            {
                return;
            }

            this.currentTripId = RemoteEventHandler.CurrentTrip.Id;

            var status = 0;
            for (var i = 0; i < RemoteEventHandler.CurrentTrip.Stop.Count; i++)
            {
                var stop = RemoteEventHandler.CurrentTrip.Stop[i];
                var stopName = this.Context.GetStopNames(stop);
                var connections = this.Context.GetConnectionHints(i);
                var index = i + 1;

                this.SendTelegram(status, index, stopName, connections);

                status = 1;
            }

            // last telegram with destination
            this.SendTelegram(2, 101, RemoteEventHandler.CurrentTrip.AnnonceExt, string.Empty);
        }

        private void SendTelegram(int status, int index, string stopName, string connections)
        {
            var stopData = new[] { status.ToString("D1"), index.ToString("D3"), stopName, connections };
            foreach (var address in this.addresses)
            {
                this.RaiseTelegramCreated(new DS021C { IbisAddress = address, StopData = stopData });
            }
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            if (this.stopListLoaded)
            {
                return;
            }

            this.SendStopList();
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            if (this.stopListLoaded && RemoteEventHandler.CurrentTrip != null
                && RemoteEventHandler.CurrentTrip.Id == this.currentTripId)
            {
                return;
            }

            this.SendStopList();
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.stopListLoaded = false;
            this.SendTelegram(0, 1, this.Context.Config.Functionality.DefaultTextStop, string.Empty);
            this.SendTelegram(2, 101, this.Context.Config.Functionality.DefaultTextDestination, string.Empty);
        }
    }
}
