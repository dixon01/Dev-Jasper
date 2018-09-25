// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementId.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System;
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// This class provide a unique ID for instances.
    /// </summary>
    [Serializable]
    public abstract class ElementId
    {
        private static int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementId"/> class.
        /// </summary>
        protected ElementId()
        {
            this.Id = Interlocked.Increment(ref nextId);
        }

        /// <summary>
        /// Gets the unique ID.
        /// </summary>
        [XmlIgnore]
        public int Id { get; private set; }
    }
}
