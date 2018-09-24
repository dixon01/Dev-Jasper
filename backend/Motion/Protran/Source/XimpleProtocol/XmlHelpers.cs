// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XmlHelpers.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Ximple;

    using NLog;

    /// <summary>The xml helpers.</summary>
    public static class XmlHelpers
    {
        /// <summary>The xml start tag.</summary>
        public const string XmlStartTag = "<?xml version";

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>De-serialize xml to object.</summary>
        /// <param name="xml">The xml.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <typeparam name="T">Object Type</typeparam>
        /// <returns>The <see cref="T"/>Object of type T.</returns>
        /// <exception cref="InvalidOperationException">Bad Xml or invalid Ximple Version.</exception>
        public static T DeserializeXmlToObject<T>(string xml, string nameSpace = "")
        {
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T), nameSpace);
                    return (T)xmlSerializer.Deserialize(stringReader);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Logger.Error(invalidOperationException, "Failed to De-serialize XML to Message Board Content model");

                // return default(T);
                throw;
            }
        }

        public static int CountXmlDocument(string xmlString)
        {
            return Regex.Matches(xmlString, XmlStartTag).Count;
        }

        /// <summary>Test if the the xml string is a single valid xml document.</summary>
        /// <param name="xmlString">The xml string.</param>
        /// <param name="count">The count.</param>
        /// <returns>The <see cref="bool"/>True if valid else false</returns>
        public static bool IsValidXmlDocument(string xmlString, out int count)
        {
            var valid = false;
            count = 0;
            if (string.IsNullOrEmpty(xmlString))
            {
                Logger.Warn("Invalid XmlDocument Empty string");
            }
            else
            {
                try
                {
                    // Check we actually have a value that represents a full single xml doc
                    // Try to load the value into a document
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);
                    valid = true;
                    count++;
                }
                catch (XmlException ex)
                {
                    // the XML string could be multiple xml documents appended together that we will have to parse latter
                    count = CountXmlDocument(xmlString);
                    if (count <= 1)
                    {
                        Logger.Warn("Partial XmlDocument {0}\nXML=[{1}]", ex.Message, xmlString);
                    }
                    else
                    {
                        Logger.Warn("Received multiple XML Documents Count={0}", count);
                    }                   
                }
            }

            return valid;
        }

        /// <summary>Xml to de-serialize to Ximple class object.</summary>
        /// <param name="xml">The xml string.</param>
        /// <returns>The <see cref="Ximple"/>.</returns>
        public static Ximple ToXimple(string xml)
        {
            return DeserializeXmlToObject<Ximple>(xml);
        }

        #endregion
    }
}