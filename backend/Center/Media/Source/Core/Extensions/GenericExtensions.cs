// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The GenericExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The GenericExtensions.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Converts a Generic to a human readable string
        /// </summary>
        /// <param name="shell">the media shell</param>
        /// <param name="table">the table</param>
        /// <param name="column">the column</param>
        /// <param name="row">the row</param>
        /// <param name="language">the language</param>
        /// <returns>a human readable representation of the generic</returns>
        public static string ConvertToHumanReadable(
            IMediaShell shell,
            DataValue<int> table,
            DataValue<int> column,
            DataValue<int> row,
            DataValue<int> language)
        {
            string tableName;

            var columnName = string.Empty;
            var rowName = string.Empty;
            var languageName = string.Empty;

            var tableRef = shell.Dictionary.Tables.FirstOrDefault(t => t.Index == table.Value);

            if (tableRef != null)
            {
                tableName = tableRef.Name;
            }
            else
            {
                tableName = string.Format(MediaStrings.FormulaValuesToStringTableNotFound, table.Value);
            }

            if (tableRef != null)
            {
                var columnRef = tableRef.Columns.FirstOrDefault(c => c.Index == column.Value);
                if (columnRef != null)
                {
                    columnName = columnRef.Name;
                }
                else
                {
                    columnName = string.Format(MediaStrings.FormulaValuesToStringColumnNotFound, column.Value);
                }
            }

            if (tableRef != null && tableRef.MultiRow)
            {
                rowName = "[" + row.Value + "]";
            }

            if (tableRef != null && tableRef.MultiLanguage)
            {
                languageName = "{";

                var languageRef = shell.Dictionary.Languages.FirstOrDefault(
                    l => l.Index == language.Value);
                if (languageRef != null)
                {
                    languageName += languageRef.Name;
                }
                else
                {
                    languageName += string.Format(
                        MediaStrings.FormulaValuesToStringLanguageNotFound, language.Value);
                }

                languageName += "}";
            }

            return "$" + tableName + "." + columnName + rowName + languageName;
        }
    }
}
