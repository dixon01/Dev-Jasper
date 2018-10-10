namespace Luminator.Multicast.Core.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MulticastReceiveTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void MulticastReceiveSubnetMaskTest()
        {
            const string McuResonseMessage = "MCU Infotransit Update Server, Action=Update, NetMask=255.255.255.224\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            //string McuResonseMessage = "MCU INFOtransit Update Server, Action=Update, NetMask=255.255.255.224";
            if (McuResonseMessage.Contains("Action=Update"))
            {
                const string Netmask = "NetMask=";
                var idx = McuResonseMessage.IndexOf(Netmask, StringComparison.Ordinal) + Netmask.Length;
                var ipString = McuResonseMessage.Substring(idx, McuResonseMessage.Length - idx);
                Console.WriteLine(ipString);
                var trimEnd = ipString.TrimEnd('\0');
                var subnetMask = IPAddress.Parse(trimEnd);
                Console.WriteLine(subnetMask);
                Assert.AreEqual(subnetMask.ToString(), "255.255.255.224");
            }
        }


        [TestMethod]
        public void ProcessTheReceivedMessageTest()
        {
            const string McuResonseMessage = "MCU Infotransit Update Server, Action=Update, NetMask=255.255.255.224";
            var garbageBytes = new byte[] { 0x00, 0x02, 0x00, 0x03, 0x04, 0x00, 0x00, 0x00, 0x00 };
            var testBytes = Encoding.UTF8.GetBytes(McuResonseMessage);
            var testbuffer = new byte[testBytes.Length + garbageBytes.Length];
            Array.Copy(testBytes, testbuffer, testBytes.Length);
            Array.Copy(garbageBytes, 0, testbuffer, testBytes.Length, garbageBytes.Length);
            Console.WriteLine($"Data In Bytes {BitConverter.ToString(testbuffer)}");
            var test = MulticastReceive.ProcessTheReceivedMessage(testbuffer);
            Assert.AreEqual(McuResonseMessage, test);
            Console.WriteLine(test);
        }

        #endregion
    }
}