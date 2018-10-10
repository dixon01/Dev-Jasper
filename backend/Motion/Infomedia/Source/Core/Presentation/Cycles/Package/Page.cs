// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Page type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// Information about a page being shown.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class that
        /// only has a single page (no paging is done).
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="duration">
        /// The time to show this page.
        /// </param>
        public Page(LayoutBase layout, TimeSpan duration)
            : this(layout, duration, 0, 1, 0, -1, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="layout">
        ///   The layout.
        /// </param>
        /// <param name="duration">
        ///   The time to show this page.
        /// </param>
        /// <param name="pageIndex">
        ///   The page index from 0 (page n out of <see cref="TotalPages"/>).
        /// </param>
        /// <param name="totalPages">
        ///   The total number of pages.
        /// </param>
        /// <param name="rowOffset">
        ///   The row offset in the given language and table.
        /// </param>
        /// <param name="languageIndex">
        ///   The language index of the language affected by paging.
        /// </param>
        /// <param name="tableIndex">
        ///   The table index of the table affected by paging.
        /// </param>
        public Page(LayoutBase layout, TimeSpan duration, int pageIndex, int totalPages, int rowOffset, int languageIndex, int tableIndex)
        {
            this.Layout = layout;
            this.Duration = duration;
            this.TableIndex = tableIndex;
            this.LanguageIndex = languageIndex;
            this.RowOffset = rowOffset;
            this.TotalPages = totalPages;
            this.PageIndex = pageIndex;
        }

        /// <summary>
        /// Gets the layout.
        /// </summary>
        public LayoutBase Layout { get; private set; }

        /// <summary>
        /// Gets the time to show this page.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets the page index from 0 (page n out of <see cref="TotalPages"/>).
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets the row offset in the given <see cref="LanguageIndex"/> and <see cref="TableIndex"/>.
        /// </summary>
        public int RowOffset { get; private set; }

        /// <summary>
        /// Gets the language index of the language affected by paging.
        /// </summary>
        public int LanguageIndex { get; private set; }

        /// <summary>
        /// Gets the table index of the table affected by paging.
        /// </summary>
        public int TableIndex { get; private set; }
    }
}