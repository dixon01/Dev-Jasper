// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Message containing a list and an array.
    /// This can be used for testing serialization.
    /// </summary>
    [Serializable]
    public class ListMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListMessage"/> class.
        /// </summary>
        public ListMessage()
        {
            this.Integers = new List<int>();
        }

        /// <summary>
        /// Gets or sets a list of integers.
        /// </summary>
        public List<int> Integers { get; set; }

        /// <summary>
        /// Gets or sets an array of simple messages.
        /// </summary>
        public SimpleMessage[] Messages { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            var other = obj as ListMessage;
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            if ((this.Integers == null) != (other.Integers == null))
            {
                return false;
            }

            if (this.Integers != null && other.Integers != null && !this.Integers.SequenceEqual(other.Integers))
            {
                return false;
            }

            if ((this.Messages == null) != (other.Messages == null))
            {
                return false;
            }

            if (this.Messages != null && other.Messages != null && !this.Messages.SequenceEqual(other.Messages))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.Messages == null ? 0 : this.Messages.Length) ^
                   (this.Integers == null ? 0 : -this.Integers.Count);
        }
    }
}
