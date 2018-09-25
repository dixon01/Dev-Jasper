

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
using Gorba.Motion.Infomedia.Entities.Screen;

using Gorba.Motion.Infomedia.RendererBase.Engine;
using Gorba.Motion.Infomedia.RendererBase.Manager;

namespace Gorba.Motion.Infomedia.RendererBase
{
    public abstract partial class RenderManagerFactoryBase<TContext>
        where TContext : IRenderContext
    {
        internal IScreenItemRenderManager<TContext> CreateRenderManager(ScreenItemBase item)
        {
            var itemInclude = item as IncludeItem;
            if (itemInclude != null)
            {
                return Connect<IncludeItem, IncludeRenderManager<TContext>, IIncludeRenderEngine<TContext>>(
                    new IncludeRenderManager<TContext>(itemInclude, this), this.CreateEngine);
            }
        
            var itemAnalogClock = item as AnalogClockItem;
            if (itemAnalogClock != null)
            {
                return Connect<AnalogClockItem, AnalogClockRenderManager<TContext>, IAnalogClockRenderEngine<TContext>>(
                    new AnalogClockRenderManager<TContext>(itemAnalogClock, this), this.CreateEngine);
            }
        
            var itemAnalogClockHand = item as AnalogClockHandItem;
            if (itemAnalogClockHand != null)
            {
                return Connect<AnalogClockHandItem, AnalogClockHandRenderManager<TContext>, IAnalogClockHandRenderEngine<TContext>>(
                    new AnalogClockHandRenderManager<TContext>(itemAnalogClockHand, this), this.CreateEngine);
            }
        
            var itemImage = item as ImageItem;
            if (itemImage != null)
            {
                return Connect<ImageItem, ImageRenderManager<TContext>, IImageRenderEngine<TContext>>(
                    new ImageRenderManager<TContext>(itemImage, this), this.CreateEngine);
            }
        
            var itemImageList = item as ImageListItem;
            if (itemImageList != null)
            {
                return Connect<ImageListItem, ImageListRenderManager<TContext>, IImageListRenderEngine<TContext>>(
                    new ImageListRenderManager<TContext>(itemImageList, this), this.CreateEngine);
            }
        
            var itemText = item as TextItem;
            if (itemText != null)
            {
                return Connect<TextItem, TextRenderManager<TContext>, ITextRenderEngine<TContext>>(
                    new TextRenderManager<TContext>(itemText, this), this.CreateEngine);
            }
        
            var itemVideo = item as VideoItem;
            if (itemVideo != null)
            {
                return Connect<VideoItem, VideoRenderManager<TContext>, IVideoRenderEngine<TContext>>(
                    new VideoRenderManager<TContext>(itemVideo, this), this.CreateEngine);
            }
        
            var itemRectangle = item as RectangleItem;
            if (itemRectangle != null)
            {
                return Connect<RectangleItem, RectangleRenderManager<TContext>, IRectangleRenderEngine<TContext>>(
                    new RectangleRenderManager<TContext>(itemRectangle, this), this.CreateEngine);
            }
        
            var itemAudioFile = item as AudioFileItem;
            if (itemAudioFile != null)
            {
                return Connect<AudioFileItem, AudioFileRenderManager<TContext>, IAudioFileRenderEngine<TContext>>(
                    new AudioFileRenderManager<TContext>(itemAudioFile, this), this.CreateEngine);
            }
        
            var itemAudioPause = item as AudioPauseItem;
            if (itemAudioPause != null)
            {
                return Connect<AudioPauseItem, AudioPauseRenderManager<TContext>, IAudioPauseRenderEngine<TContext>>(
                    new AudioPauseRenderManager<TContext>(itemAudioPause, this), this.CreateEngine);
            }
        
            var itemTextToSpeech = item as TextToSpeechItem;
            if (itemTextToSpeech != null)
            {
                return Connect<TextToSpeechItem, TextToSpeechRenderManager<TContext>, ITextToSpeechRenderEngine<TContext>>(
                    new TextToSpeechRenderManager<TContext>(itemTextToSpeech, this), this.CreateEngine);
            }
        
            throw new NotSupportedException("Type not supported: " + item.GetType().FullName);
        }
    
            
        /// <summary>
        /// Creates an <see cref="IAnalogClockRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IAnalogClockRenderEngine<TContext> CreateEngine(AnalogClockRenderManager<TContext> manager)
        {
            throw new NotSupportedException("AnalogClockItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IAnalogClockHandRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IAnalogClockHandRenderEngine<TContext> CreateEngine(AnalogClockHandRenderManager<TContext> manager)
        {
            throw new NotSupportedException("AnalogClockHandItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IImageRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IImageRenderEngine<TContext> CreateEngine(ImageRenderManager<TContext> manager)
        {
            throw new NotSupportedException("ImageItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IImageListRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IImageListRenderEngine<TContext> CreateEngine(ImageListRenderManager<TContext> manager)
        {
            throw new NotSupportedException("ImageListItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="ITextRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual ITextRenderEngine<TContext> CreateEngine(TextRenderManager<TContext> manager)
        {
            throw new NotSupportedException("TextItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IVideoRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IVideoRenderEngine<TContext> CreateEngine(VideoRenderManager<TContext> manager)
        {
            throw new NotSupportedException("VideoItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IRectangleRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IRectangleRenderEngine<TContext> CreateEngine(RectangleRenderManager<TContext> manager)
        {
            throw new NotSupportedException("RectangleItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IAudioFileRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IAudioFileRenderEngine<TContext> CreateEngine(AudioFileRenderManager<TContext> manager)
        {
            throw new NotSupportedException("AudioFileItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="IAudioPauseRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual IAudioPauseRenderEngine<TContext> CreateEngine(AudioPauseRenderManager<TContext> manager)
        {
            throw new NotSupportedException("AudioPauseItem rendering not supported");
        }
                
        /// <summary>
        /// Creates an <see cref="ITextToSpeechRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected virtual ITextToSpeechRenderEngine<TContext> CreateEngine(TextToSpeechRenderManager<TContext> manager)
        {
            throw new NotSupportedException("TextToSpeechItem rendering not supported");
        }
        
    }
}

