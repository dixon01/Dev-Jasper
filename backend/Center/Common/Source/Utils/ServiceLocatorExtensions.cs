// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Extension methods to the unity container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System;
    using System.Diagnostics.Contracts;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Extension methods to the unity container.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Tries to resolve a type or wraps the <see cref="ResolutionFailedException"/> within
        /// a more descriptive <see cref="ApplicationException"/>.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the object to resolve.
        /// </typeparam>
        /// <param name="key">Optional parameter to provide a key/name to the resolution.</param>
        /// <returns>
        /// The instance of type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// The exception wrapping the Unity specific ones.
        /// </exception>
        public static T ResolveOrThrowException<T>(this IServiceLocator serviceLocator, string key = null)
        {
            Contract.Requires(serviceLocator != null, "The service locator can't be null.");
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return serviceLocator.GetInstance<T>();
                }

                return serviceLocator.GetInstance<T>(key);
            }
            catch (ResolutionFailedException exception)
            {
                const string Message =
                    "Can't resolve an object of type '{0}'. Please verify that the unity container was"
                    + " correctly initialized.";
                var message = string.Format(Message, typeof(T));
                throw new ApplicationException(message, exception);
            }
        }
    }
}
