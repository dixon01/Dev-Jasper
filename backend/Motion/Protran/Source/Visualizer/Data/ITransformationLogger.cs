// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransformationLogger.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransformationLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data
{
    using System;

    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Non-generic interface for <see cref="TransformationLogger{T}"/>.
    /// </summary>
    public interface ITransformationLogger : ITransformer
    {
        /// <summary>
        /// Event that is fired when this transformer has 
        /// done a transformation.
        /// </summary>
        event EventHandler<TransformationEventArgs> Transformed;
    }
}