// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloaderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DownloaderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.ScreenGate
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for downloading serialized data from an HTTP server.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the data to be deserialized.
    /// </typeparam>
    internal abstract class DownloaderBase<T>
    {
        private Uri lastUri;
        private string lastETag;

        /// <summary>
        /// Gets or sets the credentials to use when connecting to the server.
        /// </summary>
        public NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets the last value downloaded and deserialized from the server.
        /// </summary>
        public T LastValue { get; private set; }

        /// <summary>
        /// Asynchronously updates the <see cref="LastValue"/> from the server
        /// if its ETag has changed (or it is being downloaded from a new URI).
        /// </summary>
        /// <param name="uri">
        /// The uri from which the content is to be downloaded.
        /// </param>
        /// <returns>
        /// A task for awaiting the result.
        /// If the result is true, the data was updated and the new value is available in
        /// <see cref="LastValue"/>.
        /// </returns>
        public async Task<bool> UpdateAsync(Uri uri)
        {
            if (uri.Equals(this.lastUri) && this.lastETag != null)
            {
                using (var response = await this.RequestAsync(uri, WebRequestMethods.Http.Head))
                {
                    var etag = response.Headers[HttpResponseHeader.ETag];
                    if (etag == this.lastETag)
                    {
                        return false;
                    }
                }
            }

            this.lastUri = uri;
            using (var response = await this.RequestAsync(uri, WebRequestMethods.Http.Get))
            {
                this.lastETag = response.Headers[HttpResponseHeader.ETag];

                var input = response.GetResponseStream();
                if (input == null)
                {
                    throw new WebException("Got no response stream from " + response.ResponseUri);
                }

                this.LastValue = this.ParseResponse(input);
            }

            return true;
        }

        /// <summary>
        /// Parses the response stream and deserializes the contents.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected abstract T ParseResponse(Stream input);

        private async Task<WebResponse> RequestAsync(Uri uri, string method)
        {
            var request = WebRequest.Create(uri);
            request.Method = method;
            request.Credentials = this.Credentials;
            request.Timeout = 1000;
            return await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
        }
    }
}