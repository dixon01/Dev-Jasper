// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Engine
{
    using System;

    /// <summary>
    /// Base interface for all render engines.
    /// This interface should never be implemented directly.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public interface IRenderEngine<TContext> : IDisposable
        where TContext : IRenderContext
    {
        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        void Render(double alpha, TContext context);
    }
}