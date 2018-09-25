// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS020Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS020Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    using NLog;

    /// <summary>
    /// Telegram provider for DS020.
    /// </summary>
    public class DS020Provider : TelegramProviderBase<DS020Config, DS020>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS020Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        public DS020Provider(DS020Config config, IIbisContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Handles the answer to a telegram.
        /// </summary>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <param name="telegram">
        /// The original telegram that was sent to get the answer.
        /// </param>
        /// <returns>
        /// True if the answer was recognized and expected and handled by this provider.
        /// </returns>
        protected override bool HandleAnswer(Telegram answer, DS020 telegram)
        {
            if (answer == null)
            {
                this.Logger.Warn("No DS120 received from {0}", telegram.IbisAddress);
                return true;
            }

            var ds120 = answer as DS120;
            if (ds120 == null)
            {
                return false;
            }

            this.Logger.Log(
                ds120.Status == 0 ? LogLevel.Debug : LogLevel.Info,
                "Received DS120 with status {0} from {1}",
                ds120.Status,
                telegram.IbisAddress);

            return true;
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS020> DoCreatePeriodicTelegrams()
        {
            foreach (var address in this.IbisConfig.Devices.GorbaTft.Addresses)
            {
                if (address > 0)
                {
                    yield return new DS020 { IbisAddress = address };
                }
            }
        }
    }
}