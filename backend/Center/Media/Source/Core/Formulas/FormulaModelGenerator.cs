// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaModelGenerator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaModelGenerator.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Irony.Interpreter.Ast;

    /// <summary>
    /// The FormulaModelGenerator.
    /// </summary>
    public class FormulaModelGenerator
    {
        private readonly IMediaShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaModelGenerator"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        public FormulaModelGenerator(IMediaShell shell)
        {
            this.shell = shell;
        }

        /// <summary>
        /// generates the evaluation model from an AST
        /// </summary>
        /// <param name="root">the AST</param>
        /// <returns>the model</returns>
        public EvalDataViewModelBase Generate(AstNode root)
        {
            var visitor = new FormulaVisitor(this.shell, this.CreateGenericEvalDataViewModel);
            root.AcceptVisitor(visitor);

            return visitor.Model;
        }

        private GenericEvalDataViewModel CreateGenericEvalDataViewModel(
            string tableName,
            string columnName,
            string rowIndex,
            string languageName)
        {
            var dictionary = this.shell.Dictionary;
            GenericEvalDataViewModel result;

            var table = dictionary.Tables.FirstOrDefault(t => t.Name == tableName);

            if (table != null)
            {
                var column = table.Columns.FirstOrDefault(c => c.Name == columnName);

                if (column != null)
                {
                    result = new GenericEvalDataViewModel(this.shell)
                    {
                        Table = new DataValue<int>(table.Index),
                        Column = new DataValue<int>(column.Index),
                    };

                    this.SetRow(result, table, rowIndex);

                    this.SetLanguage(result, table, languageName);
                }
                else
                {
                    throw new ParseException(
                        string.Format(MediaStrings.FormulaParser_Error_GENERIC_NoSuchColumnFound, columnName));
                }
            }
            else
            {
                throw new ParseException(
                    string.Format(MediaStrings.FormulaParser_Error_GENERIC_NoSuchTableFound, tableName));
            }

            return result;
        }

        private void SetRow(GenericEvalDataViewModel result, TableDataViewModel table, string rowIndex)
        {
            int rowIndexValue;
            if (int.TryParse(rowIndex, out rowIndexValue))
            {
                if (table.MultiRow)
                {
                    result.Row = new DataValue<int>(rowIndexValue);
                }
                else if (rowIndexValue != 0)
                {
                    throw new ParseException(MediaStrings.FormulaParser_Error_GENERIC_TableIsNotMultirow);
                }
            }
            else if (table.MultiRow)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_GENERIC_TableIsMultiRow);
            }
        }

        private void SetLanguage(GenericEvalDataViewModel result, TableDataViewModel table, string languageName)
        {
            var language = this.shell.Dictionary.Languages.FirstOrDefault(l => l.Name == languageName);

            if (table.MultiLanguage)
            {
                if (language != null)
                {
                    result.Language = new DataValue<int>(language.Index);
                }
                else
                {
                    throw new ParseException(MediaStrings.FormulaParser_Error_GENERIC_TablIsMultiLanguage);
                }
            }
            else
            {
                if (language != null)
                {
                    throw new ParseException(MediaStrings.FormulaParser_Error_GENERIC_TablIsNotMultiLanguage);
                }
            }
        }
    }
}
