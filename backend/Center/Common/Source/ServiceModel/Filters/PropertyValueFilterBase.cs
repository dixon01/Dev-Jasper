namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(BooleanPropertyValueFilter))]
    [KnownType(typeof(DateTimePropertyValueFilter))]
    [KnownType(typeof(GuidPropertyValueFilter))]
    [KnownType(typeof(Int32PropertyValueFilter))]
    [KnownType(typeof(NullableDateTimePropertyValueFilter))]
    [KnownType(typeof(StringPropertyValueFilter))]
    public abstract class PropertyValueFilterBase : PropertyFilterBase
    {
    }
}