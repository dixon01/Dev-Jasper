// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedSchemaSerializerBase.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GeneratedSchemaSerializerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;

    /// <summary>
    /// This class is not meant for use outside this assembly.
    /// Non-generic base class for all serializer implementations in the Generated namespace.
    /// Do not directly subclass this class but rather use
    /// <see cref="GeneratedSchemaSerializerBase{T}"/> instead.
    /// </summary>
    public abstract partial class GeneratedSchemaSerializerBase : ISchemaSerializer
    {
        void ISchemaSerializer.Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            throw new NotImplementedException();
        }

        object ISchemaSerializer.Deserialize(BecReader reader, SerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
