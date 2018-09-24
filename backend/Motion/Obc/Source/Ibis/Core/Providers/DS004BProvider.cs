// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004BProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004BProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram provider for DS004b.
    /// </summary>
    public class DS004BProvider : TelegramProviderBase<DS004BConfig, DS004B>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004BProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS004BProvider(DS004BConfig config, IIbisContext context)
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
        protected override IEnumerable<DS004B> DoCreatePeriodicTelegrams()
        {
            yield return new DS004B { StopId = this.Context.Didok };
        }
    }
}