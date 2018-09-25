// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS036.
    /// </summary>
    public class DS036Provider : TelegramProviderBase<DS036Config, DS036>
    {
        private int currentStopIndex;

        private int endAnnouncement;

        private PortListener doorsOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS036Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS036Provider(DS036Config config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();

            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Subscribe<evRazziaStart>(this.HandleRazziaStart);
            MessageDispatcher.Instance.Subscribe<evAnnouncement>(this.HandleAnnouncement);

            this.doorsOpen = new PortListener(MediAddress.Broadcast, "DoorsOpen");
            this.doorsOpen.ValueChanged += this.DoorsOpenOnValueChanged;
            this.doorsOpen.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Unsubscribe<evRazziaStart>(this.HandleRazziaStart);
            MessageDispatcher.Instance.Unsubscribe<evAnnouncement>(this.HandleAnnouncement);

            this.doorsOpen.Dispose();
            this.doorsOpen = null;

            base.Stop();
        }

        private void SendTelegram(int announcementId)
        {
            this.RaiseTelegramCreated(new DS036 { AnnouncementIndex = announcementId.ToString("D4") });
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.Logger.Debug("ServiceEnded event received");
            if (this.endAnnouncement != 0)
            {
                this.SendTelegram(this.endAnnouncement);
                this.endAnnouncement = 0;
            }
        }

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            this.currentStopIndex = e.Message.StopId;
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            if (RemoteEventHandler.CurrentTrip != null)
            {
                // Announcing system part (is part of Line Presentation Unit)
                if (RemoteEventHandler.CurrentTrip.Stop[e.Message.StopId + 1].Announce > -1)
                {
                    this.SendTelegram(RemoteEventHandler.CurrentTrip.Stop[e.Message.StopId + 1].Announce);
                }

                if (RemoteEventHandler.CurrentTrip.Stop[e.Message.StopId + 1].SignCode == 999
                    && RemoteEventHandler.CurrentTrip.Stop.Count == e.Message.StopId + 2)
                {
                    // This is the second last stop of a trip
                    // Remember to make an announcement when serviceended event received
                    this.endAnnouncement = 8009;
                }
                else
                {
                    this.endAnnouncement = 0;
                }
            }
        }

        private void DoorsOpenOnValueChanged(object sender, EventArgs e)
        {
            if (!this.Context.IsInStopBuffer || this.Context.IsTheBusDriving
                || !FlagValues.True.Equals(this.doorsOpen.Value))
            {
                return;
            }

            this.Logger.Debug("Door: opened in the bus stop");
            if (!this.Context.IsDoorCycled && this.Context.Config.Functionality.DestinationAnnouncement
                && RemoteEventHandler.CurrentTrip != null)
            {
                this.SendTelegram(RemoteEventHandler.CurrentTrip.Stop[this.currentStopIndex].SignCode);
            }
        }

        private void HandleAnnouncement(object sender, MessageEventArgs<evAnnouncement> e)
        {
            this.Logger.Info("Announcement event received N {0}", e.Message.Announcement);
            this.SendTelegram(e.Message.Announcement);
        }

        private void HandleRazziaStart(object sender, MessageEventArgs<evRazziaStart> e)
        {
            this.Logger.Info("RazziaStart event received");
            this.SendTelegram(8008);
        }
    }
}
