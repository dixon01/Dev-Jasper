// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertiesMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Messages
{
    /// <summary>
    /// Test message that contains different properties of primitive and
    /// complex type.
    /// </summary>
    public class PropertiesMessage
    {
        /// <summary>
        /// Gets or sets a String.
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// Gets or sets a SimpleMessage.
        /// </summary>
        public SimpleMessage Message { get; set; }

        /// <summary>
        /// Gets or sets a long value.
        /// </summary>
        public long LongValue { get; set; }

        /// <summary>
        /// Gets or sets an int value.
        /// </summary>
        public int IntValue { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PropertiesMessage)obj;
            return Equals(this.String, other.String)
                && Equals(this.Message, other.Message)
                && this.LongValue == other.LongValue
                && this.IntValue == other.IntValue;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.IntValue;
        }
    }
}
