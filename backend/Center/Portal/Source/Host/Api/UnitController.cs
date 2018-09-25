// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Api
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Gorba.Center.Common.ServiceModel;
    using NLog;

    /// <summary>
    /// The unit controller.
    /// </summary>
    [RoutePrefix("api/v1/resource")]
    public class UnitController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GET operation returning the configuration for the specified unit.
        /// </summary>
        /// <param name="uid">
        /// The unique identifier for the unit.
        /// </param>
        /// <param name="hash">
        /// The hash currently known by the unit.
        /// </param>
        /// <param name="seq_num">
        /// The sequential number optionally provided by the unit.
        /// </param>
        /// <returns>
        /// The response to the request of the unit.
        /// The response will contain the <see cref="HttpStatusCode.NotFound"/> if the unit was not found (or any
        /// update command for it) was found.
        /// Otherwise, the unit will reply with status <see cref="HttpStatusCode.OK"/>. If the hash provided by the
        /// unit is the same as the current hash, then an empty content is sent; otherwise, the new content is sent as
        /// stream.
        /// </returns>
        // ReSharper disable once InconsistentNaming
        public async Task<HttpResponseMessage> GetAsync(string uid, string hash = null, int? seq_num = null)
        {
            var memoryStream = new MemoryStream();
            var uriBuilder = await GetUri(uid, hash);
            using (var httpClient = new HttpClient())
            {
                var response =
                    await httpClient.GetAsync(uriBuilder.Uri.AbsoluteUri);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResponseMessage(response.StatusCode);
                }

                await(await response.Content.ReadAsStreamAsync()).CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(memoryStream) };
        }

        private static async Task<UriBuilder> GetUri(string unitId, string hash)
        {
            var backgroundSystemConfiguration =
                await BackgroundSystemConfigurationProvider.Current.GetConfigurationAsync();

            int apiHostPort = BackgroundSystemConfiguration.DefaultApiHostPort;
            
            if (backgroundSystemConfiguration != null)
            {
                apiHostPort = backgroundSystemConfiguration.ApiHostPort;
            }
            
            
            Logger.Debug($"API Host port: {apiHostPort}");
            var uriBuilder = new UriBuilder(
                Uri.UriSchemeHttp,
                backgroundSystemConfiguration.FunctionalServices.Host,
                apiHostPort,
                "api/v1/unit");
            uriBuilder.Query = "uid=" + unitId;
            if (!string.IsNullOrEmpty(hash))
            {
                uriBuilder.Query += "&hash=" + hash;
            }

            return uriBuilder;
        }
    }
}