// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004CProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004CProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram provider for DS004c.
    /// </summary>
    public class DS004CProvider : TelegramProviderBase<DS004CConfig, DS004C>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004CProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS004CProvider(DS004CConfig config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS004C> DoCreatePeriodicTelegrams()
        {
            yield return new DS004C { StopName = this.Context.DruckName };
        }
    }
}