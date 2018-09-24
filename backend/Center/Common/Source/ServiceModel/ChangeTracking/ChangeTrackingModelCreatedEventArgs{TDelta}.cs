// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingModelCreatedEventArgs{TDelta}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingModelCreatedEventArgs&lt;TDelta&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Event args for a created model event.
    /// </summary>
    /// <typeparam name="TDelta">The type of the delta object.</typeparam>
    public class ChangeTrackingModelCreatedEventArgs<TDelta> : EventArgs
        where TDelta : DeltaBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingModelCreatedEventArgs{TDelta}"/> class.
        /// </summary>
        /// <param name="trackingModel">The tracking model.</param>
        /// <exception cref="ArgumentNullException">The model object is null.</exception>
        public ChangeTrackingModelCreatedEventArgs(IChangeTrackingModel<TDelta> trackingModel)
        {
            if (trackingModel == null)
            {
                throw new ArgumentNullException("trackingModel");
            }

            this.TrackingModel = trackingModel;
        }

        /// <summary>
        /// Gets the created model.
        /// </summary>
        public IChangeTrackingModel<TDelta> TrackingModel { get; private set; }
    }
}