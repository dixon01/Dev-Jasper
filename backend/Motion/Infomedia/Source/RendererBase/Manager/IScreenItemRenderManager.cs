// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScreenItemRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IScreenItemRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// Internal interface used for render managers that can be rendered.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    internal interface IScreenItemRenderManager<TContext> : IDisposable
        where TContext : IRenderContext
    {
        /// <summary>
        /// Gets the the index by which render managers are sorted before rendering.
        /// </summary>
        int SortIndex { get; }

        /// <summary>
        /// Find the <see cref="RootRenderManager{TContext}"/> for a given
        /// screen id. If no corresponding manager is found, null is returned.
        /// </summary>
        /// <param name="id">
        /// The screen id.
        /// </param>
        /// <returns>
        /// The root renderer manager responsible for the given screen id or
        /// null if none was found.
        /// </returns>
        RootRenderManager<TContext> FindRoot(int id);

        /// <summary>
        /// Updates all items with the given update.
        /// This method does not traverse through different includes,
        /// therefore it is called after searching the right root
        /// (<see cref="FindRoot"/>)
        /// </summary>
        /// <param name="update">
        /// The screen update.
        /// </param>
        void UpdateItems(ScreenUpdate update);

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        void Update(TContext context);

        /// <summary>
        /// Renders the contents.
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