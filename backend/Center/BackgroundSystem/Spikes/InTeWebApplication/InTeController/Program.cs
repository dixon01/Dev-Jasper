// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.InTeController
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private const string HashedPassword = "";

        private static readonly TestController Controller =
            new TestController(new UserCredentials("admin", HashedPassword));

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting controller...");
            ServiceBusNotificationManagerUtility.ConfigureServiceBusNotificationManager();
            Task.Run(() => Controller.RunAsync());
            Console.WriteLine("Closing controller...");
        }
    }
}