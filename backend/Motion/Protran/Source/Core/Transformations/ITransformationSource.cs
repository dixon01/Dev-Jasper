// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransformationSource.cs" company="Gorba AG">
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
    /// Interface for all classes that can be a sender in a
    /// transformation chain.
    /// </summary>
    public interface ITransformationSource
    {
        /// <summary>
        /// Gets or sets the reference to the sink that is
        /// next in the chain.
        /// </summary>
        ITransformationSink Next { get; set; }

        /// <summary>
        /// Gets the type of object that is output by this source.
        /// </summary>
        Type OutputType { get; }
    }
}
