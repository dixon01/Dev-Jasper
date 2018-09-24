// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase
{
    /// <summary>
    /// Base interface for a render context.
    /// You should never directly implement this interface but
    /// rather create a sub-interface and then implement that.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Gets a counter that is increased by one every millisecond.
        /// This counter doesn't change during a single rendering pass.
        /// </summary>
        long MillisecondsCounter { get; }
    }
}