// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIncludeRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIncludeRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Engine
{
    /// <summary>
    /// Interface to be implemented by the render engine
    /// for an include (screen within screen). This engine is optional
    /// since usually only the contents of the include is rendered.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="IRenderEngine{TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public interface IIncludeRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
}