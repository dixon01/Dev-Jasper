// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Vdv301CustomerInformationService
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;

    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Common.IbisIP.Server;

    using NLog;

    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => Logger.FatalException("Unhandled Exception", args.ExceptionObject as Exception);

            using (var httpServer = new IbisHttpServer(new IPEndPoint(IPAddress.Any, 0)))
            {
                var service = new CustomerInformationService();

                var path = httpServer.AddService(service);

                httpServer.Start();
                Console.WriteLine("Started server on port {0}", httpServer.LocalPort);

                IDnsSdProvider dnsSdProvider = new AutoDnsSdProvider();
                dnsSdProvider.Start();

                var attributes = new Dictionary<string, string> { { "path", path }, { "ver", "1.0" } };

                dnsSdProvider.RegisterService(
                    "CustomerInformationService", Definitions.HttpProtocol, httpServer.LocalPort, attributes);

                string line = null;
                do
                {
                    if (line == "c")
                    {
                        service.UpdateStopIndex();
                        Console.WriteLine(
                            "Updated stop index to {0}", service.GetCurrentStopIndex().CurrentStopIndex.Value);
                    }

                    Console.WriteLine("Press 'c' to change the stop index or 'q' to exit");
                }
                while ((line = Console.ReadLine()) != null && line != "q");

                dnsSdProvider.Stop();
            }
        }

        private class CustomerInformationService : CustomerInformationServiceBase, IVdv301ServiceImpl
        {
            private const int StopCount = 10;
            private int currentStopIndex = 1;

            public void UpdateStopIndex()
            {
                this.currentStopIndex++;
                if (this.currentStopIndex > StopCount)
                {
                    this.currentStopIndex = 1;
                }

                this.RaiseAllDataChanged();
                this.RaiseCurrentConnectionInformationChanged();
                this.RaiseCurrentStopIndexChanged();
                this.RaiseCurrentStopPointChanged();
            }

            public override CustomerInformationServiceAllData GetAllData()
            {
                return CreateAllDataWithConnections(this.currentStopIndex);
            }

            public override CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement()
            {
                return new CustomerInformationServiceCurrentAnnouncementData
                    {
                        CurrentAnnouncement = new AnnouncementStructure
                            {
                                AnnouncementRef = new IBISIPNMTOKEN("ANN-0002"),
                                AnnouncementText = new[]
                                            {
                                                new InternationalTextType
                                                    {
                                                        Language = "de",
                                                        Value = "Endstation, bitte alle aussteigen"
                                                    },
                                                new InternationalTextType
                                                    {
                                                        Language = "fr",
                                                        Value = "Terminus, tout le monde descendre, s'il vous plaît"
                                                    },
                                                new InternationalTextType
                                                    {
                                                        Language = "en",
                                                        Value = "Terminal stop, please exit"
                                                    }
                                            }
                            }
                    };
            }

            public override CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation()
            {
                return new CustomerInformationServiceCurrentConnectionInformationData
                    {
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now)
                    };
            }

            public override CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent()
            {
                return new CustomerInformationServiceCurrentDisplayContentData
                    {
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now)
                    };
            }

            public override CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint()
            {
                var all = this.GetAllData();
                foreach (var stop in all.TripInformation[0].StopSequence)
                {
                    if (stop.StopIndex.Value == this.currentStopIndex)
                    {
                        return new CustomerInformationServiceCurrentStopPointData
                            {
                                TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now),
                                CurrentStopPoint = stop
                            };
                    }
                }

                return new CustomerInformationServiceCurrentStopPointData
                    {
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now)
                    };
            }

            public override CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex()
            {
                return new CustomerInformationServiceCurrentStopIndexData
                    {
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now),
                        CurrentStopIndex = new IBISIPint(this.currentStopIndex)
                    };
            }

            public override CustomerInformationServiceTripData GetTripData()
            {
                var all = this.GetAllData();
                return new CustomerInformationServiceTripData
                    {
                        CurrentStopIndex = all.CurrentStopIndex,
                        DefaultLanguage = new IBISIPlanguage { Value = "de" },
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now),
                        TripInformation = all.TripInformation[0]
                    };
            }

            public override CustomerInformationServiceVehicleData GetVehicleData()
            {
                var all = this.GetAllData();
                return new CustomerInformationServiceVehicleData
                    {
                        DoorState = all.DoorState,
                        DoorStateSpecified = all.DoorStateSpecified,
                        ExitSide = all.ExitSide,
                        ExitSideSpecified = all.ExitSideSpecified,
                        InPanic = all.InPanic,
                        MovingDirectionForward = all.MovingDirectionForward,
                        RouteDeviation = all.RouteDeviation,
                        TimeStamp = new IBISIPdateTime(TimeProvider.Current.Now),
                        VehicleMode = all.VehicleMode,
                        VehicleModeSpecified = all.VehicleModeSpecified,
                        VehicleRef = all.VehicleRef,
                        VehicleStopRequested = all.VehicleStopRequested
                    };
            }

            public override CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(
                IBISIPint startingStopIndex, IBISIPint numberOfStopPoints)
            {
                throw new NotImplementedException();
            }

            private static CustomerInformationServiceAllData CreateAllData(int stopIndex)
            {
                var stopSequence = new StopInformationStructure[StopCount];
                var time = new DateTime(2013, 12, 12, 17, 35, 00);
                for (int i = 0; i < stopSequence.Length; i++)
                {
                    stopSequence[i] = new StopInformationStructure
                    {
                        StopIndex = new IBISIPint(i + 1),
                        ArrivalScheduled = new IBISIPdateTime(time.AddMinutes(i * 2)),
                        DepartureScheduled = new IBISIPdateTime(time.AddMinutes((i * 2) + 1)),
                        Platform = new IBISIPstring(GetAlphaString(i)),
                        StopName = CreateInternationalTexts(
                            "Haltestelle " + (i + 1), "Arrêt " + (i + 1), "Stop " + (i + 1)),
                        DisplayContent = new[]
                            {
                                new DisplayContentStructure
                                    {
                                        Destination = new DestinationStructure
                                            {
                                                DestinationName = CreateInternationalTexts(
                                                    "Ziel " + (i + 1),
                                                    "Destination " + (i + 1),
                                                    "Destination " + (i + 1))
                                            }
                                    }
                            }
                    };
                }

                var allData = new CustomerInformationServiceAllData
                {
                    TripInformation =
                        new[]
                            {
                                new TripInformationStructure
                                    {
                                        LocationState = LocationStateEnumeration.AtStop,
                                        LocationStateSpecified = true,
                                        StopSequence = stopSequence
                                    }
                            },
                    VehicleStopRequested = new IBISIPboolean(false),
                    DoorState = DoorOpenStateEnumeration.AllDoorsClosed,
                    DoorStateSpecified = true,
                    ExitSide = ExitSideEnumeration.right,
                    ExitSideSpecified = true,
                    CurrentStopIndex = new IBISIPint(stopIndex)
                };
                return allData;
            }

            private static CustomerInformationServiceAllData CreateAllDataWithConnections(int stopIndex)
            {
                var data = CreateAllData(stopIndex);
                var time = new DateTime(2013, 12, 12, 17, 55, 00);
                foreach (var stop in data.TripInformation[0].StopSequence)
                {
                    var index = stop.StopIndex.Value;
                    stop.Connection = new ConnectionStructure[3 - (index % 4)];
                    for (int i = 0; i < stop.Connection.Length; i++)
                    {
                        var nonRandom = ((index * 7) + (i * 13)) % 11;
                        stop.Connection[i] = new ConnectionStructure
                        {
                            ConnectionState = (ConnectionStateEnumeration)(nonRandom % 3),
                            ConnectionStateSpecified = true,
                            ConnectionType = (ConnectionTypeEnumeration)(nonRandom % 2),
                            Platform = new IBISIPstring("Platform " + GetAlphaString(nonRandom)),
                            TransportMode = new VehicleStructure
                            {
                                Name = CreateInternationalTexts("Schiff", "Bateau", "Ship")
                            },
                            DisplayContent = new DisplayContentStructure
                            {
                                Destination = new DestinationStructure
                                {
                                    DestinationName = CreateInternationalTexts(
                                        "Z" + nonRandom, "D" + nonRandom, "D" + nonRandom)
                                }
                            },
                            ExpectedDepatureTime = new IBISIPdateTime(time.AddMinutes(nonRandom))
                        };
                    }
                }

                return data;
            }

            private static InternationalTextType[] CreateInternationalTexts(
                string german, string french, string english)
            {
                return new[]
                       {
                           new InternationalTextType { Language = "de", Value = german },
                           new InternationalTextType { Language = "fr", Value = french },
                           new InternationalTextType { Language = "en", Value = english }
                       };
            }

            private static string GetAlphaString(int offset)
            {
                return ((char)('A' + offset)).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
