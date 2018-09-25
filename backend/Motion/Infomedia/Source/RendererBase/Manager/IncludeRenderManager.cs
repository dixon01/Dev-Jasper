// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncludeRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IncludeRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager.Animation;

    /// <summary>
    /// Render manager for <see cref="IncludeItem"/>.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public sealed class IncludeRenderManager<TContext>
        : DrawableRenderManagerBase<IncludeItem, TContext, IIncludeRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private readonly AlphaAnimator<RootRenderManager<TContext>> include;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeRenderManager{TContext}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        internal IncludeRenderManager(IncludeItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.include = new AlphaAnimator<RootRenderManager<TContext>>(factory.CreateRenderManager(item.Include));
        }

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Update(TContext context)
        {
            this.include.Update(context);

            base.Update(context);

            this.include.DoWithValues((r, a) => r.Update(context));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            this.include.Release();
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
            if (this.include.OldValue != null)
            {
                var root = this.include.OldValue.FindRoot(id);
                if (root != null)
                {
                    return root;
                }
            }

            if (this.include.NewValue != null)
            {
                var root = this.include.NewValue.FindRoot(id);
                if (root != null)
                {
                    return root;
                }
            }

            return null;
        }

        /// <summary>
        /// Renders the included screen.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(double alpha, TContext context)
        {
            base.DoRender(alpha, context);

            // multiply alpha with include's alpha to allow double blending
            this.include.DoWithValues((r, a) => r.Render(a * alpha, context));
        }

        /// <summary>
        /// Handles property changes.
        /// </summary>
        /// <param name="change">
        /// The change information.
        /// </param>
        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Include":
                    this.include.Animate(
                        change.Animation, this.Factory.CreateRenderManager((RootItem)this.Item.Include.Clone()));
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
    }
}