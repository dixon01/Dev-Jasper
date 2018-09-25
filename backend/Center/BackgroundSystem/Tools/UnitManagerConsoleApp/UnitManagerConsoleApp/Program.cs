// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnitManagerConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Center.BackgroundSystem.Core.Units;
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// Enables to test the unit manager
    /// </summary>
    public class Program
    {
        private static UnitManager unitManager;

        private static ConfigManager<UnitManagerConfig> configManager;

        /// <summary>
        /// Main entry point of the application
        /// static void Main(string[] args)
        /// </summary>
        public static void Main()
        {
            configManager = new ConfigManager<UnitManagerConfig>();

            var unitAddressList = new List<string> { "A:2.1.1" };

            try
            {
                UnitManagerConfig unitManagerConfig = configManager.Config;
                unitAddressList = unitManagerConfig.UnitAddressList;
            }
            catch (ConfiguratorException ex)
            {
                Console.WriteLine(
                    string.Format("{0}\r\nConfigurator : {1}", ex.Message, ex.ConfigMngrName),
                    "XML Configuration error.",
                    ex.InnerException);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(string.Format("{0}\r\n{1}", ex.Message, ex.FileName), "XML Configuration error.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    string.Format("Error while reading configuration from files.\r\n{0}", ex.Message),
                    "Configuration error");
            }

            using (unitManager = new UnitManager())
            {
                UnityInitialization.Initialise(unitAddressList);

                Console.Write("Declared units :");
                foreach (var adr in unitAddressList)
                {
                    Console.Write(adr + "; ");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to start");
                Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine("Starting unit manager.");
                unitManager.Start();

                Console.WriteLine("The unit manager is started");
                Console.WriteLine("Press any key to stop it.");

                Console.ReadKey();

                Console.WriteLine("Stopping unit manager.");
                unitManager.Stop();
            }
        }
    }
}
