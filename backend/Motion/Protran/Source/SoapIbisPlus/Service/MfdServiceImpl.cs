// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MfdServiceImpl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MfdServiceImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    using MFD.MFDCustomerService;

    using NLog;

    /// <summary>
    /// The implementation of the MDF Web Service.
    /// </summary>
    internal class MfdServiceImpl : IServiceImpl, IDisposable
    {
        private const string DateTimeFormat = "yyyy.MM.dd HH:mm:ss";

        private const string DefaultErrorMessage = "Could not get error details";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, TransformationChain> transformationChains =
            new Dictionary<string, TransformationChain>();

        private readonly Dictionary<int, TripInfo> trips = new Dictionary<int, TripInfo>();

        private readonly ErrorHandler errorHandler;

        private readonly SoapIbisPlusConfig config;

        private readonly DateTimeManager dateTimeManager;

        private readonly TripManager tripManager;

        private readonly TransferManager transferManager;

        private readonly MessageManager messageManager;

        private readonly ITimer mfdServiceTimeoutTimer;

        private readonly GenericUsageHandler connectionStatusUsage;

        private ConnectionStatus connectionStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MfdServiceImpl"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="dictionary">
        /// The generic dictionary.
        /// </param>
        public MfdServiceImpl(SoapIbisPlusConfig config, Dictionary dictionary)
        {
            this.config = config;

            foreach (var chain in config.Transformations)
            {
                this.transformationChains.Add(
                    chain.Id,
                    new TransformationChain(chain.ResolveReferences(config.Transformations)));
            }

            var handlerFactory = new ItemHandlerFactory(this.transformationChains, dictionary);

            this.dateTimeManager = new DateTimeManager(config.TimeSync, dictionary);
            this.dateTimeManager.XimpleCreated += this.ManagerOnXimpleCreated;

            this.tripManager = new TripManager(config.TripInfo, handlerFactory);
            this.tripManager.XimpleCreated += this.ManagerOnXimpleCreated;

            this.transferManager = new TransferManager(config.TransferInformation, handlerFactory);
            this.transferManager.XimpleCreated += this.ManagerOnXimpleCreated;

            this.messageManager = new MessageManager(config.PassengerInfo, handlerFactory);
            this.messageManager.XimpleCreated += this.ManagerOnXimpleCreated;

            this.errorHandler = new ErrorHandler();
            this.errorHandler.NeedFullDataUpdate = true;

            this.connectionStatus = ConnectionStatus.Unknown;

            this.connectionStatusUsage = new GenericUsageHandler(
                this.config.Service.ConnectionStatusUsedFor, dictionary);

            this.mfdServiceTimeoutTimer = TimerFactory.Current.CreateTimer("MfdServiceTimeout");
            this.mfdServiceTimeoutTimer.Elapsed += this.MfdServiceTimerOnElapsed;
            this.mfdServiceTimeoutTimer.Interval = config.Service.Timeout;
            this.mfdServiceTimeoutTimer.AutoReset = true;
            this.mfdServiceTimeoutTimer.Enabled = true;
        }

        /// <summary>
        /// Event that is fired when this service created some Ximple.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Sets the date and time of the device (Needed to choose the data supply).
        /// </summary>
        /// <param name="datetime">
        /// String in the format <code>YYYY.MM.DD hh:mm:ss</code>.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int SetDateTime(string datetime)
        {
            try
            {
                Logger.Trace("SetDateTime({0})", datetime);
                DateTime date;
                if (DateTime.TryParseExact(
                    datetime, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
                {
                    this.dateTimeManager.SetDateTime(date);
                }
                else
                {
                    Logger.Warn("Couldn't parse DateTime: {0}", datetime);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Date time could not be set");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// Notifies this service about the reception of the IBIS heartbeat.
        /// </summary>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int NotifyIBISHeartbeat()
        {
            Logger.Trace("NotifyIBISHeartbeat()");
            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// Gets the service status.
        /// </summary>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int GetServiceStatus()
        {
            Logger.Trace("GetServiceStatus()");
            this.connectionStatus = ConnectionStatus.Active;
            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// If the MFD device reports an error to IBISPlus, IBISPlus requests
        /// for further error information. The error details will be sent to the logger.
        /// </summary>
        /// <param name="type">
        /// Error type.
        /// </param>
        /// <param name="number">
        /// Error number.
        /// </param>
        /// <returns>
        /// Error details.
        /// </returns>
        public string GetErrorDetails(int type, int number)
        {
            Logger.Trace("GetErrorDetails({0}, {1})", type, number);
            try
            {
                return this.errorHandler.GetErrorDetails((ErrorType)type, (ErrorNumber)number);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not get error details");
            }

            return DefaultErrorMessage;
        }

        /// <summary>
        /// Enables the MFD heartbeat.
        /// This method is currently unused.
        /// </summary>
        /// <param name="flag">
        /// The flag.
        /// </param>
        public void EnableMFDHeartBeat(bool flag)
        {
            Logger.Trace("EnableMFDHeartBeat({0})", flag);
        }

        /// <summary>
        /// This method is used to transfer the complete trip information
        /// for one trip from IBISplus to the MFD device.
        /// </summary>
        /// <param name="tripIdx">
        /// Unique identifier for a trip (Fahrtindex: 1 - 2147483644).
        /// </param>
        /// <param name="patternIdx">
        /// Unique identifier for a pattern (LinienFahrweg).
        /// This parameter is used for reporting issues only
        /// (Linienfahrwegindex: 1 - 2147483644).
        /// </param>
        /// <param name="routeNo">
        /// Route number e.g. entered by driver on MDT (Liniennummer: 1 - 9999).
        /// </param>
        /// <param name="runNo">
        /// Every trip is assigned a run number. The run number is unique within a route;
        /// at a specific point in time a specific run only occurs once within the route (1 - 99).
        /// </param>
        /// <param name="blockNo">
        /// Block number is used to select a block (1 - 99999999).
        /// </param>
        /// <param name="stopPoints">
        /// The stop points of the trip.
        /// </param>
        /// <param name="direction">
        /// Numerical identification for a direction (0 - 253)
        /// 0 - No Direction
        /// 1 - Direction A
        /// 2 - Direction B
        /// 3 - Direction A and B
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "German words.")]
        public int SetTripInfo(
            int tripIdx, int patternIdx, int routeNo, int runNo, int blockNo, StopInfoType[] stopPoints, int direction)
        {
            try
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace(
                        "SetTripInfo({0}, {1}, {2}, {3}, {4}, {5}):",
                        tripIdx,
                        patternIdx,
                        routeNo,
                        runNo,
                        blockNo,
                        direction);
                    foreach (var stopPoint in stopPoints)
                    {
                        Logger.Trace(
                            "- [{0}] {1}, {2}, {3}, {4}, {5}",
                            stopPoint.StopID,
                            stopPoint.StopLongDesc,
                            stopPoint.StopOBCDesc,
                            stopPoint.StopSymbol,
                            stopPoint.ArrivalTime,
                            stopPoint.DepartureTime);
                    }
                }

                var info = new TripInfo(tripIdx, patternIdx, routeNo, runNo, blockNo, stopPoints, direction);
                this.trips[tripIdx] = info;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set trip info");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This method is used to tell the MFD that it has to show the next trip.
        /// </summary>
        /// <param name="tripIdx">
        /// Unique identifier for a trip.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int ActivateNewTrip(int tripIdx)
        {
            Logger.Trace("ActivateNewTrip({0})", tripIdx);
            try
            {
                TripInfo trip;
                if (!this.trips.TryGetValue(tripIdx, out trip))
                {
                    Logger.Warn("Couldn't find trip with index {0}", tripIdx);
                    this.errorHandler.SetError(ErrorType.Trip, ErrorNumber.NotFound);
                    return this.errorHandler.GetErrorBitmap(0);
                }

                if (this.tripManager.LoadTrip(trip))
                {
                    this.errorHandler.NeedFullDataUpdate = false;
                    this.errorHandler.ClearError(ErrorType.Trip);
                }
                else
                {
                    this.errorHandler.SetError(ErrorType.Trip, ErrorNumber.BadData);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not activate new trip");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// Stop sequence stepping. Vehicle has positioned to next stop.
        /// </summary>
        /// <param name="stopId">
        /// IBIS plus generates a unique ID for every stop that is loaded in the TripModel.
        /// This attribute is not stored in the data supply.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int SetNextStop(int stopId)
        {
            Logger.Trace("SetNextStop({0})", stopId);

            try
            {
                if (!this.tripManager.SetNextStop(stopId))
                {
                    this.errorHandler.SetError(ErrorType.Stop, ErrorNumber.NotFound);
                }

                this.transferManager.SetNextStop(stopId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set next stop");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This method will be used to inform the MFD that the vehicle has arrived the stop.
        /// It will be sent to the MFD device, when the door criteria has been set
        /// (Vehicle is on the stop and the door can be opened).
        /// </summary>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int SetReleaseDoor()
        {
            Logger.Trace("SetReleaseDoor()");

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This call adds PassengerInfo to the MFD. Adding means, that the PassengerInformation objects
        /// are cumulated, but objects with the same InformationID are consolidated.
        /// </summary>
        /// <param name="passengerInformationList">
        /// The passenger information list.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int SetPassengerInfo(PassengerInformation[] passengerInformationList)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("SetPassengerInfo():");
                foreach (var info in passengerInformationList)
                {
                    Logger.Trace("[{0}] \"{1}\"", info.Id, info.Body);
                }
            }

            try
            {
                this.messageManager.AddPassengerInfo(passengerInformationList);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set passenger info");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// Deletes a passenger info (undocumented).
        /// </summary>
        /// <param name="informationId">
        /// The information id.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int DeletePassengerInfo(int informationId)
        {
            Logger.Trace("DeletePassengerInfo({0})", informationId);

            try
            {
                this.messageManager.DeletePassengerInfo(informationId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not delete passenger info");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This function sets the transfer information list to be displayed on the transfer screen.
        /// An empty list means the list is deleted.
        /// </summary>
        /// <param name="transferInformationList">
        /// Sorted list that contains the complete transfer info list (may be empty).
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int SetTransferInformation(TransferInformation[] transferInformationList)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("SetTransferInformation():");

                foreach (var info in transferInformationList)
                {
                    LogTransferInfo(info);
                }
            }

            try
            {
                this.transferManager.HandleTransferInformation(transferInformationList);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set transfer information");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This function updates one specific transfer information of the current transfer list.
        /// The update may insert / update or move an item in the list.
        /// </summary>
        /// <param name="position">
        /// zero - based, final position of the updated element in the list. If -1 the parameter is ignored.
        /// See documentation for more information about this parameter.
        /// </param>
        /// <param name="transferInformation">
        /// The new value of the item.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int UpdateTransferInformation(int position, TransferInformation transferInformation)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("UpdateTransferInformation({0}):", position);
                LogTransferInfo(transferInformation);
            }

            try
            {
                this.transferManager.UpdateTransferInformation(position, transferInformation);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not update transfer information");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// This function deletes the one specific transfer information from
        /// the list displayed on the transfer screen.
        /// </summary>
        /// <param name="tripKey">
        /// Unique key to identify a trip in the transfer list.
        /// Unique only within the list for one StopID.
        /// </param>
        /// <param name="stopId">
        /// IBIS plus generates a unique ID for every stop that is loaded in the TripModel.
        /// This attribute is not stored in the data supply.
        /// </param>
        /// <returns>
        /// Encoded error value.
        /// </returns>
        public int DeleteTransferInformation(int tripKey, int stopId)
        {
            Logger.Trace("DeleteTransferInformation({0}, {1})", stopId, tripKey);
            try
            {
                this.transferManager.DeleteTransferInformation(tripKey, stopId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not delete transfer information");
            }

            return this.errorHandler.GetErrorBitmap(0);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.dateTimeManager.XimpleCreated -= this.ManagerOnXimpleCreated;
            this.tripManager.XimpleCreated -= this.ManagerOnXimpleCreated;
            this.transferManager.XimpleCreated -= this.ManagerOnXimpleCreated;
            this.messageManager.XimpleCreated -= this.ManagerOnXimpleCreated;

            this.mfdServiceTimeoutTimer.Enabled = false;
            this.mfdServiceTimeoutTimer.Dispose();

            this.dateTimeManager.Dispose();
            this.tripManager.Dispose();
            this.transferManager.Dispose();
            this.messageManager.Dispose();
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="args">
        /// The event arguments.
        /// </param>
        protected void RaiseXimpleCreated(XimpleEventArgs args)
        {
            if (args.Ximple.Cells.Count == 0)
            {
                return;
            }

            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private static void LogTransferInfo(TransferInformation info)
        {
            Logger.Trace(
                "- {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                info.StopId,
                info.TripKey,
                info.PlannedDepartureTime,
                info.CalculatedDepartureTime,
                info.RouteNumber,
                info.DestinationText,
                info.TrackText);
        }

        private void ManagerOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            this.RaiseXimpleCreated(e);
        }

        private void MfdServiceTimerOnElapsed(object sender, EventArgs e)
        {
            Logger.Trace("Sending connection status: {0}", this.connectionStatus);
            var ximple = new Ximple();
            this.connectionStatusUsage.AddCell(ximple, this.connectionStatus == ConnectionStatus.Active ? "1" : "0");
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            this.connectionStatus = ConnectionStatus.Inactive;
        }
    }
}