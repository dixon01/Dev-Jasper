// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuiltInSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This interface is not meant for use outside this assembly.
//   Tagging interface to know if a generated serializer is for a built-in type.
//   This allows us to improve speed of built-in type serialization by inlining
//   the code instead of using a serializer object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// This interface is not meant for use outside this assembly.
    /// Tagging interface to know if a generated serializer is for a built-in type.
    /// This allows us to improve speed of built-in type serialization by in-lining
    /// the code instead of using a serializer object.
    /// </summary>
    public partial interface IBuiltInSchemaSerializer
    {
    }
}
