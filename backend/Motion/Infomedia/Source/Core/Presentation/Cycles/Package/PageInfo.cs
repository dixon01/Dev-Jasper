// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PageInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    /// <summary>
    /// Information about a page being shown by <see cref="PagePool"/>.
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageInfo"/> class.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="totalPages">
        /// The total pages.
        /// </param>
        /// <param name="rowOffset">
        /// The row offset.
        /// </param>
        public PageInfo(int pageIndex, int totalPages, int rowOffset)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = totalPages;
            this.RowOffset = rowOffset;
        }

        /// <summary>
        /// Gets the page index from 0 (page n out of <see cref="TotalPages"/>).
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets the row offset in the paged table.
        /// </summary>
        public int RowOffset { get; private set; }
    }
}