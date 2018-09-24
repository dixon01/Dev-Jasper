// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeUtilities.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TypeUtilities.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// the type utilities
    /// </summary>
    public class TypeUtilities
    {
        /// <summary>
        /// create a dynamically typed list
        /// </summary>
        /// <param name="source">the source</param>
        /// <returns>an enumerable</returns>
        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var type = GetCommonBaseClass(source);
            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");
            var list = listType.GetConstructor(Type.EmptyTypes).Invoke(null);

            foreach (var o in source)
            {
                addMethod.Invoke(list, new[] { o });
            }

            return (IEnumerable)list;
        }

        /// <summary>
        /// Gets the common base class
        /// </summary>
        /// <param name="e">the enumerable</param>
        /// <returns>the common type</returns>
        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        /// <summary>
        /// Gets the common base class
        /// </summary>
        /// <param name="types">the types</param>
        /// <returns>the common type</returns>
        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                {
                    ret = types[i];
                }
                else
                {
                    // This will always terminate when ret == typeof(object)
                    while (ret != null && !ret.IsAssignableFrom(types[i]))
                    {
                        ret = ret.BaseType;
                    }
                }
            }

            return ret;
        }
    }
}