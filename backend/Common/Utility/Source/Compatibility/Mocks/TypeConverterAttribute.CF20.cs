// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeConverterAttribute.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypeConverterAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System.ComponentModel
{
    using System;

    /// <summary>
    /// GUI editor attribute which is not available in Compact Framework.
    /// </summary>
    public partial class TypeConverterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public TypeConverterAttribute(Type type)
            : this(type.AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterAttribute"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        public TypeConverterAttribute(string typeName)
        {
            this.ConverterTypeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterAttribute"/> class.
        /// </summary>
        public TypeConverterAttribute()
        {
            this.ConverterTypeName = string.Empty;
        }

        /// <summary>
        /// Gets the converter type name.
        /// </summary>
        public string ConverterTypeName { get; private set; }
    }
}

// ReSharper restore CheckNamespace