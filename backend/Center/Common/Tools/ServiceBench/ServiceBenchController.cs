// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBenchController.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceBenchController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench
{
    using Gorba.Center.Common.ServiceModel;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Component used to control the lifecycle of the application.
    /// </summary>
    public class ServiceBenchController
    {
        /// <summary>
        /// Registers all the (default) services needed by the application.
        /// </summary>
        public static void RegisterServices()
        {
            // TODO: add service registration here
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IOperationService, OperationServiceProxy>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }
    }
}