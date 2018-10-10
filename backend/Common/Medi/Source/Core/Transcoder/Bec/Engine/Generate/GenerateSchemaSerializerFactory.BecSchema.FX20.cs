// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateSchemaSerializerFactory.BecSchema.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the part of the GenerateSchemaSerializerFactory type that
//   handles BecSchemas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Factory for generated serializer objects.
    /// </summary>
    internal partial class GenerateSchemaSerializerFactory
    {
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
        protected override GeneratedSchemaSerializerBase CreateBecSchemaSerializer(
            BecSchema schema, SerializationContext context)
        {
            Debug.Assert(schema.TypeName.IsKnown, "BecSchemaSerializer can only be created for a known type");

            MethodBuilder serializeBuilder;
            MethodBuilder deserializeBuilder;
            var typeBuilder = CreateTypeBuilder(
                schema.TypeName.Type, Type.EmptyTypes, out serializeBuilder, out deserializeBuilder);

            var memberInfos = new Dictionary<string, SchemaMemberInfo>();
            foreach (var memberInfo in SchemaMemberInfo.GetMembers(schema.TypeName.Type))
            {
                memberInfos.Add(memberInfo.Name, memberInfo);
            }

            var serializeIl = serializeBuilder.GetILGenerator();
            var deserializeIl = deserializeBuilder.GetILGenerator();

            var ctor = schema.TypeName.Type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new NotSupportedException(
                    schema.TypeName.FullName + " does not have a public empty constructor");
            }

            // create & populate local variable "obj" in Deserialize()
            deserializeIl.DeclareLocal(schema.TypeName.Type);
            deserializeIl.Emit(OpCodes.Newobj, ctor);
            deserializeIl.Emit(OpCodes.Stloc_0);

            var serializers = new Dictionary<string, ISchemaSerializer>();

            // serialize / deserialize all members
            foreach (var member in schema.Members)
            {
                SchemaMemberInfo memberInfo;
                memberInfos.TryGetValue(member.Name, out memberInfo);
                FieldBuilder field = null;
                var serializer = this.GetSchemaSerializer(member.Schema, context);

                // types implementing this interface are all generated at runtime, therefore ReSharper doesn't know them
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (!(serializer is IBuiltInSchemaSerializer))
                {
                    var serializerType =
                        typeof(GeneratedSchemaSerializerBase<>).MakeGenericType(member.Schema.TypeName.Type);
                    field = typeBuilder.DefineField(member.Name, serializerType, FieldAttributes.Public);

                    serializers.Add(member.Name, serializer);
                }

                this.AddMemberSerialization(serializeIl, field, member, memberInfo);
                this.AddMemberDeserialization(deserializeIl, field, member, memberInfo);
            }

            // return "obj" in Deserialize()
            deserializeIl.Emit(OpCodes.Ldloc_0);
            deserializeIl.Emit(OpCodes.Ret);

            // return void in Serialize()
            serializeIl.Emit(OpCodes.Ret);

            var type = typeBuilder.CreateType();
            var typeSerializer = Activator.CreateInstance(type) as GeneratedSchemaSerializerBase;

            // set all serializer fields
            foreach (var pair in serializers)
            {
                type.GetField(pair.Key).SetValue(typeSerializer, pair.Value);
            }

            return typeSerializer;
        }

        private static TypeBuilder CreateTypeBuilder(
            Type objectType, Type[] interfaces, out MethodBuilder serialize, out MethodBuilder deserialize)
        {
            var baseType = typeof(GeneratedSchemaSerializerBase<>).MakeGenericType(objectType);
            TypeBuilder typeBuilder;

            for (int i = 0;; i++)
            {
                try
                {
                    typeBuilder = Builder.DefineType(
                        string.Format("$.{0}Serializer{1}", objectType.FullName, i),
                        TypeAttributes.Public | TypeAttributes.Class,
                        baseType,
                        interfaces);
                    break;
                }
                catch (ArgumentException)
                {
                    // ignore and try again
                    continue;
                }
            }

            serialize = typeBuilder.DefineMethod(
                "Serialize",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new[] { objectType, typeof(BecWriter), typeof(GeneratedSerializationContext) });

            deserialize = typeBuilder.DefineMethod(
                "Deserialize",
                MethodAttributes.Public | MethodAttributes.Virtual,
                objectType,
                new[] { typeof(BecReader), typeof(GeneratedSerializationContext) });

            return typeBuilder;
        }

        private void AddMemberSerialization(
            ILGenerator il, FieldBuilder field, SchemaMember member, SchemaMemberInfo memberInfo)
        {
            if (field == null)
            {
                // load writer onto stack
                il.Emit(OpCodes.Ldarg_2); // writer
            }
            else
            {
                // load serializer stored in field onto stack
                il.Emit(OpCodes.Ldarg_0); // "this"
                il.Emit(OpCodes.Ldfld, field);
            }

            if (memberInfo == null)
            {
                il.Emit(OpCodes.Ldnull); // null
            }
            else
            {
                var propertyInfo = memberInfo.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    il.Emit(OpCodes.Ldarg_1); // obj
                    il.Emit(OpCodes.Callvirt, propertyInfo.GetGetMethod()); // get the property
                }
                else
                {
                    var fieldInfo = (FieldInfo)memberInfo.Member;
                    il.Emit(OpCodes.Ldarg_1); // obj
                    il.Emit(OpCodes.Ldfld, fieldInfo); // get the field
                }
            }

            if (field == null)
            {
                // built-in type, writer to BecWriter previously loaded onto stack (see above)
                var methods = GetBuiltInMethod(member.Schema.TypeName.Type);

                // we can use Call since BecWriter.WriteXxx() is not a virtual method
                il.Emit(OpCodes.Call, methods.WriteMethod);
            }
            else
            {
                // custom type, use serializer previously loaded onto stack (see above)
                il.Emit(OpCodes.Ldarg_2); // writer
                il.Emit(OpCodes.Ldarg_3); // context

                var method = typeof(GeneratedSchemaSerializerBase<>).MakeGenericType(member.Schema.TypeName.Type)
                                                                    .GetMethod("Serialize");
                il.Emit(OpCodes.Callvirt, method);
            }
        }

        private void AddMemberDeserialization(
            ILGenerator il, FieldInfo field, SchemaMember member, SchemaMemberInfo memberInfo)
        {
            if (field == null)
            {
                // built-in type
                var methods = GetBuiltInMethod(member.Schema.TypeName.Type);

                il.Emit(OpCodes.Ldloc_0); // obj
                il.Emit(OpCodes.Ldarg_1); // reader

                // we can use Call since BecReader.ReadXxx() is not a virtual method
                il.Emit(OpCodes.Call, methods.ReadMethod);
            }
            else
            {
                // custom type, use serializer stored in field
                il.Emit(OpCodes.Ldloc_0); // obj
                il.Emit(OpCodes.Ldarg_0); // "this"
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ldarg_1); // reader
                il.Emit(OpCodes.Ldarg_2); // context

                var method = typeof(GeneratedSchemaSerializerBase<>).MakeGenericType(member.Schema.TypeName.Type)
                                                                    .GetMethod("Deserialize");
                il.Emit(OpCodes.Callvirt, method); // call Deserialize()
            }

            if (memberInfo == null)
            {
                il.Emit(OpCodes.Pop); // return value from ReadXxx()/Deserialize()
                il.Emit(OpCodes.Pop); // obj
                return;
            }

            var propertyInfo = memberInfo.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                il.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod()); // set the property
            }
            else
            {
                var fieldInfo = (FieldInfo)memberInfo.Member;
                il.Emit(OpCodes.Stfld, fieldInfo); // set the field
            }
        }
    }
}
