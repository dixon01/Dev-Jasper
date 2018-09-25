// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterUriBuilder.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterUriBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Helpers
{
    using System;

    /// <summary>
    /// Component used to compose a valid URI starting from a given base (reference) and the host input by the user.
    /// </summary>
    public class CenterUriBuilder
    {
        /// <summary>
        /// Tries to build a valid URI.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="host">The host.</param>
        /// <param name="targetUri">The target URI.</param>
        /// <returns><c>true</c> if it was possible to build a valid URI; otherwise, <c>false</c>.</returns>
        public bool TryBuild(Uri baseUri, string host, out Uri targetUri)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException("baseUri");
            }

            if (string.IsNullOrWhiteSpace(host) || host.Contains(","))
            {
                targetUri = null;
                return false;
            }

            var schemeAugmentedHost = host.Contains("://") ? host : baseUri.Scheme + "://" + host;

            Uri uri;
            try
            {
                uri = new Uri(schemeAugmentedHost);
            }
            catch (System.UriFormatException)
            {
                targetUri = null;
                return false;
            }

            var uriBuilder = new UriBuilder(uri);
            if (uri.IsDefaultPort && !host.Contains(uri.Host + ":" + uri.Port))
            {
                uriBuilder.Port = baseUri.Port;
            }

            if (uri.PathAndQuery.Equals("/") && !host.EndsWith("/"))
            {
                uriBuilder.Path = baseUri.PathAndQuery;
                uriBuilder.Query = string.Empty;
            }

            targetUri = uriBuilder.Uri;
            return true;
        }
    }
}
