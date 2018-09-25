// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Playground.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Playground type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// Class for testing how generated types should look like.
    /// Uncomment its contents only if you need to change
    /// the code generation and need an example (use ILDASM to
    /// inspect the compiled IL).
    /// </summary>
    internal static class Playground
    {
        /*
        private class MediMessageSerializer : GeneratedSchemaSerializerBase<MediMessage>
        {
            private GeneratedSchemaSerializerBase<MediAddress> Destination;

            private GeneratedSchemaSerializerBase<object> Payload;

            private GeneratedSchemaSerializerBase<MediAddress> Source;

            public override void Serialize(MediMessage obj, BecWriter writer, GeneratedSerializationContext context)
            {
                this.Destination.Serialize(obj.Destination, writer, context);
                this.Payload.Serialize(obj.Payload, writer, context);
                this.Source.Serialize(obj.Source, writer, context);
            }

            public override MediMessage Deserialize(BecReader reader, GeneratedSerializationContext context)
            {
                MediMessage obj = new MediMessage();
                obj.Destination = this.Destination.Deserialize(reader, context);
                obj.Payload = this.Payload.Deserialize(reader, context);
                obj.Source = this.Source.Deserialize(reader, context);
                return obj;
            }
        }

        private class MediAddressSerializer : GeneratedSchemaSerializerBase<MediAddress>
        {
            public override void Serialize(MediAddress obj, BecWriter writer, GeneratedSerializationContext context)
            {
                writer.WriteString(obj.Application);
                writer.WriteString(obj.Unit);
            }

            public override MediAddress Deserialize(BecReader reader, GeneratedSerializationContext context)
            {
                MediAddress obj = new MediAddress();
                obj.Application = reader.ReadString();
                obj.Unit = reader.ReadString();
                return obj;
            }
        }

        private class BecSchemaSerializer : GeneratedSchemaSerializerBase<BecSchema>
        {
            public GeneratedSchemaSerializerBase<List<SchemaMember>> Members;

            public GeneratedSchemaSerializerBase<TypeName> Type;

            public override void Serialize(BecSchema obj, BecWriter writer, GeneratedSerializationContext context)
            {
                this.Members.Serialize(obj.Members, writer, context);
                this.Type.Serialize(obj.Type, writer, context);
            }

            public override BecSchema Deserialize(BecReader reader, GeneratedSerializationContext context)
            {
                BecSchema obj = new BecSchema();
                obj.Members = this.Members.Deserialize(reader, context);
                obj.Type = this.Type.Deserialize(reader, context);
                return obj;
            }
        }

        private class IntSerializer : GeneratedSchemaSerializerBase<int>, IBuiltInSchemaSerializer
        {
            public override void Serialize(int obj, BecWriter writer, GeneratedSerializationContext context)
            {
                writer.WriteInt32(obj);
            }

            public override int Deserialize(BecReader reader, GeneratedSerializationContext context)
            {
                return reader.ReadInt32();
            }
        }
        */
    }
}
