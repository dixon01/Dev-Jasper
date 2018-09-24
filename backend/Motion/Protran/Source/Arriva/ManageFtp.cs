// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageFtp.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManageFtp type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Protran.Arriva;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Motion.Protran.Arriva.Connections;

    using NLog;

    /// <summary>
    /// Object to manage cyclic check on Ftp server
    /// </summary>
    public class ManageFtp : IManageableTable, IManageableObject
    {
        private const int TrainLanguage = 0;
        private const int BusLanguage = 1;

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Departure EmptyDeparture = new Departure
                                                               {
                                                                   Departuretime = string.Empty,
                                                                   Delay = string.Empty,
                                                                   Platform = string.Empty,
                                                                   Destination = string.Empty,
                                                                   Pto = string.Empty,
                                                                   Line = string.Empty
                                                               };

        /// <summary>
        /// Container of all the configuration needed
        /// to interact with the remote ISM FTP server.
        /// </summary>
        private readonly ArrivaConfig arrivaConfig;

        private FileSystemWatcher watcher;

        private DeparturesConfig departuresConfig;

        private int numTrainConnectionRows;

        private int numBusConnectionRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageFtp"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public ManageFtp(ArrivaConfig config, Dictionary dictionary)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.arrivaConfig = config;
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Event that is fired if ximple created
        /// </summary>
        public virtual event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Starts all the activities with the Ftp polling
        /// </summary>
        public void Start()
        {
            this.Run();
        }

        /// <summary>
        /// Stop all the activities with the Ftp polling
        /// </summary>
        public void Stop()
        {
            this.watcher.Dispose();
            this.watcher.EnableRaisingEvents = false;
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>("Number of train connections", this.numTrainConnectionRows, true);
            yield return new ManagementProperty<int>("Number of bus connections", this.numBusConnectionRows, true);
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var dep in this.departuresConfig.Traindepartures.Departures)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<string>("Train Destination", dep.Destination, true),
                                     new ManagementProperty<string>("Train Deaprture time", dep.Departuretime, true),
                                     new ManagementProperty<string>("Train Platform", dep.Platform, true),
                                     new ManagementProperty<string>("Train Line number", dep.Line, true),
                                     new ManagementProperty<string>("Train Delay", dep.Delay, true),
                                 };
            }

            foreach (var dep in this.departuresConfig.Busdepartures.Departures)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<string>("Bus Destination", dep.Destination, true),
                                     new ManagementProperty<string>("Bus Deaprture time", dep.Departuretime, true),
                                     new ManagementProperty<string>("Bus Platform", dep.Platform, true),
                                     new ManagementProperty<string>("Bus Line number", dep.Line, true),
                                     new ManagementProperty<string>("Bus Delay", dep.Delay, true),
                                 };
            }
        }

        private void Run()
        {
            if (!Directory.Exists(this.arrivaConfig.Ftp.SourceDirectory))
            {
                Directory.CreateDirectory(this.arrivaConfig.Ftp.SourceDirectory);
                Logger.Info("Source directory of Ftp file created at {0}", this.arrivaConfig.Ftp.SourceDirectory);
            }

            this.StartFileWatcher();
        }

        private void StartFileWatcher()
        {
            this.watcher = new FileSystemWatcher();
            this.watcher.Path = this.arrivaConfig.Ftp.SourceDirectory;
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.watcher.Renamed += this.OnRenamed;
            this.watcher.Deleted += this.OnDeleted;
            this.watcher.EnableRaisingEvents = true;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (this.arrivaConfig.Ftp.Filename != e.Name)
            {
                Logger.Info("Connections filename different from configuration. Ignored", e.ChangeType);
                return;
            }

            Logger.Info("New connections file available. To be parsed", e.ChangeType);

            if (!this.arrivaConfig.Behaviour.ConnectionsEnabled)
            {
                Logger.Info("Display of connections disabled in configuration");
                return;
            }

            try
            {
                this.ProcessConnectionsFile(e);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't handle renaming of file " + e.Name, ex);
            }

            this.watcher.Dispose();
            this.watcher.EnableRaisingEvents = false;
            this.StartFileWatcher();
        }

        private void ProcessConnectionsFile(RenamedEventArgs e)
        {
            var depDecoder = new DepartureDecoder();
            try
            {
                this.departuresConfig = depDecoder.Decode(e.FullPath);
            }
            catch (Exception ex)
            {
                Logger.Info("XMl deserialization failed for file {0}. Transfer set is still valid", e.FullPath);
                Logger.Debug(ex, "ProcessConnectionsFile XMl deserialization failed");
                return;
            }

            if (this.departuresConfig == null)
            {
                Logger.Info("Connections file {0} config was null. Transfer set is still valid", e.FullPath);
                return;
            }

            if (!this.VerifyDeviceId())
            {
                return;
            }

            Logger.Info("Valid file available for connections");

            if (this.departuresConfig.Traindepartures.Departures.Count <= 0
                && this.departuresConfig.Busdepartures.Departures.Count <= 0)
            {
                Logger.Info("No connections information available in connections file");
                return;
            }

            var totalConnections = this.departuresConfig.Traindepartures.Departures.Count
                                   + this.departuresConfig.Busdepartures.Departures.Count;
            if (totalConnections > this.arrivaConfig.Behaviour.MaxDepartures)
            {
                Logger.Info(
                    "Number of connections in file {0} more than maximum allowed {1}",
                    totalConnections,
                    this.arrivaConfig.Behaviour.MaxDepartures);
                Logger.Info("Ignoring the new connections file");
                return;
            }

            var checker = new DeparturesChecker(this.departuresConfig);
            if (!checker.IsExpirationCorrect())
            {
                Logger.Info("Ignoring the new connections file");
                return;
            }

            if (checker.AreDeparturesExpired())
            {
                Logger.Info("Connections file contains expired 'Expiration Time'. Ignoring the new connections file");
                return;
            }

            checker.DeparturesExpired += this.OnDeparturesExpired;
            checker.Start();
            this.StoreConnections();
        }

        private void StoreConnections()
        {
            var ximple = new Ximple(Constants.Version2);

            this.AddConnectionReference(ximple, this.departuresConfig.Stationname, this.departuresConfig.Eta);

            var connectionIndex = 0;
            foreach (var dep in this.departuresConfig.Traindepartures.Departures)
            {
                this.FillConnection(ximple, dep, connectionIndex, TrainLanguage);
                connectionIndex++;
            }

            this.numTrainConnectionRows = connectionIndex;

            connectionIndex = 0;
            foreach (var dep in this.departuresConfig.Busdepartures.Departures)
            {
                this.FillConnection(ximple, dep, connectionIndex, BusLanguage);
                connectionIndex++;
            }

            this.numBusConnectionRows = connectionIndex;

            Logger.Info("Sent connections XIMPLE");
            Logger.Trace(() => "Connections XIMPLE is: " + ximple.ToXmlString());
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddConnectionReference(Ximple ximple, string stationname, string eta)
        {
            var connectionsRefTable = this.Dictionary.GetTableForNameOrNumber("ConnectionReference");
            var column = connectionsRefTable.GetColumnForNameOrNumber("ConnectionReferenceStopName");
            ximple.Cells.Add(this.CreateCell(0, column.Index, connectionsRefTable.Index, stationname, TrainLanguage));
            ximple.Cells.Add(this.CreateCell(0, column.Index, connectionsRefTable.Index, stationname, BusLanguage));

            column = connectionsRefTable.GetColumnForNameOrNumber("ConnectionReferenceStopTime");
            ximple.Cells.Add(this.CreateCell(0, column.Index, connectionsRefTable.Index, eta, TrainLanguage));
            ximple.Cells.Add(this.CreateCell(0, column.Index, connectionsRefTable.Index, eta, BusLanguage));
        }

        private void FillConnection(Ximple ximple, Departure dep, int connectionIndex, int language)
        {
            var connectionsTable = this.Dictionary.GetTableForNameOrNumber("Connections");
            var column = connectionsTable.GetColumnForNameOrNumber("ConnectionTime");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Departuretime, language));

            column = connectionsTable.GetColumnForNameOrNumber("ConnectionDelay");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Delay, language));

            column = connectionsTable.GetColumnForNameOrNumber("ConnectionPlatform");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Platform, language));

            column = connectionsTable.GetColumnForNameOrNumber("ConnectionDestinationName");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Destination, language));

            column = connectionsTable.GetColumnForNameOrNumber("ConnectionInfo");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Pto, language));

            column = connectionsTable.GetColumnForNameOrNumber("ConnectionLineNumber");
            ximple.Cells.Add(
                this.CreateCell(connectionIndex, column.Index, connectionsTable.Index, dep.Line, language));
        }

        private XimpleCell CreateCell(int connectionIndex, int column, int table, string value, int language)
        {
            return new XimpleCell
                       {
                           RowNumber = connectionIndex,
                           ColumnNumber = column,
                           TableNumber = table,
                           Value = value,
                           LanguageNumber = language
                       };
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            var filename = string.Format("{0}.{1}", this.arrivaConfig.Ftp.Filename, "new");
            var path = Path.Combine(this.arrivaConfig.Ftp.SourceDirectory, filename);
            if (e.FullPath == path)
            {
                // In case the deleted file is departures.xml.new
                return;
            }

            if (File.Exists(path))
            {
                Logger.Debug("{0} present", filename);
                return;
            }

            this.ClearConnections();

            Logger.Info("Connections deleted as connections file was not found", e.ChangeType);
        }

        private void ClearConnections()
        {
            try
            {
                var ximple = new Ximple(Constants.Version2);

                this.AddConnectionReference(ximple, string.Empty, string.Empty);

                for (var row = 0; row < this.numTrainConnectionRows; row++)
                {
                    this.FillConnection(ximple, EmptyDeparture, row, TrainLanguage);
                }

                for (var row = 0; row < this.numBusConnectionRows; row++)
                {
                    this.FillConnection(ximple, EmptyDeparture, row, BusLanguage);
                }

                this.numTrainConnectionRows = 0;
                this.numBusConnectionRows = 0;

                Logger.Info("Sent clear connections XIMPLE");
                Logger.Trace(() => "Connections XIMPLE is: " + ximple.ToXmlString());
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't clear connections");
            }
        }

        /// <summary>
        /// Checks the computer's name with the device ID specified in the file
        /// received via FTP, and tells whether it is for this computer or not.
        /// </summary>
        /// <returns>True if the file is for this computer, else false.</returns>
        private bool VerifyDeviceId()
        {
            if (string.Equals(
                Environment.MachineName, this.departuresConfig.Deviceid, StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.Info(
                    "The connections file with device id {0} is intended for this computer",
                    this.departuresConfig.Deviceid);
                Logger.Info("Clearing transfer set");
                this.ClearConnections();
                return true;
            }

            Logger.Info(
                "The connections file with device id {0} is not for this computer", this.departuresConfig.Deviceid);
            return false;
        }

        /// <summary>
        /// Function invoked asynchronously by a DeparturesChecker
        /// whenever it detects that a specific set of departures is expired.
        /// </summary>
        /// <param name="sender">The DeparturesChecker that has fired this event.</param>
        /// <param name="e">The event.</param>
        private void OnDeparturesExpired(object sender, DeparturesExpiredEventArgs e)
        {
            // a set of departures is expired.
            // I've to clear all the connections shown by the media player.
            Logger.Info("Connections have expired");
            this.ClearConnections();
            Logger.Info("All connections cleared");
        }

        private void RaiseXimpleCreated(XimpleEventArgs args)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
