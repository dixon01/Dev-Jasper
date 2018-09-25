// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServicesUtility.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataServicesUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Setup
{
    using System.ServiceModel;

    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.Common.ServiceModel;

    using NLog;

    /// <summary>
    /// Utility class to setup (non-change tracking) data services.
    /// </summary>
    public static partial class DataServicesUtility
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a service host for the given service with the background system configuration.
        /// </summary>
        /// <param name="instance">
        /// The instance of the service.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">
        /// The type of the service.
        /// </typeparam>
        /// <returns>
        /// The newly created <see cref="ServiceHost"/>.
        /// </returns>
        internal static ServiceHost CreateServiceHost<T>(T instance, string name)
        {
            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            Logger.Debug("Creating host for service of type <{0}> named '{1}'", typeof(T).Name, name);
            return configuration.DataServices.CreateDataServiceHost(name, instance);
        }
    }
}
