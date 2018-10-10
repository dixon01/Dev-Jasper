// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemConfigurationProviderTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemConfigurationProviderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines unit tests for the <see cref="BackgroundSystemConfigurationProvider"/> component.
    /// </summary>
    [TestClass]
    public class BackgroundSystemConfigurationProviderTest
    {
        /// <summary>
        /// Gets or sets the <see cref="TestContext"/> for the test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the test run.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Reset();
        }

        /// <summary>
        /// Cleans the test run up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Reset();
        }

        /// <summary>
        /// Tests the loading of the BackgroundSystemSettings object using the BackgroundSystemConfiguration.xml file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemConfiguration.xml")]
        public void TestValidBackgroundSystemConfiguration()
        {
            this.CreateAndSetHttpClientHandlerMock("BackgroundSystemConfiguration.xml");
            var config = BackgroundSystemConfigurationProvider.Current.GetConfiguration(
                "http://localhost/Configuration");
            Assert.AreEqual("medi://endpoint.cloudapp.net:1596", config.NotificationsConnectionString);
        }

        /// <summary>
        /// Tests the loading of the BackgroundSystemSettings object using the BackgroundSystemConfiguration.xml file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemConfiguration.xml")]
        public void BackgroundSystemConfiguration_CanRetrieve_ApiHostPort()
        {
            this.CreateAndSetHttpClientHandlerMock("BackgroundSystemConfiguration.xml");
            var config = BackgroundSystemConfigurationProvider.Current.GetConfiguration(
                "http://localhost/Configuration");
            
            Assert.AreEqual(8090, config.ApiHostPort);
        }

        /// <summary>
        /// Tests the loading of the BackgroundSystemSettings object using the BackgroundSystemConfiguration.xml file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemConfigurationWithErrors.xml")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestBackgroundSystemConfigurationWithErrors()
        {
            this.CreateAndSetHttpClientHandlerMock("BackgroundSystemConfigurationWithErrors.xml");
            try
            {
                var config = BackgroundSystemConfigurationProvider.Current.GetConfiguration(
               "http://localhost/Configuration");
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(1, exception.InnerExceptions.Count);
                throw exception.InnerException;
            }
        }

        private static void Reset()
        {
            BackgroundSystemConfigurationProvider.Reset();
            var httpMessageHandlerFactoryMock = new Mock<HttpMessageHandlerFactory>(MockBehavior.Strict);
            HttpMessageHandlerFactory.SetCurrent(httpMessageHandlerFactoryMock.Object);
        }

        private void CreateAndSetHttpClientHandlerMock(string contentPath)
        {
            var filePath = Path.Combine(this.TestContext.DeploymentDirectory, contentPath);
            if (!File.Exists(filePath))
            {
                throw new Exception("The test configuration file was not found");
            }

            var contentStream = new MemoryStream();
            using (var file = File.OpenRead(filePath))
            {
                file.CopyTo(contentStream);
            }

            contentStream.Seek(0, SeekOrigin.Begin);

            var handlerMock = new FakeHandler(contentStream);
            var factoryMock = new Mock<HttpMessageHandlerFactory>();
            factoryMock.Setup(factory => factory.Create()).Returns(handlerMock);
            HttpMessageHandlerFactory.SetCurrent(factoryMock.Object);
        }

        private class FakeHandler : HttpMessageHandler
        {
            private readonly Stream content;

            public FakeHandler(Stream content)
            {
                this.content = content;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return
                    Task.FromResult(
                        new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(this.content) });
            }
        }
    }
}