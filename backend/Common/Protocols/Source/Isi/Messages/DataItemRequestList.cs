// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemRequestList.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemRequestList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// List of strings that XML serialize to a space delimited string.
    /// Used for <see cref="IsiGet"/> and <see cref="IsiRegister"/> messages.
    /// </summary>
    public class DataItemRequestList : List<string>, IXmlSerializable
    {
        /// <summary>
        /// Wildcard (*) list usable for <see cref="IsiGet.OnChange"/>.
        /// </summary>
        public static readonly DataItemRequestList Wildcard = new DataItemRequestList("*");

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemRequestList"/> class.
        /// The list is empty by default.
        /// </summary>
        public DataItemRequestList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemRequestList"/> class
        /// with the given names.
        /// </summary>
        /// <param name="names">
        /// The names to add to this list.
        /// </param>
        public DataItemRequestList(params string[] names)
            : base(names)
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Join(" ", this.ToArray());
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var text = reader.ReadElementString();
            this.AddRange(text.Split(' '));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(string.Join(" ", this.ToArray()));
        }
    }
}
