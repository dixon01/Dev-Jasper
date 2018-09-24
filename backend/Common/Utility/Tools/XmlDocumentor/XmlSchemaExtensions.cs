// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSchemaExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlSchemaExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Schema;

    /// <summary>
    /// Extension methods for classes from <see cref="System.Xml.Schema"/>.
    /// </summary>
    public static class XmlSchemaExtensions
    {
        /// <summary>
        /// Gets all <see cref="XmlSchemaAttribute"/>s of the given <paramref name="type"/>.
        /// This includes inherited attributes.
        /// </summary>
        /// <param name="type">
        /// The schema type.
        /// </param>
        /// <returns>
        /// An enumeration over all <see cref="XmlSchemaAttribute"/>s of the given <paramref name="type"/>.
        /// </returns>
        public static IEnumerable<XmlSchemaAttribute> GetAttributes(this XmlSchemaType type)
        {
            var attributes = new List<XmlSchemaAttribute>();
            GetAttributes(type, attributes);
            return attributes;
        }

        /// <summary>
        /// Gets all <see cref="XmlSchemaElement"/>s of the given <paramref name="type"/>.
        /// This includes inherited elements.
        /// </summary>
        /// <param name="type">
        /// The schema type.
        /// </param>
        /// <returns>
        /// An enumeration over all <see cref="XmlSchemaElement"/>s of the given <paramref name="type"/>.
        /// </returns>
        public static IEnumerable<XmlSchemaElement> GetElements(this XmlSchemaType type)
        {
            var elements = new List<XmlSchemaElement>();
            GetElements(type, elements);
            return elements;
        }

        /// <summary>
        /// Gets the <see cref="XmlSchemaEnumerationFacet"/> of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The schema type.
        /// </param>
        /// <returns>
        /// An enumeration over all <see cref="XmlSchemaEnumerationFacet"/> of the given <paramref name="type"/>.
        /// </returns>
        public static IEnumerable<XmlSchemaEnumerationFacet> GetEnumerations(this XmlSchemaSimpleType type)
        {
            var restriction = type.Content as XmlSchemaSimpleTypeRestriction;
            if (restriction == null)
            {
                yield break;
            }

            foreach (var facet in restriction.Facets.OfType<XmlSchemaEnumerationFacet>())
            {
                yield return facet;
            }
        }

        private static void GetAttributes(XmlSchemaType type, List<XmlSchemaAttribute> attributes)
        {
            if (type.BaseXmlSchemaType != null)
            {
                GetAttributes(type.BaseXmlSchemaType, attributes);
            }

            var complex = type as XmlSchemaComplexType;
            if (complex == null)
            {
                return;
            }

            attributes.AddRange(complex.Attributes.Cast<XmlSchemaAttribute>());
            if (complex.ContentModel == null)
            {
                return;
            }

            var complexContent = complex.ContentModel.Content as XmlSchemaComplexContentExtension;
            if (complexContent != null && complexContent.Attributes != null)
            {
                attributes.AddRange(complexContent.Attributes.Cast<XmlSchemaAttribute>());
                return;
            }

            var simpleContent = complex.ContentModel.Content as XmlSchemaSimpleContentExtension;
            if (simpleContent != null && simpleContent.Attributes != null)
            {
                attributes.AddRange(simpleContent.Attributes.Cast<XmlSchemaAttribute>());
            }
        }

        private static void GetElements(XmlSchemaType type, List<XmlSchemaElement> elements)
        {
            if (type.BaseXmlSchemaType != null)
            {
                GetElements(type.BaseXmlSchemaType, elements);
            }

            var complex = type as XmlSchemaComplexType;
            if (complex == null)
            {
                return;
            }

            GetElements(complex.Particle, elements);

            if (complex.ContentModel == null)
            {
                return;
            }

            var complexContent = complex.ContentModel.Content as XmlSchemaComplexContentExtension;
            if (complexContent != null && complexContent.Attributes != null)
            {
                GetElements(complexContent.Particle, elements);
            }
        }

        private static void GetElements(XmlSchemaParticle particle, List<XmlSchemaElement> elements)
        {
            var sequence = particle as XmlSchemaSequence;
            if (sequence != null)
            {
                elements.AddRange(sequence.Items.Cast<XmlSchemaElement>());
                return;
            }

            var choice = particle as XmlSchemaChoice;
            if (choice != null)
            {
                elements.AddRange(choice.Items.Cast<XmlSchemaElement>());
            }
        }
    }
}
