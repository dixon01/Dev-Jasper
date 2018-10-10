// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagePool.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PagePool type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Motion.Infomedia.Core.Data;

    /// <summary>
    /// Pool of pages that contain one or more rows of a given generic table.
    /// </summary>
    public class PagePool : PoolBase<PageInfo>, IDisposable
    {
        private readonly int languageIndex;
        private readonly int tableIndex;

        private readonly int rowsPerPage;
        private readonly int maxPages;

        private readonly IPresentationContext context;

        private GenericTable table;

        private int currentPageIndex;
        private int currentPageNumber;

        private bool hadInvalidPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagePool"/> class.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <param name="rowsPerPage">
        /// The number of generic rows to show per page.
        /// </param>
        /// <param name="maxPages">
        /// The maximum number of pages.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public PagePool(int language, int table, int rowsPerPage, int maxPages, IPresentationContext context)
        {
            this.languageIndex = language;
            this.tableIndex = table;
            this.rowsPerPage = rowsPerPage;
            this.maxPages = maxPages;
            this.context = context;
            this.ResetPageIndex();

            context.Generic.AddTableChangeHandler(language, table, this.HandleTableChanged);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.context.Generic.RemoveTableChangeHandler(this.languageIndex, this.tableIndex, this.HandleTableChanged);
        }

        /// <summary>
        /// Moves to the next item.
        /// </summary>
        /// <param name="wrapAround">
        /// A flag indicating if the method should wrap around
        /// when it gets to the end of the pool or return false.
        /// </param>
        /// <returns>
        /// A flag indicating if there was a next item found.
        /// If this method returns false, <see cref="IPool{T}.CurrentItem"/> is null.
        /// </returns>
        public override bool MoveNext(bool wrapAround)
        {
            if (this.hadInvalidPage)
            {
                this.ResetPageIndex();
                this.hadInvalidPage = false;
            }

            // IMPORTANT: page numbers don't take into account empty pages,
            // page indexes do take them into account (used for row offset calculation)
            if (this.table == null)
            {
                var genericTable = this.context.Generic.GetTable(this.languageIndex, this.tableIndex);
                if (genericTable == null)
                {
                    this.CurrentItem = null;
                    return false;
                }

                this.table = genericTable;
            }

            var found = false;
            var maxPageNumber = this.currentPageNumber;
            for (int pageIndex = this.currentPageIndex + 1;
                 this.maxPages == -1 || maxPageNumber < this.maxPages;
                 pageIndex++)
            {
                int offset = pageIndex * this.rowsPerPage;

                if (this.table.RowCount <= offset)
                {
                    // no more pages available
                    break;
                }

                if (!this.HasFilledCells(offset, this.rowsPerPage))
                {
                    continue;
                }

                maxPageNumber++;
                if (!found)
                {
                    // found first page with some values
                    found = true;
                    this.currentPageIndex = pageIndex;
                    this.currentPageNumber = maxPageNumber;
                }
            }

            if (found)
            {
                this.CurrentItemValid = true;
                this.CurrentItem = new PageInfo(
                    this.currentPageNumber, maxPageNumber + 1, this.currentPageIndex * this.rowsPerPage);
                return true;
            }

            this.ResetPageIndex();
            if (wrapAround)
            {
                // try again (but without wrapping around; otherwise we get
                // in an infinite loop if there is no page available)
                return this.MoveNext(false);
            }

            this.CurrentItem = null;
            return false;
        }

        private bool HasFilledCells(int offset, int count)
        {
            for (int r = offset; r < offset + count; r++)
            {
                for (int c = 0; c < this.table.ColumnCount; c++)
                {
                    if (!string.IsNullOrEmpty(this.table.GetCellValue(this.table.GetRowNumber(r), c)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ResetPageIndex()
        {
            this.currentPageIndex = -1;
            this.currentPageNumber = -1;
        }

        private void HandleTableChanged(object sender, EventArgs e)
        {
            bool valid = false;
            if (this.table == null)
            {
                this.table = this.context.Generic.GetTable(this.languageIndex, this.tableIndex);
                if (this.table == null)
                {
                    return;
                }
            }

            if (this.currentPageIndex >= 0)
            {
                valid = this.HasFilledCells(this.currentPageIndex * this.rowsPerPage, this.rowsPerPage);
            }

            if (!valid)
            {
                valid = this.HasFilledCells(0, this.table.RowCount);
            }

            if (!valid)
            {
                this.hadInvalidPage = true;
            }

            this.CurrentItemValid = valid;
        }
    }
}