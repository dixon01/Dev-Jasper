namespace IbisIpServerTest
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Common.IbisIP.Server;

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string line;
            do
            {
                Console.WriteLine("Press 'h' to start an HTTP server, 'u' to start a UDP server or 'q' to exit");
            }
            while ((line = Console.ReadLine()) != null && line != "q" && line != "u" && line != "h");

            if (line == "h")
            {
                RunHttpServer();
            }
            else if (line == "u")
            {
                RunUdpServer();
            }
        }

        private static void RunHttpServer()
        {
            string line;
            using (var httpServer = new IbisHttpServer(new IPEndPoint(IPAddress.Any, 8888)))
            {
                httpServer.ValidateRequests = true;
                httpServer.ValidateResponses = true;

                var service = new DeviceManagementService();
                httpServer.AddService(service);

                Console.WriteLine("Starting server on port {0}", httpServer.LocalPort);
                httpServer.Start();

                IDnsSdProvider dnsSdProvider = new AutoDnsSdProvider();
                dnsSdProvider.Start();

                var attributes = new Dictionary<string, string> { { "path", "1.0" }, { "ver", "1.0" } };

                dnsSdProvider.RegisterService("DeviceManagementService", Definitions.HttpProtocol, 8888, attributes);

                Console.WriteLine("Press 'c' to change state or 'q' to exit");
                while ((line = Console.ReadLine()) != null)
                {
                    if (line == "q")
                    {
                        break;
                    }

                    if (line == "c")
                    {
                        switch (service.DeviceState)
                        {
                            case DeviceStateEnumeration.defective:
                                service.DeviceState = DeviceStateEnumeration.notavailable;
                                break;
                            case DeviceStateEnumeration.notavailable:
                                service.DeviceState = DeviceStateEnumeration.running;
                                break;
                            case DeviceStateEnumeration.running:
                                service.DeviceState = DeviceStateEnumeration.defective;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        Console.WriteLine("Changed device state to " + service.DeviceState);
                    }

                    Console.WriteLine("Press 'c' to change state or 'q' to exit");
                }

                dnsSdProvider.Stop();
            }
        }

        private static void RunUdpServer()
        {
            using (var udpServer = new IbisUdpServer())
            {
                var service = new DistanceLocationService();
                var endPoint = udpServer.AddService(service);
                Console.WriteLine("DistanceLocationService started at {0}:{1}", endPoint.Address, endPoint.Port);

                string line = null;
                do
                {
                    if (line == "u")
                    {
                        service.Update();
                    }

                    Console.WriteLine("Press 'u' to update the distance or 'q' to exit");
                }
                while ((line = Console.ReadLine()) != null && line != "q");
            }
        }
    }

    internal class DistanceLocationService : DistanceLocationServiceBase, IVdv301ServiceImpl
    {
        private double distance;

        private int pulses;

        public void Update()
        {
            this.distance += System.Math.PI;
            this.pulses += 17;

            Console.WriteLine("Distance is now {0:0.00} m ({1} pulses)", this.distance, this.pulses);

            this.RaiseDistanceLocationChanged(
                new DataUpdateEventArgs<DistanceLocationServiceDataStructure>(
                    new DistanceLocationServiceDataStructure
                        {
                            Distance = new IBISIPdouble(this.distance),
                            OdometerPulses = new IBISIPint(this.pulses)
                        }));
        }
    }

    internal class DeviceManagementService : DeviceManagementServiceBase, IVdv301ServiceImpl
    {
        private DeviceStateEnumeration deviceState;

        public DeviceManagementService()
        {
            this.deviceState = DeviceStateEnumeration.running;
        }

        public DeviceStateEnumeration DeviceState
        {
            get
            {
                return this.deviceState;
            }

            set
            {
                if (this.deviceState == value)
                {
                    return;
                }

                this.deviceState = value;
                this.RaiseDeviceStatusChanged();
            }
        }

        public override DeviceManagementServiceGetDeviceInformationResponseDataStructure GetDeviceInformation()
        {
            var info = new DeviceManagementServiceGetDeviceInformationResponseDataStructure();
            info.TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now);
            info.DeviceInformation = new DeviceInformationStructure
                                         {
                                             DataVersionList = new[]
                                                 {
                                                     new DataVersionStructure
                                                         {
                                                             DataType = new IBISIPstring("My Data"),
                                                             VersionRef = new IBISIPNMTOKEN("1.0.255.15")
                                                         }
                                                 },
                                             DeviceClass = DeviceClassEnumeration.InteriorDisplay,
                                             DeviceName = new IBISIPstring(ApplicationHelper.MachineName),
                                             Manufacturer = new IBISIPstring("Gorba"),
                                             SerialNumber = new IBISIPNMTOKEN("123456789")
                                         };
            return info;
        }

        public override DeviceManagementServiceGetDeviceConfigurationResponseDataStructure GetDeviceConfiguration()
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure SetDeviceConfiguration(IBISIPint deviceID)
        {
            throw new System.NotImplementedException();
        }

        public override DeviceManagementServiceGetDeviceStatusResponseDataStructure GetDeviceStatus()
        {
            return new DeviceManagementServiceGetDeviceStatusResponseDataStructure
                       {
                           TimeStamp = new IBISIPdateTime(DateTime.Now),
                           DeviceState = this.DeviceState
                       };
        }

        public override DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure GetDeviceErrorMessages()
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure RestartDevice()
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure DeactivateDevice()
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure ActivateDevice()
        {
            throw new System.NotImplementedException();
        }

        public override DeviceManagementServiceGetServiceInformationResponseDataStructure GetServiceInformation()
        {
            var data = new DeviceManagementServiceGetServiceInformationResponseDataStructure();
            data.TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now);
            data.ServiceInformationList = new[]
                {
                    new ServiceInformationStructure
                        {
                            Autostart = new IBISIPboolean(true),
                            Service =
                                new ServiceSpecificationStructure
                                    {
                                        IBISIPVersion = new IBISIPNMTOKEN("1.0"),
                                        ServiceName = ServiceNameEnumeration.DeviceManagementService
                                    }
                        }
                };
            return data;
        }

        public override DeviceManagementServiceGetServiceStatusResponseDataStructure GetServiceStatus()
        {
            var data = new DeviceManagementServiceGetServiceStatusResponseDataStructure();
            data.TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now);
            data.ServiceSpecificationWithStateList = new[]
                {
                    new ServiceSpecificationWithStateStructure
                        {
                            ServiceState = ServiceStateEnumeration.running,
                            ServiceSpecification =
                                new ServiceSpecificationStructure
                                    {
                                        IBISIPVersion = new IBISIPNMTOKEN("1.0"),
                                        ServiceName = ServiceNameEnumeration.DeviceManagementService
                                    }
                        }
                };
            return data;
        }

        public override DataAcceptedResponseDataStructure StartService(ServiceSpecificationStructure serviceSpecification)
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure RestartService(ServiceSpecificationStructure serviceSpecification)
        {
            throw new System.NotImplementedException();
        }

        public override DataAcceptedResponseDataStructure StopService(ServiceSpecificationStructure serviceSpecification)
        {
            throw new System.NotImplementedException();
        }
    }
}
