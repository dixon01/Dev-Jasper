namespace Luminator.Multicast.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;

    using Gorba.Motion.Obc.CommonEmb;

    using NLog;
    using NLog.LayoutRenderers;

    public static class NetworkUtils
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Random randStart = new Random(DateTime.Now.Ticks.GetHashCode());

        #endregion

        #region Public Methods and Operators

        public static string ActiveNetworkInterface()
        {
            var GoogleIp = "216.58.219.238";
            var u = new UdpClient(GoogleIp, 1);
            var localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;

            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProps = nic.GetIPProperties();
                foreach (var addrinfo in ipProps.UnicastAddresses)
                {
                    if (localAddr.Equals(addrinfo.Address))
                    {
                        return nic.Description;
                    }
                }
            }
            return "Adapter not found.";
        }

        public static IPAddress DecrementIpAddress(IPAddress address)
        {
            var bytes = address.GetAddressBytes();

            for (var k = bytes.Length - 1; k >= 0; k--)
            {
                if (bytes[k] == byte.MaxValue)
                {
                    bytes[k] = 0;
                    continue;
                }

                bytes[k]--;

                var result = new IPAddress(bytes);
                return result;
            }
           Logger.Info($"cannot decrement, return the original address.");
           return address;
        }

        public static string GetAdapterDescFromIp4Address(IPAddress ipAddress)
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //Console.WriteLine(ni.Description);

                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (ipAddress.Equals(ip.Address))
                            {
                                // Console.WriteLine(ip.Address + " : " + ni.Description);
                                return ni.Description;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static IPAddress GetAdapterIp4Address(string description)
        {
            var local = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(i => i.Description == description);
            if (local != null)
            {
                var stringAddress = local.GetIPProperties().UnicastAddresses[0].Address.ToString();
                var tempIp = IPAddress.Parse(stringAddress);
                if (tempIp.AddressFamily == AddressFamily.InterNetwork)
                {
                    return tempIp;
                }
            }
            return IPAddress.None;
        }

        public static string GetAdapterNameFromIp4Address(IPAddress ipAddress)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                if (
                    host.AddressList.Any(
                        ip =>
                        (adapter.OperationalStatus.ToString() == "Up") && (ip.AddressFamily == AddressFamily.InterNetwork) && adapter.SupportsMulticast && ipAddress.Equals(ip)))
                {
                    return adapter.Name;
                }
            }
            return string.Empty;
        }

        public static List<IPAddress> GetAllActiveLocalIpAddress()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddresses = new List<IPAddress>();
            foreach (var adapter in interfaces.Where(adapter => adapter.OperationalStatus.ToString() == "Up"))
            {
                ipAddresses.AddRange(host.AddressList.Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork));
            }
            return ipAddresses.Distinct().ToList();
        }

        public static List<NetworkInterface> GetAllActiveLocalNics()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            return interfaces.Where(adapter => adapter.OperationalStatus.ToString() == "Up").ToList();
        }

        public static List<NetworkInterface> GetAllActiveLocalPhysicalNics()
        {
            return
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(
                        networkInterface =>
                        networkInterface.OperationalStatus.ToString() == "Up" && networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && !networkInterface.Description.Contains("VirtualBox"))
                    .ToList();
        }

        public static List<IPAddress> GetAllLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork).ToList();
        }

        public static int GetBestNetworkAdaptorForMulticast()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
            {
                if (!adapter.GetIPProperties().MulticastAddresses.Any())
                {
                    continue; // most of VPN adapters will be skipped
                }
                if (!adapter.SupportsMulticast)
                {
                    continue; // multicast is meaningless for this type of connection
                }
                if (OperationalStatus.Up != adapter.OperationalStatus)
                {
                    continue; // this adapter is off or not connected
                }
                var p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                {
                    continue; // IPv4 is not configured on this adapter
                }

                // This is a suitable adaptor for multicast
                //my_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(p.Index));
                return IPAddress.HostToNetworkOrder(p.Index);
            }
            return -1; // no network adaptors valid for MUltiCast
        }

        public static IPAddress GetBestNetworkAdaptorIpForMulticast()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
            {
                if (!adapter.GetIPProperties().MulticastAddresses.Any())
                {
                    continue; // most of VPN adapters will be skipped
                }
                if (!adapter.SupportsMulticast)
                {
                    continue; // multicast is meaningless for this type of connection
                }
                if (OperationalStatus.Up != adapter.OperationalStatus)
                {
                    continue; // this adapter is off or not connected
                }
                var p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                {
                    continue; // IPv4 is not configured on this adapter
                }
                foreach (var ip in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address;
                    }
                }
            }
            return IPAddress.None; // no network adaptors valid for MUltiCast
        }

        public static IPAddress GetIpAddress(this EndPoint endPoint)
        {
            return endPoint != null ? ((IPEndPoint)endPoint).Address : IPAddress.None;
        }

        public static IPAddress GetIpFromAdaptorDesc(string desc)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if ((adapter.OperationalStatus == OperationalStatus.Up) && (ip.Address.AddressFamily == AddressFamily.InterNetwork) && adapter.Description.Equals(desc))
                    {
                        // Console.Out.WriteLine(ip.Address + "|" + adapter.Description);
                        return ip.Address;
                    }
                }
            }
            return IPAddress.None;
        }

        public static IPAddress GetLocalIpAddress()
        {
            var localIpAddr = IPAddress.None; // IPAddress.Parse(Console.ReadLine());

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIpAddr = ipAddress;
                    break;
                }
            }
            return localIpAddr;
        }

        public static List<IPAddress> GetPhysicalIpAdress()
        {
            var allIps = new List<IPAddress>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                allIps.Add(ip.Address);
                            }
                        }
                    }
                }
            }
            return allIps;
        }

        public static IPAddress IncrementIpAddress(IPAddress address)
        {
            var bytes = address.GetAddressBytes();

            for (var k = bytes.Length - 1; k >= 0; k--)
            {
                if (bytes[k] == byte.MaxValue)
                {
                    bytes[k] = 0;
                    continue;
                }

                bytes[k]++;

                var result = new IPAddress(bytes);
                return result;
            }

            // Un-incrementable, return the original address.
            return address;
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                if (reply != null)
                {
                    pingable = reply.Status == IPStatus.Success;
                }
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
                Logger.Info($"Ping at {nameOrAddress} result was not response - returning false.");
            }
            return pingable;
        }

        public static bool CheckForFtpServer(string ftpUrl, string user, string pass)
        {
            var serverFtpUrl = new Uri(ftpUrl);
            Logger.Info($" Ftp Server {serverFtpUrl}");
            var ftpUtils = new FtpUtils(serverFtpUrl.ToString(), user, pass);
            var serverUriForRepository = new Uri(serverFtpUrl + "repository.xml");
            Logger.Info($" Ftp Server check for respository {serverUriForRepository}");
            bool result;
            do
            {
                result = ftpUtils.FtpFileExists(serverUriForRepository);
                Thread.Sleep(2000);
            }
            while (result == false);
            return true;
        }
        public static List<string> GetArp()
        {
            var result = new List<string>();
            try
            {
                var arpProcess = new Process
                {
                    StartInfo =
                                         {
                                             FileName = "arp.exe",
                                             CreateNoWindow = true,
                                             Arguments = "-a",
                                             RedirectStandardOutput = true,
                                             UseShellExecute = false,
                                             RedirectStandardError = true
                                         }
                };
                arpProcess.Start();
                var streamReader = new StreamReader(arpProcess.StandardOutput.BaseStream, arpProcess.StandardOutput.CurrentEncoding);
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {

                    if (line.StartsWith("  "))
                    {
                        var items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (items.Length == 3)
                        {
                            try
                            {
                                var ip = IPAddress.Parse(items[0]);// Parse to make sure we have a valid Ip - if not move on to next ip
                                result.Add(items[0]);
                            }
                            catch (Exception e)
                            {
                                Logger.Info(e.Message + e.InnerException?.Message);
                            }
                        }
                    }
                }

                streamReader.Close();
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + e.InnerException?.Message);
            }

            return result;

        }

        public static IPAddress GetNextFreeIpAddress(IPAddress ipAddress)
        {
            var result = GetArp();
            result.ForEach(x => Logger.Info($" Arp Table: - {x}"));
            IPAddress freeIpAddress = IncrementIpAddress(ipAddress, randStart.Next(2, 10));
            if (!result.Contains(freeIpAddress.ToString()))
            {
                if (!PingHost(freeIpAddress.ToString()))
                {
                    Logger.Info($" {freeIpAddress} : Ip address ping response was false. So using it.");
                   
                }
            }
            var increment = true;
            for (var i = 0; i < 255; i++)
            {
                if (GetIpAddressLastOctet(freeIpAddress) >= 254) increment = false;
                if (GetIpAddressLastOctet(freeIpAddress) <= 1) increment = true;
                Logger.Info($" Increment : {increment}");
                freeIpAddress = increment ? IncrementIpAddress(freeIpAddress) : DecrementIpAddress(freeIpAddress);
                Logger.Info($" Free IP Address : {freeIpAddress}");
                if (!result.Contains(freeIpAddress.ToString()))
                {
                    Logger.Warn($"Got a free IP adresss {freeIpAddress} pinging it...");
                    if (!PingHost(freeIpAddress.ToString()))
                    {
                        Logger.Info($" {freeIpAddress} : Ip address ping response was false. So using it.");
                        return freeIpAddress;
                    }
                }
            }
            Logger.Warn($"GetNextFreeIpAddress Failed to get a free IP address. USB updates may fail.");
            return IPAddress.Any;
        }

        public static uint GetIpAddressLastOctet(this IPAddress ipAddress)
        {
           return  ipAddress.GetAddressBytes()[3];
        }

        public static uint GetIpAddressLastOctet(this string ipAddress)
        {
            return IPAddress.Parse(ipAddress).GetAddressBytes()[3];
        }


        public static string GetNextIpAddress(string ipAddress, int increment)
        {
            byte[] addressBytes = IPAddress.Parse(ipAddress).GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            var nextAddress = BitConverter.GetBytes(ipAsUint + increment);
            return String.Join(".", nextAddress.Reverse());
        }

        public static IPAddress IncrementIpAddress(IPAddress ipAddress , int increment)
        {
            IPAddress ip = ipAddress;
            for (int i = 0; i < increment; i++)
            {
                ip = IncrementIpAddress(ip);
            }
            return ip;
        }

        public static bool IsNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            var statistics = face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface
                            // comes online it has already accrued statistics for
                            // both received and sent...

                            if ((statistics.BytesReceived > 0) && (statistics.BytesSent > 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsReachableUri(string uriInput)
        {
            bool testStatus = false;
            var request = WebRequest.Create(uriInput);
            request.Timeout = 15000; // 15 Sec
            WebResponse response;
            try
            {
                response = request.GetResponse();
                testStatus = true; // Uri does exist                 
                response?.Close();
            }
            catch (Exception exception)
            {
                testStatus = false; // Uri does not exist
                Console.WriteLine(exception.Message);
            }
            
            return testStatus;
        }

        public static void NetshReset()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " /C netsh int ip reset c:\networkdata.log";
            Console.WriteLine("NetshReset => " + startInfo.FileName + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void NetworkCommand(string parameter)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " /C ipconfig " + " /" + parameter;
            Console.WriteLine("NetworkCommand => " + startInfo.FileName + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void NetworkReset()
        {
            NetworkCommand("release");
            NetworkCommand("renew");
            NetworkCommand("flushdns");
            NetshReset();
        }

        public static void PrintAllNetworkInterfaceInfo()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in interfaces)
            {
                Console.WriteLine(networkInterface.Name + " : " + networkInterface.Description + " : " + networkInterface.NetworkInterfaceType.GetDescription());
            }
        }

        #endregion

        public static IPAddress GetSubnetMaskForIpAddress(IPAddress address)
        {

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                Console.WriteLine(Interface.Description);
                UnicastIPAddressInformationCollection unicastIpInfoCol = Interface.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation unicatIpInfo in unicastIpInfoCol)
                {
                    if (unicatIpInfo.Address.AddressFamily == AddressFamily.InterNetwork &&
                       address.Equals(unicatIpInfo.Address))
                    {
                        //Console.WriteLine("\tIP Address is {0}", unicatIpInfo.Address);
                        //Console.WriteLine("\tSubnet Mask is {0}", unicatIpInfo.IPv4Mask);
                        Logger.Info($"   Subnet Mask is {unicatIpInfo.IPv4Mask}" );
                        return unicatIpInfo.IPv4Mask;
                    }
                }
            }
            Logger.Info($"Can't find subnetmask for IP address {address}");
            return  IPAddress.None;
        }
    }
}