// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestClient
{
    using System;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    using ServiceModel;
    using ServiceModel.Certificates;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Write("Server address: ");
                var address = Console.ReadLine();
                Console.Write("Username: ");
                var user = Console.ReadLine();
                Console.Write("Password: ");
                var pass = ReadPassword();
                try
                {
                    var uriString = string.Format("net.tcp://{0}/TestService", address);
                    Console.WriteLine("Setup channel with credentials to {0}", uriString);
                    var endPointAddress = new EndpointAddress(
                        new Uri(uriString),
                        new DnsEndpointIdentity("BackgroundSystem"));
                    var channelFactory = new ChannelFactory<ITestService>(
                        ServiceUtility.GetNetTcpBinding(),
                        endPointAddress);
                    var defaultCredentials = channelFactory.Endpoint.Behaviors.Find<ClientCredentials>();
                    channelFactory.Endpoint.Behaviors.Remove(defaultCredentials);
                    var loginCredentials = new ClientCredentials();
                    loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                        X509CertificateValidationMode.Custom;
                    loginCredentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                        new CertificateValidator();
                    loginCredentials.UserName.UserName = user;
                    loginCredentials.UserName.Password = pass;
                    channelFactory.Endpoint.Behaviors.Add(loginCredentials);

                    var testService = channelFactory.CreateChannel();
                    Console.WriteLine("Connected to service, getting test value");
                    var value = testService.GetTestValue();
                    Console.WriteLine("TestValue: {0}", value);
                }
                catch (Exception e)
                {
                    if (e is SecurityException)
                    {
                        Console.WriteLine(
                            "Security error. StackTrace: {0}",
                            e.InnerException.StackTrace);
                    }
                    else
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                    }

                    Console.WriteLine();
                }

                Console.Write("Press Enter to try again, q to quit: ");
                key = Console.ReadKey();
                Console.WriteLine();
            }
            while (key.Key != ConsoleKey.Q);
        }

        /// <summary>
        /// The read password.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);

                        // get the location of the cursor
                        int pos = Console.CursorLeft;

                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);

                        // replace it with space
                        Console.Write(" ");

                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }

                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
    }
}
