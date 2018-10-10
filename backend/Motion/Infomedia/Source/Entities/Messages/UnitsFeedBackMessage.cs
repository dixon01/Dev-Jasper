// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitsFeedBackMessage.cs" company="Luminator LTG">
//   Copyright © 2011-2017 Luminator. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    /// <summary>The unit's feed back message used to broadcast a message for a given unit.</summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class UnitsFeedBackMessage<T>
        where T : class
    {
        /// <summary>Initializes a new instance of the <see cref="UnitsFeedBackMessage{T}"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="unitName">The unit name.</param>
        public UnitsFeedBackMessage(T message, string unitName = "")
            : this()
        {
            this.Message = message;
            this.UnitName = unitName;
        }

        /// <summary>Initializes a new instance of the <see cref="UnitsFeedBackMessage{T}"/> class.</summary>
        public UnitsFeedBackMessage()
        {
            this.Created = DateTime.Now;
            this.UnitName = string.Empty;
        }

        /// <summary>Gets or sets the created.</summary>
        public DateTime Created { get; set; }

        /// <summary>Gets or sets the message entity.</summary>
        public T Message { get; set; }

        /// <summary>Gets or sets the unit name.</summary>
        public string UnitName { get; set; } = string.Empty;
    }
}