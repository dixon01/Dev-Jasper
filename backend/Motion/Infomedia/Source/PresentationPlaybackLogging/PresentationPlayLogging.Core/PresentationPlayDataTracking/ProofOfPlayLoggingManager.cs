// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProofOfPlayLoggingManager.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProofOfPlayLoggingManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.Utility.CsvFileHelper;
    using NLog;

    /// <summary>
    /// The proof of play logging manager.
    /// </summary>
    /// <typeparam name="T">The type of object we are logging
    /// </typeparam>
    public class ProofOfPlayLoggingManager<T> : ILoggingManager
        where T : class, IInfotransitPresentationInfo, new()
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CurrentEnvironmentInfo currentEnvironment;

        private readonly ICsvLogging<T> playLogger;

        private readonly Dictionary<int, LayoutSessionData<T>> sessionDataById = new Dictionary<int, LayoutSessionData<T>>();

        private readonly object sessionlock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProofOfPlayLoggingManager{T}"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProofOfPlayLoggingManager(ICsvLogging<T> logger)
        {
            this.playLogger = logger;
            this.currentEnvironment = new CurrentEnvironmentInfo();
        }

        /// <summary>The on file moved.</summary>
        public event EventHandler<string> OnFileMoved;

        /// <summary>
        /// Gets a value indicating whether logging has started
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return this.playLogger.IsStarted;
            }
        }

        /// <summary>
        /// Dispose of the logger
        /// </summary>
        public void Dispose()
        {
            this.playLogger.Dispose();
        }

        /// <summary>
        /// Handles tracking of a video playback event in the unit session data.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="messagePlaying">
        /// The message playing.
        /// </param>
        /// <param name="messageItemId">
        /// The message item id.
        /// </param>
        /// <param name="currentTime">
        /// The current time.
        /// </param>
        public void HandleVideoPlaybackEvent(string unitName, string fileName, bool messagePlaying, int messageItemId, DateTime currentTime)
        {
            // lock (this.sessionlock)
            // {
            //    var sessionData = this.GetSessionDataByUnit(unitName);
            //    sessionData.UpdateVideoPlayback(fileName, messagePlaying, currentTime);
            // }
        }

        /// <summary>
        /// Updates the current environment route information
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        public void SetRoute(string route)
        {
            lock (this.sessionlock)
            {
                this.currentEnvironment.Route = route;
            }
        }

        /// <summary>
        /// Updates the current environment vehicle information
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        public void SetVehicleId(string vehicleId)
        {
            lock (this.sessionlock)
            {
                if (string.IsNullOrEmpty(vehicleId) == false)
                {
                    this.currentEnvironment.VehicleId = vehicleId;
                }
            }
        }

        public LayoutSessionData<T> GetElementData(int id)
        {
            return this.sessionDataById[id];
        }

        public void GetElementData(int id, string unit)
        {
            var layoutSessionData = this.sessionDataById[id];
            
        }

        /// <summary>
        /// Begins logging
        /// </summary>
        /// <param name="logFileName">
        /// The log file name.
        /// </param>
        /// <param name="rollOverLogOutputFolder">
        /// The roll over log output folder.
        /// </param>
        /// <param name="fileNameRolloverType">
        /// The file name rollover type.
        /// </param>
        /// <param name="maxFileSize">
        /// The max file size.
        /// </param>
        /// <param name="maxRecords">
        /// The max records.
        /// </param>
        public void Start(string logFileName, string rollOverLogOutputFolder, FileNameRolloverType fileNameRolloverType, long maxFileSize, long maxRecords)
        {
            this.playLogger.OnFileMoved += this.LoggerOnFileMoved;
            this.playLogger.Start(logFileName, rollOverLogOutputFolder, fileNameRolloverType, maxFileSize, maxRecords);
        }

        /// <summary>
        /// Flush any current data and stop the logger.
        /// </summary>
        public void Stop()
        {
            WriteAllResults();
            this.playLogger.OnFileMoved -= this.LoggerOnFileMoved;
            this.playLogger.Stop();
        }

        /// <summary>
        /// Update the current saved position
        /// </summary>
        /// <param name="latitude">The Latitude coordinate</param>
        /// <param name="longitude">The Longitude coordinates</param>
        public void UpdateCurrentGpsPosition(double latitude, double longitude)
        {
            this.UpdateCurrentGpsPosition(latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Update the current saved position 
        /// </summary>
        /// <param name="latitude">The current latitude</param>
        /// <param name="longitude">The current longitude</param>
        public void UpdateCurrentGpsPosition(string latitude, string longitude)
        {
            lock (this.sessionlock)
            {
                if (string.IsNullOrEmpty(latitude) == false && string.IsNullOrEmpty(longitude) == false)
                {
                    this.currentEnvironment.Latitude = latitude;
                    this.currentEnvironment.Longitude = longitude;
                }
            }
        }

        /// <summary>
        /// Update the current layout session data with updated presentation data.
        /// </summary>
        /// <param name="info">The updated information</param>
        public void UpdateInfotransitPresentationInfoEntry(InfotransitPresentationInfo info)
        {
            //lock (this.sessionlock)
            //{
            //    info.GetType()
            //    var sessionData = this.GetSessionDataByUnit(info.UnitName);

            //    sessionData.UpdateItems(info);
            //}
        }

        /// <summary>
        /// Update the current layout session data with vehicle and unit information.
        /// </summary>
        /// <param name="unitInfo">The vehicle and unit information.</param>
        public void UpdateVehicleUnitInfo(VehicleUnitInfo unitInfo)
        {
            // check the Unit info where we can get Unit info details, VehicleId, Route and/or GPS coordinates when defined
            if (unitInfo == null)
            {
                return;
            }

            var route = unitInfo.VehiclePosition.Route;
            this.SetRoute(route);

            var vehicleId = unitInfo.VehicleInfo.VehicleId;
            this.SetVehicleId(vehicleId);

            var vehiclePosition = unitInfo.VehiclePosition;
            if (vehiclePosition != null)
            {
                this.UpdateCurrentGpsPosition(vehiclePosition.GeoCoordinate.Latitude, vehiclePosition.GeoCoordinate.Longitude);
            }
        }

        /// <summary>
        /// We have an update on a composer item.
        /// </summary>
        /// <param name="message">The incoming message containing element status</param>
        public void UpdateDrawableComposer(DrawableComposerInitMessage message)
        {
            if (string.IsNullOrEmpty(message.UnitName))
            {
                return;
            }

            var sessionData = this.GetSessionDataByElementId(message.ElementID);

            switch (message.Status)
            {
                case DrawableStatus.Initialized:
                    sessionData.InitializeDrawableItemData(message);
                    return;
                    
                case DrawableStatus.Rendering:
                    sessionData.UpdateItemUnits(message);
                    break;

                case DrawableStatus.Disposing:                    
                    this.WriteResults(sessionData);
                    break;
            }
        }


        private void WriteAllResults()
        {
            foreach (var sessionDataKVP in this.sessionDataById)
            {
                this.WriteResults(sessionDataKVP.Value);
            }
        }

        private void WriteResults(LayoutSessionData<T> sessionData)
        {
            List<T> items = sessionData.GetLogItems();

            foreach (var item in items)
            {
                item.Duration = 0; // Fix this sometime, to be the actual expected duration.
                item.Route = this.FirstValidString(item.Route, this.currentEnvironment.Route);
                item.StartedLatitude = item.StartedLatitude.Replace("NaN", string.Empty);
                item.StartedLongitude = item.StartedLongitude.Replace("NaN", string.Empty);
                item.StoppedLatitude = this.FirstValidString(this.currentEnvironment.Latitude, item.StoppedLatitude).Replace("NaN", string.Empty);
                item.StoppedLongitude = this.FirstValidString(this.currentEnvironment.Longitude, item.StoppedLongitude).Replace("NaN", string.Empty);
                item.VehicleId = this.currentEnvironment.VehicleId;
                item.PlayStarted = this.NonNullValue(item.PlayStarted, DateTime.Now);
                item.PlayStopped = DateTime.Now;

                // Calculate the played duration. We round up to the nearest integer.
                if (item.PlayStarted.HasValue)
                {
                    item.PlayedDuration = (long)Math.Round((item.PlayStopped.Value - item.PlayStarted.Value).TotalSeconds + 0.5, MidpointRounding.AwayFromZero);
                    item.IsPlayInterrupted = item.PlayedDuration < item.Duration;
                }

                if (item.IsValid)
                {
                    this.WriteAsync(item);
                }
            }            
        }

        private string FirstValidString(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private LayoutSessionData<T> GetSessionDataByElementId(int elementId)
        {
            if (!this.sessionDataById.TryGetValue(elementId, out var sessionData))
            {
                sessionData = new LayoutSessionData<T>(this.currentEnvironment);
                this.sessionDataById[elementId] = sessionData;
                sessionData.ElementId = elementId;
            }

            return sessionData;
        }

        private void LoggerOnFileMoved(object sender, string s)
        {
            this.OnFileMoved?.Invoke(sender, s);
        }

        private DateTime? NonNullValue(DateTime? one, DateTime other)
        {
            if (one != null)
            {
                return one;
            }

            return other;
        }

        private void WriteAll(ICollection<T> infoItems)
        {
            this.playLogger.WriteAll(infoItems);
        }

        private void WriteAsync(T infoItem)
        {
            this.playLogger.WriteAsync(infoItem);
        }
       
        /// <summary>
        /// Writes the current set of session data
        /// </summary>
        /// <param name="sessionData">The info on layout elements, by the file name.</param>
        private void WriteSessionData(LayoutSessionData<T> sessionData)
        {
            List<T> logEntries = new List<T>();
            
            lock (this.sessionlock)
            {
                // We're closing this session, but first we need to finalize the data.
                foreach (var infoItem in sessionData.InfoItems)
                {
                    T logInfo = new T
                                    {
                                        ResourceId = infoItem.ResourceId,
                                        Duration = infoItem.Duration,
                                        FileName = infoItem.FileName,
                                        Route = this.FirstValidString(infoItem.Route, this.currentEnvironment.Route),
                                        StartedLatitude = infoItem.StartedLatitude.Replace("NaN", string.Empty),
                                        StartedLongitude = infoItem.StartedLongitude.Replace("NaN", string.Empty),
                                        StoppedLatitude = this.FirstValidString(this.currentEnvironment.Latitude, infoItem.StoppedLatitude).Replace("NaN", string.Empty),
                                        StoppedLongitude = this.FirstValidString(this.currentEnvironment.Longitude, infoItem.StoppedLongitude).Replace("NaN", string.Empty),
                                        VehicleId = infoItem.VehicleId,
                                        UnitName = infoItem.UnitName,
                                        PlayStarted = this.NonNullValue(infoItem.PlayStarted, DateTime.Now),
                                        PlayStopped = this.NonNullValue(infoItem.PlayStopped, DateTime.Now)
                                    };

                    // Calculate the played duration. We round up to the nearest integer.
                    if (logInfo.PlayStopped.HasValue && logInfo.PlayStarted.HasValue)
                    {
                        logInfo.PlayedDuration = (long)Math.Round((logInfo.PlayStopped.Value - logInfo.PlayStarted.Value).TotalSeconds + 0.5, MidpointRounding.AwayFromZero);
                        logInfo.IsPlayInterrupted = logInfo.PlayedDuration < logInfo.Duration;
                    }
                    
                    // Images "stop" playing when the layout session ends.
                    if (sessionData.GetScreenItem(logInfo.FileName) is ImageItem)
                    {
                        logInfo.PlayStopped = DateTime.Now;
                    }
                    
                    if (logInfo.IsValid)
                    {
                        logEntries.Add(logInfo);
                        Logger.Debug($"Adding log entry for {logInfo.FileName}, expected duration: {logInfo.Duration}, actual: {logInfo.PlayedDuration}");
                    }
                }

                if (logEntries.Count > 0)
                {
                    this.WriteAll(logEntries);
                }
            }
        }


    }
}