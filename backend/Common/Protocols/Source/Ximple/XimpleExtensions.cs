// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleExtensions.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Ximple
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Gorba.Common.Protocols.Ximple.Generic;

    using NLog;

    /// <summary>The ximple extensions.</summary>
    public static class XimpleExtensions
    {
        #region Public Methods and Operators

        /// <summary>The create ximple cell.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="table">The table.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The value.</param>
        /// <param name="row">The row</param>
        /// <param name="language">The language or default of Zero.</param>
        /// <returns>The <see cref="XimpleCell"/>A new XimpleCell object</returns>
        /// <exception cref="ArgumentException"></exception>
        public static XimpleCell CreateXimpleCell<T>(
            this Dictionary dictionary,
            Table table,
            string columnName,
            T value,
            int row = 0,
            int language = 0)
        {
            var tableIndex = table.Index;
            var column = dictionary.FindColumn(tableIndex.ToString(), columnName);
            if (column == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid Column name {0} for Table with Index {1}, Language = {2}", columnName, tableIndex, language));
            }

            if (typeof(T) == typeof(Boolean))
            {
                // THe LAM unit wants One or Zero strings for true | false this 9/15/2016
                return new XimpleCell(Convert.ToBoolean(value) ? "1" : "0", tableIndex, row, column.Index, language);
            }

            return new XimpleCell(value != null ? value.ToString() : string.Empty, tableIndex, row, column.Index, language);
        }

        /// <summary>Find dictionary column.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="tableNumberOrNumber">The table number or table index number.</param>
        /// <param name="columNameOrNumber">The colum name or number.</param>
        /// <returns>The <see cref="Column"/>Column entity from the dictionary or null.</returns>
        public static Column FindColumn(this Dictionary dictionary, string tableNumberOrNumber, string columNameOrNumber)
        {
            Column column = null;
            var table = dictionary.GetTableForNameOrNumber(tableNumberOrNumber);
            if (table != null)
            {
                column = table.GetColumnForNameOrNumber(columNameOrNumber);
            }

            return column;
        }

        /// <summary>Find column in table.</summary>
        /// <param name="table">The dictionary table.</param>
        /// <param name="columnNameOrNumber">The column name or column number.</param>
        /// <returns>The <see cref="Column"/>Column or null</returns>
        public static Column FindColumn(this Table table, string columnNameOrNumber)
        {
            return table.GetColumnForNameOrNumber(columnNameOrNumber);
        }

        /// <summary>Find ximple cell from the collection by table number.</summary>
        /// <param name="column">The column.</param>
        /// <param name="cells">The cells.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <returns>The <see cref="XimpleCell"/>.</returns>
        public static XimpleCell FindFirstXimpleCell(Column column, IEnumerable<XimpleCell> cells, int tableNumber = 0)
        {
            XimpleCell ximpleCell = null;

            if (cells != null && column != null)
            {
                var tableIndex = tableNumber == 0 ? cells.First().TableNumber : tableNumber;
                ximpleCell = cells.FirstOrDefault(m => m.TableNumber == tableIndex && m.ColumnNumber == column.Index);
            }

            return ximpleCell;
        }

        /// <summary>The find first ximple cell value for a given Table and Column.</summary>
        /// <param name="cells">The collection of XimpleCell cells.</param>
        /// <param name="table">The valid Ximple Table.</param>
        /// <param name="columnNameOrNumber">The column Name Or Number.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is null. </exception>
        /// <exception cref="ArgumentException">Invalid Column Name or Id.</exception>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FindFirstXimpleCellValue(this IEnumerable<XimpleCell> cells, Table table, string columnNameOrNumber)
        {
            var column = FindColumn(table, columnNameOrNumber);
            if (column == null)
            {
                throw new ArgumentException(string.Format("Invalid column name {0} for Table Name {1}", columnNameOrNumber, table.Name));
            }

            var cell = FindFirstXimpleCell(column, cells, table.Index);
            return cell != null ? cell.Value : string.Empty;
        }

        /// <summary>Find table and column.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="tableNameOrNumber">The table name or number.</param>
        /// <param name="columnNameOrNumber">The column name or number.</param>
        /// <param name="tableIndex">The table index.</param>
        /// <returns>The <see cref="Column"/>.</returns>
        public static Column FindTableAndColumn(this Dictionary dictionary, string tableNameOrNumber, string columnNameOrNumber, out int tableIndex)
        {
            tableIndex = 0;
            var table = dictionary.FindXimpleTable(tableNameOrNumber);
            if (table != null)
            {
                tableIndex = table.Index;
                return table.FindColumn(columnNameOrNumber);
            }

            return null;
        }

        /// <summary>Find ximple table.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <returns>The <see cref="Table"/>.</returns>
        public static Table FindXimpleTable(this Dictionary dictionary, int tableNumber)
        {
            return dictionary.FindXimpleTable(tableNumber.ToString());
        }

        /// <summary>Find ximple table in the dictionary by table name or number.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="tableNumberOrNumber">The table number or number.</param>
        /// <returns>The <see cref="Table"/>.</returns>
        public static Table FindXimpleTable(this Dictionary dictionary, string tableNumberOrNumber)
        {
            return dictionary.Tables.FirstOrDefault(m => m.Index.ToString() == tableNumberOrNumber || m.Name == tableNumberOrNumber);
        }

        /// <summary>The get ximple cell value.</summary>
        /// <param name="cells">The cells.</param>
        /// <param name="table">The table.</param>
        /// <param name="columnNameOrNumber">The column name or number.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetXimpleCellValue<T>(this IEnumerable<XimpleCell> cells, Table table, string columnNameOrNumber, T defaultValue = default(T))
        {
            var column = FindColumn(table, columnNameOrNumber);
            if (column == null)
            {
                throw new ArgumentException(string.Format("Invalid column name {0} for Table Name {1}", columnNameOrNumber, table.Name));
            }

            var cell = FindFirstXimpleCell(column, cells, table.Index);
            return cell != null ? cell.TryGetValue<T>(defaultValue) : defaultValue;
        }

        /// <summary>
        /// Set a Ximple Cell's Value
        /// </summary>
        /// <typeparam name="T">Value Type</typeparam>
        /// <param name="cells">The cells.</param>
        /// <param name="table">The table.</param>
        /// <param name="columnNameOrNumber">The column name or number.</param>
        /// <param name="newValue">The new Cell value</param>
        public static void SetXimpleCellValue<T>(this IEnumerable<XimpleCell> cells, Table table, string columnNameOrNumber, T newValue)
        {
            var column = FindColumn(table, columnNameOrNumber);
            if (column == null)
            {
                throw new ArgumentException(string.Format("Invalid column name {0} for Table Name {1}", columnNameOrNumber, table.Name));
            }

            var cell = FindFirstXimpleCell(column, cells, table.Index);
            if (cell != null)
            {
                cell.Value = newValue.ToString();
            }
        }

        /// <summary>Try and get a boolean value from the XimpleCell.</summary>
        /// <param name="cell">The cell where the value can be true | false or 1 | 0</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool TryGetBool(this XimpleCell cell)
        {
            bool boolVal;
            int val;
            if (!int.TryParse(cell.Value, out val))
            {
                bool.TryParse(cell.Value, out boolVal);
            }
            else
            {
                boolVal = val != 0;
            }

            return boolVal;
        }

        /// <summary>The try get value.</summary>
        /// <param name="cell">The cell.</param>
        /// <param name="defaultValue">The default Value.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name and specified parameters. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.-or- <paramref name="types"/> is null.-or- One of the elements in <paramref name="types"/> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="types"/> is multidimensional. </exception>
        /// <exception cref="TargetException">NoteIn the .NET for Windows Store apps or the Portable Class Library, catch <see cref="T:System.Exception"/> instead.The <paramref name="obj"/> parameter is null and the method is not static.-or- The method is not declared or inherited by the class of <paramref name="obj"/>. -or-A static constructor is invoked, and <paramref name="obj"/> is neither null nor an instance of the class that declared the constructor.</exception>
        /// <exception cref="TargetParameterCountException">The <paramref name="parameters"/> array does not have the correct number of arguments. </exception>
        /// <exception cref="MethodAccessException">NoteIn the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.MemberAccessException"/>, instead.The caller does not have permission to execute the method or constructor that is represented by the current instance. </exception>
        public static T TryGetValue<T>(this XimpleCell cell, T defaultValue = default(T))
        {
            if (cell == null)
            {
                return default(T);
            }

            var val = cell.Value;

            try
            {
                if (typeof(T) == typeof(string))
                {
                    return val == null ? (T)((object)string.Empty) : (T)((object)val);
                }

                var m = typeof(T).GetMethod("Parse", new[] { typeof(string) });
                if (m != null)
                {
                    return (T)m.Invoke(null, new object[] { val });
                }
            }
            catch (TargetInvocationException)
            {
                Debug.WriteLine("Failed to convert Type to " + typeof(T));
                if (typeof(T) == typeof(bool))
                {
                    return (T)((object)(val == "1"));
                }

                // no default value thow exception
                if (defaultValue == null)
                {
                    throw;
                }
            }

            return defaultValue;
        }

        #endregion
    }
}