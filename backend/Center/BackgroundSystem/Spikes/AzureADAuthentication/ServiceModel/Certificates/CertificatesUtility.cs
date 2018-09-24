// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificatesUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CertificatesUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceModel.Certificates
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// The certificates utility.
    /// </summary>
    public static class CertificatesUtility
    {
        /// <summary>
        /// The get certificate.
        /// </summary>
        /// <returns>
        /// The <see cref="X509Certificate2"/>.
        /// </returns>
        public static X509Certificate2 GetCertificate()
        {
            var type = typeof(CertificatesUtility);
            var resourceName = type.Namespace + ".BackgroundSystem.pfx";
            using (var stream = type.Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception("Can't find the certificate");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return new X509Certificate2(bytes);
            }
        }
    }
}
