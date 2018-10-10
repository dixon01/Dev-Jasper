// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaSerializerFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection;

    /// <summary>
    /// A base class for all factories for schema serializer classes.
    /// </summary>
    internal abstract partial class SchemaSerializerFactory
    {
        /// <summary>
        /// Creates a serializer factory for the given type of serializer.
        /// </summary>
        /// <param name="serializer">
        /// The type of the serializer factory requested.
        /// </param>
        /// <returns>
        /// Either a <see cref="GenerateSchemaSerializerFactory"/> or a
        /// <see cref="ReflectionSchemaSerializerFactory"/>.
        /// </returns>
        public static SchemaSerializerFactory Create(BecCodecConfig.SerializerType serializer)
        {
            switch (serializer)
            {
                case BecCodecConfig.SerializerType.Generated:
                    return new GenerateSchemaSerializerFactory();
                default:
                    return new ReflectionSchemaSerializerFactory();
            }
        }
    }
}
