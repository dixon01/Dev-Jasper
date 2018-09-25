// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceContextList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Do not use this class from outside, it is only public for XML serialization.
    /// List that stores the persistence contexts for the <see cref="PersistenceService"/>.
    /// </summary>
    [XmlRoot("Persistence")]
    public class PersistenceContextList : IXmlSerializable
    {
        private readonly Dictionary<string, PersistenceContext> contexts = new Dictionary<string, PersistenceContext>();

        /// <summary>
        /// Gets or sets the owner of this list.
        /// </summary>
        internal PersistenceService Owner { get; set; }

        /// <summary>
        /// Gets the context for a given type and name.
        /// If the context can't be found, a new one is created.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// a <see cref="PersistenceContext{T}"/> for the given type and name.
        /// </returns>
        internal PersistenceContext this[Type type, string name]
        {
            get
            {
                var key = ToKey(type, name);
                PersistenceContext context;
                lock (this.contexts)
                {
                    if (!this.contexts.TryGetValue(key, out context))
                    {
                        var contextType = typeof(PersistenceContext<>).MakeGenericType(type);
                        context = (PersistenceContext)Activator.CreateInstance(contextType);
                        if (this.Owner != null)
                        {
                            context.Validity = this.Owner.DefaultValidity;
                        }

                        this.contexts.Add(key, context);
                    }
                }

                return context;
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement(); // our root element

            while (reader.NodeType == XmlNodeType.Element)
            {
                var name = reader.GetAttribute("name");
                var timestamp = DateTime.Parse(
                    reader.GetAttribute("timestamp"), null, DateTimeStyles.AdjustToUniversal);

                if (reader.IsEmptyElement)
                {
                    reader.ReadStartElement();
                    reader.MoveToContent();
                    continue;
                }

                reader.ReadStartElement(); // Context
                var type = Type.GetType(reader.ReadElementString("Type"), true, false);
                var context = this[type, name];
                context.Validity = TimeSpan.Parse(reader.ReadElementString("Validity"));
                context.LastUpdateUtc = timestamp;
                context.ReadValue(reader);
                reader.MoveToContent();
                reader.ReadEndElement();
                reader.MoveToContent();
            }

            reader.ReadEndElement(); // our root element
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var pair in this.contexts)
            {
                var keyElements = pair.Key.Split('#');
                writer.WriteStartElement("Context");
                writer.WriteAttributeString("name", keyElements[1]);

                writer.WriteStartAttribute("timestamp");
                writer.WriteValue(TimeProvider.Current.UtcNow);
                writer.WriteEndAttribute();

                writer.WriteElementString("Type", pair.Value.ValueType.AssemblyQualifiedName);
                writer.WriteElementString("Validity", pair.Value.Validity.ToString());

                pair.Value.WriteValue(writer);

                writer.WriteEndElement(); // Context
            }
        }

        private static string ToKey(Type type, string name)
        {
            return string.Format("{0}#{1}", type.FullName, name);
        }
    }
}