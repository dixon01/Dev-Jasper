// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Telegram handler for GO002 that formats the date according to
    /// its <see cref="GO002Config"/>.
    /// </summary>
    public class GO002Handler : TelegramHandler<GO002>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<GO002Handler>();

        private static readonly GO002 EmptyRow = new GO002
            {
                Pictogram = string.Empty,
                LineNumber = string.Empty,
                DepartureTime = string.Empty,
                TrackNumber = string.Empty,
                Deviation = string.Empty,
                Destination = string.Empty
            };

        private readonly SortedMap<int, ConnectionInfo> connections;
        private GO002Config config;

        private GenericUsageHandler destinationUsage;
        private GenericUsageHandler pictogramUsage;
        private GenericUsageHandler lineNumberUsage;
        private GenericUsageHandler departureTimeUsage;
        private GenericUsageHandler trackNumberUsage;
        private GenericUsageHandler deviationUsage;

        private int lastStopIndex;
        private int maxRowsSent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO002Handler"/> class.
        /// </summary>
        public GO002Handler()
            : base(10)
        {
            this.connections = new SortedMap<int, ConnectionInfo>();
            this.lastStopIndex = int.MaxValue;

            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.Save();
        }

        /// <summary>
        /// Configures the handler.
        /// </summary>
        /// <param name="telegramConfig">
        /// The config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig telegramConfig, IIbisConfigContext configContext)
        {
            this.config = (GO002Config)telegramConfig;

            var dictionary = configContext.Dictionary;
            this.destinationUsage = new GenericUsageHandler(this.config.UsedFor, dictionary);
            this.pictogramUsage = new GenericUsageHandler(this.config.UsedForPictogram, dictionary);
            this.lineNumberUsage = new GenericUsageHandler(this.config.UsedForLineNumber, dictionary);
            this.departureTimeUsage = new GenericUsageHandler(this.config.UsedForDepartureTime, dictionary);
            this.trackNumberUsage = new GenericUsageHandler(this.config.UsedForTrackNumber, dictionary);
            this.deviationUsage = new GenericUsageHandler(this.config.UsedForScheduleDeviation, dictionary);

            base.Configure(telegramConfig, configContext);
            this.Restore();
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public override bool Accept(Telegram telegram)
        {
            return (telegram is GO002 || telegram is DS010B || telegram is DS010J || telegram is DS021A)
                   && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            var ds010B = telegram as DS010B;
            if (ds010B != null)
            {
                // handle the DS010B.
                this.HandleDS010B(ds010B);
                return;
            }

            var ds010J = telegram as DS010J;
            if (ds010J != null)
            {
                // handle the DS010J.
                this.HandleDS010J(ds010J);
                return;
            }

            var ds021A = telegram as DS021A;
            if (ds021A != null)
            {
                // handle the DS010B.
                this.HandleDS021A(ds021A);
                return;
            }

            var tlgGo002 = telegram as GO002;
            if (tlgGo002 != null)
            {
                // handle the telegram.
                this.HandleInput(tlgGo002);
            }
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(GO002 telegram)
        {
            if (telegram.StopIndex < 0)
            {
                Logger.Debug("GO002 having no index, clearing all connections");
                this.connections.Clear();
                this.ClearConnectionTable();
                return;
            }

            ConnectionInfo rows;
            if (telegram.Pictogram == null)
            {
                Logger.Debug("Empty GO002 having stop index {0}, clearing those connections", telegram.StopIndex);
                if (!this.connections.TryGetValue(telegram.StopIndex, out rows))
                {
                    return;
                }

                if (this.GetClosestStop() == rows)
                {
                    this.ClearConnectionTable();
                }

                this.connections.Remove(telegram.StopIndex);
                return;
            }

            Logger.Debug("GO002 having stop index: {0}", telegram.StopIndex);
            if (!this.connections.TryGetValue(telegram.StopIndex, out rows))
            {
                rows = new ConnectionInfo();
                this.connections.Add(telegram.StopIndex, rows);
            }

            rows.Connections[telegram.RowNumber] = telegram;

            rows.IsComplete = rows.Connections.ContainsKey(this.config.LastRowIndex);
            if (rows.IsComplete && this.GetClosestStop() == rows)
            {
                // only flush if this is really for the closest stop
                this.FlushConnections(rows);
            }
        }

        private void HandleDS021A(DS021A ds021A)
        {
            int stopIndex;
            if (!ParserUtil.TryParse(ds021A.StopData[1], out stopIndex))
            {
                return;
            }

            if (stopIndex == this.config.FirstStopIndex)
            {
                // now all the information about the connections of all the
                // stops will be deleted.
                this.connections.Clear();
                Logger.Debug("DS021A handled: stop index: {0}. All connections deleted.", stopIndex);

                // now I notify an empty XIMPLE in order to clear the connection table on the screen.
                this.ClearConnectionTable();

                // like this we have no valid "next" stop
                this.lastStopIndex = int.MaxValue;
            }
        }

        private void HandleDS010B(DS010B ds010B)
        {
            this.UpdateStopIndex(ds010B.StopIndex);
        }

        private void HandleDS010J(DS010J ds010J)
        {
            this.UpdateStopIndex(ds010J.StopIndex);
        }

        private void UpdateStopIndex(int stopIndex)
        {
            if (this.lastStopIndex == stopIndex)
            {
                return;
            }

            if (this.config.DeletePassedStops)
            {
                var previous = this.lastStopIndex == int.MaxValue ? -1 : this.lastStopIndex;
                this.DeleteStops(Math.Max(previous, stopIndex - 1));
            }

            this.lastStopIndex = stopIndex;

            var rowsToSend = this.GetClosestStop();

            if (rowsToSend == null || !rowsToSend.IsComplete)
            {
                // no connections set stored for this stop.
                // Let's clear the table
                this.ClearConnectionTable();
                return;
            }

            Logger.Debug("Ready to flush connections for stop index {0}", stopIndex);
            this.FlushConnections(rowsToSend);
        }

        private void DeleteStops(int stopIndex)
        {
            for (int i = 0; i <= stopIndex; i++)
            {
                this.connections.Remove(i);
            }
        }

        private ConnectionInfo GetClosestStop()
        {
            foreach (var dictionaryEntry in this.connections)
            {
                if (dictionaryEntry.Key < this.lastStopIndex)
                {
                    // the current connections set refers to
                    // a stop that was used in the past.
                    // I skip it.
                    continue;
                }

                if (dictionaryEntry.Key > this.lastStopIndex && this.config.ShowForNextStopOnly)
                {
                    // we don't have anything for the next stop
                    return null;
                }

                return dictionaryEntry.Value;
            }

            return null;
        }

        private void FlushConnections(ConnectionInfo rows)
        {
            if (rows == null)
            {
                // invalid set of data.
                return;
            }

            int rowIndex = 0;
            var ximple = new Ximple();
            foreach (var row in rows.Connections.Values)
            {
                this.AddRow(rowIndex, row, ximple);
                rowIndex++;
            }

            while (rowIndex < this.maxRowsSent)
            {
                this.AddRow(rowIndex, EmptyRow, ximple);
                rowIndex++;
            }

            this.maxRowsSent = rowIndex;

            if (ximple.Cells.Count == 0)
            {
                return;
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void AddRow(int rowIndex, GO002 row, Ximple ximple)
        {
            this.destinationUsage.AddCell(ximple, row.Destination, rowIndex);
            this.pictogramUsage.AddCell(ximple, row.Pictogram, rowIndex);
            this.lineNumberUsage.AddCell(ximple, row.LineNumber, rowIndex);
            this.departureTimeUsage.AddCell(ximple, row.DepartureTime, rowIndex);
            this.trackNumberUsage.AddCell(ximple, row.TrackNumber, rowIndex);
            this.deviationUsage.AddCell(ximple, row.Deviation, rowIndex);
        }

        private void ClearConnectionTable()
        {
            if (this.maxRowsSent == 0)
            {
                return;
            }

            var ximple = new Ximple();
            for (int i = 0; i < this.maxRowsSent; i++)
            {
                this.AddRow(i, EmptyRow, ximple);
            }

            this.maxRowsSent = 0;
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
            Logger.Debug("Connections cleared.");
        }

        #region Persistence

        private void Save()
        {
            var context = this.GetPersistenceContext();
            context.Value = new List<ConnectionInfo>(this.connections.Values);
        }

        private void Restore()
        {
            try
            {
                var context = this.GetPersistenceContext();
                if (context.Value == null || !context.Valid)
                {
                    return;
                }

                foreach (var connectionInfo in context.Value)
                {
                    this.connections.Add(connectionInfo.StopIndex, connectionInfo);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not restore persistence");
            }
        }

        private IPersistenceContext<List<ConnectionInfo>> GetPersistenceContext()
        {
            return ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<List<ConnectionInfo>>();
        }
        #endregion

        /// <summary>
        /// Do not use this class outside the outer class. It is only public for XML serialization.
        /// Information about a connection in the list of connections.
        /// </summary>
        public class ConnectionInfo : IXmlSerializable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
            /// </summary>
            public ConnectionInfo()
            {
                this.Connections = new SortedMap<int, GO002>();
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// connection info is complete (i.e. contains row index 9).
            /// </summary>
            public bool IsComplete { get; set; }

            /// <summary>
            /// Gets the stop index of this connection info.
            /// </summary>
            public int StopIndex
            {
                get
                {
                    foreach (var connection in this.Connections)
                    {
                        return connection.Value.StopIndex;
                    }

                    return -1;
                }
            }

            /// <summary>
            /// Gets Connections.
            /// </summary>
            public SortedMap<int, GO002> Connections { get; private set; }

            XmlSchema IXmlSerializable.GetSchema()
            {
                return null;
            }

            /// <summary>
            /// Generates an object from its XML representation.
            /// </summary>
            /// <param name="reader">
            /// The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.
            /// </param>
            void IXmlSerializable.ReadXml(XmlReader reader)
            {
                reader.MoveToContent();
                var complete = reader.GetAttribute("IsComplete");
                this.IsComplete = complete != null && bool.Parse(complete);

                if (reader.IsEmptyElement)
                {
                    reader.ReadStartElement();
                    return;
                }

                reader.ReadStartElement(); // our root element

                var serializer = new XmlSerializer(typeof(GO002));
                while (reader.NodeType == XmlNodeType.Element)
                {
                    var connection = (GO002)serializer.Deserialize(reader);
                    this.Connections.Add(connection.StopIndex, connection);
                }

                reader.ReadEndElement();
            }

            /// <summary>
            /// Converts an object into its XML representation.
            /// </summary>
            /// <param name="writer">
            /// The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.
            /// </param>
            void IXmlSerializable.WriteXml(XmlWriter writer)
            {
                writer.WriteAttributeString("IsComplete", this.IsComplete.ToString(CultureInfo.InvariantCulture));
                var serializer = new XmlSerializer(typeof(GO002));
                foreach (var connection in this.Connections.Values)
                {
                    serializer.Serialize(writer, connection);
                }
            }
        }
   }
}
