// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelFactoryTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpChannelFactoryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Tests.Communication
{
    using System;

    using Gorba.Common.Protocols.Vdv453.Communication;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="HttpChannelFactory"/> class.
    /// </summary>
    [TestClass]
    public class HttpChannelFactoryTest
    {
        /// <summary>
        /// Verifies that the <see cref="HttpChannelFactory.Current"/> property is never null.
        /// </summary>
        [TestMethod]
        public void CurrentNotNullTest()
        {
            Assert.IsNotNull(HttpChannelFactory.Current);
            HttpChannelFactory.ResetToDefault();
            Assert.IsNotNull(HttpChannelFactory.Current);
        }

        /// <summary>
        /// Verifies that it's not possible to set the factory to null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotSetToNullTest()
        {
            HttpChannelFactory.OverrideDefault(null);
        }

        /// <summary>
        /// Verifies that it's possible to set the factory to a desired object.
        /// </summary>
        [TestMethod]
        public void OverrideDefaultTest()
        {
            var httpChannelFactoryMock = new Mock<HttpChannelFactory>();
            Assert.AreNotSame(httpChannelFactoryMock.Object, HttpChannelFactory.Current);
            HttpChannelFactory.OverrideDefault(httpChannelFactoryMock.Object);
            Assert.AreSame(httpChannelFactoryMock.Object, HttpChannelFactory.Current);
        }
    }
}
