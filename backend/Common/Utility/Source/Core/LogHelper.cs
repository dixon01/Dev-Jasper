// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    using NLog;

    /// <summary>
    /// Helper class for <see cref="NLog"/> that allows to get
    /// a logger for a given type (as a replacement for <see cref="LogManager.GetCurrentClassLogger()"/>.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Gets a logger with the full name.
        /// The name is simplified for generic types
        /// (only showing the type name without namespace for generic arguments).
        /// </summary>
        /// <typeparam name="T">
        /// The type for which the logger should be created.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Logger"/>.
        /// </returns>
        public static Logger GetLogger<T>()
        {
            return LogManager.GetLogger(ClassNameProvider.GetGenericClassName(typeof(T)));
        }

        /// <summary>
        /// Gets a logger with the full name.
        /// The name is simplified for generic types
        /// (only showing the type name without namespace for generic arguments).
        /// </summary>
        /// <param name="type">
        /// The type for which the logger should be created.
        /// </param>
        /// <returns>
        /// The <see cref="Logger"/>.
        /// </returns>
        public static Logger GetLogger(Type type)
        {
            return LogManager.GetLogger(ClassNameProvider.GetGenericClassName(type));
        }
    }
}
