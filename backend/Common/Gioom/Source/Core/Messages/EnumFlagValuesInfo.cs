// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlagValuesInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumFlagValuesInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Values info message object for an <see cref="EnumFlagValues"/>.
    /// </summary>
    public class EnumFlagValuesInfo : EnumValuesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFlagValuesInfo"/> class.
        /// </summary>
        public EnumFlagValuesInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFlagValuesInfo"/> class.
        /// </summary>
        /// <param name="enumFlag">
        /// The enum flag.
        /// </param>
        public EnumFlagValuesInfo(EnumFlagValues enumFlag)
            : base(enumFlag)
        {
        }

        /// <summary>
        /// Converts this <see cref="ValuesInfoBase"/> to a <see cref="EnumFlagValues"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="EnumFlagValues"/> representing this object.
        /// </returns>
        public override ValuesBase ToValues()
        {
            return new EnumFlagValues(this.GetValues());
        }
    }
}