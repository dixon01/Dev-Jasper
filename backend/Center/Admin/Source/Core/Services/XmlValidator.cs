// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Helper class for XML validation that also caches the result of the validation for speed.
    /// </summary>
    public static class XmlValidator
    {
        private const int ResultCacheCount = 10;

        private static readonly LinkedList<ValidationResult> LastResults = new LinkedList<ValidationResult>();

        /// <summary>
        /// Validates the given <paramref name="xml"/> string.
        /// </summary>
        /// <param name="xml">
        /// The XML string.
        /// </param>
        /// <param name="schema">
        /// The XML schema used to validate the XML. If this is null, the XML will only
        /// be checked for syntax errors.
        /// </param>
        /// <returns>
        /// An array of <see cref="XmlException"/>.
        /// Never null. If no error was found, it will return an empty array.
        /// </returns>
        public static XmlException[] Validate(string xml, XmlSchema schema = null)
        {
            ValidationResult result;

            lock (LastResults)
            {
                result = LastResults.FirstOrDefault(r => string.Equals(r.Xml, xml) && object.Equals(r.Schema, schema));
            }

            if (result != null)
            {
                return result.Exceptions;
            }

            var exceptions = DoValidation(xml, schema);
            lock (LastResults)
            {
                while (LastResults.Count >= ResultCacheCount)
                {
                    LastResults.RemoveLast();
                }

                LastResults.AddFirst(new ValidationResult(xml, schema, exceptions));
            }

            return exceptions;
        }

        private static XmlException[] DoValidation(string xml, XmlSchema schema)
        {
            var exceptions = new List<XmlException>();

            try
            {
                var stringReader = new StringReader(xml);
                while (stringReader.Peek() > 0xFF)
                {
                    // skip unreadable byte marks
                    stringReader.Read();
                }

                XmlReader reader;
                if (schema == null)
                {
                    reader = XmlReader.Create(stringReader);
                }
                else
                {
                    var schemas = new XmlSchemaSet();
                    schemas.Add(schema);

                    var settings = new XmlReaderSettings { Schemas = schemas, ValidationType = ValidationType.Schema };
                    settings.ValidationEventHandler += (s, e) =>
                        {
                            if (e.Severity == XmlSeverityType.Error)
                            {
                                exceptions.Add(
                                    new XmlException(
                                        e.Exception.Message,
                                        e.Exception,
                                        e.Exception.LineNumber,
                                        e.Exception.LinePosition));
                            }
                        };
                    reader = XmlReader.Create(stringReader, settings);
                }

                // read the entire document
                while (reader.Read())
                {
                }
            }
            catch (XmlException ex)
            {
                exceptions.Add(ex);
            }
            catch (Exception ex)
            {
                exceptions.Add(new XmlException(ex.Message, ex, 1, 1));
            }

            return exceptions.ToArray();
        }

        private class ValidationResult
        {
            public ValidationResult(string xml, XmlSchema schema, XmlException[] exceptions)
            {
                this.Xml = xml;
                this.Schema = schema;
                this.Exceptions = exceptions;
            }

            public string Xml { get; private set; }

            public XmlSchema Schema { get; private set; }

            public XmlException[] Exceptions { get; private set; }
        }
    }
}
