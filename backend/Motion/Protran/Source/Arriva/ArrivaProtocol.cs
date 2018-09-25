// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrivaProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Arriva;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Core.Protocols;

    using NLog;

    /// <summary>
    /// Manager about all the communications between
    /// Protran and the Arriva's remote board computer.
    /// </summary>
    public class ArrivaProtocol : IProtocol, IManageableObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly ArrivaConnection EmptyConnection = new ArrivaConnection(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty);

        /// <summary>
        /// Event used to manage the running status of this protocol.
        /// </summary>
        private readonly AutoResetEvent liveEvent = new AutoResetEvent(false);

        private readonly string wifiFileNotification;

        private readonly ConfigManager<ArrivaConfig> configManager;

        private HandleTcpClient tcpClient;

        private bool running;

        private ManageFtp manageFtp;

        private int numConnectionRows;

        private SlideShowHandler slideShow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaProtocol"/> class.
        /// Allocates all the variables needed by this object.
        /// </summary>
        public ArrivaProtocol()
        {
            this.Name = "Protocol";

            this.running = true;
            this.tcpClient = null;
            this.wifiFileNotification = "wifi.txt";

            this.configManager = new ConfigManager<ArrivaConfig>
                                     {
                                         FileName = PathManager.Instance.GetPath(FileType.Config, "Arriva.xml")
                                     };
            this.configManager.XmlSchema = ArrivaConfig.Schema;
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Gets or sets Host.
        /// The "host" object that will receive (eventually) data from this protocol
        /// and the will send (eventually) data to this protocol.
        /// </summary>
        public IProtocolHost Host { get; set; }

        /// <summary>
        /// Gets or sets Semaphore.
        /// The semaphore that grants a thread-safe behaviour to this object.
        /// </summary>
        public Mutex Semaphore { get; set; }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Deletes and closes all the objects created
        /// by this object.
        /// </summary>
        public void Stop()
        {
            this.running = false;
            this.liveEvent.Set();
            this.CloseAll();
        }

        /// <summary>
        /// Configures the protocol.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Exception thrown if the
        /// configuration contains no info about the remote server.
        /// </exception>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Starts all the internal activities to this Protocol.
        /// </summary>
        /// <param name="protranHost">The protocol's host.</param>
        public void Run(IProtocolHost protranHost)
        {
            this.Host = protranHost;
            this.tcpClient = new HandleTcpClient(this.configManager.Config.Obu);
            this.tcpClient.ArrivaNextStopDataAllarmer += this.OnArrivaNextStopDataArrived;
            this.tcpClient.ArrivaSlideShowMessageAllarmer += this.OnArrivaSlideShowMessageAllarmer;
            this.tcpClient.ArrivaAdHocMessageAllarmer += this.OnArrivaAdHocMessageAllarmer;
            if (this.configManager.Config.Behaviour.ConnectionSource == ConnectionSource.ArrivaProtocol)
            {
                this.tcpClient.ArrivaConnectionsMessageAllarmer += this.OnArrivaConnectionsMessageAllarmer;
            }

            this.tcpClient.ArrivaLineInfoAllarmer += this.OnArrivaLineInfoAllarmer;
            this.tcpClient.ArrivaWifiStatusMessageAllarmer += this.OnArrivaWifiStatusMessageAllarmer;
            this.tcpClient.Start();

            if (!this.tcpClient.IsConnected)
            {
                // wow, the client is not connected. I start a separate thread to
                // get the connection as soon as possible
                var thread = new Thread(this.Connect) { Name = "Th_Connect" };
                thread.Start();
            }

            var compare = this.configManager.Config.Behaviour.ConnectionSource == ConnectionSource.Ftp;
            if (this.configManager.Config.Ftp.PollingEnabled && compare)
            {
                this.manageFtp = new ManageFtp(this.configManager.Config, this.Dictionary);
                this.manageFtp.XimpleCreated += this.HandlerCreatedXimple;
                this.manageFtp.Start();
            }

            this.slideShow = new SlideShowHandler(this.Dictionary);
            this.slideShow.XimpleCreated += (sender, args) => this.Host.OnDataFromProtocol(this, args.Ximple);

            this.RaiseStarted(EventArgs.Empty);

            // ok. Now we can wait for the end of this protocol.
            this.liveEvent.WaitOne();

            // at this line of code, to the protocol was ordered to be stopped.
            this.CloseAll();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Arriva Client", parent, this.tcpClient);
            yield return parent.Factory.CreateManagementProvider("Slide Show Messages", parent, this.slideShow);
            yield return parent.Factory.CreateManagementProvider("Connections", parent, this.manageFtp);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Connected to Arriva OBU", this.tcpClient.IsConnected, true);
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Close all the running threads and delete all the resources
        /// allocated during the program's execution.
        /// </summary>
        private void CloseAll()
        {
            lock (this)
            {
                Logger.Info("Closing all");

                if (this.tcpClient != null)
                {
                    this.tcpClient.ArrivaNextStopDataAllarmer -= this.OnArrivaNextStopDataArrived;
                    this.tcpClient.ArrivaSlideShowMessageAllarmer -= this.OnArrivaSlideShowMessageAllarmer;
                    this.tcpClient.ArrivaAdHocMessageAllarmer -= this.OnArrivaAdHocMessageAllarmer;
                    this.tcpClient.ArrivaConnectionsMessageAllarmer -= this.OnArrivaConnectionsMessageAllarmer;
                    this.tcpClient.ArrivaLineInfoAllarmer -= this.OnArrivaLineInfoAllarmer;
                    this.tcpClient.ArrivaWifiStatusMessageAllarmer -= this.OnArrivaWifiStatusMessageAllarmer;
                    this.tcpClient.Stop();
                    this.tcpClient.Dispose();
                    this.tcpClient = null;
                }

                if (this.manageFtp != null)
                {
                    this.manageFtp.Stop();
                    this.manageFtp = null;
                }

                Logger.Info("All closed. Good bye");
            }
        }

        /// <summary>
        /// Tries to connect to the remote Arriva's TCP server.
        /// </summary>
        private void Connect()
        {
            while (this.tcpClient != null && !this.tcpClient.IsConnected)
            {
                if (!this.running)
                {
                    // I stop my temptatives of connection.
                    return;
                }

                if (this.tcpClient == null)
                {
                    continue;
                }

                this.tcpClient.Start();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Asynchronous function invoked whenever a "Next Stop slide" arrives
        /// from the remote Arriva's TCP server.
        /// </summary>
        /// <param name="sender">The object that has invoked this function.</param>
        /// <param name="nextStop">The "Next Stop slide" received by the
        /// remote Arriva's TCP server.</param>
        private void OnArrivaNextStopDataArrived(object sender, NextStopSlide nextStop)
        {
            if (sender == null || nextStop == null)
            {
                // invalid data.
                return;
            }

            // translation in XIMPLE
            var ximple = new Ximple(Constants.Version2);

            if (!this.TranslateNextStopGenericView(nextStop, ximple))
            {
                Logger.Info("Next stop translation failed");
                return;
            }

            // now it's the time to send the Ximple object
            // just created, to the protocol's host.
            this.Host.OnDataFromProtocol(this, ximple);
            Logger.Info("Arriva Protocol sent a XIMPLE to Protran.");
            Thread.Sleep(50); // <== a little pause just to flush out the data...
        }

        private bool TranslateNextStopGenericView(NextStopSlide nextStop, Ximple ximple)
        {
            var values = new List<string> { nextStop.DestCity, nextStop.DestStopName, nextStop.DestTime };
            bool destination = this.TranslateValuesInCoordinates(
                "Destination",
                new List<string> { "DestinationCity", "DestinationName", "DestinationTime" },
                values,
                ximple);

            values = new List<string> { nextStop.LineNo, nextStop.Region, nextStop.StopId, nextStop.Punctuality };
            bool route = this.TranslateValuesInCoordinates(
                "Route",
                new List<string> { "Line", "Region", "CurrentStopIndex", "Punctuality" },
                values,
                ximple);

            values = new List<string> { nextStop.PrevCity, nextStop.PrevStopName };
            bool previous = this.TranslateValuesInCoordinates(
                "Stops",
                new List<string> { "StopCity", "StopName" },
                values,
                ximple);

            // unfortunately, the translation of the other stops is not so comfortable
            // as the others, because the stop informations are stored inside a list
            // inside "nextStop". So, here below I do the translation "manually".
            Table stopsTable = this.Dictionary.GetTableForNameOrNumber("Stops");
            if (stopsTable == null)
            {
                // the incoming table is missing.
                // I cannot continue with the translation.
                Logger.Warn("Table \"Stops\" missing from the dictionary");
                return false;
            }

            // now I'll search for the columns about the stop's info
            var columnStopCity = stopsTable.GetColumnForNameOrNumber("StopCity");
            var columnStopName = stopsTable.GetColumnForNameOrNumber("StopName");
            var columnStopTime = stopsTable.GetColumnForNameOrNumber("StopTime");
            if (columnStopCity == null || columnStopName == null || columnStopTime == null)
            {
                // one of the required column for the stop is missing.
                // I cannot continue with the translation.
                Logger.Warn("Columns missing from the stops table");
                return false;
            }

            for (int i = 0; i < nextStop.NextNList.Count; i++)
            {
                this.AddStopCells(
                    nextStop.NextNList[i], ximple, stopsTable, columnStopCity, columnStopName, columnStopTime, i + 1);
            }

            return previous && destination && route;
        }

        private void AddStopCells(
            NextStopListItem stop,
            Ximple ximple,
            Table stopsTable,
            Column columnStopCity,
            Column columnStopName,
            Column columnStopTime,
            int rowNumber)
        {
            var cellStopCity = new XimpleCell
                                   {
                                       RowNumber = rowNumber,
                                       ColumnNumber = columnStopCity.Index,
                                       TableNumber = stopsTable.Index,
                                       LanguageNumber = 0,
                                       Value = stop.City
                                   };

            var cellStopName = new XimpleCell
                                   {
                                       RowNumber = rowNumber,
                                       ColumnNumber = columnStopName.Index,
                                       TableNumber = stopsTable.Index,
                                       LanguageNumber = 0,
                                       Value = stop.StopName
                                   };

            var cellStopTime = new XimpleCell
                                   {
                                       RowNumber = rowNumber,
                                       ColumnNumber = columnStopTime.Index,
                                       TableNumber = stopsTable.Index,
                                       LanguageNumber = 0,
                                       Value = stop.Time
                                   };

            ximple.Cells.AddRange(new[] { cellStopCity, cellStopName, cellStopTime });
        }

        private bool TranslateValuesInCoordinates(
            string tableName, List<string> columnNames, List<string> values, Ximple ximpleToFill)
        {
            var table = this.Dictionary.GetTableForNameOrNumber(tableName);
            if (table == null)
            {
                // the incoming table is missing.
                // I cannot continue with the translation.
                Logger.Warn("Table \"{0}\" missing from the dictionary", tableName);
                return false;
            }

            for (int i = 0; i < columnNames.Count; i++)
            {
                var columnName = columnNames[i];
                var column = table.GetColumnForNameOrNumber(columnName);
                if (column == null)
                {
                    // one of the required column for the incoming table is missing.
                    // I cannot continue with the translation.
                    Logger.Warn("Column \"{0}\" missing from the table \"{1}\"", columnName, tableName);
                    return false;
                }

                var cellToAdd = new XimpleCell
                                    {
                                        RowNumber = 0,
                                        ColumnNumber = column.Index,
                                        TableNumber = table.Index,
                                        LanguageNumber = 0,
                                        Value = values[i]
                                    };

                ximpleToFill.Cells.Add(cellToAdd);
            }

            // ok, no error found.
            return true;
        }

        private void OnArrivaWifiStatusMessageAllarmer(object sender, WifiStatusMessage data)
        {
            if (sender == null || data == null)
            {
                // invalid data.
                return;
            }

            // 10 October 2011:
            // I notify the arrival of the Wifi info
            // to the SystemManager of Francesco, creating locally a "special" file.
            bool notifyOk = this.NotifyWifiStatus(data.Version, data.WifiStatus);
            string msg = notifyOk ? "WiFi Stauts Notified" : "WiFi Status Not Notified";
            Logger.Info(msg);
        }

        private void OnArrivaLineInfoAllarmer(object sender, LineInfoMessage data)
        {
            if (sender == null || data == null)
            {
                // invalid data.
                return;
            }

            // For the "Line Info" message I've to use the
            // version of XIMPLE containing the "Generic View" feature.
            if (this.Dictionary == null)
            {
                // it's impossible to me to translate the information coming from Arriva
                // to a XIMPLE structure with the "Generic View" TAGs
                // because I don't have any dictionary.
                // So, I've to return.
                return;
            }

            var ximple = new Ximple(Constants.Version2);
            var values = new List<string> { data.Region.ToString(CultureInfo.InvariantCulture) };
            bool route = this.TranslateValuesInCoordinates(
                "Route",
                new List<string> { "Region" },
                values,
                ximple);

            if (!route)
            {
                Logger.Info("Error on translating line info information");
                return;
            }

            // now it's the time to send the Ximple object
            // just created, to the protocol's host.
            this.Host.OnDataFromProtocol(this, ximple);
            Logger.Info("Arriva Protocol sent a XIMPLE to Protran.");
            Thread.Sleep(50); // <== a little pause just to flush out the data...
        }

        private void OnArrivaConnectionsMessageAllarmer(object sender, ConnectionsMessage connectionsData)
        {
            if (sender == null || connectionsData == null)
            {
                // invalid data.
                return;
            }

            if (connectionsData.Connections == null || connectionsData.Connections.Count <= 0)
            {
                // no connection.
                return;
            }

            var ximple = new Ximple(Constants.Version2);
            if (!this.TranslateConnectionsGenericView(connectionsData, ximple))
            {
                Logger.Info("Connections translation failed");
                return;
            }

            if (ximple.Cells.Count == 0)
            {
                return;
            }

            // now it's the time to send the Ximple object
            // just created, to the protocol's host.
            this.Host.OnDataFromProtocol(this, ximple);
            Logger.Info("Arriva Protocol sent a XIMPLE to Protran");
            Thread.Sleep(50); // <== a little pause just to flush out the data...
        }

        private bool TranslateConnectionsGenericView(ConnectionsMessage connectionsData, Ximple ximple)
        {
            // 05 Semptember 2011
            // I've to use the XIMPLE's TAG referring to the Generic View
            // also fo the "Connections" information coming from Arriva.
            var table = this.Dictionary.GetTableForNameOrNumber("Connections");
            if (table == null)
            {
                // no table found.
                // I cannot translate the Arriva's information into a Generic View's cell
                Logger.Info("Connections table missing from the dictionary");
                return false;
            }

            int connectionRow = 0;
            string connectionDestCity = BufferUtils.FromBytesToString(connectionsData.DestName);
            foreach (var connection in connectionsData.Connections)
            {
                if (!this.AddConnectionRow(ximple, table, connectionDestCity, connection, connectionRow))
                {
                    return false;
                }

                connectionRow++;
            }

            for (int row = connectionRow; row < this.numConnectionRows; row++)
            {
                this.AddConnectionRow(ximple, table, string.Empty, EmptyConnection, row);
            }

            this.numConnectionRows = connectionRow;

            // ok, translation succedeed.
            return true;
        }

        private bool AddConnectionRow(
            Ximple ximple, Table table, string connectionDestCity, ArrivaConnection connection, int connectionRow)
        {
            var columnConnDestCity = table.GetColumnForNameOrNumber("ConnectionDestinationCity");
            var columnConnTranspType = table.GetColumnForNameOrNumber("ConnectionTransportType");
            var columnConnTime = table.GetColumnForNameOrNumber("ConnectionTime");
            var columnConnDestName = table.GetColumnForNameOrNumber("ConnectionDestinationName");
            var columnConnLineNumber = table.GetColumnForNameOrNumber("ConnectionLineNumber");
            var columnConnPlatform = table.GetColumnForNameOrNumber("ConnectionPlatform");
            if (columnConnDestCity == null || columnConnTranspType == null || columnConnTime == null ||
                columnConnDestName == null || columnConnLineNumber == null || columnConnPlatform == null)
            {
                // one (or more) column not found.
                // I cannot translate the current infomation to a generic view's cell.
                Logger.Info("Columns missing from the dictionary");
                return false;
            }

            var genViewCell0 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnDestCity.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = connectionDestCity
            };

            var genViewCell1 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnTranspType.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = string.Format("./{0}.jpg", connection.Type.ToString(CultureInfo.InvariantCulture))
            };

            var genViewCell2 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnTime.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = connection.Time
            };

            var genViewCell3 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnDestName.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = connection.Destination
            };

            var genViewCell4 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnLineNumber.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = connection.Line
            };

            var genViewCell5 = new XimpleCell
            {
                RowNumber = connectionRow,
                ColumnNumber = columnConnPlatform.Index,
                TableNumber = table.Index,
                LanguageNumber = 0,
                Value = connection.Platform
            };

            ximple.Cells.Add(genViewCell0);
            ximple.Cells.Add(genViewCell1);
            ximple.Cells.Add(genViewCell2);
            ximple.Cells.Add(genViewCell3);
            ximple.Cells.Add(genViewCell4);
            ximple.Cells.Add(genViewCell5);
            return true;
        }

        private void OnArrivaAdHocMessageAllarmer(object sender, AdHocMessage data)
        {
            if (sender == null || data == null)
            {
                // invalid data.
                return;
            }

            // Attention !!!
            // 28 July 2011
            // For the "Ad Hoc" message I've to use the
            // version of XIMPLE containing the "Generic View" feature.
            if (this.Dictionary == null)
            {
                // it's impossible to me to translate the information coming from Arriva
                // to a XIMPLE structure with the "Generic View" TAGs
                // because I don't have any dictionary.
                // So, I've to return.
                return;
            }

            // translation in XIMPLE
            var table = this.Dictionary.GetTableForNameOrNumber("PassengerMessages");
            if (table == null)
            {
                // no table found.
                // I cannot translate the Arriva's information into a Generic View's cell
                return;
            }

            const int Language = 0;

            var ximple = new Ximple(Constants.Version2);

            var value = data.IconId.ToString(CultureInfo.InvariantCulture);
            this.AddXimpleCell(Language, table, "MessageType", data.IconId, value, ximple);

            value = BufferUtils.FromBytesToString(data.HeaderText);
            this.AddXimpleCell(Language, table, "MessageTitle", data.IconId, value, ximple);

            value = BufferUtils.FromBytesToString(data.MsgText);
            this.AddXimpleCell(Language, table, "MessageText", data.IconId, value, ximple);

            if (ximple.Cells.Count == 0)
            {
                // no cell added to the XIMPLE structure.
                // I don't have to send nothing.
                return;
            }

            // now it's the time to send the Ximple object
            // just created, to the protocol's host.
            this.Host.OnDataFromProtocol(this, ximple);
            Logger.Info("Arriva Protocol sent a XIMPLE to Protran");
            Thread.Sleep(50); // <== a little pause just to flush out the data...
        }

        private void OnArrivaSlideShowMessageAllarmer(object sender, SlideShowMessage data)
        {
            if (sender == null || data == null)
            {
                // invalid data.
                return;
            }

            this.slideShow.AddSlideShowMessage(data);

            // if (this.Dictionary == null)
            // {
            // return;
            // }

            // Table table = this.Dictionary.GetTableFromSpeakingWord("PassengerMessages");
            // if (table == null)
            // {
            // return;
            // }

            // var ximple = new Ximple(XimpleConstants.Version2);

            // var value = data.Id.ToString(CultureInfo.InvariantCulture);
            // this.AddXimpleCell(table, "ID", data.Id, value, ximple);

            // value = data.IconId.ToString(CultureInfo.InvariantCulture);
            // this.AddXimpleCell(table, "Icon ID", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToString(data.HeaderText);
            // this.AddXimpleCell(table, "Header text", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToString(data.MsgText);
            // this.AddXimpleCell(table, "Message text", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToUnix64BitTimestamp(data.StartOfValidity);
            // this.AddXimpleCell(table, "Start validity", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToUnix64BitTimestamp(data.EndOfValidity);
            // this.AddXimpleCell(table, "End validity", data.Id, value, ximple);

            // const int Language = 1;
            // var value = data.Id.ToString(CultureInfo.InvariantCulture);
            // this.AddXimpleCell(Language, table, "MessageType", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToString(data.HeaderText);
            // this.AddXimpleCell(Language, table, "MessageTitle", data.Id, value, ximple);

            // value = BufferUtils.FromBytesToString(data.MsgText);
            // this.AddXimpleCell(Language, table, "MessageText", data.Id, value, ximple);

            // if (ximple.Cells.Count == 0)
            // {
            // // no cell added to the XIMPLE structure.
            // // I don't have to send nothing.
            // return;
            // }

            //// now it's the time to send the Ximple object
            //// just created, to the protocol's host.
            // this.Host.OnDataFromProtocol(this, ximple);
            // Logger.Info("Arriva Protocol sent a XIMPLE to Protran");
            // Thread.Sleep(50); // <== a little pause just to flush out the data...
        }

        private void AddXimpleCell(
            int language, Table table, string columnSpeakingName, int row, string value, Ximple ximple)
        {
            var column = table.GetColumnForNameOrNumber(columnSpeakingName);
            if (column == null)
            {
                // no column found.
                // I cannot translate the current infomation to a generic view's cell.
                Logger.Warn("Couldn't find column {0} in table {1}", columnSpeakingName, table.Name);
                return;
            }

            var genViewCell = new XimpleCell
                                  {
                                      RowNumber = row,
                                      ColumnNumber = column.Index,
                                      TableNumber = table.Index,
                                      LanguageNumber = language,
                                      Value = value
                                  };

            ximple.Cells.Add(genViewCell);
        }

        /// <summary>
        /// Creates locally a specific file with info about the WiFi status of Arriva.
        /// The system manager of Francesco will catch the creation of this file and
        /// will start the update process.
        /// </summary>
        /// <param name="version">The version of the "WiFi" system of Arriva.</param>
        /// <param name="wifiStatus">The status of the "WiFi" system of Arriva.</param>
        /// <returns>True of the "notification" (the creation of the file
        /// and the write operations) are ended with success, else false.</returns>
        private bool NotifyWifiStatus(short version, short wifiStatus)
        {
            // Attention !!!
            // Using the PCP Responder (simulator software of Arriva)
            // the meaning of the "wifiStatus" parameter is the following:
            //
            // In range     <==> 0
            // Out of range <==> 1
            //
            // so, I notify about an update when I receive an "In range" value.
            if (wifiStatus != 0)
            {
                // Arriva has sent to me an "WiFi" message but the wifi status
                // is different to "0".
                // So, I don't have to notify any update.
                return false;
            }

            // ok, I've to notify the need to make an update.
            Assembly dllAssembly = Assembly.GetExecutingAssembly();
            string dllAbsPath = dllAssembly.Location;

            string directotyThisDll = null;
            try
            {
                var fileInfoThisDll = new FileInfo(dllAbsPath);
                if (fileInfoThisDll.Directory != null)
                {
                    directotyThisDll = fileInfoThisDll.Directory.FullName;
                }

                if (directotyThisDll == string.Empty)
                {
                    // it was impossible to get the absolute path
                    // of the directory in which is stored this dll library.
                    return false;
                }
            }
            catch (Exception)
            {
                // an error was occured on creating the file info object
                // "around" this dll library.
                return false;
            }

            string fileNameWifiInfo = directotyThisDll + Path.DirectorySeparatorChar + this.wifiFileNotification;
            var fileInfo = new FileInfo(fileNameWifiInfo);

            if (fileInfo.Exists)
            {
                // the file "wifi.txt" already exists.
                // I've to delete the old instance of it
                // and than re-create it immediately.
                try
                {
                    File.Delete(fileInfo.FullName);
                }
                catch (Exception)
                {
                    // an error was occured on deleting the
                    // old "wifi" file.
                    // I cannot continue with this function.
                    return false;
                }

                // ok, the old file was deleted with success.
            }

            return this.WriteWifiInfoToFile(version, wifiStatus, fileInfo);
        }

        private bool WriteWifiInfoToFile(short version, short wifiStatus, FileInfo fileInfo)
        {
            // if I arrive at this line of code, it means
            // that we are ready to create the file "wifi.txt".
            var fileStream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);

            // ok, the file stream was created with success.
            // now I've to write on it some lines.
            var streamWriter = new StreamWriter(fileStream);

            // if I reach this line of code, it means
            // that the file was created with success and also
            // that I've the possibility to write on it.
            try
            {
                streamWriter.WriteLine("Version=" + version);
                streamWriter.WriteLine("Status=" + wifiStatus);
                streamWriter.Flush();
            }
            catch (Exception ex)
            {
                // an error was occured on writing some string on the file.
                Logger.Warn(ex, "Error in writing to file");
                return false;
            }

            streamWriter.Close();
            streamWriter.Dispose();
            fileStream.Close();
            fileStream.Dispose();
            return true;
        }

        private void HandlerCreatedXimple(object sender, XimpleEventArgs e)
        {
            var ximple = e.Ximple;
            if (ximple == null)
            {
                // no XIMPLE to send.
                return;
            }

            this.Host.OnDataFromProtocol(this, e.Ximple);
        }
    }
}