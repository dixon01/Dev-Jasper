// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigImplementationFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigImplementationFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Factory
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Factory that creates a configurable object from a given config using the
    /// <see cref="ImplementationAttribute"/> to determine which type is the
    /// implementation for a given config.
    /// </summary>
    public static class ConfigImplementationFactory
    {
        /// <summary>
        /// Creates a configurable object from a given config using the
        /// <see cref="ImplementationAttribute"/> to determine which type is the
        /// implementation for a given config.
        /// </summary>
        /// <param name="config">
        /// The configuration object.
        /// </param>
        /// <typeparam name="T">
        /// the type of the expected object.
        /// </typeparam>
        /// <returns>
        /// the object implemented.
        /// Its <see cref="IConfigurable{T}.Configure"/> method
        /// will be called with the given config object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// if configParent or propertyName are null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// if the type of configParent is not a valid config type.
        /// </exception>
        public static T CreateFromConfig<T>(object config) where T : class
        {
            var attrs = config.GetType().GetCustomAttributes(typeof(ImplementationAttribute), false);

            if (attrs == null || attrs.Length != 1)
            {
                throw new NotSupportedException(
                    "Type does not have ImplementationAttribute: " + config.GetType().FullName);
            }

            var attr = attrs[0] as ImplementationAttribute;

            if (attr == null || attr.Type == null)
            {
                throw new NotSupportedException("Type does not have implementation: " + config.GetType().FullName);
            }

            object implemenation = Activator.CreateInstance(attr.Type);
            if (implemenation == null)
            {
                throw new NotSupportedException("Could not create instance of " + attr.Type.FullName);
            }

            var impl = implemenation as T;
            if (impl == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "Config has wrong implementation: {0} is a {1}",
                        config.GetType().FullName,
                        implemenation.GetType()));
            }

            foreach (var method in attr.Type.GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.Name.EndsWith("Configure"))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(config))
                    {
                        method.Invoke(impl, new[] { config });
                        return impl;
                    }
                }
            }

            return impl;
        }
    }
}
