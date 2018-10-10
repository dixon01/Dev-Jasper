// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownXmlObject.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownXmlObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Xml
{
    using System.Xml;

    /// <summary>
    /// <see cref="IUnknown"/> implementation for XML.
    /// This is a placeholder for all unknown objects that get
    /// deserialized.
    /// </summary>
    internal class UnknownXmlObject : IUnknown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownXmlObject"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name this object will represent.
        /// </param>
        /// <param name="reader">
        /// The reader from which the XML content is read.
        /// </param>
        public UnknownXmlObject(TypeName typeName, XmlReader reader)
            : this(typeName, reader.ReadOuterXml())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownXmlObject"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name this object will represent.
        /// </param>
        /// <param name="xml">
        /// The XML content.
        /// </param>
        public UnknownXmlObject(TypeName typeName, string xml)
        {
            this.TypeName = typeName;
            this.Xml = xml;
        }

        /// <summary>
        /// Gets the type name of the object represented by this object.
        /// The TypeName's <see cref="Core.TypeName.IsKnown"/> should return false.
        /// </summary>
        public TypeName TypeName { get; private set; }

        /// <summary>
        /// Gets the xml content of this object.
        /// </summary>
        public string Xml { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.GetType().Name, this.TypeName.FullName);
        }
    }
}
