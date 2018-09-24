// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.BackgroundSystem.Host.Api
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// The controller to work with content resources.
    /// </summary>
    [RoutePrefix("api/v1/resource")]
    public class ContentResourceController : ApiController
    {
        /// <summary>
        /// Gets the content resource with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content to get.
        /// </param>
        /// <returns>
        /// The message with StatusCode 200 if the resource was found, and the relative stream.
        /// </returns>
        [HttpGet]
        [Route("download")]
        public async Task<HttpResponseMessage> GetAsync(string hash)
        {
            try
            {
                var contentResourceDataService = DependencyResolver.Current.Get<IContentResourceService>();
                if (!await contentResourceDataService.TestContentResourceAsync(hash, HashAlgorithmTypes.xxHash64))
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                var item = contentResourceDataService.GetResource(hash, HashAlgorithmTypes.xxHash64);

                var memoryStream = new MemoryStream();
                using (var stream = item.OpenRead())
                {
                    await stream.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(memoryStream) };
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}