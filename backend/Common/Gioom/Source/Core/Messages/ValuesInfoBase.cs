// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValuesInfoBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValuesInfoBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Values info message base class.
    /// </summary>
    [XmlInclude(typeof(FlagValuesInfo))]
    [XmlInclude(typeof(IntegerValuesInfo))]
    [XmlInclude(typeof(EnumValuesInfo))]
    [XmlInclude(typeof(EnumFlagValuesInfo))]
    public abstract class ValuesInfoBase
    {
        /// <summary>
        /// Converts the given <see cref="ValuesBase"/> to a <see cref="ValuesInfoBase"/>.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="ValuesInfoBase"/> representing the given object.
        /// </returns>
        public static ValuesInfoBase From(ValuesBase values)
        {
            var flag = values as FlagValues;
            if (flag != null)
            {
                return new FlagValuesInfo();
            }

            var integer = values as IntegerValues;
            if (integer != null)
            {
                return new IntegerValuesInfo(integer);
            }

            // IMPORTANT: this has to come before EnumValues because it's a subclass
            var enumFlag = values as EnumFlagValues;
            if (enumFlag != null)
            {
                return new EnumFlagValuesInfo(enumFlag);
            }

            var enumeration = values as EnumValues;
            if (enumeration != null)
            {
                return new EnumValuesInfo(enumeration);
            }

            throw new NotSupportedException("Can't convert " + values.GetType().Name);
        }

        /// <summary>
        /// Converts this <see cref="ValuesInfoBase"/> to a <see cref="ValuesBase"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ValuesBase"/> representing this object.
        /// </returns>
        public abstract ValuesBase ToValues();
    }
}