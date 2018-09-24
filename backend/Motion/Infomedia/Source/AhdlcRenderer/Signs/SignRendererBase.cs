// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignRendererBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignRendererBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Signs
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;

    using NLog;

    /// <summary>
    /// The base class for all renderers that render to a sign.
    /// </summary>
    public abstract class SignRendererBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignRendererBase"/> class.
        /// </summary>
        protected SignRendererBase()
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets the width of the sign.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the sign.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SignRendererBase"/> implementation from the given config.
        /// </summary>
        /// <param name="config">
        /// The sign configuration.
        /// </param>
        /// <returns>
        /// An object implementing <see cref="SignRendererBase"/>.
        /// </returns>
        public static SignRendererBase Create(SignConfig config)
        {
            var renderer = CreateRenderer(config);
            renderer.Configure(config);
            return renderer;
        }

        /// <summary>
        /// Renders the given components onto the sign.
        /// </summary>
        /// <param name="components">
        /// The components to render.
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <returns>
        /// The <see cref="IFrameProvider"/> that provides all frames to show the contents of the given components.
        /// </returns>
        public abstract IFrameProvider Render(ICollection<ComponentBase> components, IAhdlcRenderContext context);

        /// <summary>
        /// Configures this renderer.
        /// </summary>
        /// <param name="config">
        /// The sign configuration.
        /// </param>
        protected virtual void Configure(SignConfig config)
        {
            this.Width = config.Width;
            this.Height = config.Height;
        }

        private static SignRendererBase CreateRenderer(SignConfig config)
        {
            switch (config.Mode)
            {
                case SignMode.Monochrome:
                    return new MonochromeSignRenderer();
                case SignMode.Color:
                    return new ColorSignRenderer();
                case SignMode.Text:
                    return new TextSignRenderer();
                default:
                    throw new ArgumentOutOfRangeException("config");
            }
        }
    }
}
