// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CertificateValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceModel.Certificates
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// The certificate validator.
    /// </summary>
    public class CertificateValidator : X509CertificateValidator
    {
        private const string AllowedIssuerName = "CN=BackgroundSystem";

        private const string Thumbprint = "391F7B21847813FA11947AC1F06956B70A8DADB9";

        /// <summary>
        /// When overridden in a derived class, validates the X.509 certificate.
        /// </summary>
        /// <param name="certificate">
        /// The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2"/> that represents the
        /// X.509 certificate to validate.
        /// </param>
        public override void Validate(X509Certificate2 certificate)
        {
            // Check that there is a certificate.
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            // Check that the certificate issuer matches the configured issuer.
            if (AllowedIssuerName != certificate.IssuerName.Name)
            {
                throw new SecurityTokenValidationException("Certificate was not issued by a trusted issuer");
            }

            if (Thumbprint == certificate.Thumbprint)
            {
                return;
            }

            throw new SecurityTokenValidationException("Invalid  thumbprint");
        }
    }
}
