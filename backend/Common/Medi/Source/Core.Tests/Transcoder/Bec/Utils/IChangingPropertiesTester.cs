// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangingPropertiesTester.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IChangingPropertiesTester type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec;

    /// <summary>
    /// Interface to a tester (residing in a different AppDomain) for changing properties.
    /// </summary>
    public interface IChangingPropertiesTester : IDisposable
    {
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
        ClassWrapper CreateClass(string name, params PropertyDescription[] properties);

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
        ClassWrapper CreateEnum(
            string name, ClassDescription underlyingClass, params KeyValuePair<string, object>[] values);

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
        ClassWrapper GetGenericClass(ClassDescription genericType, params ClassDescription[] args);

        /// <summary>
        /// Creates an object from a dynamically created class.
        /// </summary>
        /// <param name="classWrapper">
        /// The class information created using <see cref="CreateClass"/>.
        /// </param>
        /// <returns>
        /// A wrapper around the created object.
        /// </returns>
        ObjectWrapper CreateObject(ClassWrapper classWrapper);

        /// <summary>
        /// Configures this tester to use the given type of serializer.
        /// </summary>
        /// <param name="serializerType">
        /// The serializer type.
        /// </param>
        void Configure(BecCodecConfig.SerializerType serializerType);

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
        byte[] Serialize(string target, ObjectWrapper wrapper);

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
        ObjectWrapper Deserialize(string source, byte[] data, int offset, int count);
    }
}