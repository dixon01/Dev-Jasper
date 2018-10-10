

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
using Gorba.Motion.Infomedia.RendererBase.Manager.Animation;

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
        
    public abstract partial class GraphicalRenderManagerBase<TItem, TContext, TEngine>
        : ScreenItemRenderManagerBase<TItem, TContext, TEngine>
        where TItem : GraphicalItemBase
        where TContext : IRenderContext
        where TEngine : class, IRenderEngine<TContext>
    {
        private ValueAnimator visibleAnimator;    

        internal GraphicalRenderManagerBase(TItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.visibleAnimator = new ValueAnimator(item.Visible ? 1 : 0);
            this.Initialize();
        }

        public virtual int X
        {
            get
            {
                return this.Item.X;
            }
        }

        public virtual int Y
        {
            get
            {
                return this.Item.Y;
            }
        }

        public virtual int Width
        {
            get
            {
                return this.Item.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                return this.Item.Height;
            }
        }

        public virtual double Visible
        {
            get
            {
                return (double)this.visibleAnimator.Value;
            }
        }

        public override void Update(TContext context)
        {
            this.visibleAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Visible":
                    this.visibleAnimator.Animate(change.Animation, this.Item.Visible ? 1 : 0);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public abstract partial class DrawableRenderManagerBase<TItem, TContext, TEngine>
        : GraphicalRenderManagerBase<TItem, TContext, TEngine>
        where TItem : DrawableItemBase
        where TContext : IRenderContext
        where TEngine : class, IRenderEngine<TContext>
    {    

        internal DrawableRenderManagerBase(TItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual int ZIndex
        {
            get
            {
                return this.Item.ZIndex;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class AnalogClockRenderManager<TContext>
        : DrawableRenderManagerBase<AnalogClockItem, TContext, IAnalogClockRenderEngine<TContext>>
        where TContext : IRenderContext
    {    

        internal AnalogClockRenderManager(AnalogClockItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class ImageRenderManager<TContext>
        : DrawableRenderManagerBase<ImageItem, TContext, IImageRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AlphaAnimator<string> filenameAnimator;    

        internal ImageRenderManager(ImageItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.filenameAnimator = new AlphaAnimator<string>(item.Filename);
            this.Initialize();
        }

        public virtual AlphaAnimator<string> Filename
        {
            get
            {
                return this.filenameAnimator;
            }
        }

        public virtual ElementScaling Scaling
        {
            get
            {
                return this.Item.Scaling;
            }
        }

        public virtual bool Blink
        {
            get
            {
                return this.Item.Blink;
            }
        }

        public override void Update(TContext context)
        {
            this.filenameAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Filename":
                    this.filenameAnimator.Animate(change.Animation, this.Item.Filename);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class ImageListRenderManager<TContext>
        : DrawableRenderManagerBase<ImageListItem, TContext, IImageListRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AlphaAnimator<string[]> imagesAnimator;    

        internal ImageListRenderManager(ImageListItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.imagesAnimator = new AlphaAnimator<string[]>(item.Images);
            this.Initialize();
        }

        public virtual TextOverflow Overflow
        {
            get
            {
                return this.Item.Overflow;
            }
        }

        public virtual HorizontalAlignment Align
        {
            get
            {
                return this.Item.Align;
            }
        }

        public virtual TextDirection Direction
        {
            get
            {
                return this.Item.Direction;
            }
        }

        public virtual int HorizontalImageGap
        {
            get
            {
                return this.Item.HorizontalImageGap;
            }
        }

        public virtual int VerticalImageGap
        {
            get
            {
                return this.Item.VerticalImageGap;
            }
        }

        public virtual int ImageWidth
        {
            get
            {
                return this.Item.ImageWidth;
            }
        }

        public virtual int ImageHeight
        {
            get
            {
                return this.Item.ImageHeight;
            }
        }

        public virtual string FallbackImage
        {
            get
            {
                return this.Item.FallbackImage;
            }
        }

        public virtual AlphaAnimator<string[]> Images
        {
            get
            {
                return this.imagesAnimator;
            }
        }

        public override void Update(TContext context)
        {
            this.imagesAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Images":
                    this.imagesAnimator.Animate(change.Animation, this.Item.Images);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class TextRenderManager<TContext>
        : DrawableRenderManagerBase<TextItem, TContext, ITextRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AlphaAnimator<string> textAnimator;    

        internal TextRenderManager(TextItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.textAnimator = new AlphaAnimator<string>(item.Text);
            this.Initialize();
        }

        public virtual int Rotation
        {
            get
            {
                return this.Item.Rotation;
            }
        }

        public virtual HorizontalAlignment Align
        {
            get
            {
                return this.Item.Align;
            }
        }

        public virtual VerticalAlignment VAlign
        {
            get
            {
                return this.Item.VAlign;
            }
        }

        public virtual TextOverflow Overflow
        {
            get
            {
                return this.Item.Overflow;
            }
        }

        public virtual int ScrollSpeed
        {
            get
            {
                return this.Item.ScrollSpeed;
            }
        }

        public virtual Font Font
        {
            get
            {
                return this.Item.Font;
            }
        }

        public virtual AlphaAnimator<string> Text
        {
            get
            {
                return this.textAnimator;
            }
        }

        public override void Update(TContext context)
        {
            this.textAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Text":
                    this.textAnimator.Animate(change.Animation, this.Item.Text);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class VideoRenderManager<TContext>
        : DrawableRenderManagerBase<VideoItem, TContext, IVideoRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AlphaAnimator<string> videoUriAnimator;    

        internal VideoRenderManager(VideoItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.videoUriAnimator = new AlphaAnimator<string>(item.VideoUri);
            this.Initialize();
        }

        public virtual AlphaAnimator<string> VideoUri
        {
            get
            {
                return this.videoUriAnimator;
            }
        }

        public virtual ElementScaling Scaling
        {
            get
            {
                return this.Item.Scaling;
            }
        }

        public virtual bool Replay
        {
            get
            {
                return this.Item.Replay;
            }
        }

        public virtual string FallbackImage
        {
            get
            {
                return this.Item.FallbackImage;
            }
        }

        public override void Update(TContext context)
        {
            this.videoUriAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "VideoUri":
                    this.videoUriAnimator.Animate(change.Animation, this.Item.VideoUri);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class RectangleRenderManager<TContext>
        : DrawableRenderManagerBase<RectangleItem, TContext, IRectangleRenderEngine<TContext>>
        where TContext : IRenderContext
    {
        private AlphaAnimator<string> colorAnimator;    

        internal RectangleRenderManager(RectangleItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.colorAnimator = new AlphaAnimator<string>(item.Color);
            this.Initialize();
        }

        public virtual AlphaAnimator<string> Color
        {
            get
            {
                return this.colorAnimator;
            }
        }

        public override void Update(TContext context)
        {
            this.colorAnimator.Update(context);
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                case "Color":
                    this.colorAnimator.Animate(change.Animation, this.Item.Color);
                    break;
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public abstract partial class AudioRenderManagerBase<TItem, TContext, TEngine>
        : ScreenItemRenderManagerBase<TItem, TContext, TEngine>
        where TItem : AudioItemBase
        where TContext : IRenderContext
        where TEngine : class, IRenderEngine<TContext>
    {    

        internal AudioRenderManagerBase(TItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Item.Enabled;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public abstract partial class PlaybackRenderManagerBase<TItem, TContext, TEngine>
        : AudioRenderManagerBase<TItem, TContext, TEngine>
        where TItem : PlaybackItemBase
        where TContext : IRenderContext
        where TEngine : class, IRenderEngine<TContext>
    {    

        internal PlaybackRenderManagerBase(TItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual int Volume
        {
            get
            {
                return this.Item.Volume;
            }
        }

        public virtual int Priority
        {
            get
            {
                return this.Item.Priority;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class AudioFileRenderManager<TContext>
        : PlaybackRenderManagerBase<AudioFileItem, TContext, IAudioFileRenderEngine<TContext>>
        where TContext : IRenderContext
    {    

        internal AudioFileRenderManager(AudioFileItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual string Filename
        {
            get
            {
                return this.Item.Filename;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class AudioPauseRenderManager<TContext>
        : PlaybackRenderManagerBase<AudioPauseItem, TContext, IAudioPauseRenderEngine<TContext>>
        where TContext : IRenderContext
    {    

        internal AudioPauseRenderManager(AudioPauseItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual TimeSpan Duration
        {
            get
            {
                return this.Item.Duration;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
        
    public partial class TextToSpeechRenderManager<TContext>
        : PlaybackRenderManagerBase<TextToSpeechItem, TContext, ITextToSpeechRenderEngine<TContext>>
        where TContext : IRenderContext
    {    

        internal TextToSpeechRenderManager(TextToSpeechItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item, factory)
        {
            this.Initialize();
        }

        public virtual string Voice
        {
            get
            {
                return this.Item.Voice;
            }
        }

        public virtual string Value
        {
            get
            {
                return this.Item.Value;
            }
        }

        public override void Update(TContext context)
        {
            this.DoUpdate(context);
            base.Update(context);
        }

        protected override void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
            switch (change.PropertyName)
            {
                default:
                    base.HandlePropertyChange(change);
                    break;
            }
        }
        
        partial void Initialize();
        
        partial void DoUpdate(TContext context);
    }
    
}

