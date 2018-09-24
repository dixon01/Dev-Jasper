// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericDataTabPage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericDataTabPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Control that can show generic view data in tabs with the tables.
    /// </summary>
    internal partial class GenericDataTabPage : TabControl
    {
        private readonly Dictionary<int, DataGridView> tables = new Dictionary<int, DataGridView>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDataTabPage"/> class.
        /// </summary>
        public GenericDataTabPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the language number.
        /// </summary>
        public int LanguageNumber { get; set; }

        /// <summary>
        /// Add a ximple object to this control. This updates or creates tables.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        public void AddXimple(Ximple ximple)
        {
            foreach (var cell in ximple.Cells)
            {
                var dt = this.GetDataTable(cell);
                if (dt == null)
                {
                    continue;
                }

                dt.SetCellValue(cell);
            }
        }

        /// <summary>
        /// Clears all information from the tables.
        /// </summary>
        public void ClearXimple()
        {
            this.SuspendLayout();
            this.tables.Clear();
            for (int i = this.TabPages.Count - 1; i >= 0; i--)
            {
                if (!(this.TabPages[i].Tag is int))
                {
                    continue;
                }

                this.TabPages.RemoveAt(i);
            }

            this.ResumeLayout();
        }

        private GenericDataTable GetDataTable(XimpleCell cell)
        {
            if (cell.LanguageNumber != this.LanguageNumber)
            {
                return null;
            }

            DataGridView grid;
            if (this.tables.TryGetValue(cell.TableNumber, out grid))
            {
                return grid.DataSource as GenericDataTable;
            }

            Table genTable = null;
            if (this.Dictionary != null)
            {
                genTable = this.Dictionary.Tables.Find(t => t.Index == cell.TableNumber);
            }

            var table = new GenericDataTable(genTable, cell.TableNumber);
            grid = this.CreateDataGrid(table);
            var page = new TabPage(table.GetName()) { Tag = cell.TableNumber };
            page.Controls.Add(grid);

            int index = this.TabPages.Count;
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                if (!(this.TabPages[i].Tag is int))
                {
                    continue;
                }

                if (cell.TableNumber < (int)this.TabPages[i].Tag)
                {
                    index = i;
                    break;
                }
            }

            // TabControl bug:
            // http://social.msdn.microsoft.com/forums/en-US/winforms/thread/5d10fd0c-1aa6-4092-922e-1fd7af979663
            // ReSharper disable once UnusedVariable
            var handle = this.Handle;

            this.TabPages.Insert(index, page);
            this.tables.Add(cell.TableNumber, grid);
            return table;
        }

        private DataGridView CreateDataGrid(GenericDataTable table)
        {
            var grid = new DataGridView
                {
                    DataSource = table,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToOrderColumns = false,
                    RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
                };
            grid.RowsAdded += (s, e) =>
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    row.HeaderCell.Value = table.GetRowName(((DataRowView)row.DataBoundItem).Row);
                }
            };
            return grid;
        }

        private class GenericDataTable : DataTable
        {
            private readonly Table genericTable;

            private readonly Dictionary<DataRow, string> rowLabels = new Dictionary<DataRow, string>();

            private int minRowIndex = 0;

            private int nextRowIndex = 0;

            public GenericDataTable(Table genericTable, int tableNumber)
                : base(tableNumber.ToString(CultureInfo.InvariantCulture))
            {
                this.genericTable = genericTable;
            }

            public string GetName()
            {
                if (this.genericTable == null)
                {
                    return this.TableName;
                }

                return string.Format("[{0}] {1}", this.TableName, this.genericTable.Name);
            }

            public string GetRowName(DataRow row)
            {
                string name;
                this.rowLabels.TryGetValue(row, out name);
                return name;
            }

            public void SetCellValue(XimpleCell cell)
            {
                this.EnsureColumnIndex(cell.ColumnNumber);
                this.EnsureRowIndex(cell.RowNumber);
                int row = cell.RowNumber - this.minRowIndex;
                int column = cell.ColumnNumber;
                this.Rows[row][column] = cell.Value;
            }

            private string GetColumnName(int index)
            {
                var stringIndex = index.ToString(CultureInfo.InvariantCulture);
                if (this.genericTable == null)
                {
                    return stringIndex;
                }

                var column = this.genericTable.Columns.Find(c => c.Index == index);
                return column == null ? stringIndex : string.Format("[{0}] {1}", stringIndex, column.Name);
            }

            private void EnsureColumnIndex(int columnIndex)
            {
                for (int i = this.Columns.Count; i <= columnIndex; i++)
                {
                    this.Columns.Add(this.GetColumnName(i));
                }
            }

            private void EnsureRowIndex(int rowIndex)
            {
                for (int i = this.nextRowIndex; i <= rowIndex; i++)
                {
                    var dataRow = this.NewRow();
                    dataRow.ItemArray = new object[this.Columns.Count];
                    this.rowLabels[dataRow] = i.ToString(CultureInfo.InvariantCulture);
                    this.Rows.Add(dataRow);
                    this.nextRowIndex++;
                }

                for (int i = this.minRowIndex - 1; i >= rowIndex; i--)
                {
                    var dataRow = this.NewRow();
                    dataRow.ItemArray = new object[this.Columns.Count];
                    this.rowLabels[dataRow] = i.ToString(CultureInfo.InvariantCulture);
                    this.Rows.InsertAt(dataRow, 0);
                    this.minRowIndex--;
                }
            }
        }
    }
}
