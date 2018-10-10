// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021HandlerBase{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021HandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Base class for DS021a, DS021c and GO005 handling.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram that is handled by this class.
    /// </typeparam>
    public abstract class DS021HandlerBase<T> : TelegramHandler<T>, IManageableTable
        where T : Telegram
    {
        /// <summary>
        /// Common logger for all subclasses
        /// </summary>
        protected readonly Logger Logger;

        private readonly Dictionary<int, StopInfo> stops = new Dictionary<int, StopInfo>();

        private readonly ITimer flushTimer;

        private readonly Persistence persistence;

        private DS021ConfigBase config;

        private GenericUsageHandler nameUsage;
        private GenericUsageHandler transfersUsage;
        private GenericUsageHandler transferSymbolsUsage;

        private GenericUsageHandler destNameUsage;
        private GenericUsageHandler destTransfersUsage;
        private GenericUsageHandler destTransferSymbolsUsage;

        private bool receivedLast;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021HandlerBase{T}"/> class.
        /// </summary>
        protected DS021HandlerBase()
            : base(1)
        {
            this.receivedLast = true;
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.flushTimer = TimerFactory.Current.CreateTimer("DS021-Flush");
            this.flushTimer.Elapsed += this.FlushTimerElapsed;

            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.Save();
            var context = persistenceService.GetContext<Persistence>();
            if (context.Value != null && context.Valid)
            {
                this.persistence = context.Value;
            }
            else
            {
                this.persistence = new Persistence();
            }

            this.FirstStopIndex = 1;
        }

        /// <summary>
        /// Reason why <see cref="DS021HandlerBase{T}.FlushStops"/> is called.
        /// </summary>
        protected enum FlushReason
        {
            /// <summary>
            /// A certain number of stops has been added and therefore
            /// the currently known stops are sent to Infomedia.
            /// </summary>
            Intermediate,

            /// <summary>
            /// The last stop information was received, flush all
            /// stops to Infomedia.
            /// </summary>
            Last,

            /// <summary>
            /// Do not use from outside DS021HandlerBase.
            /// Flush happens because a timeout happened since we
            /// didn't receive any stop data in the configured time.
            /// </summary>
            Timeout,

            /// <summary>
            /// Do not use from outside DS021HandlerBase.
            /// Flush happens because <see cref="DS021HandlerBase{T}.SetCurrentStopIndex"/>
            /// was called.
            /// </summary>
            IndexUpdate
        }

        /// <summary>
        /// Gets or sets the index of the first stop.
        /// This can be 0 or 1.
        /// </summary>
        protected int FirstStopIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add the destination
        /// when the last stop was received. Set this to true for DS021a
        /// and to false depending on configuration for DS021c.
        /// </summary>
        protected bool AutoAddDestination { get; set; }

        /// <summary>
        /// Gets the index of the currently shown "first" stop.
        /// </summary>
        protected int CurrentStopIndex
        {
            get
            {
                return this.persistence.CurrentStopIndex;
            }

            private set
            {
                this.persistence.CurrentStopIndex = value;
            }
        }

        /// <summary>
        /// Gets the complete StopInfo for all received stops.
        /// </summary>
        protected Dictionary<int, StopInfo> Stops
        {
            get
            {
                return this.stops;
            }
        }

        private int LastStopIndex
        {
            get
            {
                return this.persistence.LastStopIndex;
            }

            set
            {
                this.persistence.LastStopIndex = value;
            }
        }

        private bool HasMissingData
        {
            get
            {
                return this.persistence.HasMissingData;
            }

            set
            {
                this.persistence.HasMissingData = value;
            }
        }

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
            this.config = (DS021ConfigBase)cfg;
            this.CurrentStopIndex = this.FirstStopIndex;
            base.Configure(cfg, configContext);

            var dictionary = configContext.Dictionary;
            this.nameUsage = new GenericUsageHandler(this.config.UsedFor, dictionary);
            this.transfersUsage = new GenericUsageHandler(this.config.UsedForTransfers, dictionary);
            this.transferSymbolsUsage = new GenericUsageHandler(this.config.UsedForTransferSymbols, dictionary);

            this.destNameUsage = new GenericUsageHandler(this.config.UsedForDestination, dictionary);
            this.destTransfersUsage = new GenericUsageHandler(this.config.UsedForDestinationTransfers, dictionary);
            this.destTransferSymbolsUsage = new GenericUsageHandler(
                this.config.UsedForDestinationTransferSymbols, dictionary);

            this.Restore();
        }

        /// <summary>
        /// To start the handler and check for persistence. Update status of handler
        /// based on persistence.
        /// </summary>
        public override void StartCheck()
        {
            if (this.persistence.Stops == null || this.persistence.Stops.Count == 0)
            {
                this.Status = Status.NoData;
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var stop in this.Stops.Values)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<int>("Stop Index", stop.Index, true),
                                     new ManagementProperty<string>("Stop Name", stop.Name, true),
                                     new ManagementProperty<string>("Stop Transfers", stop.Transfers, true),
                                     new ManagementProperty<string>(
                                         "Stop Transfer Symbols", stop.TransferSymbols, true),
                                 };
            }
        }

        /// <summary>
        /// Gets the block index for a given generic usage.
        /// </summary>
        /// <param name="genericUsage">
        /// The generic usage.
        /// </param>
        /// <param name="defaultValue">
        /// The default value to use if no block index was defined in
        /// the <see cref="genericUsage"/> or it is null.
        /// </param>
        /// <returns>
        /// The block index.
        /// </returns>
        protected static int GetBlockIndex(GenericUsage genericUsage, int defaultValue)
        {
            var usage = genericUsage as GenericUsageDS021Base;
            if (usage == null || usage.FromBlock < 0)
            {
                return defaultValue;
            }

            // offset by 2 since the first two items are:
            // [0] telegram header
            // [1] stop index
            return usage.FromBlock + 2;
        }

        /// <summary>
        /// Clears the stops currently visible in the stop list but
        /// without clear them also from the Protran internal memory.
        /// </summary>
        protected void HideVisibleStops()
        {
            var ximple = new Ximple();
            lock (this.stops)
            {
                this.HideStops(ximple);
            }

            if (ximple.Cells.Count > 0)
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }

            this.CurrentStopIndex = 0;
        }

        /// <summary>
        /// Sets the current stop index to the given value and sends
        /// the necessary Ximple.
        /// </summary>
        /// <param name="index">
        /// The index (has to be greater than or equal to <see cref="FirstStopIndex"/>).
        /// </param>
        protected void SetCurrentStopIndex(int index)
        {
            if (this.CurrentStopIndex == index)
            {
                return;
            }

            if (index < this.FirstStopIndex)
            {
                // received a stop with an index smaller that the first stop.
                this.Logger.Debug("Received invalid stop index: {0} < {1}", index, this.FirstStopIndex);
                return;
            }

            if (index > this.LastStopIndex)
            {
                // received a stop with an index bigger than the last one
                // (so, there is an "hole" in the stops).
                this.Logger.Debug("Received invalid stop index: {0} > {1}", index, this.LastStopIndex);
                return;
            }

            this.CurrentStopIndex = index;

            int lastStopToShow = index - this.FirstStopIndex + this.config.FlushNumberOfStations;
            if (this.receivedLast || this.stops.ContainsKey(lastStopToShow))
            {
                // ok, all the indexes are now complete
                this.FlushStops(FlushReason.IndexUpdate);
            }
        }

        /// <summary>
        /// Adds a new stop to the list of stops.
        /// </summary>
        /// <param name="stop">the information about the stop</param>
        protected void AddStop(StopInfo stop)
        {
            int index = stop.Index;
            if (index < this.FirstStopIndex)
            {
                this.Logger.Warn("Received invalid stop index: {0} < {1}", index, this.FirstStopIndex);
                return;
            }

            if (this.receivedLast && index != this.FirstStopIndex)
            {
                this.Status = Status.NoData;
                this.Logger.Warn("Received invalid first stop index: {0}", index);
                return;
            }

            this.flushTimer.Enabled = false;
            this.Logger.Trace("Adding stop: '{0}' at {1}", stop.Name, index);

            lock (this.stops)
            {
                if (this.receivedLast)
                {
                    this.ClearStops();
                }

                this.LastStopIndex = Math.Max(this.LastStopIndex, index);
                this.stops[index] = stop;

                this.CheckForMissingData();

                int stopsToShow = this.stops.Count - this.CurrentStopIndex + this.FirstStopIndex;
                if (stopsToShow % this.config.FlushNumberOfStations == 0)
                {
                    this.FlushStops(FlushReason.Intermediate);
                }

                if (this.config.FlushTimeout > TimeSpan.Zero)
                {
                    this.flushTimer.AutoReset = false;
                    this.flushTimer.Interval = this.config.FlushTimeout;
                    this.flushTimer.Enabled = true;
                }
            }

            this.Status = this.HasMissingData ? Status.MissingData : Status.Ok;
        }

        /// <summary>
        /// Flushes all currently collected stops via Ximple.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        protected virtual void FlushStops(FlushReason reason)
        {
            this.Logger.Debug("FlushStops({0})", reason);

            var ximple = new Ximple();
            lock (this.stops)
            {
                for (int i = this.FirstStopIndex; i <= this.LastStopIndex; i++)
                {
                    StopInfo stop;
                    int index = i - this.CurrentStopIndex;
                    if (index < 0)
                    {
                        this.AddStopCells(ximple, StopInfo.Empty, index + this.LastStopIndex);
                        if (this.config.ShowPastStops && this.TryGetStopInfo(i, out stop))
                        {
                            this.AddStopCells(ximple, stop, index);
                        }
                    }
                    else if (this.TryGetStopInfo(i, out stop) && !this.IsHiddenLastStop(i, reason))
                    {
                        this.AddStopCells(ximple, stop, index);
                    }
                    else
                    {
                        this.AddStopCells(ximple, StopInfo.Empty, index);
                    }
                }

                if (this.config.ShowPastStops)
                {
                    for (int i = this.FirstStopIndex - this.LastStopIndex;
                         i < this.FirstStopIndex - this.CurrentStopIndex;
                         i++)
                    {
                        this.AddStopCells(ximple, StopInfo.Empty, i);
                    }
                }

                this.AddDestinationCells(ximple, reason);
            }

            if (ximple.Cells.Count > 0)
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }
        }

        /// <summary>
        /// Tries to get the stop info for the given stop index.
        /// </summary>
        /// <param name="index">
        /// The stop index.
        /// </param>
        /// <param name="stop">
        /// The stop information.
        /// </param>
        /// <returns>
        /// true if the stop was found.
        /// </returns>
        protected bool TryGetStopInfo(int index, out StopInfo stop)
        {
            return this.stops.TryGetValue(index, out stop);
        }

        /// <summary>
        /// Adds a StopInfo for an intermediate stop to a Ximple object.
        /// </summary>
        /// <param name="ximple">the Ximple object to fill</param>
        /// <param name="stop">the stop information</param>
        /// <param name="row">the row index in the Ximple object</param>
        protected virtual void AddStopCells(Ximple ximple, StopInfo stop, int row)
        {
            this.nameUsage.AddCell(ximple, stop.Name, row);
            this.transfersUsage.AddCell(ximple, stop.Transfers, row);
            this.transferSymbolsUsage.AddCell(ximple, stop.TransferSymbols, row);
        }

        /// <summary>
        /// Adds a StopInfo for the destination to a Ximple object.
        /// </summary>
        /// <param name="ximple">The Ximple object to fill.</param>
        /// <param name="destination">The destination the stop information.</param>
        protected virtual void AddDestinationCells(Ximple ximple, StopInfo destination)
        {
            int stopsLeft = this.LastStopIndex - this.CurrentStopIndex + 1;
            if (this.config.HideDestinationBelow > 0 && stopsLeft < this.config.HideDestinationBelow)
            {
                // hide the destination if HideDestinationBelow is configured and we have less stops left
                destination = StopInfo.Empty;
            }

            this.destNameUsage.AddCell(ximple, destination.Name);
            this.destTransfersUsage.AddCell(ximple, destination.Transfers);
            this.destTransferSymbolsUsage.AddCell(ximple, destination.TransferSymbols);

            this.AddDestinationTimeCells(ximple, destination);
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
        protected virtual void AddDestinationTimeCells(Ximple ximple, StopInfo destination)
        {
        }

        /// <summary>
        /// Sends a XIMPLE with empty cells in order to clear the stops visible on the
        /// stop list, but without deleting the stops from Protran internal memory.
        /// </summary>
        protected void ClearStops()
        {
            var ximple = new Ximple();
            this.ClearStops(ximple);
            if (ximple.Cells.Count > 0)
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }

            this.Status = Status.NoData;
        }

        /// <summary>
        /// Clears all stop information and fill all empty cells into the given ximple object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple to fill.
        /// </param>
        protected virtual void ClearStops(Ximple ximple)
        {
            lock (this.stops)
            {
                this.HideStops(ximple);

                this.LastStopIndex = -1;

                if (this.stops.Count > 0)
                {
                    this.AddDestinationCells(ximple, StopInfo.Empty);
                }

                this.stops.Clear();
            }

            this.CurrentStopIndex = this.FirstStopIndex;

            this.receivedLast = false;
        }

        private void HideStops(Ximple ximple)
        {
            var start = this.config.ShowPastStops ? this.FirstStopIndex - this.LastStopIndex : 0;
            var end = this.LastStopIndex - this.FirstStopIndex;
            for (int i = start; i <= end; i++)
            {
                this.AddStopCells(ximple, StopInfo.Empty, i);
            }
        }

        private void AddDestinationCells(Ximple ximple, FlushReason reason)
        {
            if (reason == FlushReason.Intermediate)
            {
                return;
            }

            StopInfo destination;
            bool destinationCellsAdded = false;

            // send the destination if we are not just updating the index
            if (reason != FlushReason.IndexUpdate || this.config.HideDestinationBelow > 0)
            {
                if (reason != FlushReason.IndexUpdate)
                {
                    this.flushTimer.Enabled = false;
                    this.receivedLast = true;

                    this.Status = this.HasMissingData ? Status.MissingData : Status.Ok;
                }

                if (this.AutoAddDestination)
                {
                    if (!this.TryGetStopInfo(this.LastStopIndex, out destination))
                    {
                        // I've to hide the destination.
                        // To do it, I insert empty cells in the XIMPLE.
                        destination = StopInfo.Empty;
                    }

                    this.AddDestinationCells(ximple, destination);
                    destinationCellsAdded = true;
                }
            }

            if (!destinationCellsAdded && reason == FlushReason.IndexUpdate)
            {
                if (!this.TryGetStopInfo(this.LastStopIndex, out destination))
                {
                    // I've to hide the destination.
                    // To do it, I insert empty cells in the XIMPLE.
                    destination = StopInfo.Empty;
                }

                this.AddDestinationTimeCells(ximple, destination);
            }
        }

        private bool IsHiddenLastStop(int index, FlushReason reason)
        {
            if (!this.receivedLast && reason != FlushReason.Last && reason != FlushReason.Timeout)
            {
                // we only hide it when we received the last update
                return false;
            }

            return index == this.LastStopIndex && this.config.HideLastStop;
        }

        private void CheckForMissingData()
        {
            for (int i = this.FirstStopIndex; i <= this.LastStopIndex; i++)
            {
                if (!this.stops.ContainsKey(i))
                {
                    this.HasMissingData = true;
                    return;
                }
            }

            this.HasMissingData = false;
        }

        private void FlushTimerElapsed(object sender, EventArgs e)
        {
            try
            {
                this.FlushStops(FlushReason.Timeout);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex,"Flush timer caused exception.");
            }
        }

        private void Save()
        {
            this.persistence.Stops = new List<StopInfo>(this.stops.Values);

            var context = ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<Persistence>();
            context.Value = this.persistence;
        }

        private void Restore()
        {
            if (this.persistence.Stops == null)
            {
                return;
            }

            this.stops.Clear();

            foreach (var stop in this.persistence.Stops)
            {
                this.stops.Add(stop.Index, stop);
            }
        }

        /// <summary>
        /// Do not use this class outside the outer class. It is only public for XML serialization.
        /// </summary>
        public class Persistence
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Persistence"/> class.
            /// </summary>
            public Persistence()
            {
                this.LastStopIndex = -1;
            }

            /// <summary>
            /// Gets or sets the list of stops.
            /// </summary>
            public List<StopInfo> Stops { get; set; }

            /// <summary>
            /// Gets or sets the index of the currently shown "first" stop.
            /// </summary>
            public int CurrentStopIndex { get; set; }

            /// <summary>
            /// Gets or sets the index of the last stop in the stop list.
            /// </summary>
            public int LastStopIndex { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the stop list contains "holes".
            /// </summary>
            public bool HasMissingData { get; set; }
        }

        /// <summary>
        /// Information about a stop in the list of next stops.
        /// </summary>
        [XmlInclude(typeof(GO005Handler.GO005StopInfo))]
        [XmlInclude(typeof(DS021AHandler.DS021AStopInfo))]
        public class StopInfo
        {
            // ReSharper disable StaticFieldInGenericType

            /// <summary>
            /// Empty stop info used to clear data.
            /// </summary>
            [XmlIgnore]
            public static readonly StopInfo Empty = new StopInfo
                                                        {
                                                            Name = string.Empty,
                                                            Transfers = string.Empty,
                                                            TransferSymbols = string.Empty
                                                        };
            // ReSharper restore StaticFieldInGenericType

            /// <summary>
            /// Gets or sets the index of this stop in the stop list.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the name of the stop.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the transfer information of the stop.
            /// </summary>
            public string Transfers { get; set; }

            /// <summary>
            /// Gets or sets the transfer symbols information of the stop.
            /// </summary>
            public string TransferSymbols { get; set; }
        }
    }
}