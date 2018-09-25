// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerializationEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The serialization engine for BEC (Binary Enhanced Coding).
    /// </summary>
    internal class SerializationEngine
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SerializationEngine>();

        private readonly Dictionary<TypeName, SchemaInfo> schemaInfos = new Dictionary<TypeName, SchemaInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationEngine"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        /// <param name="version">
        /// The codec version number.
        /// </param>
        public SerializationEngine(BecCodecConfig config, int version)
        {
            // TODO: use version number later to enable/disable features
            this.Factory = SchemaSerializerFactory.Create(config.Serializer);
        }

        /// <summary>
        /// Gets the factory for serializer objects.
        /// </summary>
        public SchemaSerializerFactory Factory { get; private set; }

        /// <summary>
        /// Deserializes an object from the given buffer.
        /// </summary>
        /// <param name="reader">
        /// The BEC reader to read the object from.
        /// </param>
        /// <param name="stringMapper">
        /// The string mapper used for mapping strings to IDs.
        /// </param>
        /// <param name="schemaMapper">
        /// The schema mapper used for mapping schema information to IDs.
        /// </param>
        /// <returns>
        /// The object or null if the buffer didn't contain a
        /// whole object or if it was a message internal to
        /// the serializer.
        /// </returns>
        public object Deserialize(BecReader reader, IMapper<string> stringMapper, IMapper<SchemaInfo> schemaMapper)
        {
            var context = new SerializationContext(this, stringMapper, schemaMapper);
            int schemaId = reader.ReadInt32();

            var schemaInfo = schemaMapper[schemaId];

            var obj = schemaInfo.Serializer.Deserialize(reader, context);

            return obj;
        }

        /// <summary>
        /// Serializes an object to an enumeration of buffers.
        /// </summary>
        /// <param name="writer">
        /// The BEC writer to write the object to.
        /// </param>
        /// <param name="obj">
        /// The object to be serialized.
        /// </param>
        /// <param name="stringMapper">
        /// The string mapper used for mapping strings to IDs.
        /// </param>
        /// <param name="schemaMapper">
        /// The schema mapper used for mapping schema information to IDs.
        /// </param>
        public void Serialize(
            BecWriter writer, object obj, IMapper<string> stringMapper, IMapper<SchemaInfo> schemaMapper)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var context = new SerializationContext(this, stringMapper, schemaMapper);

            var schemaInfo = this.GetSchemaInfo(obj, context);

            writer.WriteInt32(schemaInfo.Id);

            schemaInfo.Serializer.Serialize(obj, writer, context);

            Logger.Debug("Serialized {0}: {1}", obj.GetType().Name, obj);
        }

        /// <summary>
        /// Gets schema information for a given object.
        /// </summary>
        /// <param name="obj">
        /// The object for which you want to get schema information.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// An existing or a new schema info. This info will be cached to
        /// be returned immediately later if requested again.
        /// </returns>
        public SchemaInfo GetSchemaInfo(object obj, SerializationContext context)
        {
            SchemaInfo schemaInfo;
            var type = TypeName.GetNameFor(obj);
            if (!this.schemaInfos.TryGetValue(type, out schemaInfo))
            {
                schemaInfo = this.CreateSchemaInfo(obj, context);
                this.schemaInfos.Add(type, schemaInfo);
            }

            return schemaInfo;
        }

        /// <summary>
        /// Creates schema information for a given type schema.
        /// </summary>
        /// <param name="id">
        /// The ID to be assigned to the schema. If this is 0, the
        /// <see cref="SerializationContext.SchemaMapper"/> will be queried
        /// for a new (or existing) ID.
        /// </param>
        /// <param name="schema">
        /// The schema for which you want to create a schema info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The new schema information.
        /// </returns>
        public SchemaInfo CreateSchemaInfo(int id, ITypeSchema schema, SerializationContext context)
        {
            var schemaInfo = new SchemaInfo { Schema = schema, Id = id };

            if (id == 0)
            {
                schemaInfo.Id = context.SchemaMapper[schemaInfo];
            }

            schemaInfo.Serializer = this.Factory.CreateSchemaSerializer(schemaInfo, context);

            this.schemaInfos.Add(schema.TypeName, schemaInfo);
            return schemaInfo;
        }

        /// <summary>
        /// Gets schema information for a given schema.
        /// </summary>
        /// <param name="schema">
        /// The schema for which you want to get schema information.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// An existing or a new schema info. This info will be cached to
        /// be returned immediately later if requested again.
        /// </returns>
        public SchemaInfo GetSchemaInfo(BecSchema schema, SerializationContext context)
        {
            SchemaInfo schemaInfo;
            if (this.schemaInfos.TryGetValue(schema.TypeName, out schemaInfo))
            {
                return schemaInfo;
            }

            schemaInfo = this.CreateSchemaInfo(0, schema, context);

            // todo: is this line of code necessary?
            schemaInfo.Id = context.SchemaMapper[schemaInfo];
            return schemaInfo;
        }

        private SchemaInfo CreateSchemaInfo(object obj, SerializationContext context)
        {
            var schemaInfo = new SchemaInfo();
            var serializable = obj as IBecSerializable;
            if (serializable != null)
            {
                schemaInfo.Schema = serializable.GetSchema();
                if (schemaInfo.Schema == null)
                {
                    throw new NotSupportedException("Object did not provide a schema");
                }
            }
            else
            {
                schemaInfo.Schema = SchemaFactory.Instance.GetSchemaFor(obj);
            }

            schemaInfo.Id = context.SchemaMapper[schemaInfo];
            schemaInfo.Serializer = this.Factory.CreateSchemaSerializer(schemaInfo, context);

            return schemaInfo;
        }
    }
}