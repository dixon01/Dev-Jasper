// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerValuesInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerValuesInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Values info message object for an <see cref="IntegerValues"/>.
    /// </summary>
    public class IntegerValuesInfo : ValuesInfoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerValuesInfo"/> class.
        /// </summary>
        public IntegerValuesInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerValuesInfo"/> class.
        /// </summary>
        /// <param name="integer">
        /// The integer.
        /// </param>
        public IntegerValuesInfo(IntegerValues integer)
        {
            this.MinValue = integer.MinValue;
            this.MaxValue = integer.MaxValue;
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Converts this <see cref="ValuesInfoBase"/> to a <see cref="IntegerValues"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IntegerValues"/> representing this object.
        /// </returns>
        public override ValuesBase ToValues()
        {
            return new IntegerValues(this.MinValue, this.MaxValue);
        }
    }
}