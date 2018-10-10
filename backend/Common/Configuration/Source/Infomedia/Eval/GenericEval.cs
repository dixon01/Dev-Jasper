// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEval.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericEval type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Eval
{
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Generic coordinate evaluation.
    /// </summary>
    public sealed partial class GenericEval
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEval"/> class.
        /// </summary>
        /// <param name="coordinate">
        /// The coordinate.
        /// </param>
        public GenericEval(GenericCoordinate coordinate)
            : this(coordinate.Language, coordinate.Table, coordinate.Column, coordinate.Row)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEval"/> class.
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
        public GenericEval(int language, int table, int column, int row)
        {
            this.Language = language;
            this.Table = table;
            this.Column = column;
            this.Row = row;
        }

        /// <summary>
        /// Converts this evaluation to a <see cref="GenericCoordinate"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="GenericCoordinate"/>.
        /// </returns>
        public GenericCoordinate ToCoordinate()
        {
            return new GenericCoordinate(this.Language, this.Table, this.Column, this.Row);
        }
    }
}