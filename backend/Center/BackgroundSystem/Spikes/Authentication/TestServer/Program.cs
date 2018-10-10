// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Certificates;
    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">The args.</param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Starting Server");
            SetUserRepository();
            var service = new DummyMembershipService();
            var configuration = new RemoteServicesConfiguration
                                    {
                                        Host = "localhost",
                                        Port = 9089,
                                        Protocol = RemoveServiceProtocol.Tcp
                                    };
            using (var serviceHost = configuration.CreateFunctionalServiceHost("Membership", service))
            {
                serviceHost.Open();
                Console.WriteLine("Service started. Type <Enter> to exit");
                Console.ReadLine();
            }
        }

        private static void SetUserRepository()
        {
            var factory = new DummyUserRepositoryFactory();
            UserRepositoryFactory.SetInstance(factory);
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        private class DummyMembershipService : IMembershipService
        {
            public async Task<Common.ServiceModel.Membership.User> AuthenticateUserAsync()
            {
                return new Common.ServiceModel.Membership.User { Id = DateTime.Now.Hour };
            }
        }

        private class DummyUserRepositoryFactory : UserRepositoryFactory
        {
            public override IRepository<User> Create()
            {
                return new DummyUserRepository();
            }
        }

        private class DummyUserRepository : IRepository<User>
        {
            private readonly Dictionary<int, User> users = new Dictionary<int, User>();

            public DummyUserRepository()
            {
                this.users.Add(1, new User { HashedPassword = "gorba", IsEnabled = true, Username = "gorba" });
            }

            public void Dispose()
            {
            }

            public async Task<User> FindAsync(params object[] keyValues)
            {
                throw new NotSupportedException();
            }

            public IQueryable<User> Query()
            {
                return this.users.Values.AsQueryable();
            }

            public async Task<User> AddAsync(User entity)
            {
                this.users.Add(entity.Id, entity);
                return entity;
            }

            public async Task RemoveAsync(User entity)
            {
                if (!this.users.ContainsKey(entity.Id))
                {
                    return;
                }

                this.users.Remove(entity.Id);
            }

            public async Task<User> UpdateAsync(User entity)
            {
                throw new NotSupportedException();
            }
        }
    }
}