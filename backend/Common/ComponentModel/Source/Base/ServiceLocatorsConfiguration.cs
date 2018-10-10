//-----------------------------------------------------------------------
// <copyright file="ServiceLocatorsConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gorba.Common.ComponentModel.Base
{
    using System;
    using System.Diagnostics;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines the configuration for the default service locator in <see cref="ServiceLocators"/>.
    /// </summary>
    public static class ServiceLocatorsConfiguration
    {
        /// <summary>
        /// The initializer function.
        /// </summary>
        private static Func<IServiceLocator> initializerFunction;

        /// <summary>
        /// Occurs when the initializer function changes.
        /// </summary>
        public static event EventHandler InitializaterChanged;

        /// <summary>
        /// Gets the initializer.
        /// </summary>
        /// <value>The initializer.</value>
        public static Func<IServiceLocator> Initializer
        {
            get { return ServiceLocatorsConfiguration.initializerFunction; }
        }

        /// <summary>
        /// Sets the initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public static void SetInitializer(Func<IServiceLocator> initializer)
        {
            if (!ServiceLocators.Initialized)
            {
                ServiceLocatorsConfiguration.initializerFunction = initializer;
                if (ServiceLocatorsConfiguration.InitializaterChanged != null)
                {
                    ServiceLocatorsConfiguration.InitializaterChanged(null, EventArgs.Empty);
                }
            }
            else
            {
                Debug.WriteLine("Attempt to set the initialzer for the ServiceLocators after its usage.");
            }
        }
    }
}
