// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaSerializerFactory{TSerializerBase}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaSerializerFactory{TSerializerBase} type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Base class for factories for schema serializer objects.
    /// </summary>
    /// <typeparam name="TSerializerBase">
    /// The base class for all serializer objects returned by this factory.
    /// </typeparam>
    internal abstract class SchemaSerializerFactory<TSerializerBase> : SchemaSerializerFactory
        where TSerializerBase : class, ISchemaSerializer
    {
        private readonly Dictionary<TypeName, TSerializerBase> listTypeSerializers =
            new Dictionary<TypeName, TSerializerBase>();

        /// <summary>
        /// Creates a serializer for a given schema information.
        /// </summary>
        /// <param name="schemaInfo">
        /// The schema information.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// A serializer for the given schema.
        /// </returns>
        public override ISchemaSerializer CreateSchemaSerializer(SchemaInfo schemaInfo, SerializationContext context)
        {
            var becSchema = schemaInfo.Schema as BecSchema;
            if (becSchema == null)
            {
                return this.GetSchemaSerializer(schemaInfo.Schema, context);
            }

            if (!becSchema.TypeName.IsKnown)
            {
                var serializer = this.CreateUnknownSchemaSerializer(becSchema, context);
                return new PossibleUnknownSchemaSerializer(becSchema, null, serializer, this);
            }

            if (!typeof(IBecSerializable).IsAssignableFrom(becSchema.TypeName.Type))
            {
                var serializer = this.CreateBecSchemaSerializer(becSchema, context);
                return new PossibleUnknownSchemaSerializer(becSchema, serializer, null, this);
            }

            return this.CreateExplicitSchemaSerializer(becSchema);
        }

        /// <summary>
        /// Creates a serializer for an unknown type (i.e. one that
        /// does not exist on this node, but does exist on the source and the
        /// destination of the message).
        /// </summary>
        /// <param name="becSchema">
        /// The BEC schema of the unknown type.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// A new <see cref="UnknownSchemaSerializer"/>.
        /// </returns>
        protected virtual ISchemaSerializer CreateUnknownSchemaSerializer(
            BecSchema becSchema, SerializationContext context)
        {
            var serializer = new UnknownSchemaSerializer(becSchema);
            foreach (var member in becSchema.Members)
            {
                var memberSerializer = new UnknownSchemaMemberSerializer(member.Name);
                memberSerializer.Serializer = this.GetSchemaSerializer(member.Schema, context);
                serializer.Members.Add(memberSerializer);
            }

            return serializer;
        }

        /// <summary>
        /// Crates a serializer for the given type schema.
        /// </summary>
        /// <param name="typeSchema">
        /// The type schema.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// A new or a cached serializer for the given type schema.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// If the type schema is of a type not supported by this serializer factory.
        /// </exception>
        protected virtual TSerializerBase GetSchemaSerializer(ITypeSchema typeSchema, SerializationContext context)
        {
            var builtIn = typeSchema as BuiltInTypeSchema;
            if (builtIn != null)
            {
                return this.GetBuiltInSerializer(builtIn);
            }

            var enumSchema = typeSchema as EnumTypeSchema;
            if (enumSchema != null)
            {
                return this.GetBuiltInSerializer(enumSchema.UnderlyingSchema);
            }

            var schema = typeSchema as BecSchema;
            if (schema != null)
            {
                // Get the serializer from the engine, like this the schema gets registered.
                // Otherwise we create serializers without registering the type (and then the
                // peer doesn't know the type when deserializing).
                return (TSerializerBase)context.Engine.GetSchemaInfo(schema, context).Serializer;
            }

            var def = typeSchema as DefaultTypeSchema;
            if (def != null)
            {
                return this.GetDefaultSerializer();
            }

            var list = typeSchema as ListTypeSchema;
            if (list != null)
            {
                TSerializerBase serializer;
                if (!this.listTypeSerializers.TryGetValue(list.TypeName, out serializer))
                {
                    serializer = this.CreateListSerializer(list, context);
                    this.listTypeSerializers.Add(list.TypeName, serializer);
                }

                return serializer;
            }

            throw new NotSupportedException(
                typeSchema.GetType().FullName + " does not have a corresponding serializer.");
        }

        /// <summary>
        /// Gets a serializer for a built-in type (primitives, <see cref="string"/> or <see cref="Type"/>.
        /// </summary>
        /// <param name="primitive">
        /// The schema of the built-in type.
        /// </param>
        /// <returns>
        /// Serializer for the given schema.
        /// </returns>
        protected abstract TSerializerBase GetBuiltInSerializer(BuiltInTypeSchema primitive);

        /// <summary>
        /// Gets a serializer for all types not handled by another serializer.
        /// </summary>
        /// <returns>
        /// The default serializer.
        /// </returns>
        protected abstract TSerializerBase GetDefaultSerializer();

        /// <summary>
        /// Creates a serializer for a <see cref="BecSchema"/>.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// A generated object that can (de)serialize the type defined in the schema.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// If the type doesn't have an empty public constructor.
        /// </exception>
        protected abstract TSerializerBase CreateBecSchemaSerializer(BecSchema schema, SerializationContext context);

        /// <summary>
        /// Creates a serializer for types implementing <see cref="IBecSerializable"/>.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <returns>
        /// The serializer.
        /// </returns>
        protected abstract TSerializerBase CreateExplicitSchemaSerializer(BecSchema schema);

        /// <summary>
        /// Creates a serializer for a list or array.
        /// </summary>
        /// <param name="list">
        /// The list or array schema.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// A list or array serializer for the given schema.
        /// </returns>
        protected abstract TSerializerBase CreateListSerializer(ListTypeSchema list, SerializationContext context);

        /// <summary>
        /// This class is used as a wrapper around the known and unknown serializer objects.
        /// It solves the issue that sometimes a type becomes "known" after the serializer is already created.
        /// This mainly happens when the application is receiving unknown messages while starting and during
        /// start-up it later loads the assembly of the (previously) unknown type.
        /// </summary>
        private class PossibleUnknownSchemaSerializer : ISchemaSerializer
        {
            private readonly BecSchema schema;

            private readonly SchemaSerializerFactory<TSerializerBase> owner;

            private ISchemaSerializer known;

            private ISchemaSerializer unknown;

            public PossibleUnknownSchemaSerializer(
                BecSchema schema,
                ISchemaSerializer known,
                ISchemaSerializer unknown,
                SchemaSerializerFactory<TSerializerBase> owner)
            {
                this.schema = schema;
                this.known = known;
                this.unknown = unknown;
                this.owner = owner;
            }

            public void Serialize(object obj, BecWriter writer, SerializationContext context)
            {
                if (obj is UnknownBecObject)
                {
                    if (this.unknown == null)
                    {
                        this.unknown = this.owner.CreateUnknownSchemaSerializer(this.schema, context);
                    }

                    this.unknown.Serialize(obj, writer, context);
                    return;
                }

                if (this.known == null)
                {
                    var knownSchema =
                        SchemaFactory.Instance.GetSchema(new TypeName(this.schema.TypeName.FullName).Type);
                    this.known = this.owner.CreateBecSchemaSerializer((BecSchema)knownSchema, context);
                }

                this.known.Serialize(obj, writer, context);
            }

            public object Deserialize(BecReader reader, SerializationContext context)
            {
                if (this.known != null)
                {
                    return this.known.Deserialize(reader, context);
                }

                return this.unknown.Deserialize(reader, context);
            }
        }
    }
}
