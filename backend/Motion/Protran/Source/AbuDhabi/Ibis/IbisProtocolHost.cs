// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisProtocolHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisProtocolHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Ibis
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;
    using Gorba.Motion.Protran.AbuDhabi.StateMachineCycles;
    using Gorba.Motion.Protran.Core.Protocols;

    /// <summary>
    /// Host for the IBIS protocol. This class allows an abstraction and only indirectly
    /// references the IbisProtocol class. This class is also responsible for converting
    /// Ximple that comes from IBIS and needs special formatting for AbuDhabi that can't
    /// be done through transformations.
    /// </summary>
    public class IbisProtocolHost : XimpleSourceBase, IProtocolHost
    {
        private const string IbisProtocolImpl = "Gorba.Motion.Protran.Ibis.IbisProtocol, Gorba.Motion.Protran.Ibis";

        private static readonly Regex SlashSplitRegex = new Regex("( +/ *)|( */ +)", RegexOptions.ExplicitCapture);

        private readonly IProtocol ibisProtocol;

        private GenericUsage connectionStatusUsedFor;

        private CycleManager cycle;

        private bool specialInputState;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisProtocolHost"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public IbisProtocolHost(Dictionary dictionary)
            : base(dictionary)
        {
            // we completely decouple the host and the implementation,
            // so the AbuDhabi project doesn't have a direct reference to IBIS
            // (otherwise we end up adding IBIS specific stuff to AbuDhabi classes)
            var type = Type.GetType(IbisProtocolImpl, true, true);
            Debug.Assert(type != null, "Type can't be null");
            this.ibisProtocol = (IProtocol)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Event that is fired when the <see cref="SpecialInputState"/> changes.
        /// </summary>
        public event EventHandler SpecialInputStateChanged;

        /// <summary>
        /// Gets Config.
        /// </summary>
        [XmlIgnore]
        public AbuDhabiConfig Config { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether special input is active or not.
        /// </summary>
        public bool SpecialInputState
        {
            get
            {
                return this.specialInputState;
            }

            set
            {
                this.specialInputState = value;
                var handler = this.SpecialInputStateChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Configures this object with the given dictionary.
        /// This method has to be called before calling <see cref="Start"/>.
        /// </summary>
        /// <param name="config">
        /// The overall AbuDhabi configuration.
        /// </param>
        public void Configure(AbuDhabiConfig config)
        {
            this.connectionStatusUsedFor = config.Behaviour.ConnectionStatusUsedFor;
            this.ibisProtocol.Configure(this.Dictionary);
            this.Config = config;
        }

        /// <summary>
        /// Starts the IBIS protocol in a separate thread.
        /// </summary>
        public void Start()
        {
            this.cycle = new CycleManager();
            this.cycle.Configure(this.Config, this.Dictionary);
            this.cycle.XimpleCreated += (s, e) => this.RaiseXimpleCreated(e);
            var thread = new Thread(() => this.ibisProtocol.Run(this));
            thread.Name = "Protocol_" + this.ibisProtocol.Name;
            thread.Start();

            var ximple = new Ximple();

            foreach (var subscription in this.Config.Subscriptions)
            {
                foreach (var dataItem in subscription.DataItems)
                {
                    var connectionConfig = dataItem as ConnectionDataItemConfig;
                    if (connectionConfig != null)
                    {
                        this.ClearConnections(ximple, connectionConfig);
                    }
                }
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        /// <summary>
        /// Stops the IBIS protocol.
        /// </summary>
        public void Stop()
        {
            this.ibisProtocol.Stop();
        }

        /// <summary>
        /// The "host"'s function that will be invoked whenever a protocol
        /// wants send some data to the "host" itself.
        /// </summary>
        /// <param name="sender">The protocol that sends the data to the "host".</param>
        /// <param name="data">The data sent from the protocol "sender" to the "host".</param>
        void IProtocolHost.OnDataFromProtocol(IProtocol sender, Ximple data)
        {
            if (data == null)
            {
                return;
            }

            this.TransformXimple(data);
            if (this.cycle != null)
            {
                this.cycle.ExtractDatafromXimple(data);
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(data));
        }

        private void TransformXimple(Ximple ximple)
        {
            Table table;
            Column column;

            if (this.connectionStatusUsedFor != null)
            {
                table = this.Dictionary.GetTableForNameOrNumber(this.connectionStatusUsedFor.Table);
                column = table.GetColumnForNameOrNumber(this.connectionStatusUsedFor.Column);

                var status = ximple.Cells.Find(c => c.TableNumber == table.Index && c.ColumnNumber == column.Index);
                int statusValue;
                if (status != null && int.TryParse(status.Value, out statusValue) && statusValue != SystemStatus.None)
                {
                    // replace "1" with "2" in the system status cell to notify
                    // Infomedia we are in IBIS mode (when it gets this Ximple)
                    status.Value = SystemStatus.Ibis.ToString(CultureInfo.InvariantCulture);
                }
            }

            // todo: would be nice if this was configurable
            table = this.Dictionary.GetTableForNameOrNumber("Stops");
            column = table.GetColumnForNameOrNumber("StopName");

            var stops = ximple.Cells.FindAll(c => c.TableNumber == table.Index && c.ColumnNumber == column.Index);

            var stopsTable = this.Dictionary.GetTableForNameOrNumber("Stops");

            // convert all stops
            foreach (var cell in stops)
            {
                // remove the old cell
                ximple.Cells.Remove(cell);

                // convert the stop
                this.ConvertStop(cell, stopsTable, "Stop", ximple);
            }

            table = this.Dictionary.GetTableForNameOrNumber("Destination");
            column = table.GetColumnForNameOrNumber("DestinationName");
            var destination = ximple.Cells.Find(c => c.TableNumber == table.Index && c.ColumnNumber == column.Index);
            if (destination == null)
            {
                return;
            }

            // convert destination
            var destinationTable = this.Dictionary.GetTableForNameOrNumber("Destination");

            ximple.Cells.Remove(destination);
            this.ConvertStop(destination, destinationTable, "Destination", ximple);
        }

        private void ConvertStop(
            XimpleCell cell, Table table, string columnPrefix, Ximple ximple)
        {
            var lines = cell.Value.Split('\n');

            var english = lines.Length > 0 ? lines[0] : string.Empty;
            var parts = SlashSplitRegex.Split(english);
            this.AddStop(parts, 0, table, columnPrefix, cell.RowNumber, ximple);

            var arabic = lines.Length > 1 ? this.FixMixedString(lines[1]) : string.Empty;
            parts = SlashSplitRegex.Split(arabic);
            Array.Reverse(parts);
            this.AddStop(parts, 1, table, columnPrefix, cell.RowNumber, ximple);
        }

        private void AddStop(string[] stopParts, int language, Table table, string columnPrefix, int row, Ximple ximple)
        {
            var stopName = table.GetColumnForNameOrNumber(columnPrefix + "Name");
            var stopInfo = table.GetColumnForNameOrNumber(columnPrefix + "City");

            var first = stopParts.Length > 1 ? stopParts[0] : string.Empty;
            var second = stopParts.Length > 1 ? stopParts[1] : stopParts[0];

            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = language,
                TableNumber = table.Index,
                ColumnNumber = stopInfo.Index,
                RowNumber = row,
                Value = first
            });
            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = language,
                TableNumber = table.Index,
                ColumnNumber = stopName.Index,
                RowNumber = row,
                Value = second
            });
        }

        private string FixMixedString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // split by space but prevent from splitting inside bbCode [tags] containing spaces
            var parts = Regex.Split(value, @"(?<!\[[^\]]*) +");
            var inverted = new string[parts.Length];
            int outOff = 0;
            int lastInverted = parts.Length;
            bool uninvert = false;

            for (int i = parts.Length - 1; i >= -1; i--)
            {
                // we go until -1 and then break the end
                // to force a final flush of uninverted parts if necessary
                if (i < 0 || !this.IsArabic(parts[i]))
                {
                    if (uninvert)
                    {
                        for (int j = i + 1; j < lastInverted; j++)
                        {
                            inverted[outOff++] = parts[j];
                        }
                    }

                    if (i < 0)
                    {
                        break;
                    }

                    inverted[outOff++] = parts[i];
                    lastInverted = i;
                    uninvert = false;
                }
                else
                {
                    uninvert = true;
                }
            }

            var str = string.Join(" ", inverted);
            return str;
        }

        private bool IsArabic(string word)
        {
            if (word.Length == 0)
            {
                return false;
            }

            char c = word[0];
            if (c > 0xFF)
            {
                return true;
            }

            if (c == '(' || c == ')')
            {
                // after a parenthesis, check the next character
                return word.Length > 1 && word[1] > 0xFF;
            }

            return false;
        }

        private void ClearConnections(Ximple ximple, ConnectionDataItemConfig config)
        {
            this.ClearConnectionCell(ximple, config.UsedForTransferSymbols);
            this.ClearConnectionCell(ximple, config.UsedForConnectionDestination);
            this.ClearConnectionCell(ximple, config.UsedForConnectionLineNumber);
            this.ClearConnectionCell(ximple, config.UsedForConnectionTime);
            this.ClearConnectionCell(ximple, config.UsedForConnectionTransportType);
        }

        private void ClearConnectionCell(Ximple ximple, GenericUsage usage)
        {
            if (usage == null)
            {
                return;
            }

            var table = this.Dictionary.GetTableForNameOrNumber(usage.Table);
            if (table == null)
            {
                return;
            }

            var column = table.GetColumnForNameOrNumber(usage.Column);
            if (column == null)
            {
                return;
            }

            int row;
            if (int.TryParse(usage.Row, out row))
            {
                // single row
                this.ClearCell(ximple, table, column, row);
            }
            else
            {
                // TODO: find the right number of cells
                // all rows of the table
                for (row = 0; row < 50; row++)
                {
                    this.ClearCell(ximple, table, column, row);
                }
            }
        }

        private void ClearCell(Ximple ximple, Table table, Column column, int rowIndex)
        {
            ximple.Cells.Add(
                new XimpleCell
                    {
                        ColumnNumber = column.Index,
                        RowNumber = rowIndex,
                        TableNumber = table.Index,
                        Value = string.Empty
                    });
        }
    }
}
