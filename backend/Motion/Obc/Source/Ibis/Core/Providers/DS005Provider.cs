// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS005Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS005Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Telegram provider for DS005.
    /// </summary>
    public class DS005Provider : TimeTelegramProviderBase<DS005Config, DS005>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS005Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS005Provider(DS005Config config, IIbisContext context)
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
        protected override IEnumerable<DS005> DoCreatePeriodicTelegrams()
        {
            yield return new DS005 { Time = TimeProvider.Current.Now.ToString("HHmm", CultureInfo.InvariantCulture) };
        }
    }
}