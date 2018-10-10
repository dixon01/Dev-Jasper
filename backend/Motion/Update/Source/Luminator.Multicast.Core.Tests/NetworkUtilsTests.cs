using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminator.Multicast.Core;

namespace Luminator.Multicast.Core.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NetworkUtilsTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void ActiveNetworkInterfaceTest()
        {
            Console.WriteLine(NetworkUtils.ActiveNetworkInterface());
        }

        [TestMethod]
        public void DecrementIpAddressTest()
        {
            var result = NetworkUtils.DecrementIpAddress(IPAddress.Parse("127.1.1.5"));
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ToString(), "127.1.1.4");
        }

        [TestMethod]
        public void GetAdapterDescFromIp4AddressTest()
        {
            var result = NetworkUtils.GetAllLocalIpAddress();
            result.ForEach(x => Console.WriteLine(x + " : " + NetworkUtils.GetAdapterDescFromIp4Address(x)));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAdapterNameFromIp4AddressTest()
        {
            var result = NetworkUtils.GetAllLocalIpAddress();
            result.ForEach(x => Console.WriteLine("Adaptor Name : " + NetworkUtils.GetAdapterNameFromIp4Address(x)));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAllActiveLocalIpAddressTest()
        {
            var allActiveLocalIpAddress = NetworkUtils.GetAllActiveLocalIpAddress();
            allActiveLocalIpAddress.ForEach(x => Console.WriteLine("All Active Ips => " + x));
        }

        [TestMethod]
        public void GetAllLocalIpAddressTest()
        {
            var result = NetworkUtils.GetAllLocalIpAddress();
            result.ForEach(x => Console.WriteLine("Ipaddress + " + x));
            Assert.IsNotNull(result);
        }

       
        [TestMethod]
        public void GetBestNetworkAdaptorForMulticastTest()
        {
            var v = NetworkUtils.GetBestNetworkAdaptorForMulticast();
            Console.WriteLine(v);
            Assert.AreNotEqual(v, -1);
          
        }

        [TestMethod]
        public void GetBestNetworkAdaptorIpForMulticastTest()
        {
            var bestNetworkAdaptorIpForMulticast = NetworkUtils.GetBestNetworkAdaptorIpForMulticast();
            Console.WriteLine(bestNetworkAdaptorIpForMulticast);
        }

        [TestMethod]
        public void GetLocalIpAddrTest()
        {
            var ip = NetworkUtils.GetLocalIpAddress();
            Console.WriteLine(ip);
            Assert.IsNotNull(ip);
        }

        [TestMethod]
        public void GetPhysicalIpAdressTest()
        {
            var physicalIpAdress = NetworkUtils.GetPhysicalIpAdress();
            physicalIpAdress.ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void IncrementTest()
        {
            var result = NetworkUtils.IncrementIpAddress(IPAddress.Parse("127.1.1.1"));
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ToString(), "127.1.1.2");
        }

        [TestMethod]
        public void IsNetworkAvailableTest()
        {
            var result = NetworkUtils.IsNetworkAvailable();
            Console.WriteLine(result);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsReachableUriTest()
        {
            var result = NetworkUtils.IsReachableUri("http://www.google.com");
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void IsReachableUriTestForFtp()
        {
            //cannot be used for FTP
            var result = NetworkUtils.IsReachableUri("ftp://Swdev230/");
            Assert.AreEqual(result, false);
        }


        [TestMethod]
        public void NetworkCommandTestFlushDNs()
        {
            NetworkUtils.NetworkCommand("flushdns");
        }

        [TestMethod]
        public void NetworkCommandTestFlushReleaseRenew()
        {
            NetworkUtils.NetworkCommand("flushdns");
            NetworkUtils.NetworkCommand("release");
            NetworkUtils.NetworkCommand("renew");
        }

        [TestMethod]
        public void NetworkCommandTestRelease()
        {
            NetworkUtils.NetworkCommand("release");
        }

        [TestMethod]
        public void NetworkCommandTestRenew()
        {
            NetworkUtils.NetworkCommand("renew");
        }

        #endregion

        [TestMethod()]
        public void GetAllActiveLocalPhysicalNicsTest()
        {
            NetworkUtils.GetAllActiveLocalPhysicalNics().ForEach(x => Console.WriteLine(x.Description));
        }

        [TestMethod()]
        public void PrintAllNetworkInterfaceInfoTest()
        {
            NetworkUtils.PrintAllNetworkInterfaceInfo();
        }

        [TestMethod()]
        public void GetAdaptorNetworkInfoTest()
        {
            NetworkUtils.GetAllActiveLocalPhysicalNics().ForEach(x => Console.WriteLine(MulticastManager.SaveAdaptorNetworkInfo(x.Description, IPAddress.Any, IPAddress.None)));
        }



        [TestMethod()]
        public void ShouldGetNextIp()
        {
           var address = NetworkUtils.GetNextIpAddress("123.14.1.100", 6);
            Assert.AreNotEqual("123.14.1.100", NetworkUtils.GetNextIpAddress("123.14.1.100", 6));
            Assert.AreNotEqual("0.0.0.0", NetworkUtils.GetNextIpAddress("255.255.255.255", 1));
        }

        [TestMethod()]
        public void PingHostLocalTest()
        {
            var result = NetworkUtils.PingHost("localhost");
            Assert.IsTrue(result, "Local host ping fail - something is seriosly wrong");

        }


        [TestMethod()]
        public void PingHostTest127_0_0_1()
        {
            var result = NetworkUtils.PingHost("127.0.0.1");
            Assert.IsTrue(result, "127.0.0.1 host ping fail - something is seriosly wrong");

        }

        [TestMethod()]
        public void PingHostTestInvalidIP()
        {
            var result = NetworkUtils.PingHost("169.0.0.1");
            Assert.IsFalse(result, "127.0.0.1 host ping fail - something is seriosly wrong");

        }

        [TestMethod()]
        public void PingHostTestGoogle()
        {
            var result = NetworkUtils.PingHost("www.Google.com");
            Assert.IsTrue(result, "Google host ping fail - something is seriosly wrong");

        }

        [TestMethod]
        public void StopWatchElaspedTimeTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(2059);
            stopwatch.Stop();
            Console.WriteLine($" for loop took {stopwatch.Elapsed.TotalSeconds:##.000} seconds");
        }

        [TestMethod]
        public void GetIpAddressLastOctet()
        {
            var inc = IPAddress.Parse("192.168.2.10").GetIpAddressLastOctet();

            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
            inc = IPAddress.Parse("192.168.2.254").GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
            inc = IPAddress.Parse("192.168.2.1").GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
        }

        [TestMethod]
        public void GetIpAddressStringLastOctet()
        {
            var inc = "192.168.2.10".GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
            inc = "192.168.2.254".GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
            inc = "192.168.2.1".GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");

            inc = "192.168.2".GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");

            inc = "192".GetIpAddressLastOctet();
            Console.WriteLine(inc);
            Assert.IsTrue(inc > 0, MethodBase.GetCurrentMethod().Name + " Failed");
        }

        [TestMethod()]
        public void GetArpsTest()
        {
            var result = NetworkUtils.GetArp();
           result.ForEach(x => Console.WriteLine($" Arps - {x}"));
           
          
        }

        [TestMethod()]
        public void IncrementIPTest()
        {
            var ipAddress = IPAddress.Parse("10.10.210.5");
            var result =  NetworkUtils.IncrementIpAddress(ipAddress, 6);
            Console.WriteLine(result);
            Assert.AreEqual(IPAddress.Parse("10.10.210.11"), result);
        }

        [TestMethod()]
        public void CheckForFtpServerTest()
        {
            string ftpUrl = "ftp://swdev234/";
            string user = "Gorba";
            string pass = "Asdf1234";
            var result = NetworkUtils.CheckForFtpServer(ftpUrl, user, pass);
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void GetFreeIpTest()
        {
            var result = NetworkUtils.GetArp();
          //  result.ForEach(x => Console.WriteLine($" Arps - {x}"));
            IPAddress ip =  IPAddress.Parse("192.168.135.7");
            for (int i = 0; i < 255; i++)
            {
                ip = NetworkUtils.IncrementIpAddress(ip);
                if (!result.Contains(ip.ToString()))
                    Console.WriteLine($"Free Ip: {ip}");
            }

        }

        [TestMethod()]
        public void GetFreeIpTest2()
        {
            var ipString = "192.168.135.23";
            var ipAddress = IPAddress.Parse(ipString);
            var result = NetworkUtils.GetNextFreeIpAddress(ipAddress);
            Console.WriteLine(result);
            Assert.AreNotEqual(result, ipAddress);
        }

        [TestMethod()]
        public void GetFreeIpTest3()
        {
            var ipString = "10.210.0.1";
            var result = NetworkUtils.GetNextFreeIpAddress(IPAddress.Parse(ipString));
            Console.WriteLine(result);
            Assert.AreNotEqual(result, IPAddress.Parse(ipString));
        }

        [TestMethod()]
        public void GetFreeIpTestLocal()
        {
            var ipString = NetworkUtils.GetLocalIpAddress();
            var result = NetworkUtils.GetNextFreeIpAddress(ipString);
            Console.WriteLine(result);
            Assert.AreNotEqual(result, ipString);
        }

        [TestMethod()]
        public void PrintSubnetMasks()
        {

            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                Console.WriteLine(Interface.Description);
                UnicastIPAddressInformationCollection unicastIpInfoCol = Interface.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation unicatIpInfo in unicastIpInfoCol)
                {
                    if (unicatIpInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine("\tIP Address is {0}", unicatIpInfo.Address);
                        Console.WriteLine("\tSubnet Mask is {0}", unicatIpInfo.IPv4Mask);
                    }
                }
            }
        }

    }
}