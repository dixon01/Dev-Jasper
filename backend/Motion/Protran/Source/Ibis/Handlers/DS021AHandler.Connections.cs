// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AHandler.Connections.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Telegram handler part related to connections for DS021a that
    /// handles connection information according to their <see cref="DS021AConfig"/>.
    /// </summary>
    public partial class DS021AHandler
    {
        private readonly Dictionary<int, ConnectionInfo> connections = new Dictionary<int, ConnectionInfo>();

        private ITimer connectionsUpdateTimer;

        private int lastSentConnectionsCount;

        private GenericUsageHandler connectionStopNameUsage;
        private GenericUsageHandler connectionUsage;
        private GenericUsageHandler connectionLineNumberUsage;
        private GenericUsageHandler connectionDepartureTimeUsage;

        /// <summary>
        /// Clears all stop information and fill all empty cells into the given ximple object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple to fill.
        /// </param>
        protected override void ClearStops(Ximple ximple)
        {
            base.ClearStops(ximple);

            this.ClearConnections();
        }

        /// <summary>
        /// Flushes all currently collected stops via Ximple.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        protected override void FlushStops(FlushReason reason)
        {
            this.currentTime = TimeProvider.Current.Now;
            base.FlushStops(reason);

            if (reason == FlushReason.IndexUpdate)
            {
                this.FlushConnections();
            }
        }

        private void InitConnections()
        {
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.SaveConnections();
            var context = persistenceService.GetContext<List<ConnectionInfo>>();
            if (context.Value != null && context.Valid)
            {
                foreach (var connection in context.Value)
                {
                    this.connections[connection.TelegramIndex] = connection;
                }
            }
        }

        private void ConfigureConnections()
        {
            this.connectionsUpdateTimer = TimerFactory.Current.CreateTimer("ConnectionsUpdate");
            this.connectionsUpdateTimer.AutoReset = false;
            this.connectionsUpdateTimer.Elapsed += (s, e) => this.FlushConnections();
            if (this.config.FlushTimeout > TimeSpan.Zero)
            {
                this.connectionsUpdateTimer.Interval = this.config.FlushTimeout;
            }

            this.connectionStopNameUsage = new GenericUsageHandler(
                this.config.Connection.UsedForStopName, this.Dictionary);

            this.connectionUsage = new GenericUsageHandler(this.config.Connection.UsedFor, this.Dictionary);
            this.connectionLineNumberUsage = new GenericUsageHandler(
                this.config.Connection.UsedForLineNumber, this.Dictionary);
            this.connectionDepartureTimeUsage = new GenericUsageHandler(
                this.config.Connection.UsedForDepartureTime, this.Dictionary);
        }

        /// <summary>
        /// Entry point for connection handling of DS021a 'A' variant.
        /// </summary>
        /// <param name="index">the Ax index</param>
        /// <param name="text">the text following x04 in the payload of the telegram</param>
        private void HandleConnectionTelegram(int index, string text)
        {
            if (this.config.Connection == null || !this.config.Connection.Enabled)
            {
                this.Logger.Debug("Connection info handling is disabled, ignoring telegram");
                return;
            }

            if (index < 0 || index > 9)
            {
                this.Logger.Warn("Received invalid connection index: {0}", index);
                return;
            }

            if (index == 0)
            {
                this.ClearConnections();
                return;
            }

            if (text.Trim().Length == 0)
            {
                this.RemoveConnection(index);
                return;
            }

            var parts = text.Split(';');
            if (parts.Length != 4)
            {
                this.Logger.Warn("Received invalid connection info: '{0}' doesn't contain 4 parts", text);
                return;
            }

            int stopIndex;
            if (!ParserUtil.TryParse(parts[0], out stopIndex))
            {
                this.Logger.Warn("Received invalid connection stop index: {0}", parts[0]);
                return;
            }

            int transferTime;
            if (!ParserUtil.TryParse(parts[2], out transferTime))
            {
                this.Logger.Warn("Received invalid connection transfer time: {0}", parts[2]);
                return;
            }

            var info = new ConnectionInfo
                {
                    TelegramIndex = index,
                    StopIndex = stopIndex,
                    LineNumber = parts[1],
                    TransferTime = transferTime,
                    Destination = parts[3]
                };
            this.AddConnection(info);
        }

        private void AddConnection(ConnectionInfo info)
        {
            bool isUpdate;
            lock (((ICollection)this.connections).SyncRoot)
            {
                ConnectionInfo old;
                isUpdate = this.connections.TryGetValue(info.TelegramIndex, out old) && old.StopIndex == info.StopIndex;
                this.connections[info.TelegramIndex] = info;
            }

            if (isUpdate || info.TelegramIndex == 9)
            {
                // immediately flush since we know this is the last index
                this.FlushConnections();
            }
            else
            {
                this.RestartUpdateTimer();
            }
        }

        private void RemoveConnection(int index)
        {
            bool removed;
            lock (((ICollection)this.connections).SyncRoot)
            {
                removed = this.connections.Remove(index);
            }

            if (removed)
            {
                this.FlushConnections();
            }
        }

        private void ClearConnections()
        {
            lock (((ICollection)this.connections).SyncRoot)
            {
                this.connections.Clear();
            }

            this.FlushConnections();
        }

        private void FlushConnections()
        {
            this.connectionsUpdateTimer.Enabled = false;
            if (this.config.Connection == null || !this.config.Connection.Enabled)
            {
                return;
            }

            var ximple = new Ximple();
            var currentConnections = new List<ConnectionInfo>(10);
            int nextStopIndex = int.MaxValue;

            lock (((ICollection)this.connections).SyncRoot)
            {
                // find the next stop index to show
                foreach (var connection in this.connections.Values)
                {
                    if (connection.StopIndex >= this.CurrentStopIndex && connection.StopIndex < nextStopIndex)
                    {
                        nextStopIndex = connection.StopIndex;
                    }
                }

                if (nextStopIndex != int.MaxValue
                    && (nextStopIndex == this.CurrentStopIndex || !this.config.Connection.ShowForNextStopOnly))
                {
                    foreach (var connection in this.connections.Values)
                    {
                        if (connection.StopIndex == nextStopIndex && connection.TransferTime > 0)
                        {
                            currentConnections.Add(connection);
                        }
                    }

                    currentConnections.Sort((c1, c2) => c1.TransferTime - c2.TransferTime);

                    StopInfo stop;
                    if (this.TryGetStopInfo(nextStopIndex, out stop))
                    {
                        // add the stop name for the stop for which we are showing connection information
                        this.connectionStopNameUsage.AddCell(ximple, stop.Name);
                    }
                }
            }

            if (ximple.Cells.Count == 0)
            {
                // add an empty stop name since we don't have a current stop
                this.connectionStopNameUsage.AddCell(ximple, string.Empty);
            }

            int index = 0;
            foreach (var connection in currentConnections)
            {
                this.AddConnectionCells(ximple, connection, index++);
            }

            while (index < this.lastSentConnectionsCount)
            {
                this.AddConnectionCells(ximple, ConnectionInfo.Empty, index++);
            }

            this.lastSentConnectionsCount = currentConnections.Count;

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddConnectionCells(Ximple ximple, ConnectionInfo connection, int index)
        {
            var lineNumber = connection.LineNumber;
            if (!string.IsNullOrEmpty(lineNumber))
            {
                lineNumber = string.Format(this.config.Connection.LineNumberFormat, lineNumber);
            }

            this.connectionUsage.AddCell(ximple, connection.Destination, index);
            this.connectionLineNumberUsage.AddCell(ximple, lineNumber, index);
            this.connectionDepartureTimeUsage.AddCell(ximple, connection.TransferTimeString, index);
        }

        private void SaveConnections()
        {
            var context = ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<List<ConnectionInfo>>();
            lock (((ICollection)this.connections).SyncRoot)
            {
                context.Value = new List<ConnectionInfo>(this.connections.Values);
            }
        }

        private void RestartUpdateTimer()
        {
            this.connectionsUpdateTimer.Enabled = false;
            this.connectionsUpdateTimer.Enabled = true;
        }

        /// <summary>
        /// Connection information for DS021a 'A' variant.
        /// This class is only public to support XML serialization.
        /// </summary>
        [XmlRoot("DS021AConnectionInfo")]
        public class ConnectionInfo
        {
            /// <summary>
            /// Empty connection info useful to clear data.
            /// </summary>
            public static readonly ConnectionInfo Empty = new ConnectionInfo
                                                              {
                                                                  LineNumber = string.Empty,
                                                                  Destination = string.Empty
                                                              };

            /// <summary>
            /// Gets or sets the telegram index.
            /// This property is only used for persistence.
            /// </summary>
            public int TelegramIndex { get; set; }

            /// <summary>
            /// Gets or sets the stop index.
            /// </summary>
            public int StopIndex { get; set; }

            /// <summary>
            /// Gets or sets the line number.
            /// </summary>
            public string LineNumber { get; set; }

            /// <summary>
            /// Gets or sets the transfer time in minutes.
            /// </summary>
            public int TransferTime { get; set; }

            /// <summary>
            /// Gets or sets the destination.
            /// </summary>
            public string Destination { get; set; }

            /// <summary>
            /// Gets the transfer time as a string.
            /// </summary>
            [XmlIgnore]
            public string TransferTimeString
            {
                get
                {
                    return this.TransferTime > 0
                               ? this.TransferTime.ToString(CultureInfo.InvariantCulture)
                               : string.Empty;
                }
            }
        }
    }
}