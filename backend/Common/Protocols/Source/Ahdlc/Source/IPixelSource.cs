// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Base interface for accessing pixels of a bitmap (or any other source of pixels).
    /// </summary>
    public interface IPixelSource
    {
        /// <summary>
        /// Gets the width of the source.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the source.
        /// </summary>
        int Height { get; }
    }
}