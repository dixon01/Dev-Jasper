namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the comparison operator for Enum.
    /// </summary>
    [DataContract]
    public enum EnumComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        GreaterThan = 2,

        [EnumMember]
        GreaterThanOrEqualTo = 3,

        [EnumMember]
        LessThan = 4,

        [EnumMember]
        LessThanOrEqualTo = 5
    }

    /// <summary>
    /// Defines the comparison operator for bool.
    /// </summary>
    [DataContract]
    public enum BooleanComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1
    }

    /// <summary>
    /// Defines the comparison operator for DateTime.
    /// </summary>
    [DataContract]
    public enum DateTimeComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        GreaterThan = 2,

        [EnumMember]
        GreaterThanOrEqualTo = 3,

        [EnumMember]
        LessThan = 4,

        [EnumMember]
        LessThanOrEqualTo = 5
    }

    /// <summary>
    /// Defines the comparison operator for Guid.
    /// </summary>
    [DataContract]
    public enum GuidComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1
    }

    /// <summary>
    /// Defines the comparison operator for int.
    /// </summary>
    [DataContract]
    public enum Int32Comparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        GreaterThan = 2,

        [EnumMember]
        GreaterThanOrEqualTo = 3,

        [EnumMember]
        LessThan = 4,

        [EnumMember]
        LessThanOrEqualTo = 5
    }

    /// <summary>
    /// Defines the comparison operator for long.
    /// </summary>
    [DataContract]
    public enum Int64Comparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        GreaterThan = 2,

        [EnumMember]
        GreaterThanOrEqualTo = 3,

        [EnumMember]
        LessThan = 4,

        [EnumMember]
        LessThanOrEqualTo = 5
    }

    /// <summary>
    /// Defines the comparison operator for DateTime?.
    /// </summary>
    [DataContract]
    public enum NullableDateTimeComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        GreaterThan = 2,

        [EnumMember]
        GreaterThanOrEqualTo = 3,

        [EnumMember]
        LessThan = 4,

        [EnumMember]
        LessThanOrEqualTo = 5
    }

    /// <summary>
    /// Defines the comparison operator for string.
    /// </summary>
    [DataContract]
    public enum StringComparison
    {
        /// <summary>
        /// The values are exactly the same.
        /// </summary>
        [EnumMember]
        ExactMatch = 0,

        /// <summary>
        /// The values are different.
        /// </summary>
        [EnumMember]
        Different = 1,

        [EnumMember]
        CaseInsensitiveMatch = 2
    }
}
