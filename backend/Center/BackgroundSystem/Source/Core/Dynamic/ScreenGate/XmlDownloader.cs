// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDownloader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlDownloader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.ScreenGate
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper class for downloading XML serialized data from an HTTP server.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the data to be deserialized.
    /// </typeparam>
    internal class XmlDownloader<T> : DownloaderBase<T>
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
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(input);
        }
    }
}