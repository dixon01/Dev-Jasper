// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RootRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Render manager for a <see cref="RootItem"/> (entire hierarchy).
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    internal sealed class RootRenderManager<TContext> : RenderManagerBase<RootItem, TContext>
        where TContext : IRenderContext
    {
        private readonly RenderManagerList<TContext> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootRenderManager{TContext}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        internal RootRenderManager(RootItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item)
        {
            // Required by CF 3.5:
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            this.children =
                new RenderManagerList<TContext>(
                    item.Items.ConvertAll<IScreenItemRenderManager<TContext>>(factory.CreateRenderManager));
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.Item.Width;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.Item.Height;
            }
        }

        /// <summary>
        /// Updates the entire hierarchy of screens with the given screen update.
        /// </summary>
        /// <param name="updates">
        /// The updates.
        /// </param>
        /// <returns>
        /// True if anything was updated, otherwise false.
        /// </returns>
        public bool Update(IList<ScreenUpdate> updates)
        {
            var updated = false;
            RootRenderManager<TContext> root = null;
            foreach (var update in updates)
            {
                if (root == null || update.RootId != root.ItemId)
                {
                    root = this.FindRoot(update.RootId);
                }

                if (root != null)
                {
                    root.UpdateItems(update);
                    updated = true;
                }
            }

            return updated;
        }

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Update(TContext context)
        {
            this.children.Update(context);
            base.Update(context);
        }

        /// <summary>
        /// Renders all children.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Render(double alpha, TContext context)
        {
            this.children.Render(alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.children.Dispose();
            base.Dispose();
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
        internal override RootRenderManager<TContext> FindRoot(int id)
        {
            if (this.ItemId == id)
            {
                return this;
            }

            return this.children.FindRoot(id);
        }

        /// <summary>
        /// Updates all items with the given update.
        /// </summary>
        /// <param name="update">
        /// The screen update.
        /// </param>
        protected override void UpdateItems(ScreenUpdate update)
        {
            base.UpdateItems(update);

            this.children.UpdateItems(update);
        }
    }
}