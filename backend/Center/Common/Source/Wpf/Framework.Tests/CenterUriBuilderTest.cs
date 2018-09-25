// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterUriBuilderTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterUriBuilderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Tests
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Helpers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Define the tests for the <see cref="CenterUriBuilder"/> component.
    /// </summary>
    [TestClass]
    public class CenterUriBuilderTest
    {
        /// <summary>
        /// Tests a full Uri.
        /// </summary>
        [TestMethod]
        public void FullUriTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "net.tcp://server.gorba.com:9000/System";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("net.tcp://server.gorba.com:9000/System"), targetUri);
        }

        /// <summary>
        /// Tests a full Uri with scheme different than the base one.
        /// </summary>
        [TestMethod]
        public void FullUriWithDifferentSchemeTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com:9000/System";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:9000/System"), targetUri);
        }

        /// <summary>
        /// Tests a partial Uri with scheme different than the base one.
        /// </summary>
        [TestMethod]
        public void PartialUriWithDifferentSchemeTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com";
            var builder = new CenterUriBuilder();

            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);

            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:808/BackgroundSystem"), targetUri);
        }

        /// <summary>
        /// Tests a partial Uri with scheme and port different then the base ones.
        /// </summary>
        [TestMethod]
        public void PartialUriWithDifferentSchemeAndPortTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com:80";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:80/BackgroundSystem"), targetUri);
        }

        /// <summary>
        /// Tests a partial Uri with scheme and path different then the base ones.
        /// </summary>
        [TestMethod]
        public void PartialUriWithDifferentSchemeAndPathTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com/Path";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:808/Path"), targetUri);
        }

        /// <summary>
        /// Tests a partial URI with scheme and port different than the base ones and with an explicit empty path.
        /// </summary>
        [TestMethod]
        public void PartialUriWithDifferentSchemeAndPortExplicitEmptyPathTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com:80/";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:80/"), targetUri);
        }

        /// <summary>
        /// Tests a partial Uri with scheme, port and path different than the base ones.
        /// </summary>
        [TestMethod]
        public void PartialUriWithDifferentSchemePortAndPathTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "http://server.gorba.com:80/Path";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("http://server.gorba.com:80/Path"), targetUri);
        }

        /// <summary>
        /// Tests the builder a simple host name.
        /// </summary>
        [TestMethod]
        public void OnlyHostNameTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "server.gorba.com";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("net.tcp://server.gorba.com:808/BackgroundSystem"), targetUri);
        }

        /// <summary>
        /// Tests the builder with a host name and a port different than the base ones.
        /// </summary>
        [TestMethod]
        public void HostNameAndPortTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "server.gorba.com:9000";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("net.tcp://server.gorba.com:9000/BackgroundSystem"), targetUri);
        }

        /// <summary>
        /// Tests the builder with a host name and a port different than the base ones and an explicitly empty path.
        /// </summary>
        [TestMethod]
        public void HostNameAndPortExplicitEmptyPathTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "server.gorba.com:9000/";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("net.tcp://server.gorba.com:9000/"), targetUri);
        }

        /// <summary>
        /// Tests the builder with a host name, a port and a path different than the base ones.
        /// </summary>
        [TestMethod]
        public void HostNamePortAndPathTest()
        {
            var baseUri = new Uri("net.tcp://localhost:808/BackgroundSystem");
            var host = "server.gorba.com:9000/Path";
            var builder = new CenterUriBuilder();
            Uri targetUri;
            var result = builder.TryBuild(baseUri, host, out targetUri);
            Assert.IsTrue(result);
            Assert.AreEqual(new Uri("net.tcp://server.gorba.com:9000/Path"), targetUri);
        }
    }
}
