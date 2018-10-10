// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AnnaxRendererTest
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using MinimalisticTelnet;

    public class Program
    {
        private static TelnetConnection connection;

        public static void Main(string[] args)
        {
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            connection = new TelnetConnection("192.168.192.200", 23);
            var read = connection.Login("prb", null, 100);
            Console.WriteLine(read);

            connection.WriteLine("listenv composer");

            var regex = new Regex(@"image=\((\d+)/(\d+)\)");
            int width = 0;
            int height = 0;
            while ((read = connection.Read()) != null && read.Length > 0)
            {
                Console.WriteLine(read);
                var match = regex.Match(read);
                if (!match.Success)
                {
                    continue;
                }

                width = int.Parse(match.Groups[1].Value);
                height = int.Parse(match.Groups[2].Value);
                Console.WriteLine("Available screen size: {0} x {1}", width, height);
                break;
            }

            if (width <= 0 || height <= 0)
            {
                Console.WriteLine("Could not determine screen size");
                return;
            }

            var handler = new ScreenHandler(width, height);
            handler.Start(connection);
            Console.WriteLine("Press <enter> to quit");
            Console.ReadLine();
        }
    }
}
