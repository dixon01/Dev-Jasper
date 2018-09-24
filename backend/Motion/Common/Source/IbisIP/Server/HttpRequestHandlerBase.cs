// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpRequestHandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.IO;

    using NLog;

    /// <summary>
    /// Base class for all request handlers in the IBIS-IP server.
    /// </summary>
    internal abstract class HttpRequestHandlerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private bool validateRequests;

        private bool validateResponses;

        private XmlSchemaSet xsdSchemas;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestHandlerBase"/> class.
        /// </summary>
        protected HttpRequestHandlerBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Configures the XML validation settings.
        /// </summary>
        /// <param name="schemas">
        /// The XSD schemas to validate against.
        /// </param>
        /// <param name="validateReq">
        /// A value indicating whether to validate the XML structure of received HTTP requests.
        /// </param>
        /// <param name="validateResp">
        /// A value indicating whether to validate the XML structure of created HTTP responses.
        /// </param>
        public void SetValidation(XmlSchemaSet schemas, bool validateReq, bool validateResp)
        {
            this.xsdSchemas = schemas;
            this.validateRequests = validateReq;
            this.validateResponses = validateResp;
        }

        /// <summary>
        /// Handles the request to list services as HTML.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public abstract void HandleListRequest(HttpServer.Request request);

        /// <summary>
        /// Handles the request to execute an operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public abstract void HandleRequest(string operationName, HttpServer.Request request);

        /// <summary>
        /// Handles the request to show a form to submit HTTP POST data to an operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public void HandlePostFormRequest(string operationName, HttpServer.Request request)
        {
            var postObject = this.GetDefaultPostObject(operationName);

            var memory = new MemoryStream();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(postObject.GetType());
            serializer.Serialize(memory, postObject, ns);
            var serialized = memory.ToArray();

            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
            using (var writer = XmlWriter.Create(request.GetResponse().GetResponseStream(), settings))
            {
                writer.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
                writer.WriteAttributeString("xml", "lang", string.Empty, "en");
                writer.WriteStartElement("head");
                writer.WriteElementString("title", "Operation " + operationName);
                writer.WriteEndElement(); // head
                writer.WriteStartElement("body");
                writer.WriteElementString("h1", "Operation " + operationName);
                writer.WriteStartElement("form");
                writer.WriteAttributeString("method", "POST");
                writer.WriteAttributeString("action", operationName);
                writer.WriteAttributeString("enctype", "text/plain");
                writer.WriteStartElement("textarea");
                writer.WriteAttributeString("name", "data");
                writer.WriteAttributeString("cols", "80");
                writer.WriteAttributeString("rows", "25");
                writer.WriteString(Encoding.UTF8.GetString(serialized, 0, serialized.Length));
                writer.WriteEndElement(); // textarea
                writer.WriteStartElement("br");
                writer.WriteEndElement(); // br
                writer.WriteStartElement("input");
                writer.WriteAttributeString("type", "submit");
                writer.WriteAttributeString("value", "Submit");
                writer.WriteEndElement(); // form
                writer.WriteEndElement(); // body
                writer.WriteEndElement(); // html
            }
        }

        /// <summary>
        /// Serializes the given object to the given output.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <typeparam name="T">
        /// The type of the object to serialize.
        /// </typeparam>
        protected void Serialize<T>(Stream output, T obj)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(typeof(T));
            if (!this.validateResponses && !this.Logger.IsTraceEnabled)
            {
                serializer.Serialize(output, obj, ns);
                return;
            }

            var memory = new MemoryStream(8092);
            serializer.Serialize(memory, obj, ns);

            if (this.Logger.IsTraceEnabled)
            {
                var response = memory.ToArray();
                this.Logger.Trace("Created response: {0}", Encoding.UTF8.GetString(response, 0, response.Length));
            }

            if (this.validateResponses)
            {
                memory.Position = 0;
                var settings = new XmlReaderSettings
                                   {
                                       Schemas = this.xsdSchemas,
                                       ValidationType = ValidationType.Schema,
                                       ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
                                   };
                settings.ValidationEventHandler +=
                    (s, e) => this.LogValidationEvent(e, "Serializing " + typeof(T).Name);
                using (var reader = XmlReader.Create(memory, settings))
                {
                    // just read everything so the validation is done according to the schema
                    while (reader.Read())
                    {
                    }
                }

                this.Logger.Debug("Serialized {0}", typeof(T).Name);
            }

            memory.WriteTo(output);
        }

        /// <summary>
        /// Deserializes an object from the given input.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The newly created object.
        /// </returns>
        /// <typeparam name="T">
        /// The type of the object to deserialize.
        /// </typeparam>
        protected T Deserialize<T>(Stream input)
        {
            var serializer = new XmlSerializer(typeof(T));
            if (this.Logger.IsTraceEnabled)
            {
                var length = (int)input.Length;
                var data = new byte[length];
                var pos = 0;
                int read;
                while (length > pos && (read = input.Read(data, pos, length - pos)) > 0)
                {
                    pos += read;
                }

                this.Logger.Trace("Received request: {0}", Encoding.UTF8.GetString(data, 0, data.Length));

                input = new MemoryStream(data);
            }

            XmlReaderSettings settings = null;
            if (this.validateRequests)
            {
                settings = new XmlReaderSettings { Schemas = this.xsdSchemas };
                settings.ValidationEventHandler +=
                    (s, e) => this.LogValidationEvent(e, "Deserializing " + typeof(T).Name);
            }

            T obj;
            using (var reader = XmlReader.Create(new SkipNonXmlInputStream(input), settings))
            {
                obj = (T)serializer.Deserialize(reader);
            }

            if (this.validateRequests)
            {
                this.Logger.Debug("Deserialized {0}", typeof(T).Name);
            }

            return obj;
        }

        /// <summary>
        /// Gets the default object to be shown in the POST form for an operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> that will be XML serialized.
        /// </returns>
        protected abstract object GetDefaultPostObject(string operationName);

        /// <summary>
        /// Creates the default object of a given type.
        /// This method populates all properties of the returned object.
        /// This method is usually called from the implementation of
        /// <see cref="GetDefaultPostObject"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T CreateDefaultPostObject<T>()
            where T : new()
        {
            var obj = new T();
            FillProperties(obj);
            return obj;
        }

        private static void FillProperties(object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.GetSetMethod(false) == null)
                {
                    continue;
                }

                if (property.PropertyType == typeof(object))
                {
                    var attrs = property.GetCustomAttributes(typeof(XmlElementAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var type = ((XmlElementAttribute)attrs[0]).Type;
                        property.SetValue(obj, CreatePropertyValue(property.Name, type), null);
                        continue;
                    }
                }

                var value = CreatePropertyValue(property.Name, property.PropertyType);
                property.SetValue(obj, value, null);
            }
        }

        private static object CreatePropertyValue(string name, Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType != null)
                {
                    var array = Array.CreateInstance(elementType, 2);
                    for (int i = 0; i < 2; i++)
                    {
                        array.SetValue(CreatePropertyValue(name + "-" + i, elementType), i);
                    }

                    return array;
                }
            }

            if (type == typeof(IBISIPstring))
            {
                return new IBISIPstring(name);
            }

            if (type == typeof(IBISIPduration))
            {
                return new IBISIPduration { Value = "PT10M" };
            }

            if (type == typeof(IBISIPdate))
            {
                return new IBISIPdate { Value = TimeProvider.Current.Now.Date };
            }

            if (type == typeof(IBISIPtime))
            {
                return new IBISIPtime { Value = TimeProvider.Current.Now };
            }

            if (type == typeof(IBISIPdateTime))
            {
                return new IBISIPdateTime { Value = TimeProvider.Current.Now };
            }

            if (type == typeof(IBISIPNMTOKEN))
            {
                return new IBISIPNMTOKEN(Definitions.CurrentVersion);
            }

            if (type.Name.StartsWith("IBISIP"))
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(InternationalTextType))
            {
                return new InternationalTextType
                           {
                               ErrorCodeSpecified = false,
                               Language = "de-DE",
                               Value = name
                           };
            }

            var obj = Activator.CreateInstance(type);
            FillProperties(obj);
            return obj;
        }

        private void LogValidationEvent(ValidationEventArgs eventArgs, string prefix)
        {
            var level = eventArgs.Severity == XmlSeverityType.Error ? LogLevel.Error : LogLevel.Warn;
            this.Logger.Log(level, eventArgs.Exception, prefix + ": " + eventArgs.Message);
        }

        /// <summary>
        /// Wrapper around a stream that allows to skip non-xml characters.
        /// This stream simply skips all bytes before the first '&lt;' when
        /// <see cref="Read"/> or <see cref="ReadByte"/> are called.
        /// This stream doesn't work asynchronously.
        /// </summary>
        private class SkipNonXmlInputStream : WrapperStream
        {
            private bool xmlStarted;

            public SkipNonXmlInputStream(Stream input)
            {
                this.xmlStarted = false;
                this.Open(input);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                while (true)
                {
                    var read = base.Read(buffer, offset, count);
                    if (this.xmlStarted || read == 0)
                    {
                        return read;
                    }

                    // find the first '<'
                    var index = Array.IndexOf(buffer, (byte)'<', 0, read);

                    if (index == 0)
                    {
                        // simple case: input starts with a '<'
                        this.xmlStarted = true;
                        return read;
                    }

                    if (index < 0)
                    {
                        // we didn't find the '<', so let's read again
                        continue;
                    }

                    // complicated case: we found the '<', let's copy the data to the beginning of the buffer
                    this.xmlStarted = true;
                    Array.Copy(buffer, index, buffer, 0, read - index);
                    return read - index;
                }
            }

            public override int ReadByte()
            {
                var read = base.ReadByte();
                if (this.xmlStarted)
                {
                    return read;
                }

                while (read != '<' && read != -1)
                {
                    read = base.ReadByte();
                }

                this.xmlStarted = read != -1;
                return read;
            }
        }
    }
}