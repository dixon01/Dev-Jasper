// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Telegram handler for DS021a that handles telegrams according to
    /// their <see cref="DS021AConfig"/>.
    /// </summary>
    public partial class DS021AHandler : DS021HandlerBase<DS021A>
    {
        private DS021AConfig config;

        private DateTime currentTime;

        private GenericUsageHandler absoluteTimeUsage;
        private GenericUsageHandler relativeTimeUsage;
        private GenericUsageHandler absoluteDestTimeUsage;
        private GenericUsageHandler relativeDestTimeUsage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021AHandler"/> class.
        /// </summary>
        public DS021AHandler()
        {
            this.InitConnections();
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="cfg">
        ///     The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig cfg, IIbisConfigContext configContext)
        {
            this.config = (DS021AConfig)cfg;
            this.FirstStopIndex = this.config.FirstStopIndexValue;
            this.AutoAddDestination = true;

            base.Configure(cfg, configContext);

            this.ConfigureConnections();
            this.ConfigureNewsticker();

            var dictionary = configContext.Dictionary;
            this.absoluteTimeUsage = new GenericUsageHandler(this.config.UsedForAbsoluteTime, dictionary);
            this.relativeTimeUsage = new GenericUsageHandler(this.config.UsedForRelativeTime, dictionary);
            this.absoluteDestTimeUsage =
                new GenericUsageHandler(this.config.UsedForDestinationAbsoluteTime, dictionary);
            this.relativeDestTimeUsage =
                new GenericUsageHandler(this.config.UsedForDestinationRelativeTime, dictionary);
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// Implementations of this method usually check the type of the event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public override bool Accept(Telegram telegram)
        {
            return (telegram is DS021A || telegram is DS010B) && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            var ds010B = telegram as DS010B;
            if (ds010B != null)
            {
                if (ds010B.StopIndex == this.config.DeleteRouteIndexValue)
                {
                    // the conditions to clear the stops are satisfied.
                    this.ClearStops();
                    return;
                }

                this.SetCurrentStopIndex(ds010B.StopIndex);

                return;
            }

            var ds021A = telegram as DS021A;
            if (ds021A != null)
            {
                this.HandleInput(ds021A);
            }
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(DS021A telegram)
        {
            if (telegram.StopData == null || telegram.StopData.Length < 2)
            {
                throw new ArgumentException("DS021a has to contain at least 2 items in its data");
            }

            int stopNameBlock = GetBlockIndex(this.config.UsedFor, 2);

            var indexString = telegram.StopData[1];
            int index;
            if (!ParserUtil.TryParse(indexString, out index))
            {
                if (indexString.Length > 0 &&
                    ParserUtil.TryParse(indexString.Substring(1), out index))
                {
                    var text = telegram.StopData.Length > 2 ? telegram.StopData[stopNameBlock] : string.Empty;
                    if (indexString[0] == 'A')
                    {
                        this.HandleConnectionTelegram(index, text);
                        return;
                    }

                    if (indexString[0] == 'S')
                    {
                        this.HandleMessageTelegram(index, text);
                        return;
                    }
                }

                this.Logger.Debug("Received non-integer stop index: {0}", indexString);
                return;
            }

            if (index > this.config.EndingStopValue)
            {
                this.Logger.Warn("Received invalid stop index: {0} > {1}", index, this.config.EndingStopValue);
                return;
            }

            if (index == this.config.EndingStopValue)
            {
                this.FlushStops(FlushReason.Last);
                return;
            }

            int transfersBlock = GetBlockIndex(this.config.UsedForTransfers, 3);
            int transferSymbolsBlock = GetBlockIndex(this.config.UsedForTransferSymbols, 4);
            int timeBlock = GetBlockIndex(this.config.UsedForRelativeTime, 5);

            var time = 0;
            if (telegram.StopData.Length > timeBlock)
            {
                ParserUtil.TryParse(telegram.StopData[timeBlock].Trim(), out time);
            }

            var stop = new DS021AStopInfo
                {
                    Index = index,
                    Name =
                        telegram.StopData.Length > stopNameBlock
                            ? telegram.StopData[stopNameBlock].TrimEnd(' ')
                            : string.Empty,
                    Transfers =
                        telegram.StopData.Length > transfersBlock
                            ? telegram.StopData[transfersBlock].TrimEnd(' ')
                            : string.Empty,
                    TransferSymbols =
                        telegram.StopData.Length > transferSymbolsBlock
                            ? telegram.StopData[transferSymbolsBlock].TrimEnd(' ')
                            : string.Empty,
                    Time = time
                };

            this.AddStop(stop);
        }

        /// <summary>
        /// Adds a StopInfo for an intermediate stop to a Ximple object.
        /// </summary>
        /// <param name="ximple">the Ximple object to fill</param>
        /// <param name="stop">the stop information</param>
        /// <param name="row">the row index in the Ximple object</param>
        protected override void AddStopCells(Ximple ximple, StopInfo stop, int row)
        {
            base.AddStopCells(ximple, stop, row);
            this.AddTimeCells(ximple, stop, this.absoluteTimeUsage, this.relativeTimeUsage, row);
        }

        /// <summary>
        /// The add destination time cells.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        protected override void AddDestinationTimeCells(Ximple ximple, StopInfo destination)
        {
            base.AddDestinationTimeCells(ximple, destination);
            this.AddTimeCells(ximple, destination, this.absoluteDestTimeUsage, this.relativeDestTimeUsage, 0);
        }

        private void AddTimeCells(
            Ximple ximple,
            StopInfo stopInfo,
            GenericUsageHandler absoluteUsage,
            GenericUsageHandler relativeUsage,
            int row)
        {
            var extended = stopInfo as DS021AStopInfo;
            if (extended == null)
            {
                // this is a StopInfo, probably StopInfo.Empty, so we have to add empty values for the times
                absoluteUsage.AddCell(ximple, string.Empty, row);
                relativeUsage.AddCell(ximple, string.Empty, row);
                return;
            }

            int stopTime = 0;

            for (int i = extended.Index; i >= this.CurrentStopIndex; i--)
            {
                StopInfo stop;
                if (this.TryGetStopInfo(i, out stop))
                {
                    var stopInformation = stop as DS021AStopInfo;
                    if (stopInformation != null)
                    {
                        stopTime += stopInformation.Time;
                    }
                }
            }

            // Absolute time
            var absoluteTime = this.currentTime.AddMinutes(stopTime).ToString(this.config.AbsoluteTimeFormat);

            // Relative time
            var relativeTime = stopTime.ToString(CultureInfo.InvariantCulture);
            absoluteUsage.AddCell(ximple, absoluteTime, row);
            relativeUsage.AddCell(ximple, relativeTime, row);
        }

        /// <summary>
        /// The DS021A stop info. This is made public only to be used by persistence.
        /// </summary>
        public class DS021AStopInfo : StopInfo
        {
            /// <summary>
            /// Gets or sets the time.
            /// </summary>
            public int Time { get; set; }
        }
    }
}
