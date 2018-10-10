// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ximple.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Object that represents a whole XML string referring to the XIMPLE protocol.
//   The XIMPLE protocol's TAGs and elements, are here mapped in the referring
//   object's fields.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Ximple.Utils;

    /// <summary>
    /// Object that represents a whole XML string referring to the XIMPLE protocol.
    /// The XIMPLE protocol's TAGs and elements, are here mapped in the referring
    /// object's fields.
    /// </summary>
    public class Ximple
    {
        /// <summary>
        /// The TAG containing the version's information about the current
        /// used XIMPLE structure.
        /// </summary>
        private string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ximple"/> class.
        /// Will be used the default version value.
        /// </summary>
        public Ximple()
            : this(Constants.DefaultVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ximple"/> class
        /// with also a specific version of the referring XIMPLE structure.
        /// If the incoming "version" value is an empty or null string,
        /// will be used the default one.
        /// </summary>
        /// <param name="version">
        /// The version about the current used
        /// XIMPLE structure.
        /// </param>
        public Ximple(string version)
        {
            this.Version = version;
            this.Cells = new List<XimpleCell>();
        }

        /// <summary>
        /// Gets or sets the version about the current used XIMPLE structure.
        /// If the incoming "version" value is an empty or null string,
        /// will be used the default one.
        /// </summary>
        [XmlAttribute(AttributeName = "Version")]
        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.version = Constants.DefaultVersion;
                }
                else if (value == Constants.Version2)
                {
                    this.version = value;
                }
                else
                {
                    throw new NotSupportedException("Ximple version not supported: " + value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the version 2.0 cells.
        /// </summary>
        [XmlArrayItem("Cell")]
        public List<XimpleCell> Cells { get; set; }

        /// <summary>
        /// Converts this object to an object according to
        /// the given version.
        /// If both versions are the same, this object
        /// will be returned.
        /// </summary>
        /// <param name="newVersion">
        /// The new version number. Currently only 1.0.0. and 2.0.0 are supported.
        /// </param>
        /// <returns>
        /// a Ximple object according to the given version. This object might
        /// be the same as the one the method was called on.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// If an unsupported version number is given or the conversion is not supported.
        /// </exception>
        public Ximple ConvertTo(string newVersion)
        {
            if (this.Version == newVersion)
            {
                return this;
            }

            // currently only version 2.0.0 is supported
            throw new NotSupportedException(
                "Can't convert from Ximple version " + this.Version + " to " + newVersion);
        }

        /// <summary>
        /// Translates this object into an XML string.
        /// </summary>
        /// <returns>The XML string representing this object.</returns>
        public string ToXmlString()
        {
            // todo: use exceptions instead of empty return values!
            var xmlSerializer = new XmlSerializer(this.GetType());
            using (var memStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memStream, this);
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
                    xml = Encoding.UTF8.GetString(ximpleArray, 0, ximpleArray.Length);
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
        
        }
    }
}
