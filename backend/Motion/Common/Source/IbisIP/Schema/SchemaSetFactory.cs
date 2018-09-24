// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaSetFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaSetFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Schema
{
    using System;
    using System.Xml.Schema;

    using Gorba.Motion.Common.IbisIP.Server;

    /// <summary>
    /// Factory for the VDV 301 schema set.
    /// </summary>
    public static class SchemaSetFactory
    {
        /// <summary>
        /// Loads the VDV 301 schema set.
        /// </summary>
        /// <returns>
        /// The compiled <see cref="XmlSchemaSet"/>.
        /// </returns>
        public static XmlSchemaSet LoadSchemaSet()
        {
            var schemaSet = new XmlSchemaSet();
            var asm = typeof(IbisHttpServer).Assembly;
            foreach (var resource in asm.GetManifestResourceNames())
            {
                if (!resource.EndsWith(".xsd", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var stream = asm.GetManifestResourceStream(resource);
                if (stream != null)
                {
                    schemaSet.Add(XmlSchema.Read(stream, null));
                }
            }

            schemaSet.Compile();
            return schemaSet;
        }
    }
}
