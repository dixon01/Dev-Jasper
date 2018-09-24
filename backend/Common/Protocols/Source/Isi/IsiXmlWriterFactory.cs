// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiXmlWriterFactory.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiXmlWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi
{
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Factory class to create <see cref="XmlWriter"/>s that are compliant to the ISI specification.
    /// </summary>
    public static class IsiXmlWriterFactory
    {
        private static readonly XmlWriterSettings Settings;

        static IsiXmlWriterFactory()
        {
            Settings = new XmlWriterSettings
            {
                CloseOutput = false,
                Encoding = new UTF8Encoding(false),
                Indent = false,
                OmitXmlDeclaration = true
            };
        }

        /// <summary>
        /// Creates a new <see cref="XmlWriter"/> instance using the stream.
        /// </summary>
        /// <param name="output">
        /// The stream to which you want to write. The <see cref="XmlWriter"/> 
        /// writes XML 1.0 text syntax and appends it to the specified stream.
        /// </param>
        /// <returns>
        /// An <see cref="XmlWriter"/> object.
        /// </returns>
        public static XmlWriter Create(Stream output)
        {
            return XmlWriter.Create(output, Settings);
        }

        /// <summary>
        /// Creates a new <see cref="XmlWriter"/> instance using the stream.
        /// </summary>
        /// <param name="output">
        /// The <see cref="TextWriter"/> to which you want to write. 
        /// The <see cref="XmlWriter"/> writes XML 1.0 text syntax and 
        /// appends it to the specified <see cref="TextWriter"/>.
        /// </param>
        /// <returns>
        /// An <see cref="XmlWriter"/> object.
        /// </returns>
        public static XmlWriter Create(TextWriter output)
        {
            return XmlWriter.Create(output, Settings);
        }
    }
}
