// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultContentResourceStorageClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultContentResourceStorageClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Services
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Gorba.Center.Common.ServiceModel;
    using NLog;

    /// <summary>
    /// The default client gets the content from the REST service of the BackgroundSystem.
    /// </summary>
    internal class DefaultContentResourceStorageClient : ContentResourceStorageClientBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The retrieve content async.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<Stream> RetrieveContentAsync(string hash)
        {
            var memoryStream = new MemoryStream();
            var uriBuilder = await GetUri(hash);
            using (var httpClient = new HttpClient())
            {
                var response =
                    await httpClient.GetAsync(uriBuilder.Uri.AbsoluteUri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Resource not found");
                }

                await(await response.Content.ReadAsStreamAsync()).CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        private static async Task<UriBuilder> GetUri(string hash)
        {
            var backgroundSystemConfiguration =
                await BackgroundSystemConfigurationProvider.Current.GetConfigurationAsync();
            
            var apiHostPort = BackgroundSystemConfiguration.DefaultApiHostPort;

            if (backgroundSystemConfiguration != null)
            {
                apiHostPort = backgroundSystemConfiguration.ApiHostPort;
            }

            Logger.Debug($"Api Host port: {apiHostPort}");
            var uriBuilder = new UriBuilder(
                Uri.UriSchemeHttp,
                backgroundSystemConfiguration.FunctionalServices.Host,
                apiHostPort,
                "api/v1/resource/download");
            uriBuilder.Query = "hash=" + hash;
            return uriBuilder;
        }
    }
}