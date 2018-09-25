// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Messages
{
    using System;

    /// <summary>
    /// Simple message containing only a string.
    /// </summary>
    [Serializable]
    public class StringMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMessage"/> class.
        /// </summary>
        public StringMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMessage"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public StringMessage(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = (StringMessage)obj;

            return this.Value == other.Value;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("StringMessage[{0}]", this.Value);
        }
    }
}
