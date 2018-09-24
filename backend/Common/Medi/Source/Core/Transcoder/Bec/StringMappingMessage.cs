// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMappingMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringMappingMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Internal message between two instances of BEC to transmit
    /// string-to-id mapping information. In BEC strings are only
    /// referenced by an id to save space. The only time, strings
    /// are transmitted is with <see cref="StringMappingMessage"/>s.
    /// </summary>
    internal class StringMappingMessage : IBecSerializable
    {
        /// <summary>
        /// Gets or sets the mappings.
        /// </summary>
        public Dictionary<string, int> Mappings { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("StringMappingMessage[Count={0}]", this.Mappings.Count);
        }

        BecSchema IBecSerializable.GetSchema()
        {
            // we create a dummy schema since we want to do our own serialization
            // which will not change in the future (I hope ;-)
            return new BecSchema(new TypeName(this.GetType()));
        }

        void IBecSerializable.WriteBec(BecWriter writer, BecSchema schema)
        {
            writer.WriteInt32(this.Mappings.Count);

            foreach (var mapping in this.Mappings)
            {
                writer.WriteInt32(mapping.Value);

                // of course we can't use WriteString() here, since that would require string mapping
                writer.WriteBytes(Encoding.UTF8.GetBytes(mapping.Key));
            }
        }

        void IBecSerializable.ReadBec(BecReader reader, BecSchema schema)
        {
            int count = reader.ReadInt32();
            this.Mappings = new Dictionary<string, int>(count);

            for (int i = 0; i < count; i++)
            {
                int id = reader.ReadInt32();

                // of course we can't use ReadString() here, since that would require string mapping
                var data = reader.ReadBytes();
                string str = Encoding.UTF8.GetString(data, 0, data.Length);
                this.Mappings.Add(str, id);
            }
        }
    }
}
