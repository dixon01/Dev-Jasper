// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestServer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Tests
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Update.ServiceModel.Repository;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The test server.
    /// </summary>
    [TestClass]
    public class TestServer
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Tests the download of a file in the ClickOnce directory.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        [TestMethod]
        [DeploymentItem(@"TestData\StaticContent\ClickOnce\ClickOnce.txt", @"TestData\StaticContent\ClickOnce")]
        public async Task TestClickOnce()
        {
            var appDataPath = Path.Combine(this.TestContext.DeploymentDirectory, "TestData");
            PortalSettingsProvider.SetCurrent(
                new AppDataRootDirectoryPortalSettingsProvider(appDataPath));
            using (var server = Microsoft.Owin.Testing.TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/ClickOnce/ClickOnce.txt");
                var content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("ClickOnce", content);
            }
        }

        /// <summary>
        /// Tests the configuration file.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        [TestMethod]
        [DeploymentItem(@"TestData\BackgroundSystemConfiguration.xml", @"TestData")]
        [DeploymentItem(@"TestData\StaticContent\ClickOnce\ClickOnce.txt", @"TestData\StaticContent\ClickOnce")]
        public async Task TestConfiguration()
        {
            var result = new TaskCompletionSource<BackgroundSystemConfiguration>();
            var backgroundSystemConfiguration = new BackgroundSystemConfiguration
                                                    {
                                                        NotificationsConnectionString =
                                                            "test://gorba"
                                                    };
            result.SetResult(backgroundSystemConfiguration);
            var mockBackgroundSystemConfigurationProvider =
                new Mock<BackgroundSystemConfigurationProvider>(MockBehavior.Strict);
            mockBackgroundSystemConfigurationProvider.Setup(provider => provider.GetConfiguration(It.IsAny<string>()))
                .Returns(backgroundSystemConfiguration);
            mockBackgroundSystemConfigurationProvider.Setup(
                provider => provider.GetConfigurationAsync(It.IsAny<string>())).Returns(result.Task);
            BackgroundSystemConfigurationProvider.Set(mockBackgroundSystemConfigurationProvider.Object);
            var appDataPath = Path.Combine(this.TestContext.DeploymentDirectory, "TestData");
            PortalSettingsProvider.SetCurrent(
                new AppDataRootDirectoryPortalSettingsProvider(appDataPath));
            using (var server = Microsoft.Owin.Testing.TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Configuration");
                var stream = await response.Content.ReadAsStreamAsync();
                var configManager = new ConfigManager<BackgroundSystemConfiguration>();
                configManager.Configurator = new Configurator(stream);
                Assert.AreEqual("test://gorba", configManager.Config.NotificationsConnectionString);
            }
        }

        /// <summary>
        /// Tests the repository configuration.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        [TestMethod]
        [DeploymentItem(@"TestData\StaticContent\ClickOnce\ClickOnce.txt", @"TestData\StaticContent\ClickOnce")]
        public async Task TestRepositoryConfiguration()
        {
            var repositoryConfiguration = new RepositoryConfig
                                              {
                                                  Versions =
                                                      {
                                                          new RepositoryVersionConfig
                                                              {
                                                                  ResourceDirectory = "Resources",
                                                                  FeedbackDirectory = "Feedback",
                                                                  CommandsDirectory = "Commands"
                                                              }
                                                      }
                                              };
            var repositoryResult = new TaskCompletionSource<RepositoryConfig>();
            repositoryResult.SetResult(repositoryConfiguration);
            var mockPortalRepositoryConfigurationProvider =
                new Mock<PortalRepositoryConfigurationProvider>(MockBehavior.Strict);
          //  mockPortalRepositoryConfigurationProvider.Setup(provider => provider.GetRepositoryConfiguration())
          //      .Returns(repositoryConfiguration);
            mockPortalRepositoryConfigurationProvider.Setup(provider => provider.GetRepositoryConfigurationAsync())
                .Returns(repositoryResult.Task);
            PortalRepositoryConfigurationProvider.Set(mockPortalRepositoryConfigurationProvider.Object);
            var appDataPath = Path.Combine(this.TestContext.DeploymentDirectory, "TestData");
            PortalSettingsProvider.SetCurrent(
               new AppDataRootDirectoryPortalSettingsProvider(appDataPath));

            using (var server = Microsoft.Owin.Testing.TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Repository");
                var stream = await response.Content.ReadAsStreamAsync();
                var configManager = new ConfigManager<RepositoryConfig>();
                configManager.Configurator = new Configurator(stream);
                Assert.AreEqual("Resources", configManager.Config.Versions.First().ResourceDirectory);
                Assert.AreEqual("Feedback", configManager.Config.Versions.First().FeedbackDirectory);
            }
        }

        /// <summary>
        /// Tests the home page.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        [TestMethod]
        [DeploymentItem(@"TestData\Views\Actions\Index.html", @"TestData\Views\Actions")]
        [DeploymentItem(@"TestData\StaticContent\ClickOnce\ClickOnce.txt", @"TestData\StaticContent\ClickOnce")]
        public async Task TestIndex()
        {
            var appDataPath = Path.Combine(this.TestContext.DeploymentDirectory, "TestData");
            PortalSettingsProvider.SetCurrent(
                new AppDataRootDirectoryPortalSettingsProvider(appDataPath));
            using (var server = Microsoft.Owin.Testing.TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/");
                var content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("Index", content);
            }
        }

        /// <summary>
        /// Tests the redirect to Login for unauthorized access to applications.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        [TestMethod]
        [DeploymentItem(@"TestData\Views\Actions\Login.html", @"TestData\Views\Actions")]
        [DeploymentItem(@"TestData\StaticContent\ClickOnce\ClickOnce.txt", @"TestData\StaticContent\ClickOnce")]
        public async Task TestUnauthorizedApplications()
        {
            var appDataPath = Path.Combine(this.TestContext.DeploymentDirectory, "TestData");
            PortalSettingsProvider.SetCurrent(
                new AppDataRootDirectoryPortalSettingsProvider(appDataPath));
            using (var server = Microsoft.Owin.Testing.TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Applications");
                Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
                response = await server.HttpClient.GetAsync(response.Headers.Location);
                var content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("Login", content);
            }
        }
    }
}