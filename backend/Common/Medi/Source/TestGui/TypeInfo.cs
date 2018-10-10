// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypeInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Type information used in graphical controls to render the type
    /// just with its name, not its full name.
    /// </summary>
    internal class TypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfo"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public TypeInfo(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets Type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets all implementations of a given base type.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <returns>
        /// An enumeration over all types that implement / extend the given base type.
        /// </returns>
        public static IEnumerable<TypeInfo> GetImplementations(Type baseType)
        {
            var types = new List<TypeInfo>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                    {
                        object[] attrs = type.GetCustomAttributes(typeof(ImplementationAttribute), false);
                        if (attrs.Length == 1)
                        {
                            types.Add(new TypeInfo(type));
                        }
                    }
                }
            }

            types.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Type.Name, b.Type.Name));
            return types;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Type.Name;
        }
    }
}