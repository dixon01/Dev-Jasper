using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminator.Multicast.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.Multicast.Core.Tests
{
    [TestClass()]
    public class AdaptorNetworkInfoTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            AdaptorNetworkInfo adaptorNetwork = new AdaptorNetworkInfo();
            Console.WriteLine(adaptorNetwork);
        }

        [TestMethod]
        public void ToXmlTest()
        {
            AdaptorNetworkInfo adaptorNetwork = new AdaptorNetworkInfo();
            Console.WriteLine(adaptorNetwork.ToXml(adaptorNetwork, adaptorNetwork.GetType()));
        }

        [TestMethod]
        public void AdatorNetworkInfoLocalTest()
        {
            var physicalNics = NetworkUtils.GetAllActiveLocalPhysicalNics();
            foreach (var networkInterface in physicalNics)
            {
                var adaptorDescriptionOrName = networkInterface.Description;
                var nai = new AdaptorNetworkInfo
                              {
                                  Description = adaptorDescriptionOrName,
                                  IpAddress = NetworkUtils.GetAdapterIp4Address(adaptorDescriptionOrName),
                                  Gateway = NetworkManagement.GetGatewayForAdaptor(adaptorDescriptionOrName),
                                  IsDhcpEnabled = !NetworkManagement.IsNetworkedWithStaticIp(adaptorDescriptionOrName),
                                  Name = networkInterface.Name
                              };

                Console.WriteLine(nai.ToString());
            }
        }
    }
}