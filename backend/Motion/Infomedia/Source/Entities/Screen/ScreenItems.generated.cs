

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

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    
    public abstract partial class ScreenItemBase : ItemBase
    {

        public ScreenItemBase()
        {
        }
        
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public abstract partial class GraphicalItemBase : ScreenItemBase
    {
        private int x;
        private int y;
        private int width;
        private int height;
        private bool visible;

        public GraphicalItemBase()
        {
            this.width = -1;
            this.height = -1;
            this.visible = true;
        }
            
        public int X
        {
            get
            {
                return this.x;
            }

            set
            {
                this.SetX(value, null);
            }
        }
                
        public int Y
        {
            get
            {
                return this.y;
            }

            set
            {
                this.SetY(value, null);
            }
        }
                
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.SetWidth(value, null);
            }
        }
                
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.SetHeight(value, null);
            }
        }
                
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                this.SetVisible(value, null);
            }
        }
        
        public void SetX(int value, PropertyChangeAnimation animation)
        {
            if (this.x == value)
            {
                return;
            }

            this.x = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("X", value, animation));
        }
        
        public void SetY(int value, PropertyChangeAnimation animation)
        {
            if (this.y == value)
            {
                return;
            }

            this.y = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Y", value, animation));
        }
        
        public void SetWidth(int value, PropertyChangeAnimation animation)
        {
            if (this.width == value)
            {
                return;
            }

            this.width = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Width", value, animation));
        }
        
        public void SetHeight(int value, PropertyChangeAnimation animation)
        {
            if (this.height == value)
            {
                return;
            }

            this.height = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Height", value, animation));
        }
        
        public void SetVisible(bool value, PropertyChangeAnimation animation)
        {
            if (this.visible == value)
            {
                return;
            }

            this.visible = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Visible", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "X":
                    this.SetX((int)value, animation);
                    break;
                case "Y":
                    this.SetY((int)value, animation);
                    break;
                case "Width":
                    this.SetWidth((int)value, animation);
                    break;
                case "Height":
                    this.SetHeight((int)value, animation);
                    break;
                case "Visible":
                    this.SetVisible((bool)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public abstract partial class DrawableItemBase : GraphicalItemBase
    {
        private int zIndex;

        public DrawableItemBase()
        {
        }
            
        public int ZIndex
        {
            get
            {
                return this.zIndex;
            }

            set
            {
                this.SetZIndex(value, null);
            }
        }
        
        public void SetZIndex(int value, PropertyChangeAnimation animation)
        {
            if (this.zIndex == value)
            {
                return;
            }

            this.zIndex = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("ZIndex", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "ZIndex":
                    this.SetZIndex((int)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class AnalogClockItem : DrawableItemBase
    {
        private AnalogClockHandItem hour;
        private AnalogClockHandItem minute;
        private AnalogClockHandItem seconds;

        public AnalogClockItem()
        {
        }
            
        public AnalogClockHandItem Hour
        {
            get
            {
                return this.hour;
            }

            set
            {
                this.SetHour(value, null);
            }
        }
                
        public AnalogClockHandItem Minute
        {
            get
            {
                return this.minute;
            }

            set
            {
                this.SetMinute(value, null);
            }
        }
                
        public AnalogClockHandItem Seconds
        {
            get
            {
                return this.seconds;
            }

            set
            {
                this.SetSeconds(value, null);
            }
        }
        
        public void SetHour(AnalogClockHandItem value, PropertyChangeAnimation animation)
        {
            if (this.hour == value)
            {
                return;
            }

            this.hour = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Hour", value, animation));
        }
        
        public void SetMinute(AnalogClockHandItem value, PropertyChangeAnimation animation)
        {
            if (this.minute == value)
            {
                return;
            }

            this.minute = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Minute", value, animation));
        }
        
        public void SetSeconds(AnalogClockHandItem value, PropertyChangeAnimation animation)
        {
            if (this.seconds == value)
            {
                return;
            }

            this.seconds = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Seconds", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Hour":
                    this.SetHour((AnalogClockHandItem)value, animation);
                    break;
                case "Minute":
                    this.SetMinute((AnalogClockHandItem)value, animation);
                    break;
                case "Seconds":
                    this.SetSeconds((AnalogClockHandItem)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class AnalogClockHandItem : ImageItem
    {
        private AnalogClockHandMode mode;
        private int centerX;
        private int centerY;

        public AnalogClockHandItem()
        {
        }
            
        public AnalogClockHandMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.SetMode(value, null);
            }
        }
                
        public int CenterX
        {
            get
            {
                return this.centerX;
            }

            set
            {
                this.SetCenterX(value, null);
            }
        }
                
        public int CenterY
        {
            get
            {
                return this.centerY;
            }

            set
            {
                this.SetCenterY(value, null);
            }
        }
        
        public void SetMode(AnalogClockHandMode value, PropertyChangeAnimation animation)
        {
            if (this.mode == value)
            {
                return;
            }

            this.mode = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Mode", value, animation));
        }
        
        public void SetCenterX(int value, PropertyChangeAnimation animation)
        {
            if (this.centerX == value)
            {
                return;
            }

            this.centerX = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("CenterX", value, animation));
        }
        
        public void SetCenterY(int value, PropertyChangeAnimation animation)
        {
            if (this.centerY == value)
            {
                return;
            }

            this.centerY = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("CenterY", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Mode":
                    this.SetMode((AnalogClockHandMode)value, animation);
                    break;
                case "CenterX":
                    this.SetCenterX((int)value, animation);
                    break;
                case "CenterY":
                    this.SetCenterY((int)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class ImageItem : DrawableItemBase
    {
        private string filename;
        private ElementScaling scaling;
        private bool blink;

        public ImageItem()
        {
            this.scaling = ElementScaling.Stretch;
        }
            
        public string Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.SetFilename(value, null);
            }
        }
                
        public ElementScaling Scaling
        {
            get
            {
                return this.scaling;
            }

            set
            {
                this.SetScaling(value, null);
            }
        }
                
        public bool Blink
        {
            get
            {
                return this.blink;
            }

            set
            {
                this.SetBlink(value, null);
            }
        }
        
        public void SetFilename(string value, PropertyChangeAnimation animation)
        {
            if (this.filename == value)
            {
                return;
            }

            this.filename = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Filename", value, animation));
        }
        
        public void SetScaling(ElementScaling value, PropertyChangeAnimation animation)
        {
            if (this.scaling == value)
            {
                return;
            }

            this.scaling = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Scaling", value, animation));
        }
        
        public void SetBlink(bool value, PropertyChangeAnimation animation)
        {
            if (this.blink == value)
            {
                return;
            }

            this.blink = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Blink", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Filename":
                    this.SetFilename((string)value, animation);
                    break;
                case "Scaling":
                    this.SetScaling((ElementScaling)value, animation);
                    break;
                case "Blink":
                    this.SetBlink((bool)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class ImageListItem : DrawableItemBase
    {
        private TextOverflow overflow;
        private HorizontalAlignment align;
        private TextDirection direction;
        private int horizontalImageGap;
        private int verticalImageGap;
        private int imageWidth;
        private int imageHeight;
        private string fallbackImage;
        private string[] images;

        public ImageListItem()
        {
        }
            
        public TextOverflow Overflow
        {
            get
            {
                return this.overflow;
            }

            set
            {
                this.SetOverflow(value, null);
            }
        }
                
        public HorizontalAlignment Align
        {
            get
            {
                return this.align;
            }

            set
            {
                this.SetAlign(value, null);
            }
        }
                
        public TextDirection Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.SetDirection(value, null);
            }
        }
                
        public int HorizontalImageGap
        {
            get
            {
                return this.horizontalImageGap;
            }

            set
            {
                this.SetHorizontalImageGap(value, null);
            }
        }
                
        public int VerticalImageGap
        {
            get
            {
                return this.verticalImageGap;
            }

            set
            {
                this.SetVerticalImageGap(value, null);
            }
        }
                
        public int ImageWidth
        {
            get
            {
                return this.imageWidth;
            }

            set
            {
                this.SetImageWidth(value, null);
            }
        }
                
        public int ImageHeight
        {
            get
            {
                return this.imageHeight;
            }

            set
            {
                this.SetImageHeight(value, null);
            }
        }
                
        public string FallbackImage
        {
            get
            {
                return this.fallbackImage;
            }

            set
            {
                this.SetFallbackImage(value, null);
            }
        }
                
        public string[] Images
        {
            get
            {
                return this.images;
            }

            set
            {
                this.SetImages(value, null);
            }
        }
        
        public void SetOverflow(TextOverflow value, PropertyChangeAnimation animation)
        {
            if (this.overflow == value)
            {
                return;
            }

            this.overflow = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Overflow", value, animation));
        }
        
        public void SetAlign(HorizontalAlignment value, PropertyChangeAnimation animation)
        {
            if (this.align == value)
            {
                return;
            }

            this.align = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Align", value, animation));
        }
        
        public void SetDirection(TextDirection value, PropertyChangeAnimation animation)
        {
            if (this.direction == value)
            {
                return;
            }

            this.direction = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Direction", value, animation));
        }
        
        public void SetHorizontalImageGap(int value, PropertyChangeAnimation animation)
        {
            if (this.horizontalImageGap == value)
            {
                return;
            }

            this.horizontalImageGap = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("HorizontalImageGap", value, animation));
        }
        
        public void SetVerticalImageGap(int value, PropertyChangeAnimation animation)
        {
            if (this.verticalImageGap == value)
            {
                return;
            }

            this.verticalImageGap = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("VerticalImageGap", value, animation));
        }
        
        public void SetImageWidth(int value, PropertyChangeAnimation animation)
        {
            if (this.imageWidth == value)
            {
                return;
            }

            this.imageWidth = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("ImageWidth", value, animation));
        }
        
        public void SetImageHeight(int value, PropertyChangeAnimation animation)
        {
            if (this.imageHeight == value)
            {
                return;
            }

            this.imageHeight = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("ImageHeight", value, animation));
        }
        
        public void SetFallbackImage(string value, PropertyChangeAnimation animation)
        {
            if (this.fallbackImage == value)
            {
                return;
            }

            this.fallbackImage = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("FallbackImage", value, animation));
        }
        
        public void SetImages(string[] value, PropertyChangeAnimation animation)
        {
            if (this.images == value)
            {
                return;
            }

            this.images = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Images", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Overflow":
                    this.SetOverflow((TextOverflow)value, animation);
                    break;
                case "Align":
                    this.SetAlign((HorizontalAlignment)value, animation);
                    break;
                case "Direction":
                    this.SetDirection((TextDirection)value, animation);
                    break;
                case "HorizontalImageGap":
                    this.SetHorizontalImageGap((int)value, animation);
                    break;
                case "VerticalImageGap":
                    this.SetVerticalImageGap((int)value, animation);
                    break;
                case "ImageWidth":
                    this.SetImageWidth((int)value, animation);
                    break;
                case "ImageHeight":
                    this.SetImageHeight((int)value, animation);
                    break;
                case "FallbackImage":
                    this.SetFallbackImage((string)value, animation);
                    break;
                case "Images":
                    this.SetImages((string[])value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class TextItem : DrawableItemBase
    {
        private int rotation;
        private HorizontalAlignment align;
        private VerticalAlignment vAlign;
        private TextOverflow overflow;
        private int scrollSpeed;
        private Font font;
        private string text;

        public TextItem()
        {
            this.rotation = 0;
            this.vAlign = VerticalAlignment.Top;
            this.scrollSpeed = 0;
        }
            
        public int Rotation
        {
            get
            {
                return this.rotation;
            }

            set
            {
                this.SetRotation(value, null);
            }
        }
                
        public HorizontalAlignment Align
        {
            get
            {
                return this.align;
            }

            set
            {
                this.SetAlign(value, null);
            }
        }
                
        public VerticalAlignment VAlign
        {
            get
            {
                return this.vAlign;
            }

            set
            {
                this.SetVAlign(value, null);
            }
        }
                
        public TextOverflow Overflow
        {
            get
            {
                return this.overflow;
            }

            set
            {
                this.SetOverflow(value, null);
            }
        }
                
        public int ScrollSpeed
        {
            get
            {
                return this.scrollSpeed;
            }

            set
            {
                this.SetScrollSpeed(value, null);
            }
        }
                
        public Font Font
        {
            get
            {
                return this.font;
            }

            set
            {
                this.SetFont(value, null);
            }
        }
                
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.SetText(value, null);
            }
        }
        
        public void SetRotation(int value, PropertyChangeAnimation animation)
        {
            if (this.rotation == value)
            {
                return;
            }

            this.rotation = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Rotation", value, animation));
        }
        
        public void SetAlign(HorizontalAlignment value, PropertyChangeAnimation animation)
        {
            if (this.align == value)
            {
                return;
            }

            this.align = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Align", value, animation));
        }
        
        public void SetVAlign(VerticalAlignment value, PropertyChangeAnimation animation)
        {
            if (this.vAlign == value)
            {
                return;
            }

            this.vAlign = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("VAlign", value, animation));
        }
        
        public void SetOverflow(TextOverflow value, PropertyChangeAnimation animation)
        {
            if (this.overflow == value)
            {
                return;
            }

            this.overflow = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Overflow", value, animation));
        }
        
        public void SetScrollSpeed(int value, PropertyChangeAnimation animation)
        {
            if (this.scrollSpeed == value)
            {
                return;
            }

            this.scrollSpeed = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("ScrollSpeed", value, animation));
        }
        
        public void SetFont(Font value, PropertyChangeAnimation animation)
        {
            if (this.font == value)
            {
                return;
            }

            this.font = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Font", value, animation));
        }
        
        public void SetText(string value, PropertyChangeAnimation animation)
        {
            if (this.text == value)
            {
                return;
            }

            this.text = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Text", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Rotation":
                    this.SetRotation((int)value, animation);
                    break;
                case "Align":
                    this.SetAlign((HorizontalAlignment)value, animation);
                    break;
                case "VAlign":
                    this.SetVAlign((VerticalAlignment)value, animation);
                    break;
                case "Overflow":
                    this.SetOverflow((TextOverflow)value, animation);
                    break;
                case "ScrollSpeed":
                    this.SetScrollSpeed((int)value, animation);
                    break;
                case "Font":
                    this.SetFont((Font)value, animation);
                    break;
                case "Text":
                    this.SetText((string)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class VideoItem : DrawableItemBase
    {
        private string videoUri;
        private ElementScaling scaling;
        private bool replay;
        private string fallbackImage;

        public VideoItem()
        {
            this.scaling = ElementScaling.Stretch;
            this.replay = true;
        }
            
        public string VideoUri
        {
            get
            {
                return this.videoUri;
            }

            set
            {
                this.SetVideoUri(value, null);
            }
        }
                
        public ElementScaling Scaling
        {
            get
            {
                return this.scaling;
            }

            set
            {
                this.SetScaling(value, null);
            }
        }
                
        public bool Replay
        {
            get
            {
                return this.replay;
            }

            set
            {
                this.SetReplay(value, null);
            }
        }
                
        public string FallbackImage
        {
            get
            {
                return this.fallbackImage;
            }

            set
            {
                this.SetFallbackImage(value, null);
            }
        }
        
        public void SetVideoUri(string value, PropertyChangeAnimation animation)
        {
            if (this.videoUri == value)
            {
                return;
            }

            this.videoUri = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("VideoUri", value, animation));
        }
        
        public void SetScaling(ElementScaling value, PropertyChangeAnimation animation)
        {
            if (this.scaling == value)
            {
                return;
            }

            this.scaling = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Scaling", value, animation));
        }
        
        public void SetReplay(bool value, PropertyChangeAnimation animation)
        {
            if (this.replay == value)
            {
                return;
            }

            this.replay = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Replay", value, animation));
        }
        
        public void SetFallbackImage(string value, PropertyChangeAnimation animation)
        {
            if (this.fallbackImage == value)
            {
                return;
            }

            this.fallbackImage = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("FallbackImage", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "VideoUri":
                    this.SetVideoUri((string)value, animation);
                    break;
                case "Scaling":
                    this.SetScaling((ElementScaling)value, animation);
                    break;
                case "Replay":
                    this.SetReplay((bool)value, animation);
                    break;
                case "FallbackImage":
                    this.SetFallbackImage((string)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class RectangleItem : DrawableItemBase
    {
        private string color;

        public RectangleItem()
        {
        }
            
        public string Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.SetColor(value, null);
            }
        }
        
        public void SetColor(string value, PropertyChangeAnimation animation)
        {
            if (this.color == value)
            {
                return;
            }

            this.color = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Color", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Color":
                    this.SetColor((string)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public abstract partial class AudioItemBase : ScreenItemBase
    {
        private bool enabled;

        public AudioItemBase()
        {
            this.enabled = true;
        }
            
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                this.SetEnabled(value, null);
            }
        }
        
        public void SetEnabled(bool value, PropertyChangeAnimation animation)
        {
            if (this.enabled == value)
            {
                return;
            }

            this.enabled = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Enabled", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Enabled":
                    this.SetEnabled((bool)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public abstract partial class PlaybackItemBase : AudioItemBase
    {
        private int volume;
        private int priority;

        public PlaybackItemBase()
        {
        }
            
        public int Volume
        {
            get
            {
                return this.volume;
            }

            set
            {
                this.SetVolume(value, null);
            }
        }
                
        public int Priority
        {
            get
            {
                return this.priority;
            }

            set
            {
                this.SetPriority(value, null);
            }
        }
        
        public void SetVolume(int value, PropertyChangeAnimation animation)
        {
            if (this.volume == value)
            {
                return;
            }

            this.volume = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Volume", value, animation));
        }
        
        public void SetPriority(int value, PropertyChangeAnimation animation)
        {
            if (this.priority == value)
            {
                return;
            }

            this.priority = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Priority", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Volume":
                    this.SetVolume((int)value, animation);
                    break;
                case "Priority":
                    this.SetPriority((int)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class AudioFileItem : PlaybackItemBase
    {
        private string filename;

        public AudioFileItem()
        {
        }
            
        public string Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.SetFilename(value, null);
            }
        }
        
        public void SetFilename(string value, PropertyChangeAnimation animation)
        {
            if (this.filename == value)
            {
                return;
            }

            this.filename = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Filename", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Filename":
                    this.SetFilename((string)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class AudioPauseItem : PlaybackItemBase
    {
        private TimeSpan duration;

        public AudioPauseItem()
        {
        }
            
        [XmlIgnore]
        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                this.SetDuration(value, null);
            }
        }
        
        [XmlAttribute("Duration", DataType = "duration")]
        public string DurationXml
        {
            get
            {
                return XmlConvert.ToString(this.Duration);
            }

            set
            {
                this.Duration = XmlConvert.ToTimeSpan(value);
            }
        }
        
        public void SetDuration(TimeSpan value, PropertyChangeAnimation animation)
        {
            if (this.duration == value)
            {
                return;
            }

            this.duration = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Duration", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Duration":
                    this.SetDuration((TimeSpan)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
    public partial class TextToSpeechItem : PlaybackItemBase
    {
        private string voice;
        private string value;

        public TextToSpeechItem()
        {
        }
            
        public string Voice
        {
            get
            {
                return this.voice;
            }

            set
            {
                this.SetVoice(value, null);
            }
        }
                
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.SetValue(value, null);
            }
        }
        
        public void SetVoice(string value, PropertyChangeAnimation animation)
        {
            if (this.voice == value)
            {
                return;
            }

            this.voice = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Voice", value, animation));
        }
        
        public void SetValue(string value, PropertyChangeAnimation animation)
        {
            if (this.value == value)
            {
                return;
            }

            this.value = value;
            this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Value", value, animation));
        }
            
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Voice":
                    this.SetVoice((string)value, animation);
                    break;
                case "Value":
                    this.SetValue((string)value, animation);
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
    
}

