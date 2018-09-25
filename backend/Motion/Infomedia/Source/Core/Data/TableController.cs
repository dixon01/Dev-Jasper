// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TableController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Composer;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Controller for the generic tables. Provides methods to create, update or delete
    /// tables in their specific language. It uses the dictionary.xml to set a readable
    /// name for the tables and columns.
    /// </summary>
    public class TableController : ITableController, IManageable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<TableController>();

        private static readonly GenericCoordinate RemotePcStatusCoord = new GenericCoordinate(0, 0, 2, 0);

        private readonly Dictionary<int, GenericLanguage> languageControllers;

        private readonly ITimer initialRequestTimer;

        private readonly ITimer ximpleInactivityTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableController"/> class.
        /// </summary>
        /// <param name="ximpleInactivityConfig">
        /// The ximple Inactivity Configuration.
        /// </param>
        public TableController(XimpleInactivityConfig ximpleInactivityConfig)
        {
            this.languageControllers = new Dictionary<int, GenericLanguage>();

            MessageDispatcher.Instance.Subscribe<Ximple>((s, e) => this.UpdateData(e.Message));

            this.initialRequestTimer = TimerFactory.Current.CreateTimer("XimpleRequest");
            this.initialRequestTimer.Interval = TimeSpan.FromSeconds(10);
            this.initialRequestTimer.AutoReset = true;
            this.initialRequestTimer.Elapsed += this.OnInitialRequestTimeElapsed;

            this.ximpleInactivityTimer = TimerFactory.Current.CreateTimer("XimpleInactivity");
            this.ximpleInactivityTimer.AutoReset = false;
            this.ximpleInactivityTimer.Elapsed += (s, e) => this.SetRemotePcStatusToFalse();
            this.ximpleInactivityTimer.Interval = ximpleInactivityConfig.Timeout;
            if (ximpleInactivityConfig.AtStartup)
            {
                this.SetRemotePcStatusToFalse();
            }
            else
            {
                this.ximpleInactivityTimer.Enabled = true;
            }

            this.initialRequestTimer.Enabled = true;
        }

        /// <summary>
        /// Event that is fired whenever new data arrived from Protran.
        /// </summary>
        public event EventHandler<TableEventArgs> DataReceived;

        /// <summary>
        /// Gets the generic table for a given language and table index.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <returns>
        /// The generic table or null if it doesn't exist.
        /// </returns>
        public GenericTable GetTable(int languageIndex, int tableIndex)
        {
            GenericTable genericTable;
            GenericLanguage controller;
            if (!this.languageControllers.TryGetValue(languageIndex, out controller))
            {
                return null;
            }

            if (!controller.Tables.TryGetValue(tableIndex, out genericTable))
            {
                return null;
            }

            return genericTable;
        }

        /// <summary>
        /// Gets or creates the generic table for a given language and table index and
        /// adds the necessary cells.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <returns>
        /// The generic table which is created if needed.
        /// </returns>
        public GenericTable GetOrCreateTable(int languageIndex, int tableIndex)
        {
            GenericTable table;
            GenericLanguage controller;
            this.languageControllers.TryGetValue(languageIndex, out controller);

            if (controller == null)
            {
                controller = new GenericLanguage { Index = languageIndex };
                this.languageControllers.Add(languageIndex, controller);
            }

            controller.Tables.TryGetValue(tableIndex, out table);

            if (table == null)
            {
                table = new GenericTable(tableIndex);
                controller.Tables.Add(tableIndex, table);
            }

            return table;
        }

        /// <summary>
        /// Gets a specific cell value of a specific tableIndex.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <param name="columnIndex">
        /// The column index.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <returns>
        /// The cell value or null if the cell is not found.
        /// </returns>
        public string GetCellValue(int languageIndex, int tableIndex, int columnIndex, int rowIndex)
        {
            var genericTable = this.GetTable(languageIndex, tableIndex);
            return genericTable == null ? null : genericTable.GetCellValue(rowIndex, columnIndex);
        }

        /// <summary>
        /// Deletes all tables.
        /// </summary>
        public void ClearTables()
        {
            this.languageControllers.Clear();
        }

        /// <summary>
        /// Updates / expands the tables according to the given ximple.
        /// </summary>
        /// <param name="ximple">
        /// The new data.
        /// </param>
        public void UpdateData(Ximple ximple)
        {            
            var resetTimers = false;
            if (ximple.Cells.Count > 1)
            {
                Logger.Info("UpdateData has new Ximple resetTimers = true");
                resetTimers = true;
            }
            else if (ximple.Cells.Count == 1)
            {
                var cell = ximple.Cells[0];
                if (cell.LanguageNumber != RemotePcStatusCoord.Language
                    || cell.TableNumber != RemotePcStatusCoord.Table
                    || cell.ColumnNumber != RemotePcStatusCoord.Column
                    || cell.RowNumber != RemotePcStatusCoord.Row)
                {
                    // reset the timers only if the changed cell is not the remote PC status
                    resetTimers = true;
                    Logger.Info("UpdateData has new Ximple Cell.Count == 1, resetTimers = true");
                }
            }

            if (resetTimers)
            {
                this.initialRequestTimer.Enabled = false;
                this.ximpleInactivityTimer.Enabled = false;
            }

            var newData = new List<XimpleCell>();

            ximple = ximple.ConvertTo(Constants.Version2);
            foreach (var ximpleCell in ximple.Cells)
            {
                var table = this.GetOrCreateTable(ximpleCell.LanguageNumber, ximpleCell.TableNumber);
                Logger.Info("UpdateData Ximple Table {0}", table?.Name);
                if (table.SetCellValue(ximpleCell.RowNumber, ximpleCell.ColumnNumber, ximpleCell.Value))
                {
                    newData.Add(ximpleCell.Clone());
                }
            }

            if (resetTimers)
            {
                this.ximpleInactivityTimer.Enabled = true;
            }
            
            if (newData.Count == 0)
            {
                Logger.Warn("UpdateData got Ximple no new Data, no changes");
                this.LogCurrentTables(false);
                return;
            }

            this.LogCurrentTables(true);
            this.RaiseDataReceived(new TableEventArgs(newData));
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var languageController in this.languageControllers)
            {
                yield return
                    parent.Factory.CreateManagementProvider(
                        string.Format("Language {0}", languageController.Key),
                        parent,
                        new ManagementLanguage(languageController.Value));
            }
        }

        private void LogCurrentTables(bool hasNewValue)
        {
            if (!Logger.IsTraceEnabled)
            {
                return;
            }
            if (!hasNewValue)
            {
                Logger.Trace("No cells changed.");
            }

            Logger.Trace("Current tables in cache:");
            Logger.Trace("Number of languages: {0}", this.languageControllers.Count);
            foreach (var languageController in this.languageControllers)
            {
                Logger.Trace("  Language {0}:", languageController.Key);
                Logger.Trace("  Number of tables: {0}", languageController.Value.Tables.Count);
                foreach (var table in languageController.Value.Tables)
                {
                    Logger.Trace("    Table {0}:", table.Value.Name);
                    Logger.Trace("    Number of columns: {0}", table.Value.ColumnCount);
                    for (int c = 0; c < table.Value.ColumnCount; c++)
                    {
                        for (int r = 0; r < table.Value.RowCount; r++)
                        {
                            Logger.Trace(
                                "      Column {0}, Row {1}: {2}",
                                c,
                                r,
                                table.Value.GetCellValue(table.Value.GetRowNumber(r), c));
                        }
                    }
                }
            }
        }

        private void RaiseDataReceived(TableEventArgs e)
        {
            var handler = this.DataReceived;
            if (handler != null && e != null)
            {
                var count = e.NewValues?.Count;
                Logger.Info("RaiseDataReceived() Ximple XimpleCell Count == {0}", count);
                handler(this, e);
            }
        }

        private void OnInitialRequestTimeElapsed(object source, EventArgs eventArgs)
        {
            Logger.Info("OnInitialRequestTimeElapsed() Medi.Broadcast XimpleMessageRequest...");
            MessageDispatcher.Instance.Broadcast(new XimpleMessageRequest());
        }

        private void SetRemotePcStatusToFalse()
        {
            Logger.Info("SetRemotePcStatusToFalse() Setting Remote PC status to 0");
            var data = new Ximple();
            var cell = new XimpleCell
            {
                LanguageNumber = RemotePcStatusCoord.Language,
                TableNumber = RemotePcStatusCoord.Table,
                ColumnNumber = RemotePcStatusCoord.Column,
                RowNumber = RemotePcStatusCoord.Row,
                Value = "0"
            };

            data.Cells.Add(cell);
            this.UpdateData(data);
        }

        private class ManagementLanguage : IManageable
        {
            private readonly GenericLanguage languageController;

            public ManagementLanguage(GenericLanguage languageController)
            {
                this.languageController = languageController;
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                foreach (var table in this.languageController.Tables)
                {
                    yield return parent.Factory.CreateManagementProvider(
                    string.Format("Table {0}", table.Key), parent, new ManagementTable(table.Value));
                }
            }
        }

        private class ManagementTable : IManageableTable
        {
            private readonly GenericTable table;

            public ManagementTable(GenericTable table)
            {
                this.table = table;
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                yield break;
            }

            IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
            {
                for (int rowIndex = 0; rowIndex < this.table.RowCount; rowIndex++)
                {
                    var cells = new List<ManagementProperty>();
                    var rowNumber = this.table.GetRowNumber(rowIndex);
                    cells.Add(new ManagementProperty<int>("Row", rowNumber, true));
                    for (int column = 0; column < this.table.ColumnCount; column++)
                    {
                        cells.Add(
                            new ManagementProperty<string>(
                                this.table.GetColumnName(column),
                                this.table.GetCellValue(rowNumber, column),
                                true));
                    }

                    yield return cells;
                }
            }
        }
    }
}