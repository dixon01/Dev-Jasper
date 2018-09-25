// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.TestClient
{
    using System;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
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
            Console.WriteLine("Client starting...");
            var configuration = new RemoteServicesConfiguration
                                    {
                                        Host = "localhost",
                                        Port = 9089,
                                        Protocol = RemoveServiceProtocol.Tcp
                                    };
            ChannelScopeFactoryUtility<IMembershipService>.ConfigureAsFunctionalService(configuration, "Membership");
            var credentials = new UserCredentials("gorba", "gorba");
            using (var channel = ChannelScopeFactory<IMembershipService>.Current.Create(credentials))
            {
                Console.WriteLine("Type <Enter> to authenticate");
                Console.ReadLine();
                var authenticateUserAsync = channel.Channel.AuthenticateUserAsync();
                authenticateUserAsync.Wait();
                Console.WriteLine("Returned user {0}", authenticateUserAsync.Result.Id);
            }

            Console.WriteLine("Done");
        }
    }
}