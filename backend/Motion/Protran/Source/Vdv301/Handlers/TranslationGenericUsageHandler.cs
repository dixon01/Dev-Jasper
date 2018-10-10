// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationGenericUsageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TranslationGenericUsageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Generic usage handler that chooses the language depending on the
    /// language mapping in <see cref="Vdv301ProtocolConfig.Languages"/>.
    /// </summary>
    public class TranslationGenericUsageHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<TranslationGenericUsageHandler>();

        private readonly GenericUsage usage;

        private readonly int tableNumber;
        private readonly int columnNumber;

        private readonly Dictionary<string, int> languageNumbers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGenericUsageHandler"/> class.
        /// </summary>
        /// <param name="usage">
        /// The generic usage.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary with the table information.
        /// </param>
        /// <param name="languages">
        /// The language mappings to use.
        /// </param>
        public TranslationGenericUsageHandler(
            GenericUsage usage, Dictionary dictionary, IEnumerable<LanguageMappingConfig> languages)
        {
            if (usage == null)
            {
                return;
            }

            var table = dictionary.GetTableForNameOrNumber(usage.Table);
            if (table == null)
            {
                return;
            }

            var column = table.GetColumnForNameOrNumber(usage.Column);
            if (column == null)
            {
                return;
            }

            this.languageNumbers = new Dictionary<string, int>();
            foreach (var mapping in languages)
            {
                var language = dictionary.GetLanguageForNameOrNumber(mapping.XimpleOutput);
                int languageNumber;
                if (language != null)
                {
                    languageNumber = language.Index;
                }
                else
                {
                    ParserUtil.TryParse(usage.Language, out languageNumber);
                }

                this.languageNumbers.Add(mapping.Vdv301Input, languageNumber);
            }

            this.tableNumber = table.Index;
            this.columnNumber = column.Index;
            this.usage = usage;
        }

        /// <summary>
        /// Adds <see cref="XimpleCell"/>s to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cells should be added.
        /// </param>
        /// <param name="values">
        /// The values of the cells.
        /// </param>
        /// <param name="rowIndex">
        /// The row index for indexed usages.
        /// </param>
        /// <returns>
        /// The cells that were added or an empty list if no cell was added
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <see cref="rowIndex"/> is less than zero.
        /// </exception>
        public List<XimpleCell> AddCells(Ximple ximple, TranslatedText[] values, int rowIndex)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rowIndex", "Row can't be negative.");
            }

            if (this.usage == null)
            {
                return null;
            }

            var rowName = string.Format(this.usage.Row, rowIndex + this.usage.RowOffset);
            int row;
            if (!ParserUtil.TryParse(rowName, out row))
            {
                Logger.Warn("Couldn't parse row number: {0} for {1}", rowName, this.usage);
                return null;
            }

            var cells = new List<XimpleCell>(values.Length);
            foreach (var translatedText in values)
            {
                int languageNumber;
                if (!this.languageNumbers.TryGetValue(translatedText.Language, out languageNumber))
                {
                    Logger.Debug("Couldn't find language for translation: {0}", translatedText.Language);
                    continue;
                }

                var cell = new XimpleCell
                               {
                                   LanguageNumber = languageNumber,
                                   TableNumber = this.tableNumber,
                                   ColumnNumber = this.columnNumber,
                                   RowNumber = row,
                                   Value = translatedText.Text
                               };
                cells.Add(cell);
                ximple.Cells.Add(cell);
            }

            return cells;
        }
    }
}