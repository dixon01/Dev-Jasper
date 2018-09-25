// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentStopNameTelegramProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CurrentStopNameTelegramProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Provider for telegrams that show the current stop.
    /// </summary>
    /// <typeparam name="TConfig">The type of config used by this provider.</typeparam>
    /// <typeparam name="TTelegram">The type of telegram created by this provider.</typeparam>
    public abstract class CurrentStopNameTelegramProviderBase<TConfig, TTelegram> :
        TelegramProviderBase<TConfig, TTelegram>
        where TConfig : TelegramConfigBase
        where TTelegram : Telegram
    {
        private int currentTripId = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentStopNameTelegramProviderBase{TConfig,TTelegram}"/>
        /// class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        protected CurrentStopNameTelegramProviderBase(TConfig config, IIbisContext context)
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
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
        }

        /// <summary>
        /// Sends the stop name as a telegram.
        /// </summary>
        /// <param name="stopName">
        /// The stop name.
        /// </param>
        protected abstract void SendStopName(string stopName);

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            if (e.Message.StopId > 0 && RemoteEventHandler.CurrentTrip != null)
            {
                this.SendStopName(RemoteEventHandler.CurrentTrip.Stop[e.Message.StopId].Name1);
            }
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            if (RemoteEventHandler.CurrentTrip != null)
            {
                this.currentTripId = RemoteEventHandler.CurrentTrip.Id;
                this.SendStopName(RemoteEventHandler.CurrentTrip.Stop[e.Message.StopId + 1].Name1);
            }
            else
            {
                this.SendStopName(string.Empty);
            }
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            if (RemoteEventHandler.CurrentTrip != null)
            {
                if (this.currentTripId == RemoteEventHandler.CurrentTrip.Id)
                {
                    return;
                }

                this.currentTripId = RemoteEventHandler.CurrentTrip.Id;
            }

            this.SendStopName(string.Empty);
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            this.SendStopName(string.Empty);
        }

        private void HandleServiceStarted(object sender, MessageEventArgs<evServiceStarted> e)
        {
            this.SendStopName(string.Empty);
        }
    }
}