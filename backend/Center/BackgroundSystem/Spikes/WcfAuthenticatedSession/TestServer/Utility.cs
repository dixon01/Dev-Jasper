// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Utility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System.Reflection;
    using System.Resources;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <returns>
        /// The <see cref="X509Certificate2"/>.
        /// </returns>
        public static X509Certificate2 GetCertificate()
        {
            var stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("WcfAuthenticatedSession.TestServer.BackgroundSystem.pfx");
            if (stream == null)
            {
                throw new MissingManifestResourceException("BackgroundSystem certificate not found");
            }

            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return new X509Certificate2(bytes);
        }
    }
}