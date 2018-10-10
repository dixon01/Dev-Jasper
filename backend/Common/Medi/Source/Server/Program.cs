// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the main program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Xml.Serialization;

    using CommandLineParser;
    using CommandLineParser.Arguments;
    using CommandLineParser.Validation;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Main program for the MediServer application
    /// </summary>
    internal class Program
    {
        private static readonly Logger Logger = LogHelper.GetLogger<Program>();

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">
        /// The command line arguments
        /// </param>
        /// <returns>
        /// zero if the application exited successfully.
        /// </returns>
        internal static int Main(string[] args)
        {
            var version = ApplicationHelper.GetApplicationFileVersion();
            Logger.Info("Starting Medi Server {0}", version);
            try
            {
                var parser = new CommandLineParser();
                var options = new Options();
                parser.ExtractArgumentAttributes(options);
                parser.ParseCommandLine(args);

                if (options.Help)
                {
                    parser.ShowUsage();
                    Console.WriteLine("If no peer (-c, -s or -e) is defined, the application will load");
                    Console.WriteLine("the configuration from Medi.config in the current directory.");
                    return 1;
                }

                return Start(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        private static int Start(Options options)
        {
            MediConfig config;

            if (options.ConfigFile != null)
            {
                config = ReadConfig(options.ConfigFile);
            }
            else
            {
                config = CreateConfig(options);

                if (config.Peers.Count == 0)
                {
                    // read default config file
                    var thisAppAbsName = ApplicationHelper.GetEntryAssemblyLocation();
                    var dllDirectory = Path.GetDirectoryName(thisAppAbsName);
                    if (dllDirectory == null)
                    {
                        throw new FileNotFoundException("Could not find parent path for config directory.");
                    }

                    var cfgFileAbsName = Path.Combine(dllDirectory, "Medi.config");
                    config = ReadConfig(new FileInfo(cfgFileAbsName));
                }
            }

            if (options.CreateXmlConfig != null)
            {
                var serializer = new XmlSerializer(config.GetType());
                using (var writer = options.CreateXmlConfig.CreateText())
                {
                    serializer.Serialize(writer, config);
                }

                Console.WriteLine("Wrote XML configuration to {0}", options.CreateXmlConfig.FullName);
                return 0;
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            string unitName;
            switch (options.UnitName.ToLower())
            {
                case "machine":
                    unitName = ApplicationHelper.MachineName;
                    break;
                case "ip":
                    unitName = GetSystemIpAddress();
                    break;
                default:
                    unitName = options.UnitName;
                    break;
            }

            var appName = string.IsNullOrEmpty(options.AppName) ? Guid.NewGuid().ToString() : options.AppName;
            var configurator = new ObjectConfigurator(config, unitName, appName);

            Console.WriteLine("Configuring Message Dispatcher...");
            Console.WriteLine("Unit name: {0}", unitName);
            Console.WriteLine("App name:  {0}", appName);

            MessageDispatcher.Instance.Configure(configurator);
            Console.WriteLine("Message Dispatcher is ready to serve.");
            Console.WriteLine("Press <enter> to quit.");

            Console.ReadLine();

            return 0;
        }

        private static MediConfig ReadConfig(FileInfo file)
        {
            MediConfig config;
            var serializer = new XmlSerializer(typeof(MediConfig));
            using (var input = file.OpenRead())
            {
                config = serializer.Deserialize(input) as MediConfig;
            }

            if (config == null)
            {
                throw new ApplicationException(string.Format("Could not load config from {0}", file.FullName));
            }

            Console.WriteLine("Using config file: {0}", file.FullName);
            return config;
        }

        private static MediConfig CreateConfig(Options options)
        {
            CodecConfig codec;
            switch (options.Codec.ToLower())
            {
                case "xml":
                    codec = new XmlCodecConfig();
                    Console.WriteLine("Using XML Codec");
                    break;
                case "bec":
                    codec = new BecCodecConfig();
                    Console.WriteLine("Using Binary Enhanced Codec");
                    break;
                default:
                    throw new ArgumentException("Unsupported Codec: " + options.Codec);
            }

            var config = new MediConfig();
            AddServers(config, codec, options);
            AddClients(config, codec, options);
            AddEventHandlerServers(config, options);

            return config;
        }

        private static void AddClients(MediConfig config, CodecConfig codec, Options options)
        {
            foreach (var client in options.Clients)
            {
                var clientParts = client.Split(':');
                if (clientParts.Length != 2)
                {
                    throw new ArgumentException("Unsupported client: " + client);
                }

                int clientPort;
                if (!ParserUtil.TryParse(clientParts[1], out clientPort))
                {
                    throw new ArgumentException("Unsupported client port number: " + clientParts[1]);
                }

                Console.WriteLine("Adding TCP client for port {0}", clientPort);
                config.Peers.Add(
                    new ClientPeerConfig
                    {
                        Transport = new TcpTransportClientConfig
                                        {
                                            RemoteHost = clientParts[0], RemotePort = clientPort
                                        },
                        Codec = codec
                    });
            }
        }

        private static void AddServers(MediConfig config, CodecConfig codec, Options options)
        {
            foreach (var server in options.Servers)
            {
                int serverPort;
                if (!ParserUtil.TryParse(server, out serverPort))
                {
                    throw new ArgumentException("Unsupported server port number: " + server);
                }

                Console.WriteLine("Adding TCP server for port {0}", serverPort);
                config.Peers.Add(
                    new ServerPeerConfig
                        {
                            Transport = new TcpTransportServerConfig { LocalPort = serverPort }, Codec = codec
                        });
            }
        }

        private static void AddEventHandlerServers(MediConfig config, Options options)
        {
            foreach (var server in options.EventHandlerServers)
            {
                int serverPort;
                if (!ParserUtil.TryParse(server, out serverPort))
                {
                    throw new ArgumentException("Unsupported server port number: " + server);
                }

                Console.WriteLine("Adding Eventhandler server for port {0} for:", serverPort);
                foreach (var type in options.EventHandlerTypes)
                {
                    Console.WriteLine(" - {0}", type);
                }

                config.Peers.Add(
                    new EventHandlerPeerConfig
                        {
                            LocalPort = serverPort, SupportedMessages = options.EventHandlerTypes
                        });
            }
        }

        private static string GetSystemIpAddress()
        {
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    return ip.ToString();
                }
            }

            throw new NotSupportedException("Could not determine local IP address.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.FatalException(
                "Unhandled Exception; terminating=" + e.IsTerminating, e.ExceptionObject as Exception);
        }

        [DistinctGroupsCertification("f", "c,s,x")]
        private class Options
        {
            // this class is a bit special since its properties are
            // mostly used by reflection, therefore we disable some warnings:
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            // ReSharper disable MemberCanBePrivate.Local
            public Options()
            {
                this.Clients = new List<string>();
                this.Servers = new List<string>();
                this.EventHandlerServers = new List<string>();
                this.EventHandlerTypes = new List<string>();
                this.Codec = "xml";
                this.UnitName = "machine";
            }

            [SwitchArgument('h', "help", false, Description = "Show this help")]
            public bool Help { get; set; }

            [ValueArgument(
                typeof(string), 'c', "client", Description = "Start a Medi TCP client",
                FullDescription = "Format: <host>:<port> (multiple clients are possible)", AllowMultiple = true)]
            public List<string> Clients { get; set; }

            [ValueArgument(
                typeof(string), 's', "server", Description = "Start a Medi TCP server",
                FullDescription = "Format: <port> (multiple servers are possible)", AllowMultiple = true)]
            public List<string> Servers { get; set; }

            [ValueArgument(
                typeof(string), 'e', "eventhandler", Description = "Start an Eventhandler server",
                FullDescription = "Format: <port> (multiple EH servers are possible)", AllowMultiple = true)]
            public List<string> EventHandlerServers { get; set; }

            [ValueArgument(
                typeof(string), 't', "types", Description = "Types to register for Eventhandler servers",
                FullDescription = "Format: <FullClassName> (multiple servers are possible)", AllowMultiple = true)]
            public List<string> EventHandlerTypes { get; set; }

            [EnumeratedValueArgument(
                typeof(string),
                'o',
                "codec",
                AllowedValues = "xml,bec",
                DefaultValue = "xml",
                Description = "Define the codec (either 'xml' or 'bec')")]
            public string Codec { get; set; }

            [ValueArgument(
                typeof(string),
                'u',
                "unit",
                DefaultValue = "machine",
                Description = "Define the how the unit name is retrieved",
                FullDescription = "If it is set to 'machine', the machine name will be taken as the unit name; " +
                    "if it is set to 'ip', the IP address of the first adapter will be used")]
            public string UnitName { get; set; }

            [ValueArgument(
                typeof(string),
                'a',
                "app",
                Description = "Define the app name",
                FullDescription = "The default value is an automatically generated GUID.")]
            public string AppName { get; set; }

            [FileArgument('f', "file", Description = "Read the configuration from a file", FileMustExist = true)]
            public FileInfo ConfigFile { get; set; }

            [FileArgument('x',
                "xml",
                Description = "Create an XML config file for the given arguments",
                FileMustExist = false)]
            public FileInfo CreateXmlConfig { get; set; }

            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}
