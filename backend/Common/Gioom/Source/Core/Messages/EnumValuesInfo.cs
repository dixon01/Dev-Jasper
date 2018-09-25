// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValuesInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumValuesInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Values info message object for an <see cref="EnumValues"/>.
    /// </summary>
    public class EnumValuesInfo : ValuesInfoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValuesInfo"/> class.
        /// </summary>
        public EnumValuesInfo()
        {
            this.Values = new List<Pair>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValuesInfo"/> class.
        /// </summary>
        /// <param name="enumeration">
        /// The enumeration.
        /// </param>
        public EnumValuesInfo(EnumValues enumeration)
            : this()
        {
            foreach (var value in enumeration.Values)
            {
                this.Values.Add(new Pair { Name = value.Name, Value = value.Value });
            }
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public List<Pair> Values { get; set; }

        /// <summary>
        /// Converts this <see cref="ValuesInfoBase"/> to a <see cref="EnumValues"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="EnumValues"/> representing this object.
        /// </returns>
        public override ValuesBase ToValues()
        {
            return new EnumValues(this.GetValues());
        }

        /// <summary>
        /// Gets the valid values from the <see cref="Values"/> in this object.
        /// </summary>
        /// <returns>
        /// A dictionary of all valid values.
        /// </returns>
        protected IDictionary<int, string> GetValues()
        {
            var values = new Dictionary<int, string>(this.Values.Count);
            foreach (var pair in this.Values)
            {
                values.Add(pair.Value, pair.Name);
            }

            return values;
        }

        /// <summary>
        /// This class is not supposed to be used outside this assembly.
        /// It is only public to allow XML serialization.
        /// Pair of value and name for <see cref="EnumValuesInfo"/>.
        /// </summary>
        public class Pair
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
        }
    }
}