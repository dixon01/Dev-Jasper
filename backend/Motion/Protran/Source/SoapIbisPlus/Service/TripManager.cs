// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TripManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;
    using System.Globalization;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    using MFD.MFDCustomerService;

    /// <summary>
    /// Class that manages the current trip and creates Ximple for it.
    /// </summary>
    internal class TripManager : DataManagerBase
    {
        private static readonly StopInfoType EmptyStopInfo = new StopInfoType
                                                                 {
                                                                     StopOBCDesc = string.Empty,
                                                                     StopLongDesc = string.Empty,
                                                                     StopSymbol = string.Empty
                                                                 };

        private readonly DataItemHandler routeNumberHandler;
        private readonly DataItemHandler runNumberHandler;
        private readonly DataItemHandler blockNumberHandler;
        private readonly DataItemHandler directionHandler;

        private readonly DataItemHandler stopNumberHandler;
        private readonly DataItemHandler stopNameHandler;
        private readonly DataItemHandler stopDescriptionHandler;
        private readonly DataItemHandler stopSymbolHandler;
        private readonly TimeItemHandler stopArrivalTimeHandler;
        private readonly TimeItemHandler stopDepartureTimeHandler;

        private readonly DataItemHandler destinationNumberHandler;
        private readonly DataItemHandler destinationNameHandler;
        private readonly DataItemHandler destinationDescriptionHandler;
        private readonly DataItemHandler destinationSymbolHandler;
        private readonly TimeItemHandler destinationArrivalTimeHandler;
        private readonly TimeItemHandler destinationDepartureTimeHandler;

        private TripInfo currentTrip;

        private int nextStopIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TripManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="handlerFactory">
        /// The data item handler factory.
        /// </param>
        public TripManager(TripInfoConfig config, ItemHandlerFactory handlerFactory)
        {
            this.routeNumberHandler = handlerFactory.CreateHandler(config.RouteNumber);
            this.runNumberHandler = handlerFactory.CreateHandler(config.RunNumber);
            this.blockNumberHandler = handlerFactory.CreateHandler(config.BlockNumber);
            this.directionHandler = handlerFactory.CreateHandler(config.Direction);

            this.stopNumberHandler = handlerFactory.CreateHandler(config.StopNumber);
            this.stopNameHandler = handlerFactory.CreateHandler(config.StopName);
            this.stopDescriptionHandler = handlerFactory.CreateHandler(config.StopDescription);
            this.stopSymbolHandler = handlerFactory.CreateHandler(config.StopSymbol);
            this.stopArrivalTimeHandler = handlerFactory.CreateHandler(config.StopArrivalTime);
            this.stopDepartureTimeHandler = handlerFactory.CreateHandler(config.StopDepartureTime);

            this.destinationNumberHandler = handlerFactory.CreateHandler(config.DestinationNumber);
            this.destinationNameHandler = handlerFactory.CreateHandler(config.DestinationName);
            this.destinationDescriptionHandler = handlerFactory.CreateHandler(config.DestinationDescription);
            this.destinationSymbolHandler = handlerFactory.CreateHandler(config.DestinationSymbol);
            this.destinationArrivalTimeHandler = handlerFactory.CreateHandler(config.DestinationArrivalTime);
            this.destinationDepartureTimeHandler = handlerFactory.CreateHandler(config.DestinationDepartureTime);
        }

        /// <summary>
        /// Loads the given trip.
        /// </summary>
        /// <param name="trip">
        /// The trip data.
        /// </param>
        /// <returns>
        /// True if it was loaded successfully, false otherwise.
        /// </returns>
        public bool LoadTrip(TripInfo trip)
        {
            this.currentTrip = trip;
            this.nextStopIndex = -1;

            this.SendTripInfo();
            return true;
        }

        /// <summary>
        /// Sets the next stop ID.
        /// </summary>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        /// <returns>
        /// True if it was set successfully, false otherwise.
        /// </returns>
        public bool SetNextStop(int stopId)
        {
            if (this.currentTrip == null)
            {
                return false;
            }

            for (int i = 0; i < this.currentTrip.StopPoints.Length; i++)
            {
                var stopPoint = this.currentTrip.StopPoints[i];
                if (stopPoint.StopID == stopId)
                {
                    return this.SetNextStopIndex(i);
                }
            }

            return false;
        }

        private bool SetNextStopIndex(int index)
        {
            this.nextStopIndex = index;
            this.SendStopList();
            return true;
        }

        private void SendTripInfo()
        {
            if (this.currentTrip == null)
            {
                return;
            }

            var ximple = new Ximple();
            this.routeNumberHandler.AddCell(ximple, this.currentTrip.RouteNo);
            this.runNumberHandler.AddCell(ximple, this.currentTrip.RunNo);
            this.blockNumberHandler.AddCell(ximple, this.currentTrip.BlockNo);
            this.directionHandler.AddCell(ximple, this.currentTrip.Direction);

            var destination = this.currentTrip.StopPoints.Length > 0
                                  ? this.currentTrip.StopPoints[this.currentTrip.StopPoints.Length - 1]
                                  : EmptyStopInfo;

            // TODO: relative times should be updated every time we send the stop list
            var now = TimeProvider.Current.Now;
            this.AddDestinationInfo(ximple, destination, now);

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void SendStopList()
        {
            if (this.currentTrip == null || this.nextStopIndex < 0)
            {
                return;
            }

            var ximple = new Ximple();

            var now = TimeProvider.Current.Now;

            var row = 0;
            for (int i = this.nextStopIndex; i < this.currentTrip.StopPoints.Length; i++)
            {
                this.AddStopInfo(ximple, this.currentTrip.StopPoints[i], now, row++);
            }

            for (int i = 0; i < this.nextStopIndex; i++)
            {
                this.AddStopInfo(ximple, EmptyStopInfo, now, row++);
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddDestinationInfo(Ximple ximple, StopInfoType stopPoint, DateTime now)
        {
            this.destinationNumberHandler.AddCell(ximple, stopPoint.StopID.ToString(CultureInfo.InvariantCulture));
            this.destinationNameHandler.AddCell(ximple, stopPoint.StopOBCDesc);
            this.destinationDescriptionHandler.AddCell(ximple, stopPoint.StopLongDesc);
            this.destinationSymbolHandler.AddCell(ximple, stopPoint.StopSymbol);
            this.destinationArrivalTimeHandler.AddCells(ximple, stopPoint.ArrivalTime, now);
            this.destinationDepartureTimeHandler.AddCells(ximple, stopPoint.DepartureTime, now);
        }

        private void AddStopInfo(Ximple ximple, StopInfoType stopPoint, DateTime now, int row)
        {
            this.stopNumberHandler.AddCell(
                ximple,
                stopPoint.StopID == 0 ? string.Empty : stopPoint.StopID.ToString(CultureInfo.InvariantCulture),
                row);
            this.stopNameHandler.AddCell(ximple, stopPoint.StopOBCDesc, row);
            this.stopDescriptionHandler.AddCell(ximple, stopPoint.StopLongDesc, row);
            this.stopSymbolHandler.AddCell(ximple, stopPoint.StopSymbol, row);
            this.stopArrivalTimeHandler.AddCells(ximple, stopPoint.ArrivalTime, now, row);
            this.stopDepartureTimeHandler.AddCells(ximple, stopPoint.DepartureTime, now, row);
        }
    }
}