// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteSntpServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A class that holds the information needed to connect to a remote NTP/SNTP server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System.ComponentModel;
    using System.Net;

    /// <summary>
    /// A class that holds the information needed to connect to a remote NTP/SNTP server.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RemoteSntpServer
    {
        /// <summary>
        /// The default server host name.
        /// </summary>
        public const string DefaultHostName = "time.nist.gov";

        /// <summary>
        /// The default port number for a NTP/SNTP server.
        /// </summary>
        public const int DefaultPort = 123;

        /// <summary>
        /// A remote NTP/SNTP server configured with the default values.
        /// </summary>
        public static readonly RemoteSntpServer Default = new RemoteSntpServer();

        /// <summary>
        /// African RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Africa = new RemoteSntpServer("africa.pool.ntp.org");

        /// <summary>
        /// Apple Europe.
        /// </summary>
        public static readonly RemoteSntpServer AppleEurope = new RemoteSntpServer("time1.euro.apple.com");

        /// <summary>
        /// Apple Europe (2).
        /// </summary>
        public static readonly RemoteSntpServer AppleEurope2 = new RemoteSntpServer("time.euro.apple.com");

        /// <summary>
        /// Asian RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Asia = new RemoteSntpServer("asia.pool.ntp.org");

        /// <summary>
        /// An array of Asian RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] AsiaServers = new[]
                                                                    {
                                                                        new RemoteSntpServer("0.asia.pool.ntp.org"),
                                                                        new RemoteSntpServer("1.asia.pool.ntp.org"),
                                                                        new RemoteSntpServer("2.asia.pool.ntp.org"),
                                                                        new RemoteSntpServer("3.asia.pool.ntp.org")
                                                                    };

        /// <summary>
        /// Australian RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Australia = new RemoteSntpServer("au.pool.ntp.org");

        /// <summary>
        /// An array of Australian RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] AustraliaServers = new[]
                                                                         {
                                                                             new RemoteSntpServer("0.au.pool.ntp.org"),
                                                                             new RemoteSntpServer("1.au.pool.ntp.org"),
                                                                             new RemoteSntpServer("2.au.pool.ntp.org"),
                                                                             new RemoteSntpServer("3.au.pool.ntp.org")
                                                                         };

        /// <summary>
        /// Blue Yonder UK (Virgin Media).
        /// </summary>
        public static readonly RemoteSntpServer BlueYonder = new RemoteSntpServer("ntp.blueyonder.co.uk");

        /// <summary>
        /// Canadian RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Canada = new RemoteSntpServer("ca.pool.ntp.org");

        /// <summary>
        /// An array of Canadian RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] CanadaServers = new[]
                                                                      {
                                                                          new RemoteSntpServer("0.ca.pool.ntp.org"),
                                                                          new RemoteSntpServer("1.ca.pool.ntp.org"),
                                                                          new RemoteSntpServer("2.ca.pool.ntp.org"),
                                                                          new RemoteSntpServer("3.ca.pool.ntp.org")
                                                                      };

        /// <summary>
        /// European RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Europe = new RemoteSntpServer("europe.pool.ntp.org");

        /// <summary>
        /// An array of European RemoteSNTPServers
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] EuropeServers = new[]
                                                                      {
                                                                          new RemoteSntpServer("0.europe.pool.ntp.org"),
                                                                          new RemoteSntpServer("1.europe.pool.ntp.org"),
                                                                          new RemoteSntpServer("2.europe.pool.ntp.org"),
                                                                          new RemoteSntpServer("3.europe.pool.ntp.org")
                                                                      };

        /// <summary>
        /// The Microsoft (Redmond, Washington) RemoteSNTPServer (<c>time-nw.nist.gov</c>).
        /// </summary>
        public static readonly RemoteSntpServer Microsoft = new RemoteSntpServer("time-nw.nist.gov");

        /// <summary>
        /// North American RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer NorthAmerica = new RemoteSntpServer("north-america.pool.ntp.org");

        /// <summary>
        /// An array of North American RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] NorthAmericaServers = new[]
                                                                            {
                                                                                new RemoteSntpServer(
                                                                                    "0.north-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "1.north-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "2.north-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "3.north-america.pool.ntp.org")
                                                                            };

        /// <summary>
        /// NTL UK (Virgin Media).
        /// </summary>
        public static readonly RemoteSntpServer Ntl = new RemoteSntpServer("time.cableol.net");

        /// <summary>
        /// Pacific RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Oceania = new RemoteSntpServer("oceania.pool.ntp.org");

        /// <summary>
        /// An array of Pacific RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] OceaniaServers = new[]
                                                                    {
                                                                        new RemoteSntpServer("0.oceania.pool.ntp.org"),
                                                                        new RemoteSntpServer("1.oceania.pool.ntp.org"),
                                                                        new RemoteSntpServer("2.oceania.pool.ntp.org"),
                                                                        new RemoteSntpServer("3.oceania.pool.ntp.org")
                                                                    };

        /// <summary>
        /// General RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer Pool = new RemoteSntpServer("pool.ntp.org");

        /// <summary>
        /// An array of general RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] PoolServers = new[]
                                                                    {
                                                                        new RemoteSntpServer("0.pool.ntp.org"),
                                                                        new RemoteSntpServer("1.pool.ntp.org"),
                                                                        new RemoteSntpServer("2.pool.ntp.org")
                                                                    };

        /// <summary>
        /// South American RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer SouthAmerica = new RemoteSntpServer("south-america.pool.ntp.org");

        /// <summary>
        /// An array of South American RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] SouthAmericaServers = new[]
                                                                            {
                                                                                new RemoteSntpServer(
                                                                                    "0.south-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "1.south-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "2.south-america.pool.ntp.org"),
                                                                                new RemoteSntpServer(
                                                                                    "3.south-america.pool.ntp.org")
                                                                            };

        /// <summary>
        /// A United Kingdom GPS Primary server.
        /// </summary>
        public static readonly RemoteSntpServer UkJanetGps = new RemoteSntpServer("ntp1.ja.net");

        /// <summary>
        /// United Kingdom RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer UnitedKingdom = new RemoteSntpServer("uk.pool.ntp.org");

        /// <summary>
        /// An array of UK RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] UnitedKingdomServers = new[]
        {
            new RemoteSntpServer("0.uk.pool.ntp.org"),
            new RemoteSntpServer("1.uk.pool.ntp.org"),
            new RemoteSntpServer("2.uk.pool.ntp.org"),
            new RemoteSntpServer("3.uk.pool.ntp.org")
        };

        /// <summary>
        /// US RemoteSNTPServer.
        /// </summary>
        public static readonly RemoteSntpServer UnitedStates = new RemoteSntpServer("us.pool.ntp.org");

        /// <summary>
        /// An array of US RemoteSNTPServers.
        /// See <see cref="http://www.pool.ntp.org"/>
        /// </summary>
        public static readonly RemoteSntpServer[] UnitedStatesServers = new[]
        {
            new RemoteSntpServer("0.us.pool.ntp.org"),
            new RemoteSntpServer("1.us.pool.ntp.org"),
            new RemoteSntpServer("2.us.pool.ntp.org"),
            new RemoteSntpServer("3.us.pool.ntp.org")
        };

        /// <summary>
        /// U.S. Naval Observatory.
        /// </summary>
        public static readonly RemoteSntpServer USNavalObservatory = new RemoteSntpServer("tock.usno.navy.mil");

        /// <summary>
        /// U.S. Naval Observatory (2).
        /// </summary>
        public static readonly RemoteSntpServer USNavalObservatory2 = new RemoteSntpServer("tick.usno.navy.mil");

        /// <summary>
        /// U.S. Naval Observatory (3).
        /// </summary>
        public static readonly RemoteSntpServer USNavalObservatory3 = new RemoteSntpServer("ntp1.usno.navy.mil");

        /// <summary>
        /// A United States GPS Primary server.
        /// </summary>
        public static readonly RemoteSntpServer UsxMissionGps = new RemoteSntpServer("clock.xmission.com");

        /// <summary>
        /// The Microsoft Windows RemoteSNTPServer (time.windows.com).
        /// </summary>
        public static readonly RemoteSntpServer Windows = new RemoteSntpServer("time.windows.com");

        private string hostNameOrAddress;
        private int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSntpServer"/> class.
        /// </summary>
        /// <param name="hostNameOrAddress">
        /// The host name or address of the server.
        /// </param>
        /// <param name="port">
        /// The port to use (normally 123).
        /// </param>
        public RemoteSntpServer(string hostNameOrAddress, int port)
        {
            this.HostNameOrAddress = hostNameOrAddress;
            this.Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSntpServer"/> class.
        /// </summary>
        /// <param name="hostNameOrAddress">
        /// The host name or address of the server.
        /// </param>
        public RemoteSntpServer(string hostNameOrAddress)
            : this(hostNameOrAddress, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSntpServer"/> class.
        /// </summary>
        public RemoteSntpServer()
            : this(DefaultHostName, DefaultPort)
        {
        }

        /// <summary>
        /// Gets or sets the host name or address of the server.
        /// </summary>
        [DefaultValue(DefaultHostName)]
        public string HostNameOrAddress
        {
            get
            {
                return this.hostNameOrAddress;
            }

            set
            {
                value = value.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    value = DefaultHostName;
                }

                this.hostNameOrAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the port number that this server uses.
        /// </summary>
        [DefaultValue(DefaultPort)]
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.port = value >= 0 && value <= 65535 ? value : DefaultPort;
            }
        }

        /// <summary>
        /// Attempts to get the System.Net.IEPEndPoint of this server.
        /// </summary>
        /// <returns>The System.Net.IEPEndPoint of this server.</returns>
        public IPEndPoint GetIPEndPoint()
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(HostNameOrAddress, out ipAddress))
            {
                return new IPEndPoint(ipAddress, Port);
            }
            else
            {
                return new IPEndPoint(Dns.GetHostEntry(this.HostNameOrAddress).AddressList[0], this.Port);
            }
        }

        /// <summary>
        /// Returns the host name, IP address and port number of this server.
        /// </summary>
        /// <returns>The host name, IP address and port number of this server.</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.HostNameOrAddress, this.Port);
        }
    }
}
