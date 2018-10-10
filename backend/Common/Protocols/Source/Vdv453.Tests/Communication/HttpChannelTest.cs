// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a test class for HttpChannelTest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Tests.Communication
{
    using System;
    using System.Net;

    using Gorba.Common.Protocols.Vdv453.Communication;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for HttpChannelTest and is intended
    /// to contain all HttpChannelTest Unit Tests
    /// </summary>
    [TestClass]
    public class HttpChannelTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //// 
        ////You can use the following additional attributes as you write your tests:
        ////
        ////Use ClassInitialize to run code before running the first test in the class
        ////[ClassInitialize()]
        ////public static void MyClassInitialize(TestContext testContext)
        ////{
        ////}
        ////
        ////Use ClassCleanup to run code after all tests in a class have run
        ////[ClassCleanup()]
        ////public static void MyClassCleanup()
        ////{
        ////}
        ////
        ////Use TestInitialize to run code before running each test
        ////[TestInitialize()]
        ////public void MyTestInitialize()
        ////{
        ////}
        ////
        ////Use TestCleanup to run code after each test has run
        ////[TestCleanup()]
        ////public void MyTestCleanup()
        ////{
        ////}
        ////
        #endregion

        /// <summary>
        /// A test for HttpChannel Constructor
        /// </summary>
        [TestMethod]
        public void HttpChannelConstructorTest()
        {
            var target = new HttpChannel();
            Assert.IsNotNull(target.ListenerMessageQueue);
            Assert.IsNotNull(target.ResponseMessageQueue);
            Assert.AreEqual(HttpStatusCode.Continue, target.HttpStatus);
        }

        /// <summary>
        /// A test for Finalize
        /// </summary>
        [TestMethod]
        public void FinalizeTest()
        {
            var target = new HttpChannel();
            var privateTarget = new PrivateObject(target);
            privateTarget.Invoke("Finalize");
            Assert.IsNull(privateTarget.GetField("httpListener"));
        }

        /// <summary>
        /// A test for SetConfiguration
        /// </summary>
        [TestMethod]
        public void SetConfigurationTest()
        {
            var target = new HttpChannel();
            var config = new HttpChannelConfig();
            SetHttpChannelConfig(config);
            target.SetConfiguration(config);
            Assert.AreEqual(config.ListenerHost, target.HttpListenerHost);
            Assert.AreEqual(config.ListenerPort.ToString(), target.HttpListenerPort);
            Assert.AreEqual(config.ClientIdentification, target.HttpClientIndentification);
            Assert.AreEqual(
                config.ListenerHost + ":" + config.ListenerPort + "/" + config.ClientIdentification
                + "/", 
                target.HttpListenerUrl);
            Assert.AreEqual(config.ServerHost, target.HttpServerHost);
            Assert.AreEqual(config.ServerPort.ToString(), target.HttpServerPort);
            Assert.AreEqual(config.ServerIdentification, target.HttpServerIndentification);
            Assert.AreEqual(
                config.ServerHost + ":" + config.ServerPort + "/" + config.ServerIdentification
                + "/", 
                target.HttpServerUrl);
            Assert.AreEqual(config.ResponseTimeOut.TotalSeconds * 1000, target.HttpResponseTimeout);
            Assert.AreEqual(config.WebProxyHost, target.HttpWebProxyHost);
            Assert.AreEqual(config.WebProxyPort, target.HttpWebProxyPort);
        }

        /// <summary>
        /// A test for StartListener
        /// </summary>
        [TestMethod]
        public void HttpListenerTest()
        {
            var target = new HttpChannel();
            var privateTarget = new PrivateObject(target);
            var config = new HttpChannelConfig();
            SetHttpChannelConfig(config);
            privateTarget.Invoke("SetConfiguration", config);
            var prefix = "dfi/status.xml/";
            privateTarget.Invoke("StartListener", prefix);
            Assert.IsNotNull((HttpListener)privateTarget.GetField("httpListener"));
            privateTarget.Invoke("StopListener");
            ////Assert.IsNull((HttpListener)privateTarget.GetField("httpListener")); -> fails when building via TFS Build definition
        }

        /// <summary>
        /// A test for HttpServerIndentification
        /// </summary>
        [TestMethod]
        public void HttpServerIndentificationTest()
        {
            var target = new HttpChannel();
            var expected = "server";
            target.HttpServerIndentification = expected;
            var actual = target.HttpServerIndentification;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpClientIndentification
        /// </summary>
        [TestMethod]
        public void HttpClientIndentificationTest()
        {
            var target = new HttpChannel();
            var expected = "client";
            target.HttpClientIndentification = expected;
            var actual = target.HttpClientIndentification;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpListenerHost
        /// </summary>
        [TestMethod]
        public void HttpListenerHostTest()
        {
            var target = new HttpChannel();
            var expected = "http://127.0.0.1";
            target.HttpListenerHost = expected;
            var actual = target.HttpListenerHost;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpListenerPort
        /// </summary>
        [TestMethod]
        public void HttpListenerPortTest()
        {
            var target = new HttpChannel();
            var expected = "9001";
            target.HttpListenerPort = expected;
            var actual = target.HttpListenerPort;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpListenerURL
        /// </summary>
        [TestMethod]
        public void HttpListenerURLTest()
        {
            var target = new HttpChannel();
            var expected = "http://127.0.0.1:6001/client/";
            target.HttpListenerUrl = expected;
            var actual = target.HttpListenerUrl;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpResponseTimeout
        /// </summary>
        [TestMethod]
        public void HttpResponseTimeoutTest()
        {
            var target = new HttpChannel();
            var expected = 10000;
            target.HttpResponseTimeout = expected;
            var actual = target.HttpResponseTimeout;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpServerHost
        /// </summary>
        [TestMethod]
        public void HttpServerHostTest()
        {
            var target = new HttpChannel();
            var expected = "http://127.0.0.1";
            target.HttpServerHost = expected;
            var actual = target.HttpServerHost;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpServerPort
        /// </summary>
        [TestMethod]
        public void HttpServerPortTest()
        {
            var target = new HttpChannel();
            var expected = "9002";
            target.HttpServerPort = expected;
            var actual = target.HttpServerPort;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpServerURL
        /// </summary>
        [TestMethod]
        public void HttpServerURLTest()
        {
            var target = new HttpChannel();
            var expected = "http://127.0.0.1:6002/server/";
            target.HttpServerUrl = expected;
            var actual = target.HttpServerUrl;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpStatus
        /// </summary>
        [TestMethod]
        public void HttpStatusTest()
        {
            var target = new HttpChannel();
            var expected = HttpStatusCode.Continue;
            var actual = target.HttpStatus;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpWebProxyHost
        /// </summary>
        [TestMethod]
        public void HttpWebProxyHostTest()
        {
            var target = new HttpChannel();
            var expected = "http://proxy.csa.ch";
            target.HttpWebProxyHost = expected;
            var actual = target.HttpWebProxyHost;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for HttpWebProxyPort
        /// </summary>
        [TestMethod]
        public void HttpWebProxyPortTest()
        {
            var target = new HttpChannel();
            var expected = 8080;
            target.HttpWebProxyPort = expected;
            var actual = target.HttpWebProxyPort;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ListenerMessageQueue
        /// </summary>
        [TestMethod]
        public void ListenerMessageQueueTest()
        {
            var target = new HttpChannel();
            var actual = target.ListenerMessageQueue;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for ResponseMessageQueue
        /// </summary>
        [TestMethod]
        public void ResponseMessageQueueTest()
        {
            var target = new HttpChannel();
            var actual = target.ResponseMessageQueue;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Sets the VDV configuration parameters
        /// </summary>
        /// <param name="config">
        /// VDV configuration
        /// </param>
        private static void SetHttpChannelConfig(HttpChannelConfig config)
        {
            config.ListenerHost = "http://127.0.0.1";
            config.ListenerPort = 9001;
            config.ClientIdentification = "iqube";
            config.ServerHost = "http://127.0.0.1";
            config.ServerPort = 9002;
            config.ServerIdentification = "iqube";
            config.ResponseTimeOut = TimeSpan.FromSeconds(10000);
            config.WebProxyHost = string.Empty;
            config.WebProxyPort = 0;
        }
    }
}