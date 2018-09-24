// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationContextMock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationContextMock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Composer;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Data;
    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;

    /// <summary>
    /// Testing mock of <see cref="IPresentationContext"/>.
    /// </summary>
    internal class PresentationContextMock
        : IPresentationContext, IPresentationConfigContext, IPresentationGenericContext
    {
        private readonly CellChangeHandlers cellChangeHandlers;

        private readonly TableController tableController;

        private readonly ManualTimeProvider timeProvider =
            new ManualTimeProvider(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        private readonly MasterPresentationTimeContext timeContext;

        private readonly TestableTimerFactory.Timer contextTimer;

        private bool updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationContextMock"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration of this context.
        /// </param>
        public PresentationContextMock(InfomediaConfig config)
        {
            this.cellChangeHandlers = new CellChangeHandlers(this);

            this.Config = config;
            this.tableController = new TableController(new XimpleInactivityConfig());

            // prepare the time context
            var oldTimeProvider = TimeProvider.Current;
            TimeProvider.Current = this.timeProvider;

            var oldTimerFactory = TimerFactory.Current;
            var timerFactory = new TestableTimerFactory();
            TimerFactory.Current = timerFactory;

            this.timeContext = new MasterPresentationTimeContext();
            TimeProvider.Current = oldTimeProvider;
            TimerFactory.Current = oldTimerFactory;

            this.contextTimer = timerFactory[this.timeContext.GetType().Name].First();

            this.timeContext.NextTimeReached += (s, e) => this.timeContext.NotifyTimeReached(e.Time);
            this.timeContext.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationContextMock"/> class.
        /// </summary>
        /// <param name="layoutConfigs">
        /// The layout configurations that can be queried.
        /// </param>
        public PresentationContextMock(params LayoutConfig[] layoutConfigs)
            : this(new InfomediaConfig { Layouts = new List<LayoutConfig>(layoutConfigs) })
        {
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
        /// Gets the configuration of the currently running presentation.
        /// </summary>
        public InfomediaConfig Config { get; private set; }

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
                return this.timeContext;
            }
        }

        IPresentationConfigContext IPresentationContext.Config
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Adds time to the <see cref="Time"/> context and calls any handler
        /// that has expired during that time.
        /// This method allows for timing tests without actually using the system time.
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public void AddTime(TimeSpan duration)
        {
            this.timeProvider.AddTime(duration);
            this.RaiseUpdating(EventArgs.Empty);
            this.timeContext.UpdateUtcNow();
            this.contextTimer.RaiseElapsed();
            this.RaiseUpdated(new PresentationUpdatedEventArgs());
        }

        /// <summary>
        /// Sets time of the <see cref="Time"/> context and calls any handler
        /// that might have expired.
        /// This method allows for timing tests without actually using the system time.
        /// </summary>
        /// <param name="time">
        /// The new virtual system time.
        /// </param>
        public void SetTime(DateTime time)
        {
            this.timeProvider.SetUtcNow(time);
            this.RaiseUpdating(EventArgs.Empty);
            this.timeContext.UpdateUtcNow();
            this.contextTimer.RaiseElapsed();
            this.RaiseUpdated(new PresentationUpdatedEventArgs());
        }

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseUpdating(EventArgs e)
        {
            if (this.updating)
            {
                throw new NotSupportedException("Can't call RaiseUpdating() twice without RaiseUpdated() inbetween");
            }

            this.updating = true;
            var handler = this.Updating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Updated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseUpdated(PresentationUpdatedEventArgs e)
        {
            if (!this.updating)
            {
                throw new NotSupportedException("Can't call RaiseUpdated() twice without RaiseUpdating() inbetween");
            }

            this.updating = false;
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Sets the value of a given cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetCellValue(GenericCoordinate coord, string value)
        {
            var cell = new XimpleCell
                {
                    LanguageNumber = coord.Language,
                    TableNumber = coord.Table,
                    ColumnNumber = coord.Column,
                    RowNumber = coord.Row,
                    Value = value
                };
            this.tableController.UpdateData(new Ximple(Constants.Version2) { Cells = { cell } });

            this.cellChangeHandlers.NotifyCellChange(new[] { cell });
        }

        /// <summary>
        /// Converts the given file name into an absolute name
        /// relative to the presentation config file.
        /// </summary>
        /// <param name="filename">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        public string GetAbsolutePathRelatedToConfig(string filename)
        {
            return filename;
        }

        /// <summary>
        /// Gets the value of a generic cell.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <returns>
        /// The cell value or null if the cell is not found.
        /// </returns>
        public string GetGenericCellValue(GenericCoordinate coord)
        {
            return this.tableController.GetCellValue(coord.Language, coord.Table, coord.Column, coord.Row);
        }

        /// <summary>
        /// Gets the generic table for a given language and table index.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <returns>
        /// The generic table or null if it doesn't exist.
        /// </returns>
        public GenericTable GetTable(int languageIndex, int tableIndex)
        {
            return this.tableController.GetTable(languageIndex, tableIndex);
        }

        /// <summary>
        /// Adds a handler for a given coordinate.
        /// The handler will be called when the cell at the given
        /// coordinate changes its value.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <param name="action">
        ///   The action to be performed.
        /// </param>
        public void AddCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.AddChangeHandler(coord, action);
        }

        /// <summary>
        /// Removes a handler previously registered with <see cref="AddCellChangeHandler"/>.
        /// </summary>
        /// <param name="coord">
        ///   The coordinate.
        /// </param>
        /// <param name="action">
        ///   The action.
        /// </param>
        public void RemoveCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.RemoveChangeHandler(coord, action);
        }

        /// <summary>
        /// Adds a handler for a given table (in the given language).
        /// The handler will be called when any cell in the given table changes.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <param name="handler">
        /// The handler to be called.
        /// </param>
        public void AddTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.AddTableChangeHandler(language, table, handler);
        }

        /// <summary>
        /// Removes a handler previously registered with <see cref="AddTableChangeHandler"/>.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <param name="handler">
        /// The handler to be removed.
        /// </param>
        public void RemoveTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.RemoveTableChangeHandler(language, table, handler);
        }
    }
}