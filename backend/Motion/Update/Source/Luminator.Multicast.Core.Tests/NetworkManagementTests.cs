using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminator.Multicast.Core;

namespace Luminator.Multicast.Core.Tests
{
    using System;
    using System.Net.NetworkInformation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NetworkManagementTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void IsNetworkedStatusDownTest()
        {
            Console.WriteLine(NetworkManagement.IsNetworkedStatusDown());
        }

        [TestMethod]
        public void IsNetworkedWithStaticIpTest()
        {
            var result = NetworkManagement.IsNetworkedWithStaticIp();
            Console.WriteLine(result);
        }

        [Ignore]
        [TestMethod]
        public void SetSystemDnsTest()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void SetSystemGatewayTest()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void SetSystemIpTest()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void SetSystemWinsTest()
        {
            throw new NotImplementedException();
        }

        #endregion

        [TestMethod]
        public void IsNetworkedWithStaticIpTest_NameOrDesc()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in adapters)
            {
                Console.WriteLine(networkInterface.Name + " : " +
                                    networkInterface.Description + " : " +
                    NetworkManagement.IsNetworkedWithStaticIp(networkInterface.Name));
            }
        }

        [TestMethod()]
        public void GetGatewayForAdaptorTest()
        {
            var adapters = NetworkUtils.GetAllActiveLocalNics();
            foreach (var networkInterface in adapters)
            {
                Console.WriteLine(networkInterface.Name + " : " +
                                    networkInterface.Description + " : " +
                    NetworkManagement.GetGatewayForAdaptor(networkInterface.Description));
            }

        }
    }
}