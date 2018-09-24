// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO005Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Handler fro the GO005 stop list telegram (an extension of the "normal" DS021 used by STOAG).
    /// </summary>
    public class GO005Handler : DS021HandlerBase<GO005>
    {
        private readonly List<GO005StopInfo> nextRouteCache = new List<GO005StopInfo>();

        private GO005Config config;

        private GenericUsageHandler asciiLineNumberUsage;

        private bool hideNextStop;

        private bool hasRouteLoaded;

        private int previousDS010IndexReceived;
        private int previousDS002RunValueReceived;

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="cfg">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig cfg, IIbisConfigContext configContext)
        {
            this.config = (GO005Config)cfg;
            this.FirstStopIndex = 1;
            this.AutoAddDestination = true;

            // I initialize the variables useful for the route deletion concept.
            this.previousDS010IndexReceived = -1;
            this.previousDS002RunValueReceived = -1;

            this.asciiLineNumberUsage = new GenericUsageHandler(
                this.config.AsciiLineNumberUsedFor, configContext.Dictionary);

            base.Configure(cfg, configContext);
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if this class can handle the given event.
        /// </returns>
        public override bool Accept(Telegram telegram)
        {
            return (telegram is GO005 || telegram is DS010 || telegram is DS002) && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            var ds010 = telegram as DS010;
            if (ds010 != null)
            {
                this.HandleTelegram(ds010);

                // finished with the DS010. I can return.
                return;
            }

            var tlgGo005 = telegram as GO005;
            if (tlgGo005 != null)
            {
                this.HandleInput(tlgGo005);

                // finished with the GO005. I can return.
                return;
            }

            var ds002 = telegram as DS002;
            if (ds002 != null)
            {
                this.HandleTelegram(ds002);
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
        protected override void HandleInput(GO005 telegram)
        {
            if (telegram.StopData == null || telegram.StopData.Length < 2)
            {
                throw new ArgumentException("GO005 has to contain at least 2 items in its data");
            }

            if (telegram.StopData[1].Length < 8)
            {
                throw new ArgumentException("The 2nd part of GO005 has to contain at least 8 chars");
            }

            var lineNumber = telegram.StopData[1].Substring(0, 4).TrimEnd(' ');

            int index;
            if (!ParserUtil.TryParse(telegram.StopData[1].Substring(4, 4), out index))
            {
                this.Logger.Warn("Received non-integer stop index in {0}", telegram.StopData[1]);
                return;
            }

            // enable buffering when we have a route and get a new one
            bool bufferStop = this.hasRouteLoaded && this.config.BufferNextRoute;

            if (telegram.StopData.Length == 2)
            {
                if (!bufferStop)
                {
                    this.FlushStops(FlushReason.Last);
                }
                else
                {
                    this.nextRouteCache.Add(GO005StopInfo.FlushCache);
                }

                return;
            }

            if (telegram.StopData[1].Length != 8)
            {
                throw new ArgumentException("The 2nd part of GO005 has to contain exactly 8 chars");
            }

            int stopNameBlock = GetBlockIndex(this.config.UsedFor, 2);
            int transfersBlock = GetBlockIndex(this.config.UsedForTransfers, 3);
            int transferSymbolsBlock = GetBlockIndex(this.config.UsedForTransferSymbols, 4);

            var stop = new GO005StopInfo
                {
                    Index = index,
                    LineNumber = lineNumber,
                    Name = telegram.StopData[stopNameBlock].TrimEnd(' ', '\n'),
                    Transfers =
                        telegram.StopData.Length > transfersBlock
                            ? telegram.StopData[transfersBlock].TrimEnd(' ', '\n')
                            : string.Empty,
                    TransferSymbols =
                        telegram.StopData.Length > transferSymbolsBlock
                            ? telegram.StopData[transferSymbolsBlock].TrimEnd(' ', '\n')
                            : string.Empty
                };

            if (!bufferStop)
            {
                this.AddStop(stop);
            }
            else
            {
                if (index == 1)
                {
                    this.nextRouteCache.Clear();
                }

                this.Logger.Trace("Buffering stop: {0}", stop.Name);
                this.nextRouteCache.Add(stop);
            }
        }

        /// <summary>
        /// Adds a StopInfo for an intermediate stop to a Ximple object.
        /// </summary>
        /// <param name="ximple">the Ximple object to fill</param>
        /// <param name="stop">the stop information</param>
        /// <param name="row">the row index in the Ximple object</param>
        protected override void AddStopCells(Ximple ximple, StopInfo stop, int row)
        {
            if (row == 0)
            {
                // take the ASCII Line Number from the current stop
                var stopInfo = stop as GO005StopInfo;
                this.asciiLineNumberUsage.AddCell(ximple, stopInfo != null ? stopInfo.LineNumber : string.Empty);

                if (this.hideNextStop)
                {
                    // hide the current stop when we got the 999 index (see DS010)
                    stop = StopInfo.Empty;
                }
            }

            base.AddStopCells(ximple, stop, row);
        }

        /// <summary>
        /// Flushes all currently collected stops via Ximple.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        protected override void FlushStops(FlushReason reason)
        {
            base.FlushStops(reason);

            if (reason == FlushReason.Last || reason == FlushReason.Timeout)
            {
                // we have loaded a whole route
                this.hasRouteLoaded = true;
            }
        }

        /// <summary>
        /// Clears all stop information and fill all empty cells into the given ximple object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple to fill.
        /// </param>
        protected override void ClearStops(Ximple ximple)
        {
            base.ClearStops(ximple);

            // reset the flag, no route is currently loaded
            this.hasRouteLoaded = false;
        }

        private void HandleTelegram(DS010 telegram)
        {
            int index;
            if (!ParserUtil.TryParse(telegram.StopIndex, out index))
            {
                return;
            }

            if (this.config.DeleteRoute && this.previousDS002RunValueReceived == 0 && index == 0)
            {
                this.ClearStops();

                // now I can reset the variables for the route deletion concept.
                this.previousDS010IndexReceived = -1;
                this.previousDS002RunValueReceived = -1;
                return;
            }

            this.previousDS010IndexReceived = index;

            if (index == this.config.HideNextStopForIndex)
            {
                this.hideNextStop = true;
                this.FlushStops(FlushReason.IndexUpdate);
                return;
            }

            if (this.hideNextStop)
            {
                // reset the hide flag
                this.hideNextStop = false;
                if (index == this.CurrentStopIndex)
                {
                    // setting the stop index wouldn't work
                    // below since the index is still the same
                    this.FlushStops(FlushReason.IndexUpdate);
                    return;
                }
            }

            if ((index < this.CurrentStopIndex) && (index != 0))
            {
                this.FlushNextRouteCache();
            }

            this.SetCurrentStopIndex(index);
        }

        private void HandleTelegram(DS002 telegram)
        {
            if (!this.config.DeleteRoute)
            {
                // this feature is disabled.
                // I don't have to do nothing with the incoming DS002 telegram.
                return;
            }

            // the feature is enabled.
            this.previousDS002RunValueReceived = telegram.RunNumber;

            // now, to delete the route, I've to take care about the
            // route index previously received with the DS010 and the current run number
            if (this.previousDS010IndexReceived == 0 && telegram.RunNumber == 0)
            {
                // the last DS010 index is zero and also the run number is zero.
                // so, now I've to clear the route information.
                this.ClearStops();

                // now I can reset the variables for the route deletion concept.
                this.previousDS010IndexReceived = -1;
                this.previousDS002RunValueReceived = -1;
            }
        }

        /// <summary>
        /// Flushes the buffered stops if BufferNextRoute is enabled.
        /// </summary>
        private void FlushNextRouteCache()
        {
            if (this.nextRouteCache.Count == 0)
            {
                return;
            }

            foreach (var stop in this.nextRouteCache)
            {
                if (stop == GO005StopInfo.FlushCache)
                {
                    // we added a special stop to mark the end of the list
                    this.FlushStops(FlushReason.Last);
                    break;
                }

                this.AddStop(stop);
            }

            this.nextRouteCache.Clear();
        }

        /// <summary>
        /// Stop information with the additional line number that is
        /// available in GO005. This is made public only to be used by persistence.
        /// </summary>
        public class GO005StopInfo : StopInfo
        {
            /// <summary>
            /// Special marker stop to mark the end of a cached stop list.
            /// </summary>
            public static readonly GO005StopInfo FlushCache = new GO005StopInfo();

            /// <summary>
            /// Gets or sets the (max) 4 character line number.
            /// </summary>
            public string LineNumber { get; set; }
        }
    }
}
