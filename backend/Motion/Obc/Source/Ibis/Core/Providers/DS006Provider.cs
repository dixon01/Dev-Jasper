// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS006Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS006Provider type.
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
    /// Telegram provider for DS006.
    /// </summary>
    public class DS006Provider : TimeTelegramProviderBase<DS006Config, DS006>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS006Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS006Provider(DS006Config config, IIbisContext context)
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
        protected override IEnumerable<DS006> DoCreatePeriodicTelegrams()
        {
            var now = TimeProvider.Current.Now;
            var date = string.Format("{0:D2}{1:D2}{2:D1}", now.Day, now.Month, now.Year % 10);
            yield return new DS006 { Date = date };
        }
    }
}