// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangingPropertiesTesterWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangingPropertiesTesterWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Tests.Utils;
    using Gorba.Common.Medi.Core.Transcoder.Bec;

    /// <summary>
    /// Wrapper of an <see cref="IChangingPropertiesTester"/> residing in a different AppDomain.
    /// </summary>
    internal class ChangingPropertiesTesterWrapper : IChangingPropertiesTester
    {
        private readonly AppDomain appDomain;

        private readonly ChangingPropertiesTester wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingPropertiesTesterWrapper"/> class.
        /// </summary>
        /// <param name="appDomain">
        /// The app domain on which to create the actual tester.
        /// </param>
        public ChangingPropertiesTesterWrapper(AppDomain appDomain)
        {
            this.appDomain = appDomain;
            this.wrapped = appDomain.CreateInstanceAndUnwrap<ChangingPropertiesTester>();
        }

        /// <summary>
        /// Configures this tester to use the given type of serializer.
        /// </summary>
        /// <param name="serializerType">
        /// The serializer type.
        /// </param>
        public void Configure(BecCodecConfig.SerializerType serializerType)
        {
            this.wrapped.Configure(serializerType);
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
            return this.wrapped.CreateClass(name, properties);
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
            return this.wrapped.CreateEnum(name, underlyingClass, values);
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
            return this.wrapped.GetGenericClass(genericType, args);
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
            return this.wrapped.CreateObject(classWrapper);
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
            return this.wrapped.Serialize(target, wrapper);
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
            return this.wrapped.Deserialize(source, data, offset, count);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            AppDomain.Unload(this.appDomain);
        }
    }
}