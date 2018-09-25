// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterPresentationContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterPresentationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Master
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Core.Data;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The master presentation context.
    /// </summary>
    internal class MasterPresentationContext : IPresentationContext, IDisposable
    {
        private readonly MasterPresentationTimeContext timeContext = new MasterPresentationTimeContext();
        private readonly ITableController tableController;
        private readonly MasterPresentationGenericContext genericContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPresentationContext"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public MasterPresentationContext(IPresentationConfigContext configContext)
        {
            this.Config = configContext;
            this.tableController = ServiceLocator.Current.GetInstance<ITableController>();
            this.genericContext = new MasterPresentationGenericContext(this.tableController);
            this.tableController.DataReceived += this.TableControllerOnDataReceived;
            this.timeContext.NextTimeReached += this.TimeContextOnNextTimeReached;
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
        /// Event that is fired whenever the generic cells of the underlying
        /// table controller have changed. The listener to this 
        /// event should call <see cref="NotifyCellsChanged"/> to make sure all 
        /// registered handlers will be actually notified about this change.
        /// </summary>
        public event EventHandler<TableEventArgs> CellsChanged;

        /// <summary>
        /// Event that is fired whenever the a timer requested by <see cref="Time"/> 
        /// expires. The listener to this event should call <see cref="NotifyTimeReached"/> 
        /// to make sure all registered handlers will be actually notified about this change.
        /// </summary>
        public event EventHandler<TimeEventArgs> NextTimeReached;

        /// <summary>
        /// Gets the config context which can be used to query configuration parameters.
        /// </summary>
        public IPresentationConfigContext Config { get; private set; }

        /// <summary>
        /// Gets the generic context which can be used to query generic cell data and
        /// to subscribe to cell changes.
        /// </summary>
        public IPresentationGenericContext Generic
        {
            get
            {
                return this.genericContext;
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

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseUpdating(EventArgs e)
        {
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
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Notifies all handlers of a cell change.
        /// This method should only be called while handling <see cref="CellsChanged"/>.
        /// </summary>
        /// <param name="newValues">
        /// The new values.
        /// </param>
        public void NotifyCellsChanged(IEnumerable<XimpleCell> newValues)
        {
            this.timeContext.UpdateUtcNow();
            this.genericContext.NotifyCellChange(newValues);
        }

        /// <summary>
        /// Notifies all handlers of an expired timer.
        /// This method should only be called while handling <see cref="NextTimeReached"/>.
        /// </summary>
        /// <param name="time">
        /// The time from <see cref="TimeEventArgs.Time"/>.
        /// </param>
        public void NotifyTimeReached(DateTime time)
        {
            this.timeContext.NotifyTimeReached(time);
        }

        /// <summary>
        /// Starts this context and all related objects.
        /// </summary>
        public void Start()
        {
            this.timeContext.Start();
        }

        /// <summary>
        /// Stops this context and all related objects.
        /// </summary>
        public void Stop()
        {
            this.timeContext.Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.timeContext.Dispose();

            this.timeContext.NextTimeReached -= this.TimeContextOnNextTimeReached;
            this.tableController.DataReceived -= this.TableControllerOnDataReceived;
        }

        private void TableControllerOnDataReceived(object sender, TableEventArgs e)
        {
            var handler = this.CellsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void TimeContextOnNextTimeReached(object sender, TimeEventArgs e)
        {
            var handler = this.NextTimeReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}