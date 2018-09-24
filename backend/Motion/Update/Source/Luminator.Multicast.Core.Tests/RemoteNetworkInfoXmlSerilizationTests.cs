using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Luminator.Multicast.Core.Tests
{
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;

    [TestClass]
    public class RemoteNetworkInfoXmlSerilizationTests
    {
        [TestMethod]
        public void XmlSerializationTest()
        {
            RemoteNetworkInfo remoteNetworkInfo = new RemoteNetworkInfo
                                                      {
                                                          Host = "192.168.1.6",
                                                          Password = "Hello",
                                                          Username = "Luminator",
                                                          SubnetMask = IPAddress.Parse("255.255.255.234")
                                                      };
            XmlSerializer xs = new XmlSerializer(typeof(RemoteNetworkInfo));
            Assert.IsNotNull(xs,$"Could not intialize the XML serializer {nameof(remoteNetworkInfo)}");
            TextWriter tw = new StreamWriter(@".\remoteNetworkInfo.xml");
            xs.Serialize(tw, remoteNetworkInfo);
            tw.Close();

            RemoteNetworkInfo remoteNetworkInfo1;
            using (var sr = new StreamReader(@".\remoteNetworkInfo.xml"))
            {
                remoteNetworkInfo1 = (RemoteNetworkInfo)xs.Deserialize(sr);
                Console.WriteLine(remoteNetworkInfo);
                Console.WriteLine(remoteNetworkInfo1);
                Assert.AreEqual(remoteNetworkInfo1.Host,remoteNetworkInfo.Host);
                Assert.AreEqual(remoteNetworkInfo1.Password, remoteNetworkInfo.Password);
                Assert.AreEqual(remoteNetworkInfo1.RemoteIpAddress, remoteNetworkInfo.RemoteIpAddress);
                Assert.AreEqual(remoteNetworkInfo1.Username, remoteNetworkInfo.Username);
                Assert.AreEqual(remoteNetworkInfo1.SubnetMask, remoteNetworkInfo.SubnetMask);
                Assert.AreEqual(remoteNetworkInfo1.UpdateComplete, remoteNetworkInfo.UpdateComplete);
            }

        }

        [TestMethod]
        public void SaveTest()
        {
            RemoteNetworkInfo remoteNetworkInfo = new RemoteNetworkInfo
            {
                Host = "192.168.1.6",
                Password = "Hello",
                Username = "Luminator",
                SubnetMask = IPAddress.Parse("255.255.255.234"),
                UpdateComplete = false
            };

           var result = remoteNetworkInfo.Save(@".\remoteNetworkInfo.xml");
            Assert.IsTrue(result);


        }
        [Ignore]
        [TestMethod]
        public void LoadTest()
        {
            RemoteNetworkInfo remoteNetworkInfo = new RemoteNetworkInfo
            {
                Host = "192.168.1.6",
                Password = "Hello",
                Username = "Luminator",
                SubnetMask = IPAddress.Parse("255.255.255.234"),
                UpdateComplete = false
            };
            RemoteNetworkInfo test = new RemoteNetworkInfo();
            test = test.Load(@".\remoteNetworkInfo.xml");
            Assert.AreEqual(test.Host, remoteNetworkInfo.Host);
            Assert.AreEqual(test.Password, remoteNetworkInfo.Password);
            Assert.AreEqual(test.RemoteIpAddress, remoteNetworkInfo.RemoteIpAddress);
            Assert.AreEqual(test.Username, remoteNetworkInfo.Username);
            Assert.AreEqual(test.SubnetMask, remoteNetworkInfo.SubnetMask);
            Assert.AreEqual(test.UpdateComplete, remoteNetworkInfo.UpdateComplete);
        }

        [Ignore]
        [TestMethod]
        private void TestMulticastManagerOnRemoteNetworkInfoChanged()
        {
            RemoteNetworkInfo networkInfo = new RemoteNetworkInfo();
          //  if (this.multicastManager != null)
            {
                RemoteNetworkInfo remoteInfo = this.CreateMockRemoteInfo();
                networkInfo.Password = remoteInfo.Password;
                networkInfo.RemoteIpAddress = remoteInfo.RemoteIpAddress;
                networkInfo.Username = remoteInfo.Username;

          //      this.MulticastManagerOnRemoteNetworkInfoChanged(this, networkInfo);
            }
        }

   
        private RemoteNetworkInfo CreateMockRemoteInfo()
        {
            var multicastConfig = new MulticastConfig();
            var remoteNetworkInfo = new RemoteNetworkInfo();
            remoteNetworkInfo.Password = multicastConfig.FtpPassword;
            remoteNetworkInfo.Username = multicastConfig.FtpUsername;
            remoteNetworkInfo.RemoteIpAddress = IPAddress.Parse("192.1.2.3");
            return remoteNetworkInfo;
        }



    }
}
