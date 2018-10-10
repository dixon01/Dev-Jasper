// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class that caches types for a given name and searches for
    /// the type in all loaded assemblies.
    /// </summary>
    internal partial class TypeFactory
    {
        /// <summary>
        /// Single instance of this class.
        /// </summary>
        public static readonly TypeFactory Instance = new TypeFactory();

        private readonly Dictionary<string, Type> types = new Dictionary<string, Type>();

        private TypeFactory()
        {
        }

        /// <summary>
        /// Gets the type for a given name and stores the type in a local cache.
        /// </summary>
        /// <param name="name">
        /// The type name.
        /// </param>
        /// <returns>
        /// The type for the given name or null if the type can't be found.
        /// </returns>
        public Type this[string name]
        {
            get
            {
                Type type;

                lock (this.types)
                {
                    if (!this.types.TryGetValue(name, out type))
                    {
                        type = this.FindType(name) ?? this.FindType(this.GetType().Namespace + "." + name);
                        if (type != null)
                        {
                            this.types.Add(name, type);
                        }
                    }
                }

                return type;
            }
        }

        private Type FindType(string type)
        {
            var t = Type.GetType(type, false, false);
            if (t != null)
            {
                return t;
            }

            t = this.FindForeignType(type);
            if (t != null)
            {
                return t;
            }

            return this.FindGenericType(type);
        }

        private Type FindGenericType(string type)
        {
            var firstBracket = type.IndexOf('[');
            if (firstBracket < 0)
            {
                return null;
            }

            var lastBracket = type.LastIndexOf(']');
            if (lastBracket < firstBracket)
            {
                return null;
            }

            var genericType = this[type.Substring(0, firstBracket)];
            if (genericType == null || !genericType.IsGenericTypeDefinition)
            {
                return null;
            }

            var args = new List<Type>();
            int bracketLevel = 0;
            int startIndex = -1;
            for (int i = firstBracket; i <= lastBracket; i++)
            {
                var c = type[i];
                switch (c)
                {
                    case '[':
                        bracketLevel++;
                        if (bracketLevel == 1)
                        {
                            startIndex = i + 1;
                        }

                        continue;
                    case ']':
                        bracketLevel--;
                        if (bracketLevel != 0)
                        {
                            continue;
                        }

                        break;

                    case ',':
                        if (bracketLevel > 1)
                        {
                            continue;
                        }

                        break;
                    default:
                        continue;
                }

                var argName = type.Substring(startIndex + 1, i - startIndex - 2);
                var firstComma = argName.IndexOf(',');
                if (firstComma > 0)
                {
                    argName = argName.Substring(0, firstComma);
                }

                var argType = this[argName];
                if (argType == null)
                {
                    return null;
                }

                args.Add(argType);
                startIndex = i + 1;
            }

            return genericType.MakeGenericType(args.ToArray());
        }
    }
}
