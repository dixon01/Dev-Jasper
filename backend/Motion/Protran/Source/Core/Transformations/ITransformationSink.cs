// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransformationSink.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    /// <summary>
    /// Tagging interface for <see cref="ITransformationSink{T}"/>.
    /// </summary>
    public interface ITransformationSink
    {
        /// <summary>
        /// Gets the type of object that is expected by this source.
        /// </summary>
        Type InputType { get; }
    }
}
