// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WritableModelBase{TDelta}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WritableModelBase&lt;TDelta&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines the base class for writable models.
    /// </summary>
    /// <typeparam name="TDelta">The delta object.</typeparam>
    public abstract class WritableModelBase<TDelta> : IChangeTrackingModel<TDelta>
        where TDelta : DeltaBase
    {
        private bool isCommitted;

        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WritableModelBase{TDelta}"/> class.
        /// </summary>
        /// <param name="readableModelVersion">The readable model version.</param>
        protected WritableModelBase(Version readableModelVersion)
        {
            this.Version = readableModelVersion.Clone().Increment();
        }

        /// <summary>
        /// Occurs when a change is committed.
        /// </summary>
        public event EventHandler<ModelUpdatedEventArgs<TDelta>> Committed;

        /// <summary>
        /// Gets the version of the delta.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets or sets the delta object.
        /// </summary>
        protected TDelta Delta { get; set; }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public virtual void Commit()
        {
            if (this.isCommitted)
            {
                throw new InvalidOperationException("Model already committed");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException("WritableModelBase");
            }

            this.Delta.SetVersion(this.Version);
            this.isCommitted = true;
            this.OnCommitted();
        }

        /// <summary>
        /// Checks if this writable model has been changed since it was created.
        /// This will always return true if the model was not created from a readable model.
        /// </summary>
        /// <returns>
        /// True if this model has been changed or not yet committed to the manager.
        /// </returns>
        public abstract bool HasChanges();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Dispose(true);
        }

        /// <summary>
        /// Raises the <see cref="Committed"/> event.
        /// </summary>
        protected virtual void OnCommitted()
        {
            var e = new ModelUpdatedEventArgs<TDelta>(this.Delta);
            this.OnCommitted(e);
        }

        private void OnCommitted(ModelUpdatedEventArgs<TDelta> e)
        {
            var handler = this.Committed;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose unmanaged resources
            }

            this.Delta.Dispose();
            this.isDisposed = true;
        }
    }
}