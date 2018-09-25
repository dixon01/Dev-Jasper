// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Telegram handler for DS021c that handles telegrams according to
    /// their <see cref="DS021CConfig"/>.
    /// </summary>
    public class DS021CHandler : DS021HandlerBase<DS021C>
    {
        private DS021CConfig config;

        private bool hideNextStop;

        private GenericUsageHandler asciiLineNumberUsage;

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
            this.config = (DS021CConfig)cfg;
            this.FirstStopIndex = this.config.FirstStopIndexValue;
            this.AutoAddDestination = this.config.TakeDestinationFromLastStop;
            this.asciiLineNumberUsage = new GenericUsageHandler(
                this.config.AsciiLineNumberUsedFor, configContext.Dictionary);

            base.Configure(cfg, configContext);
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
            return (telegram is DS021C || telegram is DS010J) && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            var ds010 = telegram as DS010J;
            if (ds010 != null)
            {
                this.HandleTelegram(ds010);
                return;
            }

            var ds021B = telegram as DS021C;
            if (ds021B != null)
            {
                this.HandleInput(ds021B);
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
        protected override void HandleInput(DS021C telegram)
        {
            if (telegram.StopData == null || telegram.StopData.Length < 2)
            {
                throw new ArgumentException("DS021c has to contain at least 2 items in its data");
            }

            int status;
            if (!ParserUtil.TryParse(telegram.StopData[0], out status))
            {
                this.Logger.Debug("Received non-integer DS021c status: {0}", telegram.StopData[0]);
                return;
            }

            int index;
            if (!ParserUtil.TryParse(telegram.StopData[1], out index))
            {
                this.Logger.Warn("Received non-integer stop index: {0}", telegram.StopData[1]);
                return;
            }

            if (status == 0)
            {
                this.ClearStops();
            }

            if (!this.AddStop(index, telegram))
            {
                return;
            }

            if (status == 2)
            {
                this.FlushStops(FlushReason.Last);
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

            this.asciiLineNumberUsage.AddCell(ximple, string.Empty);
        }

        /// <summary>
        /// Adds a StopInfo for an intermediate stop to a Ximple object.
        /// </summary>
        /// <param name="ximple">
        /// the Ximple object to fill
        /// </param>
        /// <param name="stop">
        /// the stop information
        /// </param>
        /// <param name="row">
        /// the row index in the Ximple object
        /// </param>
        protected override void AddStopCells(Ximple ximple, StopInfo stop, int row)
        {
            if (row == 0 && this.hideNextStop)
            {
                // hide the current stop when we got status=3 (see DS010j)
                stop = StopInfo.Empty;
            }

            base.AddStopCells(ximple, stop, row);
        }

        private void HandleTelegram(DS010J telegram)
        {
            int oldIndex = this.CurrentStopIndex;
            bool oldHide = this.hideNextStop;
            this.hideNextStop = telegram.Status == 3;

            if (telegram.StopIndex == 0)
            {
                // the index 0 in the DS010J has the special meaning to
                // clear the stops in the perlschnur shown on the screen
                // but without clearing them from the protran's internal memory
                this.HideVisibleStops();
                return;
            }

            this.SetCurrentStopIndex(telegram.StopIndex);

            if (oldIndex == this.CurrentStopIndex && oldHide != this.hideNextStop)
            {
                // we changed the hide flag, but not the stop index, we need to force an index update
                this.FlushStops(FlushReason.IndexUpdate);
            }
        }

        private bool AddStop(int index, DS021C telegram)
        {
            if (index > 102)
            {
                this.Logger.Warn("Received invalid stop index: {0}", index);
                return false;
            }

            if (index == 100)
            {
                // we currently ignore "start stop"
                return true;
            }

            if (index == 102)
            {
                // 102: ASCII line number
                if (this.config.AsciiLineNumberUsedFor != null)
                {
                    var ximple = new Ximple();
                    int asciiLineNumberBlock = GetBlockIndex(this.config.AsciiLineNumberUsedFor, 2);
                    this.asciiLineNumberUsage.AddCell(ximple, telegram.StopData[asciiLineNumberBlock].TrimEnd(' '));
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }

                return true;
            }

            int stopNameBlock = GetBlockIndex(this.config.UsedFor, 2);
            int transfersBlock = GetBlockIndex(this.config.UsedForTransfers, 3);
            int transferSymbolsBlock = GetBlockIndex(this.config.UsedForTransferSymbols, 4);

            var stop = new StopInfo
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
                            : string.Empty
                };

            if (index == 101)
            {
                // 101: destination information
                if (this.config.TakeDestinationFromLastStop)
                {
                    // ignore record 101 if we take the destination from the last stop
                    return true;
                }

                var ximple = new Ximple();
                this.AddDestinationCells(ximple, stop);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }

                return true;
            }

            this.AddStop(stop);
            return true;
        }
    }
}
