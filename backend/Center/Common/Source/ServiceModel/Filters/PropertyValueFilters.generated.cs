namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public partial class EnumPropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public EnumComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public Enum Value { get; set; }
    }

    [DataContract]
    public partial class BooleanPropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public BooleanComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public bool Value { get; set; }
    }

    [DataContract]
    public partial class DateTimePropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public DateTimeComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public DateTime Value { get; set; }
    }

    [DataContract]
    public partial class GuidPropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public GuidComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public Guid Value { get; set; }
    }

    [DataContract]
    public partial class Int32PropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public Int32Comparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public int Value { get; set; }
    }

    [DataContract]
    public partial class Int64PropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public Int64Comparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public long Value { get; set; }
    }

    [DataContract]
    public partial class NullableDateTimePropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public NullableDateTimeComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract]
    public partial class StringPropertyValueFilter : PropertyValueFilterBase
    {
        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        [DataMember]
        public StringComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether value.
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
