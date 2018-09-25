// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS084Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS084Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    using NLog;

    /// <summary>
    /// Telegram provider for DS084.
    /// </summary>
    public class DS084Provider : TelegramProviderBase<DS084Config, DS084>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS084Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS084Provider(DS084Config config, IIbisContext context)
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
        protected override IEnumerable<DS084> DoCreatePeriodicTelegrams()
        {
            var config = this.Context.Config.Devices.PassengerCountConfig;
            for (int i = 0; i < config.NbDoors; i++)
            {
                foreach (var address in config.Cells(i))
                {
                    yield return new DS084 { IbisAddress = address };
                }
            }
        }

        /// <summary>
        /// Handles the answer to a telegram.
        /// </summary>
        /// <param name="answer">
        /// The answer or null if no answer was received.
        /// </param>
        /// <param name="telegram">
        /// The original telegram that was sent to get the answer.
        /// </param>
        /// <returns>
        /// True if the answer was recognized and expected and handled by this provider.
        /// </returns>
        protected override bool HandleAnswer(Telegram answer, DS084 telegram)
        {
            if (answer == null)
            {
                this.Logger.Warn("No DS184 received from {0}", telegram.IbisAddress);
                return true;
            }

            var ds184 = answer as DS184;
            if (ds184 == null)
            {
                return false;
            }

            this.Logger.Log(
                ds184.Status == 0 ? LogLevel.Debug : LogLevel.Info,
                "Received DS184 with status {0} from {1}",
                ds184.Status,
                telegram.IbisAddress);

            return true;
        }
    }
}