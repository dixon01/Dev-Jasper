// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Serializer for BEC (Binary Enhanced Coding).
    /// </summary>
    internal class BecSerializer : IManageable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<BecSerializer>();

        private readonly BecCodecConfig config;

        private readonly IFrameController frameController;

        private readonly SerializationEngine readEngine;
        private readonly SerializationEngine writeEngine;

        private readonly Mapper<string> readStringMapper = new StringMapper();
        private readonly Mapper<SchemaInfo> readSchemaMapper = new SchemaMapper();
        private readonly Mapper<string> writeStringMapper = new StringMapper();
        private readonly Mapper<SchemaInfo> writeSchemaMapper = new SchemaMapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="BecSerializer"/> class
        /// with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        /// <param name="frameController">
        /// The frame controller used for verifying frame IDs. This can be null.
        /// </param>
        /// <param name="version">
        /// The codec version.
        /// </param>
        public BecSerializer(BecCodecConfig config, IFrameController frameController, int version)
        {
            this.config = config;
            this.frameController = frameController;

            this.readEngine = new SerializationEngine(this.config, version);
            this.writeEngine = new SerializationEngine(this.config, version);

            this.readStringMapper.Peer = this.writeStringMapper;
            this.writeStringMapper.Peer = this.readStringMapper;

            this.readSchemaMapper.Peer = this.writeSchemaMapper;
            this.writeSchemaMapper.Peer = this.readSchemaMapper;

            AddMetaSchema(new SerializationContext(this.readEngine, this.readStringMapper, this.readSchemaMapper));
            AddMetaSchema(new SerializationContext(this.writeEngine, this.writeStringMapper, this.writeSchemaMapper));
        }

        /// <summary>
        /// Deserializes an object from the given buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer that contains a serialized object.
        /// </param>
        /// <returns>
        /// The object or null if the buffer didn't contain a
        /// whole object or if it was a message internal to
        /// the serializer.
        /// </returns>
        public object Deserialize(MessageBuffer buffer)
        {
            var input = buffer.OpenRead(0);
            var reader = new BecReader(input, this.readStringMapper);

            var obj = this.readEngine.Deserialize(reader, this.readStringMapper, this.readSchemaMapper);

            FrameInfo frame = null;
            if (this.frameController != null)
            {
                var frameId = reader.ReadUInt32();
                var ackId = reader.ReadUInt32();
                frame = new FrameInfo(frameId, ackId);
            }

            buffer.Remove((int)input.Position);

            Logger.Debug("Deserialized {0}: {1}", obj.GetType().Name, obj);

            if (frame != null)
            {
                switch (this.frameController.VerifyIncoming(frame))
                {
                    case FrameCheck.MissingFrame:
                        throw new IOException("Got a frame with wrong frame number");
                    case FrameCheck.DuplicateFrame:
                        // ignore this frame
                        return null;
                }
            }

            var mapping = obj as StringMappingMessage;
            if (mapping != null)
            {
                var mapper = new TransactionMapper<string>(this.readStringMapper);
                foreach (var change in mapping.Mappings)
                {
                    mapper.Changes.Add(change.Key, change.Value);
                }

                mapper.Commit();
                return null;
            }

            var schemaMsg = obj as SchemaMessage;
            if (schemaMsg != null)
            {
                var context = new SerializationContext(this.readEngine, this.readStringMapper, this.readSchemaMapper);
                this.readSchemaMapper.Add(
                    schemaMsg.Id,
                    this.readEngine.CreateSchemaInfo(schemaMsg.Id, schemaMsg.Schema, context));
                return null;
            }

            return obj;
        }

        /// <summary>
        /// Serializes an object to an enumeration of buffers.
        /// </summary>
        /// <param name="obj">
        /// The object to be serialized.
        /// </param>
        /// <returns>
        /// An enumeration of buffers. This can be one or more
        /// buffers, which might contain internal messages.
        /// </returns>
        public IEnumerable<SendMessageBuffer> Serialize(object obj)
        {
            var strings = new TransactionMapper<string>(this.writeStringMapper);
            var schemas = new TransactionMapper<SchemaInfo>(this.writeSchemaMapper);

            Logger.Debug("Serializing {0}: {1}", obj.GetType().Name, obj);

            var buffer = this.Serialize(obj, strings, schemas);

            List<OutputBuffer> schemaMessages = null;

            if (schemas.HasChanges)
            {
                schemaMessages = new List<OutputBuffer>(schemas.Changes.Count);
                foreach (var change in schemas.Changes)
                {
                    var schemaMessage = new SchemaMessage { Id = change.Value, Schema = change.Key.Schema };
                    schemaMessages.Add(this.Serialize(schemaMessage, strings, null));
                }
            }

            if (strings.HasChanges)
            {
                yield return
                    this.Serialize(new StringMappingMessage { Mappings = strings.Changes }, null, null)
                        .ToMessageBuffer(this.frameController);

                strings.Commit();
            }

            if (schemaMessages != null)
            {
                foreach (var schemaMessage in schemaMessages)
                {
                    yield return schemaMessage.ToMessageBuffer(this.frameController);
                }

                schemas.Commit();
            }

            if (buffer != null)
            {
                yield return buffer.ToMessageBuffer(this.frameController);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("ReadStringMapper", parent, this.readStringMapper);
            yield return parent.Factory.CreateManagementProvider("WriteStringMapper", parent, this.writeStringMapper);
        }

        private static void AddMetaSchema(SerializationContext context)
        {
            // add all classes from the Gorba.Common.Medi.Core.Transcoder.Bec.Schema namespace,
            // so we don't have to send meta data of meta data over the protocol
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(TypeName)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(BecSchema)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(BuiltInTypeSchema)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(DefaultTypeSchema)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(ListTypeSchema)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(EnumTypeSchema)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(SchemaMember)), context);

            // also add our two internal messages
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(SchemaMessage)), context);
            context.Engine.CreateSchemaInfo(0, SchemaFactory.Instance.GetSchema(typeof(StringMappingMessage)), context);
        }

        private OutputBuffer Serialize(object obj, IMapper<string> strings, IMapper<SchemaInfo> schemas)
        {
            var output = new OutputBuffer(strings);

            this.writeEngine.Serialize(output.Writer, obj, strings, schemas);

            return output;
        }

        private class OutputBuffer
        {
            private readonly MemoryStream memory;

            public OutputBuffer(IMapper<string> strings)
            {
                this.memory = new MemoryStream();
                this.Writer = new BecWriter(this.memory, strings);
            }

            public BecWriter Writer { get; private set; }

            public SendMessageBuffer ToMessageBuffer(IFrameController frameController)
            {
                uint frameId = 0;
                if (frameController != null)
                {
                    var frame = frameController.GetNextFrameInfo();
                    frameId = frame.SendFrameId;
                    this.Writer.WriteUInt32(frame.SendFrameId);
                    this.Writer.WriteUInt32(frame.AckFrameId);
                }

                return new SendMessageBuffer(frameId, this.memory.ToArray(), 0, (int)this.memory.Position);
            }
        }
    }
}