// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenRootRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenRootRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Render manager for a <see cref="ScreenRoot"/> (entire screen).
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public sealed class ScreenRootRenderManager<TContext> : RenderManagerBase<ScreenRoot, TContext>
        where TContext : IRenderContext
    {
        private readonly RootRenderManager<TContext> root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenRootRenderManager{TContext}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        internal ScreenRootRenderManager(ScreenRoot item, RenderManagerFactoryBase<TContext> factory)
            : base(item)
        {
            this.root = factory.CreateRenderManager(item.Root);
        }

        /// <summary>
        /// Gets a value indicating whether the screen is visible.
        /// This can be used to turn on or off the screen or its backlight.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.Item.Visible;
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
            foreach (var update in updates)
            {
                if (update.RootId != this.root.ItemId)
                {
                    continue;
                }

                this.UpdateItems(update);
                updated = true;
            }

            if (this.root.Update(updates))
            {
                updated = true;
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
            this.root.Update(context);
            base.Update(context);
        }

        /// <summary>
        /// Renders the contents.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Render(double alpha, TContext context)
        {
            this.root.Render(alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.root.Dispose();
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
            return this.root.FindRoot(id);
        }
    }
}