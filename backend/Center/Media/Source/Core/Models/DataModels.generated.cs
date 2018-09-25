namespace Gorba.Center.Media.Core.Models.Layout
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    [XmlRoot("Font")]
    [Serializable]
    public partial class FontDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public FontDataModel()
        {
            this.CharSpacing = 1;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Face")]
        public string Face
        {
            get;
            set;
        }

        [XmlAttribute("Height")]
        public int Height
        {
            get;
            set;
        }

        [XmlAttribute("Weight")]
        public int Weight
        {
            get;
            set;
        }

        [XmlAttribute("Italic")]
        public bool Italic
        {
            get;
            set;
        }

        [XmlAttribute("Color")]
        public string Color
        {
            get;
            set;
        }

        [XmlAttribute("OutlineColor")]
        public string OutlineColor
        {
            get;
            set;
        }

        [XmlAttribute("CharSpacing")]
        [DefaultValue(1)]
        public int CharSpacing
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("GraphicalBase")]
    [Serializable]
    public partial class GraphicalElementDataModelBase : LayoutElementDataModelBase
    {
        public GraphicalElementDataModelBase()
        {
            this.Width = -1;
            this.Height = -1;
            this.Visible = true;
            this.AdditionalInitialization();
        }

        [XmlAttribute("X")]
        public int X
        {
            get;
            set;
        }

        [XmlAttribute("Y")]
        public int Y
        {
            get;
            set;
        }

        [XmlAttribute("Width")]
        [DefaultValue(-1)]
        public int Width
        {
            get;
            set;
        }

        [XmlAttribute("Height")]
        [DefaultValue(-1)]
        public int Height
        {
            get;
            set;
        }

        [XmlAttribute("Visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get;
            set;
        }

        [XmlElement("Visible")]
        public AnimatedDynamicPropertyDataModel VisibleProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("DrawableBase")]
    [Serializable]
    public partial class DrawableElementDataModelBase : Gorba.Center.Media.Core.Models.Layout.GraphicalElementDataModelBase
    {
        public DrawableElementDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("ZIndex")]
        public int ZIndex
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AnalogClock")]
    [Serializable]
    public partial class AnalogClockElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public AnalogClockElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlElement("Hour")]
        public Gorba.Center.Media.Core.Models.Layout.AnalogClockHandElementDataModel Hour
        {
            get;
            set;
        }

        [XmlElement("Minute")]
        public Gorba.Center.Media.Core.Models.Layout.AnalogClockHandElementDataModel Minute
        {
            get;
            set;
        }

        [XmlElement("Seconds")]
        public Gorba.Center.Media.Core.Models.Layout.AnalogClockHandElementDataModel Seconds
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AnalogClockHand")]
    [Serializable]
    public partial class AnalogClockHandElementDataModel : Gorba.Center.Media.Core.Models.Layout.ImageElementDataModel
    {
        public AnalogClockHandElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Mode")]
        public AnalogClockHandMode Mode
        {
            get;
            set;
        }

        [XmlAttribute("CenterX")]
        public int CenterX
        {
            get;
            set;
        }

        [XmlAttribute("CenterY")]
        public int CenterY
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Frame")]
    [Serializable]
    public partial class FrameElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public FrameElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Id")]
        public int FrameId
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Image")]
    [Serializable]
    public partial class ImageElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public ImageElementDataModel()
        {
            this.Scaling = ElementScaling.Stretch;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Filename")]
        public string Filename
        {
            get;
            set;
        }

        [XmlElement("Filename")]
        public AnimatedDynamicPropertyDataModel FilenameProperty
        {
            get;
            set;
        }

        [XmlAttribute("Scaling")]
        [DefaultValue(ElementScaling.Stretch)]
        public ElementScaling Scaling
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("ImageList")]
    [Serializable]
    public partial class ImageListElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public ImageListElementDataModel()
        {
            this.Delimiter = ";";
            this.AdditionalInitialization();
        }

        [XmlAttribute("Overflow")]
        public TextOverflow Overflow
        {
            get;
            set;
        }

        [XmlAttribute("Align")]
        public HorizontalAlignment Align
        {
            get;
            set;
        }

        [XmlAttribute("Direction")]
        public TextDirection Direction
        {
            get;
            set;
        }

        [XmlAttribute("HorizontalImageGap")]
        public int HorizontalImageGap
        {
            get;
            set;
        }

        [XmlAttribute("VerticalImageGap")]
        public int VerticalImageGap
        {
            get;
            set;
        }

        [XmlAttribute("ImageWidth")]
        public int ImageWidth
        {
            get;
            set;
        }

        [XmlAttribute("ImageHeight")]
        public int ImageHeight
        {
            get;
            set;
        }

        [XmlAttribute("Delimiter")]
        [DefaultValue(";")]
        public string Delimiter
        {
            get;
            set;
        }

        [XmlAttribute("FilePatterns")]
        public string FilePatterns
        {
            get;
            set;
        }

        [XmlAttribute("FallbackImage")]
        public string FallbackImage
        {
            get;
            set;
        }

        [XmlAttribute("Values")]
        public string Values
        {
            get;
            set;
        }

        [XmlElement("Values")]
        public DynamicPropertyDataModel ValuesProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Text")]
    [Serializable]
    public partial class TextElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public TextElementDataModel()
        {
            this.Rotation = 0;
            this.VAlign = VerticalAlignment.Top;
            this.ScrollSpeed = 0;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Rotation")]
        [DefaultValue(0)]
        public int Rotation
        {
            get;
            set;
        }

        [XmlAttribute("Align")]
        public HorizontalAlignment Align
        {
            get;
            set;
        }

        [XmlAttribute("VAlign")]
        [DefaultValue(VerticalAlignment.Top)]
        public VerticalAlignment VAlign
        {
            get;
            set;
        }

        [XmlAttribute("Overflow")]
        public TextOverflow Overflow
        {
            get;
            set;
        }

        [XmlAttribute("ScrollSpeed")]
        [DefaultValue(0)]
        public int ScrollSpeed
        {
            get;
            set;
        }

        [XmlElement("Font")]
        public Gorba.Center.Media.Core.Models.Layout.FontDataModel Font
        {
            get;
            set;
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        [XmlElement("Value")]
        public AnimatedDynamicPropertyDataModel ValueProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Video")]
    [Serializable]
    public partial class VideoElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public VideoElementDataModel()
        {
            this.Scaling = ElementScaling.Stretch;
            this.Replay = true;
            this.AdditionalInitialization();
        }

        [XmlAttribute("VideoUri")]
        public string VideoUri
        {
            get;
            set;
        }

        [XmlElement("VideoUri")]
        public AnimatedDynamicPropertyDataModel VideoUriProperty
        {
            get;
            set;
        }

        [XmlAttribute("Scaling")]
        [DefaultValue(ElementScaling.Stretch)]
        public ElementScaling Scaling
        {
            get;
            set;
        }

        [XmlAttribute("Replay")]
        [DefaultValue(true)]
        public bool Replay
        {
            get;
            set;
        }

        [XmlAttribute("FallbackImage")]
        public string FallbackImage
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Rectangle")]
    [Serializable]
    public partial class RectangleElementDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public RectangleElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Color")]
        public string Color
        {
            get;
            set;
        }

        [XmlElement("Color")]
        public AnimatedDynamicPropertyDataModel ColorProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AudioBase")]
    [Serializable]
    public partial class AudioElementDataModelBase : LayoutElementDataModelBase
    {
        public AudioElementDataModelBase()
        {
            this.Enabled = true;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get;
            set;
        }

        [XmlElement("Enabled")]
        public DynamicPropertyDataModel EnabledProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AudioOutput")]
    [Serializable]
    public partial class AudioOutputElementDataModel : Gorba.Center.Media.Core.Models.Layout.AudioElementDataModelBase
    {
        public AudioOutputElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Volume")]
        public int Volume
        {
            get;
            set;
        }

        [XmlElement("Volume")]
        public DynamicPropertyDataModel VolumeProperty
        {
            get;
            set;
        }

        [XmlAttribute("Priority")]
        public int Priority
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("PlaybackBase")]
    [Serializable]
    public partial class PlaybackElementDataModelBase : Gorba.Center.Media.Core.Models.Layout.AudioElementDataModelBase
    {
        public PlaybackElementDataModelBase()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AudioFile")]
    [Serializable]
    public partial class AudioFileElementDataModel : Gorba.Center.Media.Core.Models.Layout.PlaybackElementDataModelBase
    {
        public AudioFileElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Filename")]
        public string Filename
        {
            get;
            set;
        }

        [XmlElement("Filename")]
        public DynamicPropertyDataModel FilenameProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("AudioPause")]
    [Serializable]
    public partial class AudioPauseElementDataModel : Gorba.Center.Media.Core.Models.Layout.PlaybackElementDataModelBase
    {
        public AudioPauseElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get;
            set;
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

        partial void AdditionalInitialization();

    }

    [XmlRoot("TextToSpeech")]
    [Serializable]
    public partial class TextToSpeechElementDataModel : Gorba.Center.Media.Core.Models.Layout.PlaybackElementDataModelBase
    {
        public TextToSpeechElementDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Voice")]
        public string Voice
        {
            get;
            set;
        }

        [XmlElement("Voice")]
        public DynamicPropertyDataModel VoiceProperty
        {
            get;
            set;
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        [XmlElement("Value")]
        public DynamicPropertyDataModel ValueProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

}
namespace Gorba.Center.Media.Core.Models.Presentation
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    [XmlRoot("Infomedia")]
    [Serializable]
    public partial class InfomediaConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public InfomediaConfigDataModel()
        {
            this.PhysicalScreens = new List<Gorba.Center.Media.Core.Models.Presentation.PhysicalScreenConfigDataModel>();
            this.VirtualDisplays = new List<Gorba.Center.Media.Core.Models.Presentation.VirtualDisplayConfigDataModel>();
            this.Evaluations = new List<Gorba.Center.Media.Core.Models.Presentation.EvaluationConfigDataModel>();
            this.CyclePackages = new List<Gorba.Center.Media.Core.Models.Presentation.CyclePackageConfigDataModel>();
            this.Pools = new List<Gorba.Center.Media.Core.Models.Presentation.PoolConfigDataModel>();
            this.Layouts = new List<Gorba.Center.Media.Core.Models.Presentation.LayoutConfigDataModel>();
            this.Fonts = new List<Gorba.Center.Media.Core.Models.Presentation.FontConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlIgnore]
        public Version Version
        {
            get;
            set;
        }

        [XmlAttribute("Version")]
        public string VersionXml
        {
            get
            {
                return this.Version.ToString();
            }
            set
            {
                this.Version = new Version(value);
            }
        }

        [XmlIgnore]
        public DateTime CreationDate
        {
            get;
            set;
        }

        [XmlAttribute("Created")]
        public string CreationDateXml
        {
            get
            {
                return this.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                this.CreationDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }

        [XmlArrayItem("PhysicalScreen")]
        public List<Gorba.Center.Media.Core.Models.Presentation.PhysicalScreenConfigDataModel> PhysicalScreens
        {
            get;
            set;
        }

        [XmlArrayItem("VirtualDisplay")]
        public List<Gorba.Center.Media.Core.Models.Presentation.VirtualDisplayConfigDataModel> VirtualDisplays
        {
            get;
            set;
        }

        [XmlElement("MasterPresentation")]
        public Gorba.Center.Media.Core.Models.Presentation.MasterPresentationConfigDataModel MasterPresentation
        {
            get;
            set;
        }

        [XmlArrayItem("Evaluation")]
        public List<Gorba.Center.Media.Core.Models.Presentation.EvaluationConfigDataModel> Evaluations
        {
            get;
            set;
        }

        [XmlElement("Cycles")]
        public Gorba.Center.Media.Core.Models.Presentation.CyclesConfigDataModel Cycles
        {
            get;
            set;
        }

        [XmlArrayItem("CyclePackage")]
        public List<Gorba.Center.Media.Core.Models.Presentation.CyclePackageConfigDataModel> CyclePackages
        {
            get;
            set;
        }

        [XmlArrayItem("Pool")]
        public List<Gorba.Center.Media.Core.Models.Presentation.PoolConfigDataModel> Pools
        {
            get;
            set;
        }

        [XmlArrayItem("Layout")]
        public List<Gorba.Center.Media.Core.Models.Presentation.LayoutConfigDataModel> Layouts
        {
            get;
            set;
        }

        [XmlArrayItem("Font")]
        public List<Gorba.Center.Media.Core.Models.Presentation.FontConfigDataModel> Fonts
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("CyclePackage")]
    [Serializable]
    public partial class CyclePackageConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public CyclePackageConfigDataModel()
        {
            this.StandardCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.StandardCycleRefConfigDataModel>();
            this.EventCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleRefConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlArrayItem("StandardCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.StandardCycleRefConfigDataModel> StandardCycles
        {
            get;
            set;
        }

        [XmlArrayItem("EventCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleRefConfigDataModel> EventCycles
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Cycles")]
    [Serializable]
    public partial class CyclesConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public CyclesConfigDataModel()
        {
            this.StandardCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.StandardCycleConfigDataModel>();
            this.EventCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlArrayItem("StandardCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.StandardCycleConfigDataModel> StandardCycles
        {
            get;
            set;
        }

        [XmlArrayItem("EventCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleConfigDataModel> EventCycles
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Evaluation")]
    [Serializable]
    public partial class EvaluationConfigDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public EvaluationConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Font")]
    [Serializable]
    public partial class FontConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public FontConfigDataModel()
        {
            this.ScreenType = PhysicalScreenType.TFT;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Path")]
        public string Path
        {
            get;
            set;
        }

        [XmlAttribute("ScreenType")]
        [DefaultValue(PhysicalScreenType.TFT)]
        public PhysicalScreenType ScreenType
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("LayoutBase")]
    [Serializable]
    public partial class LayoutConfigDataModelBase : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public LayoutConfigDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Layout")]
    [Serializable]
    public partial class LayoutConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.LayoutConfigDataModelBase
    {
        public LayoutConfigDataModel()
        {
            this.Resolutions = new List<Gorba.Center.Media.Core.Models.Presentation.ResolutionConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("BaseLayoutName")]
        public string BaseLayoutName
        {
            get;
            set;
        }

        [XmlElement("Resolution")]
        public List<Gorba.Center.Media.Core.Models.Presentation.ResolutionConfigDataModel> Resolutions
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MasterLayout")]
    [Serializable]
    public partial class MasterLayoutConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.LayoutConfigDataModelBase
    {
        public MasterLayoutConfigDataModel()
        {
            this.PhysicalScreens = new List<Gorba.Center.Media.Core.Models.Presentation.PhysicalScreenRefConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlElement("PhysicalScreen")]
        public List<Gorba.Center.Media.Core.Models.Presentation.PhysicalScreenRefConfigDataModel> PhysicalScreens
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MasterPresentation")]
    [Serializable]
    public partial class MasterPresentationConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public MasterPresentationConfigDataModel()
        {
            this.MasterCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.MasterCycleConfigDataModel>();
            this.MasterEventCycles = new List<Gorba.Center.Media.Core.Models.Presentation.Cycle.MasterEventCycleConfigDataModel>();
            this.MasterLayouts = new List<Gorba.Center.Media.Core.Models.Presentation.MasterLayoutConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlArrayItem("MasterCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.MasterCycleConfigDataModel> MasterCycles
        {
            get;
            set;
        }

        [XmlArrayItem("EventCycle")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Cycle.MasterEventCycleConfigDataModel> MasterEventCycles
        {
            get;
            set;
        }

        [XmlArrayItem("MasterLayout")]
        public List<Gorba.Center.Media.Core.Models.Presentation.MasterLayoutConfigDataModel> MasterLayouts
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("PhysicalScreen")]
    [Serializable]
    public partial class PhysicalScreenConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public PhysicalScreenConfigDataModel()
        {
            this.Visible = true;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("Type")]
        public PhysicalScreenType Type
        {
            get;
            set;
        }

        [XmlAttribute("Id")]
        public string Identifier
        {
            get;
            set;
        }

        [XmlAttribute("Width")]
        public int Width
        {
            get;
            set;
        }

        [XmlAttribute("Height")]
        public int Height
        {
            get;
            set;
        }

        [XmlAttribute("Visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get;
            set;
        }

        [XmlElement("Visible")]
        public AnimatedDynamicPropertyDataModel VisibleProperty
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("PhysicalScreenRef")]
    [Serializable]
    public partial class PhysicalScreenRefConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public PhysicalScreenRefConfigDataModel()
        {
            this.VirtualDisplays = new List<Gorba.Center.Media.Core.Models.Presentation.VirtualDisplayRefConfigDataModel>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("Ref")]
        public string Reference
        {
            get;
            set;
        }

        [XmlElement("VirtualDisplay")]
        public List<Gorba.Center.Media.Core.Models.Presentation.VirtualDisplayRefConfigDataModel> VirtualDisplays
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Pool")]
    [Serializable]
    public partial class PoolConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public PoolConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("BaseDirectory")]
        public string BaseDirectory
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Resolution")]
    [Serializable]
    public partial class ResolutionConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase, IXmlSerializable
    {
        public ResolutionConfigDataModel()
        {
            this.Elements = new List<Gorba.Center.Media.Core.Models.Layout.LayoutElementDataModelBase>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("Width")]
        public int Width
        {
            get;
            set;
        }

        [XmlAttribute("Height")]
        public int Height
        {
            get;
            set;
        }

        [XmlElement("Layout.Base")]
        public List<Gorba.Center.Media.Core.Models.Layout.LayoutElementDataModelBase> Elements
        {
            get;
            set;
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

        partial void AdditionalInitialization();

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
                    var item = (Gorba.Center.Media.Core.Models.Layout.LayoutElementDataModelBase)DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Layout");
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
                DataModelSerializer.Serialize(item, writer);
            }

        }

    }

    [XmlRoot("VirtualDisplay")]
    [Serializable]
    public partial class VirtualDisplayConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public VirtualDisplayConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("CyclePackage")]
        public string CyclePackage
        {
            get;
            set;
        }

        [XmlAttribute("Width")]
        public int Width
        {
            get;
            set;
        }

        [XmlAttribute("Height")]
        public int Height
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("VirtualDisplayRef")]
    [Serializable]
    public partial class VirtualDisplayRefConfigDataModel : Gorba.Center.Media.Core.Models.Layout.DrawableElementDataModelBase
    {
        public VirtualDisplayRefConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Ref")]
        public string Reference
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

}
namespace Gorba.Center.Media.Core.Models.Presentation.Cycle
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    [XmlRoot("CycleBase")]
    [Serializable]
    public partial class CycleConfigDataModelBase : Gorba.Center.Media.Core.Models.DataModelBase, IXmlSerializable
    {
        public CycleConfigDataModelBase()
        {
            this.Enabled = true;
            this.Sections = new List<Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get;
            set;
        }

        [XmlElement("Enabled")]
        public DynamicPropertyDataModel EnabledProperty
        {
            get;
            set;
        }

        [XmlElement("Presentation.Section.SectionBase")]
        public List<Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase> Sections
        {
            get;
            set;
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
                this.EnabledProperty = DynamicPropertyDataModel.ReadFromXml(reader);
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
            DynamicPropertyDataModel.WriteToXml("Enabled", this.EnabledProperty, writer);
        }

        partial void AdditionalInitialization();

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
                    var item = (Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase)DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Presentation.Section");
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
                DataModelSerializer.Serialize(item, writer);
            }

        }

    }

    [XmlRoot("CycleRefBase")]
    [Serializable]
    public partial class CycleRefConfigDataModelBase : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public CycleRefConfigDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Ref")]
        public string Reference
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("EventCycle")]
    [Serializable]
    public partial class EventCycleConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleConfigDataModelBase
    {
        public EventCycleConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("EventCycleBase")]
    [Serializable]
    public partial class EventCycleConfigDataModelBase : Gorba.Center.Media.Core.Models.Presentation.Cycle.CycleConfigDataModelBase
    {
        public EventCycleConfigDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlElement("Trigger")]
        public Gorba.Center.Media.Core.Models.Presentation.Cycle.GenericTriggerConfigDataModel Trigger
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("EventCycleRef")]
    [Serializable]
    public partial class EventCycleRefConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.CycleRefConfigDataModelBase
    {
        public EventCycleRefConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("GenericTrigger")]
    [Serializable]
    public partial class GenericTriggerConfigDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public GenericTriggerConfigDataModel()
        {
            this.Coordinates = new List<Gorba.Center.Media.Core.Models.Eval.GenericEvalDataModel>();
            this.AdditionalInitialization();
        }

        [XmlElement("Generic")]
        public List<Gorba.Center.Media.Core.Models.Eval.GenericEvalDataModel> Coordinates
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MasterCycle")]
    [Serializable]
    public partial class MasterCycleConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.CycleConfigDataModelBase
    {
        public MasterCycleConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MasterEventCycle")]
    [Serializable]
    public partial class MasterEventCycleConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.EventCycleConfigDataModelBase
    {
        public MasterEventCycleConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("StandardCycle")]
    [Serializable]
    public partial class StandardCycleConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.CycleConfigDataModelBase
    {
        public StandardCycleConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("StandardCycleRef")]
    [Serializable]
    public partial class StandardCycleRefConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Cycle.CycleRefConfigDataModelBase
    {
        public StandardCycleRefConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

}
namespace Gorba.Center.Media.Core.Models.Presentation.Section
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    [XmlRoot("ImageSection")]
    [Serializable]
    public partial class ImageSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public ImageSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Filename")]
        public string Filename
        {
            get;
            set;
        }

        [XmlAttribute("Frame")]
        public int Frame
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MasterSection")]
    [Serializable]
    public partial class MasterSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public MasterSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MultiSection")]
    [Serializable]
    public partial class MultiSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public MultiSectionConfigDataModel()
        {
            this.MaxPages = -1;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Language")]
        public int Language
        {
            get;
            set;
        }

        [XmlAttribute("Table")]
        public int Table
        {
            get;
            set;
        }

        [XmlAttribute("RowsPerPage")]
        public int RowsPerPage
        {
            get;
            set;
        }

        [XmlAttribute("MaxPages")]
        [DefaultValue(-1)]
        public int MaxPages
        {
            get;
            set;
        }

        [XmlAttribute("Mode")]
        public PageMode Mode
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("PoolSection")]
    [Serializable]
    public partial class PoolSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public PoolSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Pool")]
        public string Pool
        {
            get;
            set;
        }

        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode
        {
            get;
            set;
        }

        [XmlAttribute("Frame")]
        public int Frame
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("SectionBase")]
    [Serializable]
    public partial class SectionConfigDataModelBase : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public SectionConfigDataModelBase()
        {
            this.Enabled = true;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get;
            set;
        }

        [XmlElement("Enabled")]
        public DynamicPropertyDataModel EnabledProperty
        {
            get;
            set;
        }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get;
            set;
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

        [XmlAttribute("Layout")]
        public string Layout
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("StandardSection")]
    [Serializable]
    public partial class StandardSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public StandardSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("VideoSection")]
    [Serializable]
    public partial class VideoSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public VideoSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("VideoUri")]
        public string VideoUri
        {
            get;
            set;
        }

        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode
        {
            get;
            set;
        }

        [XmlAttribute("Frame")]
        public int Frame
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("WebmediaSection")]
    [Serializable]
    public partial class WebmediaSectionConfigDataModel : Gorba.Center.Media.Core.Models.Presentation.Section.SectionConfigDataModelBase
    {
        public WebmediaSectionConfigDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Filename")]
        public string Filename
        {
            get;
            set;
        }

        [XmlAttribute("VideoEndMode")]
        public VideoEndMode VideoEndMode
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

}
namespace Gorba.Center.Media.Core.Models.Eval
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    [XmlRoot("And")]
    [Serializable]
    public partial class AndEvalDataModel : Gorba.Center.Media.Core.Models.Eval.CollectionEvalDataModelBase
    {
        public AndEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("CollectionBase")]
    [Serializable]
    public partial class CollectionEvalDataModelBase : EvalDataModelBase, IXmlSerializable
    {
        public CollectionEvalDataModelBase()
        {
            this.Conditions = new List<Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase>();
            this.AdditionalInitialization();
        }

        [XmlElement("Condition")]
        public List<Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase> Conditions
        {
            get;
            set;
        }

        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
        }

        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
        }

        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Conditions.Clear();
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
                    var item = (Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase)DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Eval");
                    if (item != null)
                    {
                        this.Conditions.Add(item);
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
            foreach (var item in this.Conditions)
            {
                DataModelSerializer.Serialize(item, writer);
            }

        }

    }

    [XmlRoot("Constant")]
    [Serializable]
    public partial class ConstantEvalDataModel : EvalDataModelBase
    {
        public ConstantEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("ContainerBase")]
    [Serializable]
    public partial class ContainerEvalDataModelBase : EvalDataModelBase, IXmlSerializable
    {
        public ContainerEvalDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlElement("Evaluation")]
        public Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase Evaluation
        {
            get;
            set;
        }

        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
        }

        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
        }

        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.ReadXmlAttributes(reader);
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            this.Evaluation = (Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase)DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Eval");
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);
            if (this.Evaluation != null)
            {
                DataModelSerializer.Serialize(this.Evaluation, writer);
            }

        }

    }

    [XmlRoot("CodeConversion")]
    [Serializable]
    public partial class CodeConversionEvalDataModel : EvalDataModelBase
    {
        public CodeConversionEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("FileName")]
        public string FileName
        {
            get;
            set;
        }

        [XmlAttribute("UseImage")]
        public bool UseImage
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("CsvMapping")]
    [Serializable]
    public partial class CsvMappingEvalDataModel : EvalDataModelBase
    {
        public CsvMappingEvalDataModel()
        {
            this.Matches = new List<Gorba.Center.Media.Core.Models.Eval.MatchDynamicPropertyDataModel>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("FileName")]
        public string FileName
        {
            get;
            set;
        }

        [XmlAttribute("OutputFormat")]
        public string OutputFormat
        {
            get;
            set;
        }

        [XmlElement("DefaultValue")]
        public DynamicPropertyDataModel DefaultValue
        {
            get;
            set;
        }

        [XmlElement("Match")]
        public List<Gorba.Center.Media.Core.Models.Eval.MatchDynamicPropertyDataModel> Matches
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("MatchDynamicProperty")]
    [Serializable]
    public partial class MatchDynamicPropertyDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public MatchDynamicPropertyDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Column")]
        public int Column
        {
            get;
            set;
        }

        public DynamicPropertyDataModel Evaluation
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Date")]
    [Serializable]
    public partial class DateEvalDataModel : Gorba.Center.Media.Core.Models.Eval.DateTimeEvalDataModelBase
    {
        public DateEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlIgnore]
        public DateTime Begin
        {
            get;
            set;
        }

        [XmlAttribute("Begin")]
        public string BeginXml
        {
            get
            {
                return this.Begin.ToString("yyyy-MM-dd");
            }
            set
            {
                this.Begin = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        [XmlIgnore]
        public DateTime End
        {
            get;
            set;
        }

        [XmlAttribute("End")]
        public string EndXml
        {
            get
            {
                return this.End.ToString("yyyy-MM-dd");
            }
            set
            {
                this.End = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("DateTimeBase")]
    [Serializable]
    public partial class DateTimeEvalDataModelBase : EvalDataModelBase
    {
        public DateTimeEvalDataModelBase()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("DayOfWeek")]
    [Serializable]
    public partial class DayOfWeekEvalDataModel : Gorba.Center.Media.Core.Models.Eval.DateTimeEvalDataModelBase
    {
        public DayOfWeekEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Mon")]
        public bool Monday
        {
            get;
            set;
        }

        [XmlAttribute("Tue")]
        public bool Tuesday
        {
            get;
            set;
        }

        [XmlAttribute("Wed")]
        public bool Wednesday
        {
            get;
            set;
        }

        [XmlAttribute("Thu")]
        public bool Thursday
        {
            get;
            set;
        }

        [XmlAttribute("Fri")]
        public bool Friday
        {
            get;
            set;
        }

        [XmlAttribute("Sat")]
        public bool Saturday
        {
            get;
            set;
        }

        [XmlAttribute("Sun")]
        public bool Sunday
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Evaluation")]
    [Serializable]
    public partial class EvaluationEvalDataModel : EvalDataModelBase
    {
        public EvaluationEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Ref")]
        public string Reference
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Format")]
    [Serializable]
    public partial class FormatEvalDataModel : EvalDataModelBase, IXmlSerializable
    {
        public FormatEvalDataModel()
        {
            this.Arguments = new List<Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase>();
            this.AdditionalInitialization();
        }

        [XmlAttribute("Format")]
        public string Format
        {
            get;
            set;
        }

        [XmlElement("Argument")]
        public List<Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase> Arguments
        {
            get;
            set;
        }

        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
            var attrFormat = reader.GetAttribute("Format");
            if (attrFormat != null)
            {
                this.Format = attrFormat;
            }

        }

        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Format", this.Format);
        }

        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Arguments.Clear();
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
                    var item = (Gorba.Center.Media.Core.Models.Eval.EvalDataModelBase)DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Eval");
                    if (item != null)
                    {
                        this.Arguments.Add(item);
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
            foreach (var item in this.Arguments)
            {
                DataModelSerializer.Serialize(item, writer);
            }

        }

    }

    [XmlRoot("Generic")]
    [Serializable]
    public partial class GenericEvalDataModel : EvalDataModelBase
    {
        public GenericEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Lang")]
        public int Language
        {
            get;
            set;
        }

        [XmlAttribute("Table")]
        public int Table
        {
            get;
            set;
        }

        [XmlAttribute("Column")]
        public int Column
        {
            get;
            set;
        }

        [XmlAttribute("Row")]
        public int Row
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("If")]
    [Serializable]
    public partial class IfEvalDataModel : EvalDataModelBase
    {
        public IfEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlElement("Condition")]
        public DynamicPropertyDataModel Condition
        {
            get;
            set;
        }

        [XmlElement("Then")]
        public DynamicPropertyDataModel Then
        {
            get;
            set;
        }

        [XmlElement("Else")]
        public DynamicPropertyDataModel Else
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("IntegerCompare")]
    [Serializable]
    public partial class IntegerCompareEvalDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public IntegerCompareEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Begin")]
        public int Begin
        {
            get;
            set;
        }

        [XmlAttribute("End")]
        public int End
        {
            get;
            set;
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrBegin = reader.GetAttribute("Begin");
            if (attrBegin != null)
            {
                this.Begin = XmlConvert.ToInt32(attrBegin);
            }

            var attrEnd = reader.GetAttribute("End");
            if (attrEnd != null)
            {
                this.End = XmlConvert.ToInt32(attrEnd);
            }

            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Begin", XmlConvert.ToString(this.Begin));
            writer.WriteAttributeString("End", XmlConvert.ToString(this.End));
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Not")]
    [Serializable]
    public partial class NotEvalDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public NotEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Or")]
    [Serializable]
    public partial class OrEvalDataModel : Gorba.Center.Media.Core.Models.Eval.CollectionEvalDataModelBase
    {
        public OrEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("RegexReplace")]
    [Serializable]
    public partial class RegexReplaceEvalDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public RegexReplaceEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Pattern")]
        public string Pattern
        {
            get;
            set;
        }

        [XmlAttribute("Replacement")]
        public string Replacement
        {
            get;
            set;
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrPattern = reader.GetAttribute("Pattern");
            if (attrPattern != null)
            {
                this.Pattern = attrPattern;
            }

            var attrReplacement = reader.GetAttribute("Replacement");
            if (attrReplacement != null)
            {
                this.Replacement = attrReplacement;
            }

            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Pattern", this.Pattern);
            writer.WriteAttributeString("Replacement", this.Replacement);
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("StringCompare")]
    [Serializable]
    public partial class StringCompareEvalDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public StringCompareEvalDataModel()
        {
            this.IgnoreCase = false;
            this.AdditionalInitialization();
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        [XmlAttribute("IgnoreCase")]
        [DefaultValue(false)]
        public bool IgnoreCase
        {
            get;
            set;
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrValue = reader.GetAttribute("Value");
            if (attrValue != null)
            {
                this.Value = attrValue;
            }

            var attrIgnoreCase = reader.GetAttribute("IgnoreCase");
            if (attrIgnoreCase != null)
            {
                this.IgnoreCase = XmlConvert.ToBoolean(attrIgnoreCase);
            }

            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Value", this.Value);
            if (this.IgnoreCase != false)
            {
                writer.WriteAttributeString("IgnoreCase", XmlConvert.ToString(this.IgnoreCase));
            }

            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Switch")]
    [Serializable]
    public partial class SwitchEvalDataModel : EvalDataModelBase
    {
        public SwitchEvalDataModel()
        {
            this.Cases = new List<Gorba.Center.Media.Core.Models.Eval.CaseDynamicPropertyDataModel>();
            this.AdditionalInitialization();
        }

        [XmlElement("Value")]
        public DynamicPropertyDataModel Value
        {
            get;
            set;
        }

        [XmlElement("Case")]
        public List<Gorba.Center.Media.Core.Models.Eval.CaseDynamicPropertyDataModel> Cases
        {
            get;
            set;
        }

        [XmlElement("Default")]
        public DynamicPropertyDataModel Default
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("CaseDynamicProperty")]
    [Serializable]
    public partial class CaseDynamicPropertyDataModel : Gorba.Center.Media.Core.Models.DataModelBase
    {
        public CaseDynamicPropertyDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        public DynamicPropertyDataModel Evaluation
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("TextToImage")]
    [Serializable]
    public partial class TextToImageEvalDataModel : Gorba.Center.Media.Core.Models.Eval.ContainerEvalDataModelBase
    {
        public TextToImageEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlAttribute("FilePatterns")]
        public string FilePatterns
        {
            get;
            set;
        }

        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrFilePatterns = reader.GetAttribute("FilePatterns");
            if (attrFilePatterns != null)
            {
                this.FilePatterns = attrFilePatterns;
            }

            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

        }

        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("FilePatterns", this.FilePatterns);
            writer.WriteAttributeString("DisplayText", this.DisplayText);
        }

        protected override void WriteXmlElements(XmlWriter writer)
        {
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Time")]
    [Serializable]
    public partial class TimeEvalDataModel : Gorba.Center.Media.Core.Models.Eval.DateTimeEvalDataModelBase
    {
        public TimeEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        [XmlIgnore]
        public TimeSpan Begin
        {
            get;
            set;
        }

        [XmlAttribute("Begin")]
        public string BeginXml
        {
            get
            {
                return new DateTime(2000, 1, 1).Add(this.Begin).ToString("HH:mm:ss");
            }
            set
            {
                this.Begin = DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
            }
        }

        [XmlIgnore]
        public TimeSpan End
        {
            get;
            set;
        }

        [XmlAttribute("End")]
        public string EndXml
        {
            get
            {
                return new DateTime(2000, 1, 1).Add(this.End).ToString("HH:mm:ss");
            }
            set
            {
                this.End = DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
            }
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("BinaryOperatorBase")]
    [Serializable]
    public partial class BinaryOperatorEvalDataModelBase : EvalDataModelBase
    {
        public BinaryOperatorEvalDataModelBase()
        {
            this.AdditionalInitialization();
        }

        [XmlElement("Left")]
        public DynamicPropertyDataModel Left
        {
            get;
            set;
        }

        [XmlElement("Right")]
        public DynamicPropertyDataModel Right
        {
            get;
            set;
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("Equals")]
    [Serializable]
    public partial class EqualsEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public EqualsEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("NotEquals")]
    [Serializable]
    public partial class NotEqualsEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public NotEqualsEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("GreaterThan")]
    [Serializable]
    public partial class GreaterThanEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public GreaterThanEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("GreaterThanOrEqual")]
    [Serializable]
    public partial class GreaterThanOrEqualEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public GreaterThanOrEqualEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("LessThan")]
    [Serializable]
    public partial class LessThanEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public LessThanEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

    [XmlRoot("LessThanOrEqual")]
    [Serializable]
    public partial class LessThanOrEqualEvalDataModel : Gorba.Center.Media.Core.Models.Eval.BinaryOperatorEvalDataModelBase
    {
        public LessThanOrEqualEvalDataModel()
        {
            this.AdditionalInitialization();
        }

        partial void AdditionalInitialization();

    }

}
