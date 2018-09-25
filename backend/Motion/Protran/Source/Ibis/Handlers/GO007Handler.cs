// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO007Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO007Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Handler for the GO007 stop list telegram.
    /// </summary>
    public class GO007Handler : TelegramHandler<GO007>, IManageableTable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<GO007Handler>();

        private readonly List<StopInfo> stops = new List<StopInfo>();

        private GO007Config config;

        private GenericUsageHandler lineNumberUsage;

        private GenericUsageHandler nameUsage;

        private GenericUsageHandler transfersUsage;

        private GenericUsageHandler destNameUsage;

        private GenericUsageHandler destTransfersUsage;

        private List<StopInfo> showStopsList;
        private List<StopInfo> pastStopsList;

        private int lastStopCount;
        private int lastPastStopCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO007Handler"/> class.
        /// </summary>
        public GO007Handler()
            : base(10)
        {
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.Save();
            var context = persistenceService.GetContext<List<StopInfo>>();
            if (context.Value != null && context.Valid)
            {
                this.stops = context.Value;
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
            this.config = (GO007Config)cfg;
            var dictionary = configContext.Dictionary;

            this.lineNumberUsage = new GenericUsageHandler(this.config.UsedForLineNumber, configContext.Dictionary);
            this.nameUsage = new GenericUsageHandler(this.config.UsedFor, dictionary);
            this.transfersUsage = new GenericUsageHandler(this.config.UsedForTransfers, dictionary);

            this.destNameUsage = new GenericUsageHandler(this.config.UsedForDestination, dictionary);
            this.destTransfersUsage = new GenericUsageHandler(this.config.UsedForDestinationTransfers, dictionary);
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
            return (telegram is GO007 || telegram is DS009) && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            var go007 = telegram as GO007;
            if (go007 != null)
            {
                this.HandleInput(go007);
                return;
            }

            var ds009 = telegram as DS009;
            if (ds009 != null)
            {
                this.HandleTelegram(ds009);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var stop in this.stops)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<string>("Stop Name", stop.Name, true),
                                     new ManagementProperty<string>("Stop Transfers", stop.Transfers, true),
                                 };
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
        protected override void HandleInput(GO007 telegram)
        {
            if (telegram.StopData == null || telegram.StopData.Length < 2)
            {
                throw new ArgumentException("GO007 has to contain at least 2 items in its data");
            }

            this.stops.Clear();

            var ximple = new Ximple();
            var lineNumber = telegram.StopData[1].Trim(' ');
            this.lineNumberUsage.AddCell(ximple, lineNumber);
            if (ximple.Cells.Count > 0)
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }

            for (int i = 2; i < telegram.StopData.Length; i++)
            {
                var stopData = telegram.StopData[i].Split(new[] { '\u0005' }, 2);
                var transferData = stopData.Length > 1 ? stopData[1] : string.Empty;

                var stop = new StopInfo
                               {
                                   Name = stopData[0],
                                   Transfers = transferData,
                               };

                this.stops.Add(stop);
            }
        }

        private void HandleTelegram(DS009 telegram)
        {
            var stopName = telegram.StopName;
            var index = this.stops.FindIndex(i => i.Name.Equals(stopName));
            if (index < 0)
            {
                Logger.Warn("Got stop name that is not in list: '{0}'", stopName);
                return;
            }

            this.showStopsList = this.stops.GetRange(index, this.stops.Count - index);

            var ximple = new Ximple();
            var row = 0;
            foreach (var stopInfo in this.showStopsList)
            {
                if (row == this.showStopsList.Count - 1)
                {
                    this.AddStopCells(ximple, this.config.HideLastStop ? StopInfo.Empty : stopInfo, row);

                    this.AddDestinationCells(ximple, stopInfo);
                }
                else
                {
                    this.AddStopCells(ximple, stopInfo, row);
                }

                row++;
            }

            while (row < this.lastStopCount)
            {
                this.AddStopCells(ximple, StopInfo.Empty, row);
                row++;
            }

            this.lastStopCount = this.showStopsList.Count;
            if (this.config.ShowPastStops)
            {
                this.pastStopsList = this.stops.GetRange(0, index);
                row = -1;
                for (int i = this.pastStopsList.Count - 1; i >= 0; i--)
                {
                    this.AddStopCells(ximple, this.pastStopsList[i], row);
                    row--;
                }

                while (row > -this.lastPastStopCount)
                {
                    this.AddStopCells(ximple, StopInfo.Empty, row);
                    row--;
                }

                this.lastPastStopCount = this.pastStopsList.Count;
            }

            if (ximple.Cells.Count == 0)
            {
                return;
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddStopCells(Ximple ximple, StopInfo stop, int row)
        {
            this.nameUsage.AddCell(ximple, stop.Name, row);
            this.transfersUsage.AddCell(ximple, stop.Transfers, row);
        }

        private void AddDestinationCells(Ximple ximple, StopInfo destination)
        {
            var stopsList = this.showStopsList;
            if (stopsList != null && (this.config.HideDestinationBelow > 0 &&
                stopsList.Count < this.config.HideDestinationBelow))
            {
                destination = StopInfo.Empty;
            }

            this.destNameUsage.AddCell(ximple, destination.Name);
            this.destTransfersUsage.AddCell(ximple, destination.Transfers);
        }

        private void Save()
        {
            var context = ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<List<StopInfo>>();
            context.Value = new List<StopInfo>(this.stops);
        }

        /// <summary>
        /// The stop info.
        /// </summary>
        public class StopInfo
        {
            /// <summary>
            /// Empty stop info used to clear data.
            /// </summary>
            [XmlIgnore]
            public static readonly StopInfo Empty = new StopInfo
            {
                Name = string.Empty,
                Transfers = string.Empty,
            };

            /// <summary>
            /// Gets or sets the name of the stop.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the transfer information of the stop.
            /// </summary>
            public string Transfers { get; set; }
        }
    }
}
