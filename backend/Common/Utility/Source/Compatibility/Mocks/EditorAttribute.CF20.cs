// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorAttribute.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditorAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace System.ComponentModel
{
    using System;

    /// <summary>
    /// GUI editor attribute which is not available in Compact Framework.
    /// </summary>
    public partial class EditorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        public EditorAttribute(Type type, Type baseType)
            : this(type.AssemblyQualifiedName, baseType.AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorAttribute"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        public EditorAttribute(string typeName, Type baseType)
            : this(typeName, baseType.AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorAttribute"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="baseTypeName">
        /// The base type name.
        /// </param>
        public EditorAttribute(string typeName, string baseTypeName)
        {
            this.EditorTypeName = typeName;
            this.EditorBaseTypeName = baseTypeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorAttribute"/> class.
        /// </summary>
        public EditorAttribute()
        {
            this.EditorBaseTypeName = string.Empty;
            this.EditorTypeName = string.Empty;
        }

        /// <summary>
        /// Gets the editor base type name.
        /// </summary>
        public string EditorBaseTypeName { get; private set; }

        /// <summary>
        /// Gets the editor type name.
        /// </summary>
        public string EditorTypeName { get; private set; }
    }
}

// ReSharper restore CheckNamespace