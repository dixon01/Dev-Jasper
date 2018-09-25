// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlSchemaTestApp
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Xml;
    using System.Xml.Schema;

    using Gorba.Center.ItcsClient.ServiceModel;
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        private static string lastLoadedFile;

        private static ConfigManager<ItcsConfiguration> configManager = new ConfigManager<ItcsConfiguration>();

        private static void Main(string[] args)
        {
            Console.WriteLine("Press");
            Console.WriteLine("\t'q' to quit");
            Console.WriteLine("\t'l' to load an xml file");
            Console.WriteLine("\t's' to serialize into xml file");
            Console.WriteLine("\t't' to retry with last load file.");
            string line;
            do
            {
                Console.Write("XmlSchemaTest >");
                line = Console.ReadLine();
                switch (line)
                {
                    case "l":
                        Console.Write("Enter path to itcsconfiguration xml to load: ");
                        line = Console.ReadLine();
                        LoadConfig(line);
                        break;
                    case "t":
                        Console.Write(lastLoadedFile);
                        LoadConfig(lastLoadedFile);
                        break;

                    case "s":
                        SaveConfig();
                        break;

                    case "q":
                        break;
                    default:
                        Thread.Sleep(100);
                        break;
                }
            }
            while (line != "q");
        }

        private static void LoadConfig(string filename)
        {
            
            if (!string.IsNullOrEmpty(filename))
            {
                configManager.FileName = filename;
                lastLoadedFile = filename;
            }

            var resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlSchemaTestApp.ItcsConfiguration.xsd");
            if (resourceStream == null)
            {
                return;
            }

            var reader = new XmlTextReader(resourceStream);
            configManager.XmlSchema = XmlSchema.Read(reader, null);
            ItcsConfiguration config = null;
            try
            {
                config = configManager.Config;
            }
            catch (Exception e)
            {
                var configException = e as ConfiguratorException;
                if (configException == null)
                {
                    Console.WriteLine(e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.Message);
                    }

                    return;
                }

                var xmlValidationException = configException.InnerException as XmlValidationException;
                if (xmlValidationException == null)
                {
                    Console.WriteLine(e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.Message);
                    }

                    return;
                }

                Console.WriteLine("Invalid xml:");
                foreach (var xmlSchemaException in xmlValidationException.Exceptions)
                {
                    Console.WriteLine(
                        "{0} LineNumber: {1}, LinePosition: {2}",
                        xmlSchemaException.Message,
                        xmlSchemaException.LineNumber,
                        xmlSchemaException.LinePosition);
                }
            }

            PrintConfig(config);
        }

        private static void SaveConfig()
        {
            configManager.SaveConfig();
        }

        private static void PrintConfig(ItcsConfiguration configuration)
        {
            if (configuration != null)
            {
                Console.WriteLine(
                    "OperationDayStartUtc: "
                    + configuration.OperationDayStartUtc.ToString());
                Console.WriteLine(
                    "Hysteresis: "
                    + configuration.VdvConfiguration.DefaultSubscriptionConfiguration.Hysteresis.ToString());
            }
            else
            {
                Console.WriteLine("No configuration deserialized.");
            }
        }

        #endregion
    }
}