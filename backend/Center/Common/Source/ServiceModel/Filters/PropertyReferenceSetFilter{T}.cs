namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PropertyReferenceSetFilter<T>
    {
        [DataMember]
        public ICollection<T> Values { get; set; }
    }
}