// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;

    using Math = System.Math;

    /// <summary>
    /// Render manager for a single hand of an analog clock.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public sealed class AnalogClockHandRenderManager<TContext> :
        DrawableRenderManagerBase<AnalogClockHandItem, TContext, IAnalogClockHandRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AnalogClockRenderManager<TContext> parent;

        private int modulo;

        private int divider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockHandRenderManager{TContext}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        internal AnalogClockHandRenderManager(AnalogClockHandItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
        }

        /// <summary>
        /// Gets the the current X coordinate of the item to be rendered.
        /// </summary>
        public override int X
        {
            get
            {
                return base.X + this.parent.X + this.Item.CenterX;
            }
        }

        /// <summary>
        /// Gets the the current Y coordinate of the item to be rendered.
        /// </summary>
        public override int Y
        {
            get
            {
                return base.Y + this.parent.Y + this.Item.CenterY;
            }
        }

        /// <summary>
        /// Gets the filename of the image to be drawn.
        /// </summary>
        public string Filename
        {
            get
            {
                return this.Item.Filename;
            }
        }

        /// <summary>
        /// Gets the X coordinate of the rotation center.
        /// </summary>
        public int CenterX
        {
            get
            {
                return this.Item.CenterX;
            }
        }

        /// <summary>
        /// Gets the Y coordinate of the rotation center.
        /// </summary>
        public int CenterY
        {
            get
            {
                return this.Item.CenterY;
            }
        }

        /// <summary>
        /// Gets the rotation angle in radiant.
        /// </summary>
        public double Rotation { get; private set; }

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Update(TContext context)
        {
            var seconds = TimeProvider.Current.Now.TimeOfDay.TotalSeconds;
            var value = (seconds / this.divider) % this.modulo; // value will be between 0 and modulo
            if (this.Item.Mode == AnalogClockHandMode.Jump)
            {
                // make it jump
                value = (int)value;
            }
            else if (this.Item.Mode == AnalogClockHandMode.CatchUp && this.divider == 1)
            {
                // do Swiss railways style catch-up only for seconds hand
                value = Math.Min(value / 58.5 * 60, this.modulo);
            }

            this.Rotation = value / this.modulo * Math.PI * 2;

            base.Update(context);
        }

        /// <summary>
        /// Initializes this hand manager with its parent.
        /// </summary>
        /// <param name="parentManager">
        /// The parent manager.
        /// </param>
        /// <param name="mod">
        /// The modulo of seconds to be used.
        /// </param>
        /// <param name="div">
        /// The divider of seconds to be used.
        /// </param>
        internal void Initialize(AnalogClockRenderManager<TContext> parentManager, int mod, int div)
        {
            this.parent = parentManager;
            this.modulo = mod;
            this.divider = div;
        }
    }
}