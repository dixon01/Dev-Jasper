// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.T4Templating.Extensibility
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extends a document with another one.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="extensionPath">
        /// The extension path.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        /// <exception cref="FileLoadException">It was not possible to load a file.</exception>
        public static Stream Extend(this string sourcePath, string extensionPath)
        {
            var myXPathDoc = new XPathDocument(sourcePath);
            var myXslTrans = new XslCompiledTransform();
            var argsList = new XsltArgumentList();
            argsList.AddParam("name", string.Empty, "Partition1");
            argsList.AddParam("extension", string.Empty, extensionPath);
            var assembly = typeof(Extensions).Assembly;
            var memoryStream = new MemoryStream();
            using (
                var xsdStream =
                    assembly.GetManifestResourceStream(
                        "Gorba.Center.Common.T4Templating.Extensibility.Extender.xslt"))
            {
                if (xsdStream == null)
                {
                    throw new FileLoadException("Can't load the xslt stream");
                }

                using (var xsdReader = XmlReader.Create(xsdStream))
                {
                    var settings = new XsltSettings(true, false);
                    myXslTrans.Load(xsdReader, settings, new XmlUrlResolver());
                }
            }

            var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var myWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            if (myWriter.Settings == null)
            {
                throw new Exception();
            }

            myXslTrans.Transform(myXPathDoc, argsList, myWriter);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        /// <summary>
        /// Extends a document with another one and return the resulting Xml as a string.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="extensionPath">
        /// The extension path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> with the Xml resulting from the extension.
        /// </returns>
        public static string ExtendAsString(this string sourcePath, string extensionPath)
        {
            using (var stream = sourcePath.Extend(extensionPath))
            {
                var xmlDocument = new XmlDocument();
                using (var memoryStream = new MemoryStream())
                {
                    xmlDocument.Load(stream);
                    using (var xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlDocument.WriteContentTo(xmlWriter);
                        xmlWriter.Flush();
                        memoryStream.Flush();

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}