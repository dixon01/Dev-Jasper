

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
using Gorba.Common.Medi.Core;
using Gorba.Motion.Infomedia.Entities.Messages;

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    public static class ComposerFactory
    {
        public static IComposer CreateComposer(IPresentationContext context, IComposer parent, ElementBase element)
        {
            var elemGroup = element as GroupElement;
            if (elemGroup != null)
            {
                return new GroupComposer(context, parent, elemGroup);
            }
        
            var elemAnalogClock = element as AnalogClockElement;
            if (elemAnalogClock != null)
            {
                return new AnalogClockComposer(context, parent, elemAnalogClock);
            }
        
            var elemImage = element as ImageElement;
            if (elemImage != null)
            {
                return new ImageComposer(context, parent, elemImage);
            }
        
            var elemImageList = element as ImageListElement;
            if (elemImageList != null)
            {
                return new ImageListComposer(context, parent, elemImageList);
            }
        
            var elemText = element as TextElement;
            if (elemText != null)
            {
                return new TextComposer(context, parent, elemText);
            }
        
            var elemVideo = element as VideoElement;
            if (elemVideo != null)
            {
                return new VideoComposer(context, parent, elemVideo);
            }
        
            var elemRectangle = element as RectangleElement;
            if (elemRectangle != null)
            {
                return new RectangleComposer(context, parent, elemRectangle);
            }
        
            var elemAudioOutput = element as AudioOutputElement;
            if (elemAudioOutput != null)
            {
                return new AudioOutputComposer(context, parent, elemAudioOutput);
            }
        
            var elemAudioFile = element as AudioFileElement;
            if (elemAudioFile != null)
            {
                return new AudioFileComposer(context, parent, elemAudioFile);
            }
        
            var elemAudioPause = element as AudioPauseElement;
            if (elemAudioPause != null)
            {
                return new AudioPauseComposer(context, parent, elemAudioPause);
            }
        
            var elemTextToSpeech = element as TextToSpeechElement;
            if (elemTextToSpeech != null)
            {
                return new TextToSpeechComposer(context, parent, elemTextToSpeech);
            }
            
            throw new NotSupportedException(string.Format("Unsupported layout element {0}", element));
        }
    }

    
    public abstract partial class GraphicalComposerBase<TElement> : ComposerBase<TElement>
        where TElement : GraphicalElementBase
    {
        public DynamicPropertyHandler HandlerVisible { get; private set; }

        public GraphicalComposerBase(IPresentationContext context, IComposer parent, TElement element)
            : base(context, parent, element)
        {
            this.HandlerVisible = new DynamicPropertyHandler(element.VisibleProperty, element.Visible, context);
            this.HandlerVisible.ValueChanged += this.HandlerVisibleOnValueChanged;
            
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.HandlerVisible.ValueChanged -= this.HandlerVisibleOnValueChanged;
            this.HandlerVisible.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerVisibleOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public sealed partial class GroupComposer : GraphicalComposerBase<GroupElement>
    {

        public GroupComposer(IPresentationContext context, IComposer parent, GroupElement element)
            : base(context, parent, element)
        {
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
    }
    
    public abstract partial class DrawableComposerBase<TElement, TItem> : GraphicalComposerBase<TElement>, IPresentableComposer
        where TElement : DrawableElementBase
        where TItem : DrawableItemBase, new()
    {

        public DrawableComposerBase(IPresentationContext context, IComposer parent, TElement element)
            : base(context, parent, element)
        {
            this.Initialize();
        }
            
        public event EventHandler<AnimatedItemPropertyChangedEventArgs> ItemPropertyValueChanged;

        public TItem Item { get; private set; }

        ScreenItemBase IPresentableComposer.Item
        {
            get
            {
                return this.Item;
            }
        }
        
        public override void Dispose()
        {        
            if (this.Item != null)
            {
                this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
            }
        
            this.Deinitialize();
            base.Dispose();
        }
                
        protected virtual TItem InitializeItem()
        {
            if (this.Item != null)
            {
                this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
            }

            var item = new TItem();
            item.ZIndex = this.Element.ZIndex;
            this.Item = item;
			this.Item.ElementId = this.Element.Id;
            this.PostInitializeItem();
            this.Item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
            return item;
        }

        protected virtual void RaiseItemPropertyValueChanged(AnimatedItemPropertyChangedEventArgs e)
        {
            var handler = this.ItemPropertyValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            this.RaiseItemPropertyValueChanged(
                new AnimatedItemPropertyChangedEventArgs(this.Item, e.PropertyName, e.Value, e.Animation));
        }

        partial void PostInitializeItem();
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
    }
    
    public sealed partial class AnalogClockComposer : DrawableComposerBase<AnalogClockElement, AnalogClockItem>
    {

        public AnalogClockComposer(IPresentationContext context, IComposer parent, AnalogClockElement element)
            : base(context, parent, element)
        {        
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override AnalogClockItem InitializeItem()
        {
            var item = base.InitializeItem();
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
    }
    
    public sealed partial class ImageComposer : DrawableComposerBase<ImageElement, ImageItem>
    {
        private bool visible;

        public DynamicPropertyHandler HandlerFilename { get; private set; }

        public ImageComposer(IPresentationContext context, IComposer parent, ImageElement element)
            : base(context, parent, element)
        {
            this.HandlerFilename = new DynamicPropertyHandler(element.FilenameProperty, element.Filename, context);
            this.HandlerFilename.ValueChanged += this.HandlerFilenameOnValueChanged;

            this.InitializeItem();
            
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerFilename.ValueChanged -= this.HandlerFilenameOnValueChanged;
            this.HandlerFilename.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override ImageItem InitializeItem()
        {
            var item = base.InitializeItem();
            item.Scaling = this.Element.Scaling;
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();

        private void SendElementUpdateMessage(ImageElement element, bool isActive)
        {
            if (this.visible || this.IsVisible())
            {
                this.visible = true;
                var message = new DrawableComposerInitMessage()
                {
                    UnitName = MessageDispatcher.Instance.LocalAddress.Unit,
                    ElementID = element.Id,
                    ElementFileName = element.Filename,
                    Status = isActive ? DrawableStatus.Initialized : DrawableStatus.Disposing
                };

                MessageDispatcher.Instance.Broadcast(message);
            }
        }

        private void HandlerFilenameOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public sealed partial class ImageListComposer : DrawableComposerBase<ImageListElement, ImageListItem>
    {
        public DynamicPropertyHandler HandlerValues { get; private set; }

        public ImageListComposer(IPresentationContext context, IComposer parent, ImageListElement element)
            : base(context, parent, element)
        {
            this.HandlerValues = new DynamicPropertyHandler(element.ValuesProperty, element.Values, context);
            this.HandlerValues.ValueChanged += this.HandlerValuesOnValueChanged;
                    
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerValues.ValueChanged -= this.HandlerValuesOnValueChanged;
            this.HandlerValues.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override ImageListItem InitializeItem()
        {
            var item = base.InitializeItem();
            item.Overflow = this.Element.Overflow;
            item.Align = this.Element.Align;
            item.Direction = this.Element.Direction;
            item.HorizontalImageGap = this.Element.HorizontalImageGap;
            item.VerticalImageGap = this.Element.VerticalImageGap;
            item.ImageWidth = this.Element.ImageWidth;
            item.ImageHeight = this.Element.ImageHeight;
            item.FallbackImage = this.Element.FallbackImage;
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerValuesOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
    }
    
    public sealed partial class TextComposer : DrawableComposerBase<TextElement, TextItem>
    {
        public DynamicPropertyHandler HandlerValue { get; private set; }

        public TextComposer(IPresentationContext context, IComposer parent, TextElement element)
            : base(context, parent, element)
        {
            this.HandlerValue = new DynamicPropertyHandler(element.ValueProperty, element.Value, context);
            this.HandlerValue.ValueChanged += this.HandlerValueOnValueChanged;
                    
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerValue.ValueChanged -= this.HandlerValueOnValueChanged;
            this.HandlerValue.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override TextItem InitializeItem()
        {
            var item = base.InitializeItem();
            item.Rotation = this.Element.Rotation;
            item.Align = this.Element.Align;
            item.VAlign = this.Element.VAlign;
            item.Overflow = this.Element.Overflow;
            item.ScrollSpeed = this.Element.ScrollSpeed;
            item.Font = this.Element.Font;
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerValueOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
    }
    
    public sealed partial class VideoComposer : DrawableComposerBase<VideoElement, VideoItem>
    {
        public DynamicPropertyHandler HandlerVideoUri { get; private set; }

        public VideoComposer(IPresentationContext context, IComposer parent, VideoElement element)
            : base(context, parent, element)
        {
            this.HandlerVideoUri = new DynamicPropertyHandler(element.VideoUriProperty, element.VideoUri, context);
            this.HandlerVideoUri.ValueChanged += this.HandlerVideoUriOnValueChanged;

            this.InitializeItem();
            
            this.Initialize();
            this.Update();
        }

        public override void Dispose()
        {
            this.HandlerVideoUri.ValueChanged -= this.HandlerVideoUriOnValueChanged;
            this.HandlerVideoUri.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override VideoItem InitializeItem()
        {
            var item = base.InitializeItem();
            item.Scaling = this.Element.Scaling;
            item.Replay = this.Element.Replay;
            item.FallbackImage = this.Element.FallbackImage;
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();


        private void HandlerVideoUriOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public sealed partial class RectangleComposer : DrawableComposerBase<RectangleElement, RectangleItem>
    {
        public DynamicPropertyHandler HandlerColor { get; private set; }

        public RectangleComposer(IPresentationContext context, IComposer parent, RectangleElement element)
            : base(context, parent, element)
        {
            this.HandlerColor = new DynamicPropertyHandler(element.ColorProperty, element.Color, context);
            this.HandlerColor.ValueChanged += this.HandlerColorOnValueChanged;
                    
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerColor.ValueChanged -= this.HandlerColorOnValueChanged;
            this.HandlerColor.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override RectangleItem InitializeItem()
        {
            var item = base.InitializeItem();
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerColorOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public abstract partial class AudioComposerBase<TElement> : ComposerBase<TElement>
        where TElement : AudioElementBase
    {
        public DynamicPropertyHandler HandlerEnabled { get; private set; }

        public AudioComposerBase(IPresentationContext context, IComposer parent, TElement element)
            : base(context, parent, element)
        {
            this.HandlerEnabled = new DynamicPropertyHandler(element.EnabledProperty, element.Enabled, context);
            this.HandlerEnabled.ValueChanged += this.HandlerEnabledOnValueChanged;
            
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.HandlerEnabled.ValueChanged -= this.HandlerEnabledOnValueChanged;
            this.HandlerEnabled.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerEnabledOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public sealed partial class AudioOutputComposer : AudioComposerBase<AudioOutputElement>
    {
        public DynamicPropertyHandler HandlerVolume { get; private set; }

        public AudioOutputComposer(IPresentationContext context, IComposer parent, AudioOutputElement element)
            : base(context, parent, element)
        {
            this.HandlerVolume = new DynamicPropertyHandler(element.VolumeProperty, element.Volume, context);
            this.HandlerVolume.ValueChanged += this.HandlerVolumeOnValueChanged;
            
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerVolume.ValueChanged -= this.HandlerVolumeOnValueChanged;
            this.HandlerVolume.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerVolumeOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public abstract partial class PlaybackComposerBase<TElement, TItem> : AudioComposerBase<TElement>, IPresentableComposer
        where TElement : PlaybackElementBase
        where TItem : PlaybackItemBase, new()
    {

        public PlaybackComposerBase(IPresentationContext context, IComposer parent, TElement element)
            : base(context, parent, element)
        {
            this.Initialize();
        }
            
        public event EventHandler<AnimatedItemPropertyChangedEventArgs> ItemPropertyValueChanged;

        public TItem Item { get; private set; }

        ScreenItemBase IPresentableComposer.Item
        {
            get
            {
                return this.Item;
            }
        }
        
        public override void Dispose()
        {        
            if (this.Item != null)
            {
                this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
            }
        
            this.Deinitialize();
            base.Dispose();
        }
                
        protected virtual TItem InitializeItem()
        {
            if (this.Item != null)
            {
                this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
            }

            var item = new TItem();
            this.Item = item;
			this.Item.ElementId = this.Element.Id;
            this.PostInitializeItem();
            this.Item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
            return item;
        }

        protected virtual void RaiseItemPropertyValueChanged(AnimatedItemPropertyChangedEventArgs e)
        {
            var handler = this.ItemPropertyValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            this.RaiseItemPropertyValueChanged(
                new AnimatedItemPropertyChangedEventArgs(this.Item, e.PropertyName, e.Value, e.Animation));
        }

        partial void PostInitializeItem();
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
    }
    
    public sealed partial class AudioFileComposer : PlaybackComposerBase<AudioFileElement, AudioFileItem>
    {
        public DynamicPropertyHandler HandlerFilename { get; private set; }

        public AudioFileComposer(IPresentationContext context, IComposer parent, AudioFileElement element)
            : base(context, parent, element)
        {
            this.HandlerFilename = new DynamicPropertyHandler(element.FilenameProperty, element.Filename, context);
            this.HandlerFilename.ValueChanged += this.HandlerFilenameOnValueChanged;
                    
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerFilename.ValueChanged -= this.HandlerFilenameOnValueChanged;
            this.HandlerFilename.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override AudioFileItem InitializeItem()
        {
            var item = base.InitializeItem();
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerFilenameOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
    public sealed partial class AudioPauseComposer : PlaybackComposerBase<AudioPauseElement, AudioPauseItem>
    {

        public AudioPauseComposer(IPresentationContext context, IComposer parent, AudioPauseElement element)
            : base(context, parent, element)
        {        
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override AudioPauseItem InitializeItem()
        {
            var item = base.InitializeItem();
            item.Duration = this.Element.Duration;
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
    }
    
    public sealed partial class TextToSpeechComposer : PlaybackComposerBase<TextToSpeechElement, TextToSpeechItem>
    {
        public DynamicPropertyHandler HandlerVoice { get; private set; }
        public DynamicPropertyHandler HandlerValue { get; private set; }

        public TextToSpeechComposer(IPresentationContext context, IComposer parent, TextToSpeechElement element)
            : base(context, parent, element)
        {
            this.HandlerVoice = new DynamicPropertyHandler(element.VoiceProperty, element.Voice, context);
            this.HandlerVoice.ValueChanged += this.HandlerVoiceOnValueChanged;
            
            this.HandlerValue = new DynamicPropertyHandler(element.ValueProperty, element.Value, context);
            this.HandlerValue.ValueChanged += this.HandlerValueOnValueChanged;
                    
            this.InitializeItem();
        
            this.Initialize();
            this.Update();
        }
    
        public override void Dispose()
        {
            this.HandlerVoice.ValueChanged -= this.HandlerVoiceOnValueChanged;
            this.HandlerVoice.Dispose();
            this.HandlerValue.ValueChanged -= this.HandlerValueOnValueChanged;
            this.HandlerValue.Dispose();
            this.Deinitialize();
            base.Dispose();
        }
        
        protected override TextToSpeechItem InitializeItem()
        {
            var item = base.InitializeItem();
            return item;
        }
                
        partial void Initialize();

        partial void Update();

        partial void Deinitialize();
        
        private void HandlerVoiceOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
        private void HandlerValueOnValueChanged(object sender, EventArgs args)
        {
            this.Update();
        }
        
    }
    
}

