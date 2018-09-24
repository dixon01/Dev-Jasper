// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcStreamHandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcStreamHandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Tests
{
    using System.IO;
    using System.Text;

    using Gorba.Motion.Common.Mgi.AtmelControl;
    using Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="JsonRpcStreamHandler"/>.
    /// </summary>
    [TestClass]
    public class JsonRpcStreamHandlerTest
    {
        /// <summary>
        /// Tests the reception of an <see cref="RpcResponse"/>.
        /// </summary>
        [TestMethod]
        public void TestSucessfulResponse()
        {
            var memory =
                new MemoryStream(Encoding.ASCII.GetBytes("{ \"jsonrpc\": \"2.0\", \"result\": 0, \"id\": \"1\" }"));
            var target = new JsonRpcStreamHandler(memory);

            var read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcResponse));
            var response = (RpcResponse)read;

            var result = response.GetResult<int>();

            Assert.AreEqual("1", response.id);
            Assert.AreEqual(0, result);
            Assert.IsNull(response.error);
        }

        /// <summary>
        /// Tests the reception of an <see cref="RpcNotification"/>.
        /// </summary>
        [TestMethod]
        public void TestNotification()
        {
            var memory =
                new MemoryStream(Encoding.ASCII.GetBytes(
                    "{ \"jsonrpc\": \"2.0\", \"method\": \"notifyObject\", \"params\": { \"objectName\": " +
                    "\"InfovisionInputState\", \"Stop0\": 2, \"Stop1\": 3, \"Ignition\": 1, \"Address\": 8 } }"));
            var target = new JsonRpcStreamHandler(memory);

            var read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcNotification));
            var notification = (RpcNotification)read;

            Assert.AreEqual("notifyObject", notification.method);
            Assert.IsNotNull(notification.@params);

            var parameters = notification.GetParams<InfovisionInputState>();
            Assert.IsNotNull(parameters);

            Assert.AreEqual(2, parameters.Stop0);
            Assert.AreEqual(3, parameters.Stop1);
            Assert.AreEqual(1, parameters.Ignition);
            Assert.AreEqual(8, parameters.Address);
        }

        /// <summary>
        /// Tests the reception of an <see cref="RpcRequest"/>.
        /// </summary>
        [TestMethod]
        public void TestRequest()
        {
            var memory =
                new MemoryStream(Encoding.ASCII.GetBytes(
                    "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"registerObject\", " +
                    "\"params\": [ \"IbisState\" ] }"));
            var target = new JsonRpcStreamHandler(memory);

            var read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcRequest));
            var request = (RpcRequest)read;

            Assert.AreEqual("registerObject", request.method);
            Assert.AreEqual("1", request.id);
            Assert.IsNotNull(request.@params);

            var parameters = request.GetParams<string[]>();
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Length);

            Assert.AreEqual("IbisState", parameters[0]);
        }

        /// <summary>
        /// Tests the reception of an <see cref="RpcNotification"/> followed by
        /// an <see cref="RpcResponse"/>.
        /// </summary>
        [TestMethod]
        public void TestNotificationAndResponse()
        {
            var memory =
                new MemoryStream(Encoding.ASCII.GetBytes(
                    "{ \"jsonrpc\": \"2.0\", \"method\": \"notifyObject\", \"params\": " +
                    "{ \"objectName\": \"IbisStream\", \"Data\": [ \"a6\", \"a7\", \"a8\", \"a9\" ] } }" +
                    "{ \"jsonrpc\": \"2.0\", \"result\": 0, \"id\": \"14\" }"));
            var target = new JsonRpcStreamHandler(memory);

            var read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcNotification));
            var notification = (RpcNotification)read;

            Assert.AreEqual("notifyObject", notification.method);
            Assert.IsNotNull(notification.@params);

            var parameters = notification.GetParams<IbisStream>();
            Assert.IsNotNull(parameters);
            Assert.IsNotNull(parameters.Data);

            Assert.AreEqual(4, parameters.Data.Count);
            Assert.AreEqual("a6", parameters.Data[0]);
            Assert.AreEqual("a7", parameters.Data[1]);
            Assert.AreEqual("a8", parameters.Data[2]);
            Assert.AreEqual("a9", parameters.Data[3]);

            read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcResponse));
            var response = (RpcResponse)read;

            Assert.AreEqual("14", response.id);
            Assert.AreEqual(0L, response.result);
            Assert.IsNull(response.error);
        }
    }
}
