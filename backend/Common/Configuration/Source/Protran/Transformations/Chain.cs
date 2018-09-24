// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Chain.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of a set of transformation to be applied on a telegram.
    /// </summary>
    [Serializable]
    public class Chain : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Chain"/> class.
        /// </summary>
        public Chain()
        {
            this.Transformations = new List<TransformationConfig>();
        }

        /// <summary>
        /// Gets or sets the XML attribute called from.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Telegrams.
        /// </summary>
        public List<TransformationConfig> Transformations { get; set; }

        /// <summary>
        /// Helper method that resolves <see cref="ChainRef"/>s and returns a "flat"
        /// list of <see cref="TransformationConfig"/> objects.
        /// </summary>
        /// <param name="allChains">
        /// The list of all chains configured in this application.
        /// </param>
        /// <returns>
        /// An enumerator over all <see cref="TransformationConfig"/> objects for the given chain.
        /// </returns>
        public IEnumerable<TransformationConfig> ResolveReferences(ICollection<Chain> allChains)
        {
            foreach (var transformation in this.Transformations)
            {
                var chainRef = transformation as ChainRef;
                if (chainRef == null)
                {
                    yield return transformation;
                }
                else
                {
                    var chain = FindChain(allChains, chainRef.TransfRef);
                    foreach (var subTransform in chain.ResolveReferences(allChains))
                    {
                        yield return subTransform;
                    }
                }
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Transformations = new List<TransformationConfig>();

            this.Id = reader.GetAttribute("id");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
                var type = Type.GetType(this.GetType().Namespace + "." + reader.Name, true, true);
                if (type == null)
                {
                    throw new NullReferenceException("Type cannot be null.");
                }

                var ser = new XmlSerializer(type);
                var transformation = ser.Deserialize(reader) as TransformationConfig;
                if (transformation != null)
                {
                    this.Transformations.Add(transformation);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", this.Id);

            foreach (var transformation in this.Transformations)
            {
                var ser = new XmlSerializer(transformation.GetType());
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                ser.Serialize(writer, transformation, namespaces);
            }
        }

        /// <summary>
        /// Finds the first chain matching the given id.
        /// </summary>
        /// <param name="allChains">a list of chains</param>
        /// <param name="id">the id to look for.</param>
        /// <returns>the chain</returns>
        /// <exception cref="KeyNotFoundException">if there is no chain with the given id.</exception>
        private static Chain FindChain(IEnumerable<Chain> allChains, string id)
        {
            foreach (var chain in allChains)
            {
                if (chain.Id.Equals(id))
                {
                    return chain;
                }
            }

            throw new KeyNotFoundException("Could not find referenced chain: " + id);
        }
    }
}
