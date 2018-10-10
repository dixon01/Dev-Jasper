// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDomainExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Utils
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Extension methods for <see cref="AppDomain"/>.
    /// </summary>
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Creates a new instance of the specified type.
        /// </summary>
        /// <param name="appDomain">
        /// The app domain.
        /// </param>
        /// <typeparam name="T">
        /// The type of which an object is to be created.
        /// </typeparam>
        /// <returns>
        /// the local proxy stub for the object.
        /// </returns>
        public static T CreateInstanceAndUnwrap<T>(this AppDomain appDomain) where T : class
        {
            var type = typeof(T);
            var typeName = type.FullName;

            Debug.Assert(typeName != null, "typeName != null");
            return (T)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, typeName);
        }
    }
}
