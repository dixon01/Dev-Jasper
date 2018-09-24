// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    using MFD.MFDCustomerService;

    using NLog;

    /// <summary>
    /// Class that manages the transfer information and creates Ximple for it.
    /// </summary>
    internal class TransferManager : DataManagerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, StopTransferInformation> transferInformation =
            new Dictionary<int, StopTransferInformation>();

        private readonly DataItemHandler stopNumberHandler;
        private readonly TimeItemHandler plannedDepartureTimeHandler;
        private readonly TimeItemHandler calculatedDepartureTimeHandler;
        private readonly DataItemHandler routeNumberHandler;
        private readonly DataItemHandler destinationTextHandler;
        private readonly DataItemHandler trackTextHandler;

        private int currentStopIndex;

        private int lastSentConnectionsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="handlerFactory">
        /// The data item handler Factory.
        /// </param>
        public TransferManager(TransferInformationConfig config, ItemHandlerFactory handlerFactory)
        {
            this.stopNumberHandler = handlerFactory.CreateHandler(config.StopNumber);
            this.plannedDepartureTimeHandler = handlerFactory.CreateHandler(config.PlannedDepartureTime);
            this.calculatedDepartureTimeHandler = handlerFactory.CreateHandler(config.CalculatedDepartureTime);
            this.routeNumberHandler = handlerFactory.CreateHandler(config.RouteNumber);
            this.destinationTextHandler = handlerFactory.CreateHandler(config.DestinationText);
            this.trackTextHandler = handlerFactory.CreateHandler(config.TrackText);
        }

        /// <summary>
        /// Handles the transformation list received
        /// </summary>
        /// <param name="transferInformationList">
        /// The transfer information List.
        /// </param>
        public void HandleTransferInformation(TransferInformation[] transferInformationList)
        {
            if (transferInformationList.Length <= 0)
            {
                this.DeleteConnections();
                Logger.Trace("Received empty transfer inforamtion list. Transfer Information deleted");
                return;
            }

            foreach (var info in transferInformationList)
            {
                StopTransferInformation rows;
                if (!this.transferInformation.TryGetValue(info.StopId, out rows))
                {
                    rows = new StopTransferInformation();
                    this.transferInformation.Add(info.StopId, rows);
                }

                rows.TransferInformation[rows.Position] = info;
            }
        }

        /// <summary>
        /// Updates transfer information for the specific position. If position has a value -1, it is ignored.
        /// Updates the transfer information in the list with the same stop Id if available, else inserts the
        /// transfer information at the end of the list of transfer information
        /// </summary>
        /// <param name="position">
        /// The position of the transfer information for a specific stop Id.
        /// </param>
        /// <param name="info">
        /// The transfer information.
        /// </param>
        public void UpdateTransferInformation(int position, TransferInformation info)
        {
            if (position == -1)
            {
                Logger.Debug("Received transfer information at position {0}. Position ignored!", position);
            }

            StopTransferInformation old;
            if (this.transferInformation.TryGetValue(info.StopId, out old))
            {
                old.TransferInformation[position] = info;
            }
            else
            {
                old = new StopTransferInformation();
                old.TransferInformation[old.Position] = info;
                this.transferInformation.Add(info.StopId, old);
            }

            if (this.GetNextStop() == old)
            {
                this.SendTransferInfomation(old);
            }
        }

        /// <summary>
        /// Sends the transfer information based on the stop index.
        /// </summary>
        /// <param name="stopId">
        ///     The stop number
        /// </param>
        public void SetNextStop(int stopId)
        {
            this.currentStopIndex = stopId;

            var infos = this.GetNextStop();
            if (infos != null)
            {
                this.SendTransferInfomation(infos);
            }
        }

        /// <summary>
        /// Deletes the transfer information for a specific stop Id if it is available in the
        /// list of transfer information received.
        /// </summary>
        /// <param name="tripKey">
        /// The trip key.
        /// </param>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        public void DeleteTransferInformation(int tripKey, int stopId)
        {
            StopTransferInformation old;
            if (!this.transferInformation.TryGetValue(stopId, out old))
            {
                Logger.Info("The transfer information for the stop Id {0} was not available to delete", stopId);
                return;
            }

            for (int i = 0; i < old.TransferInformation.Count; i++)
            {
                old.TransferInformation[i] = StopTransferInformation.EmptyTransferInformation;
            }

            this.SendTransferInfomation(old);
            this.transferInformation.Remove(stopId);
        }

        private void DeleteConnections()
        {
            foreach (var info in this.transferInformation.Values)
            {
                foreach (var transfers in info.TransferInformation)
                {
                    this.DeleteTransferInformation(transfers.Value.TripKey, transfers.Value.StopId);
                }
            }

            this.transferInformation.Clear();
        }

        private StopTransferInformation GetNextStop()
        {
            foreach (var info in this.transferInformation)
            {
                if (info.Key < this.currentStopIndex)
                {
                    continue;
                }

                if (info.Key > this.currentStopIndex)
                {
                    return null;
                }

                return info.Value;
            }

            return null;
        }

        private void SendTransferInfomation(StopTransferInformation transferInfo)
        {
            var ximple = new Ximple();
            int index = 0;
            var now = TimeProvider.Current.Now;
            foreach (var info in transferInfo.TransferInformation.Values)
            {
                this.AddConnectionCells(ximple, info, index, now);
                index++;
            }

            while (index < this.lastSentConnectionsCount)
            {
                this.AddConnectionCells(ximple, StopTransferInformation.EmptyTransferInformation, index, now);
                index++;
            }

            this.lastSentConnectionsCount = index;

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddConnectionCells(Ximple ximple, TransferInformation info, int index, DateTime now)
        {
            this.stopNumberHandler.AddCell(
                ximple, info.StopId == 0 ? string.Empty : info.StopId.ToString(CultureInfo.InvariantCulture), index);
            this.plannedDepartureTimeHandler.AddCells(ximple, info.PlannedDepartureTime, now, index);
            this.calculatedDepartureTimeHandler.AddCells(ximple, info.PlannedDepartureTime, now, index);
            this.routeNumberHandler.AddCell(ximple, info.RouteNumber, index);
            this.destinationTextHandler.AddCell(ximple, info.DestinationText, index);
            this.trackTextHandler.AddCell(ximple, info.TrackText, index);
        }

        /// <summary>
        /// The transfer information for a stop id.
        /// </summary>
        public class StopTransferInformation
        {
            /// <summary>
            /// The empty transfer information.
            /// </summary>
            public static readonly TransferInformation EmptyTransferInformation = new TransferInformation
            {
                DestinationText = string.Empty,
                TrackText = string.Empty
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="StopTransferInformation"/> class.
            /// </summary>
            public StopTransferInformation()
            {
                this.TransferInformation = new Dictionary<int, TransferInformation>();
            }

            /// <summary>
            /// Gets or sets the transfer information with its position in the list.
            /// </summary>
            public Dictionary<int, TransferInformation> TransferInformation { get; set; }

            /// <summary>
            /// Gets the index to be used for the next entry of transfer information.
            /// </summary>
            public int Position
            {
                get
                {
                    return this.TransferInformation.Count;
                }
            }
        }
    }
}
