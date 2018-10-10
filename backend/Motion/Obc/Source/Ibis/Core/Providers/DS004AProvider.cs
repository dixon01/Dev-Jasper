// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004AProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004AProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram provider for DS004a.
    /// </summary>
    public class DS004AProvider : TelegramProviderBase<DS004AConfig, DS004A>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS004AProvider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS004AProvider(DS004AConfig config, IIbisContext context)
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
        protected override IEnumerable<DS004A> DoCreatePeriodicTelegrams()
        {
            yield return new DS004A { Characteristics = this.Context.RazziaCode };
        }
    }
}