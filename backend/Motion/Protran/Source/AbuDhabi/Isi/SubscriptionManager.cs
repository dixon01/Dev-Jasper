// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;
    using Gorba.Motion.Protran.AbuDhabi.Transformations;

    using NLog;

    /// <summary>
    /// Object tasked to understand weather a subscription is
    /// completely fulfilled or not.
    /// </summary>
    public class SubscriptionManager
    {
        private const char ConnectionsDelimiter = '$';
        private const char ConnectionInfoDelimiter = '§';

        private const string DefaultHostName = @"TFTXXX";

        private const string EwfManagerPath = "ewfmgr.exe";
        private const string CommitArguments = "c: -commit";

        private const string ShutdownPath = "shutdown";
        private const string ShutdownArguments = "/s /t 0";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static int nextSubscriptionId = new Random().Next(100, 1000);

        private readonly IsiClient client;

        private readonly TransformationManager transformationManager;

        private int subscriptionId;

        private string tickerEnglish;

        private string connnectionsEnglishString;

        private string connnectionsArabicString;

        private int connectionRowsEnglish;

        private int connectionRowsArabic;

        private bool serialNumberVerified;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionManager"/> class.
        /// </summary>
        /// <param name="client">The ISI client.</param>
        /// <param name="subscription">The subscription that has to be managed.</param>
        public SubscriptionManager(IsiClient client, Subscription subscription)
        {
            this.client = client;
            this.transformationManager = client.TransformationManger;

            this.Subscription = subscription;
        }

        /// <summary>
        /// Event that is fired whenever a ximple object is created
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the subscription that has to be managed.
        /// </summary>
        public Subscription Subscription { get; private set; }

        /// <summary>
        /// Subscribes to this message.
        /// </summary>
        public void Subscribe()
        {
            if (this.subscriptionId > 0)
            {
                return;
            }

            this.client.RemoteComputer.IsiMessageReceived += this.OnIsiMessageReceived;

            this.subscriptionId = Interlocked.Increment(ref nextSubscriptionId);
            var get = new IsiGet { IsiId = this.subscriptionId };

            if (this.Subscription.OnChange != null)
            {
                get.OnChange = new DataItemRequestList(this.Subscription.OnChange.Split(' '));
            }

            if (this.Subscription.Cyclic > 0)
            {
                get.Cyclic = this.Subscription.Cyclic;
            }

            get.Items = new DataItemRequestList();
            foreach (var dataItem in this.Subscription.DataItems)
            {
                get.Items.Add(dataItem.Name);
            }

            this.client.RemoteComputer.SendIsiMessage(get);
        }

        /// <summary>
        /// Unsubscribes from this message.
        /// </summary>
        public void Unsubscribe()
        {
            if (this.subscriptionId == 0)
            {
                return;
            }

            if (this.client.RemoteComputer.IsConnected)
            {
                // send an unsubscribe IsiGet message
                var get = new IsiGet { IsiId = this.subscriptionId };
                this.client.RemoteComputer.SendIsiMessage(get);
            }

            this.subscriptionId = 0;
        }

        private void OnIsiMessageReceived(object sender, IsiMessageEventArgs e)
        {
            var put = e.IsiMessage as IsiPut;
            if (put == null)
            {
                return;
            }

            if (put.IsiId != this.subscriptionId)
            {
                return;
            }

            var ximple = new Ximple();
            foreach (var dataItem in put.Items)
            {
                var config = this.Subscription.DataItems.Find(cfg => cfg.Name == dataItem.Name);

                if (config == null)
                {
                    continue;
                }

                foreach (var cell in this.HandleDataItem(dataItem, config))
                {
                    if (cell != null)
                    {
                        ximple.Cells.Add(cell);
                    }
                }
            }

            if (ximple.Cells.Count > 0)
            {
                this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            }
        }

        private IEnumerable<XimpleCell> HandleDataItem(DataItem dataItem, DataItemConfig config)
        {
            var stopConfig = config as StopDataItemConfig;
            if (stopConfig != null && stopConfig.FirstLineUsedFor != null && stopConfig.SecondLineUsedFor != null)
            {
                return this.HandleStopDataItem(dataItem, stopConfig);
            }

            var connectionConfig = config as ConnectionDataItemConfig;
            if (connectionConfig != null)
            {
                return this.HandleConnectionInfo(dataItem, connectionConfig);
            }

            return this.HandleDefaultDataItem(dataItem, config);
        }

        private string Transform(DataItem item, DataItemConfig config)
        {
            var transformer = this.transformationManager.GetChain(config.TransfRef);
            if (transformer == null)
            {
                return item.Value;
            }

            return transformer.Transform(item);
        }

        private IEnumerable<XimpleCell> HandleStopDataItem(DataItem dataItem, StopDataItemConfig config)
        {
            var value = this.Transform(dataItem, config);

            var parts = value.Split('\n');

            var first = parts.Length > 1 ? parts[0] : string.Empty;
            var second = parts.Length > 1 ? parts[1] : parts[0];

            yield return this.CreateCell(first, config.FirstLineUsedFor);
            yield return this.CreateCell(second, config.SecondLineUsedFor);
        }

        private IEnumerable<XimpleCell> HandleConnectionInfo(DataItem dataItem, ConnectionDataItemConfig config)
        {
            yield return this.CreateTransferSymbols(dataItem, config);

            if (dataItem.Name.Equals(DataItemName.CurrentStopConnectionInfo))
            {
                if (this.connnectionsEnglishString == dataItem.Value)
                {
                    yield break;
                }

                this.connnectionsEnglishString = dataItem.Value;

                foreach (
                    var connectionCell in
                        this.CreateConnectionScreen(config, dataItem.Value, this.connectionRowsEnglish, true))
                {
                    yield return connectionCell;
                }

                yield break;
            }

            if (dataItem.Name.Equals(DataItemName.CurrentStopConnectionInfoArabic))
            {
                if (this.connnectionsArabicString == dataItem.Value)
                {
                    yield break;
                }

                this.connnectionsArabicString = dataItem.Value;

                foreach (
                    var connectionCell in
                        this.CreateConnectionScreen(config, dataItem.Value, this.connectionRowsArabic, false))
                {
                    yield return connectionCell;
                }
            }
        }

        private IEnumerable<XimpleCell> CreateConnectionScreen(
            ConnectionDataItemConfig config, string connectionString, int oldRows, bool connectionLang)
        {
            var row = 0;

            var connections = connectionString.Split(ConnectionsDelimiter);
            foreach (var connection in connections)
            {
                var items = connection.Split(ConnectionInfoDelimiter);
                if (!items[0].Equals("1"))
                {
                    continue;
                }

                var connectionTime = new DateTime(0).AddMinutes(int.Parse(items[1])).ToString(config.TimeFormat);
                yield return this.CreateCell(connectionTime, config.UsedForConnectionTime, row);

                var lineNumber = items[2].TrimStart('0');
                yield return this.CreateCell(lineNumber, config.UsedForConnectionLineNumber, row);

                yield return this.CreateCell(items[3], config.UsedForConnectionDestination, row);

                var transportType = items[4];
                if (!string.IsNullOrEmpty(transportType))
                {
                    transportType = string.Format(config.TransportTypeFormat, transportType);
                }

                yield return this.CreateCell(transportType, config.UsedForConnectionTransportType, row);
                row++;
            }

            for (var r = row; r < oldRows; r++)
            {
                foreach (var cell in this.ClearConnectionCell(config, r))
                {
                    yield return cell;
                }
            }

            if (connectionLang)
            {
                this.connectionRowsEnglish = row;
            }
            else
            {
                this.connectionRowsArabic = row;
            }
        }

        private IEnumerable<XimpleCell> ClearConnectionCell(ConnectionDataItemConfig config, int row)
        {
            yield return this.CreateCell(string.Empty, config.UsedForConnectionTime, row);
            yield return this.CreateCell(string.Empty, config.UsedForConnectionLineNumber, row);
            yield return this.CreateCell(string.Empty, config.UsedForConnectionDestination, row);
            yield return this.CreateCell(string.Empty, config.UsedForConnectionTransportType, row);
        }

        private XimpleCell CreateTransferSymbols(DataItem dataItem, ConnectionDataItemConfig config)
        {
            var transferSymbols = new StringBuilder();

            var connections = dataItem.Value.Split(ConnectionsDelimiter);
            foreach (var connection in connections)
            {
                var items = connection.Split(ConnectionInfoDelimiter);
                if (items[0].Equals("1"))
                {
                    transferSymbols.Append(items[2]);
                    transferSymbols.Append(";");
                }
            }

            if (transferSymbols.Length > 0)
            {
                transferSymbols.Length--;
            }

            return this.CreateCell(transferSymbols.ToString(), config.UsedForTransferSymbols);
        }

        private IEnumerable<XimpleCell> HandleDefaultDataItem(DataItem dataItem, DataItemConfig config)
        {
            if (dataItem.Name.Equals(DataItemName.TickerText) || dataItem.Name.Equals(DataItemName.TickerTextArabic))
            {
                yield return this.CreateTickerMessage(dataItem, config);
            }
            else
            {
                if (dataItem.Name.Equals(DataItemName.VehicleNo))
                {
                    this.SetTopboxSerialNumber(dataItem, config);
                }

                var value = this.Transform(dataItem, config);
                yield return this.CreateCell(value, config.UsedFor);
            }
        }

        private void SetTopboxSerialNumber(DataItem dataItem, DataItemConfig config)
        {
            if (this.serialNumberVerified)
            {
                return;
            }

            this.serialNumberVerified = true;
            if (!Environment.MachineName.Equals(DefaultHostName, StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.Info(
                    "Hostname is: {0}. Not setting new hostname as its not default name {1}.",
                    Environment.MachineName,
                    DefaultHostName);
                return;
            }

            var vehicleNo = this.Transform(dataItem, config);
            var vehicleNoConfig = config as VehicleNo;
            if (vehicleNoConfig == null)
            {
                return;
            }

            if (!File.Exists(vehicleNoConfig.TopboxSerialNumberFile))
            {
                Logger.Warn(
                    "Topbox serial number reference file not found: {0}", vehicleNoConfig.TopboxSerialNumberFile);
                return;
            }

            var commit = false;
            using (var reader = new CsvReader(vehicleNoConfig.TopboxSerialNumberFile))
            {
                string[] line;
                while ((line = reader.GetCsvLine()) != null)
                {
                    if (line.Length < 2)
                    {
                        continue;
                    }

                    if (!line[0].Equals(vehicleNo, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var hostName = line[1];

                    if (!this.SetHostname(hostName))
                    {
                        Logger.Warn(
                            "Couldn't set hostname: should be '{0}', is '{1}'", hostName, Environment.MachineName);
                        break;
                    }

                    Logger.Info(
                        "Hostname set sucessfully: will be '{0}', currently is '{1}'",
                        hostName,
                        Environment.MachineName);
                    commit = true;
                    break;
                }
            }

            if (commit)
            {
                this.CommitAndRestartSystem();
            }
        }

        private bool SetHostname(string hostname)
        {
            // Invoke WMI to populate the machine name
            var query = string.Format("Win32_ComputerSystem.Name='{0}'", Environment.MachineName);
            using (var wmiObject = new ManagementObject(new ManagementPath(query)))
            {
                var inputArgs = wmiObject.GetMethodParameters("Rename");
                inputArgs["Name"] = hostname;

                // Set the name
                var outParams = wmiObject.InvokeMethod("Rename", inputArgs, null);
                if (outParams == null)
                {
                    return false;
                }

                var ret = (uint)outParams.Properties["ReturnValue"].Value;
                Logger.Debug("Win32_ComputerSystem.Rename() returned {0}", ret);
                return ret == 0;
            }
        }

        private void CommitAndRestartSystem()
        {
            try
            {
                var process = Process.Start(EwfManagerPath, CommitArguments);
                if (process == null)
                {
                    Logger.Warn("Couldn't start {0}", EwfManagerPath);
                    return;
                }

                if (!process.WaitForExit(20 * 1000))
                {
                    Logger.Warn("{0} didn't exit as expected");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't commit settings changes");
            }

            var processShutdown = Process.Start(ShutdownPath, ShutdownArguments);
            if (processShutdown == null)
            {
                Logger.Warn("Couldn't perform shutdown {0}", ShutdownPath);
            }
        }

        private XimpleCell CreateTickerMessage(DataItem dataItem, DataItemConfig config)
        {
            var value = this.Transform(dataItem, config);
            if (dataItem.Name.Equals(DataItemName.TickerText))
            {
                this.tickerEnglish = value;
                return null;
            }

            if (dataItem.Name.Equals(DataItemName.TickerTextArabic))
            {
                var newValue = string.Format("{0}{1}", this.tickerEnglish, value);
                return this.CreateCell(newValue, config.UsedFor);
            }

            return null;
        }

        private XimpleCell CreateCell(string value, GenericUsage usedFor, int row = 0)
        {
            if (usedFor == null)
            {
                return null;
            }

            var language = this.client.Dictionary.GetLanguageForNameOrNumber(usedFor.Language);
            var table = this.client.Dictionary.GetTableForNameOrNumber(usedFor.Table);
            var column = table.GetColumnForNameOrNumber(usedFor.Column);

            return new XimpleCell
            {
                LanguageNumber = language.Index,
                ColumnNumber = column.Index,
                RowNumber = int.Parse(string.Format(usedFor.Row, row)),
                TableNumber = table.Index,
                Value = value
            };
        }

        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
