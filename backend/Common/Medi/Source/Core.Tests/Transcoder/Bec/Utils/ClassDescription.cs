// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassDescription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClassDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;

    /// <summary>
    /// Description of a class.
    /// </summary>
    [Serializable]
    public class ClassDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDescription"/> class.
        /// </summary>
        /// <param name="type">
        /// The type to be described by this object.
        /// </param>
        public ClassDescription(Type type)
        {
            this.FullName = type.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDescription"/> class.
        /// </summary>
        /// <param name="wrapper">
        /// The type to be described by this object.
        /// </param>
        public ClassDescription(ClassWrapper wrapper)
        {
            this.FullName = wrapper.TypeName;
        }

        /// <summary>
        /// Gets the full type name.
        /// </summary>
        public string FullName { get; private set; }
    }
}