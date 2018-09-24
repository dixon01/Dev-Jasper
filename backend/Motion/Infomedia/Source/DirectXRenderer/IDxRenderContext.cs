// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDxRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDxRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.RendererBase;

    /// <summary>
    /// The context in which to render something.
    /// </summary>
    public interface IDxRenderContext : IRenderContext
    {
        /// <summary>
        /// Gets the renderer configuration.
        /// </summary>
        RendererConfig Config { get; }

        /// <summary>
        /// Gets a value indicating whether a blinking item should be rendered or not.
        /// This flag changes approximately every 0.5 seconds.
        /// <code>[bl][/bl]</code> BBCode requires this.
        /// </summary>
        bool BlinkOn { get; }

        /// <summary>
        /// Gets a the current counter which is incremented every millisecond.
        /// This is used for the scrolling feature.
        /// </summary>
        long ScrollCounter { get; }

        /// <summary>
        /// Gets the persistence view.
        /// </summary>
        PersistenceViewManager PersistenceView { get; }
    }
}