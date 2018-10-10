// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="CustomXimpleTableActions.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Xml;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using NLog;

    /// <summary>The custom ximple table action for some Ximple tables that needing special handling.</summary>
    internal class CustomXimpleTableActions : IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<CustomXimpleTableActions>();

        #endregion

        #region Fields

        private bool disposed;

        private string currentVehicleId = string.Empty;

        private string currentRoute = string.Empty;

        private string currentTrip = string.Empty;

        private GeoCoordinate currentGeoCoordinate = GeoCoordinate.Unknown;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CustomXimpleTableActions"/> class.</summary>
        /// <param name="dictionaryConfigManager">The dictionary config manager or null to use the default</param>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If errors occurred while deserializing the file</exception>
        public CustomXimpleTableActions(ConfigManager<Dictionary> dictionaryConfigManager)
        {
            if (dictionaryConfigManager != null)
            {
                this.Dictionary = dictionaryConfigManager.Config;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the dictionary.</summary>
        public Dictionary Dictionary { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Send ximple response over the socket to the client with infotainment audio status.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="audioStatus">The audio status.</param>
        /// <exception cref="ArgumentException">context.SocketState is null</exception>
        public static void SendXimpleAudioStatusResponse(Socket socket, Dictionary dictionary, AudioStatusMessage audioStatus)
        {
            if (socket == null)
            {
                throw new ArgumentException("context.SocketState is null");
            }

            if (dictionary == null)
            {
                Logger.Warn("SendXimpleResponseInfoTainmentAudioStatus Dictionary is null");
                return;
            }

            var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentAudioStatusTableIndexDefault);
            if (table == null)
            {
                Logger.Warn("No Such table Idx={0} found in dictionary to process", XimpleConfig.InfoTainmentAudioStatusTableIndexDefault);
                return;
            }

            // Create Ximple message to reply back to the external hardware via Ximple over sockets
            // The Audio status data will be received as a real time event over medi from the Peripheral Protocol that is interfacing
            // with the Audio Switch hardware/box.
            try
            {
                var ximple = new Ximple(Constants.Version2);
                if (audioStatus == null)
                {
                    ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
                }
                else
                {
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "AudioActive", audioStatus.AudioActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "PttActive", audioStatus.PttActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "LineInActive", audioStatus.LineInActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMsgActive", audioStatus.CannedMsgActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "PttLocked", audioStatus.PttLocked));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "TestActive", audioStatus.TestActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorActive", audioStatus.InteriorActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorActive", audioStatus.ExteriorActive));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorNoiseLevel", audioStatus.InteriorNoiseLevel));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorNoiseLevel", audioStatus.ExteriorNoiseLevel));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorVolume", audioStatus.InteriorVolume));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorVolume", audioStatus.ExteriorVolume));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "LockoutDuration", audioStatus.LockoutDuration));

                    // audio mux version information
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "SerialNumber", audioStatus.SerialNumber));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "SoftwareVersion", audioStatus.SoftwareVersion));
                    ximple.Cells.Add(dictionary.CreateXimpleCell(table, "HardwareVersion", audioStatus.HardwareVersion));
                }

                XimpleSocketService.SendXimpleResponse(socket, ximple);
            }
            catch (SocketException ex)
            {
                Logger.Error("SendXimpleResponseSharedFolderConfig sending Configuration {0}", ex.Message);
            }
        }

        /// <summary>The send ximple volume settings response.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="volumeSettings">The volume settings.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void SendXimpleVolumeSettingsResponse(Socket socket, Dictionary dictionary, VolumeSettingsMessage volumeSettings)
        {
            Logger.Info("{0} Enter", nameof(SendXimpleVolumeSettingsResponse));
            if (socket == null)
            {
                throw new ArgumentException("context.SocketState is null");
            }

            if (dictionary == null)
            {
                Logger.Warn("SendXimpleVolumeSettingsResponse Dictionary is null");
                return;
            }

            var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentVolumeSettingsTableIndexDefault);
            if (table == null)
            {
                Logger.Warn(
                    "SendXimpleVolumeSettingsResponse No Such Ximple Table Id = {0} found in dictionary.xml to process",
                    XimpleConfig.InfoTainmentVolumeSettingsTableIndexDefault);
                return;
            }

            // Create Ximple message to reply back to the external hardware via Ximple over sockets
            // The Audio status data will be received as a real time event over medi from the Protran Audio switch Protocol assembly that is interfacing
            // with the Audio Switch hardware/box.
            try
            {
                var ximple = new Ximple(Constants.Version2);
                if (volumeSettings == null)
                {
                    ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
                }
                else
                {
                    if (volumeSettings.RefreshIntervalMiliSeconds >= 0)
                    {
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "AudioStatusRefreshInterval", volumeSettings.RefreshIntervalMiliSeconds));
                    }
                    /* Dictionary.xml columns Table 100
                     *    <Column Index="0" Name="AudioStatusRefreshInterval" Description="The auto refresh interval for the audio hardware switch to send Audio Status over the serial port. Zero to disable, default 1000 ms" />
                          <Column Index="1" Name="InteriorMaximumVolume" Description="Interior Maximum Audio level setting" />
                          <Column Index="2" Name="InteriorMinimumVolume" Description="Interior Minimum Audio level setting" />
                          <Column Index="3" Name="InteriorDefaultVolume" Description="Interior Default Audio level setting" />
                          <Column Index="4" Name="InteriorCurrentVolume" Description="Interior Current Audio level setting" />
                          <Column Index="5" Name="ExteriorMaximumVolume" Description="Exterior Maximum Audio level setting" />
                          <Column Index="6" Name="ExteriorMinimumVolume" Description="Exterior Minimum Audio level setting" />
                          <Column Index="7" Name="ExteriorDefaultVolume" Description="Exterior Default Audio level setting" />
                          <Column Index="8" Name="ExteriorCurrentVolume" Description="Exterior Current Audio level setting" />
                     */

                    if (volumeSettings.Interior != null)
                    {
                        // Interior volume settings
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorMaximumVolume", volumeSettings.Interior.MaximumVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorMinimumVolume", volumeSettings.Interior.MinimumVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorDefaultVolume", volumeSettings.Interior.DefaultVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "InteriorCurrentVolume", volumeSettings.Interior.CurrentVolume));
                    }

                    if (volumeSettings.Exterior != null)
                    {
                        // Exterior volume settings
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorMaximumVolume", volumeSettings.Exterior.MaximumVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorMinimumVolume", volumeSettings.Exterior.MinimumVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorDefaultVolume", volumeSettings.Exterior.DefaultVolume));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "ExteriorCurrentVolume", volumeSettings.Exterior.CurrentVolume));
                    }
                }

                Logger.Info("Sending Ximple VolumeSettings to Socket client...");
                XimpleSocketService.SendXimpleResponse(socket, ximple);
            }
            catch (SocketException ex)
            {
                Logger.Error("SendXimpleResponseSharedFolderConfig sending Configuration {0}", ex.Message);
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }
        }

        /// <summary>The infotainment audio status table process from Ximple message to medi message. The
        ///     third party is requesting audio status. Send medi message AudioStatusRequest</summary>
        /// <param name="context">The context. Not Ignored used in this call, Ignored</param>
        /// <param name="cells">The cells - Ignored</param>
        public void InfoTainmentAudioStatusTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            // Get the Audio Status and reply via Ximple            
            try
            {
                Logger.Info("InfoTainmentAudioStatusTable() Send Medi message to request Audio Status Enter");

                // Send media message to request audio status to be sent out                
                // See OnAudioStatusMessageHandler in the XimpleSocketService where
                // we subscribe to the audio status changes and send a Ximple reply on the socket
                this.BroadcastMediAudioStatusRequestMessage();
            }
            catch (Exception ex)
            {
                Logger.Error("NetworkSharedFolderTable Failed to get Configuration {0}", ex.Message);
            }
        }

        /// <summary>Process received incoming Ximple message from the infotainment for canned msg play.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cells">The cells.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Invalid Column Name or Id.</exception>
        public void InfoTainmentCannedMsgPlayTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            // we have been requested to play a canned message, this can be Audio or Video
            // we need to find the resource or assent and send a Medi message to Composer to do so
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.Dictionary == null)
            {
                Logger.Error("InfoTainmentCannedMsgPlayTable Dictionary is missing");
                return;
            }

            var dictionary = this.Dictionary;
            var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentCannedMsgPlayTableIndexDefault);

            try
            {
                // Here we need to out Ximple data or medi message out so Composer can handle normally
                // Send Medi message to Composer to cause normal audio/video playback 
                // Infotainment will tell us what to play
                var ximpleCells = cells as IList<XimpleCell> ?? cells.ToList();
                if (table != null && (cells != null && ximpleCells.Any()))
                {
                    // Broadcast a medi message 
                    var fileName = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageFileName");
                    var cannedMessageId = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageID");
                    var cannedMessageZone = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageZone");

                    Logger.Info("Canned Audio Zone in Ximple = {0}", cannedMessageZone);
                    AudioZoneTypes audioZoneType = AudioZoneTypes.None;

                    if (Enum.GetNames(typeof(AudioZoneTypes)).Contains(cannedMessageZone))
                    {
                        if (Enum.TryParse(cannedMessageZone, out audioZoneType) == false)
                        {
                            Logger.Info("Unknown Audio Zone in Ximple, Default to Interior");
                            audioZoneType = AudioZoneTypes.Interior;
                        }
                    }

                    var cannedMessageEncoding = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageEncoding");
                    CannedMessageEncodingType encodingType;
                    if (Enum.TryParse(cannedMessageEncoding, out encodingType) == false)
                    {
                        encodingType = CannedMessageEncodingType.Unknown;
                    }

                    var cannedMessage = new CannedPlaybackMessage
                    {
                        FileName = fileName,
                        AudioZoneTypes = audioZoneType,
                        CannedMessageEncodingType = encodingType,
                        MessageId = cannedMessageId
                    };

                    // Note We will create a Ximple created message to allow the presentation to handle the audio zone and playback
                    Logger.Info("Send Medi message for Canned Playback {0}", cannedMessage);
                    MessageDispatcher.Instance.Broadcast(cannedMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InfotainmentCannedMsgPlayTable Failed {0}", ex.Message);
            }
        }

        /// <summary>For the infotainment Gen3 volume settings table incoming request with a Ximple response to the client.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cells">The cells.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void InfotainmentVolumeSettingsTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            // We have been asked to give volume settings back
            Logger.Info("{0} Enter", nameof(this.InfotainmentVolumeSettingsTable));
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.Dictionary == null || cells == null)
            {
                return;
            }

            var dictionary = this.Dictionary;
            var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentVolumeSettingsTableIndexDefault);
            if (table != null)
            {
                try
                {
                    // populate the message from the cells and broadcast 
                    var volumeSettingsMessage = new VolumeSettingsMessage();

                    var ximpleCells = cells as XimpleCell[] ?? cells.ToArray();

                    // empty cells mean read
                    var emptyCells = ximpleCells.Length == 1 && ximpleCells.First().Value == string.Empty;
                    const byte Ignored = VolumeSettingsMessage.Ignored;

                    if (emptyCells == false)
                    {
                        // Determine if we are Setting only the Volume levels. Look for these columns
                        if (ximpleCells.Length == 2)
                        {
                            // we only consider the two columns for the current volumes. We will NOT persist these values
                            var interiorVolumeColumn = ximpleCells.GetXimpleCellValue(table, "InteriorCurrentVolume", Ignored);
                            var exteriorVolumeColumn = ximpleCells.GetXimpleCellValue(table, "ExteriorCurrentVolume", Ignored);

                            if (interiorVolumeColumn != Ignored && exteriorVolumeColumn != Ignored)
                            {
                                // send medi message to handle changing the two volume levels. These levels are not persisted to our config file!
                                var volumeChangeMessageRequest = new VolumeChangeRequest(interiorVolumeColumn, exteriorVolumeColumn);
                                Logger.Info("Received new Volume Settings {0}", volumeChangeMessageRequest);
                                MessageDispatcher.Instance.Broadcast(volumeChangeMessageRequest);
                            }
                            else
                            {
                                Logger.Warn("Ignored Volume change request, Missing interior or exterior volume levels. Expected Ximple Cells InteriorCurrentVolume and ExteriorCurrentVolume");
                            }
                        }
                        else if (ximpleCells.Length == 1)
                        {
                            // If one Cell then we expect it to be the AudioStatusRefreshInterval
                            // when > 0 in this is the time interval the audio mux will send over RS485 it's audio status
                            // where we will capture and broadcast back to the Ximple TCP clients
                            var audioRefreshInterval = ximpleCells.GetXimpleCellValue(table, "AudioStatusRefreshInterval", (ushort)1);
                            if (audioRefreshInterval != 1)
                            {
                                // change only the refresh interval  
                                var volumeChangeMessageRequest = new VolumeChangeRequest(Ignored, Ignored, audioRefreshInterval);
                                MessageDispatcher.Instance.Broadcast(volumeChangeMessageRequest);
                            }
                            else
                            {
                                Logger.Warn("Ignored audio settings change request, Missing expected audio refresh interval. XimpleCel expected AudioStatusRefreshInterval");
                            }
                        }
                        else
                        {
                            // test for the default value and if the values was not present in a cell optionally
                            // these config level changes we want to persist unlike temporary volume level changes for current volumes for interior or exterior.
                            volumeSettingsMessage.MessageAction = MessageActions.Set;

                            // Look for the refresh interval for audio status polling by the Audio Switch.
                            var audioStatusRefreshInterval = ximpleCells.GetXimpleCellValue<ushort>(table, "AudioStatusRefreshInterval", 0);
                            if (audioStatusRefreshInterval >= 100 || audioStatusRefreshInterval == 0)
                            {
                                volumeSettingsMessage.RefreshIntervalMiliSeconds =
                                    (ushort)(audioStatusRefreshInterval >= 0 && audioStatusRefreshInterval < 30000 ? audioStatusRefreshInterval : 0);
                                Logger.Debug("RefreshIntervalMiliSeconds = {0}", volumeSettingsMessage.RefreshIntervalMiliSeconds);
                            }

                            // Default to ignore the current volume if not provided in Ximple
                            volumeSettingsMessage.Interior.CurrentVolume = Ignored;
                            volumeSettingsMessage.Exterior.CurrentVolume = Ignored;

                            var currentVol = ximpleCells.GetXimpleCellValue<byte>(table, "InteriorCurrentVolume", Ignored);
                            if (currentVol != Ignored)
                            {
                                volumeSettingsMessage.Interior.CurrentVolume = currentVol;
                                Logger.Info("InteriorCurrentVolume changed = {0}", currentVol);
                            }

                            currentVol = ximpleCells.GetXimpleCellValue<byte>(table, "ExteriorCurrentVolume", Ignored);
                            if (currentVol != Ignored)
                            {
                                volumeSettingsMessage.Exterior.CurrentVolume = currentVol;
                                Logger.Info("ExteriorCurrentVolume changed = {0}", currentVol);
                            }

                            var interiorMin = ximpleCells.GetXimpleCellValue<byte>(table, "InteriorMinimumVolume", 0);
                            volumeSettingsMessage.Interior.MinimumVolume = interiorMin;

                            var interiorMax = ximpleCells.GetXimpleCellValue<byte>(table, "InteriorMaximumVolume", 100);
                            volumeSettingsMessage.Interior.MaximumVolume = interiorMax;

                            var interiorDefaultVol = ximpleCells.GetXimpleCellValue<byte>(table, "InteriorDefaultVolume", Ignored);
                            if (interiorDefaultVol != Ignored)
                            {
                                volumeSettingsMessage.Interior.DefaultVolume = interiorDefaultVol;
                            }

                            var exteriorMin = ximpleCells.GetXimpleCellValue<byte>(table, "ExteriorMinimumVolume", 0);
                            volumeSettingsMessage.Exterior.MinimumVolume = exteriorMin;

                            var exteriorMax = ximpleCells.GetXimpleCellValue<byte>(table, "ExteriorMaximumVolume", 100);
                            volumeSettingsMessage.Exterior.MaximumVolume = exteriorMax;

                            var exteriorDefaultVol = ximpleCells.GetXimpleCellValue<byte>(table, "ExteriorDefaultVolume", Ignored);
                            if (exteriorDefaultVol != Ignored)
                            {
                                volumeSettingsMessage.Exterior.DefaultVolume = exteriorDefaultVol;
                            }

                            Logger.Info("Received new Volume Settings {0}", volumeSettingsMessage);

                            // save or set the volume settings - VolumeSettingsMessage
                            // we want to persist the min, max and default interior & exterior settings
                            MessageDispatcher.Instance.Broadcast(volumeSettingsMessage);
                        }
                    }
                    else
                    {
                        // when no cells are given, setup to return the current volume settings from hardware audio switch
                        // get the current settings and reply via the Ximple message on the server
                        Logger.Info("Received empty table to request Audio Volume Settings, sending Medi message to fulfill...");

                        // The PeripheralProtocol will handle this. Read the config settings file and send medi message when completed 
                        // when we send it back to the Ximple TCP clients as a reply
                        // see VolumeSettingsMessageRequestMediHandler in AudioSwitchHandler
                        this.BroadcastMediVolumeSettingsRequestMessage();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("InfotainmentVolumeSettingsTable Failed {0}", ex.Message);
                }
            }
        }

        public void InfoTainmentSystemStatusTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                if (this.Dictionary == null || cells == null)
                {
                    return;
                }

                var dictionary = this.Dictionary;
                var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentSystemStatusTableIndexDefault);
                var tableRoute = dictionary.FindXimpleTable(XimpleConfig.RouteTableIndex);
                if (table != null)
                {
                    var ximpleCells = cells as XimpleCell[] ?? cells.ToArray();
                    if (ximpleCells.Any())
                    {
                        // Broadcast the Vehicle Position via Medi
                        var latitude = ximpleCells.GetXimpleCellValue<double>(table, "Latitude", 0);
                        var longitude = ximpleCells.GetXimpleCellValue<double>(table, "Longitude", 0);
                        var altitude = ximpleCells.GetXimpleCellValue<double>(table, "Altitude", 0);

                        // If the vehcileId data in the Ximple is missing then use the last know current vehicle id
                        var vehicleId = ximpleCells.GetXimpleCellValue<string>(table, "VehicleId", this.currentVehicleId);
                        this.currentVehicleId = vehicleId;

                        var route = string.Empty;
                        var trip = string.Empty;
                        if (tableRoute != null)
                        {
                            // If the route data in the Ximple is missing then use the last know current route
                            route = ximpleCells.GetXimpleCellValue<string>(tableRoute, "Route", this.currentRoute);
                            this.currentRoute = route;

                            trip = ximpleCells.GetXimpleCellValue<string>(tableRoute, "Trip", this.currentTrip);
                            this.currentTrip = trip;
                        }

                        var vehiclePositionMessage = new VehiclePositionMessage(
                            new GeoCoordinate(latitude, longitude, altitude),
                            route,
                            trip);

                        if (latitude == 0 && longitude == 0)
                        {
                            vehiclePositionMessage.GeoCoordinate = this.currentGeoCoordinate;
                            Logger.Warn("VehicleId:{0} Route=[{1}] GPS Position defaulted to {2}", this.currentVehicleId, route, this.currentGeoCoordinate?.ToString());
                        }
                        else
                        {
                            this.currentGeoCoordinate = vehiclePositionMessage.GeoCoordinate;
                            Logger.Info("VehicleId:{0} {1}", vehicleId, vehiclePositionMessage);
                        }

                        // TODO can we get the Secondary unit names too ?

                        var unitNames = new List<string> { MessageDispatcher.Instance.LocalAddress.Unit };
                        var vehicleInfo = new VehicleInfo { VehicleId = this.currentVehicleId, UnitNames = unitNames };
                        var vehicleUnitInfo = new VehicleUnitInfo(vehicleInfo, vehiclePositionMessage);

                        Logger.Info("Broadcast Medi VehicleUnitInfo: {0}", vehicleUnitInfo);
                        MessageDispatcher.Instance.Broadcast(vehicleUnitInfo);

                        // legacy - use above and deprecated Medi.Subscribe on VehiclePositionMessage as VehicleUnitInfo contains this
                        if (string.IsNullOrEmpty(vehicleId) == false)
                        {
                            MessageDispatcher.Instance.Broadcast(vehicleInfo);
                        }
                    }
                }
                else
                {
                    Logger.Warn("failed to find the desired table in the dictionary InfoTainmentSystemStatusTableIndexDefault");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InfoTainmentSystemStatusTable Failed {0}", ex.Message);
            }
        }

        /// <summary>Invoke table actions.</summary>
        /// <param name="context">The context.</param>
        /// <param name="customXimpleTableMapDictionary">The custom Ximple table map dictionary.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void InvokeTableActions(
            XimpleTableActionContext context,
            Dictionary<int, Action<XimpleTableActionContext, IEnumerable<XimpleCell>>> customXimpleTableMapDictionary)
        {
            if (customXimpleTableMapDictionary == null || context == null)
            {
                throw new ArgumentNullException(nameof(context), "Invalid Arguments, Expected non-null values");
            }

            var ximpleCells = context.XimpleCells;
            var enumerable = ximpleCells as XimpleCell[] ?? ximpleCells.ToArray();
            if (!enumerable.Any())
            {
                Logger.Warn("Invoke custom Ximple Table Action No Cells defined, ignored");
                return;
            }

            try
            {
                // Note: we can have multiple ximple cells and possibilities of different tables
                // I want all the same Ximple cells for a table, process them the move to the next batch if any
                var allTableNumbers = enumerable.Select(m => m.TableNumber).Distinct();
                foreach (var tableNumber in allTableNumbers)
                {
                    Action<XimpleTableActionContext, IEnumerable<XimpleCell>> customAction;

                    if (customXimpleTableMapDictionary.TryGetValue(tableNumber, out customAction))
                    {
                        Logger.Trace("Invoke custom Ximple Table Action for TableNumber={0}", tableNumber);
                        customAction.Invoke(context, enumerable);
                    }
                    else
                    {
                        Logger.Warn("Invoke custom Ximple Table Action failed to find Function for TableNumber={0}", tableNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InvokeTableAction() Exception {0}", ex.Message);
            }
        }

        /// <summary>Respond to a network changed Ximple message from external equipment via Ximple and broadcast the change via
        ///     Medi.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cells">The cells.</param>
        public void NetworkChangedMessageTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            // this table currently has only one single Cell, a bool value Connected    
            Logger.Info("NetworkChangedMessageTable() Enter");
            if (this.Dictionary == null)
            {
                Logger.Warn("NetworkChangedMessageTable Dictionary is null");
                return;
            }

            var dictionary = this.Dictionary;
            var table = dictionary.FindXimpleTable("NetworkChangedMessage");
            var column = table.FindColumn("Connected");
            if (table != null && column != null)
            {
                // this Ximple message is in-coming status from the Infotrainment system indicating when we have a wifi network connection or not
                // relay via Medi on and the UpdateManager will subscribe to this message and handle it there.
                var cell = cells.FirstOrDefault(m => m.TableNumber == table.Index && m.ColumnNumber == column.Index);
                if (cell != null)
                {
                    XimpleSocketService.RaiseNetworkConnectionChanged(cell.TryGetBool());
                }
            }
        }

        /// <summary>Process the Ximple request from InfoTainment system to return the Network settings used for file
        ///     sharing between systems.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cells">The XimpleCell cells - ignored.</param>
        /// <exception cref="ArgumentNullException">Invalid context argument</exception>
        public void NetworkFileAccessSettingsTable(XimpleTableActionContext context, IEnumerable<XimpleCell> cells)
        {
            Logger.Info("NetworkFileAccessSettingsTable() Enter");

            // Get the SharedFolderConfig settings from XML that UpdateManager uses
            // We need that XML File read in and we send a Ximple Socket Response back to the client
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.Dictionary == null)
            {
                Logger.Warn("NetworkFileAccessSettingsTable Dictionary is null");
                return;
            }

            // Get the Data for the Shared Folder. We have this class SharedFolderConfig
            // which is used by the UpdateManager in it's config xml file
            // Get that data here and return it
            try
            {
                if (context.SocketState == null)
                {
                    throw new ArgumentException("context.SocketState is null");
                }

                // read the file and return the shared folder settings. This requires the UpdateManager.xml to be distributed
                // along with this assembly which can lead to confusion so we will avoid this case for now.
                var fileName = PathManager.Instance.GetPath(FileType.Config, "XimpleConfig.xml");
                XimpleConfig ximpleConfig = null;
                try
                {
                    // find in our xml config read and use
                    var configMgr = new ConfigManager<XimpleConfig> { FileName = fileName };
                    ximpleConfig = configMgr.Config;
                }
                catch (FileNotFoundException fileNotFound)
                {
                    Logger.Error("NetworkFileAccessSettingsTable() File Not Found {0}", fileNotFound.Message);
                }
                catch (XmlValidationException xmlValidationException)
                {
                    Logger.Error(xmlValidationException, "NetworkFileAccessSettingsTable() Xml Validate error reading file {0}", fileName);
                }

                // Use the dictionary to get the Column Index to use for the field names
                this.SendXimpleResponseNetworkFileAccessSettings(context.SocketState.Socket, this.Dictionary, ximpleConfig);
            }
            catch (Exception ex)
            {
                Logger.Error("NetworkSharedFolderTable Failed to get Configuration {0}", ex.Message);
            }
        }

        /// <summary>The send ximple response to third party for network file access settings.</summary>
        /// <param name="socket">The client socket.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="ximpleConfig">The UpdateManager updateConfig or null to generate a empty Ximple Cell.</param>
        public void SendXimpleResponseNetworkFileAccessSettings(Socket socket, Dictionary dictionary, XimpleConfig ximpleConfig)
        {
            Logger.Info("{0}() Enter", nameof(this.SendXimpleResponseNetworkFileAccessSettings));
            if (dictionary == null)
            {
                Logger.Warn("SendXimpleResponseInfoTainmentAudioStatus Dictionary is null");
                return;
            }

            try
            {
                // read the XimpleConfig.xml to get our settings.
                // Use the dictionary to get the Column Index to use for the field names, find our table first
                var table = dictionary.FindXimpleTable(XimpleConfig.NetworkFileAccessSettingsTableIndexDefault);
                if (table == null)
                {
                    Logger.Warn("No Such table Idx={0} found in dictionary to process", XimpleConfig.NetworkFileAccessSettingsTableIndexDefault);
                    return;
                }

                Debug.WriteLine("Sending Client Shared Network Folder Settings");
                var ximple = new Ximple(Constants.Version2);
                if (ximpleConfig == null)
                {
                    ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
                }
                else
                {
                    var networkFtpSettings = ximpleConfig.NetworkFtpSettings;
                    if (networkFtpSettings != null)
                    {
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Uri", networkFtpSettings.SharedUriString));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "UserName", networkFtpSettings.UserName));
                        ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Password", networkFtpSettings.Password));
                    }
                }

                XimpleSocketService.SendXimpleResponse(socket, ximple);
            }
            catch (SocketException ex)
            {
                Logger.Error("SendXimpleResponseSharedFolderConfig sending Configuration {0}", ex.Message);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Send Medi message to request audio status
        /// </summary>
        private void BroadcastMediAudioStatusRequestMessage()
        {
            Logger.Info("{0} Enter", nameof(this.BroadcastMediAudioStatusRequestMessage));
            MessageDispatcher.Instance.Broadcast(new AudioStatusRequest());
        }

        /// <summary>
        ///     Send Medi message to request volume settings
        /// </summary>
        private void BroadcastMediVolumeSettingsRequestMessage()
        {
            Logger.Info("{0} Enter", nameof(this.BroadcastMediVolumeSettingsRequestMessage));
            MessageDispatcher.Instance.Broadcast(new VolumeSettingsRequestMessage());
        }

        #endregion
    }
}