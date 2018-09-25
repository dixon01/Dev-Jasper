// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelUpdatedEventArgs{TDelta}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ModelUpdatedEventArgs&lt;TDelta&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Event args used when a model was updated.
    /// </summary>
    /// <typeparam name="TDelta">The type of the delta object.</typeparam>
    public class ModelUpdatedEventArgs<TDelta> : EventArgs
        where TDelta : DeltaBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelUpdatedEventArgs&lt;TDelta&gt;"/> class.
        /// </summary>
        /// <param name="delta">The type of the delta object.</param>
        public ModelUpdatedEventArgs(TDelta delta)
        {
            this.Delta = delta;
        }

        /// <summary>
        /// Gets the delta object.
        /// </summary>
        public TDelta Delta { get; private set; }
    }
}