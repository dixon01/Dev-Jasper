// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS001Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS001.
    /// </summary>
    public class DS001Provider : TelegramProviderBase<DS001Config, DS001>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS001Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        public DS001Provider(DS001Config config, IIbisContext context)
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
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public override void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            base.Stop();
        }

        /// <summary>
        /// Creates the periodic telegram.
        /// This method is only called when a telegram has to be really created.
        /// </summary>
        /// <returns>
        /// The telegram or null if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS001> DoCreatePeriodicTelegrams()
        {
            yield return this.CreateTelegram();
        }

        private DS001 CreateTelegram()
        {
            var lineNumber = this.Context.Line.ToString("000", CultureInfo.InvariantCulture);
            var telegram = new DS001 { LineNumber = lineNumber };
            return telegram;
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            this.RaiseTelegramCreated(this.CreateTelegram());
        }
    }
}