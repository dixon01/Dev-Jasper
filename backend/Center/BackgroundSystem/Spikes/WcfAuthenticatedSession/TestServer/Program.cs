// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System;
    using System.ServiceModel;

    using WcfAuthenticatedSession.ServiceModel;

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
            Console.WriteLine("Enter to start the server");
            Console.ReadLine();
            try
            {
                var configuration = new ServicesConfiguration();
                using (var serviceHost = new ServiceHost(typeof(SampleService)))
                {
                    var endpoint = configuration.GetEndpoint();
                    serviceHost.AddServiceEndpoint(endpoint);
                    serviceHost.SetServiceCredentials();
                    serviceHost.Open();
                    Console.WriteLine("Service started. Enter to stop");
                    Console.ReadLine();
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception: " + exception.Message);
            }

            Console.ResetColor();
            Console.WriteLine("Enter to exit");
            Console.ReadLine();
        }
    }
}
