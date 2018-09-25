

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Common;
using Gorba.Common.Configuration.Infomedia.Presentation;

namespace Gorba.Common.Configuration.Infomedia.Presentation
{    
    [XmlRoot("Infomedia")]
    [Serializable]
    public partial class InfomediaConfig
    {
        public InfomediaConfig()
        {   
            this.PhysicalScreens = new List<PhysicalScreenConfig>();   
            this.VirtualDisplays = new List<VirtualDisplayConfig>();   
            this.Evaluations = new List<EvaluationConfig>();   
            this.CyclePackages = new List<CyclePackageConfig>();   
            this.Pools = new List<PoolConfig>();   
            this.Layouts = new List<LayoutConfig>();   
            this.Fonts = new List<FontConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlIgnore]
        public Version Version { get; set; }
        
        [XmlAttribute("Version")]
        public string VersionXml { get { return this.Version.ToString(); } set { this.Version = new Version(value); } }
    
        [XmlIgnore]
        public DateTime CreationDate { get; set; }
        
        [XmlAttribute("Created")]
        public string CreationDateXml { get { return this.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"); } set { this.CreationDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); } }
    
        [XmlArrayItem("PhysicalScreen")]
        public List<PhysicalScreenConfig> PhysicalScreens { get; set; }
        
        [XmlArrayItem("VirtualDisplay")]
        public List<VirtualDisplayConfig> VirtualDisplays { get; set; }
        
        [XmlElement("MasterPresentation")]
        public MasterPresentationConfig MasterPresentation { get; set; }
        
        [XmlArrayItem("Evaluation")]
        public List<EvaluationConfig> Evaluations { get; set; }
        
        [XmlElement("Cycles")]
        public CyclesConfig Cycles { get; set; }
        
        [XmlArrayItem("CyclePackage")]
        public List<CyclePackageConfig> CyclePackages { get; set; }
        
        [XmlArrayItem("Pool")]
        public List<PoolConfig> Pools { get; set; }
        
        [XmlArrayItem("Layout")]
        public List<LayoutConfig> Layouts { get; set; }
        
        [XmlArrayItem("Font")]
        public List<FontConfig> Fonts { get; set; }
        
    }
    
    [XmlRoot("CyclePackage")]
    [Serializable]
    public partial class CyclePackageConfig
    {
        public CyclePackageConfig()
        {   
            this.StandardCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.StandardCycleRefConfig>();   
            this.EventCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.EventCycleRefConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
        [XmlArrayItem("StandardCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.StandardCycleRefConfig> StandardCycles { get; set; }
        
        [XmlArrayItem("EventCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.EventCycleRefConfig> EventCycles { get; set; }
        
    }
    
    [XmlRoot("Cycles")]
    [Serializable]
    public partial class CyclesConfig
    {
        public CyclesConfig()
        {   
            this.StandardCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.StandardCycleConfig>();   
            this.EventCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.EventCycleConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlArrayItem("StandardCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.StandardCycleConfig> StandardCycles { get; set; }
        
        [XmlArrayItem("EventCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.EventCycleConfig> EventCycles { get; set; }
        
    }
    
    [XmlRoot("Evaluation")]
    [Serializable]
    public partial class EvaluationConfig : Gorba.Common.Configuration.Infomedia.Eval.ContainerEvalBase
    {
        public EvaluationConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
    }
    
    [XmlRoot("Font")]
    [Serializable]
    public partial class FontConfig
    {
        public FontConfig()
        {   
            this.ScreenType = PhysicalScreenType.TFT;
    
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Path")]
        public string Path { get; set; }
    
        [XmlAttribute("ScreenType")]
        [DefaultValue(PhysicalScreenType.TFT)]
        public PhysicalScreenType ScreenType { get; set; }
    
    }
    
    [XmlRoot("LayoutBase")]
    [Serializable]
    public abstract partial class LayoutConfigBase
    {
        public LayoutConfigBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
    }
    
    [XmlRoot("Layout")]
    [Serializable]
    public partial class LayoutConfig : LayoutConfigBase
    {
        public LayoutConfig()
        {   
            this.Resolutions = new List<ResolutionConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("BaseLayoutName")]
        public string BaseLayoutName { get; set; }
        
        [XmlElement("Resolution")]
        public List<ResolutionConfig> Resolutions { get; set; }
        
    }
    
    [XmlRoot("MasterLayout")]
    [Serializable]
    public partial class MasterLayoutConfig : LayoutConfigBase
    {
        public MasterLayoutConfig()
        {   
            this.PhysicalScreens = new List<PhysicalScreenRefConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("PhysicalScreen")]
        public List<PhysicalScreenRefConfig> PhysicalScreens { get; set; }
        
    }
    
    [XmlRoot("MasterPresentation")]
    [Serializable]
    public partial class MasterPresentationConfig
    {
        public MasterPresentationConfig()
        {   
            this.MasterCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.MasterCycleConfig>();   
            this.MasterEventCycles = new List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.MasterEventCycleConfig>();   
            this.MasterLayouts = new List<MasterLayoutConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlArrayItem("MasterCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.MasterCycleConfig> MasterCycles { get; set; }
        
        [XmlArrayItem("EventCycle")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Cycle.MasterEventCycleConfig> MasterEventCycles { get; set; }
        
        [XmlArrayItem("MasterLayout")]
        public List<MasterLayoutConfig> MasterLayouts { get; set; }
        
    }
    
    [XmlRoot("PhysicalScreen")]
    [Serializable]
    public partial class PhysicalScreenConfig
    {
        public PhysicalScreenConfig()
        {   
            this.Visible = true;
    
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
        [XmlAttribute("Type")]
        public PhysicalScreenType Type { get; set; }
    
        [XmlAttribute("Id")]
        public string Identifier { get; set; }
    
        [XmlAttribute("Width")]
        public int Width { get; set; }
    
        [XmlAttribute("Height")]
        public int Height { get; set; }
    
        [XmlAttribute("Visible")]
        [DefaultValue(true)]
        public bool Visible { get; set; }
                    
        [XmlElement("Visible")]
        public AnimatedDynamicProperty VisibleProperty { get; set; }
    
    }
    
    [XmlRoot("PhysicalScreenRef")]
    [Serializable]
    public partial class PhysicalScreenRefConfig
    {
        public PhysicalScreenRefConfig()
        {   
            this.VirtualDisplays = new List<VirtualDisplayRefConfig>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Ref")]
        public string Reference { get; set; }
        
        [XmlElement("VirtualDisplay")]
        public List<VirtualDisplayRefConfig> VirtualDisplays { get; set; }
        
    }
    
    [XmlRoot("Pool")]
    [Serializable]
    public partial class PoolConfig
    {
        public PoolConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
        [XmlAttribute("BaseDirectory")]
        public string BaseDirectory { get; set; }
    
    }
    
    [XmlRoot("Resolution")]
    [Serializable]
    public partial class ResolutionConfig : IXmlSerializable
    {
        public ResolutionConfig()
        {   
            this.Elements = new List<Gorba.Common.Configuration.Infomedia.Layout.ElementBase>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Width")]
        public int Width { get; set; }
    
        [XmlAttribute("Height")]
        public int Height { get; set; }
    
        [XmlElement("Layout.Base")]
        public List<Gorba.Common.Configuration.Infomedia.Layout.ElementBase> Elements { get; set; }
        
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();

            this.ReadXmlAttributes(reader);

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    var item = Gorba.Common.Configuration.Infomedia.Layout.ElementSerializer.Deserialize(reader);
                    if (item != null)
                    {
                        this.Elements.Add(item);
                    }
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }
        
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);

            foreach (var item in this.Elements)
            {
                Gorba.Common.Configuration.Infomedia.Layout.ElementSerializer.Serialize(item, writer);
            }
        }
        
        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
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
        
        }
        
        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }
        
        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Width", XmlConvert.ToString(this.Width));

            writer.WriteAttributeString("Height", XmlConvert.ToString(this.Height));

        }
        
        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }
        
    }
    
    [XmlRoot("VirtualDisplay")]
    [Serializable]
    public partial class VirtualDisplayConfig
    {
        public VirtualDisplayConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
        [XmlAttribute("CyclePackage")]
        public string CyclePackage { get; set; }
        
        [XmlAttribute("Width")]
        public int Width { get; set; }
    
        [XmlAttribute("Height")]
        public int Height { get; set; }
    
    }
    
    [XmlRoot("VirtualDisplayRef")]
    [Serializable]
    public partial class VirtualDisplayRefConfig : Gorba.Common.Configuration.Infomedia.Layout.DrawableElementBase
    {
        public VirtualDisplayRefConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Ref")]
        public string Reference { get; set; }
        
    }

}

namespace Gorba.Common.Configuration.Infomedia.Presentation.Cycle
{    
    [XmlRoot("CycleBase")]
    [Serializable]
    public abstract partial class CycleConfigBase : IXmlSerializable
    {
        public CycleConfigBase()
        {   
            this.Enabled = true;
       
            this.Sections = new List<Gorba.Common.Configuration.Infomedia.Presentation.Section.SectionConfigBase>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Name")]
        public string Name { get; set; }
    
        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }
                    
        [XmlElement("Enabled")]
        public DynamicProperty EnabledProperty { get; set; }
    
        [XmlElement("Presentation.Section.SectionBase")]
        public List<Gorba.Common.Configuration.Infomedia.Presentation.Section.SectionConfigBase> Sections { get; set; }
        
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Sections.Clear();

            this.ReadXmlAttributes(reader);

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    var item = Gorba.Common.Configuration.Infomedia.Presentation.Section.SectionConfigSerializer.Deserialize(reader);
                    if (item != null)
                    {
                        this.Sections.Add(item);
                    }
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }
        
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);

            foreach (var item in this.Sections)
            {
                Gorba.Common.Configuration.Infomedia.Presentation.Section.SectionConfigSerializer.Serialize(item, writer);
            }
        }
        
        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
            var attrName = reader.GetAttribute("Name");
            if (attrName != null)
            {
                this.Name = attrName;
            }
        
            var attrEnabled = reader.GetAttribute("Enabled");
            if (attrEnabled != null)
            {
                this.Enabled = XmlConvert.ToBoolean(attrEnabled);
            }
        
        }
        
        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            if (elementName == "Enabled")
            {
                this.EnabledProperty = DynamicProperty.ReadFromXml(reader);
                return true;
            }

            return false;
        }
        
        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);

            if (this.Enabled != true)
            {
                writer.WriteAttributeString("Enabled", XmlConvert.ToString(this.Enabled));
            }

        }
        
        protected virtual void WriteXmlElements(XmlWriter writer)
        {
            DynamicProperty.WriteToXml("Enabled", this.EnabledProperty, writer);

        }
        
    }
    
    [XmlRoot("CycleRefBase")]
    [Serializable]
    public abstract partial class CycleRefConfigBase
    {
        public CycleRefConfigBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Ref")]
        public string Reference { get; set; }
        
    }
    
    [XmlRoot("EventCycle")]
    [Serializable]
    public partial class EventCycleConfig : EventCycleConfigBase
    {
        public EventCycleConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("EventCycleBase")]
    [Serializable]
    public abstract partial class EventCycleConfigBase : CycleConfigBase
    {
        public EventCycleConfigBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Trigger")]
        public GenericTriggerConfig Trigger { get; set; }
        
    }
    
    [XmlRoot("EventCycleRef")]
    [Serializable]
    public partial class EventCycleRefConfig : CycleRefConfigBase
    {
        public EventCycleRefConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("GenericTrigger")]
    [Serializable]
    public partial class GenericTriggerConfig
    {
        public GenericTriggerConfig()
        {   
            this.Coordinates = new List<Gorba.Common.Configuration.Infomedia.Eval.GenericEval>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Generic")]
        public List<Gorba.Common.Configuration.Infomedia.Eval.GenericEval> Coordinates { get; set; }
        
    }
    
    [XmlRoot("MasterCycle")]
    [Serializable]
    public partial class MasterCycleConfig : CycleConfigBase
    {
        public MasterCycleConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("MasterEventCycle")]
    [Serializable]
    public partial class MasterEventCycleConfig : EventCycleConfigBase
    {
        public MasterEventCycleConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("StandardCycle")]
    [Serializable]
    public partial class StandardCycleConfig : CycleConfigBase
    {
        public StandardCycleConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("StandardCycleRef")]
    [Serializable]
    public partial class StandardCycleRefConfig : CycleRefConfigBase
    {
        public StandardCycleRefConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }

}

namespace Gorba.Common.Configuration.Infomedia.Presentation.Section
{    
    [XmlRoot("ImageSection")]
    [Serializable]
    public partial class ImageSectionConfig : SectionConfigBase
    {
        public ImageSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Filename")]
        public string Filename { get; set; }
    
        [XmlAttribute("Frame")]
        public int Frame { get; set; }
    
    }
    
    [XmlRoot("MasterSection")]
    [Serializable]
    public partial class MasterSectionConfig : SectionConfigBase
    {
        public MasterSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("MultiSection")]
    [Serializable]
    public partial class MultiSectionConfig : SectionConfigBase
    {
        public MultiSectionConfig()
        {   
            this.MaxPages = -1;
    
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Language")]
        public int Language { get; set; }
    
        [XmlAttribute("Table")]
        public int Table { get; set; }
    
        [XmlAttribute("RowsPerPage")]
        public int RowsPerPage { get; set; }
    
        [XmlAttribute("MaxPages")]
        [DefaultValue(-1)]
        public int MaxPages { get; set; }
    
        [XmlAttribute("Mode")]
        public PageMode Mode { get; set; }
    
    }
    
    [XmlRoot("PoolSection")]
    [Serializable]
    public partial class PoolSectionConfig : SectionConfigBase
    {
        public PoolSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Pool")]
        public string Pool { get; set; }
        
        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode { get; set; }
    
        [XmlAttribute("Frame")]
        public int Frame { get; set; }
    
    }
    
    [XmlRoot("SectionBase")]
    [Serializable]
    public abstract partial class SectionConfigBase
    {
        public SectionConfigBase()
        {   
            this.Enabled = true;
    
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }
                    
        [XmlElement("Enabled")]
        public DynamicProperty EnabledProperty { get; set; }
    
        [XmlIgnore]
        public TimeSpan Duration { get; set; }
        
        [XmlAttribute("Duration", DataType = "duration")]
        public string DurationXml { get { return XmlConvert.ToString(this.Duration); } set { this.Duration = XmlConvert.ToTimeSpan(value); } }
    
        [XmlAttribute("Layout")]
        public string Layout { get; set; }
        
    }
    
    [XmlRoot("StandardSection")]
    [Serializable]
    public partial class StandardSectionConfig : SectionConfigBase
    {
        public StandardSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("VideoSection")]
    [Serializable]
    public partial class VideoSectionConfig : SectionConfigBase
    {
        public VideoSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("VideoUri")]
        public string VideoUri { get; set; }
    
        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode { get; set; }
    
        [XmlAttribute("Frame")]
        public int Frame { get; set; }
    
    }
    
    [XmlRoot("WebmediaSection")]
    [Serializable]
    public partial class WebmediaSectionConfig : SectionConfigBase
    {
        public WebmediaSectionConfig()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Filename")]
        public string Filename { get; set; }
    
        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode { get; set; }
    
    }

}


