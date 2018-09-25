// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateSchemaSerializerFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenerateSchemaSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Factory for generated serializer classes.
    /// </summary>
    internal partial class GenerateSchemaSerializerFactory : SchemaSerializerFactory<GeneratedSchemaSerializerBase>
    {
        private static readonly Dictionary<Type, BuiltInTypeMethods> BuiltInMethods;

        private static readonly ModuleBuilder Builder;

        private static readonly Dictionary<Type, GeneratedSchemaSerializerBase> SpecialBuiltInSerializers =
            new Dictionary<Type, GeneratedSchemaSerializerBase>
                {
                    { typeof(Type), new TypeSchemaSerializer() },
                    { typeof(Guid), new GuidSchemaSerializer() },
                    { typeof(DateTime), new DateTimeSchemaSerializer() }
                };

        static GenerateSchemaSerializerFactory()
        {
            var name = new AssemblyName { Name = "BecSerializers" };
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            Builder = asmBuilder.DefineDynamicModule(name.Name);

            BuiltInMethods = new Dictionary<Type, BuiltInTypeMethods>
            {
                // signed
                { typeof(sbyte), new BuiltInTypeMethods("SByte") },
                { typeof(short), new BuiltInTypeMethods("Int16") },
                { typeof(int), new BuiltInTypeMethods("Int32") },
                { typeof(long), new BuiltInTypeMethods("Int64") },

                // unsigned
                { typeof(byte), new BuiltInTypeMethods("Byte") },
                { typeof(ushort), new BuiltInTypeMethods("UInt16") },
                { typeof(uint), new BuiltInTypeMethods("UInt32") },
                { typeof(ulong), new BuiltInTypeMethods("UInt64") },

                // floating point
                { typeof(float), new BuiltInTypeMethods("Single") },
                { typeof(double), new BuiltInTypeMethods("Double") },

                // bool
                { typeof(bool), new BuiltInTypeMethods("Bool") },

                // char/string
                { typeof(char), new BuiltInTypeMethods("Char") },
                { typeof(string), new BuiltInTypeMethods("String") },

                // byte[]
                { typeof(byte[]), new BuiltInTypeMethods("Bytes") },
            };
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
        protected override GeneratedSchemaSerializerBase CreateExplicitSchemaSerializer(BecSchema schema)
        {
            var type = typeof(ExplicitSchemaSerializer<>).MakeGenericType(schema.TypeName.Type);
            var ctor = type.GetConstructor(new[] { typeof(BecSchema) });
            if (ctor == null)
            {
                return null;
            }

            return ctor.Invoke(new object[] { schema }) as GeneratedSchemaSerializerBase;
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
        protected override GeneratedSchemaSerializerBase GetBuiltInSerializer(BuiltInTypeSchema primitive)
        {
            var type = primitive.TypeName.Type;
            GeneratedSchemaSerializerBase serializer;
            if (SpecialBuiltInSerializers.TryGetValue(type, out serializer))
            {
                return serializer;
            }

            return GetBuiltInMethod(type).Serializer;
        }

        /// <summary>
        /// Gets a serializer for all types not handled by another serializer.
        /// </summary>
        /// <returns>
        /// The <see cref="DefaultSchemaSerializer.Instance"/>.
        /// </returns>
        protected override GeneratedSchemaSerializerBase GetDefaultSerializer()
        {
            return DefaultSchemaSerializer.Instance;
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
        /// An <see cref="ArraySchemaSerializer{T}"/> or an <see cref="ListSchemaSerializer{TList,TItem}"/>
        /// for the given schema.
        /// </returns>
        protected override GeneratedSchemaSerializerBase CreateListSerializer(
            ListTypeSchema list, SerializationContext context)
        {
            if (!list.TypeName.IsKnown)
            {
                return new GeneratedUnknownListSchemaSerializer(
                    list, this.GetSchemaSerializer(list.ItemSchema, context));
            }

            Type serializerType;
            if (list.TypeName.Type.IsArray)
            {
                serializerType = typeof(ArraySchemaSerializer<>).MakeGenericType(list.TypeName.Type.GetElementType());
            }
            else
            {
                // it's an IList
                serializerType = typeof(ListSchemaSerializer<,>).MakeGenericType(
                    list.TypeName.Type, list.ItemSchema.TypeName.Type);
            }

            var ctors = serializerType.GetConstructors();
            if (ctors.Length == 0)
            {
                return null;
            }

            var itemSerializer = this.GetSchemaSerializer(list.ItemSchema, context);
            return ctors[0].Invoke(new object[] { itemSerializer }) as GeneratedSchemaSerializerBase;
        }

        private static BuiltInTypeMethods GetBuiltInMethod(Type type)
        {
            if (type.IsEnum)
            {
                type = type.GetField("value__").FieldType;
            }

            return BuiltInMethods[type];
        }

        private class BuiltInTypeMethods
        {
            public BuiltInTypeMethods(string baseName)
            {
                this.ReadMethod = typeof(BecReader).GetMethod("Read" + baseName);
                this.WriteMethod = typeof(BecWriter).GetMethod("Write" + baseName);

                MethodBuilder serializeBuilder;
                MethodBuilder deserializeBuilder;
                var typeBuilder = CreateTypeBuilder(
                    this.ReadMethod.ReturnType,
                    new[] { typeof(IBuiltInSchemaSerializer) },
                    out serializeBuilder,
                    out deserializeBuilder);

                var serializeIl = serializeBuilder.GetILGenerator();
                serializeIl.Emit(OpCodes.Ldarg_2); // writer
                serializeIl.Emit(OpCodes.Ldarg_1); // value
                serializeIl.Emit(OpCodes.Call, this.WriteMethod);
                serializeIl.Emit(OpCodes.Ret);

                var deserializeIl = deserializeBuilder.GetILGenerator();
                deserializeIl.Emit(OpCodes.Ldarg_1); // reader
                deserializeIl.Emit(OpCodes.Call, this.ReadMethod);
                deserializeIl.Emit(OpCodes.Ret);

                var type = typeBuilder.CreateType();
                this.Serializer = Activator.CreateInstance(type) as GeneratedSchemaSerializerBase;
            }

            public MethodInfo ReadMethod { get; private set; }

            public MethodInfo WriteMethod { get; private set; }

            public GeneratedSchemaSerializerBase Serializer { get; private set; }
        }
    }
}
