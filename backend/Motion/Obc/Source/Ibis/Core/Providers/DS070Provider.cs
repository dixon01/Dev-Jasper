// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS070Provider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS070Provider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Telegram provider for DS070.
    /// </summary>
    public class DS070Provider : TelegramProviderBase<DS070Config, DS070>
    {
        private readonly List<int> addresses;

        private readonly Dictionary<int, int> errorCounters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS070Provider"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DS070Provider(DS070Config config, IIbisContext context)
            : base(config, context)
        {
            this.addresses = context.Config.Devices.TicketingConfig.Addresses.FindAll(a => a > 0);
            this.errorCounters = new Dictionary<int, int>();
            foreach (var address in this.addresses)
            {
                this.errorCounters[address] = 0;
            }
        }

        private TicketingModel TicketingModel
        {
            get
            {
                return this.Context.Config.Devices.TicketingConfig.Model;
            }
        }

        /// <summary>
        /// Creates the periodic telegrams.
        /// This method is only called when telegrams have to be really created.
        /// </summary>
        /// <returns>
        /// The telegrams or an empty list if currently no telegram is to be sent.
        /// </returns>
        protected override IEnumerable<DS070> DoCreatePeriodicTelegrams()
        {
            foreach (var address in this.addresses)
            {
                yield return new DS070 { IbisAddress = address };
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
        protected override bool HandleAnswer(Telegram answer, DS070 telegram)
        {
            var ds170 = answer as DS170;
            if (answer != null && ds170 == null)
            {
                return false;
            }

            if (this.TicketingModel == TicketingModel.None)
            {
                return true;
            }

            if (ds170 != null && ds170.Status == 0)
            {
                this.Logger.Trace("Received status 0 from {0}", telegram.IbisAddress);
                this.errorCounters[telegram.IbisAddress] = 0;
                return true;
            }

            this.errorCounters[telegram.IbisAddress]++;

            if (this.errorCounters[telegram.IbisAddress] != this.Config.Threshold)
            {
                this.Logger.Debug(
                    "Address {0} had {1} consecutive error(s) (status={2})",
                    telegram.IbisAddress,
                    this.errorCounters[telegram.IbisAddress],
                    ds170 == null ? -1 : ds170.Status);
                return true;
            }

            var index = this.addresses.IndexOf(telegram.IbisAddress);
            if (ds170 == null)
            {
                this.HandleNoAnswer(index);
            }
            else
            {
                this.HandleReturnValue(index, ds170.Status);
            }

            return true;
        }

        private void HandleReturnValue(int index, int status)
        {
            var errorToReport = evMessage.Messages.Undef;
            switch (status)
            {
                case 1:
                    // ticket error
                    if (index == 0)
                    {
                        errorToReport = evMessage.Messages.FailureTicketCanceler1;
                    }
                    else if (index == 1)
                    {
                        errorToReport = evMessage.Messages.FailureTicketCanceler2;
                    }

                    break;

                case 2:
                    if (index == 0)
                    {
                        errorToReport = evMessage.Messages.WarningPaper1;
                    }
                    else if (index == 1)
                    {
                        errorToReport = evMessage.Messages.WarningPaper2;
                    }

                    break;

                case 3:
                    if (index == 0)
                    {
                        errorToReport = evMessage.Messages.WarningCashBox1;
                    }
                    else if (index == 1)
                    {
                        errorToReport = evMessage.Messages.WarningCashBox2;
                    }

                    break;
            }

            this.SendMessage(errorToReport);
        }

        private void HandleNoAnswer(int index)
        {
            var errorToReport = evMessage.Messages.Undef;

            if (this.TicketingModel == TicketingModel.Krauth)
            {
                switch (index)
                {
                    case 0:
                        errorToReport = evMessage.Messages.FailureTicketing_Krauth_1;
                        break;
                    case 1:
                        errorToReport = evMessage.Messages.FailureTicketing_Krauth_2;
                        break;
                    default:
                        this.Logger.Error("Invalid ticket unit number: " + index);
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        errorToReport = evMessage.Messages.FailureTicketing_Atron_1;
                        break;
                    case 1:
                        errorToReport = evMessage.Messages.FailureTicketing_Atron_2;
                        break;
                    case 2:
                        errorToReport = evMessage.Messages.FailureTicketing_Atron_3;
                        break;
                    default:
                        this.Logger.Error("Invalid ticket unit number: " + index);
                        break;
                }
            }

            this.SendMessage(errorToReport);
        }

        private void SendMessage(evMessage.Messages message)
        {
            if (message == evMessage.Messages.Undef)
            {
                return;
            }

            this.Logger.Info("Sending message to driver: {0}", message);
            MessageDispatcher.Instance.Broadcast(
                new evMessage(
                    0,
                    evMessage.Types.Predefined,
                    evMessage.SubTypes.Error,
                    message,
                    string.Empty,
                    evMessage.Destinations.Driver));
        }
    }
}
