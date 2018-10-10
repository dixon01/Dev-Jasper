// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAhdlcRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAhdlcRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.RendererBase;

    /// <summary>
    /// Renderer context for the AHDLC renderer.
    /// </summary>
    public interface IAhdlcRenderContext : IRenderContext
    {
        /// <summary>
        /// Gets the entire renderer config.
        /// </summary>
        AhdlcRendererConfig Config { get; }

        /// <summary>
        /// Gets the alternation counter which is incremented every <see cref="AlternationInterval"/>.
        /// </summary>
        int AlternationCounter { get; }

        /// <summary>
        /// Gets or sets the alternation interval.
        /// If this value is null, there are no alternations and no timer should be started for that.
        /// </summary>
        TimeSpan? AlternationInterval { get; set; }

        /// <summary>
        /// Adds a single item to the current context.
        /// </summary>
        /// <param name="component">
        /// The component to be added.
        /// </param>
        /// <param name="changed">
        /// A flag indicating if the added item has changed since the last call to Render().
        /// This helps to track if a new command has to be sent to the sign.
        /// </param>
        void AddItem(ComponentBase component, bool changed);

        /// <summary>
        /// Get the <see cref="IFont"/> with the given font description.
        /// </summary>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// The <see cref="IFont"/>.
        /// </returns>
        IFont GetFont(Font font);
    }
}