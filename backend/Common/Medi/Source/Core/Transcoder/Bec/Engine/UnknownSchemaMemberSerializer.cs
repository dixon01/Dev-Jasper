// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSchemaMemberSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    /// <summary>
    /// Serializer wrapper for a member of an unknown type (i.e. a type that
    /// does not exist on this node, but does exist on the source and the 
    /// destination of the message).
    /// </summary>
    internal class UnknownSchemaMemberSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownSchemaMemberSerializer"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the member.
        /// </param>
        public UnknownSchemaMemberSerializer(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the serializer for this member.
        /// </summary>
        public ISchemaSerializer Serializer { get; set; }
    }
}
