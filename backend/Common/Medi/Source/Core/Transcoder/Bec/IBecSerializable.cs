// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBecSerializable.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBecSerializable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Interface for types that provide their own serialization.
    /// Types implementing this interface must provide an empty
    /// public constructor.
    /// </summary>
    public interface IBecSerializable
    {
        /// <summary>
        /// Gets the BEC schema for this object.
        /// This schema has to be the same object if it represents the same schema.
        /// Do not create a new <see cref="BecSchema"/> every time this method is called!
        /// </summary>
        /// <returns>the BEC schema for this object.</returns>
        BecSchema GetSchema();

        /// <summary>
        /// Writes the contents of this object to the given stream.
        /// </summary>
        /// <param name="writer">
        /// The writer to which this object has to be written.
        /// </param>
        /// <param name="schema">
        /// The schema to be used to write this object.
        /// </param>
        void WriteBec(BecWriter writer, BecSchema schema);

        /// <summary>
        /// Writes the contents of this object to the given stream.
        /// </summary>
        /// <param name="reader">
        /// The reader from which this object has to be read.
        /// </param>
        /// <param name="schema">
        /// The schema to be used to read this object.
        /// </param>
        void ReadBec(BecReader reader, BecSchema schema);
    }
}
