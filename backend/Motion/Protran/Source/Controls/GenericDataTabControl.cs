// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericDataTabControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericDataTabControl type.
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
    public partial class GenericDataTabControl : TabControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDataTabControl"/> class.
        /// </summary>
        public GenericDataTabControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; set; }

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
                this.EnsurePage(cell.LanguageNumber);
            }

            foreach (TabPage page in this.TabPages)
            {
                if (page.Controls.Count == 0)
                {
                    continue;
                }

                var genericPage = page.Controls[0] as GenericDataTabPage;
                if (genericPage != null)
                {
                    genericPage.AddXimple(ximple);
                }
            }
        }

        /// <summary>
        /// Clears all information from the tables.
        /// </summary>
        public void ClearXimple()
        {
            this.TabPages.Clear();
        }

        private void EnsurePage(int languageNumber)
        {
            foreach (TabPage tabPage in this.TabPages)
            {
                if ((int)tabPage.Tag == languageNumber)
                {
                    return;
                }
            }

            var language = this.Dictionary == null
                               ? null
                               : this.Dictionary.Languages.Find(l => l.Index == languageNumber);

            var pageTitle = string.Format("[{0}] {1}", languageNumber, language == null ? string.Empty : language.Name);
            var page = new TabPage(pageTitle) { Tag = languageNumber };

            var genericPage = new GenericDataTabPage
                                  {
                                      Dictionary = this.Dictionary,
                                      Dock = DockStyle.Fill,
                                      LanguageNumber = languageNumber
                                  };
            page.Controls.Add(genericPage);

            int index = this.TabPages.Count;
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                if (!(this.TabPages[i].Tag is int))
                {
                    continue;
                }

                if (languageNumber < (int)this.TabPages[i].Tag)
                {
                    index = i;
                    break;
                }
            }

            // TabControl bug:
            // http://social.msdn.microsoft.com/forums/en-US/winforms/thread/5d10fd0c-1aa6-4092-922e-1fd7af979663
            // ReSharper disable UnusedVariable
            var handle = this.Handle;

            // ReSharper restore UnusedVariable
            this.TabPages.Insert(index, page);
        }

        private DataGridView CreateDataGrid(DataTable table)
        {
            var grid = new DataGridView
                {
                    DataSource = table,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToOrderColumns = false
                };
            grid.RowsAdded += (s, e) =>
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    row.HeaderCell.Value = row.Index.ToString(CultureInfo.InvariantCulture);
                }
            };
            return grid;
        }

        private class GenericDataTable : DataTable
        {
            private readonly Table genericTable;

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

            public string GetColumnName(int index)
            {
                var stringIndex = index.ToString(CultureInfo.InvariantCulture);
                if (this.genericTable == null)
                {
                    return stringIndex;
                }

                var column = this.genericTable.Columns.Find(c => c.Index == index);
                return column == null ? stringIndex : string.Format("[{0}] {1}", stringIndex, column.Name);
            }
        }
    }
}
