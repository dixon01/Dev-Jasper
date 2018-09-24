// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080Provider.cs" company="Gorba AG">
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

    /// <summary>
    /// Telegram provider for DS080.
    /// </summary>
    public class DS080Provider : TelegramProviderBase<DS080Config, DS080>
    {
        private PortListener doorsOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS080Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS080Provider(DS080Config config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public override void Start()
        {
            base.Start();

            this.doorsOpen = new PortListener(MediAddress.Broadcast, "DoorsOpen");
            this.doorsOpen.ValueChanged += this.DoorsOpenOnValueChanged;
            this.doorsOpen.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            this.doorsOpen.Dispose();
            this.doorsOpen = null;

            base.Stop();
        }

        private void DoorsOpenOnValueChanged(object sender, EventArgs e)
        {
            if (!FlagValues.True.Equals(this.doorsOpen.Value))
            {
                return;
            }

            if (!this.Context.IsInStopBuffer || this.Context.IsTheBusDriving)
            {
                return;
            }

            this.Logger.Debug("Door: opened in the bus stop");
            this.RaiseTelegramCreated(new DS080());
        }
    }
}
