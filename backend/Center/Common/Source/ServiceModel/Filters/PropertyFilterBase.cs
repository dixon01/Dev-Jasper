namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(PropertyValueFilterBase))]
    public abstract class PropertyFilterBase
    {
    }
}