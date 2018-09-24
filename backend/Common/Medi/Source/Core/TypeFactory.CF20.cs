// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
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
            if (type.LastIndexOf(',') >= 0)
            {
                return null;
            }

            var index = 0;
            while ((index = type.IndexOf('.', index + 1)) > 0)
            {
                var dllName = type.Substring(0, index);
                var t = this[type + ", " + dllName];
                if (t != null)
                {
                    return t;
                }

                t = this[type + ", " + dllName + ".CF35"];
                if (t != null)
                {
                    return t;
                }
            }

            // ugly but necessary since those types are in the wrong DLL (compared to their namespace)
            if (type.StartsWith("Gorba.Motion.SystemManager.ServiceModel"))
            {
                var t = this[type + ", Gorba.Common.SystemManagement.Messages.CF35"];
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
