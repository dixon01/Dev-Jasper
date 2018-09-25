// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleCache.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Utils
{
    using System.Collections.Generic;

    /// <summary>
    /// Cache that stores the last instance of every Ximple cell and allows
    /// it to be dumped into a new ximple object.
    /// </summary>
    public class XimpleCache
    {
        private readonly Dictionary<string, XimpleCell> cells = new Dictionary<string, XimpleCell>();

        /// <summary>
        /// Adds all cells of the given ximple object to this class.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        public void Add(Ximple ximple)
        {
            foreach (var ximpleCell in ximple.Cells)
            {
                this.Add(ximpleCell);
            }
        }

        /// <summary>
        /// Adds a cell to this class.
        /// </summary>
        /// <param name="ximpleCell">
        /// The ximple cell.
        /// </param>
        public void Add(XimpleCell ximpleCell)
        {
            var key = CreateKey(ximpleCell);
            this.cells[key] = ximpleCell.Clone();
        }

        /// <summary>
        /// Removes a cell from this class.
        /// </summary>
        /// <param name="ximpleCell">
        /// The ximple cell. Only the coordinates of the given cell are checked.
        /// </param>
        /// <returns>
        /// True if the cell was found and removed.
        /// </returns>
        public bool Remove(XimpleCell ximpleCell)
        {
            var key = CreateKey(ximpleCell);
            return this.cells.Remove(key);
        }

        /// <summary>
        /// Creates a new Ximple object containing all cells that were
        /// previously set in <see cref="Add(Gorba.Common.Protocols.Ximple.Ximple)"/>.
        /// </summary>
        /// <returns>
        /// the new Ximple object.
        /// </returns>
        public Ximple Dump()
        {
            var ximple = new Ximple();
            ximple.Cells.AddRange(this.cells.Values);
            return ximple;
        }

        /// <summary>
        /// Clear the ximple cache
        /// </summary>
        public void Clear()
        {
            this.cells.Clear();
        }

        private static string CreateKey(XimpleCell ximpleCell)
        {
            var key = string.Format(
                "{0}#{1}#{2}#{3}",
                ximpleCell.TableNumber,
                ximpleCell.ColumnNumber,
                ximpleCell.RowNumber,
                ximpleCell.LanguageNumber);
            return key;
        }
    }
}
