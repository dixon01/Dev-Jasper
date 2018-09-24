// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expectation.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Expectation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ximple
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Expectation object for <see cref="XimpleReceiver"/>.
    /// </summary>
    public class Expectation
    {
        private readonly List<CellCheckBase> checks = new List<CellCheckBase>();

        private readonly List<CellCheckBase> checkedChecks = new List<CellCheckBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation"/> class.
        /// </summary>
        internal Expectation()
        {
        }

        /// <summary>
        /// Adds a static rule for a given 3D coordinate.
        /// </summary>
        /// <param name="table">
        /// The table to match.
        /// </param>
        /// <param name="column">
        /// The column to match.
        /// </param>
        /// <param name="row">
        /// The row to match.
        /// </param>
        /// <param name="value">
        /// The expected value.
        /// </param>
        /// <returns>
        /// this object to add more rules.
        /// </returns>
        public Expectation WithCell(int table, int column, int row, string value)
        {
            return this.WithCell(0, table, column, row, cellValue => value == cellValue, value, false);
        }

        /// <summary>
        /// Adds a dynamic rule for a given 3D coordinate.
        /// </summary>
        /// <param name="table">
        /// The table to match.
        /// </param>
        /// <param name="column">
        /// The column to match.
        /// </param>
        /// <param name="row">
        /// The row to match.
        /// </param>
        /// <param name="check">
        /// The check to be executed with the value of the cell.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// this object to add more rules.
        /// </returns>
        public Expectation WithCell(int table, int column, int row, Predicate<string> check, string value)
        {
            return this.WithCell(0, table, column, row, check, value, false);
        }

        /// <summary>
        /// Adds a static rule for a given 3D coordinate having the value
        /// expressed in hexadecimal format.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The expectation.
        /// </returns>
        public Expectation WithHexCell(int table, int column, int row, string value)
        {
            return this.WithCell(0, table, column, row, cellValue => value == cellValue, value, true);
        }

        /// <summary>
        /// The with cell version 2.
        /// </summary>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="row">
        ///     The row.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        /// The <see cref="Expectation"/>.
        /// </returns>
        public Expectation WithCellV2(int language, int table, int column, int row, string value)
        {
            return this.WithCell(language, table, column, row, cellValue => value == cellValue, value, false);
        }

        /// <summary>
        /// Adds a static rule for a given 3D coordinate having the value
        /// expressed in hexadecimal format.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The expectation.
        /// </returns>
        public Expectation WithHexCellV2(int language, int table, int column, int row, string value)
        {
            return this.WithCell(language, table, column, row, cellValue => value == cellValue, value, true);
        }

        /// <summary>
        /// Verifies this expectation against a given Ximple object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object.
        /// </param>
        /// <returns>
        /// true if all checks for this expectation are valid.
        /// </returns>
        public bool Verify(Ximple ximple)
        {
            foreach (var cellCheck in this.checks)
            {
                if (!cellCheck.Verify(ximple))
                {
                    return false;
                }

                this.checkedChecks.Add(cellCheck);
            }

            Console.WriteLine("Number of expectations: {0}", this.checkedChecks.Count);
            Console.WriteLine("Ximple count: {0}", ximple.Cells.Count);
            if (ximple.Cells.Count == this.checkedChecks.Count)
            {
                this.checkedChecks.Clear();
                return true;
            }

            this.checkedChecks.Clear();
            return false;
        }

        /// <summary>
        /// Returns the string representation of this object
        /// (all its fields).
        /// </summary>
        /// <returns>
        /// The string representation of this object
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            lock (this.checks)
            {
                foreach (var cellCheck in this.checks)
                {
                    stringBuilder.Append(cellCheck);
                    stringBuilder.Append(';');
                }
            }

            if (stringBuilder.Length == 0)
            {
                return "<none>";
            }

            stringBuilder.Length--;
            return stringBuilder.ToString();
        }

        private Expectation WithCell(
            int language, int table, int column, int row, Predicate<string> check, string value, bool isUtf8HexValue)
        {
            this.checks.Add(new CellCheck(language, table, column, row, check, value, isUtf8HexValue));
            return this;
        }
    }
}