// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagValuesInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FlagValuesInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Values info message object for an <see cref="FlagValues"/>.
    /// </summary>
    public class FlagValuesInfo : ValuesInfoBase
    {
        /// <summary>
        /// Converts this <see cref="ValuesInfoBase"/> to a <see cref="FlagValues"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="FlagValues"/> representing this object.
        /// </returns>
        public override ValuesBase ToValues()
        {
            return new FlagValues();
        }
    }
}