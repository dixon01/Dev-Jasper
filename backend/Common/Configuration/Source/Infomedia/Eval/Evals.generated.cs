

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Common;

namespace Gorba.Common.Configuration.Infomedia.Eval
{    
    [XmlRoot("And")]
    [Serializable]
    public partial class AndEval : CollectionEvalBase
    {
        public AndEval()
        {
            this.Initialize();
        }

        partial void Initialize();
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Base")]
    [Serializable]
    public partial class EvalBase
    {
        public EvalBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("CollectionBase")]
    [Serializable]
    public partial class CollectionEvalBase : EvalBase, IXmlSerializable
    {
        public CollectionEvalBase()
        {   
            this.Conditions = new List<EvalBase>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Condition")]
        public List<EvalBase> Conditions { get; set; }
        
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
            
            while (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Text)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    var item = EvalSerializer.Deserialize(reader);
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
                EvalSerializer.Serialize(item, writer);
            }
            
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
        
    }
    
    [XmlRoot("Constant")]
    [Serializable]
    public partial class ConstantEval : EvalBase
    {
        public ConstantEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Value")]
        public string Value { get; set; }
    
    }
    
    [XmlRoot("ContainerBase")]
    [Serializable]
    public partial class ContainerEvalBase : EvalBase, IXmlSerializable
    {
        public ContainerEvalBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Evaluation")]
        public EvalBase Evaluation { get; set; }
        
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
            
            this.Evaluation = EvalSerializer.Deserialize(reader);
            
            reader.ReadEndElement();
        }
        
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);
            
            EvalSerializer.Serialize(this.Evaluation, writer);
            
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
        
    }
    
    [XmlRoot("CodeConversion")]
    [Serializable]
    public partial class CodeConversionEval : EvalBase
    {
        public CodeConversionEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("FileName")]
        public string FileName { get; set; }
    
        [XmlAttribute("UseImage")]
        public bool UseImage { get; set; }
    
    }
    
    [XmlRoot("CsvMapping")]
    [Serializable]
    public partial class CsvMappingEval : EvalBase
    {
        public CsvMappingEval()
        {   
            this.Matches = new List<MatchDynamicProperty>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("FileName")]
        public string FileName { get; set; }
    
        [XmlAttribute("OutputFormat")]
        public string OutputFormat { get; set; }
    
        [XmlElement("DefaultValue")]
        public DynamicProperty DefaultValue { get; set; }
        
        [XmlElement("Match")]
        public List<MatchDynamicProperty> Matches { get; set; }
        
    }
    
    [XmlRoot("MatchDynamicProperty")]
    [Serializable]
    public partial class MatchDynamicProperty : DynamicProperty
    {
        public MatchDynamicProperty()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Column")]
        public int Column { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
            var attrColumn = reader.GetAttribute("Column");
            if (attrColumn != null)
            {
                this.Column = XmlConvert.ToInt32(attrColumn);
            }
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("Column", XmlConvert.ToString(this.Column));

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Date")]
    [Serializable]
    public partial class DateEval : DateTimeEvalBase
    {
        public DateEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlIgnore]
        public DateTime Begin { get; set; }
        
        [XmlAttribute("Begin")]
        public string BeginXml { get { return this.Begin.ToString("yyyy-MM-dd"); } set { this.Begin = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture); } }
    
        [XmlIgnore]
        public DateTime End { get; set; }
        
        [XmlAttribute("End")]
        public string EndXml { get { return this.End.ToString("yyyy-MM-dd"); } set { this.End = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture); } }
    
    }
    
    [XmlRoot("DateTimeBase")]
    [Serializable]
    public partial class DateTimeEvalBase : EvalBase
    {
        public DateTimeEvalBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("DayOfWeek")]
    [Serializable]
    public partial class DayOfWeekEval : DateTimeEvalBase
    {
        public DayOfWeekEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Mon")]
        public bool Monday { get; set; }
    
        [XmlAttribute("Tue")]
        public bool Tuesday { get; set; }
    
        [XmlAttribute("Wed")]
        public bool Wednesday { get; set; }
    
        [XmlAttribute("Thu")]
        public bool Thursday { get; set; }
    
        [XmlAttribute("Fri")]
        public bool Friday { get; set; }
    
        [XmlAttribute("Sat")]
        public bool Saturday { get; set; }
    
        [XmlAttribute("Sun")]
        public bool Sunday { get; set; }
    
    }
    
    [XmlRoot("Evaluation")]
    [Serializable]
    public partial class EvaluationEval : EvalBase
    {
        public EvaluationEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Ref")]
        public string Reference { get; set; }
        
    }
    
    [XmlRoot("Format")]
    [Serializable]
    public partial class FormatEval : EvalBase, IXmlSerializable
    {
        public FormatEval()
        {   
            this.Arguments = new List<EvalBase>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Format")]
        public string Format { get; set; }
    
        [XmlElement("Argument")]
        public List<EvalBase> Arguments { get; set; }
        
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
            
            while (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Text)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    var item = EvalSerializer.Deserialize(reader);
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
                EvalSerializer.Serialize(item, writer);
            }
            
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
        
    }
    
    [XmlRoot("Generic")]
    [Serializable]
    public partial class GenericEval : EvalBase
    {
        public GenericEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Lang")]
        public int Language { get; set; }
    
        [XmlAttribute("Table")]
        public int Table { get; set; }
    
        [XmlAttribute("Column")]
        public int Column { get; set; }
    
        [XmlAttribute("Row")]
        public int Row { get; set; }
    
    }
    
    [XmlRoot("If")]
    [Serializable]
    public partial class IfEval : EvalBase
    {
        public IfEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Condition")]
        public DynamicProperty Condition { get; set; }
        
        [XmlElement("Then")]
        public DynamicProperty Then { get; set; }
        
        [XmlElement("Else")]
        public DynamicProperty Else { get; set; }
        
    }
    
    [XmlRoot("IntegerCompare")]
    [Serializable]
    public partial class IntegerCompareEval : ContainerEvalBase
    {
        public IntegerCompareEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Begin")]
        public int Begin { get; set; }
    
        [XmlAttribute("End")]
        public int End { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
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
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("Begin", XmlConvert.ToString(this.Begin));

            writer.WriteAttributeString("End", XmlConvert.ToString(this.End));

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Not")]
    [Serializable]
    public partial class NotEval : ContainerEvalBase
    {
        public NotEval()
        {
            this.Initialize();
        }

        partial void Initialize();
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Or")]
    [Serializable]
    public partial class OrEval : CollectionEvalBase
    {
        public OrEval()
        {
            this.Initialize();
        }

        partial void Initialize();
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("RegexReplace")]
    [Serializable]
    public partial class RegexReplaceEval : ContainerEvalBase
    {
        public RegexReplaceEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Pattern")]
        public string Pattern { get; set; }
    
        [XmlAttribute("Replacement")]
        public string Replacement { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
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
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("Pattern", this.Pattern);

            writer.WriteAttributeString("Replacement", this.Replacement);

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("StringCompare")]
    [Serializable]
    public partial class StringCompareEval : ContainerEvalBase
    {
        public StringCompareEval()
        {   
            this.IgnoreCase = false;
    
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Value")]
        public string Value { get; set; }
    
        [XmlAttribute("IgnoreCase")]
        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
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
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("Value", this.Value);

            if (this.IgnoreCase != false)
            {
                writer.WriteAttributeString("IgnoreCase", XmlConvert.ToString(this.IgnoreCase));
            }

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Switch")]
    [Serializable]
    public partial class SwitchEval : EvalBase
    {
        public SwitchEval()
        {   
            this.Cases = new List<CaseDynamicProperty>();
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Value")]
        public DynamicProperty Value { get; set; }
        
        [XmlElement("Case")]
        public List<CaseDynamicProperty> Cases { get; set; }
        
        [XmlElement("Default")]
        public DynamicProperty Default { get; set; }
        
    }
    
    [XmlRoot("CaseDynamicProperty")]
    [Serializable]
    public partial class CaseDynamicProperty : DynamicProperty
    {
        public CaseDynamicProperty()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("Value")]
        public string Value { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
            var attrValue = reader.GetAttribute("Value");
            if (attrValue != null)
            {
                this.Value = attrValue;
            }
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("Value", this.Value);

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("TextToImage")]
    [Serializable]
    public partial class TextToImageEval : ContainerEvalBase
    {
        public TextToImageEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlAttribute("FilePatterns")]
        public string FilePatterns { get; set; }
            
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            
            var attrFilePatterns = reader.GetAttribute("FilePatterns");
            if (attrFilePatterns != null)
            {
                this.FilePatterns = attrFilePatterns;
            }
        
        }
        
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return base.ReadXmlElement(elementName, reader);
            
        }
        
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            
            writer.WriteAttributeString("FilePatterns", this.FilePatterns);

        }
        
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);
            
        }
        
    }
    
    [XmlRoot("Time")]
    [Serializable]
    public partial class TimeEval : DateTimeEvalBase
    {
        public TimeEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlIgnore]
        public TimeSpan Begin { get; set; }
        
        [XmlAttribute("Begin")]
        public string BeginXml { get { return new DateTime(2000, 1, 1).Add(this.Begin).ToString("HH:mm:ss"); } set { this.Begin = DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay; } }
    
        [XmlIgnore]
        public TimeSpan End { get; set; }
        
        [XmlAttribute("End")]
        public string EndXml { get { return new DateTime(2000, 1, 1).Add(this.End).ToString("HH:mm:ss"); } set { this.End = DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay; } }
    
    }
    
    [XmlRoot("BinaryOperatorBase")]
    [Serializable]
    public partial class BinaryOperatorEvalBase : EvalBase
    {
        public BinaryOperatorEvalBase()
        {
            this.Initialize();
        }

        partial void Initialize();
    
        [XmlElement("Left")]
        public DynamicProperty Left { get; set; }
        
        [XmlElement("Right")]
        public DynamicProperty Right { get; set; }
        
    }
    
    [XmlRoot("Equals")]
    [Serializable]
    public partial class EqualsEval : BinaryOperatorEvalBase
    {
        public EqualsEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("NotEquals")]
    [Serializable]
    public partial class NotEqualsEval : BinaryOperatorEvalBase
    {
        public NotEqualsEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("GreaterThan")]
    [Serializable]
    public partial class GreaterThanEval : BinaryOperatorEvalBase
    {
        public GreaterThanEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("GreaterThanOrEqual")]
    [Serializable]
    public partial class GreaterThanOrEqualEval : BinaryOperatorEvalBase
    {
        public GreaterThanOrEqualEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("LessThan")]
    [Serializable]
    public partial class LessThanEval : BinaryOperatorEvalBase
    {
        public LessThanEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
    
    [XmlRoot("LessThanOrEqual")]
    [Serializable]
    public partial class LessThanOrEqualEval : BinaryOperatorEvalBase
    {
        public LessThanOrEqualEval()
        {
            this.Initialize();
        }

        partial void Initialize();
    
    }
}

