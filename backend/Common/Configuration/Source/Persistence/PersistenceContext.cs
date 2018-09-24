// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PersistenceContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;
    using System.Xml;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Non-generic base class for <see cref="IPersistenceContext{T}"/> implementations.
    /// This class should not be used outside this namespace.
    /// </summary>
    internal abstract class PersistenceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceContext"/> class.
        /// </summary>
        protected PersistenceContext()
        {
            this.LastUpdateUtc = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the validity period.
        /// </summary>
        public TimeSpan Validity { get; set; }

        /// <summary>
        /// Gets or sets the time of the last update in UTC.
        /// </summary>
        public DateTime LastUpdateUtc { get; set; }

        /// <summary>
        /// Gets a value indicating whether the value is still valid.
        /// This method checks the <see cref="Validity"/> period.
        /// </summary>
        public bool Valid
        {
            get
            {
                return TimeProvider.Current.UtcNow - this.LastUpdateUtc < this.Validity;
            }
        }

        /// <summary>
        /// Gets the type of the <see cref="IPersistenceContext{T}.Value"/>.
        /// </summary>
        public abstract Type ValueType { get; }

        /// <summary>
        /// Write the <see cref="IPersistenceContext{T}.Value"/> to the given
        /// <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public abstract void WriteValue(XmlWriter writer);

        /// <summary>
        /// Reads the <see cref="IPersistenceContext{T}.Value"/> from the given
        /// <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public abstract void ReadValue(XmlReader reader);
    }
}
