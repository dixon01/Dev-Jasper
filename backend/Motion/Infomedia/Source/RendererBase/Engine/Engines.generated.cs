

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Common;
using Gorba.Common.Configuration.Infomedia.Layout;

using Gorba.Motion.Infomedia.Entities;

namespace Gorba.Motion.Infomedia.RendererBase.Engine
{
    
    public interface IAnalogClockRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IAnalogClockHandRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IImageRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IImageListRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface ITextRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IVideoRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IRectangleRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IAudioFileRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface IAudioPauseRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
    public interface ITextToSpeechRenderEngine<TContext> : IRenderEngine<TContext>
        where TContext : IRenderContext
    {
    }
    
}

