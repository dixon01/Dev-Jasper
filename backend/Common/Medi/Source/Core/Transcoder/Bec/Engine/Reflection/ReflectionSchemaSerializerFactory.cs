// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionSchemaSerializerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReflectionSchemaSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Serializer factory for serializer classes using reflection.
    /// </summary>
    internal class ReflectionSchemaSerializerFactory : SchemaSerializerFactory<ISchemaSerializer>
    {
        private static readonly Dictionary<Type, ISchemaSerializer> BuiltInSerializers;

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Readability is better with single line statements than with 120 char limit.")]
        static ReflectionSchemaSerializerFactory()
        {
            BuiltInSerializers = new Dictionary<Type, ISchemaSerializer>
            {
                // signed
                { typeof(sbyte), new BuiltInSchemaSerializer<sbyte>(r => r.ReadSByte(), (w, v) => w.WriteSByte(v)) },
                { typeof(short), new BuiltInSchemaSerializer<short>(r => r.ReadInt16(), (w, v) => w.WriteInt16(v)) },
                { typeof(int), new BuiltInSchemaSerializer<int>(r => r.ReadInt32(), (w, v) => w.WriteInt32(v)) },
                { typeof(long), new BuiltInSchemaSerializer<long>(r => r.ReadInt64(), (w, v) => w.WriteInt64(v)) },

                // unsigned
                { typeof(byte), new BuiltInSchemaSerializer<byte>(r => r.ReadByte(), (w, v) => w.WriteByte(v)) },
                { typeof(ushort), new BuiltInSchemaSerializer<ushort>(r => r.ReadUInt16(), (w, v) => w.WriteUInt16(v)) },
                { typeof(uint), new BuiltInSchemaSerializer<uint>(r => r.ReadUInt32(), (w, v) => w.WriteUInt32(v)) },
                { typeof(ulong), new BuiltInSchemaSerializer<ulong>(r => r.ReadUInt64(), (w, v) => w.WriteUInt64(v)) },

                // floating point
                { typeof(float), new BuiltInSchemaSerializer<float>(r => r.ReadSingle(), (w, v) => w.WriteSingle(v)) },
                { typeof(double), new BuiltInSchemaSerializer<double>(r => r.ReadDouble(), (w, v) => w.WriteDouble(v)) },

                // bool
                { typeof(bool), new BuiltInSchemaSerializer<bool>(r => r.ReadBool(), (w, v) => w.WriteBool(v)) },

                // char/string
                { typeof(char), new BuiltInSchemaSerializer<char>(r => r.ReadChar(), (w, v) => w.WriteChar(v)) },
                { typeof(string), new BuiltInSchemaSerializer<string>(r => r.ReadString(), (w, v) => w.WriteString(v)) },

                // byte[]
                { typeof(byte[]), new BuiltInSchemaSerializer<byte[]>(r => r.ReadBytes(), (w, v) => w.WriteBytes(v)) },

                // Type
                { typeof(Type), new BuiltInSchemaSerializer<Type>(
                    r =>
                        {
                            var name = r.ReadString();
                            return name == null ? null : TypeFactory.Instance[name];
                        },
                    (w, v) => w.WriteString(v != null ? v.FullName : null)) },

                // Guid
                { typeof(Guid), new BuiltInSchemaSerializer<Guid>(r => new Guid(r.ReadBytes()), (w, v) => w.WriteBytes(v.ToByteArray())) },

                // DateTime
                { typeof(DateTime), new BuiltInSchemaSerializer<DateTime>(
                    r =>
                        {
                            var ticks = r.ReadInt64();
                            var kind = r.ReadByte();
                            return new DateTime(ticks, (DateTimeKind)kind);
                        },
                    (w, v) =>
                        {
                            w.WriteInt64(v.Ticks);
                            w.WriteByte((byte)v.Kind);
                        }) },
            };
        }

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
        protected override ISchemaSerializer CreateBecSchemaSerializer(BecSchema schema, SerializationContext context)
        {
            var serializer = new BecSchemaSerializer(schema);
            var memberInfos = new Dictionary<string, SchemaMemberInfo>();
            Debug.Assert(schema.TypeName.IsKnown, "Can't create a BecSchemaSerializer for an unknown type");

            foreach (var memberInfo in SchemaMemberInfo.GetMembers(schema.TypeName.Type))
            {
                memberInfos.Add(memberInfo.Name, memberInfo);
            }

            foreach (var member in schema.Members)
            {
                SchemaMemberInfo memberInfo;
                memberInfos.TryGetValue(member.Name, out memberInfo);
                serializer.Members.Add(this.CreateMemberSerializer(member, memberInfo, context));
            }

            return serializer;
        }

        /// <summary>
        /// Creates a serializer for types implementing <see cref="IBecSerializable"/>.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <returns>
        /// The serializer.
        /// </returns>
        protected override ISchemaSerializer CreateExplicitSchemaSerializer(BecSchema schema)
        {
            return new ExplicitSchemaSerializer(schema);
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
        protected override ISchemaSerializer GetBuiltInSerializer(BuiltInTypeSchema primitive)
        {
            Type type = primitive.TypeName.Type;
            if (type.IsEnum)
            {
                type = type.GetField("value__").FieldType;
            }

            return BuiltInSerializers[type];
        }

        /// <summary>
        /// Gets a serializer for all types not handled by another serializer.
        /// </summary>
        /// <returns>
        /// The <see cref="DefaultTypeSerializer.Instance"/>.
        /// </returns>
        protected override ISchemaSerializer GetDefaultSerializer()
        {
            return DefaultTypeSerializer.Instance;
        }

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
        /// A <see cref="ListTypeSerializer"/> for the given schema.
        /// </returns>
        protected override ISchemaSerializer CreateListSerializer(ListTypeSchema list, SerializationContext context)
        {
            if (!list.TypeName.IsKnown)
            {
                return new UnknownListSchemaSerializer(list, this.GetSchemaSerializer(list.ItemSchema, context));
            }

            return new ListTypeSerializer(list, this.GetSchemaSerializer(list.ItemSchema, context));
        }

        private SchemaMemberSerializer CreateMemberSerializer(
            SchemaMember member, SchemaMemberInfo memberInfo, SerializationContext context)
        {
            var serializer = this.GetSchemaSerializer(member.Schema, context);

            if (memberInfo == null)
            {
                return new SchemaMemberSerializer(serializer);
            }

            var propertyInfo = memberInfo.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                return new SchemaMemberSerializer(propertyInfo, serializer);
            }

            var fieldInfo = memberInfo.Member as FieldInfo;
            return new SchemaMemberSerializer(fieldInfo, serializer);
        }
    }
}
