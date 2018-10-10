// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ChangeTrackingObserver.TestServer
{
    using System;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Core.Setup;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            var factory = new ServiceBusNotificationManagerFactory();
            NotificationManagerFactory.Set(factory);
            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            var services = ChangeTrackingDataServicesUtility.SetupChangeTrackingDataServicesAsync(
                configuration,
                new UserCredentials("gorba", "gorba")).Result.ToList();
            foreach (var host in services)
            {
                host.Open();
            }

            Console.WriteLine("Services started. Type <Enter> to stop them");
            Console.ReadLine();
            foreach (var host in services)
            {
                host.Close();
            }
        }
    }
}