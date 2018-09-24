// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonDownloader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonDownloader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.ScreenGate
{
    using System.IO;

    using Newtonsoft.Json;

    /// <summary>
    /// Helper class for downloading JSON serialized data from an HTTP server.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the data to be deserialized.
    /// </typeparam>
    internal class JsonDownloader<T> : DownloaderBase<T>
    {
        /// <summary>
        /// Parses the response stream and deserializes the contents.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected override T ParseResponse(Stream input)
        {
            using (var reader = new StreamReader(input))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}