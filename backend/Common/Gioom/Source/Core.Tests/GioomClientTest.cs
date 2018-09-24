// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomClientTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomClientTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Tests
{
    using System;
    using System.Linq;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="GioomClient"/>.
    /// This test is using an subclass of <see cref="GioomClient"/> because we don't
    /// want to use the singleton (which might be "polluted" from previous tests).
    /// </summary>
    [TestClass]
    public class GioomClientTest
    {
        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// Test for <see cref="GioomClient.RegisterPort"/>.
        /// </summary>
        [TestMethod]
        public void TestRegister()
        {
            var target = GioomClient.Instance;
            var port = new SimplePort("Register", true, false, new FlagValues(), FlagValues.False);

            target.RegisterPort(port);

            var result = target.BeginOpenPort(MessageDispatcher.Instance.LocalAddress, "Register", null, null);
            Assert.IsTrue(result.CompletedSynchronously);

            var openPort = target.EndOpenPort(result);
            Assert.IsNotNull(openPort);
            Assert.AreEqual(port.Info.Name, openPort.Info.Name);
        }

        /// <summary>
        /// Test for <see cref="GioomClient.RegisterPort"/> to see what happens if it is called
        /// twice with the same port name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegisterTwice()
        {
            var target = GioomClient.Instance;
            var port = new SimplePort("RegisterTwice", true, false, new FlagValues(), FlagValues.False);

            target.RegisterPort(port);

            var secondPort = new SimplePort("RegisterTwice", true, true, new IntegerValues(0, 10), 0);
            target.RegisterPort(secondPort);
        }

        /// <summary>
        /// Test for <see cref="GioomClient.DeregisterPort"/>.
        /// </summary>
        [TestMethod]
        public void TestDeregister()
        {
            var target = GioomClient.Instance;
            var port = new SimplePort("Deregister", true, false, new FlagValues(), FlagValues.False);

            target.RegisterPort(port);

            var result = target.BeginOpenPort(MessageDispatcher.Instance.LocalAddress, "Deregister", null, null);
            Assert.IsTrue(result.CompletedSynchronously);

            var openPort = target.EndOpenPort(result);
            Assert.IsNotNull(openPort);
            Assert.AreEqual(port.Info.Name, openPort.Info.Name);

            openPort.Dispose();

            target.DeregisterPort(port);

            result = target.BeginOpenPort(MessageDispatcher.Instance.LocalAddress, "Deregister", null, null);
            Assert.IsTrue(result.CompletedSynchronously);

            openPort = target.EndOpenPort(result);
            Assert.IsNull(openPort);

            var secondPort = new SimplePort("Deregister", true, true, new IntegerValues(0, 10), 0);
            target.RegisterPort(secondPort);
        }

        /// <summary>
        /// Test for <see cref="GioomClient.BeginFindPorts"/> and related methods.
        /// </summary>
        [TestMethod]
        public void TestFindPorts()
        {
            var target = GioomClient.Instance;
            var port = new SimplePort("FindPorts", true, false, new FlagValues(), FlagValues.False);

            target.RegisterPort(port);

            var result = target.BeginFindPorts(MessageDispatcher.Instance.LocalAddress, TimeSpan.Zero, null, null);
            Assert.IsTrue(result.CompletedSynchronously);

            var ports = target.EndFindPorts(result);
            Assert.IsNotNull(ports);
            var found = ports.FirstOrDefault(p => p.Name == port.Info.Name);
            Assert.IsNotNull(found);
            Assert.IsTrue(found.CanRead);
            Assert.IsFalse(found.CanWrite);
            Assert.IsInstanceOfType(found.ValidValues, typeof(FlagValues));
        }

        /// <summary>
        /// Test for <see cref="GioomClient.OpenPort"/> and related methods.
        /// </summary>
        [TestMethod]
        public void TestOpenPort()
        {
            var target = GioomClient.Instance;
            var port = new SimplePort("OpenPort", true, false, new FlagValues(), FlagValues.False);

            target.RegisterPort(port);

            var result = target.BeginFindPorts(MessageDispatcher.Instance.LocalAddress, TimeSpan.Zero, null, null);
            Assert.IsTrue(result.CompletedSynchronously);

            var ports = target.EndFindPorts(result);
            Assert.IsNotNull(ports);
            var foundPort = ports.FirstOrDefault(p => p.Name == port.Info.Name);
            Assert.IsNotNull(foundPort);

            var openPort = target.OpenPort(foundPort);
            Assert.IsNotNull(openPort);
            Assert.AreEqual(FlagValues.False, openPort.Value);

            var changes = 0;
            openPort.ValueChanged += (s, e) => changes++;
            Assert.AreEqual(0, changes);

            port.Value = FlagValues.True;
            Assert.AreEqual(FlagValues.True, openPort.Value);
            Assert.AreEqual(1, changes);

            port.Value = FlagValues.True;
            Assert.AreEqual(FlagValues.True, openPort.Value);
            Assert.AreEqual(1, changes);

            port.Value = FlagValues.False;
            Assert.AreEqual(FlagValues.False, openPort.Value);
            Assert.AreEqual(2, changes);
        }
    }
}
