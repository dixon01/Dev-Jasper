// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoRenderManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoRenderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    /// <summary>
    /// Render manager for a video.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public partial class VideoRenderManager<TContext>
    {
        partial void Initialize()
        {
            this.RenderIfInvisible = true;
        }
    }
}
