// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadableModelBase{TDelta}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadableModelBase&lt;TDelta&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a base for readable objects.
    /// </summary>
    /// <typeparam name="TDelta">The type of the delta object.</typeparam>
    public abstract class ReadableModelBase<TDelta> : ReadableModelBase
        where TDelta : DeltaBase
    {
        /// <summary>
        /// Occurs when a new model is created.
        /// </summary>
        public event EventHandler<ChangeTrackingModelCreatedEventArgs<TDelta>> ChangeTrackingModelCreated;

        /// <summary>
        /// Occurs when a model is updated.
        /// </summary>
        public event EventHandler<ModelUpdatedEventArgs<TDelta>> ChangeTrackingModelUpdated;

        /// <summary>
        /// Applies a commit result.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>A task that can be awaited.</returns>
        public virtual Task ApplyAsync(TDelta delta)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            this.LastModifiedOn = delta.LastModifiedOn;
            this.OnPropertyChanged("LastModifiedOn");
            this.Version = delta.Version;
            this.OnPropertyChanged("Version");
            this.OnUpdated(delta);

            // ReSharper restore ExplicitCallerInfoArgument
            return Task.FromResult(0);
        }

        /// <summary>
        /// Raises the <see cref="ChangeTrackingModelCreated"/> event.
        /// </summary>
        /// <param name="changeTrackingModel">The created model.</param>
        protected virtual void OnChangeTrackingModelCreated(IChangeTrackingModel<TDelta> changeTrackingModel)
        {
            var e = new ChangeTrackingModelCreatedEventArgs<TDelta>(changeTrackingModel);
            this.OnChangeTrackingModelCreated(e);
        }

        /// <summary>
        /// Raises the <see cref="ChangeTrackingModelUpdated"/> event.
        /// </summary>
        /// <param name="delta">The delta object.</param>
        protected virtual void OnUpdated(TDelta delta)
        {
            var e = new ModelUpdatedEventArgs<TDelta>(delta);
            this.OnUpdated(e);
        }

        private void OnChangeTrackingModelCreated(ChangeTrackingModelCreatedEventArgs<TDelta> e)
        {
            var handler = this.ChangeTrackingModelCreated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        private void OnUpdated(ModelUpdatedEventArgs<TDelta> e)
        {
            var handler = this.ChangeTrackingModelUpdated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }
    }
}