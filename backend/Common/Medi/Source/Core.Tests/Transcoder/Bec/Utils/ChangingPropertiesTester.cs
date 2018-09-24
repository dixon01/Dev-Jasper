// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangingPropertiesTester.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangingPropertiesTester type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// The tester (residing in a different AppDomain) for changing properties.
    /// </summary>
    public class ChangingPropertiesTester : MarshalByRefObject, IChangingPropertiesTester
    {
        private readonly ModuleBuilder moduleBuilder;

        private readonly Dictionary<string, BecSerializer> serializers = new Dictionary<string, BecSerializer>();

        private BecCodecConfig.SerializerType serializerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingPropertiesTester"/> class.
        /// </summary>
        public ChangingPropertiesTester()
        {
            var name = new AssemblyName { Name = "ChangingPropertiesTesters" };
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            this.moduleBuilder = asmBuilder.DefineDynamicModule(name.Name);
        }

        /// <summary>
        /// Configures this tester to use the given type of serializer.
        /// </summary>
        /// <param name="serType">
        /// The serializer type.
        /// </param>
        public void Configure(BecCodecConfig.SerializerType serType)
        {
            this.serializerType = serType;
            this.serializers.Clear();
        }

        /// <summary>
        /// Dynamically creates a class with the given name and properties.
        /// </summary>
        /// <param name="name">
        /// The full name of the class.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <returns>
        /// A wrapper around the created type.
        /// </returns>
        public ClassWrapper CreateClass(string name, params PropertyDescription[] properties)
        {
            var typeBuilder = this.moduleBuilder.DefineType(
                name, TypeAttributes.Public | TypeAttributes.Class, typeof(object));

            foreach (var propertyDescription in properties)
            {
                var property = typeBuilder.DefineProperty(
                    propertyDescription.Name,
                    PropertyAttributes.None,
                    FindType(propertyDescription.TypeName),
                    null);

                const MethodAttributes PropertyAttrs =
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                    | MethodAttributes.Virtual;

                var field = typeBuilder.DefineField(
                    "_" + property.Name, property.PropertyType, FieldAttributes.Private);

                var propertyGetter = typeBuilder.DefineMethod(
                    "get_" + property.Name,
                    PropertyAttrs,
                    property.PropertyType,
                    Type.EmptyTypes);

                var il = propertyGetter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ret);

                property.SetGetMethod(propertyGetter);

                var propertySetter = typeBuilder.DefineMethod(
                    "set_" + property.Name,
                    PropertyAttrs,
                    typeof(void),
                    new[] { property.PropertyType });

                il = propertySetter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Ret);

                property.SetSetMethod(propertySetter);
            }

            var type = typeBuilder.CreateType();
            return new ClassWrapper(type);
        }

        /// <summary>
        /// Dynamically creates an enum with the given name and values.
        /// </summary>
        /// <param name="name">
        /// The name of the enum type.
        /// </param>
        /// <param name="underlyingClass">
        /// The underlying class of the enum.
        /// </param>
        /// <param name="values">
        /// The values defined in the enum.
        /// </param>
        /// <returns>
        /// A wrapper around the generated enum.
        /// </returns>
        public ClassWrapper CreateEnum(
            string name, ClassDescription underlyingClass, params KeyValuePair<string, object>[] values)
        {
            var underlyingType = Type.GetType(underlyingClass.FullName, true);
            Debug.Assert(underlyingType != null, "Unknown base type");
            var enumBuilder = this.moduleBuilder.DefineEnum(name, TypeAttributes.Public, underlyingType);

            foreach (var pair in values)
            {
                enumBuilder.DefineLiteral(pair.Key, pair.Value);
            }

            var type = enumBuilder.CreateType();
            return new ClassWrapper(type);
        }

        /// <summary>
        /// Gets the reference to a generic class.
        /// </summary>
        /// <param name="genericType">
        /// The open generic type.
        /// </param>
        /// <param name="args">
        /// The generic arguments.
        /// </param>
        /// <returns>
        /// A wrapper around the type.
        /// </returns>
        public ClassWrapper GetGenericClass(ClassDescription genericType, params ClassDescription[] args)
        {
            var type = FindType(genericType.FullName).MakeGenericType(
                args.Select(d => FindType(d.FullName)).ToArray());
            return new ClassWrapper(type);
        }

        /// <summary>
        /// Creates an object from a dynamically created class.
        /// </summary>
        /// <param name="classWrapper">
        /// The class information created using <see cref="IChangingPropertiesTester.CreateClass"/>.
        /// </param>
        /// <returns>
        /// A wrapper around the created object.
        /// </returns>
        public ObjectWrapper CreateObject(ClassWrapper classWrapper)
        {
            return new ObjectWrapper(Activator.CreateInstance(classWrapper.GetWrappedType()));
        }

        /// <summary>
        /// Serializes the given object to a byte array.
        /// </summary>
        /// <param name="target">
        /// A unique identification for the target application.
        /// This is used to determine which serializer should be used.
        /// </param>
        /// <param name="wrapper">
        /// The object to be serialized.
        /// </param>
        /// <returns>
        /// The serialized data.
        /// </returns>
        public byte[] Serialize(string target, ObjectWrapper wrapper)
        {
            var memory = new MemoryStream();
            var serializer = this.GetSerializer(target);
            foreach (var buffer in serializer.Serialize(wrapper.GetObject()))
            {
                memory.Write(buffer.Buffer, buffer.Offset, buffer.Count);
            }

            return memory.ToArray();
        }

        /// <summary>
        /// Deserializes an object from the given data.
        /// </summary>
        /// <param name="source">
        /// A unique identification for the source application.
        /// This is used to determine which serializer should be used.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset into the data.
        /// </param>
        /// <param name="count">
        /// The number of valid bytes in data (taken from <see cref="offset"/>).
        /// </param>
        /// <returns>
        /// A wrapper around the deserialized object.
        /// </returns>
        public ObjectWrapper Deserialize(string source, byte[] data, int offset, int count)
        {
            var buffer = new MessageBuffer(data, offset, count);
            var serializer = this.GetSerializer(source);
            while (buffer.Count > 0)
            {
                var result = serializer.Deserialize(buffer);
                if (result != null)
                {
                    if (buffer.Count > 0)
                    {
                        throw new IOException(
                            "Couldn't deserialize object: data left after deserialization: " + buffer.Count);
                    }

                    return new ObjectWrapper(result);
                }
            }

            throw new IOException("Couldn't deserialize object: no object found");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            AppDomain.Unload(AppDomain.CurrentDomain);
        }

        private static Type FindType(string typeName)
        {
            var type = TypeFactory.Instance[typeName];
            if (type != null)
            {
                return type;
            }

            throw new TypeLoadException("Couldn't find type for " + typeName);
        }

        private BecSerializer GetSerializer(string name)
        {
            BecSerializer serializer;
            if (this.serializers.TryGetValue(name, out serializer))
            {
                return serializer;
            }

            serializer = new BecSerializer(new BecCodecConfig { Serializer = this.serializerType }, null, 1);
            this.serializers.Add(name, serializer);
            return serializer;
        }
    }
}