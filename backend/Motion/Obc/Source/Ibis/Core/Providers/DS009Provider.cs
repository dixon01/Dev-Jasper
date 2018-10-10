// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS009Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS009Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram provider for DS009.
    /// </summary>
    public class DS009Provider : CurrentStopNameTelegramProviderBase<DS009Config, DS009>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS009Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        public DS009Provider(DS009Config config, IIbisContext context)
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
            this.RaiseTelegramCreated(new DS009 { StopName = stopName });
        }
    }
}