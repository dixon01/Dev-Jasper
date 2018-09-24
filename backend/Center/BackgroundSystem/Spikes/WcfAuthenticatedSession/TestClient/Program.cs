// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestClient
{
    using System;

    using WcfAuthenticatedSession.ServiceModel;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private enum Operations
        {
            None = 0,

            Write = 1,

            Read = 2,

            Exit = 3
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Enter to start the client");
            Console.ReadLine();
            try
            {
                var operation = Cycle();
                while (operation != Operations.Exit)
                {
                    switch (operation)
                    {
                        case Operations.None:
                            break;
                        case Operations.Write:
                            var configuration = GetConfiguration();
                            Write(configuration);
                            break;
                        case Operations.Read:
                            break;
                        case Operations.Exit:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    operation = Cycle();
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

        private static ServicesConfiguration GetConfiguration()
        {
            var user = SelectUser();
            return new ServicesConfiguration { Username = user.ToString() };
        }

        private static void Write(ServicesConfiguration configuration)
        {
            var tenant = SelectTenant();
            if (tenant == Tenants.Undefined)
            {
                return;
            }

            Console.WriteLine("Value?");
            var value = Console.ReadLine();
                var service = ChannelScopeFactory.Create(configuration);

            service.Write((int)tenant, value).Wait();
        }

        private static Operations Cycle()
        {
            Console.WriteLine("1: write, 2: read, 3: exit");

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return Operations.Write;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return Operations.Read;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return Operations.Exit;
                default:
                    return Operations.None;
            }
        }

        private static Users SelectUser()
        {
            Console.WriteLine("Select user: 1 (reader), 2 (writer), 3 (God)");
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return Users.Reader;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return Users.Writer;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return Users.God;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    return Users.Unauthorized;
                default:
                    return Users.Undefined;
            }
        }

        private static Tenants SelectTenant()
        {
            Console.WriteLine("Select tenant (1-2)");
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return Tenants.Tenant1;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return Tenants.Tenant2;
                default:
                    return Tenants.Undefined;
            }
        }
    }
}
