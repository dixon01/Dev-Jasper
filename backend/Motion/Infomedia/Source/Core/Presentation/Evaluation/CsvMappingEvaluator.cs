// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CsvMappingEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Csv;

    /// <summary>
    /// Tries to find a row by matching input columns (using '*' as a wildcard)
    /// and then formatting the found row with the given output format.
    /// If no matching row is found, the default value is used instead.
    /// </summary>
    public partial class CsvMappingEvaluator
    {
        private string fileName;

        partial void Initialize()
        {
            this.fileName = this.Context.Config.GetAbsolutePathRelatedToConfig(this.CsvMappingEval.FileName);
        }

        partial void UpdateValue()
        {
            using (var reader = new CsvReader(this.fileName))
            {
                // find the first line that matches the given "match" columns
                string[] line;
                while ((line = reader.GetCsvLine()) != null)
                {
                    if (this.LineMatches(line))
                    {
                        var args = ArrayUtil.ConvertAll(line, s => (object)s);
                        this.Value = string.Format(this.CsvMappingEval.OutputFormat, args);
                        return;
                    }
                }
            }

            // if no match is found, use the default value
            this.Value = this.HandlerDefaultValue.StringValue;
        }

        private bool LineMatches(string[] line)
        {
            if (line.Length == 0)
            {
                return false;
            }

            if (line[0].Length > 0 && line[0][0] == '#')
            {
                // commented line
                return false;
            }

            for (var i = 0; i < this.CsvMappingEval.Matches.Count; i++)
            {
                var column = this.CsvMappingEval.Matches[i].Column;
                var cell = line.Length <= column ? string.Empty : line[column];

                if (!this.Matches(cell, this.HandlersMatches[i].StringValue))
                {
                    // this line doesn't match
                    return false;
                }
            }

            return true;
        }

        private bool Matches(string fromFile, string fromInput)
        {
            if (fromFile.Equals("*"))
            {
                // support for wildcards
                return true;
            }

            return fromFile.Equals(fromInput, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}