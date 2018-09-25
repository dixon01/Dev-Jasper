// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoggingManager.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The LoggingManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.Utility.CsvFileHelper;

    /// <summary>
    /// The LoggingManager interface.
    /// </summary>
    public interface ILoggingManager
    {
        /// <summary>The on file moved.</summary>
        event EventHandler<string> OnFileMoved;
        
        /// <summary>
        /// Gets a value indicating whether logging is started.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Handle updates to the current position.
        /// </summary>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        void UpdateCurrentGpsPosition(string latitude, string longitude);

        /// <summary>
        /// Handle updates to the current vehicle id.
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        void SetVehicleId(string vehicleId);

        /// <summary>
        /// Handle updates to the current route
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        void SetRoute(string route);

        /// <summary>
        /// Handle updates to video playback
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
        void HandleVideoPlaybackEvent(string unitName, string fileName, bool messagePlaying, int messageItemId, DateTime currentTime);

        /// <summary>
        /// The dispose.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Start logging
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
        void Start(string logFileName, string rollOverLogOutputFolder, FileNameRolloverType fileNameRolloverType, long maxFileSize, long maxRecords);

        /// <summary>
        /// Stop logging
        /// </summary>
        void Stop();

        /// <summary>
        /// Handle an update via InfotransitPresentationInfo message.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        void UpdateInfotransitPresentationInfoEntry(InfotransitPresentationInfo info);

        /// <summary>The update vehicle unit info.</summary>
        /// <param name="unitInfo">The unit info.</param>
        void UpdateVehicleUnitInfo(VehicleUnitInfo unitInfo);

        /// <summary>
        /// Handle a drawable composer item status message
        /// </summary>
        /// <param name="message"></param>
        void UpdateDrawableComposer(DrawableComposerInitMessage message);
    }
}