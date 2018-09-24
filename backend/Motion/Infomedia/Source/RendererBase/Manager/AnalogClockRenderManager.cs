// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// Render manager for an analog clock.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public partial class AnalogClockRenderManager<TContext>
    {
        private RenderManagerList<TContext> hands;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;

            this.hands.Dispose();

            base.Dispose();
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

            this.hands.UpdateItems(update);
        }

        /// <summary>
        /// Renders all hands.
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

            this.hands.Render(alpha, context);
        }

        partial void Initialize()
        {
            this.hands = new RenderManagerList<TContext>(this.CreateHands());
            this.Item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
        }

        partial void DoUpdate(TContext context)
        {
            this.hands.Update(context);
        }

        private IEnumerable<IScreenItemRenderManager<TContext>> CreateHands()
        {
            if (this.Item.Hour != null)
            {
                var manager =
                    (AnalogClockHandRenderManager<TContext>)this.Factory.CreateRenderManager(this.Item.Hour);
                manager.Initialize(this, 12, 3600);
                yield return manager;
            }

            if (this.Item.Minute != null)
            {
                var manager =
                    (AnalogClockHandRenderManager<TContext>)this.Factory.CreateRenderManager(this.Item.Minute);
                manager.Initialize(this, 60, 60);
                yield return manager;
            }

            if (this.Item.Seconds != null)
            {
                var manager =
                    (AnalogClockHandRenderManager<TContext>)this.Factory.CreateRenderManager(this.Item.Seconds);
                manager.Initialize(this, 60, 1);
                yield return manager;
            }
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Hour" && e.PropertyName != "Minute" && e.PropertyName != "Seconds")
            {
                return;
            }

            // easiest way: recreate all hands
            this.hands.Dispose();
            this.hands = new RenderManagerList<TContext>(this.CreateHands());
        }
    }
}