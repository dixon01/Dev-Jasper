// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeTrackingModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IChangeTrackingModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines the interface for change tracking models.
    /// </summary>
    /// <typeparam name="TDelta">The delta object.</typeparam>
    public interface IChangeTrackingModel<TDelta> : IDisposable
        where TDelta : DeltaBase
    {
        /// <summary>
        /// Occurs when 
        /// </summary>
        event EventHandler<ModelUpdatedEventArgs<TDelta>> Committed;

        /// <summary>
        /// Gets the version of the model.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        void Commit();
    }
}