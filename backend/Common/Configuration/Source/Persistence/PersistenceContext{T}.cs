// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceContext{T}.cs" company="Gorba AG">
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
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// <see cref="IPersistenceContext{T}"/> implementation used by the
    /// persistence service.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the <see cref="Value"/> of this context.
    /// </typeparam>
    internal class PersistenceContext<T> : PersistenceContext, IPersistenceContext<T>
        where T : new()
    {
        private readonly XmlSerializer serializer;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceContext{T}"/> class.
        /// </summary>
        public PersistenceContext()
        {
            this.serializer = new XmlSerializer(typeof(T));
        }

        /// <summary>
        /// Gets or sets the serializable value object.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.Revalidate();
            }
        }

        /// <summary>
        /// Gets the type of the <see cref="IPersistenceContext{T}.Value"/>.
        /// </summary>
        public override Type ValueType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Makes this context valid again (setting the timestamp to the current value).
        /// </summary>
        public void Revalidate()
        {
            this.LastUpdateUtc = TimeProvider.Current.UtcNow;
        }

        /// <summary>
        /// Write the <see cref="IPersistenceContext{T}.Value"/> to the given
        /// <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public override void WriteValue(XmlWriter writer)
        {
            this.serializer.Serialize(writer, this.Value);
        }

        /// <summary>
        /// Reads the <see cref="IPersistenceContext{T}.Value"/> from the given
        /// <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public override void ReadValue(XmlReader reader)
        {
            this.value = (T)this.serializer.Deserialize(reader);
        }
    }
}
