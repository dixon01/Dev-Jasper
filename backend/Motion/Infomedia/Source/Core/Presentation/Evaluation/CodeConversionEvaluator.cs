// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeConversionEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CodeConversionEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;

    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Utility.Csv;

    /// <summary>
    /// Tries to find a row by matching the first two input columns (using '*' as a wildcard)
    /// of a CSV file and then choosing either the third or forth column.
    /// The first column is matched against special line [0, 10, 1, 0],
    /// the second column is matched against line number [0, 10, 0, 0].
    /// If no matching row is found, the line number is used instead.
    /// If a value is found, additionally # and & are replaced with the corresponding
    /// characters from the line number; # omits leading zeros, & includes them.
    /// </summary>
    /// <example>
    /// CSV   Line  Output
    /// N##   002   N2
    /// N&&   002   N02
    /// N##   103   N3
    /// N&&   103   N03
    /// N###  123   N123
    /// ##E   007   7E
    /// </example>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1603:DocumentationMustContainValidXml", Justification = "XML escapes make examples unreadable")]
    public partial class CodeConversionEvaluator
    {
        private string fileName;

        private GenericEvaluator lineNumberEvaluator;
        private GenericEvaluator specialLineEvaluator;

        private static bool Matches(string fromFile, string fromInput)
        {
            if (fromFile.Equals("*"))
            {
                // support for wildcards
                return true;
            }

            return fromFile.Equals(fromInput, StringComparison.InvariantCultureIgnoreCase);
        }

        private static void RemoveLeadingZeros(StringBuilder value)
        {
            while (value.Length > 0 && value[0] == '0')
            {
                value.Remove(0, 1);
            }
        }

        partial void Initialize()
        {
            this.fileName = this.Context.Config.GetAbsolutePathRelatedToConfig(this.CodeConversionEval.FileName);

            this.lineNumberEvaluator = new GenericEvaluator(new GenericEval(0, 10, 0, 0), this.Context);
            this.specialLineEvaluator = new GenericEvaluator(new GenericEval(0, 10, 1, 0), this.Context);

            this.lineNumberEvaluator.ValueChanged += this.EvaluatorOnValueChanged;
            this.specialLineEvaluator.ValueChanged += this.EvaluatorOnValueChanged;
        }

        partial void Deinitialize()
        {
            this.lineNumberEvaluator.ValueChanged -= this.EvaluatorOnValueChanged;
            this.specialLineEvaluator.ValueChanged -= this.EvaluatorOnValueChanged;

            this.lineNumberEvaluator.Dispose();
            this.specialLineEvaluator.Dispose();
        }

        private void EvaluatorOnValueChanged(object sender, EventArgs eventArgs)
        {
            this.UpdateValue();
        }

        partial void UpdateValue()
        {
            using (var reader = new CsvReader(this.fileName))
            {
                // find the first line that matches
                string[] line;
                while ((line = reader.GetCsvLine()) != null)
                {
                    if (this.LineMatches(line))
                    {
                        // the image path is in column 2, the text in column 3
                        this.Value = this.GetValue(line);
                        return;
                    }
                }
            }

            // if no match is found
            this.Value = this.CodeConversionEval.UseImage ? string.Empty : this.lineNumberEvaluator.StringValue;
        }

        private string GetValue(string[] line)
        {
            string valuePattern;
            if (this.CodeConversionEval.UseImage)
            {
                valuePattern = line[2];
            }
            else if (line.Length >= 4)
            {
                valuePattern = line[3];
            }
            else
            {
                return string.Empty;
            }

            if (valuePattern.IndexOfAny(new[] { '&', '#' }) < 0)
            {
                // no placeholders
                return valuePattern;
            }

            var lineNumber = this.lineNumberEvaluator.StringValue ?? string.Empty;
            lineNumber = new string('0', valuePattern.Length) + lineNumber;
            var value = new StringBuilder(valuePattern.Length);
            var lineNumberIndex = lineNumber.Length - 1;
            var hadLineNumber = false;
            var removeZeros = false;

            // StringBuilder.Insert(int, string) is used below for CF 3.5 compatibility
            for (int i = valuePattern.Length - 1; i >= 0; i--)
            {
                var c = valuePattern[i];
                if (c == '&' || c == '#')
                {
                    hadLineNumber = true;
                    removeZeros = c == '#';
                    value.Insert(0, lineNumber[lineNumberIndex--].ToString(CultureInfo.InvariantCulture));
                    continue;
                }

                if (hadLineNumber && removeZeros)
                {
                    RemoveLeadingZeros(value);
                }

                hadLineNumber = false;
                value.Insert(0, c.ToString(CultureInfo.InvariantCulture));
            }

            if (hadLineNumber && removeZeros)
            {
                RemoveLeadingZeros(value);
            }

            return value.ToString();
        }

        private bool LineMatches(string[] line)
        {
            if (line.Length < 3)
            {
                return false;
            }

            if (line[0].Length > 0 && line[0][0] == '#')
            {
                // commented line
                return false;
            }

            return Matches(line[0], this.specialLineEvaluator.StringValue)
                   && Matches(line[1], this.lineNumberEvaluator.StringValue);
        }
    }
}
