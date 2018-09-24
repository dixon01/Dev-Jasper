// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagedPresentationContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PagedPresentationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Package
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Core.Data;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    /// <summary>
    /// Implementation of <see cref="IPresentationContext"/> that supports paging.
    /// The paging only affects the <see cref="Generic"/>'s methods.
    /// </summary>
    public class PagedPresentationContext : IPresentationContext, IPresentationGenericContext, IDisposable
    {
        private const int PagingStatusTable = 1;
        private const int NumberOfPagesColumn = 0;
        private const int PageNumberColumn = 1;

        private static readonly GenericCoordinate NumberOfPagesCoord = new GenericCoordinate
        {
            Language = 0,
            Table = PagingStatusTable,
            Column = NumberOfPagesColumn,
            Row = 0
        };

        private static readonly GenericCoordinate PageNumberCoord = new GenericCoordinate
        {
            Language = 0,
            Table = PagingStatusTable,
            Column = PageNumberColumn,
            Row = 0
        };

        private readonly CellChangeHandlers cellChangeHandlers;

        private readonly IPresentationContext parent;

        private Page page;

        private XimpleCell numPagesCell;
        private XimpleCell pageNumberCell;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedPresentationContext"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent context that is used for delegating calls to.
        /// </param>
        public PagedPresentationContext(IPresentationContext parent)
        {
            this.cellChangeHandlers = new CellChangeHandlers(this);

            this.parent = parent;
            this.parent.Updating += this.ParentOnUpdating;
            this.parent.Updated += this.ParentOnUpdated;
        }

        /// <summary>
        /// Event that is fired whenever the presentation is being updated,
        /// either because of a generic data change (see <see cref="IPresentationContext.Generic"/>) or
        /// a time change (see <see cref="IPresentationContext.Time"/>).
        /// This event will always be fired before updating and will later be
        /// followed by <see cref="IPresentationContext.Updated"/>.
        /// </summary>
        public event EventHandler Updating;

        /// <summary>
        /// Event that is fired when the presentation update process has finished.
        /// This event is always preceded by <see cref="IPresentationContext.Updating"/>.
        /// Registered listeners can update the <see cref="PresentationUpdatedEventArgs.Updates"/>
        /// to communicate changes to a screen.
        /// </summary>
        public event EventHandler<PresentationUpdatedEventArgs> Updated;

        /// <summary>
        /// Gets the config context which can be used to query configuration parameters.
        /// </summary>
        public IPresentationConfigContext Config
        {
            get
            {
                return this.parent.Config;
            }
        }

        /// <summary>
        /// Gets the generic context which can be used to query generic cell data and
        /// to subscribe to cell changes.
        /// </summary>
        public IPresentationGenericContext Generic
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the time context which can be used to get the current time and 
        /// to register to be notified when a given time is reached.
        /// </summary>
        public IPresentationTimeContext Time
        {
            get
            {
                return this.parent.Time;
            }
        }

        /// <summary>
        /// Changes the current page being shown and updates all 
        /// registered handlers if necessary.
        /// </summary>
        /// <param name="newPage">
        /// The new page.
        /// </param>
        public void ChangePage(Page newPage)
        {
            if (newPage == null)
            {
                return;
            }

            var oldPage = this.page;
            this.page = newPage;

            // update paging values
            this.numPagesCell = CoordToCell(NumberOfPagesCoord);
            this.numPagesCell.Value = newPage.TotalPages.ToString(CultureInfo.InvariantCulture);
            this.pageNumberCell = CoordToCell(PageNumberCoord);
            this.pageNumberCell.Value = (newPage.PageIndex + 1).ToString(CultureInfo.InvariantCulture);

            // let everybody know that the paging values have changed
            this.cellChangeHandlers.NotifyCellChange(new[] { this.numPagesCell, this.pageNumberCell });

            if (oldPage == null
                || oldPage.Layout.Name != newPage.Layout.Name
                || oldPage.LanguageIndex != newPage.LanguageIndex
                || oldPage.TableIndex != newPage.TableIndex
                || oldPage.RowOffset == newPage.RowOffset)
            {
                // we only notify if the row offset is the only thing that changed
                return;
            }

            var handler = this.cellChangeHandlers.GetTableChangeHandler(newPage.LanguageIndex, newPage.TableIndex);
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            foreach (var pair in this.cellChangeHandlers.GetCellChangeHandlers(newPage.LanguageIndex, newPage.TableIndex))
            {
                var origCoord = new GenericCoordinate
                                    {
                                        Language = pair.Key.Language,
                                        Table = pair.Key.Table,
                                        Column = pair.Key.Column,
                                        Row = pair.Key.Row
                                    };
                var oldCoord = this.ConvertCoordinates(origCoord, oldPage.RowOffset);
                var newCoord = this.FromPageCoordinates(origCoord);

                var oldValue = this.parent.Generic.GetGenericCellValue(oldCoord);
                var newValue = this.parent.Generic.GetGenericCellValue(newCoord);
                if (oldValue != newValue)
                {
                    // let the handler know that its value has changed
                    var cell = CoordToCell(newCoord);
                    cell.Value = newValue;
                    pair.Value(cell);
                }

                // add the handler to the right coordinate
                this.parent.Generic.RemoveCellChangeHandler(oldCoord, pair.Value);
                this.parent.Generic.AddCellChangeHandler(newCoord, pair.Value);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.parent.Updating -= this.ParentOnUpdating;
            this.parent.Updated -= this.ParentOnUpdated;
        }

        string IPresentationGenericContext.GetGenericCellValue(GenericCoordinate coord)
        {
            if (coord.Equals(NumberOfPagesCoord) && this.numPagesCell != null)
            {
                return this.numPagesCell.Value;
            }

            if (coord.Equals(PageNumberCoord) && this.pageNumberCell != null)
            {
                return this.pageNumberCell.Value;
            }

            coord = this.FromPageCoordinates(coord);
            return this.parent.Generic.GetGenericCellValue(coord);
        }

        void IPresentationGenericContext.AddCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.AddChangeHandler(coord, action);
            this.parent.Generic.AddCellChangeHandler(coord, action);
        }

        void IPresentationGenericContext.RemoveCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.RemoveChangeHandler(coord, action);
            this.parent.Generic.RemoveCellChangeHandler(coord, action);
        }

        void IPresentationGenericContext.AddTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.AddTableChangeHandler(language, table, handler);
            this.parent.Generic.AddTableChangeHandler(language, table, handler);
        }

        void IPresentationGenericContext.RemoveTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.RemoveTableChangeHandler(language, table, handler);
            this.parent.Generic.RemoveTableChangeHandler(language, table, handler);
        }

        GenericTable IPresentationGenericContext.GetTable(int languageIndex, int tableIndex)
        {
            return this.parent.Generic.GetTable(languageIndex, tableIndex);
        }

        private static XimpleCell CoordToCell(GenericCoordinate coord)
        {
            return new XimpleCell
                       {
                           LanguageNumber = coord.Language,
                           TableNumber = coord.Table,
                           ColumnNumber = coord.Column,
                           RowNumber = coord.Row
                       };
        }

        private GenericCoordinate FromPageCoordinates(GenericCoordinate coord)
        {
            return this.ConvertCoordinates(coord, this.page.RowOffset);
        }

        private GenericCoordinate ConvertCoordinates(GenericCoordinate coord, int rowOffset)
        {
            if (this.page == null
                || this.page.LanguageIndex != coord.Language
                || this.page.TableIndex != coord.Table)
            {
                // the coordinate doesn't have to be mapped
                return coord;
            }

            return new GenericCoordinate
            {
                Language = coord.Language,
                Table = coord.Table,
                Column = coord.Column,
                Row = coord.Row + rowOffset
            };
        }

        private void ParentOnUpdating(object sender, EventArgs e)
        {
            var handler = this.Updating;
            if (handler != null)
            {
                handler(this.Updating, e);
            }
        }

        private void ParentOnUpdated(object sender, PresentationUpdatedEventArgs e)
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this.Updating, e);
            }
        }
    }
}