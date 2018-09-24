// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareComponentVersionTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Tests.Compatibility
{
    using System;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="SoftwareComponentVersion"/>.
    /// </summary>
    [TestClass]
    public class SoftwareComponentVersionTest
    {
        /// <summary>
        /// Tests the <see cref="SoftwareComponentVersion"/> constructors.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTestEmptyVersionString()
        {
            var version = new SoftwareComponentVersion(string.Empty);
        }

        /// <summary>
        /// The constructor test bad version string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructorTestBadVersionString()
        {
            var version = new SoftwareComponentVersion("1.a");
        }

        /// <summary>
        /// The constructor test negative version string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructorTestNotCompleteVersionString()
        {
            var version = new SoftwareComponentVersion("1.");
        }

        /// <summary>
        /// The constructor test negative version string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTestNegativeVersionString()
        {
            var version = new SoftwareComponentVersion("1.-3");
        }

        /// <summary>
        /// The constructor test version strings.
        /// </summary>
        [TestMethod]
        public void ConstructorTestVersionStrings()
        {
            var version = new SoftwareComponentVersion("2");
            Assert.IsTrue(version.VersionSegments.Count == 1);
            Assert.IsTrue(version.VersionSegments[0] == 2);

            version = new SoftwareComponentVersion("3.11");
            Assert.IsTrue(version.VersionSegments.Count == 2);
            Assert.IsTrue(version.VersionSegments[0] == 3);
            Assert.IsTrue(version.VersionSegments[1] == 11);

            version = new SoftwareComponentVersion("3.1141.1234");
            Assert.IsTrue(version.VersionSegments.Count == 3);
            Assert.IsTrue(version.VersionSegments[0] == 3);
            Assert.IsTrue(version.VersionSegments[1] == 1141);
            Assert.IsTrue(version.VersionSegments[2] == 1234);
        }

        /// <summary>
        /// The constructor test version operators.
        /// </summary>
        [TestMethod]
        public void OperatorTests()
        {
            var versionA = new SoftwareComponentVersion("2");
            var versionB = new SoftwareComponentVersion("2");
            Assert.IsTrue(versionA == versionB);

            versionA = new SoftwareComponentVersion("2.3.1234");
            versionB = new SoftwareComponentVersion("2.3.1234");
            Assert.IsTrue(versionA == versionB);
            Assert.IsFalse(versionA > versionB);
            Assert.IsFalse(versionA < versionB);

            versionA = new SoftwareComponentVersion("2.3");
            versionB = new SoftwareComponentVersion("2.3.1234");
            Assert.IsTrue(versionA == versionB);

            versionA = new SoftwareComponentVersion("2.3.1234");
            versionB = new SoftwareComponentVersion("2.3");
            Assert.IsTrue(versionA == versionB);
            Assert.IsTrue(versionA >= versionB);
            Assert.IsTrue(versionA <= versionB);
            Assert.IsFalse(versionA != versionB);

            versionA = new SoftwareComponentVersion("2.3.1222");
            versionB = new SoftwareComponentVersion("2.3.1111");
            Assert.IsTrue(versionA > versionB);
            Assert.IsTrue(versionB < versionA);

            versionA = new SoftwareComponentVersion("2.5");
            versionB = new SoftwareComponentVersion("2.4.1234");
            Assert.IsTrue(versionA > versionB);
            Assert.IsFalse(versionA < versionB);
            Assert.IsFalse(versionB > versionA);
            Assert.IsTrue(versionB < versionA);

            versionA = new SoftwareComponentVersion("3.0");
            versionB = new SoftwareComponentVersion("2.4.1234.5678");
            Assert.IsTrue(versionA > versionB);
            Assert.IsFalse(versionA < versionB);
            Assert.IsFalse(versionB > versionA);
            Assert.IsTrue(versionB < versionA);
        }
    }
}
