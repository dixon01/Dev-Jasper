// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClickOnceMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClickOnceMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Owin;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    using NLog;

    /// <summary>
    /// The middleware that handles ClickOnce requests.
    /// </summary>
    public class ClickOnceMiddleware : OwinMiddleware
    {
        private const string ClickOncePath = "/clickonce/";

        private const string ClickOnceContainerName = "clickonce";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickOnceMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        public ClickOnceMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue
                && context.Request.Path.Value.ToLower().StartsWith(ClickOncePath))
            {
                Logger.Trace("Detected ClickOnce request");
                var redirectPath = context.Request.Path.Value.Substring(ClickOncePath.Length);
                var clickOnceBaseAddress = this.GetClickOnceBaseAddress();
                if (clickOnceBaseAddress != null)
                {
                    var redirectUrl = new Uri(clickOnceBaseAddress, redirectPath);
                    if (redirectPath.EndsWith(".application"))
                    {
                        Logger.Trace(
                            "Downloading from '{0}' and sending back the content as response for '{1}'",
                            redirectUrl,
                            context.Request.Uri);
                        using (var httpClient = new HttpClient())
                        {
                            var result = await httpClient.GetByteArrayAsync(redirectUrl);
                            await context.Response.WriteAsync(result);
                            return;
                        }
                    }

                    Logger.Trace("Redirecting '{0}' to '{1}'", context.Request.Uri, redirectUrl);
                    context.Response.Redirect(redirectUrl.ToString());
                    return;
                }
            }

            Logger.Trace("Request '{0}' not handled", context.Request.Uri);
            await this.Next.Invoke(context);
        }

        /// <summary>
        /// Gets the base address for ClickOnce.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Uri"/> to be used as base address for ClickOnce applications.
        /// </returns>
        protected virtual Uri GetClickOnceBaseAddress()
        {
             var storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting(
                        Common.Azure.PredefinedAzureItems.Settings.StorageConnectionString));

            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(ClickOnceContainerName);
            container.CreateIfNotExists();
            var uriString = string.Format(
                "http://{0}.blob.core.windows.net/{1}/",
                storageAccount.Credentials.AccountName,
                ClickOnceContainerName);
            var uri = new Uri(uriString);
            Logger.Trace("ClickOnce base address: {0}", uri);

            return uri;
        }
    }
}