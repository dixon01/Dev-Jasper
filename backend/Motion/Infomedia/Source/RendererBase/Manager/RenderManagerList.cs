// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerList.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// A list of <see cref="IScreenItemRenderManager{TContext}"/>s.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    internal class RenderManagerList<TContext> : IDisposable
        where TContext : IRenderContext
    {
        private readonly List<IScreenItemRenderManager<TContext>> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerList{TContext}"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public RenderManagerList(IEnumerable<IScreenItemRenderManager<TContext>> items)
        {
            this.items = new List<IScreenItemRenderManager<TContext>>(items);

            // order the items by their z-index, back to front
            this.items.Sort((left, right) => left.SortIndex - right.SortIndex);
        }

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
        public RootRenderManager<TContext> FindRoot(int id)
        {
            foreach (var item in this.items)
            {
                var found = item.FindRoot(id);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates all items with the given update.
        /// This method does not traverse through different includes,
        /// therefore it is called after searching the right root
        /// (<see cref="FindRoot"/>)
        /// </summary>
        /// <param name="update">
        /// The screen update.
        /// </param>
        public void UpdateItems(ScreenUpdate update)
        {
            foreach (var item in this.items)
            {
                item.UpdateItems(update);
            }
        }

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public void Update(TContext context)
        {
            foreach (var item in this.items)
            {
                item.Update(context);
            }
        }

        /// <summary>
        /// Renders all items back to front.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public void Render(double alpha, TContext context)
        {
            foreach (var item in this.items)
            {
                item.Render(alpha, context);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var item in this.items)
            {
                item.Dispose();
            }

            this.items.Clear();
        }
    }
}