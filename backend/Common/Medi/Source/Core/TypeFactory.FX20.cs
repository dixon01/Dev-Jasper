// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;

    /// <summary>
    /// Utility class that caches types for a given name and searches for
    /// the type in all loaded assemblies.
    /// </summary>
    internal partial class TypeFactory
    {
        private Type FindForeignType(string type)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(type, false, true);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
