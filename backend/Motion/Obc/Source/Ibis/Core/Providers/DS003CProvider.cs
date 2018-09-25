// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003CProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003CProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram provider for DS003c.
    /// </summary>
    public class DS003CProvider : CurrentStopNameTelegramProviderBase<DS003CConfig, DS003C>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS003CProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        public DS003CProvider(DS003CConfig config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Sends the stop name as a telegram.
        /// </summary>
        /// <param name="stopName">
        /// The stop name.
        /// </param>
        protected override void SendStopName(string stopName)
        {
            this.RaiseTelegramCreated(new DS003C { StopName = stopName });
        }
    }
}