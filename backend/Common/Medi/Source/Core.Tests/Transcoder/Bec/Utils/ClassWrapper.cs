// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClassWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils
{
    using System;

    /// <summary>
    /// Wrapper around a dynamically created class.
    /// </summary>
    public class ClassWrapper : MarshalByRefObject
    {
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassWrapper"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        internal ClassWrapper(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.type.FullName;
            }
        }

        /// <summary>
        /// Gets the wrapped type.
        /// This method should only be used within the same AppDomain that created the wrapper.
        /// </summary>
        /// <returns>
        /// The wrapped type.
        /// </returns>
        internal Type GetWrappedType()
        {
            return this.type;
        }
    }
}