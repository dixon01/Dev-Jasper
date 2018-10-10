

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Common;

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    
    [Serializable]
    public partial class Font : ICloneable
    {
        public Font()
        {   
            this.CharSpacing = 1;
    
        }
    
        [XmlAttribute("Face")]
        public string Face { get; set; }
    
        [XmlAttribute("Height")]
        public int Height { get; set; }
    
        [XmlAttribute("Weight")]
        public int Weight { get; set; }
    
        [XmlAttribute("Italic")]
        public bool Italic { get; set; }
    
        [XmlAttribute("Color")]
        public string Color { get; set; }
    
        [XmlAttribute("OutlineColor")]
        public string OutlineColor { get; set; }
    
        [XmlAttribute("CharSpacing")]
        [DefaultValue(1)]
        public int CharSpacing { get; set; }
            
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    
    [Serializable]
    public abstract partial class ElementBase : ElementId, ICloneable 
    {
        public ElementBase()
        {
        }
    
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        
    }

    [Serializable]
    public abstract partial class GraphicalElementBase : ElementBase
    {
        public GraphicalElementBase()
        {   
            this.Width = -1;
       
            this.Height = -1;
       
            this.Visible = true;
    
        }
    
        [XmlAttribute("X")]
        public int X { get; set; }
    
        [XmlAttribute("Y")]
        public int Y { get; set; }
    
        [XmlAttribute("Width")]
        [DefaultValue(-1)]
        public int Width { get; set; }
    
        [XmlAttribute("Height")]
        [DefaultValue(-1)]
        public int Height { get; set; }
    
        [XmlAttribute("Visible")]
        [DefaultValue(true)]
        public bool Visible { get; set; }
                    
        [XmlElement("Visible")]
        public AnimatedDynamicProperty VisibleProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (GraphicalElementBase)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT, Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.LED)]
    [XmlRoot("Group")]
    public partial class GroupElement : GraphicalElementBase, ICompositeElement, IXmlSerializable
    {
        public GroupElement()
        {
            this.Elements = new List<GraphicalElementBase>();
        }
    
        public override object Clone()
        {
            var clone = (GroupElement)base.Clone();
            clone.Elements = this.Elements.ConvertAll(e => (GraphicalElementBase)e.Clone());
            return clone;
        }
        
        public List<GraphicalElementBase> Elements { get; set; }

        IEnumerator<ElementBase> IEnumerable<ElementBase>.GetEnumerator()
        {
            return this.Elements.ConvertAll(e => (ElementBase)e).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Elements.GetEnumerator();
        }
        
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();
        
            var attrX = reader.GetAttribute("X");
            if (attrX != null)
            {
                this.X = XmlConvert.ToInt32(attrX);
            }
        
            var attrY = reader.GetAttribute("Y");
            if (attrY != null)
            {
                this.Y = XmlConvert.ToInt32(attrY);
            }
        
            var attrWidth = reader.GetAttribute("Width");
            if (attrWidth != null)
            {
                this.Width = XmlConvert.ToInt32(attrWidth);
            }
        
            var attrHeight = reader.GetAttribute("Height");
            if (attrHeight != null)
            {
                this.Height = XmlConvert.ToInt32(attrHeight);
            }
        
            var attrVisible = reader.GetAttribute("Visible");
            if (attrVisible != null)
            {
                this.Visible = XmlConvert.ToBoolean(attrVisible);
            }
        
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }
            
            reader.ReadStartElement();

            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
        
                if (reader.Name == "Visible")
                {
                    this.VisibleProperty = AnimatedDynamicProperty.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                var element = ElementSerializer.Deserialize(reader) as GraphicalElementBase;
                if (element != null)
                {
                    this.Elements.Add(element);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }
        
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
        
            writer.WriteAttributeString("X", XmlConvert.ToString(this.X));

            writer.WriteAttributeString("Y", XmlConvert.ToString(this.Y));

            if (this.Width != -1)
            {
                writer.WriteAttributeString("Width", XmlConvert.ToString(this.Width));
            }

            if (this.Height != -1)
            {
                writer.WriteAttributeString("Height", XmlConvert.ToString(this.Height));
            }

            if (this.Visible != true)
            {
                writer.WriteAttributeString("Visible", XmlConvert.ToString(this.Visible));
            }

            DynamicProperty.WriteToXml("Visible", this.VisibleProperty, writer);
            foreach (var element in this.Elements)
            {
                ElementSerializer.Serialize(element, writer);
            }
        }
        
    }

    [Serializable]
    public abstract partial class DrawableElementBase : GraphicalElementBase
    {
        public DrawableElementBase()
        {
        }
    
        [XmlAttribute("ZIndex")]
        public int ZIndex { get; set; }
    
        public override object Clone()
        {
            var clone = (DrawableElementBase)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT)]
    [XmlRoot("AnalogClock")]
    public partial class AnalogClockElement : DrawableElementBase
    {
        public AnalogClockElement()
        {
        }
    
        [XmlElement("Hour")]
        public AnalogClockHandElement Hour { get; set; }
        
        [XmlElement("Minute")]
        public AnalogClockHandElement Minute { get; set; }
        
        [XmlElement("Seconds")]
        public AnalogClockHandElement Seconds { get; set; }
        
        public override object Clone()
        {
            var clone = (AnalogClockElement)base.Clone();
            clone.Hour = (AnalogClockHandElement)this.Hour.Clone();
            clone.Minute = (AnalogClockHandElement)this.Minute.Clone();
            clone.Seconds = (AnalogClockHandElement)this.Seconds.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT)]
    [XmlRoot("AnalogClockHand")]
    public partial class AnalogClockHandElement : ImageElement
    {
        public AnalogClockHandElement()
        {
        }
    
        [XmlAttribute("Mode")]
        public AnalogClockHandMode Mode { get; set; }
    
        [XmlAttribute("CenterX")]
        public int CenterX { get; set; }
    
        [XmlAttribute("CenterY")]
        public int CenterY { get; set; }
    
        public override object Clone()
        {
            var clone = (AnalogClockHandElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT)]
    [XmlRoot("Frame")]
    public partial class FrameElement : DrawableElementBase
    {
        public FrameElement()
        {
        }
    
        [XmlAttribute("Id")]
        public int FrameId { get; set; }
    
        public override object Clone()
        {
            var clone = (FrameElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT, Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.LED)]
    [XmlRoot("Image")]
    public partial class ImageElement : DrawableElementBase
    {
        public ImageElement()
        {   
            this.Scaling = ElementScaling.Stretch;
    
        }
    
        [XmlAttribute("Filename")]
        public string Filename { get; set; }
                    
        [XmlElement("Filename")]
        public AnimatedDynamicProperty FilenameProperty { get; set; }
    
        [XmlAttribute("Scaling")]
        [DefaultValue(ElementScaling.Stretch)]
        public ElementScaling Scaling { get; set; }
    
        public override object Clone()
        {
            var clone = (ImageElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT)]
    [XmlRoot("ImageList")]
    public partial class ImageListElement : DrawableElementBase
    {
        public ImageListElement()
        {   
            this.Delimiter = ";";
    
        }
    
        [XmlAttribute("Overflow")]
        public TextOverflow Overflow { get; set; }
    
        [XmlAttribute("Align")]
        public HorizontalAlignment Align { get; set; }
    
        [XmlAttribute("Direction")]
        public TextDirection Direction { get; set; }
    
        [XmlAttribute("HorizontalImageGap")]
        public int HorizontalImageGap { get; set; }
    
        [XmlAttribute("VerticalImageGap")]
        public int VerticalImageGap { get; set; }
    
        [XmlAttribute("ImageWidth")]
        public int ImageWidth { get; set; }
    
        [XmlAttribute("ImageHeight")]
        public int ImageHeight { get; set; }
    
        [XmlAttribute("Delimiter")]
        [DefaultValue(";")]
        public string Delimiter { get; set; }
    
        [XmlAttribute("FilePatterns")]
        public string FilePatterns { get; set; }
    
        [XmlAttribute("FallbackImage")]
        public string FallbackImage { get; set; }
    
        [XmlAttribute("Values")]
        public string Values { get; set; }
                    
        [XmlElement("Values")]
        public DynamicProperty ValuesProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (ImageListElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT, Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.LED)]
    [XmlRoot("Text")]
    public partial class TextElement : DrawableElementBase
    {
        public TextElement()
        {   
            this.Rotation = 0;
       
            this.VAlign = VerticalAlignment.Top;
       
            this.ScrollSpeed = 0;
    
        }
    
        [XmlAttribute("Rotation")]
        [DefaultValue(0)]
        public int Rotation { get; set; }
    
        [XmlAttribute("Align")]
        public HorizontalAlignment Align { get; set; }
    
        [XmlAttribute("VAlign")]
        [DefaultValue(VerticalAlignment.Top)]
        public VerticalAlignment VAlign { get; set; }
    
        [XmlAttribute("Overflow")]
        public TextOverflow Overflow { get; set; }
    
        [XmlAttribute("ScrollSpeed")]
        [DefaultValue(0)]
        public int ScrollSpeed { get; set; }
    
        [XmlElement("Font")]
        public Font Font { get; set; }
        
        [XmlAttribute("Value")]
        public string Value { get; set; }
                    
        [XmlElement("Value")]
        public AnimatedDynamicProperty ValueProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (TextElement)base.Clone();
            clone.Font = (Font)this.Font.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.TFT)]
    [XmlRoot("Video")]
    public partial class VideoElement : DrawableElementBase
    {
        public VideoElement()
        {   
            this.Scaling = ElementScaling.Stretch;
       
            this.Replay = true;
    
        }
    
        [XmlAttribute("VideoUri")]
        public string VideoUri { get; set; }
                    
        [XmlElement("VideoUri")]
        public AnimatedDynamicProperty VideoUriProperty { get; set; }
    
        [XmlAttribute("Scaling")]
        [DefaultValue(ElementScaling.Stretch)]
        public ElementScaling Scaling { get; set; }
    
        [XmlAttribute("Replay")]
        [DefaultValue(true)]
        public bool Replay { get; set; }
    
        [XmlAttribute("FallbackImage")]
        public string FallbackImage { get; set; }
    
        public override object Clone()
        {
            var clone = (VideoElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.LED)]
    [XmlRoot("Rectangle")]
    public partial class RectangleElement : DrawableElementBase
    {
        public RectangleElement()
        {
        }
    
        [XmlAttribute("Color")]
        public string Color { get; set; }
                    
        [XmlElement("Color")]
        public AnimatedDynamicProperty ColorProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (RectangleElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    public abstract partial class AudioElementBase : ElementBase
    {
        public AudioElementBase()
        {   
            this.Enabled = true;
    
        }
    
        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }
                    
        [XmlElement("Enabled")]
        public DynamicProperty EnabledProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (AudioElementBase)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.Audio)]
    [XmlRoot("AudioOutput")]
    public partial class AudioOutputElement : AudioElementBase, ICompositeElement, IXmlSerializable
    {
        public AudioOutputElement()
        {
            this.Elements = new List<PlaybackElementBase>();
        }
    
        [XmlAttribute("Volume")]
        public int Volume { get; set; }
                    
        [XmlElement("Volume")]
        public DynamicProperty VolumeProperty { get; set; }
    
        [XmlAttribute("Priority")]
        public int Priority { get; set; }
    
        public override object Clone()
        {
            var clone = (AudioOutputElement)base.Clone();
            clone.Elements = this.Elements.ConvertAll(e => (PlaybackElementBase)e.Clone());
            return clone;
        }
        
        public List<PlaybackElementBase> Elements { get; set; }

        IEnumerator<ElementBase> IEnumerable<ElementBase>.GetEnumerator()
        {
            return this.Elements.ConvertAll(e => (ElementBase)e).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Elements.GetEnumerator();
        }
        
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();
        
            var attrEnabled = reader.GetAttribute("Enabled");
            if (attrEnabled != null)
            {
                this.Enabled = XmlConvert.ToBoolean(attrEnabled);
            }
        
            var attrVolume = reader.GetAttribute("Volume");
            if (attrVolume != null)
            {
                this.Volume = XmlConvert.ToInt32(attrVolume);
            }
        
            var attrPriority = reader.GetAttribute("Priority");
            if (attrPriority != null)
            {
                this.Priority = XmlConvert.ToInt32(attrPriority);
            }
        
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }
            
            reader.ReadStartElement();

            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
        
                if (reader.Name == "Enabled")
                {
                    this.EnabledProperty = DynamicProperty.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                if (reader.Name == "Volume")
                {
                    this.VolumeProperty = DynamicProperty.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                var element = ElementSerializer.Deserialize(reader) as PlaybackElementBase;
                if (element != null)
                {
                    this.Elements.Add(element);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }
        
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
        
            if (this.Enabled != true)
            {
                writer.WriteAttributeString("Enabled", XmlConvert.ToString(this.Enabled));
            }

            writer.WriteAttributeString("Volume", XmlConvert.ToString(this.Volume));

            writer.WriteAttributeString("Priority", XmlConvert.ToString(this.Priority));

            DynamicProperty.WriteToXml("Enabled", this.EnabledProperty, writer);

            DynamicProperty.WriteToXml("Volume", this.VolumeProperty, writer);
            foreach (var element in this.Elements)
            {
                ElementSerializer.Serialize(element, writer);
            }
        }
        
    }

    [Serializable]
    public abstract partial class PlaybackElementBase : AudioElementBase
    {
        public PlaybackElementBase()
        {
        }
    
        public override object Clone()
        {
            var clone = (PlaybackElementBase)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.Audio)]
    [XmlRoot("AudioFile")]
    public partial class AudioFileElement : PlaybackElementBase
    {
        public AudioFileElement()
        {
        }
    
        [XmlAttribute("Filename")]
        public string Filename { get; set; }
                    
        [XmlElement("Filename")]
        public DynamicProperty FilenameProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (AudioFileElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.Audio)]
    [XmlRoot("AudioPause")]
    public partial class AudioPauseElement : PlaybackElementBase
    {
        public AudioPauseElement()
        {
        }
    
        [XmlIgnore]
        public TimeSpan Duration { get; set; }
        
        [XmlAttribute("Duration", DataType = "duration")]
        public string DurationXml { get { return XmlConvert.ToString(this.Duration); } set { this.Duration = XmlConvert.ToTimeSpan(value); } }
    
        public override object Clone()
        {
            var clone = (AudioPauseElement)base.Clone();
            return clone;
        }
        
    }

    [Serializable]
    [SupportedScreenTypes(Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenType.Audio)]
    [XmlRoot("TextToSpeech")]
    public partial class TextToSpeechElement : PlaybackElementBase
    {
        public TextToSpeechElement()
        {
        }
    
        [XmlAttribute("Voice")]
        public string Voice { get; set; }
                    
        [XmlElement("Voice")]
        public DynamicProperty VoiceProperty { get; set; }
    
        [XmlAttribute("Value")]
        public string Value { get; set; }
                    
        [XmlElement("Value")]
        public DynamicProperty ValueProperty { get; set; }
    
        public override object Clone()
        {
            var clone = (TextToSpeechElement)base.Clone();
            return clone;
        }
        
    }
}

