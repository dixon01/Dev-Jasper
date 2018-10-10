// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificatesUtilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CertificatesUtilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Certificates
{
    using Gorba.Center.BackgroundSystem.Core.Certificates;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the <see cref="CertificatesUtility"/>.
    /// </summary>
    [TestClass]
    public class CertificatesUtilityTest
    {
        /// <summary>
        /// Gets the certificate and verifies the name.
        /// This test is important to trap errors that could occur in a refactoring, if the certificate is moved to a
        /// different directory.
        /// </summary>
        [TestMethod]
        public void TestGetBackgroundSystemCertificate()
        {
            var certificate = CertificatesUtility.GetBackgroundSystemCertificate();
            Assert.AreEqual("CN=BackgroundSystem", certificate.IssuerName.Name);
        }
    }
}