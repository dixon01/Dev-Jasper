// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS010JProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS010JProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS010j.
    /// </summary>
    public class DS010JProvider : TelegramProviderBase<DS010JConfig, DS010J>
    {
        private DS010J currentTelegram;

        private int currentStopIndex;

        private bool nextStopDisplayed;

        private bool tripLoaded;

        private int currentTripId;

        private PortListener doorsOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS010JProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS010JProvider(DS010JConfig config, IIbisContext context)
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
            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Subscribe<evBusDriving>(this.HandleBusDriving);

            this.doorsOpen = new PortListener(MediAddress.Broadcast, "DoorsOpen");
            this.doorsOpen.ValueChanged += this.DoorsOpenOnValueChanged;
            this.doorsOpen.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Unsubscribe<evBusDriving>(this.HandleBusDriving);

            this.doorsOpen.Dispose();
            this.doorsOpen = null;

            base.Stop();
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS010J> DoCreatePeriodicTelegrams()
        {
            if (!this.Context.Config.Devices.GorbaTft.HasUnit() || this.currentTelegram == null)
            {
                yield break;
            }

            yield return this.currentTelegram;
        }

        private void SendTelegram(int status, int stopIndex)
        {
            this.currentTelegram = new DS010J { Status = status, StopIndex = stopIndex };
            this.RaiseTelegramCreated(this.currentTelegram);
        }

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            // Initialize now the values we may need in the buffer
            this.nextStopDisplayed = false;
            this.currentStopIndex = e.Message.StopId;
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            if (!this.nextStopDisplayed)
            {
                this.SendTelegram(2, e.Message.StopId + 2);
            }

            // Reinitialise vars for next stop
            this.nextStopDisplayed = false;
        }

        private void HandleBusDriving(object sender, MessageEventArgs<evBusDriving> e)
        {
            if (e.Message.IsDriving)
            {
                this.Logger.Debug("Odometer: Bus started to drive");
                if (this.Context.IsDoorCycled && this.Context.IsInStopBuffer)
                {
                    this.Logger.Debug("Buffer: Buffer entered");

                    // Index: stoplist = 0...x, ibis = 1...(x+1) --> ibis next stop = x+2
                    this.SendTelegram(2, this.currentStopIndex + 2);
                    this.nextStopDisplayed = true;
                }
            }
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            if (RemoteEventHandler.CurrentTrip == null)
            {
                return;
            }

            if (this.tripLoaded && RemoteEventHandler.CurrentTrip.Id == this.currentTripId)
            {
                return;
            }

            this.tripLoaded = true;
            this.currentTripId = RemoteEventHandler.CurrentTrip.Id;

            this.SendTelegram(2, 1);
        }

        private void HandleTripEnded(object sender, MessageEventArgs<evTripEnded> e)
        {
            this.currentStopIndex = 0;
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.tripLoaded = false;
            this.currentStopIndex = 0;

            this.SendTelegram(2, 1);
        }

        private void DoorsOpenOnValueChanged(object sender, EventArgs e)
        {
            if (!this.Context.IsInStopBuffer || this.Context.IsTheBusDriving
                || !FlagValues.False.Equals(this.doorsOpen.Value) || this.nextStopDisplayed)
            {
                return;
            }

            this.Logger.Info("Displaying the next station");
            this.SendTelegram(3, this.currentStopIndex + 1);
        }
    }
}