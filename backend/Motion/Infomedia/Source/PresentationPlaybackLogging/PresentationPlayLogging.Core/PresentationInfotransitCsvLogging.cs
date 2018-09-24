// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="PresentationInfotransitCsvLogging.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;

    using NLog;

    /// <summary>Responsible for controlling what gets logged or not</summary>
    public class PresentationInfotransitCsvLogging :
        PresentationPlayCsvLogging<InfotransitPresentationInfo>, IPresentationInfotransitCsvLogging
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object presentationInfoLock = new object();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationInfotransitCsvLogging"/> class.
        /// </summary>
        /// <param name="loggingManager">
        /// The logging Manager.
        /// </param>
        public PresentationInfotransitCsvLogging(ILoggingManager loggingManager) : base(loggingManager)
        {
            this.CommonConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationInfotransitCsvLogging"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="loggingManager">
        /// The logging Manager.
        /// </param>
        public PresentationInfotransitCsvLogging(PresentationPlayLoggingConfig config, ILoggingManager loggingManager)
            : base(config, loggingManager)
        {
            this.CommonConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationInfotransitCsvLogging"/> class.
        /// </summary>
        /// <param name="configFileName">
        /// The config file name.
        /// </param>
        /// <param name="loggingManager">
        /// The logging Manager.
        /// </param>
        public PresentationInfotransitCsvLogging(string configFileName, ILoggingManager loggingManager)
            : base(configFileName, loggingManager)
        {
            this.CommonConstruct();
        }

        /// <summary>
        /// Subscribe to any medi messages we care about.
        /// </summary>
        protected override void SubscribeToMediMessages()
        {
            // On request we need to reply with a Medi message see Broadcast see class AudioStatusMessage     
            this.SubscribeFeedBackMessages<UnitsFeedBackMessage<ScreenChanges>>(this.OnFeedBackMessageMediHandler);
            this.SubscribeFeedBackMessages<UnitsFeedBackMessage<List<ScreenItemBase>>>(this.OnScreenUpdateFeedBackMessageMediHandler);
            this.SubscribeFeedBackMessages<VehiclePositionMessage>(this.OnVehiclePositionMessageMediHandler);
            this.SubscribeFeedBackMessages<Ximple>(this.OnXimpleMediMessageHandler);
            //this.SubscribeFeedBackMessages<AddInfotransitPresentationInfoEntry>(this.OnAddInfotransitPresentationInfoEntryHandler);
            //MessageDispatcher.Instance.Subscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);
            this.SubscribeFeedBackMessages<VehicleUnitInfo>(this.OnVehicleUnitInfoMessageHandler);
            this.SubscribeFeedBackMessages<DrawableComposerInitMessage>(this.DrawableComposerInitMessageHandler);
        }



        /// <summary>
        /// Unsubscribe from medi messages.
        /// </summary>
        protected override void UnsubscribeFromMediMessages()
        {
            this.UnsubscribeFromMediMessage<UnitsFeedBackMessage<ScreenChanges>>(this.OnFeedBackMessageMediHandler);
            this.UnsubscribeFromMediMessage<UnitsFeedBackMessage<List<ScreenItemBase>>>(this.OnScreenUpdateFeedBackMessageMediHandler);
            this.UnsubscribeFromMediMessage<VehiclePositionMessage>(this.OnVehiclePositionMessageMediHandler);
            this.UnsubscribeFromMediMessage<Ximple>(this.OnXimpleMediMessageHandler);
            //this.UnsubscribeFromMediMessage<AddInfotransitPresentationInfoEntry>(this.OnAddInfotransitPresentationInfoEntryHandler);
            //MessageDispatcher.Instance.Unsubscribe<VideoPlaybackEvent>(this.HandleVideoPlaybackEvent);
            this.UnsubscribeFromMediMessage<VehicleUnitInfo>(this.OnVehicleUnitInfoMessageHandler);
            this.UnsubscribeFromMediMessage<DrawableComposerInitMessage>(this.DrawableComposerInitMessageHandler);
        }

        /// <summary>
        /// Handle receiving infotransit presentation info.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="messageEventArgs">
        /// The message event args.
        /// </param>
        protected virtual void OnAddInfotransitPresentationInfoEntryHandler(
            object sender,
            MessageEventArgs<AddInfotransitPresentationInfoEntry> messageEventArgs)
        {
            var infotransitPresentationInfo = messageEventArgs?.Message?.InfotransitPresentationInfo;
            if (infotransitPresentationInfo != null)
            {
                // let subscriber know we have data from medi add message
                this.RaiseOnMediPresentationPlayDataReceived(infotransitPresentationInfo);
                this.LoggingManager.UpdateInfotransitPresentationInfoEntry(infotransitPresentationInfo);
            }
        }

        private void DrawableComposerInitMessageHandler(object sender, MessageEventArgs<DrawableComposerInitMessage> e)
        {
            Logger.Debug($"Drawable composer update: {e.Message.UnitName} {e.Message.ElementID} {e.Message.ElementFileName} {e.Message.Status}");
            this.LoggingManager.UpdateDrawableComposer(e.Message);
        }

        private void OnVehicleUnitInfoMessageHandler(object sender, MessageEventArgs<VehicleUnitInfo> e)
        {
            var unitInfo = e.Message;
            if (unitInfo != null)
            {
                // let subscriber know we have data from medi add message
                this.RaiseOnUnitInfoChanged(sender, unitInfo);
                this.LoggingManager.UpdateVehicleUnitInfo(unitInfo);
            }
        }

        private void HandleVideoPlaybackEvent(object sender, MessageEventArgs<VideoPlaybackEvent> e)
        {
            string fileName = Path.GetFileName(e.Message.VideoUri);
            this.LoggingManager.HandleVideoPlaybackEvent(e.Message.UnitName, fileName, e.Message.Playing, e.Message.ItemId, DateTime.Now);
        }

        private void OnScreenUpdateFeedBackMessageMediHandler(
            object sender,
            MessageEventArgs<UnitsFeedBackMessage<List<ScreenItemBase>>> messageEventArgs)
        {
        }

        /// TODO: Not sure to what extent we respond to feedback messages.
        /// <summary>Handle the subscribed incoming medi FeedBackMessage<ScreenChanges /> message.</summary>
        /// <param name="sender">The sender</param>
        /// <param name="messageEventArgs">Payload feedback message for a ScreenChanges</param>
        private void OnFeedBackMessageMediHandler(
            object sender,
            MessageEventArgs<UnitsFeedBackMessage<ScreenChanges>> messageEventArgs)
        {
            // handle the message here
            var feedbackMessage = messageEventArgs?.Message;
            try
            {
                if (feedbackMessage != null)
                {
                    var unitName = feedbackMessage.UnitName;
                    Logger.Info("{0} UnitName: {1} Enter", nameof(this.OnFeedBackMessageMediHandler), unitName);

                    var screenChanges = feedbackMessage.Message;
                    var hasChanges = screenChanges.Changes?.Any() ?? false;
                    var hasUpdates = screenChanges.Updates?.Any() ?? false;

                    if (hasChanges)
                    {
                        foreach (var screenChange in screenChanges.Changes)
                        {
                            Logger.Trace(
                                "Presentation Play - {0}:{1}: {2}",
                                screenChange.Screen.Type,
                                screenChange.Screen.Id,
                                screenChange.ScreenRoot);

                            foreach (var item in screenChange.ScreenRoot?.Root.Items)
                            {
                                var imageItem = item as ImageItem;
                                if (imageItem != null)
                                {
                                    Logger.Info("File: {0}", imageItem.Filename);
                                }
                            }
                        }
                    }

                    if (hasUpdates)
                    {
                        foreach (var screenUpdate in screenChanges.Updates)
                        {
                            foreach (var itemUpdate in screenUpdate.Updates)
                            {
                                var itemBase = itemUpdate.Value as ItemBase;
                                var proName = itemUpdate.Property;
                                if (itemBase != null)
                                {
                                }
                            }
                        }
                    }

                    // Question what is the difference between Changes and Updates?

                    // TODO 
                    // cache the data, process who the ScreenChanges are from, Master, Slave, which unit and of what type of change.
                }
            }
            catch (Exception ex)
            {
                Logger.Error("OnFeedBackMessageMediHandler Exception {0}", ex.Message);
            }
            finally
            {
                if (feedbackMessage != null)
                {
                    this.RaiseFeedbackMessageReceived(sender, messageEventArgs);
                }
            }
        }

        /// <summary>Handle the subscribed incoming medi LastVehiclePositionMessage.</summary>
        /// <param name="sender">The sender</param>
        /// <param name="messageEventArgs">Payload message for a LastVehiclePositionMessage. Should contain updated GeoCoordinates</param>
        private void OnVehiclePositionMessageMediHandler(
            object sender,
            MessageEventArgs<VehiclePositionMessage> messageEventArgs)
        {
            Logger.Info("{0} Enter", nameof(this.OnVehiclePositionMessageMediHandler));

            // handle the message here
            var vehiclePositionMessage = messageEventArgs?.Message;
            this.UpdateVehicleGpsPosition(sender, vehiclePositionMessage);
        }

        private void UpdateVehicleGpsPosition(object sender, VehiclePositionMessage vehiclePositionMessage)
        {
            if (vehiclePositionMessage != null)
            {
                // save off the GPS location. This will be a Medi generated message from the Luminator Ximple protocol or some other broadcaster
                this.LastVehiclePositionMessage = vehiclePositionMessage;
                if (this.LastVehiclePositionMessage.IsValid)
                {
                    Info("Vehicle GPS Change {0}", vehiclePositionMessage.GeoCoordinate.ToString());

                    this.LoggingManager.UpdateCurrentGpsPosition(
                        vehiclePositionMessage.GeoCoordinate.Latitude.ToString(CultureInfo.InvariantCulture),
                        vehiclePositionMessage.GeoCoordinate.Longitude.ToString(CultureInfo.InvariantCulture));

                    this.RaiseVehiclePositionChanged(sender, vehiclePositionMessage);
                }
            }
        }

        private void OnXimpleMediMessageHandler(object sender, MessageEventArgs<Ximple> messageEventArgs)
        {
            // looking for table we are interested in/for Infotransit data like Vehicle ID, Route
            var ximple = messageEventArgs?.Message;
            if (ximple == null || this.Dictionary == null)
            {
                return;
            }

            this.RaiseXimpleMessageReceived(ximple);
        }

        private void CommonConstruct()
        {
            // Use Auto Class Maping for now     base.CsvClassMapType = typeof(InfotransitePresentationInfoCsvClassMap);
            this.OnXimpleMessageReceived += this.XimpleMessageReceived;
            this.OnMediPresentationPlayDataReceived += this.OnPresentationPlayDataReceived;
        }

        private void OnPresentationPlayDataReceived(object sender, IPresentationInfo presentationInfo)
        {
            Debug.WriteLine("OnPresentationPlayDataReceived");
        }

        private void ProcessXimpleMessage(Ximple ximple)
        {
            // Note the Ximple data for these tables comes at random times and under certain circumstances.
            // VehicleId will only be sent once on systems start so save it off and reuse.
            // GPS is as equipment and movement occurs, normally once per second.
            try
            {
                var tableRoute = this.Dictionary.GetTableForNameOrNumber("Route");
                lock (this.presentationInfoLock)
                {
                    if (tableRoute != null)
                    {
                        // Empty string if not present
                        this.LoggingManager.SetRoute(ximple.Cells.FindFirstXimpleCellValue(tableRoute, "Route"));
                    }

                    var table = this.Dictionary.GetTableForNameOrNumber("InfoTainmentSystemStatus");
                    if (table != null)
                    {
                        this.LoggingManager.SetVehicleId(ximple.Cells.FindFirstXimpleCellValue(table, "VehicleId"));

                        // use the last Vehicle position if set else rely on Ximple message with lat and long
                        var latitude = ximple.Cells.FindFirstXimpleCellValue(table, "Latitude");
                        var longitude = ximple.Cells.FindFirstXimpleCellValue(table, "Longitude");

                        // If we don't have Start Lat then use it else stuf the lat, long into stopped.
                        // We will clear all when we write the record.

                        // OK take the current gps positions and save off
                        this.LoggingManager.UpdateCurrentGpsPosition(latitude, longitude);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ProcessXimpleMessage() Exception {0}", ex.Message);
            }
        }

        /// <summary>Process the Ximple medi message</summary>
        /// <param name="sender">The Sender</param>
        /// <param name="ximple">Ximple message</param>
        private void XimpleMessageReceived(object sender, Ximple ximple)
        {
            this.ProcessXimpleMessage(ximple);
        }
    }
}
