// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranXimpleCache.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtranXimpleCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;

    /// <summary>
    /// Cache that stores the last instance of every Ximple cell and allows
    /// it to be dumped into a new ximple object.
    /// </summary>
    public class ProtranXimpleCache : XimpleCache, IManageable
    {
        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            var languages = new SortedList<int, int>();
            foreach (var cell in this.Dump().Cells)
            {
                languages[cell.LanguageNumber] = cell.LanguageNumber;
            }

            foreach (var language in languages.Values)
            {
                yield return parent.Factory.CreateManagementProvider(
                    string.Format("Language {0}", language), parent, new ManagementLanguage(language, this));
            }
        }

        private class ManagementLanguage : IManageable
        {
            private readonly int language;

            private readonly ProtranXimpleCache owner;

            public ManagementLanguage(int language, ProtranXimpleCache owner)
            {
                this.language = language;
                this.owner = owner;
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                var tables = new SortedList<int, int>();
                foreach (var cell in this.owner.Dump().Cells)
                {
                    if (cell.LanguageNumber != this.language)
                    {
                        continue;
                    }

                    tables[cell.TableNumber] = cell.TableNumber;
                }

                foreach (var table in tables.Values)
                {
                    yield return parent.Factory.CreateManagementProvider(
                        string.Format("Table {0}", table), parent, new ManagementTable(table, this.owner));
                }
            }
        }

        private class ManagementTable : IManageableTable
        {
            private readonly int table;

            private readonly ProtranXimpleCache owner;

            public ManagementTable(int table, ProtranXimpleCache owner)
            {
                this.table = table;
                this.owner = owner;
            }

            IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
            {
                var cells = new List<XimpleCell>();
                foreach (var cell in this.owner.Dump().Cells)
                {
                    if (cell.TableNumber != this.table)
                    {
                        continue;
                    }

                    cells.Add(cell);
                }

                var genTable = new DataTable();
                foreach (var ximpleCell in cells)
                {
                    for (int i = genTable.Columns.Count; i <= ximpleCell.ColumnNumber; i++)
                    {
                        genTable.Columns.Add(i.ToString(CultureInfo.InvariantCulture));
                    }

                    if (genTable.Rows.Count <= ximpleCell.RowNumber)
                    {
                        var row = new object[genTable.Columns.Count];
                        for (int i = genTable.Rows.Count; i <= ximpleCell.RowNumber; i++)
                        {
                            genTable.Rows.Add(row);
                        }
                    }

                    if (genTable.Rows[ximpleCell.RowNumber][ximpleCell.ColumnNumber].Equals(ximpleCell.Value))
                    {
                        continue;
                    }

                    genTable.Rows[ximpleCell.RowNumber][ximpleCell.ColumnNumber] = ximpleCell.Value;
                }

                foreach (DataRow row in genTable.Rows)
                {
                    var values = new List<ManagementProperty>();
                    foreach (DataColumn column in genTable.Columns)
                    {
                        values.Add(
                            new ManagementProperty<string>(column.ColumnName, Convert.ToString(row[column]), true));
                    }

                    yield return values;
                }
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                yield break;
            }
        }
    }
}
