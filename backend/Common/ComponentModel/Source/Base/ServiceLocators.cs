//-----------------------------------------------------------------------
// <copyright file="ServiceLocators.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Gorba.Common.ComponentModel.Base
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// This static class is used to provide a repository for service locators, with a default one.
    /// </summary>
    public static class ServiceLocators
    {
        /// <summary>
        /// Object used for synchronization.
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// The default <see cref="Lazy"/> <see cref="IServiceLocator"/>.
        /// </summary>
        private static Lazy<IServiceLocator> defaultServiceLocator;

        /// <summary>
        /// Initializes static members of the <see cref="ServiceLocators"/> class.
        /// </summary>
        static ServiceLocators()
        {
            ServiceLocatorsConfiguration.InitializaterChanged += new EventHandler(UnityContainersConfiguration_InitializaterChanged);
        }

        /// <summary>
        /// Gets the default. <see cref="IServiceLocator"/>.
        /// </summary>
        /// <value>The default <see cref="IServiceLocator"/>.</value>
        public static IServiceLocator Default
        {
            get
            {
                IServiceLocator value;
                if (ServiceLocators.defaultServiceLocator == null)
                {
                    lock (ServiceLocators.LockObject)
                    {
                        if (ServiceLocators.defaultServiceLocator == null)
                        {
                            ServiceLocators.EnsureDefaultServiceLocator();
                        }

                        value = ServiceLocators.defaultServiceLocator.Value;
                    }
                }
                else
                {
                    value = ServiceLocators.defaultServiceLocator.Value;
                }

                Contract.Assume(value != null);
                return value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ServiceLocators"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public static bool Initialized
        {
            get { return ServiceLocators.defaultServiceLocator != null && ServiceLocators.defaultServiceLocator.IsValueCreated; }
        }

        /// <summary>
        /// Handles the InitializaterChanged event of the UnityContainersConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void UnityContainersConfiguration_InitializaterChanged(object sender, EventArgs e)
        {
            if (ServiceLocators.defaultServiceLocator == null || !ServiceLocators.defaultServiceLocator.IsValueCreated)
            {
                lock (ServiceLocators.LockObject)
                {
                    if (ServiceLocators.defaultServiceLocator == null || !ServiceLocators.defaultServiceLocator.IsValueCreated)
                    {
                        if (ServiceLocatorsConfiguration.Initializer != null)
                        {
                            ServiceLocators.defaultServiceLocator = new Lazy<IServiceLocator>(ServiceLocatorsConfiguration.Initializer);
                        }
                        else
                        {
                            ServiceLocators.defaultServiceLocator = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The default service locator initializer.
        /// </summary>
        /// <returns>The default service locator.</returns>
        private static IServiceLocator DefaultServiceLocatorInitializer()
        {
            throw new NotSupportedException("It's not possible to create a default service locator. Please initialize it through the ServiceLocatorConfiguration.");
        }

        /// <summary>
        /// Ensures the default service locator initializer.
        /// </summary>
        private static void EnsureDefaultServiceLocator()
        {
            Contract.Ensures(ServiceLocators.defaultServiceLocator != null);
            if (ServiceLocators.defaultServiceLocator == null)
            {
                lock (ServiceLocators.LockObject)
                {
                    if (ServiceLocators.defaultServiceLocator == null)
                    {
                        ServiceLocators.defaultServiceLocator = new Lazy<IServiceLocator>(ServiceLocators.DefaultServiceLocatorInitializer);
                    }
                }
            }
        }
    }
}
