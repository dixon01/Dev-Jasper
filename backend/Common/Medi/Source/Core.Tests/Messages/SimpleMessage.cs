// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Messages
{
    using System;

    /// <summary>
    /// Simple message containing only an integer.
    /// </summary>
    [Serializable]
    public class SimpleMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMessage"/> class.
        /// </summary>
        public SimpleMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMessage"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public SimpleMessage(int value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value { get; set; }

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

            var other = (SimpleMessage)obj;

            return this.Value == other.Value;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Value;
        }
    }
}
