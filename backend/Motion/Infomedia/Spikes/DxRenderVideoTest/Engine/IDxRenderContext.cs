// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDxRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDxRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using Gorba.Motion.Infomedia.RendererBase;

    /// <summary>
    /// The context in which to render something.
    /// </summary>
    public interface IDxRenderContext : IRenderContext
    {
        /// <summary>
        /// Gets a value indicating whether a blinking item should be rendered or not.
        /// This flag changes approximately every 0.5 seconds.
        /// [bl][/bl] BBcode requires this.
        /// </summary>
        bool BlinkOn { get; }

        /// <summary>
        /// Gets a the current counter which is incremented roughly every three seconds.
        /// [a][|][/a] BBcode requires this.
        /// </summary>
        int AlternationCounter { get; }

        /// <summary>
        /// Gets a the current counter which is incremented every millisecond.
        /// This is used for the scrolling feature.
        /// </summary>
        int ScrollCounter { get; }
    }
}