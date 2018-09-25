// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpSerializerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Tests
{
    using System.Net;

    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="UdcpSerializer"/>.
    /// </summary>
    [TestClass]
    public class UdcpSerializerTest
    {
        /// <summary>
        /// Tests serialization and deserialization of an empty request.
        /// </summary>
        [TestMethod]
        public void TestEmptyRequest()
        {
            var unitAddress = new UdcpAddress(new byte[] { 1, 23, 45, 67, 89, 0xAB });
            var datagram = new UdcpRequest(DatagramType.Reboot, unitAddress);
            var target = new UdcpSerializer();

            var data = target.Serialize(datagram);
            var result = target.Deserialize(data);

            Assert.IsInstanceOfType(result, typeof(UdcpRequest));
            Assert.IsNotNull(result.Header);
            Assert.AreEqual(HeaderFlags.None, result.Header.Flags);
            Assert.AreEqual(DatagramType.Reboot, result.Header.Type);
            CollectionAssert.AreEqual(unitAddress.GetAddressBytes(), result.Header.UnitAddress.GetAddressBytes());

            Assert.IsNotNull(result.Fields);
            Assert.AreEqual(0, result.Fields.Count);
        }

        /// <summary>
        /// Tests serialization and deserialization of an empty response.
        /// </summary>
        [TestMethod]
        public void TestEmptyResponse()
        {
            var unitAddress = new UdcpAddress(new byte[] { 1, 23, 45, 67, 89, 0xAB });
            var datagram = new UdcpResponse(DatagramType.Reboot, unitAddress);
            var target = new UdcpSerializer();

            var data = target.Serialize(datagram);
            var result = target.Deserialize(data);

            Assert.IsInstanceOfType(result, typeof(UdcpResponse));
            Assert.IsNotNull(result.Header);
            Assert.AreEqual(HeaderFlags.Response, result.Header.Flags);
            Assert.AreEqual(DatagramType.Reboot, result.Header.Type);
            CollectionAssert.AreEqual(unitAddress.GetAddressBytes(), result.Header.UnitAddress.GetAddressBytes());

            Assert.IsNotNull(result.Fields);
            Assert.AreEqual(0, result.Fields.Count);
        }

        /// <summary>
        /// Tests serialization and deserialization of a request with fields.
        /// </summary>
        [TestMethod]
        public void TestRequestWithFields()
        {
            var unitAddress = new UdcpAddress(new byte[] { 1, 23, 45, 67, 89, 0xAB });
            var address = IPAddress.Parse("192.168.10.115");
            var mask = IPAddress.Parse("255.255.255.0");
            var gateway = IPAddress.Parse("192.168.10.254");

            var datagram = new UdcpRequest(DatagramType.SetConfiguration, unitAddress);
            datagram.Fields.Add(new IpAddressField(address));
            datagram.Fields.Add(new NetworkMaskField(mask));
            datagram.Fields.Add(new GatewayField(gateway));

            var target = new UdcpSerializer();
            var data = target.Serialize(datagram);
            var result = target.Deserialize(data);

            Assert.IsInstanceOfType(result, typeof(UdcpRequest));
            Assert.IsNotNull(result.Header);
            Assert.AreEqual(HeaderFlags.None, result.Header.Flags);
            Assert.AreEqual(DatagramType.SetConfiguration, result.Header.Type);
            CollectionAssert.AreEqual(unitAddress.GetAddressBytes(), result.Header.UnitAddress.GetAddressBytes());

            Assert.IsNotNull(result.Fields);
            Assert.AreEqual(3, result.Fields.Count);

            var addrField = result.GetField<IpAddressField>();
            Assert.IsNotNull(addrField);
            CollectionAssert.AreEqual(address.GetAddressBytes(), addrField.Value.GetAddressBytes());

            var maskField = result.GetField<NetworkMaskField>();
            Assert.IsNotNull(maskField);
            CollectionAssert.AreEqual(mask.GetAddressBytes(), maskField.Value.GetAddressBytes());

            var gatewayField = result.GetField<GatewayField>();
            Assert.IsNotNull(gatewayField);
            CollectionAssert.AreEqual(gateway.GetAddressBytes(), gatewayField.Value.GetAddressBytes());
        }
    }
}
