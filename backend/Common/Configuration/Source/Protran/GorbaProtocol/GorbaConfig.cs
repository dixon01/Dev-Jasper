// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.GorbaProtocol
{
    using System;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The Gorba protocol configuration object.
    /// </summary>
    [XmlRoot("Gorba")]
    [Serializable]
    public class GorbaConfig
    {
        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(GorbaConfig).Assembly.GetManifestResourceStream(
                            typeof(GorbaConfig), "gorba.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find ibis.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }
    }
}