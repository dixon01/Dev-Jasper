// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContentResourceController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Services;

    using NLog;

    /// <summary>
    /// The controller to provide the REST api to work with content resources.
    /// </summary>
    [RoutePrefix("api/v1/resource")]
    public class ContentResourceController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Downloads the resource with the given <paramref name="hash"/> asynchronously.
        /// </summary>
        /// <param name="hash">
        /// The hash of the resource to download.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("download")]
        public async Task<HttpResponseMessage> GetAsync(string hash)
        {
            try
            {
                var client = DependencyResolver.Current.Get<IContentResourceStorageClient>();
                return new HttpResponseMessage(HttpStatusCode.OK)
                           {
                               Content =
                                   new StreamContent(
                                   await client.GetContentAsync(hash))
                           };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error while serving request for hash '{0}'", hash));

                
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}