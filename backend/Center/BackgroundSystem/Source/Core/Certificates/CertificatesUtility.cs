// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificatesUtility.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CertificatesUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Certificates
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Utility to handle BackgroundSystem certificates.
    /// </summary>
    public static class CertificatesUtility
    {
        /// <summary>
        /// Gets the BackgroundSystem certificate used by the system when required by the configuration.
        /// </summary>
        /// <returns>the BackgroundSystem certificate.</returns>
        public static X509Certificate2 GetBackgroundSystemCertificate()
        {
            var certificatesType = typeof(CertificatesUtility);
            var resourceName = certificatesType.Namespace + ".BackgroundSystem.pfx";
            using (var stream = certificatesType.Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception(
                        "Can't find the BackgroundSystem certificate. Ensure that the file is set as 'EmbeddedResource'"
                        + "and it is in the same directory as this class (CertificatesUtility).");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return new X509Certificate2(bytes);
            }
        }
    }
}