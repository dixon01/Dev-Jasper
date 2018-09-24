// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TreeMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The tree message.
    /// </summary>
    [Serializable]
    public class TreeMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeMessage"/> class.
        /// </summary>
        public TreeMessage()
        {
            this.Children = new List<TreeMessage>();
            this.Strings = new List<StringMessage>();
        }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public List<TreeMessage> Children { get; set; }

        /// <summary>
        /// Gets or sets the list message.
        /// </summary>
        public ListMessage List { get; set; }

        /// <summary>
        /// Gets or sets the string messages.
        /// </summary>
        public List<StringMessage> Strings { get; set; }

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

            var other = (TreeMessage)obj;

            if ((this.Children == null) != (other.Children == null))
            {
                return false;
            }

            if (this.Children != null && other.Children != null && !this.Children.SequenceEqual(other.Children))
            {
                return false;
            }

            if (this.Strings != null && other.Strings != null && !this.Strings.SequenceEqual(other.Strings))
            {
                return false;
            }

            return object.Equals(this.List, other.List);
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
            return this.Children.Count;
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TreeMessage[ChildCount={0}]", this.Children.Count);
        }
    }
}
