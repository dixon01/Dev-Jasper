// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configurator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Configurator class. Use this to Serialize and Deserialize objects
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using NLog;

    /// <summary>
    /// Configurator class. Use this to Serialize and Deserialize objects
    /// </summary>
    public class Configurator
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Configurator).FullName);

        /// <summary>
        /// The absolute path of the file containing the information
        /// to serialize/deserialize.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// Gets or sets the <see cref="MemoryStream"/>.
        /// Used for testing
        /// </summary>
        private readonly Stream stream;

        private readonly XmlSchema schema;

        private readonly XmlSchemaSet schemaSet;

        private readonly List<XmlSchemaException> exceptions;

        private IXmlLineInfo currentLineInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// Allocates all the resources needed by this object.
        /// </summary>
        /// <param name="path">
        /// The absolute path of the file containing the information to serialize/deserialize.
        /// </param>
        public Configurator(string path)
        {
            this.path = path;
            this.exceptions = new List<XmlSchemaException>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// Allocates all the resources needed by this object.
        /// </summary>
        /// <param name="path">
        /// The absolute path of the file containing the information to serialize/deserialize.
        /// </param>
        /// <param name="schema">
        /// The xml schema used for validation.
        /// </param>
        public Configurator(string path, XmlSchema schema)
        {
            this.path = path;
            this.schema = schema;
            this.exceptions = new List<XmlSchemaException>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// </summary>
        /// <param name="path">
        /// The absolute path of the file containing the information to serialize/deserialize.
        /// </param>
        /// <param name="schemaSet">
        /// The xml schema set used for validation.
        /// </param>
        public Configurator(string path, XmlSchemaSet schemaSet)
        {
            this.path = path;
            this.schemaSet = schemaSet;
            this.exceptions = new List<XmlSchemaException>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// </summary>
        /// <param name="stream">
        /// Represents the stream that could be used during serialization and deserialization operations.
        /// If the other constructor is used, the internal stream will be an FileStream created with the specified path.
        /// </param>
        public Configurator(Stream stream)
        {
            this.path = string.Empty;
            this.stream = stream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// </summary>
        /// <param name="stream">
        /// Represents the stream that could be used during serialization and deserialization operations.
        /// If the other constructor is used, the internal stream will be an FileStream created with the specified path.
        /// </param>
        /// <param name="schema">
        /// The xml schema used for validation.
        /// </param>
        public Configurator(Stream stream, XmlSchema schema)
            : this(string.Empty, schema)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// </summary>
        /// <param name="stream">
        /// Represents the stream that could be used during serialization and deserialization operations.
        /// If the other constructor is used, the internal stream will be an FileStream created with the specified path.
        /// </param>
        /// <param name="schemaSet">
        /// The xml schema set used for validation.
        /// </param>
        public Configurator(Stream stream, XmlSchemaSet schemaSet)
        {
            this.path = string.Empty;
            this.schemaSet = schemaSet;
            this.stream = stream;
            this.exceptions = new List<XmlSchemaException>();
        }

        /// <summary>
        /// Save an object as XML file.
        /// The XML file will have the name as indicated during this object's construction.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">
        /// The type used
        /// </typeparam>
        public void Serialize<T>(T obj) where T : class, new()
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "The object to serialize must be defined.");
            }

            FileStream fs = null;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                Stream streamToSerialize;
                if (this.stream != null)
                {
                    streamToSerialize = this.stream;
                }
                else
                {
                    fs = new FileStream(this.path, FileMode.Create, FileAccess.Write);
                    streamToSerialize = fs;
                }

                serializer.Serialize(streamToSerialize, obj);
            }
            catch (Exception e)
            {
                var msg = string.Format("Serialize error : {0} ({1})", typeof(T).FullName, this.path);
                throw new ConfiguratorException(msg, typeof(T), e);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Converts the object into an XML string.
        /// </summary>
        /// <typeparam name="T">The type used</typeparam>
        /// <param name="obj">
        /// The object to translate into an XML string.
        /// </param>
        /// <returns>
        /// The XML string representation of the incoming object.
        /// </returns>
        public string ToXmlString<T>(T obj) where T : class, new()
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            var memStream = new MemoryStream();
            try
            {
                xmlSerializer.Serialize(memStream, obj);
            }
            catch (Exception)
            {
                // an error was occured serializing this objct in XML format.
                // I cannot return any string.
                return string.Empty;
            }

            // ok, this object was serialized well.
            byte[] ximpleArray;
            try
            {
                ximpleArray = memStream.ToArray();
            }
            catch (Exception)
            {
                ximpleArray = null;
            }

            if (ximpleArray == null)
            {
                // an error was occured on getting the array of the serialized object.
                // I cannot return any string.
                return string.Empty;
            }

            // ok, now it's the time to translate the buffer got above
            // in a readable XML string.
            string xml;
            try
            {
                xml = System.Text.Encoding.UTF8.GetString(ximpleArray, 0, ximpleArray.Length);
            }
            catch (Exception)
            {
                xml = string.Empty;
            }

            if (string.IsNullOrEmpty(xml))
            {
                // an error was occured on getting the XML string.
                // I cannot return any string.
                return string.Empty;
            }

            // ok, everything seems ok.
            return xml;
        }

        /// <summary>
        /// Read an XML File and convert it to an object
        /// </summary>
        /// <typeparam name="T">
        /// The specific type to cast the de-serialized content stored
        /// into the file indicated during this object's construction.
        /// </typeparam>
        /// <exception cref="FileNotFoundException">If the file has not been found.</exception>
        /// <exception cref="ConfiguratorException">
        /// If an Exception occurred. The InnerException can be a <see cref="XmlValidationException"/>
        /// or another <see cref="Exception"/>
        /// </exception>
        /// <returns>
        /// The object representing the de-serialization result of the content
        /// stored into the file indicated during this object's construction.
        /// </returns>
        public T Deserialize<T>() where T : class, new()
        {
            if (this.stream == null && !File.Exists(this.path))
            {
                throw new FileNotFoundException(
                    "The file for deserialization is not found: " + this.path);
            }

            FileStream fs = null;

            try
            {
                XmlReaderSettings settings = null;
                if (this.schema != null || this.schemaSet != null)
                {
                    settings = this.GetXmlReaderSettings();
                }

                Stream streamToSerialize;
                if (this.stream != null)
                {
                    streamToSerialize = this.stream;
                    streamToSerialize.Seek(0, 0);
                }
                else
                {
                    fs = new FileStream(this.path, FileMode.Open, FileAccess.Read);
                    streamToSerialize = fs;
                }

                T result;
                var serializer = new XmlSerializer(typeof(T));

                if (settings == null)
                {
                    return serializer.Deserialize(streamToSerialize) as T;
                }

                using (var reader = XmlReader.Create(streamToSerialize, settings))
                {
                    this.currentLineInfo = reader as IXmlLineInfo;
                    result = serializer.Deserialize(reader) as T;
                    this.currentLineInfo = null;
                }

                if (this.exceptions.Count != 0)
                {
                    throw new XmlValidationException(this.exceptions);
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Deserialize<" + typeof(T) + "> Exception File=" + this.path);
                var msg = string.Format("Deserialize error : {0} ({1})", typeof(T).FullName, this.path);
                var validationException = e as XmlValidationException;
                if (validationException != null)
                {
                    foreach (var exception in validationException.Exceptions)
                    {
                        Debug.WriteLine(exception.Message);
                        Logger.Error(exception.Message);
                    }
                }
                throw new ConfiguratorException(msg, typeof(T), e);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Sets the settings for validation against a schema and
        /// adds a ValidationEventHandler to store / log errors and warnings.
        /// </summary>
        /// <returns>The created settings object.</returns>
        private XmlReaderSettings GetXmlReaderSettings()
        {
            this.exceptions.Clear();
            XmlSchemaSet schemas;
            if (this.schemaSet != null)
            {
                schemas = this.schemaSet;
            }
            else
            {
                schemas = new XmlSchemaSet();
                schemas.Add(this.schema);
            }

            var settings = new XmlReaderSettings
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema,
                ValidationFlags =
                       XmlSchemaValidationFlags.ReportValidationWarnings

                       // Attention:
                       // if we add the flag "ProcessSchemaLocation" the xsd reader
                       // will try to process the schema stored in a specific path.
                       // this path should be specified in the xsi attributes:
                       // xsi:schemaLocation
                       // xsi:noNamespaceSchemaLocation
                       // but if we don't have them, they will be assumed to be
                       // referring to the path of the process launcher.
                       //
                       // Therefore, for safety reasons I disable this flag
                       // | XmlSchemaValidationFlags.ProcessSchemaLocation
            };

            settings.ValidationEventHandler +=
                delegate(object sender, ValidationEventArgs e)
                    {
                        this.exceptions.Add(e.Exception);

                        var level = e.Severity == XmlSeverityType.Error ? LogLevel.Error : LogLevel.Warn;
                        var lineInfo = this.currentLineInfo;
                        if (lineInfo != null && lineInfo.HasLineInfo())
                        {
                            Logger.Log(
                                level,
                                "{0} at line {1}, column {2}.",
                                e.Message.TrimEnd('.'),
                                lineInfo.LineNumber,
                                lineInfo.LinePosition);
                        }
                        else
                        {
                            Logger.Log(level, e.Message);
                        }
                    };

            return settings;
        }
    }
}