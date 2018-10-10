// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDescription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;

    /// <summary>
    /// Description of a dynamically created property.
    /// </summary>
    [Serializable]
    public class PropertyDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescription"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="type">
        /// The type of the property.
        /// </param>
        public PropertyDescription(string name, Type type)
        {
            this.Name = name;
            this.TypeName = type.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescription"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="type">
        /// The type of the property.
        /// </param>
        public PropertyDescription(string name, ClassWrapper type)
        {
            this.Name = name;
            this.TypeName = type.TypeName;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the full type name.
        /// </summary>
        public string TypeName { get; private set; }
    }
}