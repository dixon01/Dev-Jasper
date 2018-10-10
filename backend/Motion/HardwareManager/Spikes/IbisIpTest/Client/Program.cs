// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IbisIpClientTest
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Common.IbisIP.Client;
    using Gorba.Motion.Common.IbisIP.Server;

    public class Program
    {
        private static readonly List<IServiceInfo> FoundServices = new List<IServiceInfo>();

        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter url or press <enter> to search for services");
            var line = Console.ReadLine();
            if (line == null)
            {
                return;
            }

            var serviceUris = new List<Uri>();
            line = line.Trim();
            if (line.Length != 0)
            {
                serviceUris.Add(new Uri(line));
            }
            else
            {
                var dnsSdProvider = new AutoDnsSdProvider();
                dnsSdProvider.Start();

                var query = dnsSdProvider.CreateQuery(Definitions.HttpProtocol);
                query.ServicesChanged += (s, e) => WriteServices(query);
                query.Start();

                Console.WriteLine("Press <enter> to request device information (once you see the service below):");
                Console.ReadLine();
                query.Stop();

                foreach (var service in query.Services)
                {
                    if (!service.Name.StartsWith("DeviceManagementService") || service.Addresses.Length == 0)
                    {
                        continue;
                    }

                    var address = service.Addresses[0].ToString();
                    string path = service.GetAttribute("path") ?? string.Empty;
                    if (path.StartsWith("/"))
                    {
                        path = path.Substring(1);
                    }

                    serviceUris.Add(new Uri("http://" + address + ":" + service.Port + "/" + path));
                }

                dnsSdProvider.Stop();
            }

            Console.WriteLine("Starting local callback server");
            var localServer = new IbisHttpServer(new IPEndPoint(IPAddress.Any, 0));
            localServer.Start();
            Console.WriteLine("Local callback server is running on port {0}", localServer.LocalPort);

            foreach (var serviceUri in serviceUris)
            {
                switch (serviceUri.Scheme)
                {
                    case "http":
                        CreateHttpClientProxy(serviceUri, localServer);
                        break;
                    case "udp":
                        CreateUdpClientProxy(serviceUri);
                        break;
                    default:
                        Console.WriteLine("Unknwon URI scheme: {0}", serviceUri.Scheme);
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }

        private static void CreateUdpClientProxy(Uri serviceUri)
        {
            var address = IPAddress.Parse(serviceUri.Host);
            var client = ServiceClientProxyFactory.Create<IDistanceLocationService>(address, serviceUri.Port);

            Console.WriteLine("Subscribing to changes of DistanceLocation");
            client.DistanceLocationChanged += ClientOnDistanceLocationChanged;
        }

        private static void ClientOnDistanceLocationChanged(
            object sender, DataUpdateEventArgs<DistanceLocationServiceDataStructure> e)
        {
            Console.WriteLine(
                "Distance changed: {0:0.00} m ({1} pulses)", e.Value.Distance.Value, e.Value.OdometerPulses.Value);
        }

        private static void CreateHttpClientProxy(Uri serviceUri, IbisHttpServer localServer)
        {
            var client = ServiceClientProxyFactory.Create<IDeviceManagementService>(
                serviceUri.Host, serviceUri.Port, serviceUri.AbsolutePath, localServer);

            var deviceInfo = client.GetDeviceInformation();
            Console.WriteLine(serviceUri);
            Console.WriteLine("  Class: {0}", deviceInfo.DeviceInformation.DeviceClass);
            Console.WriteLine("  Name:  {0}", deviceInfo.DeviceInformation.DeviceName.Value);
            Console.WriteLine();

            Console.WriteLine("  Service Status:");
            var serviceStatus = client.GetServiceStatus();
            foreach (var status in serviceStatus.ServiceSpecificationWithStateList)
            {
                Console.WriteLine(
                    "    {0} ({1}): {2}",
                    status.ServiceSpecification.ServiceName,
                    status.ServiceSpecification.IBISIPVersion.Value,
                    status.ServiceState);
            }

            Console.WriteLine("Subscribing to changes of GetDeviceStatus");
            client.DeviceStatusChanged += ClientOnDeviceStatusChanged;
        }

        private static void ClientOnDeviceStatusChanged(
            object sender,
            DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure> e)
        {
            Console.WriteLine("Device status changed @ {0:r}: {1}", e.Value.TimeStamp.Value, e.Value.DeviceState);
        }

        private static void WriteServices(IQuery query)
        {
            lock (FoundServices)
            {
                foreach (var service in query.Services)
                {
                    if (FoundServices.Contains(service) || service.Addresses.Length == 0)
                    {
                        continue;
                    }

                    Console.WriteLine("  Found {0} on {1}", service.FullName, service.Addresses[0]);
                }
            }
        }
    }
}
