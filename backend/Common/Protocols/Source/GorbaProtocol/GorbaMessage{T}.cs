// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaMessage{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.GorbaProtocol
{
    using System;

    /// <summary>
    /// A live update message with a typed content.
    /// </summary>
    /// <typeparam name="T">The type of the content.</typeparam>
    [Serializable]
    public class GorbaMessage<T> : GorbaMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaMessage{T}"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public GorbaMessage(T content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaMessage{T}"/> class.
        /// </summary>
        public GorbaMessage()
        {
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("GorbaMessage<{0}> {1}", typeof(T).Name, this.Id);
        }
    }
}